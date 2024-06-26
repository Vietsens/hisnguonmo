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
using HIS.UC.ExecuteRole;
using HIS.UC.ExecuteRole.ADO;
using HIS.UC.Account;
using HIS.UC.Account.ADO;
using HIS.Desktop.Plugins.ExecuteRoleUser.entity;
using Inventec.Common.Controls.EditorLoader;
using AutoMapper;
using ACS.Filter;
using HIS.Desktop.LocalStorage.BackendData;
using ACS.EFMODEL.DataModels;
using Inventec.Common.Adapter;
using HIS.Desktop.LocalStorage.ConfigApplication;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.Controls.Session;
using Inventec.Common.RichEditor.Base;
using HIS.Desktop.Plugins.ExecuteRoleUser.Properties;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.ApiConsumer;

namespace HIS.Desktop.Plugins.ExecuteRoleUser
{
    public partial class UC_ExecuteRoleUser : HIS.Desktop.Utility.UserControlBase
    {
        #region Declare
        List<ExecuteRoleADO> listExecuteRoleDeleteADO { get; set; }
        List<ExecuteRoleADO> listExecuteRoleInsertADO { get; set; }
        List<HIS_EXECUTE_ROLE> executeRole { get; set; }
        internal Inventec.Desktop.Common.Modules.Module currentModule;
        ExecuteRoleProcessor ERProcessor;
        ExecuteRoleADO erADO;
        AccountProcessor AccountProcessor;
        UserControl ucGridControlER;
        UserControl ucGridControlAccount;
        int start = 0;
        //int limit = 0;
        int rowCount = 0;
        int dataTotal = 0;
        int start1 = 0;
        //int limit1 = 0;
        int rowCount1 = 0;
        int dataTotal1 = 0;
        bool isCheckAll;
        bool isCheckAllUser;

        private List<AccountADO> UserAdoTotal = new List<AccountADO>();
        private List<AccountADO> listUser;
        //internal List<HIS.UC.Account.AccountADO> lstAccountADOs { get; set; }
        internal List<HIS.UC.ExecuteRole.ExecuteRoleADO> lstExecuteRoleADOs { get; set; }
        //List<ACS_USER> listUser;
        List<HIS_EXECUTE_ROLE> listExecuteRole;
        long ERIdCheckByER = 0;
        long isChoseER;
        long isChoseAccount;
        String accountIdCheckByAccount = null;
        List<HIS_EXECUTE_ROLE_USER> executeRoleUser { get; set; }
        List<HIS_EXECUTE_ROLE_USER> executeRoleUser1 { get; set; }
        bool checkRa = false;
        #endregion

        #region Constructor
        public UC_ExecuteRoleUser(Inventec.Desktop.Common.Modules.Module currentModule)
            : base(currentModule)
        {
            InitializeComponent();
        }

