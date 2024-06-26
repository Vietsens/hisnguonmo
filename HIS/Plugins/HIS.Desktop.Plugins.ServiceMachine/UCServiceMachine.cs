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
using HIS.Desktop.Plugins.ServiceMachine.Entity;
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
using HIS.UC.Machine;
using HIS.UC.Machine.ADO;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;
using DevExpress.XtraBars;
using MOS.SDO;

namespace HIS.Desktop.Plugins.ServiceMachine
{
    public partial class UCServiceMachine : HIS.Desktop.Utility.UserControlBase
    {
        List<HIS_SERVICE_TYPE> ServiceType { get; set; }

        internal Inventec.Desktop.Common.Modules.Module currentModule;
        UCMachineProcessor MachineProcessor;
        UCServiceProcessor ServiceProcessor;
        UserControl ucGridControlService;
        UserControl ucGridControlMachine;
        int start = 0;
        int limit = 0;
        int rowCount = 0;
        int dataTotal = 0;
        int start1 = 0;
        int limit1 = 0;
        int rowCount1 = 0;
        int dataTotal1 = 0;
        internal List<HIS.UC.Machine.MachineADO> lstMachineADOs { get; set; }
        internal List<HIS.UC.Service.ServiceADO> lstServiceMachineADOs { get; set; }
        List<HIS_MACHINE> listMachine;
        List<V_HIS_SERVICE> listService;
        long ServiceIdCheckByService = 0;
        long isChoseService;
        long isChoseMachine;
        long MachineIdCheckByMachine;
        bool isCheckAll;
        internal long servicetypeId;
        List<HIS_SERVICE_MACHINE> ServiceMachines { get; set; }
        List<HIS_SERVICE_MACHINE> ServiceMachineViews { get; set; }
        V_HIS_SERVICE currentService;
        HIS.UC.Machine.MachineADO currentCopyMachineAdo { get; set; }
        HIS.UC.Service.ServiceADO currentCopyServiceAdo { get; set; }

        public UCServiceMachine()
        {
            InitializeComponent();
        }
        public UCServiceMachine(Inventec.Desktop.Common.Modules.Module currentModule, long ServiceType)
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

        public UCServiceMachine(V_HIS_SERVICE serviceData, Inventec.Desktop.Common.Modules.Module _moduleData)
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

        private void UCServiceMachine_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                WaitingManager.Show();
                LoadDataToCombo();
                LoadComboStatus();
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
                            var lstCheckAll = lstServiceMachineADOs;
                            List<HIS.UC.Service.ServiceADO> lstChecks = new List<HIS.UC.Service.ServiceADO>();

