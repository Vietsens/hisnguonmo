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
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.UC.ExamTreatmentFinish.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
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
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using Inventec.Common.Controls.EditorLoader;
using DevExpress.XtraTab;
using HIS.UC.ExamTreatmentFinish.Base;
using HIS.UC.ExamTreatmentFinish.Run;
using HIS.UC.ExamTreatmentFinish.Resources;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.Library.CacheClient;
using Inventec.Desktop.Common.Message;

namespace HIS.UC.ExamTreatmentFinish.EndTypeForm
{
    public partial class FormAppointment : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        //private long dtAppointment = 0;
        private int positionHandle = -1;
        //private MOS.SDO.HisTreatmentFinishSDO currentTreatmentFinishSDO { get; set; }
        private Action<HisTreatmentFinishSDO> actEdited;
        private HIS_TREATMENT hisTreatment { get; set; }
        private List<RoomExamADO> _RoomExamADOs { get; set; }
        private bool isCheckAll = true;
        //private List<TimeFrameADO> ListTime;
        private List<HisAppointmentPeriodCountByDateSDO> ListTime;
        private bool editdtTimeAppointments = false;
        private bool editcboTimeFrame = false;
        List<SereServADO> listSereServADO = null;
        bool initNumOderBlock;
        Inventec.Desktop.Common.Modules.Module moduleData;
        private List<HisNumOrderBlockSDO> HisNumOrderBlockSDOs;
        List<HisNumOrderBlockSDO> apiResult;
        ResultChooseNumOrderBlockADO resultChooseNumOrderBlockADO;
        bool IsBlockOrder;
        private HisNumOrderBlockSDO NumOrderBlock;
        long? NumOrderBlockID = null;
        bool IsReturnClosed = false;
        bool IsRoomBlock = false;
        UCAppointmentAdvise ucAdvise;
        private bool isNotLoadWhileChangeControlStateInFirst;
        private ControlStateWorker controlStateWorker;
        private List<ControlStateRDO> currentControlStateRDO;
        string moduleLink = "HIS.UC.ExamTreatmentFinish.EndTypeForm.FormAppointment";
        #endregion

        #region Construct

