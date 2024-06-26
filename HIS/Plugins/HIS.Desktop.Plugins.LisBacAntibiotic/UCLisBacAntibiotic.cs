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
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.ADO;
using AutoMapper;
using ACS.Filter;
using HIS.Desktop.LocalStorage.BackendData;
using ACS.EFMODEL.DataModels;
using Inventec.Common.Adapter;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.UC.LisAntibiotic;
using HIS.UC.LisAntibiotic.ADO;
using HIS.UC.LisBacterium;
using HIS.UC.LisBacterium.ADO;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;
using HIS.Desktop.Common;
using DevExpress.XtraBars;
using MOS.SDO;
using HIS.Desktop.Plugins.LisBacAntibiotic.Entity;
using LIS.EFMODEL.DataModels;
using LIS.Filter;

namespace HIS.Desktop.Plugins.LisBacAntibiotic
{
    public partial class UCLisBacAntibiotic : UserControl
    {
        List<LIS_BACTERIUM_FAMILY> RoomType { get; set; }
        internal Inventec.Desktop.Common.Modules.Module currentModule;
        UCLisBacteriumProcessor RoomProcessor;
        AntibioticProcessor AntibioticProcessor;
        UserControl ucGridControlExecuteRoom;
        UserControl ucGridControlRoom;
        int rowCount = 0;
        int dataTotal = 0;
        int rowCount1 = 0;
        int dataTotal1 = 0;
        internal List<HIS.UC.LisBacterium.LisBacteriumADO> lstRoomADOs { get; set; }
        internal List<HIS.UC.LisAntibiotic.AntibioticADO> lstExecuteRoomADOs { get; set; }
        List<LIS_BACTERIUM> listRoom;
        List<LIS_ANTIBIOTIC> listExecuteRoom;
        long ExecuteRoomIdCheckByExecuteRoom = 0;
        long isChoseExecuteRoom;
        long isChoseRoom;
        long RoomIdCheckByRoom;
        long checkExecuteRoomId;
        long checkRoomId;
        bool isCheckAll;
        List<LIS_BAC_ANTIBIOTIC> BacAntibiotics { get; set; }
        List<LIS_BAC_ANTIBIOTIC> BacAntibioticViews { get; set; }
        LIS_ANTIBIOTIC currentAntibiotic;

        HIS.UC.LisAntibiotic.AntibioticADO currentCopyExroRoomAdo;
        HIS.UC.LisBacterium.LisBacteriumADO CurrentRoomCopyAdo;
        LIS_BACTERIUM Bacterium;

        public UCLisBacAntibiotic()
        {
            InitializeComponent();

        }

        public UCLisBacAntibiotic(Inventec.Desktop.Common.Modules.Module currentModule, long RoomType)
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

