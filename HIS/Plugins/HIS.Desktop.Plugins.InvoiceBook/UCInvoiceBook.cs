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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraGrid.Views.Base;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraLayout;
using Inventec.Desktop.Common.Message;
using Inventec.Common.Adapter;
using Inventec.Common.RichEditor.Base;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.Plugins.InvoiceBook.Popup.AssignAuthorized;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.InvoiceBook.Popup.LogPrint;
using HIS.Desktop.LocalStorage.ConfigApplication;
using MOS.Filter;
using HIS.Desktop.ApiConsumer;
using Inventec.UC.Paging;
using Inventec.Common.Logging;
using HIS.Desktop.Utility;

namespace HIS.Desktop.Plugins.InvoiceBook
{
    delegate void ClickPrintMenuItemButton_Click(object sender, ItemClickEventArgs e);
    public partial class UCInvoiceBook : HIS.Desktop.Utility.UserControlBase
    {
        #region overview

        string _logginname;
        List<V_HIS_USER_INVOICE_BOOK> _listUserInvoiceBooks;
        V_HIS_INVOICE _hisInvoiceFocusRowButtonPrint;
        int _rowCountInvoiceBook = 0;
        int _dataTotalInvoiceBook = 0;
        List<V_HIS_INVOICE_BOOK> _listInvoiceBooks = new List<V_HIS_INVOICE_BOOK>();
        int _rowCountInvoice = 0;
        int _dataTotalInvoice = 0;
        List<V_HIS_INVOICE> _listInvoices = new List<V_HIS_INVOICE>();
        UCInvoiceBook control;
        int _rowCountInvoiceDetail = 0;
        int _dataTotalInvoiceDetail = 0;
        List<HIS_INVOICE_DETAIL> _listInvoiceDetails = new List<HIS_INVOICE_DETAIL>();
        V_HIS_INVOICE_BOOK rowGridViewInvoiceBookFocus = new V_HIS_INVOICE_BOOK();
        V_HIS_INVOICE rowGridViewInvoiceFocus = new V_HIS_INVOICE();
        int positionHandleControlEditor = -1;
        HIS.Desktop.Plugins.InvoiceBook.UCInvoiceBook ucInvoiceBook;
        int rowCounts = 0;
        int dataTotal = 0;
        int startPage = 0;
        int startPage2 = 0;
        int startPage3 = 0;
        PagingGrid pagingGrid;
        int pageSize = 0;
        Inventec.Desktop.Common.Modules.Module Module;

        List<ACS.EFMODEL.DataModels.ACS_CONTROL> controlAcs;

        List<HIS_USER_INVOICE_BOOK> _UserInvoiceBookByLoginNames { get; set; }

        #endregion

        public UCInvoiceBook(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
            _logginname = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
            Module = module;
        }

        private void gctInvoiceBook_Click(object sender, EventArgs e)
        {

        }

