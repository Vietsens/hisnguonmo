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
using HIS.Desktop.Plugins.HisServiceRetyCat.Entity;
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
using HIS.UC.ReportRetyCat;
using HIS.UC.ReportRetyCat.ADO;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraBars;
using MOS.SDO;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using HIS.Desktop.LibraryMessage;

namespace HIS.Desktop.Plugins.HisServiceRetyCat
{
    public partial class UCServiceRetyCat : HIS.Desktop.Utility.UserControlBase
    {
        List<HIS_SERVICE_TYPE> ServiceType { get; set; }

        internal Inventec.Desktop.Common.Modules.Module currentModule;
        ReportRetyCatProcessor ReportRetyCatProcessor;
        UCServiceProcessor ServiceProcessor;
        UserControl ucGridControlService;
        UserControl ucGridControlReportRetyCat;
        int start = 0;
        int limit = 0;
        int rowCount = 0;
        int dataTotal = 0;
        int start1 = 0;
        int limit1 = 0;
        int rowCount1 = 0;
        int dataTotal1 = 0;
        internal List<HIS.UC.ReportRetyCat.ReportRetyCatADO> lstReportRetyCatADOs { get; set; }
        internal List<HIS.UC.Service.ServiceADO> lstRoomServiceADOs { get; set; }
        List<HIS_REPORT_TYPE_CAT> listReportRetyCat;
        List<V_HIS_SERVICE> listService;
        long ServiceIdCheckByService = 0;
        long isChoseService;
        long isChoseReportRetyCat;
        long RetyIdCheckByRety;
        bool isCheckAll;
        internal long servicetypeId;
        List<HIS_SERVICE_RETY_CAT> ServiceRetyCats { get; set; }
        List<HIS_SERVICE_RETY_CAT> ServiceRetyCatViews { get; set; }
        V_HIS_SERVICE currentService;
        HIS.UC.ReportRetyCat.ReportRetyCatADO currentCopyReportRetyCatAdo { get; set; }
        HIS.UC.Service.ServiceADO currentCopyServiceAdo { get; set; }

        public UCServiceRetyCat(Inventec.Desktop.Common.Modules.Module currentModule)
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

