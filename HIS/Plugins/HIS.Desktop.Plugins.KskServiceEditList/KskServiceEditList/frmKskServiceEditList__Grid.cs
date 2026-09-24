using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using HIS.Desktop.Plugins.KskServiceEditList.ADO;
using Inventec.Common.Controls.EditorLoader;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace HIS.Desktop.Plugins.KskServiceEditList
{
    public partial class frmKskServiceEditList
    {
        /// <summary>
        /// Đổ lưới theo "Phòng thực hiện" và công tắc "Tất cả dịch vụ / Dịch vụ đã chọn" (giống FillDataToGrid của AssignServiceEdit).
        /// Dịch vụ đang có / đang tick luôn hiển thị để không bị xóa ngoài ý muốn.
        /// </summary>
        private void FillDataToGrid()
        {
            try
            {
                long? roomId = this.GetSelectedRoomId();
                IEnumerable<ServiceRowADO> rows = this.allServiceRows;
                if (this.toggleSwitch.IsOn)
                {
                    rows = rows.Where(o => o.IsChecked != false);
                }
                else if (roomId.HasValue)
                {
                    rows = rows.Where(o => o.IsExisting || o.IsChecked == true || this.IsRoomOfService(o.SERVICE_ID, roomId.Value));
                }

                this.GridViewService.BeginUpdate();
                try
                {
                    this.GridControlService.DataSource = null;
                    this.GridControlService.DataSource = rows.ToList();
                }
                finally
                {
                    this.GridViewService.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private long? GetSelectedRoomId()
        {
            if (this.cboRoom.EditValue == null) return null;
            return Inventec.Common.TypeConvert.Parse.ToInt64(this.cboRoom.EditValue.ToString());
        }

        private bool IsRoomOfService(long serviceId, long roomId)
        {
            return this.roomsByService != null && this.roomsByService[serviceId].Any(o => o.ID == roomId);
        }

        private void cboRoom_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                this.FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboRoom_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    this.cboRoom.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void toggleSwitch_Toggled(object sender, EventArgs e)
        {
            try
            {
                this.GridViewService.PostEditor();
                this.FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Combo "Phòng thực hiện" riêng cho từng dịch vụ (chỉ các phòng thực hiện được dịch vụ), tạo 1 lần
        /// </summary>
        private RepositoryItemGridLookUpEdit GetRepositoryRoom(long serviceId)
        {
            RepositoryItemGridLookUpEdit repository;
            if (this.repositoryRoomDic.TryGetValue(serviceId, out repository)) return repository;

            repository = new RepositoryItemGridLookUpEdit();
            repository.NullText = "";
            this.GridControlService.RepositoryItems.Add(repository);

            List<ColumnInfo> columnInfos = new List<ColumnInfo>();
            columnInfos.Add(new ColumnInfo("ROOM_CODE", "", 80, 1));
            columnInfos.Add(new ColumnInfo("ROOM_NAME", "", 250, 2));
            ControlEditorADO controlEditorADO = new ControlEditorADO("ROOM_NAME", "ID", columnInfos, false, 330);
            List<RoomADO> rooms = this.roomsByService != null ? this.roomsByService[serviceId].ToList() : new List<RoomADO>();
            ControlEditorLoader.Load(repository, rooms, controlEditorADO);

            this.repositoryRoomDic[serviceId] = repository;
            return repository;
        }

        private void GridViewService_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                ServiceRowADO row = this.GridViewService.GetRow(e.RowHandle) as ServiceRowADO;
                if (row == null) return;
                if (e.Column == this.gcCheck)
                {
                    // Chỉ dịch vụ một phần BN có mới có trạng thái lưng chừng
                    e.RepositoryItem = row.IsPartial ? this.repositoryItemChkPartial : this.repositoryItemChk;
                }
                else if (e.Column == this.gcRoom)
                {
                    e.RepositoryItem = row.IsChecked == true ? (RepositoryItem)this.GetRepositoryRoom(row.SERVICE_ID) : this.repositoryItemTxtDisable;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GridViewService_ShowingEditor(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                ServiceRowADO row = this.GridViewService.GetFocusedRow() as ServiceRowADO;
                if (row != null && this.GridViewService.FocusedColumn == this.gcRoom && row.IsChecked != true)
                {
                    e.Cancel = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemChk_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                this.GridViewService.PostEditor();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GridViewService_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                ServiceRowADO row = this.GridViewService.GetRow(e.RowHandle) as ServiceRowADO;
                if (row == null) return;
                if (e.Column == this.gcCheck)
                {
                    // Tick thêm dịch vụ chưa có: mặc định phòng đang chọn ở "Phòng thực hiện" nếu thực hiện được
                    long? roomId = this.GetSelectedRoomId();
                    if (row.IsChecked == true && !row.IsExisting && roomId.HasValue && this.IsRoomOfService(row.SERVICE_ID, roomId.Value))
                    {
                        row.RoomId = roomId;
                    }
                }
                else if (e.Column == this.gcRoom)
                {
                    row.IsRoomChanged = row.IsExisting && row.RoomId.HasValue && row.RoomId != row.OriginalRoomId;
                }
                this.GridViewService.RefreshRow(e.RowHandle);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Dịch vụ đang ở nhiều phòng: hiển thị danh sách phòng thay vì để trống
        /// </summary>
        private void GridViewService_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            try
            {
                if (e.Column != this.gcRoom || e.ListSourceRowIndex < 0) return;
                ServiceRowADO row = this.GridViewService.GetRow(this.GridViewService.GetRowHandle(e.ListSourceRowIndex)) as ServiceRowADO;
                if (row == null) return;
                string roomName;
                if (row.RoomId.HasValue && this.roomNameDic != null && this.roomNameDic.TryGetValue(row.RoomId.Value, out roomName))
                {
                    e.DisplayText = roomName;
                }
                else if (!row.RoomId.HasValue && !String.IsNullOrEmpty(row.CurrentRoomDisplay))
                {
                    e.DisplayText = row.CurrentRoomDisplay;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Tô màu dòng thay đổi: bỏ tick (xóa) đỏ, tick thêm xanh dương, đổi phòng cam
        /// </summary>
        private void GridViewService_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                ServiceRowADO row = this.GridViewService.GetRow(e.RowHandle) as ServiceRowADO;
                if (row == null) return;
                if (row.IsExisting && row.IsChecked == false)
                    e.Appearance.ForeColor = Color.Red;
                else if (row.IsChecked == true && (!row.IsExisting || row.IsPartial))
                    e.Appearance.ForeColor = Color.Blue;
                else if (row.IsRoomChanged && e.Column == this.gcRoom)
                    e.Appearance.ForeColor = Color.DarkOrange;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
