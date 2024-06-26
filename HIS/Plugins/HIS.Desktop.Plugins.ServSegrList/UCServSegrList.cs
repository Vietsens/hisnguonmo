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
using HIS.UC.HisService;
using MOS.EFMODEL.DataModels;
using HIS.UC.ServiceGroup;
using HIS.UC.ServiceGroup.ADO;
using HIS.UC.HisService.ADO;
using Inventec.Core;
using MOS.Filter;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.ServSegrList.Entity;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Desktop.Common.Message;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

namespace HIS.Desktop.Plugins.ServSegrList
{
    public partial class UCServSegrList : UserControl
    {
        internal List<HIS.UC.ServiceGroup.ServiceGroupADO> GroupAdo { get; set; }
        internal List<HIS.UC.HisService.ServiceADO> serviceAdo { get; set; }
        List<HIS_SERVICE_TYPE> serviceType;
        int start = 0;
        int limit = 0;
        int rowCount = 0;
        int dataTotal = 0;
        int start1 = 0;
        int limit1 = 0;
        int rowCount1 = 0;
        int dataTotal1 = 0;
        long checkGroup = 0;
        long checkService = 0;
        UCServiceGroupProcessor groupProcessor = null;
        ServiceProcessor serviceProcessor = null;
        UserControl ucService;
        UserControl ucGroup;
        long isChoseService = 0;
        long isChoseGroup = 0;
        bool isCheckAll;
        List<V_HIS_SERVICE> listService = new List<V_HIS_SERVICE>();
        List<HIS_SERVICE_GROUP> listGroup = new List<HIS_SERVICE_GROUP>();
        List<V_HIS_SERV_SEGR> listGroupService = new List<V_HIS_SERV_SEGR>();
        public UCServSegrList()
        {
            InitializeComponent();
            InitGroup();
            InitService();
        }

