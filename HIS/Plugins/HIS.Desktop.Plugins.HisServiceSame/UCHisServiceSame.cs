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
using HIS.Desktop.Plugins.HisServiceSame.Entity;
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
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;
using HIS.Desktop.Common;
using DevExpress.XtraBars;
using MOS.SDO;

namespace HIS.Desktop.Plugins.HisServiceSame
{
    public partial class UCHisServiceSame : HIS.Desktop.Utility.UserControlBase
    {
        List<HIS_SERVICE_TYPE> ServiceType { get; set; }

        internal Inventec.Desktop.Common.Modules.Module currentModule;
        UCServiceProcessor RoomProcessor;
        UCServiceProcessor ServiceProcessor;
        UserControl ucGridControlService;
        UserControl ucGridControlRoom;
        int start = 0;
        int limit = 0;
        int rowCount = 0;
        int dataTotal = 0;
        int start1 = 0;
        int limit1 = 0;
        int rowCount1 = 0;
        int dataTotal1 = 0;
        internal List<HIS.UC.Service.ServiceADO> lstRoomADOs { get; set; }
        internal List<HIS.UC.Service.ServiceADO> lstRoomServiceADOs { get; set; }
        List<V_HIS_SERVICE> listRoom;
        List<V_HIS_SERVICE> listService;
        long ServiceIdCheckByService = 0;
        long isChoseService;
        long isChoseRoom;
        long RoomIdCheckByRoom;
        bool isCheckAll;
        internal long servicetypeId;
        List<HIS_SERVICE_SAME> ServiceRooms { get; set; }
        List<HIS_SERVICE_SAME> ServiceRoomViews { get; set; }
        V_HIS_SERVICE currentService;
        HIS.UC.Service.ServiceADO currentCopyServiceAdo;
        HIS.UC.Service.ServiceADO CurrentRoomCopyAdo;

        public UCHisServiceSame(Inventec.Desktop.Common.Modules.Module currentModule)
            : base(currentModule)
        {
            InitializeComponent();
        }

        public UCHisServiceSame(Inventec.Desktop.Common.Modules.Module currentModule, long ServiceType)
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

