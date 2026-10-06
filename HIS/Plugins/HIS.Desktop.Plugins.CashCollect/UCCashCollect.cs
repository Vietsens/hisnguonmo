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
using AutoMapper;
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.LocalStorage.SdaConfigKey;
using HIS.Desktop.Plugins.CashCollect.Entity;
using HIS.Desktop.Plugins.CashCollect.Resources;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.Utility;
using HIS.UC.CashCollect;
using HIS.UC.CashCollect.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.LocalStorage.SdaConfig;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.CashCollect
{
    public partial class UCCashCollect : UserControlBase
    {
        List<HIS_ACCOUNT_BOOK> bookCollection { get; set; }
        string loginName = null;
        int start = 0;
        int limit = 0;
        int rowCount = 0;
        int dataTotal = 0;
        decimal totalPay = 0;
        long cashoutId;
        MOS.EFMODEL.DataModels.HIS_CASHOUT currentData;
        internal MOS.EFMODEL.DataModels.V_HIS_TRANSACTION TransactionData = null;
        UserControl ucGridControl;
        UCCashCollectProcessor CashCollectProcessor;
        List<CashCollectADO> inputAdo = new List<CashCollectADO>();
        List<HIS_ACCOUNT_BOOK> AccountBookSelecteds;
        //List<HIS_ACCOUNT_BOOK> listAccountBook = new List<HIS_ACCOUNT_BOOK>();
        List<V_HIS_TRANSACTION> dataClick = new List<V_HIS_TRANSACTION>();
        List<V_HIS_TRANSACTION> lstTranSaction { get; set; }
        List<HIS_CASHOUT> cashout { get; set; }
        List<HIS_CASHOUT> listCashOut;
        List<V_HIS_TRANSACTION> dataByIsColect = new List<V_HIS_TRANSACTION>();
        //Dictionary<long, CashCollectADO> dicMediMateAdo = new Dictionary<long, CashCollectADO>();

        internal Inventec.Desktop.Common.Modules.Module currentModule;

        public UCCashCollect(Inventec.Desktop.Common.Modules.Module currentModule)
            : base(currentModule)
        {
            InitializeComponent();
        }

        public UCCashCollect(Inventec.Desktop.Common.Modules.Module currentModule, long accountBookID)
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

        public void FindShortcut()
        {
            try
            {
                btnFind_Click(null, null);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public void EditShortcut()
        {
            try
            {
                if (btnEdit.Enabled == false)
                {
                    return;
                }
                btnEdit_Click(null, null);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public void AddShortcut()
        {
            try
            {
                btnAdd_Click(null, null);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public void CancelShortcut()
        {
            try
            {
                btnCancel_Click(null, null);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitUcgrid()
        {
            try
            {
                CashCollectProcessor = new UCCashCollectProcessor();
                CashCollectInitADO ado = new CashCollectInitADO();
                ado.ListCashCollectColumn = new List<HIS.UC.CashCollect.CashCollectColumn>();
                ado.CashCollectGrid_CustomUnboundColumnData = gridViewTransaction_CustomUnboundColumnData;
                ado.btn_Un_Collect_Click = btn_Un_Collect_Click;
                ado.check_Changed = check_Changed;
                ado.CheckAll_Click = checkAll_Click;

                CashCollectColumn colNopQuy = new CashCollectColumn("   ", "check", 30, true);
                colNopQuy.VisibleIndex = 1;
                colNopQuy.Visible = false;
                colNopQuy.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListCashCollectColumn.Add(colNopQuy);

                CashCollectColumn colSTT = new CashCollectColumn("STT", "STT", 30, false);
                colSTT.VisibleIndex = 0;
                colSTT.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListCashCollectColumn.Add(colSTT);

                //CashCollectColumn colHuy = new CashCollectColumn("   ", "Delete", 30, true);
                //colHuy.VisibleIndex = 2;
                //colHuy.Visible = false;
                //colHuy.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                //ado.ListCashCollectColumn.Add(colHuy);

                CashCollectColumn colMaGiaoDich = new CashCollectColumn("Mã giao dịch", "TRANSACTION_CODE", 100, false);
                colMaGiaoDich.VisibleIndex = 3;
                ado.ListCashCollectColumn.Add(colMaGiaoDich);

                CashCollectColumn colSoTien = new CashCollectColumn("Số tiền", "AMOUNT", 100, false);
                colSoTien.VisibleIndex = 4;
                colSoTien.Format = new DevExpress.Utils.FormatInfo();
                colSoTien.Format.FormatString = "#,##0.0000";
                colSoTien.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                colSoTien.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListCashCollectColumn.Add(colSoTien);

                CashCollectColumn colKetChuyen = new CashCollectColumn("Kết chuyển", "KC_AMOUNT", 100, false);
                colKetChuyen.VisibleIndex = 5;
                colKetChuyen.Format = new DevExpress.Utils.FormatInfo();
                colKetChuyen.Format.FormatString = "#,##0.0000";
                colKetChuyen.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                colKetChuyen.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListCashCollectColumn.Add(colKetChuyen);

                CashCollectColumn colHinhThuc = new CashCollectColumn("Hình thức", "PAY_FORM_NAME", 100, false);
                colHinhThuc.VisibleIndex = 6;
                ado.ListCashCollectColumn.Add(colHinhThuc);

                CashCollectColumn colGiaoDichVien = new CashCollectColumn("Giao dịch viên", "CASHIER", 100, false);
                colGiaoDichVien.VisibleIndex = 7;
                colGiaoDichVien.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListCashCollectColumn.Add(colGiaoDichVien);

                CashCollectColumn colPhongThuNgan = new CashCollectColumn("Phòng thu ngân", "CASHIER_ROOM_NAME", 100, false);
                colPhongThuNgan.VisibleIndex = 12;
                ado.ListCashCollectColumn.Add(colPhongThuNgan);

                CashCollectColumn colSo = new CashCollectColumn("Số", "NUM_ORDER", 50, false);
                colSo.VisibleIndex = 9;
                ado.ListCashCollectColumn.Add(colSo);

                CashCollectColumn colMaSo = new CashCollectColumn("Mã sổ", "ACCOUNT_BOOK_CODE", 100, false);
                colMaSo.VisibleIndex = 10;
                ado.ListCashCollectColumn.Add(colMaSo);

                CashCollectColumn colTenSo = new CashCollectColumn("Tên sổ", "ACCOUNT_BOOK_NAME", 100, false);
                colTenSo.VisibleIndex = 11;
                ado.ListCashCollectColumn.Add(colTenSo);

                CashCollectColumn colLoaiGiaoDich = new CashCollectColumn("Loại giao dịch", "TRANSACTION_TYPE_NAME", 100, false);
                colLoaiGiaoDich.VisibleIndex = 8;
                ado.ListCashCollectColumn.Add(colLoaiGiaoDich);

                CashCollectColumn colMaDieuTri = new CashCollectColumn("Mã điều trị", "TREATMENT_CODE", 100, false);
                colMaDieuTri.VisibleIndex = 13;
                ado.ListCashCollectColumn.Add(colMaDieuTri);

                CashCollectColumn colTenBenhNhan = new CashCollectColumn("Tên Bệnh nhân", "TDL_PATIENT_NAME", 100, false);
                colTenBenhNhan.VisibleIndex = 14;
                ado.ListCashCollectColumn.Add(colTenBenhNhan);

                CashCollectColumn colNgaySinh = new CashCollectColumn("Ngày sinh", "DOB_STR", 100, false);
                colNgaySinh.VisibleIndex = 15;
                colNgaySinh.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListCashCollectColumn.Add(colNgaySinh);

                CashCollectColumn colGioiTinh = new CashCollectColumn("Giới tính", "TDL_PATIENT_GENDER_NAME", 100, false);
                colGioiTinh.VisibleIndex = 16;
                ado.ListCashCollectColumn.Add(colGioiTinh);

                CashCollectColumn colMaBenhNhan = new CashCollectColumn("Mã bệnh nhân", "TDL_PATIENT_CODE", 100, false);
                colMaBenhNhan.VisibleIndex = 17;
                ado.ListCashCollectColumn.Add(colMaBenhNhan);

                CashCollectColumn colThoiGianTao = new CashCollectColumn("Thời gian tạo", "CREATE_TIME_STR", 150, false);
                colThoiGianTao.VisibleIndex = 18;
                colThoiGianTao.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListCashCollectColumn.Add(colThoiGianTao);

                CashCollectColumn colNguoiTao = new CashCollectColumn("Người tạo", "CREATOR", 100, false);
                colNguoiTao.VisibleIndex = 19;
                ado.ListCashCollectColumn.Add(colNguoiTao);

                CashCollectColumn colThoiGianSua = new CashCollectColumn("Thời gian sửa", "MODIFY_TIME_STR", 150, false);
                colThoiGianSua.VisibleIndex = 20;
                colThoiGianSua.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListCashCollectColumn.Add(colThoiGianSua);

                CashCollectColumn colNguoiSua = new CashCollectColumn("Người sửa", "MODIFIER", 100, false);
                colNguoiSua.VisibleIndex = 21;
                ado.ListCashCollectColumn.Add(colNguoiSua);

                CashCollectColumn colMienGiam = new CashCollectColumn("Miễn giảm", "EXEMPTION", 100, false);
                colMienGiam.VisibleIndex = 22;
                colMienGiam.Format = new DevExpress.Utils.FormatInfo();
                colMienGiam.Format.FormatString = "#,##0.0000";
                colMienGiam.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                colMienGiam.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListCashCollectColumn.Add(colMienGiam);

                CashCollectColumn colQuyThanhToan = new CashCollectColumn("Quỹ thanh toán", "BILL_FUND_AMOUNT", 100, false);
                colQuyThanhToan.VisibleIndex = 23;
                colQuyThanhToan.Format = new DevExpress.Utils.FormatInfo();
                colQuyThanhToan.Format.FormatString = "#,##0.0000";
                colQuyThanhToan.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                colQuyThanhToan.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListCashCollectColumn.Add(colQuyThanhToan);

                this.ucGridControl = (UserControl)CashCollectProcessor.Run(ado);


                if (ucGridControl != null)
                {
                    this.panelControl1.Controls.Add(this.ucGridControl);
                    this.ucGridControl.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCCashCollect_Load(object sender, EventArgs e)
        {
            try
            {
                //SetIcon();
                WaitingManager.Show();
                txtLoginName.EditValue = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                dtDayFrom.EditValue = DateTime.Now;
                dtDayTo.EditValue = DateTime.Now;
                dtCashOutTime.EditValue = DateTime.Now;
                btnEdit.Enabled = false;
                spinNumOrderFrom.EditValue = null;
                spinNumOrderTo.EditValue = null;
                InitUcgrid();
                LoadDataToCombo();
                LoadDataToComboBookCollection1();
                FillDataToGrid(this);
                FillDataToGridCashout();
                SetCaptionByLanguageKey();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToComboBookCollection1()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 100, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("LOGINNAME", "USERNAME", columnInfos, false, 350);
                ControlEditorLoader.Load(cboCashier, BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>(), controlEditorADO);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridCashout()
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                HisCashoutFilter cashoutFilter = new HisCashoutFilter();
                //cashoutFilter.CREATOR = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                cashout = new BackendAdapter(param).Get<List<HIS_CASHOUT>>(
                    "api/HisCashout/Get", ApiConsumers.MosConsumer, cashoutFilter, param);
                if (cashout != null)
                {
                    listCashOut = cashout.Where(o => o.LOGINNAME == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName()).ToList();
                    gridControlCashout.DataSource = listCashOut;
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGrid(UCCashCollect frmCashCollect)
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

                this.currentTransactionFilter = BuildTransactionFilter();
                FillDataToGridTransaction(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridTransaction, param, numPageSize);

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridTransaction(object data)
        {
            try
            {
                WaitingManager.Show();
                int start = ((CommonParam)data).Start ?? 0;
                int limit = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start, limit);
                // Paging reuses the filter of the last search so that every page (and "select all") matches it
                MOS.Filter.HisTransactionViewFilter TransactionFilter = this.currentTransactionFilter ?? BuildTransactionFilter();

                var rs = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_TRANSACTION>>(
                 HIS.Desktop.ApiConsumer.HisRequestUriStore.HIS_TRANSACTION_GETVIEW,
                 HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                 TransactionFilter,
                 param);

                if (rs != null)
                {
                    dataByIsColect = rs.Data;

                    if (dataByIsColect == null)
                    {
                        WaitingManager.Hide();
                        return;
                    }
                    inputAdo = new List<CashCollectADO>();
                    Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_TRANSACTION, CashCollectADO>();
                    foreach (var item in dataByIsColect)
                    {
                        CashCollectADO CashCollectADO = Mapper.Map<MOS.EFMODEL.DataModels.V_HIS_TRANSACTION, CashCollectADO>(item);
                        // Keep the selection made on other pages / by "select all"
                        CashCollectADO.check = !item.CASHOUT_ID.HasValue && dicSelectedTransaction.ContainsKey(item.ID);
                        inputAdo.Add(CashCollectADO);
                    }
                }
                if (ucGridControl != null)
                {
                    rowCount = (data == null ? 0 : inputAdo.Count);
                    dataTotal = (rs.Param == null ? 0 : rs.Param.Count ?? 0);
                    CashCollectProcessor.Reload(ucGridControl, inputAdo);
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Builds the transaction filter from the search controls.</summary>
        /// <remarks>Exceptions are left to the caller on purpose: never query with a partially built filter.</remarks>
        private MOS.Filter.HisTransactionViewFilter BuildTransactionFilter()
        {
            MOS.Filter.HisTransactionViewFilter TransactionFilter = new MOS.Filter.HisTransactionViewFilter();
            TransactionFilter.IS_CANCEL = false;
            TransactionFilter.IS_ACTIVE = 1;
            TransactionFilter.ORDER_FIELD = "CREATE_TIME";
            TransactionFilter.ORDER_DIRECTION = "DESC";
            TransactionFilter.KEY_WORD = txtKeyword.Text;
            if (!String.IsNullOrEmpty(txtFindTransactionCode.Text))
            {
                string code = txtFindTransactionCode.Text.Trim();
                if (code.Length < 12)
                {
                    code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                    txtFindTransactionCode.Text = code;
                }
                TransactionFilter.TRANSACTION_CODE__EXACT = code;
            }
            if (AccountBookSelecteds != null && AccountBookSelecteds.Count > 0)
            {
                TransactionFilter.ACCOUNT_BOOK_IDs = AccountBookSelecteds.Select(o => o.ID).ToList();
            }
            TransactionFilter.CASHIER_ROOM_IDs = GetSelectedCashierRoomIds();
            if (cboCashier.EditValue != null)
            {
                TransactionFilter.CREATOR = (string)cboCashier.Text;
            }

            if ((long)cboStatus.EditValue == 3)
            {
                TransactionFilter.HAS_CASHOUT = false;
            }
            else if ((long)cboStatus.EditValue == 2)
            {
                TransactionFilter.CASHOUT_IDs = cashout.Select(o => o.ID).ToList();
            }
            if (spinNumOrderFrom.EditValue != null)
            {
                TransactionFilter.NUM_ORDER_FROM = (long)spinNumOrderFrom.Value;
            }
            if (spinNumOrderTo.EditValue != null)
            {
                TransactionFilter.NUM_ORDER_TO = (long)spinNumOrderTo.Value;
            }
            if (dtDayFrom.EditValue != null && dtDayFrom.DateTime != DateTime.MinValue)
            {
                TransactionFilter.TRANSACTION_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                   Convert.ToDateTime(dtDayFrom.EditValue).ToString("yyyyMMdd") + "000000");
            }
            if (dtDayTo.EditValue != null && dtDayTo.DateTime != DateTime.MinValue)
            {
                TransactionFilter.TRANSACTION_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                    Convert.ToDateTime(dtDayTo.EditValue).ToString("yyyyMMdd") + "235959");
            }
            return TransactionFilter;
        }

        private void LoadDataToCombo()
        {
            try
            {
                InitBookCollectionCheck();
                LoadDataToComboBookCollection();
                InitCashierRoomCheck();
                LoadDataToComboCashierRoom();
                LoadDataToComboStatus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitBookCollectionCheck()
        {
            try
            {
                GridCheckMarksSelection1 gridCheck = new GridCheckMarksSelection1(cboBookCollection.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection1.SelectionChangedEventHandler(SelectionGrid__BookCollection);
                cboBookCollection.Properties.Tag = gridCheck;
                cboBookCollection.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection1 gridCheckMark = cboBookCollection.Properties.Tag as GridCheckMarksSelection1;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboBookCollection.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__BookCollection(object sender, EventArgs e)
        {
            try
            {
                AccountBookSelecteds = new List<HIS_ACCOUNT_BOOK>();
                foreach (MOS.EFMODEL.DataModels.HIS_ACCOUNT_BOOK rv in (sender as GridCheckMarksSelection1).Selection)
                {
                    if (rv != null)
                        AccountBookSelecteds.Add(rv);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToComboStatus()
        {
            try
            {
                List<Status> status = new List<Status>();
                status.Add(new Status(1, "Tất cả"));
                status.Add(new Status(2, "Đã nộp"));
                status.Add(new Status(3, "Chưa nộp"));

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                //columnInfos.Add(new ColumnInfo("id", "", 50, 1));
                columnInfos.Add(new ColumnInfo("statusName", "", 300, 2));
                var a = HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer;
                ControlEditorADO controlEditorADO = new ControlEditorADO("statusName", "id", columnInfos, false, 350);
                ControlEditorLoader.Load(cboStatus, status, controlEditorADO);
                cboStatus.EditValue = status[2].id;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToComboBookCollection()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisAccountBookFilter filter = new HisAccountBookFilter();
                bookCollection = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_ACCOUNT_BOOK>>(
                    "api/HisAccountBook/Get",
                HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                filter,
                param);
                // var datas = BackendDataWorker.Get<HIS_ACCOUNT_BOOK>();
                if (bookCollection != null)
                {
                    cboBookCollection.Properties.DataSource = bookCollection;
                    cboBookCollection.Properties.DisplayMember = "ACCOUNT_BOOK_NAME";
                    cboBookCollection.Properties.ValueMember = "ID";
                    DevExpress.XtraGrid.Columns.GridColumn col2 = cboBookCollection.Properties.View.Columns.AddField("ACCOUNT_BOOK_NAME");
                    col2.VisibleIndex = 1;
                    col2.Width = 300;
                    col2.Caption = "";
                    cboBookCollection.Properties.PopupFormWidth = 300;
                    cboBookCollection.Properties.View.OptionsView.ShowColumnHeaders = false;
                    cboBookCollection.Properties.View.OptionsSelection.MultiSelect = true;
                    GridCheckMarksSelection1 gridCheckMark = cboBookCollection.Properties.Tag as GridCheckMarksSelection1;
                    if (gridCheckMark != null)
                    {
                        gridCheckMark.ClearSelection(cboBookCollection.Properties.View);
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeyword_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    WaitingManager.Show();
                    SearchTransaction();
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>New search: the previous selection belonged to the previous filter, so it is cleared.</summary>
        private void SearchTransaction()
        {
            try
            {
                ClearSelectedTransactions();
                FillDataToGrid(this);
                RefreshCollectGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTransaction_CustomUnboundColumnData(V_HIS_TRANSACTION data, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (data != null)
                {
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
                        else if (e.Column.FieldName == "MODIFY_TIME_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
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

        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                SearchTransaction();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btn_Un_Collect_Click(V_HIS_TRANSACTION data)
        {
            try
            {
                long CashoutID;
                List<long> lstTransactionIds = new List<long>();
                //CashoutID = (long)data.CASHOUT_ID;
                lstTransactionIds.Add(data.ID);
                CommonParam param = new CommonParam();
                bool success = false;
                HisCashoutSDO cashoutSDO = new HisCashoutSDO();
                cashoutSDO.TransactionIds = lstTransactionIds;
                cashoutSDO.CashoutTime = (long)data.MODIFY_TIME;
                cashoutSDO.Amount = data.AMOUNT;
                cashoutSDO.Id = (long)data.CASHOUT_ID;

                #region Message
                MessageManager.Show(param, success);
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void check_Changed(List<HIS.UC.CashCollect.CashCollectADO> data)
        {
            try
            {
                // "data" only holds the checked rows of the current page; the selection spans every page
                SyncSelectionWithCurrentPage();
                RefreshCollectGrid();
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.CashCollect.Resources.Lang", typeof(HIS.Desktop.Plugins.CashCollect.UCCashCollect).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCCashCollect.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnFind.Text = Inventec.Common.Resource.Get.Value("UCCashCollect.btnFind.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboBookCollection.Properties.NullText = Inventec.Common.Resource.Get.Value("UCCashCollect.cboBookCollection.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboStatus.Properties.NullText = Inventec.Common.Resource.Get.Value("UCCashCollect.cboStatus.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCCashCollect.txtKeyword.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("UCCashCollect.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciAmountSum.Text = Inventec.Common.Resource.Get.Value("UCCashCollect.lciAmountSum.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCashierRoom.Text = Inventec.Common.Resource.Get.Value("UCCashCollect.lciCashierRoom.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewCollect_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    V_HIS_TRANSACTION data = (V_HIS_TRANSACTION)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            try
                            {
                                e.Value = e.ListSourceRowIndex + 1 + start;
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri STT", ex);
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
                        else if (e.Column.FieldName == "MODIFY_TIME_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
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

        private void repositoryItemButtonEdit_Delete_Click(object sender, EventArgs e)
        {
            try
            {
                var data = (HIS.UC.CashCollect.CashCollectADO)gridViewCollect.GetFocusedRow();
                if (data == null) return;

                if (dicSelectedTransaction.Remove(data.ID))
                {
                    // Newly selected transaction: also untick it on the transaction grid
                    SetCheckAllState(false);
                    ApplySelectionToCurrentPage();
                }
                else if (dataClick != null)
                {
                    // Transaction of the edited cashout: it is released when "Sửa" is saved
                    dataClick.RemoveAll(o => o.ID == data.ID);
                }
                RefreshCollectGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewCashout_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    HIS_CASHOUT data = (HIS_CASHOUT)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "CASHOUT_TIME_STR")
                        {
                            try
                            {
                                // Deposit time chosen by the user (was CREATE_TIME, which never reflected "Ngày nộp")
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CASHOUT_TIME);
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

        private void gridViewCashout_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            try
            {
                var row = (HIS_CASHOUT)gridViewCashout.GetFocusedRow();
                if (row == null) return;

                WaitingManager.Show();
                btnEdit.Enabled = true;
                btnAdd.Enabled = false;
                this.currentData = row;
                cashoutId = row.ID;
                // Editing another cashout: forget what was ticked for the previous one
                ClearSelectedTransactions();

                CommonParam param = new CommonParam();
                MOS.Filter.HisTransactionViewFilter TransactionFilter = new MOS.Filter.HisTransactionViewFilter();
                TransactionFilter.CASHOUT_ID = row.ID;
                lstTranSaction = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_TRANSACTION>>(
               "api/HisTransaction/GetView",
               HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
               TransactionFilter,
               param);
                dataClick = lstTranSaction != null ? lstTranSaction.Where(o => o.CASHOUT_ID == row.ID).ToList() : new List<V_HIS_TRANSACTION>();
                // DateTime value (not a string) so that saving does not depend on the regional date format
                dtCashOutTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(row.CASHOUT_TIME);

                FillDataToGrid(this);
                RefreshCollectGrid();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnAdd.Enabled) return;

                // Selection of every page (manual ticks and "select all"), not only the rows of the current page
                List<CashCollectADO> collects = GetTransactionsToCollect();
                if (collects.Count == 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.ChuaChonGiaoDichNopQuy,
                        HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                    return;
                }

                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;

                HisCashoutSDO newdata = new HisCashoutSDO();
                newdata.CashoutTime = GetCashoutTimeInput();
                if (newdata.CashoutTime <= 0)
                {
                    newdata.CashoutTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0;
                }
                newdata.TransactionIds = collects.Select(p => p.ID).ToList();
                newdata.Amount = CalcCashoutAmount(collects);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => newdata), newdata));

                var outPut = new BackendAdapter(param).Post<HIS_CASHOUT>(
                    "/api/HisCashout/Create",
                HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                newdata,
                param);
                if (outPut != null)
                {
                    success = true;
                    ResetToAddMode();
                    FillDataToGrid(this);
                    FillDataToGridCashout();
                }
                WaitingManager.Hide();
                #region Show message
                MessageManager.Show(this.ParentForm, param, success);
                #endregion
                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                txtKeyword.EditValue = null;
                txtFindTransactionCode.EditValue = null;
                txtAmountSum.EditValue = null;
                GridCheckMarksSelection1 gridCheckMark = cboBookCollection.Properties.Tag as GridCheckMarksSelection1;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboBookCollection.Properties.View);
                }
                cboBookCollection.Text = "";
                cboBookCollection.EditValue = null;
                ClearCashierRoomSelection();
                cboCashier.EditValue = null;
                spinNumOrderFrom.EditValue = null;
                spinNumOrderTo.EditValue = null;
                dtDayFrom.EditValue = DateTime.Now;
                dtDayTo.EditValue = DateTime.Now;
                dtCashOutTime.EditValue = DateTime.Now;
                ResetToAddMode();
                FillDataToGrid(this);
                FillDataToGridCashout();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                if (cashoutId <= 0) return;

                WaitingManager.Show();
                // Transactions kept in the cashout + newly selected ones (the right grid)
                List<CashCollectADO> collects = GetTransactionsToCollect();
                HisCashoutSDO dataSDO = new HisCashoutSDO();
                dataSDO.TransactionIds = collects.Select(o => o.ID).ToList();
                CommonParam param = new CommonParam();
                bool success = false;

                dataSDO.Id = cashoutId;
                dataSDO.Amount = CalcCashoutAmount(collects);
                if (dataSDO.TransactionIds.Count == 0)
                {
                    // No transaction left in the cashout: delete it
                    success = new BackendAdapter(param).Post<bool>("api/HisCashout/Delete", ApiConsumers.MosConsumer,
                           dataSDO.Id, param);
                    if (success)
                    {
                        ResetToAddMode();
                        FillDataToGridCashout();
                        FillDataToGrid(this);
                    }
                    WaitingManager.Hide();
                    MessageManager.Show(this.ParentForm, param, success);
                    SessionManager.ProcessTokenLost(param);
                    return;
                }
                HIS_CASHOUT data = cashout != null ? cashout.FirstOrDefault(o => o.ID == cashoutId) : null;
                dataSDO.CashoutTime = GetCashoutTimeInput();
                if (dataSDO.CashoutTime <= 0 && data != null)
                {
                    dataSDO.CashoutTime = data.CASHOUT_TIME;
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataSDO), dataSDO));

                var outPut = new BackendAdapter(param).Post<HIS_CASHOUT>(
                    "api/HisCashout/Update",
                                HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                dataSDO,
                                param);
                if (outPut != null)
                {
                    success = true;
                    ResetToAddMode();
                    FillDataToGridCashout();
                    FillDataToGrid(this);
                }
                WaitingManager.Hide();
                #region Show message
                MessageManager.Show(this.ParentForm, param, success);
                #endregion
                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonDelete_Cashout_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                var data = (HIS_CASHOUT)gridViewCashout.GetFocusedRow();
                if (MessageBox.Show(String.Format(ResourceMessage.BanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    if (data.ID != null)
                    {
                        bool success = false;
                        CommonParam param = new CommonParam();
                        success = new BackendAdapter(param).Post<bool>("api/HisCashout/Delete", ApiConsumers.MosConsumer,
                            data.ID, param);
                        if (success)
                        {
                            if (cashoutId == data.ID)
                            {
                                // The cashout being edited no longer exists
                                ResetToAddMode();
                            }
                            FillDataToGridCashout();
                            RefreshCollectGrid();
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

        private void txtFindTransactionCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    WaitingManager.Show();
                    SearchTransaction();
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboBookCollection_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection1 gridCheckMark = sender is SearchLookUpEdit ? (sender as SearchLookUpEdit).Properties.Tag as GridCheckMarksSelection1 : (sender as RepositoryItemSearchLookUpEdit).Tag as GridCheckMarksSelection1;
                if (gridCheckMark == null) return;
                foreach (MOS.EFMODEL.DataModels.HIS_ACCOUNT_BOOK rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(", "); }
                    sb.Append(rv.ACCOUNT_BOOK_NAME.ToString());
                }
                e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboBookCollection_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboBookCollection.EditValue != null && cboBookCollection.EditValue != cboBookCollection.OldEditValue)
                    {
                        MOS.EFMODEL.DataModels.HIS_ACCOUNT_BOOK gt = bookCollection.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboBookCollection.EditValue.ToString()));
                        if (gt != null)
                        {
                            btnFind.Focus();
                        }
                    }
                    else
                    {
                        btnFind.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboBookCollection_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboBookCollection.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_ACCOUNT_BOOK gt = bookCollection.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboBookCollection.EditValue.ToString()));
                        if (gt != null)
                        {
                            btnFind.Focus();
                        }
                    }
                    else
                    {
                        cboBookCollection.ShowPopup();
                    }
                }
                else
                {
                    cboBookCollection.ShowPopup();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