        private void UCServSegrList_Load(object sender, EventArgs e)
        {
            try
            {
                LoadDataToCombo();
                LoadComboStatus();
                FillDataToGrid1(this);
                FillDataToGrid2(this);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitGroup()
        {
            try
            {
                groupProcessor = new UCServiceGroupProcessor();
                ServiceGroupInitADO ado = new ServiceGroupInitADO();
                ado.ListServiceGroupColumn = new List<UC.ServiceGroup.ServiceGroupColumn>();
                ado.btn_Radio_Enable_Click1 = btn_Radio_Enable_Click_Group;
                ado.gridViewServiceGroup_MouseDownMest = MouseDownGroup;

                ServiceGroupColumn colRadio2 = new ServiceGroupColumn("   ", "radioGroup", 30, true);
                colRadio2.VisibleIndex = 0;
                colRadio2.Visible = false;
                colRadio2.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListServiceGroupColumn.Add(colRadio2);

                ServiceGroupColumn colCheck2 = new ServiceGroupColumn("   ", "checkGroup", 30, true);
                colCheck2.VisibleIndex = 1;
                colCheck2.image = img.Images[0];
                colCheck2.Caption = "Chọn tất cả";
                colCheck2.Visible = false;
                colCheck2.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListServiceGroupColumn.Add(colCheck2);

                ServiceGroupColumn colMaPhong = new ServiceGroupColumn("Mã nhóm", "SERVICE_GROUP_CODE", 80, false);
                colMaPhong.VisibleIndex = 2;
                ado.ListServiceGroupColumn.Add(colMaPhong);

                ServiceGroupColumn colTenPhong = new ServiceGroupColumn("Tên nhóm", "SERVICE_GROUP_NAME", 150, false);
                colTenPhong.VisibleIndex = 3;
                ado.ListServiceGroupColumn.Add(colTenPhong);



                this.ucGroup = (UserControl)groupProcessor.Run(ado);
                if (ucGroup != null)
                {
                    this.panelControl1.Controls.Add(this.ucGroup);
                    this.ucGroup.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void InitService()
        {
            try
            {
                serviceProcessor = new ServiceProcessor();
                HisServiceInitADO ado = new HisServiceInitADO();
                ado.ServiceColumns = new List<UC.HisService.ServiceColumn>();
                ado.gridViewService_MouseDown = MouseDownService;
                ado.btnRadioEnable_Click = btn_Radio_Enable_Click_Service;

                ServiceColumn colRadio1 = new ServiceColumn("   ", "radio2", 30, true,false);
                colRadio1.VisibleIndex = 0;
                colRadio1.Visible = false;
                colRadio1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ServiceColumns.Add(colRadio1);

                ServiceColumn colCheck1 = new ServiceColumn("   ", "check2", 30, true,false);
                colCheck1.VisibleIndex = 1;
                colCheck1.Visible = false;
                colCheck1.image = img.Images[0];
                colCheck1.Caption = "Chọn tất cả";
                //colCheck1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                colCheck1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ServiceColumns.Add(colCheck1);

                ServiceColumn colMaPhong = new ServiceColumn("Mã dịch vụ", "SERVICE_CODE", 100, false,true);
                colMaPhong.VisibleIndex = 2;
                ado.ServiceColumns.Add(colMaPhong);

                ServiceColumn colTenPhong = new ServiceColumn("Tên dịch vụ", "SERVICE_NAME", 200, true,true);
                colTenPhong.VisibleIndex = 3;
                ado.ServiceColumns.Add(colTenPhong);


                ServiceColumn colSL = new ServiceColumn("Số lượng", "AMOUNT_STR", 100, true, true);
                colSL.VisibleIndex = 4;
                ado.ServiceColumns.Add(colSL);

                ServiceColumn colHaoPhi = new ServiceColumn("Hao phí", "IS_EXPEND", 100, true, true);
                colHaoPhi.VisibleIndex = 6;
                colHaoPhi.Visible = false;
                //colCheck1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                colCheck1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ServiceColumns.Add(colHaoPhi);

                this.ucService = (UserControl)serviceProcessor.Run(ado);
                if (ucService != null)
                {
                    this.panelControl2.Controls.Add(this.ucService);
                    this.ucService.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void LoadDataToCombo()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SERVICE_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("SERVICE_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_TYPE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboGroupService, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SERVICE_TYPE>(), controlEditorADO);
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
                status.Add(new Status(1, "Nhóm dịch vụ"));
                status.Add(new Status(2, "Dịch vụ"));

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("statusName", "", 300, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("statusName", "id", columnInfos, false, 350);
                ControlEditorLoader.Load(cboStatus, status, controlEditorADO);
                cboStatus.EditValue = status[0].id;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void FillDataToGroup(UCServSegrList _Group)
        {
            try
            {
                int numPageSize = 0;
                if (ucPaging1.pagingGrid != null)
                {
                    numPageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = (int)ConfigApplications.NumPageSize;
                }
                FillDataToGridGroup(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridGroup, param);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void FillDataToGridGroup(object data)
        {
            try
            {
                WaitingManager.Show();
                listGroup = new List<HIS_SERVICE_GROUP>();
                int start = ((CommonParam)data).Start ?? 0;
                int limit = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start, limit);
                MOS.Filter.HisServiceGroupFilter groupFilter = new MOS.Filter.HisServiceGroupFilter();
                groupFilter.ORDER_FIELD = "SERVICE_ROUP";
                groupFilter.ORDER_DIRECTION = "ASC";
                groupFilter.KEY_WORD = txtKeyword1.Text;

                //if (cboGroupService.EditValue != null)
                //{
                //    groupFilter.ID = (long)cboGroupService.EditValue;
                //}
                //long isChoseMety = 0;
                if ((long)cboStatus.EditValue == 1)
                {
                    isChoseGroup = (long)cboStatus.EditValue;
                }
                if ((long)cboStatus.EditValue == 2)
                {
                    isChoseGroup = (long)cboStatus.EditValue;
                }
                var rs = new BackendAdapter(param).GetRO<List<HIS_SERVICE_GROUP>>(
                    "api/HisServiceGroup/Get",
                    ApiConsumers.MosConsumer,
                    groupFilter,
                    param);

                GroupAdo = new List<HIS.UC.ServiceGroup.ServiceGroupADO>();
                if (rs != null && rs.Data.Count > 0)
                {
                    //List<V_HIS_ROOM> dataByRoom = new List<V_HIS_ROOM>();
                    listGroup = rs.Data;
                    foreach (var item in listGroup)
                    {
                        //HIS.UC.RoomTypeList.RoomTypeListADO RoomtypeADO = new HIS.UC.RoomTypeList.RoomTypeListADO(item);
                        HIS.UC.ServiceGroup.ServiceGroupADO medistockADO = new HIS.UC.ServiceGroup.ServiceGroupADO(item);
                        if (isChoseService == 1)
                        {
                            medistockADO.isKeyChooseGroup = true;
                            //btnCheckAll1.Enabled = false;
                        }
                        GroupAdo.Add(medistockADO);
                    }
                }
                if (listGroupService != null && listGroupService.Count > 0)
                {

                    foreach (var itemGroup in listGroupService)
                    {
                        var check = GroupAdo.FirstOrDefault(o => o.ID == itemGroup.SERVICE_GROUP_ID);
                        if (check != null)
                        {
                            check.checkGroup = true;
                        }
                    }
                }

                GroupAdo = GroupAdo.OrderByDescending(p => p.checkGroup).ToList();
                if (ucGroup != null)
                {
                    groupProcessor.Reload(ucGroup, GroupAdo);
                }
                rowCount = (data == null ? 0 : GroupAdo.Count);
                dataTotal = (rs.Param == null ? 0 : rs.Param.Count ?? 0);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void FillDataToGrid1(UCServSegrList _Group)
        {
            try
            {
                int numPageSize = 0;
                if (ucPaging1.pagingGrid != null)
                {
                    numPageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = (int)ConfigApplications.NumPageSize;
                }
                FillDataToGridGroup(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridGroup, param);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void FillDataToMety(UCServSegrList _Service)
        {
            try
            {
                int numPageSize = 0;
                if (ucPaging2.pagingGrid != null)
                {
                    numPageSize = ucPaging2.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = (int)ConfigApplications.NumPageSize;
                }
                FillDataToGridService(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging2.Init(FillDataToGridService, param);
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
                MOS.Filter.HisServiceViewFilter serviceFilter = new MOS.Filter.HisServiceViewFilter();
                serviceFilter.ORDER_FIELD = "SERVICE_NAME";
                serviceFilter.ORDER_DIRECTION = "ASC";
                serviceFilter.KEY_WORD = txtKeyword2.Text;

                if (cboGroupService.EditValue != null)
                    serviceFilter.SERVICE_TYPE_ID = (long)cboGroupService.EditValue;
                long isChoseStock = 0;
                if ((long)cboStatus.EditValue == 2)
                {
                    isChoseService = (long)cboStatus.EditValue;
                }
                if ((long)cboStatus.EditValue == 1)
                {
                    isChoseService = (long)cboStatus.EditValue;
                }
                var rs = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE>>(
                    "api/HisService/GetView",
                    HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                    serviceFilter,
                    param);

                serviceAdo = new List<HIS.UC.HisService.ServiceADO>();
                if (rs != null && rs.Data.Count > 0)
                {
                    listService = rs.Data;
                    foreach (var item in listService)
                    {
                        HIS.UC.HisService.ServiceADO metypeADO = new HIS.UC.HisService.ServiceADO(item);
                        if (isChoseStock == 2)
                        {
                            metypeADO.isKeyChoose1 = true;
                            //btnCheckAll2.Enabled = false;
                        }
                        serviceAdo.Add(metypeADO);
                    }
                }
                if (listGroupService != null && listGroupService.Count > 0)
                {
                    foreach (var item in listGroupService)
                    {
                        var mediStockMety = serviceAdo.FirstOrDefault(o => o.ID == item.SERVICE_ID);
                        if (mediStockMety != null)
                        {
                            mediStockMety.check2 = true;
                            mediStockMety.AMOUNT_STR = item.AMOUNT;
                            mediStockMety.IS_EXPEND = item.IS_EXPEND == 1 ? true : false;
                        }
                    }
                }
                serviceAdo = serviceAdo.OrderByDescending(p => p.check2).ToList();
                if (ucService != null)
                {
                    serviceProcessor.Reload(ucService, serviceAdo);
                }
                rowCount = (data == null ? 0 : serviceAdo.Count);
                dataTotal = (rs.Param == null ? 0 : rs.Param.Count ?? 0);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void FillDataToGrid2(UCServSegrList _Service)
        {
            try
            {
                int numPageSize = 0;
                if (ucPaging2.pagingGrid != null)
                {
                    numPageSize = ucPaging2.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = (int)ConfigApplications.NumPageSize;
                }
                FillDataToGridService(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging2.Init(FillDataToGridService, param);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSearch1_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToGrid1(this);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSearch2_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToGrid2(this);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void btn_Radio_Enable_Click_Group(HIS_SERVICE_GROUP data)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                MOS.Filter.HisServSegrFilter filter = new HisServSegrFilter();
                filter.SERVICE_GROUP_ID = data.ID;
                checkGroup = data.ID;
                listGroupService = new List<V_HIS_SERV_SEGR>();
                listGroupService = new BackendAdapter(param).Get<List<V_HIS_SERV_SEGR>>(
                                "api/HisServSegr/GetView",
                                HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                filter,
                                param);
                List<HIS.UC.HisService.ServiceADO> dataNew = new List<HIS.UC.HisService.ServiceADO>();
                dataNew = (from r in listService select new HIS.UC.HisService.ServiceADO(r)).ToList();
                if (listGroupService != null && listGroupService.Count > 0)
                {
                    foreach (var item in listGroupService)
                    {
                        var groupService = dataNew.FirstOrDefault(o => o.ID == item.SERVICE_ID);
                        if (groupService != null)
                        {
                            groupService.check2 = true;
                            groupService.AMOUNT_STR = item.AMOUNT;
                            groupService.IS_EXPEND = item.IS_EXPEND == 1 ? true : false;
                        }
                    }

                    dataNew = dataNew.OrderByDescending(p => p.check2).ToList();
                    if (ucService != null)
                    {
                        serviceProcessor.Reload(ucService, dataNew);
                    }
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
        private void btn_Radio_Enable_Click_Service(V_HIS_SERVICE data)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                MOS.Filter.HisServSegrFilter filter = new HisServSegrFilter();
                filter.SERVICE_ID = data.ID;
                checkService = data.ID;
                //filter.
                listGroupService = new List<V_HIS_SERV_SEGR>();
                listGroupService = new BackendAdapter(param).Get<List<V_HIS_SERV_SEGR>>(
                                "api/HisServSerg/GetView",
                                HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                filter,
                                param);
                List<HIS.UC.ServiceGroup.ServiceGroupADO> dataNew = new List<HIS.UC.ServiceGroup.ServiceGroupADO>();
                dataNew = (from r in listGroup select new HIS.UC.ServiceGroup.ServiceGroupADO(r)).ToList();
                if (listGroupService != null && listGroupService.Count > 0)
                {

                    foreach (var itemStock in listGroupService)
                    {
                        var check = dataNew.FirstOrDefault(o => o.ID == itemStock.SERVICE_GROUP_ID);
                        if (check != null)
                        {
                            check.checkGroup = true;
                        }
                    }

                    dataNew = dataNew.OrderByDescending(p => p.checkGroup).ToList();
                    if (ucGroup != null)
                    {
                        groupProcessor.Reload(ucGroup, dataNew);
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

        private void MouseDownGroup(object sender, MouseEventArgs e)
        {
            try
            {
                if (isChoseGroup == 1)
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
                        if (hi.Column.FieldName == "checkGroup")
                        {
                            var lstCheckAll = GroupAdo;
                            List<HIS.UC.ServiceGroup.ServiceGroupADO> lstChecks = new List<HIS.UC.ServiceGroup.ServiceGroupADO>();

                            if (lstCheckAll != null && lstCheckAll.Count > 0)
                            {
                                var roomCheckedNum = GroupAdo.Where(o => o.checkGroup == true).Count();
                                var roomNum = GroupAdo.Count();
                                if ((roomCheckedNum > 0 && roomCheckedNum < roomNum) || roomCheckedNum == 0)
                                {
                                    isCheckAll = true;
                                    hi.Column.Image = img.Images[0];
                                }

                                if (roomCheckedNum == roomNum)
                                {
                                    isCheckAll = false;
                                    hi.Column.Image = img.Images[1];
                                }
                                if (isCheckAll)
                                {
                                    foreach (var item in lstCheckAll)
                                    {
                                        if (item.ID != null)
                                        {
                                            item.checkGroup = true;
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
                                            item.checkGroup = false;
                                            lstChecks.Add(item);
                                        }
                                        else
                                        {
                                            lstChecks.Add(item);
                                        }
                                    }
                                    isCheckAll = true;
                                }

                                groupProcessor.Reload(ucGroup, lstChecks);
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
        private void MouseDownService(object sender, MouseEventArgs e)
        {
            try
            {
                if (isChoseService == 2)
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
                        if (hi.Column.FieldName == "check2")
                        {
                            var lstCheckAll = serviceAdo;
                            List<HIS.UC.HisService.ServiceADO> lstChecks = new List<HIS.UC.HisService.ServiceADO>();

                            if (lstCheckAll != null && lstCheckAll.Count > 0)
                            {
                                var roomCheckedNum = serviceAdo.Where(o => o.check2 == true).Count();
                                var roomNum = serviceAdo.Count();
                                if ((roomCheckedNum > 0 && roomCheckedNum < roomNum) || roomCheckedNum == 0)
                                {
                                    isCheckAll = true;
                                    hi.Column.Image = img.Images[0];
                                }

                                if (roomCheckedNum == roomNum)
                                {
                                    isCheckAll = false;
                                    hi.Column.Image = img.Images[1];
                                }
                                if (isCheckAll)
                                {
                                    foreach (var item in lstCheckAll)
                                    {
                                        if (item.ID != null)
                                        {
                                            item.check2 = true;
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
                                            item.check2 = false;
                                            lstChecks.Add(item);
                                        }
                                        else
                                        {
                                            lstChecks.Add(item);
                                        }
                                    }
                                    isCheckAll = true;
                                }

                                serviceProcessor.Reload(ucService, lstChecks);
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (ucService != null && ucGroup != null)
                {
                    bool success = false;
                    CommonParam param = new CommonParam();
                    List<HIS.UC.HisService.ServiceADO> services = serviceProcessor.GetDataGridView(ucService) as List<HIS.UC.HisService.ServiceADO>;
                    List<HIS.UC.ServiceGroup.ServiceGroupADO> groups = groupProcessor.GetDataGridView(ucGroup) as List<HIS.UC.ServiceGroup.ServiceGroupADO>;
                    if (isChoseGroup == 1 && services != null && services.Count > 0)
                    {
                        if (services != null && services.Count > 0)
                        {
                            var dataCheckeds = services.Where(p => (p.check2 == true)).ToList();

                            //List xóa
                            var dataDeletes = services.Where(o => listGroupService.Select(p => p.SERVICE_ID)
                           .Contains(o.ID) && o.check2 == false).ToList();

                            //list them
                            var dataCreates = dataCheckeds.Where(o => !listGroupService.Select(p => p.SERVICE_ID)
                                .Contains(o.ID)).ToList();

                            // List update
                            var dataUpdate = dataCheckeds.Where(o => listGroupService.Select(p => p.SERVICE_ID)
                                .Contains(o.ID)).ToList();

                            if (dataUpdate != null && dataUpdate.Count > 0)
                            {
                                // List<HIS_MEDI_STOCK_METY> stockMetyUpdates = new List<HIS_MEDI_STOCK_METY>();
                                var serviceGroupUpdates = new List<V_HIS_SERV_SEGR>();
                                foreach (var item in dataUpdate)
                                {
                                    var serviceGroup = listGroupService.FirstOrDefault(o => o.SERVICE_ID == item.ID && o.SERVICE_GROUP_ID == checkGroup);
                                    if (serviceGroup != null)
                                    {
                                        serviceGroup.AMOUNT = item.AMOUNT_STR;
                                        serviceGroup.IS_EXPEND = short.Parse(item.IS_EXPEND == true ? "1" : "0");
                                        serviceGroupUpdates.Add(serviceGroup);
                                    }
                                }
                                if (serviceGroupUpdates != null && serviceGroupUpdates.Count > 0)
                                {
                                    //var updateResult = new BackendAdapter(param).Post<List<V_HIS_SERV_SEGR>>(
                                    //           "/api/HisMediStockMety/UpdateList",
                                    //           HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                    //           serviceGroupUpdates,
                                    //           param);
                                    //if (updateResult != null && updateResult.Count > 0)
                                    //{
                                    //    listGroupService.AddRange(updateResult);
                                    //    success = true;
                                    //}
                                }
                            }

                            if (dataDeletes != null && dataDeletes.Count > 0)// && dataDeletes.Count < 15)
                            {
                                //List<long> deleteIds = listGroupService.Where(o => dataDeletes.Select(p => p.ID)
                                //    .Contains(o.SERVICE_ID)).Select(o => o.ID).ToList();
                                //bool deleteResult = new BackendAdapter(param).Post<bool>(
                                //          "/api/HisMediStockMety/DeleteList",
                                //          HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                //          deleteIds,
                                //          param);
                                //if (deleteResult)
                                //{
                                //    listGroupService = listGroupService.Where(o => !deleteIds.Contains(o.ID)).ToList();
                                //    success = true;
                                //}
                            }

                            if (dataCreates != null && dataCreates.Count > 0)
                            {
                                List<V_HIS_SERV_SEGR> serviceGroupCreates = new List<V_HIS_SERV_SEGR>();
                                foreach (var item in dataCreates)
                                {
                                    V_HIS_SERV_SEGR serviceGroup = new V_HIS_SERV_SEGR();
                                    serviceGroup.SERVICE_ID = item.ID;
                                    serviceGroup.SERVICE_GROUP_ID = checkGroup;
                                    serviceGroup.AMOUNT = item.AMOUNT_STR;
                                    serviceGroup.IS_EXPEND = short.Parse(item.IS_EXPEND == true ? "1" : "0");
                                    serviceGroupCreates.Add(serviceGroup);
                                }

                                //var createResult = new BackendAdapter(param).Post<List<V_HIS_SERV_SEGR>>(
                                //           "/api/HisMediStockMety/CreateList",
                                //           HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                //           serviceGroupCreates,
                                //           param);
                                //if (createResult != null && createResult.Count > 0)
                                //{
                                //    listGroupService.AddRange(createResult);
                                //    success = true;
                                //}
                            }
                            services = services.OrderByDescending(p => p.check2).ToList();
                            serviceProcessor.Reload(ucService, services);
                        }
                    }

                    if (isChoseService == 2 && groups != null && groups.Count > 0)
                    {
                        HIS.UC.HisService.ServiceADO serviceType = services.FirstOrDefault(o => o.ID == checkService);
                        var dataCheckeds = groups.Where(p => (p.checkGroup == true)).ToList();

                        //List xóa
                        var dataDeletes = groups.Where(o => listGroupService.Select(p => p.SERVICE_GROUP_ID)
                       .Contains(o.ID) && o.checkGroup == false).ToList();

                        //list them
                        var dataCreates = dataCheckeds.Where(o => !listGroupService.Select(p => p.SERVICE_GROUP_ID)
                            .Contains(o.ID)).ToList();

                        //list update
                        var dataUpdates = dataCheckeds.Where(o => listGroupService.Select(p => p.SERVICE_GROUP_ID)
                            .Contains(o.ID)).ToList();

                        if (dataDeletes != null && dataDeletes.Count > 0)// && dataDeletes.Count < 5)
                        {
                            List<long> deleteIds = listGroupService.Where(o => dataDeletes.Select(p => p.ID)
                                .Contains(o.SERVICE_GROUP_ID)).Select(o => o.ID).ToList();
                            //bool deleteResult = new BackendAdapter(param).Post<bool>(
                            //          "/api/HisMediStockMety/DeleteList",
                            //          HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                            //          deleteIds,
                            //          param);
                            //if (deleteResult)
                            //{
                            //    listGroupService = listGroupService.Where(o => !deleteIds.Contains(o.ID)).ToList();
                            //    success = true;
                            //}

                        }

                        if (dataUpdates != null && dataUpdates.Count > 0 && serviceType != null)
                        {
                            // List<HIS_MEDI_STOCK_METY> stockMetyUpdates = new List<HIS_MEDI_STOCK_METY>();
                            var stockMetyUpdates = new List<V_HIS_SERV_SEGR>();
                            foreach (var item in dataUpdates)
                            {
                                var serviceGroup = listGroupService.FirstOrDefault(o => o.SERVICE_GROUP_ID == item.ID && o.SERVICE_ID == checkService);
                                if (serviceGroup != null)
                                {
                                    serviceGroup.AMOUNT = serviceType.AMOUNT_STR;
                                    serviceGroup.IS_EXPEND = short.Parse(serviceType.IS_EXPEND == true ? "1" : "0");
                                    stockMetyUpdates.Add(serviceGroup);
                                }
                            }
                            if (stockMetyUpdates != null && stockMetyUpdates.Count > 0)
                            {
                                //var updateResult = new BackendAdapter(param).Post<List<V_HIS_SERV_SEGR>>(
                                //           "/api/HisMediStockMety/UpdateList",
                                //           HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                //           stockMetyUpdates,
                                //           param);
                                //if (updateResult != null && updateResult.Count > 0)
                                //{
                                //    listGroupService.AddRange(updateResult);
                                //    success = true;
                                //}
                            }
                        }

                        if (dataCreates != null && dataCreates.Count > 0 && serviceType != null)
                        {
                            List<V_HIS_SERV_SEGR> serviceGroupCreates = new List<V_HIS_SERV_SEGR>();
                            foreach (var item in dataCreates)
                            {
                                V_HIS_SERV_SEGR serviceGroup = new V_HIS_SERV_SEGR();
                                serviceGroup.SERVICE_GROUP_ID = item.ID;
                                serviceGroup.SERVICE_ID = checkService;
                                serviceGroup.AMOUNT = serviceType.AMOUNT_STR;
                                serviceGroup.IS_EXPEND = short.Parse(serviceType.IS_EXPEND == true ? "1" : "0");
                                serviceGroupCreates.Add(serviceGroup);
                            }

                            //var createResult = new BackendAdapter(param).Post<List<V_HIS_SERV_SEGR>>(
                            //           "/api/HisMediStockMety/CreateList",
                            //           HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                            //           serviceGroupCreates,
                            //           param);
                            //if (createResult != null && createResult.Count > 0)
                            //{
                            //    listGroupService.AddRange(createResult);
                            //    success = true;
                            //}

                        }
                        groups = groups.OrderByDescending(p => p.checkGroup).ToList();
                        groupProcessor.Reload(ucGroup, groups);
                    }
                    MessageManager.Show(this.ParentForm, param, success);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboStatus_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                isChoseGroup = 0;
                isChoseService = 0;
                listGroupService = new List<V_HIS_SERV_SEGR>();
                FillDataToGrid1(this);
                FillDataToGrid2(this);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeyword1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch1.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeyword2_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch2.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
