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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.DashboardTreatmentBedRoom.ADO;
using Inventec.Desktop.Common.LanguageManager;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.DashboardTreatmentBedRoom
{
    public partial class frmTreatmentBedRoom : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        Inventec.Desktop.Common.Modules.Module currentModule;

        /// <summary>Phòng đầu vào — lấy từ module đang mở.</summary>
        long roomId;

        /// <summary>Khoa suy ra từ phòng đầu vào.</summary>
        long departmentId;

        /// <summary>Danh sách phòng của khoa đang bind lên grid.</summary>
        List<RoomCheckADO> roomAdos = new List<RoomCheckADO>();

        /// <summary>Trạng thái checkbox "chọn tất cả" vẽ ở header cột Chọn.</summary>
        bool isCheckedAll = false;

        /// <summary>Màn hình mở rộng đang mở, null khi chưa mở.</summary>
        frmDashboard dashboard;

        /// <summary>
        /// Cờ chặn đệ quy: gán lại toggleSwitch1.IsOn trong chính handler Toggled sẽ bắn lại Toggled.
        /// </summary>
        bool isSyncingToggle = false;
        #endregion

        #region Constructor
        public frmTreatmentBedRoom(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            try
            {
                InitializeComponent();
                this.currentModule = module;
                this.roomId = (module != null ? module.RoomId : 0);

                // InitializeComponent gán lại Text từ designer nên phải khôi phục tên module sau đó
                if (module != null && !string.IsNullOrEmpty(module.text))
                {
                    this.Text = module.text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Load
        private void frmTreatmentBedRoom_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                LoadDataToGridRoom();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.DashboardTreatmentBedRoom.Resources.Lang", typeof(frmTreatmentBedRoom).Assembly);

                // Tên form ưu tiên tên module cấu hình, chỉ lấy theo ngôn ngữ khi module không có tên
                if (this.currentModule == null || string.IsNullOrEmpty(this.currentModule.text))
                {
                    this.Text = GetLang("frmTreatmentBedRoom.Text", this.Text);
                }

                this.lciDepartment.Text = GetLang("frmTreatmentBedRoom.lciDepartment.Text", this.lciDepartment.Text);
                this.txtDepartment.Properties.NullValuePrompt = GetLang("frmTreatmentBedRoom.txtDepartment.NullValuePrompt", this.txtDepartment.Properties.NullValuePrompt);
                this.gcCheck.Caption = GetLang("frmTreatmentBedRoom.gcCheck.Caption", this.gcCheck.Caption);
                this.gcCheck.ToolTip = GetLang("frmTreatmentBedRoom.gcCheck.ToolTip", this.gcCheck.ToolTip);
                this.gcRoomCode.Caption = GetLang("frmTreatmentBedRoom.gcRoomCode.Caption", this.gcRoomCode.Caption);
                this.gcRoomName.Caption = GetLang("frmTreatmentBedRoom.gcRoomName.Caption", this.gcRoomName.Caption);
                this.lciColumnCount.Text = GetLang("frmTreatmentBedRoom.lciColumnCount.Text", this.lciColumnCount.Text);
                this.toggleSwitch1.Properties.OffText = GetLang("frmTreatmentBedRoom.toggleSwitch1.OffText", this.toggleSwitch1.Properties.OffText);
                this.toggleSwitch1.Properties.OnText = GetLang("frmTreatmentBedRoom.toggleSwitch1.OnText", this.toggleSwitch1.Properties.OnText);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string GetLang(string key, string defaultValue)
        {
            try
            {
                string value = Inventec.Common.Resource.Get.Value(key, Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                return string.IsNullOrEmpty(value) ? defaultValue : value;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return defaultValue;
        }
        #endregion

        #region Load data
        /// <summary>
        /// Từ ID phòng đầu vào lấy ra khoa, rồi lấy toàn bộ phòng đang hoạt động của khoa đó lên grid.
        /// </summary>
        private void LoadDataToGridRoom()
        {
            try
            {
                this.departmentId = 0;
                this.roomAdos = new List<RoomCheckADO>();
                this.txtDepartment.Text = "";

                List<V_HIS_ROOM> allRooms = BackendDataWorker.Get<V_HIS_ROOM>();

                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                var userRoomByUserIds = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_USER_ROOM>().Where(o => o.LOGINNAME == loginName 
                && (o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)).Select(o => o.ROOM_ID).ToList();

                V_HIS_ROOM currentRoom = allRooms.FirstOrDefault(o => o.ID == this.roomId);
                if (currentRoom == null)
                {
                    Inventec.Common.Logging.LogSystem.Warn("Khong tim thay phong lam viec trong cache theo ROOM_ID: " + this.roomId);
                    BindDataToGridRoom();
                    return;
                }

                this.departmentId = currentRoom.DEPARTMENT_ID;
                this.txtDepartment.Text = string.Format("{0} - {1}", currentRoom.DEPARTMENT_CODE, currentRoom.DEPARTMENT_NAME);

                List<V_HIS_ROOM> rooms = (userRoomByUserIds != null && userRoomByUserIds.Count > 0) ? BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__BUONG
                            && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                            && userRoomByUserIds.Contains(o.ID)
                            && o.DEPARTMENT_ID == currentRoom.DEPARTMENT_ID).ToList() : null;

                // Người dùng chưa được gán phòng nào thì rooms là null — để nguyên grid rỗng, đừng ném lỗi
                if (rooms == null)
                {
                    Inventec.Common.Logging.LogSystem.Warn("Nguoi dung khong duoc gan buong nao trong khoa. DEPARTMENT_ID: " + this.departmentId);
                    BindDataToGridRoom();
                    return;
                }

                // Mặc định tích sẵn toàn bộ phòng của khoa, người dùng bỏ bớt phòng không muốn xem
                this.roomAdos = rooms.Select(o => new RoomCheckADO()
                {
                    ROOM_ID = o.ID,
                    ROOM_CODE = o.ROOM_CODE,
                    ROOM_NAME = o.ROOM_NAME,
                    IsCheck = true
                }).ToList();

                BindDataToGridRoom();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BindDataToGridRoom()
        {
            try
            {
                this.gridViewRoom.BeginUpdate();
                try
                {
                    this.gridControlRoom.DataSource = this.roomAdos;
                }
                finally
                {
                    this.gridViewRoom.EndUpdate();
                }
                RecomputeHeaderCheckState();
                this.gridControlRoom.Invalidate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region CheckAll
        /// <summary>
        /// Vẽ checkbox "chọn tất cả" ở header cột Chọn.
        /// </summary>
        private void gridViewRoom_CustomDrawColumnHeader(object sender, DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventArgs e)
        {
            try
            {
                if (e.Column != null && e.Column == this.gcCheck)
                {
                    e.Info.Caption = string.Empty;
                    e.Info.InnerElements.Clear();
                    e.Painter.DrawObject(e.Info);
                    DrawHeaderCheckBox(e.Cache, GetHeaderCheckBoxBounds(e.Bounds), this.isCheckedAll);
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private System.Drawing.Rectangle GetHeaderCheckBoxBounds(System.Drawing.Rectangle headerBounds)
        {
            int size = 16;
            int x = headerBounds.X + (headerBounds.Width - size) / 2;
            int y = headerBounds.Y + (headerBounds.Height - size) / 2;
            return new System.Drawing.Rectangle(x, y, size, size);
        }

        private void DrawHeaderCheckBox(DevExpress.Utils.Drawing.GraphicsCache cache, System.Drawing.Rectangle bounds, bool isChecked)
        {
            try
            {
                DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo info = this.repositoryItemCheckEdit1.CreateViewInfo() as DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo;
                DevExpress.XtraEditors.Drawing.CheckEditPainter painter = this.repositoryItemCheckEdit1.CreatePainter() as DevExpress.XtraEditors.Drawing.CheckEditPainter;
                if (info == null || painter == null)
                {
                    return;
                }
                info.EditValue = isChecked;
                info.Bounds = bounds;
                info.CalcViewInfo(null);
                DevExpress.XtraEditors.Drawing.ControlGraphicsInfoArgs args = new DevExpress.XtraEditors.Drawing.ControlGraphicsInfoArgs(info, cache, bounds);
                painter.Draw(args);
                args.Cache = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Click vào header cột Chọn → đảo trạng thái tích chọn của toàn bộ dòng.
        /// </summary>
        private void gridControlRoom_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button != MouseButtons.Left)
                {
                    return;
                }
                DevExpress.XtraGrid.Views.Grid.GridView view = this.gridControlRoom.MainView as DevExpress.XtraGrid.Views.Grid.GridView;
                if (view == null)
                {
                    return;
                }
                DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo hit = view.CalcHitInfo(new System.Drawing.Point(e.X, e.Y));
                if (hit.InColumn && hit.Column == this.gcCheck)
                {
                    this.isCheckedAll = !this.isCheckedAll;
                    SetCheckAll(this.isCheckedAll);
                    this.gridControlRoom.Invalidate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Đồng bộ trạng thái header khi người dùng tích/bỏ tích từng dòng.
        /// </summary>
        private void gridViewRoom_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                if (e.Column == this.gcCheck)
                {
                    RecomputeHeaderCheckState();
                    this.gridControlRoom.Invalidate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetCheckAll(bool isChecked)
        {
            try
            {
                this.gridViewRoom.PostEditor();
                if (this.roomAdos == null || this.roomAdos.Count == 0)
                {
                    return;
                }
                foreach (RoomCheckADO item in this.roomAdos)
                {
                    item.IsCheck = isChecked;
                }
                this.gridViewRoom.RefreshData();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void RecomputeHeaderCheckState()
        {
            try
            {
                this.isCheckedAll = this.roomAdos != null && this.roomAdos.Count > 0 && this.roomAdos.All(o => o.IsCheck);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Extended screen
        /// <summary>
        /// Gạt bật → mở màn hình mở rộng với đúng các phòng đang tích và chu kỳ làm mới đang đặt.
        /// Gạt tắt → đóng màn hình mở rộng.
        /// </summary>
        private void toggleSwitch1_Toggled(object sender, EventArgs e)
        {
            try
            {
                if (this.isSyncingToggle)
                {
                    return;
                }

                if (this.toggleSwitch1.IsOn)
                {
                    OpenDashboard();
                }
                else
                {
                    CloseDashboard();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void OpenDashboard()
        {
            try
            {
                if (this.dashboard != null && !this.dashboard.IsDisposed)
                {
                    this.dashboard.Activate();
                    return;
                }

                List<long> roomIds = GetCheckedRoomIds();
                if (roomIds == null || roomIds.Count == 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(
                        this,
                        GetLang("frmTreatmentBedRoom.MsgNoRoomChecked", "Chua chon phong nao."),
                        this.Text,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    SetToggleWithoutEvent(false);
                    return;
                }

                this.dashboard = new frmDashboard(this.departmentId, roomIds, GetReloadSecond(), GetColumnCount());
                this.dashboard.FormClosed += Dashboard_FormClosed;
                this.dashboard.Show(this);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                SetToggleWithoutEvent(false);
            }
        }

        private void CloseDashboard()
        {
            try
            {
                if (this.dashboard == null)
                {
                    return;
                }

                frmDashboard closing = this.dashboard;
                this.dashboard = null;
                closing.FormClosed -= Dashboard_FormClosed;
                if (!closing.IsDisposed)
                {
                    closing.Close();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Người dùng bấm ESC bên màn hình mở rộng → gạt công tắc về tắt cho khớp trạng thái.
        /// </summary>
        private void Dashboard_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                this.dashboard = null;
                SetToggleWithoutEvent(false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetToggleWithoutEvent(bool isOn)
        {
            try
            {
                this.isSyncingToggle = true;
                this.toggleSwitch1.IsOn = isOn;
            }
            finally
            {
                this.isSyncingToggle = false;
            }
        }

        /// <summary>
        /// Số cột đang đặt trên spinColumnCount. Bỏ trống hoặc đọc lỗi thì trả 0 để
        /// frmDashboard tự rơi về mặc định — chỗ chốt giá trị nằm bên đó, không phải ở đây.
        /// </summary>
        private int GetColumnCount()
        {
            try
            {
                if (this.spinColumnCount.EditValue == null)
                {
                    return 0;
                }
                return Convert.ToInt32(this.spinColumnCount.Value);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return 0;
        }

        /// <summary>
        /// Chu kỳ làm mới đang đặt trên spinReloadTime, đơn vị giây. 0 = không tự làm mới.
        /// </summary>
        private int GetReloadSecond()
        {
            try
            {
                if (this.spinReloadTime.EditValue == null)
                {
                    return 0;
                }

                int second = Convert.ToInt32(this.spinReloadTime.Value);
                return second < 0 ? 0 : second;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return 0;
        }
        #endregion

        #region Public
        /// <summary>
        /// Khoa suy ra từ phòng đầu vào (0 nếu không tìm được phòng trong cache).
        /// </summary>
        public long GetDepartmentId()
        {
            return this.departmentId;
        }

        /// <summary>
        /// Danh sách ID phòng người dùng đang tích chọn trên grid.
        /// </summary>
        public List<long> GetCheckedRoomIds()
        {
            try
            {
                this.gridViewRoom.PostEditor();
                this.gridViewRoom.UpdateCurrentRow();
                if (this.roomAdos != null)
                {
                    return this.roomAdos.Where(o => o.IsCheck).Select(o => o.ROOM_ID).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return new List<long>();
        }
        #endregion

        #region Dispose data
        public override void ProcessDisposeModuleDataAfterClose()
        {
            try
            {
                // Dong man hinh thiet lap thi phai dong luon man hinh mo rong dang treo theo no,
                // khong thi cua so khong vien do se o lai tren desktop ma khong con duong dong
                CloseDashboard();
                this.roomAdos = null;
                this.currentModule = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion
    }
}