        public UCLisBacAntibiotic(LIS_ANTIBIOTIC MedicineTypeData)
        {
            InitializeComponent();
            try
            {
                this.currentAntibiotic = MedicineTypeData;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
        public UCLisBacAntibiotic(LIS_BACTERIUM executeRoom1)
        {
            InitializeComponent();
            try
            {
                this.Bacterium = executeRoom1;
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
                InitUcgridViewMedicineType();
                InitUcgridViewRoom();

                if (this.currentAntibiotic == null && this.Bacterium == null)
                {
                    FillDataToGrid1__MedicineType(this);
                    FillDataToGrid2__Room(this);
                }



                else if (this.currentAntibiotic != null)
                {
                    FillDataToGrid1__ExecuteRoom_Default(this);
                    FillDataToGrid2__Room(this);
                    if (currentAntibiotic != null)
                    {
                        btn_Radio_Enable_Click1(currentAntibiotic);
                    }
                }
                else if (this.Bacterium != null)
                {
                    FillDataToGrid1__MedicineType(this);
                    FillDataToGrid1_Room_Default(this);
                    if (Bacterium != null)
                    {

                        btn_Radio_Enable_Click(Bacterium);
                        cboRoomType.EditValue = Bacterium.BACTERIUM_FAMILY_ID;
                    }


                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void FillDataToGrid1_Room(object data)
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

                FillDataToGridRoom_Default(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridRoom_Default, param, numPageSize);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void FillDataToGridExecuteRoom_Default(object data)
        {

            try
            {
                WaitingManager.Show();
                this.listExecuteRoom = new List<LIS_ANTIBIOTIC>();
                int start = ((CommonParam)data).Start ?? 0;
                int limit = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start, limit);
                MOS.Filter.HisExecuteRoomViewFilter hisExecuteRoomViewFilter = new HisExecuteRoomViewFilter();
                hisExecuteRoomViewFilter.IS_ACTIVE = 1;
                hisExecuteRoomViewFilter.ID = this.currentAntibiotic.ID;

                //if (cboServiceType.EditValue != null)

                //    ServiceFillter.SERVICE_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboServiceType.EditValue ?? "0").ToString());

                if ((long)cboChoose.EditValue == 1)
                {
                    isChoseExecuteRoom = (long)cboChoose.EditValue;
                }

                var rs = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<LIS.EFMODEL.DataModels.LIS_ANTIBIOTIC>>(
                                                     "api/LisAntibiotic/Get",
                     HIS.Desktop.ApiConsumer.ApiConsumers.LisConsumer,
                     hisExecuteRoomViewFilter,
                     param);

                this.lstExecuteRoomADOs = new List<AntibioticADO>();

                if (rs != null && rs.Data.Count > 0)
                {
                    this.listExecuteRoom = new List<LIS_ANTIBIOTIC>();
                    this.listExecuteRoom = rs.Data;
                    foreach (var item in this.listExecuteRoom)
                    {
                        AntibioticADO RoomServiceADO = new AntibioticADO(item);
                        if (isChoseExecuteRoom == 1)
                        {
                            RoomServiceADO.isKeyChoose = true;
                            RoomServiceADO.radio1 = true;
                        }
                        this.lstExecuteRoomADOs.Add(RoomServiceADO);
                    }
                }

                if (BacAntibioticViews != null && BacAntibioticViews.Count > 0)
                {
                    foreach (var itemUsername in BacAntibioticViews)
                    {
                        var check = this.lstExecuteRoomADOs.FirstOrDefault(o => o.ID == itemUsername.ID);
                        if (check != null)
                        {
                            check.check1 = true;
                        }
                    }
                }

                this.lstExecuteRoomADOs = this.lstExecuteRoomADOs.OrderByDescending(p => p.check1).ToList();
                if (ucGridControlExecuteRoom != null)
                {
                    AntibioticProcessor.Reload(ucGridControlExecuteRoom, this.lstExecuteRoomADOs);
                }
                rowCount = (data == null ? 0 : lstExecuteRoomADOs.Count);
                dataTotal = (rs.Param == null ? 0 : rs.Param.Count ?? 0);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void gridViewMedicineType_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (isChoseExecuteRoom == 1)
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
                            var lstCheckAll = lstExecuteRoomADOs;
                            List<HIS.UC.LisAntibiotic.AntibioticADO> lstChecks = new List<HIS.UC.LisAntibiotic.AntibioticADO>();

                            if (lstCheckAll != null && lstCheckAll.Count > 0)
                            {
                                var ServiceCheckedNum = lstExecuteRoomADOs.Where(o => o.check1 == true).Count();
                                var ServiceNum = lstExecuteRoomADOs.Count();
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

                                AntibioticProcessor.Reload(ucGridControlExecuteRoom, lstChecks);
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.LisBacAntibiotic.Resources.Lang", typeof(HIS.Desktop.Plugins.LisBacAntibiotic.UCLisBacAntibiotic).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCLisBacAntibiotic.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboChoose.Properties.NullText = Inventec.Common.Resource.Get.Value("UCLisBacAntibiotic.cboChoose.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UCLisBacAntibiotic.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnFind2.Text = Inventec.Common.Resource.Get.Value("UCLisBacAntibiotic.btnFind2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("UCLisBacAntibiotic.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnFind1.Text = Inventec.Common.Resource.Get.Value("UCLisBacAntibiotic.btnFind1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboRoomType.Properties.NullText = Inventec.Common.Resource.Get.Value("UCLisBacAntibiotic.cboRoomType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword2.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCLisBacAntibiotic.txtKeyword2.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword1.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCLisBacAntibiotic.txtKeyword1.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("UCLisBacAntibiotic.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciRoomType.Text = Inventec.Common.Resource.Get.Value("UCLisBacAntibiotic.lciRoomType.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar2.Text = Inventec.Common.Resource.Get.Value("UCLisBacAntibiotic.bar2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitUcgridViewMedicineType()
        {
            try
            {
                AntibioticProcessor = new AntibioticProcessor();
                LisAntibioticInitADO ado = new LisAntibioticInitADO();
                ado.ListExecuteRoomColumn = new List<AntibioticColumn>();
                ado.ExecuteRoomGrid_MouseDown = gridViewMedicineType_MouseDown;
                ado.btn_Radio_Enable_Click = btn_Radio_Enable_Click1;
                ado.gridView_MouseRightClick = ExecuteRoomGridView_MouseRightClick;

                AntibioticColumn colRadio2 = new AntibioticColumn("   ", "radio1", 30, true);
                colRadio2.VisibleIndex = 0;
                colRadio2.Visible = false;
                colRadio2.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListExecuteRoomColumn.Add(colRadio2);

                AntibioticColumn colCheck2 = new AntibioticColumn("   ", "check1", 30, true);
                colCheck2.VisibleIndex = 1;
                colCheck2.image = imageCollectionService.Images[0];
                colCheck2.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListExecuteRoomColumn.Add(colCheck2);

                AntibioticColumn colMaPhong = new AntibioticColumn("Mã kháng sinh", "ANTIBIOTIC_CODE", 65, false);
                colMaPhong.VisibleIndex = 2;
                ado.ListExecuteRoomColumn.Add(colMaPhong);

                AntibioticColumn colTenDichvu = new AntibioticColumn("Tên kháng sinh", "ANTIBIOTIC_NAME", 100, false);
                colTenDichvu.VisibleIndex = 3;
                ado.ListExecuteRoomColumn.Add(colTenDichvu);

                //AntibioticColumn colDuocLaySoUuTien = new AntibioticColumn("Ưu tiên", "isHoldOrder", 60, true);
                //colDuocLaySoUuTien.VisibleIndex = 4;
                //colDuocLaySoUuTien.ToolTip = "Được lấy số ưu tiên";
                //ado.ListExecuteRoomColumn.Add(colDuocLaySoUuTien);

                //AntibioticColumn colDuocPhepYeuCau = new AntibioticColumn("Yêu cầu", "isAllowRequest", 60, true);
                //colDuocPhepYeuCau.VisibleIndex = 5;
                //colDuocPhepYeuCau.ToolTip = "Được phép yêu cầu";
                //ado.ListExecuteRoomColumn.Add(colDuocPhepYeuCau);

                //AntibioticColumn colMaLoaidichvu = new AntibioticColumn("Loại phòng", "ROOM_TYPE_NAME", 80, false);
                //colMaLoaidichvu.VisibleIndex = 6;
                //ado.ListExecuteRoomColumn.Add(colMaLoaidichvu);

                //AntibioticColumn colDepartment = new AntibioticColumn("Khoa", "DEPARTMENT_NAME", 80, false);
                //colDepartment.VisibleIndex = 7;
                //ado.ListExecuteRoomColumn.Add(colDepartment);

                this.ucGridControlExecuteRoom = (UserControl)AntibioticProcessor.Run(ado);
                if (ucGridControlExecuteRoom != null)
                {
                    this.panelControl1.Controls.Add(this.ucGridControlExecuteRoom);
                    this.ucGridControlExecuteRoom.Dock = DockStyle.Fill;
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
                            List<HIS.UC.LisBacterium.LisBacteriumADO> lstChecks = new List<HIS.UC.LisBacterium.LisBacteriumADO>();

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

        private void btn_Radio_Enable_Click1(LIS_ANTIBIOTIC data)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                LIS.Filter.LisBacAntibioticFilter filter = new LisBacAntibioticFilter();
                filter.ANTIBIOTIC_ID = data.ID;
                checkExecuteRoomId = data.ID;
                ExecuteRoomIdCheckByExecuteRoom = data.ID;

                BacAntibiotics = new BackendAdapter(param).Get<List<LIS_BAC_ANTIBIOTIC>>(
                                    "api/LisBacAntibiotic/Get",
                                HIS.Desktop.ApiConsumer.ApiConsumers.LisConsumer,
                                filter,
                                param);
                List<HIS.UC.LisBacterium.LisBacteriumADO> dataNew = new List<HIS.UC.LisBacterium.LisBacteriumADO>();
                dataNew = (from r in listRoom select new LisBacteriumADO(r)).ToList();
                if (BacAntibiotics != null && BacAntibiotics.Count > 0)
                {
                    foreach (var itemRoom in BacAntibiotics)
                    {
                        var check = dataNew.FirstOrDefault(o => o.ID == itemRoom.BACTERIUM_ID);
                        if (check != null)
                        {
                            check.check1 = true;
                            //if (itemRoom.IS_ALLOW_REQUEST == 1)
                            //{
                            //    check.isAllowRequest = true;
                            //}
                            //else
                            //{
                            //    check.isAllowRequest = false;
                            //}
                            //if (itemRoom.IS_HOLD_ORDER == 1)
                            //{
                            //    check.isHoldOrder = true;
                            //}
                            //else
                            //{
                            //    check.isHoldOrder = false;
                            //}
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
                    FillDataToGrid2__Room(this);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitUcgridViewRoom()
        {
            try
            {
                RoomProcessor = new UCLisBacteriumProcessor();
                RoomInitADO ado = new RoomInitADO();
                ado.ListRoomColumn = new List<UC.LisBacterium.LisBacteriumColumn>();
                ado.gridViewRoom_MouseDownRoom = gridViewRoom_MouseRoom;
                ado.btn_Radio_Enable_Click = btn_Radio_Enable_Click;
                ado.rooom_MouseRightClick = RoomGridView_MouseRightClick;

                LisBacteriumColumn colRadio1 = new LisBacteriumColumn("   ", "radio1", 30, true);
                colRadio1.VisibleIndex = 0;
                colRadio1.Visible = false;
                colRadio1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListRoomColumn.Add(colRadio1);

                LisBacteriumColumn colCheck1 = new LisBacteriumColumn("   ", "check1", 30, true);
                colCheck1.VisibleIndex = 1;
                colCheck1.image = imageCollectionRoom.Images[0];
                colCheck1.Visible = false;
                colCheck1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListRoomColumn.Add(colCheck1);

                LisBacteriumColumn colMaPhong = new LisBacteriumColumn("Mã vi khuẩn", "BACTERIUM_CODE", 50, false);
                colMaPhong.VisibleIndex = 2;
                ado.ListRoomColumn.Add(colMaPhong);

                LisBacteriumColumn colTenPhong = new LisBacteriumColumn("Tên vi khuẩn", "BACTERIUM_NAME", 100, false);
                colTenPhong.VisibleIndex = 3;
                ado.ListRoomColumn.Add(colTenPhong);

                //LisBacteriumColumn colUuTien = new LisBacteriumColumn("Ưu tiên", "isHoldOrder", 60, true);
                //colUuTien.VisibleIndex = 4;
                //colUuTien.ToolTip = "Được lấy số ưu tiên";
                //ado.ListRoomColumn.Add(colUuTien);

                //LisBacteriumColumn colYeuCau = new LisBacteriumColumn("Yêu cầu", "isAllowRequest", 60, true);
                //colYeuCau.VisibleIndex = 5;
                //colYeuCau.ToolTip = "Được phép yêu cầu";
                //ado.ListRoomColumn.Add(colYeuCau);

                //LisBacteriumColumn colLoaiPhong = new LisBacteriumColumn("Loại phòng", "ROOM_TYPE_NAME", 60, false);
                //colLoaiPhong.VisibleIndex = 6;
                //ado.ListRoomColumn.Add(colLoaiPhong);

                //LisBacteriumColumn colKhoa = new LisBacteriumColumn("Khoa", "DEPARTMENT_NAME", 80, false);
                //colKhoa.VisibleIndex = 7;
                //ado.ListRoomColumn.Add(colKhoa);


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

        private void btn_Radio_Enable_Click(LIS_BACTERIUM data)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                LIS.Filter.LisBacAntibioticFilter filter = new LisBacAntibioticFilter();
                filter.BACTERIUM_ID = data.ID;
                RoomIdCheckByRoom = data.ID;
                checkRoomId = data.ID;
                BacAntibioticViews = new BackendAdapter(param).Get<List<LIS_BAC_ANTIBIOTIC>>(
                                         "api/LisBacAntibiotic/Get",

                                HIS.Desktop.ApiConsumer.ApiConsumers.LisConsumer,
                                filter,
                                param);
                List<HIS.UC.LisAntibiotic.AntibioticADO> dataNew = new List<HIS.UC.LisAntibiotic.AntibioticADO>();
                dataNew = (from r in listExecuteRoom select new HIS.UC.LisAntibiotic.AntibioticADO(r)).ToList();
                if (BacAntibioticViews != null && BacAntibioticViews.Count > 0)
                {

                    foreach (var itemService in BacAntibioticViews)
                    {
                        var check = dataNew.FirstOrDefault(o => o.ID == itemService.ANTIBIOTIC_ID);
                        if (check != null)
                        {
                            check.check1 = true;
                            //if (itemService.IS_HOLD_ORDER == 1)
                            //{
                            //    check.isHoldOrder = true;
                            //}
                            //else
                            //{
                            //    check.isHoldOrder = false;
                            //}
                            //if (itemService.IS_ALLOW_REQUEST == 1)
                            //{
                            //    check.isAllowRequest = true;
                            //}
                            //else
                            //{
                            //    check.isAllowRequest = false;
                            //}
                        }
                    }


                    dataNew = dataNew.OrderByDescending(p => p.check1).ToList();


                    if (ucGridControlExecuteRoom != null)
                    {
                        AntibioticProcessor.Reload(ucGridControlExecuteRoom, dataNew);
                    }
                }
                else
                {
                    FillDataToGrid1__MedicineType(this);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGrid2__Room(UCLisBacAntibiotic uCRoomService)
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
        private void FillDataToGrid2_ExecuteRoom(UCLisBacAntibiotic uCRoomService)
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

                FillDataToGridExecuteRoom(new CommonParam(0, numPageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount1;
                param.Count = dataTotal1;
                ucPaging2.Init(FillDataToGridExecuteRoom, param, numPageSize);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void FillDataToGridExecuteRoom(object data)
        {
            try
            {
                WaitingManager.Show();
                listExecuteRoom = new List<LIS_ANTIBIOTIC>();
                int start1 = ((CommonParam)data).Start ?? 0;
                int limit1 = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start1, limit1);
                LIS.Filter.LisAntibioticFilter ExecuteRoomFillter = new LisAntibioticFilter();
                ExecuteRoomFillter.IS_ACTIVE = 1;
                ExecuteRoomFillter.ORDER_FIELD = "MODIFY_TIME";
                ExecuteRoomFillter.ORDER_DIRECTION = "DESC";
                ExecuteRoomFillter.KEY_WORD = txtKeyword2.Text;
                if ((long)cboChoose.EditValue == 1)
                {
                    isChoseExecuteRoom = (long)cboChoose.EditValue;
                }
                //if (cboRoomType.EditValue != null)
                //    ExecuteRoomFillter. = Inventec.Common.TypeConvert.Parse.ToInt64((cboRoomType.EditValue ?? "0").ToString());
                var sar = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<LIS.EFMODEL.DataModels.LIS_ANTIBIOTIC>>(
                   HIS.Desktop.ApiConsumer.HisRequestUriStore.HIS_EXECUTE_ROOM_GETVIEW,
                    HIS.Desktop.ApiConsumer.ApiConsumers.LisConsumer,
                      ExecuteRoomFillter,
                    param);

                lstExecuteRoomADOs = new List<AntibioticADO>();
                if (sar != null && sar.Data.Count > 0)
                {
                    listExecuteRoom = sar.Data;
                    foreach (var item in listExecuteRoom)
                    {
                        AntibioticADO roomaccountADO = new AntibioticADO(item);
                        if (isChoseRoom == 2)
                        {
                            roomaccountADO.isKeyChoose = true;
                        }
                        lstExecuteRoomADOs.Add(roomaccountADO);
                    }
                }

                if (BacAntibiotics != null && BacAntibiotics.Count > 0)
                {
                    foreach (var itemUsername in BacAntibiotics)
                    {
                        var check = lstExecuteRoomADOs.FirstOrDefault(o => o.ID == itemUsername.BACTERIUM_ID);
                        if (check != null)
                        {
                            check.check1 = true;
                        }
                    }
                }
                lstExecuteRoomADOs = lstExecuteRoomADOs.OrderByDescending(p => p.check1).Distinct().ToList();

                if (ucGridControlExecuteRoom != null)
                {
                    AntibioticProcessor.Reload(ucGridControlExecuteRoom, lstExecuteRoomADOs);
                }
                rowCount1 = (data == null ? 0 : lstExecuteRoomADOs.Count);
                dataTotal1 = (sar.Param == null ? 0 : sar.Param.Count ?? 0);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void FillDataToGridRoomData(object data)
        {
            try
            {
                WaitingManager.Show();
                listRoom = new List<LIS_BACTERIUM>();
                int start1 = ((CommonParam)data).Start ?? 0;
                int limit1 = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start1, limit1);
                LIS.Filter.LisBacteriumFilter RoomFillter = new LisBacteriumFilter();
                RoomFillter.IS_ACTIVE = 1;
                RoomFillter.ID = this.Bacterium.ID;
                RoomFillter.ORDER_FIELD = "MODIFY_TIME";
                RoomFillter.ORDER_DIRECTION = "DESC";
                RoomFillter.KEY_WORD = txtKeyword2.Text;
                if ((long)cboChoose.EditValue == 2)
                {
                    isChoseRoom = (long)cboChoose.EditValue;
                }
                if (cboRoomType.EditValue != null)
                    RoomFillter.BACTERIUM_FAMILY_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboRoomType.EditValue ?? "0").ToString());
                var sar = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<LIS.EFMODEL.DataModels.LIS_BACTERIUM>>(
                   "api/LisBacterium/Get",
                    HIS.Desktop.ApiConsumer.ApiConsumers.LisConsumer,
                      RoomFillter,
                    param);

                lstRoomADOs = new List<LisBacteriumADO>();
                if (sar != null && sar.Data.Count > 0)
                {
                    listRoom = sar.Data;
                    foreach (var item in listRoom)
                    {
                        LisBacteriumADO roomaccountADO = new LisBacteriumADO(item);
                        if (isChoseRoom == 2)
                        {
                            roomaccountADO.isKeyChoose = true;
                        }
                        if (this.Bacterium != null && 1 == 1)
                        {
                            roomaccountADO.radio1 = true;
                        }
                        lstRoomADOs.Add(roomaccountADO);
                    }
                }

                if (BacAntibiotics != null && BacAntibiotics.Count > 0)
                {
                    foreach (var itemUsername in BacAntibiotics)
                    {
                        var check = lstRoomADOs.FirstOrDefault(o => o.ID == itemUsername.BACTERIUM_ID);
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
        private void FillDataToGridRoom(object data)
        {
            try
            {
                WaitingManager.Show();
                listRoom = new List<LIS_BACTERIUM>();
                int start1 = ((CommonParam)data).Start ?? 0;
                int limit1 = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start1, limit1);
                LIS.Filter.LisBacteriumFilter RoomFillter = new LisBacteriumFilter();
                RoomFillter.IS_ACTIVE = 1;
                RoomFillter.ORDER_FIELD = "MODIFY_TIME";
                RoomFillter.ORDER_DIRECTION = "DESC";
                RoomFillter.KEY_WORD = txtKeyword2.Text;
                if ((long)cboChoose.EditValue == 2)
                {
                    isChoseRoom = (long)cboChoose.EditValue;
                }
                if (cboRoomType.EditValue != null)
                    RoomFillter.BACTERIUM_FAMILY_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboRoomType.EditValue ?? "0").ToString());
                var sar = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<LIS.EFMODEL.DataModels.LIS_BACTERIUM>>(
                   "api/LisBacterium/Get",
                    HIS.Desktop.ApiConsumer.ApiConsumers.LisConsumer,
                      RoomFillter,
                    param);

                lstRoomADOs = new List<LisBacteriumADO>();
                if (sar != null && sar.Data.Count > 0)
                {
                    listRoom = sar.Data;
                    foreach (var item in listRoom)
                    {
                        LisBacteriumADO roomaccountADO = new LisBacteriumADO(item);
                        if (isChoseRoom == 2)
                        {
                            roomaccountADO.isKeyChoose = true;
                        }
                        if (this.Bacterium != null && 1 == 1)
                        {
                            roomaccountADO.radio1 = true;
                        }
                        lstRoomADOs.Add(roomaccountADO);
                    }
                }

                if (BacAntibiotics != null && BacAntibiotics.Count > 0)
                {
                    foreach (var itemUsername in BacAntibiotics)
                    {
                        var check = lstRoomADOs.FirstOrDefault(o => o.ID == itemUsername.BACTERIUM_ID);
                        if (check != null)
                        {
                            check.check1 = true;
                            //if (itemUsername.IS_HOLD_ORDER == 1)
                            //{
                            //    check.isHoldOrder = true;
                            //}
                            //else
                            //{
                            //    check.isHoldOrder = false;
                            //}
                            //if (itemUsername.IS_ALLOW_REQUEST == 1)
                            //{
                            //    check.isAllowRequest = true;
                            //}
                            //else
                            //{
                            //    check.isAllowRequest = false;
                            //}
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

        private void FillDataToGrid1__MedicineType(UCLisBacAntibiotic UCRoomService)
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

                FillDataToGridMedicineType(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridMedicineType, param, numPageSize);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void FillDataToGrid1_Room_Default(UCLisBacAntibiotic UCRoomService)
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

                FillDataToGridRoomData(new CommonParam(0, numPageSize));
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

        private void FillDataToGrid1__ExecuteRoom_Default(UCLisBacAntibiotic UCRoomService)
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

                FillDataToGridExecuteRoom_Default(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridExecuteRoom_Default, param, numPageSize);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridMedicineType(object data)
        {
            try
            {
                WaitingManager.Show();
                listExecuteRoom = new List<LIS_ANTIBIOTIC>();
                int start = ((CommonParam)data).Start ?? 0;
                int limit = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start, limit);
                LIS.Filter.LisAntibioticFilter ServiceFillter = new LisAntibioticFilter();
                ServiceFillter.IS_ACTIVE = 1;
                ServiceFillter.ORDER_FIELD = "MODIFY_TIME";
                ServiceFillter.ORDER_DIRECTION = "DESC";
                ServiceFillter.KEY_WORD = txtKeyword1.Text;

                if ((long)cboChoose.EditValue == 1)
                {
                    isChoseExecuteRoom = (long)cboChoose.EditValue;
                }

                var rs = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<LIS.EFMODEL.DataModels.LIS_ANTIBIOTIC>>(
                                                     "api/LisAntibiotic/Get",
                     HIS.Desktop.ApiConsumer.ApiConsumers.LisConsumer,
                     ServiceFillter,
                     param);

                lstExecuteRoomADOs = new List<AntibioticADO>();

                if (rs != null && rs.Data.Count > 0)
                {

                    listExecuteRoom = rs.Data;
                    foreach (var item in listExecuteRoom)
                    {
                        AntibioticADO RoomServiceADO = new AntibioticADO(item);
                        if (isChoseExecuteRoom == 1)
                        {
                            RoomServiceADO.isKeyChoose = true;
                        }
                        lstExecuteRoomADOs.Add(RoomServiceADO);
                    }
                }

                if (BacAntibioticViews != null && BacAntibioticViews.Count > 0)
                {
                    foreach (var itemUsername in BacAntibioticViews)
                    {
                        var check = lstExecuteRoomADOs.FirstOrDefault(o => o.ID == itemUsername.ANTIBIOTIC_ID);
                        if (check != null)
                        {
                            check.check1 = true;
                            //if (itemUsername.IS_ALLOW_REQUEST == 1)
                            //{
                            //    check.isAllowRequest = true;
                            //}
                            //else
                            //{
                            //    check.isAllowRequest = false;
                            //}
                            //if (itemUsername.IS_HOLD_ORDER == 1)
                            //{
                            //    check.isHoldOrder = true;
                            //}
                            //else
                            //{
                            //    check.isHoldOrder = false;
                            //}
                        }
                    }
                }

                lstExecuteRoomADOs = lstExecuteRoomADOs.OrderByDescending(p => p.check1).Distinct().ToList();
                if (ucGridControlExecuteRoom != null)
                {
                    AntibioticProcessor.Reload(ucGridControlExecuteRoom, lstExecuteRoomADOs);
                }
                rowCount = (data == null ? 0 : lstExecuteRoomADOs.Count);
                dataTotal = (rs.Param == null ? 0 : rs.Param.Count ?? 0);
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
                this.listRoom = new List<LIS_BACTERIUM>();
                int start = ((CommonParam)data).Start ?? 0;
                int limit = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start, limit);
                MOS.Filter.HisRoomFilter hisRoomViewFilter = new HisRoomFilter();
                hisRoomViewFilter.IS_ACTIVE = 1;
                hisRoomViewFilter.ID = this.Bacterium.ID;

                //if (cboServiceType.EditValue != null)

                //    ServiceFillter.SERVICE_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboServiceType.EditValue ?? "0").ToString());

                if ((long)cboChoose.EditValue == 2)
                {
                    isChoseRoom = (long)cboChoose.EditValue;
                }

                var rs = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<LIS.EFMODEL.DataModels.LIS_BACTERIUM>>(
                                                     "api/LisBacterium/Get",
                     HIS.Desktop.ApiConsumer.ApiConsumers.LisConsumer,
                     hisRoomViewFilter,
                     param);

                this.lstRoomADOs = new List<LisBacteriumADO>();

                if (rs != null && rs.Data.Count > 0)
                {
                    this.listRoom = new List<LIS_BACTERIUM>();
                    this.listRoom = rs.Data;
                    foreach (var item in this.listRoom)
                    {
                        LisBacteriumADO RoomServiceADO = new LisBacteriumADO(item);
                        if (isChoseRoom == 2)
                        {
                            RoomServiceADO.isKeyChoose = true;
                            RoomServiceADO.radio1 = true;
                        }
                        this.listRoom.Add(RoomServiceADO);
                    }
                }

                if (BacAntibioticViews != null && BacAntibioticViews.Count > 0)
                {
                    foreach (var itemUsername in BacAntibioticViews)
                    {
                        var check = this.lstRoomADOs.FirstOrDefault(o => o.ID == itemUsername.ID);
                        if (check != null)
                        {
                            check.check1 = true;
                        }
                    }
                }

                this.lstRoomADOs = this.lstRoomADOs.OrderByDescending(p => p.check1).ToList();
                if (ucGridControlRoom != null)
                {
                    RoomProcessor.Reload(ucGridControlRoom, this.lstRoomADOs);
                }
                rowCount = (data == null ? 0 : lstRoomADOs.Count);
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
                LisBacteriumFamilyFilter filter = new LisBacteriumFamilyFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.LIS_RS.COMMON.IS_ACTIVE__TRUE;
                var BacteriumFamilies = new BackendAdapter(param).Get<List<LIS_BACTERIUM_FAMILY>>("api/LisBacteriumFamily/Get", ApiConsumer.ApiConsumers.LisConsumer, filter, param).ToList();
                LoadDataToComboServiceType(cboRoomType, BacteriumFamilies.Where(o => o.IS_ACTIVE == 1).ToList());
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
                status.Add(new Status(1, "Kháng sinh"));
                status.Add(new Status(2, "Vi khuẩn"));

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("statusName", "", 300, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("statusName", "id", columnInfos, false, 350);
                ControlEditorLoader.Load(cboChoose, status, controlEditorADO);
                if (this.currentAntibiotic != null)
                {
                    cboChoose.EditValue = status[0].id;
                }
                else
                {
                    cboChoose.EditValue = status[1].id;
                }

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToComboServiceType(DevExpress.XtraEditors.GridLookUpEdit cboServiceType, List<LIS_BACTERIUM_FAMILY> ServiceType)
        {
            try
            {
                cboServiceType.Properties.DataSource = ServiceType;
                cboServiceType.Properties.DisplayMember = "BACTERIUM_FAMILY_NAME";
                cboServiceType.Properties.ValueMember = "ID";

                cboServiceType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboServiceType.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboServiceType.Properties.ImmediatePopup = true;
                cboServiceType.ForceInitialize();
                cboServiceType.Properties.View.Columns.Clear();

                GridColumn aColumnCode = cboServiceType.Properties.View.Columns.AddField("BACTERIUM_FAMILY_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 100;

                GridColumn aColumnName = cboServiceType.Properties.View.Columns.AddField("BACTERIUM_FAMILY_NAME");
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
                FillDataToGrid1__MedicineType(this);
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
                FillDataToGrid2__Room(this);
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
                BacAntibioticViews = null;
                BacAntibiotics = null;
                isChoseRoom = 0;
                isChoseExecuteRoom = 0;
                FillDataToGrid1__MedicineType(this);
                FillDataToGrid2__Room(this);
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
                if (ucGridControlRoom != null && ucGridControlExecuteRoom != null)
                {
                    object Bacterium = RoomProcessor.GetDataGridView(ucGridControlRoom);
                    object ExecuteRoom = AntibioticProcessor.GetDataGridView(ucGridControlExecuteRoom);
                    bool success = false;
                    CommonParam param = new CommonParam();
                    if (isChoseExecuteRoom == 1)
                    {
                        if (Bacterium is List<HIS.UC.LisBacterium.LisBacteriumADO>)
                        {
                            lstRoomADOs = (List<HIS.UC.LisBacterium.LisBacteriumADO>)Bacterium;

                            if (lstRoomADOs != null && lstRoomADOs.Count > 0)
                            {
                                //List<long> listServiceRooms = ServiceRooms.Select(p => p.SERVICE_ID).ToList();
                                if (ExecuteRoom is List<HIS.UC.LisAntibiotic.AntibioticADO>)
                                {
                                    lstExecuteRoomADOs = (List<HIS.UC.LisAntibiotic.AntibioticADO>)ExecuteRoom;
                                }

                                //HIS.UC.LisAntibiotic.AntibioticADO executeRoomViewAdo = new AntibioticADO();
                                //if (lstExecuteRoomADOs != null && lstExecuteRoomADOs.Count > 0)
                                //{
                                //    executeRoomViewAdo = lstExecuteRoomADOs.FirstOrDefault(o => o.radio1);
                                //}

                                var dataCheckeds = lstRoomADOs.Where(p => p.check1 == true).ToList();

                                //List xoa

                                var dataDeletes = lstRoomADOs.Where(o => BacAntibiotics.Select(p => p.BACTERIUM_ID)
                                    .Contains(o.ID) && o.check1 == false).ToList();


                                //list them
                                var dataCreates = dataCheckeds.Where(o => !BacAntibiotics.Select(p => p.BACTERIUM_ID)
                                    .Contains(o.ID)).ToList();

                                // List update
                                var dataUpdate = dataCheckeds.Where(o => BacAntibiotics.Select(p => p.BACTERIUM_ID)
                                    .Contains(o.ID)).ToList();

                                if (dataUpdate != null && dataUpdate.Count > 0)
                                {
                                    // List<HIS_MEDI_STOCK_MATY> stockMetyUpdates = new List<HIS_MEDI_STOCK_MATY>();
                                    List<LIS_BAC_ANTIBIOTIC> exroRoomUpdates = new List<LIS_BAC_ANTIBIOTIC>();
                                    List<LIS_BAC_ANTIBIOTIC> executeRoomErrors = new List<LIS_BAC_ANTIBIOTIC>();
                                    foreach (var item in dataUpdate)
                                    {
                                        var exroRoom = BacAntibiotics.FirstOrDefault(o => o.BACTERIUM_ID == item.ID && o.ANTIBIOTIC_ID == checkExecuteRoomId);
                                        if (exroRoom != null)
                                        {
                                            LIS_BAC_ANTIBIOTIC exroRoomResult = new LIS_BAC_ANTIBIOTIC();
                                            AutoMapper.Mapper.CreateMap<LIS_BAC_ANTIBIOTIC, LIS_BAC_ANTIBIOTIC>();
                                            exroRoomResult = AutoMapper.Mapper.Map<LIS_BAC_ANTIBIOTIC>(exroRoom);
                                            //if (item.isAllowRequest)
                                            //{
                                            //    exroRoomResult.IS_ALLOW_REQUEST = 1;
                                            //}
                                            //else
                                            //{
                                            //    exroRoomResult.IS_ALLOW_REQUEST = null;
                                            //}
                                            //if (item.isHoldOrder)
                                            //{
                                            //    exroRoomResult.IS_HOLD_ORDER = 1;
                                            //}
                                            //else
                                            //{
                                            //    exroRoomResult.IS_HOLD_ORDER = null;
                                            //}
                                            //if (exroRoomResult.IS_HOLD_ORDER == null && exroRoomResult.IS_ALLOW_REQUEST == null)
                                            //{
                                            //    executeRoomErrors.Add(exroRoomResult);
                                            //}
                                            exroRoomUpdates.Add(exroRoomResult);
                                        }
                                    }

                                    if (exroRoomUpdates != null && exroRoomUpdates.Count > 0)
                                    {
                                        //if (executeRoomErrors != null && executeRoomErrors.Count > 0)
                                        //{
                                        //    WaitingManager.Hide();
                                        //    MessageManager.Show("Phòng xử lý đang chọn phải chọn ít nhất 1 trong 2 'Được lấy số ưu tiên' và 'Được phép yêu cầu'");
                                        //    return;
                                        //}
                                        var updateResult = new BackendAdapter(param).Post<List<LIS_BAC_ANTIBIOTIC>>(
                                                   "/api/LisBacAntibiotic/UpdateList",
                                                   HIS.Desktop.ApiConsumer.ApiConsumers.LisConsumer,
                                                   exroRoomUpdates,
                                                   param);
                                        if (updateResult != null && updateResult.Count > 0)
                                        {
                                            AutoMapper.Mapper.CreateMap<LIS_BAC_ANTIBIOTIC, LIS_BAC_ANTIBIOTIC>();
                                            var vUpdateResults = AutoMapper.Mapper.Map<List<LIS_BAC_ANTIBIOTIC>, List<LIS_BAC_ANTIBIOTIC>>(updateResult);
                                            BacAntibiotics.AddRange(vUpdateResults);
                                            success = true;
                                        }
                                    }
                                }

                                if (dataDeletes.Count == 0 && dataCheckeds.Count == 0)
                                {
                                    WaitingManager.Hide();
                                    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn vi khuẩn", "Thông báo");
                                    return;
                                }
                                if (dataCheckeds != null)
                                {
                                    success = true;
                                }

                                if (dataDeletes != null && dataDeletes.Count > 0)
                                {
                                    List<LIS_BAC_ANTIBIOTIC> deleteSds = BacAntibiotics.Where(o => dataDeletes.Select(p => p.ID)
                                        .Contains(o.BACTERIUM_ID)).ToList();
                                    bool deleteResult = new BackendAdapter(param).Post<bool>(
                                              "api/LisBacAntibiotic/DeleteList",
                                              HIS.Desktop.ApiConsumer.ApiConsumers.LisConsumer,
                                              deleteSds,
                                              param);
                                    if (deleteResult)
                                        success = true;
                                    BacAntibiotics = BacAntibiotics.Where(o => !deleteSds.Select(p => p.ID).Contains(o.ID)).ToList();
                                }

                                if (dataCreates != null && dataCreates.Count > 0)
                                {
                                    List<LIS_BAC_ANTIBIOTIC> ServiceRoomCreates = new List<LIS_BAC_ANTIBIOTIC>();
                                    List<LIS_BAC_ANTIBIOTIC> executeRoomErrors = new List<LIS_BAC_ANTIBIOTIC>();
                                    foreach (var item in dataCreates)
                                    {
                                        LIS_BAC_ANTIBIOTIC ServiceRoom = new LIS_BAC_ANTIBIOTIC();
                                        ServiceRoom.ANTIBIOTIC_ID = ExecuteRoomIdCheckByExecuteRoom;
                                        ServiceRoom.BACTERIUM_ID = item.ID;
                                        //if (item.isAllowRequest)
                                        //{
                                        //    ServiceRoom.IS_ALLOW_REQUEST = 1;
                                        //}
                                        //else
                                        //{
                                        //    ServiceRoom.IS_ALLOW_REQUEST = null;
                                        //}
                                        //if (item.isHoldOrder)
                                        //{
                                        //    ServiceRoom.IS_HOLD_ORDER = 1;
                                        //}
                                        //else
                                        //{
                                        //    ServiceRoom.IS_HOLD_ORDER = null;
                                        //}
                                        //if (ServiceRoom.IS_HOLD_ORDER == null && ServiceRoom.IS_ALLOW_REQUEST == null)
                                        //{
                                        //    executeRoomErrors.Add(ServiceRoom);
                                        //}
                                        ServiceRoomCreates.Add(ServiceRoom);
                                    }
                                    //if (executeRoomErrors != null && executeRoomErrors.Count > 0)
                                    //{
                                    //    WaitingManager.Hide();
                                    //    MessageManager.Show("Phòng xử lý đang chọn phải chọn ít nhất 1 trong 2 'Được lấy số ưu tiên' và 'Được phép yêu cầu'");
                                    //    return;
                                    //}
                                    var createResult = new BackendAdapter(param).Post<List<LIS_BAC_ANTIBIOTIC>>(
                                               "api/LisBacAntibiotic/CreateList",
                                               HIS.Desktop.ApiConsumer.ApiConsumers.LisConsumer,
                                               ServiceRoomCreates,
                                               param);
                                    if (createResult != null && createResult.Count > 0)
                                        success = true;
                                    AutoMapper.Mapper.CreateMap<LIS_BAC_ANTIBIOTIC, LIS_BAC_ANTIBIOTIC>();
                                    var vCreateResults = AutoMapper.Mapper.Map<List<LIS_BAC_ANTIBIOTIC>, List<LIS_BAC_ANTIBIOTIC>>(createResult);
                                    BacAntibiotics.AddRange(vCreateResults);

                                    lstRoomADOs = lstRoomADOs.OrderByDescending(p => p.check1).ToList();
                                    if (ucGridControlRoom != null)
                                    {
                                        RoomProcessor.Reload(ucGridControlRoom, lstRoomADOs);
                                    }
                                }
                            }
                        }
                    }

                    if (isChoseRoom == 2)
                    {
                        if (ExecuteRoom is List<HIS.UC.LisAntibiotic.AntibioticADO>)
                        {
                            lstExecuteRoomADOs = (List<HIS.UC.LisAntibiotic.AntibioticADO>)ExecuteRoom;

                            if (lstExecuteRoomADOs != null && lstExecuteRoomADOs.Count > 0)
                            {
                                //List<long> listRoomServices = ServiceRoom.Select(p => p.BACTERIUM_ID).ToList();

                                var dataChecked = lstExecuteRoomADOs.Where(p => p.check1 == true).ToList();
                                //if (dataChecked == null || dataChecked.Count == 0)
                                //{
                                //    return;
                                //}

                                // List update
                                var dataUpdate = dataChecked.Where(o => BacAntibioticViews.Select(p => p.ANTIBIOTIC_ID)
                                    .Contains(o.ID)).ToList();
                                //List xoa

                                var dataDelete = lstExecuteRoomADOs.Where(o => BacAntibioticViews.Select(p => p.ANTIBIOTIC_ID)
                                    .Contains(o.ID) && o.check1 == false).ToList();

                                //list them
                                var dataCreate = dataChecked.Where(o => !BacAntibioticViews.Select(p => p.ANTIBIOTIC_ID)
                                    .Contains(o.ID)).ToList();

                                if (dataUpdate != null && dataUpdate.Count > 0)
                                {
                                    // List<HIS_MEDI_STOCK_MATY> stockMetyUpdates = new List<HIS_MEDI_STOCK_MATY>();
                                    List<LIS_BAC_ANTIBIOTIC> exroRoomUpdates = new List<LIS_BAC_ANTIBIOTIC>();
                                    List<AntibioticADO> executeRoomErrors = new List<AntibioticADO>();
                                    foreach (var item in dataUpdate)
                                    {
                                        var exroRoom = BacAntibioticViews.FirstOrDefault(o => o.ANTIBIOTIC_ID == item.ID && o.BACTERIUM_ID == checkRoomId);
                                        if (exroRoom != null)
                                        {
                                            LIS_BAC_ANTIBIOTIC exroRoomResult = new LIS_BAC_ANTIBIOTIC();
                                            AutoMapper.Mapper.CreateMap<LIS_BAC_ANTIBIOTIC, LIS_BAC_ANTIBIOTIC>();
                                            exroRoomResult = AutoMapper.Mapper.Map<LIS_BAC_ANTIBIOTIC>(exroRoom);
                                            //if (item.isAllowRequest)
                                            //{
                                            //    exroRoomResult.IS_ALLOW_REQUEST = 1;
                                            //}
                                            //else
                                            //{
                                            //    exroRoomResult.IS_ALLOW_REQUEST = null;
                                            //}
                                            //if (item.isHoldOrder)
                                            //{
                                            //    exroRoomResult.IS_HOLD_ORDER = 1;
                                            //}
                                            //else
                                            //{
                                            //    exroRoomResult.IS_HOLD_ORDER = null;
                                            //}
                                            //if (exroRoomResult.IS_HOLD_ORDER == null && exroRoomResult.IS_ALLOW_REQUEST == null)
                                            //{
                                            //    executeRoomErrors.Add(item);
                                            //}
                                            //mediStockMaty.IS_ALLOW_REQUEST = item.IS_ALLOW_REQUEST;"0");
                                            exroRoomUpdates.Add(exroRoomResult);
                                        }
                                    }
                                    if (exroRoomUpdates != null && exroRoomUpdates.Count > 0)
                                    {
                                        //if (executeRoomErrors != null && executeRoomErrors.Count > 0)
                                        //{
                                        //    string error = "";
                                        //    foreach (var item in executeRoomErrors)
                                        //    {
                                        //        error += item.ANTIBIOTIC_CODE + " - " + item.ANTIBIOTIC_CODE + "; ";
                                        //    }
                                        //    WaitingManager.Hide();
                                        //    MessageManager.Show("Các phòng sau phải chọn ít nhất 1 trong 2 'Được lấy số ưu tiên' và 'Được phép yêu cầu': " + error);
                                        //    return;
                                        //}
                                        var updateResult = new BackendAdapter(param).Post<List<LIS_BAC_ANTIBIOTIC>>(
                                                   "/api/LisBacAntibiotic/UpdateList",
                                                   HIS.Desktop.ApiConsumer.ApiConsumers.LisConsumer,
                                                   exroRoomUpdates,
                                                   param);
                                        if (updateResult != null && updateResult.Count > 0)
                                        {
                                            AutoMapper.Mapper.CreateMap<LIS_BAC_ANTIBIOTIC, LIS_BAC_ANTIBIOTIC>();
                                            var vUpdateResults = AutoMapper.Mapper.Map<List<LIS_BAC_ANTIBIOTIC>, List<LIS_BAC_ANTIBIOTIC>>(updateResult);
                                            BacAntibioticViews.AddRange(vUpdateResults);
                                            success = true;
                                        }
                                    }
                                }

                                if (dataDelete.Count == 0 && dataChecked.Count == 0)
                                {
                                    WaitingManager.Hide();
                                    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn kháng sinh", "Thông báo");
                                    return;
                                }
                                if (dataChecked != null)
                                {
                                    success = true;
                                }

                                if (dataDelete != null && dataDelete.Count > 0)
                                {

                                    List<LIS_BAC_ANTIBIOTIC> deleteObject = BacAntibioticViews.Where(o => dataDelete.Select(p => p.ID)
                                        .Contains(o.ANTIBIOTIC_ID)).ToList();
                                    bool deleteResult = new BackendAdapter(param).Post<bool>(
                                              "api/LisBacAntibiotic/DeleteList",
                                              HIS.Desktop.ApiConsumer.ApiConsumers.LisConsumer,
                                              deleteObject,
                                              param);
                                    if (deleteResult)
                                        success = true;
                                    BacAntibioticViews = BacAntibioticViews.Where(o => !deleteObject.Select(p => p.ID).Contains(o.ID)).ToList();
                                }

                                if (dataCreate != null && dataCreate.Count > 0)
                                {
                                    List<LIS_BAC_ANTIBIOTIC> ServiceRoomCreate = new List<LIS_BAC_ANTIBIOTIC>();
                                    List<AntibioticADO> executeRoomErrors = new List<AntibioticADO>();
                                    foreach (var item in dataCreate)
                                    {
                                        LIS_BAC_ANTIBIOTIC ServiceRoomID = new LIS_BAC_ANTIBIOTIC();
                                        ServiceRoomID.BACTERIUM_ID = RoomIdCheckByRoom;
                                        ServiceRoomID.ANTIBIOTIC_ID = item.ID;
                                        //if (item.isAllowRequest)
                                        //{
                                        //    ServiceRoomID.IS_ALLOW_REQUEST = 1;
                                        //}
                                        //else
                                        //{
                                        //    ServiceRoomID.IS_ALLOW_REQUEST = null;
                                        //}
                                        //if (item.isHoldOrder)
                                        //{
                                        //    ServiceRoomID.IS_HOLD_ORDER = 1;
                                        //}
                                        //else
                                        //{
                                        //    ServiceRoomID.IS_HOLD_ORDER = null;
                                        //}
                                        //if (ServiceRoomID.IS_HOLD_ORDER == null && ServiceRoomID.IS_ALLOW_REQUEST == null)
                                        //{
                                        //    executeRoomErrors.Add(item);
                                        //}
                                        ServiceRoomCreate.Add(ServiceRoomID);
                                    }
                                    //if (executeRoomErrors != null && executeRoomErrors.Count > 0)
                                    //{
                                    //    string error = "";
                                    //    foreach (var item in executeRoomErrors)
                                    //    {
                                    //        error += item.ANTIBIOTIC_NAME + " - " + item.ANTIBIOTIC_NAME + "; ";
                                    //    }
                                    //    WaitingManager.Hide();
                                    //    MessageManager.Show("Các phòng sau phải chọn ít nhất 1 trong 2 'Được lấy số ưu tiên' và 'Được phép yêu cầu': " + error);
                                    //    return;
                                    //}

                                    var createResult = new BackendAdapter(param).Post<List<LIS_BAC_ANTIBIOTIC>>(
                                               "/api/LisBacAntibiotic/CreateList",
                                               HIS.Desktop.ApiConsumer.ApiConsumers.LisConsumer,
                                               ServiceRoomCreate,
                                               param);
                                    if (createResult != null && createResult.Count > 0)
                                        success = true;
                                    AutoMapper.Mapper.CreateMap<LIS_BAC_ANTIBIOTIC, LIS_BAC_ANTIBIOTIC>();
                                    var vCreateResults = AutoMapper.Mapper.Map<List<LIS_BAC_ANTIBIOTIC>, List<LIS_BAC_ANTIBIOTIC>>(createResult);
                                    BacAntibioticViews.AddRange(vCreateResults);
                                }

                                lstExecuteRoomADOs = lstExecuteRoomADOs.OrderByDescending(p => p.check1).ToList();
                                if (ucGridControlRoom != null)
                                {
                                    AntibioticProcessor.Reload(ucGridControlExecuteRoom, lstExecuteRoomADOs);
                                }
                            }
                        }

                    }

                    WaitingManager.Hide();
                    MessageManager.Show(this.ParentForm, param, success);
                }
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
                    FillDataToGrid1__MedicineType(this);

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
                    FillDataToGrid2__Room(this);
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
                    cboRoomType.Focus();
                    cboRoomType.ShowPopup();
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

        private void cboRoomType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboRoomType.EditValue != null)
                    {
                        LIS_BACTERIUM_FAMILY data = RoomType.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboRoomType.EditValue.ToString()));
                        if (data != null)
                        {
                            cboRoomType.Properties.Buttons[1].Visible = true;
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
                    cboRoomType.EditValue = null;
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
                if (cboRoomType.EditValue != null)
                {
                    LIS_BACTERIUM_FAMILY data = RoomType.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboRoomType.EditValue.ToString()));
                    if (data != null)
                    {
                        cboRoomType.Properties.Buttons[1].Visible = true;
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
                if (this.currentAntibiotic == null)
                {
                    FillDataToGrid1__MedicineType(this);
                    FillDataToGrid2__Room(this);
                }
                else
                {
                    FillDataToGrid1__ExecuteRoom_Default(this);
                    FillDataToGrid1__MedicineType(this);
                    LIS.Filter.LisBacteriumFilter filter = new LIS.Filter.LisBacteriumFilter();

                    var room = BackendDataWorker.Get<LIS_BACTERIUM>().FirstOrDefault(o => o.ID == this.currentAntibiotic.ID);//TODO
                    btn_Radio_Enable_Click(room);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void MedicineTypeGridView_MouseRightClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //try
            //{
            //    if ((e.Item is BarButtonItem) && sender != null && sender is HIS.UC.LisAntibiotic.AntibioticADO)
            //    {
            //        var type = (HIS.UC.LisAntibiotic.Popup.PopupMenuProcessor.ItemType)e.Item.Tag;
            //        switch (type)
            //        {
            //            case HIS.UC.LisAntibiotic.Popup.PopupMenuProcessor.ItemType.Copy:
            //                {
            //                    if (isChoseService != 1)
            //                    {
            //                        MessageManager.Show("Vui lòng chọn dịch vụ!");
            //                        break;
            //                    }
            //                    this.currentCopyServiceAdo = (HIS.UC.LisAntibiotic.AntibioticADO)sender;
            //                    break;
            //                }
            //            case HIS.UC.LisAntibiotic.Popup.PopupMenuProcessor.ItemType.Paste:
            //                {
            //                    var currentPaste = (HIS.UC.LisAntibiotic.AntibioticADO)sender;
            //                    bool success = false;
            //                    CommonParam param = new CommonParam();
            //                    if (this.currentCopyServiceAdo == null && isChoseService != 1)
            //                    {
            //                        MessageManager.Show("Vui lòng copy!");
            //                        break;
            //                    }
            //                    if (this.currentCopyServiceAdo != null && currentPaste != null && isChoseService == 1)
            //                    {
            //                        if (this.currentCopyServiceAdo.ID == currentPaste.ID)
            //                        {
            //                            MessageManager.Show("Trùng dữ liệu copy và paste");
            //                            break;
            //                        }
            //                        HisServiceRoomCopyByServiceSDO hisMestMatyCopyByMatySDO = new HisServiceRoomCopyByServiceSDO();
            //                        hisMestMatyCopyByMatySDO.CopyServiceId = this.currentCopyServiceAdo.ID;
            //                        hisMestMatyCopyByMatySDO.PasteServiceId = currentPaste.ID;
            //                        var result = new BackendAdapter(param).Post<List<LIS_BAC_ANTIBIOTIC>>("api/HisServiceRoom/CopyByService", ApiConsumer.ApiConsumers.MosConsumer, hisMestMatyCopyByMatySDO, param);
            //                        if (result != null)
            //                        {
            //                            success = true;

            //                        }
            //                    }
            //                    MessageManager.Show(this.ParentForm, param, success);
            //                    break;
            //                }
            //            default:
            //                break;
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //}
        }

        private void RoomGridView_MouseRightClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if ((e.Item is BarButtonItem) && sender != null && sender is HIS.UC.LisBacterium.LisBacteriumADO)
                {
                    var type = (HIS.UC.LisBacterium.Popup.PopupMenuProcessor.ItemType)e.Item.Tag;
                    switch (type)
                    {
                        case HIS.UC.LisBacterium.Popup.PopupMenuProcessor.ItemType.CopyPhongSangPhong:
                            {
                                if (isChoseRoom != 2)
                                {
                                    MessageManager.Show("Vui lòng chọn phòng!");
                                    break;
                                }
                                this.CurrentRoomCopyAdo = (HIS.UC.LisBacterium.LisBacteriumADO)sender;
                                break;
                            }
                        case HIS.UC.LisBacterium.Popup.PopupMenuProcessor.ItemType.PastePhongSangPhong:
                            {
                                var currentPaste = (HIS.UC.LisBacterium.LisBacteriumADO)sender;
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
                                    HisExroRoomCopyByRoomSDO hisMestMatyCopyByMatySDO = new HisExroRoomCopyByRoomSDO();
                                    hisMestMatyCopyByMatySDO.CopyRoomId = this.CurrentRoomCopyAdo.ID;
                                    hisMestMatyCopyByMatySDO.PasteRoomId = currentPaste.ID;
                                    var result = new BackendAdapter(param).Post<List<LIS_BAC_ANTIBIOTIC>>("api/HisExroRoom/CopyByRoom", ApiConsumer.ApiConsumers.MosConsumer, hisMestMatyCopyByMatySDO, param);
                                    if (result != null)
                                    {
                                        success = true;
                                        List<HIS.UC.LisAntibiotic.AntibioticADO> dataNew = new List<HIS.UC.LisAntibiotic.AntibioticADO>();
                                        dataNew = (from r in listExecuteRoom select new HIS.UC.LisAntibiotic.AntibioticADO(r)).ToList();
                                        if (result != null && result.Count > 0)
                                        {

                                            foreach (var itemService in result)
                                            {
                                                var check = dataNew.FirstOrDefault(o => o.ID == itemService.ANTIBIOTIC_ID);
                                                if (check != null)
                                                {
                                                    check.check1 = true;
                                                    //if (itemService.IS_HOLD_ORDER == 1)
                                                    //{
                                                    //    check.isHoldOrder = true;
                                                    //}
                                                    //else
                                                    //{
                                                    //    check.isHoldOrder = false;
                                                    //}
                                                    //if (itemService.IS_ALLOW_REQUEST == 1)
                                                    //{
                                                    //    check.isAllowRequest = true;
                                                    //}
                                                    //else
                                                    //{
                                                    //    check.isAllowRequest = false;
                                                    //}
                                                }
                                            }


                                            dataNew = dataNew.OrderByDescending(p => p.check1).ToList();


                                            if (ucGridControlExecuteRoom != null)
                                            {
                                                AntibioticProcessor.Reload(ucGridControlExecuteRoom, dataNew);
                                            }
                                        }
                                        else
                                        {
                                            FillDataToGrid1__MedicineType(this);
                                        }
                                        BacAntibioticViews = new List<LIS_BAC_ANTIBIOTIC>();
                                        AutoMapper.Mapper.CreateMap<LIS_BAC_ANTIBIOTIC, LIS_BAC_ANTIBIOTIC>();
                                        BacAntibioticViews = AutoMapper.Mapper.Map<List<LIS_BAC_ANTIBIOTIC>>(result);
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

        private void ExecuteRoomGridView_MouseRightClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if ((e.Item is BarButtonItem) && sender != null && sender is HIS.UC.LisAntibiotic.AntibioticADO)
                {
                    var type = (HIS.UC.LisAntibiotic.Popup.PopupMenuProcessor.ItemType)e.Item.Tag;
                    switch (type)
                    {
                        case HIS.UC.LisAntibiotic.Popup.PopupMenuProcessor.ItemType.Copy:
                            {
                                if (isChoseExecuteRoom != 1)
                                {
                                    MessageManager.Show("Vui lòng chọn phòng xử lý!");
                                    break;
                                }
                                this.currentAntibiotic = (HIS.UC.LisAntibiotic.AntibioticADO)sender;
                                break;
                            }
                        case HIS.UC.LisAntibiotic.Popup.PopupMenuProcessor.ItemType.Paste:
                            {
                                var currentPaste = (HIS.UC.LisAntibiotic.AntibioticADO)sender;
                                bool success = false;
                                CommonParam param = new CommonParam();
                                if (this.currentAntibiotic == null && isChoseExecuteRoom != 1)
                                {
                                    MessageManager.Show("Vui lòng copy!");
                                    break;
                                }
                                if (this.currentAntibiotic != null && currentPaste != null && isChoseExecuteRoom == 1)
                                {
                                    if (this.currentAntibiotic.ID == currentPaste.ID)
                                    {
                                        MessageManager.Show("Trùng dữ liệu copy và paste");
                                        break;
                                    }
                                    HisExroRoomCopyByExroSDO hisMestMatyCopyByMatySDO = new HisExroRoomCopyByExroSDO();
                                    hisMestMatyCopyByMatySDO.CopyExecuteRoomId = this.currentAntibiotic.ID;
                                    hisMestMatyCopyByMatySDO.PasteExecuteRoomId = currentPaste.ID;
                                    var result = new BackendAdapter(param).Post<List<LIS_BAC_ANTIBIOTIC>>("api/HisExroRoom/CopyByExecuteRoom", ApiConsumer.ApiConsumers.MosConsumer, hisMestMatyCopyByMatySDO, param);
                                    if (result != null)
                                    {
                                        success = true;
                                        List<HIS.UC.LisBacterium.LisBacteriumADO> dataNew = new List<HIS.UC.LisBacterium.LisBacteriumADO>();
                                        dataNew = (from r in listRoom select new LisBacteriumADO(r)).ToList();
                                        if (result != null && result.Count > 0)
                                        {
                                            foreach (var itemRoom in result)
                                            {
                                                var check = dataNew.FirstOrDefault(o => o.ID == itemRoom.BACTERIUM_ID);
                                                if (check != null)
                                                {
                                                    check.check1 = true;
                                                    //if (itemRoom.IS_ALLOW_REQUEST == 1)
                                                    //{
                                                    //    check.isAllowRequest = true;
                                                    //}
                                                    //else
                                                    //{
                                                    //    check.isAllowRequest = false;
                                                    //}
                                                    //if (itemRoom.IS_HOLD_ORDER == 1)
                                                    //{
                                                    //    check.isHoldOrder = true;
                                                    //}
                                                    //else
                                                    //{
                                                    //    check.isHoldOrder = false;
                                                    //}
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
                                            FillDataToGrid2__Room(this);
                                        }
                                        BacAntibiotics = new List<LIS_BAC_ANTIBIOTIC>();
                                        AutoMapper.Mapper.CreateMap<LIS_BAC_ANTIBIOTIC, LIS_BAC_ANTIBIOTIC>();
                                        BacAntibiotics = AutoMapper.Mapper.Map<List<LIS_BAC_ANTIBIOTIC>>(result);
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

        private void panelControl2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}







