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
using ACS.EFMODEL.DataModels;
using DevExpress.XtraEditors;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.TransactionList
{
    public partial class frmTransactionList : HIS.Desktop.Utility.FormBase
    {
        #region Bo loc Diem thu / Nguoi thu / So thu + Tong tien
        // 3 GridLookUpEdit (grdCashierRoom/grdCashier/grdAccountBook) + lblTotalAmount khai bao trong Designer.
        // Chon nhieu nhu bo loc "Loai giao dich", nho lua chon qua ControlState.
        // Loc backend: CASHIER_ROOM_IDs, CASHIER_LOGINNAMEs, ACCOUNT_BOOK_IDs (HisTransactionViewFilter).

        List<V_HIS_CASHIER_ROOM> ListDiemThu = new List<V_HIS_CASHIER_ROOM>();
        List<ACS_USER> ListNguoiThu = new List<ACS_USER>();
        List<HIS_ACCOUNT_BOOK> ListSoThu = new List<HIS_ACCOUNT_BOOK>();
        List<HIS_ACCOUNT_BOOK> dataSoThu = new List<HIS_ACCOUNT_BOOK>();

        private const string TOTAL_AMOUNT_CAPTION = "Tổng tiền: ";

        /// <summary>Goi trong frmTransactionList_Load, truoc InitControlState.</summary>
        private void InitCashierFilterCombos()
        {
            try
            {
                InitCheck(grdCashierRoom, SelectionGrid__DiemThu);
                InitMultiCheckCombo(grdCashierRoom, BackendDataWorker.Get<V_HIS_CASHIER_ROOM>().Where(o => o.IS_ACTIVE == 1).OrderBy(o => o.CASHIER_ROOM_CODE).ToList(), "CASHIER_ROOM_CODE", "CASHIER_ROOM_NAME");

                InitCheck(grdCashier, SelectionGrid__NguoiThu);
                InitMultiCheckCombo(grdCashier, BackendDataWorker.Get<ACS_USER>().Where(o => o.IS_ACTIVE == 1).OrderBy(o => o.LOGINNAME).ToList(), "LOGINNAME", "USERNAME");

                HisAccountBookFilter accountBookFilter = new HisAccountBookFilter();
                accountBookFilter.IS_ACTIVE = 1;
                dataSoThu = new BackendAdapter(new CommonParam()).Get<List<HIS_ACCOUNT_BOOK>>("api/HisAccountBook/Get", ApiConsumers.MosConsumer, accountBookFilter, null) ?? new List<HIS_ACCOUNT_BOOK>();
                dataSoThu = dataSoThu.OrderBy(o => o.ACCOUNT_BOOK_CODE).ToList();
                InitCheck(grdAccountBook, SelectionGrid__SoThu);
                InitMultiCheckCombo(grdAccountBook, dataSoThu, "ACCOUNT_BOOK_CODE", "ACCOUNT_BOOK_NAME");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Nhu InitCombo nhung hien 2 cot ma + ten va dong tim kiem (danh muc dai).</summary>
        private void InitMultiCheckCombo(GridLookUpEdit cbo, object data, string codeMember, string nameMember)
        {
            try
            {
                cbo.Properties.DataSource = data;
                cbo.Properties.DisplayMember = nameMember;
                cbo.Properties.ValueMember = "ID";
                DevExpress.XtraGrid.Columns.GridColumn colCode = cbo.Properties.View.Columns.AddField(codeMember);
                colCode.VisibleIndex = 1;
                colCode.Width = 90;
                colCode.Caption = "Mã";
                DevExpress.XtraGrid.Columns.GridColumn colName = cbo.Properties.View.Columns.AddField(nameMember);
                colName.VisibleIndex = 2;
                colName.Width = 200;
                colName.Caption = "Tên";
                cbo.Properties.PopupFormWidth = 320;
                cbo.Properties.View.OptionsView.ShowColumnHeaders = true;
                cbo.Properties.View.OptionsView.ShowAutoFilterRow = true;
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

        private void SelectionGrid__DiemThu(object sender, EventArgs e)
        {
            try
            {
                this.ListDiemThu = new List<V_HIS_CASHIER_ROOM>();
                foreach (V_HIS_CASHIER_ROOM rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        this.ListDiemThu.Add(rv);
                }
                grdCashierRoom.Text = string.Join(", ", this.ListDiemThu.Select(o => o.CASHIER_ROOM_NAME));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__NguoiThu(object sender, EventArgs e)
        {
            try
            {
                this.ListNguoiThu = new List<ACS_USER>();
                foreach (ACS_USER rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        this.ListNguoiThu.Add(rv);
                }
                grdCashier.Text = string.Join(", ", this.ListNguoiThu.Select(o => o.USERNAME));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__SoThu(object sender, EventArgs e)
        {
            try
            {
                this.ListSoThu = new List<HIS_ACCOUNT_BOOK>();
                foreach (HIS_ACCOUNT_BOOK rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        this.ListSoThu.Add(rv);
                }
                grdAccountBook.Text = string.Join(", ", this.ListSoThu.Select(o => o.ACCOUNT_BOOK_NAME));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void grdCashierRoom_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = this.ListDiemThu != null ? string.Join(", ", this.ListDiemThu.Select(o => o.CASHIER_ROOM_NAME)) : "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void grdCashier_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = this.ListNguoiThu != null ? string.Join(", ", this.ListNguoiThu.Select(o => o.USERNAME)) : "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void grdAccountBook_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = this.ListSoThu != null ? string.Join(", ", this.ListSoThu.Select(o => o.ACCOUNT_BOOK_NAME)) : "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void grdCashierRoom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            SaveCashierFilterState(grdCashierRoom.Name, this.ListDiemThu.Select(o => o.ID));
        }

        private void grdCashier_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            SaveCashierFilterState(grdCashier.Name, this.ListNguoiThu.Select(o => o.ID));
        }

        private void grdAccountBook_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            SaveCashierFilterState(grdAccountBook.Name, this.ListSoThu.Select(o => o.ID));
        }

        /// <summary>Gan gia tri 3 bo loc vao filter (goi trong FillDataToGridTransaction).</summary>
        private void SetCashierFilter(HisTransactionViewFilter filter)
        {
            try
            {
                if (filter == null) return;
                if (this.ListDiemThu != null && this.ListDiemThu.Count > 0)
                {
                    filter.CASHIER_ROOM_IDs = this.ListDiemThu.Select(o => o.ID).ToList();
                }
                if (this.ListNguoiThu != null && this.ListNguoiThu.Count > 0)
                {
                    filter.CASHIER_LOGINNAMEs = this.ListNguoiThu.Select(o => o.LOGINNAME).ToList();
                }
                if (this.ListSoThu != null && this.ListSoThu.Count > 0)
                {
                    filter.ACCOUNT_BOOK_IDs = this.ListSoThu.Select(o => o.ID).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Tong tien theo toan bo bo loc hien tai (moi trang, khong chi trang dang xem).
        /// Tong = TotalPrice - CancelPrice (bo giao dich da huy).
        /// </summary>
        private void LoadTotalAmount(HisTransactionViewFilter filter)
        {
            decimal total = 0;
            try
            {
                CommonParam param = new CommonParam();
                HisTransactionTotalPriceSDO sdo = new BackendAdapter(param).Get<HisTransactionTotalPriceSDO>("api/HisTransaction/GetTotalPriceSdo", ApiConsumers.MosConsumer, filter, param);
                if (sdo != null)
                {
                    total = sdo.TotalPrice - sdo.CancelPrice;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            lblTotalAmount.Text = TOTAL_AMOUNT_CAPTION + Inventec.Common.Number.Convert.NumberToString(total, ConfigApplications.NumberSeperator);
        }

        #region ControlState — nho lua chon bo loc giua cac phien
        /// <summary>Goi ngay sau InitControlState trong frmTransactionList_Load.</summary>
        private void InitCashierFilterControlState()
        {
            try
            {
                if (this.currentControlStateRDO == null || this.currentControlStateRDO.Count == 0) return;

                RestoreCashierFilterSelection(grdCashierRoom, BackendDataWorker.Get<V_HIS_CASHIER_ROOM>(), o => o.ID);
                RestoreCashierFilterSelection(grdCashier, BackendDataWorker.Get<ACS_USER>(), o => o.ID);
                RestoreCashierFilterSelection(grdAccountBook, dataSoThu, o => o.ID);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void RestoreCashierFilterSelection<T>(GridLookUpEdit cbo, List<T> source, Func<T, long> getId)
        {
            var item = this.currentControlStateRDO.FirstOrDefault(o => o.KEY == cbo.Name);
            if (item == null || String.IsNullOrEmpty(item.VALUE) || source == null) return;

            List<long> ids = item.VALUE.Split(',').Select(s => Inventec.Common.TypeConvert.Parse.ToInt64(s)).ToList();
            List<T> selected = source.Where(o => ids.Contains(getId(o))).ToList();
            GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
            if (gridCheckMark == null || selected.Count == 0) return;

            gridCheckMark.ClearSelection(cbo.Properties.View);
            cbo.EditValue = null;
            gridCheckMark.SelectAll(selected);
        }

        private void SaveCashierFilterState(string key, IEnumerable<long> ids)
        {
            try
            {
                if (this.controlStateWorker == null) return;
                string strIDS = string.Join(",", ids);

                if (this.currentControlStateRDO == null)
                    this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();

                var csAddOrUpdate = this.currentControlStateRDO.FirstOrDefault(o => o.KEY == key && o.MODULE_LINK == moduleLink);
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = strIDS;
                }
                else
                {
                    this.currentControlStateRDO.Add(new HIS.Desktop.Library.CacheClient.ControlStateRDO
                    {
                        KEY = key,
                        MODULE_LINK = moduleLink,
                        VALUE = strIDS
                    });
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion
        #endregion
    }
}
