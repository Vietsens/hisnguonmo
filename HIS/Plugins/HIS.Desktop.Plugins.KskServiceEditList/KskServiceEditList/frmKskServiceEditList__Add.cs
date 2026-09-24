using DevExpress.XtraEditors;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.KskServiceEditList.ADO;
using Inventec.Common.Controls.EditorLoader;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.KskServiceEditList
{
    public partial class frmKskServiceEditList
    {
        private List<HIS_KSK> listKsk;

        private void InitComboKsk()
        {
            try
            {
                this.listKsk = BackendDataWorker.Get<HIS_KSK>()
                    .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    .OrderBy(o => o.KSK_CODE).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("KSK_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("KSK_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("KSK_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(this.cboKsk, this.listKsk, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboKsk_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                this.currentKskServices = new List<KskServiceADO>();
                if (this.cboKsk.EditValue != null)
                {
                    long kskId = Inventec.Common.TypeConvert.Parse.ToInt64(this.cboKsk.EditValue.ToString());
                    Dictionary<long, V_HIS_SERVICE> serviceDic = BackendDataWorker.Get<V_HIS_SERVICE>().ToDictionary(o => o.ID);
                    foreach (HIS_KSK_SERVICE ks in BackendDataWorker.Get<HIS_KSK_SERVICE>()
                        .Where(o => o.KSK_ID == kskId && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE))
                    {
                        V_HIS_SERVICE service;
                        if (!serviceDic.TryGetValue(ks.SERVICE_ID, out service)) continue;
                        KskServiceADO ado = new KskServiceADO();
                        ado.ID = ks.SERVICE_ID;
                        ado.KSK_ID = ks.KSK_ID;
                        ado.SERVICE_CODE = service.SERVICE_CODE;
                        ado.SERVICE_NAME = service.SERVICE_NAME;
                        ado.ROOM_ID = ks.ROOM_ID;
                        ado.AMOUNT = ks.AMOUNT;
                        ado.PRICE = ks.PRICE.HasValue ? ks.PRICE * (1 + (ks.VAT_RATIO ?? 0)) : null;
                        this.currentKskServices.Add(ado);
                    }
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SERVICE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("SERVICE_NAME", "", 300, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_NAME", "ID", columnInfos, false, 400);
                ControlEditorLoader.Load(this.cboKskService, this.currentKskServices.OrderBy(o => o.SERVICE_CODE).ToList(), controlEditorADO);
                this.cboKskService.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboKskService_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                KskServiceADO kskService = this.GetSelectedKskService();
                List<RoomADO> rooms = kskService != null && this.roomsByService != null
                    ? this.roomsByService[kskService.ID].ToList() : new List<RoomADO>();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("ROOM_CODE", "", 80, 1));
                columnInfos.Add(new ColumnInfo("ROOM_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("ROOM_NAME", "ID", columnInfos, false, 330);
                ControlEditorLoader.Load(this.cboAddRoom, rooms, controlEditorADO);

                // Mặc định phòng theo nhóm dịch vụ KSK (giống Import)
                this.cboAddRoom.EditValue = kskService != null && rooms.Any(o => o.ID == kskService.ROOM_ID) ? (object)kskService.ROOM_ID : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private KskServiceADO GetSelectedKskService()
        {
            if (this.cboKskService.EditValue == null) return null;
            long serviceId = Inventec.Common.TypeConvert.Parse.ToInt64(this.cboKskService.EditValue.ToString());
            return this.currentKskServices.FirstOrDefault(o => o.ID == serviceId);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                KskServiceADO kskService = this.GetSelectedKskService();
                if (kskService == null)
                {
                    XtraMessageBox.Show(Resources.ResourceMessage.ChuaChonDichVu, this.GetTitleMessage());
                    this.cboKskService.Focus();
                    return;
                }
                if (this.cboAddRoom.EditValue == null)
                {
                    XtraMessageBox.Show(Resources.ResourceMessage.ChuaChonPhong, this.GetTitleMessage());
                    this.cboAddRoom.Focus();
                    return;
                }
                if (this.addServices.Any(o => o.SERVICE_ID == kskService.ID))
                {
                    XtraMessageBox.Show(String.Format(Resources.ResourceMessage.DichVuDaCoTrongDanhSachThem, kskService.SERVICE_NAME), this.GetTitleMessage());
                    return;
                }

                long roomId = Inventec.Common.TypeConvert.Parse.ToInt64(this.cboAddRoom.EditValue.ToString());
                HIS_KSK ksk = this.listKsk.FirstOrDefault(o => o.ID == kskService.KSK_ID);
                AddServiceADO ado = new AddServiceADO();
                ado.KSK_ID = kskService.KSK_ID;
                ado.KSK_NAME = ksk != null ? ksk.KSK_NAME : "";
                ado.SERVICE_ID = kskService.ID;
                ado.SERVICE_CODE = kskService.SERVICE_CODE;
                ado.SERVICE_NAME = kskService.SERVICE_NAME;
                ado.ROOM_ID = roomId;
                ado.ROOM_NAME = this.cboAddRoom.Text;
                ado.AMOUNT = kskService.AMOUNT;
                ado.PRICE = kskService.PRICE;
                this.addServices.Add(ado);

                this.RefreshGridAdd();
                this.cboKskService.EditValue = null;
                this.cboKskService.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemBtnDeleteAdd_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                AddServiceADO row = this.gridViewAdd.GetFocusedRow() as AddServiceADO;
                if (row == null) return;
                this.addServices.Remove(row);
                this.RefreshGridAdd();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void RefreshGridAdd()
        {
            this.gridViewAdd.BeginUpdate();
            try
            {
                this.gridControlAdd.DataSource = null;
                this.gridControlAdd.DataSource = this.addServices;
            }
            finally
            {
                this.gridViewAdd.EndUpdate();
            }
        }

        private void gridViewAdd_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column == this.gcAddStt)
                {
                    e.Value = e.ListSourceRowIndex + 1;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string GetTitleMessage()
        {
            return HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao);
        }
    }
}
