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
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.BedHistory.ADO;
using HIS.Desktop.Utilities.Extensions;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using MOS.ServicePaty;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOS.Filter;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using DevExpress.XtraPrinting.Native;
using static DevExpress.Data.Helpers.ExpressiveSortInfo;

namespace HIS.Desktop.Plugins.BedHistory
{
    public partial class FormBedHistory : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        private System.Globalization.CultureInfo cultureLang;
        private L_HIS_TREATMENT_BED_ROOM _TreatmentBedRoom { get; set; }
        private List<V_HIS_BED_LOG> lstBedLogChecks { get; set; }
        private List<ADO.HisBedHistoryADO> bedLogChecks;
        private List<ADO.HisBedHistoryADO> bedLogCheckProcessing;
        private List<HIS_PATIENT_TYPE> currentPatientTypeWithPatientTypeAlter = null;
        private HisBedServiceReqSDO hisBedServiceReqSDO { get; set; }
        private bool isCheckAll = true;
        private MOS.SDO.WorkPlaceSDO WorkPlaceSDO;
        private List<V_HIS_SERVICE> _services { get; set; }

        private bool IsAdmin = false;
        private bool IsDisable;
        private string loginName;
        private List<HIS_SERVICE_REQ> ListServiceReqForSereServs { get; set; }

        private List<V_HIS_SERVICE> VHisBedServiceTypes { get; set; }//check dv khóa thì ko lấy cấu hình
        private List<V_HIS_SERVICE_ROOM> ListServiceBedByRooms { get; set; }
        private List<HIS_DEPARTMENT> _Departments { get; set; }
        private List<V_HIS_BED_ROOM> ListVBedRoom { get; set; }
        private List<HIS_SERVICE_REQ> _ServiceReqs { get; set; }
        private List<HisBedADO> dataBedADOs { get; set; }

        HisTreatmentWithPatientTypeInfoSDO CurrentTreatment { get; set; }

        List<ADO.HisBedServiceTypeADO> ListBedServiceTypes;

        private string commonString__true = "1";
        HIS_DEPARTMENT currentDepartment = new HIS_DEPARTMENT();

        List<HIS_BED_BSTY> hisBedBstys;

        List<V_HIS_TREATMENT_BED_ROOM> lstHisTreatmentBedRoom;
        const int MaxReq = 500;
        long oldPrimaryTypeId;
        bool IsFromTreatment;
        bool IsDisableDelete;
        HIS_SERE_SERV currentBedServiceReq;
        List<HIS_SERE_SERV> currentBedSereServs { get; set; }
        private List<V_HIS_BED_LOG> listCurrentBedLog { get; set; }

        long BhytExceedDayAllowForInPatient = Convert.ToInt16(HisConfigs.Get<string>(HisConfigKeys.CONFIG_KEY__BHYT__EXCEED_DAY_ALLOW_FOR_IN_PATIENT));
        string IsUsingBedTemp = HisConfigs.Get<string>(HisConfigKeys.CONFIG_KEY__MOS__HIS_SERE_SERV__IS__USING_BED_TEMP);
        string IsShareBed = HisConfigs.Get<string>(HisConfigKeys.CONFIG_KEY__IsShareBedFeeOffAllPatients);
        string WarningOverTotalPatientPrice__IsCheck = HisConfigs.Get<string>(HisConfigKeys.CONFIG_KEY__WarningOverTotalPatientPrice__IsCheck);
        RefeshReference refesh;
        Dictionary<long, List<V_HIS_BED_LOG>> dicBedLog = new Dictionary<long, List<V_HIS_BED_LOG>>();
        Dictionary<long, List<long>> dicTreatmentBedRoom = new Dictionary<long, List<long>>();
        #endregion

