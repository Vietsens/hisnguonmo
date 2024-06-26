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
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.ExpMestSaleTransactionList.frmControl;
using HIS.Desktop.Plugins.Library.ElectronicBill;
using HIS.Desktop.Plugins.Library.ElectronicBill.Base;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExpMestSaleTransactionList
{
    public partial class frmExpMestSaleTransactionList : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module currentModule;
        long _cashierRoomId;

        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;

        List<HIS_TRANSACTION_TYPE> _TypeSelecteds;

        long _currentCashierRoomId = 0;

        string loginName = "";

        public frmExpMestSaleTransactionList()
        {
            InitializeComponent();
        }

        public frmExpMestSaleTransactionList(Inventec.Desktop.Common.Modules.Module module, ExpMestSaleTranADO iputAdo)
            : base(module)
        {
            InitializeComponent();
            try
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("Dau vao tu Thanh toan nha thuoc", iputAdo));
                this.currentModule = module;
                this.txtTreatmentCode.Text = iputAdo.TreatmentCode;
                this._cashierRoomId = iputAdo.CashierRoomId ?? 0;
                if (iputAdo.NumOrder != null && iputAdo.NumOrder > 0)
                    this.txtNumOrder.Text = iputAdo.NumOrder.ToString();

                this.txtExpMestCode.Text = iputAdo.ExpMestCode;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public frmExpMestSaleTransactionList(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
            try
            {
                this.currentModule = module;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmExpMestSaleTransactionList_Load(object sender, EventArgs e)
        {
            try
            {
                SetIcon();
                _TypeSelecteds = new List<HIS_TRANSACTION_TYPE>();
                Base.ResourceLangManager.InitResourceLanguageManager();
                dtCreateTimeFrom.DateTime = DateTime.Now;
                dtCreateTimeTo.DateTime = DateTime.Now;
                LoadComboType();
                LoadCashierRoom();

                LoadDataGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon
                  (System.IO.Path.Combine
                  (LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory,
                  System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));

                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadComboType()
        {
            try
            {
                List<HIS_TRANSACTION_TYPE> _listData = new List<HIS_TRANSACTION_TYPE>();

                _listData = BackendDataWorker.Get<HIS_TRANSACTION_TYPE>().Where(p => p.IS_ACTIVE == 1).ToList();

                HIS_TRANSACTION_TYPE ado = new HIS_TRANSACTION_TYPE();
                ado.ID = -1;
                ado.TRANSACTION_TYPE_NAME = "Phiếu xuất";
                ado.TRANSACTION_TYPE_CODE = "-1";
                _listData.Add(ado);

                InitCheck(cboType, SelectionGrid__Type);
                InitCombo(cboType, _listData, "TRANSACTION_TYPE_NAME", "ID");//gan data vao 

                ResetComboType(cboType);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ResetComboType(GridLookUpEdit cbo)
        {
            try
            {
                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cbo.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void LoadCashierRoom()
        {
            try
            {
                cboCashierRoom.EditValue = null;

                List<V_HIS_CASHIER_ROOM> _listData = new List<V_HIS_CASHIER_ROOM>();
                this.loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                var rooms = BackendDataWorker.Get<V_HIS_USER_ROOM>().Where(p => p.LOGINNAME == loginName && p.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__TN).ToList();
                if (rooms != null && rooms.Count > 0)
                {
                    _listData = BackendDataWorker.Get<V_HIS_CASHIER_ROOM>().Where(p => p.IS_ACTIVE == 1 && rooms.Select(o => o.ROOM_ID).Distinct().Contains(p.ROOM_ID)).ToList();
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("CASHIER_ROOM_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("CASHIER_ROOM_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("CASHIER_ROOM_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboCashierRoom, _listData, controlEditorADO);

                if (_cashierRoomId > 0)
                {
                    cboCashierRoom.EditValue = _cashierRoomId;
                }
                else
                {
                    var room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(p => p.ID == this.currentModule.RoomId);
                    if (room != null)
                    {
                        if (room.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__TN)
                        {
                            var dataDefault = _listData.FirstOrDefault(p => p.ROOM_ID == room.ID);
                            if (dataDefault != null)
                            {
                                cboCashierRoom.EditValue = dataDefault.ID;
                            }
                        }
                        else
                        {
                            var dataDefault = _listData.FirstOrDefault(p => p.DEPARTMENT_ID == room.DEPARTMENT_ID);
                            if (dataDefault != null)
                            {
                                cboCashierRoom.EditValue = dataDefault.ID;
                            }
                        }
                    }
                }
                if (cboCashierRoom.EditValue != null)
                {
                    this._currentCashierRoomId = (long)cboCashierRoom.EditValue;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitCombo(GridLookUpEdit cbo, object data, string DisplayValue, string ValueMember)
        {
            try
            {
                cbo.Properties.DataSource = data;
                cbo.Properties.DisplayMember = DisplayValue;
                cbo.Properties.ValueMember = ValueMember;
                DevExpress.XtraGrid.Columns.GridColumn col2 = cbo.Properties.View.Columns.AddField(DisplayValue);

                col2.VisibleIndex = 1;
                col2.Width = 200;
                col2.Caption = "Tất cả";
                cbo.Properties.PopupFormWidth = 200;
                cbo.Properties.View.OptionsView.ShowColumnHeaders = true;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;
                cbo.Properties.View.OptionsSelection.ShowCheckBoxSelectorInColumnHeader = DevExpress.Utils.DefaultBoolean.True;

                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.SelectAll(cbo.Properties.DataSource);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void InitCheck(GridLookUpEdit cbo, GridCheckMarksSelection.SelectionChangedEventHandler eventSelect)
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cbo.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(eventSelect);
                cbo.Properties.Tag = gridCheck;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cbo.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__Type(object sender, EventArgs e)
        {
            try
            {
                _TypeSelecteds = new List<HIS_TRANSACTION_TYPE>();
                foreach (HIS_TRANSACTION_TYPE rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        _TypeSelecteds.Add(rv);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboType_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string typeName = "";
                if (_TypeSelecteds != null && _TypeSelecteds.Count > 0)
                {
                    typeName = string.Join(",", _TypeSelecteds.Select(p => p.TRANSACTION_TYPE_NAME));
                }

                e.DisplayText = typeName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboType_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtAccountBookName.Focus();
                    txtAccountBookName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataGrid()
        {
            try
            {
                WaitingManager.Show();
                int pagingSize = 0;
                if (ucPaging1.pagingGrid != null)
                {
                    pagingSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    pagingSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                GridPaging(new CommonParam(0, pagingSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(GridPaging, param, pagingSize, this.gridControlData);
                gridControlData.RefreshDataSource();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void GridPaging(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                ApiResultObject<List<DHisTransExpSDO>> apiResult = null;
                DHisTransExpFilter filter = new DHisTransExpFilter();

                SetFilter(ref filter);

                gridViewData.BeginUpdate();

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("filter:", filter));

                apiResult = new Inventec.Common.Adapter.BackendAdapter
                    (paramCommon).GetRO<List<DHisTransExpSDO>>
                    ("api/HisExpMest/PharmacyCashierGet", ApiConsumer.ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var datas = apiResult.Data;
                    if (datas != null && datas.Count > 0)
                    {
                        gridControlData.DataSource = datas;
                        rowCount = (datas == null ? 0 : datas.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                    else
                    {
                        gridControlData.DataSource = null;
                        rowCount = (datas == null ? 0 : datas.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                gridViewData.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetFilter(ref DHisTransExpFilter filter)
        {
            try
            {
                filter.KEY_WORD = txtSearch.Text.Trim();
                filter.ORDER_FIELD = "CREATE_TIME";
                filter.ORDER_DIRECTION = "DESC";
                if (!String.IsNullOrEmpty(txtExpMestCode.Text))
                {
                    string code = txtExpMestCode.Text.Trim();
                    if (code.Length < 12 && checkDigit(code))
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtExpMestCode.Text = code;
                    }
                    filter.EXP_MEST_CODE__EXACT = code;
                }
                else if (!String.IsNullOrEmpty(txtTreatmentCode.Text))
                {
                    string code = txtTreatmentCode.Text.Trim();
                    if (code.Length < 12 && checkDigit(code))
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtTreatmentCode.Text = code;
                    }
                    filter.TREATMENT_CODE__EXACT = code;
                }
                else if (!String.IsNullOrEmpty(txtAccountBookCode.Text))
                {
                    string code = txtAccountBookCode.Text.Trim();

                    filter.ACCOUNT_BOOK_CODE__EXACT = code;
                }
                else if (!string.IsNullOrEmpty(txtNumOrder.Text))
                {
                    string code = txtNumOrder.Text.Trim();
                    if (checkDigit(code))
                    {
                        filter.NUM_ORDER__EQUAL = Inventec.Common.TypeConvert.Parse.ToInt64(code);
                    }
                }
                else
                {
                    if (_TypeSelecteds != null && _TypeSelecteds.Count > 0)
                    {
                        filter.TYPE_IDs = _TypeSelecteds.Where(p => p.ID != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT).Select(o => o.ID).ToList();
                    }


                    if (!String.IsNullOrEmpty(txtAccountBookName.Text))
                    {
                        filter.ACCOUNT_BOOK_NAME = txtAccountBookName.Text.Trim();
                    }
                    if (!String.IsNullOrEmpty(txtTemplateCode.Text))
                    {
                        filter.TEMPLATE_CODE = txtTemplateCode.Text.Trim();
                    }
                    if (!String.IsNullOrEmpty(txtSymbolCode.Text))
                    {
                        filter.SYMBOL_CODE = txtSymbolCode.Text.Trim();
                    }

                    if (dtCreateTimeFrom.EditValue != null && dtCreateTimeFrom.DateTime != DateTime.MinValue)
                        filter.CREATE_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                            Convert.ToDateTime(dtCreateTimeFrom.EditValue).ToString("yyyyMMdd") + "000000");

                    if (dtCreateTimeTo.EditValue != null && dtCreateTimeTo.DateTime != DateTime.MinValue)
                        filter.CREATE_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                            Convert.ToDateTime(dtCreateTimeTo.EditValue).ToString("yyyyMMdd") + "235959");

                }

                if (cboCashierRoom.EditValue != null)
                {
                    filter.CASHIER_ROOM_ID = (long)cboCashierRoom.EditValue;
                }

                if (this.currentModule != null && this.currentModule.RoomId > 0)
                {
                    var dataMediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(p => p.ROOM_ID == this.currentModule.RoomId);
                    if (dataMediStock != null)
                    {
                        filter.MEDI_STOCK_ID = dataMediStock.ID;
                    }
                }

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("DHisTransExpFilter", filter));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool checkDigit(string s)
        {
            bool result = false;
            try
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (char.IsDigit(s[i]) == true) result = true;
                    else result = false;
                }
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return result;
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                LoadDataGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnRefesh_Click(object sender, EventArgs e)
        {
            try
            {
                ResetComboType(cboType);
                this._TypeSelecteds = null;
                cboType.EditValue = null;
                cboType.Text = "";
                txtSearch.Text = "";
                txtTreatmentCode.Text = "";
                txtAccountBookCode.Text = "";
                txtNumOrder.Text = "";
                txtExpMestCode.Text = "";
                cboCashierRoom.EditValue = this._currentCashierRoomId;
                dtCreateTimeFrom.DateTime = DateTime.Now;
                dtCreateTimeTo.DateTime = DateTime.Now;
                //cboType
                txtAccountBookName.Text = "";
                txtTemplateCode.Text = "";
                txtSymbolCode.Text = "";
                gridControlData.DataSource = null;
                LoadDataGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboCashierRoom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    dtCreateTimeFrom.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboCashierRoom_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtCreateTimeFrom.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewData_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var data = (DHisTransExpSDO)gridViewData.GetRow(e.RowHandle);
                    if (data != null)
                    {
                        if (e.Column.FieldName == "DELETE")
                        {
                            if (data.IS_CANCEL != 1 && (data.CASHIER_LOGINNAME == this.loginName
                                || HIS.Desktop.IsAdmin.CheckLoginAdmin.IsAdmin(this.loginName))
                                )
                            {
                                e.RepositoryItem = repositoryItemButton__Delete;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemButton__Delete_D;
                            }
                        }
                        else if (e.Column.FieldName == "BILL_DETAIL")
                        {
                            try
                            {
                                if (data.TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT)
                                {
                                    e.RepositoryItem = repositoryItemBtnViewBillDetail;
                                }
                                else
                                {
                                    e.RepositoryItem = repositoryItemBtnViewBillDetail_D;
                                }
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewData_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.ListSourceRowIndex >= 0 && e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (DHisTransExpSDO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            try
                            {
                                e.Value = e.ListSourceRowIndex + 1 + (ucPaging1.pagingGrid.CurrentPage - 1) * (ucPaging1.pagingGrid.PageSize);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "CASHIER")
                        {
                            try
                            {
                                e.Value = data.CASHIER_LOGINNAME + (String.IsNullOrEmpty(data.CASHIER_USERNAME) ? "" : " - " + data.CASHIER_USERNAME);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "DOB_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "THUC_THU_STR")
                        {
                            try
                            {
                                decimal? ado = data.AMOUNT - (data.KC_AMOUNT ?? 0) - (data.TDL_BILL_FUND_AMOUNT ?? 0) - (data.EXEMPTION ?? 0);
                                e.Value = ado;
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewData_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var data = (DHisTransExpSDO)gridViewData.GetRow(e.RowHandle);
                    if (data != null)
                    {
                        if (data.IS_CANCEL == 1)
                        {
                            if (e.Column.FieldName == "STT" || e.Column.FieldName == "DELETE" || e.Column.FieldName == "ChangeLock")
                                return;
                            e.Appearance.ForeColor = Color.Gray; //Giao dịch đã bị hủy => Màu nâu
                            e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font.FontFamily, e.Appearance.Font.Size, FontStyle.Italic);
                            e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Strikeout);
                            if (!String.IsNullOrWhiteSpace(data.INVOICE_CODE))//Với các giao dịch đã xuất hóa đơn điện tử (HIS_TRANSACTION có INVOICE_CODE), thì hiển thị bôi đậm
                            {
                                e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, FontStyle.Bold | FontStyle.Strikeout);
                            }
                        }
                        else
                            if (data.TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT)
                            {
                                e.Appearance.ForeColor = Color.Blue; //Giao dịch thanh toán => Màu xanh nước biển
                                if (!String.IsNullOrWhiteSpace(data.INVOICE_CODE))//Với các giao dịch đã xuất hóa đơn điện tử (HIS_TRANSACTION có INVOICE_CODE), thì hiển thị bôi đậm
                                {
                                    e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Bold);
                                }
                            }
                            else if (data.TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TU)
                            {
                                e.Appearance.ForeColor = Color.Green; //Giao dịch tạm ứng => Màu xanh lá cây
                            }
                            else if (data.TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__HU)
                            {
                                e.Appearance.ForeColor = Color.Red; //Giao dịch hoàn ứng => Màu đỏ
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        BarManager baManager = null;
        DHisTransExpSDO transactionPrint = null;
        PopupMenuProcessor popupMenuProcessor = null;

        private void gridViewData_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            try
            {
                GridHitInfo hi = e.HitInfo;
                this.transactionPrint = null;
                if (hi.InRowCell)
                {
                    this.transactionPrint = (DHisTransExpSDO)gridViewData.GetFocusedRow();
                    if (this.baManager == null)
                    {
                        this.baManager = new BarManager();
                        this.baManager.Form = this;
                    }
                    if (this.transactionPrint != null)
                    {
                        this.popupMenuProcessor = new PopupMenuProcessor(this.transactionPrint, this.baManager, MouseRightItemClick);
                        this.popupMenuProcessor.InitMenu();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButton__Delete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var dataRow = (DHisTransExpSDO)gridViewData.GetFocusedRow();
                if (dataRow != null)
                {
                    if (!string.IsNullOrEmpty(dataRow.TRANSACTION_CODE))
                    {
                        HuyBienLaiHoaDon(dataRow.TRANSACTION_CODE);
                    }
                    else if (!string.IsNullOrEmpty(dataRow.EXP_MEST_CODE))
                    {
                        DeleteExpMest(dataRow);
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void HuyBienLaiHoaDon(string transactionCode)
        {
            try
            {
                MOS.Filter.HisTransactionViewFilter filter = new HisTransactionViewFilter();
                filter.TRANSACTION_CODE__EXACT = transactionCode;

                var dataTransaction = new BackendAdapter(new CommonParam()).Get<List<V_HIS_TRANSACTION>>
             ("api/HisTransaction/GetView", ApiConsumer.ApiConsumers.MosConsumer, filter, null).FirstOrDefault();

                if (dataTransaction != null)
                {
                    //if (!this.CancelElectronicBill(dataTransaction))
                    //{
                    //    return;
                    //}

                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TransactionCancel").FirstOrDefault();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.TransactionCancel'");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        var room = BackendDataWorker.Get<V_HIS_CASHIER_ROOM>().FirstOrDefault(p => p.ID == dataTransaction.CASHIER_ROOM_ID);
                        long roomId = room != null ? room.ROOM_ID : this.currentModule.RoomId;
                        long roomTypeId = room != null ? room.ROOM_TYPE_ID : this.currentModule.RoomTypeId;
                        List<object> listArgs = new List<object>();
                        listArgs.Add(dataTransaction);
                        listArgs.Add((DelegateSelectData)ReLoadDataBeforDelete);
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, roomId, roomTypeId), listArgs);
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("moduleData", PluginInstance.GetModuleWithWorkingRoom(moduleData, roomId, roomTypeId)));
                        if (extenceInstance == null)
                        {
                            throw new ArgumentNullException("extenceInstance is null");
                        }
                        ((Form)extenceInstance).ShowDialog();
                    }
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Error("khong tim thay giao dich tuong ung voi TRANSACTION_CODE = " + transactionCode);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void DeleteExpMest(DHisTransExpSDO dataRow)
        {
            try
            {
                if (!string.IsNullOrEmpty(dataRow.EXP_MEST_CODE))
                {
                    string[] str = dataRow.EXP_MEST_CODE.Split(',');
                    List<string> dataStrs = str.ToList();

                    if (dataStrs != null && dataStrs.Count == 1)
                    {
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(
                            MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong),
                            MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            bool success = false;
                            CommonParam param = new CommonParam();

                            PharmacyCashierExpCancelSDO sdo = new PharmacyCashierExpCancelSDO();
                            sdo.ExpMestCode = dataStrs[0];

                            var room = BackendDataWorker.Get<V_HIS_CASHIER_ROOM>().FirstOrDefault(p => p.ID == dataRow.CASHIER_ROOM_ID);
                            sdo.WorkingRoomId = room != null ? room.ROOM_ID : this.currentModule.RoomId;

                            var rs = new BackendAdapter(param).Post<bool>("api/HisExpMest/PharmacyCashierExpCancel", ApiConsumers.MosConsumer, sdo, param);
                            if (rs)
                            {
                                success = true;
                                ReLoadDataBeforDelete(success);
                            }
                            MessageManager.Show(this.ParentForm, param, success);
                        }
                    }
                    else
                    {
                        frmExpMest frm = new frmExpMest(dataStrs, dataRow);
                        frm.ShowDialog();
                        this.LoadDataGrid();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //private bool CancelElectronicBill(V_HIS_TRANSACTION transaction)
        //{
        //    bool result = true;
        //    try
        //    {
        //        if (transaction != null && !String.IsNullOrEmpty(transaction.INVOICE_CODE) && !String.IsNullOrEmpty(transaction.INVOICE_SYS))
        //        {
        //            string serviceConfig = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(SdaConfigKey.ELECTRONIC_BILL__CONFIG);
        //            if (transaction.INVOICE_SYS != ProviderType.VIETTEL)
        //            {
        //                ElectronicBillDataInput dataInput = new ElectronicBillDataInput();
        //                dataInput.PartnerInvoiceID = Inventec.Common.TypeConvert.Parse.ToInt64(transaction.INVOICE_CODE);
        //                dataInput.InvoiceCode = transaction.INVOICE_CODE;
        //                dataInput.NumOrder = transaction.NUM_ORDER;
        //                dataInput.SymbolCode = transaction.SYMBOL_CODE;
        //                dataInput.TemplateCode = transaction.TEMPLATE_CODE;
        //                dataInput.TransactionTime = transaction.TRANSACTION_TIME;
        //                dataInput.Branch = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId());
        //                ElectronicBillProcessor electronicBillProcessor = new ElectronicBillProcessor(dataInput);
        //                ElectronicBillResult electronicBillResult = electronicBillProcessor.Run(ElectronicBillType.ENUM.CANCEL_INVOICE);

        //                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => electronicBillResult), electronicBillResult));
        //                if (electronicBillResult != null && !electronicBillResult.Success)
        //                {

        //                    string mes = "";
        //                    if (electronicBillResult.Messages != null && electronicBillResult.Messages.Count > 0)
        //                    {
        //                        foreach (var item in electronicBillResult.Messages)
        //                        {
        //                            mes += item + ";";
        //                        }
        //                    }

        //                    DialogResult myResult;
        //                    myResult = MessageBox.Show("Hủy hóa đơn điện tử thất bại." + mes + ". Bạn có muốn tiếp tục hủy giao dịch trên HIS?", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
        //                    if (myResult != DialogResult.OK)
        //                    {
        //                        result = false;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result = false;
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //    return result;
        //}

        private void ReLoadDataBeforDelete(object rs)
        {
            try
            {
                this.LoadDataGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTreatmentCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.LoadDataGrid();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtAccountBookCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.LoadDataGrid();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtNumOrder_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.LoadDataGrid();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtExpMestCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.LoadDataGrid();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtCreateTimeFrom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    dtCreateTimeTo.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtCreateTimeTo_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    cboType.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    txtAccountBookName.Focus();
                    txtAccountBookName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtAccountBookName_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtSymbolCode.Focus();
                    txtSymbolCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTemplateCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTemplateCode.Focus();
                    txtTemplateCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSymbolCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtnViewBillDetail_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var dataRow = (DHisTransExpSDO)gridViewData.GetFocusedRow();
                if (dataRow != null)
                {
                    if (!string.IsNullOrEmpty(dataRow.TRANSACTION_CODE) && dataRow.TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT)
                    {
                        MOS.Filter.HisTransactionFilter filter = new HisTransactionFilter();
                        filter.TRANSACTION_CODE__EXACT = dataRow.TRANSACTION_CODE;

                        var dataTransaction = new BackendAdapter(new CommonParam()).Get<List<HIS_TRANSACTION>>
                     ("api/HisTransaction/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, null).FirstOrDefault();

                        if (dataTransaction != null && dataTransaction.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT)
                        {
                            Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TransactionBillDetail").FirstOrDefault();
                            if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.TransactionBillDetail'");
                            if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                            {
                                List<object> listArgs = new List<object>();
                                listArgs.Add(moduleData);
                                listArgs.Add(dataTransaction.ID);
                                var extenceInstance = PluginInstance.GetPluginInstance(moduleData, listArgs);
                                if (extenceInstance == null)
                                {
                                    throw new ArgumentNullException("moduleData is null");
                                }
                                ((Form)extenceInstance).ShowDialog();
                            }
                        }
                        else
                        {
                            Inventec.Common.Logging.LogSystem.Error("khong tim thay giao dich tuong ung voi TRANSACTION_CODE = " + dataRow.TRANSACTION_CODE);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem_Tim_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem__LamLai_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                btnRefesh_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem__F1_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                txtTreatmentCode.Focus();
                txtTreatmentCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem__F2_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                txtAccountBookCode.Focus();
                txtAccountBookCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSearch_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.LoadDataGrid();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
