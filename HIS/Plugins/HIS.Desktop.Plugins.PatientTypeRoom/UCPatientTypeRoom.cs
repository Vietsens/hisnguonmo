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
using HIS.UC.PatientType;
using HIS.UC.PatientType.ADO;
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
using HIS.Desktop.Plugins.PatientTypeRoom.Entity;

namespace HIS.Desktop.Plugins.PatientTypeRoom
{
    public partial class UCPatientTypeRoom : HIS.Desktop.Utility.UserControlBase
    {
        List<HIS_ROOM_TYPE> RoomType { get; set; }

        internal Inventec.Desktop.Common.Modules.Module currentModule;
        UCRoomProcessor RoomProcessor;
        UCPatientTypeProcessor PatientTypeProcessor;
        UserControl ucGridControlPatientType;
        UserControl ucGridControlRoom;
        int rowCount = 0;
        int dataTotal = 0;
        int rowCount1 = 0;
        int dataTotal1 = 0;
        internal List<HIS.UC.Room.RoomAccountADO> lstRoomADOs { get; set; }
        internal List<HIS.UC.PatientType.PatientTypeADO> lstPatientTypeADOs { get; set; }
        List<V_HIS_ROOM> listRoom;
        List<HIS_PATIENT_TYPE> listPatientType;
        long PatientTypeIdCheckByPatientType = 0;
        long isChosePatientType;
        long isChoseRoom;
        long RoomIdCheckByRoom;
        bool isCheckAll;
        List<HIS_PATIENT_TYPE_ROOM> PatientTypeRooms { get; set; }
        List<HIS_PATIENT_TYPE_ROOM> PatientTypeRoomViews { get; set; }
        V_HIS_EXECUTE_ROOM currentExecuteRoom;
        HIS.UC.PatientType.PatientTypeADO currentCopyPatientTypeAdo;
        HIS.UC.Room.RoomAccountADO CurrentRoomCopyAdo;

        public UCPatientTypeRoom(Inventec.Desktop.Common.Modules.Module currentModule)
            : base(currentModule)
        {
            InitializeComponent();

        }

        public UCPatientTypeRoom(Inventec.Desktop.Common.Modules.Module currentModule, long RoomType)
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

