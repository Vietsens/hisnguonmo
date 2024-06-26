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
using MOS.EFMODEL.DataModels;
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using MOS.Filter;
using DevExpress.XtraGrid.Columns;
using HIS.Desktop.Plugins.RoomService.Entity;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.ADO;
using AutoMapper;
using ACS.Filter;
using HIS.Desktop.LocalStorage.BackendData;
using ACS.EFMODEL.DataModels;
using Inventec.Common.Adapter;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.UC.Service;
using HIS.UC.Service.ADO;
using HIS.UC.Room;
using HIS.UC.Room.ADO;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;
using HIS.Desktop.Common;
using DevExpress.XtraBars;
using MOS.SDO;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.RoomService.Resources;
using System.IO;

namespace HIS.Desktop.Plugins.RoomService
{
    public partial class UCRoomService : HIS.Desktop.Utility.UserControlBase
    {
        List<HIS_SERVICE_TYPE> ServiceType { get; set; }

        internal Inventec.Desktop.Common.Modules.Module currentModule;
        UCRoomProcessor RoomProcessor;
        UCServiceProcessor ServiceProcessor;
        UserControl ucGridControlService;
        UserControl ucGridControlRoom;
        int rowCount = 0;
        int dataTotal = 0;
        int rowCount1 = 0;
        int dataTotal1 = 0;
        internal List<HIS.UC.Room.RoomAccountADO> lstRoomADOs { get; set; }
        internal List<HIS.UC.Service.ServiceADO> lstRoomServiceADOs { get; set; }
        List<V_HIS_ROOM> listRoom;
        List<V_HIS_SERVICE> listService;
        long ServiceIdCheckByService = 0;
        long isChoseService;
        long isChoseRoom;
        long RoomIdCheckByRoom;
        bool isCheckAll;
        List<V_HIS_SERVICE_ROOM> ServiceRooms { get; set; }
        List<V_HIS_SERVICE_ROOM> ServiceRoomViews { get; set; }
        V_HIS_SERVICE currentService;
        V_HIS_ROOM Room;
        HIS.UC.Service.ServiceADO currentCopyServiceAdo;
        HIS.UC.Room.RoomAccountADO CurrentRoomCopyAdo;

        public UCRoomService(Inventec.Desktop.Common.Modules.Module currentModule)
            : base(currentModule)
        {
            InitializeComponent();
        }

