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
using MOS.EFMODEL.DataModels;
using DevExpress.Utils.Menu;
using Inventec.Core;
using Inventec.Common.Adapter;
using HIS.Desktop.Controls.Session;
using Inventec.Common.RichEditor.Base;
using HIS.Desktop.Print;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using MOS.Filter;
using AutoMapper;
using ACS.Filter;
using ACS.EFMODEL.DataModels;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraGrid.Views.Grid;

namespace HIS.Desktop.Plugins.InvoiceBook
{
    public partial class frmCreateInvoice : Form
    {
        #region OverView------------------------------------------------------------------------------------------------------
        public V_HIS_INVOICE_BOOK _invoiceBookWitchUcBook = new V_HIS_INVOICE_BOOK();
        public V_HIS_INVOICE HisInvoiceInUc = new V_HIS_INVOICE();
        public List<HIS_PAY_FORM> _lisPayForms = new List<HIS_PAY_FORM>();
        private List<HIS_BRANCH> _hisBranch = new List<HIS_BRANCH>();
        private V_HIS_INVOICE _hisInvoiceFocusRowButtonPrint;
        private List<HIS_INVOICE_DETAIL_NEW> _listInvoiceDetailNews = new List<HIS_INVOICE_DETAIL_NEW>();
        private bool _isCalculatorPrice = true;
        private decimal _totalPrice;
        int positionHandleControl = -1;
        HIS_BRANCH branch = null;
        V_HIS_INVOICE invoices = null;
        public Inventec.Desktop.Common.Modules.Module currentModule;
        //UCInvoiceBook ucInvoiceBook = new UCInvoiceBook();
        #endregion

        public frmCreateInvoice()
        {
            InitializeComponent();
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnRefreshControl_Click(null, null);

        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (btnSaveInvoiceBook.Enabled == false)
                return;
            else
            {
                btnSaveInvoiceBook.Focus();
                btnSaveInvoiceBook_Click(null, null);
            }

        }

        //private void grvCreateInvoiceDetail_ShowingEditor(object sender, CancelEventArgs e)
        //{
        //    try
        //    {
        //        string error = GetError(grvCreateInvoiceDetail.FocusedRowHandle, grvCreateInvoiceDetail.FocusedColumn);
        //        if (error == string.Empty) return;
        //        grvCreateInvoiceDetail.SetColumnError(grvCreateInvoiceDetail.FocusedColumn, error, ErrorType.Warning);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}




        #region Print

        //private void AddControlToBtnPrint()
        //{
        //    try
        //    {

        //        DXPopupMenu menu = new DXPopupMenu();
        //        // In hóa đơn đỏ
        //        menu.Items.Add(new DXMenuItem("In hóa đơn đỏ", new EventHandler(OnClickInHoaDonDo)));
        //        menu.Items.Add(new DXMenuItem("In hóa đơn đỏ (Phân trang)", new EventHandler(OnClickInHoaDonChiTietDichVu)));
        //        cboPrint.DropDownControl = menu;
        //        cboPrint.Enabled = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        //private void OnClickInHoaDonDo(object sender, EventArgs e)
        //{
        //    CommonParam param = new CommonParam();
        //    try
        //    {
        //        Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
        //        store.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__InHoaDonDo_MPS000115, DeletegatePrintTemplate);


        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}