        public UCPatientTypeRoom(V_HIS_EXECUTE_ROOM PatientTypeData, Inventec.Desktop.Common.Modules.Module currentModule)
            : base(currentModule)
        {
            InitializeComponent();
            try
            {
                this.currentExecuteRoom = PatientTypeData;
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
                InitUcgridViewPatientType();
                InitUcgridViewRoom();
                if (this.currentExecuteRoom == null)
                {
                    FillDataToGrid1__PatientType(this);
                    FillDataToGrid2__Room(this);
                }
                else
                {
                    FillDataToGrid1__Room_Default(this);
                    FillDataToGrid1__PatientType(this);
                    var room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == currentExecuteRoom.ROOM_ID);
                    if (room != null)
                    {
                        btn_Radio_Enable_Click(room);
                        cboRoomType.EditValue = room.ROOM_TYPE_ID;
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

        private void gridViewPatientType_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (isChosePatientType == 1)
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
                        if (hi.Column.FieldName == "checkMedi")
                        {
                            var lstCheckAll = lstPatientTypeADOs;
                            List<HIS.UC.PatientType.PatientTypeADO> lstChecks = new List<HIS.UC.PatientType.PatientTypeADO>();

                            if (lstCheckAll != null && lstCheckAll.Count > 0)
                            {
                                var ServiceCheckedNum = lstPatientTypeADOs.Where(o => o.checkMedi == true).Count();
                                var ServiceNum = lstPatientTypeADOs.Count();
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
                                            item.checkMedi = true;
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
                                            item.checkMedi = false;
                                            lstChecks.Add(item);
                                        }
                                        else
                                        {
                                            lstChecks.Add(item);
                                        }
                                    }
                                    isCheckAll = true;
                                }

                                PatientTypeProcessor.Reload(ucGridControlPatientType, lstChecks);


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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.PatientTypeRoom.Resources.Lang", typeof(HIS.Desktop.Plugins.PatientTypeRoom.UCPatientTypeRoom).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCPatientTypeRoom.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboChoose.Properties.NullText = Inventec.Common.Resource.Get.Value("UCPatientTypeRoom.cboChoose.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("UCPatientTypeRoom.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UCPatientTypeRoom.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnFind2.Text = Inventec.Common.Resource.Get.Value("UCPatientTypeRoom.btnFind2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("UCPatientTypeRoom.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnFind1.Text = Inventec.Common.Resource.Get.Value("UCPatientTypeRoom.btnFind1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboRoomType.Properties.NullText = Inventec.Common.Resource.Get.Value("UCPatientTypeRoom.cboServiceType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword2.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCPatientTypeRoom.txtKeyword2.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword1.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCPatientTypeRoom.txtKeyword1.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciRoomType.Text = Inventec.Common.Resource.Get.Value("UCPatientTypeRoom.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("UCPatientTypeRoom.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar2.Text = Inventec.Common.Resource.Get.Value("UCPatientTypeRoom.bar2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitUcgridViewPatientType()
        {
            try
            {
                PatientTypeProcessor = new UCPatientTypeProcessor();
                PatientTypeInitADO ado = new PatientTypeInitADO();
                ado.ListPatientTypeColumn = new List<UC.PatientType.PatientTypeColumn>();
                ado.gridViewPatientType_MouseDownMedi = gridViewPatientType_MouseDown;
                ado.btn_Radio_Enable_Click_Medi = btn_Radio_Enable_Click1;
                ado.gridView_MouseRightClick = PatientTypeGridView_MouseRightClick;

                PatientTypeColumn colRadio2 = new PatientTypeColumn("   ", "radioMedi", 30, true);
                colRadio2.VisibleIndex = 0;
                colRadio2.Visible = false;
                colRadio2.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListPatientTypeColumn.Add(colRadio2);

                PatientTypeColumn colCheck2 = new PatientTypeColumn("   ", "checkMedi", 30, true);
                colCheck2.VisibleIndex = 1;
                colCheck2.image = imageCollectionService.Images[0];
                colCheck2.Visible = false;
                colCheck2.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListPatientTypeColumn.Add(colCheck2);

                PatientTypeColumn colMaDichvu = new PatientTypeColumn("Mã đối tượng", "PATIENT_TYPE_CODE", 60, false);
                colMaDichvu.VisibleIndex = 2;
                ado.ListPatientTypeColumn.Add(colMaDichvu);

                PatientTypeColumn colTenDichvu = new PatientTypeColumn("Tên đối tượng", "PATIENT_TYPE_NAME", 300, false);
                colTenDichvu.VisibleIndex = 3;
                ado.ListPatientTypeColumn.Add(colTenDichvu);

                //PatientTypeColumn colMaLoaidichvu = new PatientTypeColumn("Đơn vị tính", "SERVICE_UNIT_NAME", 80, false);
                //colMaLoaidichvu.VisibleIndex = 4;
                //ado.ListPatientTypeColumn.Add(colMaLoaidichvu);

                this.ucGridControlPatientType = (UserControl)PatientTypeProcessor.Run(ado);
                if (ucGridControlPatientType != null)
                {
                    this.panelControl1.Controls.Add(this.ucGridControlPatientType);
                    this.ucGridControlPatientType.Dock = DockStyle.Fill;
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

        private void btn_Radio_Enable_Click1(HIS_PATIENT_TYPE data)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                MOS.Filter.HisPatientTypeRoomFilter filter = new HisPatientTypeRoomFilter();
                filter.PATIENT_TYPE_ID = data.ID;
                //if (this.cboRoomType.EditValue != null&&(long)this.cboRoomType.EditValue!=0)
                //{ 
                //filter.
                //}
                PatientTypeIdCheckByPatientType = data.ID;

                PatientTypeRooms = new BackendAdapter(param).Get<List<HIS_PATIENT_TYPE_ROOM>>(
                                    "api/HisPatientTypeRoom/Get",
                                HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                filter,
                                param);
                List<HIS.UC.Room.RoomAccountADO> dataNew = new List<HIS.UC.Room.RoomAccountADO>();
                dataNew = (from r in listRoom select new RoomAccountADO(r)).ToList();
                if (PatientTypeRooms != null && PatientTypeRooms.Count > 0)
                {
                    foreach (var itemRoom in PatientTypeRooms)
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

                RoomColumn colLoaiPhong = new RoomColumn("Loại phòng", "ROOM_TYPE_NAME", 100, false);
                colLoaiPhong.VisibleIndex = 4;
                ado.ListRoomColumn.Add(colLoaiPhong);

                RoomColumn colKhoa = new RoomColumn("Khoa", "DEPARTMENT_NAME", 100, false);
                colKhoa.VisibleIndex = 5;
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
                MOS.Filter.HisPatientTypeRoomFilter filter = new HisPatientTypeRoomFilter();
                filter.ROOM_ID = data.ID;
                RoomIdCheckByRoom = data.ID;
                PatientTypeRoomViews = new BackendAdapter(param).Get<List<HIS_PATIENT_TYPE_ROOM>>(
                                         "api/HisPatientTypeRoom/Get",

                                HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                filter,
                                param);
                List<HIS.UC.PatientType.PatientTypeADO> dataNew = new List<HIS.UC.PatientType.PatientTypeADO>();
                dataNew = (from r in listPatientType select new HIS.UC.PatientType.PatientTypeADO(r)).ToList();
                if (PatientTypeRoomViews != null && PatientTypeRoomViews.Count > 0)
                {

                    foreach (var itemService in PatientTypeRoomViews)
                    {
                        var check = dataNew.FirstOrDefault(o => o.ID == itemService.PATIENT_TYPE_ID);
                        if (check != null)
                        {
                            check.checkMedi = true;
                        }
                    }

                    dataNew = dataNew.OrderByDescending(p => p.checkMedi).ToList();

                    if (ucGridControlPatientType != null)
                    {
                        PatientTypeProcessor.Reload(ucGridControlPatientType, dataNew);
                    }
                }
                else
                {
                    FillDataToGrid1__PatientType(this);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGrid2__Room(UCPatientTypeRoom uCRoomService)
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
                if (cboRoomType.EditValue != null)
                    RoomFillter.ROOM_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboRoomType.EditValue ?? "0").ToString());
                var sar = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_ROOM>>(
                   HIS.Desktop.ApiConsumer.HisRequestUriStore.HIS_ROOM_GETVIEW,
                    HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                      RoomFillter,
                    param);
                List<long> listRadio = (lstRoomADOs ?? new List<RoomAccountADO>()).Where(o => o.radio1 == true).Select(p => p.ID).ToList();
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
                        lstRoomADOs.Add(roomaccountADO);
                    }
                }

                if (PatientTypeRooms != null && PatientTypeRooms.Count > 0)
                {
                    foreach (var itemUsername in PatientTypeRooms)
                    {
                        var check = lstRoomADOs.FirstOrDefault(o => o.ID == itemUsername.ROOM_ID);
                        if (check != null)
                        {
                            check.check1 = true;
                        }
                    }
                }
                if (listRadio != null && listRadio.Count > 0)
                {
                    foreach (var rd in listRadio)
                    {
                        var radio = lstRoomADOs.FirstOrDefault(o => o.ID == rd);
                        if (radio != null)
                        {
                            radio.radio1 = radio.isKeyChoose;
                        }
                    }
                }
                lstRoomADOs = lstRoomADOs.OrderByDescending(p => p.check1).ThenByDescending(p => p.radio1).Distinct().ToList();

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

        private void FillDataToGrid1__PatientType(UCPatientTypeRoom UCRoomService)
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

                FillDataToGridPatientType(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridPatientType, param, numPageSize);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGrid1__Room_Default(UCPatientTypeRoom UCRoomService)
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

                FillDataToGridRoom_Default(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging2.Init(FillDataToGridRoom_Default, param, numPageSize);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridPatientType(object data)
        {
            try
            {
                WaitingManager.Show();
                listPatientType = new List<HIS_PATIENT_TYPE>();
                int start = ((CommonParam)data).Start ?? 0;
                int limit = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start, limit);
                MOS.Filter.HisPatientTypeFilter ServiceFillter = new HisPatientTypeFilter();
                ServiceFillter.IS_ACTIVE = 1;
                ServiceFillter.ORDER_FIELD = "MODIFY_TIME";
                ServiceFillter.ORDER_DIRECTION = "DESC";
                ServiceFillter.KEY_WORD = txtKeyword1.Text;

                if ((long)cboChoose.EditValue == 1)
                {
                    isChosePatientType = (long)cboChoose.EditValue;
                }

                var rs = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>>(
                                                     "api/HisPatientType/Get",
                     HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                     ServiceFillter,
                     param);
                List<long> listRadio = (lstPatientTypeADOs ?? new List<PatientTypeADO>()).Where(o => o.radioMedi == true).Select(p => p.ID).ToList();
                lstPatientTypeADOs = new List<PatientTypeADO>();

                if (rs != null && rs.Data.Count > 0)
                {

                    listPatientType = rs.Data;
                    foreach (var item in listPatientType)
                    {
                        PatientTypeADO RoomServiceADO = new PatientTypeADO(item);
                        if (isChosePatientType == 1)
                        {
                            RoomServiceADO.isKeyChooseMedi = true;
                        }
                        lstPatientTypeADOs.Add(RoomServiceADO);
                    }
                }

                if (PatientTypeRoomViews != null && PatientTypeRoomViews.Count > 0)
                {
                    foreach (var itemUsername in PatientTypeRoomViews)
                    {
                        var check = lstPatientTypeADOs.FirstOrDefault(o => o.ID == itemUsername.PATIENT_TYPE_ID);
                        if (check != null)
                        {
                            check.checkMedi = true;
                        }
                    }
                }

                if (listRadio != null && listRadio.Count > 0)
                {
                    foreach (var rd in listRadio)
                    {
                        var check = lstPatientTypeADOs.FirstOrDefault(o => o.ID == rd);
                        if (check != null)
                        {
                            check.radioMedi = check.isKeyChooseMedi;
                        }
                    }
                }

                lstPatientTypeADOs = lstPatientTypeADOs.OrderByDescending(p => p.checkMedi).ThenByDescending(p => p.radioMedi).Distinct().ToList();
                if (ucGridControlPatientType != null)
                {
                    PatientTypeProcessor.Reload(ucGridControlPatientType, lstPatientTypeADOs);
                }
                rowCount = (data == null ? 0 : lstPatientTypeADOs.Count);
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
                listRoom = new List<V_HIS_ROOM>();
                int start = ((CommonParam)data).Start ?? 0;
                int limit = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start, limit);
                MOS.Filter.HisRoomViewFilter hisRoomViewFilter = new HisRoomViewFilter();
                hisRoomViewFilter.IS_ACTIVE = 1;
                hisRoomViewFilter.ID = this.currentExecuteRoom.ROOM_ID;

                //if (cboServiceType.EditValue != null)

                //    ServiceFillter.SERVICE_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboServiceType.EditValue ?? "0").ToString());

                if ((long)cboChoose.EditValue == 2)
                {
                    isChoseRoom = (long)cboChoose.EditValue;
                }

                var rs = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_ROOM>>(
                                                     "api/HisRoom/GetView",
                     HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                     hisRoomViewFilter,
                     param);

                this.lstRoomADOs = new List<RoomAccountADO>();

                if (rs != null && rs.Data.Count > 0)
                {
                    this.listRoom = new List<V_HIS_ROOM>();
                    this.listRoom = rs.Data;
                    foreach (var item in this.listRoom)
                    {
                        RoomAccountADO RoomServiceADO = new RoomAccountADO(item);
                        if (isChoseRoom == 2)
                        {
                            RoomServiceADO.isKeyChoose = true;
                            RoomServiceADO.radio1 = true;
                        }
                        this.lstRoomADOs.Add(RoomServiceADO);
                    }
                }

                if (PatientTypeRoomViews != null && PatientTypeRoomViews.Count > 0)
                {
                    foreach (var itemUsername in PatientTypeRoomViews)
                    {
                        var check = this.lstRoomADOs.FirstOrDefault(o => o.ID == itemUsername.ROOM_ID);
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
                RoomType = BackendDataWorker.Get<HIS_ROOM_TYPE>().Where(o => o.IS_ACTIVE == 1).ToList();
                LoadDataToComboServiceType(cboRoomType, RoomType);
                cboRoomType.EditValue = IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__XL;
                cboRoomType.Enabled = false;

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
                status.Add(new Status(1, "Đối tượng"));
                status.Add(new Status(2, "Phòng"));

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("statusName", "", 300, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("statusName", "id", columnInfos, false, 350);
                ControlEditorLoader.Load(cboChoose, status, controlEditorADO);
                cboChoose.EditValue = status[1].id;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToComboServiceType(DevExpress.XtraEditors.GridLookUpEdit cboServiceType, List<HIS_ROOM_TYPE> ServiceType)
        {
            try
            {
                cboServiceType.Properties.DataSource = ServiceType;
                cboServiceType.Properties.DisplayMember = "ROOM_TYPE_NAME";
                cboServiceType.Properties.ValueMember = "ID";

                cboServiceType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboServiceType.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboServiceType.Properties.ImmediatePopup = true;
                cboServiceType.ForceInitialize();
                cboServiceType.Properties.View.Columns.Clear();

                GridColumn aColumnCode = cboServiceType.Properties.View.Columns.AddField("ROOM_TYPE_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 100;

                GridColumn aColumnName = cboServiceType.Properties.View.Columns.AddField("ROOM_TYPE_NAME");
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
                FillDataToGrid1__PatientType(this);
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
                PatientTypeRoomViews = null;
                PatientTypeRooms = null;
                isChoseRoom = 0;
                isChosePatientType = 0;
                FillDataToGrid1__PatientType(this);
                FillDataToGrid2__Room(this);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            bool success = false;
            CommonParam param = new CommonParam();
            try
            {

                if (ucGridControlRoom != null && ucGridControlPatientType != null)
                {
                    object Room = RoomProcessor.GetDataGridView(ucGridControlRoom);
                    object Service = PatientTypeProcessor.GetDataGridView(ucGridControlPatientType);
                    if (isChosePatientType == 1)
                    {
                        if (this.lstPatientTypeADOs == null || !this.lstPatientTypeADOs.Exists(o => o.radioMedi))
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn đối tượng", "Thông báo");
                            return;
                        }
                        if (Room is List<HIS.UC.Room.RoomAccountADO>)
                        {
                            lstRoomADOs = (List<HIS.UC.Room.RoomAccountADO>)Room;

                            if (lstRoomADOs != null && lstRoomADOs.Count > 0)
                            {
                                //List<long> listServiceRooms = ServiceRooms.Select(p => p.SERVICE_ID).ToList();

                                var dataCheckeds = lstRoomADOs.Where(p => p.check1 == true).ToList();

                                //       //List xoa

                                var dataDeletes = lstRoomADOs.Where(o => PatientTypeRooms.Select(p => p.ROOM_ID)
                                    .Contains(o.ID) && o.check1 == false).ToList();


                                //list them
                                var dataCreates = dataCheckeds.Where(o => !PatientTypeRooms.Select(p => p.ROOM_ID)
                                    .Contains(o.ID)).ToList();
                                if (dataDeletes.Count == 0 && dataCreates.Count == 0)
                                {
                                    if (PatientTypeRooms.Where(o => lstRoomADOs.Exists(p => p.ID == o.ROOM_ID)).ToList().Count == 0)
                                    {
                                        MessageBox.Show("Chưa chọn phòng");
                                        return;
                                    }
                                    else
                                    {
                                        MessageBox.Show("Đối tượng đã thiết lập cho các phòng được chọn");
                                        return;
                                    }
                                }
                                if (dataCheckeds != null)
                                {
                                    success = true;
                                }
                                WaitingManager.Show();
                                if (dataDeletes != null && dataDeletes.Count > 0)
                                {
                                    List<long> deleteSds = PatientTypeRooms.Where(o => dataDeletes.Select(p => p.ID)
                                        .Contains(o.ROOM_ID)).Select(o => o.ID).ToList();
                                    bool deleteResult = new BackendAdapter(param).Post<bool>(
                                              "api/HisPatientTypeRoom/DeleteList",
                                              HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                              deleteSds,
                                              param);
                                    if (deleteResult)
                                        success = true;
                                    PatientTypeRooms = PatientTypeRooms.Where(o => !deleteSds.Contains(o.ID)).ToList();
                                }

                                if (dataCreates != null && dataCreates.Count > 0)
                                {
                                    List<HIS_PATIENT_TYPE_ROOM> ServiceRoomCreates = new List<HIS_PATIENT_TYPE_ROOM>();
                                    foreach (var item in dataCreates)
                                    {
                                        HIS_PATIENT_TYPE_ROOM ServiceRoom = new HIS_PATIENT_TYPE_ROOM();
                                        ServiceRoom.PATIENT_TYPE_ID = PatientTypeIdCheckByPatientType;
                                        ServiceRoom.ROOM_ID = item.ID;
                                        ServiceRoomCreates.Add(ServiceRoom);
                                    }

                                    var createResult = new BackendAdapter(param).Post<List<HIS_PATIENT_TYPE_ROOM>>(
                                               "api/HisPatientTypeRoom/CreateList",
                                               HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                               ServiceRoomCreates,
                                               param);
                                    if (createResult != null && createResult.Count > 0)
                                        success = true;
                                    AutoMapper.Mapper.CreateMap<HIS_PATIENT_TYPE_ROOM, HIS_PATIENT_TYPE_ROOM>();
                                    var vCreateResults = AutoMapper.Mapper.Map<List<HIS_PATIENT_TYPE_ROOM>, List<HIS_PATIENT_TYPE_ROOM>>(createResult);
                                    PatientTypeRooms.AddRange(vCreateResults);
                                }

                                WaitingManager.Hide();
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
                        if (this.lstRoomADOs == null || !this.lstRoomADOs.Exists(o => o.radio1))
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn phòng", "Thông báo");
                            return;
                        }
                        if (Service is List<HIS.UC.PatientType.PatientTypeADO>)
                        {
                            lstPatientTypeADOs = (List<HIS.UC.PatientType.PatientTypeADO>)Service;

                            if (lstPatientTypeADOs != null && lstPatientTypeADOs.Count > 0)
                            {
                                //List<long> listRoomServices = ServiceRoom.Select(p => p.ROOM_ID).ToList();

                                var dataChecked = lstPatientTypeADOs.Where(p => p.checkMedi == true).ToList();
                                //List xoa

                                var dataDelete = lstPatientTypeADOs.Where(o => PatientTypeRoomViews.Select(p => p.PATIENT_TYPE_ID)
                                    .Contains(o.ID) && o.checkMedi == false).ToList();

                                //list them
                                var dataCreate = dataChecked.Where(o => !PatientTypeRoomViews.Select(p => p.PATIENT_TYPE_ID)
                                    .Contains(o.ID)).ToList();


                                if (dataDelete.Count == 0 && dataCreate.Count == 0)
                                {
                                    if (PatientTypeRoomViews.Where(o => lstPatientTypeADOs.Exists(p => p.ID == o.PATIENT_TYPE_ID)).ToList().Count == 0)
                                    {
                                        MessageBox.Show("Chưa chọn đối tượng");
                                        return;
                                    }
                                    else
                                    {
                                        MessageBox.Show("Phòng đã thiết lập cho các đối tượng được chọn");
                                        return;
                                    }
                                }
                                if (dataChecked != null)
                                {
                                    success = true;
                                }
                                WaitingManager.Show();
                                if (dataDelete != null && dataDelete.Count > 0)
                                {

                                    List<long> deleteId = PatientTypeRoomViews.Where(o => dataDelete.Select(p => p.ID)
                                        .Contains(o.PATIENT_TYPE_ID)).Select(o => o.ID).ToList();
                                    bool deleteResult = new BackendAdapter(param).Post<bool>(
                                              "api/HisPatientTypeRoom/DeleteList",
                                              HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                              deleteId,
                                              param);
                                    if (deleteResult)
                                        success = true;
                                    PatientTypeRoomViews = PatientTypeRoomViews.Where(o => !deleteId.Contains(o.ID)).ToList();
                                }

                                if (dataCreate != null && dataCreate.Count > 0)
                                {
                                    List<HIS_PATIENT_TYPE_ROOM> ServiceRoomCreate = new List<HIS_PATIENT_TYPE_ROOM>();
                                    foreach (var item in dataCreate)
                                    {
                                        HIS_PATIENT_TYPE_ROOM ServiceRoomID = new HIS_PATIENT_TYPE_ROOM();
                                        ServiceRoomID.ROOM_ID = RoomIdCheckByRoom;
                                        ServiceRoomID.PATIENT_TYPE_ID = item.ID;
                                        ServiceRoomCreate.Add(ServiceRoomID);
                                    }

                                    var createResult = new BackendAdapter(param).Post<List<HIS_PATIENT_TYPE_ROOM>>(
                                               "/api/HisPatientTypeRoom/CreateList",
                                               HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                               ServiceRoomCreate,
                                               param);
                                    if (createResult != null && createResult.Count > 0)
                                        success = true;
                                    AutoMapper.Mapper.CreateMap<HIS_PATIENT_TYPE_ROOM, HIS_PATIENT_TYPE_ROOM>();
                                    var vCreateResults = AutoMapper.Mapper.Map<List<HIS_PATIENT_TYPE_ROOM>, List<HIS_PATIENT_TYPE_ROOM>>(createResult);
                                    PatientTypeRoomViews.AddRange(vCreateResults);
                                }
                                WaitingManager.Hide();
                                lstPatientTypeADOs = lstPatientTypeADOs.OrderByDescending(p => p.checkMedi).ToList();
                                if (ucGridControlRoom != null)
                                {
                                    PatientTypeProcessor.Reload(ucGridControlPatientType, lstPatientTypeADOs);
                                }
                            }
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            MessageManager.Show(this.ParentForm, param, success);
        }

        private void txtKeyword1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToGrid1__PatientType(this);

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
                        HIS_ROOM_TYPE data = RoomType.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboRoomType.EditValue.ToString()));
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
                    HIS_ROOM_TYPE data = RoomType.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboRoomType.EditValue.ToString()));
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
                if (this.currentExecuteRoom == null)
                {
                    FillDataToGrid1__PatientType(this);
                    FillDataToGrid2__Room(this);
                }
                else
                {
                    FillDataToGrid1__Room_Default(this);
                    FillDataToGrid1__PatientType(this);
                    var room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.currentExecuteRoom.ROOM_ID);
                    btn_Radio_Enable_Click(room);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void PatientTypeGridView_MouseRightClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if ((e.Item is BarButtonItem) && sender != null && sender is HIS.UC.PatientType.PatientTypeADO)
                {
                    var type = (HIS.UC.PatientType.Popup.PopupMenuProcessor.ItemType)e.Item.Tag;
                    switch (type)
                    {
                        case HIS.UC.PatientType.Popup.PopupMenuProcessor.ItemType.Copy:
                            {
                                if (isChosePatientType != 1)
                                {
                                    MessageManager.Show("Vui lòng chọn đối tượng!");
                                    break;
                                }
                                this.currentCopyPatientTypeAdo = (HIS.UC.PatientType.PatientTypeADO)sender;
                                break;
                            }
                        case HIS.UC.PatientType.Popup.PopupMenuProcessor.ItemType.Paste:
                            {
                                var currentPaste = (HIS.UC.PatientType.PatientTypeADO)sender;
                                bool success = false;
                                CommonParam param = new CommonParam();
                                if (this.currentCopyPatientTypeAdo == null && isChosePatientType != 1)
                                {
                                    MessageManager.Show("Vui lòng copy!");
                                    break;
                                }
                                if (this.currentCopyPatientTypeAdo != null && currentPaste != null && isChosePatientType == 1)
                                {
                                    //if (this.currentCopyPatientTypeAdo.ID == currentPaste.ID)
                                    //{
                                    //    MessageManager.Show("Trùng dữ liệu copy và paste");
                                    //    break;
                                    //}
                                    //HisMetyRoomCopyByPatientTypeSDO hisMestMatyCopyByMatySDO = new HisMetyRoomCopyByPatientTypeSDO();
                                    //hisMestMatyCopyByMatySDO.CopyPatientTypeId = this.currentCopyPatientTypeAdo.ID;
                                    //hisMestMatyCopyByMatySDO.PastePatientTypeId = currentPaste.ID;
                                    //var result = new BackendAdapter(param).Post<List<HIS_PATIENT_TYPE_ROOM>>("api/HisPatientTypeRoom/CopyByPatientType", ApiConsumer.ApiConsumers.MosConsumer, hisMestMatyCopyByMatySDO, param);
                                    //if (result != null)
                                    //{
                                    //    success = true;
                                    //    List<HIS.UC.Room.RoomAccountADO> dataNew = new List<HIS.UC.Room.RoomAccountADO>();
                                    //    dataNew = (from r in listRoom select new RoomAccountADO(r)).ToList();
                                    //    if (result != null && result.Count > 0)
                                    //    {
                                    //        foreach (var itemRoom in result)
                                    //        {
                                    //            var check = dataNew.FirstOrDefault(o => o.ID == itemRoom.ROOM_ID);
                                    //            if (check != null)
                                    //            {
                                    //                check.check1 = true;
                                    //            }
                                    //        }
                                    //    }
                                    //    dataNew = dataNew.OrderByDescending(p => p.check1).ToList();
                                    //    if (ucGridControlRoom != null)
                                    //    {
                                    //        RoomProcessor.Reload(ucGridControlRoom, dataNew);
                                    //    }
                                    //    else
                                    //    {
                                    //        FillDataToGrid2__Room(this);
                                    //    }
                                    //}
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
                                    HisMetyRoomCopyByRoomSDO hisMestMatyCopyByMatySDO = new HisMetyRoomCopyByRoomSDO();
                                    hisMestMatyCopyByMatySDO.CopyRoomId = this.CurrentRoomCopyAdo.ID;
                                    hisMestMatyCopyByMatySDO.PasteRoomId = currentPaste.ID;
                                    var result = new BackendAdapter(param).Post<List<HIS_PATIENT_TYPE_ROOM>>("api/HisPatientTypeRoom/CopyByRoom", ApiConsumer.ApiConsumers.MosConsumer, hisMestMatyCopyByMatySDO, param);
                                    if (result != null)
                                    {
                                        success = true;
                                        List<HIS.UC.PatientType.PatientTypeADO> dataNew = new List<HIS.UC.PatientType.PatientTypeADO>();
                                        dataNew = (from r in listPatientType select new HIS.UC.PatientType.PatientTypeADO(r)).ToList();
                                        if (result != null && result.Count > 0)
                                        {

                                            foreach (var itemService in result)
                                            {
                                                var check = dataNew.FirstOrDefault(o => o.ID == itemService.PATIENT_TYPE_ID);
                                                if (check != null)
                                                {
                                                    check.checkMedi = true;
                                                }
                                            }

                                            dataNew = dataNew.OrderByDescending(p => p.checkMedi).ToList();

                                            if (ucGridControlPatientType != null)
                                            {
                                                PatientTypeProcessor.Reload(ucGridControlPatientType, dataNew);
                                            }
                                        }
                                        else
                                        {
                                            FillDataToGrid1__PatientType(this);
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







