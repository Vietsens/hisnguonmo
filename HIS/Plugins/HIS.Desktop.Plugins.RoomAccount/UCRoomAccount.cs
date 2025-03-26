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
using HIS.UC.Room;
using HIS.UC.Room.ADO;
using HIS.UC.Account;
using HIS.UC.Account.ADO;
using HIS.Desktop.Plugins.RoomAccount.Entity;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.ADO;
using AutoMapper;
using ACS.Filter;
using HIS.Desktop.LocalStorage.BackendData;
using ACS.EFMODEL.DataModels;
using Inventec.Common.Adapter;
using HIS.Desktop.LocalStorage.ConfigApplication;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using DevExpress.XtraBars;
using MOS.SDO;
using HIS.UC.Room.Popup;
using HIS.Desktop.ApiConsumer;

namespace HIS.Desktop.Plugins.RoomAccount
{
    public partial class UCRoomAccount : HIS.Desktop.Utility.UserControlBase
    {
        List<HIS_ROOM_TYPE> roomType { get; set; }
        internal Inventec.Desktop.Common.Modules.Module currentModule;
        UCRoomProcessor RoomProcessor;
        AccountProcessor AccountProcessor;
        UserControl ucGridControlRoom;
        UserControl ucGridControlAccount;
        int rowCount = 0;
        int dataTotal = 0;
        int rowCount1 = 0;
        int dataTotal1 = 0;
        //internal List<HIS.UC.Account.AccountADO> lstAccountADOs { get; set; }
        internal List<HIS.UC.Room.RoomAccountADO> lstRoomADOs { get; set; }
        //List<ACS_USER> listUser;
        List<V_HIS_ROOM> listRoom;
        long roomIdCheckByRoom = 0;
        long isChoseRoom;
        long isChoseAccount;
        bool isCheckAll;
        String accountIdCheckByAccount = null;
        HIS.UC.Room.RoomAccountADO currentCopyRoom;
        HIS.UC.Account.AccountADO currentCopyAccount;

        internal List<HIS_USER_ROOM> userRooms { get; set; }
        internal List<V_HIS_USER_ROOM> userRoomViews { get; set; }

        private List<AccountADO> UserAdoTotal = new List<AccountADO>();
        private List<AccountADO> listUser;

        public UCRoomAccount(Inventec.Desktop.Common.Modules.Module _moduleData)
            : base(_moduleData)
        {
            InitializeComponent();
        }

        public UCRoomAccount(Inventec.Desktop.Common.Modules.Module currentModule, long roomTypeID)
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