        private void OnClickInHoaDonChiTietDichVu(object sender, EventArgs e)
        {

            try
            {
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__InHoaDonDo_MPS000115, DeletegatePrintTemplate);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool DeletegatePrintTemplate(string printCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printCode)
                {
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__InHoaDonDo_MPS000115:
                        InHoaDonChiTietDichVu(printCode, fileName, ref result);
                        break;
                    //case PrintTypeCodeStore.PRINT_TYPE_CODE__IN_HOA_DON_CHITIET_DICHVU_MPS000152:
                    //InHoaDonDo(printCode, fileName, ref result);
                    //    break;                   
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        //private void InHoaDonDo(string printTypeCode, string fileName, ref bool result)
        //{
        //    CommonParam param = new CommonParam();
        //    bool success = false;
        //    List<HIS_PAY_FORM> payForm = new List<HIS_PAY_FORM>();
        //    try
        //    {
        //        //HisInvoiceFilter filter= new HisInvoiceFilter();
        //        //filter.ID= printInvoice.ID;
        //        //invoices= new BackendAdapter(param).Get<V_HIS_INVOICE>(ApiConsumer.HisRequestUriStore.HIS_INVOICE_GET__VIEW,ApiConsumer.ApiConsumers.MosConsumer,filter,null);       
        //        var _invoice = printInvoice;
        //        HisPayFormFilter filter = new HisPayFormFilter();
        //        filter.ID = _invoice.PAY_FORM_ID;
        //        payForm = new BackendAdapter(param).Get<List<HIS_PAY_FORM>>(HisRequestUriStore.HIS_PAY_FORM_GET, ApiConsumers.MosConsumer, filter, null);
        //        HisInvoiceDetailFilter detailFiter = new HisInvoiceDetailFilter();
        //        detailFiter.INVOICE_ID = _invoice.ID;
        //        var listDetail = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_INVOICE_DETAIL>>(ApiConsumer.HisRequestUriStore.HIS_INVOICE_DETAIL, ApiConsumers.MosConsumer, detailFiter, null);
        //        if (_invoice != null && listDetail != null)
        //        {
        //            MPS.Processor.Mps000115.PDO.Mps000115PDO rdo = new MPS.Processor.Mps000115.PDO.Mps000115PDO(_invoice, listDetail);
        //            result = MPS.Printer.Run(printTypeCode, fileName, rdo);             
        //        }               
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}


        private void InHoaDonChiTietDichVu(string printTypeCode, string fileName, ref bool result)
        {
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
                var _invoice = printInvoice;
                HisInvoiceDetailFilter detailFiter = new HisInvoiceDetailFilter();
                detailFiter.INVOICE_ID = _invoice.ID;
                //var listDetail = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_INVOICE_DETAIL>>(ApiConsumer.HisRequestUriStore.HIS_INVOICE_DETAIL, ApiConsumers.MosConsumer, detailFiter, null);
                string creatorUserName = "";
                AcsUserFilter userFilter = new AcsUserFilter();
                userFilter.LOGINNAME = printInvoice.CREATOR;
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
                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode("", printTypeCode, currentModule != null ? currentModule.RoomId : 0);
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
        }

        #endregion


        private void spinExemption_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinVatRatio.Focus();
                    spinVatRatio.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinVatRatio_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtBuyerName.Focus();
                    txtBuyerName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtBuyerName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtBuyerTaxCode.Focus();
                    txtBuyerTaxCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtBuyerTaxCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtBuyerAccountNumber.Focus();
                    txtBuyerAccountNumber.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtBuyerAccountNumber_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtBuyerOrganization.Focus();
                    txtBuyerOrganization.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtBuyerOrganization_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtBuyerAddress.Focus();
                    txtBuyerAddress.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtBuyerAddress_KeyDown(object sender, KeyEventArgs e)
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDescription_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtSellerName.Focus();
                    txtSellerName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSellerName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtSellerTaxCode.Focus();
                    txtSellerTaxCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSellerTaxCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtSellerAccountNumber.Focus();
                    txtSellerAccountNumber.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSellerAccountNumber_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtSellerPhone.Focus();
                    txtSellerPhone.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSellerPhone_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtSellerAddress.Focus();
                    txtSellerAddress.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSellerAddress_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    grvCreateInvoiceDetail.Focus();
                    //txtBuyerName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPrint_Click(object sender, EventArgs e)
        {
            try
            {
                OnClickInHoaDonChiTietDichVu(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spDonGia_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                try
                {
                    if (char.IsSymbol(e.KeyChar))
                    {
                        e.Handled = true;
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void spSoLuong_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (char.IsSymbol(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spChietKhau_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                try
                {
                    if (char.IsSymbol(e.KeyChar))
                    {
                        e.Handled = true;
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void grvCreateInvoiceDetail_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                if (view.FocusedColumn.FieldName == "AMOUNT")
                {
                    if (Inventec.Common.TypeConvert.Parse.ToDecimal(e.Value.ToString()) < 0)
                    {
                        e.Valid = false;
                        e.ErrorText = "Số lượng phải lớn hơn hoặc bằng 0";
                    }
                }
                if (view.FocusedColumn.FieldName == "PRICE")
                {
                    if (Inventec.Common.TypeConvert.Parse.ToDecimal(e.Value.ToString()) < 0)
                    {
                        e.Valid = false;
                        e.ErrorText = "Giá phải lớn hơn hoặc bằng 0";
                    }
                }
                if (view.FocusedColumn.FieldName == "DISCOUNT")
                {
                    if (Inventec.Common.TypeConvert.Parse.ToDecimal(e.Value.ToString()) < 0)
                    {
                        e.Valid = false;
                        e.ErrorText = "Chiết khấu phải lớn hơn hoặc bằng 0";
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void grvCreateInvoiceDetail_InvalidValueException(object sender, DevExpress.XtraEditors.Controls.InvalidValueExceptionEventArgs e)
        {
            try
            {
                //Loại xử lý khi xảy ra exception Hiển thị. k cho nhập
                e.ExceptionMode = DevExpress.XtraEditors.Controls.ExceptionMode.DisplayError;
                //Show thông báo lỗi ở cột
                grvCreateInvoiceDetail.SetColumnError(grvCreateInvoiceDetail.FocusedColumn, e.ErrorText, ErrorType.Warning);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                    if (currentModule == null) currentModule = new Inventec.Desktop.Common.Modules.Module();
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("Inventec.Desktop.Plugins.PrintLog", currentModule.RoomId, currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