        #region Construct
        public FormBedHistory(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
            try
            {
                Resources.ResourceLanguageManager.InitResourceLanguageManager();
                cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                WorkPlaceSDO = WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == module.RoomId);
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationManager.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public FormBedHistory(L_HIS_TREATMENT_BED_ROOM listBedRoom, Inventec.Desktop.Common.Modules.Module module, bool isDisable, RefeshReference refesh)
            : this(module)
        {
            try
            {
                this.refesh = refesh;
                WorkPlaceSDO = WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == module.RoomId);
                this._TreatmentBedRoom = listBedRoom;
                this.Text = module.text;
                loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                IsAdmin = HIS.Desktop.IsAdmin.CheckLoginAdmin.IsAdmin(loginName);
                this.IsDisable = isDisable;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        List<HIS_SERVICE_CONDITION> lstConditionService { get; set; }
        private void FormBedHistory_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                CheckWarningOverTotalPatientPrice();
                EnableControl();
                LoadKeysFromlanguage();
                SetDefaultValueControl();

                _services = BackendDataWorker.Get<V_HIS_SERVICE>();
                FillDataToControl();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region Private method
        /// <summary>
        /// Kiem tra no vien phi
        /// Mức tiền cảnh báo nợ viện phí đối với BN nội trú và tủ trực
        /// </summary>
        private async Task CheckWarningOverTotalPatientPrice()
        {
            try
            {
                if (this.WarningOverTotalPatientPrice__IsCheck != "1" && WarningOverTotalPatientPrice__IsCheck != "3")
                {
                    return;
                }

                CommonParam param = new CommonParam();
                HisTreatmentFeeViewFilter filter = new HisTreatmentFeeViewFilter();
                filter.ID = _TreatmentBedRoom.TREATMENT_ID;
                var treatmentFees = await new BackendAdapter(param)
                    .GetAsync<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_FEE>>(HisRequestUriStore.HIS_TREATMENT_GETFEEVIEW, ApiConsumers.MosConsumer, filter, param);

                //So tien benh nhan can thu
                if (treatmentFees == null || treatmentFees.Count == 0)
                    return;

                decimal totalPrice = 0;
                decimal totalHeinPrice = 0;
                decimal totalPatientPrice = 0;
                decimal totalDeposit = 0;
                decimal totalDebt = 0;
                decimal totalBill = 0;
                decimal totalBillTransferAmount = 0;
                decimal totalRepay = 0;
                decimal total_obtained_price = 0;
                totalPrice = treatmentFees[0].TOTAL_PRICE ?? 0; // tong tien
                totalHeinPrice = treatmentFees[0].TOTAL_HEIN_PRICE ?? 0;
                totalPatientPrice = treatmentFees[0].TOTAL_PATIENT_PRICE ?? 0; // bn tra
                totalDeposit = treatmentFees[0].TOTAL_DEPOSIT_AMOUNT ?? 0;
                totalDebt = treatmentFees[0].TOTAL_DEBT_AMOUNT ?? 0;
                totalBill = treatmentFees[0].TOTAL_BILL_AMOUNT ?? 0;
                totalBillTransferAmount = treatmentFees[0].TOTAL_BILL_TRANSFER_AMOUNT ?? 0;
                totalRepay = treatmentFees[0].TOTAL_REPAY_AMOUNT ?? 0;
                total_obtained_price = (totalDeposit + totalDebt + totalBill - totalBillTransferAmount - totalRepay);//Da thu benh nhan
                decimal transfer = totalPatientPrice - total_obtained_price;//Phai thu benh nhan

                if (transfer > 0)
                {
                    var myResult = DevExpress.XtraEditors.XtraMessageBox.Show(this, String.Format("Bệnh nhân đang thiếu viện phí {0} đồng. Bạn có muốn tiếp tục?", Inventec.Common.Number.Convert.NumberToString(transfer, ConfigApplications.NumberSeperator)), ResourceMessage.ThongBao, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                    if (myResult != DialogResult.OK)
                    {
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #region Load
        private void EnableControl()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisTreatmentFilter filter = new HisTreatmentFilter();
                filter.ID = _TreatmentBedRoom.TREATMENT_ID;
                var lstTreatment = new BackendAdapter(param).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                IsFromTreatment = this.IsDisable;
                if (lstTreatment != null && lstTreatment.Count > 0)
                {
                    var currentTreatment = lstTreatment.First();
                    if (currentTreatment.IS_PAUSE != 1)
                    {
                        this.IsDisable = false;
                        gridControlBedHistory.RefreshDataSource();
                        gridControlBedServiceReq.RefreshDataSource();
                        BtnSaveBedLog.Enabled = true;
                        btnAssigns.Enabled = true;

                    }
                    if (IsFromTreatment)
                    {
                        if (currentTreatment.IS_PAUSE == 1)
                        {
                            this.IsDisable = true;
                            gridControlBedHistory.RefreshDataSource();
                            gridViewBedHistory.OptionsBehavior.ReadOnly = true;
                            gridControlBedServiceReq.RefreshDataSource();
                            gridViewBedServiceReq.OptionsBehavior.ReadOnly = true;
                            repositoryItemCboBedCode.Buttons[1].Visible = false;
                            repositoryItemCboBed.Buttons[1].Visible = false;
                            repositoryItemCboBedServiceType.Buttons[1].Visible = false;
                            repositoryItemCboNamGhep2.Buttons[1].Visible = false;
                            repositoryItemCboPrimaryPatientType.Buttons[1].Visible = false;
                            BtnSaveBedLog.Enabled = false;
                            btnAssigns.Enabled = false;
                        }
                    }
                    if (_TreatmentBedRoom != null && _TreatmentBedRoom.BED_ROOM_ID != WorkPlaceSDO.BedRoomId)
                    {
                        BtnSaveBedLog.Enabled = false;
                        btnAssigns.Enabled = false;
                    }
                }
                CheckEnableChkSplitByResult();

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
                this.btnAssigns.Text = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_BED_HISTORY__BTN_ASSIGNS");
                this.Gv_BedHistory__Gc_BedName.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_BED_HISTORY__GV_BED_HISTORY__GC_BED_NAME");
                this.Gv_BedHistory__Gc_BedServiceTypeName.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_BED_HISTORY__GV_BED_HISTORY__GC_BED_SERVICE_TYPE_NAME");
                this.Gv_BedHistory__Gc_BedTypeCode.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_BED_HISTORY__GV_BED_HISTORY__GC_BED_TYPE_CODE");
                this.Gv_BedHistory__Gc_BedTypeName.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_BED_HISTORY__GV_BED_HISTORY__GC_BED_TYPE_NAME");
                this.Gv_BedHistory__Gc_FinishTime.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_BED_HISTORY__GV_BED_HISTORY__GC_FINISH_TIME");
                this.Gv_BedHistory__Gc_StartTime.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_BED_HISTORY__GV_BED_HISTORY__GC_START_TIME");
                this.Gv_BedServiceReq__Gc_IntructionTime.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_BED_HISTORY__GV_BED_SERVICE_REQ__GC_INTRUCTION_TIME");
                this.Gv_BedServiceReq__Gc_ServiceReqCode.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_BED_HISTORY__GV_BED_SERVICE_REQ__GC_SERVICE_REQ_CODE");
                this.Gv_BedServiceReq__Gc_STT.Caption = this.Gv_BedServiceType__Gc_STT.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_BED_HISTORY__GC_STT");
                this.Gv_BedServiceType__Gc_BedServiceTypeName.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_BED_HISTORY__GV_BED_SERVICE_TYPE__GC_BED_SERVICE_TYPE_NAME");
                this.Gv_BedServiceType__Gc_IsExpend.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_BED_HISTORY__GV_BED_SERVICE_TYPE__GC_IS_EXPEND");
                this.Gv_BedServiceType__Gc_IsOutKtcFee.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_BED_HISTORY__GV_BED_SERVICE_TYPE__GC_IS_OUT_KTC_FEE");
                this.Gv_BedServiceType__Gc_NamGhep.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_BED_HISTORY__GV_BED_SERVICE_TYPE__GC_NAMGHEP");
                this.Gv_BedServiceType__Gc_PatientTypeName.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_BED_HISTORY__GV_BED_SERVICE_TYPE__GC_PATIENT_TYPE_NAME");
                this.Gv_BedServiceType__Gc_Price.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_BED_HISTORY__GV_BED_SERVICE_TYPE__GC_PRICE");
                this.Gv_BedServiceType__Gc_SoNgay.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_BED_HISTORY__GV_BED_SERVICE_TYPE__GC_SO_NGAY");
                this.lciTotalServicePrice.Text = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_BED_HISTORY__LCI_TOTAL_SERVICE_PRICE");
                this.ButtonDelete.Buttons[0].ToolTip = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_BED_HISTORY__TOOL_TIP_BUTTON_DELETE");
                this.ButtonEdit.Buttons[0].ToolTip = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_BED_HISTORY__TOOL_TIP_BUTTON_DEIT");
                this.LciNotCountHours.Text = GetLanguageControl("IVT_LANGUAGE_KEY__FORM_BED_HISTORY__LCI_NOT_COUNT_HOURS");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string GetLanguageControl(string key)
        {
            return Inventec.Common.Resource.Get.Value(key, Resources.ResourceLanguageManager.LanguageFormBedHistory, cultureLang);
        }

        private void SetDefaultValueControl()
        {
            try
            {
                ProcessLoadHistreatment();
                PatientTypeWithPatientTypeAlter();
                ProcessDepartment();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitBedRoomCheck()
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cboBedRoom.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(SelectionGrid__BedRoom);
                cboBedRoom.Properties.Tag = gridCheck;
                cboBedRoom.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cboBedRoom.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboBedRoom.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__BedRoom(object sender, EventArgs e)
        {
            try
            {
                this.ListVBedRoom = new List<V_HIS_BED_ROOM>();
                foreach (V_HIS_BED_ROOM rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        this.ListVBedRoom.Add(rv);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboBedRoom()
        {
            try
            {
                var datas = BackendDataWorker.Get<V_HIS_BED_ROOM>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                if (datas != null)
                {
                    cboBedRoom.Properties.DataSource = datas;
                    cboBedRoom.Properties.DisplayMember = "BED_ROOM_NAME";
                    cboBedRoom.Properties.ValueMember = "ID";
                    DevExpress.XtraGrid.Columns.GridColumn col2 = cboBedRoom.Properties.View.Columns.AddField("BED_ROOM_NAME");
                    col2.VisibleIndex = 1;
                    col2.Width = 200;
                    col2.Caption = "";
                    cboBedRoom.Properties.PopupFormWidth = 200;
                    cboBedRoom.Properties.View.OptionsView.ShowColumnHeaders = false;
                    cboBedRoom.Properties.View.OptionsSelection.MultiSelect = true;
                    GridCheckMarksSelection gridCheckMark = cboBedRoom.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        gridCheckMark.ClearSelection(cboBedRoom.Properties.View);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetValueBedRoom(GridLookUpEdit gridLookUpEdit, List<V_HIS_BED_ROOM> listSelect, List<V_HIS_BED_ROOM> listAll)
        {
            try
            {
                gridLookUpEdit.Focus();
                if (listSelect != null)
                {
                    var selectFilter = listAll.Where(o => listSelect.Exists(p => o.ID == p.ID)).ToList();
                    if (selectFilter != null && selectFilter.Count > 0)
                    {
                        listAll = listAll.OrderByDescending(o => listSelect.Exists(p => o.ID == p.ID)).ToList();
                    }

                    gridLookUpEdit.Properties.DataSource = listAll;
                    GridCheckMarksSelection gridCheckMark = gridLookUpEdit.Properties.Tag as GridCheckMarksSelection;
                    gridCheckMark.Selection.Clear();
                    gridCheckMark.Selection.AddRange(selectFilter);
                }
                gridLookUpEdit.Text = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessLoadHistreatment()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisTreatmentWithPatientTypeInfoFilter filter = new MOS.Filter.HisTreatmentWithPatientTypeInfoFilter();
                filter.TREATMENT_ID = _TreatmentBedRoom.TREATMENT_ID;

                var apiResult = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HisTreatmentWithPatientTypeInfoSDO>>("api/HisTreatment/GetTreatmentWithPatientTypeInfoSdo", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                if (apiResult != null && apiResult.Count > 0)
                {
                    CurrentTreatment = apiResult.FirstOrDefault();
                }

                if (CurrentTreatment != null)
                {
                    if (CurrentTreatment.CLINICAL_IN_TIME != null)//Thêm
                        LblInTime.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(CurrentTreatment.CLINICAL_IN_TIME ?? 0);
                    else //Thêm
                        LblInTime.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(CurrentTreatment.IN_TIME);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToControl()
        {
            try
            {
                KiemTraXemCoPTTT();
                LoadDataBedServiceType();
                RefeshDataToCboBed(0, new HisBedHistoryADO(), true);

                LoadDataComboNamGhep();

                Gv_BedServiceType__Gc_PrimaryPatientType.Visible = false;
                Gv_BedHistory__Gc_PrimaryPatientTypeId.Visible = false;

                LoadDataToCboPatientType(repositoryItemCboPatientType, currentPatientTypeWithPatientTypeAlter);
                LoadDataToCboPatientType(repositoryItemCboPrimaryPatientType, currentPatientTypeWithPatientTypeAlter);
                LoadDataToCboPatientType(repositoryItemCboBedPatientType, currentPatientTypeWithPatientTypeAlter);
                LoadDataToCboPatientType(repositoryItemCboBedPrimaryPatientType, currentPatientTypeWithPatientTypeAlter);
                InitComboServiceCondition(repServiceCondition, lstConditionService);
                InitComboServiceCondition(repServiceCondition, lstConditionService);

                if (!string.IsNullOrWhiteSpace(Base.GlobalStore.IsPrimaryPatientType) && Base.GlobalStore.IsPrimaryPatientType != "0")
                {
                    Gv_BedServiceType__Gc_PrimaryPatientType.Visible = true;
                    Gv_BedHistory__Gc_PrimaryPatientTypeId.Visible = true;
                }
                InitCboRequestUser();
                InitDataCboOtherPaySource();

                LoadDataGridServiceReq();

                GetAllBedLog();

                FillDataToGridBedLog();

                InitCboTreatmentFinish();

                if (IsDisable)
                {
                    BtnSaveBedLog.Enabled = false;
                    btnAssigns.Enabled = false;
                    ChkNotCountHours.Enabled = false;
                }

                DtOutTime.DateTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetAllBedLog()
        {
            try
            {
                listCurrentBedLog = new List<V_HIS_BED_LOG>();
                MOS.Filter.HisBedLogViewFilter filterBedLog = new MOS.Filter.HisBedLogViewFilter();
                filterBedLog.TREATMENT_ID = _TreatmentBedRoom.TREATMENT_ID;
                listCurrentBedLog = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.V_HIS_BED_LOG>>(Base.GlobalStore.HIS_BED_LOG_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, filterBedLog, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataBedServiceType()
        {
            try
            {
                CommonParam param = new CommonParam();
                this.VHisBedServiceTypes = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G && o.IS_ACTIVE == 1).ToList();
                lstConditionService = BackendDataWorker.Get<HIS_SERVICE_CONDITION>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && VHisBedServiceTypes.Select(p => p.ID).Contains(o.SERVICE_ID)).ToList();
                this.hisBedBstys = BackendDataWorker.Get<HIS_BED_BSTY>().Where(o => o.IS_ACTIVE == 1).ToList();

                //Load theo servieRoom lấy ra các dịch vụ giường của room OK
                if (IsDisable)
                    this.ListServiceBedByRooms = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE_ROOM>().Where(o => o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G && o.IS_ACTIVE == 1).ToList();
                else
                {
                    this.ListServiceBedByRooms = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE_ROOM>().Where(o =>
                     //(o.ROOM_ID == this.WorkPlaceSDO.RoomId) &&
                     o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G
                    && o.IS_ACTIVE == 1).ToList();
                }

                if (ListServiceBedByRooms != null
                    && ListServiceBedByRooms.Count > 0
                    && this.VHisBedServiceTypes != null
                    && this.VHisBedServiceTypes.Count > 0)
                {
                    ListServiceBedByRooms = ListServiceBedByRooms.Where(p => this.VHisBedServiceTypes.Select(o => o.ID).ToList().Contains(p.SERVICE_ID)).ToList();
                    ListServiceBedByRooms = ListServiceBedByRooms.OrderBy(p => p.SERVICE_CODE).ToList();
                }

                FillDataCboBedServiceType(this.repositoryItemCboBedServiceType, ListServiceBedByRooms);
                gridControlBedHistory.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataCboBedServiceType(DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit cboBedServiceType, List<V_HIS_SERVICE_ROOM> data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SERVICE_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("SERVICE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_NAME", "SERVICE_ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboBedServiceType, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessDepartment()
        {
            try
            {
                var departmentCodes = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(Base.GlobalStore.EXE_CREATE_BED_LOG_DEPARTMENT_CODES);//cấu hình

                string pattern = ",";
                System.Text.RegularExpressions.Regex myRegex = new System.Text.RegularExpressions.Regex(pattern);
                string[] Codes = myRegex.Split(departmentCodes);

                long departmentId = 0;
                if (this.WorkPlaceSDO != null)
                {
                    departmentId = this.WorkPlaceSDO.DepartmentId;
                }

                _Departments = new List<HIS_DEPARTMENT>();

                InitBedRoomCheck();
                InitComboBedRoom();

                if (IsDisable)
                {
                    this.ListVBedRoom = LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_BED_ROOM>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                    _Departments = LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEPARTMENT>();
                }
                else
                {
                    _Departments = LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEPARTMENT>().Where(p => p.ID == departmentId && Codes.Contains(p.DEPARTMENT_CODE)).ToList();
                    this.ListVBedRoom = LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_BED_ROOM>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList().Where(o => o.DEPARTMENT_ID == departmentId && o.ROOM_ID == this.WorkPlaceSDO.RoomId).ToList();
                }

                var allData = LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_BED_ROOM>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList().OrderByDescending(o => o.DEPARTMENT_ID == departmentId).ToList();

                SetValueBedRoom(cboBedRoom, this.ListVBedRoom, allData);

                currentDepartment = LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == departmentId);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Kiem tra cau hinh PTTT
        /// </summary>
        private void CheckCardConfigCCC(HisBedHistoryADO bedHistory)
        {
            try
            {
                if (_Departments != null && _Departments.Count > 0)//Kiểm tra cấu hình khoa
                {
                    SSVsFromTime(bedHistory);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void KiemTraXemCoPTTT()
        {
            try
            {
                this._ServiceReqs = new List<HIS_SERVICE_REQ>();
                CommonParam param = new CommonParam();
                MOS.Filter.HisServiceReqFilter serviceReqFilter = new MOS.Filter.HisServiceReqFilter();
                serviceReqFilter.TREATMENT_ID = this._TreatmentBedRoom.TREATMENT_ID;
                serviceReqFilter.SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PT;
                serviceReqFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                this._ServiceReqs = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumer.ApiConsumers.MosConsumer, serviceReqFilter, param).Where(p => p.FINISH_TIME != null).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SSVsFromTime(HisBedHistoryADO bedHistory)
        {
            try
            {
                CommonParam param = new CommonParam();
                if (this._ServiceReqs != null && this._ServiceReqs.Count > 0)
                {
                    MOS.Filter.HisSereServFilter sereServFilter = new MOS.Filter.HisSereServFilter();
                    sereServFilter.SERVICE_REQ_IDs = _ServiceReqs.Select(p => p.ID).ToList();
                    var _SereServPttts = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>(Base.GlobalStore.HIS_SERE_SERV_GET, ApiConsumer.ApiConsumers.MosConsumer, sereServFilter, param).ToList();

                    List<HIS_SERE_SERV> DsSereServPttts = new List<HIS_SERE_SERV>();

                    foreach (var item in _ServiceReqs)
                    {
                        DateTime finishTime = (DateTime)Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(item.FINISH_TIME ?? 0);
                        // dtFromTime.DateTime = finishTime;
                        DateTime fromTime = bedHistory != null ? bedHistory.startTime : new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
                        if (fromTime < finishTime)
                        {
                            MessageBox.Show(ResourceMessage.ERROR_FINISH_TIME_PTTT + finishTime.ToString(),
                                ResourceMessage.ThongBao);
                            return;
                        }
                        TimeSpan diff = fromTime - finishTime;
                        if (diff.Days <= 10)
                        {
                            DsSereServPttts.Add(_SereServPttts.FirstOrDefault(p => p.SERVICE_REQ_ID == item.ID));
                        }
                    }

                    if (DsSereServPttts != null && DsSereServPttts.Count > 0)
                    {
                        // MessageBox.Show("Check lai");
                        var SereServ = DsSereServPttts.FirstOrDefault();
                        MOS.Filter.HisSereServPtttViewFilter sSPtttFilter = new MOS.Filter.HisSereServPtttViewFilter();
                        sSPtttFilter.SERE_SERV_ID = SereServ.ID;
                        V_HIS_SERE_SERV_PTTT hisSereServPttt = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV_PTTT>>(Base.GlobalStore.HIS_SERE_SERV_PTTT_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, sSPtttFilter, param).FirstOrDefault();
                        if (hisSereServPttt != null)
                        {
                            LoadPTTTGroup(hisSereServPttt, bedHistory);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadPTTTGroup(V_HIS_SERE_SERV_PTTT sereServPttt, HisBedHistoryADO bedHistory)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisPtttGroupFilter filter = new MOS.Filter.HisPtttGroupFilter();
                filter.ID = sereServPttt.PTTT_GROUP_ID;
                List<HIS_PTTT_GROUP> _PtttGroups = new BackendAdapter(param).Get<List<HIS_PTTT_GROUP>>(Base.GlobalStore.HIS_PTTT_GROUP_GET, ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                if (_PtttGroups != null && _PtttGroups.Count > 0)
                {
                    MOS.Filter.HisPtttGroupBestFilter bestfilter = new MOS.Filter.HisPtttGroupBestFilter();
                    bestfilter.PTTT_GROUP_IDs = _PtttGroups.Select(s => s.ID).Distinct().ToList();
                    List<HIS_PTTT_GROUP_BEST> _ptttBest = new BackendAdapter(param).Get<List<HIS_PTTT_GROUP_BEST>>("/api/HisPtttGroupBest/Get", ApiConsumer.ApiConsumers.MosConsumer, bestfilter, param);
                    if (_ptttBest != null && _ptttBest.Count > 0)
                    {
                        var serviceId = this.VHisBedServiceTypes.SingleOrDefault(p => p.ID == _ptttBest[0].BED_SERVICE_TYPE_ID);
                        if (serviceId != null)
                        {
                            RefeshDataToCboBed(serviceId.ID, bedHistory, false);
                        }
                        else
                        {
                            RefeshDataToCboBed(0, bedHistory, false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Load lai data cbo Bed
        /// </summary>
        /// <param name="serviceId"></param>
        private void RefeshDataToCboBed(long serviceId, HisBedHistoryADO bedHistory, bool firstLoad)
        {
            try
            {
                CommonParam param = new CommonParam();

                List<V_HIS_BED> hisBeds = LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_BED>();
                if (firstLoad)
                {
                    if (this.ListVBedRoom != null && this.ListVBedRoom.Count > 0)
                    {
                        var beds = hisBeds.Where(o => this.ListVBedRoom.Select(s => s.ID).Contains(o.BED_ROOM_ID)).ToList();
                        LoadDataCboBed(this.repositoryItemCboBed, beds, bedHistory);
                    }
                    else
                        LoadDataCboBed(this.repositoryItemCboBed, hisBeds, bedHistory);
                }
                else
                {
                    List<long> lstBedIds = new List<long>();
                    if (serviceId != 0)
                    {
                        if (hisBedBstys != null && hisBedBstys.Count > 0)
                            lstBedIds = hisBedBstys.Where(p => p.BED_SERVICE_TYPE_ID == serviceId).Select(p => p.BED_ID).ToList();
                    }

                    var lstBeds = hisBeds.Where(p => lstBedIds.Contains(p.ID)).ToList();
                    if (lstBeds != null && lstBeds.Count > 0)
                    {
                        LoadDataCboBed(this.repositoryItemCboBed, lstBeds, bedHistory);
                        bedHistory.BED_ID = lstBeds[0].ID;
                        bedHistory.BED_CODE = lstBeds[0].BED_CODE;
                        bedHistory.BED_CODE_ID = lstBeds[0].ID;
                        bedHistory.BED_TYPE_CODE = lstBeds[0].BED_TYPE_CODE;
                        bedHistory.BED_TYPE_ID = lstBeds[0].BED_TYPE_ID;
                        bedHistory.BED_TYPE_NAME = lstBeds[0].BED_TYPE_NAME;
                        bedHistory.IsBedStretcher = lstBeds[0].IS_BED_STRETCHER == 1;
                    }
                    else
                    {
                        if (this.ListVBedRoom != null && this.ListVBedRoom.Count > 0)
                        {
                            var beds = hisBeds.Where(o => this.ListVBedRoom.Select(s => s.ID).Contains(o.BED_ROOM_ID)).ToList();
                            LoadDataCboBed(this.repositoryItemCboBed, beds, bedHistory);
                        }
                        else
                            LoadDataCboBed(this.repositoryItemCboBed, hisBeds, bedHistory);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataCboBed(DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit cboBedName, List<V_HIS_BED> datas, HisBedHistoryADO bedHistory)
        {
            try
            {
                dataBedADOs = new List<HisBedADO>();
                dataBedADOs = ProcessDataBedAdo(datas, bedHistory);

                InitCboBed(cboBedName);
                InitCboBedCode(repositoryItemCboBedCode);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitCboBed(DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit cboBedName)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("BED_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("BED_NAME", "", 250, 2));
                columnInfos.Add(new ColumnInfo("AMOUNT_STR", "", 50, 3));
                ControlEditorADO controlEditorADO = new ControlEditorADO("BED_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboBedName, dataBedADOs, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitCboBedCode(DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit cboBedName)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("BED_CODE", "", 50, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("BED_CODE", "BED_CODE_ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboBedName, dataBedADOs, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<HisBedADO> ProcessDataBedAdo(List<V_HIS_BED> datas, HisBedHistoryADO bedHistory)
        {
            List<HisBedADO> result = null;
            try
            {
                if (datas != null && datas.Count > 0)
                {
                    result = new List<HisBedADO>();
                    result.AddRange((from r in datas select new HisBedADO(r)).ToList());

                    long? startTimeFilter = null;
                    long? finishTimeFilter = null;
                    if (bedHistory.startTime != null && bedHistory.startTime != DateTime.MinValue)
                    {
                        startTimeFilter = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(bedHistory.startTime) ?? 0;
                    }
                    if (bedHistory.finishTime != null && bedHistory.finishTime != DateTime.MinValue)
                    {
                        finishTimeFilter = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(bedHistory.finishTime) ?? 0;
                    }

                    List<long> bedIds = datas.Select(p => p.ID).Distinct().ToList();

                    //api moi

                    MOS.SDO.TakeBedsInUseSDO sdo = new TakeBedsInUseSDO();
                    sdo.BedIds = bedIds;
                    sdo.StartTime = startTimeFilter ?? 0;
                    sdo.FinishTime = finishTimeFilter;
                    CommonParam param = new CommonParam();
                    Inventec.Common.Logging.LogSystem.Debug("Du lieu goi den api: HisBedLog/TakeBedsInUse. TakeBedsInUseSDO: " + Inventec.Common.Logging.LogUtil.TraceData("TakeBedsInUseSDO", sdo));
                    List<HIS_BED_LOG> dataBedLogs = new BackendAdapter(param).Post<List<HIS_BED_LOG>>("/api/HisBedLog/TakeBedsInUse", ApiConsumers.MosConsumer, sdo, param);

                    if (dataBedLogs != null && dataBedLogs.Count > 0)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("Du lieu goi den api tra ve: HisBedLog/TakeBedsInUse. dataBedLogs: " + Inventec.Common.Logging.LogUtil.TraceData("dataBedLogs", dataBedLogs.Select(s => s.BED_ID).ToList()));

                        foreach (var itemADO in result)
                        {

                            //var dataByBedLogs_onStartTime = dataBedLogs.Where(p => p.BED_ID == itemADO.ID && p.START_TIME <= startTimeFilter && (!p.FINISH_TIME.HasValue || (p.FINISH_TIME.HasValue && p.FINISH_TIME.Value >= startTimeFilter))).ToList() ?? new List<HIS_BED_LOG>();
                            //List<HIS_BED_LOG> dataByBedLogs_onFinishTime = new List<HIS_BED_LOG>();
                            //if (finishTimeFilter != null)
                            //    dataByBedLogs_onFinishTime = dataBedLogs.Where(p => p.BED_ID == itemADO.ID 
                            //    && (p.START_TIME <= finishTimeFilter && (!p.FINISH_TIME.HasValue || (p.FINISH_TIME.HasValue && p.FINISH_TIME.Value >= finishTimeFilter)))
                            //    ||(p.S)).ToList() ?? new List<HIS_BED_LOG>();
                            //else
                            //    dataByBedLogs_onFinishTime = dataBedLogs.Where(p => p.BED_ID == itemADO.ID && (!p.FINISH_TIME.HasValue || (p.FINISH_TIME.HasValue && p.FINISH_TIME.Value >= startTimeFilter))).ToList() ?? new List<HIS_BED_LOG>();

                            //lọc thời gian mới
                            // Lấy danh sách log theo startTimeFilter
                            var dataByBedLogs_onStartTime = dataBedLogs
                                .Where(p => p.BED_ID == itemADO.ID
                                        && (p.START_TIME <= startTimeFilter
                                            && (!p.FINISH_TIME.HasValue || p.FINISH_TIME.Value >= startTimeFilter)
                                            || (p.START_TIME >= startTimeFilter && (!p.FINISH_TIME.HasValue || p.START_TIME <= finishTimeFilter))))
                                .ToList() ?? new List<HIS_BED_LOG>();

                            List<HIS_BED_LOG> dataByBedLogs_onFinishTime = new List<HIS_BED_LOG>();

                            if (finishTimeFilter != null)
                            {
                                // Nếu finishTimeFilter có giá trị, tìm các log có thời gian phù hợp trong khoảng từ startTime đến finishTime
                                dataByBedLogs_onFinishTime = dataBedLogs
                                    .Where(p => p.BED_ID == itemADO.ID
                                            && (p.START_TIME <= finishTimeFilter
                                                && (!p.FINISH_TIME.HasValue || p.FINISH_TIME.Value >= finishTimeFilter)
                                                || (p.START_TIME >= startTimeFilter && (!p.FINISH_TIME.HasValue || p.START_TIME <= finishTimeFilter))))
                                    .ToList() ?? new List<HIS_BED_LOG>();
                            }
                            else
                            {
                                // Nếu finishTimeFilter không có giá trị, chỉ xét các log có finishTime >= startTimeFilter
                                dataByBedLogs_onFinishTime = dataBedLogs
                                    .Where(p => p.BED_ID == itemADO.ID
                                            && (!p.FINISH_TIME.HasValue || p.FINISH_TIME.Value >= startTimeFilter))
                                    .ToList() ?? new List<HIS_BED_LOG>();
                            }

                            List<HIS_BED_LOG> dataByBedLogs = new List<HIS_BED_LOG>();
                            dataByBedLogs.AddRange(dataByBedLogs_onStartTime);
                            dataByBedLogs.AddRange(dataByBedLogs_onFinishTime);
                            dataByBedLogs = dataByBedLogs.Distinct().ToList();
                            if (dataByBedLogs_onStartTime != null && dataByBedLogs_onStartTime.Count > 0)
                            {
                                itemADO.BedLogStartIds = dataByBedLogs_onStartTime.Select(o => o.ID).ToList();
                            }
                            if (dataByBedLogs_onFinishTime != null && dataByBedLogs_onFinishTime.Count > 0)
                            {
                                itemADO.BedLogFinishIds = dataByBedLogs_onFinishTime.Select(o => o.ID).ToList();
                            }

                            if (dataByBedLogs != null && dataByBedLogs.Count > 0)
                            {
                                if (itemADO.MAX_CAPACITY.HasValue)
                                {
                                    if (dataByBedLogs.Count >= itemADO.MAX_CAPACITY)
                                        itemADO.IsKey = 2;
                                    else
                                        itemADO.IsKey = 1;
                                }
                                else
                                    itemADO.IsKey = 1;
                                itemADO.BedLogAllIds = dataByBedLogs.Select(o => o.ID).ToList();
                                itemADO.AMOUNT = dataByBedLogs.Count;

                                itemADO.AMOUNT_STR = dataByBedLogs.Count + "/" + itemADO.MAX_CAPACITY;
                                itemADO.TREATMENT_BED_ROOM_IDs = dataByBedLogs.Select(o => o.TREATMENT_BED_ROOM_ID).ToList();
                                dicTreatmentBedRoom[itemADO.ID] = itemADO.TREATMENT_BED_ROOM_IDs;
                            }
                        }

                    }
                    //api cu
                    //int skip = 0;
                    //while (bedIds.Count - skip > 0)
                    //{
                    //    var listIds = bedIds.Skip(skip).Take(MaxReq).ToList();
                    //    skip += MaxReq;

                    //    MOS.Filter.HisBedLogFilter filter = new MOS.Filter.HisBedLogFilter();
                    //    filter.BED_IDs = listIds;
                    //    if (startTimeFilter != null)
                    //    {
                    //        //filter.START_TIME_TO = startTimeFilter;
                    //        filter.FINISH_TIME_FROM__OR__NULL = startTimeFilter;
                    //    }
                    //    if (finishTimeFilter != null)
                    //    {
                    //        filter.START_TIME_TO = finishTimeFilter;
                    //    }
                    //    CommonParam param = new CommonParam();
                    //    var dataBedLogs = new BackendAdapter(param).Get<List<HIS_BED_LOG>>("api/HisBedLog/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);

                    //}
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void LoadCurrentPatientTypeAlter(long treatmentId, long intructionTime, ref MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER hisPatientTypeAlter)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisPatientTypeAlterViewAppliedFilter filter = new MOS.Filter.HisPatientTypeAlterViewAppliedFilter();
                filter.TreatmentId = treatmentId;
                if (intructionTime > 0)
                    filter.InstructionTime = intructionTime;
                else
                    filter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                hisPatientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(param).Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER>(ApiConsumer.HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumer.ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PatientTypeWithPatientTypeAlter()
        {
            try
            {
                if (
                    Base.GlobalStore.HisVPatientTypeAllows != null
                    && Base.GlobalStore.HisVPatientTypeAllows.Count > 0
                    && Base.GlobalStore.HisPatientTypes != null
                    && Base.GlobalStore.HisPatientTypes.Count > 0
                    && this.CurrentTreatment != null
                    )
                {
                    var patientTypeAllow = Base.GlobalStore.HisVPatientTypeAllows.Where(o => o.PATIENT_TYPE_ID == this.CurrentTreatment.TDL_PATIENT_TYPE_ID).Select(m => m.PATIENT_TYPE_ALLOW_ID).Distinct().ToList();
                    if (patientTypeAllow != null && patientTypeAllow.Count > 0)
                    {
                        this.currentPatientTypeWithPatientTypeAlter = Base.GlobalStore.HisPatientTypes.Where(o => patientTypeAllow.Contains(o.ID)).ToList();

                        //Inventec.Common.Logging.LogSystem.Debug("currentPatientTypeWithPatientTypeAlter: " + string.Join(",", currentPatientTypeWithPatientTypeAlter.Select(s => s.PATIENT_TYPE_NAME)));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataComboNamGhep()
        {
            try
            {
                //Gán số lượng nằm ghép 1 giường
                List<ADO.ShareCountADO> lstNamGhep = new List<ADO.ShareCountADO>();
                for (int i = 2; i <= 5; i++)
                {
                    ADO.ShareCountADO item = new ADO.ShareCountADO();
                    item.ID = i;
                    item.ShareCount = i;
                    lstNamGhep.Add(item);
                }

                repositoryItemCboNamGhep.DataSource = lstNamGhep;
                repositoryItemCboNamGhep.DisplayMember = "ShareCount";
                repositoryItemCboNamGhep.ValueMember = "ID";

                repositoryItemCboNamGhep.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                repositoryItemCboNamGhep.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                repositoryItemCboNamGhep.ImmediatePopup = true;
                repositoryItemCboNamGhep.View.Columns.Clear();

                DevExpress.XtraGrid.Columns.GridColumn col = repositoryItemCboNamGhep.View.Columns.AddField("ShareCount");
                col.Caption = "Số lượng nằm ghép";
                col.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                col.Visible = true;
                col.VisibleIndex = 1;
                col.Width = 50;


                repositoryItemCboNamGhep2.DataSource = lstNamGhep;
                repositoryItemCboNamGhep2.DisplayMember = "ShareCount";
                repositoryItemCboNamGhep2.ValueMember = "ID";

                repositoryItemCboNamGhep2.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                repositoryItemCboNamGhep2.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                repositoryItemCboNamGhep2.ImmediatePopup = true;
                repositoryItemCboNamGhep2.View.Columns.Clear();

                DevExpress.XtraGrid.Columns.GridColumn col2 = repositoryItemCboNamGhep2.View.Columns.AddField("ShareCount");
                col2.Caption = "Số lượng nằm ghép";
                col2.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                col2.Visible = true;
                col2.VisibleIndex = 1;
                col2.Width = 50;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToCboPatientType(List<HIS_PATIENT_TYPE> data)
        {
            try
            {
                repositoryItemCboPatientType.DataSource = data;
                repositoryItemCboPatientType.DisplayMember = "PATIENT_TYPE_NAME";
                repositoryItemCboPatientType.ValueMember = "ID";

                repositoryItemCboPatientType.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                repositoryItemCboPatientType.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                repositoryItemCboPatientType.ImmediatePopup = true;
                repositoryItemCboPatientType.View.Columns.Clear();

                DevExpress.XtraGrid.Columns.GridColumn aColumnCode = repositoryItemCboPatientType.View.Columns.AddField("PATIENT_TYPE_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 50;

                DevExpress.XtraGrid.Columns.GridColumn aColumnName = repositoryItemCboPatientType.View.Columns.AddField("PATIENT_TYPE_NAME");
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

        private void FillDataToGridBedLog()
        {
            try
            {
                WaitingManager.Show();
                gridControlBedHistory.DataSource = null;
                List<V_HIS_BED_LOG> vHisBedLogs = new List<V_HIS_BED_LOG>();
                if (!IsDisable && !chkBHAll.Checked && listCurrentBedLog != null && listCurrentBedLog.Count() > 0)
                {
                    GridCheckMarksSelection gridCheckMark = cboBedRoom.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark == null) return;

                    List<long> lstBedRoomCheck = new List<long>();
                    foreach (V_HIS_BED_ROOM rv in gridCheckMark.Selection)
                        lstBedRoomCheck.Add(rv.ID);
                    if (lstBedRoomCheck.Count() > 0)
                    {
                        vHisBedLogs = listCurrentBedLog.Where(o => lstBedRoomCheck.Contains(o.BED_ROOM_ID)).ToList();
                    }
                    else
                    {
                        vHisBedLogs = listCurrentBedLog.Where(o => _TreatmentBedRoom.BED_ROOM_ID == o.BED_ROOM_ID).ToList();
                    }
                }
                else
                {
                    vHisBedLogs = listCurrentBedLog;
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _TreatmentBedRoom), _TreatmentBedRoom));
                bedLogChecks = new List<ADO.HisBedHistoryADO>();
                if (vHisBedLogs != null && vHisBedLogs.Count() > 0)
                {
                    bedLogChecks.AddRange((from r in vHisBedLogs select new ADO.HisBedHistoryADO(r, LocalStorage.LocalData.GlobalVariables.ActionEdit, true, ListServiceReqForSereServs, dataBedADOs)).ToList());
                    bedLogChecks.ForEach(o =>
                    {
                        LoadAppliedPatientType(o.PATIENT_TYPE_ID ?? 0, o.BED_SERVICE_TYPE_ID ?? 0, ref o);
                    });
                    bedLogChecks = bedLogChecks.OrderByDescending(p => p.START_TIME).ToList();
                    bedLogChecks.First().Action = LocalStorage.LocalData.GlobalVariables.ActionAdd;
                }
                else
                {
                    ADO.HisBedHistoryADO ado = new ADO.HisBedHistoryADO();
                    ado.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                    ado.CREATOR = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    bedLogChecks.Add(ado);
                }

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("row___________________", bedLogChecks));
                bedLogChecks.ForEach(o => CheckErrorDataBedLog(o));
                gridControlBedHistory.DataSource = bedLogChecks;

                if (gridControlBedServiceType.DataSource != null)
                {
                    btnAssigns.Enabled = true;
                }
                else
                {
                    btnAssigns.Enabled = false;
                }
                #region Code Old
                //foreach (var item in listBedLog)
                //{
                //    ADO.HisBedHistoryADO ahisBedLog = new ADO.HisBedHistoryADO();
                //    AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_BED_LOG, ADO.HisBedHistoryADO>();
                //    ahisBedLog = AutoMapper.Mapper.Map<MOS.EFMODEL.DataModels.V_HIS_BED_LOG, ADO.HisBedHistoryADO>(item);
                //    ahisBedLog.startTime = (DateTime)Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(item.START_TIME);
                //    if (item.FINISH_TIME != null)
                //    {
                //        ahisBedLog.finishTime = (DateTime)Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(item.FINISH_TIME ?? 0);
                //    }
                //    else
                //    {
                //        ahisBedLog.finishTime = null;
                //    }
                //    bedLogChecks.Add(ahisBedLog);
                //}



                //if (bedLogChecks != null && bedLogChecks.Count > 0)
                //{
                //    var check = bedLogChecks.Select(o => o.IsChecked).ToList();
                //    if (check != null && check.Count > 0 && check.Contains(true))
                //    {
                //        btnCalculated.Enabled = true;
                //        btnAssigns.Enabled = true;
                //    }
                //    else
                //    {
                //        btnCalculated.Enabled = false;
                //        btnAssigns.Enabled = false;
                //    }
                //}
                //else
                //{
                //    btnCalculated.Enabled = false;
                //    btnAssigns.Enabled = false;
                //}
                #endregion
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataGridServiceReq()
        {
            try
            {
                currentBedSereServs = new List<HIS_SERE_SERV>();
                CommonParam param = new CommonParam();
                MOS.Filter.HisServiceReqFilter filter = new MOS.Filter.HisServiceReqFilter();
                filter.TREATMENT_ID = _TreatmentBedRoom.TREATMENT_ID;
                filter.SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__G;
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;

                ListServiceReqForSereServs = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>(Base.GlobalStore.HIS_SERVICE_REQ_GET, ApiConsumer.ApiConsumers.MosConsumer, filter, param);

                if (ListServiceReqForSereServs != null && ListServiceReqForSereServs.Count > 0)
                {
                    ListServiceReqForSereServs = ListServiceReqForSereServs.OrderBy(p => p.INTRUCTION_TIME).ToList();

                    MOS.Filter.HisSereServFilter ssFilter = new MOS.Filter.HisSereServFilter();
                    ssFilter.SERVICE_REQ_IDs = ListServiceReqForSereServs.Select(s => s.ID).ToList();
                    var data = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumer.ApiConsumers.MosConsumer, ssFilter, param);

                    if (data != null && data.Count > 0)
                    {
                        foreach (var item in data)
                        {
                            HIS_SERE_SERV ss = new HIS_SERE_SERV();
                            Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERE_SERV>(ss, item);
                            currentBedSereServs.Add(ss);
                        }
                        data = data.OrderBy(o => o.TDL_INTRUCTION_TIME).ToList();
                        gridControlBedServiceReq.DataSource = data;
                    }


                }
                else
                {
                    gridControlBedServiceReq.BeginUpdate();
                    gridControlBedServiceReq.DataSource = null;
                    gridControlBedServiceReq.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Event Bed History
        private void gridViewBedHistory_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                var ado = (ADO.HisBedHistoryADO)this.gridViewBedHistory.GetFocusedRow();

                if (ado != null)
                {
                    bool isChange = ado.ID > 0;
                    bool isServiceReqAssigned = ado.IS_SERVICE_REQ_ASSIGNED == 1;
                    CheckErrorDataBedLog(ado);

                    if (e.Column.FieldName == "startTime")
                    {
                        ado.IsSave = false;
                        ado.IsChecked = false;
                        RefeshDataToCboBed(0, ado, false);
                        CheckCardConfigCCC(ado);
                        if (isChange && !ado.Error)
                        {
                            if (isServiceReqAssigned)
                            {
                                if (MessageBox.Show("Đã có thông tin chỉ định dịch vụ giường tương ứng. Nếu thay đổi, phần mềm sẽ tự động cập nhật lại \"Số lượng\"/\"Số lượng tạm tính\" của chỉ định dịch vụ giường. Bạn có chắc muốn thực hiện không?", "Thông báo", MessageBoxButtons.YesNo) != DialogResult.Yes)
                                {
                                    return;
                                }
                            }
                            ProcessSaveBedLog(ado);
                        }

                        if (dataBedADOs != null && dataBedADOs.Count > 0)
                        {
                            var bed = dataBedADOs.FirstOrDefault(o => o.ID == ado.BED_CODE_ID);
                            if ((bed != null && bed.AMOUNT > 0) || bed == null)
                            {
                                ado.BED_ID = 0;
                                ado.BED_CODE = null;
                                ado.BED_CODE_ID = 0;
                                ado.BED_TYPE_CODE = null;
                                ado.BED_TYPE_ID = 0;
                                ado.BED_TYPE_NAME = null;
                                ado.BED_SERVICE_TYPE_CODE = null;
                                ado.BED_SERVICE_TYPE_ID = null;
                                ado.SHARE_COUNT = null;
                                ado.ErrorMessagePrimaryPatientTypeId = "";
                                ado.ErrorTypePrimaryPatientTypeId = ErrorType.None;
                                ado.PATIENT_TYPE_ID = null;
                                ado.PRIMARY_PATIENT_TYPE_ID = null;
                                ado.SERVICE_CONDITION_ID = null;
                            }
                        }
                        if (!BreakServiceCondition)
                        {
                            var data = gridControlBedServiceType.DataSource as List<HisBedServiceTypeADO>;
                            if (data != null && data.Count > 0)
                                CountTimeBed();
                        }

                    }
                    else if (e.Column.FieldName == "finishTime")
                    {
                        ado.IsSave = false;
                        ado.IsChecked = false;
                        if (isChange && !ado.Error)
                        {
                            if (isServiceReqAssigned && IsUsingBedTemp == "1")
                            {
                                if (MessageBox.Show("Đã có thông tin chỉ định dịch vụ giường tương ứng. Nếu thay đổi, phần mềm sẽ tự động cập nhật lại \"Số lượng\"/\"Số lượng tạm tính\" của chỉ định dịch vụ giường. Bạn có chắc muốn thực hiện không?", "Thông báo", MessageBoxButtons.YesNo) != DialogResult.Yes)
                                {
                                    return;
                                }
                            }
                            ProcessSaveBedLog(ado);
                        }
                        if (!BreakServiceCondition)
                        {
                            RefeshDataToCboBed(0, ado, false);
                            var data = gridControlBedServiceType.DataSource as List<HisBedServiceTypeADO>;
                            if (data != null && data.Count > 0)
                                CountTimeBed();
                        }

                    }
                    else if (e.Column.FieldName == "IsChecked")
                    {
                        listCurrentBedLog.Where(s => s.ID == ado.ID).ForEach(o =>
                        {
                            o.START_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(ado.startTime) ?? 0;
                            o.FINISH_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(ado.finishTime);
                            o.BED_ID = ado.BED_ID;
                            o.BED_SERVICE_TYPE_ID = ado.BED_SERVICE_TYPE_ID;

                        });
                        CheckErrorDataBedLog(ado);
                        //if ((ado.ErrorTypeStartTime == ErrorType.Warning && ado.ErrorMessageStartTime == ResourceMessage.ERROR_OVERLAP_START_TIME) || (ado.ErrorTypeFinishTime == ErrorType.Warning && ado.ErrorMessageFinishTime == ResourceMessage.ERROR_OVERLAP_START_TIME)) return;

                        if (!ado.IsSave && !ado.Error)
                        {
                            ProcessSaveBedLog(ado, ado.IsChecked);
                        }
                        if (isChange && ado.IsChecked)
                        {
                            RefeshDataToCboBed(0, ado, false);
                        }
                        if (ado.IsChecked && IsShareBed == "1")
                        {
                            HisBedLogViewFilter filter = new HisBedLogViewFilter();
                            filter.BED_ID = ado.BED_ID;
                            if (dicTreatmentBedRoom.ContainsKey(ado.BED_ID))
                                filter.TREATMENT_BED_ROOM_IDs = dicTreatmentBedRoom[ado.BED_ID];
                            var bedLogs = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_BED_LOG>>("api/HisBedLog/GetView", ApiConsumer.ApiConsumers.MosConsumer, filter, null);
                            dicBedLog[ado.BED_ID] = bedLogs;
                        }
                        //if (ado.Error) return;
                        var exitsBed = bedLogChecks.FirstOrDefault(s => s.BED_ID == ado.BED_ID && s.START_TIME != ado.START_TIME);

                        if (exitsBed != null)
                        {
                            var adoStart = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(ado.startTime);
                            var adoFinish = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(ado.finishTime);
                            //if((adoStart >= exitsBed.START_TIME && adoStart <= exitsBed.FINISH_TIME) 
                            //    || (adoFinish >= exitsBed.START_TIME && adoFinish <= exitsBed.FINISH_TIME)
                            //    )
                            //{

                            //}
                            if (adoStart <= exitsBed.START_TIME)
                            {
                                // start time nhỏ hơn hoặc bằng thời gian bắt đầu của giường cũ
                                if (adoFinish >= exitsBed.START_TIME || adoFinish >= exitsBed.FINISH_TIME)
                                {
                                    // Bao trọn hoặc chồng lên thời gian giường cũ
                                    if (adoFinish >= exitsBed.FINISH_TIME)
                                    {
                                        // Nếu bao trọn cả khoảng thời gian của giường cũ
                                        if (ado.IsChecked) return;
                                    }
                                    else
                                    {
                                        // Chỉ chồng lên thời gian bắt đầu
                                        if (ado.IsChecked) return;
                                    }
                                }
                            }
                            else
                            {
                                // start time lớn hơn thời gian bắt đầu của giường cũ
                                if (adoStart <= exitsBed.FINISH_TIME)
                                {
                                    // Chồng lên thời gian kết thúc của giường cũ
                                    if (ado.IsChecked) return;
                                }
                            }


                        }
                        if (!BreakServiceCondition)
                            CountTimeBed();

                        var data = gridControlBedServiceType.DataSource;
                        //gridControlBedHistory.RefreshDataSource();

                    }
                    else if (e.Column.FieldName == "BED_SERVICE_TYPE_ID" || e.Column.FieldName == "AmoutNamGhep")
                    {
                        if (ado.BED_SERVICE_TYPE_ID.HasValue && ado.BED_SERVICE_TYPE_ID.Value > 0)
                        {
                            var bedType = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o => o.ID == ado.BED_SERVICE_TYPE_ID.Value);
                            ado.BED_SERVICE_TYPE_CODE = bedType != null ? bedType.SERVICE_CODE : null;
                            ado.BILL_PATIENT_TYPE_ID = bedType != null ? bedType.BILL_PATIENT_TYPE_ID : null;
                            ado.IS_NOT_CHANGE_BILL_PATY = bedType != null ? bedType.IS_NOT_CHANGE_BILL_PATY == 1 : false;
                            ado.IS_NOT_CHANGE_BILL_PATY_DEFAULT = ado.IS_NOT_CHANGE_BILL_PATY;
                            ChoosePatientTypeDefaultlService(CurrentTreatment.TDL_PATIENT_TYPE_ID ?? 0, ado.BED_SERVICE_TYPE_ID.Value, ado);
                        }
                        else
                        {
                            ado.BED_SERVICE_TYPE_CODE = null;
                            ado.BILL_PATIENT_TYPE_ID = null;
                            ado.IS_NOT_CHANGE_BILL_PATY = false;
                        }
                        CheckErrorDataBedLog(ado);

                        ado.IsChecked = false;
                        ado.IsSave = false;
                    }
                    else if (e.Column.FieldName == "BED_CODE" || e.Column.FieldName == "PRIMARY_PATIENT_TYPE_ID" || e.Column.FieldName == "PATIENT_TYPE_ID")
                    {

                        //CheckErrorDataBedLog(ado);
                        ado.IsSave = false;
                        ado.IsChecked = false;

                    }

                    if (e.Column.FieldName == "BED_CODE_ID" || e.Column.FieldName == "BED_ID")
                    {
                        if (isChange && !ado.Error)
                        {
                            ProcessSaveBedLog(ado);
                        }
                    }
                    else if (e.Column.FieldName == "BED_SERVICE_TYPE_CODE" || e.Column.FieldName == "BED_SERVICE_TYPE_ID" || e.Column.FieldName == "PATIENT_TYPE_ID")
                    {
                        if (isChange && !ado.Error)
                        {
                            if (isServiceReqAssigned)
                            {
                                if (MessageBox.Show("Đã có thông tin chỉ định dịch vụ giường tương ứng.Nếu thay đổi, phần mềm sẽ tự động cập nhật lại \"dịch vụ giường\" và \"đối tượng thanh toán\" của chỉ định dịch vụ giường. Bạn có chắc muốn thực hiện không?", "Thông báo", MessageBoxButtons.YesNo) != DialogResult.Yes)
                                {
                                    return;
                                }
                            }
                            ProcessSaveBedLog(ado);
                        }
                    }
                    else if (e.Column.FieldName == "PRIMARY_PATIENT_TYPE_ID" || e.Column.FieldName == "SHARE_COUNT")
                    {
                        if (isChange && !ado.Error)
                        {
                            ProcessSaveBedLog(ado);
                        }
                    }
                    if (!ado.IsBedStretcher)
                    {
                        ado.SERVICE_CONDITION_ID = null;
                    }
                    gridControlBedHistory.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                gridControlBedHistory.RefreshDataSource();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CheckErrorDataBedLog(HisBedHistoryADO ado)
        {
            try
            {
                bool sucess = true;
                if (ado != null)
                {
                    if (ado.BED_ID <= 0)
                    {
                        ado.ErrorMessageBedId = ResourceMessage.ERROR_BED_ID;
                        ado.ErrorTypeBedId = ErrorType.Warning;
                        sucess = false;
                    }
                    else
                    {
                        ado.ErrorMessageBedId = "";
                        ado.ErrorTypeBedId = ErrorType.None;
                    }

                    if (ado.startTime == DateTime.MaxValue || ado.startTime == DateTime.MinValue)
                    {
                        ado.ErrorMessageStartTime = ResourceMessage.ThieuTruongDuLieuBatBuoc;
                        ado.ErrorTypeStartTime = ErrorType.Warning;
                        sucess = false;
                    }
                    else
                    {
                        ado.ErrorMessageStartTime = "";
                        ado.ErrorTypeStartTime = ErrorType.None;
                    }

                    if (CurrentTreatment != null && ado.ErrorTypeStartTime == ErrorType.None)
                    {
                        long startTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(ado.startTime) ?? 0;
                        if (startTime > 0 && startTime < CurrentTreatment.IN_TIME)
                        {
                            ado.ErrorMessageStartTime = ResourceMessage.ThoiGianBatDauKhongDuocNhoHonThoiGianVaoVien;
                            ado.ErrorTypeStartTime = ErrorType.Warning;
                            sucess = false;
                        }
                        else
                        {
                            ado.ErrorMessageStartTime = "";
                            ado.ErrorTypeStartTime = ErrorType.None;
                        }
                    }

                    if (ado.IsChecked && (!ado.BED_SERVICE_TYPE_ID.HasValue || ado.BED_SERVICE_TYPE_ID.Value <= 0))
                    {
                        ado.ErrorMessageBebServiceTypeId = ResourceMessage.ThieuTruongDuLieuBatBuoc;
                        ado.ErrorTypeBebServiceTypeId = ErrorType.Warning;
                        sucess = false;
                    }
                    else
                    {
                        ado.ErrorMessageBebServiceTypeId = "";
                        ado.ErrorTypeBebServiceTypeId = ErrorType.None;
                    }

                    if (ado.IsChecked && (!ado.finishTime.HasValue || ado.finishTime.Value == DateTime.MaxValue || ado.finishTime.Value == DateTime.MinValue))
                    {
                        ado.ErrorMessageFinishTime = ResourceMessage.ThieuTruongDuLieuBatBuoc;
                        ado.ErrorTypeFinishTime = ErrorType.Warning;
                        sucess = false;
                    }
                    else
                    {
                        ado.ErrorMessageFinishTime = "";
                        ado.ErrorTypeFinishTime = ErrorType.None;
                    }

                    if (ado.ErrorTypeFinishTime == ErrorType.None)
                    {
                        if (ado.IsChecked && ado.finishTime.HasValue && ado.finishTime < ado.startTime)
                        {
                            ado.ErrorMessageFinishTime = ResourceMessage.ERROR_FROM_TO_TIME;
                            ado.ErrorTypeFinishTime = ErrorType.Warning;
                            sucess = false;
                        }

                        else
                        {
                            ado.ErrorMessageFinishTime = "";
                            ado.ErrorTypeFinishTime = ErrorType.None;
                        }
                    }
                    if ((ado.ID == 0 || !ado.HasServiceReq) && (ado.ErrorTypeFinishTime == ErrorType.None || ado.ErrorTypeStartTime == ErrorType.None || ado.ErrorMessageStartTime == ResourceMessage.ERROR_OVERLAP_START_TIME || ado.ErrorMessageFinishTime == ResourceMessage.ERROR_OVERLAP_FINISH_TIME))
                    {
                        CheckWarningTimeOverLap(ado);
                    }

                    if (ado.ErrorTypeBedId == ErrorType.Warning
                        || (ado.ErrorTypeStartTime == ErrorType.Warning && ado.ErrorMessageStartTime != ResourceMessage.ERROR_OVERLAP_START_TIME)
                        || (ado.ErrorTypeFinishTime == ErrorType.Warning && ado.ErrorMessageFinishTime != ResourceMessage.ERROR_OVERLAP_FINISH_TIME) || ado.ErrorTypeBebServiceTypeId == ErrorType.Warning
                        || ado.ErrorTypePrimaryPatientTypeId == ErrorType.Warning)

                    {
                        ado.Error = true;
                    }
                    else
                        ado.Error = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CheckWarningTimeOverLap(HisBedHistoryADO ado)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("CheckWarningTimeOverLap. 1");
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("HisBedHistoryADO", ado));
                var vHisBedLogs = new List<HisBedHistoryADO>();
                bool isWarning = false;
                List<ADO.HisBedHistoryADO> A = new List<ADO.HisBedHistoryADO>();
                List<ADO.HisBedHistoryADO> B = new List<ADO.HisBedHistoryADO>();
                if (ListServiceReqForSereServs == null || dataBedADOs == null)
                {
                    throw new ArgumentNullException("ListServiceReqForSereServs hoặc dataBedADOs khong duoc null");
                }
                if (this.listCurrentBedLog != null && this.listCurrentBedLog.Count() > 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("listCurrentBedLog", listCurrentBedLog));
                    vHisBedLogs = listCurrentBedLog
                                .Where(r => r != null)
                                .Select(r => new ADO.HisBedHistoryADO(r, LocalStorage.LocalData.GlobalVariables.ActionEdit, true, ListServiceReqForSereServs, dataBedADOs))
                                .ToList();
                }
                Inventec.Common.Logging.LogSystem.Debug("CheckWarningTimeOverLap. 2");
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("vHisBedLogs", vHisBedLogs));
                if (vHisBedLogs != null)
                {
                    vHisBedLogs = vHisBedLogs.Where(o => o.ID != ado.ID).ToList();
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("bedLogChecks", bedLogChecks));
                    var newBed = bedLogChecks != null ? bedLogChecks.Where(o => o.ID == 0 && o != ado).ToList() : new List<HisBedHistoryADO>();
                    Inventec.Common.Logging.LogSystem.Debug("CheckWarningTimeOverLap. 2.1");
                    if (newBed.Count > 0) vHisBedLogs.AddRange(newBed);

                    if (vHisBedLogs.Count() == 0) return;
                    Inventec.Common.Logging.LogSystem.Debug("CheckWarningTimeOverLap. 2.2");
                    if (ado.finishTime == null)
                    {
                        A = vHisBedLogs.Where(o => o.startTime <= ado.startTime).ToList();
                        if (A == null || A.Count() == 0) return;
                        B = A.Where(o => o.finishTime == null || o.finishTime > ado.startTime).ToList();
                        isWarning = B != null && B.Count() > 0;
                        Inventec.Common.Logging.LogSystem.Debug("CheckWarningTimeOverLap. 2.2.1");
                    }
                    else
                    {
<<<<<<< .mine
                        Inventec.Common.Logging.LogSystem.Debug("CheckWarningTimeOverLap. 2.2.2");
                        A = vHisBedLogs.Where(o => o.startTime < ado.finishTime).ToList();
                        if (A == null || A.Count() == 0) return;
                        Inventec.Common.Logging.LogSystem.Debug("CheckWarningTimeOverLap. 2.2.3");
                        if (A.Exists(o => o.finishTime == null)) isWarning = true;
                        else
=======
                        B = A.Where(o => o.finishTime > ado.startTime).ToList();
                        if (B == null || B.Count() == 0) return;
                        //if (B.Exists(o => o.finishTime >= ado.finishTime || o.startTime <= ado.startTime)) isWarning = true; logic check cu
                        foreach (var _b in B)


>>>>>>> .theirs
                        {
                            Inventec.Common.Logging.LogSystem.Debug("CheckWarningTimeOverLap. 2.2.4");
                            B = A.Where(o => o.finishTime > ado.startTime).ToList();
                            if (B == null || B.Count() == 0) return;
                            //if (B.Exists(o => o.finishTime >= ado.finishTime || o.startTime <= ado.startTime)) isWarning = true; logic check cu
                            Inventec.Common.Logging.LogSystem.Debug("CheckWarningTimeOverLap. 2.2.5");
                            foreach (var _b in B)
                            {
                                var adoStart = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(ado.startTime);
                                var adoFinish = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(ado.finishTime);
                                //if((adoStart >= exitsBed.START_TIME && adoStart <= exitsBed.FINISH_TIME) 
                                //    || (adoFinish >= exitsBed.START_TIME && adoFinish <= exitsBed.FINISH_TIME)
                                //    )
                                //{

                                //}
                                if (adoStart <= _b.START_TIME)
                                {
                                    // start time nhỏ hơn hoặc bằng thời gian bắt đầu của giường cũ
                                    if (adoFinish >= _b.START_TIME || adoFinish >= _b.FINISH_TIME)
                                    {
                                        // Bao trọn hoặc chồng lên thời gian giường cũ
                                        if (adoFinish >= _b.FINISH_TIME)
                                        {
                                            // Nếu bao trọn cả khoảng thời gian của giường cũ
                                            isWarning = true;
                                        }
                                        else
                                        {
                                            // Chỉ chồng lên thời gian bắt đầu
                                            isWarning = true;
                                        }
                                    }
                                }
                                else
                                {
                                    // start time lớn hơn thời gian bắt đầu của giường cũ
                                    if (adoStart <= _b.FINISH_TIME)
                                    {
                                        // Chồng lên thời gian kết thúc của giường cũ
                                        isWarning = true;
                                    }
                                }
                            }
                        }
                    }
                    Inventec.Common.Logging.LogSystem.Debug("CheckWarningTimeOverLap. 3");
                    if (isWarning)
                    {
                        if (ado.startTime != null)
                        {
                            ado.ErrorTypeStartTime = ErrorType.Warning;
                            ado.ErrorMessageStartTime = ResourceMessage.ERROR_OVERLAP_START_TIME;
                        }
                        if (ado.finishTime != null)
                        {
                            ado.ErrorTypeFinishTime = ErrorType.Warning;
                            ado.ErrorMessageFinishTime = ResourceMessage.ERROR_OVERLAP_FINISH_TIME;
                        }
                    }
                }
                Inventec.Common.Logging.LogSystem.Debug("CheckWarningTimeOverLap. end");


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UpdateDateTimeBedLog()
        {
            try
            {
                WaitingManager.Show();
                bool success = false;
                CommonParam param = new CommonParam();
                var dataVhisBedLog = (ADO.HisBedHistoryADO)gridViewBedHistory.GetFocusedRow();
                if (dataVhisBedLog.finishTime < dataVhisBedLog.startTime)
                {
                    WaitingManager.Hide();
                    MessageManager.Show(ResourceMessage.ERROR_END_FINISH_TIME);
                    return;
                }
                else
                {
                    AutoMapper.Mapper.CreateMap<V_HIS_BED_LOG, HIS_BED_LOG>();
                    var hisBedLog = AutoMapper.Mapper.Map<V_HIS_BED_LOG, HIS_BED_LOG>(dataVhisBedLog);

                    hisBedLog.START_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dataVhisBedLog.startTime) ?? 0;
                    hisBedLog.FINISH_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dataVhisBedLog.finishTime);
                    var hisBedLogUpdate = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_BED_LOG>(Base.GlobalStore.HIS_BED_LOG_UPDATE, ApiConsumer.ApiConsumers.MosConsumer, hisBedLog, param);
                    if (hisBedLogUpdate != null)
                    {
                        success = true;
                        dataVhisBedLog.IsSave = true;
                    }
                    else
                    {
                        dataVhisBedLog.IsSave = false;
                    }
                }
                WaitingManager.Hide();

                #region Show message
                Inventec.Desktop.Common.Message.MessageManager.Show(this, param, success);
                #endregion

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void gridViewBedHistory_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView View = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    string name = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName().Trim();
                    string creator = (gridViewBedHistory.GetRowCellValue(e.RowHandle, "CREATOR") ?? "").ToString();
                    int action = Inventec.Common.TypeConvert.Parse.ToInt32((gridViewBedHistory.GetRowCellValue(e.RowHandle, "Action") ?? "").ToString());
                    bool isSave = Inventec.Common.TypeConvert.Parse.ToBoolean((gridViewBedHistory.GetRowCellValue(e.RowHandle, "IsSave") ?? "").ToString());

                    var data = (HisBedHistoryADO)gridViewBedHistory.GetRow(e.RowHandle);
                    if (data != null)
                    {
                        if (e.Column.FieldName == "IsChecked")
                        {
                            if (IsDisable)
                                e.RepositoryItem = CheckEditDisable;
                            else
                                e.RepositoryItem = (data.HasServiceReq || data.BED_SERVICE_TYPE_ID == null) ? CheckEditDisable : CheckEdit;
                            if (_TreatmentBedRoom.BED_ROOM_ID != WorkPlaceSDO.BedRoomId)
                            {
                                e.RepositoryItem = CheckEditDisable;
                            }
                        }
                        else if (e.Column.FieldName == "FinishTimeEdit")
                        {
                            //if (IsDisable)
                            //    e.RepositoryItem = ButtonEditDisable;
                            //else
                            e.RepositoryItem = (data.HasServiceReq) ? repEditFinishTimeEnable : repEditFinishTimeDisable;
                        }
                        else if (e.Column.FieldName == "btnDelete")
                        {
                            if (IsDisable)
                                e.RepositoryItem = ButtonDeleteDisable;
                            else
                            {
                                bool showbtn = false;
                                if ((IsAdmin || data.CREATOR == name || data.DEPARTMENT_ID == WorkPlaceSDO.DepartmentId))
                                {
                                    showbtn = true;
                                }
                                e.RepositoryItem = showbtn ? ButtonDelete : ButtonDeleteDisable;
                            }
                            if (_TreatmentBedRoom.BED_ROOM_ID != WorkPlaceSDO.BedRoomId)
                            {
                                e.RepositoryItem = ButtonDeleteDisable;
                            }

                        }
                        else if (e.Column.FieldName == "btnEdit")
                        {
                            if (IsDisable)
                                e.RepositoryItem = ButtonEditDisable;
                            else
                                e.RepositoryItem = (data.CREATOR != name) ? ButtonEditDisable : ButtonEdit;
                        }
                        else if (e.Column.FieldName == "finishTime")
                        {
                            if (IsDisable)
                                e.RepositoryItem = DateDisable;
                            else
                                e.RepositoryItem = (data.HasServiceReq) ? DateDisable : DateFinish;
                            if (_TreatmentBedRoom.BED_ROOM_ID != WorkPlaceSDO.BedRoomId)
                            {
                                e.RepositoryItem = DateDisable;
                            }
                        }
                        else if (e.Column.FieldName == "startTime")
                        {
                            if (IsDisable)
                                e.RepositoryItem = DateDisable;
                            else
                                e.RepositoryItem = (data.HasServiceReq) ? DateDisable : DateFinish;
                            if (_TreatmentBedRoom.BED_ROOM_ID != WorkPlaceSDO.BedRoomId)
                            {
                                e.RepositoryItem = DateDisable;
                            }

                        }
                        else if (e.Column.FieldName == "BUTTON_ADD")
                        {
                            if (IsDisable)
                                e.RepositoryItem = repositoryItemBtnAddDisable;
                            else
                            {
                                if (action == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd)
                                {
                                    e.RepositoryItem = repositoryItemBtnAdd;
                                }
                                else if (action == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit)
                                {
                                    e.RepositoryItem = repositoryItemBtnAddDisable;
                                }
                                if (_TreatmentBedRoom.BED_ROOM_ID != WorkPlaceSDO.BedRoomId)
                                {
                                    e.RepositoryItem = repositoryItemBtnAddDisable;
                                }
                            }

                        }
                        else if (e.Column.FieldName == "BUTTON_SAVE")
                        {
                            if (IsDisable)
                                e.RepositoryItem = repositoryItemBtnSaveDisable;
                            else
                                e.RepositoryItem = (data.CREATOR != name) ? repositoryItemBtnSaveDisable : repositoryItemBtnSaveBedLog;
                        }
                        else if (e.Column.FieldName == Gv_BedHistory__Gc_PatientType.FieldName)
                        {
                            e.RepositoryItem = repositoryItemCboBedPatientType;
                        }
                        else if (e.Column.FieldName == Gv_BedHistory__Gc_PrimaryPatientTypeId.FieldName)
                        {
                            e.RepositoryItem = repositoryItemCboBedPrimaryPatientType;
                        }
                        else if (e.Column.FieldName == "SERVICE_CONDITION_ID")
                        {
                            if (data.IsBedStretcher)
                            {
                                e.RepositoryItem = repServiceCondition;
                            }
                            else
                            {
                                e.RepositoryItem = repServiceConditionDis;
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

        private void gridViewBedHistory_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    else if (e.Column.FieldName == "PRICE_STR")
                    {
                        ADO.HisBedHistoryADO data = (ADO.HisBedHistoryADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                        if (data != null && data.PATIENT_TYPE_ID > 0)
                        {
                            long instructionTime = data.START_TIME > 0 ? data.START_TIME : (Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(data.startTime) ?? 0);
                            V_HIS_SERVICE_PATY data_ServicePrice = new V_HIS_SERVICE_PATY();
                            if (data.PRIMARY_PATIENT_TYPE_ID.HasValue)
                            {
                                data_ServicePrice = ServicePatyUtil.GetApplied(Base.GlobalStore.HisVServicePatys, WorkPlaceSDO.BranchId, WorkPlaceSDO.RoomId, WorkPlaceSDO.RoomId, WorkPlaceSDO.DepartmentId, instructionTime, CurrentTreatment.IN_TIME, data.BED_SERVICE_TYPE_ID ?? 0, data.PRIMARY_PATIENT_TYPE_ID ?? 0, null, 1, null, data.SERVICE_CONDITION_ID);
                            }
                            else
                            {
                                data_ServicePrice = ServicePatyUtil.GetApplied(Base.GlobalStore.HisVServicePatys, WorkPlaceSDO.BranchId, WorkPlaceSDO.RoomId, WorkPlaceSDO.RoomId, WorkPlaceSDO.DepartmentId, instructionTime, CurrentTreatment.IN_TIME, data.BED_SERVICE_TYPE_ID ?? 0, data.PATIENT_TYPE_ID ?? 0, null, 1, null, data.SERVICE_CONDITION_ID);
                            }

                            if (data_ServicePrice != null)
                            {
                                e.Value = Inventec.Common.Number.Convert.NumberToString((data_ServicePrice.PRICE * (1 + data_ServicePrice.VAT_RATIO)), ConfigApplications.NumberSeperator);
                            }
                            else
                            {
                                e.Value = 0;
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

        private void gridViewBedHistory_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                GridHitInfo hi = view.CalcHitInfo(e.Location);
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    if (hi.InRowCell)
                    {
                        if (hi.Column.RealColumnEdit.GetType() == typeof(DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit))
                        {
                            view.FocusedRowHandle = hi.RowHandle;
                            view.FocusedColumn = hi.Column;
                            view.ShowEditor();
                            DevExpress.XtraEditors.CheckEdit checkEdit = view.ActiveEditor as DevExpress.XtraEditors.CheckEdit;
                            DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo checkInfo = (DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo)checkEdit.GetViewInfo();
                            Rectangle glyphRect = checkInfo.CheckInfo.GlyphRect;
                            GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                            Rectangle gridGlyphRect =
                                new Rectangle(viewInfo.GetGridCellInfo(hi).Bounds.X + glyphRect.X,
                                 viewInfo.GetGridCellInfo(hi).Bounds.Y + glyphRect.Y,
                                 glyphRect.Width,
                                 glyphRect.Height);
                            if (!gridGlyphRect.Contains(e.Location))
                            {
                                view.CloseEditor();
                                if (!view.IsCellSelected(hi.RowHandle, hi.Column))
                                {
                                    view.SelectCell(hi.RowHandle, hi.Column);
                                }
                                else
                                {
                                    view.UnselectCell(hi.RowHandle, hi.Column);
                                }
                            }
                            else
                            {
                                checkEdit.Checked = !checkEdit.Checked;
                                view.CloseEditor();
                            }
                            (e as DevExpress.Utils.DXMouseEventArgs).Handled = true;
                        }
                    }
                    if (hi.HitTest == GridHitTest.Column)
                    {
                        if (hi.Column.FieldName == "IsChecked")
                        {
                            gridViewBedHistory.BeginUpdate();
                            if (isCheckAll == true)
                            {
                                foreach (var item in this.bedLogChecks)
                                {

                                    item.IsChecked = true;
                                    CheckErrorDataBedLog(item);
                                    if (item.Error
                                           || !string.IsNullOrEmpty(item.ErrorMessageStartTime)
                                           || !string.IsNullOrEmpty(item.ErrorMessageFinishTime)
                                           || !string.IsNullOrEmpty(item.ErrorMessageBebServiceTypeId)
                                           || !string.IsNullOrEmpty(item.ErrorMessageBedId)
                                           || !string.IsNullOrEmpty(item.ErrorMessagePatientTypeId)
                                           || !string.IsNullOrEmpty(item.ErrorMessagePrimaryPatientTypeId)
                                           || item.ErrorTypeStartTime == ErrorType.Warning
                                           || item.ErrorTypeFinishTime == ErrorType.Warning)
                                    {
                                        item.IsChecked = false;
                                    }
                                    if (item.HasServiceReq || item.ID == 0)
                                    {
                                        item.IsChecked = false;
                                    }
                                }
                                isCheckAll = false;
                            }
                            else
                            {
                                foreach (var item in this.bedLogChecks)
                                {
                                    item.IsChecked = false;
                                }
                                isCheckAll = true;
                            }
                            gridViewBedHistory.EndUpdate();

                            if (!IsDisable)
                            {
                                ProcessAllBedLogChecks();
                                CountTimeBed();
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

        private void ProcessAllBedLogChecks()
        {
            try
            {
                var bedLogCheck = bedLogChecks.Where(o => o.IsChecked == true).ToList();
                foreach (var item in bedLogCheck)
                {
                    CheckErrorDataBedLog(item);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ButtonEdit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var dataVhisBedLog = (MOS.EFMODEL.DataModels.V_HIS_BED_LOG)gridViewBedHistory.GetFocusedRow();
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.BedAssign").FirstOrDefault();
                if (moduleData == null)
                {
                    Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.BedAssign");
                    MessageManager.Show(ResourceMessage.TaiKhoanKhongCoQuyenThucHienChucNang);
                }

                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    Desktop.ADO.BedLogADO bedLogAdo = new Desktop.ADO.BedLogADO(null, dataVhisBedLog);
                    moduleData.RoomId = WorkPlaceSDO.RoomId;
                    moduleData.RoomTypeId = WorkPlaceSDO.RoomTypeId;
                    List<object> listArgs = new List<object>();
                    listArgs.Add(bedLogAdo);
                    var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, WorkPlaceSDO.RoomId, WorkPlaceSDO.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    ((Form)extenceInstance).ShowDialog();
                    LoadDataGridServiceReq();
                    FillDataToGridBedLog();
                }
                else
                {
                    MessageManager.Show(ResourceMessage.TaiKhoanKhongCoQuyenThucHienChucNang);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ButtonDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                var deleteVhisBedLog = (MOS.EFMODEL.DataModels.V_HIS_BED_LOG)gridViewBedHistory.GetFocusedRow();
                if (deleteVhisBedLog != null && DevExpress.XtraEditors.XtraMessageBox.Show(
                    ResourceMessage.HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong,
                    ResourceMessage.ThongBao,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    bool IsRemoveShareCount = false;

                    if (deleteVhisBedLog.ID > 0)
                    {
                        if (IsShareBed == "1" && deleteVhisBedLog.SHARE_COUNT > 1 && DevExpress.XtraEditors.XtraMessageBox.Show("Giường hiện tại có nằm ghép. Bạn có muốn cập nhật thông tin nằm ghép của bệnh nhân khác không?", ResourceMessage.ThongBao, MessageBoxButtons.YesNo) == DialogResult.Yes)
                            IsRemoveShareCount = true;
                        var success = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>(Base.GlobalStore.HIS_BED_LOG_DELETE, ApiConsumer.ApiConsumers.MosConsumer, deleteVhisBedLog.ID, param);
                        if (success == true)
                        {
                            gridControlBedHistory.BeginUpdate();
                            success = true;
                            listCurrentBedLog = listCurrentBedLog.Where(o => o.ID != deleteVhisBedLog.ID).ToList();
                            if (listCurrentBedLog == null) listCurrentBedLog = new List<V_HIS_BED_LOG>();
                            gridViewBedHistory.DeleteRow(gridViewBedHistory.FocusedRowHandle);
                            gridControlBedHistory.RefreshDataSource();
                            gridControlBedHistory.EndUpdate();
                        }

                        #region Show message
                        Inventec.Desktop.Common.Message.MessageManager.Show(this, param, success);
                        #endregion
                    }
                    if (deleteVhisBedLog.BED_ID > 0)
                    {
                        var bedLogAllids = dataBedADOs.FirstOrDefault(o => o.ID == deleteVhisBedLog.BED_ID).BedLogAllIds;
                        if (bedLogAllids != null && bedLogAllids.Count > 0 && IsRemoveShareCount)
                        {
                            var lstbedLogIds = bedLogAllids.Where(o => o != deleteVhisBedLog.ID).ToList();
                            RemoveShareCount(lstbedLogIds);
                        }
                    }




                    if (bedLogChecks == null || bedLogChecks.Count <= 0)
                    {
                        ADO.HisBedHistoryADO ado = new ADO.HisBedHistoryADO();
                        ado.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                        ado.CREATOR = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                        bedLogChecks.Add(ado);
                        gridControlBedHistory.BeginUpdate();
                        gridControlBedHistory.DataSource = bedLogChecks;
                        gridControlBedHistory.EndUpdate();
                    }
                    else
                    {
                        if (!bedLogChecks.Exists(o => o.Action == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd))
                        {
                            bedLogChecks.First().Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                        }
                    }

                    if (!IsDisable)
                    {
                        ProcessAllBedLogChecks();
                        CountTimeBed();
                    }

                    #region Process has exception
                    HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtnAdd_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                ProcessAllBedLogChecks();
                SaveAllBedLog();
                ADO.HisBedHistoryADO ado = new ADO.HisBedHistoryADO();
                ado.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                ado.CREATOR = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();

                var maxTime = bedLogChecks.Where(o => o.finishTime.HasValue).ToList();
                if (maxTime != null && maxTime.Count > 0)
                {
                    maxTime = maxTime.OrderByDescending(o => o.finishTime.Value).ToList();
                    ado.startTime = maxTime.First().finishTime.Value;
                }

                this.bedLogChecks.Add(ado);
                gridControlBedHistory.BeginUpdate();
                gridControlBedHistory.DataSource = null;
                gridControlBedHistory.DataSource = bedLogChecks;
                gridControlBedHistory.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtnSaveBedLog_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (ADO.HisBedHistoryADO)gridViewBedHistory.GetFocusedRow();
                ProcessSaveBedLog(row);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool BreakServiceCondition = false;
        public void ProcessSaveBedLog(ADO.HisBedHistoryADO row, bool IsCheckValidForm = true)
        {
            try
            {
                bool success = false;
                BreakServiceCondition = false;
                CommonParam param = new CommonParam();
                if (IsCheckValidForm && !ValidSereServWithCondition(row))
                    return;
                var resultRow = SaveDataBedLog(row, ref success, ref param);
                if (resultRow != null)
                {
                    gridViewBedHistory.BeginUpdate();
                    row.IsSave = true;
                    row.BedIdAfterSave = resultRow.BED_ID;
                    gridControlBedHistory.RefreshDataSource();
                    gridViewBedHistory.EndUpdate();
                }


                if ((param.BugCodes != null && param.BugCodes.Count > 0) || (param.Messages != null && param.Messages.Count > 0))
                {
                    #region Show message
                    MessageManager.Show(this, param, success);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private HIS_BED_LOG SaveDataBedLog(ADO.HisBedHistoryADO row, ref bool success, ref CommonParam param, bool isClose = false)
        {
            HIS_BED_LOG outPut = new HIS_BED_LOG();
            try
            {
                if (row != null)
                {
                    HisBedLogSDO inPut = new HisBedLogSDO();
                    SaveBedLogData(row, ref inPut);
                    bool isCreate = row.ID == 0;
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("inPut___________________", inPut));
                    outPut = new BackendAdapter(param).Post<HIS_BED_LOG>(isCreate ? Base.GlobalStore.HIS_BED_LOG_CREATE : Base.GlobalStore.HIS_BED_LOG_UPDATE, ApiConsumer.ApiConsumers.MosConsumer, inPut, param);
                    if (outPut != null)
                    {
                        success = true;
                        if (!isClose)
                        {
                            if (isCreate)
                            {
                                MOS.Filter.HisBedLogViewFilter filterBedLog = new MOS.Filter.HisBedLogViewFilter();
                                filterBedLog.ID = outPut.ID;
                                var vHisBedLogs = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.V_HIS_BED_LOG>>(Base.GlobalStore.HIS_BED_LOG_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, filterBedLog, null);

                                if (vHisBedLogs != null && vHisBedLogs.Count == 1)
                                {
                                    listCurrentBedLog.Add(vHisBedLogs.First());
                                }
                                Inventec.Common.Mapper.DataObjectMapper.Map<ADO.HisBedHistoryADO>(row, outPut);
                            }
                            else
                            {
                                var a = listCurrentBedLog.FirstOrDefault(o => o.ID == row.ID);
                                Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_BED_LOG>(a, row);
                            }
                            LoadDataGridServiceReq();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                success = false;
                outPut = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return outPut;
        }

        private bool CheckDataBedLog(HisBedHistoryADO row)
        {
            bool result = true;
            try
            {
                string messageError = "";
                if (row != null)
                {
                    if (row.BED_ID <= 0)
                    {
                        messageError += ResourceMessage.ERROR_BED_ID;
                    }

                    //Ktra thời gian kết thúc
                    DateTime finishTime = row.finishTime.Value;
                    DateTime fromTime = row.startTime;
                    if (finishTime != DateTime.MinValue && finishTime < fromTime)
                    {
                        messageError += (ResourceMessage.ERROR_FROM_TO_TIME + fromTime.ToString());
                    }
                }
                else
                {
                    messageError += ResourceMessage.ThieuTruongDuLieuBatBuoc;
                }

                if (!String.IsNullOrWhiteSpace(messageError))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(messageError);
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
            return result;
        }

        private void SaveBedLogData(ADO.HisBedHistoryADO row, ref HisBedLogSDO inPut)
        {
            try
            {
                if (row.ID > 0)
                {
                    inPut.Id = row.ID;
                }
                inPut.ServiceConditionId = row.SERVICE_CONDITION_ID;
                inPut.BedId = row.BED_ID;
                inPut.BedServiceTypeId = row.BED_SERVICE_TYPE_ID;
                if (row.finishTime.HasValue && row.finishTime.Value != DateTime.MinValue && row.finishTime.Value != DateTime.MaxValue)
                {
                    inPut.FinishTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(row.finishTime) ?? 0;
                }

                inPut.PatientTypeId = row.PATIENT_TYPE_ID;
                inPut.PrimaryPatientTypeId = row.PRIMARY_PATIENT_TYPE_ID;
                inPut.ShareCount = row.SHARE_COUNT;
                inPut.StartTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(row.startTime) ?? 0;

                inPut.TreatmentBedRoomId = _TreatmentBedRoom.ID > 0 ? _TreatmentBedRoom.ID : this.WorkPlaceSDO.BedRoomId.Value;
                inPut.WorkingRoomId = this.WorkPlaceSDO.RoomId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemCboBed_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    var cbo = sender as HIS.Desktop.Utilities.Extensions.CustomGridLookUpEdit;
                    if (cbo.EditValue != null)
                    {
                        var row = (ADO.HisBedHistoryADO)gridViewBedHistory.GetFocusedRow();
                        if (cbo.EditValue != cbo.OldEditValue)
                        {
                            row.IsSave = false;
                        }
                        long bedIdValue = (long)cbo.EditValue;
                        var dataBed = this.dataBedADOs.FirstOrDefault(p => p.ID == bedIdValue);
                        if (dataBed != null)
                        {
                            var shareCountdf = row.SHARE_COUNT;
                            row.SHARE_COUNT = null;

                            if (dataBed.IsKey == 1)
                            {
                                if (_TreatmentBedRoom != null)
                                {
                                    var treatmentTYpe = BackendDataWorker.Get<HIS_TREATMENT_TYPE>().FirstOrDefault(o => o.ID == _TreatmentBedRoom.TDL_TREATMENT_TYPE_ID);
                                    if (treatmentTYpe != null)
                                    {
                                        if (treatmentTYpe.IS_NOT_ALLOW_SHARE_BED == 1)
                                        {
                                            DevExpress.XtraEditors.XtraMessageBox.Show(string.Format(ResourceMessage.KhongDuocNamGhepGiuong, treatmentTYpe.TREATMENT_TYPE_NAME), ResourceMessage.ThongBao, MessageBoxButtons.OK);
                                            cbo.EditValue = null;
                                            cbo.ShowPopup();
                                            return;
                                        }
                                        else
                                        {
                                            HisTreatmentBedRoomFilter bedRoomFilter = new HisTreatmentBedRoomFilter();
                                            bedRoomFilter.IDs = dataBed.TREATMENT_BED_ROOM_IDs;
                                            bedRoomFilter.IS_ACTIVE = 1;

                                            var bedRooms = new BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT_BED_ROOM>>("api/HisTreatmentBedRoom/GetView", ApiConsumer.ApiConsumers.MosConsumer, bedRoomFilter, null);
                                            List<string> lstPatientName = new List<string>();
                                            if (bedRooms != null && bedRooms.Count > 0)
                                            {
                                                bedRooms = bedRooms.Where(o => o.TREATMENT_ID != _TreatmentBedRoom.TREATMENT_ID).ToList();
                                            }

                                            foreach (var item in bedRooms)
                                            {
                                                lstPatientName.Add(item.TREATMENT_CODE + " - " + item.TDL_PATIENT_NAME);
                                            }

                                            if (DevExpress.XtraEditors.XtraMessageBox.Show(string.Format("Giường đã có bệnh nhân {0} nằm. Bạn có muốn cho bệnh nhân nằm ghép không?", string.Join(", ", lstPatientName)), ResourceMessage.ThongBao, MessageBoxButtons.YesNo) == DialogResult.No)
                                            {
                                                cbo.EditValue = null;
                                                cbo.ShowPopup();
                                                return;
                                            }
                                            if (row.ID <= 0)
                                            {
                                                if (IsShareBed != "1")
                                                {
                                                    row.SHARE_COUNT = dataBed.AMOUNT + 1;
                                                }
                                                else
                                                {
                                                    row.SHARE_COUNT = GetMaxShareCount(dataBed, row);
                                                }
                                            }
                                            else
                                            {
                                                var bedId = row.BED_ID != 0 ? row.BED_ID : row.BedIdAfterSave;
                                                if (bedId != 0 && bedId != dataBed.ID && shareCountdf > 1 && IsShareBed == "1" && DevExpress.XtraEditors.XtraMessageBox.Show("Giường hiện tại có nằm ghép. Bạn có muốn cập nhật thông tin nằm ghép của bệnh nhân khác không?", ResourceMessage.ThongBao, MessageBoxButtons.YesNo) == DialogResult.Yes)
                                                {
                                                    var dataBedOld = this.dataBedADOs.FirstOrDefault(p => p.BED_CODE_ID == bedId);
                                                    var UpdateShareCountOld = UpdateShareCount(dataBedOld, row);
                                                    Inventec.Common.Logging.LogSystem.Debug("UpdateShareCountOld: " + UpdateShareCountOld);
                                                }
                                                List<long> lstMax = new List<long>();
                                                var bedLogAllids = dataBed.BedLogAllIds;
                                                if (bedLogAllids != null && bedLogAllids.Count > 0)
                                                {
                                                    lstMax = bedLogAllids.Where(o => o != row.ID).ToList();
                                                }
                                                if (lstMax != null && lstMax.Count > 0)
                                                    row.SHARE_COUNT = GetShareCount(lstMax);
                                            }
                                        }
                                    }
                                }
                            }
                            else if (dataBed.IsKey == 2)
                            {
                                CommonParam param = new CommonParam();
                                HisTreatmentBedRoomViewFilter filterTreatmentBedRoom = new HisTreatmentBedRoomViewFilter();
                                filterTreatmentBedRoom.IDs = dataBed.TREATMENT_BED_ROOM_IDs;
                                filterTreatmentBedRoom.IS_ACTIVE = 1;
                                lstHisTreatmentBedRoom = new BackendAdapter(param).Get<List<V_HIS_TREATMENT_BED_ROOM>>("api/HisTreatmentBedRoom/GetView", ApiConsumer.ApiConsumers.MosConsumer, filterTreatmentBedRoom, param);
                                List<string> lstPatientName = new List<string>();
                                //if (lstHisTreatmentBedRoom != null && lstHisTreatmentBedRoom.Count > 0)
                                //{
                                //    lstHisTreatmentBedRoom = lstHisTreatmentBedRoom.Where(o => o.TREATMENT_ID != _TreatmentBedRoom.TREATMENT_ID).ToList();
                                //}

                                if (lstHisTreatmentBedRoom != null && lstHisTreatmentBedRoom.Count > 0)
                                {
                                    lstHisTreatmentBedRoom = lstHisTreatmentBedRoom.Where(o => !bedLogChecks.Select(s => s.TREATMENT_ID).Contains(o.TREATMENT_ID)).ToList();
                                }
                                foreach (var item in lstHisTreatmentBedRoom)
                                {
                                    lstPatientName.Add(item.TREATMENT_CODE + " - " + item.TDL_PATIENT_NAME);
                                }
                                if (lstPatientName != null && lstPatientName.Count > 0)
                                {
                                    DevExpress.XtraEditors.XtraMessageBox.Show(string.Format("Giường {0} vượt quá số lượng nằm ghép tối đa - Những bệnh nhân đang nằm : {1}", dataBed.BED_NAME, string.Join(", ", lstPatientName)), ResourceMessage.ThongBao);

                                    cbo.EditValue = null;
                                    cbo.ShowPopup();
                                    return;
                                }

                            }
                            else
                            {
                                var bedId = row.BED_ID != 0 ? row.BED_ID : row.BedIdAfterSave;
                                if (bedId != 0 && bedId != dataBed.ID && IsShareBed == "1" && DevExpress.XtraEditors.XtraMessageBox.Show("Giường hiện tại có nằm ghép. Bạn có muốn cập nhật thông tin nằm ghép của bệnh nhân khác không?", ResourceMessage.ThongBao, MessageBoxButtons.YesNo) == DialogResult.Yes)
                                {
                                    var dataBedOld = this.dataBedADOs.FirstOrDefault(p => p.BED_CODE_ID == bedId);
                                    var UpdateShareCountOld = UpdateShareCount(dataBedOld, row);
                                    Inventec.Common.Logging.LogSystem.Debug("UpdateShareCountOld: " + UpdateShareCountOld);
                                }
                            }
                        }
                        row.BED_ID = bedIdValue;
                        row.BED_CODE = dataBed.BED_CODE;
                        row.BED_NAME = dataBed.BED_NAME;
                        row.BED_CODE_ID = dataBed.BED_CODE_ID;
                        row.BED_TYPE_CODE = dataBed.BED_TYPE_CODE;
                        row.BED_TYPE_ID = dataBed.BED_TYPE_ID;
                        row.BED_TYPE_NAME = dataBed.BED_TYPE_NAME;
                        row.IsBedStretcher = dataBed.IS_BED_STRETCHER == 1;
                        LoadDataToCboBedServiceType(row);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemCboBed_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    var cbo = sender as HIS.Desktop.Utilities.Extensions.CustomGridLookUpEdit;
                    cbo.Properties.Buttons[1].Visible = false;
                    var data = (ADO.HisBedHistoryADO)gridViewBedHistory.GetFocusedRow();
                    if (data != null && !data.HasServiceReq)
                    {
                        data.BED_ID = 0;
                        data.BED_CODE_ID = 0;
                        data.BED_TYPE_NAME = null;
                        data.BED_SERVICE_TYPE_CODE = null;
                        data.BED_SERVICE_TYPE_ID = null;
                        data.PATIENT_TYPE_ID = null;
                        data.PRIMARY_PATIENT_TYPE_ID = null;
                        data.SHARE_COUNT = null;
                    }
                    gridViewBedHistory.EditingValue = null;
                    gridControlBedHistory.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToCboBedServiceType(ADO.HisBedHistoryADO row)
        {
            try
            {
                row.BED_SERVICE_TYPE_ID = null;
                row.BED_SERVICE_TYPE_CODE = null;
                row.PRIMARY_PATIENT_TYPE_ID = null;
                row.PATIENT_TYPE_ID = null;
                row.ErrorMessagePrimaryPatientTypeId = null;
                row.ErrorTypePrimaryPatientTypeId = ErrorType.None;
                row.ErrorTypeStartTime = ErrorType.None;
                row.ErrorTypeFinishTime = ErrorType.None;
                var currentServiceTypeByBeds = ProcessServiceRoom(row.BED_ID);
                if (currentServiceTypeByBeds != null && currentServiceTypeByBeds.Count > 0)
                {
                    currentServiceTypeByBeds = currentServiceTypeByBeds.OrderBy(p => p.SERVICE_CODE).ToList();

                    row.BED_SERVICE_TYPE_ID = currentServiceTypeByBeds[0].SERVICE_ID;
                    row.BED_SERVICE_TYPE_CODE = currentServiceTypeByBeds[0].SERVICE_CODE;

                    var bedType = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o => o.ID == currentServiceTypeByBeds[0].SERVICE_ID);
                    row.BILL_PATIENT_TYPE_ID = bedType != null ? bedType.BILL_PATIENT_TYPE_ID : null;
                    row.IS_NOT_CHANGE_BILL_PATY = bedType != null ? bedType.IS_NOT_CHANGE_BILL_PATY == 1 : false;
                    row.IS_NOT_CHANGE_BILL_PATY_DEFAULT = row.IS_NOT_CHANGE_BILL_PATY;
                    ChoosePatientTypeDefaultlService(CurrentTreatment.TDL_PATIENT_TYPE_ID ?? 0, currentServiceTypeByBeds[0].SERVICE_ID, row);
                }
                gridControlBedHistory.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<V_HIS_SERVICE_ROOM> ProcessServiceRoom(long bedId)
        {
            List<V_HIS_SERVICE_ROOM> result = null;
            try
            {
                CommonParam param = new CommonParam();

                List<HIS_BED_BSTY> lstBedServiceTypes = hisBedBstys != null && hisBedBstys.Count > 0 ? hisBedBstys.Where(o => o.BED_ID == bedId && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList() : null;
                if (!IsDisable && !chkBHAll.Checked && lstBedServiceTypes != null && lstBedServiceTypes.Count > 0 && !chkBedAll.Checked)
                {

                    List<V_HIS_BED_ROOM> bedRoom = ListVBedRoom != null && ListVBedRoom.Count > 0 ? ListVBedRoom.Where(o => o.ROOM_ID == this.WorkPlaceSDO.RoomId).ToList() : null;
                    if (bedRoom != null && bedRoom.Count > 0)
                    {
                        List<HisBedADO> hisBed = dataBedADOs != null && dataBedADOs.Count > 0 ? dataBedADOs.Where(o => bedRoom.Select(s => s.ID).Contains(o.BED_ROOM_ID)).ToList() : null;
                        if (hisBed != null && hisBed.Count > 0)
                        {
                            lstBedServiceTypes = lstBedServiceTypes.Where(o => hisBed.Select(s => s.ID).Contains(o.BED_ID)).ToList();
                        }
                    }
                }

                List<long> bedServiceTypeIds = new List<long>();
                if (lstBedServiceTypes != null && lstBedServiceTypes.Count > 0)
                {
                    bedServiceTypeIds = lstBedServiceTypes.Select(p => p.BED_SERVICE_TYPE_ID).ToList();
                }

                var lstBedServiceTypeByBedId = VHisBedServiceTypes.Where(p => bedServiceTypeIds.Contains(p.ID)).ToList();
                List<long> serviceIds = new List<long>();
                if (lstBedServiceTypeByBedId != null && lstBedServiceTypeByBedId.Count > 0)
                {
                    serviceIds = lstBedServiceTypeByBedId.Select(p => p.ID).ToList();
                }

                if (IsDisable)
                    result = BackendDataWorker.Get<V_HIS_SERVICE_ROOM>().Where(p =>
                         serviceIds.Contains(p.SERVICE_ID)
                         && p.IS_ACTIVE == 1)
                         .ToList();
                else
                    result = ListServiceBedByRooms.Where(p => serviceIds.Contains(p.SERVICE_ID)).GroupBy(o => o.SERVICE_ID).Select(g => g.First()).ToList();
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void gridViewBedHistory_ShownEditor(object sender, EventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                var row = (ADO.HisBedHistoryADO)gridViewBedHistory.GetFocusedRow();

                if (view.FocusedColumn.FieldName == "BED_SERVICE_TYPE_ID" && view.ActiveEditor is GridLookUpEdit)
                {
                    GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;
                    if (editor != null)
                    {
                        FillDataIntoBedServiceTypeCombo(row, editor);
                        editor.EditValue = row.BED_SERVICE_TYPE_ID;
                    }
                }
                else if (view.FocusedColumn.FieldName == "BED_ID" && view.ActiveEditor is GridLookUpEdit)
                {
                    GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;
                    if (editor != null)
                    {
                        FillDataCboBed(row, editor, false);
                        editor.EditValue = row.BED_ID;
                    }
                }
                else if (view.FocusedColumn.FieldName == "BED_CODE_ID" && view.ActiveEditor is GridLookUpEdit)
                {
                    GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;
                    if (editor != null)
                    {
                        FillDataCboBed(row, editor, true);
                        editor.EditValue = row.BED_CODE;
                    }
                }
                else if (view.FocusedColumn.FieldName == Gv_BedHistory__Gc_PatientType.FieldName && view.ActiveEditor is GridLookUpEdit)
                {
                    GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;
                    if (row != null)
                    {
                        this.FillDataIntoPatientTypeCombo(row, editor);
                        editor.EditValue = row.PATIENT_TYPE_ID;
                    }
                }
                else if (view.FocusedColumn.FieldName == "SERVICE_CONDITION_ID" && view.ActiveEditor is GridLookUpEdit)
                {
                    GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;
                    if (row != null)
                    {
                        this.FillDataIntoServiceConditionCombo(row, editor);
                        editor.EditValue = row.SERVICE_CONDITION_ID;
                    }
                }
                else if (view.FocusedColumn.FieldName == Gv_BedHistory__Gc_PrimaryPatientTypeId.FieldName && view.ActiveEditor is GridLookUpEdit)
                {
                    GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;
                    if (row != null)
                    {
                        if ((Base.GlobalStore.IsPrimaryPatientType == "2" && this.CurrentTreatment.PRIMARY_PATIENT_TYPE_ID == row.PRIMARY_PATIENT_TYPE_ID) || (row.BILL_PATIENT_TYPE_ID.HasValue && row.IsContainAppliedPatientType && row.PRIMARY_PATIENT_TYPE_ID.HasValue && row.BILL_PATIENT_TYPE_ID == row.BILL_PATIENT_TYPE_ID && (row.IS_NOT_CHANGE_BILL_PATY || row.IS_NOT_CHANGE_BILL_PATY_DEFAULT)))
                        {
                            editor.ReadOnly = true;
                            editor.Properties.Buttons[1].Visible = false;
                        }
                        else
                        {
                            this.FillDataIntoPrimaryPatientTypeCombo(row, editor);
                            editor.EditValue = row.PRIMARY_PATIENT_TYPE_ID;
                            editor.Properties.Buttons[1].Visible = true;
                        }
                    }
                    else
                    {
                        editor.ReadOnly = true;
                        editor.Properties.Buttons[1].Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataCboBed(HisBedHistoryADO row, GridLookUpEdit editor, bool isBedCode)
        {
            try
            {
                CommonParam param = new CommonParam();
                List<V_HIS_BED> hisBeds = LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_BED>().Where(o => o.IS_ACTIVE == 1).ToList();
                if (chkBHAll.Checked && this.WorkPlaceSDO != null && !chkBedAll.Checked)
                {
                    var lstRoomNotInDepartment = LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_BED_ROOM>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList().Where(o => o.DEPARTMENT_ID != this.WorkPlaceSDO.DepartmentId).ToList();
                    if (lstRoomNotInDepartment != null && lstRoomNotInDepartment.Count > 0)
                    {
                        this.ListVBedRoom = ListVBedRoom.Where(o => !lstRoomNotInDepartment.Exists(p => p.ID == o.ID)).ToList();
                    }
                }
                if (chkBedAll.Checked)
                    this.ListVBedRoom = LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_BED_ROOM>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                List<long> lstBedIds = new List<long>();
                if (row.BED_SERVICE_TYPE_ID.HasValue && row.BED_SERVICE_TYPE_ID != 0)
                {
                    lstBedIds = hisBedBstys.Where(p => p.BED_SERVICE_TYPE_ID == row.BED_SERVICE_TYPE_ID).Select(p => p.BED_ID).ToList();

                    if (this.ListVBedRoom != null && this.ListVBedRoom.Count > 0)
                    {
                        var beds = hisBeds.Where(o => this.ListVBedRoom.Select(s => s.ID).Contains(o.BED_ROOM_ID) && lstBedIds.Contains(o.ID)).ToList();
                        InitComboBed(editor, beds, row, isBedCode);
                    }
                    else
                    {
                        var lstBeds = hisBeds.Where(p => lstBedIds.Contains(p.ID)).ToList();
                        InitComboBed(editor, lstBeds, row, isBedCode);
                    }
                }
                else if (this.ListVBedRoom != null && this.ListVBedRoom.Count > 0)
                {
                    var beds = hisBeds.Where(o => this.ListVBedRoom.Select(s => s.ID).Contains(o.BED_ROOM_ID)).ToList();
                    InitComboBed(editor, beds, row, isBedCode);
                }
                else
                    InitComboBed(editor, hisBeds, row, isBedCode);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataIntoBedServiceTypeCombo(HisBedHistoryADO row, GridLookUpEdit editor)
        {
            try
            {
                var currentServiceTypeByBeds = ProcessServiceRoom(row.BED_ID).Distinct().ToList();
                // Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("currentServiceTypeByBeds_____", currentServiceTypeByBeds));
                InitComboServiceRoom(editor, currentServiceTypeByBeds);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboServiceRoom(DevExpress.XtraEditors.GridLookUpEdit Combo, List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM> data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SERVICE_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("SERVICE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_NAME", "SERVICE_ID", columnInfos, false, 250);
                ControlEditorLoader.Load(Combo, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboBed(DevExpress.XtraEditors.GridLookUpEdit Combo, List<V_HIS_BED> datas, HisBedHistoryADO bedHistory, bool isBedCode)
        {
            try
            {
                dataBedADOs = new List<HisBedADO>();
                dataBedADOs = ProcessDataBedAdo(datas, bedHistory);

                if (isBedCode)
                {
                    List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                    columnInfos.Add(new ColumnInfo("BED_CODE", "", 50, 1));
                    ControlEditorADO controlEditorADO = new ControlEditorADO("BED_CODE", "BED_CODE_ID", columnInfos, false, 250);
                    ControlEditorLoader.Load(Combo, dataBedADOs, controlEditorADO);
                }
                else
                {
                    List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                    columnInfos.Add(new ColumnInfo("BED_CODE", "", 50, 1));
                    columnInfos.Add(new ColumnInfo("BED_NAME", "", 250, 2));
                    columnInfos.Add(new ColumnInfo("AMOUNT_STR", "", 50, 3));
                    ControlEditorADO controlEditorADO = new ControlEditorADO("BED_NAME", "ID", columnInfos, false, 250);
                    ControlEditorLoader.Load(Combo, dataBedADOs, controlEditorADO);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemCboBedServiceType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    var cbo = sender as HIS.Desktop.Utilities.Extensions.CustomGridLookUpEdit;
                    if (cbo.EditValue != null)
                    {
                        var row = (ADO.HisBedHistoryADO)gridViewBedHistory.GetFocusedRow();
                        if (cbo.EditValue != cbo.OldEditValue)
                        {
                            row.IsSave = false;
                        }
                    }
                    gridControlBedHistory.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemCboNamGhep2_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    var cbo = sender as HIS.Desktop.Utilities.Extensions.CustomGridLookUpEdit;
                    cbo.Properties.Buttons[1].Visible = false;
                    var data = (ADO.HisBedHistoryADO)gridViewBedHistory.GetFocusedRow();
                    if (data != null && !data.HasServiceReq)
                    {
                        data.SHARE_COUNT = null;
                    }
                    gridViewBedHistory.EditingValue = null;
                    gridControlBedHistory.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemCboNamGhep2_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    var cbo = sender as HIS.Desktop.Utilities.Extensions.CustomGridLookUpEdit;
                    var data = (ADO.HisBedServiceTypeADO)gridViewBedServiceType.GetFocusedRow();
                    if (gridViewBedServiceType.EditingValue != null)
                    {
                        if (data != null)
                        {
                            data.AmmoutNamGhep = Convert.ToInt32(gridViewBedServiceType.EditingValue);
                        }
                        cbo.Properties.Buttons[1].Visible = true;
                    }
                    else
                    {
                        if (data != null)
                        {
                            data.AmmoutNamGhep = null;
                        }
                        cbo.Properties.Buttons[1].Visible = false;
                    }
                    gridControlBedServiceType.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemCboBedServiceType_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    var cbo = sender as HIS.Desktop.Utilities.Extensions.CustomGridLookUpEdit;
                    cbo.Properties.Buttons[1].Visible = false;
                    var data = (ADO.HisBedHistoryADO)gridViewBedHistory.GetFocusedRow();
                    if (data != null && !data.HasServiceReq)
                    {
                        data.BED_SERVICE_TYPE_CODE = "";
                        data.BED_TYPE_ID = 0;
                    }
                    gridViewBedHistory.EditingValue = null;
                    gridControlBedHistory.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtnRemove_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                var deleteVhisBedLog = (MOS.EFMODEL.DataModels.V_HIS_BED_LOG)gridViewBedHistory.GetFocusedRow();
                if (deleteVhisBedLog != null && DevExpress.XtraEditors.XtraMessageBox.Show(
                    ResourceMessage.HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong,
                    ResourceMessage.ThongBao,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (deleteVhisBedLog.ID > 0)
                    {
                        var success = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>(Base.GlobalStore.HIS_BED_LOG_DELETE, ApiConsumer.ApiConsumers.MosConsumer, deleteVhisBedLog.ID, param);
                        if (success == true)
                        {
                            success = true;
                        }

                        #region Show message
                        Inventec.Desktop.Common.Message.MessageManager.Show(this, param, success);
                        #endregion
                    }
                    gridControlBedHistory.BeginUpdate();
                    gridViewBedHistory.DeleteRow(gridViewBedHistory.FocusedRowHandle);
                    gridControlBedHistory.RefreshDataSource();
                    gridControlBedHistory.EndUpdate();

                    #region Process has exception
                    HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewBedHistory_CustomRowColumnError(object sender, Inventec.Desktop.CustomControl.RowColumnErrorEventArgs e)
        {
            try
            {
                var index = this.gridViewBedHistory.GetDataSourceRowIndex(e.RowHandle);
                if (index < 0)
                {
                    e.Info.ErrorType = ErrorType.None;
                    e.Info.ErrorText = "";
                    return;
                }
                var listDatas = this.gridViewBedHistory.DataSource as List<ADO.HisBedHistoryADO>;
                var row = listDatas[index];
                if (e.ColumnName == "BED_ID")
                {
                    if (row.ErrorTypeBedId == ErrorType.Warning)
                    {
                        e.Info.ErrorType = (ErrorType)(row.ErrorTypeBedId);
                        e.Info.ErrorText = (string)(row.ErrorMessageBedId);
                    }
                    else
                    {
                        e.Info.ErrorType = (ErrorType)(ErrorType.None);
                        e.Info.ErrorText = "";
                    }
                }
                else if (e.ColumnName == "finishTime")
                {
                    if (row.ErrorTypeFinishTime == ErrorType.Warning)
                    {
                        e.Info.ErrorType = (ErrorType)(row.ErrorTypeFinishTime);
                        e.Info.ErrorText = (string)(row.ErrorMessageFinishTime);
                    }
                    else
                    {
                        e.Info.ErrorType = (ErrorType)(ErrorType.None);
                        e.Info.ErrorText = "";
                    }
                }
                else if (e.ColumnName == "startTime")
                {
                    if (row.ErrorTypeStartTime == ErrorType.Warning)
                    {
                        e.Info.ErrorType = (ErrorType)(row.ErrorTypeStartTime);
                        e.Info.ErrorText = (string)(row.ErrorMessageStartTime);
                    }
                    else
                    {
                        e.Info.ErrorType = (ErrorType)(ErrorType.None);
                        e.Info.ErrorText = "";
                    }
                }
                else if (e.ColumnName == "BED_SERVICE_TYPE_ID")
                {
                    if (row.ErrorTypeBebServiceTypeId == ErrorType.Warning)
                    {
                        e.Info.ErrorType = (ErrorType)(row.ErrorTypeBebServiceTypeId);
                        e.Info.ErrorText = (string)(row.ErrorMessageBebServiceTypeId);
                    }
                    else
                    {
                        e.Info.ErrorType = (ErrorType)(ErrorType.None);
                        e.Info.ErrorText = "";
                    }
                }
                else if (e.ColumnName == "PRIMARY_PATIENT_TYPE_ID")
                {
                    if (row.ErrorTypePrimaryPatientTypeId == ErrorType.Warning)
                    {
                        e.Info.ErrorType = (ErrorType)(row.ErrorTypePrimaryPatientTypeId);
                        e.Info.ErrorText = (string)(row.ErrorMessagePrimaryPatientTypeId);
                    }
                    else
                    {
                        e.Info.ErrorType = (ErrorType)(ErrorType.None);
                        e.Info.ErrorText = "";
                    }
                }
                else if (e.ColumnName == "PATIENT_TYPE_ID")
                {
                    if (row.ErrorTypePatientTypeId == ErrorType.Warning)
                    {
                        e.Info.ErrorType = (ErrorType)(row.ErrorTypePatientTypeId);
                        e.Info.ErrorText = (string)(row.ErrorMessagePatientTypeId);
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

        private void SaveAllBedLog(bool isClose = false)
        {
            try
            {
                if (!IsDisable)
                {
                    var bedLog = bedLogChecks.Where(o => !o.Error && o.startTime != DateTime.MinValue && o.startTime != DateTime.MaxValue
                        && o.BED_SERVICE_TYPE_ID != null && !o.HasServiceReq).ToList();
                    foreach (var item in bedLog)
                    {
                        if (!item.IsSave)
                        {
                            bool success = false;
                            CommonParam param = new CommonParam();
                            SaveDataBedLog(item, ref success, ref param, isClose);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FormBedHistory_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (refesh != null)
                    refesh();
                var bedLogCheck = bedLogChecks.Where(o => !o.HasServiceReq).ToList();
                foreach (var item in bedLogCheck)
                {
                    CheckErrorDataBedLog(item);
                }
                SaveAllBedLog(true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemCboBedCode_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
            {
                var cbo = sender as HIS.Desktop.Utilities.Extensions.CustomGridLookUpEdit;
                if (cbo.EditValue != null)
                {
                    var row = (ADO.HisBedHistoryADO)gridViewBedHistory.GetFocusedRow();
                    if (cbo.EditValue != cbo.OldEditValue)
                    {
                        row.IsSave = false;
                    }
                    long bedcodeId = (long)cbo.EditValue;
                    var dataBed = this.dataBedADOs.FirstOrDefault(p => p.BED_CODE_ID == bedcodeId);
                    //var listSelectedBed = gridControlBedHistory.DataSource;
                    if (dataBed != null)
                    {
                        var shareCountdf = row.SHARE_COUNT;
                        row.SHARE_COUNT = null;

                        if (dataBed.IsKey == 1)
                        {
                            if (_TreatmentBedRoom != null)
                            {
                                var treatmentTYpe = BackendDataWorker.Get<HIS_TREATMENT_TYPE>().FirstOrDefault(o => o.ID == _TreatmentBedRoom.TDL_TREATMENT_TYPE_ID);
                                if (treatmentTYpe != null)
                                {
                                    if (treatmentTYpe.IS_NOT_ALLOW_SHARE_BED == 1)
                                    {
                                        DevExpress.XtraEditors.XtraMessageBox.Show(string.Format(ResourceMessage.KhongDuocNamGhepGiuong, treatmentTYpe.TREATMENT_TYPE_NAME), ResourceMessage.ThongBao, MessageBoxButtons.OK);
                                        cbo.EditValue = null;
                                        cbo.ShowPopup();
                                        return;
                                    }
                                    else
                                    {
                                        HisTreatmentBedRoomFilter bedRoomFilter = new HisTreatmentBedRoomFilter();
                                        bedRoomFilter.IDs = dataBed.TREATMENT_BED_ROOM_IDs;
                                        bedRoomFilter.IS_ACTIVE = 1;

                                        var bedRooms = new BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT_BED_ROOM>>("api/HisTreatmentBedRoom/GetView", ApiConsumer.ApiConsumers.MosConsumer, bedRoomFilter, null);
                                        List<string> lstPatientName = new List<string>();
                                        if (bedRooms != null && bedRooms.Count > 0)
                                        {
                                            bedRooms = bedRooms.Where(o => o.TREATMENT_ID != _TreatmentBedRoom.TREATMENT_ID).ToList();
                                        }

                                        foreach (var item in bedRooms)
                                        {
                                            lstPatientName.Add(item.TREATMENT_CODE + " - " + item.TDL_PATIENT_NAME);
                                        }

                                        if (DevExpress.XtraEditors.XtraMessageBox.Show(string.Format("Giường đã có bệnh nhân {0} nằm. Bạn có muốn cho bệnh nhân nằm ghép không?", string.Join(", ", lstPatientName)), ResourceMessage.ThongBao, MessageBoxButtons.YesNo) == DialogResult.No)
                                        {
                                            cbo.EditValue = null;
                                            cbo.ShowPopup();
                                            return;
                                        }
                                        if (row.ID <= 0)
                                        {
                                            if (IsShareBed != "1")
                                            {
                                                row.SHARE_COUNT = dataBed.AMOUNT + 1;
                                            }
                                            else
                                            {
                                                row.SHARE_COUNT = GetMaxShareCount(dataBed, row);
                                            }
                                        }
                                        else
                                        {
                                            var bedId = row.BED_ID != 0 ? row.BED_ID : row.BedIdAfterSave;
                                            if (bedId != 0 && bedId != dataBed.ID && shareCountdf > 1 && IsShareBed == "1" && DevExpress.XtraEditors.XtraMessageBox.Show("Giường hiện tại có nằm ghép. Bạn có muốn cập nhật thông tin nằm ghép của bệnh nhân khác không?", ResourceMessage.ThongBao, MessageBoxButtons.YesNo) == DialogResult.Yes)
                                            {
                                                var dataBedOld = this.dataBedADOs.FirstOrDefault(p => p.BED_CODE_ID == bedId);
                                                var UpdateShareCountOld = UpdateShareCount(dataBedOld, row);
                                                Inventec.Common.Logging.LogSystem.Debug("UpdateShareCountOld: " + UpdateShareCountOld);
                                            }
                                            List<long> lstMax = new List<long>();
                                            var bedLogAllids = dataBed.BedLogAllIds;
                                            if (bedLogAllids != null && bedLogAllids.Count > 0)
                                            {
                                                lstMax = bedLogAllids.Where(o => o != row.ID).ToList();
                                            }
                                            if (lstMax != null && lstMax.Count > 0)
                                                row.SHARE_COUNT = GetShareCount(lstMax);
                                        }
                                    }
                                }
                            }
                        }
                        else if (dataBed.IsKey == 2)
                        {
                            CommonParam param = new CommonParam();
                            HisTreatmentBedRoomViewFilter filterTreatmentBedRoom = new HisTreatmentBedRoomViewFilter();
                            filterTreatmentBedRoom.IDs = dataBed.TREATMENT_BED_ROOM_IDs;
                            filterTreatmentBedRoom.IS_ACTIVE = 1;
                            lstHisTreatmentBedRoom = new BackendAdapter(param).Get<List<V_HIS_TREATMENT_BED_ROOM>>("api/HisTreatmentBedRoom/GetView", ApiConsumer.ApiConsumers.MosConsumer, filterTreatmentBedRoom, param);
                            List<string> lstPatientName = new List<string>();

                            if (lstHisTreatmentBedRoom != null && lstHisTreatmentBedRoom.Count > 0)
                            {
                                lstHisTreatmentBedRoom = lstHisTreatmentBedRoom.Where(o => !bedLogChecks.Select(s => s.TREATMENT_ID).Contains(o.TREATMENT_ID)).ToList();
                            }
                            foreach (var item in lstHisTreatmentBedRoom)
                            {
                                lstPatientName.Add(item.TREATMENT_CODE + " - " + item.TDL_PATIENT_NAME);
                            }
                            if (lstPatientName != null && lstPatientName.Count > 0)
                            {
                                DevExpress.XtraEditors.XtraMessageBox.Show(string.Format("Giường {0} vượt quá số lượng nằm ghép tối đa - Những bệnh nhân đang nằm : {1}", dataBed.BED_NAME, string.Join(", ", lstPatientName)), ResourceMessage.ThongBao);

                                cbo.EditValue = null;
                                cbo.ShowPopup();
                                return;
                            }

                        }
                        else
                        {
                            var bedId = row.BED_ID != 0 ? row.BED_ID : row.BedIdAfterSave;
                            if (bedId != 0 && bedId != dataBed.ID && IsShareBed == "1" && DevExpress.XtraEditors.XtraMessageBox.Show("Giường hiện tại có nằm ghép. Bạn có muốn cập nhật thông tin nằm ghép của bệnh nhân khác không?", ResourceMessage.ThongBao, MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                var dataBedOld = this.dataBedADOs.FirstOrDefault(p => p.BED_CODE_ID == bedId);
                                var UpdateShareCountOld = UpdateShareCount(dataBedOld, row);
                                Inventec.Common.Logging.LogSystem.Debug("UpdateShareCountOld: " + UpdateShareCountOld);
                            }
                        }
                    }

                    row.BED_ID = dataBed.ID;
                    row.BED_CODE = dataBed.BED_CODE;
                    row.BED_NAME = dataBed.BED_NAME;
                    row.BED_CODE_ID = dataBed.BED_CODE_ID;
                    row.BED_TYPE_CODE = dataBed.BED_TYPE_CODE;
                    row.BED_TYPE_ID = dataBed.BED_TYPE_ID;
                    row.BED_TYPE_NAME = dataBed.BED_TYPE_NAME;
                    row.IsBedStretcher = dataBed.IS_BED_STRETCHER == 1;
                    LoadDataToCboBedServiceType(row);
                }
            }
        }
        private long? GetMaxShareCount(HisBedADO dataBed, HisBedHistoryADO row)
        {
            long? max = null;
            try
            {
                List<long> lstMax = new List<long>();

                if (dataBed.BedLogStartIds != null && dataBed.BedLogStartIds.Count > 0)
                {
                    lstMax.AddRange(dataBed.BedLogStartIds);
                    //lstMax.Add(GetShareCount(dataBed.BedLogStartIds));
                    //if (dataBed.BedLogFinishIds != null && dataBed.BedLogFinishIds.Count > 0)
                    //    dataBed.BedLogFinishIds = dataBed.BedLogFinishIds.Where(o => !dataBed.BedLogStartIds.Exists(p => p == o)).ToList();
                }
                if (dataBed.BedLogFinishIds != null && dataBed.BedLogFinishIds.Count > 0)
                {
                    lstMax.AddRange(dataBed.BedLogFinishIds);
                    //lstMax.Add(GetShareCount(dataBed.BedLogFinishIds));
                }
                lstMax = lstMax.Distinct().ToList();
                if (lstMax != null && lstMax.Count > 0)
                    max = GetShareCount(lstMax);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return max;
        }

        private long? UpdateShareCount(HisBedADO dataBed, HisBedHistoryADO row)
        {
            long? max = null;
            try
            {
                List<long> lstMax = new List<long>();

                var bedLogAllids = dataBed.BedLogAllIds;
                if (bedLogAllids != null && bedLogAllids.Count > 0)
                {
                    lstMax = bedLogAllids.Where(o => o != row.ID).ToList();
                }
                if (lstMax != null && lstMax.Count > 0)
                    max = RemoveShareCount(lstMax);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return max;
        }

        private long? GetShareCount(List<long> bedLogids)
        {
            CommonParam param = new CommonParam();
            UpdateShareCountSDO sdo = new UpdateShareCountSDO();
            sdo.ShareCount = bedLogids.Count + 1;
            sdo.BedLogIds = bedLogids;

            Inventec.Common.Logging.LogSystem.Debug("__INPUT GetShareCount:___" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sdo), sdo));
            var currentBedServiceReq = new Inventec.Common.Adapter.BackendAdapter(param).Post<List<HIS_BED_LOG>>("api/HisBedLog/UpdateShareCount", ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
            if (currentBedServiceReq != null && currentBedServiceReq.Count > 0)
            {
                return currentBedServiceReq.Max(o => o.SHARE_COUNT);
            }
            return null;
        }

        private long? RemoveShareCount(List<long> bedLogids)
        {
            if (bedLogids == null || bedLogids.Count == 0)
                return null;
            CommonParam param = new CommonParam();
            UpdateShareCountSDO sdo = new UpdateShareCountSDO();
            sdo.ShareCount = bedLogids.Count;
            sdo.BedLogIds = bedLogids;
            if (sdo.ShareCount == 0)
                sdo.ShareCount = null;
            Inventec.Common.Logging.LogSystem.Debug("__INPUT RemoveShareCount:___" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sdo), sdo));
            var currentBedServiceReq = new Inventec.Common.Adapter.BackendAdapter(param).Post<List<HIS_BED_LOG>>("api/HisBedLog/UpdateShareCount", ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
            if (currentBedServiceReq != null && currentBedServiceReq.Count > 0)
            {
                return currentBedServiceReq.Max(o => o.SHARE_COUNT);
            }
            return null;
        }

        private void repositoryItemCboBedCode_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    var cbo = sender as HIS.Desktop.Utilities.Extensions.CustomGridLookUpEdit;
                    cbo.Properties.Buttons[1].Visible = false;
                    var data = (ADO.HisBedHistoryADO)gridViewBedHistory.GetFocusedRow();
                    if (data != null && !data.HasServiceReq)
                    {
                        data.BED_CODE = null;
                        data.BED_CODE_ID = 0;
                        data.BED_ID = 0;
                        data.BED_TYPE_NAME = null;
                        data.BED_SERVICE_TYPE_CODE = null;
                        data.BED_SERVICE_TYPE_ID = null;
                        data.PATIENT_TYPE_ID = null;
                        data.PRIMARY_PATIENT_TYPE_ID = null;
                        data.SHARE_COUNT = null;
                    }
                    gridViewBedHistory.EditingValue = null;
                    gridControlBedHistory.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CheckEditDisable_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                var row = (ADO.HisBedHistoryADO)gridViewBedHistory.GetFocusedRow();

                if (row != null)
                {
                    gridViewBedHistory.BeginUpdate();
                    row.IsChecked = false;
                    gridViewBedHistory.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                gridViewBedHistory.EndUpdate();
            }
        }

        private void gridViewBedHistory_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                gridViewBedHistory.BeginUpdate();
                var row = (ADO.HisBedHistoryADO)gridViewBedHistory.GetFocusedRow();

                if (row != null)
                {
                    row.IsChecked = !row.IsChecked;
                    if (row.HasServiceReq || row.ID == 0)
                    {
                        row.IsChecked = false;
                    }
                }

                gridViewBedHistory.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                gridViewBedHistory.EndUpdate();
            }
        }

        private void repositoryItemCboBedPatientType_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                DevExpress.XtraEditors.GridLookUpEdit edit = sender as DevExpress.XtraEditors.GridLookUpEdit;
                if (edit == null) return;
                if (edit.EditValue != null && edit.EditValue != edit.OldEditValue)
                {
                    var row = (HisBedHistoryADO)gridViewBedHistory.GetFocusedRow();
                    if (row != null)
                    {
                        LoadAppliedPatientType((long)edit.EditValue, row.BED_SERVICE_TYPE_ID ?? 0, ref row);
                        if (Base.GlobalStore.IsPrimaryPatientType == commonString__true
                            && row.BILL_PATIENT_TYPE_ID.HasValue
                            && ((List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>)(edit.Properties.DataSource)).Exists(o => o.ID == row.BILL_PATIENT_TYPE_ID))
                        {
                            if (row.IsContainAppliedPatientType)
                            {
                                row.PATIENT_TYPE_ID = (long)edit.EditValue;
                                row.IS_NOT_CHANGE_BILL_PATY = false;
                                row.PRIMARY_PATIENT_TYPE_ID = row.BILL_PATIENT_TYPE_ID;
                                return;
                            }
                            else
                            {
                                row.IS_NOT_CHANGE_BILL_PATY = row.IS_NOT_CHANGE_BILL_PATY_DEFAULT;
                                //return;
                            }
                        }
                        //else
                        {
                            var pt = Base.GlobalStore.HisPatientTypes.FirstOrDefault(o => (o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((edit.EditValue ?? "").ToString()) || o.PATIENT_TYPE_NAME == (edit.EditValue ?? "").ToString()));
                            if (pt != null)
                            {
                                var dataBhyt = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HisConfigKeys.HIS_CONFIG__PATIENT_TYPE_CODE__BHYT));
                                if (pt.ID == (dataBhyt != null ? dataBhyt.ID : 0))
                                {
                                    gridViewBedHistory.SetRowCellValue(gridViewBedHistory.FocusedRowHandle, gridColumnIsKH__TabService.FieldName, false);
                                }
                                gridViewBedHistory.SetRowCellValue(gridViewBedHistory.FocusedRowHandle, Gv_BedHistory__Gc_PatientType, pt.ID);
                                row.PATIENT_TYPE_ID = pt.ID;
                                long instructionTime = row.START_TIME > 0 ? row.START_TIME : (Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(row.startTime) ?? 0);
                                long treatmentTime = this.CurrentTreatment.IN_TIME;
                                List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> currentPatientTypeTemps = new List<HIS_PATIENT_TYPE>();
                                var patientType = currentPatientTypeWithPatientTypeAlter.Where(o => o.IS_ACTIVE == 1).ToList();
                                foreach (var item in patientType)
                                {
                                    var itemPaty = MOS.ServicePaty.ServicePatyUtil.GetApplied(Base.GlobalStore.HisVServicePatys, WorkPlaceSDO.BranchId, this.WorkPlaceSDO.RoomId, this.WorkPlaceSDO.RoomId, this.WorkPlaceSDO.DepartmentId, instructionTime, this.CurrentTreatment.IN_TIME, row.BED_SERVICE_TYPE_ID ?? 0, item.ID, null, 1, null, row.SERVICE_CONDITION_ID);
                                    if (itemPaty != null)
                                    {
                                        currentPatientTypeTemps.Add(item);
                                    }
                                }
                                if (Base.GlobalStore.IsPrimaryPatientType == "1"
                                    && row.BILL_PATIENT_TYPE_ID.HasValue
                                    && row.PATIENT_TYPE_ID != row.BILL_PATIENT_TYPE_ID.Value
                                    && row.IsContainAppliedPatientType
                                )
                                {
                                    if (currentPatientTypeTemps.Exists(o => o.ID == row.BILL_PATIENT_TYPE_ID.Value))
                                    {
                                        var priPaty = currentPatientTypeTemps.FirstOrDefault(o => o.ID == row.BILL_PATIENT_TYPE_ID.Value);
                                        row.PRIMARY_PATIENT_TYPE_ID = priPaty.ID;
                                    }
                                }
                                else
                                {
                                    row.PRIMARY_PATIENT_TYPE_ID = null;
                                }
                            }
                        }
                        var PatientTypeId = row.PATIENT_TYPE_ID ?? 0;
                        var PrimaryPatientTypeId = row.PRIMARY_PATIENT_TYPE_ID ?? 0;
                        if ((PatientTypeId > 0 && PrimaryPatientTypeId > 0 && PatientTypeId == PrimaryPatientTypeId) || PrimaryPatientTypeId == 0)
                        {
                            row.PRIMARY_PATIENT_TYPE_ID = null;
                            //gridViewBedHistory.SetRowCellValue(gridViewBedHistory.FocusedRowHandle, Gv_BedHistory__Gc_PrimaryPatientTypeId, null);
                        }
                        //else if (PrimaryPatientTypeId > 0)
                        //{

                        //    gridViewBedHistory.SetRowCellValue(gridViewBedHistory.FocusedRowHandle, Gv_BedHistory__Gc_PrimaryPatientTypeId, PrimaryPatientTypeId);
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemCboBedPrimaryPatientType_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                DevExpress.XtraEditors.GridLookUpEdit edit = sender as DevExpress.XtraEditors.GridLookUpEdit;
                if (edit == null) return;
                if (edit.EditValue != null)
                {
                    var row = (HisBedHistoryADO)gridViewBedHistory.GetFocusedRow();

                    if (row != null)
                    {

                        if (Base.GlobalStore.IsPrimaryPatientType == "2" && this.CurrentTreatment.PRIMARY_PATIENT_TYPE_ID > 0 && row.PRIMARY_PATIENT_TYPE_ID == this.CurrentTreatment.PRIMARY_PATIENT_TYPE_ID)
                        {
                            row.PRIMARY_PATIENT_TYPE_ID = this.CurrentTreatment.PRIMARY_PATIENT_TYPE_ID;
                            var billPaty = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == row.PRIMARY_PATIENT_TYPE_ID);
                            string patyName = billPaty != null ? billPaty.PATIENT_TYPE_NAME : "";
                            MessageManager.Show(String.Format(ResourceMessage.DichVuCoDTPTBatBuocKhongChoPhepSua, row.SERVICE_NAME, patyName));
                            return;
                        }
                        else
                        {
                            var pt = Base.GlobalStore.HisPatientTypes.FirstOrDefault(o => (o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((edit.EditValue ?? "").ToString()) || o.PATIENT_TYPE_NAME == (edit.EditValue ?? "").ToString()));
                            if (pt != null)
                            {
                                var dataBhyt = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HisConfigKeys.HIS_CONFIG__PATIENT_TYPE_CODE__BHYT));

                                gridViewBedHistory.SetRowCellValue(gridViewBedHistory.FocusedRowHandle, Gv_BedHistory__Gc_PrimaryPatientTypeId, pt.ID);
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

        private void repositoryItemCboBedPrimaryPatientType_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    var row = (HisBedHistoryADO)gridViewBedHistory.GetFocusedRow();
                    if (row != null)
                    {
                        if (Base.GlobalStore.IsPrimaryPatientType == commonString__true
                           && row.BILL_PATIENT_TYPE_ID.HasValue
                           && row.PATIENT_TYPE_ID != row.BILL_PATIENT_TYPE_ID
                           && row.IS_NOT_CHANGE_BILL_PATY_DEFAULT)
                        {
                            var billPaty = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == row.BILL_PATIENT_TYPE_ID.Value);
                            string patyName = billPaty != null ? billPaty.PATIENT_TYPE_NAME : "";
                            MessageManager.Show(String.Format(ResourceMessage.DichVuCoDTTTBatBuocKhongChoPhepSua, row.BED_NAME, patyName));
                            row.PRIMARY_PATIENT_TYPE_ID = row.BILL_PATIENT_TYPE_ID;
                        }
                        else if (Base.GlobalStore.IsPrimaryPatientType == "2" && this.CurrentTreatment.PRIMARY_PATIENT_TYPE_ID > 0)
                        {
                            row.PRIMARY_PATIENT_TYPE_ID = this.CurrentTreatment.PRIMARY_PATIENT_TYPE_ID;
                            var billPaty = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == row.PRIMARY_PATIENT_TYPE_ID);
                            string patyName = billPaty != null ? billPaty.PATIENT_TYPE_NAME : "";
                            MessageManager.Show(String.Format(ResourceMessage.DichVuCoDTPTBatBuocKhongChoPhepSua, row.SERVICE_NAME, patyName));
                        }
                        else
                        {
                            row.PRIMARY_PATIENT_TYPE_ID = null;
                            gridControlBedHistory.RefreshDataSource();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Event Bed Service Type
        private void gridViewBedServiceType_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                setDefaultMedicineMaterialTotalPrice();

                var ado = (ADO.HisBedServiceTypeADO)this.gridViewBedServiceType.GetFocusedRow();
                if (ado != null)
                {
                    if (e.Column.FieldName == "PATIENT_TYPE_ID")
                    {
                        ado.OTHER_PAY_SOURCE_ID = ProcessAutoSetOtherPaySource(ado);
                        ado.HasConfigOtherSourcePay = false;
                        var paty = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == ado.PATIENT_TYPE_ID);
                        if (paty != null && !String.IsNullOrWhiteSpace(paty.OTHER_PAY_SOURCE_IDS))
                        {
                            ado.HasConfigOtherSourcePay = true;
                        }
                        //if (Base.GlobalStore.IsPrimaryPatientType == "2")
                        {
                            if (ado.PATIENT_TYPE_ID == ado.PRIMARY_PATIENT_TYPE_ID)
                                ado.PRIMARY_PATIENT_TYPE_ID = null;
                        }
                    }
                    else if (e.Column.FieldName == "AmmoutNamGhep")
                    {
                        if (IsShareBed == "1" && ado.IsSplitDayOrResult)
                        {
                            CommonParam param = new CommonParam();
                            UpdateShareCountSDO sdo = new UpdateShareCountSDO();
                            sdo.ShareCount = (ado.AmmoutNamGhep ?? 0);
                            sdo.BedLogIds = new List<long>() { ado.ID };
                            var currentBedServiceReq = new Inventec.Common.Adapter.BackendAdapter(param).Post<List<HIS_BED_LOG>>("api/HisBedLog/UpdateShareCount", ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                            if (currentBedServiceReq != null && currentBedServiceReq.Count > 0)
                            {
                                ado.AmmoutNamGhep = currentBedServiceReq.Sum(o => o.SHARE_COUNT);
                            }
                        }
                        else if (!ado.IsSplitDayOrResult)
                            ado.AmmoutNamGhep = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void setDefaultMedicineMaterialTotalPrice()
        {
            try
            {
                totalPrice = 0;
                List<ADO.HisBedServiceTypeADO> datas = (List<ADO.HisBedServiceTypeADO>)gridViewBedServiceType.DataSource;

                if (datas != null && datas.Count > 0)
                {
                    datas = datas.Where(p => p.PATIENT_TYPE_ID != 0 && p.IsExpend == false && CheckServicePaty(p)).ToList();
                    foreach (var item in datas)
                    {
                        item.START_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(item.IntructionTime) ?? 0;
                    }

                    lblTotalServicePrice.Text = Inventec.Common.Number.Convert.NumberToString(totalPrice, ConfigApplications.NumberSeperator);
                }
                else
                {
                    lblTotalServicePrice.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        decimal totalPrice = 0;
        private bool CheckServicePaty(ADO.HisBedServiceTypeADO data)
        {
            bool result = false;
            try
            {
                long instructionTime = data.START_TIME;
                //Lay thong tin chinh sach gia duoc ap dung cho sere_serv
                V_HIS_SERVICE_PATY appliedServicePaty = ServicePatyUtil.GetApplied(Base.GlobalStore.HisVServicePatys, WorkPlaceSDO.BranchId, WorkPlaceSDO.RoomId, WorkPlaceSDO.RoomId, WorkPlaceSDO.DepartmentId, instructionTime, _TreatmentBedRoom.CLINICAL_IN_TIME ?? 0, data.BED_SERVICE_TYPE_ID ?? 0, data.PATIENT_TYPE_ID ?? 0, null, null, null, null);

                //Lay thong tin chinh sach gia duoc ap dung cho sere_serv
                V_HIS_SERVICE_PATY primaryServicePaty = ServicePatyUtil.GetApplied(Base.GlobalStore.HisVServicePatys, WorkPlaceSDO.BranchId, WorkPlaceSDO.RoomId, WorkPlaceSDO.RoomId, WorkPlaceSDO.DepartmentId, instructionTime, _TreatmentBedRoom.CLINICAL_IN_TIME ?? 0, data.BED_SERVICE_TYPE_ID ?? 0, data.PRIMARY_PATIENT_TYPE_ID ?? 0, null, null, null, null);

                if (appliedServicePaty != null)
                {
                    //ko nam ghep tinh 100%
                    //ghep 2 tinh 50%
                    //ghep 3 tro len tinh 30%
                    decimal ratio = 1m;
                    if (data.SHARE_COUNT.HasValue)
                    {
                        if (data.SHARE_COUNT.Value > 2)
                        {
                            ratio = 0.3m;
                        }
                        else if (data.SHARE_COUNT.Value == 2)
                        {
                            ratio = 0.5m;
                        }
                    }

                    if (data.SHARE_COUNT.HasValue)
                    {
                        totalPrice += (data.AMOUNT) * (appliedServicePaty.PRICE * ratio);
                    }
                    else
                    {
                        //có phụ thu thì tính theo giá phụ thu
                        if (primaryServicePaty != null)
                        {
                            totalPrice += (data.AMOUNT) * (primaryServicePaty.PRICE * (1 + primaryServicePaty.VAT_RATIO));
                        }
                        else // ko thì theo giá thường
                        {
                            totalPrice += (data.AMOUNT) * (appliedServicePaty.PRICE * (1 + appliedServicePaty.VAT_RATIO));
                        }
                    }
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void gridViewBedServiceType_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                HisBedServiceTypeADO data = (HisBedServiceTypeADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                if (e.Column.FieldName == "PATIENT_TYPE_ID")
                {
                    e.RepositoryItem = repositoryItemCboPatientType;
                }
                else if (e.Column.FieldName == "PRIMARY_PATIENT_TYPE_ID")
                {
                    e.RepositoryItem = repositoryItemCboPrimaryPatientType;
                }
                else if (e.Column.FieldName == "OTHER_PAY_SOURCE_ID")
                {
                    e.RepositoryItem = repositoryItemCboOtherPaySource;
                }
                else if (e.Column.FieldName == "REQUEST_LOGINNAME")
                {
                    e.RepositoryItem = repositoryItemCboRequestUser;
                }
                else if (e.Column.FieldName == "AmmoutNamGhep")
                {
                    if (data.IsSplitDayOrResult)
                        e.RepositoryItem = repositoryItemSpinAmount;
                    else
                        e.RepositoryItem = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewBedServiceType_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    else if (e.Column.FieldName == "PRICE_DISPLAY")
                    {
                        ADO.HisBedServiceTypeADO data = (ADO.HisBedServiceTypeADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                        if (data != null && data.PATIENT_TYPE_ID > 0)
                        {
                            V_HIS_SERVICE_PATY data_ServicePrice = new V_HIS_SERVICE_PATY();
                            if (data.PRIMARY_PATIENT_TYPE_ID.HasValue)
                            {
                                data_ServicePrice = ServicePatyUtil.GetApplied(Base.GlobalStore.HisVServicePatys, WorkPlaceSDO.BranchId, WorkPlaceSDO.RoomId, WorkPlaceSDO.RoomId, WorkPlaceSDO.DepartmentId, data.START_TIME, CurrentTreatment.IN_TIME, data.BED_SERVICE_TYPE_ID ?? 0, data.PRIMARY_PATIENT_TYPE_ID ?? 0, null, null, null, data.SERVICE_CONDITION_ID);
                            }
                            else
                            {
                                data_ServicePrice = ServicePatyUtil.GetApplied(Base.GlobalStore.HisVServicePatys, WorkPlaceSDO.BranchId, WorkPlaceSDO.RoomId, WorkPlaceSDO.RoomId, WorkPlaceSDO.DepartmentId, data.START_TIME, CurrentTreatment.IN_TIME, data.BED_SERVICE_TYPE_ID ?? 0, data.PATIENT_TYPE_ID ?? 0, null, null, null, data.SERVICE_CONDITION_ID);
                            }

                            if (data_ServicePrice != null)
                            {
                                e.Value = Inventec.Common.Number.Convert.NumberToString((data_ServicePrice.PRICE * (1 + data_ServicePrice.VAT_RATIO)), ConfigApplications.NumberSeperator);
                            }
                            else
                            {
                                e.Value = 0;
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

        private void gridViewBedServiceType_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);
                    if (hi.InRowCell)
                    {
                        if (hi.Column.RealColumnEdit.GetType() == typeof(DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit))
                        {
                            view.FocusedRowHandle = hi.RowHandle;
                            view.FocusedColumn = hi.Column;
                            view.ShowEditor();
                            DevExpress.XtraEditors.CheckEdit checkEdit = view.ActiveEditor as DevExpress.XtraEditors.CheckEdit;
                            DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo checkInfo = (DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo)checkEdit.GetViewInfo();
                            Rectangle glyphRect = checkInfo.CheckInfo.GlyphRect;
                            GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                            Rectangle gridGlyphRect =
                                new Rectangle(viewInfo.GetGridCellInfo(hi).Bounds.X + glyphRect.X,
                                 viewInfo.GetGridCellInfo(hi).Bounds.Y + glyphRect.Y,
                                 glyphRect.Width,
                                 glyphRect.Height);
                            if (!gridGlyphRect.Contains(e.Location))
                            {
                                view.CloseEditor();
                                if (!view.IsCellSelected(hi.RowHandle, hi.Column))
                                {
                                    view.SelectCell(hi.RowHandle, hi.Column);
                                }
                                else
                                {
                                    view.UnselectCell(hi.RowHandle, hi.Column);
                                }
                            }
                            else
                            {
                                checkEdit.Checked = !checkEdit.Checked;
                                view.CloseEditor();
                            }
                            (e as DevExpress.Utils.DXMouseEventArgs).Handled = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemCboPatientType_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                DevExpress.XtraEditors.GridLookUpEdit edit = sender as DevExpress.XtraEditors.GridLookUpEdit;
                if (edit == null) return;
                if (edit.EditValue != null)
                {
                    var pt = Base.GlobalStore.HisPatientTypes.FirstOrDefault(o => (o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((edit.EditValue ?? "").ToString()) || o.PATIENT_TYPE_NAME == (edit.EditValue ?? "").ToString()));
                    if (pt != null)
                    {
                        var dataBhyt = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HisConfigKeys.HIS_CONFIG__PATIENT_TYPE_CODE__BHYT));
                        if (pt.ID == (dataBhyt != null ? dataBhyt.ID : 0))
                        {
                            gridViewBedServiceType.SetRowCellValue(gridViewBedServiceType.FocusedRowHandle, gridColumnIsKH__TabService.FieldName, false);
                        }

                        gridViewBedServiceType.SetRowCellValue(gridViewBedServiceType.FocusedRowHandle, gridColumnPatientTypeCode__TabService, pt.PATIENT_TYPE_CODE);
                        gridViewBedServiceType.SetRowCellValue(gridViewBedServiceType.FocusedRowHandle, Gv_BedServiceType__Gc_PatientTypeName, pt.ID);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemCboPrimaryPatientType_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                DevExpress.XtraEditors.GridLookUpEdit edit = sender as DevExpress.XtraEditors.GridLookUpEdit;
                if (edit == null) return;
                if (edit.EditValue != null)
                {
                    var row = (HisBedServiceTypeADO)gridViewBedServiceType.GetFocusedRow();
                    if (row != null)
                    {
                        if (Base.GlobalStore.IsPrimaryPatientType == "2" && this.CurrentTreatment.PRIMARY_PATIENT_TYPE_ID > 0 && row.PRIMARY_PATIENT_TYPE_ID == this.CurrentTreatment.PRIMARY_PATIENT_TYPE_ID)
                        {
                            row.PRIMARY_PATIENT_TYPE_ID = this.CurrentTreatment.PRIMARY_PATIENT_TYPE_ID;
                            var billPaty = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == row.PRIMARY_PATIENT_TYPE_ID);
                            string patyName = billPaty != null ? billPaty.PATIENT_TYPE_NAME : "";
                            MessageManager.Show(String.Format(ResourceMessage.DichVuCoDTPTBatBuocKhongChoPhepSua, row.SERVICE_NAME, patyName));
                            return;
                        }
                        else
                        {
                            var pt = Base.GlobalStore.HisPatientTypes.FirstOrDefault(o => (o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((edit.EditValue ?? "").ToString()) || o.PATIENT_TYPE_NAME == (edit.EditValue ?? "").ToString()));
                            if (pt != null)
                            {
                                var dataBhyt = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HisConfigKeys.HIS_CONFIG__PATIENT_TYPE_CODE__BHYT));
                                if (pt.ID == (dataBhyt != null ? dataBhyt.ID : 0))
                                {
                                    gridViewBedServiceType.SetRowCellValue(gridViewBedServiceType.FocusedRowHandle, gridColumnIsKH__TabService.FieldName, false);
                                }
                                gridViewBedServiceType.SetRowCellValue(gridViewBedServiceType.FocusedRowHandle, Gv_BedServiceType__Gc_PrimaryPatientType, pt.ID);
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

        private void repositoryItemCboNamGhep_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    repositoryItemCboNamGhep.Properties.Buttons[1].Visible = false;
                    var data = (ADO.HisBedServiceTypeADO)gridViewBedServiceType.GetFocusedRow();
                    if (data != null)
                    {
                        data.AmmoutNamGhep = null;
                    }
                    gridViewBedServiceType.EditingValue = null;
                    gridControlBedServiceType.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemCboNamGhep_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    var data = (ADO.HisBedServiceTypeADO)gridViewBedServiceType.GetFocusedRow();
                    if (gridViewBedServiceType.EditingValue != null)
                    {
                        if (data != null)
                        {
                            data.AmmoutNamGhep = Convert.ToInt32(gridViewBedServiceType.EditingValue);
                        }
                        repositoryItemCboNamGhep.Properties.Buttons[1].Visible = true;
                    }
                    else
                    {
                        if (data != null)
                        {
                            data.AmmoutNamGhep = null;
                        }
                        repositoryItemCboNamGhep.Properties.Buttons[1].Visible = false;
                    }
                    gridControlBedServiceType.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemCboPrimaryPatientType_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    var row = (HisBedServiceTypeADO)gridViewBedServiceType.GetFocusedRow();
                    if (row != null)
                    {
                        if (Base.GlobalStore.IsPrimaryPatientType == "2" && this.CurrentTreatment.PRIMARY_PATIENT_TYPE_ID > 0 && row.PRIMARY_PATIENT_TYPE_ID == this.CurrentTreatment.PRIMARY_PATIENT_TYPE_ID)
                        {
                            var billPaty = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == row.PRIMARY_PATIENT_TYPE_ID);
                            string patyName = billPaty != null ? billPaty.PATIENT_TYPE_NAME : "";
                            MessageManager.Show(String.Format(ResourceMessage.DichVuCoDTPTBatBuocKhongChoPhepSua, row.SERVICE_NAME, patyName));
                            return;
                        }
                        else
                        {
                            row.PRIMARY_PATIENT_TYPE_ID = null;
                            gridControlBedServiceType.RefreshDataSource();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewBedServiceType_ShownEditor(object sender, EventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                HisBedServiceTypeADO data = view.GetFocusedRow() as HisBedServiceTypeADO;
                if (view.FocusedColumn.FieldName == "PRIMARY_PATIENT_TYPE_ID" && view.ActiveEditor is GridLookUpEdit)
                {
                    GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;
                    if (data != null)
                    {
                        if (Base.GlobalStore.IsPrimaryPatientType == "2" && this.CurrentTreatment.PRIMARY_PATIENT_TYPE_ID == data.PRIMARY_PATIENT_TYPE_ID)
                        {
                            editor.ReadOnly = true;
                        }
                        else
                        {
                            FillDataIntoPrimaryPatientTypeCombo(data, editor);
                            editor.EditValue = data.PRIMARY_PATIENT_TYPE_ID;
                        }
                    }
                    else
                    {
                        editor.ReadOnly = true;
                    }
                }
                else if (view.FocusedColumn.FieldName == "PATIENT_TYPE_ID" && view.ActiveEditor is GridLookUpEdit)
                {
                    GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;
                    if (data != null && !data.BILL_PATIENT_TYPE_ID.HasValue)
                    {
                        this.FillDataIntoPatientTypeCombo(data, editor);
                        editor.EditValue = data.PATIENT_TYPE_ID;
                    }
                }
                else if (view.FocusedColumn.FieldName == "OTHER_PAY_SOURCE_ID" && view.ActiveEditor is GridLookUpEdit)
                {
                    GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;
                    if (data != null)
                    {
                        this.FillDataIntoOtherPaySourceCombo(data, editor);
                        editor.EditValue = data.OTHER_PAY_SOURCE_ID;
                    }
                }
                else if (view.FocusedColumn.FieldName == "REQUEST_LOGINNAME" && view.ActiveEditor is Inventec.Desktop.CustomControl.CustomGridLookUpEditWithFilterMultiColumn)
                {
                    Inventec.Desktop.CustomControl.CustomGridLookUpEditWithFilterMultiColumn editor = view.ActiveEditor as Inventec.Desktop.CustomControl.CustomGridLookUpEditWithFilterMultiColumn;
                    editor.Properties.AutoComplete = false;
                    if (Base.GlobalStore.ShowRequestUser == "1")
                    {
                        if (data != null)
                        {
                            this.InitCboRequestUser(editor);
                            editor.EditValue = data.REQUEST_LOGINNAME;
                        }
                    }
                    else
                        editor.ReadOnly = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Event Bed Service Req
        private void gridViewBedServiceReq_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    HIS_SERE_SERV data = (HIS_SERE_SERV)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "INTRUCTION_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.TDL_INTRUCTION_TIME);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewBedServiceReq_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    HIS_SERE_SERV data = (HIS_SERE_SERV)((IList)((BaseView)sender).DataSource)[e.RowHandle];

                    if (e.Column.FieldName == "SERVICE_REQ_DELETE")
                    {
                        long reqId = Inventec.Common.TypeConvert.Parse.ToInt64((gridViewBedServiceReq.GetRowCellValue(e.RowHandle, "SERVICE_REQ_ID") ?? "0").ToString().Trim());
                        HIS_SERVICE_REQ req = new HIS_SERVICE_REQ();

                        if (reqId > 0)
                        {
                            req = ListServiceReqForSereServs.FirstOrDefault(o => o.ID == reqId);
                        }
                        if (IsDisable)
                        {
                            e.RepositoryItem = repositoryItemBtnDeleteServiceReqDisable;
                        }
                        else
                        {
                            if ((req.CREATOR == this.loginName || req.REQUEST_LOGINNAME == this.loginName || IsAdmin) && (req.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL))
                            {
                                e.RepositoryItem = repositoryItemBtnDeleteServiceReq;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemBtnDeleteServiceReqDisable;
                            }
                            if (_TreatmentBedRoom.BED_ROOM_ID != WorkPlaceSDO.BedRoomId)
                            {
                                e.RepositoryItem = repositoryItemBtnDeleteServiceReqDisable;
                            }

                        }
                    }
                    else if (e.Column.FieldName == "AMOUNT_TEMP")
                    {
                        e.RepositoryItem = data.AMOUNT_TEMP != null && data.TDL_REQUEST_DEPARTMENT_ID == this.WorkPlaceSDO.DepartmentId ? repositoryItemSpinEditAmountTemp : repositoryItemSpinEditAmountTempDis;
                    }
                    else if (e.Column.FieldName == "AMOUNT")
                    {
                        if (listCurrentBedLog != null && listCurrentBedLog.Count() > 0)
                        {
                            var req = ListServiceReqForSereServs.FirstOrDefault(o => o.ID == data.SERVICE_REQ_ID && o.BED_LOG_ID != null);
                            if (req != null)
                            {
                                e.RepositoryItem = listCurrentBedLog.FirstOrDefault(o => o.ID == req.BED_LOG_ID).FINISH_TIME != null && data.TDL_REQUEST_DEPARTMENT_ID == this.WorkPlaceSDO.DepartmentId ? repositoryItemSpinEditAmount : repositoryItemSpinEditAmountDis;
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
        #endregion

        #region Click
        private void CountTimeBed()
        {
            try
            {
                gridControlBedServiceType.DataSource = null;
                this.bedLogCheckProcessing = new List<ADO.HisBedHistoryADO>();
                if (bedLogChecks != null && bedLogChecks.Count > 0)
                {
                    bedLogChecks.ForEach(o => SetTime(o));
                    this.bedLogCheckProcessing = bedLogChecks.Where(o => o.IsChecked == true && !o.Error && o.finishTime != null && o.finishTime != DateTime.MinValue && o.BED_SERVICE_TYPE_ID != null && o.START_TIME > 0).ToList();
                    if (this.bedLogCheckProcessing != null && this.bedLogCheckProcessing.Count > 0)
                    {
                        TotalTime();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void SetTime(HisBedHistoryADO data)
        {
            try
            {
                data.FINISH_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(data.finishTime);
                data.START_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(data.startTime) ?? 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Tính thời gian
        /// </summary>
        private void TotalTime()
        {
            try
            {
                if (this.bedLogCheckProcessing != null && this.bedLogCheckProcessing.Count > 0)
                {
                    //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("bedLogCheckProcessing_____", bedLogCheckProcessing));

                    WaitingManager.Show();
                    var ListBedLogGroup = this.bedLogCheckProcessing.GroupBy(p => new { p.BED_SERVICE_TYPE_ID, p.PATIENT_TYPE_ID, p.PRIMARY_PATIENT_TYPE_ID, p.SHARE_COUNT }).Select(grc => grc.ToList()).ToList();
                    ListBedServiceTypes = new List<ADO.HisBedServiceTypeADO>();

                    foreach (var itemGroups in ListBedLogGroup)
                    {
                        this.ExecuteTotalDateTimeBed(itemGroups, ref ListBedServiceTypes);
                    }

                    if (ChkSplitDay.Checked || chkSplitByResult.Checked)
                    {
                        var splitData = ProcessSplitDay();
                        //Gán data
                        gridControlBedServiceType.DataSource = null;
                        gridControlBedServiceType.DataSource = splitData;
                        Gv_BedServiceType__Gc_NamGhep.OptionsColumn.AllowEdit = true;
                    }
                    else
                    {
                        //Gán data
                        gridControlBedServiceType.DataSource = null;
                        gridControlBedServiceType.DataSource = ListBedServiceTypes;
                        Gv_BedServiceType__Gc_NamGhep.OptionsColumn.AllowEdit = false;
                    }
                    //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("ListBedServiceTypes_____", ListBedServiceTypes));

                    if (ListBedServiceTypes.Count > 0)
                    {
                        btnAssigns.Enabled = true;
                    }
                    else
                        btnAssigns.Enabled = false;

                    //Tổng tiền
                    setDefaultMedicineMaterialTotalPrice();
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Tính số ngày giường
        /// </summary>
        /// <param name="bebHistoryAdos"></param>
        /// <param name="result"></param>
        private void ExecuteTotalDateTimeBed(List<HisBedHistoryADO> bebHistoryAdos, ref List<HisBedServiceTypeADO> result)
        {
            try
            {
                if (bebHistoryAdos != null && bebHistoryAdos.Count > 0)
                {
                    foreach (var bed in bebHistoryAdos)
                    {
                        ADO.HisBedServiceTypeADO bedServiceType = new ADO.HisBedServiceTypeADO();
                        decimal tongSoNgayGiuong = 0;
                        bebHistoryAdos = bebHistoryAdos.OrderBy(o => o.startTime).ToList();
                        Inventec.Common.Mapper.DataObjectMapper.Map<ADO.HisBedServiceTypeADO>(bedServiceType, bebHistoryAdos.FirstOrDefault());
                        long? namghep = null;
                        //Review
                        tongSoNgayGiuong = ProcessTotalBedDay(bebHistoryAdos, ref namghep);

                        bedServiceType.BED_SERVICE_TYPE_NAME = _services.FirstOrDefault(p => p.ID == bed.BED_SERVICE_TYPE_ID).SERVICE_NAME;
                        bedServiceType.AMOUNT = tongSoNgayGiuong <= 0 ? 1 : tongSoNgayGiuong;
                        bedServiceType.IsExpend = false;
                        bedServiceType.IsOutKtcFee = false;
                        bedServiceType.REQUEST_LOGINNAME = this.loginName;
                        if (!bedServiceType.PATIENT_TYPE_ID.HasValue)
                            ChoosePatientTypeDefaultlService(CurrentTreatment.TDL_PATIENT_TYPE_ID ?? 0, bedServiceType.BED_SERVICE_TYPE_ID.Value, bedServiceType);

                        if ((ChkSplitDay.Checked || chkSplitByResult.Checked) && bebHistoryAdos.FirstOrDefault().PRIMARY_PATIENT_TYPE_ID.HasValue)
                        {
                            bedServiceType.PRIMARY_PATIENT_TYPE_ID = bebHistoryAdos.FirstOrDefault().PRIMARY_PATIENT_TYPE_ID;
                        }

                        if (namghep.HasValue)
                        {
                            bedServiceType.AmmoutNamGhep = namghep;
                        }

                        bedServiceType.IsBedStretcher = bed.IsBedStretcher;
                        bedServiceType.SERVICE_CONDITION_ID = bed.SERVICE_CONDITION_ID;
                        bedServiceType.IntructionTime = bed.startTime;
                        bedServiceType.OTHER_PAY_SOURCE_ID = ProcessAutoSetOtherPaySource(bedServiceType);
                        var paty = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == bedServiceType.PATIENT_TYPE_ID);
                        if (paty != null && !String.IsNullOrWhiteSpace(paty.OTHER_PAY_SOURCE_IDS))
                        {
                            bedServiceType.HasConfigOtherSourcePay = true;
                        }
                        if (ChkSplitDay.Checked || chkSplitByResult.Checked)
                            bedServiceType.IsSplitDayOrResult = true;
                        result.Add(bedServiceType);
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnAssigns_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                //Review
                if (!btnAssigns.Enabled) return;
                if (CurrentTreatment.TDL_TREATMENT_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU)
                {
                    Inventec.Common.Logging.LogSystem.Debug("BedServiceType_NotAllow_For_OutPatient__:" + HisConfigKeys.BedServiceType_NotAllow_For_OutPatient);
                    if (HisConfigKeys.BedServiceType_NotAllow_For_OutPatient == "1")
                    {
                        if (XtraMessageBox.Show("Bệnh nhân không phải diện điều trị nội trú. Bạn có chắc muốn chỉ định dịch vụ giường cho bệnh nhân không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.None, MessageBoxDefaultButton.Button2) == DialogResult.No) return;
                    }
                    else if (HisConfigKeys.BedServiceType_NotAllow_For_OutPatient == "2")
                    {
                        XtraMessageBox.Show("Không cho phép chỉ định giường đối với bệnh nhân không phải điều trị nội trú", "Thông báo"); return;
                    }
                }
                WaitingManager.Show();
                var dataBedServiceTypeForSave = gridViewBedServiceType.DataSource as List<ADO.HisBedServiceTypeADO>;
                bool valid = true;
                valid = valid && CheckValidPatientTypeGridService(dataBedServiceTypeForSave, param);

                if (valid && !String.IsNullOrWhiteSpace(LblTreatmentDayCount.Text))
                {
                    decimal amountBed = 0;
                    if (gridControlBedServiceReq.DataSource != null)
                    {
                        List<HIS_SERE_SERV> lstSereServ = gridControlBedServiceReq.DataSource as List<HIS_SERE_SERV>;
                        if (lstSereServ != null && lstSereServ.Count > 0)
                        {
                            amountBed += lstSereServ.Sum(s => s.AMOUNT);
                        }
                    }

                    amountBed += dataBedServiceTypeForSave.Sum(s => s.AMOUNT);
                    decimal dayCount = Inventec.Common.TypeConvert.Parse.ToDecimal(LblTreatmentDayCount.Text);

                    if (amountBed > dayCount)
                    {
                        WaitingManager.Hide();
                        if (DevExpress.XtraEditors.XtraMessageBox.Show("Tổng số ngày giường điều trị lớn hơn số ngày điều trị, bạn có muốn tiếp tục?", ResourceMessage.ThongBao, MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                        {
                            return;
                        }
                        WaitingManager.Show();
                    }
                }
                valid = valid && ValidSereServWithCondition(dataBedServiceTypeForSave);
                if (!valid)
                    return;
                if (valid)
                {
                    hisBedServiceReqSDO = new MOS.SDO.HisBedServiceReqSDO();
                    hisBedServiceReqSDO.BedServices = new List<HisBedServiceSDO>();

                    ProcessBedServiceReqSDO(hisBedServiceReqSDO, dataBedServiceTypeForSave);
                    Inventec.Common.Logging.LogSystem.Debug("hisBedServiceReqSDO gui len" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => hisBedServiceReqSDO), hisBedServiceReqSDO));
                    var currentBedServiceReq = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisServiceReqListResultSDO>("api/HisServiceReq/CreateByBedLog", ApiConsumer.ApiConsumers.MosConsumer, hisBedServiceReqSDO, param);
                    //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param));
                    if (currentBedServiceReq != null)
                    {
                        success = true;
                        gridControlBedServiceType.DataSource = null;
                        //btnAssigns.Enabled = false;
                        if (currentBedServiceReq.ServiceReqs != null && currentBedServiceReq.ServiceReqs.Count() > 0)
                        {
                            foreach (var item in currentBedServiceReq.ServiceReqs)
                            {
                                HIS_SERVICE_REQ serviceReq = new HIS_SERVICE_REQ();
                                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERVICE_REQ>(serviceReq, item);
                                ListServiceReqForSereServs.Add(serviceReq);
                            }
                        }
                        GetAllBedLog();
                        LoadDataGridServiceReq();
                        FillDataToGridBedLog();

                        //xóa Tổng tiền
                        setDefaultMedicineMaterialTotalPrice();
                    }
                }
                WaitingManager.Hide();

                #region Show message
                MessageManager.Show(this, param, success);
                #endregion

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        /// <summary>
        /// Kiem Tra Doi Tuong Thanh Toan
        /// </summary>
        /// <param name="dataBedServiceTypeModel"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        private bool CheckValidPatientTypeGridService(List<ADO.HisBedServiceTypeADO> dataBedServiceTypeModel, CommonParam param)
        {
            bool valid = true;
            try
            {
                if (dataBedServiceTypeModel != null && dataBedServiceTypeModel.Count > 0)
                {
                    List<string> mess = new List<string>();
                    foreach (var item in dataBedServiceTypeModel)
                    {
                        if (item.PATIENT_TYPE_ID == null || item.PATIENT_TYPE_ID == 0)
                        {
                            valid = valid && false;
                            mess.Add(ResourceMessage.ChiDinhDichVu_KhongCoDoiTuongThanhToan);
                            Inventec.Common.Logging.LogSystem.Error("Dich vu (" + item.SERVICE_CODE + "-" + item.SERVICE_NAME + " khong co doi tuong thanh toan.");
                            //break;
                        }

                        if (item.HasConfigOtherSourcePay && !item.OTHER_PAY_SOURCE_ID.HasValue)
                        {
                            mess.Add(String.Format(ResourceMessage.ChiDinhDichVu_BatBuocChonNguonKhac, item.PATIENT_TYPE_NAME));
                        }
                    }

                    if (!valid || mess.Count > 0)
                    {
                        mess = mess.Distinct().ToList();
                        if (param.Messages == null) param.Messages = new List<string>();

                        param.Messages.AddRange(mess);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }

        /// <summary>
        /// Xu ly SDO
        /// </summary>
        /// <param name="HisServiceReqSDO"></param>
        /// <param name="dataBedServiceTypeModel"></param>
        private void ProcessBedServiceReqSDO(HisBedServiceReqSDO HisServiceReqSDO, List<ADO.HisBedServiceTypeADO> dataBedServiceTypeModel)
        {
            try
            {
                if (this._TreatmentBedRoom != null)
                {
                    HisServiceReqSDO.TreatmentId = this._TreatmentBedRoom.TREATMENT_ID;
                }

                HisServiceReqSDO.RequestRoomId = WorkPlaceSDO.RoomId;

                if (dataBedServiceTypeModel != null && dataBedServiceTypeModel.Count > 0)
                {
                    var hisEmlpyees = BackendDataWorker.Get<HIS_EMPLOYEE>().ToList();
                    foreach (var bedService in dataBedServiceTypeModel)
                    {
                        HisBedServiceSDO bedSdo = new HisBedServiceSDO();
                        bedSdo.ServiceReqDetails = new List<ServiceReqDetailSDO>();
                        bedSdo.BedLogId = bedService.ID;
                        bedSdo.InstructionTime = bedService.START_TIME;
                        bedSdo.RequestLoginname = bedService.REQUEST_LOGINNAME;
                        var requestUsername = hisEmlpyees.FirstOrDefault(o => o.LOGINNAME == bedSdo.RequestLoginname);
                        bedSdo.RequestUsename = requestUsername != null ? requestUsername.TDL_USERNAME : "";
                        ServiceReqDetailSDO sdo = new ServiceReqDetailSDO();
                        sdo.ShareCount = bedService.AmmoutNamGhep;
                        sdo.Amount = System.Convert.ToDecimal(bedService.AMOUNT);
                        sdo.PatientTypeId = bedService.PATIENT_TYPE_ID ?? 0;
                        sdo.RoomId = WorkPlaceSDO.RoomId;
                        sdo.ServiceId = bedService.BED_SERVICE_TYPE_ID ?? 0;
                        sdo.IsExpend = (bedService.IsExpend == true ? 1 : (short?)null);
                        sdo.IsOutParentFee = (bedService.IsOutKtcFee == true ? 1 : (short?)null);
                        sdo.PrimaryPatientTypeId = bedService.PRIMARY_PATIENT_TYPE_ID;
                        sdo.OtherPaySourceId = bedService.OTHER_PAY_SOURCE_ID;
                        sdo.ServiceConditionId = bedService.SERVICE_CONDITION_ID;
                        bedSdo.ServiceReqDetails.Add(sdo);
                        HisServiceReqSDO.BedServices.Add(bedSdo);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ChkNotCountHours_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                CountTimeBed();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion
        #endregion

        #region Shotcut
        private void barButtonAssign_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnAssigns_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion


        private void repositoryItemCustomGridLookUp__CboBed_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView View = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    long IsKey = Inventec.Common.TypeConvert.Parse.ToInt64((View.GetRowCellValue(e.RowHandle, "IsKey") ?? "0").ToString());
                    if (IsKey == 1)
                    {
                        e.Appearance.ForeColor = Color.Blue;
                    }
                    else if (IsKey == 2)
                    {
                        e.Appearance.ForeColor = Color.Red;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barBtnSaveBedLog_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                BtnSaveBedLog_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnSaveBedLog_Click(object sender, EventArgs e)
        {
            try
            {
                if (!BtnSaveBedLog.Enabled) return;
                if (bedLogChecks != null && bedLogChecks.Count > 0)
                {
                    foreach (var item in bedLogChecks)
                    {
                        CheckErrorDataBedLog(item);
                    }

                    var bedLog = bedLogChecks.Where(o => !o.Error).ToList();

                    if (bedLog != null && bedLog.Count > 0 && bedLog.Exists(o => !o.IsSave))
                    {
                        bool success = true;
                        CommonParam paramTotal = new CommonParam();
                        foreach (var item in bedLog)
                        {
                            if (!item.IsSave)
                            {
                                bool successlog = false;
                                CommonParam param = new CommonParam();
                                SaveDataBedLog(item, ref successlog, ref param);

                                success = success && successlog;
                                if (param.BugCodes != null && param.BugCodes.Count > 0)
                                {
                                    if (paramTotal.BugCodes == null) paramTotal.BugCodes = new List<string>();
                                    paramTotal.BugCodes.AddRange(param.BugCodes);
                                }

                                if (param.Messages != null && param.Messages.Count > 0)
                                {
                                    if (paramTotal.Messages == null) paramTotal.Messages = new List<string>();
                                    paramTotal.Messages.AddRange(param.Messages);
                                }
                            }
                        }

                        if (paramTotal.BugCodes != null && paramTotal.BugCodes.Count > 0)
                        {
                            paramTotal.BugCodes = paramTotal.BugCodes.Distinct().ToList();
                        }

                        if (paramTotal.Messages != null && paramTotal.Messages.Count > 0)
                        {
                            paramTotal.Messages = paramTotal.Messages.Distinct().ToList();
                        }

                        #region Show message
                        MessageManager.Show(this, paramTotal, success);
                        #endregion
                    }
                    else
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.TatCaDuLieuDaLuu, ResourceMessage.ThongBao);
                    }
                }
                else
                {
                    if (gridControlBedHistory.DataSource != null)
                        DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.TatCaDuLieuDaLuu, ResourceMessage.ThongBao);
                    else
                        DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.KhongXacDinhDuocDuLieu, ResourceMessage.ThongBao);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void DtOutTime_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (DtOutTime.EditValue != null)
                {
                    TxtTreatmentEndTypeCode.Focus();
                    TxtTreatmentEndTypeCode.SelectAll();
                }

                LblTreatmentDayCount.Text = CountTreatmentDay() + "";
                CheckEnableChkSplitByResult();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboTreatmentEndType_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (CboTreatmentEndType.EditValue != null)
                {
                    var endType = BackendDataWorker.Get<HIS_TREATMENT_END_TYPE>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(CboTreatmentEndType.EditValue.ToString()));
                    if (endType != null)
                    {
                        TxtTreatmentEndTypeCode.Text = endType.TREATMENT_END_TYPE_CODE;
                        TxtTreatmentResultCode.Focus();
                        TxtTreatmentResultCode.SelectAll();
                    }
                }

                LblTreatmentDayCount.Text = CountTreatmentDay() + "";
                CheckEnableChkSplitByResult();
                Gv_BedServiceType__Gc_NamGhep.OptionsColumn.AllowEdit = false;
                if (chkSplitByResult.Checked && DtOutTime.EditValue != null && CboTreatmentResult.EditValue != null)
                {
                    gridControlBedServiceType.DataSource = null;
                    var splitData = ProcessSplitDay();
                    gridControlBedServiceType.DataSource = splitData;
                    Gv_BedServiceType__Gc_NamGhep.OptionsColumn.AllowEdit = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CheckEnableChkSplitByResult()
        {
            try
            {
                bool isEnableChkSplitByResult = DtOutTime.EditValue != null && DtOutTime.DateTime != DateTime.MinValue && !string.IsNullOrEmpty(TxtTreatmentEndTypeCode.Text) && CboTreatmentEndType.EditValue != null && !string.IsNullOrEmpty(TxtTreatmentResultCode.Text) && CboTreatmentResult.EditValue != null;

                if (isEnableChkSplitByResult)
                {
                    chkSplitByResult.Enabled = true;
                    ChkNotCountHours.Enabled = ChkSplitDay.Enabled = false;
                }
                else
                {
                    ChkNotCountHours.Enabled = ChkSplitDay.Enabled = true;
                    chkSplitByResult.Enabled = chkSplitByResult.Checked = false;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboTreatmentResult_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (CboTreatmentResult.EditValue != null)
                {
                    var resultTreat = BackendDataWorker.Get<HIS_TREATMENT_RESULT>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(CboTreatmentResult.EditValue.ToString()));
                    if (resultTreat != null)
                    {
                        TxtTreatmentResultCode.Text = resultTreat.TREATMENT_RESULT_CODE;
                    }
                }

                LblTreatmentDayCount.Text = CountTreatmentDay() + "";
                CheckEnableChkSplitByResult();
                Gv_BedServiceType__Gc_NamGhep.OptionsColumn.AllowEdit = false;
                if (chkSplitByResult.Checked && DtOutTime.EditValue != null && CboTreatmentResult.EditValue != null)
                {
                    gridControlBedServiceType.DataSource = null;
                    var splitData = ProcessSplitDay();
                    gridControlBedServiceType.DataSource = splitData;
                    Gv_BedServiceType__Gc_NamGhep.OptionsColumn.AllowEdit = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void TxtTreatmentEndTypeCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool showCbo = true;
                    if (!String.IsNullOrEmpty(TxtTreatmentEndTypeCode.Text.Trim()))
                    {
                        string code = TxtTreatmentEndTypeCode.Text.Trim();
                        var listData = BackendDataWorker.Get<HIS_TREATMENT_END_TYPE>().Where(o => o.TREATMENT_END_TYPE_CODE.Contains(code)).ToList();
                        var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.TREATMENT_END_TYPE_CODE == code).ToList() : listData) : null;
                        if (result != null && result.Count > 0)
                        {
                            showCbo = false;
                            TxtTreatmentEndTypeCode.Text = result.First().TREATMENT_END_TYPE_CODE;
                            CboTreatmentEndType.EditValue = result.First().ID;
                            TxtTreatmentResultCode.Focus();
                            TxtTreatmentResultCode.SelectAll();
                        }
                    }
                    if (showCbo)
                    {
                        CboTreatmentEndType.Focus();
                        CboTreatmentEndType.ShowPopup();
                    }
                }

                LblTreatmentDayCount.Text = CountTreatmentDay() + "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void TxtTreatmentResultCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool showCbo = true;
                    if (!String.IsNullOrEmpty(TxtTreatmentResultCode.Text.Trim()))
                    {
                        string code = TxtTreatmentResultCode.Text.Trim();
                        var listData = BackendDataWorker.Get<HIS_TREATMENT_RESULT>().Where(o => o.TREATMENT_RESULT_CODE.Contains(code)).ToList();
                        var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.TREATMENT_RESULT_CODE == code).ToList() : listData) : null;
                        if (result != null && result.Count > 0)
                        {
                            showCbo = false;
                            TxtTreatmentResultCode.Text = result.First().TREATMENT_RESULT_CODE;
                            CboTreatmentResult.EditValue = result.First().ID;
                        }
                    }
                    if (showCbo)
                    {
                        CboTreatmentResult.Focus();
                        CboTreatmentResult.ShowPopup();
                    }
                }

                LblTreatmentDayCount.Text = CountTreatmentDay() + "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboTreatmentEndType_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    CboTreatmentEndType.EditValue = null;
                    TxtTreatmentEndTypeCode.Text = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboTreatmentResult_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    CboTreatmentResult.EditValue = null;
                    TxtTreatmentResultCode.Text = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToCboPrimaryPatientType(List<HIS_PATIENT_TYPE> data)
        {
            try
            {
                repositoryItemCboPrimaryPatientType.DataSource = data;
                repositoryItemCboPrimaryPatientType.DisplayMember = "PATIENT_TYPE_NAME";
                repositoryItemCboPrimaryPatientType.ValueMember = "ID";

                repositoryItemCboPrimaryPatientType.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                repositoryItemCboPrimaryPatientType.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                repositoryItemCboPrimaryPatientType.ImmediatePopup = true;
                repositoryItemCboPrimaryPatientType.View.Columns.Clear();

                DevExpress.XtraGrid.Columns.GridColumn aColumnCode = repositoryItemCboPrimaryPatientType.View.Columns.AddField("PATIENT_TYPE_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 50;

                DevExpress.XtraGrid.Columns.GridColumn aColumnName = repositoryItemCboPrimaryPatientType.View.Columns.AddField("PATIENT_TYPE_NAME");
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

        private void ChkSplitDay_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (ChkSplitDay.Checked)
                {
                    var splitData = ProcessSplitDay();
                    //Gán data
                    gridControlBedServiceType.DataSource = null;
                    gridControlBedServiceType.DataSource = splitData;
                    Gv_BedServiceType__Gc_NamGhep.OptionsColumn.AllowEdit = true;
                }
                else
                {
                    //Gán data
                    gridControlBedServiceType.DataSource = null;
                    gridControlBedServiceType.DataSource = ListBedServiceTypes;
                    Gv_BedServiceType__Gc_NamGhep.OptionsColumn.AllowEdit = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtnDeleteServiceReq_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (gridViewBedServiceReq.FocusedRowHandle >= 0)
                {
                    CommonParam param = new CommonParam();
                    bool success = false;
                    var data = (HIS_SERE_SERV)gridViewBedServiceReq.GetFocusedRow();
                    if (data == null)
                    {
                        //Inventec.Common.Logging.LogSystem.Info("Data thuc hien huy yeu cau dich vu null: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
                        return;
                    }

                    if (MessageBox.Show(ResourceMessage.HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong, ResourceMessage.ThongBao, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        if (CheckParentBeforeDelete(data.ID) && DevExpress.XtraEditors.XtraMessageBox.Show("Đã có y lệnh đính kèm (CLS). Bạn có chắc chắn muốn xóa không?", "Thông báo", MessageBoxButtons.YesNo) == DialogResult.No)
                        {
                            return;
                        }

                        WaitingManager.Show();
                        MOS.SDO.HisServiceReqSDO sdo = new MOS.SDO.HisServiceReqSDO();
                        sdo.Id = data.SERVICE_REQ_ID;
                        sdo.RequestRoomId = this.currentModuleBase.RoomId;
                        success = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>("api/HisServiceReq/Delete", ApiConsumer.ApiConsumers.MosConsumer, sdo, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                        WaitingManager.Hide();
                        //if (success)
                        {
                            LoadDataGridServiceReq();
                        }
                        if (success) FillDataToGridBedLog();

                        #region Show message
                        MessageManager.Show(this, param, success);
                        #endregion

                        #region Process has exception
                        HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool CheckParentBeforeDelete(long _serviceReqId)
        {
            bool result = false;
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisServiceReqFilter serviceReqFilter = new MOS.Filter.HisServiceReqFilter();
                serviceReqFilter.PARENT_ID = _serviceReqId;
                var serviceReqParent = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumer.ApiConsumers.MosConsumer, serviceReqFilter, param);
                if (serviceReqParent != null && serviceReqParent.Count > 0)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
            return result;
        }

        private void gridControlBedHistory_ProcessGridKey(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (gridViewBedHistory.FocusedColumn != Gv_BedHistory__Gc_StartTime && gridViewBedHistory.FocusedColumn != Gv_BedHistory__Gc_FinishTime)
                    {
                        if (gridViewBedHistory.FocusedColumn.VisibleIndex == gridViewBedHistory.VisibleColumns.Count - 1)
                        {
                            gridViewBedHistory.FocusedRowHandle++;
                            e.Handled = true;
                            gridViewBedHistory.FocusedColumn = gridView1.VisibleColumns.First();
                        }
                        else
                        {
                            gridViewBedHistory.FocusedColumn = gridViewBedHistory.GetVisibleColumn(gridViewBedHistory.FocusedColumn.VisibleIndex + 1);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void DateStart_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    MoveNextTime(sender, gridViewBedHistory, e);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void MoveNextTime(object dateEdit, GridView gridView, KeyEventArgs e)
        {
            try
            {
                DateEdit edit = dateEdit as DateEdit;

                if (!String.IsNullOrWhiteSpace(edit.Text) && edit.Text.IndexOf(edit.SelectedText, edit.SelectionStart) != (edit.Text.Length - 2))
                {
                    string timeSeparator = "/";
                    if (edit.SelectionStart == 6)
                    {
                        timeSeparator = " ";
                    }
                    else if (edit.SelectionStart > 6)
                    {
                        timeSeparator = ":";
                    }

                    if (edit.SelectedText.Length == 2 || edit.SelectedText.Length == 4)
                    {
                        edit.SelectionStart = edit.Text.IndexOf(timeSeparator, edit.SelectionStart) + 1;
                        if (edit.SelectionStart == 6)
                        {
                            timeSeparator = " ";
                        }
                        else if (edit.SelectionStart > 6)
                        {
                            timeSeparator = ":";
                        }

                        edit.SelectionLength = edit.Text.IndexOf(timeSeparator, edit.SelectionStart) - edit.SelectionStart;
                        e.Handled = true;
                    }
                    else
                    {
                        edit.SelectionStart = 0;
                        edit.SelectionLength = 2;
                        e.Handled = true;
                    }
                }
                else
                {
                    gridView.FocusedColumn = gridView.GetVisibleColumn(gridView.FocusedColumn.VisibleIndex + 1);
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void DateFinish_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    MoveNextTime(sender, gridViewBedHistory, e);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemDtIntructionTime_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    MoveNextTime(sender, gridViewBedServiceType, e);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridControlBedServiceType_ProcessGridKey(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (gridViewBedServiceType.FocusedColumn != Gv_BedServiceType__Gc_IntructionTime)
                    {
                        if (gridViewBedServiceType.FocusedColumn.VisibleIndex == gridViewBedServiceType.VisibleColumns.Count - 1)
                        {
                            gridViewBedHistory.FocusedRowHandle++;
                            e.Handled = true;
                            gridViewBedServiceType.FocusedColumn = gridView1.VisibleColumns.First();
                        }
                        else
                        {
                            gridViewBedServiceType.FocusedColumn = gridViewBedServiceType.GetVisibleColumn(gridViewBedServiceType.FocusedColumn.VisibleIndex + 1);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboBedRoom_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null) return;
                foreach (V_HIS_BED_ROOM rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(", "); }
                    sb.Append(rv.BED_ROOM_NAME);
                }
                e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboBedRoom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    //load lại danh sách lịch sử giường, giường
                    FillDataToGridBedLog();
                    RefeshDataToCboBed(0, new HisBedHistoryADO(), true);
                    gridViewBedHistory.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void customGridView3_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView View = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            if (e.RowHandle >= 0)
            {
                long IsKey = Inventec.Common.TypeConvert.Parse.ToInt64((View.GetRowCellValue(e.RowHandle, "IsKey") ?? "0").ToString());
                if (IsKey == 1)
                {
                    e.Appearance.ForeColor = Color.Blue;
                }
                else if (IsKey == 2)
                {
                    e.Appearance.ForeColor = Color.Red;
                }
            }
        }

        private void repositoryItemCboOtherPaySource_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    var row = (HisBedServiceTypeADO)gridViewBedServiceType.GetFocusedRow();
                    if (row != null)
                    {
                        row.OTHER_PAY_SOURCE_ID = null;
                        gridControlBedServiceType.RefreshDataSource();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemCboOtherPaySource_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                DevExpress.XtraEditors.GridLookUpEdit edit = sender as DevExpress.XtraEditors.GridLookUpEdit;
                if (edit == null) return;
                if (edit.EditValue != null)
                {
                    var row = (HisBedServiceTypeADO)gridViewBedServiceType.GetFocusedRow();
                    if (row != null)
                    {
                        var pt = BackendDataWorker.Get<HIS_OTHER_PAY_SOURCE>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(edit.EditValue.ToString()));
                        if (pt != null)
                        {
                            gridViewBedServiceType.SetRowCellValue(gridViewBedServiceType.FocusedRowHandle, Gv_BedServiceType__Gc_OtherSource, pt.ID);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UpdateSereServ(long id, decimal? amount, decimal? amountTemp, string url, HIS_SERE_SERV currentSereServ)
        {
            try
            {
                bool success = false;
                CommonParam param = new CommonParam();
                WaitingManager.Show();
                UpdateBedAmountSDO sdo = new UpdateBedAmountSDO();
                sdo.SereServId = id;
                sdo.WorkingRoomId = this.WorkPlaceSDO.RoomId;
                sdo.Amount = amount != null ? (decimal)amount : (decimal)amountTemp;
                var rs = new BackendAdapter(param).Post<HIS_SERE_SERV>(url, ApiConsumers.MosConsumer, sdo, param);
                if (rs != null)
                {
                    success = true;
                    if (amount != null)
                        currentSereServ.AMOUNT = (decimal)amount;
                    else if (amountTemp != null)
                        currentSereServ.AMOUNT_TEMP = (decimal)amountTemp;
                }
                WaitingManager.Hide();
                Inventec.Desktop.Common.Message.MessageManager.Show(this, param, success);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemSpinEditAmount_Leave(object sender, EventArgs e)
        {
            try
            {
                var row = (HIS_SERE_SERV)gridViewBedServiceReq.GetFocusedRow();
                SpinEdit spn = sender as SpinEdit;
                if (row != null)
                {
                    var currentSereServBed = currentBedSereServs.FirstOrDefault(o => o.ID == row.ID);
                    if (currentSereServBed != null)
                    {
                        if (currentSereServBed.AMOUNT == spn.Value) return;
                        if (MessageBox.Show("Bạn có muốn thay đổi số lượng của chỉ định dịch vụ giường hay không?",
                            ResourceMessage.ThongBao, MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            string url = "api/HisSereServ/UpdateBedAmount";
                            UpdateSereServ(row.ID, spn.Value, null, url, currentSereServBed);
                        }
                        else

                            gridViewBedServiceReq.SetFocusedRowCellValue("AMOUNT", currentSereServBed.AMOUNT);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemSpinEditAmountTemp_Leave(object sender, EventArgs e)
        {
            try
            {
                var row = (HIS_SERE_SERV)gridViewBedServiceReq.GetFocusedRow();
                SpinEdit spn = sender as SpinEdit;
                if (row != null)
                {
                    var currentSereServBed = currentBedSereServs.FirstOrDefault(o => o.ID == row.ID);
                    if (currentSereServBed != null)
                    {
                        if (currentSereServBed.AMOUNT_TEMP == spn.Value) return;
                        if (MessageBox.Show("Bạn có muốn thay đổi số lượng tạm tính của chỉ định dịch vụ giường hay không?",
                            ResourceMessage.ThongBao, MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            string url = "api/HisSereServ/UpdateBedTempAmount";
                            UpdateSereServ(row.ID, null, spn.Value, url, currentSereServBed);
                        }
                        else
                            gridViewBedServiceReq.SetFocusedRowCellValue("AMOUNT_TEMP", currentSereServBed.AMOUNT_TEMP);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkSplitByResult_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                gridControlBedServiceType.DataSource = null;
                if (chkSplitByResult.Checked)
                {
                    ChkNotCountHours.Checked = ChkSplitDay.Checked = ChkNotCountHours.Enabled = ChkSplitDay.Enabled = false;
                    var splitData = ProcessSplitDay();
                    gridControlBedServiceType.DataSource = splitData;
                    Gv_BedServiceType__Gc_NamGhep.OptionsColumn.AllowEdit = true;
                }
                else
                {
                    ChkNotCountHours.Enabled = ChkSplitDay.Enabled = true;
                    gridControlBedServiceType.DataSource = ListBedServiceTypes;
                    Gv_BedServiceType__Gc_NamGhep.OptionsColumn.AllowEdit = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkBHAll_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                long departmentId = 0;
                if (this.WorkPlaceSDO != null)
                {
                    departmentId = this.WorkPlaceSDO.DepartmentId;
                }
                if (chkBHAll.Checked)
                {
                    this.ListVBedRoom = LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_BED_ROOM>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                }
                else
                {
                    var lstRoomNotInDepartment = LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_BED_ROOM>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList().Where(o => o.DEPARTMENT_ID != this.WorkPlaceSDO.DepartmentId).ToList();
                    if (lstRoomNotInDepartment != null && lstRoomNotInDepartment.Count > 0)
                    {
                        this.ListVBedRoom = ListVBedRoom.Where(o => !lstRoomNotInDepartment.Exists(p => p.ID == o.ID)).ToList();
                    }
                }

                if (this.ListVBedRoom == null || this.ListVBedRoom.Count <= 0)
                {
                    this.ListVBedRoom = LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_BED_ROOM>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList().Where(o => o.ROOM_ID == this.WorkPlaceSDO.RoomId).ToList();
                }
                //Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("this.ListVBedRoom__:", this.ListVBedRoom));


                var allData = LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_BED_ROOM>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList().OrderByDescending(o => o.DEPARTMENT_ID == departmentId).ToList();

                SetValueBedRoom(cboBedRoom, this.ListVBedRoom, allData);

                LoadDataGridServiceReq();
                FillDataToGridBedLog();
                RefeshDataToCboBed(0, new HisBedHistoryADO(), true);
                chkBHAll.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkBedAll_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkBedAll.Checked)
                    this.ListVBedRoom = LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_BED_ROOM>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                RefeshDataToCboBed(0, new HisBedHistoryADO(), true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemCboRequestUser_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                Inventec.Desktop.CustomControl.CustomGridLookUpEditWithFilterMultiColumn edit = sender as Inventec.Desktop.CustomControl.CustomGridLookUpEditWithFilterMultiColumn;
                if (edit == null) return;
                if (edit.EditValue != null)
                {
                    var row = (HisBedServiceTypeADO)gridViewBedServiceType.GetFocusedRow();
                    if (row != null)
                    {
                        var emp = BackendDataWorker.Get<HIS_EMPLOYEE>().FirstOrDefault(o => o.LOGINNAME == edit.EditValue.ToString());
                        if (emp != null)
                        {
                            gridViewBedServiceType.SetRowCellValue(gridViewBedServiceType.FocusedRowHandle, Gv_BedServiceType__Gc_RequestLoginname, emp.LOGINNAME);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repEditFinishTimeEnable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (ADO.HisBedHistoryADO)gridViewBedHistory.GetFocusedRow();
                if (row != null && DevExpress.XtraEditors.XtraMessageBox.Show("Bạn có muốn cập nhật thời gian kết thúc lịch sử giường theo thời gian lớn nhất của y lệnh giường tương ứng không",
                   ResourceMessage.ThongBao,
                   MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    row.finishTime = (DateTime)Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(row.MaxIntructionTime ?? 0);
                    gridControlBedHistory.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repServiceCondition_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {

            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    var data = (ADO.HisBedHistoryADO)gridViewBedHistory.GetFocusedRow();
                    if (data != null && data.IsBedStretcher)
                    {
                        data.SERVICE_CONDITION_ID = null;
                    }
                    gridViewBedHistory.EditingValue = null;
                    gridControlBedHistory.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        private bool ValidSereServWithCondition(List<ADO.HisBedServiceTypeADO> dataList)
        {
            bool valid = true;
            try
            {
                List<string> lstString = new List<string>();
                foreach (var item in dataList)
                {
                    if (item.IsBedStretcher && item.PATIENT_TYPE_ID != null)
                    {
                        var service = VHisBedServiceTypes.FirstOrDefault(o => o.ID == item.BED_SERVICE_TYPE_ID);
                        var dataCondition = BranchDataWorker.ServicePatyWithListPatientType(service.ID, new List<long> { item.PATIENT_TYPE_ID ?? 0 });
                        if (dataCondition != null && dataCondition.Count > 0 && lstConditionService != null && lstConditionService.Count > 0)
                        {
                            dataCondition = dataCondition.Where(o => lstConditionService.Exists(p => p.SERVICE_ID == service.ID && p.ID == o.SERVICE_CONDITION_ID)).ToList();
                        }
                        if (dataCondition != null && dataCondition.Count > 0 && dataCondition.Exists(t => t.SERVICE_CONDITION_ID.HasValue && t.SERVICE_CONDITION_ID > 0) && !dataCondition.Exists(t => t.SERVICE_CONDITION_ID == null || t.SERVICE_CONDITION_ID == 0))
                        {
                            dataCondition = dataCondition.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.SERVICE_CONDITION_ID.HasValue && o.SERVICE_CONDITION_ID > 0 && o.SERVICE_ID == service.ID).ToList();
                            if (dataCondition != null && dataCondition.Count > 0)
                            {
                                List<V_HIS_SERVICE_PATY> dataConditionTmps = new List<V_HIS_SERVICE_PATY>();
                                foreach (var itemCon in dataCondition)
                                {
                                    if (dataConditionTmps.Count == 0 || !dataConditionTmps.Exists(t => t.SERVICE_CONDITION_NAME == itemCon.SERVICE_CONDITION_NAME && t.HEIN_RATIO == itemCon.HEIN_RATIO))
                                    {
                                        dataConditionTmps.Add(itemCon);
                                    }
                                }
                                dataCondition.Clear();
                                dataCondition.AddRange(dataConditionTmps);
                            }
                        }
                        else
                        {
                            dataCondition = null;
                        }
                        if (dataCondition != null && dataCondition.Count > 0 && (item.SERVICE_CONDITION_ID == null || item.SERVICE_CONDITION_ID == 0))
                        {
                            lstString.Add(service.SERVICE_NAME);
                        }
                    }
                }
                if (lstString != null && lstString.Count > 0)
                {
                    WaitingManager.Hide();
                    MessageBox.Show(string.Format("Dịch vụ {0} chưa được nhập điều kiện", string.Join(", ", lstString.Distinct().ToList())), "Thông báo");
                    valid = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }
        private bool ValidSereServWithCondition(HisBedHistoryADO row)
        {
            bool valid = true;
            try
            {
                if (row != null && row.IsBedStretcher && row.PATIENT_TYPE_ID != null)
                {
                    string sereServConditionStr = "";
                    var service = VHisBedServiceTypes.FirstOrDefault(o => o.ID == row.BED_SERVICE_TYPE_ID);
                    var dataCondition = BranchDataWorker.ServicePatyWithListPatientType(service.ID, new List<long> { row.PATIENT_TYPE_ID ?? 0 });
                    if (dataCondition != null && dataCondition.Count > 0 && lstConditionService != null && lstConditionService.Count > 0)
                    {
                        dataCondition = dataCondition.Where(o => lstConditionService.Exists(p => p.SERVICE_ID == service.ID && p.ID == o.SERVICE_CONDITION_ID)).ToList();
                    }
                    if (dataCondition != null && dataCondition.Count > 0 && dataCondition.Exists(t => t.SERVICE_CONDITION_ID.HasValue && t.SERVICE_CONDITION_ID > 0) && !dataCondition.Exists(t => t.SERVICE_CONDITION_ID == null || t.SERVICE_CONDITION_ID == 0))
                    {
                        dataCondition = dataCondition.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.SERVICE_CONDITION_ID.HasValue && o.SERVICE_CONDITION_ID > 0 && o.SERVICE_ID == service.ID).ToList();
                        if (dataCondition != null && dataCondition.Count > 0)
                        {
                            List<V_HIS_SERVICE_PATY> dataConditionTmps = new List<V_HIS_SERVICE_PATY>();
                            foreach (var itemCon in dataCondition)
                            {
                                if (dataConditionTmps.Count == 0 || !dataConditionTmps.Exists(t => t.SERVICE_CONDITION_NAME == itemCon.SERVICE_CONDITION_NAME && t.HEIN_RATIO == itemCon.HEIN_RATIO))
                                {
                                    dataConditionTmps.Add(itemCon);
                                }
                            }
                            dataCondition.Clear();
                            dataCondition.AddRange(dataConditionTmps);
                        }
                    }
                    else
                    {
                        dataCondition = null;
                    }
                    if (dataCondition != null && dataCondition.Count > 0 && (row.SERVICE_CONDITION_ID == null || row.SERVICE_CONDITION_ID == 0))
                    {
                        sereServConditionStr = service.SERVICE_NAME;
                    }

                    if (!String.IsNullOrEmpty(sereServConditionStr))
                    {
                        WaitingManager.Hide();
                        BreakServiceCondition = true;
                        MessageBox.Show(string.Format("Dịch vụ {0} chưa được nhập điều kiện", sereServConditionStr), "Thông báo");
                        valid = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }
    }
}
