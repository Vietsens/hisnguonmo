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
using Inventec.Desktop.Common.Message;
using HIS.Desktop.Plugins.ListTestIndexMap.entity;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraEditors;
using HIS.Desktop.LocalStorage.ConfigApplication;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using HIS.UC.TestIndex;
using HIS.UC.MachineIndex;
using LIS.EFMODEL.DataModels;
using HIS.UC.TestIndex.ADO;
using HIS.UC.MachineIndex.ADO;
using LIS.Filter;

namespace HIS.Desktop.Plugins.ListTestIndexMap
{
    public partial class UCListTestIndexMap : HIS.Desktop.Utility.UserControlBase
    {
        UCTestIndexProcessor TestIndexProcessor;
        UCMachineIndexProcessor MachineIndexProcessor;
        UserControl ucGridControlTestIndex;
        UserControl ucGridControlMachineIndex;
        int start = 0;
        int limit = 0;
        int rowCount = 0;
        int dataTotal = 0;
        int start1 = 0;
        int limit1 = 0;
        int rowCount1 = 0;
        int dataTotal1 = 0;
        internal List<HIS.UC.TestIndex.TestIndexADO> lstTestIndexADOs { get; set; }
        internal List<HIS.UC.MachineIndex.MachineIndexADO> lstMachineIndexADOs { get; set; }
        List<V_HIS_TEST_INDEX> listTestIndex;
        List<LIS_MACHINE_INDEX> listMachineIndex;
        string TestIndexIdCheckByTestIndex;
        string TestIndexNameCheckByTestIndex;
        long MachineIndexIdCheck = 0;
        long isChooseTestIndex;
        long isChooseMachineIndex;
        bool isCheckAll;
        bool statecheckColumn;
        List<V_LIS_TEST_INDEX_MAP> mestTestIndexs { get; set; }
        List<LIS_TEST_INDEX_MAP> TestIndexMachineIndexs { get; set; }
        List<LIS_MACHINE> listMachine;

