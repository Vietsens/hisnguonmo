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
using HIS.Desktop.Plugins.SdaConfigAppUser.entity;
using HIS.UC.Account;
using HIS.UC.Account.ADO;
using Inventec.Core;
using HIS.UC.ConfigApp;
using HIS.UC.ConfigApp.ADO;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Adapter;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraEditors;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using SDA.EFMODEL.DataModels;
using ACS.EFMODEL.DataModels;
using ACS.Filter;
using SDA.Filter;
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using System.Collections;
using DevExpress.XtraGrid;

namespace HIS.Desktop.Plugins.SdaConfigAppUser
{
    public partial class UCSdaConfigAppUser : HIS.Desktop.Utility.UserControlBase
    {
        AccountProcessor AccountProcessor;
        UCConfigAppProcessor ConfigAppProcessor;
        UserControl ucGridControlAccount;
        UserControl ucGridControlConfigApp;
        int start = 0;
        int limit = 0;
        int rowCount = 0;
        int dataTotal = 0;
        int start1 = 0;
        int limit1 = 0;
        int rowCount1 = 0;
        int dataTotal1 = 0;
        bool checkRa = false;
        List<AccountADO> listCheckAccountAdos = new List<AccountADO>();
        List<ConfigAppADO> listCheckConfigAppAdos = new List<ConfigAppADO>();
        internal List<HIS.UC.Account.AccountADO> lstAccountADOs { get; set; }
        internal List<HIS.UC.ConfigApp.ConfigAppADO> lstConfigAppADOs { get; set; }
        List<ACS_USER> listAccount;
        List<SDA_CONFIG_APP> listConfigApp;
        String accountIdCheckByAccount = null;
        long ConfigAppIdCheck = 0;
        long isChooseAccount;
        long isChooseConfigApp;
        bool isCheckAll;
        bool statecheckColumn;
        List<SDA_CONFIG_APP_USER> matyAccounts { get; set; }
        List<SDA_CONFIG_APP_USER> matyConfigApps { get; set; }
        Action delegateRefresh;
        Inventec.Desktop.Common.Modules.Module moduleData;

        public UCSdaConfigAppUser(Inventec.Desktop.Common.Modules.Module moduleData, Inventec.Common.WebApiClient.ApiConsumer sdaConsumer, Inventec.Common.WebApiClient.ApiConsumer acsConsumer, Action delegateRefresh, long numPageSize, string applicationCode)
            : base(moduleData)
        {
            InitializeComponent();
            this.moduleData = moduleData;
            this.delegateRefresh = delegateRefresh;
            ConfigApplications.NumPageSize = numPageSize;
            GlobalVariables.APPLICATION_CODE = applicationCode;
            ApiConsumers.SdaConsumer = sdaConsumer;
            ApiConsumers.AcsConsumer = acsConsumer;
        }

        #region Phím tắt
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
        #endregion