        public UCServiceRetyCat(Inventec.Desktop.Common.Modules.Module currentModule, V_HIS_SERVICE serviceData)
            : base(currentModule)
        {
            InitializeComponent();
            try
            {
                this.currentService = serviceData;
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

        public UCServiceRetyCat(Inventec.Desktop.Common.Modules.Module currentModule, long ServiceType)
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

        private void UCRoomService_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                LoadDataToCombo();
                LoadComboStatus();
                InitUcgrid1();
                InitUcgrid2();
                if (this.currentService == null)
                {
                    FillDataToGridService(this);
                    FillDataToGridReportTypeCat(this);
                }
                else
                {
                    FillDataToGrid1_Service(this);
                    FillDataToGridReportTypeCat(this);
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

        private void gridViewReportRetyCat_MouseRoom(object sender, MouseEventArgs e)
        {
            try
            {
                if (isChoseReportRetyCat == 2)
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
                            var lstCheckAll = lstReportRetyCatADOs;
                            List<HIS.UC.ReportRetyCat.ReportRetyCatADO> lstChecks = new List<HIS.UC.ReportRetyCat.ReportRetyCatADO>();

                            if (lstCheckAll != null && lstCheckAll.Count > 0)
                            {
                                var RoomCheckedNum = lstReportRetyCatADOs.Where(o => o.check1 == true).Count();
                                var RoomtmNum = lstReportRetyCatADOs.Count();
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

                                ReportRetyCatProcessor.Reload(ucGridControlReportRetyCat, lstChecks);


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
                MOS.Filter.HisServiceRoomFilter filter = new HisServiceRoomFilter();
                filter.SERVICE_ID = data.ID;
                ServiceIdCheckByService = data.ID;

                ServiceRetyCats = new BackendAdapter(param).Get<List<HIS_SERVICE_RETY_CAT>>(
                                    "api/HisServiceRetyCat/Get",
                                HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                filter,
                                param);
                List<HIS.UC.ReportRetyCat.ReportRetyCatADO> dataNew = new List<HIS.UC.ReportRetyCat.ReportRetyCatADO>();
                dataNew = (from r in listReportRetyCat select new ReportRetyCatADO(r)).ToList();
                if (ServiceRetyCats != null && ServiceRetyCats.Count > 0)
                {
                    foreach (var itemRoom in ServiceRetyCats)
                    {
                        var check = dataNew.FirstOrDefault(o => o.ID == itemRoom.REPORT_TYPE_CAT_ID);
                        if (check != null)
                        {
                            check.check1 = true;
                        }
                    }
                }
                dataNew = dataNew.OrderByDescending(p => p.check1).ToList();
                if (ucGridControlReportRetyCat != null)
                {
                    ReportRetyCatProcessor.Reload(ucGridControlReportRetyCat, dataNew);
                }
                else
                {
                    FillDataToGridReportTypeCat(this);
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
                ReportRetyCatProcessor = new ReportRetyCatProcessor();
                ReportRetyCatInitADO ado = new ReportRetyCatInitADO();
                ado.ListReportRetyCatColumn = new List<UC.ReportRetyCat.ReportRetyCatColumn>();
                ado.GridViewReportRetyCat_MouseDown = gridViewReportRetyCat_MouseRoom;
                ado.btn_Radio_Enable_Click = btn_Radio_Enable_Click;
                ado.gridView_MouseRightClick = ReportRetyCatGridView_MouseRightClick;

                ReportRetyCatColumn colRadio1 = new ReportRetyCatColumn("   ", "radio1", 30, true);
                colRadio1.VisibleIndex = 0;
                colRadio1.Visible = false;
                colRadio1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListReportRetyCatColumn.Add(colRadio1);

                ReportRetyCatColumn colCheck1 = new ReportRetyCatColumn("   ", "check1", 30, true);
                colCheck1.VisibleIndex = 1;
                colCheck1.image = imageCollectionRoom.Images[0];
                colCheck1.Visible = false;
                colCheck1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListReportRetyCatColumn.Add(colCheck1);

                ReportRetyCatColumn colMaPhong = new ReportRetyCatColumn("Mã loại báo cáo", "REPORT_TYPE_CODE", 60, false);
                colMaPhong.VisibleIndex = 2;
                ado.ListReportRetyCatColumn.Add(colMaPhong);

                ReportRetyCatColumn colTenPhong = new ReportRetyCatColumn("Mã báo cáo", "CATEGORY_CODE", 100, false);
                colTenPhong.VisibleIndex = 3;
                ado.ListReportRetyCatColumn.Add(colTenPhong);

                ReportRetyCatColumn colLoaiPhong = new ReportRetyCatColumn("Tên báo cáo", "CATEGORY_NAME", 100, false);
                colLoaiPhong.VisibleIndex = 4;
                ado.ListReportRetyCatColumn.Add(colLoaiPhong);

                this.ucGridControlReportRetyCat = (UserControl)ReportRetyCatProcessor.Run(ado);
                if (ucGridControlReportRetyCat != null)
                {
                    this.panelControl2.Controls.Add(this.ucGridControlReportRetyCat);
                    this.ucGridControlReportRetyCat.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void btn_Radio_Enable_Click(HIS_REPORT_TYPE_CAT data)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                MOS.Filter.HisServiceRetyCatFilter filter = new HisServiceRetyCatFilter();
                filter.REPORT_TYPE_CAT_ID = data.ID;
                RetyIdCheckByRety = data.ID;
                ServiceRetyCatViews = new BackendAdapter(param).Get<List<HIS_SERVICE_RETY_CAT>>(
                                         "api/HisServiceRetyCat/Get",

                                HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                filter,
                                param);
                List<HIS.UC.Service.ServiceADO> dataNew = new List<HIS.UC.Service.ServiceADO>();
                dataNew = (from r in listService select new HIS.UC.Service.ServiceADO(r)).ToList();
                if (ServiceRetyCatViews != null && ServiceRetyCatViews.Count > 0)
                {

                    foreach (var itemService in ServiceRetyCatViews)
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
                    FillDataToGridService(this);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridReportTypeCat(UCServiceRetyCat uCRoomService)
        {
            try
            {
                RetyIdCheckByRety = 0;
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
                listReportRetyCat = new List<HIS_REPORT_TYPE_CAT>();
                int start1 = ((CommonParam)data).Start ?? 0;
                int limit1 = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start1, limit1);
                MOS.Filter.HisReportTypeCatFilter RoomFillter = new HisReportTypeCatFilter();
                RoomFillter.ORDER_FIELD = "MODIFY_TIME";
                RoomFillter.ORDER_DIRECTION = "DESC";
                RoomFillter.KEY_WORD = txtKeyword2.Text;
                if ((long)cboChoose.EditValue == 2)
                {
                    isChoseReportRetyCat = (long)cboChoose.EditValue;
                }

                var sar = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<MOS.EFMODEL.DataModels.HIS_REPORT_TYPE_CAT>>(
                   "api/HisReportTypeCat/Get",
                    HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                      RoomFillter,
                    param);

                lstReportRetyCatADOs = new List<ReportRetyCatADO>();
                if (sar != null && sar.Data.Count > 0)
                {
                    listReportRetyCat = sar.Data;
                    foreach (var item in listReportRetyCat)
                    {
                        ReportRetyCatADO reportTypeCatADO = new ReportRetyCatADO(item);
                        if (isChoseReportRetyCat == 2)
                        {
                            reportTypeCatADO.isKeyChoose = true;
                        }
                        lstReportRetyCatADOs.Add(reportTypeCatADO);
                    }
                }

                if (ServiceRetyCats != null && ServiceRetyCats.Count > 0)
                {
                    foreach (var itemUsername in ServiceRetyCats)
                    {
                        var check = lstReportRetyCatADOs.FirstOrDefault(o => o.ID == itemUsername.REPORT_TYPE_CAT_ID);
                        if (check != null)
                        {
                            check.check1 = true;
                        }
                    }
                }
                lstReportRetyCatADOs = lstReportRetyCatADOs.OrderByDescending(p => p.check1).ToList();

                if (ucGridControlReportRetyCat != null)
                {
                    ReportRetyCatProcessor.Reload(ucGridControlReportRetyCat, lstReportRetyCatADOs);
                }
                rowCount1 = (data == null ? 0 : lstReportRetyCatADOs.Count);
                dataTotal1 = (sar.Param == null ? 0 : sar.Param.Count ?? 0);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridService(UCServiceRetyCat UCRoomService)
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

        private void FillDataToGrid1_Service(UCServiceRetyCat UCRoomService)
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
                ServiceFillter.ORDER_FIELD = "MODIFY_TIME";
                ServiceFillter.ORDER_DIRECTION = "DESC";
                ServiceFillter.KEY_WORD = txtKeyword1.Text;

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

                if (ServiceRetyCatViews != null && ServiceRetyCatViews.Count > 0)
                {
                    foreach (var itemUsername in ServiceRetyCatViews)
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

                if (ServiceRetyCatViews != null && ServiceRetyCatViews.Count > 0)
                {
                    foreach (var itemUsername in ServiceRetyCatViews)
                    {
                        var check = lstRoomServiceADOs.FirstOrDefault(o => o.ID == itemUsername.SERVICE_ID);
                        if (check != null)
                        {
                            check.checkService = true;
                            check.radioService = true;
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
                status.Add(new Status(2, "Nhóm loại báo cáo"));

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

        private void btnFindService_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                FillDataToGridService(this);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnFindReportTypeCat_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                FillDataToGridReportTypeCat(this);
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
                ServiceRetyCatViews = null;
                ServiceRetyCats = null;
                isChoseReportRetyCat = 0;
                isChoseService = 0;
                FillDataToGridService(this);
                FillDataToGridReportTypeCat(this);
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
                if (ucGridControlReportRetyCat != null && ucGridControlService != null)
                {
                    object Room = ReportRetyCatProcessor.GetDataGridView(ucGridControlReportRetyCat);
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
                        if (Room is List<HIS.UC.ReportRetyCat.ReportRetyCatADO>)
                        {
                            lstReportRetyCatADOs = (List<HIS.UC.ReportRetyCat.ReportRetyCatADO>)Room;

                            if (lstReportRetyCatADOs != null && lstReportRetyCatADOs.Count > 0)
                            {
                                //List<long> listServiceRooms = ServiceRooms.Select(p => p.SERVICE_ID).ToList();

                                var dataCheckeds = lstReportRetyCatADOs.Where(p => p.check1 == true).ToList();

                                //List xoa

                                var dataDeletes = lstReportRetyCatADOs.Where(o => ServiceRetyCats.Select(p => p.REPORT_TYPE_CAT_ID)
                                    .Contains(o.ID) && o.check1 == false).ToList();


                                //list them
                                var dataCreates = dataCheckeds.Where(o => !ServiceRetyCats.Select(p => p.REPORT_TYPE_CAT_ID)
                                    .Contains(o.ID)).ToList();
                                if (dataDeletes.Count == 0 && dataCheckeds.Count == 0)
                                {
                                    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn dịch vụ nhóm loại báo cáo", "Thông báo");
                                    return;
                                }
                                if (dataCheckeds != null)
                                {
                                    success = true;
                                }

                                if (dataDeletes != null && dataDeletes.Count > 0)
                                {
                                    List<long> deleteSds = ServiceRetyCats.Where(o => dataDeletes.Select(p => p.ID)
                                        .Contains(o.REPORT_TYPE_CAT_ID)).Select(o => o.ID).ToList();
                                    bool deleteResult = new BackendAdapter(param).Post<bool>(
                                              "api/HisServiceRetyCat/DeleteList",
                                              HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                              deleteSds,
                                              param);
                                    if (deleteResult)
                                        success = true;
                                    ServiceRetyCats = ServiceRetyCats.Where(o => !deleteSds.Contains(o.ID)).ToList();
                                }

                                if (dataCreates != null && dataCreates.Count > 0)
                                {
                                    List<HIS_SERVICE_RETY_CAT> ServiceRoomCreates = new List<HIS_SERVICE_RETY_CAT>();
                                    foreach (var item in dataCreates)
                                    {
                                        HIS_SERVICE_RETY_CAT ServiceRoom = new HIS_SERVICE_RETY_CAT();
                                        ServiceRoom.SERVICE_ID = ServiceIdCheckByService;
                                        ServiceRoom.REPORT_TYPE_CAT_ID = item.ID;
                                        ServiceRoomCreates.Add(ServiceRoom);
                                    }

                                    var createResult = new BackendAdapter(param).Post<List<HIS_SERVICE_RETY_CAT>>(
                                               "api/HisServiceRetyCat/CreateList",
                                               HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                               ServiceRoomCreates,
                                               param);
                                    if (createResult != null && createResult.Count > 0)
                                        success = true;
                                    AutoMapper.Mapper.CreateMap<HIS_SERVICE_RETY_CAT, HIS_SERVICE_RETY_CAT>();
                                    var vCreateResults = AutoMapper.Mapper.Map<List<HIS_SERVICE_RETY_CAT>, List<HIS_SERVICE_RETY_CAT>>(createResult);
                                    ServiceRetyCats.AddRange(vCreateResults);
                                }

                                lstReportRetyCatADOs = lstReportRetyCatADOs.OrderByDescending(p => p.check1).ToList();
                                if (ucGridControlReportRetyCat != null)
                                {
                                    ReportRetyCatProcessor.Reload(ucGridControlReportRetyCat, lstReportRetyCatADOs);
                                }
                            }
                        }
                    }

                    if (isChoseReportRetyCat == 2)
                    {
                        if (RetyIdCheckByRety == 0)
                        {
                            WaitingManager.Hide();
                            DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn nhóm báo cáo", "Thông báo");
                            return;
                        }

                        if (Service is List<HIS.UC.Service.ServiceADO>)
                        {
                            lstRoomServiceADOs = (List<HIS.UC.Service.ServiceADO>)Service;

                            if (lstRoomServiceADOs != null && lstRoomServiceADOs.Count > 0)
                            {
                                //List<long> listRoomServices = ServiceRoom.Select(p => p.ROOM_ID).ToList();

                                var dataChecked = lstRoomServiceADOs.Where(p => p.checkService == true).ToList();
                                //List xoa

                                var dataDelete = lstRoomServiceADOs.Where(o => ServiceRetyCatViews.Select(p => p.SERVICE_ID)
                                    .Contains(o.ID) && o.checkService == false).ToList();

                                //list them
                                var dataCreate = dataChecked.Where(o => !ServiceRetyCatViews.Select(p => p.SERVICE_ID)
                                    .Contains(o.ID)).ToList();

                                if (dataDelete.Count == 0 && dataChecked.Count == 0)
                                {
                                    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn nhóm loại báo cáo dịch vụ", "Thông báo");
                                    return;
                                }
                                if (dataChecked != null)
                                {
                                    success = true;
                                }

                                if (dataDelete != null && dataDelete.Count > 0)
                                {

                                    List<long> deleteId = ServiceRetyCatViews.Where(o => dataDelete.Select(p => p.ID)
                                        .Contains(o.SERVICE_ID)).Select(o => o.ID).ToList();
                                    bool deleteResult = new BackendAdapter(param).Post<bool>(
                                              "api/HisServiceRetyCat/DeleteList",
                                              HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                              deleteId,
                                              param);
                                    if (deleteResult)
                                        success = true;
                                    ServiceRetyCatViews = ServiceRetyCatViews.Where(o => !deleteId.Contains(o.ID)).ToList();
                                }

                                if (dataCreate != null && dataCreate.Count > 0)
                                {
                                    List<HIS_SERVICE_RETY_CAT> ServiceRoomCreate = new List<HIS_SERVICE_RETY_CAT>();
                                    foreach (var item in dataCreate)
                                    {
                                        HIS_SERVICE_RETY_CAT ServiceRoomID = new HIS_SERVICE_RETY_CAT();
                                        ServiceRoomID.REPORT_TYPE_CAT_ID = RetyIdCheckByRety;
                                        ServiceRoomID.SERVICE_ID = item.ID;
                                        ServiceRoomCreate.Add(ServiceRoomID);
                                    }

                                    var createResult = new BackendAdapter(param).Post<List<HIS_SERVICE_RETY_CAT>>(
                                               "/api/HisServiceRetyCat/CreateList",
                                               HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                               ServiceRoomCreate,
                                               param);
                                    if (createResult != null && createResult.Count > 0)
                                        success = true;
                                    AutoMapper.Mapper.CreateMap<HIS_SERVICE_RETY_CAT, HIS_SERVICE_RETY_CAT>();
                                    var vCreateResults = AutoMapper.Mapper.Map<List<HIS_SERVICE_RETY_CAT>, List<HIS_SERVICE_RETY_CAT>>(createResult);
                                    ServiceRetyCatViews.AddRange(vCreateResults);
                                }

                                lstRoomServiceADOs = lstRoomServiceADOs.OrderByDescending(p => p.checkService).ToList();
                                if (ucGridControlReportRetyCat != null)
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
                    FillDataToGridService(this);

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
                    FillDataToGridReportTypeCat(this);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void FindShortcutService()
        {
            try
            {
                btnFindService_Click(null, null);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void FindShortcutReportTypeCat()
        {
            try
            {
                btnFindReportTypeCat_Click(null, null);
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
                                    HisServiceRetyCatCopyByServiceSDO hisMestMatyCopyByMatySDO = new HisServiceRetyCatCopyByServiceSDO();
                                    hisMestMatyCopyByMatySDO.CopyServiceId = this.currentCopyServiceAdo.ID;
                                    hisMestMatyCopyByMatySDO.PasteServiceId = currentPaste.ID;
                                    var result = new BackendAdapter(param).Post<List<HIS_SERVICE_RETY_CAT>>("api/HisServiceRetyCat/CopyByService", ApiConsumer.ApiConsumers.MosConsumer, hisMestMatyCopyByMatySDO, param);
                                    if (result != null)
                                    {
                                        success = true;
                                        ServiceRetyCats = result;
                                        List<HIS.UC.ReportRetyCat.ReportRetyCatADO> dataNew = new List<HIS.UC.ReportRetyCat.ReportRetyCatADO>();
                                        dataNew = (from r in listReportRetyCat select new ReportRetyCatADO(r)).ToList();
                                        if (ServiceRetyCats != null && ServiceRetyCats.Count > 0)
                                        {
                                            foreach (var itemRoom in ServiceRetyCats)
                                            {
                                                var check = dataNew.FirstOrDefault(o => o.ID == itemRoom.REPORT_TYPE_CAT_ID);
                                                if (check != null)
                                                {
                                                    check.check1 = true;
                                                }
                                            }
                                        }
                                        dataNew = dataNew.OrderByDescending(p => p.check1).ToList();
                                        if (ucGridControlReportRetyCat != null)
                                        {
                                            ReportRetyCatProcessor.Reload(ucGridControlReportRetyCat, dataNew);
                                        }
                                        else
                                        {
                                            FillDataToGridReportTypeCat(this);
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

        private void ReportRetyCatGridView_MouseRightClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if ((e.Item is BarButtonItem) && sender != null && sender is HIS.UC.ReportRetyCat.ReportRetyCatADO)
                {
                    var type = (HIS.UC.ReportRetyCat.Popup.PopupMenuProcessor.ItemType)e.Item.Tag;
                    switch (type)
                    {
                        case HIS.UC.ReportRetyCat.Popup.PopupMenuProcessor.ItemType.Copy:
                            {
                                if (isChoseReportRetyCat != 2)
                                {
                                    MessageManager.Show("Vui lòng chọn nhóm báo cáo!");
                                    break;
                                }
                                this.currentCopyReportRetyCatAdo = (HIS.UC.ReportRetyCat.ReportRetyCatADO)sender;
                                break;
                            }
                        case HIS.UC.ReportRetyCat.Popup.PopupMenuProcessor.ItemType.Paste:
                            {
                                var currentPaste = (HIS.UC.ReportRetyCat.ReportRetyCatADO)sender;
                                bool success = false;
                                CommonParam param = new CommonParam();
                                if (this.currentCopyReportRetyCatAdo == null && isChoseReportRetyCat != 2)
                                {
                                    MessageManager.Show("Vui lòng copy!");
                                    break;
                                }
                                if (this.currentCopyReportRetyCatAdo != null && currentPaste != null && isChoseReportRetyCat == 2)
                                {
                                    if (this.currentCopyReportRetyCatAdo.ID == currentPaste.ID)
                                    {
                                        MessageManager.Show("Trùng dữ liệu copy và paste");
                                        break;
                                    }
                                    HisServiceRetyCatCopyByRetyCatSDO hisMestMatyCopyByMatySDO = new HisServiceRetyCatCopyByRetyCatSDO();
                                    hisMestMatyCopyByMatySDO.CopyReportTypeCatId = this.currentCopyReportRetyCatAdo.ID;
                                    hisMestMatyCopyByMatySDO.PasteReportTypeCatId = currentPaste.ID;
                                    var result = new BackendAdapter(param).Post<List<HIS_SERVICE_RETY_CAT>>("api/HisServiceRetyCat/CopyByRetyCat", ApiConsumer.ApiConsumers.MosConsumer, hisMestMatyCopyByMatySDO, param);
                                    if (result != null)
                                    {
                                        success = true;
                                        ServiceRetyCatViews = result;
                                        List<HIS.UC.Service.ServiceADO> dataNew = new List<HIS.UC.Service.ServiceADO>();
                                        dataNew = (from r in listService select new HIS.UC.Service.ServiceADO(r)).ToList();
                                        if (ServiceRetyCatViews != null && ServiceRetyCatViews.Count > 0)
                                        {

                                            foreach (var itemService in ServiceRetyCatViews)
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
                                            FillDataToGridService(this);
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

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                if (!btnImport.Enabled)
                    return;
                List<object> listArgs = new List<object>();
                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.HisImportServiceRetyCat", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);

            }
            catch (NullReferenceException ex)
            {
                WaitingManager.Hide();
                MessageBox.Show(MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNay), MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                MessageBox.Show(MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNay), MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void ImportShortcut()
        {
            try
            {
                btnImport_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void refreshForm()
        {
            btnFindService_Click(null, null);
            btnFindReportTypeCat_Click(null, null);
        }
    }
}