        private void UCRoomAccount_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                LoadDataUserAdo();
                LoadDataToCombo();
                InitUcgrid1();
                InitUcgrid2();
                FillDataToGrid1(this);
                FillDataToGrid2(this);
                SetCaptionByLanguageKey();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.RoomAccount.Resources.Lang", typeof(HIS.Desktop.Plugins.RoomAccount.UCRoomAccount).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCRoomAccount.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword1.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCRoomAccount.txtKeyword1.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword2.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCRoomAccount.txtKeyword2.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboChoose.Properties.NullText = Inventec.Common.Resource.Get.Value("UCRoomAccount.cboChoose.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("UCRoomAccount.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UCRoomAccount.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnFind2.Text = Inventec.Common.Resource.Get.Value("UCRoomAccount.btnFind2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("UCRoomAccount.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnFind1.Text = Inventec.Common.Resource.Get.Value("UCRoomAccount.btnFind1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboRoomType.Properties.NullText = Inventec.Common.Resource.Get.Value("UCRoomAccount.cboRoomType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("UCRoomAccount.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("UCRoomAccount.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataUserAdo()
        {
            try
            {
                List<ACS_USER> acsUser = BackendDataWorker.Get<ACS_USER>();
                List<V_HIS_EMPLOYEE> employee = BackendDataWorker.Get<V_HIS_EMPLOYEE>();
                if (acsUser != null && acsUser.Count > 0)
                {
                    foreach (var item in acsUser)
                    {
                        HIS.UC.Account.AccountADO userADO = new HIS.UC.Account.AccountADO(item);

                        if (employee != null && employee.Count > 0)
                        {
                            var HisEmployee = employee.FirstOrDefault(o => o.LOGINNAME == item.LOGINNAME);
                            if (HisEmployee != null)
                            {
                                userADO.DOB = HisEmployee.DOB;
                                userADO.DIPLOMA = HisEmployee.DIPLOMA;
                                userADO.DEPARTMENT_ID = HisEmployee.DEPARTMENT_ID;
                                userADO.DEPARTMENT_NAME = HisEmployee.DEPARTMENT_NAME;
                            }
                        }

                        this.UserAdoTotal.Add(userADO);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitUcgrid1()
        {
            try
            {
                RoomProcessor = new UCRoomProcessor();
                RoomInitADO ado = new RoomInitADO();
                ado.ListRoomColumn = new List<RoomColumn>();
                ado.gridViewRoom_MouseDownRoom = gridViewRoom_MouseDownRoom;
                ado.btn_Radio_Enable_Click = btn_Radio_Enable_Click;
                ado.rooom_MouseRightClick = this.rooom_MouseRightClick;

                RoomColumn colRadio1 = new RoomColumn("   ", "radio1", 30, true);
                colRadio1.VisibleIndex = 0;
                colRadio1.Visible = false;
                colRadio1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListRoomColumn.Add(colRadio1);

                RoomColumn colCheck1 = new RoomColumn("   ", "check1", 30, true);
                colCheck1.VisibleIndex = 1;
                colCheck1.image = imageCollectionRoom1.Images[0];
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
                    this.panelControl1.Controls.Add(this.ucGridControlRoom);
                    this.ucGridControlRoom.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewRoom_MouseDownRoom(object sender, MouseEventArgs e)
        {
            try
            {
                if (isChoseRoom == 1)
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
                            List<HIS.UC.Room.RoomAccountADO> lstRoomChecks = new List<HIS.UC.Room.RoomAccountADO>();

                            if (lstCheckAll != null && lstCheckAll.Count > 0)
                            {
                                var roomCheckedNum = lstRoomADOs.Where(o => o.check1 == true).Count();
                                var roomNum = lstRoomADOs.Count();
                                if ((roomCheckedNum > 0 && roomCheckedNum < roomNum) || roomCheckedNum == 0)
                                {
                                    isCheckAll = true;
                                    hi.Column.Image = imageCollectionRoom1.Images[1];
                                }

                                if (roomCheckedNum == roomNum)
                                {
                                    isCheckAll = false;
                                    hi.Column.Image = imageCollectionRoom1.Images[0];
                                }

                                if (isCheckAll)
                                {
                                    foreach (var item in lstCheckAll)
                                    {
                                        if (item.ID != null)
                                        {
                                            item.check1 = true;
                                            lstRoomChecks.Add(item);
                                        }
                                        else
                                        {
                                            lstRoomChecks.Add(item);
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
                                            lstRoomChecks.Add(item);
                                        }
                                        else
                                        {
                                            lstRoomChecks.Add(item);
                                        }
                                    }
                                    isCheckAll = true;
                                }

                                RoomProcessor.Reload(ucGridControlRoom, lstRoomChecks);
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

        private void btn_Radio_Enable_Click(V_HIS_ROOM data)
        {
            try
            {

                WaitingManager.Show();
                CommonParam param = new CommonParam();
                MOS.Filter.HisUserRoomFilter filter = new HisUserRoomFilter();
                filter.ROOM_ID = data.ID;
                roomIdCheckByRoom = data.ID;
                userRooms = new BackendAdapter(param).Get<List<HIS_USER_ROOM>>(
                                HIS.Desktop.ApiConsumer.HisRequestUriStore.HIS_USER_ROOM_GET,
                                HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                filter,
                                param);
                FillDataToGrid2(this);
                if (userRooms != null && userRooms.Count > 0)
                {
                    foreach (var itemUsername in userRooms)
                    {
                        var check = listUser.FirstOrDefault(o => o.LOGINNAME == itemUsername.LOGINNAME);
                        if (check != null)
                        {
                            check.check2 = true;
                        }
                    }

                    if (chkHasValue.Checked)
                    {
                        listUser = listUser.Where(o => userRooms.Select(p => p.LOGINNAME).Contains(o.LOGINNAME)).ToList();
                    }

                    listUser = listUser.OrderByDescending(p => p.check2).ToList();
                    if (ucGridControlAccount != null)
                    {
                        AccountProcessor.Reload(ucGridControlAccount, listUser);
                    }
                }
                else
                {
                    if (chkHasValue.Checked)
                    {
                        if (ucGridControlAccount != null)
                        {
                            AccountProcessor.Reload(ucGridControlAccount, null);
                        }
                    }
                    else
                    {
                        FillDataToGrid2(this);
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

        private void InitUcgrid2()
        {
            try
            {
                AccountProcessor = new AccountProcessor();
                AccountInitADO ado = new AccountInitADO();
                ado.ListAccountColumn = new List<UC.Account.AccountColumn>();
                ado.gridViewAccount_MouseDownAccount = gridViewAccount_MouseDownAccount;
                ado.btn_Radio_Enable_Click1 = btn_Radio_Enable_Click1;
                ado.GridView_MouseRightClick = GridViewAccount_MouseRightClick;
                ado.AccountGrid_CustomUnboundColumnData = gridViewUser_CustomUnboundColumnData;

                AccountColumn colRadio2 = new AccountColumn("   ", "radio2", 30, true);
                colRadio2.VisibleIndex = 0;
                colRadio2.Visible = false;
                colRadio2.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListAccountColumn.Add(colRadio2);

                AccountColumn colCheck2 = new AccountColumn("   ", "check2", 30, true);
                colCheck2.VisibleIndex = 1;
                colCheck2.Visible = false;
                colCheck2.image = imageCollectionAccount.Images[0];
                colCheck2.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListAccountColumn.Add(colCheck2);

                AccountColumn colTenDangNhap = new AccountColumn("Tên đăng nhập", "LOGINNAME", 120, false);
                colTenDangNhap.VisibleIndex = 2;
                ado.ListAccountColumn.Add(colTenDangNhap);

                AccountColumn colHoTen = new AccountColumn("Họ tên", "USERNAME", 120, false);
                colHoTen.VisibleIndex = 3;
                ado.ListAccountColumn.Add(colHoTen);

                AccountColumn colNgaySinh = new AccountColumn("Ngày sinh", "DOB_STR", 150, false);
                colNgaySinh.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                colNgaySinh.VisibleIndex = 4;
                ado.ListAccountColumn.Add(colNgaySinh);

                AccountColumn colSDT = new AccountColumn("Số điện thoại", "MOBILE", 150, false);
                colSDT.VisibleIndex = 5;
                ado.ListAccountColumn.Add(colSDT);

                AccountColumn colEmail = new AccountColumn("Email", "EMAIL", 200, false);
                colEmail.VisibleIndex = 6;
                ado.ListAccountColumn.Add(colEmail);

                AccountColumn colCCHN = new AccountColumn("Chứng chỉ hành nghề", "DIPLOMA", 300, false);
                colCCHN.VisibleIndex = 7;
                ado.ListAccountColumn.Add(colCCHN);

                AccountColumn colKhoaPhong = new AccountColumn("Khoa phòng", "DEPARTMENT_NAME", 400, false);
                colKhoaPhong.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                colKhoaPhong.VisibleIndex = 8;
                ado.ListAccountColumn.Add(colKhoaPhong);

                this.ucGridControlAccount = (UserControl)AccountProcessor.Run(ado);
                if (ucGridControlAccount != null)
                {
                    this.panelControl2.Controls.Add(this.ucGridControlAccount);
                    this.ucGridControlAccount.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewUser_CustomUnboundColumnData(ACS_USER data, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (data != null)
                {
                    var currentRow = this.listUser.FirstOrDefault(o => o.LOGINNAME == data.LOGINNAME);
                    if (currentRow != null)
                    {
                        if (e.Column.FieldName == "DOB_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(currentRow.DOB ?? 0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewAccount_MouseDownAccount(object sender, MouseEventArgs e)
        {
            try
            {
                if (isChoseAccount == 2)
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
                            if (listUser != null && listUser.Count > 0)
                            {
                                if (isCheckAll)
                                {
                                    foreach (var item in listUser)
                                    {
                                        item.check2 = true;
                                    }

                                    hi.Column.Image = imageCollectionAccount.Images[0];
                                    isCheckAll = false;
                                }
                                else
                                {
                                    foreach (var item in listUser)
                                    {
                                        item.check2 = false;
                                    }

                                    hi.Column.Image = imageCollectionAccount.Images[1];
                                    isCheckAll = true;
                                }

                                AccountProcessor.Reload(ucGridControlAccount, listUser);
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

        private void btn_Radio_Enable_Click1(ACS_USER data)
        {
            try
            {

                CommonParam param = new CommonParam();
                MOS.Filter.HisUserRoomViewFilter filter = new HisUserRoomViewFilter();
                filter.LOGINNAME = data.LOGINNAME;
                accountIdCheckByAccount = data.LOGINNAME;
                userRoomViews = new BackendAdapter(param).Get<List<V_HIS_USER_ROOM>>(
                                HIS.Desktop.ApiConsumer.HisRequestUriStore.HIS_USER_ROOM_GETVIEW,
                                HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                filter,
                                param);
                FillDataToGrid1(this);
                lstRoomADOs = new List<HIS.UC.Room.RoomAccountADO>();
                lstRoomADOs = (from r in listRoom select new RoomAccountADO(r)).ToList();
                if (userRoomViews != null && userRoomViews.Count > 0)
                {

                    foreach (var itemUsername in userRoomViews)
                    {
                        var check = lstRoomADOs.FirstOrDefault(o => o.ID == itemUsername.ROOM_ID);
                        if (check != null)
                        {
                            check.check1 = true;
                        }
                    }

                    if (chkHasValue.Checked)
                    {
                        lstRoomADOs = lstRoomADOs.Where(o => userRoomViews.Select(p => p.ROOM_ID).Contains(o.ID)).ToList();
                    }

                    lstRoomADOs = lstRoomADOs.OrderByDescending(p => p.check1).ToList();
                    if (ucGridControlRoom != null)
                    {
                        RoomProcessor.Reload(ucGridControlRoom, lstRoomADOs);
                    }

                    //lstRoomADOs = lstRoomADOs.OrderByDescending(p => !p.check1).ToList();
                    //if (ucGridControlRoom != null)
                    //{
                    //    RoomProcessor.Reload(ucGridControlRoom, lstRoomADOs);
                    //}
                }
                else
                {
                    if (chkHasValue.Checked)
                    {
                        if (ucGridControlRoom != null)
                        {
                            RoomProcessor.Reload(ucGridControlRoom, null);
                        }
                    }
                    else
                    {
                        FillDataToGrid1(this);
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGrid2(UCRoomAccount uCRoomAccount)
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
                FillDataToGridAccount(new CommonParam(0, numPageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount1;
                param.Count = dataTotal1;
                ucPaging2.Init(FillDataToGridAccount, param, numPageSize);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridAccount(object data)
        {
            try
            {
                WaitingManager.Show();
                isChoseAccount = (long)cboChoose.EditValue;

                var startUser = ((CommonParam)data).Start ?? 0;
                var limitUser = ((CommonParam)data).Limit ?? 0;
                var query = UserAdoTotal.AsQueryable();
                string keyword = txtKeyword2.Text.Trim();
                if (!String.IsNullOrEmpty(keyword))
                {
                    query = query.Where(o =>
                            (!String.IsNullOrEmpty(o.DIPLOMA) ? o.DIPLOMA : "").ToUpper().Contains(keyword.ToUpper())
                            || (!String.IsNullOrEmpty(o.EMAIL) ? o.EMAIL : "").ToUpper().Contains(keyword.ToUpper())
                            || (!String.IsNullOrEmpty(o.DEPARTMENT_NAME) ? o.DEPARTMENT_NAME : "").ToUpper().Contains(keyword.ToUpper())
                            || (!String.IsNullOrEmpty(o.MOBILE) ? o.MOBILE : "").ToUpper().Contains(keyword.ToUpper())
                            || (!String.IsNullOrEmpty(o.LOGINNAME) ? o.LOGINNAME : "").ToUpper().Contains(keyword.ToUpper())
                            || (!String.IsNullOrEmpty(o.ACCOUNT_NUMBER) ? o.ACCOUNT_NUMBER : "").ToUpper().Contains(keyword.ToUpper())
                            || (!String.IsNullOrEmpty(o.BANK) ? o.BANK : "").ToUpper().Contains(keyword.ToUpper())
                            || (!String.IsNullOrEmpty(o.USERNAME) ? o.USERNAME : "").ToUpper().Contains(keyword.ToUpper())
                            );
                }

                if (!chkIsActive.Checked)
                {
                    query = query.Where(o => o.IS_ACTIVE == 1);
                }

                query = query.OrderByDescending(o => o.check2).ThenBy(o => o.LOGINNAME);
                dataTotal1 = query.Count();
                this.listUser = query.Skip(startUser).Take(limitUser).ToList();
                rowCount1 = (listUser == null ? 0 : listUser.Count);

                foreach (var item in listUser)
                {
                    item.isKeyChoose1 = (isChoseAccount == 2);
                    item.check2 = ((userRooms != null && userRooms.Count > 0) ? userRooms.Exists(e => e.LOGINNAME == item.LOGINNAME) : false);
                }

                AccountProcessor.Reload(ucGridControlAccount, listUser);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGrid1(UCRoomAccount uCRoomAccount)
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
                FillDataToGridRoom(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridRoom, param, numPageSize);
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
                int start = ((CommonParam)data).Start ?? 0;
                int limit = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start, limit);
                MOS.Filter.HisRoomFilter RoomFillter = new HisRoomFilter();
                RoomFillter.ORDER_FIELD = "DEPARTMENT_NAME";
                RoomFillter.ORDER_DIRECTION = "ASC";
                RoomFillter.KEY_WORD = txtKeyword1.Text;
                RoomFillter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                if (cboRoomType.EditValue != null)
                    RoomFillter.ROOM_TYPE_ID = (long)cboRoomType.EditValue;

                if ((long)cboChoose.EditValue == 1)
                {
                    isChoseRoom = (long)cboChoose.EditValue;
                }

                var rs = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_ROOM>>(
                     HIS.Desktop.ApiConsumer.HisRequestUriStore.HIS_ROOM_GETVIEW,
                     HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                     RoomFillter,
                     param);

                lstRoomADOs = new List<RoomAccountADO>();

                if (rs != null && rs.Data.Count > 0)
                {
                    //List<V_HIS_ROOM> dataByRoom = new List<V_HIS_ROOM>();
                    listRoom = rs.Data;
                    foreach (var item in listRoom)
                    {
                        RoomAccountADO RoomAccountADO = new RoomAccountADO(item);
                        if (isChoseRoom == 1)
                        {
                            RoomAccountADO.isKeyChoose = true;
                        }
                        lstRoomADOs.Add(RoomAccountADO);
                    }
                }

                if (userRoomViews != null && userRoomViews.Count > 0)
                {
                    if (chkHasValue.Checked)
                    {
                        List<V_HIS_ROOM> roomCheck = new List<V_HIS_ROOM>();
                        HisRoomViewFilter roomFilterGet = new HisRoomViewFilter();
                        roomFilterGet.ORDER_FIELD = "ROOM_CODE";
                        roomFilterGet.ORDER_DIRECTION = "ASC";
                        roomFilterGet.KEY_WORD = txtKeyword1.Text;
                        roomFilterGet.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;

                        if (userRoomViews.Count <= 100)
                        {
                            roomFilterGet.IDs = userRoomViews.Select(o => o.ROOM_ID).ToList();
                            roomCheck = new BackendAdapter(param).Get<List<V_HIS_ROOM>>("api/HisRoom/GetView", ApiConsumer.ApiConsumers.MosConsumer, roomFilterGet, param);
                        }
                        else
                        {
                            int dem = userRoomViews.Count / 100;
                            if (userRoomViews.Count % 100 != 0)
                            {
                                dem = dem + 1;
                            }

                            for (int i = 0; i < dem; i++)
                            {
                                roomFilterGet.IDs = userRoomViews.Skip(100 * i).Take(100).Select(o => o.ID).ToList();
                                var roomGet = new BackendAdapter(param).Get<List<V_HIS_ROOM>>("api/HisRoom/GetView", ApiConsumer.ApiConsumers.MosConsumer, roomFilterGet, param);

                                if (roomGet != null && roomGet.Count > 0)
                                {
                                    roomCheck.AddRange(roomGet);
                                }
                            }

                        }

                        lstRoomADOs = new List<RoomAccountADO>();
                        if (roomCheck != null && roomCheck.Count > 0)
                        {
                            listRoom = roomCheck;

                            foreach (var item in listRoom)
                            {
                                RoomAccountADO roomAccountADO = new RoomAccountADO(item);
                                if (isChoseRoom == 1)
                                {
                                    roomAccountADO.isKeyChoose = true;
                                }

                                lstRoomADOs.Add(roomAccountADO);
                            }
                        }
                    }

                    foreach (var itemUsername in userRoomViews)
                    {
                        var check = lstRoomADOs.FirstOrDefault(o => o.ID == itemUsername.ROOM_ID);
                        if (check != null)
                        {
                            check.check1 = true;
                        }
                    }

                }
                lstRoomADOs = lstRoomADOs.OrderByDescending(p => p.check1).ToList();

                if (lstRoomADOs != null && lstRoomADOs.Count > 0)
                {
                    CommonParam paramCommon = new CommonParam();
                    Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.V_HIS_DATA_STORE>> apiResult = null;
                    HisDataStoreViewFilter filter = new HisDataStoreViewFilter();
                    apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_DATA_STORE>>("api/HisDataStore/GetView", ApiConsumers.MosConsumer, filter, paramCommon);
                    if (apiResult != null)
                    {
                        var rsdata = (List<MOS.EFMODEL.DataModels.V_HIS_DATA_STORE>)apiResult.Data;
                        foreach (var item in rsdata)
                        {
                            foreach (var subitem in lstRoomADOs)
                            {
                                if (item.ROOM_ID == subitem.ID)
                                {
                                    subitem.IS_ACTIVE = item.IS_ACTIVE;
                                    break;
                                }
                            }
                        }
                    }
                    lstRoomADOs = lstRoomADOs.Where(o => o.IS_ACTIVE == 1).ToList();
                }

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
                CommonParam param = new CommonParam();
                MOS.Filter.HisRoomTypeFilter RoomTypeFilter = new HisRoomTypeFilter();
                roomType = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_ROOM_TYPE>>(
                    HIS.Desktop.ApiConsumer.HisRequestUriStore.HIS_ROOM_TYPE_GET,
                    HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                    RoomTypeFilter,
                    param);
                LoadDataToComboRoomType(cboRoomType, roomType);
                LoadComboStatus();
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
                status.Add(new Status(1, "Phòng"));
                status.Add(new Status(2, "Tài khoản"));

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

        private void LoadDataToComboRoomType(DevExpress.XtraEditors.GridLookUpEdit cboRoomType, List<HIS_ROOM_TYPE> roomType)
        {
            try
            {
                cboRoomType.Properties.DataSource = roomType;
                cboRoomType.Properties.DisplayMember = "ROOM_TYPE_NAME";
                cboRoomType.Properties.ValueMember = "ID";

                cboRoomType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboRoomType.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboRoomType.Properties.ImmediatePopup = true;
                cboRoomType.ForceInitialize();
                cboRoomType.Properties.View.Columns.Clear();

                GridColumn aColumnCode = cboRoomType.Properties.View.Columns.AddField("ROOM_TYPE_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 100;

                GridColumn aColumnName = cboRoomType.Properties.View.Columns.AddField("ROOM_TYPE_NAME");
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
                isChoseAccount = 0;
                isChoseRoom = 0;
                userRooms = null;
                userRoomViews = null;
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
                if (ucGridControlAccount != null && ucGridControlRoom != null)
                {
                    object account = AccountProcessor.GetDataGridView(ucGridControlAccount);
                    object room = RoomProcessor.GetDataGridView(ucGridControlRoom);
                    bool success = false;
                    CommonParam param = new CommonParam();
                    if (isChoseRoom == 1)
                    {
                        if (listUser != null && listUser.Count > 0)
                        {
                            //Danh sach cac user duoc check
                            List<String> listLoginName = userRooms.Select(p => p.LOGINNAME).ToList();

                            var dataCheckeds = listUser.Where(p => p.check2 == true).ToList();

                            //List xoa

                            var dataDeletes = listUser.Where(o => userRooms.Select(p => p.LOGINNAME)
                                .Contains(o.LOGINNAME) && o.check2 == false).ToList();

                            //list them
                            var dataCreates = dataCheckeds.Where(o => !userRooms.Select(p => p.LOGINNAME)
                                .Contains(o.LOGINNAME)).ToList();

                            if (dataCheckeds.Count == 0 && dataDeletes.Count == 0)
                            {
                                DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn phòng", "Thông báo");
                                return;
                            }
                            if (dataCheckeds != null)
                            {
                                success = true;
                            }

                            if (dataDeletes != null && dataDeletes.Count > 0)
                            {
                                List<long> deleteIds = userRooms.Where(o => dataDeletes.Select(p => p.LOGINNAME)
                                    .Contains(o.LOGINNAME)).Select(o => o.ID).ToList();
                                bool deleteResult = new BackendAdapter(param).Post<bool>(
                                          "/api/HisUserRoom/DeleteList",
                                          HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                          deleteIds,
                                          param);
                                if (deleteResult)
                                {
                                    success = true;
                                    userRooms = userRooms.Where(o => !deleteIds.Contains(o.ID)).ToList();
                                }
                            }

                            if (dataCreates != null && dataCreates.Count > 0)
                            {
                                List<HIS_USER_ROOM> userRoomCreates = new List<HIS_USER_ROOM>();
                                foreach (var item in dataCreates)
                                {
                                    HIS_USER_ROOM userRoom = new HIS_USER_ROOM();
                                    userRoom.LOGINNAME = item.LOGINNAME;
                                    userRoom.ROOM_ID = roomIdCheckByRoom;
                                    userRoomCreates.Add(userRoom);
                                }

                                var createResult = new BackendAdapter(param).Post<List<HIS_USER_ROOM>>(
                                           "/api/HisUserRoom/CreateList",
                                           HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                           userRoomCreates,
                                           param);
                                if (createResult != null && createResult.Count > 0)
                                {
                                    success = true;
                                    userRooms.AddRange(createResult);
                                }
                            }

                            listUser = listUser.OrderByDescending(p => p.check2).ToList();
                            if (ucGridControlAccount != null)
                            {
                                AccountProcessor.Reload(ucGridControlAccount, listUser);
                            }
                            else
                            {
                                FillDataToGrid2(this);
                            }
                        }
                    }
                    if (isChoseAccount == 2)
                    {
                        if (accountIdCheckByAccount == null)
                        {
                            //success = true;
                            WaitingManager.Hide();
                            return;
                        }
                        if (room is List<HIS.UC.Room.RoomAccountADO>)
                        {
                            lstRoomADOs = (List<HIS.UC.Room.RoomAccountADO>)room;

                            if (lstRoomADOs != null && lstRoomADOs.Count > 0)
                            {
                                //Danh sach cac user duoc check

                                var listRoomID = userRoomViews.Select(p => p.ROOM_ID).ToList();

                                var dataChecked = lstRoomADOs.Where(p => p.check1 == true).ToList();

                                //List xoa

                                var dataDelete = lstRoomADOs.Where(o => userRoomViews.Select(p => p.ROOM_ID)
                                    .Contains(o.ID) && o.check1 == false).ToList();

                                //list them
                                var dataCreate = dataChecked.Where(o => !userRoomViews.Select(p => p.ROOM_ID)
                                    .Contains(o.ID)).ToList();

                                if (dataChecked.Count == 0 && dataDelete.Count == 0)
                                {
                                    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn phòng", "Thông báo");
                                    return;
                                }
                                if (dataChecked != null)
                                {
                                    success = true;
                                }

                                if (dataDelete != null && dataDelete.Count > 0)
                                {
                                    List<long> deleteId = userRoomViews.Where(o => dataDelete.Select(p => p.ID)

                                        .Contains(o.ROOM_ID)).Select(o => o.ID).ToList();
                                    bool deleteResult = new BackendAdapter(param).Post<bool>(
                                              "/api/HisUserRoom/DeleteList",
                                              HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                              deleteId,
                                              param);
                                    if (deleteResult)
                                    {
                                        success = true;
                                        userRoomViews = userRoomViews.Where(o => !deleteId.Contains(o.ID)).ToList();
                                    }
                                }

                                if (dataCreate != null && dataCreate.Count > 0)
                                {
                                    List<HIS_USER_ROOM> userRoomCreate = new List<HIS_USER_ROOM>();
                                    foreach (var item in dataCreate)
                                    {
                                        HIS_USER_ROOM userRoomID = new HIS_USER_ROOM();
                                        userRoomID.ROOM_ID = item.ID;
                                        userRoomID.LOGINNAME = accountIdCheckByAccount;
                                        userRoomCreate.Add(userRoomID);
                                    }

                                    var createResults = new BackendAdapter(param).Post<List<HIS_USER_ROOM>>(
                                               "/api/HisUserRoom/CreateList",
                                               HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                               userRoomCreate,
                                               param);
                                    if (createResults != null && createResults.Count > 0)
                                    {
                                        success = true;
                                        AutoMapper.Mapper.CreateMap<HIS_USER_ROOM, V_HIS_USER_ROOM>();
                                        var vCreateResults = AutoMapper.Mapper.Map<List<HIS_USER_ROOM>, List<V_HIS_USER_ROOM>>(createResults);
                                        userRoomViews.AddRange(vCreateResults);
                                    }
                                }

                                lstRoomADOs = lstRoomADOs.OrderByDescending(p => p.check1).ToList();
                                if (ucGridControlRoom != null)
                                {
                                    RoomProcessor.Reload(ucGridControlRoom, lstRoomADOs);
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

        private void txtKeyword2_KeyUp(object sender, KeyEventArgs e)
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
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyword1_KeyUp(object sender, KeyEventArgs e)
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
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboRoomType_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboRoomType.Properties.Buttons[1].Visible = false;
                    cboRoomType.EditValue = null;
                }

                HisRoomTypeFilter filter = new HisRoomTypeFilter();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboRoomType_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboRoomType.EditValue != null)
                    {
                        HIS_ROOM_TYPE data = roomType.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboRoomType.EditValue.ToString()));
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

        private void rooom_MouseRightClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if ((e.Item is BarButtonItem) && sender != null && sender is HIS.UC.Room.RoomAccountADO)
                {
                    var type = (PopupMenuProcessor.ItemType)e.Item.Tag;
                    switch (type)
                    {
                        case PopupMenuProcessor.ItemType.CopyPhongSangPhong:
                            {
                                if (isChoseRoom != 1)
                                {
                                    MessageManager.Show("Vui lòng chọn phòng!");
                                    break;
                                }
                                this.currentCopyRoom = (HIS.UC.Room.RoomAccountADO)sender;
                                break;
                            }
                        case PopupMenuProcessor.ItemType.PastePhongSangPhong:
                            {
                                var currentPasteRoom = (HIS.UC.Room.RoomAccountADO)sender;
                                bool success = false;
                                CommonParam param = new CommonParam();
                                if (isChoseRoom != 1)
                                {
                                    MessageManager.Show("Vui lòng chọn phòng!");
                                    break;
                                }
                                if (this.currentCopyRoom == null)
                                {
                                    MessageManager.Show("Vui lòng copy!");
                                    break;
                                }
                                if (this.currentCopyRoom != null && currentPasteRoom != null && isChoseRoom == 1)
                                {
                                    if (this.currentCopyRoom.ID == currentPasteRoom.ID)
                                    {
                                        MessageManager.Show("Trùng dữ liệu copy và paste");
                                        break;
                                    }
                                    HisUserRoomCopyByRoomSDO hisUserRoomCopyByRoomSDO = new HisUserRoomCopyByRoomSDO();
                                    hisUserRoomCopyByRoomSDO.CopyRoomId = currentCopyRoom.ID;
                                    hisUserRoomCopyByRoomSDO.PasteRoomId = currentPasteRoom.ID;
                                    var result = new BackendAdapter(param).Post<List<HIS_USER_ROOM>>("api/HisUserRoom/CopyByRoom", ApiConsumer.ApiConsumers.MosConsumer, hisUserRoomCopyByRoomSDO, param);
                                    if (result != null)
                                    {
                                        success = true;
                                        FillDataToGrid2(this);
                                        userRooms = result;
                                        if (userRooms != null && userRooms.Count > 0)
                                        {
                                            foreach (var itemUsername in userRooms)
                                            {
                                                var check = listUser.FirstOrDefault(o => o.LOGINNAME == itemUsername.LOGINNAME);
                                                if (check != null)
                                                {
                                                    check.check2 = true;
                                                }
                                            }

                                            if (chkHasValue.Checked)
                                            {
                                                listUser = listUser.Where(o => userRooms.Select(p => p.LOGINNAME).Contains(o.LOGINNAME)).ToList();
                                            }

                                            listUser = listUser.OrderByDescending(p => p.check2).ToList();
                                            if (ucGridControlAccount != null)
                                            {
                                                AccountProcessor.Reload(ucGridControlAccount, listUser);
                                            }
                                        }
                                        else
                                        {
                                            if (chkHasValue.Checked)
                                            {
                                                if (ucGridControlAccount != null)
                                                {
                                                    AccountProcessor.Reload(ucGridControlAccount, null);
                                                }
                                            }
                                            else
                                            {
                                                FillDataToGrid2(this);
                                            }
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

        private void GridViewAccount_MouseRightClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if ((e.Item is BarButtonItem) && sender != null && sender is HIS.UC.Account.AccountADO)
                {
                    var type = (HIS.UC.Account.Popup.PopupMenuProcessor.ItemType)e.Item.Tag;
                    switch (type)
                    {
                        case HIS.UC.Account.Popup.PopupMenuProcessor.ItemType.Copy:
                            {
                                if (isChoseAccount != 2)
                                {
                                    MessageManager.Show("Vui lòng chọn tài khoản!");
                                    break;
                                }
                                this.currentCopyAccount = (HIS.UC.Account.AccountADO)sender;
                                break;
                            }
                        case HIS.UC.Account.Popup.PopupMenuProcessor.ItemType.Paste:
                            {
                                bool success = false;
                                CommonParam param = new CommonParam();
                                var currentPasteAccount = (HIS.UC.Account.AccountADO)sender;
                                if (isChoseAccount != 2)
                                {
                                    MessageManager.Show("Vui lòng chọn tài khoản!");
                                    break;
                                }
                                if (this.currentCopyAccount == null)
                                {
                                    MessageManager.Show("Vui lòng copy!");
                                    break;
                                }
                                if (this.currentCopyAccount != null && currentPasteAccount != null && isChoseAccount == 2)
                                {
                                    if (this.currentCopyAccount.LOGINNAME == currentPasteAccount.LOGINNAME)
                                    {
                                        MessageManager.Show("Trùng dữ liệu copy và paste");
                                        break;
                                    }
                                    HisUserRoomCopyByLoginnameSDO hisUserRoomCopyByLoginnameSDO = new HisUserRoomCopyByLoginnameSDO();
                                    hisUserRoomCopyByLoginnameSDO.CopyLoginname = currentCopyAccount.LOGINNAME;
                                    hisUserRoomCopyByLoginnameSDO.PasteLoginname = currentPasteAccount.LOGINNAME;
                                    var result = new BackendAdapter(param).Post<List<HIS_USER_ROOM>>("api/HisUserRoom/CopyByLoginname", ApiConsumer.ApiConsumers.MosConsumer, hisUserRoomCopyByLoginnameSDO, param);
                                    if (result != null)
                                    {
                                        success = true;
                                        FillDataToGrid1(this);
                                        AutoMapper.Mapper.CreateMap<HIS_USER_ROOM, V_HIS_USER_ROOM>();
                                        userRoomViews = AutoMapper.Mapper.Map<List<HIS_USER_ROOM>, List<V_HIS_USER_ROOM>>(result);
                                        lstRoomADOs = new List<HIS.UC.Room.RoomAccountADO>();
                                        lstRoomADOs = (from r in listRoom select new RoomAccountADO(r)).ToList();
                                        if (userRoomViews != null && userRoomViews.Count > 0)
                                        {

                                            foreach (var itemUsername in userRoomViews)
                                            {
                                                var check = lstRoomADOs.FirstOrDefault(o => o.ID == itemUsername.ROOM_ID);
                                                if (check != null)
                                                {
                                                    check.check1 = true;
                                                }
                                            }

                                            if (chkHasValue.Checked)
                                            {
                                                lstRoomADOs = lstRoomADOs.Where(o => userRoomViews.Select(p => p.ROOM_ID).Contains(o.ID)).ToList();
                                            }

                                            lstRoomADOs = lstRoomADOs.OrderByDescending(p => p.check1).ToList();
                                            if (ucGridControlRoom != null)
                                            {
                                                RoomProcessor.Reload(ucGridControlRoom, lstRoomADOs);
                                            }

                                            //lstRoomADOs = lstRoomADOs.OrderByDescending(p => !p.check1).ToList();
                                            //if (ucGridControlRoom != null)
                                            //{
                                            //    RoomProcessor.Reload(ucGridControlRoom, lstRoomADOs);
                                            //}
                                        }
                                        else
                                        {
                                            if (chkHasValue.Checked)
                                            {
                                                if (ucGridControlRoom != null)
                                                {
                                                    RoomProcessor.Reload(ucGridControlRoom, null);
                                                }
                                            }
                                            else
                                            {
                                                FillDataToGrid1(this);
                                            }
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