        private void UCSdaConfigAppUser_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                SetCaptionByLanguageKey();
                LoadComboStatus();
                InitUCgridAccount();
                InitUCgridConfigApp();
                FillDataToGridAccount(this);
                FillDataToGridConfigApp(this);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadComboStatus()
        {
            try
            {
                List<Status> status = new List<Status>();
                status.Add(new Status(1, "Tài khoản"));
                status.Add(new Status(2, "Cấu hình tài khoản"));

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

        private void FillDataToGridConfigApp(UCSdaConfigAppUser uCSdaConfigAppUser)
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

                FillDataToGridConfigAppPage(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount1;
                param.Count = dataTotal1;
                ucPaging2.Init(FillDataToGridConfigAppPage, param, numPageSize, (GridControl)ConfigAppProcessor.GetGridControl(this.ucGridControlConfigApp));
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridConfigAppPage(object data)
        {
            try
            {
                WaitingManager.Show();
                listConfigApp = new List<SDA_CONFIG_APP>();
                int start1 = ((CommonParam)data).Start ?? 0;
                int limit1 = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start1, limit1);
                SdaConfigAppFilter ConfigAppFilter = new SdaConfigAppFilter();
                ConfigAppFilter.KEY_WORD = txtKeyword2.Text;
                ConfigAppFilter.APP_CODE_ACCEPT = GlobalVariables.APPLICATION_CODE;

                if ((long)cboChoose.EditValue == 2)
                {
                    isChooseConfigApp = (long)cboChoose.EditValue;
                }

                var mest = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<SDA_CONFIG_APP>>(
                    "api/SdaConfigApp/Get",
                    ApiConsumers.SdaConsumer,
                    ConfigAppFilter,
                    param);

                lstConfigAppADOs = new List<ConfigAppADO>();
                if (mest != null && mest.Data.Count > 0)
                {
                    listConfigApp = mest.Data;
                    foreach (var item in listConfigApp)
                    {
                        ConfigAppADO ConfigAppADO = new ConfigAppADO(item);
                        ConfigAppADO.VALUE_STR = item.DEFAULT_VALUE;
                        if (isChooseConfigApp == 2)
                        {
                            ConfigAppADO.isKeyChooseConfigApp = true;
                        }
                        lstConfigAppADOs.Add(ConfigAppADO);
                    }
                }

                if (matyAccounts != null && matyAccounts.Count > 0)
                {
                    foreach (var itemUsername in matyAccounts)
                    {
                        var check = lstConfigAppADOs.FirstOrDefault(o => o.ID == itemUsername.CONFIG_APP_ID);
                        if (check != null)
                        {
                            check.checkConfigApp = true;
                            check.VALUE_STR = itemUsername.VALUE;
                        }
                    }
                }
                lstConfigAppADOs = lstConfigAppADOs.OrderByDescending(p => p.checkConfigApp).ToList();

                if (ConfigAppIdCheck != 0 && isChooseConfigApp == 2)
                {
                    var radioConfigApp = lstConfigAppADOs.Where(o => o.ID == ConfigAppIdCheck).FirstOrDefault();
                    if (radioConfigApp != null)
                    {
                        radioConfigApp.radioConfigApp = true;
                    }
                }
                lstConfigAppADOs = lstConfigAppADOs.OrderByDescending(p => p.radioConfigApp).ToList();

                if (listCheckConfigAppAdos != null && listCheckConfigAppAdos.Count > 0)
                {
                    foreach (var item in listCheckConfigAppAdos)
                    {
                        var check = lstConfigAppADOs.FirstOrDefault(o => o.ID == item.ID);
                        if (check != null)
                        {
                            lstConfigAppADOs.FirstOrDefault(o => o.ID == item.ID).checkConfigApp = item.checkConfigApp;
                            lstConfigAppADOs.FirstOrDefault(o => o.ID == item.ID).VALUE_STR = item.VALUE_STR;
                        }
                    }
                }

                if (ucGridControlConfigApp != null)
                {
                    ConfigAppProcessor.Reload(ucGridControlConfigApp, lstConfigAppADOs);
                }
                rowCount1 = (data == null ? 0 : lstConfigAppADOs.Count);
                dataTotal1 = (mest.Param == null ? 0 : mest.Param.Count ?? 0);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridAccount(UCSdaConfigAppUser uCSdaConfigAppUser)
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

                FillDataToGridAccountPage(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridAccountPage, param, numPageSize, (GridControl)AccountProcessor.GetGridControl(this.ucGridControlAccount));
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridAccountPage(object data)
        {
            try
            {
                WaitingManager.Show();
                listAccount = new List<ACS_USER>();
                int start = ((CommonParam)data).Start ?? 0;
                int limit = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start, limit);
                AcsUserFilter AccountFilter = new AcsUserFilter();
                AccountFilter.ORDER_FIELD = "LOGINNAME";
                AccountFilter.ORDER_DIRECTION = "ASC";
                AccountFilter.KEY_WORD = txtKeyword1.Text;

                if ((long)cboChoose.EditValue == 1)
                {
                    isChooseAccount = (long)cboChoose.EditValue;
                }

                var rs = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<ACS_USER>>(
                    "api/AcsUser/Get",
                    ApiConsumers.AcsConsumer,
                    AccountFilter,
                    param);

                lstAccountADOs = new List<AccountADO>();
                if (rs != null && rs.Data.Count > 0)
                {
                    listAccount = rs.Data;
                    foreach (var item in listAccount)
                    {
                        AccountADO AccountAccountADO = new AccountADO(item);
                        if (isChooseAccount == 1)
                        {
                            AccountAccountADO.isKeyChoose1 = true;
                        }
                        lstAccountADOs.Add(AccountAccountADO);
                    }
                }

                if (matyConfigApps != null && matyConfigApps.Count > 0)
                {
                    foreach (var itemUsername in matyConfigApps)
                    {
                        var check = lstAccountADOs.FirstOrDefault(o => o.LOGINNAME == itemUsername.LOGINNAME);
                        if (check != null)
                        {
                            check.check2 = true;
                        }
                    }
                }
                lstAccountADOs = lstAccountADOs.OrderByDescending(p => p.check2).ToList();

                if (accountIdCheckByAccount != null && isChooseAccount == 1)
                {
                    var checkSevice = lstAccountADOs.Where(o => o.LOGINNAME == accountIdCheckByAccount).FirstOrDefault();
                    if (checkSevice != null)
                    {
                        checkSevice.radio2 = true;
                    }
                    lstAccountADOs = lstAccountADOs.OrderByDescending(p => p.radio2).ToList();
                }

                if (listCheckAccountAdos != null && listCheckAccountAdos.Count > 0)
                {
                    foreach (var item in listCheckAccountAdos)
                    {
                        var checks = lstAccountADOs.FirstOrDefault(o => o.ID == item.ID);
                        if (checks != null)
                        {
                            lstAccountADOs.FirstOrDefault(o => o.ID == item.ID).check2 = item.check2;
                        }
                    }
                }

                if (ucGridControlAccount != null)
                {
                    AccountProcessor.Reload(ucGridControlAccount, lstAccountADOs);
                }
                rowCount = (data == null ? 0 : lstAccountADOs.Count);
                dataTotal = (rs.Param == null ? 0 : rs.Param.Count ?? 0);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitUCgridConfigApp()
        {
            try
            {
                var culture = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                var langManager = Resources.ResourceLanguageManager.LanguageResource;

                ConfigAppProcessor = new UCConfigAppProcessor();
                ConfigAppInitADO ado = new ConfigAppInitADO();
                ado.ListConfigAppColumn = new List<ConfigAppColumn>();
                ado.gridViewConfigApp_MouseDownMest = gridViewConfigApp_MouseDownMate;
                ado.btn_Radio_Enable_Click1 = btn_Radio_Enable_Click_Mate;
                ado.Check__Enable_CheckedChanged = ConfigAppCheckedChanged;

                ConfigAppColumn colRadio2 = new ConfigAppColumn("   ", "radioConfigApp", 30, true);
                colRadio2.VisibleIndex = 0;
                colRadio2.Visible = false;
                colRadio2.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListConfigAppColumn.Add(colRadio2);

                ConfigAppColumn colCheck2 = new ConfigAppColumn("   ", "checkConfigApp", 30, true);
                colCheck2.VisibleIndex = 1;
                colCheck2.Visible = false;
                colCheck2.image = imageCollectionConfigApp.Images[0];
                colCheck2.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListConfigAppColumn.Add(colCheck2);

                ConfigAppColumn colMaUngDung = new ConfigAppColumn("Mã ứng dụng", "APP_CODE", 60, false);
                colMaUngDung.VisibleIndex = 2;
                ado.ListConfigAppColumn.Add(colMaUngDung);

                ConfigAppColumn colKEY = new ConfigAppColumn("KEY", "KEY", 500, false);
                colKEY.VisibleIndex = 3;
                ado.ListConfigAppColumn.Add(colKEY);

                ConfigAppColumn colGiaTri = new ConfigAppColumn("Giá trị", "DEFAULT_VALUE", 100, true);
                colGiaTri.VisibleIndex = 4;
                colGiaTri.Visible = false;
                colGiaTri.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListConfigAppColumn.Add(colGiaTri);

                this.ucGridControlConfigApp = (UserControl)ConfigAppProcessor.Run(ado);

                if (ucGridControlConfigApp != null)
                {
                    this.panelControl2.Controls.Add(this.ucGridControlConfigApp);
                    this.ucGridControlConfigApp.Dock = DockStyle.Fill;
                }

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ConfigAppCheckedChanged(ConfigAppADO data)
        {
            try
            {
                bool success = false;
                var sources = (List<ConfigAppADO>)ConfigAppProcessor.GetDataGridView(ucGridControlConfigApp);
                var itemSources = sources.FirstOrDefault(o => o.ID == data.ID);
                if (listCheckConfigAppAdos != null && listCheckConfigAppAdos.Count > 0)
                {
                    foreach (var item in listCheckConfigAppAdos)
                    {
                        if (data.ID == item.ID)
                        {
                            listCheckConfigAppAdos.FirstOrDefault(o => o.ID == itemSources.ID).checkConfigApp = itemSources.checkConfigApp;
                            success = true;
                            break;
                        }
                    }
                    if (!success)
                    {
                        listCheckConfigAppAdos.Add(itemSources);
                    }
                }
                else
                {
                    listCheckConfigAppAdos.Add(itemSources);
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewConfigApp_MouseDownMate(object sender, MouseEventArgs e)
        {
            try
            {
                if (isChooseConfigApp == 2)
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
                        if (hi.Column.FieldName == "checkConfigApp")
                        {
                            var lstCheckAll = lstConfigAppADOs;
                            List<HIS.UC.ConfigApp.ConfigAppADO> lstChecks = new List<HIS.UC.ConfigApp.ConfigAppADO>();

                            if (lstCheckAll != null && lstCheckAll.Count > 0)
                            {
                                var ConfigAppCheckedNum = lstConfigAppADOs.Where(o => o.checkConfigApp == true).Count();
                                var ConfigAppNum = lstConfigAppADOs.Count();
                                if ((ConfigAppCheckedNum > 0 && ConfigAppCheckedNum < ConfigAppNum) || ConfigAppCheckedNum == 0)
                                {
                                    isCheckAll = true;
                                    hi.Column.Image = imageCollectionConfigApp.Images[1];
                                }

                                if (ConfigAppCheckedNum == ConfigAppNum)
                                {
                                    isCheckAll = false;
                                    hi.Column.Image = imageCollectionConfigApp.Images[0];
                                }
                                if (isCheckAll)
                                {
                                    foreach (var item in lstCheckAll)
                                    {
                                        if (item.ID != null)
                                        {
                                            item.checkConfigApp = true;
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
                                            item.checkConfigApp = false;
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
                                ConfigAppProcessor.Reload(ucGridControlConfigApp, lstChecks);
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

        private void btn_Radio_Enable_Click_Mate(SDA_CONFIG_APP data)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                SdaConfigAppUserFilter matyAccountFilter = new SdaConfigAppUserFilter();
                matyAccountFilter.CONFIG_APP_ID = data.ID;
                ConfigAppIdCheck = data.ID;

                matyConfigApps = new BackendAdapter(param).Get<List<SDA_CONFIG_APP_USER>>(
                    "api/SdaConfigAppUser/Get",
                   ApiConsumers.SdaConsumer,
                   matyAccountFilter,
                   param);
                lstAccountADOs = new List<HIS.UC.Account.AccountADO>();

                lstAccountADOs = (from r in listAccount select new AccountADO(r)).ToList();
                if (matyConfigApps != null && matyConfigApps.Count > 0)
                {
                    foreach (var itemUsername in matyConfigApps)
                    {
                        var check = lstAccountADOs.FirstOrDefault(o => o.LOGINNAME == itemUsername.LOGINNAME);
                        if (check != null)
                        {
                            check.check2 = true;
                        }
                    }
                }

                lstAccountADOs = lstAccountADOs.OrderByDescending(p => p.check2).ToList();
                if (ucGridControlAccount != null)
                {
                    AccountProcessor.Reload(ucGridControlAccount, lstAccountADOs);
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

        private void InitUCgridAccount()
        {
            try
            {
                var culture = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                var langManager = Resources.ResourceLanguageManager.LanguageResource;
                AccountProcessor = new AccountProcessor();
                AccountInitADO ado = new AccountInitADO();
                ado.ListAccountColumn = new List<AccountColumn>();
                ado.gridViewAccount_MouseDownAccount = gridViewAccount_MouseDownMest;
                ado.btn_Radio_Enable_Click1 = btn_Radio_Enable_Click1;
                ado.Check__Enable_CheckedChanged = AccountCheckedChanged;

                AccountColumn colRadio1 = new AccountColumn("   ", "radio2", 30, true);
                colRadio1.VisibleIndex = 0;
                colRadio1.Visible = false;
                colRadio1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListAccountColumn.Add(colRadio1);

                AccountColumn colCheck1 = new AccountColumn("   ", "check2", 30, true);
                colCheck1.VisibleIndex = 1;
                colCheck1.image = imageCollectionAccount.Images[0];
                colCheck1.Visible = false;
                colCheck1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListAccountColumn.Add(colCheck1);

                AccountColumn colTenDangNhap = new AccountColumn("Tên đăng nhập", "LOGINNAME", 120, false);
                colTenDangNhap.VisibleIndex = 2;
                ado.ListAccountColumn.Add(colTenDangNhap);

                AccountColumn colHoTen = new AccountColumn("Họ tên", "USERNAME", 120, false);
                colHoTen.VisibleIndex = 3;
                ado.ListAccountColumn.Add(colHoTen);

                this.ucGridControlAccount = (UserControl)AccountProcessor.Run(ado);
                if (ucGridControlAccount != null)
                {
                    this.panelControl1.Controls.Add(this.ucGridControlAccount);
                    this.ucGridControlAccount.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void AccountCheckedChanged(AccountADO data)
        {
            try
            {
                bool success = false;
                var sources = (List<AccountADO>)AccountProcessor.GetDataGridView(ucGridControlAccount);
                var itemSources = sources.FirstOrDefault(o => o.ID == data.ID);
                if (listCheckAccountAdos != null && listCheckAccountAdos.Count > 0)
                {
                    foreach (var item in listCheckAccountAdos)
                    {
                        if (data.ID == item.ID)
                        {
                            listCheckAccountAdos.FirstOrDefault(o => o.ID == itemSources.ID).check2 = itemSources.check2;
                            success = true;
                            break;
                        }
                    }
                    if (!success)
                    {
                        listCheckAccountAdos.Add(itemSources);
                    }
                }
                else
                {
                    listCheckAccountAdos.Add(itemSources);
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewAccount_MouseDownMest(object sender, MouseEventArgs e)
        {
            try
            {
                if (isChooseAccount == 1)
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
                            var lstCheckAll = lstAccountADOs;
                            List<HIS.UC.Account.AccountADO> lstChecks = new List<HIS.UC.Account.AccountADO>();

                            if (lstCheckAll != null && lstCheckAll.Count > 0)
                            {
                                var AccountCheckedNum = lstAccountADOs.Where(o => o.check2 == true).Count();
                                var AccountNum = lstAccountADOs.Count();
                                if ((AccountCheckedNum > 0 && AccountCheckedNum < AccountNum) || AccountCheckedNum == 0)
                                {
                                    isCheckAll = true;
                                    hi.Column.Image = imageCollectionAccount.Images[1];
                                }

                                if (AccountCheckedNum == AccountNum)
                                {
                                    isCheckAll = false;
                                    hi.Column.Image = imageCollectionAccount.Images[0];
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

                                AccountProcessor.Reload(ucGridControlAccount, lstChecks);
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

        private void btn_Radio_Enable_Click1(ACS_USER data)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                SdaConfigAppUserFilter matyAccountFilter = new SdaConfigAppUserFilter();
                matyAccountFilter.LOGINNAME = data.LOGINNAME;
                accountIdCheckByAccount = data.LOGINNAME;

                matyAccounts = new BackendAdapter(param).Get<List<SDA_CONFIG_APP_USER>>(
                                "api/SdaConfigAppUser/Get",
                               ApiConsumers.SdaConsumer,
                                matyAccountFilter,
                                param);
                lstConfigAppADOs = new List<HIS.UC.ConfigApp.ConfigAppADO>();
                lstConfigAppADOs = (from r in listConfigApp select new ConfigAppADO(r)).ToList();
                if (matyAccounts != null && matyAccounts.Count > 0)
                {
                    foreach (var itemUsername in matyAccounts)
                    {
                        var check = lstConfigAppADOs.FirstOrDefault(o => o.ID == itemUsername.CONFIG_APP_ID);
                        if (check != null)
                        {
                            check.checkConfigApp = true;
                            check.DEFAULT_VALUE = itemUsername.VALUE;
                        }
                    }
                }

                lstConfigAppADOs = lstConfigAppADOs.OrderByDescending(p => p.checkConfigApp).ToList();
                if (ucGridControlConfigApp != null)
                {
                    ConfigAppProcessor.Reload(ucGridControlConfigApp, lstConfigAppADOs);
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

        private void btnFind2_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                FillDataToGridConfigApp(this);
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
                FillDataToGridAccount(this);
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
                checkRa = false;
                isChooseConfigApp = 0;
                isChooseAccount = 0;
                txtKeyword1.Text = null;
                txtKeyword2.Text = null;
                matyConfigApps = null;
                matyAccounts = null;
                lstConfigAppADOs = null;
                lstAccountADOs = null;
                ConfigAppIdCheck = 0;
                accountIdCheckByAccount = null;
                listCheckAccountAdos = new List<AccountADO>();
                listCheckConfigAppAdos = new List<ConfigAppADO>();
                FillDataToGridConfigApp(this);
                FillDataToGridAccount(this);
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
                if (ucGridControlConfigApp != null && ucGridControlAccount != null)
                {
                    object ConfigApp = ConfigAppProcessor.GetDataGridView(ucGridControlConfigApp);
                    object Account = AccountProcessor.GetDataGridView(ucGridControlAccount);
                    bool success = false;
                    CommonParam param = new CommonParam();
                    if (isChooseAccount == 1)
                    {
                        if (ConfigApp is List<HIS.UC.ConfigApp.ConfigAppADO>)
                        {
                            this.lstConfigAppADOs = (List<HIS.UC.ConfigApp.ConfigAppADO>)ConfigApp;

                            if (this.lstConfigAppADOs != null && this.lstConfigAppADOs.Count > 0 && checkRa == true)
                            {
                                //Danh sach cac user duoc check

                                var dataCheckeds = this.lstConfigAppADOs.Where(p => p.checkConfigApp == true).ToList();

                                //List xoa

                                var dataDeletes = this.lstConfigAppADOs.Where(o => matyAccounts.Select(p => p.CONFIG_APP_ID)
                                    .Contains(o.ID) && o.checkConfigApp == false).ToList();

                                //list them
                                var dataCreates = dataCheckeds.Where(o => !matyAccounts.Select(p => p.CONFIG_APP_ID)
                                    .Contains(o.ID)).ToList();
                                //List update
                                var dataUpdate = dataCheckeds.Where(o => matyAccounts.Select(p => p.CONFIG_APP_ID)
                                   .Contains(o.ID)).ToList();

                                if (dataCheckeds.Count == 0 && dataDeletes.Count == 0)
                                {
                                    WaitingManager.Hide();
                                    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn cấu hình", "Thông báo");
                                    return;
                                }

                                //xử lý update
                                if (dataUpdate != null && dataUpdate.Count > 0)
                                {
                                    UpdateConfigAppProcess(dataUpdate, param);
                                }

                                //xử lý delete
                                if (dataDeletes != null && dataDeletes.Count > 0)
                                {
                                    DeleteConfigAppProcess(dataDeletes, param);
                                }

                                //xử lý thêm
                                if (dataCreates != null && dataCreates.Count > 0)
                                {
                                    CreateConfigAppProcess(dataCreates, param);
                                }

                                if (!string.IsNullOrEmpty(accountIdCheckByAccount))
                                {
                                    ACS_USER acsUser = new ACS_USER();
                                    acsUser.LOGINNAME = accountIdCheckByAccount;
                                    btn_Radio_Enable_Click1(acsUser);
                                }
                            }
                        }
                    }
                    if (isChooseConfigApp == 2)
                    {
                        if (Account is List<HIS.UC.Account.AccountADO>)
                        {
                            this.lstAccountADOs = (List<HIS.UC.Account.AccountADO>)Account;

                            if (this.lstAccountADOs != null && this.lstAccountADOs.Count > 0)
                            {
                                //bool success = false;
                                HIS.UC.ConfigApp.ConfigAppADO ConfigAppType = this.lstConfigAppADOs.FirstOrDefault(o => o.ID == ConfigAppIdCheck);
                                //Danh sach cac user duoc check

                                var dataChecked = this.lstAccountADOs.Where(p => p.check2 == true).ToList();


                                //List xoa

                                var dataDelete = this.lstAccountADOs.Where(o => matyConfigApps.Select(p => p.LOGINNAME)
                                    .Contains(o.LOGINNAME) && o.check2 == false).ToList();

                                //list them
                                var dataCreate = dataChecked.Where(o => !matyConfigApps.Select(p => p.LOGINNAME)
                                    .Contains(o.LOGINNAME)).ToList();
                                //list update
                                var dataUpdate = dataChecked.Where(o => matyConfigApps.Select(p => p.LOGINNAME)
                                   .Contains(o.LOGINNAME)).ToList();

                                if (dataChecked.Count == 0 && dataDelete.Count == 0)
                                {
                                    WaitingManager.Hide();
                                    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn tài khoản", "Thông báo");
                                    return;
                                }

                                //xử lý update
                                if (dataUpdate != null && dataUpdate.Count > 0)
                                {
                                    UpdateAccountProcess(dataUpdate, ConfigAppType, param);
                                }

                                //xử lý delete
                                if (dataDelete != null && dataDelete.Count > 0)
                                {
                                    DeleteAccountProcess(dataDelete, param);
                                }

                                //xử lý Create
                                if (dataCreate != null && dataCreate.Count > 0)
                                {
                                    CreateAccountProcess(dataCreate, ConfigAppType, param);
                                }

                                if (ConfigAppIdCheck > 0)
                                {
                                    SDA_CONFIG_APP sdaConfigApp = new SDA_CONFIG_APP();
                                    sdaConfigApp.ID = ConfigAppIdCheck;
                                    btn_Radio_Enable_Click_Mate(sdaConfigApp);
                                }

                                //this.lstAccountADOs = this.lstAccountADOs.OrderByDescending(p => p.check2).ToList();
                                //if (ucGridControlAccount != null)
                                //{
                                //    AccountProcessor.Reload(ucGridControlAccount, this.lstAccountADOs);
                                //}
                            }
                        }
                    }
                    WaitingManager.Hide();
                    MessageManager.Show(this.ParentForm, param, !param.HasException);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool CreateAccountProcess(List<AccountADO> dataCreate, ConfigAppADO ConfigAppType, CommonParam param)
        {
            bool success = false;
            if (dataCreate != null && dataCreate.Count > 0 && ConfigAppType != null)
            {
                if (string.IsNullOrEmpty(ConfigAppType.DEFAULT_VALUE))
                {
                    var deleteIdsConfigAppUser = matyConfigApps.Where(o => o.CONFIG_APP_ID == ConfigAppIdCheck).Select(p => p.ID).ToList();
                    if (deleteIdsConfigAppUser != null && deleteIdsConfigAppUser.Count > 0)
                    {
                        bool deleteResult = new BackendAdapter(param).Post<bool>(
                          "api/SdaConfigAppUser/DeleteList",
                         ApiConsumers.SdaConsumer,
                          deleteIdsConfigAppUser,
                          param);
                        if (deleteResult)
                        {
                            success = true;
                            matyConfigApps = matyConfigApps.Where(o => !deleteIdsConfigAppUser.Contains(o.ID)).ToList();
                            if (this.delegateRefresh != null)
                                this.delegateRefresh();
                        }
                    }
                }
                else
                {
                    List<SDA_CONFIG_APP_USER> mestAccountCreate = new List<SDA_CONFIG_APP_USER>();
                    foreach (var item in dataCreate)
                    {
                        SDA_CONFIG_APP_USER mestAccount = new SDA_CONFIG_APP_USER();
                        mestAccount.LOGINNAME = item.LOGINNAME;
                        mestAccount.CONFIG_APP_ID = ConfigAppIdCheck;
                        mestAccount.VALUE = ConfigAppType.DEFAULT_VALUE;
                        mestAccountCreate.Add(mestAccount);
                    }

                    var createResult = new BackendAdapter(param).Post<List<SDA_CONFIG_APP_USER>>(
                               "api/SdaConfigAppUser/CreateList",
                               ApiConsumers.SdaConsumer,
                               mestAccountCreate,
                               param);
                    if (createResult != null && createResult.Count > 0)
                    {
                        success = true;
                        AutoMapper.Mapper.CreateMap<SDA_CONFIG_APP_USER, SDA_CONFIG_APP_USER>();
                        var vCreateResults = AutoMapper.Mapper.Map<List<SDA_CONFIG_APP_USER>, List<SDA_CONFIG_APP_USER>>(createResult);
                        matyConfigApps.AddRange(vCreateResults);
                        if (this.delegateRefresh != null)
                            this.delegateRefresh();
                    }
                }
            }
            return success;
        }

        //Hàm xử lý xóa Account
        private bool DeleteAccountProcess(List<AccountADO> dataDelete, CommonParam param)
        {
            bool success = false;
            if (dataDelete != null && dataDelete.Count > 0)
            {
                List<long> deleteId = matyConfigApps.Where(o => dataDelete.Select(p => p.LOGINNAME)
                    .Contains(o.LOGINNAME)).Select(o => o.ID).ToList();
                bool deleteResult = new BackendAdapter(param).Post<bool>(
                          "api/SdaConfigAppUser/DeleteList",
                         ApiConsumers.SdaConsumer,
                          deleteId,
                          param);
                if (deleteResult)
                {
                    success = true;
                    matyConfigApps = matyConfigApps.Where(o => !deleteId.Contains(o.ID)).ToList();
                    if (this.delegateRefresh != null)
                        this.delegateRefresh();
                }
            }
            return success;
        }

        //Hàm xử lý update Account
        private bool UpdateAccountProcess(List<AccountADO> dataUpdate, ConfigAppADO ConfigAppType, CommonParam param)
        {
            bool success = false;
            if (dataUpdate != null && dataUpdate.Count > 0 && ConfigAppType != null)
            {
                if (string.IsNullOrEmpty(ConfigAppType.DEFAULT_VALUE))
                {
                    var deleteIdsConfigAppUser = matyConfigApps.Where(o => o.CONFIG_APP_ID == ConfigAppIdCheck).Select(p => p.ID).ToList();
                    if (deleteIdsConfigAppUser != null && deleteIdsConfigAppUser.Count > 0)
                    {
                        bool deleteResult = new BackendAdapter(param).Post<bool>(
                          "api/SdaConfigAppUser/DeleteList",
                         ApiConsumers.SdaConsumer,
                          deleteIdsConfigAppUser,
                          param);
                        if (deleteResult)
                        {
                            success = true;
                            matyConfigApps = matyConfigApps.Where(o => !deleteIdsConfigAppUser.Contains(o.ID)).ToList();
                            if (this.delegateRefresh != null)
                                this.delegateRefresh();
                        }
                    }
                }
                else
                {
                    var ConfigAppMetyUpdates = new List<SDA_CONFIG_APP_USER>();
                    var DeleteIdConfigAppUser = new List<long>();
                    foreach (var item in dataUpdate)
                    {
                        var ConfigAppMaty = matyConfigApps.FirstOrDefault(o => o.CONFIG_APP_ID == ConfigAppIdCheck && o.LOGINNAME == item.LOGINNAME);
                        if (ConfigAppMaty != null)
                        {
                            ConfigAppMaty.VALUE = ConfigAppType.DEFAULT_VALUE;
                            ConfigAppMetyUpdates.Add(ConfigAppMaty);
                        }
                    }
                    if (ConfigAppMetyUpdates != null && ConfigAppMetyUpdates.Count > 0)
                    {
                        var updateResult = new BackendAdapter(param).Post<List<SDA_CONFIG_APP_USER>>(
                                   "/api/SdaConfigAppUser/UpdateList",
                                   ApiConsumers.SdaConsumer,
                                   ConfigAppMetyUpdates,
                                   param);
                        if (updateResult != null && updateResult.Count > 0)
                        {
                            //listMediStockMety.AddRange(updateResult);
                            success = true;
                            if (this.delegateRefresh != null)
                                this.delegateRefresh();
                        }
                    }
                }
            }
            return success;
        }

        //xử lý thêm Config
        private bool CreateConfigAppProcess(List<ConfigAppADO> dataCreates, CommonParam param)
        {
            bool success = false;
            var dataDeletes = dataCreates.Where(o => string.IsNullOrEmpty(o.DEFAULT_VALUE)).ToList();
            dataCreates = dataCreates.Where(o => !string.IsNullOrEmpty(o.DEFAULT_VALUE)).ToList();
            if (dataCreates != null && dataCreates.Count > 0)
            {
                List<SDA_CONFIG_APP_USER> MestAccountCreates = new List<SDA_CONFIG_APP_USER>();
                foreach (var item in dataCreates)
                {
                    SDA_CONFIG_APP_USER mestAccount = new SDA_CONFIG_APP_USER();
                    mestAccount.CONFIG_APP_ID = item.ID;
                    mestAccount.LOGINNAME = accountIdCheckByAccount;
                    mestAccount.VALUE = item.DEFAULT_VALUE;
                    MestAccountCreates.Add(mestAccount);
                }

                var createResult = new BackendAdapter(param).Post<List<SDA_CONFIG_APP_USER>>(
                           "api/SdaConfigAppUser/CreateList",
                           ApiConsumers.SdaConsumer,
                           MestAccountCreates,
                           param);
                if (createResult != null && createResult.Count > 0)
                {
                    success = true;
                    AutoMapper.Mapper.CreateMap<SDA_CONFIG_APP_USER, SDA_CONFIG_APP_USER>();
                    var vCreateResults = AutoMapper.Mapper.Map<List<SDA_CONFIG_APP_USER>, List<SDA_CONFIG_APP_USER>>(createResult);
                    matyAccounts.AddRange(vCreateResults);
                    if (this.delegateRefresh != null)
                        this.delegateRefresh();
                }
            }
            if (dataDeletes != null && dataDeletes.Count > 0)
            {
                List<long> deleteIds = matyAccounts.Where(o => dataDeletes.Select(p => p.ID)
                    .Contains(o.CONFIG_APP_ID)).Select(o => o.ID).ToList();
                if (deleteIds != null && deleteIds.Count > 0)
                {
                    bool deleteResult = new BackendAdapter(param).Post<bool>(
                              "api/SdaConfigAppUser/DeleteList",
                              ApiConsumers.SdaConsumer,
                              deleteIds,
                              param);
                    if (deleteResult)
                    {
                        success = true;
                        if (this.delegateRefresh != null)
                            this.delegateRefresh();
                        matyAccounts = matyAccounts.Where(o => !deleteIds.Contains(o.ID)).ToList();
                    }
                }
            }
            return success;
        }

        //Hàm xóa ConfigApp
        private bool DeleteConfigAppProcess(List<ConfigAppADO> dataDeletes, CommonParam param)
        {
            bool success = false;
            if (dataDeletes != null && dataDeletes.Count > 0)
            {
                List<long> deleteIds = matyAccounts.Where(o => dataDeletes.Select(p => p.ID)
                    .Contains(o.CONFIG_APP_ID)).Select(o => o.ID).ToList();
                bool deleteResult = new BackendAdapter(param).Post<bool>(
                          "api/SdaConfigAppUser/DeleteList",
                          ApiConsumers.SdaConsumer,
                          deleteIds,
                          param);
                if (deleteResult)
                {
                    success = true;
                    if (this.delegateRefresh != null)
                        this.delegateRefresh();
                    matyAccounts = matyAccounts.Where(o => !deleteIds.Contains(o.ID)).ToList();
                }
            }
            return success;
        }

        //Hàm update ConfigApp
        private bool UpdateConfigAppProcess(List<ConfigAppADO> dataUpdate, CommonParam param)
        {

            bool success = false;
            var dataDeletes = dataUpdate.Where(o => string.IsNullOrEmpty(o.DEFAULT_VALUE)).ToList();
            dataUpdate = dataUpdate.Where(o => !string.IsNullOrEmpty(o.DEFAULT_VALUE)).ToList();
            if (dataUpdate != null && dataUpdate.Count > 0)
            {
                var SdaConfigAppUserUpdates = new List<SDA_CONFIG_APP_USER>();
                foreach (var item in dataUpdate)
                {

                    var SdaConfigAppUser = matyAccounts.FirstOrDefault(o => o.CONFIG_APP_ID == item.ID && o.LOGINNAME == accountIdCheckByAccount);
                    if (SdaConfigAppUser != null)
                    {
                        SdaConfigAppUser.VALUE = item.DEFAULT_VALUE;
                        SdaConfigAppUserUpdates.Add(SdaConfigAppUser);
                    }
                }
                if (SdaConfigAppUserUpdates != null && SdaConfigAppUserUpdates.Count > 0)
                {
                    var updateResult = new BackendAdapter(param).Post<List<SDA_CONFIG_APP_USER>>(
                               "/api/SdaConfigAppUser/UpdateList",
                               ApiConsumers.SdaConsumer,
                               SdaConfigAppUserUpdates,
                               param);
                    if (updateResult != null && updateResult.Count > 0)
                    {
                        //listMediStockMety.AddRange(updateResult);
                        success = true;
                        if (this.delegateRefresh != null)
                            this.delegateRefresh();
                    }
                }
            }

            if (dataDeletes != null && dataDeletes.Count > 0)
            {
                List<long> deleteIds = matyAccounts.Where(o => dataDeletes.Select(p => p.ID)
                    .Contains(o.CONFIG_APP_ID)).Select(o => o.ID).ToList();
                bool deleteResult = new BackendAdapter(param).Post<bool>(
                          "api/SdaConfigAppUser/DeleteList",
                          ApiConsumers.SdaConsumer,
                          deleteIds,
                          param);
                if (deleteResult)
                {
                    success = true;
                    if (this.delegateRefresh != null)
                        this.delegateRefresh();
                    matyAccounts = matyAccounts.Where(o => !deleteIds.Contains(o.ID)).ToList();
                }
            }

            return success;
        }

        //xử lý tìm kiếm tài khoản
        private void txtKeyword1_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToGridAccount(this);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //xử lý tìm kiếm Config
        private void txtKeyword2_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToGridConfigApp(this);
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
                //Khởi tạo đối tượng resources
                Resources.ResourceLanguageManager.LanguageResource = new System.Resources.ResourceManager("HIS.Desktop.Plugins.SdaConfigAppUser.Resources.Lang", typeof(HIS.Desktop.Plugins.SdaConfigAppUser.UCSdaConfigAppUser).Assembly);
                //Gán giá trị cho các control
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
