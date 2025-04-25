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
using System.Linq;
using System.Windows.Forms;
using MOS.EFMODEL.DataModels;
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using MOS.Filter;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Adapter;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.UC.ExecuteRoomView;
using HIS.UC.ExecuteRoomView.ADO;
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
using HIS.Desktop.Plugins.ExroRoom.Entity;
using System.Windows.Media.Media3D;
using DevExpress.XtraGrid.Views.Base;
using Inventec.Common.Logging;

namespace HIS.Desktop.Plugins.ExroRoom
{
    public partial class UCExroRoom : HIS.Desktop.Utility.UserControlBase
    {
        private List<HIS_ROOM_TYPE> RoomType { get; set; }

        internal Inventec.Desktop.Common.Modules.Module currentModule;
        private UCRoomProcessor RoomProcessor;
        private ExecuteRoomViewProcessor ExecuteRoomViewProcessor;
        private UserControl ucGridControlExecuteRoom;
        private UserControl ucGridControlRoom;
        private int rowCount = 0;
        private int dataTotal = 0;
        private int rowCount1 = 0;
        private int dataTotal1 = 0;
        internal List<HIS.UC.Room.RoomAccountADO> lstRoomADOs { get; set; }
        private HIS.UC.ExecuteRoomView.ExecuteRoomViewADO ExecuteRoomADO;
        internal List<HIS.UC.ExecuteRoomView.ExecuteRoomViewADO> lstExecuteRoomADOs { get; set; }
        private List<V_HIS_ROOM> listRoom;
        private List<V_HIS_EXECUTE_ROOM> listExecuteRoom;
        private List<V_HIS_BED_ROOM> lstBedRoom;
        private long ExecuteRoomIdCheckByExecuteRoom = 0;
        private long isChoseExecuteRoom;
        private long isChoseRoom;
        private long RoomIdCheckByRoom;
        private long checkExecuteRoomId;
        private long checkRoomId;
        private bool isCheckAll;
        private List<V_HIS_EXRO_ROOM> ExroRooms { get; set; }
        private List<V_HIS_EXRO_ROOM> ExroRoomViews { get; set; }
        private V_HIS_EXECUTE_ROOM currentExecuteRoom;
        private V_HIS_BED_ROOM currentBedRoom;
        private V_HIS_RECEPTION_ROOM currentReceptionRoom;
        private HIS.UC.Room.RoomAccountADO CurrentRoomCopyAdo;
        private V_HIS_ROOM Room;
        private DevExpress.XtraEditors.CheckEdit chk = new CheckEdit();
        public UCExroRoom(Inventec.Desktop.Common.Modules.Module currentModule)
            : base(currentModule)
        {
            InitializeComponent();
        }