        public UCListTestIndexMap(Inventec.Desktop.Common.Modules.Module _moduleData)
            : base(_moduleData)
        {
            InitializeComponent();
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

        private void UCListTestIndexMap_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();

                //GetLisMachine();
                LoadComboStatus();
                InitUCgridTestIndex();
                InitUCgridMachineIndex();
                FillDataToGridTestIndex(this);
                FillDataToGridMachineIndex(this);
                SetCaptionByLanguageKey();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetLisMachine()
        {
            try
            {
                CommonParam param = new CommonParam();
                LisMachineFilter filter = new LisMachineFilter();
                filter.IS_ACTIVE = 1;

                var rsMachine = new BackendAdapter(param).Get<List<LIS_MACHINE>>("api/LisMachine/Get", ApiConsumers.LisConsumer, filter, param);
                if (rsMachine != null && rsMachine.Count > 0)
                    listMachine = rsMachine;
                else
                    listMachine = new List<LIS_MACHINE>();
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
                status.Add(new Status(1, "Chỉ số xét nghiệm"));
                status.Add(new Status(2, "Chỉ số máy XN"));

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("statusName", "", 300, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("statusName", "id", columnInfos, false, 350);
                ControlEditorLoader.Load(cboChoose, status, controlEditorADO);
                cboChoose.EditValue = status[1].id;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridMachineIndex(UCListTestIndexMap uCListTestIndexMap)
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
                    numPageSize = (int)ConfigApplications.NumPageSize;
                }

                FillDataToGridMachineIndexPage(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount1;
                param.Count = dataTotal1;
                ucPaging1.Init(FillDataToGridMachineIndexPage, param, numPageSize);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridMachineIndexPage(object data)
        {
            try
            {
                WaitingManager.Show();
                listMachineIndex = new List<LIS_MACHINE_INDEX>();
                int start1 = ((CommonParam)data).Start ?? 0;
                int limit1 = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start1, limit1);
                LisMachineIndexFilter MachineIndexFilter = new LisMachineIndexFilter();
                MachineIndexFilter.KEY_WORD = txtKeyword1.Text;
                MachineIndexFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                MachineIndexFilter.ORDER_DIRECTION = "DESC";
                MachineIndexFilter.ORDER_FIELD = "MODIFY_TIME";

                if ((long)cboChoose.EditValue == 2)
                {
                    isChooseMachineIndex = (long)cboChoose.EditValue;
                }

                var mest = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<LIS_MACHINE_INDEX>>(
                    "api/LisMachineIndex/Get",
                    HIS.Desktop.ApiConsumer.ApiConsumers.LisConsumer,
                    MachineIndexFilter,
                    param);

                lstMachineIndexADOs = new List<MachineIndexADO>();
                if (mest != null && mest.Data.Count > 0)
                {
                    listMachineIndex = mest.Data;
                    foreach (var item in listMachineIndex)
                    {
                        MachineIndexADO MachineIndexADO = new MachineIndexADO(item);
                        if (isChooseMachineIndex == 2)
                        {
                            MachineIndexADO.isKeyChooseMachine = true;
                        }
                        lstMachineIndexADOs.Add(MachineIndexADO);
                    }
                }

                if (mestTestIndexs != null && mestTestIndexs.Count > 0)
                {
                    foreach (var itemUsername in mestTestIndexs)
                    {
                        var check = lstMachineIndexADOs.FirstOrDefault(o => o.ID == itemUsername.MACHINE_INDEX_ID);
                        if (check != null)
                        {
                            check.checkMachine = true;
                        }
                    }
                }
                lstMachineIndexADOs = lstMachineIndexADOs.OrderByDescending(p => p.checkMachine).ToList();

                if (ucGridControlMachineIndex != null)
                {
                    MachineIndexProcessor.Reload(ucGridControlMachineIndex, lstMachineIndexADOs);
                }
                rowCount1 = (data == null ? 0 : lstMachineIndexADOs.Count);
                dataTotal1 = (mest.Param == null ? 0 : mest.Param.Count ?? 0);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridTestIndex(UCListTestIndexMap uCListTestIndexMap)
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
                    numPageSize = (int)ConfigApplications.NumPageSize;
                }

                FillDataToGridTestIndexPage(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging2.Init(FillDataToGridTestIndexPage, param, numPageSize);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridTestIndexPage(object data)
        {
            try
            {
                WaitingManager.Show();
                listTestIndex = new List<V_HIS_TEST_INDEX>();
                int start = ((CommonParam)data).Start ?? 0;
                int limit = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start, limit);
                MOS.Filter.HisTestIndexViewFilter TestIndexFilter = new HisTestIndexViewFilter();
                TestIndexFilter.KEY_WORD = txtKeyword2.Text;
                TestIndexFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                TestIndexFilter.ORDER_DIRECTION = "DESC";
                TestIndexFilter.ORDER_FIELD = "MODIFY_TIME";

                if ((long)cboChoose.EditValue == 1)
                {
                    isChooseTestIndex = (long)cboChoose.EditValue;
                }

                var rs = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_TEST_INDEX>>(
                    "api/HisTestIndex/GetView",
                    HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                    TestIndexFilter,
                    param);

                lstTestIndexADOs = new List<TestIndexADO>();
                if (rs != null && rs.Data.Count > 0)
                {
                    listTestIndex = rs.Data;
                    foreach (var item in listTestIndex)
                    {
                        TestIndexADO TestIndexADO = new TestIndexADO(item);
                        if (isChooseTestIndex == 1)
                        {
                            TestIndexADO.isKeyChoose = true;
                        }
                        lstTestIndexADOs.Add(TestIndexADO);
                    }
                }

                if (TestIndexMachineIndexs != null && TestIndexMachineIndexs.Count > 0)
                {
                    foreach (var itemUsername in TestIndexMachineIndexs)
                    {
                        var check = lstTestIndexADOs.FirstOrDefault(o => o.TEST_INDEX_CODE == itemUsername.TEST_INDEX_CODE);
                        if (check != null)
                        {
                            check.check1 = true;
                        }
                    }
                }

                lstTestIndexADOs = lstTestIndexADOs.OrderByDescending(p => p.check1).ToList();

                if (ucGridControlTestIndex != null)
                {
                    TestIndexProcessor.Reload(ucGridControlTestIndex, lstTestIndexADOs);
                }
                rowCount = (data == null ? 0 : lstTestIndexADOs.Count);
                dataTotal = (rs.Param == null ? 0 : rs.Param.Count ?? 0);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitUCgridMachineIndex()
        {
            try
            {
                MachineIndexProcessor = new UCMachineIndexProcessor();
                MachineIndexInitADO ado = new MachineIndexInitADO();
                ado.ListMachineIndexColumn = new List<MachineIndexColumn>();
                ado.gridViewMachineIndex_MouseDownMest = gridViewMachineIndex_MouseDownMest;
                ado.btn_Radio_Enable_Click1 = btn_Radio_Enable_Click1;
                //ado.MachineIndexGrid_CustomUnboundColumnData = CustomUnboundColumnDataMachine;

                MachineIndexColumn colRadio2 = new MachineIndexColumn("   ", "radioMachine", 30, true);
                colRadio2.VisibleIndex = 0;
                colRadio2.Visible = false;
                colRadio2.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListMachineIndexColumn.Add(colRadio2);

                MachineIndexColumn colCheck2 = new MachineIndexColumn("   ", "checkMachine", 30, true);
                colCheck2.VisibleIndex = 1;
                colCheck2.Visible = false;
                colCheck2.image = imageCollectionMachineIndex.Images[0];
                colCheck2.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListMachineIndexColumn.Add(colCheck2);

                MachineIndexColumn colMaKhoXuat = new MachineIndexColumn("Mã chỉ số máy", "MACHINE_INDEX_CODE", 60, false);
                colMaKhoXuat.VisibleIndex = 2;
                ado.ListMachineIndexColumn.Add(colMaKhoXuat);

                MachineIndexColumn colTenKhoXuat = new MachineIndexColumn("Tên chi số máy", "MACHINE_INDEX_NAME", 100, false);
                colTenKhoXuat.VisibleIndex = 3;
                ado.ListMachineIndexColumn.Add(colTenKhoXuat);

                MachineIndexColumn colMayXetNghiem = new MachineIndexColumn("Máy xét nghiệm", "MACHINE_NAME", 100, false, DevExpress.Data.UnboundColumnType.Object);
                colMayXetNghiem.VisibleIndex = 4;
                ado.ListMachineIndexColumn.Add(colMayXetNghiem);

                MachineIndexColumn colNguoiTao = new MachineIndexColumn("Người tạo", "CREATOR", 100, false);
                colNguoiTao.VisibleIndex = 5;
                ado.ListMachineIndexColumn.Add(colNguoiTao);

                MachineIndexColumn colNguoiSua = new MachineIndexColumn("Người sửa", "MODIFIER", 100, false);
                colNguoiSua.VisibleIndex = 6;
                ado.ListMachineIndexColumn.Add(colNguoiSua);

                this.ucGridControlMachineIndex = (UserControl)MachineIndexProcessor.Run(ado);

                if (ucGridControlMachineIndex != null)
                {
                    this.panelControlMachineIndex.Controls.Add(this.ucGridControlMachineIndex);
                    this.ucGridControlMachineIndex.Dock = DockStyle.Fill;
                }

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMachineIndex_MouseDownMest(object sender, MouseEventArgs e)
        {
            try
            {
                if (isChooseMachineIndex == 2)
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
                        if (hi.Column.FieldName == "checkMachine")
                        {
                            var lstCheckAll = lstMachineIndexADOs;
                            List<HIS.UC.MachineIndex.MachineIndexADO> lstChecks = new List<HIS.UC.MachineIndex.MachineIndexADO>();

                            if (lstCheckAll != null && lstCheckAll.Count > 0)
                            {
                                var MachineIndexCheckedNum = lstMachineIndexADOs.Where(o => o.checkMachine == true).Count();
                                var MachineIndexNum = lstMachineIndexADOs.Count();
                                if ((MachineIndexCheckedNum > 0 && MachineIndexCheckedNum < MachineIndexNum) || MachineIndexCheckedNum == 0)
                                {
                                    isCheckAll = true;
                                    hi.Column.Image = imageCollectionMachineIndex.Images[1];
                                }

                                if (MachineIndexCheckedNum == MachineIndexNum)
                                {
                                    isCheckAll = false;
                                    hi.Column.Image = imageCollectionMachineIndex.Images[0];
                                }
                                if (isCheckAll)
                                {
                                    foreach (var item in lstCheckAll)
                                    {
                                        if (item.ID != null)
                                        {
                                            item.checkMachine = true;
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
                                            item.checkMachine = false;
                                            lstChecks.Add(item);
                                        }
                                        else
                                        {
                                            lstChecks.Add(item);
                                        }
                                    }
                                    isCheckAll = true;
                                }

                                //ReloadData
                                MachineIndexProcessor.Reload(ucGridControlMachineIndex, lstChecks);
                                //??

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

        private void btn_Radio_Enable_Click1(LIS_MACHINE_INDEX data)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                LIS.Filter.LisTestIndexMapFilter listTestIndexFilter = new LisTestIndexMapFilter();
                listTestIndexFilter.MACHINE_INDEX_IDs = new List<long>() { data.ID };
                MachineIndexIdCheck = data.ID;

                TestIndexMachineIndexs = new BackendAdapter(param).Get<List<LIS_TEST_INDEX_MAP>>(
                    "api/LisTestIndexMap/Get",
                   ApiConsumers.LisConsumer,
                   listTestIndexFilter,
                   param);
                lstTestIndexADOs = new List<HIS.UC.TestIndex.TestIndexADO>();

                lstTestIndexADOs = (from r in listTestIndex select new TestIndexADO(r)).ToList();
                if (TestIndexMachineIndexs != null && TestIndexMachineIndexs.Count > 0)
                {
                    foreach (var itemUsername in TestIndexMachineIndexs)
                    {
                        var check = lstTestIndexADOs.FirstOrDefault(o => o.TEST_INDEX_CODE == itemUsername.TEST_INDEX_CODE);
                        if (check != null)
                        {
                            check.check1 = true;
                        }
                    }
                }

                lstTestIndexADOs = lstTestIndexADOs.OrderByDescending(p => p.check1).ToList();
                if (ucGridControlTestIndex != null)
                {
                    TestIndexProcessor.Reload(ucGridControlTestIndex, lstTestIndexADOs);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitUCgridTestIndex()
        {
            try
            {
                TestIndexProcessor = new UCTestIndexProcessor();
                TestIndexInitADO ado = new TestIndexInitADO();
                ado.ListTestIndexColumn = new List<TestIndexColumn>();
                ado.gridViewTestIndex_MouseDownTestIndex = gridViewTestIndex_MouseDownTestIndex;
                ado.btn_Radio_Enable_Click = btn_Radio_Enable_Click;

                TestIndexColumn colRadio1 = new TestIndexColumn("   ", "radio1", 30, true);
                colRadio1.VisibleIndex = 0;
                colRadio1.Visible = false;
                colRadio1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListTestIndexColumn.Add(colRadio1);

                TestIndexColumn colCheck1 = new TestIndexColumn("   ", "check1", 30, true);
                colCheck1.VisibleIndex = 1;
                colCheck1.image = imageCollectionTestIndex.Images[0];
                colCheck1.Visible = false;
                colCheck1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListTestIndexColumn.Add(colCheck1);

                TestIndexColumn colMaPhong = new TestIndexColumn("Mã chỉ số XN", "TEST_INDEX_CODE", 50, false);
                colMaPhong.VisibleIndex = 2;
                ado.ListTestIndexColumn.Add(colMaPhong);

                TestIndexColumn colTenPhong = new TestIndexColumn("Tên chỉ số XN", "TEST_INDEX_NAME", 120, false);
                colTenPhong.VisibleIndex = 3;
                ado.ListTestIndexColumn.Add(colTenPhong);

                TestIndexColumn colLoaiPhong = new TestIndexColumn("Mã ký hiệu", "TEST_INDEX_UNIT_SYMBOL", 100, false);
                colLoaiPhong.VisibleIndex = 4;
                ado.ListTestIndexColumn.Add(colLoaiPhong);

                TestIndexColumn colDichVu = new TestIndexColumn("Tên dịch vụ", "SERVICE_NAME", 120, false);
                colDichVu.VisibleIndex = 5;
                ado.ListTestIndexColumn.Add(colDichVu);

                this.ucGridControlTestIndex = (UserControl)TestIndexProcessor.Run(ado);
                if (ucGridControlTestIndex != null)
                {
                    this.panelControlTestIndex.Controls.Add(this.ucGridControlTestIndex);
                    this.ucGridControlTestIndex.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTestIndex_MouseDownTestIndex(object sender, MouseEventArgs e)
        {
            try
            {
                if (isChooseTestIndex == 1)
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
                            var lstCheckAll = lstTestIndexADOs;
                            List<HIS.UC.TestIndex.TestIndexADO> lstChecks = new List<HIS.UC.TestIndex.TestIndexADO>();

                            if (lstCheckAll != null && lstCheckAll.Count > 0)
                            {
                                var TestIndexCheckedNum = lstTestIndexADOs.Where(o => o.check1 == true).Count();
                                var TestIndexNum = lstTestIndexADOs.Count();
                                if ((TestIndexCheckedNum > 0 && TestIndexCheckedNum < TestIndexNum) || TestIndexCheckedNum == 0)
                                {
                                    isCheckAll = true;
                                    hi.Column.Image = imageCollectionTestIndex.Images[1];
                                }

                                if (TestIndexCheckedNum == TestIndexNum)
                                {
                                    isCheckAll = false;
                                    hi.Column.Image = imageCollectionTestIndex.Images[0];
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

                                TestIndexProcessor.Reload(ucGridControlTestIndex, lstChecks);
                                //??

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

        private void btn_Radio_Enable_Click(V_HIS_TEST_INDEX data)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                LisTestIndexMapViewFilter ListTestIndexFilter = new LisTestIndexMapViewFilter();
                ListTestIndexFilter.TEST_INDEX_CODE__EXACTs = new List<string>() { data.TEST_INDEX_CODE };
                TestIndexIdCheckByTestIndex = data.TEST_INDEX_CODE;
                TestIndexNameCheckByTestIndex = data.TEST_INDEX_NAME;

                mestTestIndexs = new BackendAdapter(param).Get<List<V_LIS_TEST_INDEX_MAP>>(
                                "api/LisTestIndexMap/GetView",
                                HIS.Desktop.ApiConsumer.ApiConsumers.LisConsumer,
                                ListTestIndexFilter,
                                param);
                lstMachineIndexADOs = new List<HIS.UC.MachineIndex.MachineIndexADO>();
                lstMachineIndexADOs = (from r in listMachineIndex select new MachineIndexADO(r)).ToList();
                if (mestTestIndexs != null && mestTestIndexs.Count > 0)
                {
                    foreach (var itemUsername in mestTestIndexs)
                    {
                        var check = lstMachineIndexADOs.FirstOrDefault(o => o.ID == itemUsername.MACHINE_INDEX_ID);
                        if (check != null)
                        {
                            check.checkMachine = true;
                        }
                    }
                }

                lstMachineIndexADOs = lstMachineIndexADOs.OrderByDescending(p => p.checkMachine).ToList();
                if (ucGridControlMachineIndex != null)
                {
                    MachineIndexProcessor.Reload(ucGridControlMachineIndex, lstMachineIndexADOs);
                }
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
                FillDataToGridTestIndex(this);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnFind1_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                FillDataToGridMachineIndex(this);
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
                WaitingManager.Show();
                isChooseMachineIndex = 0;
                isChooseTestIndex = 0;
                TestIndexMachineIndexs = null;
                mestTestIndexs = null;
                FillDataToGridMachineIndex(this);
                FillDataToGridTestIndex(this);
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
                WaitingManager.Show();
                if (ucGridControlMachineIndex != null && ucGridControlTestIndex != null)
                {
                    object MachineIndex = MachineIndexProcessor.GetDataGridView(ucGridControlMachineIndex);
                    object TestIndex = TestIndexProcessor.GetDataGridView(ucGridControlTestIndex);
                    bool success = false;
                    CommonParam param = new CommonParam();
                    if (isChooseTestIndex == 1)
                    {
                        if (MachineIndex is List<HIS.UC.MachineIndex.MachineIndexADO>)
                        {
                            lstMachineIndexADOs = (List<HIS.UC.MachineIndex.MachineIndexADO>)MachineIndex;

                            if (lstMachineIndexADOs != null && lstMachineIndexADOs.Count > 0)
                            {
                                //Danh sach cac user duoc check

                                //var listMachineIndex = mestTestIndexs.Select(p => p.MEDI_STOCK_ID).ToList();

                                var dataCheckeds = lstMachineIndexADOs.Where(p => p.checkMachine == true).ToList();

                                //List xoa

                                var dataDeletes = lstMachineIndexADOs.Where(o => mestTestIndexs.Select(p => p.MACHINE_INDEX_ID)
                                    .Contains(o.ID) && o.checkMachine == false).ToList();

                                //list them
                                var dataCreates = dataCheckeds.Where(o => !mestTestIndexs.Select(p => p.MACHINE_INDEX_ID)
                                    .Contains(o.ID)).ToList();
                                if (dataCheckeds.Count == 0 && dataDeletes.Count == 0)
                                {
                                    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn chỉ số", "Thông báo");
                                    return;
                                }
                                if (dataCheckeds != null && dataCheckeds.Count > 0)
                                {
                                    success = true;
                                }

                                if (dataDeletes != null && dataDeletes.Count > 0)
                                {
                                    List<long> deleteIds = mestTestIndexs.Where(o => dataDeletes.Select(p => p.ID)
                                        .Contains(o.MACHINE_INDEX_ID ?? 0)).Select(o => o.ID).ToList();
                                    bool deleteResult = new BackendAdapter(param).Post<bool>(
                                              "api/LisTestIndexMap/DeleteList",
                                              HIS.Desktop.ApiConsumer.ApiConsumers.LisConsumer,
                                              deleteIds,
                                              param);
                                    if (deleteResult)
                                    {
                                        success = true;
                                        mestTestIndexs = mestTestIndexs.Where(o => !deleteIds.Contains(o.ID)).ToList();
                                    }
                                }

                                if (dataCreates != null && dataCreates.Count > 0)
                                {
                                    List<LIS_TEST_INDEX_MAP> MestTestIndexCreates = new List<LIS_TEST_INDEX_MAP>();
                                    foreach (var item in dataCreates)
                                    {
                                        LIS_TEST_INDEX_MAP mestTestIndex = new LIS_TEST_INDEX_MAP();
                                        mestTestIndex.MACHINE_INDEX_ID = item.ID;
                                        mestTestIndex.TEST_INDEX_NAME = TestIndexNameCheckByTestIndex;
                                        mestTestIndex.TEST_INDEX_CODE = TestIndexIdCheckByTestIndex;
                                        MestTestIndexCreates.Add(mestTestIndex);
                                    }

                                    var createResult = new BackendAdapter(param).Post<List<LIS_TEST_INDEX_MAP>>(
                                               "api/LisTestIndexMap/CreateList",
                                               HIS.Desktop.ApiConsumer.ApiConsumers.LisConsumer,
                                               MestTestIndexCreates,
                                               param);
                                    if (createResult != null && createResult.Count > 0)
                                    {
                                        success = true;
                                        AutoMapper.Mapper.CreateMap<HIS_TEST_INDEX, LIS_TEST_INDEX_MAP>();
                                        LisTestIndexMapViewFilter ListTestIndexFilter = new LisTestIndexMapViewFilter();
                                        ListTestIndexFilter.IDs = createResult.Select(o => o.ID).ToList();
                                        var dataNew = new BackendAdapter(param).Get<List<V_LIS_TEST_INDEX_MAP>>(
                                                        "api/LisTestIndexMap/GetView",
                                                        HIS.Desktop.ApiConsumer.ApiConsumers.LisConsumer,
                                                        ListTestIndexFilter,
                                                        param);
                                        //var vCreateResults = AutoMapper.Mapper.Map<List<HIS_TEST_INDEX>, List<LIS_TEST_INDEX_MAP>>(createResult);
                                        mestTestIndexs.AddRange(dataNew);
                                    }
                                }

                                lstMachineIndexADOs = lstMachineIndexADOs.OrderByDescending(p => p.checkMachine).ToList();
                                if (ucGridControlMachineIndex != null)
                                {
                                    MachineIndexProcessor.Reload(ucGridControlMachineIndex, lstMachineIndexADOs);
                                }
                            }
                        }
                    }
                    if (isChooseMachineIndex == 2)
                    {
                        if (TestIndex is List<HIS.UC.TestIndex.TestIndexADO>)
                        {
                            lstTestIndexADOs = (List<HIS.UC.TestIndex.TestIndexADO>)TestIndex;

                            if (lstTestIndexADOs != null && lstTestIndexADOs.Count > 0)
                            {
                                //bool success = false;
                                //Danh sach cac user duoc check

                                //var listTestIndexID = TestIndexMachineIndexs.Select(p => p.TestIndex_ID).ToList();

                                var dataChecked = lstTestIndexADOs.Where(p => p.check1 == true).ToList();


                                //List xoa

                                var dataDelete = lstTestIndexADOs.Where(o => TestIndexMachineIndexs.Select(p => p.TEST_INDEX_CODE)
                                    .Contains(o.TEST_INDEX_CODE) && o.check1 == false).ToList();

                                //list them
                                var dataCreate = dataChecked.Where(o => !TestIndexMachineIndexs.Select(p => p.TEST_INDEX_CODE)
                                    .Contains(o.TEST_INDEX_CODE)).ToList();
                                if (dataChecked.Count == 0 && dataDelete.Count == 0)
                                {
                                    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn dịch vụ", "Thông báo");
                                    return;
                                }
                                if (dataChecked != null)
                                {
                                    success = true;
                                }

                                if (dataDelete != null && dataDelete.Count > 0)
                                {
                                    List<long> deleteId = TestIndexMachineIndexs.Where(o => dataDelete.Select(p => p.TEST_INDEX_CODE)
                                        .Contains(o.TEST_INDEX_CODE)).Select(o => o.ID).ToList();
                                    bool deleteResult = new BackendAdapter(param).Post<bool>(
                                              "api/LisTestIndexMap/DeleteList",
                                              HIS.Desktop.ApiConsumer.ApiConsumers.LisConsumer,
                                              deleteId,
                                              param);
                                    if (deleteResult)
                                    {
                                        success = true;
                                        TestIndexMachineIndexs = TestIndexMachineIndexs.Where(o => !deleteId.Contains(o.ID)).ToList();
                                    }
                                }

                                if (dataCreate != null && dataCreate.Count > 0)
                                {
                                    List<LIS_TEST_INDEX_MAP> mestTestIndexCreate = new List<LIS_TEST_INDEX_MAP>();
                                    foreach (var item in dataCreate)
                                    {
                                        LIS_TEST_INDEX_MAP mestTestIndex = new LIS_TEST_INDEX_MAP();
                                        mestTestIndex.TEST_INDEX_CODE = item.TEST_INDEX_CODE;
                                        mestTestIndex.TEST_INDEX_NAME = item.TEST_INDEX_NAME;
                                        mestTestIndex.MACHINE_INDEX_ID = MachineIndexIdCheck;
                                        mestTestIndexCreate.Add(mestTestIndex);
                                    }

                                    var createResult = new BackendAdapter(param).Post<List<LIS_TEST_INDEX_MAP>>(
                                               "api/LisTestIndexMap/CreateList",
                                               HIS.Desktop.ApiConsumer.ApiConsumers.LisConsumer,
                                               mestTestIndexCreate,
                                               param);
                                    if (createResult != null && createResult.Count > 0)
                                    {
                                        success = true;
                                        AutoMapper.Mapper.CreateMap<HIS_TEST_INDEX, LIS_TEST_INDEX_MAP>();
                                        //var vCreateResults = AutoMapper.Mapper.Map<List<HIS_TEST_INDEX>, List<LIS_TEST_INDEX_MAP>>(createResult);
                                        TestIndexMachineIndexs.AddRange(createResult);
                                    }
                                }

                                lstTestIndexADOs = lstTestIndexADOs.OrderByDescending(p => p.check1).ToList();
                                if (ucGridControlTestIndex != null)
                                {
                                    TestIndexProcessor.Reload(ucGridControlTestIndex, lstTestIndexADOs);
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

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeyword1_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToGridMachineIndex(this);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeyword2_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToGridTestIndex(this);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
    }
}
