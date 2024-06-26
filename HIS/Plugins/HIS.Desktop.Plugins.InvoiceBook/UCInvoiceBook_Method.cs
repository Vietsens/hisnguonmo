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
using DevExpress.XtraBars;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.InvoiceBook.Base;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.Print;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Common.Adapter;
using ACS.EFMODEL.DataModels;
using ACS.Filter;

namespace HIS.Desktop.Plugins.InvoiceBook
{
    public partial class UCInvoiceBook : HIS.Desktop.Utility.UserControlBase
    {
        #region Method_UC-----------------------------------------------------------------------------------------------------

        private void ValidateControl()
        {

        }

        private void RemoveControlError()
        {
            try
            {
                txtInvoiceBookName.Focus();
                dxValidationProvider1.RemoveControlError(spTotal);
                dxValidationProvider1.RemoveControlError(spFromOrder);
                dxValidationProvider1.RemoveControlError(txtSymbolCode);
                dxValidationProvider1.RemoveControlError(txtTemplateCode);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void RefreshControl()
        {
            try
            {
                WaitingManager.Show();
                this.ActionType = GlobalVariables.ActionAdd;
                DataInvoiceBook = new MOS.EFMODEL.DataModels.V_HIS_INVOICE_BOOK();
                EnableButton(GlobalVariables.ActionAdd, this);
                FillDataToControl(null, this);
                positionHandle = -1;
                RemoveControlError();
                WaitingManager.Hide();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion End_Method_UC

        #region Method_InvoiceBook--------------------------------------------------------------------------------------------

        internal static void EnableControlEdit(int action, UCInvoiceBook control)
        {
            try
            {
                control.txtDescription.ReadOnly = (action == GlobalVariables.ActionView);
                control.spFromOrder.ReadOnly = (action == GlobalVariables.ActionView);
                control.spTotal.ReadOnly = (action == GlobalVariables.ActionView);
                control.txtSymbolCode.ReadOnly = (action == GlobalVariables.ActionView);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDataToInvoiceBook()
        {
            try
            {
                if (ucPaging1.pagingGrid != null)
                {
                    pageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    pageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }
                LoadDataInvoiceBook(new CommonParam(0, pageSize));
                var param = new CommonParam
                {
                    Limit = _rowCountInvoiceBook,
                    Count = _dataTotalInvoiceBook
                };
                ucPaging1.Init(LoadDataInvoiceBook, param, pageSize, gctInvoiceBook);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataInvoiceBook(object param)
        {
            try
            {
                _listInvoiceBooks = new List<V_HIS_INVOICE_BOOK>();
                gctInvoiceBook.DataSource = null;
                var result = InvoiceBookGetDatas(param);
                if (result == null) return;
                _listInvoiceBooks = result.Data;
                _rowCountInvoiceBook = _listInvoiceBooks == null ? 0 : _listInvoiceBooks.Count;
                _dataTotalInvoiceBook = (result.Param == null ? 0 : result.Param.Count ?? 0);
                gctInvoiceBook.BeginUpdate();
                gctInvoiceBook.DataSource = _listInvoiceBooks;
                gctInvoiceBook.EndUpdate();

                grvInvoiceBook.Focus();
                rowGridViewInvoiceBookFocus = (V_HIS_INVOICE_BOOK)grvInvoiceBook.GetFocusedRow();
                if (rowGridViewInvoiceBookFocus == null)
                {
                    gctInvoice.DataSource = null;
                    gctInvoiceDetail.DataSource = null;
                    return;
                }
                SetDataToInvoice();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion End_Method_InvoiceBook

        #region Method_Invoice------------------------------------------------------------------------------------------------

        private void SetDataToInvoice()
        {
            try
            {
                if (ucPaging2.pagingGrid != null)
                {
                    pageSize = ucPaging2.pagingGrid.PageSize;
                }
                else
                {
                    pageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }
                LoadDataInvoice(new CommonParam(0, pageSize));
                var param = new CommonParam
                {
                    Limit = _rowCountInvoice,
                    Count = _dataTotalInvoice
                };
                ucPaging2.Init(LoadDataInvoice, param, pageSize, gctInvoice);

                gctInvoice.Focus();
                rowGridViewInvoiceFocus = (V_HIS_INVOICE)grvInvoice.GetFocusedRow();
                if (rowGridViewInvoiceFocus == null)
                {
                    gctInvoiceDetail.DataSource = null;
                    return;
                }
                SetDataToInvoiceDetai();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataInvoice(object param)
        {
            _listInvoices = new List<V_HIS_INVOICE>();
            gctInvoice.DataSource = null;
            var result = InvoiceGetDatas(rowGridViewInvoiceBookFocus, param);
            if (result == null) return;
            _listInvoices = result.Data;
            _rowCountInvoice = _listInvoices == null ? 0 : _listInvoices.Count;
            _dataTotalInvoice = (result.Param == null ? 0 : result.Param.Count ?? 0);
            gctInvoice.BeginUpdate();
            gctInvoice.DataSource = _listInvoices;
            gctInvoice.EndUpdate();
        }

        #endregion End_Method_Invoice

        #region Method_InvoiceDetail------------------------------------------------------------------------------------------

        private void SetDataToInvoiceDetai()
        {
            try
            {
                if (ucPaging3.pagingGrid != null)
                {
                    pageSize = ucPaging3.pagingGrid.PageSize;
                }
                else
                {
                    pageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }
                LoadDataInvoiceDetail(new CommonParam(0, pageSize));
                var param = new CommonParam
                {
                    Limit = _rowCountInvoiceDetail,
                    Count = _dataTotalInvoiceDetail
                };
                ucPaging3.Init(LoadDataInvoiceDetail, param, pageSize,gctInvoiceDetail);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataInvoiceDetail(object param)
        {
            _listInvoiceDetails = new List<HIS_INVOICE_DETAIL>();
            gctInvoiceDetail.DataSource = null;
            var result = InvoiceDetailGetDatas(rowGridViewInvoiceFocus, param);
            if (result == null) return;
            _listInvoiceDetails = result.Data;
            _rowCountInvoiceDetail = _listInvoiceDetails == null ? 0 : _listInvoiceDetails.Count;
            _dataTotalInvoiceDetail = (result.Param == null ? 0 : result.Param.Count ?? 0);
            gctInvoiceDetail.BeginUpdate();
            gctInvoiceDetail.DataSource = _listInvoiceDetails;
            gctInvoiceDetail.EndUpdate();
        }

        #endregion End_Method_InvoiceDetail

        #region Method_MenuClickMouse-----------------------------------------------------------------------------------------

        private void ShowMenuWhenClickMouse(ClickPrintMenuItemButton_Click treatmentMouseRightClick, BarManager barManager)
        {
            try
            {
                var menu = new PopupMenu(barManager);
                menu.ItemLinks.Clear();

                var printInvoice = new BarButtonItem(barManager, "In hóa đơn đỏ", 8) { Tag = MenuPrintType.PrintInvoice };
                printInvoice.ItemClick += new ItemClickEventHandler(treatmentMouseRightClick);
                menu.AddItems(new BarItem[] { printInvoice });

                var printInvoiceOrder = new BarButtonItem(barManager, "In hóa đơn đỏ(Phân trang)", 8) { Tag = MenuPrintType.PrintInvoiceOrder };
                printInvoiceOrder.ItemClick += new ItemClickEventHandler(treatmentMouseRightClick);
                menu.AddItems(new BarItem[] { printInvoiceOrder });

                menu.ShowPopup(Cursor.Position);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void ClickPrintMenuItemButton_Click(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (!(e.Item is BarButtonItem)) return;
                var type = (MenuPrintType)(e.Item.Tag);
                switch (type)
                {
                    case MenuPrintType.PrintInvoice:
                        ClickPrintInvoice();
                        break;
                    case MenuPrintType.PrintInvoiceOrder:
                        ClickPrintInvoiceOrder();
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ClickPrintInvoiceOrder()
        {
            try
            {
                var richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__InHoaDonDo_MPS000115, delegateRunPrintTemplte1);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ClickPrintInvoice()
        {
            try
            {
                var richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__InHoaDonDo_MPS000115, delegateRunPrintTemplte);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool delegateRunPrintTemplte(string printTypeCode, string fileName)
        {
            bool result = false;
            List<HIS_PAY_FORM> payForm = new List<HIS_PAY_FORM>();
            try
            {
                if (this._listInvoices == null)
                    return false;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                var invoice1 = (V_HIS_INVOICE)grvInvoice.GetFocusedRow();
                HisPayFormFilter filter = new HisPayFormFilter();
                filter.ID = invoice1.PAY_FORM_ID;
                payForm = new BackendAdapter(param).Get<List<HIS_PAY_FORM>>(HisRequestUriStore.HIS_PAY_FORM_GET, ApiConsumers.MosConsumer, filter, null);
                HisInvoiceDetailFilter detailFiter = new HisInvoiceDetailFilter();
                detailFiter.INVOICE_ID = invoice1.ID;
                var listDetail = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_INVOICE_DETAIL>>(ApiConsumer.HisRequestUriStore.HIS_INVOICE_DETAIL, ApiConsumers.MosConsumer, detailFiter, null);
                if (invoice1 != null && listDetail != null)
                {
                    MPS.Processor.Mps000115.PDO.Mps000115PDO rdo = new MPS.Processor.Mps000115.PDO.Mps000115PDO(invoice1, listDetail);
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode("", printTypeCode, Module != null ? Module.RoomId : 0);
                    PrintData.ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog;
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                if (result)
                {
                    HIS_INVOICE_PRINT print = new HIS_INVOICE_PRINT();
                    print.INVOICE_ID = invoice1.ID;
                    print.PRINT_TIME = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                    print.LOGINNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_INVOICE_PRINT>(ApiConsumer.HisRequestUriStore.HIS_INVOICE_PRINT_CREATE, ApiConsumers.MosConsumer, print, null);
                    if (rs == null)
                    {
                        Inventec.Common.Logging.LogSystem.Info("Tao du lieu HisInvoicePrint that bai khi in hoa don do: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => invoice1), invoice1));
                    }
                }
                WaitingManager.Hide();
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private bool delegateRunPrintTemplte1(string printTypeCode, string fileName)
        {
            bool result = false;
            CommonParam param = new CommonParam();
            bool success = false;
            List<MPS.Processor.Mps000115.PDO.InvoiceDetailADO> invoiceADO = new List<MPS.Processor.Mps000115.PDO.InvoiceDetailADO>();
            List<HIS_PAY_FORM> payForm = new List<HIS_PAY_FORM>();
            try
            {
                long rowCountGridFirstPage = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<long>("HIS.Desktop.Plugins.InvoiceBook.rowCountGridFirstPage"), rowCountGridNextPage = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<long>("HIS.Desktop.Plugins.InvoiceBook.rowCountGridNextPage"), totalNextPages = 0;
                List<HIS_INVOICE_DETAIL> invoiceDetails = new List<HIS_INVOICE_DETAIL>();
                //HisInvoiceFilter filter= new HisInvoiceFilter();
                //filter.ID= printInvoice.ID;
                //invoices= new BackendAdapter(param).Get<V_HIS_INVOICE>(ApiConsumer.HisRequestUriStore.HIS_INVOICE_GET__VIEW,ApiConsumer.ApiConsumers.MosConsumer,filter,null);
                //AutoMapper.Mapper.CreateMap<HIS_INVOICE, V_HIS_INVOICE>();
                var _invoice = (V_HIS_INVOICE)grvInvoice.GetFocusedRow(); ;
                HisInvoiceDetailFilter detailFiter = new HisInvoiceDetailFilter();
                detailFiter.INVOICE_ID = _invoice.ID;
                //var listDetail = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_INVOICE_DETAIL>>(ApiConsumer.HisRequestUriStore.HIS_INVOICE_DETAIL, ApiConsumers.MosConsumer, detailFiter, null);
                string creatorUserName = "";
                AcsUserFilter userFilter = new AcsUserFilter();
                userFilter.LOGINNAME = _invoice.CREATOR;
                var getUser = new BackendAdapter(param).Get<List<ACS_USER>>(AcsRequestUriStore.ACS_USER_GET, ApiConsumers.AcsConsumer, userFilter, null);
                if (getUser != null && getUser.Count > 0)
                {
                    creatorUserName = getUser.FirstOrDefault().USERNAME;
                }

                List<string> titles = new List<string>();
                titles.Add("Liên 1: Lưu");
                titles.Add("Liên 2: Giao người mua");

                if (_invoice != null)
                {
                    HisPayFormFilter filter = new HisPayFormFilter();
                    filter.ID = _invoice.PAY_FORM_ID;
                    payForm = new BackendAdapter(param).Get<List<HIS_PAY_FORM>>(HisRequestUriStore.HIS_PAY_FORM_GET, ApiConsumers.MosConsumer, filter, null);

                    MOS.Filter.HisInvoiceDetailFilter invoiceDetailFilter = new MOS.Filter.HisInvoiceDetailFilter();
                    invoiceDetailFilter.INVOICE_ID = _invoice.ID;
                    invoiceDetails = new BackendAdapter(param).Get<List<HIS_INVOICE_DETAIL>>(HisRequestUriStore.HIS_INVOICE_DETAIL_GET, ApiConsumers.MosConsumer, invoiceDetailFilter, null);
                    // tong so trang
                    totalNextPages = (invoiceDetails.Count - rowCountGridFirstPage) / rowCountGridNextPage + 2;
                    for (int i = 0; i < invoiceDetails.Count; i++)
                    {
                        MPS.Processor.Mps000115.PDO.InvoiceDetailADO _invoiceAdo = new MPS.Processor.Mps000115.PDO.InvoiceDetailADO();
                        AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.HIS_INVOICE_DETAIL, MPS.Processor.Mps000115.PDO.InvoiceDetailADO>();
                        _invoiceAdo = AutoMapper.Mapper.Map<MOS.EFMODEL.DataModels.HIS_INVOICE_DETAIL, MPS.Processor.Mps000115.PDO.InvoiceDetailADO>(invoiceDetails[i]);
                        if (i < rowCountGridFirstPage)
                        {
                            _invoiceAdo.PageId = 0;
                        }
                        else
                        {
                            _invoiceAdo.PageId = ((i - rowCountGridFirstPage) < 0 ? 0 : (i - rowCountGridFirstPage)) / rowCountGridNextPage + 1;
                        }
                        _invoiceAdo.NUM_ORDER = i + 1;
                        invoiceADO.Add(_invoiceAdo);
                    }
                }


                List<MPS.Processor.Mps000115.PDO.TotalNextPage> totalADO = new List<MPS.Processor.Mps000115.PDO.TotalNextPage>();
                List<long> pageIds = invoiceADO.Select(o => o.PageId ?? 0).ToList();


                for (int i = 0; i < totalNextPages; i++)
                {
                    MPS.Processor.Mps000115.PDO.TotalNextPage totalNextPage = new MPS.Processor.Mps000115.PDO.TotalNextPage();
                    if (i == 0)
                    {
                        totalNextPage.id = 0;
                        totalNextPage.Name = "";
                    }
                    else
                    {
                        totalNextPage.id = i;
                        totalNextPage.Name = "Tiep theo trang truoc - trang " + (i + 1) + "/" + (totalNextPages);
                    }

                    var containItem = pageIds.Contains(totalNextPage.id);
                    if (containItem != true)
                    {
                        totalNextPage.Name = "";
                    }
                    totalADO.Add(totalNextPage);

                }


                if (_invoice != null && invoiceDetails != null)
                {

                    MPS.Processor.Mps000115.PDO.Mps000115PDO rdo = new MPS.Processor.Mps000115.PDO.Mps000115PDO(_invoice, invoiceDetails, invoiceADO, totalADO, payForm, creatorUserName);
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode("", printTypeCode, Module != null ? Module.RoomId : 0);
                    PrintData.ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog;
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                    if (result == true)
                    {

                        HIS_INVOICE_PRINT print = new HIS_INVOICE_PRINT();
                        print.INVOICE_ID = _invoice.ID;
                        print.PRINT_TIME = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                        print.LOGINNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                        var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_INVOICE_PRINT>(ApiConsumer.HisRequestUriStore.HIS_INVOICE_PRINT_CREATE, ApiConsumers.MosConsumer, print, null);
                        if (rs == null)
                        {
                            Inventec.Common.Logging.LogSystem.Info("Tao du lieu HisInvoicePrint that bai khi in hoa don do: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _invoice), _invoice));
                        }

                    }
                    //success = true;                  
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        #endregion

        public void SearchInvoice()
        {
            try
            {
                if (string.IsNullOrEmpty(txtSearchInvoice.Text.Trim()))
                {
                    txtSearchInvoice.Text = null;
                    LoadDataInvoice(new CommonParam(0, 100));

                }
                else
                {
                    SetDataToInvoice();
                    LoadDataInvoice(new CommonParam(0, 100));

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CallModuleShowPrintLog(string printTypeCode, string uniqueCode)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(printTypeCode) && !String.IsNullOrWhiteSpace(uniqueCode))
                {
                    //goi modul
                    HIS.Desktop.ADO.PrintLogADO ado = new HIS.Desktop.ADO.PrintLogADO(printTypeCode, uniqueCode);

                    List<object> listArgs = new List<object>();
                    listArgs.Add(ado);
                    if (Module == null) Module = new Inventec.Desktop.Common.Modules.Module();

                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("Inventec.Desktop.Plugins.PrintLog", Module.RoomId, Module.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
