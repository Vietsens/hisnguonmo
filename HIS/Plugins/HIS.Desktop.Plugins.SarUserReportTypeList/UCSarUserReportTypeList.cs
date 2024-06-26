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
using HIS.UC.ReportType;
using HIS.UC.Account;
using ACS.EFMODEL.DataModels;
using SAR.EFMODEL.DataModels;
using HIS.UC.Account.ADO;
using HIS.UC.ReportType.ADO;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.Common.Adapter;
using HIS.Desktop.Plugins.SarUserReportTypeList.Entity;
using Inventec.Common.Controls.EditorLoader;
using SAR.Filter;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using DevExpress.XtraBars;
using SAR.SDO;
using DevExpress.XtraGrid;
using ACS.Filter;
using DevExpress.XtraEditors;
using MOS.EFMODEL.DataModels;
using MOS.Filter;

namespace HIS.Desktop.Plugins.SarUserReportTypeList
{
    public partial class UCSarUserReportTypeList : UserControl
    {
        private const int MAX_REQUEST_LENGTH_PARAM = 500;
        private List<HIS.UC.Account.AccountADO> UserAdoTotal = new List<AccountADO>();
        private List<HIS.UC.ReportType.ReportTypeADO> ReportAdo { get; set; }
        private int rowCount = 0;
        private int dataTotal = 0;
        private string checkUser = null;
        private long checkReport = 0;
        private UCReportTypeProcessor reportProcessor = null;
        private AccountProcessor userProcessor = null;
        private UserControl ucUser;
        private UserControl ucReport;
        private long isChoseUser = 0;
        private long isChoseReport = 0;
        private bool isCheckAll;
        private bool checkRa = false;
        private SAR_REPORT_TYPE reportType = new SAR_REPORT_TYPE();
        private List<AccountADO> listUser = new List<AccountADO>();
        private List<SAR_REPORT_TYPE> listReportType = new List<SAR_REPORT_TYPE>();
        private List<SAR_USER_REPORT_TYPE> listUserReportType = new List<SAR_USER_REPORT_TYPE>();
        private List<SAR_REPORT_TYPE_GROUP> ReportTypeGroupSelecteds;
        private List<SAR_REPORT_TYPE_GROUP> ReportTypeGroupData;
        private HIS.UC.ReportType.ReportTypeADO currentCopyReportTypeAdo;
        private HIS.UC.Account.AccountADO currentCopyAccountAdo;

        private int rowCountUser = 0;
        private int dataTotalUser = 0;
        private int startUser = 0;
        private int limitUser = 0;

