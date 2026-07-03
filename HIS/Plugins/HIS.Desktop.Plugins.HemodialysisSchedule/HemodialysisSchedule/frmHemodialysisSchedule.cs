/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * Xếp lịch chạy thận MỚI (4.2.1): đưa BN vào slot Phòng+Ngày+Ca, KHÔNG sinh y lệnh.
 */
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.HemodialysisSchedule.ADO;
using HIS.Desktop.Plugins.HemodialysisSchedule.Filter;
using HIS.Desktop.Plugins.HemodialysisSchedule.SDO;
using HIS.Desktop.Utilities;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HemodialysisSchedule
{
    public partial class frmHemodialysisSchedule : FormBase
    {
        #region Declare
        Inventec.Desktop.Common.Modules.Module moduleData;
        long currentRoomId;
        long currentDepartmentId;
        string loginName;

        List<HemodialysisScheduleADO> scheduleADOs;
        List<TreatmentInfoADO> treatmentADOs;
        List<ExpMestTemplateADO> templateADOs;
        #endregion

        #region Construct
        public frmHemodialysisSchedule(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();
                this.moduleData = moduleData;

                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Load
        private void frmHemodialysisSchedule_Load(object sender, EventArgs e)
        {
            try
            {
                LoadContext();
                InitComboRoom();
                InitComboShift();
                InitComboDepartment();
                LoadTemplates();
                SetDefaultValue();
                LoadScheduleGrid();
                LoadTreatmentGrid();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void LoadContext()
        {
            try
            {
                var workPlace = HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetWorkPlace(this.moduleData);
                if (workPlace != null)
                {
                    this.currentRoomId = workPlace.RoomId;
                    this.currentDepartmentId = workPlace.DepartmentId;
                }
                this.loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        #endregion

        #region Init combo
        private void InitComboRoom()
        {
            try
            {
                var rooms = BackendDataWorker.Get<V_HIS_EXECUTE_ROOM>()
                    .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    .OrderBy(o => o.EXECUTE_ROOM_NAME)
                    .ToList();
                cboRoom.Properties.DataSource = rooms;
                cboRoom.Properties.DisplayMember = "EXECUTE_ROOM_NAME";
                cboRoom.Properties.ValueMember = "ROOM_ID";
                cboRoom.Properties.PopupFormWidth = 300;
                cboRoom.Properties.View.OptionsView.ShowAutoFilterRow = true;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void InitComboShift()
        {
            try
            {
                var shifts = new List<ShiftADO>();
                for (short i = 1; i <= 5; i++)
                {
                    shifts.Add(new ShiftADO(i, "Ca " + i));
                }
                cboShift.Properties.DataSource = shifts;
                cboShift.Properties.DisplayMember = "NAME";
                cboShift.Properties.ValueMember = "ID";
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void InitComboDepartment()
        {
            try
            {
                var departments = BackendDataWorker.Get<HIS_DEPARTMENT>()
                    .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    .OrderBy(o => o.DEPARTMENT_NAME)
                    .ToList();
                cboDepartment.Properties.DataSource = departments;
                cboDepartment.Properties.DisplayMember = "DEPARTMENT_NAME";
                cboDepartment.Properties.ValueMember = "ID";
                cboDepartment.Properties.PopupFormWidth = 300;
                cboDepartment.Properties.View.OptionsView.ShowAutoFilterRow = true;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void SetDefaultValue()
        {
            try
            {
                cboRoom.EditValue = this.currentRoomId;
                cboShift.EditValue = (short)1;
                dtDate.DateTime = DateTime.Now;
                dtCopyFromDate.DateTime = DateTime.Now.AddDays(-1);

                cboDepartment.EditValue = this.currentDepartmentId;
                chkAllDepartment.Checked = false;
                dtInTimeFrom.DateTime = DateTime.Now.AddDays(-7);
                dtInTimeTo.DateTime = DateTime.Now;

                txtSearchTop.Text = "";
                txtSearchBottom.Text = "";
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Templates (Gói vật tư)
        private void LoadTemplates()
        {
            try
            {
                CommonParam param = new CommonParam();
                var filter = new ExpMestTemplateFilter();
                filter.CREATOR = this.loginName;
                filter.IS_PUBLIC = 1;
                filter.IS_KIDNEY = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;

                this.templateADOs = new BackendAdapter(param).Get<List<ExpMestTemplateADO>>(
                    HisRequestUriStore.HIS_EXP_MEST_TEMPLATE_GET, ApiConsumers.MosConsumer, filter, param)
                    ?? new List<ExpMestTemplateADO>();

                repoTemplate.DataSource = this.templateADOs;
                repoTemplate.ValueMember = "ID";
                repoTemplate.DisplayMember = "EXP_MEST_TEMPLATE_NAME";

                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                this.templateADOs = new List<ExpMestTemplateADO>();
            }
        }

        private string GetTemplateName(long? id)
        {
            if (id == null || this.templateADOs == null) return "";
            var t = this.templateADOs.FirstOrDefault(o => o.ID == id.Value);
            return t != null ? t.EXP_MEST_TEMPLATE_NAME : "";
        }
        #endregion

        #region Load schedule grid (vùng trên)
        private void LoadScheduleGrid()
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                var filter = new HemodialysisScheduleFilter();
                filter.ROOM_ID = GetSelectedRoomId();
                filter.SCHEDULE_DATE = GetDateNumber(dtDate);
                filter.KIDNEY_SHIFT = GetSelectedShift();
                filter.KEY_WORD = string.IsNullOrWhiteSpace(txtSearchTop.Text) ? null : txtSearchTop.Text.Trim();
                filter.ORDER_FIELD = "KIDNEY_SHIFT";
                filter.ORDER_DIRECTION = "ASC";

                this.scheduleADOs = new BackendAdapter(param).Get<List<HemodialysisScheduleADO>>(
                    HisRequestUriStore.HEMODIALYSIS_SCHEDULE_GET, ApiConsumers.MosConsumer, filter, param)
                    ?? new List<HemodialysisScheduleADO>();

                // Đổ tên gói vật tư để hiển thị nếu backend chưa join sẵn
                foreach (var ado in this.scheduleADOs)
                {
                    if (string.IsNullOrEmpty(ado.EXP_MEST_TEMPLATE_NAME))
                        ado.EXP_MEST_TEMPLATE_NAME = GetTemplateName(ado.EXP_MEST_TEMPLATE_ID);
                }

                gridControlSchedule.DataSource = this.scheduleADOs;
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            finally
            {
                WaitingManager.Hide();
            }
        }

        private long? GetSelectedRoomId()
        {
            if (cboRoom.EditValue == null) return null;
            return Inventec.Common.TypeConvert.Parse.ToInt64(cboRoom.EditValue.ToString());
        }

        private short? GetSelectedShift()
        {
            if (cboShift.EditValue == null) return null;
            return Inventec.Common.TypeConvert.Parse.ToInt16(cboShift.EditValue.ToString());
        }

        private long? GetDateNumber(DevExpress.XtraEditors.DateEdit dateEdit)
        {
            try
            {
                if (dateEdit.EditValue == null) return null;
                return Convert.ToInt64(dateEdit.DateTime.ToString("yyyyMMdd"));
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
                return null;
            }
        }
        #endregion

        #region Load treatment grid (vùng dưới)
        private void LoadTreatmentGrid()
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                var filter = new TreatmentInfoFilter();
                filter.IS_IN_TREATMENT = true;
                if (!chkAllDepartment.Checked)
                {
                    filter.DEPARTMENT_ID = cboDepartment.EditValue != null
                        ? (long?)Inventec.Common.TypeConvert.Parse.ToInt64(cboDepartment.EditValue.ToString())
                        : this.currentDepartmentId;
                }
                var inFrom = GetDateNumber(dtInTimeFrom);
                var inTo = GetDateNumber(dtInTimeTo);
                filter.IN_TIME_FROM = inFrom != null ? (long?)Convert.ToInt64(inFrom.Value.ToString() + "000000") : null;
                filter.IN_TIME_TO = inTo != null ? (long?)Convert.ToInt64(inTo.Value.ToString() + "235959") : null;
                filter.KEY_WORD = string.IsNullOrWhiteSpace(txtSearchBottom.Text) ? null : txtSearchBottom.Text.Trim();
                filter.ORDER_FIELD = "IN_TIME";
                filter.ORDER_DIRECTION = "DESC";

                this.treatmentADOs = new BackendAdapter(param).Get<List<TreatmentInfoADO>>(
                    HisRequestUriStore.V_HIS_TREATMENT_4_GET, ApiConsumers.MosConsumer, filter, param)
                    ?? new List<TreatmentInfoADO>();

                gridControlTreatment.DataSource = this.treatmentADOs;
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            finally
            {
                WaitingManager.Hide();
            }
        }
        #endregion

        #region Toolbar buttons
        private void btnSearchTop_Click(object sender, EventArgs e)
        {
            try { LoadScheduleGrid(); }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private void btnSearchBottom_Click(object sender, EventArgs e)
        {
            try { LoadTreatmentGrid(); }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Commit editor đang mở (inline edit đã tự lưu qua CellValueChanged) rồi refresh
                gridViewSchedule.CloseEditor();
                gridViewSchedule.UpdateCurrentRow();
                LoadScheduleGrid();
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                gridControlSchedule.ShowPrintPreview();
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }
        #endregion

        #region R5 — Đưa vào lịch
        private void btnAddToSchedule_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                gridViewTreatment.CloseEditor();
                gridViewTreatment.UpdateCurrentRow();

                var selecteds = (this.treatmentADOs ?? new List<TreatmentInfoADO>())
                    .Where(o => o.IsSelected).ToList();
                if (selecteds.Count == 0)
                {
                    XtraMessageBoxLike(Resources.ResourceMessageLang.VuiLongChonItNhatMotBenhNhan);
                    return;
                }

                long? roomId = GetSelectedRoomId();
                long? scheduleDate = GetDateNumber(dtDate);
                short? shift = GetSelectedShift();
                if (scheduleDate == null)
                {
                    XtraMessageBoxLike(Resources.ResourceMessageLang.VuiLongChonNgayXepLich);
                    return;
                }

                var toCreate = new List<HemodialysisScheduleADO>();
                foreach (var t in selecteds)
                {
                    toCreate.Add(new HemodialysisScheduleADO()
                    {
                        TREATMENT_ID = t.ID,
                        PATIENT_ID = t.PATIENT_ID,
                        ROOM_ID = roomId ?? this.currentRoomId,
                        SCHEDULE_DATE = scheduleDate.Value,
                        KIDNEY_SHIFT = shift ?? (short)1,
                        // EXP_MEST_TEMPLATE_ID, NOTE để trống — cập nhật inline sau
                    });
                }

                WaitingManager.Show();
                var result = new BackendAdapter(param).Post<List<HemodialysisScheduleADO>>(
                    HisRequestUriStore.HEMODIALYSIS_SCHEDULE_CREATE_LIST, ApiConsumers.MosConsumer, toCreate, param);
                WaitingManager.Hide();

                if (result != null)
                {
                    success = true;
                    LoadScheduleGrid();
                    MessageManager.Show(this, param, success);
                    XtraMessageBoxLike(string.Format(Resources.ResourceMessageLang.DuaVaoLichThanhCongFormat, result.Count));
                }
                else
                {
                    MessageManager.Show(this, param, success);
                }

                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.treatmentADOs == null) return;
                foreach (var t in this.treatmentADOs)
                {
                    t.IsSelected = chkSelectAll.Checked;
                }
                gridViewTreatment.RefreshData();
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }
        #endregion

        #region Inline edit (Gói vật tư / Ghi chú) + Delete
        private void gridViewSchedule_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                if (e.RowHandle < 0) return;
                if (e.Column.FieldName != "EXP_MEST_TEMPLATE_ID" && e.Column.FieldName != "NOTE") return;

                gridViewSchedule.CloseEditor();
                gridViewSchedule.UpdateCurrentRow();

                var ado = gridViewSchedule.GetRow(e.RowHandle) as HemodialysisScheduleADO;
                if (ado == null) return;

                if (e.Column.FieldName == "EXP_MEST_TEMPLATE_ID")
                    ado.EXP_MEST_TEMPLATE_NAME = GetTemplateName(ado.EXP_MEST_TEMPLATE_ID);

                UpdateSlot(ado);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void UpdateSlot(HemodialysisScheduleADO ado)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                WaitingManager.Show();
                var result = new BackendAdapter(param).Post<HemodialysisScheduleADO>(
                    HisRequestUriStore.HEMODIALYSIS_SCHEDULE_UPDATE, ApiConsumers.MosConsumer, ado, param);
                WaitingManager.Hide();
                success = result != null;
                MessageManager.Show(this, param, success);
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        private void repoDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                var ado = gridViewSchedule.GetFocusedRow() as HemodialysisScheduleADO;
                if (ado == null)
                {
                    XtraMessageBoxLike(Resources.ResourceMessageLang.VuiLongChonSlotDeXoa);
                    return;
                }
                if (!ConfirmYesNo(Resources.ResourceMessageLang.XacNhanXoaSlot)) return;

                WaitingManager.Show();
                var result = new BackendAdapter(param).Post<bool>(
                    HisRequestUriStore.HEMODIALYSIS_SCHEDULE_DELETE, ApiConsumers.MosConsumer, ado.ID, param);
                WaitingManager.Hide();
                success = (param.HasException == false);
                if (success) LoadScheduleGrid();
                MessageManager.Show(this, param, success);
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }
        #endregion

        #region R6 — Sao chép lịch
        private void btnCopy_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            try
            {
                long? roomId = GetSelectedRoomId();
                long? sourceDate = GetDateNumber(dtCopyFromDate);
                long? targetDate = GetDateNumber(dtDate);
                if (sourceDate == null)
                {
                    XtraMessageBoxLike(Resources.ResourceMessageLang.VuiLongChonNgayNguon);
                    return;
                }
                if (targetDate == null)
                {
                    XtraMessageBoxLike(Resources.ResourceMessageLang.VuiLongChonNgayXepLich);
                    return;
                }
                if (sourceDate == targetDate)
                {
                    XtraMessageBoxLike(Resources.ResourceMessageLang.NgayNguonVaDichTrung);
                    return;
                }

                // Đọc lịch ngày nguồn + ngày đích để dựng bảng tóm tắt sẽ thêm / sẽ skip
                var sourceList = GetScheduleByDate(roomId, sourceDate.Value, param);
                if (sourceList == null || sourceList.Count == 0)
                {
                    XtraMessageBoxLike(Resources.ResourceMessageLang.KhongCoBanGhiDeSaoChep);
                    return;
                }
                var targetList = GetScheduleByDate(roomId, targetDate.Value, param);
                var targetKeys = new HashSet<string>((targetList ?? new List<HemodialysisScheduleADO>())
                    .Select(o => o.TREATMENT_ID + "|" + o.KIDNEY_SHIFT));

                var willAdd = new List<HemodialysisScheduleADO>();
                var willSkip = new List<HemodialysisScheduleADO>();
                foreach (var s in sourceList)
                {
                    if (targetKeys.Contains(s.TREATMENT_ID + "|" + s.KIDNEY_SHIFT))
                        willSkip.Add(s);
                    else
                        willAdd.Add(s);
                }

                using (var frm = new frmCopyScheduleConfirm(
                    cboRoom.Text, sourceDate.Value, targetDate.Value, willAdd, willSkip))
                {
                    if (frm.ShowDialog(this) != DialogResult.OK) return;
                }

                // Xác nhận → gọi API CopySchedule thực hiện insert + skip trùng
                var sdo = new CopyScheduleSDO()
                {
                    ROOM_ID = roomId ?? this.currentRoomId,
                    SOURCE_DATE = sourceDate.Value,
                    TARGET_DATE = targetDate.Value
                };
                WaitingManager.Show();
                var result = new BackendAdapter(param).Post<CopyScheduleResultADO>(
                    HisRequestUriStore.HEMODIALYSIS_SCHEDULE_COPY, ApiConsumers.MosConsumer, sdo, param);
                WaitingManager.Hide();

                if (result != null)
                {
                    LoadScheduleGrid();
                    MessageManager.Show(this, param, true);
                    XtraMessageBoxLike(string.Format(Resources.ResourceMessageLang.SaoChepThanhCongFormat, result.AddedCount));
                }
                else
                {
                    MessageManager.Show(this, param, false);
                }
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        private List<HemodialysisScheduleADO> GetScheduleByDate(long? roomId, long date, CommonParam param)
        {
            try
            {
                var filter = new HemodialysisScheduleFilter();
                filter.ROOM_ID = roomId;
                filter.SCHEDULE_DATE = date;
                return new BackendAdapter(param).Get<List<HemodialysisScheduleADO>>(
                    HisRequestUriStore.HEMODIALYSIS_SCHEDULE_GET, ApiConsumers.MosConsumer, filter, param)
                    ?? new List<HemodialysisScheduleADO>();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                return new List<HemodialysisScheduleADO>();
            }
        }
        #endregion

        #region Grid display helpers
        private void gridViewSchedule_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.FieldName == "STT")
                {
                    var view = sender as GridView;
                    var ds = view != null ? view.DataSource as IList : null;
                    if (ds != null) e.Value = e.ListSourceRowIndex + 1;
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private void gridViewTreatment_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.FieldName == "STT")
                {
                    var view = sender as GridView;
                    var ds = view != null ? view.DataSource as IList : null;
                    if (ds != null) e.Value = e.ListSourceRowIndex + 1;
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private void gridViewSchedule_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
        {
            try
            {
                if (e.Value == null) return;
                if (e.Column.FieldName == "SCHEDULE_DATE")
                    e.DisplayText = FormatDateNumber(e.Value, false);
                else if (e.Column.FieldName == "TDL_PATIENT_DOB")
                    e.DisplayText = FormatDob(e.Value, GetRowShort(sender, "TDL_PATIENT_IS_HAS_NOT_DAY_DOB"));
                else if (e.Column.FieldName == "IN_TIME")
                    e.DisplayText = FormatDateNumber(e.Value, true);
                else if (e.Column.FieldName == "KIDNEY_SHIFT")
                    e.DisplayText = "Ca " + e.Value;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private void gridViewTreatment_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
        {
            try
            {
                if (e.Value == null) return;
                if (e.Column.FieldName == "TDL_PATIENT_DOB")
                    e.DisplayText = FormatDob(e.Value, GetRowShort(sender, "TDL_PATIENT_IS_HAS_NOT_DAY_DOB"));
                else if (e.Column.FieldName == "IN_TIME")
                    e.DisplayText = FormatDateNumber(e.Value, false);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private short GetRowShort(object sender, string field)
        {
            try
            {
                var view = sender as GridView;
                if (view == null) return 0;
                var v = view.GetRowCellValue(view.FocusedRowHandle, field);
                return Inventec.Common.TypeConvert.Parse.ToInt16((v ?? "").ToString());
            }
            catch { return 0; }
        }

        private string FormatDateNumber(object value, bool isTime)
        {
            try
            {
                long num = Convert.ToInt64(value);
                if (num <= 0) return "";
                if (isTime)
                    return Inventec.Common.DateTime.Convert.TimeNumberToTimeString(num) ?? "";
                return Inventec.Common.DateTime.Convert.TimeNumberToDateString(num) ?? "";
            }
            catch { return ""; }
        }

        private string FormatDob(object value, short isHasNotDay)
        {
            try
            {
                long num = Convert.ToInt64(value);
                if (num <= 0) return "";
                if (isHasNotDay == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    return num.ToString().Substring(0, 4);
                return Inventec.Common.DateTime.Convert.TimeNumberToDateString(num) ?? "";
            }
            catch { return ""; }
        }
        #endregion

        #region Message helpers
        private void XtraMessageBoxLike(string message)
        {
            DevExpress.XtraEditors.XtraMessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private bool ConfirmYesNo(string message)
        {
            return DevExpress.XtraEditors.XtraMessageBox.Show(message, this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }
        #endregion

        #region Filter events
        private void txtSearchTop_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) LoadScheduleGrid();
        }

        private void txtSearchBottom_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) LoadTreatmentGrid();
        }

        private void chkAllDepartment_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                cboDepartment.Enabled = !chkAllDepartment.Checked;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }
        #endregion
    }
}