        public UCHisServiceSame(V_HIS_SERVICE serviceData, Inventec.Desktop.Common.Modules.Module currentModule)
            : base(currentModule)
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
                if (this.currentService == null)
                {
                    FillDataToGrid1(this);
                    FillDataToGrid2(this);
                }
                else
                {
                    FillDataToGrid1_Default(this);
                    FillDataToGrid2(this);
                    btn_Radio_Enable_Click1(this.currentService);
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
                if (isChoseService == 1)
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
                        if (hi.Column.FieldName == "checkService")
                        {
                            var lstCheckAll = lstRoomServiceADOs;
                            List<HIS.UC.Service.ServiceADO> lstChecks = new List<HIS.UC.Service.ServiceADO>();

                            if (lstCheckAll != null && lstCheckAll.Count > 0)
                            {
                                var ServiceCheckedNum = lstRoomServiceADOs.Where(o => o.checkService == true).Count();
                                var ServiceNum = lstRoomServiceADOs.Count();
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
                    }
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisServiceSame.Resources.Lang", typeof(HIS.Desktop.Plugins.HisServiceSame.UCHisServiceSame).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCHisServiceSame.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboChoose.Properties.NullText = Inventec.Common.Resource.Get.Value("UCHisServiceSame.cboChoose.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("UCHisServiceSame.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.btnFind2.Text = Inventec.Common.Resource.Get.Value("UCHisServiceSame.btnFind2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("UCHisServiceSame.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnFind1.Text = Inventec.Common.Resource.Get.Value("UCHisServiceSame.btnFind1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboServiceType.Properties.NullText = Inventec.Common.Resource.Get.Value("UCHisServiceSame.cboServiceType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword2.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCHisServiceSame.txtKeyword2.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword1.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCHisServiceSame.txtKeyword1.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("UCHisServiceSame.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("UCHisServiceSame.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar2.Text = Inventec.Common.Resource.Get.Value("UCHisServiceSame.bar2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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

                ServiceColumn colCheck2 = new ServiceColumn("   ", "checkService", 30, true);
                colCheck2.VisibleIndex = 1;
                colCheck2.image = imageCollectionService.Images[0];
                colCheck2.Visible = false;
                colCheck2.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListServiceColumn.Add(colCheck2);

                ServiceColumn colMaDichvu = new ServiceColumn("Mã dịch vụ", "SERVICE_CODE", 60, false);
                colMaDichvu.VisibleIndex = 2;
                ado.ListServiceColumn.Add(colMaDichvu);

                ServiceColumn colTenDichvu = new ServiceColumn("Tên dịch vụ", "SERVICE_NAME", 300, false);
                colTenDichvu.VisibleIndex = 3;
                ado.ListServiceColumn.Add(colTenDichvu);

                ServiceColumn colMaLoaidichvu = new ServiceColumn("Loại dịch vụ", "SERVICE_TYPE_NAME", 80, false);
                colMaLoaidichvu.VisibleIndex = 4;
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
                        if (hi.Column.FieldName == "checkService")
                        {
                            var lstCheckAll = lstRoomADOs;
                            List<HIS.UC.Service.ServiceADO> lstChecks = new List<HIS.UC.Service.ServiceADO>();

                            if (lstCheckAll != null && lstCheckAll.Count > 0)
                            {
                                var RoomCheckedNum = lstRoomADOs.Where(o => o.checkService == true).Count();
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
                                            item.checkService = false;
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
                MOS.Filter.HisServiceSameFilter filter = new HisServiceSameFilter();
                filter.SERVICE_ID__OR__SAME_ID = data.ID;
                ServiceIdCheckByService = data.ID;

                ServiceRooms = new BackendAdapter(param).Get<List<HIS_SERVICE_SAME>>(
                                    "api/HisServiceSame/Get",
                                HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                filter,
                                param);
                List<HIS.UC.Service.ServiceADO> dataNew = new List<HIS.UC.Service.ServiceADO>();
                dataNew = (from r in listRoom select new ServiceADO(r)).ToList();
                if (ServiceRooms != null && ServiceRooms.Count > 0)
                {
                    foreach (var itemRoom in ServiceRooms)
                    {
                        var check = dataNew.FirstOrDefault(o => o.ID == itemRoom.SAME_ID);
                        var check2 = dataNew.FirstOrDefault(o => o.ID == itemRoom.SERVICE_ID);

                        if (check != null && check.ID != data.ID)
                        {
                            check.checkService = true;
                        }
                        if (check2 != null && check2.ID != data.ID)
                        {
                            check2.checkService = true;
                        }
                    }

                    //foreach (var item in dataNew)
                    //{
                    //     var check = ServiceRooms.FirstOrDefault(o => o.SAME_ID == item.ID);
                    //     if (check!=null)
                    //     {
                    //         item.checkService = true;
                    //     }
                    //}
                }
                dataNew = dataNew.OrderByDescending(p => p.checkService).ToList();
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
                RoomProcessor = new UCServiceProcessor();
                ServiceInitADO ado = new ServiceInitADO();
                ado.ListServiceColumn = new List<ServiceColumn>();
                ado.gridViewService_MouseDownMest = gridViewRoom_MouseRoom;
                ado.btn_Radio_Enable_Click1 = btn_Radio_Enable_Click;
                ado.gridView_MouseRightClick = RoomGridView_MouseRightClick;

                ServiceColumn colRadio1 = new ServiceColumn("   ", "radioService", 30, true);
                colRadio1.VisibleIndex = 0;
                colRadio1.Visible = false;
                colRadio1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListServiceColumn.Add(colRadio1);

                ServiceColumn colCheck1 = new ServiceColumn("   ", "checkService", 30, true);
                colCheck1.VisibleIndex = 1;
                colCheck1.image = imageCollectionRoom.Images[0];
                colCheck1.Visible = false;
                colCheck1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListServiceColumn.Add(colCheck1);

                ServiceColumn colMaPhong = new ServiceColumn("Mã dịch vụ", "SERVICE_CODE", 80, false);
                colMaPhong.VisibleIndex = 2;
                ado.ListServiceColumn.Add(colMaPhong);

                ServiceColumn colTenPhong = new ServiceColumn("Tên dịch vụ", "SERVICE_NAME", 150, false);
                colTenPhong.VisibleIndex = 3;
                ado.ListServiceColumn.Add(colTenPhong);

                ServiceColumn colLoaiPhong = new ServiceColumn("Loại dịch vụ", "SERVICE_TYPE_NAME", 100, false);
                colLoaiPhong.VisibleIndex = 4;
                ado.ListServiceColumn.Add(colLoaiPhong);

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

        private void btn_Radio_Enable_Click(V_HIS_SERVICE data)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                MOS.Filter.HisServiceSameFilter filter = new HisServiceSameFilter();
                filter.SAME_ID = data.ID;
                RoomIdCheckByRoom = data.ID;
                ServiceRoomViews = new BackendAdapter(param).Get<List<HIS_SERVICE_SAME>>(
                                         "api/HisServiceSame/Get",

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

        private void FillDataToGrid2(UCHisServiceSame uCRoomService)
        {
            try
            {
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

        private void FillDataToGridRoom(object data)
        {
            try
            {
                WaitingManager.Show();
                listRoom = new List<V_HIS_SERVICE>();
                int start1 = ((CommonParam)data).Start ?? 0;
                int limit1 = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start1, limit1);
                MOS.Filter.HisServiceViewFilter RoomFillter = new HisServiceViewFilter();
                RoomFillter.IS_ACTIVE = 1;
                RoomFillter.ORDER_FIELD = "MODIFY_TIME";
                RoomFillter.ORDER_DIRECTION = "DESC";
                RoomFillter.KEY_WORD = txtKeyword2.Text;
                RoomFillter.SERVICE_TYPE_IDs = new List<long> { IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KHAC,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PHCN,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN};
                if ((long)cboChoose.EditValue == 2)
                {
                    isChoseRoom = (long)cboChoose.EditValue;
                }

                var sar = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE>>(
                   HIS.Desktop.ApiConsumer.HisRequestUriStore.HIS_SERVICE_GETVIEW,
                    HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                      RoomFillter,
                    param);

                lstRoomADOs = new List<ServiceADO>();
                if (sar != null && sar.Data.Count > 0)
                {
                    listRoom = sar.Data;
                    foreach (var item in listRoom)
                    {
                        ServiceADO roomaccountADO = new ServiceADO(item);
                        if (isChoseRoom == 2)
                        {
                            roomaccountADO.isKeyChooseService = true;
                        }
                        lstRoomADOs.Add(roomaccountADO);
                    }
                }

                if (ServiceRooms != null && ServiceRooms.Count > 0)
                {
                    foreach (var itemUsername in ServiceRooms)
                    {
                        var check = lstRoomADOs.FirstOrDefault(o => o.ID == itemUsername.SAME_ID);
                        if (check != null && check.ID != ServiceIdCheckByService)
                        {
                            check.checkService = true;
                        }

                        var check2 = lstRoomADOs.FirstOrDefault(o => o.ID == itemUsername.SERVICE_ID);
                        if (check2 != null && check2.ID != ServiceIdCheckByService)
                        {
                            check2.checkService = true;
                        }
                    }
                }
                lstRoomADOs = lstRoomADOs.OrderByDescending(p => p.checkService).Distinct().ToList();

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

        private void FillDataToGrid1(UCHisServiceSame UCHisServiceSame)
        {
            try
            {
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

        private void FillDataToGrid1_Default(UCHisServiceSame UCHisServiceSame)
        {
            try
            {
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

                if (cboActive.EditValue != null)
                {
                    if (Inventec.Common.TypeConvert.Parse.ToInt16((cboActive.EditValue ?? "0").ToString()) == 1)
                        ServiceFillter.IS_ACTIVE = 1;
                    else
                    {
                        ServiceFillter.IS_ACTIVE = 0;
                    }
                }

                //ServiceFillter.IS_ACTIVE = 1;
                ServiceFillter.ORDER_FIELD = "MODIFY_TIME";
                ServiceFillter.ORDER_DIRECTION = "DESC";
                ServiceFillter.KEY_WORD = txtKeyword1.Text;
                ServiceFillter.SERVICE_TYPE_IDs = new List<long> { IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KHAC,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PHCN,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN};

                if (cboServiceType.EditValue != null)

                    ServiceFillter.SERVICE_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboServiceType.EditValue ?? "0").ToString());

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
                ServiceFillter.SERVICE_TYPE_IDs = new List<long> { IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KHAC,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PHCN,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN};
                if (cboActive.EditValue != null)
                {
                    ServiceFillter.IS_ACTIVE = Inventec.Common.TypeConvert.Parse.ToInt16((cboActive.EditValue ?? "0").ToString());
                }
                ServiceFillter.ID = this.currentService.ID;

                if (cboServiceType.EditValue != null)

                    ServiceFillter.SERVICE_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboServiceType.EditValue ?? "0").ToString());

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
                ServiceTypeFilter.IDs = new List<long> { IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KHAC,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PHCN,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN};

                ServiceType = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_SERVICE_TYPE>>(
                             "api/HisServiceType/Get",
                    HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                    ServiceTypeFilter,
                    param);
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
                status.Add(new Status(2, "Dịch vụ"));

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
                columnInfos.Add(new ColumnInfo("statusName", "", 300, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("statusName", "id", columnInfos, false, 350);
                ControlEditorLoader.Load(cboActive, status, controlEditorADO);
                //cboChoose.EditValue = status[0].id;
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
                        if (Room is List<HIS.UC.Service.ServiceADO>)
                        {
                            lstRoomADOs = (List<HIS.UC.Service.ServiceADO>)Room;

                            if (lstRoomADOs != null && lstRoomADOs.Count > 0)
                            {
                                //List<long> listServiceRooms = ServiceRooms.Select(p => p.SERVICE_ID).ToList();

                                var dataCheckeds = lstRoomADOs.Where(p => p.checkService == true).ToList();

                                //List xoa
                                var serviceSameDelete = ServiceRooms != null && ServiceRooms.Count > 0 && dataCheckeds != null && dataCheckeds.Count > 0 ? ServiceRooms.Where(o => !dataCheckeds.Select(p => p.ID).Contains(o.SERVICE_ID) && !dataCheckeds.Select(p => p.ID).Contains(o.SAME_ID)).ToList() : ServiceRooms;
                                List<HIS.UC.Service.ServiceADO> dataDeletes = new List<ServiceADO>();
                                var dataDeletes2 = serviceSameDelete != null && serviceSameDelete.Count > 0 ? lstRoomADOs.Where(o => serviceSameDelete.Select(p => p.SERVICE_ID)
                                    .Contains(o.ID) && o.checkService == false).ToList() : null;
                                var dataDeletes1 = serviceSameDelete != null && serviceSameDelete.Count > 0 ? lstRoomADOs.Where(o => serviceSameDelete.Select(p => p.SAME_ID)
                                    .Contains(o.ID) && o.checkService == false).ToList() : null;

                                if (dataDeletes2 != null && dataDeletes2.Count > 0)
                                {
                                    dataDeletes.AddRange(dataDeletes2);
                                }
                                if (dataDeletes1 != null && dataDeletes1.Count > 0)
                                {
                                    dataDeletes.AddRange(dataDeletes1);
                                }

                                //list them
                                var dataCreates = dataCheckeds.Where(o => !ServiceRooms.Select(p => p.SAME_ID)
                                    .Contains(o.ID)).ToList();
                                if (dataDeletes.Count == 0 && dataCheckeds.Count == 0)
                                {
                                    WaitingManager.Hide();
                                    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn dịch vụ", "Thông báo");
                                    return;
                                }
                                if (dataCheckeds != null)
                                {
                                    success = true;
                                }

                                if (dataDeletes != null && dataDeletes.Count > 0 && serviceSameDelete != null && serviceSameDelete.Count > 0)
                                {
                                    List<long> deleteSds = new List<long>();
                                    List<long> deleteSds1 = serviceSameDelete.Where(o => dataDeletes.Select(p => p.ID)
                                        .Contains(o.SAME_ID)).Select(o => o.ID).ToList();
                                    List<long> deleteSds2 = serviceSameDelete.Where(o => dataDeletes.Select(p => p.ID)
                                        .Contains(o.SERVICE_ID)).Select(o => o.ID).ToList();

                                    if (deleteSds1 != null && deleteSds1.Count > 0)
                                    {
                                        deleteSds.AddRange(deleteSds1);
                                    }
                                    if (deleteSds2 != null && deleteSds2.Count > 0)
                                    {
                                        deleteSds.AddRange(deleteSds2);
                                    }
                                    deleteSds = deleteSds.Distinct().ToList();
                                    bool deleteResult = new BackendAdapter(param).Post<bool>(
                                              "api/HisServiceSame/DeleteList",
                                              HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                              deleteSds,
                                              param);
                                    if (deleteResult)
                                        success = true;
                                    ServiceRooms = ServiceRooms.Where(o => !deleteSds.Contains(o.ID)).ToList();
                                }

                                if (dataCreates != null && dataCreates.Count > 0)
                                {
                                    List<HIS_SERVICE_SAME> ServiceRoomCreates = new List<HIS_SERVICE_SAME>();
                                    foreach (var item in dataCreates)
                                    {
                                        HIS_SERVICE_SAME ServiceRoom = new HIS_SERVICE_SAME();
                                        ServiceRoom.SERVICE_ID = ServiceIdCheckByService;
                                        ServiceRoom.SAME_ID = item.ID;
                                        ServiceRoomCreates.Add(ServiceRoom);
                                    }

                                    var createResult = new BackendAdapter(param).Post<List<HIS_SERVICE_SAME>>(
                                               "api/HisServiceSame/CreateList",
                                               HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                               ServiceRoomCreates,
                                               param);
                                    if (createResult != null && createResult.Count > 0)
                                        success = true;
                                    AutoMapper.Mapper.CreateMap<HIS_SERVICE_SAME, HIS_SERVICE_SAME>();
                                    var vCreateResults = AutoMapper.Mapper.Map<List<HIS_SERVICE_SAME>, List<HIS_SERVICE_SAME>>(createResult);
                                    ServiceRooms.AddRange(vCreateResults);
                                }

                                lstRoomADOs = lstRoomADOs.OrderByDescending(p => p.checkService).ToList();
                                if (ucGridControlRoom != null)
                                {
                                    RoomProcessor.Reload(ucGridControlRoom, lstRoomADOs);
                                }
                            }
                        }
                    }

                    if (isChoseRoom == 2)
                    {
                        if (Service is List<HIS.UC.Service.ServiceADO>)
                        {
                            lstRoomServiceADOs = (List<HIS.UC.Service.ServiceADO>)Service;

                            if (lstRoomServiceADOs != null && lstRoomServiceADOs.Count > 0)
                            {
                                //List<long> listRoomServices = ServiceRoom.Select(p => p.ROOM_ID).ToList();

                                var dataChecked = lstRoomServiceADOs.Where(p => p.checkService == true).ToList();
                                //List xoa

                                var dataDelete = lstRoomServiceADOs.Where(o => ServiceRoomViews.Select(p => p.SERVICE_ID)
                                    .Contains(o.ID) && o.checkService == false).ToList();

                                //list them
                                var dataCreate = dataChecked.Where(o => !ServiceRoomViews.Select(p => p.SERVICE_ID)
                                    .Contains(o.ID)).ToList();

                                if (dataDelete.Count == 0 && dataChecked.Count == 0)
                                {
                                    WaitingManager.Hide();
                                    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn dịch vụ", "Thông báo");
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
                                    List<HIS_SERVICE_SAME> ServiceRoomCreate = new List<HIS_SERVICE_SAME>();
                                    foreach (var item in dataCreate)
                                    {
                                        HIS_SERVICE_SAME ServiceRoomID = new HIS_SERVICE_SAME();
                                        ServiceRoomID.SAME_ID = RoomIdCheckByRoom;
                                        ServiceRoomID.SERVICE_ID = item.ID;
                                        ServiceRoomCreate.Add(ServiceRoomID);
                                    }

                                    var createResult = new BackendAdapter(param).Post<List<HIS_SERVICE_SAME>>(
                                               "/api/HisServiceRoom/CreateList",
                                               HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                               ServiceRoomCreate,
                                               param);
                                    if (createResult != null && createResult.Count > 0)
                                        success = true;
                                    AutoMapper.Mapper.CreateMap<HIS_SERVICE_SAME, HIS_SERVICE_SAME>();
                                    var vCreateResults = AutoMapper.Mapper.Map<List<HIS_SERVICE_SAME>, List<HIS_SERVICE_SAME>>(createResult);
                                    ServiceRoomViews.AddRange(vCreateResults);
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
                if (this.currentService == null)
                {
                    FillDataToGrid1(this);
                    FillDataToGrid2(this);
                }
                else
                {
                    FillDataToGrid1_Default(this);
                    FillDataToGrid2(this);
                    btn_Radio_Enable_Click1(this.currentService);
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
                                        List<HIS.UC.Service.ServiceADO> dataNew = new List<HIS.UC.Service.ServiceADO>();
                                        dataNew = (from r in listRoom select new ServiceADO(r)).ToList();
                                        if (result != null && result.Count > 0)
                                        {
                                            foreach (var itemRoom in result)
                                            {
                                                var check = dataNew.FirstOrDefault(o => o.ID == itemRoom.ROOM_ID);
                                                if (check != null)
                                                {
                                                    check.checkService = true;
                                                }
                                            }
                                        }
                                        dataNew = dataNew.OrderByDescending(p => p.checkService).ToList();
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
                if ((e.Item is BarButtonItem) && sender != null && sender is HIS.UC.Service.ServiceADO)
                {
                    var type = (HIS.UC.Service.Popup.PopupMenuProcessor.ItemType)e.Item.Tag;
                    switch (type)
                    {
                        case HIS.UC.Service.Popup.PopupMenuProcessor.ItemType.Copy:
                            {
                                if (isChoseRoom != 2)
                                {
                                    MessageManager.Show("Vui lòng chọn phòng!");
                                    break;
                                }
                                this.CurrentRoomCopyAdo = (HIS.UC.Service.ServiceADO)sender;
                                break;
                            }
                        case HIS.UC.Service.Popup.PopupMenuProcessor.ItemType.Paste:
                            {
                                var currentPaste = (HIS.UC.Service.ServiceADO)sender;
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
            BackendDataWorker.Reset<V_HIS_SERVICE>();
            BackendDataWorker.Reset<HIS_SERVICE>();
            BackendDataWorker.Reset<V_HIS_SERVICE_PATY>();
            BackendDataWorker.Reset<HIS.Desktop.LocalStorage.BackendData.ADO.ServiceComboADO>();
            BackendDataWorker.Reset<V_HIS_SERVICE_ROOM>();
            MessageBox.Show("Xử lý thành công", "Thông báo");
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            BackendDataWorker.CacheMonitorSyncExecute((typeof(V_HIS_SERVICE)).ToString(), false);
            BackendDataWorker.CacheMonitorSyncExecute((typeof(V_HIS_SERVICE_PATY)).ToString(), false);
            BackendDataWorker.CacheMonitorSyncExecute((typeof(V_HIS_SERVICE_ROOM)).ToString(), false);
            BackendDataWorker.CacheMonitorSyncExecute((typeof(HIS_SERVICE)).ToString(), false);
            MessageBox.Show("Xử lý thành công", "Thông báo");
        }
    }
}