        private void UC_ExecuteRoleUser_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                WaitingManager.Show();
                txtSearchER.Focus();
                txtSearchER.SelectAll();
                LoadDataUserAdo();
                LoadDataToCombo();
                InitUcgrid1();
                InitUcgrid2();
                FillDataToGrid1(this);
                FillDataToGrid2(this);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Load column
        private void InitUcgrid1()
        {
            try
            {
                ERProcessor = new ExecuteRoleProcessor();
                ExecuteRoleInitADO ado = new ExecuteRoleInitADO();
                ado.ListExecuteRoleColumn = new List<ExecuteRoleColumn>();
                ado.btn_Radio_Enable_Click = btn_Radio_Enable_Click;
                ado.ExecuteRoleGrid_CellValueChanged = ExecuteRoleGrid_CellValueChanged;
                ado.ExecuteRoleGrid_MouseDown = ExecuteRole_MouseDown;

                ExecuteRoleColumn colRadio1 = new ExecuteRoleColumn("   ", "radio1", 30, true);
                colRadio1.VisibleIndex = 0;
                colRadio1.Visible = false;
                colRadio1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListExecuteRoleColumn.Add(colRadio1);

                ExecuteRoleColumn colCheck1 = new ExecuteRoleColumn("   ", "check1", 30, true);
                colCheck1.VisibleIndex = 1;
                colCheck1.Visible = false;
                colCheck1.image = imageCollection.Images[0];
                colCheck1.ToolTip = Inventec.Common.Resource.Get.Value("UC_ExecuteRoleUser.ChonTatCa", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                colCheck1.imageAlignment = StringAlignment.Center;
                colCheck1.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListExecuteRoleColumn.Add(colCheck1);

                ExecuteRoleColumn colMaVaiTro = new ExecuteRoleColumn(Inventec.Common.Resource.Get.Value("UC_ExecuteRoleUser.MaVaiTro", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()), "EXECUTE_ROLE_CODE", 120, false);
                colMaVaiTro.VisibleIndex = 2;
                ado.ListExecuteRoleColumn.Add(colMaVaiTro);

                ExecuteRoleColumn colTenVaiTro = new ExecuteRoleColumn(Inventec.Common.Resource.Get.Value("UC_ExecuteRoleUser.TenVaiTro", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()), "EXECUTE_ROLE_NAME", 120, false);
                colTenVaiTro.VisibleIndex = 3;
                ado.ListExecuteRoleColumn.Add(colTenVaiTro);

                this.ucGridControlER = (UserControl)ERProcessor.Run(ado);
                if (ucGridControlER != null)
                {
                    this.panelControl1.Controls.Add(this.ucGridControlER);
                    this.ucGridControlER.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
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
                ado.btn_Radio_Enable_Click1 = btn_Radio_Enable_Click1;
                ado.gridViewAccount_MouseDownAccount = Account_MouseDown;
                ado.AccountGrid_CustomUnboundColumnData = gridViewUser_CustomUnboundColumnData;
                object image = Properties.Resources.ResourceManager.GetObject("check1");

                AccountColumn colRadio2 = new AccountColumn("   ", "radio2", 30, true);
                colRadio2.VisibleIndex = 0;
                colRadio2.Visible = false;
                colRadio2.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListAccountColumn.Add(colRadio2);

                AccountColumn colCheck2 = new AccountColumn("   ", "check2", 30, true);
                colCheck2.VisibleIndex = 1;
                colCheck2.Visible = false;
                colCheck2.image = imageCollection.Images[0];
                //colCheck2.image = StringAlignment.Center;
                colCheck2.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListAccountColumn.Add(colCheck2);

                AccountColumn colTenDangNhap = new AccountColumn(Inventec.Common.Resource.Get.Value("UC_ExecuteRoleUser.TenDangNhap", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()), "LOGINNAME", 120, false);
                colTenDangNhap.VisibleIndex = 2;
                ado.ListAccountColumn.Add(colTenDangNhap);

                AccountColumn colHoTen = new AccountColumn(Inventec.Common.Resource.Get.Value("UC_ExecuteRoleUser.HoTen", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()), "USERNAME", 120, false);
                colHoTen.VisibleIndex = 3;
                ado.ListAccountColumn.Add(colHoTen);

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
        #endregion

        #region Load Combo
        private void LoadDataToCombo()
        {
            try
            {
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
                status.Add(new Status(1, "Vai trò thực hiện"));
                status.Add(new Status(2, "Tài khoản"));

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("statusName", "", 300, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("statusName", "id", columnInfos, false, 350);
                ControlEditorLoader.Load(cboChooseBy, status, controlEditorADO);
                cboChooseBy.EditValue = status[0].id;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Method
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

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.ExecuteRoleUser.Resources.Lang", typeof(HIS.Desktop.Plugins.ExecuteRoleUser.UC_ExecuteRoleUser).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UC_ExecuteRoleUser.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboChooseBy.Properties.NullText = Inventec.Common.Resource.Get.Value("UC_ExecuteRoleUser.cboChooseBy.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("UC_ExecuteRoleUser.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("UC_ExecuteRoleUser.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearchA.Text = Inventec.Common.Resource.Get.Value("UC_ExecuteRoleUser.btnSearchA.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtSearchA.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UC_ExecuteRoleUser.txtSearchA.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UC_ExecuteRoleUser.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSeachER.Text = Inventec.Common.Resource.Get.Value("UC_ExecuteRoleUser.btnSeachER.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtSearchER.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UC_ExecuteRoleUser.txtSearchER.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("UC_ExecuteRoleUser.layoutControlItem12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToGrid2(UC_ExecuteRoleUser uCExecuteRoleUser)
        {
            try
            {
                int numPageSize;
                if (ucPaging2.pagingGrid != null)
                {
                    numPageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
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
                isChoseAccount = (long)cboChooseBy.EditValue;

                var startUser = ((CommonParam)data).Start ?? 0;
                var limitUser = ((CommonParam)data).Limit ?? 0;
                var query = UserAdoTotal.AsQueryable();
                string keyword = txtSearchA.Text.Trim();
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
                    query = query.Where(o => o.IS_ACTIVE == IMSys.DbConfig.ACS_RS.COMMON.IS_ACTIVE__TRUE);
                }

                query = query.OrderByDescending(o => o.check2).ThenBy(o => o.LOGINNAME);
                dataTotal1 = query.Count();
                this.listUser = query.Skip(startUser).Take(limitUser).ToList();
                rowCount1 = (listUser == null ? 0 : listUser.Count);

                foreach (var item in listUser)
                {
                    item.isKeyChoose1 = (isChoseAccount == 2);
                    item.check2 = ((executeRoleUser != null && executeRoleUser.Count > 0) ? executeRoleUser.Exists(e => e.LOGINNAME == item.LOGINNAME) : false);
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

        private void FillDataToGrid1(UC_ExecuteRoleUser uCExecuteRoleUser)
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
                FillDataToGridExecuteRole(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridExecuteRole, param, numPageSize);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridExecuteRole(object data)
        {
            try
            {
                WaitingManager.Show();
                listExecuteRole = new List<HIS_EXECUTE_ROLE>();
                int start = ((CommonParam)data).Start ?? 0;
                int limit = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start, limit);
                MOS.Filter.HisExecuteRoleFilter ERFillter = new HisExecuteRoleFilter();
                ERFillter.ORDER_FIELD = "MODIFY_TIME";
                ERFillter.ORDER_DIRECTION = "DESC";
                ERFillter.KEY_WORD = txtSearchER.Text;

                if ((long)cboChooseBy.EditValue == 1)
                {
                    isChoseER = (long)cboChooseBy.EditValue;
                }

                var rs = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE>>(
                     HisRequestUriStore.HIS_EXECUTE_ROLE_GET,
                     HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                     ERFillter,
                     param);

                lstExecuteRoleADOs = new List<HIS.UC.ExecuteRole.ExecuteRoleADO>();

                if (rs != null && rs.Data.Count > 0)
                {
                    //List<V_HIS_ROOM> dataByRoom = new List<V_HIS_ROOM>();
                    listExecuteRole = rs.Data;
                    foreach (var item in listExecuteRole)
                    {
                        HIS.UC.ExecuteRole.ExecuteRoleADO ExecuteRoleADO = new HIS.UC.ExecuteRole.ExecuteRoleADO(item);
                        if (isChoseER == 1)
                        {
                            ExecuteRoleADO.isKeyChoose = true;
                        }
                        lstExecuteRoleADOs.Add(ExecuteRoleADO);
                    }
                }

                if (executeRoleUser1 != null && executeRoleUser1.Count > 0)
                {
                    foreach (var item in executeRoleUser1)
                    {
                        var check = lstExecuteRoleADOs.FirstOrDefault(o => o.ID == item.EXECUTE_ROLE_ID);
                        if (check != null)
                        {
                            check.check1 = true;
                        }
                    }
                }

                lstExecuteRoleADOs = lstExecuteRoleADOs.OrderByDescending(p => p.check1).ToList();

                if (ucGridControlER != null)
                {
                    ERProcessor.Reload(ucGridControlER, lstExecuteRoleADOs);
                }
                rowCount = (data == null ? 0 : lstExecuteRoleADOs.Count);
                dataTotal = (rs.Param == null ? 0 : rs.Param.Count ?? 0);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
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

        public void Import()
        {
            try
            {
                if (btnImport.Enabled)
                {
                    btnImport.Focus();
                    btnImport_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Event
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                List<object> listArgs = new List<object>();
                listArgs.Add((HIS.Desktop.Common.RefeshReference)refreshForm);
                if (this.currentModule != null)
                {
                    CallModule callModule = new CallModule(CallModule.HisImportServiceRoom, this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
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

        private void refreshForm()
        {
            try
            {
                WaitingManager.Show();
                FillDataToGrid1(this);
                FillDataToGrid2(this);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboChoose_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                checkRa = false;
                isChoseAccount = 0;
                isChoseER = 0;
                executeRoleUser = null;
                executeRoleUser1 = null;
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
                bool resultSuccess = false;

                CommonParam param = new CommonParam();
                WaitingManager.Show();
                if (ucGridControlAccount != null && ucGridControlER != null)
                {
                    object account = AccountProcessor.GetDataGridView(ucGridControlAccount);
                    object ExecuteR = ERProcessor.GetDataGridView(ucGridControlER);
                    if (isChoseER == 1)
                    {
                        //checkRa = false;
                        if (account is List<HIS.UC.Account.AccountADO>)
                        {
                            var data = (List<HIS.UC.Account.AccountADO>)account;
                            if (data != null && data.Count > 0)
                            {
                                if (checkRa == true)
                                {
                                    //Danh sach cac user duoc check
                                    MOS.Filter.HisExecuteRoleUserFilter filter = new HisExecuteRoleUserFilter();
                                    filter.EXECUTE_ROLE_ID = ERIdCheckByER;

                                    var erUser = new BackendAdapter(param).Get<List<HIS_EXECUTE_ROLE_USER>>(
                                       HisRequestUriStore.HIS_EXECUTE_ROLE_USER_GET,
                                       HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                       filter,
                                       param);

                                    List<String> listLoginName = erUser.Select(p => p.LOGINNAME).ToList();

                                    var dataCheckeds = data.Where(p => p.check2 == true).ToList();

                                    //List xoa

                                    var dataDeletes = data.Where(o => erUser.Select(p => p.LOGINNAME)
                                        .Contains(o.LOGINNAME) && o.check2 == false).ToList();

                                    //list them
                                    var dataCreates = dataCheckeds.Where(o => !erUser.Select(p => p.LOGINNAME)
                                        .Contains(o.LOGINNAME)).ToList();
                                    if (dataCheckeds.Count != erUser.Select(p => p.LOGINNAME).Count())
                                    {
                                        if (dataDeletes != null && dataDeletes.Count > 0)
                                        {
                                            List<long> deleteIds = erUser.Where(o => dataDeletes.Select(p => p.LOGINNAME)
                                                .Contains(o.LOGINNAME)).Select(o => o.ID).ToList();
                                            bool deleteResult = new BackendAdapter(param).Post<bool>(
                                                      "/api/HisExecuteRoleUser/DeleteList",
                                                      HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                                      deleteIds,
                                                      param);
                                            if (deleteResult)
                                                resultSuccess = true;
                                        }

                                        if (dataCreates != null && dataCreates.Count > 0)
                                        {
                                            List<HIS_EXECUTE_ROLE_USER> erUserCreates = new List<HIS_EXECUTE_ROLE_USER>();
                                            foreach (var item in dataCreates)
                                            {
                                                HIS_EXECUTE_ROLE_USER erUserCreate = new HIS_EXECUTE_ROLE_USER();
                                                erUserCreate.LOGINNAME = item.LOGINNAME;
                                                erUserCreate.EXECUTE_ROLE_ID = ERIdCheckByER;
                                                erUserCreates.Add(erUserCreate);
                                            }

                                            var createResult = new BackendAdapter(param).Post<List<HIS_EXECUTE_ROLE_USER>>(
                                                       "/api/HisExecuteRoleUser/CreateList",
                                                       HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                                       erUserCreates,
                                                       param);
                                            if (createResult != null && createResult.Count > 0)
                                                resultSuccess = true;
                                        }
                                        WaitingManager.Hide();
                                        #region Show message
                                        MessageManager.Show(this.ParentForm, param, resultSuccess);
                                        #endregion

                                        #region Process has exception
                                        SessionManager.ProcessTokenLost(param);
                                        #endregion
                                    }

                                    data = data.OrderByDescending(p => p.check2).ToList();
                                    if (ucGridControlAccount != null)
                                    {
                                        AccountProcessor.Reload(ucGridControlAccount, data);
                                    }
                                }
                                else
                                {
                                    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn vai trò thực hiện");
                                }
                            }
                        }
                    }
                    if (isChoseAccount == 2)
                    {
                        //checkRa = false;
                        if (ExecuteR is List<HIS.UC.ExecuteRole.ExecuteRoleADO>)
                        {
                            var data = (List<HIS.UC.ExecuteRole.ExecuteRoleADO>)ExecuteR;
                            if (data != null && data.Count > 0)
                            {
                                if (checkRa == true)
                                {
                                    //Danh sach cac user duoc check
                                    MOS.Filter.HisExecuteRoleUserFilter filter = new HisExecuteRoleUserFilter();
                                    filter.LOGINNAME__EXACT = accountIdCheckByAccount;
                                    var erUser = new BackendAdapter(param).Get<List<HIS_EXECUTE_ROLE_USER>>(
                                       HisRequestUriStore.HIS_EXECUTE_ROLE_USER_GET,
                                       HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                       filter,
                                       param);

                                    var listERID = erUser.Select(p => p.EXECUTE_ROLE_ID).ToList();

                                    var dataChecked = data.Where(p => p.check1 == true).ToList();

                                    //List xoa

                                    var dataDelete = data.Where(o => erUser.Select(p => p.EXECUTE_ROLE_ID)
                                        .Contains(o.ID) && o.check1 == false).ToList();

                                    //list them
                                    var dataCreate = dataChecked.Where(o => !erUser.Select(p => p.EXECUTE_ROLE_ID)
                                        .Contains(o.ID)).ToList();
                                    if (dataChecked.Count != erUser.Select(p => p.EXECUTE_ROLE_ID).Count())
                                    {
                                        if (dataDelete != null && dataDelete.Count > 0)
                                        {
                                            List<long> deleteId = erUser.Where(o => dataDelete.Select(p => p.ID)

                                                .Contains(o.EXECUTE_ROLE_ID)).Select(o => o.ID).ToList();
                                            bool deleteResult = new BackendAdapter(param).Post<bool>(
                                                      "/api/HisExecuteRoleUser/DeleteList",
                                                      HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                                      deleteId,
                                                      param);
                                            if (deleteResult)
                                                resultSuccess = true;
                                        }

                                        if (dataCreate != null && dataCreate.Count > 0)
                                        {
                                            List<HIS_EXECUTE_ROLE_USER> erUserCreate = new List<HIS_EXECUTE_ROLE_USER>();
                                            foreach (var item in dataCreate)
                                            {
                                                HIS_EXECUTE_ROLE_USER erUserID = new HIS_EXECUTE_ROLE_USER();
                                                erUserID.EXECUTE_ROLE_ID = item.ID;
                                                erUserID.LOGINNAME = accountIdCheckByAccount;
                                                erUserCreate.Add(erUserID);
                                            }

                                            var createResult = new BackendAdapter(param).Post<List<HIS_EXECUTE_ROLE_USER>>(
                                                       "/api/HisExecuteRoleUser/CreateList",
                                                       HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                                       erUserCreate,
                                                       param);
                                            if (createResult != null && createResult.Count > 0)
                                                resultSuccess = true;
                                        }
                                        WaitingManager.Hide();
                                        #region Show message
                                        MessageManager.Show(this.ParentForm, param, resultSuccess);
                                        #endregion

                                        #region Process has exception
                                        SessionManager.ProcessTokenLost(param);
                                        #endregion
                                    }

                                    data = data.OrderByDescending(p => p.check1).ToList();
                                    if (ucGridControlER != null)
                                    {
                                        ERProcessor.Reload(ucGridControlER, data);
                                    }
                                }
                                else
                                {
                                    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn tài khoản");
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ExecuteRoleGrid_CellValueChanged(ExecuteRoleADO data, CellValueChangedEventArgs e)
        {
            try
            {
                //if (data != null)
                //{
                //    if (data.check1 == true)
                //    {

                //        listExecuteRoleInsertADO.Add(data);
                //    }
                //    else
                //    {

                //        listExecuteRoleDeleteADO.Add(data);
                //    }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ExecuteRole_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (isChoseER == 1)
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
                            var lstCheckAll = lstExecuteRoleADOs;
                            if (lstExecuteRoleADOs != null && lstExecuteRoleADOs.Count > 0)
                            {
                                if (isCheckAll)
                                {
                                    foreach (var item in lstExecuteRoleADOs)
                                    {
                                        item.check1 = true;
                                    }
                                    isCheckAll = false;
                                    hi.Column.Image = imageCollection.Images[1];
                                }
                                else
                                {
                                    foreach (var item in lstExecuteRoleADOs)
                                    {
                                        item.check1 = false;
                                    }
                                    isCheckAll = true;
                                    hi.Column.Image = imageCollection.Images[0];
                                }

                                ERProcessor.Reload(ucGridControlER, lstExecuteRoleADOs);
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

        private void Account_MouseDown(object sender, MouseEventArgs e)
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
                                if (isCheckAllUser)
                                {
                                    foreach (var item in listUser)
                                    {
                                        item.check2 = true;
                                    }
                                    isCheckAllUser = false;
                                    hi.Column.Image = imageCollection.Images[1];
                                }
                                else
                                {
                                    foreach (var item in listUser)
                                    {
                                        item.check2 = false;
                                    }
                                    isCheckAllUser = true;
                                    hi.Column.Image = imageCollection.Images[0];
                                }

                                //ReloadData
                                AccountProcessor.Reload(ucGridControlAccount, listUser);
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

        private void btn_Radio_Enable_Click(HIS_EXECUTE_ROLE data, ExecuteRoleADO adoER)
        {
            try
            {
                erADO = new ExecuteRoleADO();
                erADO = adoER;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                MOS.Filter.HisExecuteRoleUserFilter filter = new HisExecuteRoleUserFilter();
                filter.EXECUTE_ROLE_ID = data.ID;
                ERIdCheckByER = data.ID;
                executeRoleUser = new BackendAdapter(param).Get<List<HIS_EXECUTE_ROLE_USER>>(
                                HisRequestUriStore.HIS_EXECUTE_ROLE_USER_GET,
                                HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                filter,
                                param);
                List<HIS.UC.Account.AccountADO> dataNew = new List<HIS.UC.Account.AccountADO>();
                dataNew = (from r in listUser select new HIS.UC.Account.AccountADO(r)).ToList();
                if (executeRoleUser != null && executeRoleUser.Count > 0)
                {
                    foreach (var itemUsername in executeRoleUser)
                    {
                        var check = dataNew.FirstOrDefault(o => o.LOGINNAME == itemUsername.LOGINNAME);
                        if (check != null)
                        {
                            check.check2 = true;
                        }
                    }
                }

                dataNew = dataNew.OrderByDescending(p => p.check2).ToList();
                if (ucGridControlAccount != null)
                {
                    AccountProcessor.Reload(ucGridControlAccount, dataNew);
                }
                WaitingManager.Hide();
                checkRa = true;
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
                MOS.Filter.HisExecuteRoleUserFilter filter = new HisExecuteRoleUserFilter();
                filter.LOGINNAME__EXACT = data.LOGINNAME;
                accountIdCheckByAccount = data.LOGINNAME;
                executeRoleUser1 = new BackendAdapter(param).Get<List<HIS_EXECUTE_ROLE_USER>>(
                                HisRequestUriStore.HIS_EXECUTE_ROLE_USER_GET,
                                HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                filter,
                                param);
                List<HIS.UC.ExecuteRole.ExecuteRoleADO> dataNew = new List<HIS.UC.ExecuteRole.ExecuteRoleADO>();
                dataNew = (from r in listExecuteRole select new HIS.UC.ExecuteRole.ExecuteRoleADO(r)).ToList();
                if (executeRoleUser1 != null && executeRoleUser1.Count > 0)
                {

                    foreach (var itemUsername in executeRoleUser1)
                    {
                        var check = dataNew.FirstOrDefault(o => o.ID == itemUsername.EXECUTE_ROLE_ID);
                        if (check != null)
                        {
                            check.check1 = true;
                        }
                    }

                    dataNew = dataNew.OrderByDescending(p => p.check1).ToList();
                    if (ucGridControlER != null)
                    {
                        ERProcessor.Reload(ucGridControlER, dataNew);
                    }
                }
                else
                {
                    FillDataToGrid1(this);
                }
                WaitingManager.Hide();
                checkRa = true;
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSearchER_KeyDown(object sender, KeyEventArgs e)
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

        private void txtSearchA_KeyDown(object sender, KeyEventArgs e)
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
        #endregion
    }
}