                            if (lstCheckAll != null && lstCheckAll.Count > 0)
                            {
                                var ServiceCheckedNum = lstServiceMachineADOs.Where(o => o.checkService == true).Count();
                                var ServiceNum = lstServiceMachineADOs.Count();
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.ServiceMachine.Resources.Lang", typeof(HIS.Desktop.Plugins.ServiceMachine.UCServiceMachine).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCServiceMachine.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboChoose.Properties.NullText = Inventec.Common.Resource.Get.Value("UCServiceMachine.cboChoose.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("UCServiceMachine.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UCServiceMachine.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnFind2.Text = Inventec.Common.Resource.Get.Value("UCServiceMachine.btnFind2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("UCServiceMachine.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnFind1.Text = Inventec.Common.Resource.Get.Value("UCServiceMachine.btnFind1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboServiceType.Properties.NullText = Inventec.Common.Resource.Get.Value("UCServiceMachine.cboServiceType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword2.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCServiceMachine.txtKeyword2.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword1.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCServiceMachine.txtKeyword1.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("UCServiceMachine.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("UCServiceMachine.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar2.Text = Inventec.Common.Resource.Get.Value("UCServiceMachine.bar2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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

        private void gridViewMachine_MouseMachine(object sender, MouseEventArgs e)
        {
            try
            {
                if (isChoseMachine == 2)
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
                            var lstCheckAll = lstMachineADOs;
                            List<HIS.UC.Machine.MachineADO> lstChecks = new List<HIS.UC.Machine.MachineADO>();

                            if (lstCheckAll != null && lstCheckAll.Count > 0)
                            {
                                var MachineCheckedNum = lstMachineADOs.Where(o => o.check1 == true).Count();
                                var MachinetmNum = lstMachineADOs.Count();
                                if ((MachineCheckedNum > 0 && MachineCheckedNum < MachinetmNum) || MachineCheckedNum == 0)
                                {
                                    isCheckAll = true;
                                    hi.Column.Image = imageCollectionMachine.Images[1];
                                }

                                if (MachineCheckedNum == MachinetmNum)
                                {
                                    isCheckAll = false;
                                    hi.Column.Image = imageCollectionMachine.Images[0];
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

                                MachineProcessor.Reload(ucGridControlMachine, lstChecks);


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
                MOS.Filter.HisServiceMachineFilter filter = new HisServiceMachineFilter();
                filter.SERVICE_ID = data.ID;
                ServiceIdCheckByService = data.ID;

                ServiceMachines = new BackendAdapter(param).Get<List<HIS_SERVICE_MACHINE>>(
                                    "api/HisServiceMachine/Get",
                                HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                filter,
                                param);
                List<HIS.UC.Machine.MachineADO> dataNew = new List<HIS.UC.Machine.MachineADO>();
                dataNew = (from r in listMachine select new MachineADO(r)).ToList();
                if (ServiceMachines != null && ServiceMachines.Count > 0)
                {
                    foreach (var itemMachine in ServiceMachines)
                    {
                        var check = dataNew.FirstOrDefault(o => o.ID == itemMachine.MACHINE_ID);
                        if (check != null)
                        {
                            check.check1 = true;
                        }
                    }
                }
                dataNew = dataNew.OrderByDescending(p => p.check1).ToList();
                if (ucGridControlMachine != null)
                {
                    MachineProcessor.Reload(ucGridControlMachine, dataNew);
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
                MachineProcessor = new UCMachineProcessor();
                MachineInitADO ado = new MachineInitADO();
                ado.ListMachineColumn = new List<UC.Machine.MachineColumn>();
                ado.gridViewMachine_MouseDownMachine = gridViewMachine_MouseMachine;
                ado.btn_Radio_Enable_Click = btn_Radio_Enable_Click;
                ado.gridView_MouseRightClick = MachineGridView_MouseRightClick;

                MachineColumn colRadio1 = new MachineColumn("   ", "radio1", 30, true);
                colRadio1.VisibleIndex = 0;
                colRadio1.Visible = false;
                colRadio1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListMachineColumn.Add(colRadio1);

                MachineColumn colCheck1 = new MachineColumn("   ", "check1", 30, true);
                colCheck1.VisibleIndex = 1;
                colCheck1.image = imageCollectionMachine.Images[0];
                colCheck1.Visible = false;
                colCheck1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListMachineColumn.Add(colCheck1);

                MachineColumn colMaPhong = new MachineColumn("Mã máy", "MACHINE_CODE", 60, false);
                colMaPhong.VisibleIndex = 2;
                ado.ListMachineColumn.Add(colMaPhong);

                MachineColumn colTenPhong = new MachineColumn("Tên máy", "MACHINE_NAME", 100, false);
                colTenPhong.VisibleIndex = 3;
                ado.ListMachineColumn.Add(colTenPhong);

                MachineColumn colLoaiPhong = new MachineColumn("Nhóm máy", "MACHINE_GROUP_CODE", 100, false);
                colLoaiPhong.VisibleIndex = 4;
                ado.ListMachineColumn.Add(colLoaiPhong);

                MachineColumn colKhoa = new MachineColumn("Số seri", "SERIAL_NUMBER", 100, false);
                colKhoa.VisibleIndex = 5;
                ado.ListMachineColumn.Add(colKhoa);


                this.ucGridControlMachine = (UserControl)MachineProcessor.Run(ado);
                if (ucGridControlMachine != null)
                {
                    this.panelControl2.Controls.Add(this.ucGridControlMachine);
                    this.ucGridControlMachine.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btn_Radio_Enable_Click(HIS_MACHINE data)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                MOS.Filter.HisServiceMachineFilter filter = new HisServiceMachineFilter();
                filter.MACHINE_ID = data.ID;
                MachineIdCheckByMachine = data.ID;
                ServiceMachineViews = new BackendAdapter(param).Get<List<HIS_SERVICE_MACHINE>>(
                                         "api/HisServiceMachine/Get",

                                HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                filter,
                                param);
                List<HIS.UC.Service.ServiceADO> dataNew = new List<HIS.UC.Service.ServiceADO>();
                dataNew = (from r in listService select new HIS.UC.Service.ServiceADO(r)).ToList();
                if (ServiceMachineViews != null && ServiceMachineViews.Count > 0)
                {

                    foreach (var itemService in ServiceMachineViews)
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

        private void FillDataToGrid2(UCServiceMachine uCServiceMachine)
        {
            try
            {
                MachineIdCheckByMachine = 0;
                int numPageSize;
                if (ucPaging2.pagingGrid != null)
                {
                    numPageSize = ucPaging2.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                FillDataToGridMachine(new CommonParam(0, numPageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount1;
                param.Count = dataTotal1;
                ucPaging2.Init(FillDataToGridMachine, param, numPageSize);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridMachine(object data)
        {
            try
            {
                WaitingManager.Show();
                listMachine = new List<HIS_MACHINE>();
                int start1 = ((CommonParam)data).Start ?? 0;
                int limit1 = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start1, limit1);
                MOS.Filter.HisMachineFilter MachineFillter = new HisMachineFilter();
                MachineFillter.IS_ACTIVE = 1;
                MachineFillter.ORDER_FIELD = "MODIFY_TIME";
                MachineFillter.ORDER_DIRECTION = "DESC";
                MachineFillter.KEY_WORD = txtKeyword2.Text;
                if ((long)cboChoose.EditValue == 2)
                {
                    isChoseMachine = (long)cboChoose.EditValue;
                }

                var sar = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<MOS.EFMODEL.DataModels.HIS_MACHINE>>(
                   "api/HisMachine/Get",
                    HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                      MachineFillter,
                    param);

                lstMachineADOs = new List<MachineADO>();
                if (sar != null && sar.Data.Count > 0)
                {
                    listMachine = sar.Data;
                    foreach (var item in listMachine)
                    {
                        MachineADO roomaccountADO = new MachineADO(item);
                        if (isChoseMachine == 2)
                        {
                            roomaccountADO.isKeyChoose = true;
                        }
                        lstMachineADOs.Add(roomaccountADO);
                    }
                }

                if (ServiceMachines != null && ServiceMachines.Count > 0)
                {
                    foreach (var itemUsername in ServiceMachines)
                    {
                        var check = lstMachineADOs.FirstOrDefault(o => o.ID == itemUsername.MACHINE_ID);
                        if (check != null)
                        {
                            check.check1 = true;
                        }
                    }
                }
                lstMachineADOs = lstMachineADOs.OrderByDescending(p => p.check1).Distinct().ToList();

                if (ucGridControlMachine != null)
                {
                    MachineProcessor.Reload(ucGridControlMachine, lstMachineADOs);
                }
                rowCount1 = (data == null ? 0 : lstMachineADOs.Count);
                dataTotal1 = (sar.Param == null ? 0 : sar.Param.Count ?? 0);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGrid1(UCServiceMachine UCServiceMachine)
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

        private void FillDataToGrid1_Default(UCServiceMachine UCServiceMachine)
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
                ServiceFillter.IS_ACTIVE = 1;
                ServiceFillter.ORDER_FIELD = "MODIFY_TIME";
                ServiceFillter.ORDER_DIRECTION = "DESC";
                ServiceFillter.KEY_WORD = txtKeyword1.Text;

                if (cboServiceType.EditValue != null)

                    ServiceFillter.SERVICE_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboServiceType.EditValue ?? "0").ToString());
                else
                    ServiceFillter.SERVICE_TYPE_IDs = BackendDataWorker.Get<HIS_SERVICE_TYPE>().Where(o => o.ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC && o.ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT && o.ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU).Select(o => o.ID).ToList();

                if ((long)cboChoose.EditValue == 1)
                {
                    isChoseService = (long)cboChoose.EditValue;
                }

                var rs = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE>>(
                                                     "api/HisService/GetView",
                     HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                     ServiceFillter,
                     param);

                lstServiceMachineADOs = new List<ServiceADO>();

                if (rs != null && rs.Data.Count > 0)
                {

                    listService = rs.Data;
                    foreach (var item in listService)
                    {
                        ServiceADO ServiceMachineADO = new ServiceADO(item);
                        if (isChoseService == 1)
                        {
                            ServiceMachineADO.isKeyChooseService = true;
                        }
                        lstServiceMachineADOs.Add(ServiceMachineADO);
                    }
                }

                if (ServiceMachineViews != null && ServiceMachineViews.Count > 0)
                {
                    foreach (var itemUsername in ServiceMachineViews)
                    {
                        var check = lstServiceMachineADOs.FirstOrDefault(o => o.ID == itemUsername.SERVICE_ID);
                        if (check != null)
                        {
                            check.checkService = true;
                        }
                    }
                }

                lstServiceMachineADOs = lstServiceMachineADOs.OrderByDescending(p => p.checkService).Distinct().ToList();
                if (ucGridControlService != null)
                {
                    ServiceProcessor.Reload(ucGridControlService, lstServiceMachineADOs);
                }
                rowCount = (data == null ? 0 : lstServiceMachineADOs.Count);
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
                ServiceFillter.IS_ACTIVE = 1;
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

                lstServiceMachineADOs = new List<ServiceADO>();

                if (rs != null && rs.Data.Count > 0)
                {

                    listService = rs.Data;
                    foreach (var item in listService)
                    {
                        ServiceADO ServiceMachineADO = new ServiceADO(item);
                        if (isChoseService == 1)
                        {
                            ServiceMachineADO.isKeyChooseService = true;
                            ServiceMachineADO.radioService = true;
                        }
                        lstServiceMachineADOs.Add(ServiceMachineADO);
                    }
                }

                if (ServiceMachineViews != null && ServiceMachineViews.Count > 0)
                {
                    foreach (var itemUsername in ServiceMachineViews)
                    {
                        var check = lstServiceMachineADOs.FirstOrDefault(o => o.ID == itemUsername.SERVICE_ID);
                        if (check != null)
                        {
                            check.checkService = true;
                        }
                    }
                }

                lstServiceMachineADOs = lstServiceMachineADOs.OrderByDescending(p => p.checkService).ToList();
                if (ucGridControlService != null)
                {
                    ServiceProcessor.Reload(ucGridControlService, lstServiceMachineADOs);
                }
                rowCount = (data == null ? 0 : lstServiceMachineADOs.Count);
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
                    param).Where(o => o.ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU && o.ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC && o.ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT).ToList();

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
                status.Add(new Status(2, "Máy"));

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
                ServiceMachineViews = null;
                ServiceMachines = null;
                isChoseMachine = 0;
                isChoseService = 0;
                MachineIdCheckByMachine = 0;
                ServiceIdCheckByService = 0;
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
                if (ucGridControlMachine != null && ucGridControlService != null)
                {
                    object Machine = MachineProcessor.GetDataGridView(ucGridControlMachine);
                    object Service = ServiceProcessor.GetDataGridView(ucGridControlService);
                    bool success = false;
                    CommonParam param = new CommonParam();
                    if (isChoseService == 1)
                    {
                        if (ServiceIdCheckByService == 0)
                        {
                            WaitingManager.Hide();
                            DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn dịch vụ");
                            return;
                        }

                        if (Machine is List<HIS.UC.Machine.MachineADO>)
                        {
                            lstMachineADOs = (List<HIS.UC.Machine.MachineADO>)Machine;

                            if (lstMachineADOs != null && lstMachineADOs.Count > 0)
                            {
                                //List<long> listServiceMachines = ServiceMachines.Select(p => p.SERVICE_ID).ToList();

                                var dataCheckeds = lstMachineADOs.Where(p => p.check1 == true).ToList();

                                //List xoa

                                var dataDeletes = lstMachineADOs.Where(o => ServiceMachines.Select(p => p.MACHINE_ID)
                                    .Contains(o.ID) && o.check1 == false).ToList();


                                //list them
                                var dataCreates = dataCheckeds.Where(o => !ServiceMachines.Select(p => p.MACHINE_ID)
                                    .Contains(o.ID)).ToList();

                                if (dataDeletes != null && dataDeletes.Count == 0 && dataCreates != null && dataCreates.Count == 0)
                                {
                                    WaitingManager.Hide();
                                    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn máy", "Thông báo");
                                    return;
                                }

                                if (dataDeletes != null && dataDeletes.Count > 0)
                                {
                                    List<long> deleteSds = ServiceMachines.Where(o => dataDeletes.Select(p => p.ID)
                                        .Contains(o.MACHINE_ID)).Select(o => o.ID).ToList();
                                    bool deleteResult = new BackendAdapter(param).Post<bool>(
                                              "api/HisServiceMachine/DeleteList",
                                              HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                              deleteSds,
                                              param);
                                    if (deleteResult)
                                        success = true;
                                    ServiceMachines = ServiceMachines.Where(o => !deleteSds.Contains(o.ID)).ToList();
                                }

                                if (dataCreates != null && dataCreates.Count > 0)
                                {
                                    List<HIS_SERVICE_MACHINE> ServiceMachineCreates = new List<HIS_SERVICE_MACHINE>();
                                    foreach (var item in dataCreates)
                                    {
                                        HIS_SERVICE_MACHINE ServiceMachine = new HIS_SERVICE_MACHINE();
                                        ServiceMachine.SERVICE_ID = ServiceIdCheckByService;
                                        ServiceMachine.MACHINE_ID = item.ID;
                                        ServiceMachineCreates.Add(ServiceMachine);
                                    }

                                    var createResult = new BackendAdapter(param).Post<List<HIS_SERVICE_MACHINE>>(
                                               "api/HisServiceMachine/CreateList",
                                               HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                               ServiceMachineCreates,
                                               param);
                                    if (createResult != null && createResult.Count > 0)
                                        success = true;
                                    AutoMapper.Mapper.CreateMap<HIS_SERVICE_MACHINE, HIS_SERVICE_MACHINE>();
                                    var vCreateResults = AutoMapper.Mapper.Map<List<HIS_SERVICE_MACHINE>, List<HIS_SERVICE_MACHINE>>(createResult);
                                    ServiceMachines.AddRange(vCreateResults);
                                }

                                lstMachineADOs = lstMachineADOs.OrderByDescending(p => p.check1).ToList();
                                if (ucGridControlMachine != null)
                                {
                                    MachineProcessor.Reload(ucGridControlMachine, lstMachineADOs);
                                }
                            }
                        }
                    }

                    if (isChoseMachine == 2)
                    {
                        if (MachineIdCheckByMachine == 0)
                        {
                            WaitingManager.Hide();
                            DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn máy");
                            return;
                        }

                        if (Service is List<HIS.UC.Service.ServiceADO>)
                        {
                            lstServiceMachineADOs = (List<HIS.UC.Service.ServiceADO>)Service;

                            if (lstServiceMachineADOs != null && lstServiceMachineADOs.Count > 0)
                            {
                                //List<long> listServiceMachines = ServiceMachine.Select(p => p.MACHINE_ID).ToList();

                                var dataChecked = lstServiceMachineADOs.Where(p => p.checkService == true).ToList();
                                //List xoa

                                var dataDelete = lstServiceMachineADOs.Where(o => ServiceMachineViews.Select(p => p.SERVICE_ID)
                                    .Contains(o.ID) && o.checkService == false).ToList();

                                //list them
                                var dataCreate = dataChecked.Where(o => !ServiceMachineViews.Select(p => p.SERVICE_ID)
                                    .Contains(o.ID)).ToList();

                                if (dataDelete != null && dataDelete.Count == 0 && dataCreate != null && dataCreate.Count == 0)
                                {
                                    WaitingManager.Hide();
                                    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn dịch vụ", "Thông báo");
                                    return;
                                }

                                if (dataDelete != null && dataDelete.Count > 0)
                                {

                                    List<long> deleteId = ServiceMachineViews.Where(o => dataDelete.Select(p => p.ID)
                                        .Contains(o.SERVICE_ID)).Select(o => o.ID).ToList();
                                    bool deleteResult = new BackendAdapter(param).Post<bool>(
                                              "api/HisServiceMachine/DeleteList",
                                              HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                              deleteId,
                                              param);
                                    if (deleteResult)
                                        success = true;
                                    ServiceMachineViews = ServiceMachineViews.Where(o => !deleteId.Contains(o.ID)).ToList();
                                }

                                if (dataCreate != null && dataCreate.Count > 0)
                                {
                                    List<HIS_SERVICE_MACHINE> ServiceMachineCreate = new List<HIS_SERVICE_MACHINE>();
                                    foreach (var item in dataCreate)
                                    {
                                        HIS_SERVICE_MACHINE ServiceMachineID = new HIS_SERVICE_MACHINE();
                                        ServiceMachineID.MACHINE_ID = MachineIdCheckByMachine;
                                        ServiceMachineID.SERVICE_ID = item.ID;
                                        ServiceMachineCreate.Add(ServiceMachineID);
                                    }

                                    var createResult = new BackendAdapter(param).Post<List<HIS_SERVICE_MACHINE>>(
                                               "/api/HisServiceMachine/CreateList",
                                               HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                               ServiceMachineCreate,
                                               param);
                                    if (createResult != null && createResult.Count > 0)
                                        success = true;
                                    AutoMapper.Mapper.CreateMap<HIS_SERVICE_MACHINE, HIS_SERVICE_MACHINE>();
                                    var vCreateResults = AutoMapper.Mapper.Map<List<HIS_SERVICE_MACHINE>, List<HIS_SERVICE_MACHINE>>(createResult);
                                    ServiceMachineViews.AddRange(vCreateResults);
                                }

                                lstServiceMachineADOs = lstServiceMachineADOs.OrderByDescending(p => p.checkService).ToList();
                                if (ucGridControlMachine != null)
                                {
                                    ServiceProcessor.Reload(ucGridControlService, lstServiceMachineADOs);
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
                                    HisServiceMachineCopyByServiceSDO hisMestMatyCopyByMatySDO = new HisServiceMachineCopyByServiceSDO();
                                    hisMestMatyCopyByMatySDO.CopyServiceId = this.currentCopyServiceAdo.ID;
                                    hisMestMatyCopyByMatySDO.PasteServiceId = currentPaste.ID;
                                    var result = new BackendAdapter(param).Post<List<HIS_SERVICE_MACHINE>>("api/HisServiceMachine/CopyByService", ApiConsumer.ApiConsumers.MosConsumer, hisMestMatyCopyByMatySDO, param);
                                    if (result != null)
                                    {
                                        success = true;
                                        List<HIS.UC.Machine.MachineADO> dataNew = new List<HIS.UC.Machine.MachineADO>();
                                        dataNew = (from r in listMachine select new MachineADO(r)).ToList();
                                        if (result != null && result.Count > 0)
                                        {
                                            foreach (var itemMachine in result)
                                            {
                                                var check = dataNew.FirstOrDefault(o => o.ID == itemMachine.MACHINE_ID);
                                                if (check != null)
                                                {
                                                    check.check1 = true;
                                                }
                                            }
                                        }
                                        dataNew = dataNew.OrderByDescending(p => p.check1).ToList();
                                        if (ucGridControlMachine != null)
                                        {
                                            MachineProcessor.Reload(ucGridControlMachine, dataNew);
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

        private void MachineGridView_MouseRightClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            try
            {
                if ((e.Item is BarButtonItem) && sender != null && sender is HIS.UC.Machine.MachineADO)
                {
                    var type = (HIS.UC.Machine.Popup.PopupMenuProcessor.ItemType)e.Item.Tag;
                    switch (type)
                    {
                        case HIS.UC.Machine.Popup.PopupMenuProcessor.ItemType.Copy:
                            {
                                if (isChoseMachine != 2)
                                {
                                    MessageManager.Show("Vui lòng chọn máy!");
                                    break;
                                }
                                this.currentCopyMachineAdo = (HIS.UC.Machine.MachineADO)sender;
                                break;
                            }
                        case HIS.UC.Machine.Popup.PopupMenuProcessor.ItemType.Paste:
                            {
                                var currentPaste = (HIS.UC.Machine.MachineADO)sender;
                                bool success = false;
                                CommonParam param = new CommonParam();
                                if (this.currentCopyMachineAdo == null && isChoseMachine != 2)
                                {
                                    MessageManager.Show("Vui lòng copy!");
                                    break;
                                }
                                if (this.currentCopyMachineAdo != null && currentPaste != null && isChoseMachine == 2)
                                {
                                    if (this.currentCopyMachineAdo.ID == currentPaste.ID)
                                    {
                                        MessageManager.Show("Trùng dữ liệu copy và paste");
                                        break;
                                    }
                                    HisServiceMachineCopyByMachineSDO hisMestMatyCopyByMatySDO = new HisServiceMachineCopyByMachineSDO();
                                    hisMestMatyCopyByMatySDO.CopyMachineId = this.currentCopyMachineAdo.ID;
                                    hisMestMatyCopyByMatySDO.PasteMachineId = currentPaste.ID;
                                    var result = new BackendAdapter(param).Post<List<HIS_SERVICE_MACHINE>>("api/HisServiceMachine/CopyByMachine", ApiConsumer.ApiConsumers.MosConsumer, hisMestMatyCopyByMatySDO, param);
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
    }
}







