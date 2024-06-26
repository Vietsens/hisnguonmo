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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ACS.EFMODEL.DataModels;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.Plugins.UserAccountBook;
using HIS.UC.AccountBook;
using ACS.UC.User;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.BackendData;
using DevExpress.XtraBars;
using HIS.Desktop.Common;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using ACS.UC.User.ADO;
using Inventec.Core;
using MOS.Filter;
using Inventec.Common.Adapter;
using HIS.UC.AccountBook.ADO;
using HIS.Desktop.LocalStorage.ConfigApplication;
using ACS.Filter;
using HIS.Desktop.Plugins.UserAccountBook.Entity;
using Inventec.Common.Controls.EditorLoader;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

namespace HIS.Desktop.Plugins.HisUserAccountBook
{
    public partial class frmUserAccountBook :  HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module moduleData;
        V_HIS_ACCOUNT_BOOK accountBook;

        UCAccountBookProcessor AccountBookProcessor;
        UCUserProcessor UserProcessor;
        UserControl ucGridControlUser;
        UserControl ucGridControlAccountBook;
        int rowCount = 0;
        int dataTotal = 0;
        int rowCount1 = 0;
        int dataTotal1 = 0;
        internal List<HIS.UC.AccountBook.AccountBookADO> lstAccountBookADOs { get; set; }
        internal List<ACS.UC.User.UserADO> lstUserADOs { get; set; }
        List<HIS_ACCOUNT_BOOK> listAccountBook;
        List<ACS_USER> listUser;
        string LoginnameCheckByUser = "";
        long isChoseUser;
        long isChoseAccountBook;
        long AccountBookIdCheckByAccountBook;
        bool isCheckAll;
        List<HIS_USER_ACCOUNT_BOOK> UserAccountBooks { get; set; }
        List<HIS_USER_ACCOUNT_BOOK> UserAccountBookViews { get; set; }
        long? currentAccountBookId;
        ACS.UC.User.UserADO currentCopyUserAdo;
        HIS.UC.AccountBook.AccountBookADO CurrentAccountBookCopyAdo;
        public frmUserAccountBook(Inventec.Desktop.Common.Modules.Module moduleData, V_HIS_ACCOUNT_BOOK accountBook)
            : base(moduleData)
        {
            InitializeComponent();
            this.moduleData = moduleData;
            if (this.moduleData != null)
            {
                this.Text = moduleData.text;
            }
            this.accountBook = accountBook;
            if (accountBook != null)
            {
                currentAccountBookId = accountBook.ID;

                cboAccountBook.Enabled = false;
            }
        }
        public frmUserAccountBook(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            InitializeComponent();
            this.moduleData = moduleData;
            if (this.moduleData != null)
            {
                this.Text = moduleData.text;
            }
        }
        public frmUserAccountBook()
        {
            InitializeComponent();
        }

