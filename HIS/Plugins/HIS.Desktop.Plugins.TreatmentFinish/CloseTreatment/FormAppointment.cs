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
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraExport;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.TreatmentFinish.ADO;
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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TreatmentFinish.CloseTreatment
{
    public partial class FormAppointment : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        private long dtAppointment = 0;
        private int positionHandle = -1;
        private MOS.SDO.HisTreatmentFinishSDO currentTreatmentFinishSDO { get; set; }
        internal FormTreatmentFinish Form;
        internal delegate void GetString(MOS.SDO.HisTreatmentFinishSDO currentTreatmentFinishSDO);
        internal GetString MyGetData;

        List<RoomExamADO> _RoomExamADOs { get; set; }
        bool isCheckAll = true;

        private List<HisAppointmentPeriodCountByDateSDO> ListTime;
        private bool editdtTimeAppointments = false;
        private bool editcboTimeFrame = false;
        Inventec.Desktop.Common.Modules.Module moduleDataAppointment;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorkerAppointment;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateAppointmentRDO;
        const string moduleLink = "HIS.Desktop.Plugins.TreatmentFinish.CloseTreatment.FormAppointment";
        bool isNotLoadWhileChangeControlStateInFirstAppointment;

        private List<HisNumOrderBlockSDO> HisNumOrderBlockSDOs;
        ResultChooseNumOrderBlockADO resultChooseNumOrderBlockADO;
        bool initNumOderBlock;
        bool IsBlockOrder;
        private HisNumOrderBlockSDO NumOrderBlock;
        long? NumOrderBlockID = null;
        bool IsReturnClosed = false;
        bool IsRoomBlock = false;
        List<HisNumOrderBlockSDO> apiResult;

        HIS_TREATMENT currentTreatment { get; set; }
        #endregion

        #region Construct
        public FormAppointment()
        {
            InitializeComponent();
        }

        public FormAppointment(Inventec.Desktop.Common.Modules.Module _moduleDataAppointment, long _dtAppointment, bool IsBlockOrder)
            : base(_moduleDataAppointment)
        {
            InitializeComponent();
            try
            {
                this.dtAppointment = _dtAppointment;
                this.moduleDataAppointment = _moduleDataAppointment;
                this.IsBlockOrder = IsBlockOrder;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FormAppointment_Load(object sender, EventArgs e)
        {
            try
            {
                this.initNumOderBlock = true;
                SetIcon();

                LoadKeysFromlanguage();

                LoadRoomExam();

                SetDefaultValueControl();
                if (IsBlockOrder)
                {
                    this.Size = new Size(566, 650);
                    this.layoutControlItem10.Size = new System.Drawing.Size(550, 700);
                    this.lciTimeAppointments.Size = new Size(300, 26);

                    layoutControlItem8.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    layoutControlItem9.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    layoutControlItem10.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    layoutControlItem7.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutControlItem4.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lciTimeFrame.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutControlItem5.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    InitCboRoomEx();
                }
                else
                {
                    this.Size = new Size(566, 400);
                    layoutControlItem5.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    layoutControlItem8.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutControlItem9.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutControlItem10.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutControlItem7.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    layoutControlItem4.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    lciTimeFrame.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    InitCboTimeFrame();
                    if (HIS.Desktop.Plugins.TreatmentFinish.Config.ConfigKey.NumOrderIssueOption == "1")
                    {
                        editdtTimeAppointments = true;
                        if (currentTreatmentFinishSDO != null && currentTreatmentFinishSDO.AppointmentTime.HasValue)
                        {
                            cboTimeFrame.EditValue = currentTreatmentFinishSDO.AppointmentPeriodId;
                        }
                        else
                        {
                            cboTimeFrame.EditValue = ProcessGetFromTime(dtTimeAppointments.DateTime);
                        }
                        editdtTimeAppointments = false;
                    }
                }
                InitControlAppointmentState();
                ValidationTimeAppointments();
                
                this.initNumOderBlock = false;
                this.StartPosition = FormStartPosition.CenterParent;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion
        private void InitCboRoomEx()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("ROOM_CODE", "Mã", 100, 1));
                columnInfos.Add(new ColumnInfo("ROOM_NAME", "Tên phòng", 250, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("ROOM_NAME", "ID", columnInfos, true, 350);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(cboRoomEx, _RoomExamADOs, controlEditorADO);
                if (_RoomExamADOs.Where(o => o.IsCheck).ToList() != null && _RoomExamADOs.Where(o => o.IsCheck).ToList().Count > 0)
                {
                    cboRoomEx.EditValue = _RoomExamADOs.Where(o => o.IsCheck).ToList().FirstOrDefault().ID;
                }
                else
                {
                    cboRoomEx.EditValue = this.moduleDataAppointment.RoomId;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void setBlock()
        {
            try
            {

                CommonParam param = new CommonParam();
                HisNumOrderBlockOccupiedStatusFilter filter = new HisNumOrderBlockOccupiedStatusFilter();

                if (dtTimeAppointments.EditValue != null && dtTimeAppointments.DateTime != DateTime.MinValue)
                {
                    filter.ISSUE_DATE = Inventec.Common.TypeConvert.Parse.ToInt64(dtTimeAppointments.DateTime.ToString("yyyyMMdd") + "000000");
                }

                if (cboRoomEx.EditValue != null)
                {
                    filter.ROOM_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboRoomEx.EditValue.ToString());
                }

                apiResult = new BackendAdapter(param).Get<List<HisNumOrderBlockSDO>>("api/HisNumOrderBlock/GetOccupiedStatus", ApiConsumers.MosConsumer, filter, param);

                ProcessCreateTab(apiResult);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessCreateTab(List<HisNumOrderBlockSDO> dataNumOrder)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataNumOrder), dataNumOrder));
                xtraTabControl1.TabPages.Clear();
                if (dataNumOrder != null && dataNumOrder.Count > 0)
                {
                    var groupTime = dataNumOrder.GroupBy(o => new { o.ROOM_TIME_ID, o.ROOM_TIME_NAME, o.ROOM_TIME_FROM, o.ROOM_TIME_TO }).ToList();
                    foreach (var times in groupTime)
                    {
                        DevExpress.XtraTab.XtraTabPage tab = new DevExpress.XtraTab.XtraTabPage();
                        tab.Text = !String.IsNullOrWhiteSpace(times.Key.ROOM_TIME_NAME) ? times.Key.ROOM_TIME_NAME : string.Format("{0} - {1}", Base.GlobalVariablesProcess.GenerateHour(times.Key.ROOM_TIME_FROM), Base.GlobalVariablesProcess.GenerateHour(times.Key.ROOM_TIME_TO));
                        UCTimes uc = new UCTimes(times.ToList(), SelectNumOrder);
                        uc.Dock = DockStyle.Fill;
                        tab.Controls.Add(uc);
                        xtraTabControl1.TabPages.Add(tab);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SelectNumOrder(TimeADO data)
        {
            try
            {
                this.NumOrderBlock = data;
                if (data != null)
                {
                    this.lblStt.Text = data.NUM_ORDER + "";
                    long time = Inventec.Common.TypeConvert.Parse.ToInt64(dtTimeAppointments.DateTime.ToString("yyyyMMdd") + data.HOUR_STR.Replace(":", "") + "00");
                    dtTimeAppointments.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(time);
                    NumOrderBlockID = data.NUM_ORDER_BLOCK_ID;
                    Inventec.Common.Logging.LogSystem.Error(NumOrderBlockID + "_____NumOrderBlockID");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessLoadGetOccupiedStatusBlock()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("ProcessLoadGetOccupiedStatus.1");
                List<HisNumOrderBlockSDO> HisNumOrderBlockSDOs = new List<HisNumOrderBlockSDO>();
                NumOrderBlockID = null;
                HisNumOrderBlockOccupiedStatusFilter filter = new HisNumOrderBlockOccupiedStatusFilter();
                filter.ISSUE_DATE = Inventec.Common.TypeConvert.Parse.ToInt64(dtTimeAppointments.DateTime.ToString("yyyyMMdd") + "000000");
                filter.ROOM_ID = Int64.Parse(cboRoomEx.EditValue.ToString());
                var HisNumOrderBlockSDOTmps = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HisNumOrderBlockSDO>>("api/HisNumOrderBlock/GetOccupiedStatus", ApiConsumers.MosConsumer, filter, null);
                if (HisNumOrderBlockSDOTmps != null && HisNumOrderBlockSDOTmps.Count > 0)
                {
                    HisNumOrderBlockSDOs = HisNumOrderBlockSDOTmps.Where(o => o.IS_ISSUED == null || o.IS_ISSUED != 1).OrderBy(o => o.FROM_TIME).ToList();
                    if (!(HisNumOrderBlockSDOs != null && HisNumOrderBlockSDOs.Count > 0))
                    {
                        Inventec.Common.Logging.LogSystem.Debug("ProcessLoadGetOccupiedStatus.3");
                        List<HisAppointmentPeriodCountByDateSDO> ListTime = new List<HisAppointmentPeriodCountByDateSDO>();
                        NumOrderBlockID = null;
                        DevExpress.XtraEditors.XtraMessageBox.Show(String.Format(ResourceMessage.DaCapHetSoKhamVaoNgay, dtTimeAppointments.DateTime.ToString("dd/MM")),
                             Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                    }
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(String.Format(ResourceMessage.DaCapHetSoKhamVaoNgay, dtTimeAppointments.DateTime.ToString("dd/MM")),
                             Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                    NumOrderBlockID = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitCboTimeFrame()
        {
            try
            {
                if (HIS.Desktop.Plugins.TreatmentFinish.Config.ConfigKey.NumOrderIssueOption == "1")
                {
                    layoutControlItem7.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    cboTimeFrame.Properties.Buttons[0].Visible = cboTimeFrame.Properties.Buttons[1].Visible = true;
                    cboTimeFrame.Enabled = true;
                    cboTimeFrame.Properties.ReadOnly = false;
                    ProcessLoadAppointmentCount();
                    cboTimeFrame.Properties.DataSource = ListTime;
                    cboTimeFrame.Properties.DisplayMember = "TIME_FRAME";
                    cboTimeFrame.Properties.ValueMember = "ID";
                    cboTimeFrame.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                    cboTimeFrame.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                    cboTimeFrame.Properties.ImmediatePopup = true;
                    cboTimeFrame.ForceInitialize();
                    cboTimeFrame.Properties.View.Columns.Clear();
                    cboTimeFrame.Properties.PopupFormSize = new System.Drawing.Size(300, 250);

                    if (cboTimeFrame.Properties.View.Columns.Count == 0)
                    {
                        GridColumn aColumnCode = cboTimeFrame.Properties.View.Columns.AddField("TIME_FRAME");
                        aColumnCode.Caption = "Mã";
                        aColumnCode.Visible = true;
                        aColumnCode.VisibleIndex = 1;
                        aColumnCode.Width = 100;
                        aColumnCode.UnboundType = DevExpress.Data.UnboundColumnType.Object;

                        GridColumn aColumnName = cboTimeFrame.Properties.View.Columns.AddField("COUNT_TIME_FRAME");
                        aColumnName.Caption = "Tên";
                        aColumnName.Visible = true;
                        aColumnName.VisibleIndex = 2;
                        aColumnName.Width = 200;
                        aColumnName.UnboundType = DevExpress.Data.UnboundColumnType.Object;
                    }
                }
                else if (HIS.Desktop.Plugins.TreatmentFinish.Config.ConfigKey.NumOrderIssueOption == "2")
                {
                    layoutControlItem7.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    cboTimeFrame.Properties.Buttons[0].Visible = cboTimeFrame.Properties.Buttons[1].Visible = false;
                    cboTimeFrame.Enabled = false;
                    cboTimeFrame.Properties.ReadOnly = true;
                    cboTimeFrame.Properties.DataSource = HisNumOrderBlockSDOs;
                    if (cboTimeFrame.Properties.View.Columns.Count == 0)
                    {
                        cboTimeFrame.Properties.DisplayMember = "TIME_FRAME";
                        cboTimeFrame.Properties.ValueMember = "NUM_ORDER_BLOCK_ID";
                        cboTimeFrame.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                        cboTimeFrame.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                        cboTimeFrame.Properties.ImmediatePopup = true;
                        cboTimeFrame.ForceInitialize();
                        cboTimeFrame.Properties.View.Columns.Clear();
                        cboTimeFrame.Properties.PopupFormSize = new System.Drawing.Size(300, 250);

                        GridColumn aColumnCode = cboTimeFrame.Properties.View.Columns.AddField("TIME_FRAME");
                        aColumnCode.Caption = "Khung giờ";
                        aColumnCode.Visible = true;
                        aColumnCode.VisibleIndex = 1;
                        aColumnCode.Width = 200;
                        aColumnCode.UnboundType = DevExpress.Data.UnboundColumnType.Object;
                    }
                    ProcessLoadGetOccupiedStatus();

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessLoadGetOccupiedStatus()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("ProcessLoadGetOccupiedStatus.1");
                cboTimeFrame.EditValue = null;
                HisNumOrderBlockSDOs = new List<HisNumOrderBlockSDO>();

                HisNumOrderBlockOccupiedStatusFilter filter = new HisNumOrderBlockOccupiedStatusFilter();
                filter.ISSUE_DATE = Inventec.Common.TypeConvert.Parse.ToInt64(dtTimeAppointments.DateTime.ToString("yyyyMMdd") + "000000");
                if (_RoomExamADOs != null && _RoomExamADOs.Count > 0 && _RoomExamADOs.Exists(o => o.IsCheck))
                {
                    filter.ROOM_ID = _RoomExamADOs.Where(o => o.IsCheck).FirstOrDefault().ID;
                }
                var HisNumOrderBlockSDOTmps = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HisNumOrderBlockSDO>>("api/HisNumOrderBlock/GetOccupiedStatus", ApiConsumers.MosConsumer, filter, null);
                if (HisNumOrderBlockSDOTmps != null && HisNumOrderBlockSDOTmps.Count > 0)
                {
                    HisNumOrderBlockSDOs = HisNumOrderBlockSDOTmps.Where(o => o.IS_ISSUED == null || o.IS_ISSUED != 1).OrderBy(o => o.FROM_TIME).ToList();
                    if (HisNumOrderBlockSDOs != null && HisNumOrderBlockSDOs.Count > 0)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("ProcessLoadGetOccupiedStatus.2");
                        cboTimeFrame.Properties.DataSource = HisNumOrderBlockSDOs;
                        cboTimeFrame.Update();

                        if (resultChooseNumOrderBlockADO != null && resultChooseNumOrderBlockADO.NumOrderBlock != null && resultChooseNumOrderBlockADO.NumOrderBlock.NUM_ORDER_BLOCK_ID > 0)
                        {
                            cboTimeFrame.EditValue = resultChooseNumOrderBlockADO.NumOrderBlock.NUM_ORDER_BLOCK_ID;
                        }
                        else
                            cboTimeFrame.EditValue = HisNumOrderBlockSDOs.FirstOrDefault().NUM_ORDER_BLOCK_ID;
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("HisNumOrderBlockSDOs.FirstOrDefault()", HisNumOrderBlockSDOs.FirstOrDefault())
                            + Inventec.Common.Logging.LogUtil.TraceData("resultChooseNumOrderBlockADO", resultChooseNumOrderBlockADO));
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Debug("ProcessLoadGetOccupiedStatus.3");
                        ListTime = new List<HisAppointmentPeriodCountByDateSDO>();
                        cboTimeFrame.EditValue = null;
                        DevExpress.XtraEditors.XtraMessageBox.Show(String.Format(ResourceMessage.DaCapHetSoKhamVaoNgay, dtTimeAppointments.DateTime.ToString("dd/MM")),
                             Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                    }
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(String.Format(ResourceMessage.DaCapHetSoKhamVaoNgay, dtTimeAppointments.DateTime.ToString("dd/MM")),
                            Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => filter), filter)
                   + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisNumOrderBlockSDOTmps), HisNumOrderBlockSDOTmps)
                   + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisNumOrderBlockSDOs), HisNumOrderBlockSDOs));
                Inventec.Common.Logging.LogSystem.Debug("ProcessLoadGetOccupiedStatus.4");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessLoadAppointmentCount()
        {
            try
            {
                ListTime = new List<HisAppointmentPeriodCountByDateSDO>();

                HisAppointmentPeriodCountByDateFilter filter = new HisAppointmentPeriodCountByDateFilter();
                filter.APPOINTMENT_DATE = Inventec.Common.TypeConvert.Parse.ToInt64(dtTimeAppointments.DateTime.ToString("yyyyMMdd") + "000000");
                filter.BRANCH_ID = HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId();
                filter.IS_ACTIVE = 1;
                ListTime = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HisAppointmentPeriodCountByDateSDO>>("api/HisAppointmentPeriod/GetCountByDate", ApiConsumers.MosConsumer, filter, null);
                if (ListTime != null && ListTime.Count > 0)
                {
                    ListTime = ListTime.OrderBy(o => o.FROM_HOUR).ThenBy(o => o.FROM_MINUTE).ThenBy(o => o.TO_HOUR).ThenBy(o => o.TO_MINUTE).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadRoomExam()
        {
            try
            {
                gridControlRoomExam.DataSource = null;

                var _vHisExecuteRooms = BackendDataWorker.Get<HIS_EXECUTE_ROOM>().Where(p =>
                     p.IS_ACTIVE == 1
                     && p.IS_EXAM == 1).ToList();

                _RoomExamADOs = new List<RoomExamADO>();
                foreach (var item in _vHisExecuteRooms)
                {
                    RoomExamADO ado = new RoomExamADO()
                    {
                        EXECUTE_ROOM_ID = item.ID,
                        ID = item.ROOM_ID,
                        ROOM_CODE = item.EXECUTE_ROOM_CODE,
                        ROOM_NAME = item.EXECUTE_ROOM_NAME,
                        MAX_APPOINTMENT_BY_DAY = item.MAX_APPOINTMENT_BY_DAY
                    };

                    var _room = BackendDataWorker.Get<HIS_ROOM>().Where(p => p.ID == item.ROOM_ID).FirstOrDefault();
                    if (_room != null)
                    {
                        ado.IS_BLOCK_NUM_ORDER = _room.IS_BLOCK_NUM_ORDER;
                    }

                    _RoomExamADOs.Add(ado);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region Private method

        private void SetAppoinmentTime()
        {
            try
            {
                long priorityAppoinmentTime = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<long>(Config.TreatmentEndCFG.PRESCRIPTION_TIME_AND_APPOINTMENT_TIME_KEY);
                if (priorityAppoinmentTime == 1)
                {
                    MOS.Filter.HisServiceReqFilter filter = new MOS.Filter.HisServiceReqFilter();
                    filter.TREATMENT_ID = Form.treatmentId;
                    filter.SERVICE_REQ_TYPE_IDs = new List<long> { IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK };
                    filter.ORDER_DIRECTION = "DESC";
                    filter.ORDER_FIELD = "CREATE_TIME";
                    var serviceReqs = new Inventec.Common.Adapter.BackendAdapter(new Inventec.Core.CommonParam()).Get<List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, filter, null);

                    var serviceReq = serviceReqs.Where(o => o.USE_TIME_TO.HasValue).OrderByDescending(o => o.USE_TIME_TO).FirstOrDefault();
                    if (serviceReq != null)
                    {
                        DateTime dtUseTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(serviceReq.USE_TIME_TO.Value) ?? DateTime.MinValue;
                        dtTimeAppointments.DateTime = dtUseTime.AddDays((double)1);
                        return;
                    }
                }

                long appoinmentTimeDefault = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(Config.TreatmentEndCFG.TREATMENT_END___APPOINTMENT_TIME_DEFAULT_KEY));
                if (appoinmentTimeDefault > 0)
                {
                    long endTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(Form.dtEndTime.DateTime) ?? 0;
                    if (endTime > 0)
                    {
                        dtTimeAppointments.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Calculation.Add(endTime, appoinmentTimeDefault - 1, Inventec.Common.DateTime.Calculation.UnitDifferenceTime.DAY) ?? 0) ?? DateTime.Now;
                    }
                    else
                    {
                        dtTimeAppointments.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(
                            Inventec.Common.DateTime.Calculation.Add(Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0,
                            appoinmentTimeDefault - 1,
                            Inventec.Common.DateTime.Calculation.UnitDifferenceTime.DAY
                            ) ?? 0) ?? DateTime.Now;
                    }
                }
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
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.TreatmentFinish.Resources.Lang", typeof(FormAppointment).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("FormAppointment.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblStt.Text = Inventec.Common.Resource.Get.Value("FormAppointment.lblStt.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboRoomEx.Properties.NullText = Inventec.Common.Resource.Get.Value("FormAppointment.cboRoomEx.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("FormAppointment.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItemSave.Caption = Inventec.Common.Resource.Get.Value("FormAppointment.barButtonItemSave.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnChonKhungGio.ToolTip = Inventec.Common.Resource.Get.Value("FormAppointment.bbtnChonKhungGio.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAppointmentService.Text = Inventec.Common.Resource.Get.Value("FormAppointment.btnAppointmentService.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtAdvise.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("FormAppointment.txtAdvise.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboTimeFrame.Properties.NullText = Inventec.Common.Resource.Get.Value("FormAppointment.cboTimeFrame.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtSearch.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("FormAppointment.txtSearch.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridViewRoomExam.OptionsFind.FindNullPrompt = Inventec.Common.Resource.Get.Value("FormAppointment.gridViewRoomExam.OptionsFind.FindNullPrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("FormAppointment.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("FormAppointment.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("FormAppointment.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTimeAppointments.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("FormAppointment.lciTimeAppointments.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTimeAppointments.Text = Inventec.Common.Resource.Get.Value("FormAppointment.lciTimeAppointments.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem4.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("FormAppointment.layoutControlItem4.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem4.Text = Inventec.Common.Resource.Get.Value("FormAppointment.layoutControlItem4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("FormAppointment.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("FormAppointment.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTimeFrame.Text = Inventec.Common.Resource.Get.Value("FormAppointment.lciTimeFrame.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciAdvise.Text = Inventec.Common.Resource.Get.Value("FormAppointment.lciAdvise.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("FormAppointment.layoutControlItem8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem9.Text = Inventec.Common.Resource.Get.Value("FormAppointment.layoutControlItem9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem10.Text = Inventec.Common.Resource.Get.Value("FormAppointment.layoutControlItem10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("FormAppointment.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblKhongCanhBaoNgayNghi.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("FormAppointment.lblKhongCanhBaoNgayNghi.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblKhongCanhBaoNgayNghi.Text = Inventec.Common.Resource.Get.Value("FormAppointment.lblKhongCanhBaoNgayNghi.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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
                currentTreatmentFinishSDO = Form.hisTreatmentFinishSDO_process;

                if (Form.currentHisTreatment != null) this.currentTreatment = Form.currentHisTreatment;

                if (currentTreatmentFinishSDO.AppointmentTime.HasValue)
                {
                    DateTime? timeAppointment = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentTreatmentFinishSDO.AppointmentTime.Value);
                    dtTimeAppointments.EditValue = timeAppointment;
                }
                else
                {
                    SetAppoinmentTime();
                }

                txtAdvise.Text = currentTreatmentFinishSDO.Advise;

                if (HIS.Desktop.Plugins.TreatmentFinish.Config.ConfigKey.NumOrderIssueOption == "1")
                {
                    if (currentTreatmentFinishSDO.AppointmentPeriodId.HasValue)
                    {
                        cboTimeFrame.EditValue = currentTreatmentFinishSDO.AppointmentPeriodId;
                    }
                    else if (Form.currentHisTreatment != null && Form.currentHisTreatment.APPOINTMENT_PERIOD_ID.HasValue)
                    {
                        cboTimeFrame.EditValue = Form.currentHisTreatment.APPOINTMENT_PERIOD_ID;
                    }
                }

                //xuandv
                if (Form.currentHisTreatment != null
                    && !string.IsNullOrEmpty(Form.currentHisTreatment.APPOINTMENT_EXAM_ROOM_IDS)
                    && _RoomExamADOs != null && _RoomExamADOs.Count > 0)
                {
                    string[] ids = Form.currentHisTreatment.APPOINTMENT_EXAM_ROOM_IDS.Split(',');
                    foreach (var item in _RoomExamADOs)
                    {
                        var dataCheck = ids.FirstOrDefault(p => p.Trim() == item.ID.ToString().Trim());
                        if (!string.IsNullOrEmpty(dataCheck))
                        {
                            item.IsCheck = true;
                            if (HIS.Desktop.Plugins.TreatmentFinish.Config.ConfigKey.NumOrderIssueOption == "2")
                            {
                                break;
                            }
                        }
                    }
                }
                else if (currentTreatmentFinishSDO != null
                    && currentTreatmentFinishSDO.AppointmentExamRoomIds != null
                    && currentTreatmentFinishSDO.AppointmentExamRoomIds.Count > 0
                    && _RoomExamADOs != null && _RoomExamADOs.Count > 0)
                {
                    if (HIS.Desktop.Plugins.TreatmentFinish.Config.ConfigKey.NumOrderIssueOption == "2")
                    {
                        _RoomExamADOs.ForEach(k => k.IsCheck = false);
                    }
                    foreach (var item in _RoomExamADOs)
                    {
                        long dataCheck = currentTreatmentFinishSDO.AppointmentExamRoomIds.FirstOrDefault(p => p == item.ID);
                        if (dataCheck > 0)
                        {
                            item.IsCheck = true;
                            if (HIS.Desktop.Plugins.TreatmentFinish.Config.ConfigKey.NumOrderIssueOption == "2")
                            {
                                break;
                            }
                        }
                    }
                }
                _RoomExamADOs = _RoomExamADOs.OrderByDescending(p => p.IsCheck).ThenBy(p => p.ROOM_CODE).ToList();

                gridControlRoomExam.BeginUpdate();
                gridControlRoomExam.DataSource = _RoomExamADOs;
                gridControlRoomExam.EndUpdate();
                dtTimeAppointments.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool GetAppoinmentCountTrue()
        {
            bool count = false;
            try
            {
                CommonParam param = new CommonParam();
                HisAppointmentServFilter appointmentServFilter = new HisAppointmentServFilter();
                appointmentServFilter.TREATMENT_ID = Form.currentHisTreatment.ID;
                List<HIS_APPOINTMENT_SERV> appoinmentServs = new BackendAdapter(param)
                    .Get<List<HIS_APPOINTMENT_SERV>>("api/HisAppointmentServ/Get", ApiConsumers.MosConsumer, appointmentServFilter, param);
                if (appoinmentServs != null && appoinmentServs.Count > 0)
                {
                    count = true;
                }
                return count;
            }
            catch (Exception ex)
            {
                return count;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return count;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                IsReturnClosed = false;

                if (IsRoomBlock && NumOrderBlockID == null)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.BanChuaChonStt,
                        ResourceMessage.ThongBao);
                    return;
                }


                this.positionHandle = -1;
                if (!dxValidationProvider.Validate())
                {
                    return;
                }

                if (Config.ConfigKey.MustChooseSeviceInCaseOfAppointment == "1")
                {
                    if (!GetAppoinmentCountTrue())
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.BanBatBuocChonDichVuHenKhamKhiKetThucDieuTri,
                        "", MessageBoxButtons.OK);
                        return;
                    }
                }

                if (Config.ConfigKey.AutoCreateWhenAppointment == "1" &&
                    this.currentTreatment != null &&
                    this.currentTreatment.TDL_PATIENT_TYPE_ID == BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == Config.ConfigKey.PatienTypeCode_BHYT).ID &&
                    dtTimeAppointments.EditValue != null &&
                    currentTreatment.TDL_HEIN_CARD_TO_TIME != null &&
                    Convert.ToInt64(dtTimeAppointments.DateTime.ToString("yyyyMMdd")) > Convert.ToInt64(currentTreatment.TDL_HEIN_CARD_TO_TIME.ToString().Substring(0, 8)))
                {
                    string _HanThe = Inventec.Common.DateTime.Convert.TimeNumberToDateString(currentTreatment.TDL_HEIN_CARD_TO_TIME ?? 0);
                    string _TGHenKham = Inventec.Common.DateTime.Convert.SystemDateTimeToDateString(dtTimeAppointments.DateTime);
                    string _DoiTuongBN = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == Config.ConfigKey.patientTypeCodeHospitalFee).PATIENT_TYPE_NAME;
                    if (MessageBox.Show(string.Format(ResourceMessage.TheBHYTCuaBNSeHetHanVaoNgay, _HanThe, _TGHenKham, _DoiTuongBN), ResourceMessage.ThongBao, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    {
                        dtTimeAppointments.Focus();
                        return;
                    }
                }

                //Kiểm tra nếu số ngày hẹn khám vượt quá số ngày được cấu hình
                if (Config.ConfigKey.MaxOfAppointmentDays.HasValue && spDay.Value > Config.ConfigKey.MaxOfAppointmentDays.Value)
                {
                    //Tùy chọn xử lý trong trường hợp vượt quá số ngày hẹn khám. 1: Cảnh báo. 2: Chặn, không cho xử trí
                    if (Config.ConfigKey.WarningOptionWhenExceedingMaxOfAppointmentDays == 2)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(string.Format(ResourceMessage.CanhBaoNgayHenToiDa, Config.ConfigKey.MaxOfAppointmentDays),
                        ResourceMessage.ThongBao,
                        MessageBoxButtons.OK);
                        return;
                    }
                    else if (Config.ConfigKey.WarningOptionWhenExceedingMaxOfAppointmentDays == 1)
                    {
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(string.Format(ResourceMessage.BanCoMuonTiepTuc, string.Format(ResourceMessage.CanhBaoNgayHenToiDa, Config.ConfigKey.MaxOfAppointmentDays)),
                        ResourceMessage.ThongBao,
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;
                    }
                }
                if (!chkKhongCanhBaoNgayNghi.Checked)
                {
                    if (dtTimeAppointments.DateTime.DayOfWeek == DayOfWeek.Sunday)
                    {
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.CanhBaoNgayHenLaChuNhat,
                        ResourceMessage.ThongBao,
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;
                    }
                    else if (dtTimeAppointments.DateTime.DayOfWeek == DayOfWeek.Saturday)
                    {
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.CanhBaoNgayHenLaThuBay,
                        ResourceMessage.ThongBao,
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;
                    }
                }

                long dtAppointmentTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtTimeAppointments.DateTime) ?? 0;

                if (dtAppointmentTime >= dtAppointment)
                {
                    if (IsBlockOrder)
                    {
                        IsReturnClosed = true;
                        List<long> lst = new List<long>();
                        lst.Add(Int64.Parse(cboRoomEx.EditValue.ToString()));
                        currentTreatmentFinishSDO.AppointmentExamRoomIds = lst;
                    }
                    else
                    {
                        IsReturnClosed = true;
                        var datas = (List<RoomExamADO>)gridControlRoomExam.DataSource;
                        if (datas != null && datas.Count > 0)
                        {
                            List<RoomExamADO> seleted = datas.Where(p => p.IsCheck).ToList();

                            if (seleted != null && seleted.Count > 0)
                            {
                                if (!this.CheckMaxAppointment(seleted))
                                {
                                    return;
                                }
                                currentTreatmentFinishSDO.AppointmentExamRoomIds = seleted.Select(s => s.ID).Distinct().ToList(); ;
                            }
                            else
                            {
                                currentTreatmentFinishSDO.AppointmentExamRoomIds = null;
                            }
                        }
                        else
                        {
                            currentTreatmentFinishSDO.AppointmentExamRoomIds = null;
                        }
                    }

                    currentTreatmentFinishSDO.Advise = txtAdvise.Text;
                    currentTreatmentFinishSDO.AppointmentTime = dtAppointmentTime;
                    this.Form.txtAdvised.Text = txtAdvise.Text;

                    if (HIS.Desktop.Plugins.TreatmentFinish.Config.ConfigKey.NumOrderIssueOption == "1")
                    {
                        if (cboTimeFrame.EditValue != null)
                        {
                            currentTreatmentFinishSDO.AppointmentPeriodId = Inventec.Common.TypeConvert.Parse.ToInt64(cboTimeFrame.EditValue.ToString());
                        }
                        else
                        {
                            currentTreatmentFinishSDO.AppointmentPeriodId = null;
                        }
                    }
                    else if (HIS.Desktop.Plugins.TreatmentFinish.Config.ConfigKey.NumOrderIssueOption == "2")
                    {
                        if (cboTimeFrame.EditValue != null && Inventec.Common.TypeConvert.Parse.ToInt64(cboTimeFrame.EditValue.ToString()) > 0)
                        {
                            currentTreatmentFinishSDO.NumOrderBlockId = Inventec.Common.TypeConvert.Parse.ToInt64(cboTimeFrame.EditValue.ToString());
                        }
                    }
                    if (IsBlockOrder)
                    {
                        currentTreatmentFinishSDO.AppointmentPeriodId = null;
                        currentTreatmentFinishSDO.NumOrderBlockId = NumOrderBlockID;
                    }


                    MyGetData(currentTreatmentFinishSDO);
                    this.Close();
                }
                else
                {
                    MessageBox.Show(ResourceMessage.CanhBaoThoiGianHenKhamSoVoiThoiGianKetThucDieuTri);
                    dtTimeAppointments.Focus();
                    dtTimeAppointments.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region Validation
        private void dxValidationProvider_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                DevExpress.XtraEditors.BaseEdit edit = e.InvalidControl as DevExpress.XtraEditors.BaseEdit;
                if (edit == null)
                    return;

                DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo viewInfo = edit.GetViewInfo() as DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo;
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

        private void ValidationTimeAppointments()
        {
            try
            {
                Validation.DateEditValidationRule icdMainRule = new Validation.DateEditValidationRule();
                icdMainRule.dtDateEdit = dtTimeAppointments;
                icdMainRule.ErrorText = ResourceMessage.TruongDuLieuBatBuoc;
                icdMainRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider.SetValidationRule(dtTimeAppointments, icdMainRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion
        #endregion

        #region Shotcut
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
        #endregion

        private bool CheckMaxAppointment(List<RoomExamADO> selected)
        {
            bool result = true;
            try
            {
                LogSystem.Debug("Selected: \n" + LogUtil.TraceData("seleted", selected));
                List<long> executeRoomIds = selected != null ? selected.Where(o => (o.MAX_APPOINTMENT_BY_DAY ?? 0) > 0).Select(s => s.EXECUTE_ROOM_ID).ToList() : null;
                if (executeRoomIds != null && executeRoomIds.Count > 0)
                {
                    HisExecuteRoomAppointedFilter filter = new HisExecuteRoomAppointedFilter();
                    filter.EXECUTE_ROOM_IDs = executeRoomIds;
                    filter.INTR_OR_APPOINT_DATE = Convert.ToInt64(dtTimeAppointments.DateTime.ToString("yyyyMMdd") + "000000");
                    LogSystem.Debug("Filter: \n" + LogUtil.TraceData("Filter", filter));

                    List<HisExecuteRoomAppointedSDO> sdos = new BackendAdapter(new CommonParam()).Get<List<HisExecuteRoomAppointedSDO>>("api/HisExecuteRoom/GetCountAppointed", ApiConsumers.MosConsumer, filter, null);
                    List<HisExecuteRoomAppointedSDO> overs = sdos != null ? sdos.Where(o => (o.MaxAmount ?? 0) > 0 && (o.CurrentAmount ?? 0) > 0 && o.CurrentAmount.Value >= o.MaxAmount).ToList() : null;
                    LogSystem.Debug("sdos: \n" + LogUtil.TraceData("sdos", sdos));
                    if (overs != null && overs.Count > 0)
                    {
                        string names = String.Join(", ", overs.Select(s => String.Format("{0}({1}/{2})", s.ExecuteRoomName, s.CurrentAmount, s.MaxAmount)).ToList());
                        string mess = String.Format(ResourceMessage.PhongKhamCoSoLuotKhamVuotDinhMuc, names);
                        if (XtraMessageBox.Show(mess, ResourceMessage.ThongBao, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != System.Windows.Forms.DialogResult.Yes)
                            return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void dtTimeAppointments_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (IsBlockOrder)
                {
                }
                else
                {
                    if (!editcboTimeFrame)
                    {
                        editdtTimeAppointments = true;
                        DateEdit editor = sender as DateEdit;
                        if (editor != null)
                        {
                            this.CalculateDayNum();

                            if (editor.OldEditValue != null && HIS.Desktop.Plugins.TreatmentFinish.Config.ConfigKey.NumOrderIssueOption == "1")
                            {
                                DateTime oldValue = (DateTime)editor.OldEditValue;
                                if (oldValue != DateTime.MinValue && (editor.DateTime.Day != oldValue.Day || editor.DateTime.Month != oldValue.Month || editor.DateTime.Year != oldValue.Year))
                                {
                                    cboTimeFrame.EditValue = null;
                                    cboTimeFrame.Properties.DataSource = null;
                                    ProcessLoadAppointmentCount();
                                    cboTimeFrame.Properties.DataSource = ListTime;
                                    cboTimeFrame.EditValue = ProcessGetFromTime(editor.DateTime);
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
            finally
            {
                editdtTimeAppointments = false;
            }
        }

        private void spinSickLeaveDay_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                SpinEdit editor = sender as SpinEdit;
                if (editor != null && editor.EditorContainsFocus)
                    this.CalculateDateTo();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CalculateDayNum()
        {
            try
            {
                if (dtTimeAppointments.EditValue != null)
                {
                    TimeSpan ts = (TimeSpan)(dtTimeAppointments.DateTime.Date - DateTime.Now.Date);
                    spDay.Value = ts.Days;
                    if (HIS.Desktop.Plugins.TreatmentFinish.Config.ConfigKey.NumOrderIssueOption == "1")
                    {
                        cboTimeFrame.EditValue = ProcessGetFromTime(dtTimeAppointments.DateTime);
                    }
                    else if (HIS.Desktop.Plugins.TreatmentFinish.Config.ConfigKey.NumOrderIssueOption == "2" && !initNumOderBlock)
                    {
                        ProcessLoadGetOccupiedStatus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CalculateDateTo()
        {
            try
            {
                if (dtTimeAppointments.EditValue != null)
                {
                    DateTime appoint = DateTime.Now.AddDays((double)(spDay.Value));
                    dtTimeAppointments.DateTime = new DateTime(appoint.Year, appoint.Month, appoint.Day, dtTimeAppointments.DateTime.Hour, dtTimeAppointments.DateTime.Minute, dtTimeAppointments.DateTime.Second);
                    if (IsBlockOrder)
                    {
                        if (cboRoomEx.EditValue != null)
                        {
                            xtraTabControl1.TabPages.Clear();
                            var dataRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == Int64.Parse(cboRoomEx.EditValue.ToString()));
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataRoom), dataRoom));
                            if (dataRoom.IS_BLOCK_NUM_ORDER != null && dataRoom.IS_BLOCK_NUM_ORDER == 1)
                            {
                                setBlock();
                                ProcessLoadGetOccupiedStatusBlock();
                            }
                            else
                            {
                                xtraTabControl1.TabPages.Clear();
                            }
                        }
                        else
                        {
                            xtraTabControl1.TabPages.Clear();
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewRoomExam_MouseDown(object sender, MouseEventArgs e)
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
                                if (this._RoomExamADOs != null && this._RoomExamADOs.Count > 0)
                                {
                                    var dataChecks = this._RoomExamADOs.Where(p => p.IsCheck).ToList();
                                    if (dataChecks != null && dataChecks.Count > 0)
                                    {
                                        gridColumnCheck.Image = imageListIcon.Images[5];
                                    }
                                    else
                                    {
                                        gridColumnCheck.Image = imageListIcon.Images[6];
                                    }
                                }
                            }
                            (e as DevExpress.Utils.DXMouseEventArgs).Handled = true;
                        }
                    }
                    if (hi.HitTest == GridHitTest.Column)
                    {
                        if (hi.Column.FieldName == "IsCheck")
                        {
                            gridColumnCheck.Image = imageListIcon.Images[5];
                            gridViewRoomExam.BeginUpdate();
                            if (this._RoomExamADOs == null)
                                this._RoomExamADOs = new List<RoomExamADO>();
                            if (isCheckAll == true)
                            {
                                if (HIS.Desktop.Plugins.TreatmentFinish.Config.ConfigKey.NumOrderIssueOption == "1")
                                    foreach (var item in this._RoomExamADOs)
                                    {
                                        item.IsCheck = true;
                                    }
                                isCheckAll = false;
                            }
                            else
                            {
                                gridColumnCheck.Image = imageListIcon.Images[6];
                                foreach (var item in this._RoomExamADOs)
                                {
                                    item.IsCheck = false;
                                }
                                isCheckAll = true;
                            }
                            gridViewRoomExam.EndUpdate();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSearch_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                string str = txtSearch.Text.Trim();
                List<RoomExamADO> _RoomExamADONews = new List<RoomExamADO>();
                if (!string.IsNullOrEmpty(str))
                {
                    _RoomExamADONews = _RoomExamADOs.Where(p => p.ROOM_CODE.ToUpper().Contains(str.ToUpper())
                        || p.ROOM_NAME.ToUpper().Contains(str.ToUpper())).ToList();
                }
                else
                {
                    _RoomExamADONews = _RoomExamADOs;
                }
                _RoomExamADONews = _RoomExamADONews.OrderByDescending(p => p.IsCheck).ThenBy(p => p.ROOM_CODE).ToList();

                gridControlRoomExam.BeginUpdate();
                gridControlRoomExam.DataSource = _RoomExamADONews;
                gridControlRoomExam.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridLookUpEdit1View_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                if (HIS.Desktop.Plugins.TreatmentFinish.Config.ConfigKey.NumOrderIssueOption == "1")
                {
                    var row = (HisAppointmentPeriodCountByDateSDO)gridLookUpEdit1View.GetRow(e.RowHandle);
                    if (row != null)
                    {
                        if (row.CURRENT_COUNT >= (row.MAXIMUM ?? 0) && (row.MAXIMUM ?? 0) > 0)
                        {
                            e.Appearance.ForeColor = Color.Red;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spDay_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (ListTime != null && ListTime.Count > 0)
                    {
                        cboTimeFrame.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTimeFrame_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (HIS.Desktop.Plugins.TreatmentFinish.Config.ConfigKey.NumOrderIssueOption == "1")
                {
                    if (cboTimeFrame.EditValue != null && !editdtTimeAppointments)
                    {
                        editcboTimeFrame = true;
                        string id = cboTimeFrame.EditValue.ToString();
                        var rowdata = ListTime.FirstOrDefault(o => o.ID == Convert.ToInt64(id));
                        if (rowdata != null)
                        {
                            string timeStr = dtTimeAppointments.DateTime.ToString("HHmm");
                            long time = Inventec.Common.TypeConvert.Parse.ToInt64(timeStr);
                            long timeFrom = Inventec.Common.TypeConvert.Parse.ToInt64(string.Format("{0:00}", rowdata.FROM_HOUR ?? 0) + string.Format("{0:00}", rowdata.FROM_MINUTE ?? 0));
                            long timeto = Inventec.Common.TypeConvert.Parse.ToInt64(string.Format("{0:00}", rowdata.TO_HOUR ?? 23) + "" + string.Format("{0:00}", rowdata.TO_MINUTE ?? 59));
                            if (!(timeFrom <= time && time <= timeto))
                            {
                                int hour = (int)(rowdata.FROM_HOUR ?? 0);
                                int minute = (int)(rowdata.FROM_MINUTE ?? 0);
                                dtTimeAppointments.DateTime = new DateTime(dtTimeAppointments.DateTime.Year, dtTimeAppointments.DateTime.Month, dtTimeAppointments.DateTime.Day, hour, minute, 0);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            finally
            {
                editcboTimeFrame = false;
            }
        }

        private long? ProcessGetFromTime(DateTime dateTime)
        {
            long? result = null;
            try
            {
                if (ListTime != null && ListTime.Count > 0 && dateTime != DateTime.MinValue)
                {
                    string timeStr = dateTime.ToString("HHmm");
                    long time = Inventec.Common.TypeConvert.Parse.ToInt64(timeStr);
                    var lstTimefe = ListTime.Where(o =>
                        Inventec.Common.TypeConvert.Parse.ToInt64(string.Format("{0:00}", o.FROM_HOUR ?? 0) + string.Format("{0:00}", o.FROM_MINUTE ?? 0)) <= time &&
                         time <= Inventec.Common.TypeConvert.Parse.ToInt64(string.Format("{0:00}", o.TO_HOUR ?? 23) + "" + string.Format("{0:00}", o.TO_MINUTE ?? 59))).ToList();

                    if (lstTimefe != null && lstTimefe.Count > 0)
                    {
                        result = lstTimefe.First().ID;
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

        private string GetSeperateTimeFromString(string time)
        {
            string result = "";
            try
            {
                if (!String.IsNullOrEmpty(time) && time.Length >= 4)
                {
                    result = string.Format("{0}:{1}", time.Substring(0, 2), time.Substring(2, 2));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void cboTimeFrame_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                if (cboTimeFrame.EditValue != null && HIS.Desktop.Plugins.TreatmentFinish.Config.ConfigKey.NumOrderIssueOption == "1")
                {
                    var dataRow = ListTime.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboTimeFrame.EditValue.ToString()));
                    if (dataRow != null)
                    {
                        e.DisplayText = string.Format("{0}:{1} - {2}:{3}", dataRow.FROM_HOUR ?? 0, dataRow.FROM_MINUTE ?? 0, dataRow.TO_HOUR ?? 23, dataRow.TO_MINUTE ?? 59);
                    }
                }
                else if (cboTimeFrame.EditValue != null && HIS.Desktop.Plugins.TreatmentFinish.Config.ConfigKey.NumOrderIssueOption == "2")
                {
                    var dataRow = HisNumOrderBlockSDOs.FirstOrDefault(o => o.NUM_ORDER_BLOCK_ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboTimeFrame.EditValue.ToString()));
                    if (dataRow != null)
                    {
                        e.DisplayText = string.Format("{0} - {1}", GetSeperateTimeFromString(dataRow.FROM_TIME), GetSeperateTimeFromString(dataRow.TO_TIME));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridLookUpEdit1View_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound && ((IList)((BaseView)sender).DataSource) != null)
                {
                    if (HIS.Desktop.Plugins.TreatmentFinish.Config.ConfigKey.NumOrderIssueOption == "1")
                    {
                        var dataRow = (HisAppointmentPeriodCountByDateSDO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                        if (dataRow == null) return;

                        if (e.Column.FieldName == "TIME_FRAME")
                        {
                            e.Value = string.Format("{0}:{1} - {2}:{3}", dataRow.FROM_HOUR ?? 0, dataRow.FROM_MINUTE ?? 0, dataRow.TO_HOUR ?? 23, dataRow.TO_MINUTE ?? 59);
                        }
                        else if (e.Column.FieldName == "COUNT_TIME_FRAME")
                        {
                            e.Value = dataRow.CURRENT_COUNT + "/" + dataRow.MAXIMUM;
                        }
                    }
                    else if (HIS.Desktop.Plugins.TreatmentFinish.Config.ConfigKey.NumOrderIssueOption == "2")
                    {
                        var dataRow = (HisNumOrderBlockSDO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                        if (dataRow == null) return;

                        if (e.Column.FieldName == "TIME_FRAME")
                        {
                            e.Value = string.Format("{0} - {1}", dataRow.FROM_TIME, dataRow.TO_TIME);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTimeFrame_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    if (cboTimeFrame.EditValue != null && HIS.Desktop.Plugins.TreatmentFinish.Config.ConfigKey.NumOrderIssueOption == "1")
                    {
                        string id = cboTimeFrame.EditValue.ToString();
                        var rowdata = ListTime.FirstOrDefault(o => o.ID == Convert.ToInt64(id));
                        if (rowdata != null && rowdata.CURRENT_COUNT >= (rowdata.MAXIMUM ?? 0) && (rowdata.MAXIMUM ?? 0) > 0)
                        {
                            if (DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.KhungGioVuotQuaSoLuong,
                            ResourceMessage.ThongBao,
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTimeFrame_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboTimeFrame.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnAppointmentService_Click(object sender, EventArgs e)
        {
            try
            {
                if (Form != null && Form.currentHisTreatment != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(Form.currentHisTreatment.ID);

                    CallModule.Run(CallModule.AppointmentService, Form.module.RoomId, Form.module.RoomTypeId, listArgs);
                }
                else
                {
                    throw new ArgumentNullException("Treatment is null");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnChonKhungGio_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                List<object> listArgs = new List<object>();
                NumOrderBlockChooserADO numOrderBlockChooserADO = new Desktop.ADO.NumOrderBlockChooserADO();

                var datas = (List<RoomExamADO>)gridControlRoomExam.DataSource;
                if (datas != null && datas.Count > 0)
                {
                    List<RoomExamADO> seleted = datas.Where(p => p.IsCheck).ToList();

                    if (seleted != null && seleted.Count > 0)
                    {
                        numOrderBlockChooserADO.DefaultRoomId = seleted.FirstOrDefault().ID;
                    }
                    else
                    {
                        MessageBox.Show(ResourceMessage.BanChuaChonPhongKhamLanSau);
                        Inventec.Common.Logging.LogSystem.Warn("Bạn chưa chọn phòng khám lần sau");
                        return;
                    }
                }
                numOrderBlockChooserADO.DefaultDate = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtTimeAppointments.DateTime) ?? 0;
                numOrderBlockChooserADO.ListRoom = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => datas.Exists(k => k.ID == o.ID)).ToList();
                numOrderBlockChooserADO.DelegateChooseData = ActChooseDataProcess;//TODO
                bool IsNeedTime = true;
                listArgs.Add(IsNeedTime);

                Inventec.Common.Logging.LogSystem.Debug("Call module HIS.Desktop.Plugins.HisNumOrderBlockChooser: Input data: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => numOrderBlockChooserADO.DefaultRoomId), numOrderBlockChooserADO.DefaultRoomId)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => numOrderBlockChooserADO.DefaultDate), numOrderBlockChooserADO.DefaultDate)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => numOrderBlockChooserADO.ListRoom), numOrderBlockChooserADO.ListRoom));

                listArgs.Add(numOrderBlockChooserADO);

                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.HisNumOrderBlockChooser", 0, 0, listArgs);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ActChooseDataProcess(ResultChooseNumOrderBlockADO _resultChooseNumOrderBlockADO)
        {
            try
            {
                //TODO
                this.resultChooseNumOrderBlockADO = _resultChooseNumOrderBlockADO;
                if ((Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtTimeAppointments.DateTime) ?? 0) != this.resultChooseNumOrderBlockADO.Date)
                {
                    dtTimeAppointments.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.resultChooseNumOrderBlockADO.Date);
                }
                _RoomExamADOs = (List<RoomExamADO>)gridControlRoomExam.DataSource;
                if (_RoomExamADOs != null && _RoomExamADOs.Count > 0)
                {
                    if (HIS.Desktop.Plugins.TreatmentFinish.Config.ConfigKey.NumOrderIssueOption == "2")
                    {
                        _RoomExamADOs.ForEach(k => k.IsCheck = false);
                    }
                    foreach (var item in _RoomExamADOs)
                    {
                        if (item.ID == this.resultChooseNumOrderBlockADO.RoomId)
                        {
                            item.IsCheck = true;
                        }
                    }
                }

                gridControlRoomExam.RefreshDataSource();
                ProcessLoadGetOccupiedStatus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewRoomExam_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                var dataRow = (RoomExamADO)this.gridViewRoomExam.GetFocusedRow();
                if (dataRow.IsCheck)
                {
                    string currentNumOrderIssueOption = HIS.Desktop.Plugins.TreatmentFinish.Config.ConfigKey.NumOrderIssueOption;
                    HIS.Desktop.Plugins.TreatmentFinish.Config.ConfigKey.NumOrderIssueOption = (dataRow != null && dataRow.IS_BLOCK_NUM_ORDER == 1) ? "2" : "1";

                    if (HIS.Desktop.Plugins.TreatmentFinish.Config.ConfigKey.AutoCreateWhenAppointment == "1")
                    {
                        _RoomExamADOs.ForEach(k => k.IsCheck = false);
                        dataRow.IsCheck = true;
                        gridControlRoomExam.RefreshDataSource();
                    }

                    bool isChangeConfig = (currentNumOrderIssueOption != HIS.Desktop.Plugins.TreatmentFinish.Config.ConfigKey.NumOrderIssueOption);
                    if (isChangeConfig)
                    {
                        this.initNumOderBlock = true;
                        InitCboTimeFrame();
                        this.initNumOderBlock = false;
                    }

                    if (HIS.Desktop.Plugins.TreatmentFinish.Config.ConfigKey.NumOrderIssueOption == "2")
                    {
                        if (!isChangeConfig)
                            ProcessLoadGetOccupiedStatus();
                    }
                    else if (HIS.Desktop.Plugins.TreatmentFinish.Config.ConfigKey.NumOrderIssueOption == "1")
                    {
                        editdtTimeAppointments = true;
                        if (currentTreatmentFinishSDO != null && currentTreatmentFinishSDO.AppointmentTime.HasValue && currentTreatmentFinishSDO.AppointmentPeriodId > 0)
                        {
                            cboTimeFrame.EditValue = currentTreatmentFinishSDO.AppointmentPeriodId;
                        }
                        else
                        {
                            cboTimeFrame.EditValue = ProcessGetFromTime(dtTimeAppointments.DateTime);
                        }
                        editdtTimeAppointments = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboRoomEx_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboRoomEx.EditValue != null)
                {
                    NumOrderBlockID = null;
                    IsRoomBlock = false;
                    xtraTabControl1.TabPages.Clear();
                    var dataRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == Int64.Parse(cboRoomEx.EditValue.ToString()));
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataRoom), dataRoom));
                    if (dataRoom.IS_BLOCK_NUM_ORDER != null && dataRoom.IS_BLOCK_NUM_ORDER == 1)
                    {
                        IsRoomBlock = true;
                        setBlock();
                        ProcessLoadGetOccupiedStatusBlock();
                    }
                    else
                    {
                        xtraTabControl1.TabPages.Clear();

                    }
                }
                else
                {
                    xtraTabControl1.TabPages.Clear();

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtTimeAppointments_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (IsBlockOrder)
                {
                    if (cboRoomEx.EditValue != null)
                    {
                        xtraTabControl1.TabPages.Clear();
                        var dataRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == Int64.Parse(cboRoomEx.EditValue.ToString()));
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataRoom), dataRoom));
                        if (dataRoom.IS_BLOCK_NUM_ORDER != null && dataRoom.IS_BLOCK_NUM_ORDER == 1)
                        {
                            setBlock();
                            ProcessLoadGetOccupiedStatusBlock();
                        }
                        else
                        {
                            xtraTabControl1.TabPages.Clear();
                        }
                    }
                    else
                    {
                        xtraTabControl1.TabPages.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtTimeAppointments_PreviewKeyDown_1(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (IsBlockOrder)
                    {
                        if (cboRoomEx.EditValue != null)
                        {
                            xtraTabControl1.TabPages.Clear();
                            var dataRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == Int64.Parse(cboRoomEx.EditValue.ToString()));
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataRoom), dataRoom));
                            if (dataRoom.IS_BLOCK_NUM_ORDER != null && dataRoom.IS_BLOCK_NUM_ORDER == 1)
                            {
                                setBlock();
                                ProcessLoadGetOccupiedStatusBlock();
                            }
                            else
                            {
                                xtraTabControl1.TabPages.Clear();
                            }
                        }
                        else
                        {
                            xtraTabControl1.TabPages.Clear();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FormAppointment_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                if (!IsReturnClosed)
                {
                    HIS.Desktop.Plugins.TreatmentFinish.Config.ConfigKey.NumOrderIssueOption = "1";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkKhongCanhBaoNgayNghi_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirstAppointment)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateAppointmentRDO != null && this.currentControlStateAppointmentRDO.Count > 0) ? this.currentControlStateAppointmentRDO.Where(o => o.KEY == chkKhongCanhBaoNgayNghi.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkKhongCanhBaoNgayNghi.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkKhongCanhBaoNgayNghi.Name;
                    csAddOrUpdate.VALUE = (chkKhongCanhBaoNgayNghi.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateAppointmentRDO == null)
                        this.currentControlStateAppointmentRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateAppointmentRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorkerAppointment.SetData(this.currentControlStateAppointmentRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void InitControlAppointmentState()
        {
            isNotLoadWhileChangeControlStateInFirstAppointment = true;
            try
            {
                this.controlStateWorkerAppointment = new Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateAppointmentRDO = controlStateWorkerAppointment.GetData(moduleLink);
                if (this.currentControlStateAppointmentRDO != null && this.currentControlStateAppointmentRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateAppointmentRDO)
                    {
                        if (item.KEY == chkKhongCanhBaoNgayNghi.Name)
                        {
                            chkKhongCanhBaoNgayNghi.Checked = item.VALUE == "1";
                        }
                        if(item.KEY == chkInPhieuHenKham.Name)
                        {
                            chkInPhieuHenKham.Checked = item.VALUE == "2";
                        }
                        if (item.KEY == chkKyPhieuHenKham.Name)
                        {
                            chkKyPhieuHenKham.Checked = item.VALUE == "3";
                        }
                        if (item.KEY == chkXemTrcKhiIn.Name)
                        {
                            chkXemTrcKhiIn.Checked = item.VALUE == "4";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            isNotLoadWhileChangeControlStateInFirstAppointment = false;
        }

        private void chkInPhieuHenKham_CheckedChanged(object sender, EventArgs e)
        {
            try
            {               
                if (isNotLoadWhileChangeControlStateInFirstAppointment)
                {
                    return;
                }

                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO controlStateRDO = (this.currentControlStateAppointmentRDO != null && this.currentControlStateAppointmentRDO.Count > 0) ? this.currentControlStateAppointmentRDO.Where(o => o.KEY == chkInPhieuHenKham.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => controlStateRDO), controlStateRDO));
                if (controlStateRDO != null)
                {
                    controlStateRDO.VALUE = (chkInPhieuHenKham.Checked ? "2" : "");
                }
                else
                {
                    controlStateRDO = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    controlStateRDO.KEY = chkInPhieuHenKham.Name;
                    controlStateRDO.VALUE = (chkInPhieuHenKham.Checked ? "2" : "");
                    controlStateRDO.MODULE_LINK = moduleLink;
                    if (this.currentControlStateAppointmentRDO == null)
                        this.currentControlStateAppointmentRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateAppointmentRDO.Add(controlStateRDO);
                }
                this.controlStateWorkerAppointment.SetData(this.currentControlStateAppointmentRDO);
                if (chkInPhieuHenKham.Checked)
                {
                    chkXemTrcKhiIn.Checked = false;
                }
                AppointmentPrintOptionsStorageADO.InPhieuHenKham = chkInPhieuHenKham.Checked;
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }           
        }

        private void chkXemTrcKhiIn_CheckedChanged(object sender, EventArgs e)  
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirstAppointment) return;
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO controlStateRDO = (this.currentControlStateAppointmentRDO != null && this.currentControlStateAppointmentRDO.Count > 0) ? this.currentControlStateAppointmentRDO.Where(o => o.KEY == chkXemTrcKhiIn.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if(controlStateRDO != null)
                {
                    controlStateRDO.VALUE = (chkXemTrcKhiIn.Checked ? "4" : "");
                }
                else
                {
                    controlStateRDO = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    controlStateRDO.KEY = chkXemTrcKhiIn.Name;
                    controlStateRDO.VALUE = (chkXemTrcKhiIn.Checked ? "4" : "");
                    controlStateRDO.MODULE_LINK = moduleLink;

                    if (this.currentControlStateAppointmentRDO == null)
                    {
                        this.currentControlStateAppointmentRDO = new List<Desktop.Library.CacheClient.ControlStateRDO>();
                    }
                    this.currentControlStateAppointmentRDO.Add(controlStateRDO);
                }
                if (chkXemTrcKhiIn.Checked)
                {
                    chkInPhieuHenKham.Checked = false;
                }
                this.controlStateWorkerAppointment.SetData(this.currentControlStateAppointmentRDO);
                AppointmentPrintOptionsStorageADO.XemTruocKhiIn = chkXemTrcKhiIn.Checked;
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
            
        }

        private void chkKyPhieuHenKham_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirstAppointment) return;
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO controlStateRDO = (this.currentControlStateAppointmentRDO != null && this.currentControlStateAppointmentRDO.Count > 0) ? this.currentControlStateAppointmentRDO.Where(o => o.KEY == chkKyPhieuHenKham.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if(controlStateRDO != null)
                {
                    controlStateRDO.VALUE = (chkKyPhieuHenKham.Checked ? "3" : "0");
                }
                else
                {
                    controlStateRDO = new Desktop.Library.CacheClient.ControlStateRDO();
                    controlStateRDO.KEY = chkKyPhieuHenKham.Name;
                    controlStateRDO.VALUE = (chkKyPhieuHenKham.Checked ? "3" : "0");
                    controlStateRDO.MODULE_LINK = moduleLink;

                    if(this.currentControlStateAppointmentRDO == null)
                    {
                        this.currentControlStateAppointmentRDO = new List<Desktop.Library.CacheClient.ControlStateRDO>();
                    }
                    this.currentControlStateAppointmentRDO.Add(controlStateRDO);
                }
                this.controlStateWorkerAppointment.SetData(this.currentControlStateAppointmentRDO);
                AppointmentPrintOptionsStorageADO.KyPhieuHenKham = chkKyPhieuHenKham.Checked;
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }
    }
}
