using ACS.EFMODEL.DataModels;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.KskServiceEditList.ADO;
using HIS.Desktop.Plugins.KskServiceEditList.Worker;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.KskServiceEditList
{
    public partial class frmKskServiceEditList
    {
        private List<ACS_USER> listAcsUser;

        private void InitComboLogin()
        {
            try
            {
                this.listAcsUser = BackendDataWorker.Get<ACS_USER>()
                    .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 120, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 370);
                ControlEditorLoader.Load(this.cboLogin, this.listAcsUser, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultValue()
        {
            try
            {
                this.lblContract.Text = String.Format("{0} - {1}", this.kskContract.KSK_CONTRACT_CODE, this.kskContract.WORK_PLACE_NAME);
                this.lblPatientCount.Text = String.Format(Resources.ResourceMessage.ApDungChoBenhNhan, this.treatments.Count);
                this.cboLogin.EditValue = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                this.dtIntructionTime.DateTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Lấy dịch vụ của các hồ sơ đã chọn, gộp theo SERVICE_ID rồi bind grid "Dịch vụ hiện có"
        /// </summary>
        private void LoadExistServices()
        {
            try
            {
                WaitingManager.Show();
                List<HIS_SERE_SERV> sereServs = new KskServiceEditWorker().GetSereServs(this.treatments.Select(o => o.ID).ToList());
                this.BuildRoomLookup();
                this.existServices = this.BuildExistServices(sereServs);
                WaitingManager.Hide();

                this.gridViewExist.BeginUpdate();
                try
                {
                    this.gridControlExist.DataSource = null;
                    this.gridControlExist.DataSource = this.existServices;
                }
                finally
                {
                    this.gridViewExist.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BuildRoomLookup()
        {
            this.roomsByService = BackendDataWorker.Get<V_HIS_SERVICE_ROOM>()
                .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                .ToLookup(o => o.SERVICE_ID, o => new RoomADO { ID = o.ROOM_ID, ROOM_CODE = o.ROOM_CODE, ROOM_NAME = o.ROOM_NAME });
        }

        /// <summary>
        /// Gộp dịch vụ theo SERVICE_ID — tính sẵn chuỗi hiển thị trước khi bind
        /// </summary>
        private List<ExistServiceADO> BuildExistServices(List<HIS_SERE_SERV> sereServs)
        {
            List<ExistServiceADO> result = new List<ExistServiceADO>();
            if (sereServs == null || !sereServs.Any()) return result;

            Dictionary<long, HIS_SERVICE_TYPE> serviceTypeDic = BackendDataWorker.Get<HIS_SERVICE_TYPE>().ToDictionary(o => o.ID);
            Dictionary<long, V_HIS_ROOM> roomDic = BackendDataWorker.Get<V_HIS_ROOM>().ToDictionary(o => o.ID);
            int total = this.treatments.Count;

            foreach (IGrouping<long, HIS_SERE_SERV> g in sereServs.GroupBy(o => o.SERVICE_ID))
            {
                HIS_SERE_SERV first = g.First();
                ExistServiceADO ado = new ExistServiceADO();
                ado.SERVICE_ID = g.Key;
                ado.SERVICE_CODE = first.TDL_SERVICE_CODE;
                ado.SERVICE_NAME = first.TDL_SERVICE_NAME;
                HIS_SERVICE_TYPE serviceType;
                ado.SERVICE_TYPE_NAME = serviceTypeDic.TryGetValue(first.TDL_SERVICE_TYPE_ID, out serviceType) ? serviceType.SERVICE_TYPE_NAME : "";
                ado.PatientCount = g.Select(o => o.TDL_TREATMENT_ID).Distinct().Count();
                ado.PatientCountDisplay = ado.PatientCount + "/" + total;
                ado.ExecutedCount = g.Where(o => o.EXECUTE_TIME.HasValue).Select(o => o.TDL_TREATMENT_ID).Distinct().Count();

                List<string> roomNames = g.Select(o => o.TDL_EXECUTE_ROOM_ID).Distinct()
                    .Select(id => { V_HIS_ROOM r; return roomDic.TryGetValue(id, out r) ? r.ROOM_NAME : id.ToString(); })
                    .ToList();
                ado.CurrentRoomDisplay = roomNames.Count == 1 ? roomNames[0] : Resources.ResourceMessage.NhieuPhong;
                ado.CurrentRoomTooltip = String.Join(", ", roomNames);
                result.Add(ado);
            }
            return result.OrderBy(o => o.SERVICE_TYPE_NAME).ThenBy(o => o.SERVICE_CODE).ToList();
        }

        /// <summary>
        /// Combo "Phòng mới" riêng cho từng dịch vụ (chỉ các phòng thực hiện được dịch vụ), tạo 1 lần
        /// </summary>
        private RepositoryItemGridLookUpEdit GetRepositoryNewRoom(long serviceId)
        {
            RepositoryItemGridLookUpEdit repository;
            if (this.repositoryNewRoomDic.TryGetValue(serviceId, out repository)) return repository;

            repository = new RepositoryItemGridLookUpEdit();
            repository.NullText = "";
            repository.Buttons.Add(new EditorButton(ButtonPredefines.Delete));
            repository.ButtonClick += this.repositoryNewRoom_ButtonClick;
            this.gridControlExist.RepositoryItems.Add(repository);

            List<ColumnInfo> columnInfos = new List<ColumnInfo>();
            columnInfos.Add(new ColumnInfo("ROOM_CODE", "", 80, 1));
            columnInfos.Add(new ColumnInfo("ROOM_NAME", "", 250, 2));
            ControlEditorADO controlEditorADO = new ControlEditorADO("ROOM_NAME", "ID", columnInfos, false, 330);
            List<RoomADO> rooms = this.roomsByService != null ? this.roomsByService[serviceId].ToList() : new List<RoomADO>();
            ControlEditorLoader.Load(repository, rooms, controlEditorADO);

            this.repositoryNewRoomDic[serviceId] = repository;
            return repository;
        }

        private void repositoryNewRoom_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    this.gridViewExist.SetFocusedRowCellValue(this.gcExistNewRoom, null);
                    this.gridViewExist.HideEditor();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewExist_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.Column != this.gcExistNewRoom) return;
                ExistServiceADO row = this.gridViewExist.GetRow(e.RowHandle) as ExistServiceADO;
                if (row == null) return;
                e.RepositoryItem = row.IsDelete ? (RepositoryItem)this.repositoryItemTxtDisable : this.GetRepositoryNewRoom(row.SERVICE_ID);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewExist_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column == this.gcExistStt)
                {
                    e.Value = e.ListSourceRowIndex + 1;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemChkDelete_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                this.gridViewExist.PostEditor();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// "Xóa" và "Phòng mới" loại trừ nhau
        /// </summary>
        private void gridViewExist_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                ExistServiceADO row = this.gridViewExist.GetRow(e.RowHandle) as ExistServiceADO;
                if (row == null) return;
                if (e.Column == this.gcExistIsDelete && row.IsDelete)
                {
                    row.NewRoomId = null;
                }
                else if (e.Column == this.gcExistNewRoom && row.NewRoomId.HasValue)
                {
                    row.IsDelete = false;
                }
                this.gridViewExist.RefreshRow(e.RowHandle);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
