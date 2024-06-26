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
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Plugins.ImpMestTypeUser.Entity;
using DevExpress.XtraGrid.Views.Layout.Modes;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using MOS.Filter;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.Plugins.ImpMestTypeUser.Resources;
using HIS.Desktop.Controls.Session;
using HIS.UC.ImpMestType;
using HIS.UC.Account;
using HIS.UC.Account.ADO;
using HIS.UC.ImpMestType.ADO;
using DevExpress.XtraBars;
using MOS.SDO;
using HIS.Desktop.LocalStorage.BackendData;
using ACS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.ImpMestTypeUser
{
    public partial class UCImpMestTypeUser : HIS.Desktop.Utility.UserControlBase
    {

        internal List<HIS.Desktop.ADO.ImpMestTypeADO> ImpMestTypeAdo { get; set; }
        //internal List<HIS.UC.Account.AccountADO> AccountAdo { get; set; }
        int start = 0;
        int limit = 0;
        int rowCount = 0;
        int dataTotal = 0;
        int start1 = 0;
        int limit1 = 0;
        int rowCount1 = 0;
        int dataTotal1 = 0;
        string checkStock = "";
        long checkMaty = 0;
        ImpMestTypeProcessor impMestTypeProcessor = null;
        AccountProcessor accountProcessor = null;
        UserControl ucImpMestType;
        UserControl ucAccount;
        long isChoseStock = 0;
        long isChoseImpMestType = 0;
        bool isCheckAll;
        bool checkRa = false;
        List<HIS_IMP_MEST_TYPE> listImpMestType = new List<HIS_IMP_MEST_TYPE>();
        //List<ACS.EFMODEL.DataModels.ACS_USER> listAcsUser = new List<ACS.EFMODEL.DataModels.ACS_USER>();
        List<HIS_IMP_MEST_TYPE_USER> listImpMestTypeUser = new List<HIS_IMP_MEST_TYPE_USER>();
        HIS.UC.Account.AccountADO currentCopyAccountAdo;
        HIS.Desktop.ADO.ImpMestTypeADO currentCopyImpmestTypeAdo;

        private List<AccountADO> UserAdoTotal = new List<AccountADO>();
        private List<AccountADO> listUser;

        public UCImpMestTypeUser(Inventec.Desktop.Common.Modules.Module _moduleData)
            : base(_moduleData)
        {
            InitializeComponent();
            InitAccount();
            InitImpMestType();
        }

        private void UCMediStockMatyList_Load(object sender, EventArgs e)
        {
            try
            {
                LoadDataUserAdo();
                LoadComboStatus();
                FillDataToGrid1(this);
                FillDataToGrid2(this);
                SetCaptionByLanguageKey();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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

        private void LoadComboStatus()
        {
            try
            {
                List<Status> status = new List<Status>();
                status.Add(new Status(1, "Tài khoản"));
                status.Add(new Status(2, "Loại nhập"));

                List<Inventec.Common.Controls.EditorLoader.ColumnInfo> columnInfos = new List<Inventec.Common.Controls.EditorLoader.ColumnInfo>();
                columnInfos.Add(new Inventec.Common.Controls.EditorLoader.ColumnInfo("statusName", "", 300, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("statusName", "id", columnInfos, false, 350);
                ControlEditorLoader.Load(cboStatus, status, controlEditorADO);
                cboStatus.EditValue = status[0].id;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitAccount()
        {
            try
            {
                accountProcessor = new AccountProcessor();
                AccountInitADO ado = new AccountInitADO();
                ado.ListAccountColumn = new List<AccountColumn>();
                ado.btn_Radio_Enable_Click1 = btn_Radio_Enable_Click1;
                ado.gridViewAccount_MouseDownAccount = gridViewStock_MouseDownStock;
                ado.GridView_MouseRightClick = AccountGridView_MouseRightClick;
                ado.AccountGrid_CustomUnboundColumnData = gridViewUser_CustomUnboundColumnData;

                AccountColumn colRadio2 = new AccountColumn("   ", "radio2", 30, true);
                colRadio2.VisibleIndex = 0;
                colRadio2.Visible = false;
                colRadio2.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListAccountColumn.Add(colRadio2);

                AccountColumn colCheck2 = new AccountColumn("   ", "check2", 30, true);
                colCheck2.VisibleIndex = 1;
                colCheck2.image = imgStock.Images[0];
                colCheck2.Caption = "Chọn tất cả";
                colCheck2.Visible = false;
                colCheck2.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListAccountColumn.Add(colCheck2);

                AccountColumn colMaPhong = new AccountColumn("Tài khoản", "LOGINNAME", 80, false);
                colMaPhong.VisibleIndex = 2;
                ado.ListAccountColumn.Add(colMaPhong);

                AccountColumn colTenPhong = new AccountColumn("Họ tên", "USERNAME", 150, false);
                colTenPhong.VisibleIndex = 3;
                ado.ListAccountColumn.Add(colTenPhong);

                AccountColumn colDOB = new AccountColumn("Ngày sinh", "DOB_STR", 100, false);
                colDOB.VisibleIndex = 4;
                colDOB.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListAccountColumn.Add(colDOB);

                AccountColumn colDiploma = new AccountColumn("Chứng chỉ hành nghề", "DIPLOMA", 200, false);
                colDiploma.VisibleIndex = 5;
                ado.ListAccountColumn.Add(colDiploma);

                AccountColumn colDepartment = new AccountColumn("Khoa phòng", "DEPARTMENT_NAME", 100, false);
                colDepartment.VisibleIndex = 6;
                colDepartment.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListAccountColumn.Add(colDepartment);

                AccountColumn colPhoneNumber = new AccountColumn("Số điện thoại", "MOBILE", 100, false);
                colPhoneNumber.VisibleIndex = 7;
                ado.ListAccountColumn.Add(colPhoneNumber);

                AccountColumn colEmail = new AccountColumn("Email", "EMAIL", 100, false);
                colEmail.VisibleIndex = 8;
                ado.ListAccountColumn.Add(colEmail);

                this.ucAccount = (UserControl)accountProcessor.Run(ado);
                if (ucAccount != null)
                {
                    this.pnlUser.Controls.Add(this.ucAccount);
                    this.ucAccount.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGrid1(UCImpMestTypeUser _Stock)
        {
            try
            {
                int numPageSize = 0;
                if (ucPagingUser.pagingGrid != null)
                {
                    numPageSize = ucPagingUser.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = (int)ConfigApplications.NumPageSize;
                }
                FillDataToGridAccount(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPagingUser.Init(FillDataToGridAccount, param);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToStock(UCImpMestTypeUser _Stock)
        {
            try
            {
                int numPageSize = 0;
                if (ucPagingUser.pagingGrid != null)
                {
                    numPageSize = ucPagingUser.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = (int)ConfigApplications.NumPageSize;
                }
                FillDataToGridAccount(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPagingUser.Init(FillDataToGridAccount, param);
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
                isChoseStock = (long)cboStatus.EditValue;

                var startUser = ((CommonParam)data).Start ?? 0;
                var limitUser = ((CommonParam)data).Limit ?? 0;
                var query = UserAdoTotal.AsQueryable();
                string keyword = txtKeywordUser.Text.Trim();
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

                if (!chkShowLockUser.Checked)
                {
                    query = query.Where(o => o.IS_ACTIVE == 1);
                }

                query = query.OrderByDescending(o => o.check2).ThenBy(o => o.LOGINNAME);
                dataTotal1 = query.Count();
                this.listUser = query.Skip(startUser).Take(limitUser).ToList();
                rowCount1 = (listUser == null ? 0 : listUser.Count);

                foreach (var item in listUser)
                {
                    item.isKeyChoose1 = (isChoseStock == 1);
                    item.check2 = ((listImpMestTypeUser != null && listImpMestTypeUser.Count > 0) ? listImpMestTypeUser.Exists(e => e.LOGINNAME == item.LOGINNAME) : false);
                }

                accountProcessor.Reload(ucAccount, listUser);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitImpMestType()
        {
            try
            {
                impMestTypeProcessor = new ImpMestTypeProcessor();
                ImpMestTypeInitADO ado = new ImpMestTypeInitADO();
                ado.ListImpMestTypeColumn = new List<ImpMestTypeColumn>();
                ado.gridView_MouseDown = gridViewImpMestType_MouseDownImpMestType;
                ado.btnRadioEnable_Click = btn_Radio_Enable_Click;
                ado.gridView_MouseRightClick = ImpMestTypeGridView_MouseRightClick;

                ImpMestTypeColumn colRadio1 = new ImpMestTypeColumn("   ", "radioImpMestType", 30, true, false);
                colRadio1.VisibleIndex = 0;
                colRadio1.Visible = false;
                colRadio1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListImpMestTypeColumn.Add(colRadio1);

                ImpMestTypeColumn colCheck1 = new ImpMestTypeColumn("   ", "checkImpMestType", 30, true, false);
                colCheck1.VisibleIndex = 1;
                colCheck1.Visible = false;
                colCheck1.image = imgMaty.Images[0];
                colCheck1.Caption = "Chọn tất cả";
                //colCheck1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                colCheck1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListImpMestTypeColumn.Add(colCheck1);

                ImpMestTypeColumn colMaPhong = new ImpMestTypeColumn("Mã loại nhập", "IMP_MEST_TYPE_CODE", 100, false, true);
                colMaPhong.VisibleIndex = 2;
                ado.ListImpMestTypeColumn.Add(colMaPhong);

                ImpMestTypeColumn colTenPhong = new ImpMestTypeColumn("Tên loại nhập", "IMP_MEST_TYPE_NAME", 200, false, true);
                colTenPhong.VisibleIndex = 3;
                ado.ListImpMestTypeColumn.Add(colTenPhong);

                this.ucImpMestType = (UserControl)impMestTypeProcessor.Run(ado);
                if (ucImpMestType != null)
                {
                    this.pnlImpMestType.Controls.Add(this.ucImpMestType);
                    this.ucImpMestType.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void FillDataToGrid2(UCImpMestTypeUser _Material)
        {
            try
            {
                int numPageSize = 0;
                if (ucPagingImpMestType.pagingGrid != null)
                {
                    numPageSize = ucPagingImpMestType.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = (int)ConfigApplications.NumPageSize;
                }
                FillDataToGridImpMestType(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPagingImpMestType.Init(FillDataToGridImpMestType, param);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToMaty(UCImpMestTypeUser _Mety)
        {
            try
            {
                int numPageSize = 0;
                if (ucPagingImpMestType.pagingGrid != null)
                {
                    numPageSize = ucPagingImpMestType.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = (int)ConfigApplications.NumPageSize;
                }
                FillDataToGridImpMestType(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPagingImpMestType.Init(FillDataToGridImpMestType, param);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridImpMestType(object data)
        {
            try
            {
                WaitingManager.Show();
                listImpMestType = new List<HIS_IMP_MEST_TYPE>();
                int start = ((CommonParam)data).Start ?? 0;
                int limit = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start, limit);
                MOS.Filter.HisImpMestTypeFilter metyFilter = new MOS.Filter.HisImpMestTypeFilter();
                metyFilter.ORDER_FIELD = "IMP_MEST_TYPE_NAME";
                metyFilter.ORDER_DIRECTION = "ASC";
                metyFilter.KEY_WORD = txtKeywordImpMestType.Text;

                //if (cboRoomType.EditValue != null)
                //    RoomFillter.ROOM_TYPE_ID = (long)cboRoomType.EditValue;
                //long isChoseStock = 0;
                if (cboStatus.EditValue != null && (long)cboStatus.EditValue == 2)
                {
                    isChoseImpMestType = (long)cboStatus.EditValue;
                }
                if (cboStatus.EditValue != null && (long)cboStatus.EditValue == 1)
                {
                    isChoseImpMestType = (long)cboStatus.EditValue;
                }
                var rs = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<HIS_IMP_MEST_TYPE>>(
                    "api/HisImpMestType/Get",
                    HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                    metyFilter,
                    param);

                ImpMestTypeAdo = new List<HIS.Desktop.ADO.ImpMestTypeADO>();
                if (rs != null && rs.Data.Count > 0)
                {
                    listImpMestType = rs.Data;
                    foreach (var item in listImpMestType)
                    {
                        HIS.Desktop.ADO.ImpMestTypeADO matypeADO = new HIS.Desktop.ADO.ImpMestTypeADO(item);
                        if (isChoseImpMestType == 2)
                        {
                            matypeADO.isKeyChoose = true;
                            //btnCheckAll2.Enabled = false;
                        }
                        ImpMestTypeAdo.Add(matypeADO);
                    }
                }
                if (listImpMestTypeUser != null && listImpMestTypeUser.Count > 0)
                {
                    foreach (var item in listImpMestTypeUser)
                    {
                        var mediStockMety = ImpMestTypeAdo.FirstOrDefault(o => o.ID == item.IMP_MEST_TYPE_ID);
                        if (mediStockMety != null)
                        {
                            mediStockMety.checkImpMestType = true;
                            //mediStockMety.ALERT_MAX_IN_STOCK_STR = item.ALERT_MAX_IN_STOCK;
                            //mediStockMety.ALERT_MIN_IN_STOCK_STR = item.ALERT_MIN_IN_STOCK;
                            //mediStockMety.IS_GOODS_RESTRICT = item.IS_GOODS_RESTRICT == 1 ? true : false;
                            //mediStockMety.IS_PREVENT_MAX = item.IS_PREVENT_MAX == 1 ? true : false;
                        }
                    }
                }
                ImpMestTypeAdo = ImpMestTypeAdo.OrderByDescending(p => p.checkImpMestType).ToList();
                if (ucImpMestType != null)
                {
                    impMestTypeProcessor.Reload(ucImpMestType, ImpMestTypeAdo);
                }
                rowCount = (data == null ? 0 : ImpMestTypeAdo.Count);
                dataTotal = (rs.Param == null ? 0 : rs.Param.Count ?? 0);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btn_Radio_Enable_Click1(ACS.EFMODEL.DataModels.ACS_USER data)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                HisImpMestTypeUserFilter filter = new HisImpMestTypeUserFilter();
                filter.LOGINNAME = data.LOGINNAME;
                checkStock = data.LOGINNAME;
                listImpMestTypeUser = new List<HIS_IMP_MEST_TYPE_USER>();
                listImpMestTypeUser = new BackendAdapter(param).Get<List<HIS_IMP_MEST_TYPE_USER>>(
                                "api/HisImpMestTypeUser/Get",
                                HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                filter,
                                param);
                List<HIS.Desktop.ADO.ImpMestTypeADO> dataNew = new List<HIS.Desktop.ADO.ImpMestTypeADO>();
                dataNew = (from r in listImpMestType select new HIS.Desktop.ADO.ImpMestTypeADO(r)).ToList();
                if (listImpMestTypeUser != null && listImpMestTypeUser.Count > 0)
                {
                    foreach (var item in listImpMestTypeUser)
                    {
                        var mediStockMety = dataNew.FirstOrDefault(o => o.ID == item.IMP_MEST_TYPE_ID);
                        if (mediStockMety != null)
                        {
                            mediStockMety.checkImpMestType = true;
                            //mediStockMety.ALERT_MAX_IN_STOCK_STR = item.ALERT_MAX_IN_STOCK;
                            //mediStockMety.ALERT_MIN_IN_STOCK_STR = item.ALERT_MIN_IN_STOCK;
                            //mediStockMety.IS_GOODS_RESTRICT = item.IS_GOODS_RESTRICT == 1 ? true : false;
                            //mediStockMety.IS_PREVENT_MAX = item.IS_PREVENT_MAX == 1 ? true : false;
                        }
                    }

                    dataNew = dataNew.OrderByDescending(p => p.checkImpMestType).ToList();
                    if (ucImpMestType != null)
                    {
                        impMestTypeProcessor.Reload(ucImpMestType, dataNew);
                    }
                }
                else
                {
                    FillDataToGrid2(this);
                }
                checkRa = true;
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btn_Radio_Enable_Click(HIS_IMP_MEST_TYPE data)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                HisImpMestTypeUserFilter filter = new HisImpMestTypeUserFilter();
                filter.IMP_MEST_TYPE_ID = data.ID;
                checkMaty = data.ID;
                //filter.
                listImpMestTypeUser = new List<HIS_IMP_MEST_TYPE_USER>();
                listImpMestTypeUser = new BackendAdapter(param).Get<List<HIS_IMP_MEST_TYPE_USER>>(
                                "api/HisImpMestTypeUser/Get",
                                HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                filter,
                                param);

                if (listImpMestTypeUser != null && listImpMestTypeUser.Count > 0)
                {
                    foreach (var itemStock in listImpMestTypeUser)
                    {
                        var check = listUser.FirstOrDefault(o => o.LOGINNAME == itemStock.LOGINNAME);
                        if (check != null)
                        {
                            check.check2 = true;
                        }
                    }

                    listUser = listUser.OrderByDescending(p => p.check2).ToList();
                    if (ucAccount != null)
                    {
                        accountProcessor.Reload(ucAccount, listUser);
                    }
                }
                else
                {
                    FillDataToGrid1(this);
                }
                checkRa = true;
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewImpMestType_MouseDownImpMestType(object sender, MouseEventArgs e)
        {
            try
            {
                if (isChoseImpMestType == 2)
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
                        if (hi.Column.FieldName == "checkImpMestType")
                        {
                            var lstCheckAll = ImpMestTypeAdo;
                            List<HIS.Desktop.ADO.ImpMestTypeADO> lstChecks = new List<HIS.Desktop.ADO.ImpMestTypeADO>();

                            if (lstCheckAll != null && lstCheckAll.Count > 0)
                            {
                                var roomCheckedNum = ImpMestTypeAdo.Where(o => o.checkImpMestType == true).Count();
                                var roomNum = ImpMestTypeAdo.Count();
                                if ((roomCheckedNum > 0 && roomCheckedNum < roomNum) || roomCheckedNum == 0)
                                {
                                    isCheckAll = true;
                                    hi.Column.Image = imgMaty.Images[0];
                                }

                                if (roomCheckedNum == roomNum)
                                {
                                    isCheckAll = false;
                                    hi.Column.Image = imgMaty.Images[1];
                                }
                                if (isCheckAll)
                                {
                                    foreach (var item in lstCheckAll)
                                    {
                                        if (item.ID != null)
                                        {
                                            item.checkImpMestType = true;
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
                                            item.checkImpMestType = false;
                                            lstChecks.Add(item);
                                        }
                                        else
                                        {
                                            lstChecks.Add(item);
                                        }
                                    }
                                    isCheckAll = true;
                                }

                                impMestTypeProcessor.Reload(ucImpMestType, lstChecks);
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

        private void gridViewStock_MouseDownStock(object sender, MouseEventArgs e)
        {
            try
            {
                if (isChoseStock == 1)
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

                                    hi.Column.Image = imgStock.Images[1];
                                    isCheckAll = false;
                                }
                                else
                                {
                                    foreach (var item in listUser)
                                    {
                                        item.check2 = false;
                                    }

                                    hi.Column.Image = imgStock.Images[0];
                                    isCheckAll = true;
                                }

                                accountProcessor.Reload(ucAccount, listUser);
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.ImpMestTypeUser.Resources.Lang", typeof(HIS.Desktop.Plugins.ImpMestTypeUser.UCImpMestTypeUser).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCMediStockMatyList.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboStatus.Properties.NullText = Inventec.Common.Resource.Get.Value("UCMediStockMatyList.cboStatus.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("UCMediStockMatyList.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("UCMediStockMatyList.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UCMediStockMatyList.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("UCMediStockMatyList.layoutControlItem12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboStatus_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                checkRa = false;
                isChoseStock = 0;
                isChoseImpMestType = 0;
                listImpMestTypeUser = new List<HIS_IMP_MEST_TYPE_USER>();
                FillDataToGrid1(this);
                FillDataToGrid2(this);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSearch1_Click(object sender, EventArgs e)
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

        private void btnSearch2_Click(object sender, EventArgs e)
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (ucImpMestType != null && ucAccount != null)
                {
                    bool success = false;
                    CommonParam param = new CommonParam();
                    List<HIS.Desktop.ADO.ImpMestTypeADO> medicineMatys = impMestTypeProcessor.GetDataGridView(ucImpMestType) as List<HIS.Desktop.ADO.ImpMestTypeADO>;
                    List<HIS.UC.Account.AccountADO> medicineStocks = accountProcessor.GetDataGridView(ucAccount) as List<HIS.UC.Account.AccountADO>;
                    if (isChoseImpMestType == 1 && medicineMatys != null && medicineMatys.Count > 0)
                    {
                        if (checkRa == true)
                        {
                            if (medicineMatys != null && medicineMatys.Count > 0)
                            {
                                //Check List
                                var dataCheckeds = medicineMatys.Where(p => (p.checkImpMestType == true)).ToList();

                                //List xóa
                                var dataDeletes = medicineMatys.Where(o => listImpMestTypeUser.Select(p => p.IMP_MEST_TYPE_ID)
                               .Contains(o.ID) && o.checkImpMestType == false).ToList();

                                //list them
                                var dataCreates = dataCheckeds.Where(o => !listImpMestTypeUser.Select(p => p.IMP_MEST_TYPE_ID)
                                    .Contains(o.ID)).ToList();

                                // List update
                                //var dataUpdate = dataCheckeds.Where(o => listImpMestTypeUser.Select(p => p.IMP_MEST_TYPE_ID)
                                //    .Contains(o.ID)).ToList();

                                //if (dataUpdate != null && dataUpdate.Count > 0)
                                //{
                                //    var stockMatyUpdates = new List<HIS_IMP_MEST_TYPE_USER>();
                                //    foreach (var item in dataUpdate)
                                //    {
                                //        var mediStockMaty = listImpMestTypeUser.FirstOrDefault(o => o.IMP_MEST_TYPE_ID == item.ID && o.LOGINNAME == checkStock);
                                //        if (mediStockMaty != null)
                                //        {
                                //            //mediStockMaty.ALERT_MIN_IN_STOCK = item.ALERT_MIN_IN_STOCK_STR;
                                //            //mediStockMaty.ALERT_MAX_IN_STOCK = item.ALERT_MAX_IN_STOCK_STR;
                                //            //mediStockMaty.IS_GOODS_RESTRICT = short.Parse(item.IS_GOODS_RESTRICT == true ? "1" : "0");
                                //            //mediStockMaty.IS_PREVENT_MAX = short.Parse(item.IS_PREVENT_MAX == true ? "1" : "0");
                                //            stockMatyUpdates.Add(mediStockMaty);
                                //        }
                                //    }
                                //    if (stockMatyUpdates != null && stockMatyUpdates.Count > 0)
                                //    {
                                //        var updateResult = new BackendAdapter(param).Post<List<HIS_IMP_MEST_TYPE_USER>>(
                                //                   "/api/HisImpMestTypeUser/UpdateList",
                                //                   HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                //                   stockMatyUpdates,
                                //                   param);
                                //        if (updateResult != null && updateResult.Count > 0)
                                //        {
                                //            listImpMestTypeUser.AddRange(updateResult);
                                //            success = true;
                                //        }
                                //    }
                                //}

                                if (dataDeletes != null && dataDeletes.Count > 0)// && dataDeletes.Count < 15)
                                {
                                    List<long> deleteIds = listImpMestTypeUser.Where(o => dataDeletes.Select(p => p.ID)
                                        .Contains(o.IMP_MEST_TYPE_ID)).Select(o => o.ID).ToList();
                                    bool deleteResult = new BackendAdapter(param).Post<bool>(
                                              "/api/HisImpMestTypeUser/DeleteList",
                                              HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                              deleteIds,
                                              param);
                                    if (deleteResult)
                                    {
                                        listImpMestTypeUser = listImpMestTypeUser.Where(o => !deleteIds.Contains(o.ID)).ToList();
                                        success = true;
                                    }
                                }

                                if (dataCreates != null && dataCreates.Count > 0)
                                {
                                    List<HIS_IMP_MEST_TYPE_USER> stockMetyCreates = new List<HIS_IMP_MEST_TYPE_USER>();
                                    foreach (var item in dataCreates)
                                    {
                                        HIS_IMP_MEST_TYPE_USER stockMety = new HIS_IMP_MEST_TYPE_USER();
                                        stockMety.IMP_MEST_TYPE_ID = item.ID;
                                        stockMety.LOGINNAME = checkStock;
                                        //stockMety.ALERT_MIN_IN_STOCK = item.ALERT_MIN_IN_STOCK_STR;
                                        //stockMety.ALERT_MAX_IN_STOCK = item.ALERT_MAX_IN_STOCK_STR;
                                        //stockMety.IS_GOODS_RESTRICT = short.Parse(item.IS_GOODS_RESTRICT == true ? "1" : "0");
                                        //stockMety.IS_PREVENT_MAX = short.Parse(item.IS_PREVENT_MAX == true ? "1" : "0");
                                        stockMetyCreates.Add(stockMety);
                                    }

                                    var createResult = new BackendAdapter(param).Post<List<HIS_IMP_MEST_TYPE_USER>>(
                                               "/api/HisImpMestTypeUser/CreateList",
                                               HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                               stockMetyCreates,
                                               param);
                                    if (createResult != null && createResult.Count > 0)
                                    {
                                        listImpMestTypeUser.AddRange(createResult);
                                        success = true;
                                    }
                                }
                                WaitingManager.Hide();
                                #region Show message
                                MessageManager.Show(this.ParentForm, param, success);
                                #endregion

                                #region Process has exception
                                SessionManager.ProcessTokenLost(param);
                                #endregion
                                medicineMatys = medicineMatys.OrderByDescending(p => p.checkImpMestType).ToList();
                                impMestTypeProcessor.Reload(ucImpMestType, medicineMatys);
                            }
                        }
                        else
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn Kho");
                        }
                    }

                    if (isChoseStock == 2 && medicineStocks != null && medicineStocks.Count > 0)
                    {
                        if (checkRa == true)
                        {
                            HIS.Desktop.ADO.ImpMestTypeADO medicineType = medicineMatys.FirstOrDefault(o => o.ID == checkMaty);
                            //Check List
                            var dataCheckeds = medicineStocks.Where(p => (p.check2 == true)).ToList();

                            //List xóa
                            var dataDeletes = medicineStocks.Where(o => listImpMestTypeUser.Select(p => p.LOGINNAME)
                           .Contains(o.LOGINNAME) && o.check2 == false).ToList();

                            //list them
                            var dataCreates = dataCheckeds.Where(o => !listImpMestTypeUser.Select(p => p.LOGINNAME)
                                .Contains(o.LOGINNAME)).ToList();

                            //list update
                            var dataUpdates = dataCheckeds.Where(o => listImpMestTypeUser.Select(p => p.LOGINNAME)
                                .Contains(o.LOGINNAME)).ToList();

                            if (dataDeletes != null && dataDeletes.Count > 0)// && dataDeletes.Count < 5)
                            {
                                List<long> deleteIds = listImpMestTypeUser.Where(o => dataDeletes.Select(p => p.LOGINNAME)
                                    .Contains(o.LOGINNAME)).Select(o => o.ID).ToList();
                                bool deleteResult = new BackendAdapter(param).Post<bool>(
                                          "/api/HisImpMestTypeUser/DeleteList",
                                          HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                          deleteIds,
                                          param);
                                if (deleteResult)
                                {
                                    listImpMestTypeUser = listImpMestTypeUser.Where(o => !deleteIds.Contains(o.ID)).ToList();
                                    success = true;
                                }

                            }

                            //if (dataUpdates != null && dataUpdates.Count > 0 && medicineType != null)
                            //{
                            //    // List<HIS_IMP_MEST_TYPE_USER> stockMetyUpdates = new List<HIS_IMP_MEST_TYPE_USER>();
                            //    var stockMetyUpdates = new List<HIS_IMP_MEST_TYPE_USER>();
                            //    foreach (var item in dataUpdates)
                            //    {
                            //        var metyStock = listImpMestTypeUser.FirstOrDefault(o => o.LOGINNAME == item.LOGINNAME && o.IMP_MEST_TYPE_ID == checkMaty);
                            //        if (metyStock != null)
                            //        {
                            //            //metyStock.ALERT_MIN_IN_STOCK = medicineType.ALERT_MIN_IN_STOCK_STR;
                            //            //metyStock.ALERT_MAX_IN_STOCK = medicineType.ALERT_MAX_IN_STOCK_STR;
                            //            //metyStock.IS_GOODS_RESTRICT = short.Parse(medicineType.IS_GOODS_RESTRICT == true ? "1" : "0");
                            //            //metyStock.IS_PREVENT_MAX = short.Parse(medicineType.IS_PREVENT_MAX == true ? "1" : "0");
                            //            stockMetyUpdates.Add(metyStock);
                            //        }
                            //    }
                            //    if (stockMetyUpdates != null && stockMetyUpdates.Count > 0)
                            //    {
                            //        var updateResult = new BackendAdapter(param).Post<List<HIS_IMP_MEST_TYPE_USER>>(
                            //                   "/api/HisImpMestTypeUser/UpdateList",
                            //                   HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                            //                   stockMetyUpdates,
                            //                   param);
                            //        if (updateResult != null && updateResult.Count > 0)
                            //        {
                            //            //listMediStockMaty.AddRange(updateResult);
                            //            success = true;
                            //        }
                            //    }
                            //}

                            if (dataCreates != null && dataCreates.Count > 0 && medicineType != null)
                            {
                                List<HIS_IMP_MEST_TYPE_USER> matyStockCreates = new List<HIS_IMP_MEST_TYPE_USER>();
                                foreach (var item in dataCreates)
                                {
                                    HIS_IMP_MEST_TYPE_USER matyStock = new HIS_IMP_MEST_TYPE_USER();
                                    matyStock.LOGINNAME = item.LOGINNAME;
                                    matyStock.IMP_MEST_TYPE_ID = checkMaty;
                                    //matyStock.ALERT_MIN_IN_STOCK = medicineType.ALERT_MIN_IN_STOCK_STR;
                                    //matyStock.ALERT_MAX_IN_STOCK = medicineType.ALERT_MAX_IN_STOCK_STR;
                                    //matyStock.IS_GOODS_RESTRICT = short.Parse(medicineType.IS_GOODS_RESTRICT == true ? "1" : "0");
                                    //matyStock.IS_PREVENT_MAX = short.Parse(medicineType.IS_PREVENT_MAX == true ? "1" : "0");
                                    matyStockCreates.Add(matyStock);
                                }

                                var createResult = new BackendAdapter(param).Post<List<HIS_IMP_MEST_TYPE_USER>>(
                                           "/api/HisImpMestTypeUser/CreateList",
                                           HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                           matyStockCreates,
                                           param);
                                if (createResult != null && createResult.Count > 0)
                                {
                                    listImpMestTypeUser.AddRange(createResult);
                                    success = true;
                                }

                            }
                            WaitingManager.Hide();
                            #region Show message
                            MessageManager.Show(this.ParentForm, param, success);
                            #endregion

                            #region Process has exception
                            SessionManager.ProcessTokenLost(param);
                            #endregion
                            medicineStocks = medicineStocks.OrderByDescending(p => p.check2).ToList();
                            accountProcessor.Reload(ucAccount, medicineStocks);
                        }
                        else
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn Loại vật tư");
                        }
                    }

                    //var mediMetyCheckeds = medicineMatys.Where(p => (p.check1 == true)).ToList();
                    //var mediStockCheckeds = medicineStocks.Where(p => (p.checkMest == true)).ToList();

                    //if (mediMetyCheckeds.Count == 0 && mediStockCheckeds.Count == 0)
                    //{
                    //    param.Messages.Add(String.Format(ResourceMessage.ChuaChonKhoLoaiThuoc));
                    //}
                    //if (mediMetyCheckeds.Count == 0 && mediStockCheckeds.Count != 0)
                    //{
                    //    param.Messages.Add(String.Format(ResourceMessage.ChuaChonLoaiVatTu));
                    //} if (mediMetyCheckeds.Count != 0 && mediStockCheckeds.Count == 0)
                    //{
                    //    param.Messages.Add(String.Format(ResourceMessage.ChuaChonKho));
                    //}
                    //MessageManager.Show(this.ParentForm, param, success);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void FindImpMestType()
        {
            try
            {
                btnSearchImpMestType.Focus();
                btnSearch1_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void FindAccount()
        {
            try
            {
                btnSearchUser.Focus();
                btnSearch2_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void Save()
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
                    btnSearch1_Click(null, null);
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
                    btnSearch2_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void Grid_RowCellClick(ACS.EFMODEL.DataModels.ACS_USER data)
        {
            try
            {
                if (data != null && listImpMestTypeUser != null && listImpMestTypeUser.Count > 0)
                {
                    HIS_IMP_MEST_TYPE_USER mediStockMaty = listImpMestTypeUser.FirstOrDefault(o => o.LOGINNAME == data.LOGINNAME);
                    if (mediStockMaty != null)
                    {
                        //matyProcessor.ReloadRow(ucMaty, mediStockMaty);// TODO
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void AccountGridView_MouseRightClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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
                                if (isChoseStock != 1)
                                {
                                    MessageManager.Show("Vui lòng chọn tài khoản!");
                                    break;
                                }
                                this.currentCopyAccountAdo = (HIS.UC.Account.AccountADO)sender;
                                break;
                            }
                        case HIS.UC.Account.Popup.PopupMenuProcessor.ItemType.Paste:
                            {
                                var currentPaste = (HIS.UC.Account.AccountADO)sender;
                                bool success = false;
                                CommonParam param = new CommonParam();
                                if (this.currentCopyAccountAdo == null && isChoseStock != 1)
                                {
                                    MessageManager.Show("Vui lòng copy!");
                                    break;
                                }
                                if (this.currentCopyAccountAdo != null && currentPaste != null && isChoseStock == 1)
                                {
                                    if (this.currentCopyAccountAdo.ID == currentPaste.ID)
                                    {
                                        MessageManager.Show("Trùng dữ liệu copy và paste");
                                        break;
                                    }
                                    HisImpMestTypeUserCopyByLoginnameSDO hisMestMetyCopyByMediStockSDO = new HisImpMestTypeUserCopyByLoginnameSDO();
                                    hisMestMetyCopyByMediStockSDO.CopyLoginname = currentCopyAccountAdo.LOGINNAME;
                                    hisMestMetyCopyByMediStockSDO.PasteLoginname = currentPaste.LOGINNAME;
                                    var result = new BackendAdapter(param).Post<List<HIS_IMP_MEST_TYPE_USER>>("api/HisImpMestTypeUser/CopyByLoginname", ApiConsumer.ApiConsumers.MosConsumer, hisMestMetyCopyByMediStockSDO, param);
                                    if (result != null)
                                    {
                                        success = true;
                                        listImpMestTypeUser = result;
                                        List<HIS.Desktop.ADO.ImpMestTypeADO> dataNew = new List<HIS.Desktop.ADO.ImpMestTypeADO>();
                                        dataNew = (from r in listImpMestType select new HIS.Desktop.ADO.ImpMestTypeADO(r)).ToList();
                                        if (listImpMestTypeUser != null && listImpMestTypeUser.Count > 0)
                                        {
                                            foreach (var item in listImpMestTypeUser)
                                            {
                                                var mediStockMety = dataNew.FirstOrDefault(o => o.ID == item.IMP_MEST_TYPE_ID);
                                                if (mediStockMety != null)
                                                {
                                                    mediStockMety.checkImpMestType = true;
                                                    //mediStockMety.ALERT_MAX_IN_STOCK_STR = item.ALERT_MAX_IN_STOCK;
                                                    //mediStockMety.ALERT_MIN_IN_STOCK_STR = item.ALERT_MIN_IN_STOCK;
                                                    //mediStockMety.IS_GOODS_RESTRICT = item.IS_GOODS_RESTRICT == 1 ? true : false;
                                                    //mediStockMety.IS_PREVENT_MAX = item.IS_PREVENT_MAX == 1 ? true : false;
                                                }
                                            }

                                            dataNew = dataNew.OrderByDescending(p => p.checkImpMestType).ToList();
                                            if (ucImpMestType != null)
                                            {
                                                impMestTypeProcessor.Reload(ucImpMestType, dataNew);
                                            }
                                        }
                                        else
                                        {
                                            FillDataToGrid2(this);
                                        }
                                        checkRa = true;
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

        private void ImpMestTypeGridView_MouseRightClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if ((e.Item is BarButtonItem) && sender != null && sender is HIS.Desktop.ADO.ImpMestTypeADO)
                {
                    var type = (HIS.UC.ImpMestType.Popup.PopupMenuProcessor.ItemType)e.Item.Tag;
                    switch (type)
                    {
                        case HIS.UC.ImpMestType.Popup.PopupMenuProcessor.ItemType.Copy:
                            {
                                if (isChoseImpMestType != 2)
                                {
                                    MessageManager.Show("Vui lòng chọn loại nhập!");
                                    break;
                                }
                                this.currentCopyImpmestTypeAdo = (HIS.Desktop.ADO.ImpMestTypeADO)sender;
                                break;
                            }
                        case HIS.UC.ImpMestType.Popup.PopupMenuProcessor.ItemType.Paste:
                            {
                                var currentPaste = (HIS.Desktop.ADO.ImpMestTypeADO)sender;
                                bool success = false;
                                CommonParam param = new CommonParam();
                                if (this.currentCopyImpmestTypeAdo == null && isChoseImpMestType != 2)
                                {
                                    MessageManager.Show("Vui lòng copy!");
                                    break;
                                }
                                if (this.currentCopyImpmestTypeAdo != null && currentPaste != null && isChoseImpMestType == 2)
                                {
                                    if (this.currentCopyImpmestTypeAdo.ID == currentPaste.ID)
                                    {
                                        MessageManager.Show("Trùng dữ liệu copy và paste");
                                        break;
                                    }
                                    HisImpMestTypeUserCopyByTypeSDO hisMestMetyCopyByMediStockSDO = new HisImpMestTypeUserCopyByTypeSDO();
                                    hisMestMetyCopyByMediStockSDO.CopyImpMestTypeId = currentCopyImpmestTypeAdo.ID;
                                    hisMestMetyCopyByMediStockSDO.PasteImpMestTypeId = currentPaste.ID;
                                    var result = new BackendAdapter(param).Post<List<HIS_IMP_MEST_TYPE_USER>>("api/HisImpMestTypeUser/CopyByType", ApiConsumer.ApiConsumers.MosConsumer, hisMestMetyCopyByMediStockSDO, param);
                                    if (result != null)
                                    {
                                        success = true;
                                        listImpMestTypeUser = result;
                                        if (listImpMestTypeUser != null && listImpMestTypeUser.Count > 0)
                                        {
                                            foreach (var itemStock in listImpMestTypeUser)
                                            {
                                                var check = listUser.FirstOrDefault(o => o.LOGINNAME == itemStock.LOGINNAME);
                                                if (check != null)
                                                {
                                                    check.check2 = true;
                                                }
                                            }

                                            listUser = listUser.OrderByDescending(p => p.check2).ToList();
                                            if (ucAccount != null)
                                            {
                                                accountProcessor.Reload(ucAccount, listUser);
                                            }
                                        }
                                        else
                                        {
                                            FillDataToGrid1(this);
                                        }
                                        checkRa = true;
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

        private void gridViewUser_CustomUnboundColumnData(ACS_USER data, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                var HisEmployee = BackendDataWorker.Get<HIS_EMPLOYEE>().FirstOrDefault(o => o.LOGINNAME == data.LOGINNAME);
                if (HisEmployee == null) return;
                if (e.Column.FieldName != "DOB_STR" && e.Column.FieldName != "DEPARTMENT_ID_STR") return;
                if (e.Column.FieldName == "DOB_STR")
                {
                    if (HisEmployee != null)

                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(HisEmployee.DOB ?? 0);
                }

                else if (e.Column.FieldName == "DEPARTMENT_ID_STR")
                {

                    if (HisEmployee.DEPARTMENT_ID != null)
                    {
                        var lstRs = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.ID == HisEmployee.DEPARTMENT_ID.Value).ToList();
                        if (lstRs != null && lstRs.Count > 0)
                        {
                            var rs = lstRs.First();
                            if (rs != null)
                            {

                                e.Value = rs.DEPARTMENT_NAME ?? "";
                            }
                        }
                    }
                    //e.Value = HisEmployee.DEPARTMENT_ID != null ? BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == HisEmployee.DEPARTMENT_ID.Value).DEPARTMENT_NAME : null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


    }
}
