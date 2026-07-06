/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * Xếp lịch chạy thận MỚI (4.2.1): đưa BN vào slot Phòng+Ngày+Ca, KHÔNG sinh y lệnh.
 * Hiển thị dạng UserControl (tab trong cửa sổ chính) trên nền LayoutControl DevExpress.
 */
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
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
    public partial class ucHemodialysisSchedule : UserControlBase
    {
        #region Declare
        Inventec.Desktop.Common.Modules.Module moduleData;
        long currentRoomId;
        long currentDepartmentId;
        string loginName;

        /// <summary>Tiêu đề dùng cho các hộp thoại thông báo (UC không có Text như Form).</summary>
        private readonly string captionText = "Xếp lịch chạy thận";

        List<HemodialysisScheduleADO> scheduleADOs;
        List<TreatmentInfoADO> treatmentADOs;
        List<ExpMestTemplateADO> templateADOs;

        /// <summary>Trạng thái checkbox "chọn tất cả" ở header cột chọn của lưới bệnh nhân.</summary>
        bool isHeaderSelectAll = false;
        #endregion

        #region Construct
        public ucHemodialysisSchedule()
        {
            InitializeComponent();
        }

        public ucHemodialysisSchedule(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();
                this.moduleData = moduleData;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Load
        private void ucHemodialysisSchedule_Load(object sender, EventArgs e)
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
                // Định nghĩa cột tường minh để popup không tự sinh toàn bộ field entity (và không lặp cột khi Load chạy lại)
                gridViewRoom.Columns.Clear();
                gridViewRoom.Columns.AddVisible("EXECUTE_ROOM_CODE").Caption = "Mã phòng";
                gridViewRoom.Columns.AddVisible("EXECUTE_ROOM_NAME").Caption = "Tên phòng";
                gridViewRoom.OptionsView.ShowAutoFilterRow = true;
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
                // Định nghĩa cột tường minh để popup không tự sinh toàn bộ field entity (và không lặp cột khi Load chạy lại)
                gridViewDept.Columns.Clear();
                gridViewDept.Columns.AddVisible("DEPARTMENT_CODE").Caption = "Mã khoa";
                gridViewDept.Columns.AddVisible("DEPARTMENT_NAME").Caption = "Tên khoa";
                gridViewDept.OptionsView.ShowAutoFilterRow = true;
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
        /// <summary>
        /// Nạp danh sách gói vật tư cho combo/inline (giống plugin Xếp lịch chạy thận cũ):
        /// - Nguồn: cache RAM HIS_EXP_MEST_TEMPLATE, fallback api/HisExpMestTemplate/Get.
        /// - Lọc: (CREATOR = user OR IS_PUBLIC = 1) AND IS_KIDNEY = 1 AND IS_ACTIVE = 1.
        /// - Hiển thị 2 cột (Mã + Tên gói); Value = ID.
        /// </summary>
        private void LoadTemplates()
        {
            try
            {
                List<HIS_EXP_MEST_TEMPLATE> datas;
                if (BackendDataWorker.IsExistsKey<HIS_EXP_MEST_TEMPLATE>())
                {
                    datas = BackendDataWorker.Get<HIS_EXP_MEST_TEMPLATE>();
                }
                else
                {
                    CommonParam param = new CommonParam();
                    datas = new BackendAdapter(param).Get<List<HIS_EXP_MEST_TEMPLATE>>(
                        HisRequestUriStore.HIS_EXP_MEST_TEMPLATE_GET, ApiConsumers.MosConsumer, new ExpMestTemplateFilter(), param);
                    if (datas != null)
                        BackendDataWorker.UpdateToRam(typeof(HIS_EXP_MEST_TEMPLATE), datas, Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss")));
                    SessionManager.ProcessTokenLost(param);
                }
                datas = datas ?? new List<HIS_EXP_MEST_TEMPLATE>();

                this.templateADOs = datas
                    .Where(o => (o.CREATOR == this.loginName || (o.IS_PUBLIC ?? -1) == 1)
                        && o.IS_KIDNEY == 1 && o.IS_ACTIVE == 1)
                    .Select(o => new ExpMestTemplateADO
                    {
                        ID = o.ID,
                        EXP_MEST_TEMPLATE_CODE = o.EXP_MEST_TEMPLATE_CODE,
                        EXP_MEST_TEMPLATE_NAME = o.EXP_MEST_TEMPLATE_NAME,
                        IS_PUBLIC = o.IS_PUBLIC,
                        IS_KIDNEY = o.IS_KIDNEY,
                        IS_ACTIVE = o.IS_ACTIVE,
                        CREATOR = o.CREATOR
                    })
                    .OrderBy(o => o.EXP_MEST_TEMPLATE_NAME)
                    .ToList();

                // Combo inline trên lưới lịch (cột "Gói vật tư")
                repoTemplate.DataSource = this.templateADOs;
                repoTemplate.ValueMember = "ID";
                repoTemplate.DisplayMember = "EXP_MEST_TEMPLATE_NAME";
                repoTemplateView.Columns.Clear();
                repoTemplateView.Columns.AddVisible("EXP_MEST_TEMPLATE_CODE").Caption = "Mã gói";
                repoTemplateView.Columns.AddVisible("EXP_MEST_TEMPLATE_NAME").Caption = "Tên gói";

                // Combo Gói vật tư trên thanh lọc
                cboTemplate.Properties.DataSource = this.templateADOs;
                cboTemplate.Properties.ValueMember = "ID";
                cboTemplate.Properties.DisplayMember = "EXP_MEST_TEMPLATE_NAME";
                cboTemplate.Properties.PopupFormWidth = 400;
                gridViewTemplate.Columns.Clear();
                gridViewTemplate.Columns.AddVisible("EXP_MEST_TEMPLATE_CODE").Caption = "Mã gói";
                gridViewTemplate.Columns.AddVisible("EXP_MEST_TEMPLATE_NAME").Caption = "Tên gói";
                gridViewTemplate.OptionsView.ShowAutoFilterRow = true;
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

                // Lấy đúng model bảng HIS_HEMODIALYSIS_SCHEDULE rồi map sang ADO hiển thị
                var entities = new BackendAdapter(param).Get<List<HIS_HEMODIALYSIS_SCHEDULE>>(
                    HisRequestUriStore.HEMODIALYSIS_SCHEDULE_GET, ApiConsumers.MosConsumer, filter, param)
                    ?? new List<HIS_HEMODIALYSIS_SCHEDULE>();

                // Enrich thông tin BN/điều trị (tên, mã, ngày sinh, giới tính, ngày vào...):
                // gọi API HIS_TREATMENT/Get theo danh sách TREATMENT_ID đã xếp — KHÔNG lấy từ cache RAM.
                Dictionary<long, HIS_TREATMENT> treaDict = new Dictionary<long, HIS_TREATMENT>();
                var treaIds = entities.Where(o => o.TREATMENT_ID.HasValue)
                    .Select(o => o.TREATMENT_ID.Value).Distinct().ToList();
                if (treaIds.Count > 0)
                {
                    var treaFilter = new MOS.Filter.HisTreatmentFilter();
                    treaFilter.IDs = treaIds;
                    var treas = new BackendAdapter(param).Get<List<HIS_TREATMENT>>(
                        HisRequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, treaFilter, param)
                        ?? new List<HIS_TREATMENT>();
                    foreach (var t in treas)
                        if (!treaDict.ContainsKey(t.ID)) treaDict[t.ID] = t;
                }

                this.scheduleADOs = new List<HemodialysisScheduleADO>();
                foreach (var o in entities)
                {
                    var ado = new HemodialysisScheduleADO()
                    {
                        ID = o.ID,
                        TREATMENT_ID = o.TREATMENT_ID ?? 0,
                        PATIENT_ID = o.PATIENT_ID ?? 0,
                        ROOM_ID = o.ROOM_ID ?? 0,
                        SCHEDULE_DATE = o.SCHEDULE_DATE ?? 0,
                        KIDNEY_SHIFT = (short)(o.KIDNEY_SHIFT ?? 0),
                        EXP_MEST_TEMPLATE_ID = o.EXP_MEST_TEMPLATE_ID,
                        NOTE = o.NOTE,
                        CREATE_TIME = o.CREATE_TIME,
                        CREATOR = o.CREATOR,
                        MODIFY_TIME = o.MODIFY_TIME,
                        MODIFIER = o.MODIFIER,
                        EXP_MEST_TEMPLATE_NAME = GetTemplateName(o.EXP_MEST_TEMPLATE_ID)
                    };

                    HIS_TREATMENT trea = null;
                    if (o.TREATMENT_ID.HasValue)
                        treaDict.TryGetValue(o.TREATMENT_ID.Value, out trea);
                    if (trea != null)
                    {
                        ado.TREATMENT_CODE = trea.TREATMENT_CODE;
                        ado.TDL_PATIENT_NAME = trea.TDL_PATIENT_NAME;
                        ado.TDL_PATIENT_CODE = trea.TDL_PATIENT_CODE;
                        ado.TDL_PATIENT_DOB = trea.TDL_PATIENT_DOB;
                        ado.TDL_PATIENT_IS_HAS_NOT_DAY_DOB = trea.TDL_PATIENT_IS_HAS_NOT_DAY_DOB;
                        ado.TDL_PATIENT_GENDER_ID = trea.TDL_PATIENT_GENDER_ID;
                        ado.TDL_PATIENT_GENDER_NAME = trea.TDL_PATIENT_GENDER_NAME;
                        ado.IN_TIME = trea.IN_TIME;
                    }

                    this.scheduleADOs.Add(ado);
                }

                gridViewSchedule.BeginUpdate();
                try
                {
                    gridControlSchedule.DataSource = this.scheduleADOs;
                }
                finally
                {
                    gridViewSchedule.EndUpdate();
                }
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

                // Reset trạng thái checkbox "chọn tất cả" ở header mỗi lần nạp lại danh sách
                isHeaderSelectAll = false;

                gridViewTreatment.BeginUpdate();
                try
                {
                    gridControlTreatment.DataSource = this.treatmentADOs;
                }
                finally
                {
                    gridViewTreatment.EndUpdate();
                }
                gridViewTreatment.InvalidateColumnHeader(colSelect);
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

                // Validate các trường bắt buộc theo API CreateList (RoomId, ScheduleDate, KidneyShift 1..5)
                if (roomId == null)
                {
                    XtraMessageBoxLike(Resources.ResourceMessageLang.VuiLongChonPhongChay);
                    return;
                }
                if (scheduleDate == null)
                {
                    XtraMessageBoxLike(Resources.ResourceMessageLang.VuiLongChonNgayXepLich);
                    return;
                }
                if (shift == null || shift < 1 || shift > 5)
                {
                    XtraMessageBoxLike(Resources.ResourceMessageLang.VuiLongChonCa);
                    return;
                }

                // Giá trị tùy chọn áp mặc định cho tất cả slot mới: gói vật tư + ghi chú trên thanh lọc
                long? templateId = cboTemplate.EditValue != null
                    ? (long?)Inventec.Common.TypeConvert.Parse.ToInt64(cboTemplate.EditValue.ToString())
                    : (long?)null;
                string note = string.IsNullOrWhiteSpace(txtNote.Text) ? null : txtNote.Text.Trim();

                var toCreate = new List<HIS_HEMODIALYSIS_SCHEDULE>();
                foreach (var t in selecteds)
                {
                    toCreate.Add(new HIS_HEMODIALYSIS_SCHEDULE()
                    {
                        TREATMENT_ID = t.ID,
                        PATIENT_ID = t.PATIENT_ID,
                        ROOM_ID = roomId.Value,
                        SCHEDULE_DATE = scheduleDate.Value,
                        KIDNEY_SHIFT = (long)shift.Value,
                        EXP_MEST_TEMPLATE_ID = templateId,
                        NOTE = note,
                    });
                }

                WaitingManager.Show();
                var result = new BackendAdapter(param).Post<MOS.SDO.HisHemodialysisScheduleSaveResultSDO>(
                    HisRequestUriStore.HEMODIALYSIS_SCHEDULE_CREATE_LIST, ApiConsumers.MosConsumer, toCreate, param);
                WaitingManager.Hide();

                if (result != null)
                {
                    success = true;
                    LoadScheduleGrid();
                    XtraMessageBoxLike(string.Format(Resources.ResourceMessageLang.DuaVaoLichThanhCongFormat, result.AddedCount));
                }
                else
                {
                    MessageManager.Show(this.ParentForm, param, success);
                }

                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Click vào ô checkbox ở HEADER cột chọn → tích/bỏ tích toàn bộ bệnh nhân.
        /// </summary>
        private void gridViewTreatment_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button != MouseButtons.Left) return;
                GridHitInfo hitInfo = gridViewTreatment.CalcHitInfo(e.Location);
                if (hitInfo.InColumn && hitInfo.Column != null && hitInfo.Column.FieldName == "IsSelected")
                {
                    isHeaderSelectAll = !isHeaderSelectAll;
                    gridViewTreatment.BeginUpdate();
                    try
                    {
                        if (this.treatmentADOs != null)
                            foreach (var t in this.treatmentADOs) t.IsSelected = isHeaderSelectAll;
                    }
                    finally { gridViewTreatment.EndUpdate(); }
                    gridViewTreatment.InvalidateColumnHeader(hitInfo.Column);
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Vẽ checkbox "chọn tất cả" trong header cột chọn của lưới bệnh nhân.</summary>
        private void gridViewTreatment_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {
            try
            {
                if (e.Column == null || e.Column.FieldName != "IsSelected") return;

                e.Info.InnerElements.Clear();
                e.Painter.DrawObject(e.Info);

                RepositoryItemCheckEdit checkEdit = e.Column.ColumnEdit as RepositoryItemCheckEdit;
                if (checkEdit != null)
                {
                    int size = 16;
                    int x = e.Bounds.X + (e.Bounds.Width - size) / 2;
                    int y = e.Bounds.Y + (e.Bounds.Height - size) / 2;
                    Rectangle rect = new Rectangle(x, y, size, size);

                    var info = (DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo)checkEdit.CreateViewInfo();
                    var painter = (DevExpress.XtraEditors.Drawing.CheckEditPainter)checkEdit.CreatePainter();
                    info.EditValue = isHeaderSelectAll;
                    info.Bounds = rect;
                    info.CalcViewInfo(e.Graphics);
                    using (DevExpress.Utils.Drawing.GraphicsCache cache = new DevExpress.Utils.Drawing.GraphicsCache(e.Graphics))
                    {
                        painter.Draw(new DevExpress.XtraEditors.Drawing.ControlGraphicsInfoArgs(info, cache, rect));
                    }
                }
                e.Handled = true;
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
                if (!success) MessageManager.Show(this.ParentForm, param, success);
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
                MessageManager.Show(this.ParentForm, param, success);
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

                // Đọc lịch ngày nguồn + ngày đích để dựng bảng tóm tắt
                var sourceList = GetScheduleByDate(roomId, sourceDate.Value, param);
                if (sourceList == null || sourceList.Count == 0)
                {
                    XtraMessageBoxLike(Resources.ResourceMessageLang.KhongCoBanGhiDeSaoChep);
                    return;
                }
                var targetList = GetScheduleByDate(roomId, targetDate.Value, param);

                // Enrich tên bệnh nhân cho danh sách nguồn để hiển thị trong popup (BN đã có trong ca)
                EnrichScheduleNames(sourceList, param);

                using (var frm = new frmCopyScheduleConfirm(
                    cboRoom.Text, sourceDate.Value, targetDate.Value, sourceList, targetList))
                {
                    if (frm.ShowDialog(this) != DialogResult.OK) return;
                }

                // Xác nhận → gọi API CopySchedule thực hiện insert + skip trùng
                var sdo = new MOS.SDO.HisHemodialysisScheduleCopySDO()
                {
                    RoomId = roomId ?? this.currentRoomId,
                    SourceDate = sourceDate.Value,
                    TargetDate = targetDate.Value
                };
                WaitingManager.Show();
                var result = new BackendAdapter(param).Post<MOS.SDO.HisHemodialysisScheduleSaveResultSDO>(
                    HisRequestUriStore.HEMODIALYSIS_SCHEDULE_COPY, ApiConsumers.MosConsumer, sdo, param);
                WaitingManager.Hide();

                if (result != null)
                {
                    LoadScheduleGrid();
                    XtraMessageBoxLike(string.Format(Resources.ResourceMessageLang.SaoChepThanhCongFormat, result.AddedCount));
                }
                else
                {
                    MessageManager.Show(this.ParentForm, param, false);
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

        /// <summary>
        /// Bổ sung tên/mã BN cho danh sách slot lịch (dữ liệu từ /Get chỉ là entity, không có tên):
        /// gọi API HIS_TREATMENT/Get theo danh sách TREATMENT_ID — KHÔNG lấy từ cache RAM.
        /// </summary>
        private void EnrichScheduleNames(List<HemodialysisScheduleADO> list, CommonParam param)
        {
            try
            {
                if (list == null || list.Count == 0) return;
                var ids = list.Select(o => o.TREATMENT_ID).Where(id => id > 0).Distinct().ToList();
                if (ids.Count == 0) return;

                var treaFilter = new MOS.Filter.HisTreatmentFilter();
                treaFilter.IDs = ids;
                var treas = new BackendAdapter(param).Get<List<HIS_TREATMENT>>(
                    HisRequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, treaFilter, param)
                    ?? new List<HIS_TREATMENT>();

                var dict = new Dictionary<long, HIS_TREATMENT>();
                foreach (var t in treas)
                    if (!dict.ContainsKey(t.ID)) dict[t.ID] = t;

                foreach (var o in list)
                {
                    HIS_TREATMENT t;
                    if (dict.TryGetValue(o.TREATMENT_ID, out t) && t != null)
                    {
                        o.TDL_PATIENT_NAME = t.TDL_PATIENT_NAME;
                        o.TDL_PATIENT_CODE = t.TDL_PATIENT_CODE;
                        o.TREATMENT_CODE = t.TREATMENT_CODE;
                    }
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
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
                else if (e.Column.FieldName == "IN_TIME"
                    || e.Column.FieldName == "CREATE_TIME"
                    || e.Column.FieldName == "MODIFY_TIME")
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
                if (e.Column.FieldName == "IN_TIME")
                    e.DisplayText = FormatDateNumber(e.Value, true);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
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
        #endregion

        #region Message helpers
        private void XtraMessageBoxLike(string message)
        {
            DevExpress.XtraEditors.XtraMessageBox.Show(message, this.captionText, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private bool ConfirmYesNo(string message)
        {
            return DevExpress.XtraEditors.XtraMessageBox.Show(message, this.captionText, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
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
                bool enable = !chkAllDepartment.Checked;
                cboDepartment.Enabled = enable;
                txtDepartmentCode.Enabled = enable;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }
        #endregion

        #region Nhập mã → chọn phòng/khoa tương ứng
        /// <summary>
        /// Nút "X" (Delete) trên các GridLookUpEdit → xóa giá trị combo về null.
        /// Dùng chung cho cboRoom, cboShift, cboDepartment.
        /// </summary>
        private void cboClearButton_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind != DevExpress.XtraEditors.Controls.ButtonPredefines.Delete) return;
                var edit = sender as DevExpress.XtraEditors.GridLookUpEdit;
                if (edit != null) edit.EditValue = null;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Chọn phòng ở cboRoom → điền ngược mã phòng (EXECUTE_ROOM_CODE) vào ô txtRoomCode.
        /// </summary>
        private void cboRoom_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboRoom.EditValue == null)
                {
                    txtRoomCode.Text = "";
                    return;
                }
                long roomId = Inventec.Common.TypeConvert.Parse.ToInt64(cboRoom.EditValue.ToString());
                var room = BackendDataWorker.Get<V_HIS_EXECUTE_ROOM>().FirstOrDefault(o => o.ROOM_ID == roomId);
                txtRoomCode.Text = room != null ? room.EXECUTE_ROOM_CODE : "";
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Chọn khoa ở cboDepartment → điền ngược mã khoa (DEPARTMENT_CODE) vào ô txtDepartmentCode.
        /// </summary>
        private void cboDepartment_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboDepartment.EditValue == null)
                {
                    txtDepartmentCode.Text = "";
                    return;
                }
                long deptId = Inventec.Common.TypeConvert.Parse.ToInt64(cboDepartment.EditValue.ToString());
                var dept = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == deptId);
                txtDepartmentCode.Text = dept != null ? dept.DEPARTMENT_CODE : "";
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Enter trên ô mã phòng chạy → tìm phòng theo mã (EXECUTE_ROOM_CODE) và
        /// chọn vào cboRoom (cùng loại), sau đó tải lại lịch. Không thấy → mở popup combo.
        /// </summary>
        private void txtRoomCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode != Keys.Enter) return;

                string searchCode = (txtRoomCode.Text ?? "").Trim();
                if (string.IsNullOrEmpty(searchCode))
                {
                    cboRoom.Focus();
                    cboRoom.ShowPopup();
                    return;
                }

                var data = BackendDataWorker.Get<V_HIS_EXECUTE_ROOM>()
                    .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                        && !string.IsNullOrEmpty(o.EXECUTE_ROOM_CODE)
                        && o.EXECUTE_ROOM_CODE.ToLower().Contains(searchCode.ToLower()))
                    .ToList();
                var result = data.Count > 1
                    ? data.Where(o => o.EXECUTE_ROOM_CODE.ToLower() == searchCode.ToLower()).ToList()
                    : data;

                if (result != null && result.Count > 0)
                {
                    cboRoom.EditValue = result.First().ROOM_ID;
                    txtRoomCode.Text = result.First().EXECUTE_ROOM_CODE;
                    LoadScheduleGrid();
                }
                else
                {
                    cboRoom.Focus();
                    cboRoom.ShowPopup();
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Enter trên ô mã khoa → tìm khoa theo mã (DEPARTMENT_CODE) và chọn vào
        /// cboDepartment (cùng loại), sau đó tải lại danh sách bệnh nhân.
        /// </summary>
        private void txtDepartmentCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode != Keys.Enter) return;

                string searchCode = (txtDepartmentCode.Text ?? "").Trim();
                if (string.IsNullOrEmpty(searchCode))
                {
                    cboDepartment.Focus();
                    cboDepartment.ShowPopup();
                    return;
                }

                var data = BackendDataWorker.Get<HIS_DEPARTMENT>()
                    .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                        && !string.IsNullOrEmpty(o.DEPARTMENT_CODE)
                        && o.DEPARTMENT_CODE.ToLower().Contains(searchCode.ToLower()))
                    .ToList();
                var result = data.Count > 1
                    ? data.Where(o => o.DEPARTMENT_CODE.ToLower() == searchCode.ToLower()).ToList()
                    : data;

                if (result != null && result.Count > 0)
                {
                    if (chkAllDepartment.Checked) chkAllDepartment.Checked = false;
                    cboDepartment.EditValue = result.First().ID;
                    txtDepartmentCode.Text = result.First().DEPARTMENT_CODE;
                    LoadTreatmentGrid();
                }
                else
                {
                    cboDepartment.Focus();
                    cboDepartment.ShowPopup();
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }
        #endregion

        #region Điều hướng ngày / ca (nút lùi - tới)
        private void btnPrevDate_Click(object sender, EventArgs e)
        {
            ShiftScheduleDate(-1);
        }

        private void btnNextDate_Click(object sender, EventArgs e)
        {
            ShiftScheduleDate(1);
        }

        private void ShiftScheduleDate(int days)
        {
            try
            {
                DateTime baseDate = dtDate.EditValue != null ? dtDate.DateTime : DateTime.Now;
                dtDate.DateTime = baseDate.AddDays(days);
                LoadScheduleGrid();
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private void btnPrevShift_Click(object sender, EventArgs e)
        {
            ShiftKidneyShift(-1);
        }

        private void btnNextShift_Click(object sender, EventArgs e)
        {
            ShiftKidneyShift(1);
        }

        /// <summary>Đổi ca lọc thận trong khoảng hợp lệ [1..5] rồi tải lại lịch.</summary>
        private void ShiftKidneyShift(int step)
        {
            try
            {
                short current = GetSelectedShift() ?? (short)1;
                short next = (short)(current + step);
                if (next < 1) next = 1;
                if (next > 5) next = 5;
                if (next == current && cboShift.EditValue != null) return;
                cboShift.EditValue = next;
                LoadScheduleGrid();
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }
        #endregion
    }
}