        public FormAppointment(HIS_TREATMENT treatment, Action<HisTreatmentFinishSDO> _actEdited, bool IsBlockOrder)
        {
            InitializeComponent();
            try
            {
                this.hisTreatment = treatment;
                this.actEdited = _actEdited;
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
                isNotLoadWhileChangeControlStateInFirst = true;
                InitControlState();
                this.initNumOderBlock = true;
                SetIcon();

                //LoadKeysFromlanguage();
                SetCaptionByLanguageKey();
                LoadRoomExam();

                SetDefaultValueControl();

                if (IsBlockOrder)
                {
                    this.Size = new Size(566, 650);
                    this.layoutControlItem10.Size = new System.Drawing.Size(550, 700);
                    this.lciTimeAppointments.Size = new Size(300, 26);
                    layoutControlItem9.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    layoutControlItem11.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    layoutControlItem10.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    layoutControlItem5.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutControlItem4.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutControlItem8.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lciTimeFrame.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    InitCboRoomEx();
                    //setBlock();
                }
                else
                {
                    layoutControlItem9.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutControlItem11.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutControlItem10.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutControlItem5.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    layoutControlItem4.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    layoutControlItem8.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    lciTimeFrame.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;

                    InitCboTimeFrame();

                    if (HIS.UC.ExamTreatmentFinish.Config.HisConfig.NumOrderIssueOption == "1")
                    {
                        editdtTimeAppointments = true;
                        cboTimeFrame.EditValue = ProcessGetFromTime(dtTimeAppointments.DateTime);
                        if (hisTreatment.APPOINTMENT_TIME.HasValue)//đã chọn 1 lần mở lại sẽ có thông tin kết thúc sẽ hiển thị theo lần chọn trước
                        {
                            cboTimeFrame.EditValue = hisTreatment.APPOINTMENT_PERIOD_ID;
                        }

                        editdtTimeAppointments = false;
                    }

                }

                //ucAdvise = new UCAppointmentAdvise();
                //panelControl1.Controls.Add(ucAdvise);
                //ucAdvise.Dock = DockStyle.Fill;
                //ucAdvise.SetValueAdvise(hisTreatment.ADVISE);
                ValidationTimeAppointments();
                ValidationControlMaxLength(this.txtAdvise, 500);
                this.initNumOderBlock = false;
                this.StartPosition = FormStartPosition.CenterParent;
                isNotLoadWhileChangeControlStateInFirst = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Private method
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
                if (_RoomExamADOs.Where(o => o.IsCheck).ToList() != null)
                {
                    cboRoomEx.EditValue = _RoomExamADOs.Where(o => o.IsCheck).ToList().FirstOrDefault().ID;
                }
                else
                {
                    cboRoomEx.EditValue = null;
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

                apiResult = new BackendAdapter(param).Get<List<HisNumOrderBlockSDO>>("api/HisNumOrderBlock/GetOccupiedStatus", Desktop.ApiConsumer.ApiConsumers.MosConsumer, filter, param);

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
                        XtraTabPage tab = new XtraTabPage();
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
                    Inventec.Common.Logging.LogSystem.Error(NumOrderBlockID+"_____NumOrderBlockID");
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
                var HisNumOrderBlockSDOTmps = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HisNumOrderBlockSDO>>("api/HisNumOrderBlock/GetOccupiedStatus", Desktop.ApiConsumer.ApiConsumers.MosConsumer, filter, null);
                if (HisNumOrderBlockSDOTmps != null && HisNumOrderBlockSDOTmps.Count > 0)
                {
                    HisNumOrderBlockSDOs = HisNumOrderBlockSDOTmps.Where(o => o.IS_ISSUED == null || o.IS_ISSUED != 1).OrderBy(o => o.FROM_TIME).ToList();
                    if (!(HisNumOrderBlockSDOs != null && HisNumOrderBlockSDOs.Count > 0))
                    {                       
                        Inventec.Common.Logging.LogSystem.Debug("ProcessLoadGetOccupiedStatus.3");
                        List<HisAppointmentPeriodCountByDateSDO> ListTime = new List<HisAppointmentPeriodCountByDateSDO>();
                        NumOrderBlockID = null;
                        DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Đã cấp hết số khám vào ngày {0}", dtTimeAppointments.DateTime.ToString("dd/MM")),
                             Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                    }
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Đã cấp hết số khám vào ngày {0}", dtTimeAppointments.DateTime.ToString("dd/MM")),
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
                Inventec.Common.Logging.LogSystem.Error(HIS.UC.ExamTreatmentFinish.Config.HisConfig.NumOrderIssueOption + "___HIS.UC.ExamTreatmentFinish.Config.HisConfig.NumOrderIssueOption");
                if (HIS.UC.ExamTreatmentFinish.Config.HisConfig.NumOrderIssueOption == "1")
                {
                    layoutControlItem8.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
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
                else if (HIS.UC.ExamTreatmentFinish.Config.HisConfig.NumOrderIssueOption == "2")
                {
                    layoutControlItem8.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
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
                //filter.IS_ACTIVE = 1;
                var HisNumOrderBlockSDOTmps = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HisNumOrderBlockSDO>>("api/HisNumOrderBlock/GetOccupiedStatus", Desktop.ApiConsumer.ApiConsumers.MosConsumer, filter, null);
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
                        DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Đã cấp hết số khám vào ngày {0}", dtTimeAppointments.DateTime.ToString("dd/MM")),
                             Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                    }
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Đã cấp hết số khám vào ngày {0}", dtTimeAppointments.DateTime.ToString("dd/MM")),
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
                ListTime = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HisAppointmentPeriodCountByDateSDO>>("api/HisAppointmentPeriod/GetCountByDate", Desktop.ApiConsumer.ApiConsumers.MosConsumer, filter, null);
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

        private void SetAppoinmentTime()
        {
            try
            {
                long priorityAppoinmentTime = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<long>(SdaConfig.PRESCRIPTION_TIME_AND_APPOINTMENT_TIME_KEY);
                if (priorityAppoinmentTime == 1)
                {
                    MOS.Filter.HisServiceReqFilter filter = new MOS.Filter.HisServiceReqFilter();
                    filter.TREATMENT_ID = hisTreatment.ID;
                    filter.SERVICE_REQ_TYPE_IDs = new List<long> { IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK };
                    filter.ORDER_DIRECTION = "DESC";
                    filter.ORDER_FIELD = "CREATE_TIME";
                    var serviceReqs = new Inventec.Common.Adapter.BackendAdapter(new Inventec.Core.CommonParam()).Get<List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ>>("api/HisServiceReq/Get", Desktop.ApiConsumer.ApiConsumers.MosConsumer, filter, null);

                    var serviceReq = serviceReqs.Where(o => o.USE_TIME_TO.HasValue).OrderByDescending(o => o.USE_TIME_TO).FirstOrDefault();
                    if (serviceReq != null)
                    {
                        DateTime dtUseTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(serviceReq.USE_TIME_TO.Value) ?? DateTime.MinValue;
                        dtTimeAppointments.DateTime = dtUseTime.AddDays((double)1);
                        return;
                    }
                }

                long appoinmentTimeDefault = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(SdaConfig.TREATMENT_END___APPOINTMENT_TIME_DEFAULT_KEY));
                if (appoinmentTimeDefault > 0)
                {
                    //long endTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(Form.dtEndTime.DateTime) ?? 0;
                    long endTime = hisTreatment.OUT_TIME ?? 0;
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
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
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
                //layout
                //this.Text = Form.GetStringFromKey("IVT_LANGUAGE_KEY__FORM_APPOINTMENT__TEXT");
                //this.lciTimeAppointments.Text = Form.GetStringFromKey("IVT_LANGUAGE_KEY__FORM_APPOINTMENT__LCI_APPOINTMENT_TIME");
                //this.btnSave.Text = Form.GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TREATMENT_FINISH__CLOSE_TREATMENT_SAVE");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện FormAppointment
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.UC.ExamTreatmentFinish.Resources.Lang", typeof(FormAppointment).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("FormAppointment.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblStt.Text = Inventec.Common.Resource.Get.Value("FormAppointment.lblStt.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboRoomEx.Properties.NullText = Inventec.Common.Resource.Get.Value("FormAppointment.cboRoomEx.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("FormAppointment.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItemSave.Caption = Inventec.Common.Resource.Get.Value("FormAppointment.barButtonItemSave.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnChonKhungGio.ToolTip = Inventec.Common.Resource.Get.Value("FormAppointment.bbtnChonKhungGio.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnServiceAppoint.Text = Inventec.Common.Resource.Get.Value("FormAppointment.btnServiceAppoint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnServiceAppoint.ToolTip = Inventec.Common.Resource.Get.Value("FormAppointment.btnServiceAppoint.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboTimeFrame.Properties.NullText = Inventec.Common.Resource.Get.Value("FormAppointment.cboTimeFrame.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtAdvise.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("FormAppointment.txtAdvise.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("FormAppointment.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("FormAppointment.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTimeFrame.Text = Inventec.Common.Resource.Get.Value("FormAppointment.lciTimeFrame.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem9.Text = Inventec.Common.Resource.Get.Value("FormAppointment.layoutControlItem9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem10.Text = Inventec.Common.Resource.Get.Value("FormAppointment.layoutControlItem10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem11.Text = Inventec.Common.Resource.Get.Value("FormAppointment.layoutControlItem11.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("FormAppointment.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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
                txtAdvise.Text = hisTreatment.ADVISE;

                if (hisTreatment.APPOINTMENT_TIME.HasValue)
                {
                    DateTime? timeAppointment = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(hisTreatment.APPOINTMENT_TIME.Value);
                    dtTimeAppointments.EditValue = timeAppointment;
                }
                else
                {
                    SetAppoinmentTime();
                }

                if (hisTreatment != null
                    && !string.IsNullOrEmpty(hisTreatment.APPOINTMENT_EXAM_ROOM_IDS)
                    && _RoomExamADOs != null && _RoomExamADOs.Count > 0)
                {
                    string[] ids = hisTreatment.APPOINTMENT_EXAM_ROOM_IDS.Split(',');
                    foreach (var item in _RoomExamADOs)
                    {
                        var dataCheck = ids.FirstOrDefault(p => p.Trim() == item.ID.ToString().Trim());
                        if (!string.IsNullOrEmpty(dataCheck))
                        {
                            item.IsCheck = true;
                            if (HIS.UC.ExamTreatmentFinish.Config.HisConfig.NumOrderIssueOption == "2")
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
                appointmentServFilter.TREATMENT_ID = hisTreatment.ID;
                List<HIS_APPOINTMENT_SERV> appoinmentServs = new BackendAdapter(param)
                    .Get<List<HIS_APPOINTMENT_SERV>>("api/HisAppointmentServ/Get", Desktop.ApiConsumer.ApiConsumers.MosConsumer, appointmentServFilter, param);
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
                this.positionHandle = -1;
                //if (apiResult != null && apiResult.Count > 0)
                //{
                //    ProcessLoadGetOccupiedStatusBlock();
                //    if (IsReturn) return;
                //}
                IsReturnClosed = false;
                //bool checkValidate = true;
                ////checkValidate = checkValidate && dxValidationProvider.Validate();
                //checkValidate = checkValidate && ucAdvise.GetValidate();
              

                if (!dxValidationProvider.Validate())
                {
                    return;
                }
                if (Config.HisConfig.AutoCreateWhenAppoinment == "1"
                    && hisTreatment.TDL_PATIENT_TYPE_ID == BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT")).ID 
                    && hisTreatment.TDL_HEIN_CARD_TO_TIME != null && Int64.Parse(dtTimeAppointments.DateTime.ToString("yyyyMMdd")) > Int64.Parse(hisTreatment.TDL_HEIN_CARD_TO_TIME.ToString().Substring(0, 8)) 
                    && DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Thẻ BHYT của bệnh nhân sẽ hết hạn vào ngày {0}, bạn có chắc muốn hẹn khám vào ngày {1} không? Nếu tiếp tục, phần mềm sẽ tự động đổi sang đối tượng {2}", Inventec.Common.DateTime.Convert.TimeNumberToDateString(hisTreatment.TDL_HEIN_CARD_TO_TIME ?? 0), dtTimeAppointments.DateTime.ToString("dd/MM/yyyy"), BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.HOSPITAL_FEE")).PATIENT_TYPE_NAME),Resources.ResourceMessage.ThongBao,MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    dtTimeAppointments.Focus();
                    dtTimeAppointments.SelectAll();
                    return;
                }
                
                if (IsRoomBlock && NumOrderBlockID == null)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Bạn chưa chọn số thứ tự",
                        Resources.ResourceMessage.ThongBao);
                    return;
                }

                if (Config.HisConfig.MustChooseSeviceInCaseOfAppointment == "1")
                {
                    if (!GetAppoinmentCountTrue())
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Bạn bắt buộc chọn dịch vụ hẹn khám khi kết thúc điều trị",
                        Resources.ResourceMessage.ThongBao);
                        return;
                    }
                }
                
                //Kiểm tra nếu số ngày hẹn khám vượt quá số ngày được cấu hình
                if (Config.HisConfig.MaxOfAppointmentDays.HasValue && spDay.Value > Config.HisConfig.MaxOfAppointmentDays.Value)
                {
                    //Tùy chọn xử lý trong trường hợp vượt quá số ngày hẹn khám. 1: Cảnh báo. 2: Chặn, không cho xử trí
                    if (Config.HisConfig.WarningOptionWhenExceedingMaxOfAppointmentDays == 2)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(string.Format(Resources.ResourceMessage.CanhBaoNgayHenToiDa, Config.HisConfig.MaxOfAppointmentDays),
                        Resources.ResourceMessage.ThongBao,
                        MessageBoxButtons.OK);
                        return;
                    }
                    else if (Config.HisConfig.WarningOptionWhenExceedingMaxOfAppointmentDays == 1)
                    {
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(string.Format("{0} Bạn có muốn tiếp tục không?", string.Format(Resources.ResourceMessage.CanhBaoNgayHenToiDa, Config.HisConfig.MaxOfAppointmentDays)),
                        Resources.ResourceMessage.ThongBao,
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;
                    }
                }

                if (!chkNotCheckT7CN.Checked)
                {
                    if (dtTimeAppointments.DateTime.DayOfWeek == DayOfWeek.Sunday)
                    {
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceMessage.CanhBaoNgayHenLaChuNhat,
                        Resources.ResourceMessage.ThongBao,
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;
                    }
                    else if (dtTimeAppointments.DateTime.DayOfWeek == DayOfWeek.Saturday)
                    {
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceMessage.CanhBaoNgayHenLaThuBay,
                        Resources.ResourceMessage.ThongBao,
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;
                    }
                }
                long dtAppointmentTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtTimeAppointments.DateTime) ?? 0;
                long dtOutTime = hisTreatment.OUT_TIME ?? 99999999999999;
                if (dtAppointmentTime >= dtOutTime)
                {
                    HisTreatmentFinishSDO currentTreatmentFinishSDO = new HisTreatmentFinishSDO();
                    currentTreatmentFinishSDO.Advise = txtAdvise.Text.Trim();
                    currentTreatmentFinishSDO.AppointmentTime = dtAppointmentTime;
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
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => seleted), seleted));
                            if (seleted != null && seleted.Count > 0)
                            {
                                if (!this.CheckMaxAppointment(seleted))
                                {
                                    return;
                                }
                                currentTreatmentFinishSDO.AppointmentExamRoomIds = seleted.Select(s => s.ID).Distinct().ToList();
                            }
                        }
                    }

                    if (HIS.UC.ExamTreatmentFinish.Config.HisConfig.NumOrderIssueOption == "1")
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
                    else if (HIS.UC.ExamTreatmentFinish.Config.HisConfig.NumOrderIssueOption == "2")
                    {
                        
                        if (cboTimeFrame.EditValue != null && Inventec.Common.TypeConvert.Parse.ToInt64(cboTimeFrame.EditValue.ToString()) > 0)
                        {
                            currentTreatmentFinishSDO.NumOrderBlockId = Inventec.Common.TypeConvert.Parse.ToInt64(cboTimeFrame.EditValue.ToString());
                            //currentTreatmentFinishSDO.NumOrderBlockNumOrder = HisNumOrderBlockSDOs.FirstOrDefault().NUM_ORDER;//TODO
                        }



                    }
                    if (IsBlockOrder)
                    {
                        currentTreatmentFinishSDO.AppointmentPeriodId = null;
                        currentTreatmentFinishSDO.NumOrderBlockId = NumOrderBlockID;
                    }

                    Inventec.Common.Logging.LogSystem.Debug("FormAppointment:"+Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentTreatmentFinishSDO), currentTreatmentFinishSDO));
                    actEdited(currentTreatmentFinishSDO);
                    this.Close();
                }
                else
                {
                    MessageBox.Show(Resources.ResourceMessage.CanhBaoThoiGianHenKhamSoVoiThoiGianKetThucDieuTri);
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
                ControlEditValidationRule icdMainRule = new ControlEditValidationRule();
                icdMainRule.editor = dtTimeAppointments;
                icdMainRule.ErrorText = Resources.ResourceMessage.TruongDuLieuBatBuoc;
                icdMainRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider.SetValidationRule(dtTimeAppointments, icdMainRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationControlMaxLength(BaseEdit control, int? maxLength)
        {
            ControlMaxLengthValidationRule validate = new ControlMaxLengthValidationRule();
            validate.editor = control;
            validate.maxLength = maxLength;
            validate.ErrorText = String.Format(Resources.ResourceMessage.TruongDuLieuVuotQuaKyTu, maxLength);
            validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
            this.dxValidationProvider.SetValidationRule(control, validate);
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

                    List<HisExecuteRoomAppointedSDO> sdos = new BackendAdapter(new CommonParam()).Get<List<HisExecuteRoomAppointedSDO>>("api/HisExecuteRoom/GetCountAppointed", Desktop.ApiConsumer.ApiConsumers.MosConsumer, filter, null);
                    List<HisExecuteRoomAppointedSDO> overs = sdos != null ? sdos.Where(o => (o.MaxAmount ?? 0) > 0 && (o.CurrentAmount ?? 0) > 0 && o.CurrentAmount.Value >= o.MaxAmount).ToList() : null;
                    LogSystem.Debug("sdos: \n" + LogUtil.TraceData("sdos", sdos));
                    if (overs != null && overs.Count > 0)
                    {
                        string names = String.Join(", ", overs.Select(s => String.Format("{0}({1}/{2})", s.ExecuteRoomName, s.CurrentAmount, s.MaxAmount)).ToList());
                        string mess = String.Format(Resources.ResourceMessage.PhongKhamCoSoLuotKhamVuotDinhMuc, names);
                        if (XtraMessageBox.Show(mess, Resources.ResourceMessage.ThongBao, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != System.Windows.Forms.DialogResult.Yes)
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
                    //setBlock();
                    //btnSave.Focus();

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

                            if (editor.OldEditValue != null && HIS.UC.ExamTreatmentFinish.Config.HisConfig.NumOrderIssueOption == "1")
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

                    if (HIS.UC.ExamTreatmentFinish.Config.HisConfig.NumOrderIssueOption == "1")
                    {
                        cboTimeFrame.EditValue = ProcessGetFromTime(dtTimeAppointments.DateTime);
                    }
                    else if (HIS.UC.ExamTreatmentFinish.Config.HisConfig.NumOrderIssueOption == "2" && !initNumOderBlock)
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
                                if (HIS.UC.ExamTreatmentFinish.Config.HisConfig.NumOrderIssueOption == "1")
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

        private void btnServiceAppoint_Click(object sender, EventArgs e)
        {
            try
            {
                List<object> listArgs = new List<object>();
                listArgs.Add(hisTreatment.ID);

                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.AppointmentService", 0, 0, listArgs);
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
                if (HIS.UC.ExamTreatmentFinish.Config.HisConfig.NumOrderIssueOption == "1")
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

        private void gridLookUpEdit1View_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                if (HIS.UC.ExamTreatmentFinish.Config.HisConfig.NumOrderIssueOption == "1")
                {
                    HisAppointmentPeriodCountByDateSDO row = (HisAppointmentPeriodCountByDateSDO)gridLookUpEdit1View.GetRow(e.RowHandle);
                    if (row != null)
                    {
                        if ((row.MAXIMUM ?? 0) > 0 && row.CURRENT_COUNT >= (row.MAXIMUM ?? 0))
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

        private void gridLookUpEdit1View_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound && ((IList)((BaseView)sender).DataSource) != null)
                {
                    if (HIS.UC.ExamTreatmentFinish.Config.HisConfig.NumOrderIssueOption == "1")
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
                    else if (HIS.UC.ExamTreatmentFinish.Config.HisConfig.NumOrderIssueOption == "2")
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
                if (cboTimeFrame.EditValue != null && HIS.UC.ExamTreatmentFinish.Config.HisConfig.NumOrderIssueOption == "1")
                {
                    var dataRow = ListTime.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboTimeFrame.EditValue.ToString()));
                    if (dataRow != null)
                    {
                        e.DisplayText = string.Format("{0}:{1} - {2}:{3}", dataRow.FROM_HOUR ?? 0, dataRow.FROM_MINUTE ?? 0, dataRow.TO_HOUR ?? 23, dataRow.TO_MINUTE ?? 59);
                    }
                }
                else if (cboTimeFrame.EditValue != null && HIS.UC.ExamTreatmentFinish.Config.HisConfig.NumOrderIssueOption == "2")
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

        private void cboTimeFrame_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    if (cboTimeFrame.EditValue != null && HIS.UC.ExamTreatmentFinish.Config.HisConfig.NumOrderIssueOption == "1")
                    {
                        string id = cboTimeFrame.EditValue.ToString();
                        var rowdata = ListTime.FirstOrDefault(o => o.ID == Convert.ToInt64(id));
                        if (rowdata != null && (rowdata.MAXIMUM ?? 0) > 0 && rowdata.CURRENT_COUNT >= (rowdata.MAXIMUM ?? 0))
                        {
                            if (DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceMessage.KhungGioVuotQuaSoLuong,
                            Resources.ResourceMessage.ThongBao,
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                            {
                                cboTimeFrame.ShowPopup();
                                return;
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
                        MessageBox.Show("Bạn chưa chọn phòng khám lần sau");
                        Inventec.Common.Logging.LogSystem.Warn("Bạn chưa chọn phòng khám lần sau");
                        return;
                    }
                }
                numOrderBlockChooserADO.DefaultDate = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtTimeAppointments.DateTime) ?? 0;
                //numOrderBlockChooserADO.DisableDate = false;
                //numOrderBlockChooserADO.DisableRoom = false;
                numOrderBlockChooserADO.ListRoom = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => datas.Exists(k => k.ID == o.ID)).ToList();
                numOrderBlockChooserADO.DelegateChooseData = ActChooseDataProcess;//TODO
                bool IsNeedTimeString = true;
                listArgs.Add(IsNeedTimeString);

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
                    if (HIS.UC.ExamTreatmentFinish.Config.HisConfig.NumOrderIssueOption == "2")
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
                //gridControlRoomExam.DataSource = null;
                //gridControlRoomExam.DataSource = _RoomExamADOs;
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
                    string currentNumOrderIssueOption = HIS.UC.ExamTreatmentFinish.Config.HisConfig.NumOrderIssueOption;
                    HIS.UC.ExamTreatmentFinish.Config.HisConfig.NumOrderIssueOption = (dataRow != null && dataRow.IS_BLOCK_NUM_ORDER == 1) ? "2" : "1";

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentNumOrderIssueOption), currentNumOrderIssueOption)
                        + Inventec.Common.Logging.LogUtil.TraceData("ConfigKeyCFG.NumOrderIssueOption", HIS.UC.ExamTreatmentFinish.Config.HisConfig.NumOrderIssueOption));

                    if (Config.HisConfig.AutoCreateWhenAppoinment == "1" || HIS.UC.ExamTreatmentFinish.Config.HisConfig.NumOrderIssueOption == "2")
                    {
                        _RoomExamADOs.ForEach(k => k.IsCheck = false);
                        dataRow.IsCheck = true;
                        gridControlRoomExam.RefreshDataSource();
                    }

                    bool isChangeConfig = (currentNumOrderIssueOption != HIS.UC.ExamTreatmentFinish.Config.HisConfig.NumOrderIssueOption);
                    if (isChangeConfig)
                    {
                        this.initNumOderBlock = true;
                        InitCboTimeFrame();
                        this.initNumOderBlock = false;
                    }

                    if (HIS.UC.ExamTreatmentFinish.Config.HisConfig.NumOrderIssueOption == "2")
                    {
                        if (!isChangeConfig)
                            ProcessLoadGetOccupiedStatus();
                    }
                    else if (HIS.UC.ExamTreatmentFinish.Config.HisConfig.NumOrderIssueOption == "1")
                    {
                        editdtTimeAppointments = true;
                        cboTimeFrame.EditValue = ProcessGetFromTime(dtTimeAppointments.DateTime);
                        if (hisTreatment.APPOINTMENT_TIME.HasValue && hisTreatment.APPOINTMENT_PERIOD_ID > 0)//đã chọn 1 lần mở lại sẽ có thông tin kết thúc sẽ hiển thị theo lần chọn trước
                        {
                            cboTimeFrame.EditValue = hisTreatment.APPOINTMENT_PERIOD_ID;
                        }
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("cboTimeFrame.EditValue", cboTimeFrame.EditValue)
                                 + Inventec.Common.Logging.LogUtil.TraceData("hisTreatment.APPOINTMENT_PERIOD_ID", (hisTreatment != null && hisTreatment.APPOINTMENT_PERIOD_ID.HasValue) ? hisTreatment.APPOINTMENT_PERIOD_ID : 0)
                                 + Inventec.Common.Logging.LogUtil.TraceData("ListTime", ListTime));
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
                    if (dataRoom.IS_BLOCK_NUM_ORDER !=null && dataRoom.IS_BLOCK_NUM_ORDER == 1)
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

        private void dtTimeAppointments_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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
                    HIS.UC.ExamTreatmentFinish.Config.HisConfig.NumOrderIssueOption = "1";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkNotCheckT7CN_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkNotCheckT7CN.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkNotCheckT7CN.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkNotCheckT7CN.Name;
                    csAddOrUpdate.VALUE = (chkNotCheckT7CN.Checked ? "1" : "");
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void InitControlState()
        {
            isNotLoadWhileChangeControlStateInFirst = true;
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(moduleLink);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentControlStateRDO), currentControlStateRDO));
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == chkNotCheckT7CN.Name)
                        {
                            chkNotCheckT7CN.Checked = item.VALUE == "1";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            isNotLoadWhileChangeControlStateInFirst = false;
        }
    }
}