        public UCRoomService(Inventec.Desktop.Common.Modules.Module currentModule, long ServiceType)
            : base(currentModule)
        {
            InitializeComponent();
            try
            {
                this.currentModule = currentModule;
                if (this.currentModule != null)
                {
                    this.Text = currentModule.text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public UCRoomService(V_HIS_SERVICE serviceData, Inventec.Desktop.Common.Modules.Module _moduleData)
            : base(_moduleData)
        {
            InitializeComponent();
            try
            {
                this.currentService = serviceData;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public UCRoomService(V_HIS_ROOM executeRoom, Inventec.Desktop.Common.Modules.Module _moduleData)
            : base(_moduleData)
        {
            InitializeComponent();
            try
            {
                this.Room = executeRoom;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UCRoomService_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                WaitingManager.Show();
                LoadDataToCombo();
                LoadComboStatus();
                LoadComboActive();
                InitUcgrid1();
                InitUcgrid2();
                if (this.currentService == null && this.Room == null)
                {
                    FillDataToGrid1(this);
                    FillDataToGrid2(this);
                }
                else if (this.Room == null)
                {
                    cboChoose.EditValue = (long)1;
                    cboChoose.Enabled = false;
                    FillDataToGrid1_Default(this);
                    FillDataToGrid2(this);
                    btn_Radio_Enable_Click1(this.currentService);
                }
                else if (this.currentService == null)
                {
                    cboChoose.EditValue = (long)2;
                    cboChoose.Enabled = false;
                    FillDataToGrid1(this);
                    FillDataToGrid2_Default(this);
                    btn_Radio_Enable_Click(this.Room);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewService_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);

                    if (hi.HitTest == GridHitTest.Column && isChoseService != 1)
                    {
                        if (hi.Column.FieldName == "checkService")
                        {
                            ProcessButtonCheckAllService(hi);
                        }
                        if (hi.Column.FieldName == "ABOUT_REQUEST")
                        {
                            ProcessButtonCheckAllRequest(hi);
                        }
                        if (hi.Column.FieldName == "ABOUT_EXECUTE")
                        {
                            ProcessButtonCheckAllExecute(hi);
                        }
                    }
                    else if (hi.HitTest == GridHitTest.RowCell && isChoseService != 1)
                    {
                        if (hi.Column.FieldName == "checkService")
                        {
                            ProcessButtonCheckService(hi);
                        }
                        if (hi.Column.FieldName == "ABOUT_REQUEST")
                        {
                            ProcessButtonCheckRequest(hi);
                        }
                        if (hi.Column.FieldName == "ABOUT_EXECUTE")
                        {
                            ProcessButtonCheckExecute(hi);
                        }
                    }
                }

                this.txtKeyword1.Focus();
                this.txtKeyword1.SelectAll();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessButtonCheckExecute(GridHitInfo hi)
        {
            try
            {
                HIS.UC.Service.ServiceADO dataRow = (HIS.UC.Service.ServiceADO)hi.View.GetRow(hi.RowHandle);
                if (dataRow.ABOUT_EXECUTE)
                {
                    hi.View.SetRowCellValue(hi.RowHandle, "checkService", dataRow.ABOUT_REQUEST);
                    dataRow.ABOUT_EXECUTE = false;
                }
                else
                {
                    hi.View.SetRowCellValue(hi.RowHandle, "checkService", true);
                    dataRow.ABOUT_EXECUTE = true;
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
            }
        }

        private void ProcessButtonCheckRequest(GridHitInfo hi)
        {
            try
            {
                HIS.UC.Service.ServiceADO dataRow = (HIS.UC.Service.ServiceADO)hi.View.GetRow(hi.RowHandle);
                if (dataRow.ABOUT_REQUEST)
                {
                    hi.View.SetRowCellValue(hi.RowHandle, "checkService", dataRow.ABOUT_EXECUTE);
                    dataRow.ABOUT_REQUEST = false;
                }
                else
                {
                    hi.View.SetRowCellValue(hi.RowHandle, "checkService", true);
                    dataRow.ABOUT_REQUEST = true;
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
            }
        }

        private void ProcessButtonCheckService(GridHitInfo hi)
        {
            try
            {
                HIS.UC.Service.ServiceADO dataRow = (HIS.UC.Service.ServiceADO)hi.View.GetRow(hi.RowHandle);
                if (dataRow.checkService)
                {
                    hi.View.SetRowCellValue(hi.RowHandle, "ABOUT_REQUEST", false);
                    hi.View.SetRowCellValue(hi.RowHandle, "ABOUT_EXECUTE", false);
                    dataRow.checkService = false;
                }
                else
                {
                    hi.View.SetRowCellValue(hi.RowHandle, "ABOUT_EXECUTE", true);
                    dataRow.checkService = true;
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessButtonCheckAllService(GridHitInfo hi)
        {
            try
            {
                List<HIS.UC.Service.ServiceADO> lstCheckAll = (List<HIS.UC.Service.ServiceADO>)ServiceProcessor.GetDataGridView(ucGridControlService);
                List<HIS.UC.Service.ServiceADO> lstChecks = new List<HIS.UC.Service.ServiceADO>();

                if (lstCheckAll != null && lstCheckAll.Count > 0)
                {
                    var ServiceCheckedNum = lstCheckAll.Where(o => o.checkService == true).Count();
                    var ServiceNum = lstCheckAll.Count();
                    if ((ServiceCheckedNum > 0 && ServiceCheckedNum < ServiceNum) || ServiceCheckedNum == 0)
                    {
                        isCheckAll = true;
                        hi.Column.Image = imageCollectionService.Images[1];
                    }

                    if (ServiceCheckedNum == ServiceNum)
                    {
                        isCheckAll = false;
                        hi.Column.Image = imageCollectionService.Images[0];
                    }

                    if (isCheckAll)
                    {
                        foreach (var item in lstCheckAll)
                        {
                            if (item.ID != null)
                            {
                                item.checkService = true;
                                item.ABOUT_EXECUTE = true;
                                lstChecks.Add(item);
                            }
                            else
                            {
                                lstChecks.Add(item);
                            }
                        }

                        isCheckAll = false;
                    }
                    else
                    {
                        foreach (var item in lstCheckAll)
                        {
                            if (item.ID != null)
                            {
                                item.checkService = false;
                                item.ABOUT_REQUEST = false;
                                item.ABOUT_EXECUTE = false;
                                lstChecks.Add(item);
                            }
                            else
                            {
                                lstChecks.Add(item);
                            }
                        }

                        isCheckAll = true;
                    }

                    ServiceProcessor.Reload(ucGridControlService, lstChecks);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessButtonCheckAllRequest(GridHitInfo hi)
        {
            try
            {
                List<HIS.UC.Service.ServiceADO> lstCheckAll = (List<HIS.UC.Service.ServiceADO>)ServiceProcessor.GetDataGridView(ucGridControlService);
                List<HIS.UC.Service.ServiceADO> lstChecks = new List<HIS.UC.Service.ServiceADO>();

                if (lstCheckAll != null && lstCheckAll.Count > 0)
                {
                    var ServiceCheckedNum = lstCheckAll.Where(o => o.ABOUT_REQUEST == true).Count();
                    var ServiceNum = lstCheckAll.Count();
                    if ((ServiceCheckedNum > 0 && ServiceCheckedNum < ServiceNum) || ServiceCheckedNum == 0)
                    {
                        isCheckAll = true;
                    }

                    if (ServiceCheckedNum == ServiceNum)
                    {
                        isCheckAll = false;
                    }

                    if (isCheckAll)
                    {
                        foreach (var item in lstCheckAll)
                        {
                            if (item.ID != null)
                            {
                                item.ABOUT_REQUEST = true;
                                item.checkService = true;
                                lstChecks.Add(item);
                            }
                            else
                            {
                                lstChecks.Add(item);
                            }
                        }

                        isCheckAll = false;
                    }
                    else
                    {
                        foreach (var item in lstCheckAll)
                        {
                            if (item.ID != null)
                            {
                                if (item.ABOUT_EXECUTE == true)
                                {
                                    item.ABOUT_REQUEST = false;
                                }
                                else
                                {
                                    item.ABOUT_REQUEST = false;
                                    item.checkService = false;
                                }

                                lstChecks.Add(item);
                            }
                            else
                            {
                                lstChecks.Add(item);
                            }
                        }

                        isCheckAll = true;
                    }

                    ServiceProcessor.Reload(ucGridControlService, lstChecks);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessButtonCheckAllExecute(GridHitInfo hi)
        {
            try
            {
                List<HIS.UC.Service.ServiceADO> lstCheckAll = (List<HIS.UC.Service.ServiceADO>)ServiceProcessor.GetDataGridView(ucGridControlService);
                List<HIS.UC.Service.ServiceADO> lstChecks = new List<HIS.UC.Service.ServiceADO>();

                if (lstCheckAll != null && lstCheckAll.Count > 0)
                {
                    var ServiceCheckedNum = lstCheckAll.Where(o => o.ABOUT_EXECUTE == true).Count();
                    var ServiceNum = lstCheckAll.Count();
                    if ((ServiceCheckedNum > 0 && ServiceCheckedNum < ServiceNum) || ServiceCheckedNum == 0)
                    {
                        isCheckAll = true;
                    }

                    if (ServiceCheckedNum == ServiceNum)
                    {
                        isCheckAll = false;
                    }

                    if (isCheckAll)
                    {
                        foreach (var item in lstCheckAll)
                        {
                            if (item.ID != null)
                            {
                                item.ABOUT_EXECUTE = true;
                                item.checkService = true;
                                lstChecks.Add(item);
                            }
                            else
                            {
                                item.ABOUT_EXECUTE = false;
                                lstChecks.Add(item);
                            }
                        }

                        isCheckAll = false;
                    }
                    else
                    {
                        foreach (var item in lstCheckAll)
                        {
                            if (item.ID != null)
                            {
                                if (item.ABOUT_REQUEST == true)
                                {
                                    item.ABOUT_EXECUTE = false;
                                }
                                else
                                {
                                    item.ABOUT_EXECUTE = false;
                                    item.checkService = false;
                                }
                                lstChecks.Add(item);
                            }
                            else
                            {
                                lstChecks.Add(item);
                            }
                        }

                        isCheckAll = true;
                    }

                    ServiceProcessor.Reload(ucGridControlService, lstChecks);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.RoomService.Resources.Lang", typeof(HIS.Desktop.Plugins.RoomService.UCRoomService).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCRoomService.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboChoose.Properties.NullText = Inventec.Common.Resource.Get.Value("UCRoomService.cboChoose.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UCRoomService.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnFind2.Text = Inventec.Common.Resource.Get.Value("UCRoomService.btnFind2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("UCRoomService.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnFind1.Text = Inventec.Common.Resource.Get.Value("UCRoomService.btnFind1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboServiceType.Properties.NullText = Inventec.Common.Resource.Get.Value("UCRoomService.cboServiceType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword2.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCRoomService.txtKeyword2.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword1.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCRoomService.txtKeyword1.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("UCRoomService.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("UCRoomService.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar2.Text = Inventec.Common.Resource.Get.Value("UCRoomService.bar2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitUcgrid1()
        {
            try
            {
                ServiceProcessor = new UCServiceProcessor();
                ServiceInitADO ado = new ServiceInitADO();
                ado.ListServiceColumn = new List<UC.Service.ServiceColumn>();
                ado.gridViewService_MouseDownMest = gridViewService_MouseDown;
                ado.btn_Radio_Enable_Click1 = btn_Radio_Enable_Click1;
                ado.gridView_MouseRightClick = ServiceGridView_MouseRightClick;

                ServiceColumn colRadio2 = new ServiceColumn("   ", "radioService", 30, true);
                colRadio2.VisibleIndex = 0;
                colRadio2.Visible = false;
                colRadio2.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListServiceColumn.Add(colRadio2);

                ServiceColumn colCheck2 = new ServiceColumn("   ", "checkService", 30, false);
                colCheck2.VisibleIndex = 1;
                colCheck2.image = imageCollectionService.Images[0];
                colCheck2.Visible = false;
                colCheck2.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListServiceColumn.Add(colCheck2);
                if (this.Room != null)
                {
                    ServiceColumn colChkExecute = new ServiceColumn("Xử lý", "ABOUT_EXECUTE", 30, false);
                    colChkExecute.VisibleIndex = 2;
                    //colChkExecute.image = imageCollectionService.Images[0];
                    colChkExecute.Visible = false;
                    colChkExecute.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                    ado.ListServiceColumn.Add(colChkExecute);

                    ServiceColumn colChkRequest = new ServiceColumn("Yêu cầu", "ABOUT_REQUEST", 30, false);
                    colChkRequest.VisibleIndex = 3;
                    //colChkRequest.image = imageCollectionService.Images[0];
                    colChkRequest.Visible = false;
                    colChkRequest.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                    ado.ListServiceColumn.Add(colChkRequest);
                }

                ServiceColumn colMaDichvu = new ServiceColumn("Mã dịch vụ", "SERVICE_CODE", 60, false);
                colMaDichvu.VisibleIndex = 4;
                ado.ListServiceColumn.Add(colMaDichvu);

                ServiceColumn colTenDichvu = new ServiceColumn("Tên dịch vụ", "SERVICE_NAME", 300, false);
                colTenDichvu.VisibleIndex = 5;
                ado.ListServiceColumn.Add(colTenDichvu);

                ServiceColumn colMaLoaidichvu = new ServiceColumn("Loại dịch vụ", "SERVICE_TYPE_NAME", 80, false);
                colMaLoaidichvu.VisibleIndex = 6;
                ado.ListServiceColumn.Add(colMaLoaidichvu);

                this.ucGridControlService = (UserControl)ServiceProcessor.Run(ado);
                if (ucGridControlService != null)
                {
                    this.panelControl1.Controls.Add(this.ucGridControlService);
                    this.ucGridControlService.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewRoom_MouseRoom(object sender, MouseEventArgs e)
        {
            try
            {
                if (isChoseRoom == 2)
                {
                    return;
                }

                WaitingManager.Show();
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);

                    if (hi.HitTest == GridHitTest.Column)
                    {
                        if (hi.Column.FieldName == "check1")
                        {
                            var lstCheckAll = lstRoomADOs;
                            List<HIS.UC.Room.RoomAccountADO> lstChecks = new List<HIS.UC.Room.RoomAccountADO>();

                            if (lstCheckAll != null && lstCheckAll.Count > 0)
                            {
                                var RoomCheckedNum = lstRoomADOs.Where(o => o.check1 == true).Count();
                                var RoomtmNum = lstRoomADOs.Count();
                                if ((RoomCheckedNum > 0 && RoomCheckedNum < RoomtmNum) || RoomCheckedNum == 0)
                                {
                                    isCheckAll = true;
                                    hi.Column.Image = imageCollectionRoom.Images[1];
                                }

                                if (RoomCheckedNum == RoomtmNum)
                                {
                                    isCheckAll = false;
                                    hi.Column.Image = imageCollectionRoom.Images[0];
                                }

                                if (isCheckAll)
                                {
                                    foreach (var item in lstCheckAll)
                                    {
                                        if (item.ID != null)
                                        {
                                            item.check1 = true;
                                            lstChecks.Add(item);
                                        }
                                        else
                                        {
                                            lstChecks.Add(item);
                                        }
                                    }

                                    isCheckAll = false;
                                }
                                else
                                {
                                    foreach (var item in lstCheckAll)
                                    {
                                        if (item.ID != null)
                                        {
                                            item.check1 = false;
                                            lstChecks.Add(item);
                                        }
                                        else
                                        {
                                            lstChecks.Add(item);
                                        }
                                    }

                                    isCheckAll = true;
                                }

                                RoomProcessor.Reload(ucGridControlRoom, lstChecks);
                            }
                        }
                    }

                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btn_Radio_Enable_Click1(V_HIS_SERVICE data)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                MOS.Filter.HisServiceRoomViewFilter filter = new HisServiceRoomViewFilter();
                filter.SERVICE_ID = data.ID;
                ServiceIdCheckByService = data.ID;

                ServiceRooms = new BackendAdapter(param).Get<List<V_HIS_SERVICE_ROOM>>(
                                    "api/HisServiceRoom/GetView",
                                HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                filter,
                                param);
                List<HIS.UC.Room.RoomAccountADO> dataNew = new List<HIS.UC.Room.RoomAccountADO>();
                dataNew = (from r in listRoom select new RoomAccountADO(r, true)).ToList();
                if (ServiceRooms != null && ServiceRooms.Count > 0)
                {
                    foreach (var itemRoom in ServiceRooms)
                    {
                        var check = dataNew.FirstOrDefault(o => o.ID == itemRoom.ROOM_ID);
                        if (check != null)
                        {
                            check.check1 = true;
                            check.checkPriority = (itemRoom.IS_PRIORITY == 1);
                        }
                    }
                }

                dataNew = dataNew.OrderByDescending(p => p.check1).ToList();
                if (ucGridControlRoom != null)
                {
                    RoomProcessor.Reload(ucGridControlRoom, dataNew);
                }
                else
                {
                    FillDataToGrid2(this);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitUcgrid2()
        {
            try
            {
                RoomProcessor = new UCRoomProcessor();
                RoomInitADO ado = new RoomInitADO();
                ado.ListRoomColumn = new List<UC.Room.RoomColumn>();
                ado.gridViewRoom_MouseDownRoom = gridViewRoom_MouseRoom;
                ado.btn_Radio_Enable_Click = btn_Radio_Enable_Click;
                ado.rooom_MouseRightClick = RoomGridView_MouseRightClick;

                RoomColumn colRadio1 = new RoomColumn("   ", "radio1", 30, true);
                colRadio1.VisibleIndex = 0;
                colRadio1.Visible = false;
                colRadio1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListRoomColumn.Add(colRadio1);

                RoomColumn colCheck1 = new RoomColumn("   ", "check1", 30, true);
                colCheck1.VisibleIndex = 1;
                colCheck1.image = imageCollectionRoom.Images[0];
                colCheck1.Visible = false;
                colCheck1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListRoomColumn.Add(colCheck1);

                RoomColumn colMaPhong = new RoomColumn("Mã phòng", "ROOM_CODE", 60, false);
                colMaPhong.VisibleIndex = 2;
                ado.ListRoomColumn.Add(colMaPhong);

                RoomColumn colTenPhong = new RoomColumn("Tên phòng", "ROOM_NAME", 100, false);
                colTenPhong.VisibleIndex = 3;
                ado.ListRoomColumn.Add(colTenPhong);

                RoomColumn colPriority = new RoomColumn("Ưu tiên", "checkPriority", 40, true);
                colPriority.VisibleIndex = 4;
                ado.ListRoomColumn.Add(colPriority);

                RoomColumn colLoaiPhong = new RoomColumn("Loại phòng", "ROOM_TYPE_NAME", 100, false);
                colLoaiPhong.VisibleIndex = 5;
                ado.ListRoomColumn.Add(colLoaiPhong);

                RoomColumn colKhoa = new RoomColumn("Khoa", "DEPARTMENT_NAME", 100, false);
                colKhoa.VisibleIndex = 6;
                ado.ListRoomColumn.Add(colKhoa);

                this.ucGridControlRoom = (UserControl)RoomProcessor.Run(ado);
                if (ucGridControlRoom != null)
                {
                    this.panelControl2.Controls.Add(this.ucGridControlRoom);
                    this.ucGridControlRoom.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btn_Radio_Enable_Click(V_HIS_ROOM data)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                MOS.Filter.HisServiceRoomViewFilter filter = new HisServiceRoomViewFilter();
                filter.ROOM_ID = data.ID;
                RoomIdCheckByRoom = data.ID;
                ServiceRoomViews = new BackendAdapter(param).Get<List<V_HIS_SERVICE_ROOM>>(
                                         "api/HisServiceRoom/GetView",
                                HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                filter,
                                param);
                List<HIS.UC.Service.ServiceADO> dataNew = new List<HIS.UC.Service.ServiceADO>();
                dataNew = (from r in listService select new HIS.UC.Service.ServiceADO(r)).ToList();
                if (ServiceRoomViews != null && ServiceRoomViews.Count > 0)
                {
                    foreach (var itemService in ServiceRoomViews)
                    {
                        var check = dataNew.FirstOrDefault(o => o.ID == itemService.SERVICE_ID);
                        if (check != null)
                        {
                            check.checkService = true;
                            check.ABOUT_REQUEST = (itemService.IS_REQUEST == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);
                            check.ABOUT_EXECUTE = (itemService.IS_EXECUTE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);
                        }
                    }

                    dataNew = dataNew.OrderByDescending(p => p.checkService).ToList();

                    if (ucGridControlService != null)
                    {
                        ServiceProcessor.Reload(ucGridControlService, dataNew);
                    }
                }
                else
                {
                    FillDataToGrid1(this);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGrid2(UCRoomService uCRoomService)
        {
            try
            {
                RoomIdCheckByRoom = 0;
                int numPageSize;
                if (ucPaging2.pagingGrid != null)
                {
                    numPageSize = ucPaging2.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                FillDataToGridRoom(new CommonParam(0, numPageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount1;
                param.Count = dataTotal1;
                ucPaging2.Init(FillDataToGridRoom, param, numPageSize);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGrid2_Default(UCRoomService UCRoomService)
        {
            try
            {
                RoomIdCheckByRoom = 0;
                int numPageSize;
                if (ucPaging2.pagingGrid != null)
                {
                    numPageSize = ucPaging2.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                FillDataToGridRoom_Default(new CommonParam(0, numPageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount1;
                param.Count = dataTotal1;
                ucPaging2.Init(FillDataToGridRoom, param, numPageSize);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridRoom(object data)
        {
            try
            {
                WaitingManager.Show();
                listRoom = new List<V_HIS_ROOM>();
                int start1 = ((CommonParam)data).Start ?? 0;
                int limit1 = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start1, limit1);
                MOS.Filter.HisRoomViewFilter RoomFillter = new HisRoomViewFilter();
                RoomFillter.IS_ACTIVE = 1;
                RoomFillter.ORDER_FIELD = "MODIFY_TIME";
                RoomFillter.ORDER_DIRECTION = "DESC";
                RoomFillter.KEY_WORD = txtKeyword2.Text;
                if ((long)cboChoose.EditValue == 2)
                {
                    isChoseRoom = (long)cboChoose.EditValue;
                }

                var sar = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_ROOM>>(
                   HIS.Desktop.ApiConsumer.HisRequestUriStore.HIS_ROOM_GETVIEW,
                    HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                      RoomFillter,
                    param);

                lstRoomADOs = new List<RoomAccountADO>();
                if (sar != null && sar.Data.Count > 0)
                {
                    listRoom = sar.Data;
                    foreach (var item in listRoom)
                    {
                        RoomAccountADO roomaccountADO = new RoomAccountADO(item);
                        if (isChoseRoom == 2)
                        {
                            roomaccountADO.isKeyChoose = true;
                        }
                        else
                        {
                            roomaccountADO.IsEnablePriority = true;
                        }
                        lstRoomADOs.Add(roomaccountADO);
                    }
                }

                if (ServiceRooms != null && ServiceRooms.Count > 0)
                {
                    foreach (var itemUsername in ServiceRooms)
                    {
                        var check = lstRoomADOs.FirstOrDefault(o => o.ID == itemUsername.ROOM_ID);
                        if (check != null)
                        {
                            check.check1 = true;
                            check.checkPriority = (itemUsername.IS_PRIORITY == 1);
                        }
                    }
                }
                lstRoomADOs = lstRoomADOs.OrderByDescending(p => p.check1).Distinct().ToList();

                if (ucGridControlRoom != null)
                {
                    RoomProcessor.Reload(ucGridControlRoom, lstRoomADOs);
                }
                rowCount1 = (data == null ? 0 : lstRoomADOs.Count);
                dataTotal1 = (sar.Param == null ? 0 : sar.Param.Count ?? 0);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridRoom_Default(object data)
        {
            try
            {
                WaitingManager.Show();
                listRoom = new List<V_HIS_ROOM>();
                int start1 = ((CommonParam)data).Start ?? 0;
                int limit1 = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start1, limit1);
                MOS.Filter.HisRoomViewFilter RoomFillter = new HisRoomViewFilter();
                RoomFillter.IS_ACTIVE = 1;
                RoomFillter.ORDER_FIELD = "MODIFY_TIME";
                RoomFillter.ORDER_DIRECTION = "DESC";
                RoomFillter.KEY_WORD = txtKeyword2.Text;
                if ((long)cboChoose.EditValue == 2)
                {
                    isChoseRoom = (long)cboChoose.EditValue;
                }
                if (this.Room != null)
                {
                    RoomFillter.ID = this.Room.ID;
                }
                var sar = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_ROOM>>(
                   HIS.Desktop.ApiConsumer.HisRequestUriStore.HIS_ROOM_GETVIEW,
                    HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                      RoomFillter,
                    param);

                lstRoomADOs = new List<RoomAccountADO>();

                if (sar != null && sar.Data.Count > 0)
                {
                    listRoom = sar.Data;
                    foreach (var item in listRoom)
                    {
                        RoomAccountADO roomaccountADO = new RoomAccountADO(item);
                        if (isChoseRoom == 2)
                        {
                            roomaccountADO.isKeyChoose = true;
                            roomaccountADO.radio1 = true;
                        }
                        else
                        {
                            roomaccountADO.IsEnablePriority = true;
                        }
                        lstRoomADOs.Add(roomaccountADO);
                    }
                }

                if (ServiceRooms != null && ServiceRooms.Count > 0)
                {
                    foreach (var itemUsername in ServiceRooms)
                    {
                        var check = lstRoomADOs.FirstOrDefault(o => o.ID == itemUsername.ROOM_ID);
                        if (check != null)
                        {
                            check.check1 = true;
                        }
                    }
                }
                lstRoomADOs = lstRoomADOs.OrderByDescending(p => p.check1).Distinct().ToList();

                if (ucGridControlRoom != null)
                {
                    RoomProcessor.Reload(ucGridControlRoom, lstRoomADOs);
                }
                rowCount1 = (data == null ? 0 : lstRoomADOs.Count);
                dataTotal1 = (sar.Param == null ? 0 : sar.Param.Count ?? 0);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGrid1(UCRoomService UCRoomService)
        {
            try
            {
                ServiceIdCheckByService = 0;
                int numPageSize;
                if (ucPaging1.pagingGrid != null)
                {
                    numPageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                FillDataToGridService(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridService, param, numPageSize);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGrid1_Default(UCRoomService UCRoomService)
        {
            try
            {
                ServiceIdCheckByService = 0;
                int numPageSize;
                if (ucPaging1.pagingGrid != null)
                {
                    numPageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                FillDataToGridService_Default(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridService_Default, param, numPageSize);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridService(object data)
        {
            try
            {
                WaitingManager.Show();
                listService = new List<V_HIS_SERVICE>();
                int start = ((CommonParam)data).Start ?? 0;
                int limit = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start, limit);
                MOS.Filter.HisServiceViewFilter ServiceFillter = new HisServiceViewFilter();

                SetFilterServiceView(ref ServiceFillter);

                if ((long)cboChoose.EditValue == 1)
                {
                    isChoseService = (long)cboChoose.EditValue;
                }

                var rs = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE>>(
                                                     "api/HisService/GetView",
                     HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                     ServiceFillter,
                     param);

                lstRoomServiceADOs = new List<ServiceADO>();

                if (rs != null && rs.Data.Count > 0)
                {
                    listService = rs.Data;
                    foreach (var item in listService)
                    {
                        ServiceADO RoomServiceADO = new ServiceADO(item);
                        if (isChoseService == 1)
                        {
                            RoomServiceADO.isKeyChooseService = true;
                        }
                        lstRoomServiceADOs.Add(RoomServiceADO);
                    }
                }

                if (ServiceRoomViews != null && ServiceRoomViews.Count > 0)
                {
                    foreach (var itemUsername in ServiceRoomViews)
                    {
                        var check = lstRoomServiceADOs.FirstOrDefault(o => o.ID == itemUsername.SERVICE_ID);
                        if (check != null)
                        {
                            check.checkService = true;
                            check.ABOUT_REQUEST = (itemUsername.IS_REQUEST == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);
                            check.ABOUT_EXECUTE = (itemUsername.IS_EXECUTE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);
                        }
                    }
                }

                lstRoomServiceADOs = lstRoomServiceADOs.OrderByDescending(p => p.checkService).Distinct().ToList();
                if (ucGridControlService != null)
                {
                    ServiceProcessor.Reload(ucGridControlService, lstRoomServiceADOs);
                }
                rowCount = (data == null ? 0 : lstRoomServiceADOs.Count);
                dataTotal = (rs.Param == null ? 0 : rs.Param.Count ?? 0);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetFilterServiceView(ref HisServiceViewFilter ServiceFillter)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(txtServiceCode_Exact.Text))
                {
                    ServiceFillter.SERVICE_CODE__EXACT = txtServiceCode_Exact.Text;
                }
                else
                {
                    if (cboActive.EditValue != null)
                    {
                        if (Inventec.Common.TypeConvert.Parse.ToInt16((cboActive.EditValue ?? "0").ToString()) == 1)
                            ServiceFillter.IS_ACTIVE = 1;
                        else
                        {
                            ServiceFillter.IS_ACTIVE = 0;
                        }
                    }

                    ServiceFillter.ORDER_FIELD = "MODIFY_TIME";
                    ServiceFillter.ORDER_DIRECTION = "DESC";
                    ServiceFillter.KEY_WORD = txtKeyword1.Text;

                    if (cboServiceType.EditValue != null)
                        ServiceFillter.SERVICE_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboServiceType.EditValue ?? "0").ToString());
                    else
                    {
                        ServiceFillter.SERVICE_TYPE_IDs = new List<long> {IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA,
                        IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G,
                        IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__GPBL,
                        IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH,
                        IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KHAC,
                        IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS,
                        IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PHCN,
                        IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT,
                        IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA,
                        IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN,
                        IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT,
                        IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN,
                        IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__AN
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridService_Default(object data)
        {
            try
            {
                WaitingManager.Show();
                listService = new List<V_HIS_SERVICE>();
                int start = ((CommonParam)data).Start ?? 0;
                int limit = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start, limit);
                MOS.Filter.HisServiceViewFilter ServiceFillter = new HisServiceViewFilter();
                if (cboActive.EditValue != null)
                {
                    ServiceFillter.IS_ACTIVE = Inventec.Common.TypeConvert.Parse.ToInt16((cboActive.EditValue ?? "0").ToString());
                }
                ServiceFillter.ID = this.currentService.ID;

                if (cboServiceType.EditValue != null)

                    ServiceFillter.SERVICE_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboServiceType.EditValue ?? "0").ToString());
                else
                {
                    ServiceFillter.SERVICE_TYPE_IDs = new List<long> {IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__GPBL,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KHAC,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PHCN,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN
                    };
                }

                if ((long)cboChoose.EditValue == 1)
                {
                    isChoseService = (long)cboChoose.EditValue;
                }

                var rs = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE>>(
                                                     "api/HisService/GetView",
                     HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                     ServiceFillter,
                     param);

                lstRoomServiceADOs = new List<ServiceADO>();

                if (rs != null && rs.Data.Count > 0)
                {

                    listService = rs.Data;
                    foreach (var item in listService)
                    {
                        ServiceADO RoomServiceADO = new ServiceADO(item);
                        if (isChoseService == 1)
                        {
                            RoomServiceADO.isKeyChooseService = true;
                            RoomServiceADO.radioService = true;
                        }
                        lstRoomServiceADOs.Add(RoomServiceADO);
                    }
                }

                if (ServiceRoomViews != null && ServiceRoomViews.Count > 0)
                {
                    foreach (var itemUsername in ServiceRoomViews)
                    {
                        var check = lstRoomServiceADOs.FirstOrDefault(o => o.ID == itemUsername.SERVICE_ID);
                        if (check != null)
                        {
                            check.checkService = true;
                            check.ABOUT_REQUEST = (itemUsername.IS_REQUEST == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);
                            check.ABOUT_EXECUTE = (itemUsername.IS_EXECUTE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);
                        }
                    }
                }

                lstRoomServiceADOs = lstRoomServiceADOs.OrderByDescending(p => p.checkService).ToList();
                if (ucGridControlService != null)
                {
                    ServiceProcessor.Reload(ucGridControlService, lstRoomServiceADOs);
                }
                rowCount = (data == null ? 0 : lstRoomServiceADOs.Count);
                dataTotal = (rs.Param == null ? 0 : rs.Param.Count ?? 0);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToCombo()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisServiceTypeFilter ServiceTypeFilter = new HisServiceTypeFilter();
                ServiceType = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_SERVICE_TYPE>>(
                             "api/HisServiceType/Get",
                    HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                    ServiceTypeFilter,
                    param);
                if (ServiceType != null && ServiceType.Count > 0)
                {
                    ServiceType = ServiceType.Where(o => o.ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC &&
                        o.ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT &&
                        o.ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU).ToList();
                }
                LoadDataToComboServiceType(cboServiceType, ServiceType);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadComboStatus()
        {
            try
            {
                List<Status> status = new List<Status>();
                status.Add(new Status(1, "Dịch vụ"));
                status.Add(new Status(2, "Phòng"));

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("statusName", "", 300, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("statusName", "id", columnInfos, false, 350);
                ControlEditorLoader.Load(cboChoose, status, controlEditorADO);
                cboChoose.EditValue = status[0].id;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadComboActive()
        {
            try
            {
                List<Status> status = new List<Status>();
                status.Add(new Status(1, "Không khóa"));
                status.Add(new Status(2, "Khóa"));

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("statusName", "", 200, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("statusName", "id", columnInfos, false, 200);
                ControlEditorLoader.Load(cboActive, status, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToComboServiceType(DevExpress.XtraEditors.GridLookUpEdit cboServiceType, List<HIS_SERVICE_TYPE> ServiceType)
        {
            try
            {
                cboServiceType.Properties.DataSource = ServiceType;
                cboServiceType.Properties.DisplayMember = "SERVICE_TYPE_NAME";
                cboServiceType.Properties.ValueMember = "ID";

                cboServiceType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboServiceType.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboServiceType.Properties.ImmediatePopup = true;
                cboServiceType.ForceInitialize();
                cboServiceType.Properties.View.Columns.Clear();

                GridColumn aColumnCode = cboServiceType.Properties.View.Columns.AddField("SERVICE_TYPE_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 100;

                GridColumn aColumnName = cboServiceType.Properties.View.Columns.AddField("SERVICE_TYPE_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 200;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnFind1_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                FillDataToGrid1(this);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnFind2_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                FillDataToGrid2(this);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboChoose_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                ServiceRoomViews = null;
                ServiceRooms = null;
                isChoseRoom = 0;
                isChoseService = 0;
                FillDataToGrid1(this);
                FillDataToGrid2(this);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                if (ucGridControlRoom != null && ucGridControlService != null)
                {
                    object Room = RoomProcessor.GetDataGridView(ucGridControlRoom);
                    object Service = ServiceProcessor.GetDataGridView(ucGridControlService);
                    bool success = false;
                    CommonParam param = new CommonParam();
                    if (isChoseService == 1)
                    {
                        if (ServiceIdCheckByService == 0)
                        {
                            WaitingManager.Hide();
                            DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn dịch vụ", "Thông báo");
                            return;
                        }
                        if (Room is List<HIS.UC.Room.RoomAccountADO>)
                        {
                            lstRoomADOs = (List<HIS.UC.Room.RoomAccountADO>)Room;

                            if (lstRoomADOs != null && lstRoomADOs.Count > 0)
                            {
                                var dataCheckeds = lstRoomADOs.Where(p => p.check1 == true).ToList();

                                //List xoa
                                var dataDeletes = lstRoomADOs.Where(o => ServiceRooms.Select(p => p.ROOM_ID)
                                    .Contains(o.ID) && o.check1 == false).ToList();

                                //list them
                                var dataCreates = dataCheckeds.Where(o => !ServiceRooms.Select(p => p.ROOM_ID)
                                    .Contains(o.ID)).ToList();

                                //list them
                                var dataUpdates = dataCheckeds.Where(o => ServiceRooms.Select(p => p.ROOM_ID)
                                    .Contains(o.ID)).ToList();
                                if (dataDeletes.Count == 0 && dataCheckeds.Count == 0)
                                {
                                    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn dịch vụ phòng", "Thông báo");
                                    return;
                                }

                                if (dataCheckeds != null)
                                {
                                    success = true;
                                }

                                if (dataDeletes != null && dataDeletes.Count > 0)
                                {
                                    List<long> deleteSds = ServiceRooms.Where(o => dataDeletes.Select(p => p.ID)
                                        .Contains(o.ROOM_ID)).Select(o => o.ID).ToList();
                                    bool deleteResult = new BackendAdapter(param).Post<bool>(
                                              "api/HisServiceRoom/DeleteList",
                                              HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                              deleteSds,
                                              param);
                                    if (deleteResult)
                                        success = true;
                                    ServiceRooms = ServiceRooms.Where(o => !deleteSds.Contains(o.ID)).ToList();
                                }

                                if (dataCreates != null && dataCreates.Count > 0)
                                {
                                    List<HIS_SERVICE_ROOM> ServiceRoomCreates = new List<HIS_SERVICE_ROOM>();
                                    foreach (var item in dataCreates)
                                    {
                                        HIS_SERVICE_ROOM ServiceRoom = new HIS_SERVICE_ROOM();
                                        ServiceRoom.SERVICE_ID = ServiceIdCheckByService;
                                        ServiceRoom.ROOM_ID = item.ID;
                                        ServiceRoom.IS_EXECUTE = 1;
                                        ServiceRoom.IS_EXECUTE = 1;
                                        ServiceRoom.IS_PRIORITY = item.checkPriority ? new Nullable<short>(1) : null;
                                        ServiceRoomCreates.Add(ServiceRoom);
                                    }

                                    var createResult = new BackendAdapter(param).Post<List<HIS_SERVICE_ROOM>>(
                                               "api/HisServiceRoom/CreateList",
                                               HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                               ServiceRoomCreates,
                                               param);
                                    if (createResult != null && createResult.Count > 0)
                                        success = true;
                                    AutoMapper.Mapper.CreateMap<HIS_SERVICE_ROOM, V_HIS_SERVICE_ROOM>();
                                    var vCreateResults = AutoMapper.Mapper.Map<List<HIS_SERVICE_ROOM>, List<V_HIS_SERVICE_ROOM>>(createResult);
                                    ServiceRooms.AddRange(vCreateResults);
                                }

                                if (dataUpdates != null && dataUpdates.Count > 0)
                                {
                                    List<HIS_SERVICE_ROOM> ServiceRoomUpdates = new List<HIS_SERVICE_ROOM>();
                                    foreach (var item in dataUpdates)
                                    {
                                        var serviceRoom = ServiceRooms.FirstOrDefault(o => o.ROOM_ID == item.ID);
                                        if (serviceRoom == null) continue;
                                        HIS_SERVICE_ROOM ServiceRoom = new HIS_SERVICE_ROOM();
                                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERVICE_ROOM>(ServiceRoom, serviceRoom);

                                        ServiceRoom.IS_EXECUTE = 1;
                                        ServiceRoom.IS_PRIORITY = item.checkPriority ? new Nullable<short>(1) : null;
                                        ServiceRoomUpdates.Add(ServiceRoom);
                                    }

                                    var createResult = new BackendAdapter(param).Post<List<HIS_SERVICE_ROOM>>(
                                               "api/HisServiceRoom/UpdateList",
                                               HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                               ServiceRoomUpdates,
                                               param);
                                    if (createResult != null && createResult.Count > 0)
                                        success = true;
                                }

                                lstRoomADOs = lstRoomADOs.OrderByDescending(p => p.check1).ToList();
                                if (ucGridControlRoom != null)
                                {
                                    RoomProcessor.Reload(ucGridControlRoom, lstRoomADOs);
                                }
                            }
                        }
                    }

                    if (isChoseRoom == 2)
                    {
                        if (RoomIdCheckByRoom == 0)
                        {
                            WaitingManager.Hide();
                            DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn phòng", "Thông báo");
                            return;
                        }

                        if (Service is List<HIS.UC.Service.ServiceADO>)
                        {
                            lstRoomServiceADOs = (List<HIS.UC.Service.ServiceADO>)Service;

                            if (lstRoomServiceADOs != null && lstRoomServiceADOs.Count > 0)
                            {
                                var dataChecked = lstRoomServiceADOs.Where(p => p.checkService == true).ToList();

                                //List xoa
                                var dataDelete = lstRoomServiceADOs.Where(o => ServiceRoomViews.Select(p => p.SERVICE_ID)
                                    .Contains(o.ID) && o.checkService == false).ToList();

                                //list them
                                var dataCreate = dataChecked.Where(o => !ServiceRoomViews.Select(p => p.SERVICE_ID)
                                    .Contains(o.ID)).ToList();

                                //list sua
                                var dataUpdate = dataChecked.Where(o => ServiceRoomViews.Exists(p => p.SERVICE_ID == o.ID && ((p.IS_REQUEST == 1) != o.ABOUT_REQUEST) || (p.IS_EXECUTE == 1) != o.ABOUT_EXECUTE)).ToList();

                                if (dataDelete.Count == 0 && dataChecked.Count == 0)
                                {
                                    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn phòng dịch vụ", "Thông báo");
                                    return;
                                }

                                if (dataChecked != null)
                                {
                                    success = true;
                                }

                                if (dataDelete != null && dataDelete.Count > 0)
                                {

                                    List<long> deleteId = ServiceRoomViews.Where(o => dataDelete.Select(p => p.ID)
                                        .Contains(o.SERVICE_ID)).Select(o => o.ID).ToList();
                                    bool deleteResult = new BackendAdapter(param).Post<bool>(
                                              "api/HisServiceRoom/DeleteList",
                                              HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                              deleteId,
                                              param);
                                    if (deleteResult)
                                        success = true;
                                    ServiceRoomViews = ServiceRoomViews.Where(o => !deleteId.Contains(o.ID)).ToList();
                                }

                                if (dataCreate != null && dataCreate.Count > 0)
                                {
                                    List<HIS_SERVICE_ROOM> ServiceRoomCreate = new List<HIS_SERVICE_ROOM>();
                                    foreach (var item in dataCreate)
                                    {
                                        HIS_SERVICE_ROOM ServiceRoomID = new HIS_SERVICE_ROOM();
                                        ServiceRoomID.ROOM_ID = RoomIdCheckByRoom;
                                        ServiceRoomID.SERVICE_ID = item.ID;
                                        ServiceRoomID.IS_EXECUTE = item.ABOUT_EXECUTE ? (short?)1 : null;
                                        ServiceRoomID.IS_REQUEST = item.ABOUT_REQUEST ? (short?)1 : null;
                                        ServiceRoomCreate.Add(ServiceRoomID);
                                    }

                                    var createResult = new BackendAdapter(param).Post<List<HIS_SERVICE_ROOM>>(
                                               "/api/HisServiceRoom/CreateList",
                                               HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                               ServiceRoomCreate,
                                               param);
                                    if (createResult != null && createResult.Count > 0)
                                        success = true;
                                    AutoMapper.Mapper.CreateMap<HIS_SERVICE_ROOM, V_HIS_SERVICE_ROOM>();
                                    var vCreateResults = AutoMapper.Mapper.Map<List<HIS_SERVICE_ROOM>, List<V_HIS_SERVICE_ROOM>>(createResult);
                                    ServiceRoomViews.AddRange(vCreateResults);
                                }

                                if (dataUpdate != null && dataUpdate.Count > 0)
                                {
                                    List<HIS_SERVICE_ROOM> ServiceRoomUpdate = new List<HIS_SERVICE_ROOM>();
                                    foreach (var item in dataUpdate)
                                    {
                                        var serviceRoom = ServiceRoomViews.FirstOrDefault(o => o.SERVICE_ID == item.ID);
                                        if (serviceRoom == null) continue;
                                        HIS_SERVICE_ROOM ServiceRoomID = new HIS_SERVICE_ROOM();
                                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERVICE_ROOM>(ServiceRoomID, serviceRoom);
                                        ServiceRoomID.IS_EXECUTE = item.ABOUT_EXECUTE ? (short?)1 : null;
                                        ServiceRoomID.IS_REQUEST = item.ABOUT_REQUEST ? (short?)1 : null;
                                        ServiceRoomUpdate.Add(ServiceRoomID);
                                    }

                                    var createResult = new BackendAdapter(param).Post<List<HIS_SERVICE_ROOM>>(
                                               "/api/HisServiceRoom/UpdateList",
                                               HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                               ServiceRoomUpdate,
                                               param);
                                    if (createResult != null && createResult.Count > 0)
                                        success = true;
                                }

                                lstRoomServiceADOs = lstRoomServiceADOs.OrderByDescending(p => p.checkService).ToList();
                                if (ucGridControlRoom != null)
                                {
                                    ServiceProcessor.Reload(ucGridControlService, lstRoomServiceADOs);
                                }
                            }
                        }
                    }
                    MessageManager.Show(this.ParentForm, param, success);
                }

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyword1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToGrid1(this);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeyword2_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToGrid2(this);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void FindShortcut1()
        {
            try
            {
                btnFind1_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void FindShortcut2()
        {
            try
            {
                btnFind2_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SaveShortcut()
        {
            try
            {
                btnSave.Focus();
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyword1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboServiceType.Focus();
                    cboServiceType.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboServiceType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtKeyword2.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboServiceType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboServiceType.EditValue != null)
                    {
                        HIS_SERVICE_TYPE data = ServiceType.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboServiceType.EditValue.ToString()));
                        if (data != null)
                        {
                            cboServiceType.Properties.Buttons[1].Visible = true;
                            btnFind1.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboServiceType_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboServiceType.Properties.Buttons[1].Visible = false;
                    cboServiceType.EditValue = null;
                }

                HisServiceTypeFilter filter = new HisServiceTypeFilter();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboChoose_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboServiceType.EditValue != null)
                {
                    HIS_SERVICE_TYPE data = ServiceType.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboServiceType.EditValue.ToString()));
                    if (data != null)
                    {
                        cboServiceType.Properties.Buttons[1].Visible = true;
                        btnFind1.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                List<object> listArgs = new List<object>();
                listArgs.Add((RefeshReference)RefreshData);
                if (this.currentModule != null)
                {
                    CallModule callModule = new CallModule(CallModule.HisImportServiceRoom, currentModule.RoomId, currentModule.RoomTypeId, listArgs);
                }
                else
                {
                    CallModule callModule = new CallModule(CallModule.HisImportServiceRoom, 0, 0, listArgs);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void RefreshData()
        {
            try
            {
                if (this.currentService == null && this.Room == null)
                {
                    FillDataToGrid1(this);
                    FillDataToGrid2(this);
                }
                else if (this.Room == null)
                {
                    cboChoose.EditValue = (long)1;
                    cboChoose.Enabled = false;
                    FillDataToGrid1_Default(this);
                    FillDataToGrid2(this);
                    btn_Radio_Enable_Click1(this.currentService);
                }
                else if (this.currentService == null)
                {
                    cboChoose.EditValue = (long)2;
                    cboChoose.Enabled = false;
                    FillDataToGrid1(this);
                    FillDataToGrid2_Default(this);
                    btn_Radio_Enable_Click(this.Room);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ServiceGridView_MouseRightClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if ((e.Item is BarButtonItem) && sender != null && sender is HIS.UC.Service.ServiceADO)
                {
                    var type = (HIS.UC.Service.Popup.PopupMenuProcessor.ItemType)e.Item.Tag;
                    switch (type)
                    {
                        case HIS.UC.Service.Popup.PopupMenuProcessor.ItemType.Copy:
                            {
                                if (isChoseService != 1)
                                {
                                    MessageManager.Show("Vui lòng chọn dịch vụ!");
                                    break;
                                }
                                this.currentCopyServiceAdo = (HIS.UC.Service.ServiceADO)sender;
                                break;
                            }
                        case HIS.UC.Service.Popup.PopupMenuProcessor.ItemType.Paste:
                            {
                                var currentPaste = (HIS.UC.Service.ServiceADO)sender;
                                bool success = false;
                                CommonParam param = new CommonParam();
                                if (this.currentCopyServiceAdo == null && isChoseService != 1)
                                {
                                    MessageManager.Show("Vui lòng copy!");
                                    break;
                                }
                                if (this.currentCopyServiceAdo != null && currentPaste != null && isChoseService == 1)
                                {
                                    if (this.currentCopyServiceAdo.ID == currentPaste.ID)
                                    {
                                        MessageManager.Show("Trùng dữ liệu copy và paste");
                                        break;
                                    }
                                    HisServiceRoomCopyByServiceSDO hisMestMatyCopyByMatySDO = new HisServiceRoomCopyByServiceSDO();
                                    hisMestMatyCopyByMatySDO.CopyServiceId = this.currentCopyServiceAdo.ID;
                                    hisMestMatyCopyByMatySDO.PasteServiceId = currentPaste.ID;
                                    var result = new BackendAdapter(param).Post<List<HIS_SERVICE_ROOM>>("api/HisServiceRoom/CopyByService", ApiConsumer.ApiConsumers.MosConsumer, hisMestMatyCopyByMatySDO, param);
                                    if (result != null)
                                    {
                                        success = true;
                                        List<HIS.UC.Room.RoomAccountADO> dataNew = new List<HIS.UC.Room.RoomAccountADO>();
                                        dataNew = (from r in listRoom select new RoomAccountADO(r)).ToList();
                                        if (result != null && result.Count > 0)
                                        {
                                            foreach (var itemRoom in result)
                                            {
                                                var check = dataNew.FirstOrDefault(o => o.ID == itemRoom.ROOM_ID);
                                                if (check != null)
                                                {
                                                    check.check1 = true;
                                                }
                                            }
                                        }
                                        dataNew = dataNew.OrderByDescending(p => p.check1).ToList();
                                        if (ucGridControlRoom != null)
                                        {
                                            RoomProcessor.Reload(ucGridControlRoom, dataNew);
                                        }
                                        else
                                        {
                                            FillDataToGrid2(this);
                                        }
                                    }
                                }
                                MessageManager.Show(this.ParentForm, param, success);
                                break;
                            }
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void RoomGridView_MouseRightClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if ((e.Item is BarButtonItem) && sender != null && sender is HIS.UC.Room.RoomAccountADO)
                {
                    var type = (HIS.UC.Room.Popup.PopupMenuProcessor.ItemType)e.Item.Tag;
                    switch (type)
                    {
                        case HIS.UC.Room.Popup.PopupMenuProcessor.ItemType.CopyPhongSangPhong:
                            {
                                if (isChoseRoom != 2)
                                {
                                    MessageManager.Show("Vui lòng chọn phòng!");
                                    break;
                                }
                                this.CurrentRoomCopyAdo = (HIS.UC.Room.RoomAccountADO)sender;
                                break;
                            }
                        case HIS.UC.Room.Popup.PopupMenuProcessor.ItemType.PastePhongSangPhong:
                            {
                                var currentPaste = (HIS.UC.Room.RoomAccountADO)sender;
                                bool success = false;
                                CommonParam param = new CommonParam();
                                if (this.CurrentRoomCopyAdo == null && isChoseRoom != 2)
                                {
                                    MessageManager.Show("Vui lòng copy!");
                                    break;
                                }
                                if (this.CurrentRoomCopyAdo != null && currentPaste != null && isChoseRoom == 2)
                                {
                                    if (this.CurrentRoomCopyAdo.ID == currentPaste.ID)
                                    {
                                        MessageManager.Show("Trùng dữ liệu copy và paste");
                                        break;
                                    }
                                    HisServiceRoomCopyByRoomSDO hisMestMatyCopyByMatySDO = new HisServiceRoomCopyByRoomSDO();
                                    hisMestMatyCopyByMatySDO.CopyRoomId = this.CurrentRoomCopyAdo.ID;
                                    hisMestMatyCopyByMatySDO.PasteRoomId = currentPaste.ID;
                                    var result = new BackendAdapter(param).Post<List<HIS_SERVICE_ROOM>>("api/HisServiceRoom/CopyByRoom", ApiConsumer.ApiConsumers.MosConsumer, hisMestMatyCopyByMatySDO, param);
                                    if (result != null)
                                    {
                                        success = true;
                                        List<HIS.UC.Service.ServiceADO> dataNew = new List<HIS.UC.Service.ServiceADO>();
                                        dataNew = (from r in listService select new HIS.UC.Service.ServiceADO(r)).ToList();
                                        if (result != null && result.Count > 0)
                                        {
                                            foreach (var itemService in result)
                                            {
                                                var check = dataNew.FirstOrDefault(o => o.ID == itemService.SERVICE_ID);
                                                if (check != null)
                                                {
                                                    check.checkService = true;
                                                    check.ABOUT_REQUEST = (itemService.IS_REQUEST == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);
                                                    check.ABOUT_EXECUTE = (itemService.IS_EXECUTE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);
                                                }
                                            }

                                            dataNew = dataNew.OrderByDescending(p => p.checkService).ToList();

                                            if (ucGridControlService != null)
                                            {
                                                ServiceProcessor.Reload(ucGridControlService, dataNew);
                                            }
                                        }
                                        else
                                        {
                                            FillDataToGrid1(this);
                                        }
                                    }
                                }
                                MessageManager.Show(this.ParentForm, param, success);
                                break;
                            }
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                BackendDataWorker.Reset<V_HIS_SERVICE>();
                BackendDataWorker.Reset<HIS_SERVICE>();
                BackendDataWorker.Reset<V_HIS_SERVICE_PATY>();
                BackendDataWorker.Reset<HIS.Desktop.LocalStorage.BackendData.ADO.ServiceComboADO>();
                BackendDataWorker.Reset<V_HIS_SERVICE_ROOM>();
                MessageBox.Show("Xử lý thành công", "Thông báo");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            try
            {
                BackendDataWorker.CacheMonitorSyncExecute((typeof(V_HIS_SERVICE)).ToString(), false);
                BackendDataWorker.CacheMonitorSyncExecute((typeof(V_HIS_SERVICE_PATY)).ToString(), false);
                BackendDataWorker.CacheMonitorSyncExecute((typeof(V_HIS_SERVICE_ROOM)).ToString(), false);
                BackendDataWorker.CacheMonitorSyncExecute((typeof(HIS_SERVICE)).ToString(), false);
                MessageBox.Show("Xử lý thành công", "Thông báo");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnExportExcel_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.FlexCellExport.Store store = new Inventec.Common.FlexCellExport.Store(true);

                string templateFile = System.IO.Path.Combine(Application.StartupPath + "\\Tmp\\Exp", "EXPORT_SERVICE_ROOM.xlsx");

                //chọn đường dẫn
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "Excel 2007 later file (*.xlsx)|*.xlsx|Excel 97-2003 file(*.xls)|*.xls";
                if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    //getdata
                    WaitingManager.Show();
                    if (!File.Exists(templateFile))
                    {
                        store = null;
                        DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Không tìm thấy file", templateFile));
                        return;
                    }

                    store.ReadTemplate(System.IO.Path.GetFullPath(templateFile));
                    if (store.TemplatePath == "")
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Biểu mẫu đang mở hoặc không tồn tại file template. Vui lòng kiểm tra lại. (" + templateFile + ")");
                        return;
                    }

                    //chọn phòng thì xất theo phòng.
                    //không chọn phòng thì xuất tất cả
                    if (isChoseRoom == 2 && ServiceRoomViews != null && ServiceRoomViews.Count > 0)
                    {
                        ProcessData(ServiceRoomViews, ref store);
                    }
                    else
                    {
                        CommonParam param = new CommonParam();
                        MOS.Filter.HisServiceRoomViewFilter filter = new HisServiceRoomViewFilter();
                        filter.IS_ACTIVE = 1;
                        var lstServiceRoomViews = new BackendAdapter(param).Get<List<V_HIS_SERVICE_ROOM>>("api/HisServiceRoom/GetView",
                                          HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                          filter,
                                          param);

                        ProcessData(lstServiceRoomViews, ref store);
                    }
                    WaitingManager.Hide();

                    if (store != null)
                    {
                        try
                        {
                            if (store.OutFile(saveFileDialog1.FileName))
                            {
                                DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.HisRoomService__ExportExcel__ThanhCong);

                                if (MessageBox.Show("Bạn có muốn mở file?",
                                    "Thông báo", MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Question) == DialogResult.Yes)
                                    System.Diagnostics.Process.Start(saveFileDialog1.FileName);
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn(ex);
                        }
                    }
                    else
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.HisRoomService__ExportExcel__ThatBai);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessData(List<V_HIS_SERVICE_ROOM> HisServiceRoom, ref Inventec.Common.FlexCellExport.Store store)
        {
            try
            {
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.SetCommonFunctions();

                if (HisServiceRoom != null && HisServiceRoom.Count > 0)
                {
                    HisServiceRoom = HisServiceRoom.OrderBy(o => o.ROOM_NAME).ThenBy(o => o.SERVICE_TYPE_CODE).ThenBy(o => o.SERVICE_NAME).ToList();
                    objectTag.AddObjectData(store, "EXPORT", HisServiceRoom);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtServiceCode_Exact_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToGrid1(this);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}