        private void frmUserAccountBook_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                WaitingManager.Show();
                BackendDataWorker.Reset<HIS_ACCOUNT_BOOK>();
                LoadDataToCombo();
                LoadComboStatus();
                InitUcgridViewUser();
                InitUcgridViewAccountBook();
                if (this.currentAccountBookId == null)
                {
                    FillDataToGrid__User();
                    FillDataToGrid2__AccountBook();
                }
                else
                {
                    FillDataToGrid1__AccountBook_Default();
                    FillDataToGrid__User();
                    var accountBook = BackendDataWorker.Get<HIS_ACCOUNT_BOOK>().FirstOrDefault(o => o.ID == currentAccountBookId);
                    if (accountBook != null)
                    {
                        btn_Radio_Enable_Click(accountBook);
                        cboAccountBook.EditValue = accountBook.ID;
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


        private void UCAccountBookService_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                WaitingManager.Show();
                LoadDataToCombo();
                LoadComboStatus();
                InitUcgridViewUser();
                InitUcgridViewAccountBook();
                if (this.currentAccountBookId == null)
                {
                    FillDataToGrid__User();
                    FillDataToGrid2__AccountBook();
                }
                else
                {
                    FillDataToGrid1__AccountBook_Default();
                    FillDataToGrid__User();
                    var accountBook = BackendDataWorker.Get<HIS_ACCOUNT_BOOK>().FirstOrDefault(o => o.ID == currentAccountBookId);
                    if (accountBook != null)
                    {
                        btn_Radio_Enable_Click(accountBook);
                        cboAccountBook.EditValue = accountBook.ID;
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

        private void gridViewUser_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (isChoseUser == 1)
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
                            var lstCheckAll = lstUserADOs;
                            List<ACS.UC.User.UserADO> lstChecks = new List<ACS.UC.User.UserADO>();

                            if (lstCheckAll != null && lstCheckAll.Count > 0)
                            {
                                var ServiceCheckedNum = lstUserADOs.Where(o => o.checkMedi == true).Count();
                                var ServiceNum = lstUserADOs.Count();
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

                                UserProcessor.Reload(ucGridControlUser, lstChecks);


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
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitUcgridViewUser()
        {
            try
            {
                UserProcessor = new UCUserProcessor();
                UserInitADO ado = new UserInitADO();
                ado.ListUserColumn = new List<ACS.UC.User.UserColumn>();
                ado.gridViewUser_MouseDownMedi = gridViewUser_MouseDown;
                ado.btn_Radio_Enable_Click_Medi = btn_Radio_Enable_Click1;
                ado.gridView_MouseRightClick = UserGridView_MouseRightClick;

                UserColumn colRadio2 = new UserColumn("   ", "radioMedi", 30, true);
                colRadio2.VisibleIndex = 0;
                colRadio2.Visible = false;
                colRadio2.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListUserColumn.Add(colRadio2);

                UserColumn colCheck2 = new UserColumn("   ", "checkMedi", 30, true);
                colCheck2.VisibleIndex = 1;
                colCheck2.image = imageCollectionService.Images[0];
                colCheck2.Visible = false;
                colCheck2.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListUserColumn.Add(colCheck2);

                UserColumn colMaDichvu = new UserColumn("Tài khoản", "LOGINNAME", 100, false);
                colMaDichvu.VisibleIndex = 2;
                ado.ListUserColumn.Add(colMaDichvu);

                UserColumn colTenDichvu = new UserColumn("Tên", "USERNAME", 300, false);
                colTenDichvu.VisibleIndex = 3;
                ado.ListUserColumn.Add(colTenDichvu);

                //UserColumn colMaLoaidichvu = new UserColumn("Đơn vị tính", "SERVICE_UNIT_NAME", 80, false);
                //colMaLoaidichvu.VisibleIndex = 4;
                //ado.ListUserColumn.Add(colMaLoaidichvu);

                this.ucGridControlUser = (UserControl)UserProcessor.Run(ado);
                if (ucGridControlUser != null)
                {
                    this.panelControl1.Controls.Add(this.ucGridControlUser);
                    this.ucGridControlUser.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewAccountBook_MouseAccountBook(object sender, MouseEventArgs e)
        {
            try
            {
                if (isChoseAccountBook == 2)
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
                            var lstCheckAll = lstAccountBookADOs;
                            List<HIS.UC.AccountBook.AccountBookADO> lstChecks = new List<HIS.UC.AccountBook.AccountBookADO>();

                            if (lstCheckAll != null && lstCheckAll.Count > 0)
                            {
                                var AccountBookCheckedNum = lstAccountBookADOs.Where(o => o.checkMedi == true).Count();
                                var AccountBooktmNum = lstAccountBookADOs.Count();
                                if ((AccountBookCheckedNum > 0 && AccountBookCheckedNum < AccountBooktmNum) || AccountBookCheckedNum == 0)
                                {
                                    isCheckAll = true;
                                    hi.Column.Image = imageCollectionAccountBook.Images[1];
                                }

                                if (AccountBookCheckedNum == AccountBooktmNum)
                                {
                                    isCheckAll = false;
                                    hi.Column.Image = imageCollectionAccountBook.Images[0];
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

                                AccountBookProcessor.Reload(ucGridControlAccountBook, lstChecks);


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

        private void btn_Radio_Enable_Click1(ACS_USER data)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                MOS.Filter.HisUserAccountBookFilter filter = new HisUserAccountBookFilter();
                filter.LOGINNAME__EXACT = data.LOGINNAME;
                //if (this.cboAccountBookType.EditValue != null&&(long)this.cboAccountBookType.EditValue!=0)
                //{ 
                //filter.
                //}
                LoginnameCheckByUser = data.LOGINNAME;

                UserAccountBooks = new BackendAdapter(param).Get<List<HIS_USER_ACCOUNT_BOOK>>(
                                    "api/HisUserAccountBook/Get",
                                HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                filter,
                                param);
                List<HIS.UC.AccountBook.AccountBookADO> dataNew = new List<HIS.UC.AccountBook.AccountBookADO>();
                dataNew = (from r in listAccountBook select new AccountBookADO(r)).ToList();
                if (UserAccountBooks != null && UserAccountBooks.Count > 0)
                {
                    foreach (var itemAccountBook in UserAccountBooks)
                    {
                        var check = dataNew.FirstOrDefault(o => o.ID == itemAccountBook.ACCOUNT_BOOK_ID);
                        if (check != null)
                        {
                            check.checkMedi = true;
                        }
                    }
                }
                dataNew = dataNew.OrderByDescending(p => p.checkMedi).ToList();
                if (ucGridControlAccountBook != null)
                {
                    AccountBookProcessor.Reload(ucGridControlAccountBook, dataNew);
                }
                else
                {
                    FillDataToGrid2__AccountBook();
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitUcgridViewAccountBook()
        {
            try
            {
                AccountBookProcessor = new UCAccountBookProcessor();
                AccountBookInitADO ado = new AccountBookInitADO();
                ado.ListAccountBookColumn = new List<UC.AccountBook.AccountBookColumn>();
                ado.gridViewAccountBook_MouseDownMedi = gridViewAccountBook_MouseAccountBook;
                ado.btn_Radio_Enable_Click_Medi = btn_Radio_Enable_Click;
                ado.gridView_MouseRightClick = AccountBookGridView_MouseRightClick;

                AccountBookColumn colradioMedi = new AccountBookColumn("   ", "radioMedi", 30, true);
                colradioMedi.VisibleIndex = 0;
                colradioMedi.Visible = false;
                colradioMedi.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListAccountBookColumn.Add(colradioMedi);

                AccountBookColumn colcheckMedi = new AccountBookColumn("   ", "checkMedi", 30, true);
                colcheckMedi.VisibleIndex = 1;
                colcheckMedi.image = imageCollectionAccountBook.Images[0];
                colcheckMedi.Visible = false;
                colcheckMedi.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListAccountBookColumn.Add(colcheckMedi);

                AccountBookColumn colMaPhong = new AccountBookColumn("Mã Sổ", "ACCOUNT_BOOK_CODE", 60, false);
                colMaPhong.VisibleIndex = 2;
                ado.ListAccountBookColumn.Add(colMaPhong);

                AccountBookColumn colTenPhong = new AccountBookColumn("Tên Sổ", "ACCOUNT_BOOK_NAME", 100, false);
                colTenPhong.VisibleIndex = 3;
                ado.ListAccountBookColumn.Add(colTenPhong);

                AccountBookColumn colLoaiPhong = new AccountBookColumn("Mẫu", "TEMPLATE_CODE", 100, false);
                colLoaiPhong.VisibleIndex = 4;
                ado.ListAccountBookColumn.Add(colLoaiPhong);

                AccountBookColumn colTenso = new AccountBookColumn("Ký hiệu", "SYMBOL_CODE", 100, false);
                colTenso.VisibleIndex = 5;
                ado.ListAccountBookColumn.Add(colTenso);

                //AccountBookColumn colKhoa = new AccountBookColumn("Khoa", "DEPARTMENT_NAME", 100, false);
                //colKhoa.VisibleIndex = 5;
                //ado.ListAccountBookColumn.Add(colKhoa);


                this.ucGridControlAccountBook = (UserControl)AccountBookProcessor.Run(ado);
                if (ucGridControlAccountBook != null)
                {
                    this.panelControl2.Controls.Add(this.ucGridControlAccountBook);
                    this.ucGridControlAccountBook.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btn_Radio_Enable_Click(HIS_ACCOUNT_BOOK data)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                MOS.Filter.HisUserAccountBookFilter filter = new HisUserAccountBookFilter();
                filter.ACCOUNT_BOOK_ID = data.ID;
                AccountBookIdCheckByAccountBook = data.ID;
                UserAccountBookViews = new BackendAdapter(param).Get<List<HIS_USER_ACCOUNT_BOOK>>(
                                         "api/HisUserAccountBook/Get",

                                HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                filter,
                                param);
                List<ACS.UC.User.UserADO> dataNew = new List<ACS.UC.User.UserADO>();
                dataNew = (from r in listUser select new ACS.UC.User.UserADO(r)).ToList();
                if (UserAccountBookViews != null && UserAccountBookViews.Count > 0)
                {

                    foreach (var itemService in UserAccountBookViews)
                    {
                        var check = dataNew.FirstOrDefault(o => o.LOGINNAME == itemService.LOGINNAME);
                        if (check != null)
                        {
                            check.checkMedi = true;
                        }
                    }

                    dataNew = dataNew.OrderByDescending(p => p.checkMedi).ToList();

                    if (ucGridControlUser != null)
                    {
                        UserProcessor.Reload(ucGridControlUser, dataNew);
                    }
                }
                else
                {
                    FillDataToGrid__User();
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGrid2__AccountBook()
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

                FillDataToGridAccountBook(new CommonParam(0, numPageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount1;
                param.Count = dataTotal1;
                ucPaging2.Init(FillDataToGridAccountBook, param, numPageSize);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridAccountBook(object data)
        {
            try
            {
                WaitingManager.Show();
                //lấy các danh sách quyền của tài khoản hiện tại
                CommonParam paramGet = new CommonParam();
                List<HIS_USER_ACCOUNT_BOOK> listAccountBookOfLoginname = null;
                string loginname = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                if (loginname != null)
                {
                    MOS.Filter.HisUserAccountBookFilter filter = new HisUserAccountBookFilter();
                    filter.IS_ACTIVE = 1;
                    filter.LOGINNAME__EXACT = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();

                    listAccountBookOfLoginname = new BackendAdapter(paramGet).Get<List<HIS_USER_ACCOUNT_BOOK>>(
                                      "api/HisUserAccountBook/Get",
                                    HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                    filter,
                                    paramGet);
                }
                if (listAccountBookOfLoginname == null)
                {
                    listAccountBookOfLoginname = new List<HIS_USER_ACCOUNT_BOOK>();
                }
                WaitingManager.Hide();
                WaitingManager.Show();
                listAccountBook = new List<HIS_ACCOUNT_BOOK>();
                int start1 = ((CommonParam)data).Start ?? 0;
                int limit1 = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start1, limit1);
                MOS.Filter.HisAccountBookFilter AccountBookFillter = new HisAccountBookFilter();
                AccountBookFillter.IS_ACTIVE = 1;
                AccountBookFillter.ORDER_FIELD = "MODIFY_TIME";
                AccountBookFillter.ORDER_DIRECTION = "DESC";
                AccountBookFillter.KEY_WORD = txtKeyword2.Text;
                if ((long)cboChoose.EditValue == 2)
                {
                    isChoseAccountBook = (long)cboChoose.EditValue;
                }
                AccountBookFillter.IDs = listAccountBookOfLoginname.Select(o => o.ACCOUNT_BOOK_ID).Distinct().ToList();
                if (cboAccountBook.EditValue != null)
                    AccountBookFillter.ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboAccountBook.EditValue ?? "0").ToString());
                var sar = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<MOS.EFMODEL.DataModels.HIS_ACCOUNT_BOOK>>(
                   HIS.Desktop.ApiConsumer.HisRequestUriStore.HIS_ACCOUNT_BOOK_GET,
                    HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                      AccountBookFillter,
                    param);

                List<long> listRadio = (lstAccountBookADOs ?? new List<AccountBookADO>()).Where(o => o.radioMedi == true).Select(p => p.ID).ToList();
                lstAccountBookADOs = new List<AccountBookADO>();
                if (sar != null && sar.Data.Count > 0)
                {
                  
                    listAccountBook = sar.Data;

                    foreach (var item in listAccountBook)
                    {
                        AccountBookADO roomaccountADO = new AccountBookADO(item);
                        if (isChoseAccountBook == 2)
                        {
                            roomaccountADO.isKeyChooseMedi = true;
                        }
                        lstAccountBookADOs.Add(roomaccountADO);
                    }
                }

                if (UserAccountBooks != null && UserAccountBooks.Count > 0)
                {
                    foreach (var itemUsername in UserAccountBooks)
                    {
                        var check = lstAccountBookADOs.FirstOrDefault(o => o.ID == itemUsername.ACCOUNT_BOOK_ID);
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
                        var radio = lstAccountBookADOs.FirstOrDefault(o => o.ID == rd);
                        if (radio != null)
                        {
                            radio.radioMedi = radio.isKeyChooseMedi;
                        }
                    }
                }
                lstAccountBookADOs = lstAccountBookADOs.OrderByDescending(p => p.checkMedi).ThenByDescending(p => p.radioMedi).Distinct().ToList();

                if (ucGridControlAccountBook != null)
                {
                    AccountBookProcessor.Reload(ucGridControlAccountBook, lstAccountBookADOs);
                }
                rowCount1 = (data == null ? 0 : lstAccountBookADOs.Count);
                dataTotal1 = (sar.Param == null ? 0 : sar.Param.Count ?? 0);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGrid__User()
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

                FillDataToGridUser(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridUser, param, numPageSize);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGrid1__AccountBook_Default()
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

                FillDataToGridAccountBook_Default(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging2.Init(FillDataToGridAccountBook_Default, param, numPageSize);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridUser(object data)
        {
            try
            {
                WaitingManager.Show();
                listUser = new List<ACS_USER>();
                int start = ((CommonParam)data).Start ?? 0;
                int limit = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start, limit);
                ACS.Filter.AcsUserFilter ServiceFillter = new AcsUserFilter();
                ServiceFillter.IS_ACTIVE = 1;
                ServiceFillter.ORDER_FIELD = "MODIFY_TIME";
                ServiceFillter.ORDER_DIRECTION = "DESC";
                ServiceFillter.KEY_WORD = txtKeyword1.Text;

                if ((long)cboChoose.EditValue == 1)
                {
                    isChoseUser = (long)cboChoose.EditValue;
                }

                var rs = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<ACS.EFMODEL.DataModels.ACS_USER>>(
                                                     "api/AcsUser/Get",
                     HIS.Desktop.ApiConsumer.ApiConsumers.AcsConsumer,
                     ServiceFillter,
                     param);
                List<long> listRadio = (lstUserADOs ?? new List<UserADO>()).Where(o => o.radioMedi == true).Select(p => p.ID).ToList();
                lstUserADOs = new List<UserADO>();

                if (rs != null && rs.Data.Count > 0)
                {

                    listUser = rs.Data;
                    foreach (var item in listUser)
                    {
                        UserADO AccountBookServiceADO = new UserADO(item);
                        if (isChoseUser == 1)
                        {
                            AccountBookServiceADO.isKeyChooseMedi = true;
                        }
                        lstUserADOs.Add(AccountBookServiceADO);
                    }
                }

                if (UserAccountBookViews != null && UserAccountBookViews.Count > 0)
                {
                    foreach (var itemUsername in UserAccountBookViews)
                    {
                        var check = lstUserADOs.FirstOrDefault(o => o.LOGINNAME == itemUsername.LOGINNAME);
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
                        var check = lstUserADOs.FirstOrDefault(o => o.ID == rd);
                        if (check != null)
                        {
                            check.radioMedi = check.isKeyChooseMedi;
                        }
                    }
                }

                lstUserADOs = lstUserADOs.OrderByDescending(p => p.checkMedi).ThenByDescending(p => p.radioMedi).Distinct().ToList();
                if (ucGridControlUser != null)
                {
                    UserProcessor.Reload(ucGridControlUser, lstUserADOs);
                }
                rowCount = (data == null ? 0 : lstUserADOs.Count);
                dataTotal = (rs.Param == null ? 0 : rs.Param.Count ?? 0);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridAccountBook_Default(object data)
        {
            try
            {
                WaitingManager.Show();
                listAccountBook = new List<HIS_ACCOUNT_BOOK>();
                int start = ((CommonParam)data).Start ?? 0;
                int limit = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start, limit);
                MOS.Filter.HisAccountBookFilter hisAccountBookFilter = new HisAccountBookFilter();
                hisAccountBookFilter.IS_ACTIVE = 1;
                hisAccountBookFilter.ID = this.currentAccountBookId;

                //if (cboServiceType.EditValue != null)

                //    ServiceFillter.SERVICE_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboServiceType.EditValue ?? "0").ToString());

                if ((long)cboChoose.EditValue == 2)
                {
                    isChoseAccountBook = (long)cboChoose.EditValue;
                }

                var rs = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<MOS.EFMODEL.DataModels.HIS_ACCOUNT_BOOK>>(
                                                     "api/HisAccountBook/Get",
                     HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                     hisAccountBookFilter,
                     param);

                this.lstAccountBookADOs = new List<AccountBookADO>();

                if (rs != null && rs.Data.Count > 0)
                {
                    this.listAccountBook = new List<HIS_ACCOUNT_BOOK>();
                    this.listAccountBook = rs.Data;
                    foreach (var item in this.listAccountBook)
                    {
                        AccountBookADO AccountBookServiceADO = new AccountBookADO(item);
                        if (isChoseAccountBook == 2)
                        {
                            AccountBookServiceADO.isKeyChooseMedi = true;
                            AccountBookServiceADO.radioMedi = true;
                        }
                        this.lstAccountBookADOs.Add(AccountBookServiceADO);
                    }
                }

                if (UserAccountBookViews != null && UserAccountBookViews.Count > 0)
                {
                    foreach (var itemUsername in UserAccountBookViews)
                    {
                        var check = this.lstAccountBookADOs.FirstOrDefault(o => o.ID == itemUsername.ACCOUNT_BOOK_ID);
                        if (check != null)
                        {
                            check.checkMedi = true;
                        }
                    }
                }

                this.lstAccountBookADOs = this.lstAccountBookADOs.OrderByDescending(p => p.checkMedi).ToList();
                if (ucGridControlAccountBook != null)
                {
                    AccountBookProcessor.Reload(ucGridControlAccountBook, this.lstAccountBookADOs);
                }
                rowCount = (data == null ? 0 : lstAccountBookADOs.Count);
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
                
                var AccountBook= BackendDataWorker.Get<HIS_ACCOUNT_BOOK>().Where(o => o.IS_ACTIVE == 1).ToList();
                LoadDataToComboServiceType(cboAccountBook, AccountBook);
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
                status.Add(new Status(1, "Tài khoản"));
                status.Add(new Status(2, "Sổ biên lai"));

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

        private void LoadDataToComboServiceType(DevExpress.XtraEditors.GridLookUpEdit cboServiceType, List<HIS_ACCOUNT_BOOK> ServiceType)
        {
            try
            {
                cboServiceType.Properties.DataSource = ServiceType;
                cboServiceType.Properties.DisplayMember = "ACCOUNT_BOOK_NAME";
                cboServiceType.Properties.ValueMember = "ID";

                cboServiceType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboServiceType.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboServiceType.Properties.ImmediatePopup = true;
                cboServiceType.ForceInitialize();
                cboServiceType.Properties.View.Columns.Clear();

                GridColumn aColumnCode = cboServiceType.Properties.View.Columns.AddField("ACCOUNT_BOOK_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 100;

                GridColumn aColumnName = cboServiceType.Properties.View.Columns.AddField("ACCOUNT_BOOK_NAME");
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
                FillDataToGrid__User();
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
                FillDataToGrid2__AccountBook();
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
                UserAccountBookViews = null;
                UserAccountBooks = null;
                isChoseAccountBook = 0;
                isChoseUser = 0;
                FillDataToGrid__User();
                FillDataToGrid2__AccountBook();
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

                if (ucGridControlAccountBook != null && ucGridControlUser != null)
                {
                    object AccountBook = AccountBookProcessor.GetDataGridView(ucGridControlAccountBook);
                    object Service = UserProcessor.GetDataGridView(ucGridControlUser);
                    if (isChoseUser == 1)
                    {
                        if (this.lstUserADOs == null || !this.lstUserADOs.Exists(o => o.radioMedi))
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn Tài khoản", "Thông báo");
                            return;
                        }
                        if (AccountBook is List<HIS.UC.AccountBook.AccountBookADO>)
                        {
                            lstAccountBookADOs = (List<HIS.UC.AccountBook.AccountBookADO>)AccountBook;

                            if (lstAccountBookADOs != null && lstAccountBookADOs.Count > 0)
                            {
                                //List<long> listServiceAccountBooks = ServiceAccountBooks.Select(p => p.SERVICE_ID).ToList();

                                var dataCheckeds = lstAccountBookADOs.Where(p => p.checkMedi == true).ToList();

                                //       //List xoa

                                var dataDeletes = lstAccountBookADOs.Where(o => UserAccountBooks.Select(p => p.ACCOUNT_BOOK_ID)
                                    .Contains(o.ID) && o.checkMedi == false).ToList();


                                //list them
                                var dataCreates = dataCheckeds.Where(o => !UserAccountBooks.Select(p => p.ACCOUNT_BOOK_ID)
                                    .Contains(o.ID)).ToList();
                                if (dataDeletes.Count == 0 && dataCreates.Count == 0)
                                {
                                    if (UserAccountBooks.Where(o => lstAccountBookADOs.Exists(p => p.ID == o.ACCOUNT_BOOK_ID)).ToList().Count == 0)
                                    {
                                        MessageBox.Show("Chưa chọn Sổ biên lai","Thông báo");
                                        return;
                                    }
                                    else
                                    {
                                        MessageBox.Show("Tài khoản đã thiết lập cho các Sổ biên lai được chọn", "Thông báo");
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
                                    List<long> deleteSds = UserAccountBooks.Where(o => dataDeletes.Select(p => p.ID)
                                        .Contains(o.ACCOUNT_BOOK_ID)).Select(o => o.ID).ToList();
                                    bool deleteResult = new BackendAdapter(param).Post<bool>(
                                              "api/HisUserAccountBook/DeleteList",
                                              HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                              deleteSds,
                                              param);
                                    if (deleteResult)
                                        success = true;
                                    UserAccountBooks = UserAccountBooks.Where(o => !deleteSds.Contains(o.ID)).ToList();
                                }

                                if (dataCreates != null && dataCreates.Count > 0)
                                {
                                    List<HIS_USER_ACCOUNT_BOOK> ServiceAccountBookCreates = new List<HIS_USER_ACCOUNT_BOOK>();
                                    foreach (var item in dataCreates)
                                    {
                                        HIS_USER_ACCOUNT_BOOK ServiceAccountBook = new HIS_USER_ACCOUNT_BOOK();
                                        ServiceAccountBook.LOGINNAME = LoginnameCheckByUser;
                                        ServiceAccountBook.ACCOUNT_BOOK_ID = item.ID;
                                        ServiceAccountBookCreates.Add(ServiceAccountBook);
                                    }

                                    var createResult = new BackendAdapter(param).Post<List<HIS_USER_ACCOUNT_BOOK>>(
                                               "api/HisUserAccountBook/CreateList",
                                               HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                               ServiceAccountBookCreates,
                                               param);
                                    if (createResult != null && createResult.Count > 0)
                                        success = true;
                                    AutoMapper.Mapper.CreateMap<HIS_USER_ACCOUNT_BOOK, HIS_USER_ACCOUNT_BOOK>();
                                    var vCreateResults = AutoMapper.Mapper.Map<List<HIS_USER_ACCOUNT_BOOK>, List<HIS_USER_ACCOUNT_BOOK>>(createResult);
                                    UserAccountBooks.AddRange(vCreateResults);
                                }

                                WaitingManager.Hide();
                                lstAccountBookADOs = lstAccountBookADOs.OrderByDescending(p => p.checkMedi).ToList();
                                if (ucGridControlAccountBook != null)
                                {
                                    AccountBookProcessor.Reload(ucGridControlAccountBook, lstAccountBookADOs);
                                }
                            }

                        }
                    }

                    if (isChoseAccountBook == 2)
                    {
                        if (this.lstAccountBookADOs == null || !this.lstAccountBookADOs.Exists(o => o.radioMedi))
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn Sổ biên lai", "Thông báo");
                            return;
                        }
                        if (Service is List<ACS.UC.User.UserADO>)
                        {
                            lstUserADOs = (List<ACS.UC.User.UserADO>)Service;

                            if (lstUserADOs != null && lstUserADOs.Count > 0)
                            {
                                //List<long> listAccountBookServices = ServiceAccountBook.Select(p => p.ACCOUNT_BOOK_ID).ToList();

                                var dataChecked = lstUserADOs.Where(p => p.checkMedi == true).ToList();
                                //List xoa

                                var dataDelete = lstUserADOs.Where(o => UserAccountBookViews.Select(p => p.LOGINNAME)
                                    .Contains(o.LOGINNAME) && o.checkMedi == false).ToList();

                                //list them
                                var dataCreate = dataChecked.Where(o => !UserAccountBookViews.Select(p => p.LOGINNAME)
                                    .Contains(o.LOGINNAME)).ToList();


                                if (dataDelete.Count == 0 && dataCreate.Count == 0)
                                {
                                    if (UserAccountBookViews.Where(o => lstUserADOs.Exists(p => p.LOGINNAME == o.LOGINNAME)).ToList().Count == 0)
                                    {
                                        MessageBox.Show("Chưa chọn Tài khoản", "Thông báo");
                                        return;
                                    }
                                    else
                                    {
                                        MessageBox.Show("Sổ biên lai đã thiết lập cho các Tài khoản được chọn", "Thông báo");
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

                                    List<long> deleteId = UserAccountBookViews.Where(o => dataDelete.Select(p => p.LOGINNAME)
                                        .Contains(o.LOGINNAME)).Select(o => o.ID).ToList();
                                    bool deleteResult = new BackendAdapter(param).Post<bool>(
                                              "api/HisUserAccountBook/DeleteList",
                                              HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                              deleteId,
                                              param);
                                    if (deleteResult)
                                        success = true;
                                    UserAccountBookViews = UserAccountBookViews.Where(o => !deleteId.Contains(o.ID)).ToList();
                                }

                                if (dataCreate != null && dataCreate.Count > 0)
                                {
                                    List<HIS_USER_ACCOUNT_BOOK> ServiceAccountBookCreate = new List<HIS_USER_ACCOUNT_BOOK>();
                                    foreach (var item in dataCreate)
                                    {
                                        HIS_USER_ACCOUNT_BOOK ServiceAccountBookID = new HIS_USER_ACCOUNT_BOOK();
                                        ServiceAccountBookID.ACCOUNT_BOOK_ID = AccountBookIdCheckByAccountBook;
                                        ServiceAccountBookID.LOGINNAME = item.LOGINNAME;
                                        ServiceAccountBookCreate.Add(ServiceAccountBookID);
                                    }

                                    var createResult = new BackendAdapter(param).Post<List<HIS_USER_ACCOUNT_BOOK>>(
                                               "/api/HisUserAccountBook/CreateList",
                                               HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                               ServiceAccountBookCreate,
                                               param);
                                    if (createResult != null && createResult.Count > 0)
                                        success = true;
                                    AutoMapper.Mapper.CreateMap<HIS_USER_ACCOUNT_BOOK, HIS_USER_ACCOUNT_BOOK>();
                                    var vCreateResults = AutoMapper.Mapper.Map<List<HIS_USER_ACCOUNT_BOOK>, List<HIS_USER_ACCOUNT_BOOK>>(createResult);
                                    UserAccountBookViews.AddRange(vCreateResults);
                                }
                                WaitingManager.Hide();
                                lstUserADOs = lstUserADOs.OrderByDescending(p => p.checkMedi).ToList();
                                if (ucGridControlAccountBook != null)
                                {
                                    UserProcessor.Reload(ucGridControlUser, lstUserADOs);
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
                    FillDataToGrid__User();

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
                    FillDataToGrid2__AccountBook();
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
                    cboAccountBook.Focus();
                    cboAccountBook.ShowPopup();
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

        private void cboAccountBookType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboAccountBook.EditValue != null)
                    {
                        HIS_ACCOUNT_BOOK data = (cboAccountBook.Properties.DataSource as List<HIS_ACCOUNT_BOOK>).SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboAccountBook.EditValue.ToString()));
                        if (data != null)
                        {
                            cboAccountBook.Properties.Buttons[1].Visible = true;
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
                    cboAccountBook.EditValue = null;
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
                if (cboAccountBook.EditValue != null)
                {
                    HIS_ACCOUNT_BOOK data = (cboAccountBook.Properties.DataSource as List<HIS_ACCOUNT_BOOK>).SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboAccountBook.EditValue.ToString()));
                    if (data != null)
                    {
                        cboAccountBook.Properties.Buttons[1].Visible = true;
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
                if (this.moduleData != null)
                {
                    //CallModule callModule = new CallModule(CallModule.HisImportServiceAccountBook, moduleData.AccountBookId, moduleData.AccountBookId, listArgs);
                }
                else
                {
                    CallModule callModule = new CallModule(CallModule.HisImportServiceAccountBook, 0, 0, listArgs);
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
                if (this.currentAccountBookId == null)
                {
                    FillDataToGrid__User();
                    FillDataToGrid2__AccountBook();
                }
                else
                {
                    FillDataToGrid1__AccountBook_Default();
                    FillDataToGrid__User();
                    var room = BackendDataWorker.Get<HIS_ACCOUNT_BOOK>().FirstOrDefault(o => o.ID == this.currentAccountBookId);
                    btn_Radio_Enable_Click(room);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void UserGridView_MouseRightClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if ((e.Item is BarButtonItem) && sender != null && sender is ACS.UC.User.UserADO)
                {
                    var type = (ACS.UC.User.Popup.PopupMenuProcessor.ItemType)e.Item.Tag;
                    switch (type)
                    {
                        case ACS.UC.User.Popup.PopupMenuProcessor.ItemType.Copy:
                            {
                                if (isChoseUser != 1)
                                {
                                    MessageManager.Show("Vui lòng chọn Tài khoản!");
                                    break;
                                }
                                this.currentCopyUserAdo = (ACS.UC.User.UserADO)sender;
                                break;
                            }
                        case ACS.UC.User.Popup.PopupMenuProcessor.ItemType.Paste:
                            {
                                var currentPaste = (ACS.UC.User.UserADO)sender;
                                bool success = false;
                                CommonParam param = new CommonParam();
                                if (this.currentCopyUserAdo == null && isChoseUser != 1)
                                {
                                    MessageManager.Show("Vui lòng copy!");
                                    break;
                                }
                                if (this.currentCopyUserAdo != null && currentPaste != null && isChoseUser == 1)
                                {
                                    //if (this.currentCopyUserAdo.ID == currentPaste.ID)
                                    //{
                                    //    MessageManager.Show("Trùng dữ liệu copy và paste");
                                    //    break;
                                    //}
                                    //HisMetyAccountBookCopyByUserSDO hisMestMatyCopyByMatySDO = new HisMetyAccountBookCopyByUserSDO();
                                    //hisMestMatyCopyByMatySDO.CopyUserId = this.currentCopyUserAdo.ID;
                                    //hisMestMatyCopyByMatySDO.PasteUserId = currentPaste.ID;
                                    //var result = new BackendAdapter(param).Post<List<HIS_USER_ACCOUNT_BOOK>>("api/HisUserAccountBook/CopyByUser", ApiConsumer.ApiConsumers.MosConsumer, hisMestMatyCopyByMatySDO, param);
                                    //if (result != null)
                                    //{
                                    //    success = true;
                                    //    List<HIS.UC.AccountBook.AccountBookADO> dataNew = new List<HIS.UC.AccountBook.AccountBookADO>();
                                    //    dataNew = (from r in listAccountBook select new AccountBookADO(r)).ToList();
                                    //    if (result != null && result.Count > 0)
                                    //    {
                                    //        foreach (var itemAccountBook in result)
                                    //        {
                                    //            var check = dataNew.FirstOrDefault(o => o.ID == itemAccountBook.ACCOUNT_BOOK_ID);
                                    //            if (check != null)
                                    //            {
                                    //                check.checkMedi = true;
                                    //            }
                                    //        }
                                    //    }
                                    //    dataNew = dataNew.OrderByDescending(p => p.checkMedi).ToList();
                                    //    if (ucGridControlAccountBook != null)
                                    //    {
                                    //        AccountBookProcessor.Reload(ucGridControlAccountBook, dataNew);
                                    //    }
                                    //    else
                                    //    {
                                    //        FillDataToGrid2__AccountBook();
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

        private void AccountBookGridView_MouseRightClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if ((e.Item is BarButtonItem) && sender != null && sender is HIS.UC.AccountBook.AccountBookADO)
                {
                    var type = (HIS.UC.AccountBook.Popup.PopupMenuProcessor.ItemType)e.Item.Tag;
                    switch (type)
                    {
                        case HIS.UC.AccountBook.Popup.PopupMenuProcessor.ItemType.Copy:
                            {
                                if (isChoseAccountBook != 2)
                                {
                                    MessageManager.Show("Vui lòng chọn Sổ biên lai!");
                                    break;
                                }
                                this.CurrentAccountBookCopyAdo = (HIS.UC.AccountBook.AccountBookADO)sender;
                                break;
                            }
                        case HIS.UC.AccountBook.Popup.PopupMenuProcessor.ItemType.Paste:
                            {
                                var currentPaste = (HIS.UC.AccountBook.AccountBookADO)sender;
                                bool success = false;
                                CommonParam param = new CommonParam();
                                if (this.CurrentAccountBookCopyAdo == null && isChoseAccountBook != 2)
                                {
                                    MessageManager.Show("Vui lòng copy!");
                                    break;
                                }
                                if (this.CurrentAccountBookCopyAdo != null && currentPaste != null && isChoseAccountBook == 2)
                                {
                                    if (this.CurrentAccountBookCopyAdo.ID == currentPaste.ID)
                                    {
                                        MessageManager.Show("Trùng dữ liệu copy và paste");
                                        break;
                                    }
                                    //HisMetyAccountBookCopyByAccountBookSDO hisMestMatyCopyByMatySDO = new HisMetyAccountBookCopyByAccountBookSDO();
                                    //hisMestMatyCopyByMatySDO.CopyAccountBookId = this.CurrentAccountBookCopyAdo.ID;
                                    //hisMestMatyCopyByMatySDO.PasteAccountBookId = currentPaste.ID;
                                    //var result = new BackendAdapter(param).Post<List<HIS_USER_ACCOUNT_BOOK>>("api/HisUserAccountBook/CopyByAccountBook", ApiConsumer.ApiConsumers.MosConsumer, hisMestMatyCopyByMatySDO, param);
                                    if (1 != 1)
                                    {
                                        //success = true;
                                        //List<ACS.UC.User.UserADO> dataNew = new List<ACS.UC.User.UserADO>();
                                        //dataNew = (from r in listUser select new ACS.UC.User.UserADO(r)).ToList();
                                        //if (result != null && result.Count > 0)
                                        //{

                                        //    foreach (var itemService in result)
                                        //    {
                                        //        var check = dataNew.FirstOrDefault(o => o.ID == itemService.USER_ID);
                                        //        if (check != null)
                                        //        {
                                        //            check.checkMedi = true;
                                        //        }
                                        //    }

                                        //    dataNew = dataNew.OrderByDescending(p => p.checkMedi).ToList();

                                        //    if (ucGridControlUser != null)
                                        //    {
                                        //        UserProcessor.Reload(ucGridControlUser, dataNew);
                                        //    }
                                        //}
                                        //else
                                        //{
                                        //    FillDataToGrid1__User();
                                        //}
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

        private void bbtnD_ItemClick(object sender, ItemClickEventArgs e)
        {
            btnFind2.Focus();
            btnFind2_Click(null, null);
        }

        private void bbtnF_ItemClick(object sender, ItemClickEventArgs e)
        {
            btnFind1.Focus();
            btnFind1_Click(null,null);
        }

        private void bbtnS_ItemClick(object sender, ItemClickEventArgs e)
        {
            btnSave.Focus();
            btnSave_Click(null, null);
        }
    }
}