        public UCExroRoom(Inventec.Desktop.Common.Modules.Module currentModule, long RoomType)
            : base(currentModule)
        {
            InitializeComponent();
            try
            {
                this.currentModule = currentModule;
                if (this.currentModule != null)
                {
                    Text = currentModule.text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public UCExroRoom(V_HIS_EXECUTE_ROOM MedicineTypeData, V_HIS_BED_ROOM BedRoom, Inventec.Desktop.Common.Modules.Module _moduleData)
            : base(_moduleData)
        {
            InitializeComponent();
            try
            {
                currentExecuteRoom = MedicineTypeData;
               
                currentBedRoom = BedRoom;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public UCExroRoom(V_HIS_RECEPTION_ROOM ReceptionRoomData, Inventec.Desktop.Common.Modules.Module _moduleData)
            : base(_moduleData)
        {
            InitializeComponent();
            try
            {
                currentReceptionRoom = ReceptionRoomData;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public UCExroRoom(V_HIS_ROOM executeRoom1, Inventec.Desktop.Common.Modules.Module _moduleData)
            : base(_moduleData)
        {
            InitializeComponent();
            try
            {
                Room = executeRoom1;
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
                //LoadExecuteRoomData();

                if (currentExecuteRoom == null && Room == null && currentReceptionRoom == null)
                {
                    FillDataToGrid1__MedicineType(this);
                    FillDataToGrid2__Room(this);

                }
                else
                {
                    if (currentExecuteRoom != null)
                    {
                        FillDataToGrid1__ExecuteRoom_Default(this);
                        FillDataToGrid2__Room(this);
                        if (currentExecuteRoom != null)
                        {
                            Inventec.Common.Mapper.DataObjectMapper.Map<ExecuteRoomViewADO>(ExecuteRoomADO, currentExecuteRoom);
                            btn_Radio_Enable_Click1(ExecuteRoomADO);
                            cboRoomType.EditValue = currentExecuteRoom.ROOM_TYPE_ID;
                        }
                    }
                    else
                    {
                        if (Room != null)
                        {
                            FillDataToGrid1__MedicineType(this);
                            FillDataToGrid1_Room_Default(this);
                            if (Room != null)
                            {
                                btn_Radio_Enable_Click(Room);
                                cboRoomType.EditValue = Room.ROOM_TYPE_ID;
                            }
                        }
                        else
                        {
                            if (currentReceptionRoom != null)
                            {
                                FillDataToGrid1__MedicineType(this);
                                FillDataToGrid2_Room_Default(this);
                                if (currentReceptionRoom != null)
                                {
                                    btn_Radio_Enable_Click2(currentReceptionRoom);
                                    cboRoomType.EditValue = currentReceptionRoom.ROOM_TYPE_ID;
                                }
                            }
                        }
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

        private void FillDataToGridExecuteRoom_Default(object data)
        {
            try
            {
                WaitingManager.Show();
                listExecuteRoom = new List<V_HIS_EXECUTE_ROOM>();
                lstBedRoom = new List<V_HIS_BED_ROOM>();
                var start = ((CommonParam)data).Start ?? 0;
                var limit = ((CommonParam)data).Limit ?? 0;
                var param = new CommonParam(start, limit);

                var hisExecuteRoomViewFilter = new HisExecuteRoomViewFilter();
                hisExecuteRoomViewFilter.IS_ACTIVE = 1;
                hisExecuteRoomViewFilter.ID = currentExecuteRoom.ID;

                var hisbedRoomViewFilter = new HisBedRoomFilter();
                hisExecuteRoomViewFilter.IS_ACTIVE = 1;
                hisExecuteRoomViewFilter.ID = currentBedRoom.ID;
                if ((long)cboChoose.EditValue == 1)
                {
                    isChoseExecuteRoom = (long)cboChoose.EditValue;
                }

                var rs = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM>>(
                                                     "api/HisExecuteRoom/GetView",
                     HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                     hisExecuteRoomViewFilter,
                     param);

                lstExecuteRoomADOs = new List<ExecuteRoomViewADO>();

                if (rs != null && rs.Count > 0)
                {
                    listExecuteRoom = new List<V_HIS_EXECUTE_ROOM>();
                    listExecuteRoom = rs;
                    foreach (var item in listExecuteRoom)
                    {
                        var RoomServiceADO = new ExecuteRoomViewADO(item);
                        if (isChoseExecuteRoom == 1)
                        {
                            RoomServiceADO.isKeyChoose = true;
                        }
                        lstExecuteRoomADOs.Add(RoomServiceADO);
                    }
                }

                var BedRoom = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_BED_ROOM>>(
                                     "api/HisBedRoom/GetView",
                     HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                     hisbedRoomViewFilter,
                     param);

                if (BedRoom != null && BedRoom.Count > 0)
                {
                    lstBedRoom = BedRoom;
                    foreach (var item in lstBedRoom)
                    {
                        var RoomServiceADO = new ExecuteRoomViewADO(item);
                        if (isChoseExecuteRoom == 1)
                        {
                            RoomServiceADO.isKeyChoose = true;
                        }
                        lstExecuteRoomADOs.Add(RoomServiceADO);
                    }
                }

                if (!string.IsNullOrWhiteSpace(txtKeyword1.Text))
                {
                    var keyword = txtKeyword1.Text.Trim().ToLower();
                    lstExecuteRoomADOs = lstExecuteRoomADOs
                        .Where(o =>
                            (!string.IsNullOrEmpty(o.EXECUTE_ROOM_CODE) && o.EXECUTE_ROOM_CODE.ToLower().Contains(keyword)) ||
                            (!string.IsNullOrEmpty(o.EXECUTE_ROOM_NAME) && o.EXECUTE_ROOM_NAME.ToLower().Contains(keyword)))
                        .ToList();
                }

                if (ExroRoomViews != null && ExroRoomViews.Count > 0)
                {
                    foreach (var itemUsername in ExroRoomViews)
                    {
                        var check = lstExecuteRoomADOs.FirstOrDefault(o => (o.ID == itemUsername.EXECUTE_ROOM_ID && o.RoomType == "EXECUTE") || (o.ID == itemUsername.BED_ROOM_ID && o.RoomType == "BED"));
                        if (check != null)
                        {
                            check.check1 = true;
                        }
                    }
                }
                lstExecuteRoomADOs = lstExecuteRoomADOs.OrderByDescending(p => p.check1).Distinct().Skip(start).Take(limit).ToList();
                //lstExecuteRoomADOs = lstExecuteRoomADOs.OrderByDescending(p => p.check1).ToList();
                if (ucGridControlExecuteRoom != null)
                {
                    ExecuteRoomViewProcessor.Reload(ucGridControlExecuteRoom, lstExecuteRoomADOs);
                }
                //rowCount = (data == null ? 0 : lstExecuteRoomADOs.Count);
                //dataTotal = (rs.Param == null ? 0 : rs.Param.Count ?? 0);
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
                    var view = sender as GridView;
                    var hi = view.CalcHitInfo(e.Location);

                    if (hi.HitTest == GridHitTest.Column)
                    {
                        if (hi.Column.FieldName == "isPriorityRequire")
                        {
                            var lstCheckAll = lstExecuteRoomADOs;
                            var lstChecks = new List<HIS.UC.ExecuteRoomView.ExecuteRoomViewADO>();

                            if (lstCheckAll != null && lstCheckAll.Count > 0)
                            {
                                var ServiceCheckedNum = lstExecuteRoomADOs.Where(o => o.isPriorityRequire == true).Count();
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
                                            item.isPriorityRequire = true;
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
                                            item.isPriorityRequire = false;
                                            lstChecks.Add(item);
                                        }
                                        else
                                        {
                                            lstChecks.Add(item);
                                        }
                                    }
                                    isCheckAll = true;
                                }

                                ExecuteRoomViewProcessor.Reload(ucGridControlExecuteRoom, lstChecks);
                            }
                        }
                        if (hi.Column.FieldName == "isHoldOrder")
                        {
                            var lstCheckAll = lstExecuteRoomADOs;
                            var lstChecks = new List<HIS.UC.ExecuteRoomView.ExecuteRoomViewADO>();

                            if (lstCheckAll != null && lstCheckAll.Count > 0)
                            {
                                var ServiceCheckedNum = lstExecuteRoomADOs.Where(o => o.isHoldOrder == true).Count();
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
                                            item.isHoldOrder = true;
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
                                            item.isHoldOrder = false;
                                            lstChecks.Add(item);
                                        }
                                        else
                                        {
                                            lstChecks.Add(item);
                                        }
                                    }
                                    isCheckAll = true;
                                }

                                ExecuteRoomViewProcessor.Reload(ucGridControlExecuteRoom, lstChecks);
                            }
                        }
                        if (hi.Column.FieldName == "isAllowRequest")
                        {
                            var lstCheckAll = lstExecuteRoomADOs;
                            var lstChecks = new List<HIS.UC.ExecuteRoomView.ExecuteRoomViewADO>();

                            if (lstCheckAll != null && lstCheckAll.Count > 0)
                            {
                                var ServiceCheckedNum = lstExecuteRoomADOs.Where(o => o.isAllowRequest == true).Count();
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
                                            item.isAllowRequest = true;
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
                                            item.isAllowRequest = false;
                                            lstChecks.Add(item);
                                        }
                                        else
                                        {
                                            lstChecks.Add(item);
                                        }
                                    }
                                    isCheckAll = true;
                                }

                                ExecuteRoomViewProcessor.Reload(ucGridControlExecuteRoom, lstChecks);
                            }
                        }
                        if (hi.Column.FieldName == "check1" )
                        {
                            var lstCheckAll = lstExecuteRoomADOs;
                            var lstChecks = new List<HIS.UC.ExecuteRoomView.ExecuteRoomViewADO>();

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

                                ExecuteRoomViewProcessor.Reload(ucGridControlExecuteRoom, lstChecks);
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.ExroRoom.Resources.Lang", typeof(HIS.Desktop.Plugins.ExroRoom.UCExroRoom).Assembly);


                layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCMedicineTypeRoom.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                cboChoose.Properties.NullText = Inventec.Common.Resource.Get.Value("UCMedicineTypeRoom.cboChoose.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                layoutControl2.Text = Inventec.Common.Resource.Get.Value("UCMedicineTypeRoom.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                btnFind2.Text = Inventec.Common.Resource.Get.Value("UCMedicineTypeRoom.btnFind2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                btnSave.Text = Inventec.Common.Resource.Get.Value("UCMedicineTypeRoom.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                btnFind1.Text = Inventec.Common.Resource.Get.Value("UCMedicineTypeRoom.btnFind1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                cboRoomType.Properties.NullText = Inventec.Common.Resource.Get.Value("UCMedicineTypeRoom.cboServiceType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                txtKeyword2.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCMedicineTypeRoom.txtKeyword2.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                txtKeyword1.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCMedicineTypeRoom.txtKeyword1.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                lciRoomType.Text = Inventec.Common.Resource.Get.Value("UCMedicineTypeRoom.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("UCMedicineTypeRoom.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                bar2.Text = Inventec.Common.Resource.Get.Value("UCMedicineTypeRoom.bar2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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
                ExecuteRoomViewProcessor = new ExecuteRoomViewProcessor();
                var ado = new ExecuteRoomViewInitADO();
                ado.ListExecuteRoomColumn = new List<ExecuteRoomViewColumn>();
                ado.ExecuteRoomGrid_MouseDown = gridViewMedicineType_MouseDown;
                ado.btn_Radio_Enable_Click = btn_Radio_Enable_Click1;
                ado.gridView_MouseRightClick = ExecuteRoomGridView_MouseRightClick;

                var colRadio2 = new ExecuteRoomViewColumn("   ", "radio1", 30, true);
                colRadio2.VisibleIndex = 0;
                colRadio2.Visible = false;
                colRadio2.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListExecuteRoomColumn.Add(colRadio2);

                var colCheck2 = new ExecuteRoomViewColumn("   ", "check1", 30, true);
                colCheck2.VisibleIndex = 1;
                colCheck2.image = imageCollectionService.Images[0];
                
                colCheck2.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListExecuteRoomColumn.Add(colCheck2);

                var colMaPhong = new ExecuteRoomViewColumn("Mã phòng xử lý", "EXECUTE_ROOM_CODE", 65, false);
                colMaPhong.VisibleIndex = 2;
                
                ado.ListExecuteRoomColumn.Add(colMaPhong);

                var colTenDichvu = new ExecuteRoomViewColumn("Tên phòng xử lý", "EXECUTE_ROOM_NAME", 100, false);
                colTenDichvu.VisibleIndex = 3;
                ado.ListExecuteRoomColumn.Add(colTenDichvu);

  
                var colDuocLaySoChiDinh = new ExecuteRoomViewColumn("Ưu tiên chỉ định", "isPriorityRequire", 60, true);
                colDuocLaySoChiDinh.VisibleIndex = 4;
                colDuocLaySoChiDinh.image = imageCollectionRoom.Images[0];
                colDuocLaySoChiDinh.imageAlignment = System.Drawing.StringAlignment.Near;
                colDuocLaySoChiDinh.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListExecuteRoomColumn.Add(colDuocLaySoChiDinh);

                var colDuocLaySoUuTien = new ExecuteRoomViewColumn("Ưu tiên số thứ tự", "isHoldOrder", 65, true);
                colDuocLaySoUuTien.VisibleIndex = 5;
                colDuocLaySoUuTien.image = imageCollectionRoom.Images[0];
                colDuocLaySoUuTien.imageAlignment = System.Drawing.StringAlignment.Near;
                colDuocLaySoUuTien.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                colDuocLaySoUuTien.ToolTip = "Được lấy số ưu tiên";
                ado.ListExecuteRoomColumn.Add(colDuocLaySoUuTien);

                var colDuocPhepYeuCau = new ExecuteRoomViewColumn("Yêu cầu", "isAllowRequest", 40, true);
                colDuocPhepYeuCau.VisibleIndex = 6;
                colDuocPhepYeuCau.image = imageCollectionRoom.Images[0];
                colDuocPhepYeuCau.imageAlignment = System.Drawing.StringAlignment.Near;
                colDuocPhepYeuCau.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                colDuocPhepYeuCau.ToolTip = "Được phép yêu cầu";
                ado.ListExecuteRoomColumn.Add(colDuocPhepYeuCau);

                var colMaLoaidichvu = new ExecuteRoomViewColumn("Loại phòng", "ROOM_TYPE_NAME", 80, false);
                colMaLoaidichvu.VisibleIndex = 7;
                ado.ListExecuteRoomColumn.Add(colMaLoaidichvu);

                var colDepartment = new ExecuteRoomViewColumn("Khoa", "DEPARTMENT_NAME", 80, false);
                colDepartment.VisibleIndex = 8;
                ado.ListExecuteRoomColumn.Add(colDepartment);

                ucGridControlExecuteRoom = (UserControl)ExecuteRoomViewProcessor.Run(ado);
                if (ucGridControlExecuteRoom != null)
                {
                    panelControl1.Controls.Add(ucGridControlExecuteRoom);
                    ucGridControlExecuteRoom.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        
        private void InitColumnExecuteRoomView()
        {
            try
            {
                 var ListExecuteRoomColumn = new List<ExecuteRoomViewColumn>();              

                var colCheck2 = new ExecuteRoomViewColumn("   ", "check1", 30, true);
                colCheck2.VisibleIndex = 1;
                colCheck2.image = imageCollectionService.Images[0];
                colCheck2.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ListExecuteRoomColumn.Add(colCheck2);

                var colDuocLaySoChiDinh = new ExecuteRoomViewColumn("Ưu tiên chỉ định", "isPriorityRequire", 60, true);
                colDuocLaySoChiDinh.VisibleIndex = 4;
                colDuocLaySoChiDinh.image = imageCollectionRoom.Images[0];
                colDuocLaySoChiDinh.imageAlignment = System.Drawing.StringAlignment.Near;
                colDuocLaySoChiDinh.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ListExecuteRoomColumn.Add(colDuocLaySoChiDinh);

                var colDuocLaySoUuTien = new ExecuteRoomViewColumn("Ưu tiên số thứ tự", "isHoldOrder", 65, true);
                colDuocLaySoUuTien.VisibleIndex = 5;
                colDuocLaySoUuTien.image = imageCollectionRoom.Images[0];
                colDuocLaySoUuTien.imageAlignment = System.Drawing.StringAlignment.Near;
                colDuocLaySoUuTien.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                colDuocLaySoUuTien.ToolTip = "Được lấy số ưu tiên";
              ListExecuteRoomColumn.Add(colDuocLaySoUuTien);

                var colDuocPhepYeuCau = new ExecuteRoomViewColumn("Yêu cầu", "isAllowRequest", 40, true);
                colDuocPhepYeuCau.VisibleIndex = 6;
                colDuocPhepYeuCau.image = imageCollectionRoom.Images[0];
                colDuocPhepYeuCau.imageAlignment = System.Drawing.StringAlignment.Near;
                colDuocPhepYeuCau.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                colDuocPhepYeuCau.ToolTip = "Được phép yêu cầu";
                ListExecuteRoomColumn.Add(colDuocPhepYeuCau);


                ExecuteRoomViewProcessor.ReloadColumn(ucGridControlExecuteRoom, ListExecuteRoomColumn);
             
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
                    var view = sender as GridView;
                    var hi = view.CalcHitInfo(e.Location);

                    if (hi.HitTest == GridHitTest.Column)
                    {
                        if (hi.Column.FieldName == "isPriorityRequire")
                        {
                            var lstCheckAll = lstRoomADOs;
                            var lstChecks = new List<HIS.UC.Room.RoomAccountADO>();

                            if (lstCheckAll != null && lstCheckAll.Count > 0)
                            {
                                var RoomCheckedNum = lstRoomADOs.Where(o => o.isPriorityRequire == true).Count();
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
                                            item.isPriorityRequire = true;
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
                                            item.isPriorityRequire = false;
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
                        if (hi.Column.FieldName == "isHoldOrder")
                        {
                            var lstCheckAll = lstRoomADOs;
                            var lstChecks = new List<HIS.UC.Room.RoomAccountADO>();

                            if (lstCheckAll != null && lstCheckAll.Count > 0)
                            {
                                var RoomCheckedNum = lstRoomADOs.Where(o => o.isHoldOrder == true).Count();
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
                                            item.isHoldOrder = true;
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
                                            item.isHoldOrder = false;
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
                        if (hi.Column.FieldName == "isAllowRequest")
                        {
                            var lstCheckAll = lstRoomADOs;
                            var lstChecks = new List<HIS.UC.Room.RoomAccountADO>();

                            if (lstCheckAll != null && lstCheckAll.Count > 0)
                            {
                                var RoomCheckedNum = lstRoomADOs.Where(o => o.isAllowRequest == true).Count();
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
                                            item.isAllowRequest = true;
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
                                            item.isAllowRequest = false;
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
                        if (hi.Column.FieldName == "check1")
                        {
                            var lstCheckAll = lstRoomADOs;
                            var lstChecks = new List<HIS.UC.Room.RoomAccountADO>();

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

        private void btn_Radio_Enable_Click1(HIS.UC.ExecuteRoomView.ExecuteRoomViewADO data)
        {          
            try
            {
                WaitingManager.Show();
                var param = new CommonParam();
                var filter = new HisExroRoomViewFilter();
                filter.EXECUTE_BED_ROOM_ID = data.ID;
                checkExecuteRoomId = data.ID;
                ExecuteRoomIdCheckByExecuteRoom = data.ID;

                ExroRooms = new BackendAdapter(param).Get<List<V_HIS_EXRO_ROOM>>(
                                        "api/HisExroRoom/GetView",
                                    HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                    filter,
                                    param);
                var dataNew = new List<HIS.UC.Room.RoomAccountADO>();
                dataNew = (from r in listRoom
                            select new RoomAccountADO(r)).ToList();
                

                if (ExroRooms != null && ExroRooms.Count > 0)
                {
                    foreach (var itemRoom in ExroRooms)
                    {
                        var check = dataNew.FirstOrDefault(o => o.ID == itemRoom.ROOM_ID);
                        if (check != null)
                        {
                            check.check1 = true;
                            if (itemRoom.IS_ALLOW_REQUEST == 1)
                            {
                                check.isAllowRequest = true;
                            }
                            else
                            {
                                check.isAllowRequest = false;
                            }
                            if (itemRoom.IS_HOLD_ORDER == 1)
                            {
                                check.isHoldOrder = true;
                            }
                            else
                            {
                                check.isHoldOrder = false;
                            }
                            if (itemRoom.IS_PRIORITY_REQUIRE == 1)
                            {
                                check.isPriorityRequire = true;
                            }
                            else
                            {
                                check.isPriorityRequire = false;
                            }
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
                var ado = new RoomInitADO();
                ado.ListRoomColumn = new List<UC.Room.RoomColumn>();
                ado.gridViewRoom_MouseDownRoom = gridViewRoom_MouseRoom;
                ado.btn_Radio_Enable_Click = btn_Radio_Enable_Click;
                ado.rooom_MouseRightClick = RoomGridView_MouseRightClick;

                var colRadio1 = new RoomColumn(" ", "radio1", 30, true);
                colRadio1.VisibleIndex = 0;
                colRadio1.Visible = false;
                colRadio1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListRoomColumn.Add(colRadio1);

                var colCheck1 = new RoomColumn(" ", "check1", 30, true);
                colCheck1.VisibleIndex = 1;
                colCheck1.image = imageCollectionRoom.Images[0];
                colCheck1.Visible = false;
                colCheck1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListRoomColumn.Add(colCheck1);

                var colMaPhong = new RoomColumn("Mã phòng chỉ định", "ROOM_CODE", 50, false);
                colMaPhong.VisibleIndex = 2;
                ado.ListRoomColumn.Add(colMaPhong);

                var colTenPhong = new RoomColumn("Tên phòng chỉ định", "ROOM_NAME", 100, false);
                colTenPhong.VisibleIndex = 3;
                ado.ListRoomColumn.Add(colTenPhong);

                var colUuTienCD = new RoomColumn("Ưu tiên chỉ định", "isPriorityRequire", 60, true);
                colUuTienCD.VisibleIndex = 4;
                colUuTienCD.image = imageCollectionRoom.Images[0];             
                colUuTienCD.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListRoomColumn.Add(colUuTienCD);

                var colUuTienSTT = new RoomColumn("Ưu tiên số thứ tự", "isHoldOrder", 65, true);
                colUuTienSTT.VisibleIndex = 5;
                colUuTienSTT.image = imageCollectionRoom.Images[0];
                colUuTienSTT.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                colUuTienSTT.ToolTip = "Được lấy số ưu tiên";
                ado.ListRoomColumn.Add(colUuTienSTT);

                var colYeuCau = new RoomColumn("Yêu cầu", "isAllowRequest", 40, true);
                colYeuCau.VisibleIndex = 6;
                colYeuCau.image = imageCollectionRoom.Images[0];
                colYeuCau.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                colYeuCau.ToolTip = "Được phép yêu cầu";
                ado.ListRoomColumn.Add(colYeuCau);

                var colLoaiPhong = new RoomColumn("Loại phòng", "ROOM_TYPE_NAME", 60, false);
                colLoaiPhong.VisibleIndex = 7;
                ado.ListRoomColumn.Add(colLoaiPhong);

                var colKhoa = new RoomColumn("Khoa", "DEPARTMENT_NAME", 80, false);
                colKhoa.VisibleIndex = 8;
                ado.ListRoomColumn.Add(colKhoa);


                ucGridControlRoom = (UserControl)RoomProcessor.Run(ado);
                if (ucGridControlRoom != null)
                {
                    panelControl2.Controls.Add(ucGridControlRoom);
                    ucGridControlRoom.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void InitUcRoom()
        {
            try
            {
                var ListRoomColumn = new List<RoomColumn>();

                var colCheck1 = new RoomColumn(" ", "check1", 30, true);
                colCheck1.VisibleIndex = 1;
                colCheck1.image = imageCollectionRoom.Images[0];
                colCheck1.Visible = false;
                colCheck1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ListRoomColumn.Add(colCheck1);
          

                var colUuTienCD = new RoomColumn("Ưu tiên chỉ định", "isPriorityRequire", 60, true);
                colUuTienCD.VisibleIndex = 4;
                colUuTienCD.image = imageCollectionRoom.Images[0];
                colUuTienCD.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ListRoomColumn.Add(colUuTienCD);

                var colUuTienSTT = new RoomColumn("Ưu tiên số thứ tự", "isHoldOrder", 65, true);
                colUuTienSTT.VisibleIndex = 5;
                colUuTienSTT.image = imageCollectionRoom.Images[0];
                colUuTienSTT.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                colUuTienSTT.ToolTip = "Được lấy số ưu tiên";
                ListRoomColumn.Add(colUuTienSTT);

                var colYeuCau = new RoomColumn("Yêu cầu", "isAllowRequest", 40, true);
                colYeuCau.VisibleIndex = 6;
                colYeuCau.image = imageCollectionRoom.Images[0];
                colYeuCau.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                colYeuCau.ToolTip = "Được phép yêu cầu";
                ListRoomColumn.Add(colYeuCau);

                RoomProcessor.ReloadColumn(ucGridControlRoom, ListRoomColumn);

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
                var param = new CommonParam();
                var filter = new HisExroRoomViewFilter();
                filter.ROOM_ID = data.ID;
                RoomIdCheckByRoom = data.ID;
                checkRoomId = data.ID;
                ExroRoomViews = new BackendAdapter(param).Get<List<V_HIS_EXRO_ROOM>>(
                                         "api/HisExroRoom/GetView",

                                HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                filter,
                                param);
                var dataNew = lstExecuteRoomADOs;

                foreach (var room in dataNew)
                {
                    room.check1 = false;
                    room.isAllowRequest = false;
                    room.isHoldOrder = false;
                    room.isPriorityRequire = false;
                }

                if (ExroRoomViews != null && ExroRoomViews.Count > 0)
                {
                    foreach (var itemService in ExroRoomViews)
                    {
                        var check = dataNew.FirstOrDefault(o => (o.ID == itemService.EXECUTE_ROOM_ID && o.RoomType == "EXECUTE") || (o.ID == itemService.BED_ROOM_ID && o.RoomType == "BED"));
                        if (check != null)
                        {
                            check.check1 = true;
                            if (itemService.IS_PRIORITY_REQUIRE == 1)
                            {    
                                check.isPriorityRequire = true;
                            }
                            else
                            {
                                check.isPriorityRequire = false;    
                            }
                            if (itemService.IS_HOLD_ORDER == 1)
                            {
                                check.isHoldOrder = true;
                            }
                            else
                            {
                                check.isHoldOrder = false;
                            }
                            if (itemService.IS_ALLOW_REQUEST == 1)
                            {
                                check.isAllowRequest = true;
                            }
                            else
                            {
                                check.isAllowRequest = false;
                            }
                        }     
                    }
                        


                    dataNew = dataNew.OrderByDescending(p => p.check1).ToList();


                    if (ucGridControlExecuteRoom != null)
                    {
                        ExecuteRoomViewProcessor.Reload(ucGridControlExecuteRoom, dataNew);
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
        private void btn_Radio_Enable_Click2(V_HIS_RECEPTION_ROOM data)
        {
            try
            {
                WaitingManager.Show();
                var param = new CommonParam();
                var filter = new HisExroRoomViewFilter();
                filter.ROOM_ID = data.ROOM_ID;
                RoomIdCheckByRoom = data.ROOM_ID;
                checkRoomId = data.ROOM_ID;
                ExroRoomViews = new BackendAdapter(param).Get<List<V_HIS_EXRO_ROOM>>(
                                         "api/HisExroRoom/GetView",

                                HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                filter,
                                param);
                var dataNew = lstExecuteRoomADOs;
                //dataNew = (from r in listExecuteRoom
                //            select new HIS.UC.ExecuteRoomView.ExecuteRoomViewADO(r)).ToList();
                if (ExroRoomViews != null && ExroRoomViews.Count > 0)
                {
                    foreach (var itemService in ExroRoomViews)
                    {
                        var check = dataNew.FirstOrDefault(o => (o.ID == itemService.EXECUTE_ROOM_ID && o.RoomType == "EXECUTE") || (o.ID == itemService.BED_ROOM_ID && o.RoomType == "BED"));
                        if (check != null)
                        {
                            check.check1 = true;
                            if (itemService.IS_PRIORITY_REQUIRE == 1)
                            {
                                check.isPriorityRequire = true;
                            }
                            else
                            {
                                check.isPriorityRequire = false;
                            }
                            if (itemService.IS_HOLD_ORDER == 1)
                            {
                                check.isHoldOrder = true;
                            }
                            else
                            {
                                check.isHoldOrder = false;
                            }
                            if (itemService.IS_ALLOW_REQUEST == 1)
                            {
                                check.isAllowRequest = true;
                            }
                            else
                            {
                                check.isAllowRequest = false;
                            }
                        }
                    }


                    dataNew = dataNew.OrderByDescending(p => p.check1).ToList();


                    if (ucGridControlExecuteRoom != null)
                    {
                        ExecuteRoomViewProcessor.Reload(ucGridControlExecuteRoom, dataNew);
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

        private void FillDataToGrid2__Room(UCExroRoom uCRoomService)
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
                var param = new CommonParam();
                param.Limit = rowCount1;
                param.Count = dataTotal1;
                ucPaging2.Init(FillDataToGridRoom, param, numPageSize);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGrid2_ExecuteRoom(UCExroRoom uCRoomService)
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
                var param = new CommonParam();
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
                listExecuteRoom = new List<V_HIS_EXECUTE_ROOM>();
                lstBedRoom = new List<V_HIS_BED_ROOM>();
                var start1 = ((CommonParam)data).Start ?? 0;
                var limit1 = ((CommonParam)data).Limit ?? 0;
                var param = new CommonParam(start1, limit1);
                var ExecuteRoomFillter = new HisExecuteRoomFilter();
                ExecuteRoomFillter.IS_ACTIVE = 1;
                ExecuteRoomFillter.ORDER_FIELD = "MODIFY_TIME";
                ExecuteRoomFillter.ORDER_DIRECTION = "DESC";
                ExecuteRoomFillter.KEY_WORD = txtKeyword2.Text;

                var BedRoomFillter = new HisBedRoomFilter();
                BedRoomFillter.IS_ACTIVE = 1;
                BedRoomFillter.ORDER_FIELD = "MODIFY_TIME";
                BedRoomFillter.ORDER_DIRECTION = "DESC";
                BedRoomFillter.KEY_WORD = txtKeyword2.Text;
                if ((long)cboChoose.EditValue == 1)
                {
                    isChoseExecuteRoom = (long)cboChoose.EditValue;
                }
                if (cboRoomType.EditValue != null)
                {
                    ExecuteRoomFillter.ROOM_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboRoomType.EditValue ?? "0").ToString());
                }

                var sar = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM>>(
                   HIS.Desktop.ApiConsumer.HisRequestUriStore.HIS_EXECUTE_ROOM_GETVIEW,
                    HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                      ExecuteRoomFillter,
                    param);

                lstExecuteRoomADOs = new List<ExecuteRoomViewADO>();
                if (sar != null && sar.Count > 0)
                {
                    listExecuteRoom = sar;
                    foreach (var item in listExecuteRoom)
                    {
                        var roomaccountADO = new ExecuteRoomViewADO(item);
                        if (isChoseRoom == 2)
                        {
                            roomaccountADO.isKeyChoose = true;
                        }
                        lstExecuteRoomADOs.Add(roomaccountADO);
                    }
                }

                var BED = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_BED_ROOM>>(
                   HIS.Desktop.ApiConsumer.HisRequestUriStore.HIS_BED_ROOM_GETVIEW,
                    HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                      BedRoomFillter,
                    param);

                if (BED != null && BED.Count > 0)
                {
                    lstBedRoom = BED;
                    foreach (var item in lstBedRoom)
                    {
                        var roomaccountADO = new ExecuteRoomViewADO(item);
                        if (isChoseRoom == 2)
                        {
                            roomaccountADO.isKeyChoose = true;
                        }
                        lstExecuteRoomADOs.Add(roomaccountADO);
                    }
                }

                if (!string.IsNullOrWhiteSpace(txtKeyword1.Text))
                {
                    var keyword = txtKeyword1.Text.Trim().ToLower();
                    lstExecuteRoomADOs = lstExecuteRoomADOs
                        .Where(o =>
                            (!string.IsNullOrEmpty(o.EXECUTE_ROOM_CODE) && o.EXECUTE_ROOM_CODE.ToLower().Contains(keyword)) ||
                            (!string.IsNullOrEmpty(o.EXECUTE_ROOM_NAME) && o.EXECUTE_ROOM_NAME.ToLower().Contains(keyword)))
                        .ToList();
                }

                if (ExroRoomViews != null && ExroRoomViews.Count > 0)
                {
                    foreach (var itemUsername in ExroRoomViews)
                    {
                        
                        var check = lstExecuteRoomADOs.FirstOrDefault(o => (o.ID == itemUsername.EXECUTE_ROOM_ID && o.RoomType == "EXECUTE") || (o.ID == itemUsername.BED_ROOM_ID && o.RoomType == "BED"));
                        if (check != null)
                        {
                            check.check1 = true;
                        }
                    }
                }
                lstExecuteRoomADOs = lstExecuteRoomADOs.OrderByDescending(p => p.check1).Distinct().Skip(start1).Take(limit1).ToList();

                //if (ExroRooms != null && ExroRooms.Count > 0)
                //{
                //    foreach (var itemUsername in ExroRooms)
                //    {
                //        var check = lstExecuteRoomADOs.FirstOrDefault(o => o.ID == itemUsername.ROOM_ID);
                //        if (check != null)
                //        {
                //            check.check1 = true;
                //        }
                //    }
                //}
                //lstExecuteRoomADOs = lstExecuteRoomADOs.OrderByDescending(p => p.check1).Distinct().ToList();

                if (ucGridControlExecuteRoom != null)
                {
                    ExecuteRoomViewProcessor.Reload(ucGridControlExecuteRoom, lstExecuteRoomADOs);
                }
                //rowCount1 = (data == null ? 0 : lstExecuteRoomADOs.Count);
                //dataTotal1 = (sar.Param == null ? 0 : sar.Param.Count ?? 0);
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
                listRoom = new List<V_HIS_ROOM>();
                var start1 = ((CommonParam)data).Start ?? 0;
                var limit1 = ((CommonParam)data).Limit ?? 0;
                var param = new CommonParam(start1, limit1);
                var RoomFillter = new HisRoomViewFilter();
                RoomFillter.IS_ACTIVE = 1;
                RoomFillter.ID = Room.ID;
                RoomFillter.ORDER_FIELD = "MODIFY_TIME";
                RoomFillter.ORDER_DIRECTION = "DESC";
                RoomFillter.KEY_WORD = txtKeyword2.Text;
                if ((long)cboChoose.EditValue == 2)
                {
                    isChoseRoom = (long)cboChoose.EditValue;
                }
                if (cboRoomType.EditValue != null)
                {
                    RoomFillter.ROOM_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboRoomType.EditValue ?? "0").ToString());
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
                        var roomaccountADO = new RoomAccountADO(item);
                        if (isChoseRoom == 2)
                        {
                            roomaccountADO.isKeyChoose = true;
                        }
                        if (Room != null && 1 == 1)
                        {
                            roomaccountADO.radio1 = true;
                        }
                        lstRoomADOs.Add(roomaccountADO);
                    }
                }

                if (ExroRooms != null && ExroRooms.Count > 0)
                {
                    foreach (var itemUsername in ExroRooms)
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

        private void FillDataToGridRoomData_Default(object data)
        {
            try
            {
                WaitingManager.Show();
                listRoom = new List<V_HIS_ROOM>();
                var start1 = ((CommonParam)data).Start ?? 0;
                var limit1 = ((CommonParam)data).Limit ?? 0;
                var param = new CommonParam(start1, limit1);
                var RoomFillter = new HisRoomViewFilter();
                RoomFillter.IS_ACTIVE = 1;
                RoomFillter.ID = currentReceptionRoom.ROOM_ID;
                RoomFillter.ORDER_FIELD = "MODIFY_TIME";
                RoomFillter.ORDER_DIRECTION = "DESC";
                RoomFillter.KEY_WORD = txtKeyword2.Text;
                if ((long)cboChoose.EditValue == 2)
                {
                    isChoseRoom = (long)cboChoose.EditValue;
                }
                if (cboRoomType.EditValue != null)
                {
                    RoomFillter.ROOM_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboRoomType.EditValue ?? "0").ToString());
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
                        var roomaccountADO = new RoomAccountADO(item);
                        if (isChoseRoom == 2)
                        {
                            roomaccountADO.isKeyChoose = true;
                        }
                        if (currentReceptionRoom != null && 1 == 1)
                        {
                            roomaccountADO.radio1 = true;
                        }
                        lstRoomADOs.Add(roomaccountADO);
                    }
                }

                if (ExroRooms != null && ExroRooms.Count > 0)
                {
                    foreach (var itemUsername in ExroRooms)
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
        private void FillDataToGridRoom(object data)
        {
            try
            {
                WaitingManager.Show();
                listRoom = new List<V_HIS_ROOM>();
                var start1 = ((CommonParam)data).Start ?? 0;
                var limit1 = ((CommonParam)data).Limit ?? 0;
                var param = new CommonParam(start1, limit1);
                var RoomFillter = new HisRoomViewFilter();
                RoomFillter.IS_ACTIVE = 1;
                RoomFillter.ORDER_FIELD = "MODIFY_TIME";
                RoomFillter.ORDER_DIRECTION = "DESC";
                RoomFillter.KEY_WORD = txtKeyword2.Text;
                if ((long)cboChoose.EditValue == 2)
                {
                    isChoseRoom = (long)cboChoose.EditValue;
                }
                if (cboRoomType.EditValue != null)
                {
                    RoomFillter.ROOM_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboRoomType.EditValue ?? "0").ToString());
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
                        var roomaccountADO = new RoomAccountADO(item);
                        if (isChoseRoom == 2)
                        {
                            roomaccountADO.isKeyChoose = true;
                        }
                        if (Room != null && 1 == 1)
                        {
                            roomaccountADO.radio1 = true;
                        }
                        lstRoomADOs.Add(roomaccountADO);
                    }
                }

                if (ExroRooms != null && ExroRooms.Count > 0)
                {
                    foreach (var itemUsername in ExroRooms)
                    {
                        var check = lstRoomADOs.FirstOrDefault(o => o.ID == itemUsername.ROOM_ID);
                        if (check != null)
                        {
                            check.check1 = true;
                            if (itemUsername.IS_PRIORITY_REQUIRE == 1)
                            {
                                check.isPriorityRequire = true;
                            }
                            else
                            {
                                check.isPriorityRequire = false;
                            }
                            if (itemUsername.IS_HOLD_ORDER == 1)
                            {
                                check.isHoldOrder = true;
                            }
                            else
                            {
                                check.isHoldOrder = false;
                            }
                            if (itemUsername.IS_ALLOW_REQUEST == 1)
                            {
                                check.isAllowRequest = true;
                            }
                            else
                            {
                                check.isAllowRequest = false;
                            }
                        }
                    }
                }
                lstRoomADOs = lstRoomADOs.OrderByDescending(p => p.check1).Distinct().ToList();

                if (ucGridControlRoom != null)
                {
                    RoomProcessor.Reload(ucGridControlRoom, lstRoomADOs);
                    InitUcRoom();
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

        private void FillDataToGridRoom_Default1(object data)
        {
            try
            {
                WaitingManager.Show();
                listRoom = new List<V_HIS_ROOM>();
                var start1 = ((CommonParam)data).Start ?? 0;
                var limit1 = ((CommonParam)data).Limit ?? 0;
                var param = new CommonParam(start1, limit1);
                var RoomFillter = new HisRoomViewFilter();
                RoomFillter.IS_ACTIVE = 1;
                RoomFillter.ORDER_FIELD = "MODIFY_TIME";
                RoomFillter.ORDER_DIRECTION = "DESC";
                RoomFillter.KEY_WORD = txtKeyword2.Text;
                if ((long)cboChoose.EditValue == 2)
                {
                    isChoseRoom = (long)cboChoose.EditValue;
                }
                if (cboRoomType.EditValue != null)
                {
                    RoomFillter.ROOM_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboRoomType.EditValue ?? "0").ToString());
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
                        var roomaccountADO = new RoomAccountADO(item);
                        if (isChoseRoom == 2)
                        {
                            roomaccountADO.isKeyChoose = true;
                        }
                        if (currentReceptionRoom != null && 1 == 1)
                        {
                            roomaccountADO.radio1 = true;
                        }
                        lstRoomADOs.Add(roomaccountADO);
                    }
                }

                if (ExroRooms != null && ExroRooms.Count > 0)
                {
                    foreach (var itemUsername in ExroRooms)
                    {
                        var check = lstRoomADOs.FirstOrDefault(o => o.ID == itemUsername.ROOM_ID);
                        if (check != null)
                        {
                            check.check1 = true;
                            if (itemUsername.IS_PRIORITY_REQUIRE == 1)
                            {
                                check.isPriorityRequire = true;
                            }
                            else
                            {
                                check.isPriorityRequire = false;
                            }
                            if (itemUsername.IS_HOLD_ORDER == 1)
                            {
                                check.isHoldOrder = true;
                            }
                            else
                            {
                                check.isHoldOrder = false;
                            }
                            if (itemUsername.IS_ALLOW_REQUEST == 1)
                            {
                                check.isAllowRequest = true;
                            }
                            else
                            {
                                check.isAllowRequest = false;
                            }
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

        private void FillDataToGrid1__MedicineType(UCExroRoom UCRoomService)
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
                var param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridMedicineType, param, numPageSize);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGrid1_Room_Default(UCExroRoom UCRoomService)
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
                var param = new CommonParam();
                param.Limit = rowCount1;
                param.Count = dataTotal1;
                ucPaging2.Init(FillDataToGridRoom, param, numPageSize);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGrid2_Room_Default(UCExroRoom UCRoomService)
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

                FillDataToGridRoomData_Default(new CommonParam(0, numPageSize));
                var param = new CommonParam();
                param.Limit = rowCount1;
                param.Count = dataTotal1;
                ucPaging2.Init(FillDataToGridRoom_Default1, param, numPageSize);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGrid1__ExecuteRoom_Default(UCExroRoom UCRoomService)
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

                var param = new CommonParam();
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
                var start = ((CommonParam)data).Start ?? 0; // số bắt đầu
                var limit = ((CommonParam)data).Limit ?? 0; // số lượng phần tử cần lấy
                var param = new CommonParam(start, limit);
                listExecuteRoom = new List<V_HIS_EXECUTE_ROOM>();
                lstBedRoom = new List<V_HIS_BED_ROOM>();

                var ServiceFillter = new HisExecuteRoomViewFilter();
                ServiceFillter.IS_ACTIVE = 1;

                var bedRoomFillter = new HisBedRoomFilter();
                ServiceFillter.IS_ACTIVE = 1;

                if ((long)cboChoose.EditValue == 1)
                {
                    isChoseExecuteRoom = (long)cboChoose.EditValue;
                }

                var rs = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM>>(
                                                     "api/HisExecuteRoom/GetView",
                     HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                     ServiceFillter,
                     param);

                lstExecuteRoomADOs = new List<ExecuteRoomViewADO>();

                if (rs != null && rs.Count > 0)
                {
                    listExecuteRoom = rs;
                    foreach (var item in listExecuteRoom)
                    {
                        var RoomServiceADO = new ExecuteRoomViewADO(item);
                        if (isChoseExecuteRoom == 1)
                        {
                            RoomServiceADO.isKeyChoose = true;
                        }
                        lstExecuteRoomADOs.Add(RoomServiceADO);
                    }
                }

                var BedRoom = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_BED_ROOM>>(
                                     "api/HisBedRoom/GetView",
                     HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                     bedRoomFillter,
                     param);

                if (BedRoom != null && BedRoom.Count > 0)
                {
                    lstBedRoom = BedRoom;
                    foreach (var item in lstBedRoom)
                    {
                        var RoomServiceADO = new ExecuteRoomViewADO(item);
                        if (isChoseExecuteRoom == 1)
                        {
                            RoomServiceADO.isKeyChoose = true;
                        }
                        lstExecuteRoomADOs.Add(RoomServiceADO);
                    }
                }

                
                #region
                //var ServiceFillter = new HisExecuteRoomViewFilter();
                //ServiceFillter.IS_ACTIVE = 1;

                //if ((long)cboChoose.EditValue == 1)
                //{
                //    isChoseExecuteRoom = (long)cboChoose.EditValue;
                //}

                //var rs = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM>>(
                //                                     "api/HisExecuteRoom/GetView",
                //     HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                //     ServiceFillter,
                //     param);

                //lstExecuteRoomADOs = new List<ExecuteRoomViewADO>();

                //if (rs != null && rs.Count > 0)
                //{
                //    listExecuteRoom = rs;
                //    foreach (var item in listExecuteRoom)
                //    {
                //        var RoomServiceADO = new ExecuteRoomViewADO(item);
                //        if (isChoseExecuteRoom == 1)
                //        {
                //            RoomServiceADO.isKeyChoose = true;
                //        }
                //        lstExecuteRoomADOs.Add(RoomServiceADO);
                //    }
                //}

                //var BedRoom = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_BED_ROOM>>(
                //                     "api/HisBedRoom/GetView",
                //     HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                //     ServiceFillter,
                //     param);

                //if (BedRoom != null && BedRoom.Count > 0)
                //{
                //    lstBedRoom = BedRoom;
                //    foreach (var item in lstBedRoom)
                //    {
                //        var RoomServiceADO = new ExecuteRoomViewADO(item);
                //        if (isChoseExecuteRoom == 1)
                //        {
                //            RoomServiceADO.isKeyChoose = true;
                //        }
                //        lstExecuteRoomADOs.Add(RoomServiceADO);
                //    }
                //}

                #endregion
                if (!string.IsNullOrWhiteSpace(txtKeyword1.Text))
                {
                    var keyword = txtKeyword1.Text.Trim().ToLower();
                    lstExecuteRoomADOs = lstExecuteRoomADOs
                        .Where(o =>
                            (!string.IsNullOrEmpty(o.EXECUTE_ROOM_CODE) && o.EXECUTE_ROOM_CODE.ToLower().Contains(keyword)) ||
                            (!string.IsNullOrEmpty(o.EXECUTE_ROOM_NAME) && o.EXECUTE_ROOM_NAME.ToLower().Contains(keyword)))
                        .ToList();
                }

                if (ExroRoomViews != null && ExroRoomViews.Count > 0)
                {
                    foreach (var itemUsername in ExroRoomViews)
                    {
                        var check = lstExecuteRoomADOs.FirstOrDefault(o => (o.ID == itemUsername.EXECUTE_ROOM_ID && o.RoomType == "EXECUTE") || (o.ID == itemUsername.BED_ROOM_ID && o.RoomType == "BED"));
                        if (check != null)
                        {
                            check.check1 = true;
                            if (itemUsername.IS_ALLOW_REQUEST == 1)
                            {
                                check.isAllowRequest = true;
                            }
                            else
                            {
                                check.isAllowRequest = false;
                            }
                            if (itemUsername.IS_HOLD_ORDER == 1)
                            {
                                check.isHoldOrder = true;
                            }
                            else
                            {
                                check.isHoldOrder = false;
                            }
                            if (itemUsername.IS_PRIORITY_REQUIRE == 1)
                            {
                                check.isPriorityRequire = true;
                            }
                            else
                            {
                                check.isPriorityRequire = false;
                            }
                        }
                    }
                }

                //lstExecuteRoomADOs = lstExecuteRoomADOs.OrderByDescending(p => p.check1).Distinct().ToList();
                lstExecuteRoomADOs = lstExecuteRoomADOs.OrderByDescending(p => p.check1).Distinct().Skip(start).Take(limit).ToList();
                if (ucGridControlExecuteRoom != null)
                {
                    ExecuteRoomViewProcessor.Reload(ucGridControlExecuteRoom, lstExecuteRoomADOs);
                    InitColumnExecuteRoomView();
                }
                //rowCount = (data == null ? 0 : lstExecuteRoomADOs.Count);
                //dataTotal = (rs == null ? 0 : rs.Count);
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
                var start = ((CommonParam)data).Start ?? 0;
                var limit = ((CommonParam)data).Limit ?? 0;
                var param = new CommonParam(start, limit);
                var hisRoomViewFilter = new HisRoomFilter();
                hisRoomViewFilter.IS_ACTIVE = 1;
                hisRoomViewFilter.ID = Room.ID;





                if ((long)cboChoose.EditValue == 2)
                {
                    isChoseRoom = (long)cboChoose.EditValue;
                }

                var rs = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_ROOM>>(
                                                     "api/HisRoom/GetView",
                     HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                     hisRoomViewFilter,
                     param);

                lstRoomADOs = new List<RoomAccountADO>();

                if (rs != null && rs.Data.Count > 0)
                {
                    listRoom = new List<V_HIS_ROOM>();
                    listRoom = rs.Data;
                    foreach (var item in listRoom)
                    {
                        var RoomServiceADO = new RoomAccountADO(item);
                        if (isChoseRoom == 2)
                        {
                            RoomServiceADO.isKeyChoose = true;
                            RoomServiceADO.radio1 = true;
                        }
                        listRoom.Add(RoomServiceADO);
                    }
                }

                if (ExroRoomViews != null && ExroRoomViews.Count > 0)
                {
                    foreach (var itemUsername in ExroRoomViews)
                    {
                        var check = lstRoomADOs.FirstOrDefault(o => o.ID == itemUsername.ID);
                        if (check != null)
                        {
                            check.check1 = true;
                        }
                    }
                }

                lstRoomADOs = lstRoomADOs.OrderByDescending(p => p.check1).ToList();
                if (ucGridControlRoom != null)
                {
                    RoomProcessor.Reload(ucGridControlRoom, lstRoomADOs);
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
                LoadDataToComboServiceType(cboRoomType, BackendDataWorker.Get<HIS_ROOM_TYPE>().Where(o => o.IS_ACTIVE == 1).ToList());
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
                var status = new List<Status>();
                status.Add(new Status(1, "Phòng xử lý"));
                status.Add(new Status(2, "Phòng chỉ định"));

                var columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("statusName", string.Empty, 300, 2));
                var controlEditorADO = new ControlEditorADO("statusName", "id", columnInfos, false, 350);
                ControlEditorLoader.Load(cboChoose, status, controlEditorADO);
                if (currentExecuteRoom != null)
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

                var aColumnCode = cboServiceType.Properties.View.Columns.AddField("ROOM_TYPE_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 100;

                var aColumnName = cboServiceType.Properties.View.Columns.AddField("ROOM_TYPE_NAME");
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
                ExroRoomViews = null;
                ExroRooms = null;
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
                    var Room = RoomProcessor.GetDataGridView(ucGridControlRoom);
                    var ExecuteRoom = ExecuteRoomViewProcessor.GetDataGridView(ucGridControlExecuteRoom);
                    var success = false;
                    var param = new CommonParam();
                    if (isChoseExecuteRoom == 1)
                    {
                        if (Room is List<HIS.UC.Room.RoomAccountADO>)
                        {
                            lstRoomADOs = (List<HIS.UC.Room.RoomAccountADO>)Room;

                            if (lstRoomADOs != null && lstRoomADOs.Count > 0)
                            {
                                if (ExecuteRoom is List<HIS.UC.ExecuteRoomView.ExecuteRoomViewADO>)
                                {
                                    lstExecuteRoomADOs = (List<HIS.UC.ExecuteRoomView.ExecuteRoomViewADO>)ExecuteRoom;
                                }







                                var dataCheckeds = lstRoomADOs.Where(p => p.check1 == true).ToList();



                                var dataDeletes = lstRoomADOs.Where(o => ExroRooms.Select(p => p.ROOM_ID)
                                    .Contains(o.ID) && o.check1 == false).ToList();



                                var dataCreates = dataCheckeds.Where(o => !ExroRooms.Select(p => p.ROOM_ID)
                                    .Contains(o.ID)).ToList();


                                var dataUpdate = dataCheckeds.Where(o => ExroRooms.Select(p => p.ROOM_ID)
                                    .Contains(o.ID)).ToList();

                                if (dataUpdate != null && dataUpdate.Count > 0)
                                {
                                    var exroRoomUpdates = new List<HIS_EXRO_ROOM>();
                                    var executeRoomErrors = new List<HIS_EXRO_ROOM>();
                                    foreach (var item in dataUpdate)
                                    {
                                        var exroRoom = ExroRooms.FirstOrDefault(o => o.ROOM_ID == item.ID && o.EXECUTE_ROOM_ID == checkExecuteRoomId);
                                        if (exroRoom != null)
                                        {
                                            var exroRoomResult = new HIS_EXRO_ROOM();
                                            AutoMapper.Mapper.CreateMap<V_HIS_EXRO_ROOM, HIS_EXRO_ROOM>();
                                            exroRoomResult = AutoMapper.Mapper.Map<HIS_EXRO_ROOM>(exroRoom);
                                            if (item.isAllowRequest)
                                            {
                                                exroRoomResult.IS_ALLOW_REQUEST = 1;
                                            }
                                            else
                                            {
                                                exroRoomResult.IS_ALLOW_REQUEST = null;
                                            }
                                            if (item.isPriorityRequire)
                                            {
                                                exroRoomResult.IS_PRIORITY_REQUIRE = 1;
                                            }
                                            else
                                            {
                                                exroRoomResult.IS_PRIORITY_REQUIRE = null;
                                            }
                                            if (item.isHoldOrder)
                                            {
                                                exroRoomResult.IS_HOLD_ORDER = 1;
                                            }
                                            else
                                            {
                                                exroRoomResult.IS_HOLD_ORDER = null;
                                            }
                                            if (exroRoomResult.IS_HOLD_ORDER == null && exroRoomResult.IS_ALLOW_REQUEST == null)
                                            {
                                                executeRoomErrors.Add(exroRoomResult);
                                            }
                                            exroRoomUpdates.Add(exroRoomResult);
                                        }
                                    }

                                    if (exroRoomUpdates != null && exroRoomUpdates.Count > 0)
                                    {
                                        if (executeRoomErrors != null && executeRoomErrors.Count > 0)
                                        {
                                            WaitingManager.Hide();
                                            MessageManager.Show("Phòng xử lý đang chọn phải chọn ít nhất 1 trong 2 'Được lấy số ưu tiên' và 'Được phép yêu cầu'");
                                            return;
                                        }
                                        var updateResult = new BackendAdapter(param).Post<List<HIS_EXRO_ROOM>>(
                                                   "/api/HisExroRoom/UpdateList",
                                                   HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                                   exroRoomUpdates,
                                                   param);
                                        if (updateResult != null && updateResult.Count > 0)
                                        {
                                            AutoMapper.Mapper.CreateMap<HIS_EXRO_ROOM, V_HIS_EXRO_ROOM>();
                                            var vUpdateResults = AutoMapper.Mapper.Map<List<HIS_EXRO_ROOM>, List<V_HIS_EXRO_ROOM>>(updateResult);
                                            ExroRooms.AddRange(vUpdateResults);
                                            success = true;
                                        }
                                    }
                                }

                                if (dataDeletes.Count == 0 && dataCheckeds.Count == 0)
                                {
                                    WaitingManager.Hide();
                                    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn phòng", "Thông báo");
                                    return;
                                }
                                if (dataCheckeds != null)
                                {
                                    success = true;
                                }

                                if (dataDeletes != null && dataDeletes.Count > 0)
                                {
                                    var deleteSds = ExroRooms.Where(o => dataDeletes.Select(p => p.ID)
                                        .Contains(o.ROOM_ID)).Select(o => o.ID).ToList();
                                    var deleteResult = new BackendAdapter(param).Post<bool>(
                                              "api/HisExroRoom/DeleteList",
                                              HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                              deleteSds,
                                              param);
                                    if (deleteResult)
                                    {
                                        success = true;
                                    }
                                    ExroRooms = ExroRooms.Where(o => !deleteSds.Contains(o.ID)).ToList();
                                }

                                if (dataCreates != null && dataCreates.Count > 0)
                                {
                                    var ServiceRoomCreates = new List<HIS_EXRO_ROOM>();
                                    var executeRoomErrors = new List<HIS_EXRO_ROOM>();
                                    foreach (var item in dataCreates)
                                    {
                                        var ServiceRoom = new HIS_EXRO_ROOM();
                                        ServiceRoom.EXECUTE_ROOM_ID = ExecuteRoomIdCheckByExecuteRoom;
                                        ServiceRoom.ROOM_ID = item.ID;
                                        if (item.isAllowRequest)
                                        {
                                            ServiceRoom.IS_ALLOW_REQUEST = 1;
                                        }
                                        else
                                        {
                                            ServiceRoom.IS_ALLOW_REQUEST = null;
                                        }
                                        if (item.isPriorityRequire)
                                        {
                                            ServiceRoom.IS_PRIORITY_REQUIRE = 1;
                                        }
                                        else
                                        {
                                            ServiceRoom.IS_PRIORITY_REQUIRE = null;
                                        }
                                        if (item.isHoldOrder)
                                        {
                                            ServiceRoom.IS_HOLD_ORDER = 1;
                                        }
                                        else
                                        {
                                            ServiceRoom.IS_HOLD_ORDER = null;
                                        }
                                        if (ServiceRoom.IS_HOLD_ORDER == null && ServiceRoom.IS_ALLOW_REQUEST == null)
                                        {
                                            executeRoomErrors.Add(ServiceRoom);
                                        }
                                        ServiceRoomCreates.Add(ServiceRoom);
                                    }
                                    if (executeRoomErrors != null && executeRoomErrors.Count > 0)
                                    {
                                        WaitingManager.Hide();
                                        MessageManager.Show("Phòng xử lý đang chọn phải chọn ít nhất 1 trong 2 'Được lấy số ưu tiên' và 'Được phép yêu cầu'");
                                        return;
                                    }
                                    var createResult = new BackendAdapter(param).Post<List<HIS_EXRO_ROOM>>(
                                               "api/HisExroRoom/CreateList",
                                               HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                               ServiceRoomCreates,
                                               param);
                                    if (createResult != null && createResult.Count > 0)
                                    {
                                        success = true;
                                    }
                                    AutoMapper.Mapper.CreateMap<HIS_EXRO_ROOM, V_HIS_EXRO_ROOM>();
                                    var vCreateResults = AutoMapper.Mapper.Map<List<HIS_EXRO_ROOM>, List<V_HIS_EXRO_ROOM>>(createResult);
                                    ExroRooms.AddRange(vCreateResults);

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
                        if (ExecuteRoom is List<HIS.UC.ExecuteRoomView.ExecuteRoomViewADO>)
                        {
                            lstExecuteRoomADOs = (List<HIS.UC.ExecuteRoomView.ExecuteRoomViewADO>)ExecuteRoom;

                            if (lstExecuteRoomADOs != null && lstExecuteRoomADOs.Count > 0)
                            {
                                var dataChecked = lstExecuteRoomADOs.Where(p => p.check1 == true).ToList();

                                

                                //var dataUpdate = dataChecked.Where(o => ExroRoomViews.Select(p => p.EXECUTE_ROOM_ID)
                                //    .Contains(o.ID)).ToList();
                                var dataUpdate = dataChecked.Where(o =>
                                        (o.RoomType == "EXECUTE" && ExroRoomViews.Any(p => p.EXECUTE_ROOM_ID == o.ID)) ||
                                        (o.RoomType == "BED" && ExroRoomViews.Any(p => p.BED_ROOM_ID == o.ID))
                                    ).ToList();

                                //var dataDelete = lstExecuteRoomADOs.Where(o => ExroRoomViews.Select(p => p.EXECUTE_ROOM_ID)
                                //    .Contains(o.ID) && o.check1 == false).ToList();
                                //var executeRoomIds = ExroRoomViews
                                //    .Where(p => p.EXECUTE_ROOM_ID.HasValue)
                                //    .Select(p => p.EXECUTE_ROOM_ID.Value)
                                //    .ToList();



                                //var dataDelete = lstExecuteRoomADOs
                                //    .Where(o => executeRoomIds.Contains(o.ID) && o.check1 == false)
                                //    .ToList();

                                var dataDelete = lstExecuteRoomADOs.Where(o =>
                                        ((o.RoomType == "EXECUTE" && ExroRoomViews.Any(p => p.EXECUTE_ROOM_ID == o.ID)) ||
                                        (o.RoomType == "BED" && ExroRoomViews.Any(p => p.BED_ROOM_ID == o.ID)) && o.check1 == false)
                                    ).ToList();

                                var dataCreate = dataChecked.Where(o =>
                                        (o.RoomType == "EXECUTE" && !ExroRoomViews.Any(p => p.EXECUTE_ROOM_ID == o.ID)) ||
                                        (o.RoomType == "BED" && !ExroRoomViews.Any(p => p.BED_ROOM_ID == o.ID))
                                    ).ToList();

                                if (dataUpdate != null && dataUpdate.Count > 0)
                                {
                                    var exroRoomUpdates = new List<HIS_EXRO_ROOM>();
                                    var executeRoomErrors = new List<ExecuteRoomViewADO>();
                                    foreach (var item in dataUpdate)
                                    {
                                        var exroRoom = ExroRoomViews.FirstOrDefault(o => (o.EXECUTE_ROOM_ID == item.ID || o.BED_ROOM_ID == item.ID) && o.ROOM_ID == checkRoomId);
                                        if (exroRoom != null)
                                        {
                                            var exroRoomResult = new HIS_EXRO_ROOM();
                                            AutoMapper.Mapper.CreateMap<V_HIS_EXRO_ROOM, HIS_EXRO_ROOM>();
                                            exroRoomResult = AutoMapper.Mapper.Map<HIS_EXRO_ROOM>(exroRoom);
                                            if (item.isAllowRequest)
                                            {
                                                exroRoomResult.IS_ALLOW_REQUEST = 1;
                                            }
                                            else
                                            {
                                                exroRoomResult.IS_ALLOW_REQUEST = null;
                                            }
                                            if (item.isPriorityRequire)
                                            {
                                                exroRoomResult.IS_PRIORITY_REQUIRE = 1;
                                            }
                                            else
                                            {
                                                exroRoomResult.IS_PRIORITY_REQUIRE = null;
                                            }
                                            if (item.isHoldOrder)
                                            {
                                                exroRoomResult.IS_HOLD_ORDER = 1;
                                            }
                                            else
                                            {
                                                exroRoomResult.IS_HOLD_ORDER = null;
                                            }
                                            //if (item.RoomType == "BED")
                                            //{
                                            //    exroRoomResult.BED_ROOM_ID = item.ID;
                                            //}
                                            //else
                                            //{
                                            //    exroRoomResult.BED_ROOM_ID = null;
                                            //}
                                            if (exroRoomResult.IS_HOLD_ORDER == null && exroRoomResult.IS_ALLOW_REQUEST == null)
                                            {
                                                executeRoomErrors.Add(item);
                                            }

                                            exroRoomUpdates.Add(exroRoomResult);
                                        }
                                    }
                                    if (exroRoomUpdates != null && exroRoomUpdates.Count > 0)
                                    {
                                        if (executeRoomErrors != null && executeRoomErrors.Count > 0)
                                        {
                                            var error = string.Empty;
                                            foreach (var item in executeRoomErrors)
                                            {
                                                error += item.EXECUTE_ROOM_CODE + " - " + item.EXECUTE_ROOM_NAME + "; ";
                                            }
                                            WaitingManager.Hide();
                                            MessageManager.Show("Các phòng sau phải chọn ít nhất 1 trong 2 'Được lấy số ưu tiên' và 'Được phép yêu cầu': " + error);
                                            return;
                                        }
                                        var updateResult = new BackendAdapter(param).Post<List<HIS_EXRO_ROOM>>(
                                                   "/api/HisExroRoom/UpdateList",
                                                   HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                                   exroRoomUpdates,
                                                   param);
                                        if (updateResult != null && updateResult.Count > 0)
                                        {
                                            AutoMapper.Mapper.CreateMap<HIS_EXRO_ROOM, V_HIS_EXRO_ROOM>();
                                            var vUpdateResults = AutoMapper.Mapper.Map<List<HIS_EXRO_ROOM>, List<V_HIS_EXRO_ROOM>>(updateResult);
                                            ExroRoomViews.AddRange(vUpdateResults);
                                            success = true;
                                        }
                                    }
                                }

                                if (dataDelete.Count == 0 && dataChecked.Count == 0)
                                {
                                    WaitingManager.Hide();
                                    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn phòng xử lý", "Thông báo");
                                    return;
                                }
                                if (dataChecked != null)
                                {
                                    success = true;
                                }

                                if (dataDelete != null && dataDelete.Count > 0)
                                {
                                    //var deleteId = ExroRoomViews.Where(o => dataDelete.Select(p => p.ID)
                                    //    .Contains(o.EXECUTE_ROOM_ID)).Select(o => o.ID).ToList();
                                    var deleteId = ExroRoomViews
                                        .Where(o => (o.EXECUTE_ROOM_ID.HasValue && dataDelete.Any(p => p.ID == o.EXECUTE_ROOM_ID.Value)) 
                                        || (o.BED_ROOM_ID.HasValue && dataDelete.Any(p => p.ID == o.BED_ROOM_ID.Value)))
                                        .Select(o => o.ID)
                                        .ToList();
                                    var deleteResult = new BackendAdapter(param).Post<bool>(
                                              "api/HisExroRoom/DeleteList",
                                              HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                              deleteId,
                                              param);
                                    if (deleteResult)
                                    {
                                        success = true;
                                    }
                                    ExroRoomViews = ExroRoomViews.Where(o => !deleteId.Contains(o.ID)).ToList();
                                }

                                if (dataCreate != null && dataCreate.Count > 0)
                                {
                                    var ServiceRoomCreate = new List<HIS_EXRO_ROOM>();
                                    var executeRoomErrors = new List<ExecuteRoomViewADO>();
                                    foreach (var item in dataCreate)
                                    {
                                        var ServiceRoomID = new HIS_EXRO_ROOM();
                                        ServiceRoomID.ROOM_ID = RoomIdCheckByRoom;
                                        if (item.RoomType == "BED")
                                        {
                                            ServiceRoomID.BED_ROOM_ID = item.ID;
                                        }
                                        else
                                        {
                                            ServiceRoomID.EXECUTE_ROOM_ID = item.ID;
                                        }
                                        if (item.isAllowRequest)
                                        {
                                            ServiceRoomID.IS_ALLOW_REQUEST = 1;
                                        }
                                        else
                                        {
                                            ServiceRoomID.IS_ALLOW_REQUEST = null;
                                        }
                                        if (item.isPriorityRequire)
                                        {
                                            ServiceRoomID.IS_PRIORITY_REQUIRE = 1;
                                        }
                                        else
                                        {
                                            ServiceRoomID.IS_PRIORITY_REQUIRE = null;
                                        }
                                        if (item.isHoldOrder)
                                        {
                                            ServiceRoomID.IS_HOLD_ORDER = 1;
                                        }
                                        else
                                        {
                                            ServiceRoomID.IS_HOLD_ORDER = null;
                                        }
                                        //if (item.RoomType == "BED")
                                        //{
                                        //    ServiceRoomID.BED_ROOM_ID = item.ID;
                                        //}
                                        //else
                                        //{
                                        //    ServiceRoomID.BED_ROOM_ID = null;
                                        //}
                                        if (ServiceRoomID.IS_HOLD_ORDER == null && ServiceRoomID.IS_ALLOW_REQUEST == null)
                                        {
                                            executeRoomErrors.Add(item);
                                        }
                                        ServiceRoomCreate.Add(ServiceRoomID);
                                    }
                                    if (executeRoomErrors != null && executeRoomErrors.Count > 0)
                                    {
                                        var error = string.Empty;
                                        foreach (var item in executeRoomErrors)
                                        {
                                            error += item.EXECUTE_ROOM_CODE + " - " + item.EXECUTE_ROOM_NAME + "; ";
                                        }
                                        WaitingManager.Hide();
                                        MessageManager.Show("Các phòng sau phải chọn ít nhất 1 trong 2 'Được lấy số ưu tiên' và 'Được phép yêu cầu': " + error);
                                        return;
                                    }

                                    var createResult = new BackendAdapter(param).Post<List<HIS_EXRO_ROOM>>(
                                               "/api/HisExroRoom/CreateList",
                                               HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                               ServiceRoomCreate,
                                               param);
                                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData($"ServiceRoomCreate", ServiceRoomCreate));
                                    if (createResult != null && createResult.Count > 0)
                                    {
                                        success = true;    
                                    }
                                    AutoMapper.Mapper.CreateMap<HIS_EXRO_ROOM, V_HIS_EXRO_ROOM>();
                                    var vCreateResults = AutoMapper.Mapper.Map<List<HIS_EXRO_ROOM>, List<V_HIS_EXRO_ROOM>>(createResult);
                                    ExroRoomViews.AddRange(vCreateResults);
                                }

                                lstExecuteRoomADOs = lstExecuteRoomADOs.OrderByDescending(p => p.check1).ToList();
                                if (ucGridControlRoom != null)
                                {
                                    ExecuteRoomViewProcessor.Reload(ucGridControlExecuteRoom, lstExecuteRoomADOs);
                                }
                            }
                        }
                    }

                    WaitingManager.Hide();
                    MessageManager.Show(ParentForm, param, success);
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
                        var data = RoomType.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboRoomType.EditValue.ToString()));
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

                var filter = new HisServiceTypeFilter();
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
                    var data = RoomType.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboRoomType.EditValue.ToString()));
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
                var listArgs = new List<object>();
                listArgs.Add((RefeshReference)RefreshData);
                if (currentModule != null)
                {
                    var callModule = new CallModule(CallModule.HisImportServiceRoom, currentModule.RoomId, currentModule.RoomTypeId, listArgs);
                }
                else
                {
                    var callModule = new CallModule(CallModule.HisImportServiceRoom, 0, 0, listArgs);
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
                if (currentExecuteRoom == null && currentBedRoom == null)
                {
                    FillDataToGrid1__MedicineType(this);
                    FillDataToGrid2__Room(this);
                }
                else
                {
                    FillDataToGrid1__ExecuteRoom_Default(this);
                    FillDataToGrid1__MedicineType(this);
                    var room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == currentExecuteRoom.ROOM_ID);
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
                            if (isChoseRoom != 2)
                            {
                                MessageManager.Show("Vui lòng chọn phòng!");
                                break;
                            }
                            CurrentRoomCopyAdo = (HIS.UC.Room.RoomAccountADO)sender;
                            break;
                        case HIS.UC.Room.Popup.PopupMenuProcessor.ItemType.PastePhongSangPhong:
                            {
                                var currentPaste = (HIS.UC.Room.RoomAccountADO)sender;
                                var success = false;
                                var param = new CommonParam();
                                if (CurrentRoomCopyAdo == null && isChoseRoom != 2)
                                {
                                    MessageManager.Show("Vui lòng copy!");
                                    break;
                                }
                                if (CurrentRoomCopyAdo != null && currentPaste != null && isChoseRoom == 2)
                                {
                                    if (CurrentRoomCopyAdo.ID == currentPaste.ID)
                                    {
                                        MessageManager.Show("Trùng dữ liệu copy và paste");
                                        break;
                                    }
                                    var hisMestMatyCopyByMatySDO = new HisExroRoomCopyByRoomSDO();
                                    hisMestMatyCopyByMatySDO.CopyRoomId = CurrentRoomCopyAdo.ID;
                                    hisMestMatyCopyByMatySDO.PasteRoomId = currentPaste.ID;
                                    var result = new BackendAdapter(param).Post<List<HIS_EXRO_ROOM>>("api/HisExroRoom/CopyByRoom", ApiConsumer.ApiConsumers.MosConsumer, hisMestMatyCopyByMatySDO, param);
                                    if (result != null)
                                    {
                                        success = true;
                                        var dataNew = new List<HIS.UC.ExecuteRoomView.ExecuteRoomViewADO>();
                                        dataNew = (from r in listExecuteRoom
                                                    select new HIS.UC.ExecuteRoomView.ExecuteRoomViewADO(r)).ToList();
                                        if (result != null && result.Count > 0)
                                        {
                                            foreach (var itemService in result)
                                            {
                                                var check = dataNew.FirstOrDefault(o => o.ID == itemService.EXECUTE_ROOM_ID);
                                                if (check != null)
                                                {
                                                    check.check1 = true;
                                                    if (itemService.IS_PRIORITY_REQUIRE == 1)
                                                    {
                                                        check.isPriorityRequire = true;
                                                    }
                                                    else
                                                    {
                                                        check.isPriorityRequire = false;
                                                    }
                                                    if (itemService.IS_HOLD_ORDER == 1)
                                                    {
                                                        check.isHoldOrder = true;
                                                    }
                                                    else
                                                    {
                                                        check.isHoldOrder = false;
                                                    }
                                                    if (itemService.IS_ALLOW_REQUEST == 1)
                                                    {
                                                        check.isAllowRequest = true;
                                                    }
                                                    else
                                                    {
                                                        check.isAllowRequest = false;
                                                    }
                                                }
                                            }


                                            dataNew = dataNew.OrderByDescending(p => p.check1).ToList();


                                            if (ucGridControlExecuteRoom != null)
                                            {
                                                ExecuteRoomViewProcessor.Reload(ucGridControlExecuteRoom, dataNew);
                                            }
                                        }
                                        else
                                        {
                                            FillDataToGrid1__MedicineType(this);
                                        }
                                        ExroRoomViews = new List<V_HIS_EXRO_ROOM>();
                                        AutoMapper.Mapper.CreateMap<HIS_EXRO_ROOM, V_HIS_EXRO_ROOM>();
                                        ExroRoomViews = AutoMapper.Mapper.Map<List<V_HIS_EXRO_ROOM>>(result);
                                    }
                                }
                                MessageManager.Show(ParentForm, param, success);
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
                if ((e.Item is BarButtonItem) && sender != null && sender is HIS.UC.ExecuteRoomView.ExecuteRoomViewADO)
                {
                    var type = (HIS.UC.ExecuteRoomView.Popup.PopupMenuProcessor.ItemType)e.Item.Tag;
                    switch (type)
                    {
                        case HIS.UC.ExecuteRoomView.Popup.PopupMenuProcessor.ItemType.Copy:
                            if (isChoseExecuteRoom != 1)
                            {
                                MessageManager.Show("Vui lòng chọn phòng xử lý!");
                                break;
                            }
                            currentExecuteRoom = (HIS.UC.ExecuteRoomView.ExecuteRoomViewADO)sender;
                            break;
                        case HIS.UC.ExecuteRoomView.Popup.PopupMenuProcessor.ItemType.Paste:
                            {
                                var currentPaste = (HIS.UC.ExecuteRoomView.ExecuteRoomViewADO)sender;
                                var success = false;
                                var param = new CommonParam();
                                if (currentExecuteRoom == null && isChoseExecuteRoom != 1)
                                {
                                    MessageManager.Show("Vui lòng copy!");
                                    break;
                                }
                                if (currentExecuteRoom != null && currentPaste != null && isChoseExecuteRoom == 1)
                                {
                                    if (currentExecuteRoom.ID == currentPaste.ID)
                                    {
                                        MessageManager.Show("Trùng dữ liệu copy và paste");
                                        break;
                                    }
                                    var hisMestMatyCopyByMatySDO = new HisExroRoomCopyByExroSDO();
                                    hisMestMatyCopyByMatySDO.CopyExecuteRoomId = currentExecuteRoom.ID;
                                    hisMestMatyCopyByMatySDO.PasteExecuteRoomId = currentPaste.ID;
                                    var result = new BackendAdapter(param).Post<List<HIS_EXRO_ROOM>>("api/HisExroRoom/CopyByExecuteRoom", ApiConsumer.ApiConsumers.MosConsumer, hisMestMatyCopyByMatySDO, param);
                                    if (result != null)
                                    {
                                        success = true;
                                        var dataNew = new List<HIS.UC.Room.RoomAccountADO>();
                                        dataNew = (from r in listRoom
                                                    select new RoomAccountADO(r)).ToList();
                                        if (result != null && result.Count > 0)
                                        {
                                            foreach (var itemRoom in result)
                                            {
                                                var check = dataNew.FirstOrDefault(o => o.ID == itemRoom.ROOM_ID);
                                                if (check != null)
                                                {
                                                    check.check1 = true;
                                                    if (itemRoom.IS_ALLOW_REQUEST == 1)
                                                    {
                                                        check.isAllowRequest = true;
                                                    }
                                                    else
                                                    {
                                                        check.isAllowRequest = false;
                                                    }
                                                    if (itemRoom.IS_HOLD_ORDER == 1)
                                                    {
                                                        check.isHoldOrder = true;
                                                    }
                                                    else
                                                    {
                                                        check.isHoldOrder = false;
                                                    }
                                                    if (itemRoom.IS_PRIORITY_REQUIRE == 1)
                                                    {
                                                        check.isPriorityRequire = true;
                                                    }
                                                    else
                                                    {
                                                        check.isPriorityRequire = false;
                                                    }
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
                                        ExroRooms = new List<V_HIS_EXRO_ROOM>();
                                        AutoMapper.Mapper.CreateMap<HIS_EXRO_ROOM, V_HIS_EXRO_ROOM>();
                                        ExroRooms = AutoMapper.Mapper.Map<List<V_HIS_EXRO_ROOM>>(result);
                                    }
                                }
                                MessageManager.Show(ParentForm, param, success);
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







