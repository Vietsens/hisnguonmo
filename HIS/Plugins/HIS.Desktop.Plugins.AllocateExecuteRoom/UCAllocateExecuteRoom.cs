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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using MOS.EFMODEL.DataModels;
using DevExpress.XtraGrid.Views.Base;
using System.Collections;
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using MOS.Filter;
using Inventec.Common.Adapter;
using HIS.Desktop.Utilities.Extensions;
using DevExpress.Utils.Menu;
using HIS.Desktop.Plugins.AllocateExecuteRoom.Base;
using HIS.Desktop.Plugins.AllocateExecuteRoom.ADO;

namespace HIS.Desktop.Plugins.AllocateExecuteRoom
{
    public partial class UCAllocateExecuteRoom : UserControlBase
    {
        #region Derlare
        string datetime;
        int rowCount = 0;
        int dataTotal = 0;
        int numPageSize;
        int lastRowHandle = -1;

         internal long roomId;
        internal long roomTypeId;

        Inventec.Desktop.Common.Modules.Module currentModule { get; set; }
        string ModuleLinkName = "HIS.Desktop.Plugins.AllocateExecuteRoom";
        long timeCount = 0;
        long maxTimeReload = 0;

        internal List<ServiceReq1ADO> serviceReqs1 { get; set; }
        internal ServiceReq1ADO currentHisServiceReq1 { get; set; }
        internal HIS_SERVICE_REQ currentServiceReqPrint { get; set; }
        HIS_SERVICE_REQ resultPrint { get; set; }
        HIS_EXP_MEST prescriptionPrint { get; set; }
        V_HIS_SERVICE_REQ serviceReqPrintRaw { get; set; }
        internal List<RoomADO> roomCounter { get; set; }
        internal RoomADO currentRoomCounter { get; set; }
        internal string typeCodeFind__KeyWork_InDate = "Ngày";
        internal string typeCodeFind_InDate = "Ngày";
        internal string typeCodeFind__InMonth = "Tháng";
        internal string typeCodeFind_RangeDate = "Khoảng ngày";
        bool isNotLoadWhileChangeControlStateInFirst;

        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;

        #endregion