        public UCSarUserReportTypeList(Inventec.Desktop.Common.Modules.Module moduleData)
        {
            InitializeComponent();
            try
            {
                HIS.Desktop.Plugins.SarUserReportTypeList.Load.Init();
                ApiConsumers.SetConsunmer((Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.Init(new CommonParam())).TokenCode);
                ConfigApplicationWorker.Init();
                InitUser();
                InitReport();
                LoadComboStatus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCSarUserReportTypeList_Load(object sender, EventArgs e)
        {
            try
            {
                LoadDataUserAdo();
                FillDataUser();
                FillDataReportType();
                SetCaptionByLanguageKey();
                LoadCboReportTypeGroup();
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
                List<ACS_USER> acsUser = GlobalVariables.ACS_USER_DATAs;

                List<V_HIS_EMPLOYEE> employee = new List<V_HIS_EMPLOYEE>();
                try
                {
                    Inventec.Core.CommonParam paramCommon = new Inventec.Core.CommonParam();
                    HisEmployeeFilter emFilter = new HisEmployeeFilter();
                    emFilter.IS_ACTIVE = 1;
                    employee = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<V_HIS_EMPLOYEE>>("api/HisEmployee/GetView", ApiConsumers.MosConsumer, emFilter, paramCommon);
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }

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
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitUser()
        {
            try
            {
                userProcessor = new AccountProcessor();
                AccountInitADO ado = new AccountInitADO();
                ado.ListAccountColumn = new List<AccountColumn>();
                ado.gridViewAccount_MouseDownAccount = gridViewUser_MouseDownUser;
                ado.btn_Radio_Enable_Click1 = btn_Radio_Enable_Click_USER;
                ado.GridView_MouseRightClick = AccountGridView_MouseRightClick;
                ado.AccountGrid_CustomUnboundColumnData = gridViewUser_CustomUnboundColumnData;

                AccountColumn colRadio2 = new AccountColumn("   ", "radio2", 30, true);
                colRadio2.VisibleIndex = 0;
                colRadio2.Visible = false;
                colRadio2.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListAccountColumn.Add(colRadio2);

                AccountColumn colCheck2 = new AccountColumn("   ", "check2", 30, true);
                colCheck2.VisibleIndex = 1;
                colCheck2.Visible = false;
                colCheck2.image = img.Images[1];
                colCheck2.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListAccountColumn.Add(colCheck2);

                AccountColumn colTenDangNhap = new AccountColumn("Tên đăng nhập", "LOGINNAME", 120, false);
                colTenDangNhap.VisibleIndex = 2;
                ado.ListAccountColumn.Add(colTenDangNhap);

                AccountColumn colHoTen = new AccountColumn("Họ tên", "USERNAME", 300, false);
                colHoTen.VisibleIndex = 3;
                ado.ListAccountColumn.Add(colHoTen);

                AccountColumn colNgaySinh = new AccountColumn("Ngày sinh", "DOB_STR", 150, false);
                colNgaySinh.VisibleIndex = 4;
                colNgaySinh.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListAccountColumn.Add(colNgaySinh);

                AccountColumn colSDT = new AccountColumn("Số điện thoại", "MOBILE", 150, false);
                colSDT.VisibleIndex = 5;
                ado.ListAccountColumn.Add(colSDT);

                AccountColumn colEmail = new AccountColumn("Email", "EMAIL", 200, false);
                colEmail.VisibleIndex = 6;
                ado.ListAccountColumn.Add(colEmail);

                AccountColumn colCCHN = new AccountColumn("Chứng chỉ hành nghề", "DIPLOMA", 100, false);
                colCCHN.VisibleIndex = 7;
                ado.ListAccountColumn.Add(colCCHN);

                AccountColumn colKhoaPhong = new AccountColumn("Khoa phòng", "DEPARTMENT_NAME", 200, false);
                colKhoaPhong.VisibleIndex = 8;
                colKhoaPhong.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListAccountColumn.Add(colKhoaPhong);

                this.ucUser = (UserControl)userProcessor.Run(ado);
                if (ucUser != null)
                {
                    this.pnlUser.Controls.Add(this.ucUser);
                    this.ucUser.Dock = DockStyle.Fill;
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

        private void InitReport()
        {
            try
            {
                reportProcessor = new UCReportTypeProcessor();
                ReportTypeInitADO ado = new ReportTypeInitADO();
                ado.ListReportTypeColumn = new List<ReportTypeColumn>();
                ado.gridViewReportType_MouseDownReportType = gridViewReport_MouseDownReport;
                ado.btn_Radio_Enable_Click1 = btn_Radio_Enable_Click_REPORT;
                ado.GridView_MouseRightClick = ReportTypeGridView_MouseRightClick;

                ReportTypeColumn colRadio2 = new ReportTypeColumn("   ", "radioReport", 30, true);
                colRadio2.VisibleIndex = 0;
                colRadio2.Visible = false;
                colRadio2.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListReportTypeColumn.Add(colRadio2);

                ReportTypeColumn colCheck2 = new ReportTypeColumn("   ", "checkReport", 30, true);
                colCheck2.VisibleIndex = 1;
                colCheck2.Visible = false;
                colCheck2.image = img.Images[1];
                colCheck2.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListReportTypeColumn.Add(colCheck2);

                ReportTypeColumn colTenDangNhap = new ReportTypeColumn("Mã báo cáo", "REPORT_TYPE_CODE", 120, false);
                colTenDangNhap.VisibleIndex = 2;
                ado.ListReportTypeColumn.Add(colTenDangNhap);

                ReportTypeColumn colHoTen = new ReportTypeColumn("Tên báo cáo", "REPORT_TYPE_NAME", 300, false);
                colHoTen.VisibleIndex = 3;
                ado.ListReportTypeColumn.Add(colHoTen);

                this.ucReport = (UserControl)reportProcessor.Run(ado);
                if (ucReport != null)
                {
                    this.pnlReportType.Controls.Add(this.ucReport);
                    this.ucReport.Dock = DockStyle.Fill;
                }
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
                status.Add(new Status(2, "Loại báo cáo"));

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("statusName", "", 300, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("statusName", "id", columnInfos, false, 350);
                ControlEditorLoader.Load(cboSTT, status, controlEditorADO);
                cboSTT.EditValue = status[0].id;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataUser()
        {
            try
            {
                int numPageSize = 0;
                if (ucPaging1.pagingGrid != null)
                {
                    numPageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = (int)ConfigApplications.NumPageSize;
                }
                FillDataToGridUser(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCountUser;
                param.Count = dataTotalUser;
                ucPaging1.Init(FillDataToGridUser, param, numPageSize);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridUser(object param)
        {
            try
            {
                WaitingManager.Show();
                isChoseReport = (long)cboSTT.EditValue;

                startUser = ((CommonParam)param).Start ?? 0;
                limitUser = ((CommonParam)param).Limit ?? 0;
                var query = UserAdoTotal.AsQueryable();
                string keyword = txtKeyWord1.Text.Trim();
                //keyword = Inventec.Common.String.Convert.UnSignVNese(keyword.Trim().ToLower());
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
                query = query.OrderByDescending(o => o.check2).ThenBy(o => o.LOGINNAME);
                dataTotalUser = query.Count();
                this.listUser = query.Skip(startUser).Take(limitUser).ToList();
                rowCountUser = (listUser == null ? 0 : listUser.Count);

                foreach (var item in listUser)
                {
                    item.isKeyChoose1 = (isChoseReport == 1);
                    item.check2 = ((listUserReportType != null && listUserReportType.Count > 0) ? listUserReportType.Exists(e => e.LOGINNAME == item.LOGINNAME) : false);
                }

                userProcessor.Reload(ucUser, listUser);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataReportType()
        {
            try
            {
                int numPageSize = 0;
                if (ucPaging2.pagingGrid != null)
                {
                    numPageSize = ucPaging2.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = (int)ConfigApplications.NumPageSize;
                }
                FillDataToGridReport(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging2.Init(FillDataToGridReport, param, numPageSize, (GridControl)reportProcessor.GetGridControl(this.ucReport));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridReport(object data)
        {
            try
            {
                WaitingManager.Show();
                listReportType = new List<SAR_REPORT_TYPE>();
                int start = ((CommonParam)data).Start ?? 0;
                int limit = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start, limit);
                SAR.Filter.SarReportTypeFilter metyFilter = new SAR.Filter.SarReportTypeFilter();
                metyFilter.ORDER_FIELD = "REPORT_TYPE_NAME";
                metyFilter.ORDER_DIRECTION = "ASC";

                if (!string.IsNullOrWhiteSpace(txtKeyWord2.Text))
                {
                    metyFilter.KEY_WORD = txtKeyWord2.Text;
                }
                if (ReportTypeGroupSelecteds != null)
                {
                    metyFilter.REPORT_TYPE_GROUP_IDs = ReportTypeGroupSelecteds.Select(o => o.ID).ToList();
                }
                //else
                //{
                //    metyFilter.KEY_WORD = String.IsNullOrWhiteSpace(GlobalVariables.APPLICATION_CODE) ? null : GlobalVariables.APPLICATION_CODE.Substring(0,3);
                //}

                //if (cboRoomType.EditValue != null)
                //    RoomFillter.ROOM_TYPE_ID = (long)cboRoomType.EditValue;
                //long isChoseStock = 0;
                if ((long)cboSTT.EditValue == 2)
                {
                    isChoseUser = (long)cboSTT.EditValue;
                }
                if ((long)cboSTT.EditValue == 1)
                {
                    isChoseUser = (long)cboSTT.EditValue;
                }
                var rs = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<SAR_REPORT_TYPE>>(
                    "api/SarReportType/Get",
                    ApiConsumers.SarConsumer,
                    metyFilter,
                    param);

                ReportAdo = new List<HIS.UC.ReportType.ReportTypeADO>();
                if (rs != null && rs.Data.Count > 0)
                {
                    listReportType = rs.Data;
                    foreach (var item in listReportType)
                    {
                        HIS.UC.ReportType.ReportTypeADO metypeADO = new HIS.UC.ReportType.ReportTypeADO(item);
                        if (isChoseUser == 2)
                        {
                            metypeADO.isKeyChooseReport = true;
                            //btnCheckAll2.Enabled = false;
                        }
                        ReportAdo.Add(metypeADO);
                    }
                }
                if (listUserReportType != null && listUserReportType.Count > 0)
                {
                    foreach (var item in listUserReportType)
                    {
                        var mediStockMety = ReportAdo.FirstOrDefault(o => o.ID == item.REPORT_TYPE_ID);
                        if (mediStockMety != null)
                        {
                            mediStockMety.checkReport = true;
                        }
                    }
                }
                ReportAdo = ReportAdo.OrderByDescending(p => p.checkReport).ToList();
                if (ucReport != null)
                {
                    reportProcessor.Reload(ucReport, ReportAdo);
                }
                rowCount = (data == null ? 0 : ReportAdo.Count);
                dataTotal = (rs.Param == null ? 0 : rs.Param.Count ?? 0);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btn_Radio_Enable_Click_USER(ACS_USER data)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                SarUserReportTypeFilter filter = new SarUserReportTypeFilter();
                filter.LOGINNAME = data.LOGINNAME;
                checkUser = data.LOGINNAME;
                listUserReportType = new List<SAR_USER_REPORT_TYPE>();
                listUserReportType = new BackendAdapter(param).Get<List<SAR_USER_REPORT_TYPE>>(
                                "api/SarUserReportType/Get",
                                ApiConsumers.SarConsumer,
                                filter,
                                param);
                var reportTypeIds = listUserReportType.Select(o => o.REPORT_TYPE_ID).Distinct().ToList();
                List<SAR_REPORT_TYPE> listReportTypeChecked = new List<SAR_REPORT_TYPE>();
                if (reportTypeIds.Count > 0)
                {
                    var skip = 0;
                    while (reportTypeIds.Count - skip > 0)
                    {
                        var lists = reportTypeIds.Skip(skip).Take(MAX_REQUEST_LENGTH_PARAM).ToList();
                        skip = skip + MAX_REQUEST_LENGTH_PARAM;
                        SarReportTypeFilter SarReportTypefilter = new SarReportTypeFilter();
                        SarReportTypefilter.IS_ACTIVE = 1;
                        SarReportTypefilter.IDs = lists;
                        var reportTypeCheckSub = new BackendAdapter(param).Get<List<SAR_REPORT_TYPE>>("api/SarReportType/Get", ApiConsumers.SarConsumer, SarReportTypefilter, param);
                        if (reportTypeCheckSub != null)
                        {
                            listReportTypeChecked.AddRange(reportTypeCheckSub);
                        }
                    }
                }
                var listView = listReportTypeChecked.Take(listReportType.Count).ToList();
                if (listView.Count < listReportType.Count)
                {
                    listView.AddRange(listReportType.Where(o => !listView.Exists(p => p.ID == o.ID)).Take(listReportType.Count - listView.Count).ToList());
                }
                List<HIS.UC.ReportType.ReportTypeADO> dataNew = new List<HIS.UC.ReportType.ReportTypeADO>();
                dataNew = (from r in listView select new HIS.UC.ReportType.ReportTypeADO(r)).ToList();
                if (listUserReportType != null && listUserReportType.Count > 0)
                {
                    foreach (var item in listUserReportType)
                    {
                        var userReportType = dataNew.FirstOrDefault(o => o.ID == item.REPORT_TYPE_ID);
                        if (userReportType != null)
                        {
                            userReportType.checkReport = true;
                        }
                    }

                    dataNew = dataNew.OrderByDescending(p => p.checkReport).ToList();
                    if (ucReport != null)
                    {
                        reportProcessor.Reload(ucReport, dataNew);
                    }
                }
                else
                {
                    FillDataReportType();
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

        private void btn_Radio_Enable_Click_REPORT(SAR_REPORT_TYPE data)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                SarUserReportTypeFilter filter = new SarUserReportTypeFilter();
                filter.REPORT_TYPE_ID = data.ID;

                checkReport = data.ID;
                //filter.
                listUserReportType = new List<SAR_USER_REPORT_TYPE>();
                listUserReportType = new BackendAdapter(param).Get<List<SAR_USER_REPORT_TYPE>>(
                                "api/SarUserReportType/Get",
                                ApiConsumers.SarConsumer,
                                filter,
                                param);
                var loginnames = listUserReportType.Select(o => o.LOGINNAME).Distinct().ToList();
                List<AccountADO> listUserChecked = new List<AccountADO>();
                if (loginnames.Count > 0)
                {
                    listUserChecked = this.UserAdoTotal.Where(o => loginnames.Contains(o.LOGINNAME)).ToList();
                }

                var listView = listUserChecked.Take(listUser.Count).ToList();
                if (listView.Count < listUser.Count)
                {
                    listView.AddRange(listUser.Where(o => !listView.Exists(p => p.LOGINNAME == o.LOGINNAME)).Take(listUser.Count - listView.Count).ToList());
                }

                if (listUserReportType != null && listUserReportType.Count > 0)
                {
                    foreach (var itemStock in listUserReportType)
                    {
                        var check = listView.FirstOrDefault(o => o.LOGINNAME == itemStock.LOGINNAME);
                        if (check != null)
                        {
                            check.check2 = true;
                        }
                    }

                    listView = listView.OrderByDescending(p => p.check2).ToList();
                    if (ucUser != null)
                    {
                        userProcessor.Reload(ucUser, listView);
                    }
                }
                else
                {
                    FillDataUser();
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

        private void gridViewReport_MouseDownReport(object sender, MouseEventArgs e)
        {
            try
            {
                if (isChoseReport == 2)
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
                        if (hi.Column.FieldName == "checkReport")
                        {
                            var lstCheckAll = ReportAdo;
                            List<HIS.UC.ReportType.ReportTypeADO> lstChecks = new List<HIS.UC.ReportType.ReportTypeADO>();

                            if (lstCheckAll != null && lstCheckAll.Count > 0)
                            {
                                var roomCheckedNum = ReportAdo.Where(o => o.checkReport == true).Count();
                                var roomNum = ReportAdo.Count();
                                if ((roomCheckedNum > 0 && roomCheckedNum < roomNum) || roomCheckedNum == 0)
                                {
                                    isCheckAll = true;
                                    hi.Column.Image = img.Images[0];
                                }

                                if (roomCheckedNum == roomNum)
                                {
                                    isCheckAll = false;
                                    hi.Column.Image = img.Images[1];
                                }
                                if (isCheckAll)
                                {
                                    foreach (var item in lstCheckAll)
                                    {
                                        if (item.ID != null)
                                        {
                                            item.checkReport = true;
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
                                            item.checkReport = false;
                                            lstChecks.Add(item);
                                        }
                                        else
                                        {
                                            lstChecks.Add(item);
                                        }
                                    }
                                    isCheckAll = true;
                                }

                                reportProcessor.Reload(ucReport, lstChecks);
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

        private void gridViewUser_MouseDownUser(object sender, MouseEventArgs e)
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
                        if (hi.Column.FieldName == "check2")
                        {
                            if (listUser != null && listUser.Count > 0)
                            {
                                var roomCheckedNum = listUser.Where(o => o.check2 == true).Count();
                                var roomNum = listUser.Count();
                                if ((roomCheckedNum > 0 && roomCheckedNum < roomNum) || roomCheckedNum == 0)
                                {
                                    isCheckAll = true;
                                    hi.Column.Image = img.Images[0];
                                }

                                if (roomCheckedNum == roomNum)
                                {
                                    isCheckAll = false;
                                    hi.Column.Image = img.Images[1];
                                }

                                if (isCheckAll)
                                {
                                    foreach (var item in listUser)
                                    {
                                        item.check2 = true;
                                    }

                                    isCheckAll = false;
                                }
                                else
                                {
                                    foreach (var item in listUser)
                                    {
                                        item.check2 = false;
                                    }
                                    isCheckAll = true;
                                }

                                userProcessor.Reload(ucUser, listUser);
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

        private void txtKeyWord1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch1.Focus();
                    btnSearch1_Click(null, null);
                }
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
                FillDataUser();
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
                FillDataReportType();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeyWord2_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch2.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboSTT_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                checkRa = false;
                isChoseReport = 0;
                isChoseUser = 0;
                listUserReportType = new List<SAR_USER_REPORT_TYPE>();
                FillDataUser();
                FillDataReportType();
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (ucReport != null && ucUser != null)
                {
                    bool success = false;
                    CommonParam param = new CommonParam();
                    List<HIS.UC.Account.AccountADO> userAcs = userProcessor.GetDataGridView(ucUser) as List<HIS.UC.Account.AccountADO>;
                    List<HIS.UC.ReportType.ReportTypeADO> reportSar = reportProcessor.GetDataGridView(ucReport) as List<HIS.UC.ReportType.ReportTypeADO>;
                    if (isChoseUser == 1 && reportSar != null && reportSar.Count > 0)
                    {
                        if (reportSar != null && reportSar.Count > 0)
                        {
                            if (checkRa == true)
                            {
                                var dataCheckeds = reportSar.Where(p => (p.checkReport == true)).ToList();

                                //List xóa
                                var dataDeletes = reportSar.Where(o => listUserReportType.Select(p => p.REPORT_TYPE_ID)
                               .Contains(o.ID) && o.checkReport == false).ToList();

                                //list them
                                var dataCreates = dataCheckeds.Where(o => !listUserReportType.Select(p => p.REPORT_TYPE_ID)
                                    .Contains(o.ID)).ToList();

                                if (dataCheckeds.Count == 0 && dataDeletes.Count == 0)
                                {
                                    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn Loại báo cáo", "Thông báo");
                                    return;
                                }
                                if (dataDeletes != null && dataDeletes.Count > 0)// && dataDeletes.Count < 15)
                                {
                                    List<long> deleteIds = listUserReportType.Where(o => dataDeletes.Select(p => p.ID)
                                        .Contains(o.REPORT_TYPE_ID)).Select(o => o.ID).ToList();
                                    bool deleteResult = new BackendAdapter(param).Post<bool>(
                                              "api/SarUserReportType/DeleteList",
                                              ApiConsumers.SarConsumer,
                                              deleteIds,
                                              param);
                                    if (deleteResult)
                                    {
                                        listUserReportType = listUserReportType.Where(o => !deleteIds.Contains(o.ID)).ToList();
                                        success = true;
                                    }
                                }

                                if (dataCreates != null && dataCreates.Count > 0)
                                {
                                    List<SAR_USER_REPORT_TYPE> userReportCreates = new List<SAR_USER_REPORT_TYPE>();
                                    foreach (var item in dataCreates)
                                    {
                                        SAR_USER_REPORT_TYPE stockMety = new SAR_USER_REPORT_TYPE();
                                        stockMety.REPORT_TYPE_ID = item.ID;
                                        stockMety.LOGINNAME = checkUser;
                                        userReportCreates.Add(stockMety);
                                    }

                                    var createResult = new BackendAdapter(param).Post<List<SAR_USER_REPORT_TYPE>>(
                                               "api/SarUserReportType/CreateList",
                                               ApiConsumers.SarConsumer,
                                               userReportCreates,
                                               param);
                                    if (createResult != null && createResult.Count > 0)
                                    {
                                        listUserReportType.AddRange(createResult);
                                        success = true;
                                    }
                                }
                                else
                                {
                                    success = true;
                                }
                                WaitingManager.Hide();
                                #region Show message
                                MessageManager.Show(this.ParentForm, param, success);
                                #endregion

                                #region Process has exception
                                SessionManager.ProcessTokenLost(param);
                                #endregion
                                reportSar = reportSar.OrderByDescending(p => p.checkReport).ToList();
                                reportProcessor.Reload(ucReport, reportSar);
                            }
                            else
                            {
                                DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn Tài khoản", "Thông báo");
                            }
                        }
                    }

                    if (isChoseReport == 2 && userAcs != null && userAcs.Count > 0)
                    {
                        if (checkRa == true)
                        {
                            HIS.UC.ReportType.ReportTypeADO reportType = reportSar.FirstOrDefault(o => o.ID == checkReport);

                            var dataCheckeds = userAcs.Where(p => (p.check2 == true)).ToList();

                            //List xóa
                            var dataDeletes = userAcs.Where(o => listUserReportType.Select(p => p.LOGINNAME)
                           .Contains(o.LOGINNAME) && o.check2 == false).ToList();

                            //list them
                            var dataCreates = dataCheckeds.Where(o => !listUserReportType.Select(p => p.LOGINNAME)
                                .Contains(o.LOGINNAME)).ToList();

                            if (dataCheckeds.Count == 0 && dataDeletes.Count == 0)
                            {
                                DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn tài khoản", "Thông báo");
                                return;
                            }
                            if (dataDeletes != null && dataDeletes.Count > 0)// && dataDeletes.Count < 5)
                            {
                                List<long> deleteIds = listUserReportType.Where(o => dataDeletes.Select(p => p.LOGINNAME)
                                    .Contains(o.LOGINNAME)).Select(o => o.ID).ToList();
                                bool deleteResult = new BackendAdapter(param).Post<bool>(
                                          "api/SarUserReportType/DeleteList",
                                          ApiConsumers.SarConsumer,
                                          deleteIds,
                                          param);
                                if (deleteResult)
                                {
                                    listUserReportType = listUserReportType.Where(o => !deleteIds.Contains(o.ID)).ToList();
                                    success = true;
                                }
                            }

                            if (dataCreates != null && dataCreates.Count > 0 && reportType != null)
                            {
                                List<SAR_USER_REPORT_TYPE> metyStockCreates = new List<SAR_USER_REPORT_TYPE>();
                                foreach (var item in dataCreates)
                                {
                                    SAR_USER_REPORT_TYPE metyStock = new SAR_USER_REPORT_TYPE();
                                    metyStock.LOGINNAME = item.LOGINNAME;
                                    metyStock.REPORT_TYPE_ID = checkReport;
                                    metyStockCreates.Add(metyStock);
                                }

                                var createResult = new BackendAdapter(param).Post<List<SAR_USER_REPORT_TYPE>>(
                                           "api/SarUserReportType/CreateList",
                                           ApiConsumers.SarConsumer,
                                           metyStockCreates,
                                           param);
                                if (createResult != null && createResult.Count > 0)
                                {
                                    listUserReportType.AddRange(createResult);
                                    success = true;
                                }
                            }
                            else
                            {
                                success = true;
                            }
                            WaitingManager.Hide();
                            #region Show message
                            MessageManager.Show(this.ParentForm, param, success);
                            #endregion

                            #region Process has exception
                            SessionManager.ProcessTokenLost(param);
                            #endregion
                            userAcs = userAcs.OrderByDescending(p => p.check2).ToList();
                            userProcessor.Reload(ucUser, userAcs);
                        }
                        else
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn Loại báo cáo", "Thông báo");
                        }
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.SarUserReportTypeList.Resources.Lang", typeof(HIS.Desktop.Plugins.SarUserReportTypeList.UCSarUserReportTypeList).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCSarUserReportTypeList.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("UCSarUserReportTypeList.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboSTT.Properties.NullText = Inventec.Common.Resource.Get.Value("UCSarUserReportTypeList.cboSTT.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("UCSarUserReportTypeList.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch1.Text = Inventec.Common.Resource.Get.Value("UCSarUserReportTypeList.btnSearch1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyWord1.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCSarUserReportTypeList.txtKeyWord1.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UCSarUserReportTypeList.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch2.Text = Inventec.Common.Resource.Get.Value("UCSarUserReportTypeList.btnSearch2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyWord2.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCSarUserReportTypeList.txtKeyWord2.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lkSTT.Text = Inventec.Common.Resource.Get.Value("UCSarUserReportTypeList.lkSTT.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.LcReportTypeGroup.Text = Inventec.Common.Resource.Get.Value("UCSarUserReportTypeList.LcReportTypeGroup.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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
                                if (isChoseUser != 1)
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
                                if (this.currentCopyAccountAdo == null && isChoseUser != 1)
                                {
                                    MessageManager.Show("Vui lòng copy!");
                                    break;
                                }
                                if (this.currentCopyAccountAdo != null && currentPaste != null && isChoseUser == 1)
                                {
                                    if (this.currentCopyAccountAdo.ID == currentPaste.ID)
                                    {
                                        MessageManager.Show("Trùng dữ liệu copy và paste");
                                        break;
                                    }
                                    SarUserReportTypeCopyByUserSDO hisMestMatyCopyByMatySDO = new SarUserReportTypeCopyByUserSDO();
                                    hisMestMatyCopyByMatySDO.CopyUserLoginname = this.currentCopyAccountAdo.LOGINNAME;
                                    hisMestMatyCopyByMatySDO.PasteUserLoginname = currentPaste.LOGINNAME;
                                    var result = new BackendAdapter(param).Post<List<SAR_USER_REPORT_TYPE>>("api/SarUserReportType/CopyByUser", ApiConsumers.SarConsumer, hisMestMatyCopyByMatySDO, param);
                                    if (result != null)
                                    {
                                        success = true;

                                        listUserReportType = result;
                                        List<HIS.UC.ReportType.ReportTypeADO> dataNew = new List<HIS.UC.ReportType.ReportTypeADO>();
                                        dataNew = (from r in listReportType select new HIS.UC.ReportType.ReportTypeADO(r)).ToList();
                                        if (listUserReportType != null && listUserReportType.Count > 0)
                                        {
                                            foreach (var item in listUserReportType)
                                            {
                                                var userReportType = dataNew.FirstOrDefault(o => o.ID == item.REPORT_TYPE_ID);
                                                if (userReportType != null)
                                                {
                                                    userReportType.checkReport = true;
                                                }
                                            }

                                            dataNew = dataNew.OrderByDescending(p => p.checkReport).ToList();
                                            if (ucReport != null)
                                            {
                                                reportProcessor.Reload(ucReport, dataNew);
                                            }
                                        }
                                        else
                                        {
                                            FillDataReportType();
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

        private void ReportTypeGridView_MouseRightClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if ((e.Item is BarButtonItem) && sender != null && sender is HIS.UC.ReportType.ReportTypeADO)
                {
                    var type = (HIS.UC.ReportType.Popup.PopupMenuProcessor.ItemType)e.Item.Tag;
                    switch (type)
                    {
                        case HIS.UC.ReportType.Popup.PopupMenuProcessor.ItemType.Copy:
                            {
                                if (isChoseReport != 2)
                                {
                                    MessageManager.Show("Vui lòng chọn loại báo cáo!");
                                    break;
                                }
                                this.currentCopyReportTypeAdo = (HIS.UC.ReportType.ReportTypeADO)sender;
                                break;
                            }
                        case HIS.UC.ReportType.Popup.PopupMenuProcessor.ItemType.Paste:
                            {
                                var currentPaste = (HIS.UC.ReportType.ReportTypeADO)sender;
                                bool success = false;
                                CommonParam param = new CommonParam();
                                if (this.currentCopyReportTypeAdo == null && isChoseReport != 2)
                                {
                                    MessageManager.Show("Vui lòng copy!");
                                    break;
                                }
                                if (this.currentCopyReportTypeAdo != null && currentPaste != null && isChoseReport == 2)
                                {
                                    if (this.currentCopyReportTypeAdo.ID == currentPaste.ID)
                                    {
                                        MessageManager.Show("Trùng dữ liệu copy và paste");
                                        break;
                                    }
                                    SarUserReportTypeCopyByReportTypeSDO hisMestMatyCopyByMatySDO = new SarUserReportTypeCopyByReportTypeSDO();
                                    hisMestMatyCopyByMatySDO.CopyReportTypeId = this.currentCopyReportTypeAdo.ID;
                                    hisMestMatyCopyByMatySDO.PasteReportTypeId = currentPaste.ID;
                                    var result = new BackendAdapter(param).Post<List<SAR_USER_REPORT_TYPE>>("api/SarUserReportType/CopyByReportType", ApiConsumers.SarConsumer, hisMestMatyCopyByMatySDO, param);
                                    if (result != null && result.Count > 0)
                                    {
                                        success = true;
                                        listUserReportType = result;

                                        if (listUser != null && listUser.Count > 0)
                                        {
                                            foreach (var itemStock in listUser)
                                            {
                                                var check = listUserReportType.FirstOrDefault(o => o.LOGINNAME == itemStock.LOGINNAME);
                                                if (check != null)
                                                {
                                                    itemStock.check2 = true;
                                                }
                                            }

                                            listUser = listUser.OrderByDescending(p => p.check2).ToList();
                                            if (ucUser != null)
                                            {
                                                userProcessor.Reload(ucUser, listUser);
                                            }
                                        }
                                        else
                                        {
                                            FillDataUser();
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

        #region ---Load data combox ReportTypeGroup
        private void LoadCboReportTypeGroup()
        {
            try
            {
                InitReportTypeGroupCheck();

                CommonParam param = new CommonParam();
                SarReportTypeGroupFilter SarReportTypeGroupfilter = new SarReportTypeGroupFilter();
                SarReportTypeGroupfilter.IS_ACTIVE = 1;
                this.ReportTypeGroupData = new BackendAdapter(param).Get<List<SAR_REPORT_TYPE_GROUP>>("api/SarReportTypeGroup/Get", ApiConsumers.SarConsumer, SarReportTypeGroupfilter, param);

                InitReportTypeGroupFilter(this.ReportTypeGroupData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitReportTypeGroupCheck()
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cboReportTypeGroup.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(SelectionGrid__ReportTypeGroupSelectFilter);
                cboReportTypeGroup.Properties.Tag = gridCheck;
                cboReportTypeGroup.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cboReportTypeGroup.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboReportTypeGroup.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SelectionGrid__ReportTypeGroupSelectFilter(object sender, EventArgs e)
        {
            try
            {
                ReportTypeGroupSelecteds = new List<SAR_REPORT_TYPE_GROUP>();
                foreach (SAR_REPORT_TYPE_GROUP rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        ReportTypeGroupSelecteds.Add(rv);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitReportTypeGroupFilter(List<SAR_REPORT_TYPE_GROUP> datas)
        {
            try
            {
                if (datas != null)
                {
                    cboReportTypeGroup.Properties.DataSource = datas;
                    cboReportTypeGroup.Properties.DisplayMember = "REPORT_TYPE_GROUP_NAME";
                    cboReportTypeGroup.Properties.ValueMember = "ID";
                    DevExpress.XtraGrid.Columns.GridColumn col2 = cboReportTypeGroup.Properties.View.Columns.AddField("REPORT_TYPE_GROUP_NAME");
                    col2.VisibleIndex = 1;
                    col2.Width = 200;
                    col2.Caption = "";
                    cboReportTypeGroup.Properties.PopupFormWidth = 200;
                    cboReportTypeGroup.Properties.View.OptionsView.ShowColumnHeaders = false;
                    cboReportTypeGroup.Properties.View.OptionsSelection.MultiSelect = true;
                    GridCheckMarksSelection gridCheckMark = cboReportTypeGroup.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        gridCheckMark.ClearSelection(cboReportTypeGroup.Properties.View);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboReportTypeGroup_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null) return;
                foreach (SAR_REPORT_TYPE_GROUP rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(", "); }
                    sb.Append(rv.REPORT_TYPE_GROUP_NAME.ToString());
                }
                e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboReportTypeGroup_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    btnSearch2.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboReportTypeGroup_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                    if (gridCheckMark == null) return;
                    gridCheckMark.ClearSelection(cboReportTypeGroup.Properties.View);
                    cboReportTypeGroup.Focus();
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