        private void btnAuthorizedInvoiceBookE_Click(object sender, EventArgs e)
        {
            try
            {
                var dataFocuseRow = (V_HIS_INVOICE_BOOK)grvInvoiceBook.GetFocusedRow();
                var assignAuthorized = new frmAssignAuthorized
                {
                    HisInvoiceBookInUc = dataFocuseRow
                };
                assignAuthorized.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnCreateInvoiceBookE_Click(object sender, EventArgs e)
        {
            try
            {
                var dataFocuseRow = (V_HIS_INVOICE_BOOK)grvInvoiceBook.GetFocusedRow();
                var assignAuthorized = new frmCreateInvoice { _invoiceBookWitchUcBook = dataFocuseRow, currentModule = Module };
                assignAuthorized.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void deleteE_Click(object sender, EventArgs e)
        {

        }

        private void deleteE_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {

            CommonParam param = new CommonParam();
            bool result = false;
            try
            {
                WaitingManager.Show();
                var row = (MOS.EFMODEL.DataModels.V_HIS_INVOICE_BOOK)grvInvoiceBook.GetFocusedRow();
                if (row != null)
                {
                    MOS.EFMODEL.DataModels.HIS_INVOICE_BOOK aInvoiceBook = new MOS.EFMODEL.DataModels.HIS_INVOICE_BOOK();
                    Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_INVOICE_BOOK>(aInvoiceBook, row);
                    bool outPut = new BackendAdapter(param).Post<bool>(ApiConsumer.HisRequestUriStore.HIS_INVOICE_BOOK_DELETE, ApiConsumer.ApiConsumers.MosConsumer, aInvoiceBook, null);
                    if (outPut)
                    {
                        result = true;
                        //ucInvoiceBook.MeShow();
                        FillDataToGridInvoiceBook(this);
                    }
                }
                WaitingManager.Hide();
                #region Show message
                ResultManager.ShowMessage(param, result);
                #endregion

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }

        }

        public void FillDataToGridInvoiceBook(UCInvoiceBook control)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisInvoiceBookViewFilter filter = new HisInvoiceBookViewFilter();
                filter.ORDER_DIRECTION = "MODIFY_TIME";
                filter.ORDER_FIELD = "DESC";
                _listInvoiceBooks = new BackendAdapter(param).Get<List<V_HIS_INVOICE_BOOK>>(HisRequestUriStore.HIS_INVOICE_BOOK_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, filter, null);
                control.gctInvoiceBook.BeginUpdate();
                control.gctInvoiceBook.DataSource = null;
                control.gctInvoiceBook.DataSource = _listInvoiceBooks;
                control.gctInvoiceBook.Focus();
                if (_listInvoiceBooks != null && _listInvoiceBooks.Count > 0)
                {
                    control.ActionType = GlobalVariables.ActionView;
                    EnableButton(control.ActionType, control);
                }
                control.gctInvoiceBook.EndUpdate();
                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtInvoiceBookName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spTotal.Focus();
                    spTotal.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spTotal_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spFromOrder.Focus();
                    spFromOrder.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spFromOrder_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSymbolCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTemplateCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtReleaseDate.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtReleaseDate_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDescription.Focus();
                    txtDescription.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDescription_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSaveInvoiceBook.Focus();
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void BtnNewShortcut()
        {
            if (btnRefreshInvoiceBook.Enabled)
                btnRefreshInvoiceBook_Click(null, null);
        }

        public void BtnSaveShortcut()
        {
            if (btnSaveInvoiceBook.Enabled)
                btnSaveInvoiceBook_Click(null, null);
        }

        public void MeShow()
        {
            try
            {
                this.ActionType = GlobalVariables.ActionView;
                //InvoicePrintPageCFG.LoadConfig();
                EnableButton(this.ActionType, this);
                FillDataToGridInvoiceBook(this);
                var row = (MOS.EFMODEL.DataModels.V_HIS_INVOICE_BOOK)grvInvoiceBook.GetFocusedRow();
                if (row != null)
                {
                    FillDataToGridInvoice(row, this);
                }
                ValidationControls();
                gctInvoice.DataSource = null;
                btnSaveInvoiceBook.Enabled = true;
                ActionType = GlobalVariables.ActionAdd;
                RemoveControlError();
                FillDataToControl(null, this);
                txtInvoiceBookName.Focus();
                txtInvoiceBookName.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnHistoryPrint_Click(object sender, EventArgs e)
        {
            try
            {
                var row = (V_HIS_INVOICE)grvInvoice.GetFocusedRow();
                LogPrint logPrint = new LogPrint(row);
                logPrint.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void grvInvoice_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                var row = (V_HIS_INVOICE)grvInvoice.GetRow(e.RowHandle);
                if (row != null && row.IS_CANCEL == 1)
                {
                    e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Strikeout);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void repositoryItemButtonEdit__E_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (grvInvoice.FocusedRowHandle >= 0)
                {
                    var data = (V_HIS_INVOICE)grvInvoice.GetFocusedRow();
                    if (data != null)
                    {
                        Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TransactionBillInfoEdit").FirstOrDefault();
                        if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.TransactionBillInfoEdit");
                        if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                        {
                            List<object> listArgs = new List<object>();
                            listArgs.Add(data);
                            listArgs.Add((HIS.Desktop.Common.DelegateRefreshData)RefeshDataBefoEdit);
                            listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.Module.RoomId, this.Module.RoomTypeId));
                            var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.Module.RoomId, this.Module.RoomTypeId), listArgs);
                            if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                            ((Form)extenceInstance).ShowDialog();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void RefeshDataBefoEdit()
        {
            try
            {
                SetDataToInvoice();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadUserInvoiceBooks()
        {
            try
            {
                _UserInvoiceBookByLoginNames = new List<HIS_USER_INVOICE_BOOK>();
                if (string.IsNullOrEmpty(this._logginname))
                    return;
                List<long> _invoiceIds = _listInvoices.Select(p => p.INVOICE_BOOK_ID).Distinct().ToList();
                MOS.Filter.HisUserInvoiceBookFilter filter = new HisUserInvoiceBookFilter();
                filter.LOGINNAME = this._logginname;

                _UserInvoiceBookByLoginNames = new BackendAdapter(null).Get<List<HIS_USER_INVOICE_BOOK>>("api/HisUserInvoiceBook/Get", ApiConsumers.MosConsumer, filter, null);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