        public UCAllocateExecuteRoom(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
            
            try
            {
                this.currentModule = module;
                this.roomId = module.RoomId;
                this.roomTypeId = module.RoomTypeId;
            }
            catch (Exception ex)
            {
                 Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public void Add()
        {
            btnFind_Click(null,null);
        }
        public void Save()
        {
            if(btnSave.Enabled)
                btnSave_Click(null, null);
        }

        private void UCAllocateExecuteRoom_Load(object sender, EventArgs e)
        {
            try
            {
                InitTypeFind();
                LoadActionButtonRefesh(true);
                LoadDefaultData();
                FillDataToGridControl();
                this.InitControlState();
            }
            catch (Exception ex)
            {
              Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                btnFind.Enabled = false;
                currentHisServiceReq1 = null;
                FillDataToGridControl();
                btnFind.Enabled = true;
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnCodeFind_Click(object sender, EventArgs e)
        {
            try
            {
                var btnMenuCodeFind = sender as DXMenuItem;
                btnCodeFind.Text = btnMenuCodeFind.Caption;
                this.typeCodeFind__KeyWork_InDate = btnMenuCodeFind.Caption;

                FormatDtIntructionDate();

                if (this.typeCodeFind__KeyWork_InDate == this.typeCodeFind_InDate || this.typeCodeFind__KeyWork_InDate == this.typeCodeFind__InMonth)
                {
                    lciIntructionDateTo.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
                else
                {
                    lciIntructionDateTo.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }

            }
            catch (Exception ex)
            {
                       Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnPreviewIntructionDate_Click(object sender, EventArgs e)
        {
            try
            {
                if (dtIntructionDate.EditValue != null && dtIntructionDate.DateTime != DateTime.MinValue && !String.IsNullOrWhiteSpace(btnCodeFind.Text))
                {
                    var currentdate = dtIntructionDate.DateTime;
                    if (this.typeCodeFind__KeyWork_InDate == this.typeCodeFind_InDate)
                        dtIntructionDate.EditValue = currentdate.AddDays(-1);
                    else
                        dtIntructionDate.EditValue = currentdate.AddMonths(-1);

                    btnFind_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnNextIntructionDate_Click(object sender, EventArgs e)
        {
            try
            {
                if (dtIntructionDate.EditValue != null && dtIntructionDate.DateTime != DateTime.MinValue && !String.IsNullOrWhiteSpace(btnCodeFind.Text))
                {
                    var currentdate = dtIntructionDate.DateTime;
                    if (this.typeCodeFind__KeyWork_InDate == this.typeCodeFind_InDate)
                        dtIntructionDate.EditValue = currentdate.AddDays(1);
                    else
                        dtIntructionDate.EditValue = currentdate.AddMonths(1);

                    btnFind_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                txtSearchKey.Focus();
                txtSearchKey.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                txtServiceReqCode.Focus();
                txtServiceReqCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                txtSearchKeyRoom.Focus();
                txtSearchKeyRoom.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnFind_Click(null,null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
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
                ServiceReq1ADO serADO= new ServiceReq1ADO();
                var serviceReqAlls = this.gridView1.DataSource as List<ServiceReq1ADO>;
                if (serviceReqAlls != null)
                {
                    foreach (var item in serviceReqAlls)
                    {
                        if (item.isCheckRow)
                            serADO = item;
                    }
                }

                RoomADO roADO = new RoomADO();
               var roomAlls = this.gridView2.DataSource as List<RoomADO>;
               if (roomAlls != null)
               {
                   foreach (var item in roomAlls)
                   {
                       if (item.IsChecked)
                           roADO = item;
                   }
               }
               if (!serADO.isCheckRow ||!roADO.IsChecked )
               {
                   DevExpress.XtraEditors.XtraMessageBox.Show("Bạn bắt buộc phải y lệnh và phòng cần chuyển trước khi \"Lưu\"", "Thông báo");
                   return;
               }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serADO), serADO));
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => roADO), roADO));
               WaitingManager.Show();
               bool success = false;
               CommonParam param = new CommonParam();
               HIS_SERVICE_REQ dataSend = new HIS_SERVICE_REQ();
               dataSend.ID = serADO.ID;
               dataSend.PRIORITY = serADO.PRIORITY;
               dataSend.INTRUCTION_TIME = serADO.INTRUCTION_TIME;
               dataSend.EXECUTE_ROOM_ID = roADO.ROOM_ID;
               dataSend.REQUEST_ROOM_ID = this.roomId;

               resultPrint = new BackendAdapter(param).Post<HIS_SERVICE_REQ>(
                         "api/HisServiceReq/ChangeRoom",
                         HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                         dataSend,
                         param);
               Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => resultPrint), resultPrint));
               if (resultPrint != null)
               {
                   success = true;
                   btnPrint.Enabled = true;
                   Print();
                   txtServiceReqCode.Focus();
                   txtServiceReqCode.SelectAll();
                   gridControl1.DataSource = null;
                   gridControl1.BeginUpdate();
                   serviceReqs1.RemoveAll(o => o == serADO);
                   Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceReqs1), serviceReqs1));
                   gridControl1.DataSource = serviceReqs1;
                   gridControl1.EndUpdate();
                   gridControl2.DataSource = null;
                   

               }

               WaitingManager.Hide();
               #region Show message
               MessageManager.Show(this.ParentForm, param,success);
               #endregion

               #region Process has exception
               SessionManager.ProcessTokenLost(param);
               #endregion

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                  Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Print()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisServiceReqViewFilter filter = new HisServiceReqViewFilter();
                filter.ID = resultPrint.ID;
                var data = new BackendAdapter(param).Get<List<V_HIS_SERVICE_REQ>>(
                  "api/HisServiceReq/GetView",
                  HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                  filter,
                  param);
                if (data != null && data.Count > 0)
                {
                    serviceReqPrintRaw = data.FirstOrDefault();
                }

                if (chkInPhieuChiDinh.Checked)
                   {
                       
                       InPhieuChiDinh(resultPrint);
                   }
                   if (chkInSTT.Checked)
                   {
                       InSTT(serviceReqPrintRaw);
                   }
            }
            catch (Exception ex)
            {    WaitingManager.Hide();
                      Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtServiceReqCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnFind_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSearchKey_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnFind_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void DoubleClickGridServiceReq()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("gridViewServiceReq_RowCellClick. 6");
                gridView1.OptionsSelection.EnableAppearanceFocusedCell = true;
                gridView1.OptionsSelection.EnableAppearanceFocusedRow = true;
                currentHisServiceReq1 = (ServiceReq1ADO)gridView1.GetFocusedRow();
                LoadDataToPanelRight(currentHisServiceReq1);
                Inventec.Common.Logging.LogSystem.Debug("gridViewServiceReq_RowCellClick. 7");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkCungKhoa_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkCungKhoa.Name && o.MODULE_LINK == ModuleLinkName).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkCungKhoa.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkCungKhoa.Name;
                    csAddOrUpdate.VALUE = (chkCungKhoa.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = ModuleLinkName;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkCungKhuVuc_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkCungKhuVuc.Name && o.MODULE_LINK == ModuleLinkName).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkCungKhuVuc.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkCungKhuVuc.Name;
                    csAddOrUpdate.VALUE = (chkCungKhuVuc.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = ModuleLinkName;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkInPhieuChiDinh_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkInPhieuChiDinh.Name && o.MODULE_LINK == ModuleLinkName).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkInPhieuChiDinh.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkInPhieuChiDinh.Name;
                    csAddOrUpdate.VALUE = (chkInPhieuChiDinh.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = ModuleLinkName;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkInSTT_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkInSTT.Name && o.MODULE_LINK == ModuleLinkName).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkInSTT.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkInSTT.Name;
                    csAddOrUpdate.VALUE = (chkInSTT.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = ModuleLinkName;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSearchKeyRoom_TextChanged(object sender, EventArgs e)
        {
            try
            {
                LoadDataToPanelRight(currentHisServiceReq1);
            }
            catch (Exception ex)
            {
                      Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView2_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                bool isChangeColor = false;
                string colToiDa = "";
                string colToiDaBH = "";
                string colTong = "";
                if (this.gridView2.GetRowCellValue(e.RowHandle, "MAX_REQUEST_BY_DAY") != null)
                {
                    colToiDa = this.gridView2.GetRowCellValue(e.RowHandle, "MAX_REQUEST_BY_DAY").ToString(); 
                }
                if (this.gridView2.GetRowCellValue(e.RowHandle, "MAX_REQ_BHYT_BY_DAY") != null)
                {
                    colToiDaBH = this.gridView2.GetRowCellValue(e.RowHandle, "MAX_REQ_BHYT_BY_DAY").ToString();
                }

                if (this.gridView2.GetRowCellValue(e.RowHandle, "TOTAL_TODAY_SERVICE_REQ_Str") != null)
                {
                    colTong = this.gridView2.GetRowCellValue(e.RowHandle, "TOTAL_TODAY_SERVICE_REQ_Str").ToString();
                }
                
                if(!string.IsNullOrEmpty(colTong) && !string.IsNullOrEmpty(colToiDa) && !string.IsNullOrEmpty(colToiDaBH))
                {
                    if(Int64.Parse(colToiDa)<=Int64.Parse(colTong) || Int64.Parse(colToiDaBH)<=Int64.Parse(colTong))
                    {
                        isChangeColor = true;
                    }
                }
                if (!string.IsNullOrEmpty(colTong) && !string.IsNullOrEmpty(colToiDa))
                {
                    if (Int64.Parse(colToiDa) <= Int64.Parse(colTong))
                    {
                        isChangeColor = true;
                    }
                }
                if (!string.IsNullOrEmpty(colTong) && !string.IsNullOrEmpty(colToiDaBH))
                {
                    if (Int64.Parse(colToiDaBH) <= Int64.Parse(colTong))
                    {
                        isChangeColor = true;
                    }
                }
                if (isChangeColor)
                {
                    e.Appearance.ForeColor = Color.Red;
                    e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                }
                else
                    e.Appearance.ForeColor = Color.Black;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemCheckEdit1_Click(object sender, EventArgs e)
        {
            try
            {
                 DoubleClickGridServiceReq();                  
            }
            catch (Exception ex)
            {
                    Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView2_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Space)
                {                  
                    var room = (RoomADO)this.gridView2.GetFocusedRow();
                    if (room != null)
                    {
                        this.gridView2.BeginUpdate();
                        var roomAlls = this.gridView2.DataSource as List<RoomADO>;
                        if (roomAlls != null)
                        {
                            foreach (var ro in roomAlls)
                            {
                                ro.IsChecked = false;
                                if (ro.ID == room.ID)
                                {
                                    ro.IsChecked = !ro.IsChecked;
                                }
                            }
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => roomAlls), roomAlls));
                            this.gridView2.GridControl.DataSource = roomAlls;
                        }
                        this.gridView2.EndUpdate();
                    }
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    btnSave_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView2_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                gridControl2.BeginUpdate();
                var userRoomADO = (RoomADO)this.gridView2.GetFocusedRow();
                if (userRoomADO != null)
                {
                    var roomAlls = this.gridView2.DataSource as List<RoomADO>;
                    if (roomAlls != null)
                    {
                        foreach (var ro in roomAlls)
                        {
                            ro.IsChecked = false;
                            if (ro.ID == userRoomADO.ID)
                            {
                                ro.IsChecked = !ro.IsChecked;
                            }
                        }

                        this.gridView2.GridControl.DataSource = roomAlls;
                    }
                    this.gridControl2.RefreshDataSource();
                    if (userRoomADO.IsChecked)
                    {
                        btnSave.Focus();
                    }
                }
                gridControl2.EndUpdate();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void timerAutoReload_Tick(object sender, EventArgs e)
        {
            try
            {
                TimeRemainDisplay();
                if (timeCount == maxTimeReload)
                {
                    timerAutoReload.Stop();
                    btnFind_Click(null, null);
                    return;
                }
                timeCount = timeCount + 1000;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void TimeRemainDisplay()
        {
            try
            {
                long timeRemain = maxTimeReload / 1000 - ((timeCount) / 1000);
                if (timeRemain > 0)
                {
                    lblAutoReload.Text = String.Format("{0}", timeRemain);
                    lblAutoReload.ToolTip = String.Format("Tự động tải lại sau {0} giây", timeRemain);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void btnRefesh_Click(object sender, EventArgs e)
        {
            try
            {
                switch ((EnumUtil.REFESH_ENUM)btnRefesh.Tag)
                {
                    case EnumUtil.REFESH_ENUM.ON:
                        btnRefesh.Image = imageList1.Images[0];
                        btnRefesh.Tag = EnumUtil.REFESH_ENUM.OFF;
                        btnFind_Click(null, null);
                        break;
                    default:
                        btnRefesh.Image = imageList1.Images[1];
                        btnRefesh.Tag = EnumUtil.REFESH_ENUM.ON;
                        timerAutoReload.Stop();
                        timeCount = 0;
                        lblAutoReload.Text = "";
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnPrint.Enabled) return;
                InitMenuPrint();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Space)
                {
                    if (this.gridView1.IsEditing)
                        this.gridView1.CloseEditor();

                    if (this.gridView1.FocusedRowModified)
                        this.gridView1.UpdateCurrentRow();

                    var ss = (ServiceReq1ADO)this.gridView1.GetFocusedRow();
                    if (ss != null)
                    {
                        this.gridView1.BeginUpdate();
                        var serAlls = this.gridView1.DataSource as List<ServiceReq1ADO>;
                        if (serAlls != null)
                        {
                            foreach (var ro in serAlls)
                            {
                                ro.isCheckRow = false;
                                if (ro.ID == ss.ID)
                                {
                                    ro.isCheckRow = !ro.isCheckRow;
                                }
                            }
                            this.gridView1.GridControl.DataSource = serAlls;
                        }
                        this.gridView1.EndUpdate();
                    }

                }
                else if (e.KeyCode == Keys.Enter)
                {
                    DoubleClickGridServiceReq();
                }
            }
            catch (Exception ex)
            {
                  Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemCheckEdit3_Click(object sender, EventArgs e)
        {
            try
            {
                    var userRoomADO = (RoomADO)this.gridView2.GetFocusedRow();
                   var roomAlls = this.gridView2.DataSource as List<RoomADO>;
                    gridControl2.BeginUpdate();
                    if (roomAlls != null)
                    {
                        foreach (var ro in roomAlls)
                        {
                            ro.IsChecked = false;
                            if (ro.ID == userRoomADO.ID)
                            {
                                ro.IsChecked = !ro.IsChecked;
                            }
                        }

                        this.gridControl2.DataSource = roomAlls;
                    }
                    this.gridControl2.EndUpdate();
                    btnSave.Focus();
                    
            }
            catch (Exception ex)
            {
                        Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                gridControl1.BeginUpdate();
                var serviceReqADO = (ServiceReq1ADO)this.gridView1.GetFocusedRow();
                if (serviceReqADO != null)
                {
                    serviceReqADO.isCheckRow = !serviceReqADO.isCheckRow;
                    this.gridControl1.RefreshDataSource();
                }
                gridControl1.EndUpdate();
                DoubleClickGridServiceReq();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        

    }
}
