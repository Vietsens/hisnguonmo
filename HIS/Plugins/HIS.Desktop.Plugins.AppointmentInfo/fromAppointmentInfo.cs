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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.AppointmentInfo.ADO;
using HIS.Desktop.Plugins.AppointmentInfo.Config;
using HIS.Desktop.Plugins.AppointmentInfo.Resources;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
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
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AppointmentInfo
{
    public partial class fromAppointmentInfo : FormBase
    {

        private int positionHandle = -1;

        private V_HIS_TREATMENT_4 treatment;
        private RefeshReference refresh = null;

        private List<ExamRoomADO> listExamRoom = new List<ExamRoomADO>();
        private List<HisAppointmentPeriodCountByDateSDO> listTimeFrame = new List<HisAppointmentPeriodCountByDateSDO>();

        private bool editTime = false;
        private bool editCombo = false;
        private bool isCheckAll = true;

        public fromAppointmentInfo(Inventec.Desktop.Common.Modules.Module module, V_HIS_TREATMENT_4 treat, RefeshReference _refresh)
            : base(module)
        {
            InitializeComponent();
            this.treatment = treat;
            this.refresh = _refresh;
            HisConfigCFG.LoadConfig();
        }

        private void fromAppointmentInfo_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                if (this.treatment != null)
                {
                    gridCol_Select.Image = imageListIcon.Images[6];
                    this.ValidationTimeAppointments();
                    this.LoadExamRoom();
                    this.SetDefaultValueControl();
                    this.InitCboTimeFrame();
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadExamRoom()
        {
            try
            {
                List<HIS_EXECUTE_ROOM> executeRooms = BackendDataWorker.Get<HIS_EXECUTE_ROOM>().Where(p => p.IS_ACTIVE == 1 && p.IS_EXAM == 1).ToList();

                this.listExamRoom = new List<ExamRoomADO>();
                foreach (var item in executeRooms)
                {
                    ExamRoomADO ado = new ExamRoomADO(item);
                    this.listExamRoom.Add(ado);
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
                ProcessLoadAppointmentCount();

                cboTimeFrame.Properties.DataSource = this.listTimeFrame;
                cboTimeFrame.Properties.DisplayMember = "TIME_FRAME";
                cboTimeFrame.Properties.ValueMember = "ID";
                cboTimeFrame.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboTimeFrame.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboTimeFrame.Properties.ImmediatePopup = true;
                cboTimeFrame.ForceInitialize();
                cboTimeFrame.Properties.View.Columns.Clear();
                cboTimeFrame.Properties.PopupFormSize = new System.Drawing.Size(300, 250);

                GridColumn aColumnCode = cboTimeFrame.Properties.View.Columns.AddField("TIME_FRAME");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 100;
                aColumnCode.UnboundType = UnboundColumnType.Object;

                GridColumn aColumnName = cboTimeFrame.Properties.View.Columns.AddField("COUNT_TIME_FRAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 200;
                aColumnName.UnboundType = UnboundColumnType.Object;
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
                this.listTimeFrame = new List<HisAppointmentPeriodCountByDateSDO>();

                HisAppointmentPeriodCountByDateFilter filter = new HisAppointmentPeriodCountByDateFilter();
                filter.APPOINTMENT_DATE = Convert.ToInt64(dtAppointmentTime.DateTime.ToString("yyyyMMdd") + "000000");
                filter.BRANCH_ID = HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId();
                filter.IS_ACTIVE = 1;
                this.listTimeFrame = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HisAppointmentPeriodCountByDateSDO>>("api/HisAppointmentPeriod/GetCountByDate", ApiConsumers.MosConsumer, filter, null);
                if (this.listTimeFrame != null && this.listTimeFrame.Count > 0)
                {
                    this.listTimeFrame = this.listTimeFrame.OrderBy(o => o.FROM_HOUR ?? 0).ThenBy(o => o.FROM_MINUTE ?? 0).ThenBy(o => o.TO_HOUR ?? 23).ThenBy(o => o.TO_MINUTE ?? 59).ToList();
                }
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
                if (this.treatment.APPOINTMENT_TIME.HasValue)
                {
                    DateTime? timeAppointment = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.treatment.APPOINTMENT_TIME.Value);
                    dtAppointmentTime.EditValue = timeAppointment;
                }
                else
                {
                    SetAppoinmentTime();
                }

                txtAdvise.Text = this.treatment.ADVISE;

                this.editCombo = true;
                if (this.treatment.APPOINTMENT_PERIOD_ID.HasValue)
                {
                    cboTimeFrame.EditValue = this.treatment.APPOINTMENT_PERIOD_ID.Value;
                }
                else
                {
                    cboTimeFrame.EditValue = ProcessGetFromTime(dtAppointmentTime.DateTime);
                }
                this.editCombo = false;

                if (!string.IsNullOrEmpty(this.treatment.APPOINTMENT_EXAM_ROOM_IDS) && this.listExamRoom != null && this.listExamRoom.Count > 0)
                {
                    string[] ids = this.treatment.APPOINTMENT_EXAM_ROOM_IDS.Split(',');
                    foreach (ExamRoomADO item in this.listExamRoom)
                    {
                        var dataCheck = ids.FirstOrDefault(p => p.Trim() == item.ID.ToString().Trim());
                        if (!string.IsNullOrEmpty(dataCheck))
                            item.IsCheck = true;
                    }
                }

                this.listExamRoom = this.listExamRoom.OrderByDescending(p => p.IsCheck).ThenBy(p => p.EXECUTE_ROOM_CODE).ToList();

                gridControlExamRoom.BeginUpdate();
                gridControlExamRoom.DataSource = this.listExamRoom;
                gridControlExamRoom.EndUpdate();

                dtAppointmentTime.Focus();
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
                if (HisConfigCFG.AppointmentTimeOption == "1")
                {
                    HisServiceReqFilter filter = new HisServiceReqFilter();
                    filter.TREATMENT_ID = this.treatment.ID;
                    filter.SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK;

                    List<HIS_SERVICE_REQ> serviceReqs = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, filter, null);

                    var serviceReq = serviceReqs != null ? serviceReqs.Where(o => o.USE_TIME_TO.HasValue).OrderByDescending(o => o.USE_TIME_TO).FirstOrDefault() : null;
                    if (serviceReq != null)
                    {
                        DateTime dtUseTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(serviceReq.USE_TIME_TO.Value) ?? DateTime.MinValue;
                        dtAppointmentTime.DateTime = dtUseTime.AddDays((double)1);
                        return;
                    }
                }

                if (HisConfigCFG.AppointmentTimeDefault > 0)
                {
                    if (this.treatment.OUT_TIME.HasValue)
                    {
                        dtAppointmentTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Calculation.Add(this.treatment.OUT_TIME.Value, HisConfigCFG.AppointmentTimeDefault - 1, Inventec.Common.DateTime.Calculation.UnitDifferenceTime.DAY) ?? 0) ?? DateTime.Now;
                    }
                    else
                    {
                        dtAppointmentTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(
                            Inventec.Common.DateTime.Calculation.Add(Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0,
                            HisConfigCFG.AppointmentTimeDefault - 1,
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

        private void ValidationTimeAppointments()
        {
            try
            {
                this.ValidationSingleControl(this.dtAppointmentTime, dxValidationProvider1);
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
                if (dtAppointmentTime.EditValue != null)
                {
                    TimeSpan ts = (TimeSpan)(dtAppointmentTime.DateTime.Date - DateTime.Now.Date);
                    spinDayNumber.Value = ts.Days;
                    cboTimeFrame.EditValue = ProcessGetFromTime(dtAppointmentTime.DateTime);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private long? ProcessGetFromTime(DateTime dateTime)
        {
            long? result = null;
            try
            {
                if (this.listTimeFrame != null && this.listTimeFrame.Count > 0 && dateTime != DateTime.MinValue)
                {
                    string timeStr = dateTime.ToString("HHmm");
                    long time = Convert.ToInt64(timeStr);
                    var lstTimefe = this.listTimeFrame.Where(o =>
                        Convert.ToInt64(string.Format("{0:00}", o.FROM_HOUR ?? 0) + string.Format("{0:00}", o.FROM_MINUTE ?? 0)) <= time &&
                         time <= Convert.ToInt64(string.Format("{0:00}", o.TO_HOUR ?? 23) + "" + string.Format("{0:00}", o.TO_MINUTE ?? 59))).ToList();

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

        private void CalculateDateTo()
        {
            try
            {
                if (dtAppointmentTime.EditValue != null)
                {
                    DateTime appoint = DateTime.Now.AddDays((double)(spinDayNumber.Value));
                    dtAppointmentTime.DateTime = new DateTime(appoint.Year, appoint.Month, appoint.Day, dtAppointmentTime.DateTime.Hour, dtAppointmentTime.DateTime.Minute, dtAppointmentTime.DateTime.Second);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtAppointmentTime_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (!this.editCombo)
                {
                    this.editTime = true;
                    DateEdit editor = sender as DateEdit;
                    if (editor != null)
                    {
                        this.CalculateDayNum();

                        if (editor.OldEditValue != null)
                        {
                            DateTime oldValue = (DateTime)editor.OldEditValue;
                            if (oldValue != DateTime.MinValue && (editor.DateTime.Day != oldValue.Day || editor.DateTime.Month != oldValue.Month || editor.DateTime.Year != oldValue.Year))
                            {
                                cboTimeFrame.EditValue = null;
                                cboTimeFrame.Properties.DataSource = null;
                                ProcessLoadAppointmentCount();
                                cboTimeFrame.Properties.DataSource = this.listTimeFrame;
                                cboTimeFrame.EditValue = ProcessGetFromTime(editor.DateTime);
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
                this.editTime = false;
            }
        }

        private void dtAppointmentTime_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinDayNumber.Focus();
                    spinDayNumber.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinDayNumber_EditValueChanged(object sender, EventArgs e)
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

        private void spinDayNumber_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.listTimeFrame != null && this.listTimeFrame.Count > 0)
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

        private void cboTimeFrame_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    if (cboTimeFrame.EditValue != null)
                    {
                        string id = cboTimeFrame.EditValue.ToString();
                        var rowdata = this.listTimeFrame.FirstOrDefault(o => o.ID == Convert.ToInt64(id));
                        if (rowdata != null && rowdata.CURRENT_COUNT >= (rowdata.MAXIMUM ?? 0) && (rowdata.MAXIMUM ?? 0) > 0)
                        {
                            if (DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.KhungGioVuotQuaSoLuong, ResourceMessage.ThongBao, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTimeFrame_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                if (cboTimeFrame.EditValue != null)
                {
                    var dataRow = this.listTimeFrame.FirstOrDefault(o => o.ID == Convert.ToInt64(cboTimeFrame.EditValue.ToString()));
                    if (dataRow != null)
                    {
                        e.DisplayText = string.Format("{0}:{1} - {2}:{3}", dataRow.FROM_HOUR ?? 0, dataRow.FROM_MINUTE ?? 0, dataRow.TO_HOUR ?? 23, dataRow.TO_MINUTE ?? 59);
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
                if (cboTimeFrame.EditValue != null && !this.editTime)
                {
                    this.editCombo = true;
                    string id = cboTimeFrame.EditValue.ToString();
                    var rowdata = this.listTimeFrame.FirstOrDefault(o => o.ID == Convert.ToInt64(id));
                    if (rowdata != null)
                    {
                        string timeStr = dtAppointmentTime.DateTime.ToString("HHmm");
                        long time = Convert.ToInt64(timeStr);
                        long timeFrom = Convert.ToInt64(string.Format("{0:00}", rowdata.FROM_HOUR ?? 0) + string.Format("{0:00}", rowdata.FROM_MINUTE ?? 0));
                        long timeto = Convert.ToInt64(string.Format("{0:00}", rowdata.TO_HOUR ?? 23) + "" + string.Format("{0:00}", rowdata.TO_MINUTE ?? 59));
                        if (!(timeFrom <= time && time <= timeto))
                        {
                            int hour = (int)(rowdata.FROM_HOUR ?? 0);
                            int minute = (int)(rowdata.FROM_MINUTE ?? 0);
                            dtAppointmentTime.DateTime = new DateTime(dtAppointmentTime.DateTime.Year, dtAppointmentTime.DateTime.Month, dtAppointmentTime.DateTime.Day, hour, minute, 0);
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
                this.editCombo = false;
            }
        }

        private void txtSearch_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                string str = txtSearch.Text.Trim();
                List<ExamRoomADO> listData = new List<ExamRoomADO>();
                if (!string.IsNullOrEmpty(str))
                {
                    listData = this.listExamRoom.Where(p => p.EXECUTE_ROOM_CODE.ToUpper().Contains(str.ToUpper()) || p.EXECUTE_ROOM_NAME.ToUpper().Contains(str.ToUpper())).ToList();
                }
                else
                {
                    listData = this.listExamRoom;
                }
                listData = listData.OrderByDescending(p => p.IsCheck).ThenBy(p => p.EXECUTE_ROOM_CODE).ToList();

                gridControlExamRoom.BeginUpdate();
                gridControlExamRoom.DataSource = listData;
                gridControlExamRoom.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExamRoom_MouseDown(object sender, MouseEventArgs e)
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
                                if (this.listExamRoom != null && this.listExamRoom.Count > 0)
                                {
                                    var dataChecks = this.listExamRoom.Where(p => p.IsCheck).ToList();
                                    if (dataChecks != null && dataChecks.Count > 0)
                                    {
                                        gridCol_Select.Image = imageListIcon.Images[5];
                                    }
                                    else
                                    {
                                        gridCol_Select.Image = imageListIcon.Images[6];
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
                            gridCol_Select.Image = imageListIcon.Images[5];
                            gridViewExamRoom.BeginUpdate();
                            if (this.listExamRoom == null)
                                this.listExamRoom = new List<ExamRoomADO>();
                            if (isCheckAll)
                            {
                                foreach (var item in this.listExamRoom)
                                {
                                    item.IsCheck = true;
                                }
                                isCheckAll = false;
                            }
                            else
                            {
                                gridCol_Select.Image = imageListIcon.Images[6];
                                foreach (var item in this.listExamRoom)
                                {
                                    item.IsCheck = false;
                                }
                                isCheckAll = true;
                            }
                            gridViewExamRoom.EndUpdate();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtAdvise_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSave.Focus();
                    e.Handled = true;
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
                if (btnAppointmentService.Enabled && this.treatment != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(this.treatment.ID);

                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.AppointmentService", this.currentModuleBase.RoomId, this.currentModuleBase.RoomTypeId, listArgs);
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
                this.positionHandle = -1;
                if (!btnSave.Enabled || !dxValidationProvider1.Validate())
                {
                    return;
                }

                //Kiểm tra nếu số ngày hẹn khám vượt quá số ngày được cấu hình
                if (HisConfigCFG.MaxOfAppointmentDays.HasValue && spinDayNumber.Value > HisConfigCFG.MaxOfAppointmentDays.Value)
                {
                    //Tùy chọn xử lý trong trường hợp vượt quá số ngày hẹn khám. 1: Cảnh báo. 2: Chặn, không cho xử trí
                    if (HisConfigCFG.WarningOptionWhenExceedingMaxOfAppointmentDays.HasValue && HisConfigCFG.WarningOptionWhenExceedingMaxOfAppointmentDays.Value == 2)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(string.Format(ResourceMessage.CanhBaoNgayHenToiDa, HisConfigCFG.MaxOfAppointmentDays),
                        ResourceMessage.ThongBao,
                        MessageBoxButtons.OK);
                        return;
                    }
                    else if (HisConfigCFG.WarningOptionWhenExceedingMaxOfAppointmentDays == 1)
                    {
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(string.Format(ResourceMessage.CanhBaoNgayHenToiDaBanCoMuonTiepTuc, HisConfigCFG.MaxOfAppointmentDays),
                        ResourceMessage.ThongBao,
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;
                    }
                }

                if (this.dtAppointmentTime.DateTime.DayOfWeek == DayOfWeek.Sunday)
                {
                    if (DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.CanhBaoNgayHenLaChuNhat,
                    ResourceMessage.ThongBao,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;
                }
                else if (this.dtAppointmentTime.DateTime.DayOfWeek == DayOfWeek.Saturday)
                {
                    if (DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.CanhBaoNgayHenLaThuBay,
                    ResourceMessage.ThongBao,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;
                }

                long dtAppointmentTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(this.dtAppointmentTime.DateTime) ?? 0;

                if (this.treatment.OUT_TIME.HasValue && dtAppointmentTime < this.treatment.OUT_TIME.Value)
                {
                    string appoint = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dtAppointmentTime);
                    string outTime = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this.treatment.OUT_TIME.Value);
                    MessageBox.Show(String.Format(ResourceMessage.CanhBaoThoiGianHenKhamSoVoiThoiGianKetThucDieuTri, appoint, outTime), ResourceMessage.ThongBao);
                    this.dtAppointmentTime.Focus();
                    this.dtAppointmentTime.SelectAll();
                    return;
                }
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;

                TreatmentAppointmentInfoSDO appointmentSDO = new TreatmentAppointmentInfoSDO();
                appointmentSDO.TreatmentId = this.treatment.ID;

                var datas = (List<ExamRoomADO>)gridControlExamRoom.DataSource;
                if (datas != null && datas.Count > 0)
                {
                    List<ExamRoomADO> seleted = datas.Where(p => p.IsCheck).ToList();

                    if (seleted != null && seleted.Count > 0)
                    {
                        if (!this.CheckMaxAppointment(seleted))
                        {
                            return;
                        }
                        appointmentSDO.AppointmentExamRoomIds = seleted.Select(s => s.ID).Distinct().ToList(); ;
                    }
                    else
                    {
                        appointmentSDO.AppointmentExamRoomIds = null;
                    }
                }
                else
                {
                    appointmentSDO.AppointmentExamRoomIds = null;
                }

                appointmentSDO.Advise = txtAdvise.Text;
                appointmentSDO.AppointmentTime = dtAppointmentTime;

                if (cboTimeFrame.EditValue != null)
                {
                    appointmentSDO.AppointmentPeriodId = Convert.ToInt64(cboTimeFrame.EditValue.ToString());
                }
                else
                {
                    appointmentSDO.AppointmentPeriodId = null;
                }

                LogSystem.Debug(LogUtil.TraceData("Input", appointmentSDO));
                HIS_TREATMENT rs = new BackendAdapter(param).Post<HIS_TREATMENT>("api/HisTreatment/UpdateAppointmentInfo", ApiConsumers.MosConsumer, appointmentSDO, param);
                if (rs != null)
                {
                    success = true;
                    if (this.refresh != null) this.refresh();
                }

                WaitingManager.Hide();
                MessageManager.Show(this, param, success);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool CheckMaxAppointment(List<ExamRoomADO> selected)
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
                    filter.INTR_OR_APPOINT_DATE = Convert.ToInt64(this.dtAppointmentTime.DateTime.ToString("yyyyMMdd") + "000000");
                    LogSystem.Debug("Filter: \n" + LogUtil.TraceData("Filter", filter));

                    List<HisExecuteRoomAppointedSDO> sdos = new BackendAdapter(new CommonParam()).Get<List<HisExecuteRoomAppointedSDO>>("api/HisExecuteRoom/GetCountAppointed", ApiConsumers.MosConsumer, filter, null);
                    List<HisExecuteRoomAppointedSDO> overs = sdos != null ? sdos.Where(o => (o.MaxAmount ?? 0) > 0 && (o.CurrentAmount ?? 0) > 0 && o.CurrentAmount.Value >= o.MaxAmount).ToList() : null;
                    LogSystem.Debug("sdos: \n" + LogUtil.TraceData("sdos", sdos));
                    if (overs != null && overs.Count > 0)
                    {
                        string names = String.Join(", ", overs.Select(s => String.Format("{0}({1}/{2})", s.ExecuteRoomName, s.CurrentAmount, s.MaxAmount)).ToList());
                        string mess = String.Format(ResourceMessage.PhongKhamCoSoLuotKhamVuotDinhMuc, names);
                        WaitingManager.Hide();
                        if (XtraMessageBox.Show(mess, ResourceMessage.ThongBao, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != System.Windows.Forms.DialogResult.Yes)
                        {
                            return false;
                        }
                        else
                        {
                            WaitingManager.Show();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void barBtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
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

        private void gridLookUpEdit1View_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound && cboTimeFrame.Properties.DataSource != null)
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
                var row = (HisAppointmentPeriodCountByDateSDO)gridLookUpEdit1View.GetRow(e.RowHandle);
                if (row != null)
                {
                    if (row.CURRENT_COUNT >= (row.MAXIMUM ?? 0) && (row.MAXIMUM ?? 0) > 0)
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
    }
}
