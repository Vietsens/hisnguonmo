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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid;
using HIS.Desktop.Plugins.InvoiceBook.Popup.AssignAuthorized;
using Inventec.Common.Adapter;
using Inventec.Common.RichEditor.Base;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;
using SAR.EFMODEL.DataModels;
using HIS.Desktop.Plugins.InvoiceBook.Popup.CancelInvoice;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Controls.Session;
using MOS.Filter;
using Inventec.Desktop.Common.LibraryMessage;
using DevExpress.Data;
using DevExpress.Utils.Menu;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;

namespace HIS.Desktop.Plugins.InvoiceBook
{
    public partial class UCInvoiceBook : HIS.Desktop.Utility.UserControlBase
    {
        internal int ActionType = 0;
        int positionHandle = -1;
        internal static int rowCount = 0;
        internal MOS.EFMODEL.DataModels.V_HIS_INVOICE_BOOK DataInvoiceBook { get; set; }
        #region Event_UC------------------------------------------------------------------------------------------------------
        private void UCInvoiceBook_Load(object sender, EventArgs e)
        {
            try
            {
                //InvoicePrintPageCFG.LoadConfig();
                SetCaptionByLanguageKey();
                ActionType = GlobalVariables.ActionAdd;
                ValidationControls();
                txtInvoiceBookName.Focus();
                txtInvoiceBookName.SelectAll();
                LoadAcsControls();
                LoadUserInvoiceBooks();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadAcsControls()
        {
            try
            {
                this.controlAcs = new List<ACS.EFMODEL.DataModels.ACS_CONTROL>();

                if (GlobalVariables.AcsAuthorizeSDO != null)
                {
                    this.controlAcs = GlobalVariables.AcsAuthorizeSDO.ControlInRoles;
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.InvoiceBook.Resources.Lang", typeof(HIS.Desktop.Plugins.InvoiceBook.UCInvoiceBook).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layout_Ct_Main.Text = Inventec.Common.Resource.Get.Value("UCInvoiceBook.layout_Ct_Main.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lo_Ct_Right.Text = Inventec.Common.Resource.Get.Value("UCInvoiceBook.lo_Ct_Right.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UCInvoiceBook.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn37.Caption = Inventec.Common.Resource.Get.Value("UCInvoiceBook.gridColumn37.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn38.Caption = Inventec.Common.Resource.Get.Value("UCInvoiceBook.gridColumn38.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn39.Caption = Inventec.Common.Resource.Get.Value("UCInvoiceBook.gridColumn39.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn40.Caption = Inventec.Common.Resource.Get.Value("UCInvoiceBook.gridColumn40.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn41.Caption = Inventec.Common.Resource.Get.Value("UCInvoiceBook.gridColumn41.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn42.Caption = Inventec.Common.Resource.Get.Value("UCInvoiceBook.gridColumn42.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn43.Caption = Inventec.Common.Resource.Get.Value("UCInvoiceBook.gridColumn43.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCInvoiceBook.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("UCInvoiceBook.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("UCInvoiceBook.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn16.Caption = Inventec.Common.Resource.Get.Value("UCInvoiceBook.gridColumn16.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn18.Caption = Inventec.Common.Resource.Get.Value("UCInvoiceBook.gridColumn18.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn19.Caption = Inventec.Common.Resource.Get.Value("UCInvoiceBook.gridColumn19.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn20.Caption = Inventec.Common.Resource.Get.Value("UCInvoiceBook.gridColumn20.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn21.Caption = Inventec.Common.Resource.Get.Value("UCInvoiceBook.gridColumn21.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn45.Caption = Inventec.Common.Resource.Get.Value("UCInvoiceBook.gridColumn45.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn22.Caption = Inventec.Common.Resource.Get.Value("UCInvoiceBook.gridColumn22.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn23.Caption = Inventec.Common.Resource.Get.Value("UCInvoiceBook.gridColumn23.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn24.Caption = Inventec.Common.Resource.Get.Value("UCInvoiceBook.gridColumn24.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn25.Caption = Inventec.Common.Resource.Get.Value("UCInvoiceBook.gridColumn25.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn26.Caption = Inventec.Common.Resource.Get.Value("UCInvoiceBook.gridColumn26.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn27.Caption = Inventec.Common.Resource.Get.Value("UCInvoiceBook.gridColumn27.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn28.Caption = Inventec.Common.Resource.Get.Value("UCInvoiceBook.gridColumn28.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn29.Caption = Inventec.Common.Resource.Get.Value("UCInvoiceBook.gridColumn29.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn30.Caption = Inventec.Common.Resource.Get.Value("UCInvoiceBook.gridColumn30.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn31.Caption = Inventec.Common.Resource.Get.Value("UCInvoiceBook.gridColumn31.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn32.Caption = Inventec.Common.Resource.Get.Value("UCInvoiceBook.gridColumn32.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn33.Caption = Inventec.Common.Resource.Get.Value("UCInvoiceBook.gridColumn33.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn34.Caption = Inventec.Common.Resource.Get.Value("UCInvoiceBook.gridColumn34.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn35.Caption = Inventec.Common.Resource.Get.Value("UCInvoiceBook.gridColumn35.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn36.Caption = Inventec.Common.Resource.Get.Value("UCInvoiceBook.gridColumn36.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearchInvoice.Text = Inventec.Common.Resource.Get.Value("UCInvoiceBook.btnSearchInvoice.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtSearchInvoice.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCInvoiceBook.txtSearchInvoice.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem20.Text = Inventec.Common.Resource.Get.Value("UCInvoiceBook.layoutControlItem20.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lo_Ct_Left.Text = Inventec.Common.Resource.Get.Value("UCInvoiceBook.lo_Ct_Left.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRefreshInvoiceBook.Text = Inventec.Common.Resource.Get.Value("UCInvoiceBook.btnRefreshInvoiceBook.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSaveInvoiceBook.Text = Inventec.Common.Resource.Get.Value("UCInvoiceBook.btnSaveInvoiceBook.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("UCInvoiceBook.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("UCInvoiceBook.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("UCInvoiceBook.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("UCInvoiceBook.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn46.Caption = Inventec.Common.Resource.Get.Value("UCInvoiceBook.gridColumn46.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("UCInvoiceBook.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("UCInvoiceBook.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("UCInvoiceBook.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("UCInvoiceBook.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn17.Caption = Inventec.Common.Resource.Get.Value("UCInvoiceBook.gridColumn17.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn44.Caption = Inventec.Common.Resource.Get.Value("UCInvoiceBook.gridColumn44.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("UCInvoiceBook.gridColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn13.Caption = Inventec.Common.Resource.Get.Value("UCInvoiceBook.gridColumn13.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn12.Caption = Inventec.Common.Resource.Get.Value("UCInvoiceBook.gridColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn15.Caption = Inventec.Common.Resource.Get.Value("UCInvoiceBook.gridColumn15.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn14.Caption = Inventec.Common.Resource.Get.Value("UCInvoiceBook.gridColumn14.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn47.Caption = Inventec.Common.Resource.Get.Value("UCInvoiceBook.gridColumn47.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem4.Text = Inventec.Common.Resource.Get.Value("UCInvoiceBook.layoutControlItem4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("UCInvoiceBook.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("UCInvoiceBook.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("UCInvoiceBook.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem9.Text = Inventec.Common.Resource.Get.Value("UCInvoiceBook.layoutControlItem9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("UCInvoiceBook.layoutControlItem8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("UCInvoiceBook.layoutControlItem7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandleControlEditor == -1)
                {
                    positionHandleControlEditor = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControlEditor > edit.TabIndex)
                {
                    positionHandleControlEditor = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSearchInvoice_Click(object sender, EventArgs e)
        {
            try
            {
                SearchInvoice();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSearchInvoice_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SearchInvoice();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        internal static void UpdateDataFormInvoiceBookToDTO(MOS.EFMODEL.DataModels.V_HIS_INVOICE_BOOK data, UCInvoiceBook control)
        {
            try
            {
                data.INVOICE_BOOK_NAME = control.txtInvoiceBookName.Text;
                data.TOTAL = (long)control.spTotal.Value;
                data.FROM_NUM_ORDER = (long)(control.spFromOrder.Value);
                data.DESCRIPTION = control.txtDescription.Text;
                data.SYMBOL_CODE = control.txtSymbolCode.Text.Trim();
                data.TEMPLATE_CODE = control.txtTemplateCode.Text.Trim();
                if (control.dtReleaseDate.EditValue != null && control.dtReleaseDate.DateTime != DateTime.MinValue)
                {
                    data.RELEASE_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(control.dtReleaseDate.EditValue).ToString("yyyyMMdd") + "000000");
                }
                else
                {
                    data.RELEASE_TIME = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSaveInvoiceBook_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            try
            {
                bool success = false;
                if (!btnSaveInvoiceBook.Enabled)
                    return;

                positionHandle = -1;
                if (!dxValidationProvider1.Validate())
                    return;
                this.DataInvoiceBook = new MOS.EFMODEL.DataModels.V_HIS_INVOICE_BOOK();
                UpdateDataFormInvoiceBookToDTO(this.DataInvoiceBook, this);
                if (ActionType == GlobalVariables.ActionAdd)
                {
                    WaitingManager.Show();
                    MOS.EFMODEL.DataModels.HIS_INVOICE_BOOK aInvoiceBook = new MOS.EFMODEL.DataModels.HIS_INVOICE_BOOK();
                    Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_INVOICE_BOOK>(aInvoiceBook, this.DataInvoiceBook);

                    aInvoiceBook = new BackendAdapter(param).Post<HIS_INVOICE_BOOK>(ApiConsumer.HisRequestUriStore.HIS_INVOICE_BOOK_CREATE, ApiConsumer.ApiConsumers.MosConsumer, aInvoiceBook, null);

                    if (aInvoiceBook != null)
                    {
                        success = true;
                        FillDataToControl(aInvoiceBook, this);
                        FillDataToGridInvoiceBook(this);
                        //txtAccountBookCode.Text = this.DataInvoiceBook.ACCOUNT_BOOK_CODE;
                        //txtAccountBookName.Focus();
                        //txtAccountBookName.SelectAll();
                        this.ActionType = GlobalVariables.ActionView;
                        EnableButton(GlobalVariables.ActionView, this);
                    }
                    WaitingManager.Hide();
                    gctInvoice.DataSource = null;
                    _listInvoices = null;
                }
                else if (ActionType == GlobalVariables.ActionEdit)
                {
                    MOS.EFMODEL.DataModels.HIS_INVOICE_BOOK aInvoiceBook = new MOS.EFMODEL.DataModels.HIS_INVOICE_BOOK();
                    Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_INVOICE_BOOK>(aInvoiceBook, this.DataInvoiceBook);
                    var resultData = new BackendAdapter(param).Post<HIS_INVOICE_BOOK>(HisRequestUriStore.HIS_INVOICE_BOOK_UPDATE, ApiConsumer.ApiConsumers.MosConsumer, aInvoiceBook, null);
                    if (resultData != null)
                    {
                        success = true;
                        aInvoiceBook = resultData;
                        FillDataToControl(aInvoiceBook, this);
                        LoadDataInvoiceBook(param);
                        //txtAccountBookName.Focus();
                        //txtAccountBookName.SelectAll();
                        this.ActionType = GlobalVariables.ActionView;
                        EnableButton(GlobalVariables.ActionView, this);
                    }
                    else
                    {
                        MOS.Filter.HisInvoiceBookFilter filter = new MOS.Filter.HisInvoiceBookFilter();
                        filter.ID = this.DataInvoiceBook.ID;

                        var list = new BackendAdapter(param).Get<List<HIS_INVOICE_BOOK>>(ApiConsumer.HisRequestUriStore.HIS_INVOICE_BOOK_GET__VIEW, ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                        if (list != null && list.Count > 0)
                        {
                            Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_INVOICE_BOOK>(this.DataInvoiceBook, list[0]);
                            var oldData = _listInvoiceBooks.FirstOrDefault(o => o.ID == list[0].ID);
                            if (oldData != null)
                            {
                                oldData.FROM_NUM_ORDER = list[0].FROM_NUM_ORDER;
                                oldData.TOTAL = list[0].TOTAL;
                            }
                        }
                    }
                }
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
                Inventec.Common.Logging.LogSystem.Fatal(ex);
                Inventec.Desktop.Common.LibraryMessage.MessageUtil.SetParam(param, Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongTBXuatHienExceptionChuaKiemDuocSoat);
            }
        }

        private void FillDataToGridInvoice(MOS.EFMODEL.DataModels.V_HIS_INVOICE_BOOK dataInvoice, UCInvoiceBook control)
        {
            try
            {

                HisInvoiceFilter filter = new HisInvoiceFilter();
                CommonParam param = new CommonParam();
                filter.KEY_WORD = control.txtSearchInvoice.Text;
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                filter.INVOICE_BOOK_ID = dataInvoice.ID;
                _listInvoices = new BackendAdapter(param).Get<List<V_HIS_INVOICE>>(ApiConsumer.HisRequestUriStore.HIS_INVOICE_GET__VIEW, ApiConsumer.ApiConsumers.MosConsumer, filter, null);
                control.gctInvoice.DataSource = _listInvoices;
                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal static void EnableButton(int action, UCInvoiceBook control)
        {
            try
            {

                control.btnSaveInvoiceBook.Enabled = (action == GlobalVariables.ActionAdd || action == GlobalVariables.ActionEdit);
                control.btnRefreshInvoiceBook.Enabled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal static void FillDataToControl(MOS.EFMODEL.DataModels.HIS_INVOICE_BOOK data, UCInvoiceBook control)
        {
            try
            {
                if (data != null)
                {
                    control.txtInvoiceBookName.Text = data.INVOICE_BOOK_NAME;
                    control.spFromOrder.Value = data.FROM_NUM_ORDER;
                    control.spTotal.Value = data.TOTAL;
                    control.txtDescription.Text = data.DESCRIPTION;
                    control.txtSymbolCode.Text = data.SYMBOL_CODE;
                    control.txtTemplateCode.Text = data.TEMPLATE_CODE;
                    control.dtReleaseDate.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.RELEASE_TIME ?? 0);
                }
                else
                {
                    control.txtInvoiceBookName.Text = "";
                    control.spFromOrder.Value = 0;
                    control.spTotal.Value = 0;
                    control.txtDescription.Text = "";
                    control.txtSymbolCode.Text = "";
                    control.txtTemplateCode.Text = "";
                    control.dtReleaseDate.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnRefreshInvoiceBook_Click(object sender, EventArgs e)
        {
            try
            {
                RefreshControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion End_Event_UC

        #region Event_InvoiceBook---------------------------------------------------------------------------------------------

        private void gctInvoiceBook_Load(object sender, EventArgs e)
        {
            try
            {
                _listUserInvoiceBooks = new List<V_HIS_USER_INVOICE_BOOK>();
                var result = UserInvoiceBookGetData();
                if (result != null || result.Any())
                    _listUserInvoiceBooks.AddRange(result);

                SetDataToInvoiceBook();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void grvInvoiceBook_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.V_HIS_INVOICE_BOOK data = (MOS.EFMODEL.DataModels.V_HIS_INVOICE_BOOK)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;

                        if (e.Column.FieldName == "STT")
                        {
                            try
                            {
                                e.Value = e.ListSourceRowIndex + 1 + startPage;
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho truong STT", ex);
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
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho truong CREATE_TIME", ex);
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
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho truong MODIFY_TIME", ex);
                            }

                        }
                        else if (e.Column.FieldName == "RELEASE_TIME_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.RELEASE_TIME ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho truong RELEASE_TIME", ex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void grvInvoiceBook_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                MOS.EFMODEL.DataModels.V_HIS_INVOICE_BOOK data = null;
                if (e.RowHandle > -1)
                {
                    data = (MOS.EFMODEL.DataModels.V_HIS_INVOICE_BOOK)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                }
                if (e.RowHandle >= 0)
                {
                    if (e.Column.FieldName == "AUTHORIZE")
                    {
                        try
                        {
                            if (data.CREATOR == _logginname)
                            {
                                e.RepositoryItem = btnAuthorizedInvoiceBookE;
                            }
                            else
                            {
                                e.RepositoryItem = btnAuthorizedInvoiceBookD;
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn(ex);
                        }
                    }
                    if (e.Column.FieldName == "DELETE")
                    {
                        try
                        {
                            if (data.CREATOR == _logginname)
                            {
                                e.RepositoryItem = deleteE;
                            }
                            else
                            {
                                e.RepositoryItem = deleteD;
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn(ex);
                        }
                    }
                    if (e.Column.FieldName == "CREATE")
                    {
                        try
                        {
                            e.RepositoryItem = btnCreateInvoiceBookE;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void grvInvoiceBook_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            try
            {
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

        #endregion End_Event_InvoiceBook

        #region Event_Invoice-------------------------------------------------------------------------------------------------

        private void grvInvoice_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                var data = (V_HIS_INVOICE)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                if (data == null) return;
                switch (e.Column.FieldName)
                {
                    case "DELETE":
                        if (data.CREATOR == _logginname && data.IS_CANCEL != 1)
                            e.RepositoryItem = btnDeleteInvoiceE;
                        else
                            e.RepositoryItem = btnDeleteInvoiceD;
                        break;
                    case "PRINT_INVOICE":
                        if ((data.CREATOR == _logginname
                            || (_UserInvoiceBookByLoginNames != null
                            && _UserInvoiceBookByLoginNames.Exists(p => p.INVOICE_BOOK_ID == data.INVOICE_BOOK_ID)
                            )
                            )
                            && data.IS_CANCEL != 1
                            )
                            e.RepositoryItem = btnPrintInvoiceE;
                        else
                            e.RepositoryItem = btnPrintInvoiceD;
                        break;
                    case "EDIT_INVOICE":
                        if (data.IS_ACTIVE == 1
                                && data.IS_CANCEL != 1
                                && (data.CREATOR == this._logginname
                                || HIS.Desktop.IsAdmin.CheckLoginAdmin.IsAdmin(this._logginname))
                                && (controlAcs != null && controlAcs.Exists(o => o.CONTROL_CODE == "HIS000018")))
                        {
                            e.RepositoryItem = repositoryItemButtonEdit__E;
                        }
                        else
                            e.RepositoryItem = repositoryItemButtonEdit__D;
                        //if (data.CREATOR == _logginname && data.IS_CANCEL != 1)
                        //    e.RepositoryItem = repositoryItemButtonEdit__E;
                        //else
                        //    e.RepositoryItem = repositoryItemButtonEdit__D;
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void grvInvoice_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (!e.IsGetData || e.Column.UnboundType == DevExpress.Data.UnboundColumnType.Bound) return;
                var data = (V_HIS_INVOICE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                if (data == null) return;
                switch (e.Column.FieldName)
                {
                    case "NUMBER_ORDER":
                        e.Value = e.ListSourceRowIndex + 1 + startPage2;
                        break;
                    case "INVOICE_TIME_STR":
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.INVOICE_TIME);
                        break;
                    case "CREATE_TIME_STR":
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        break;
                    case "MODIFY_TIME_STR":
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void grvInvoice_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            try
            {
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

        private void btnPrintInvoiceE_Click(object sender, EventArgs e)
        {
            try
            {
                //_hisInvoiceFocusRowButtonPrint = (V_HIS_INVOICE)grvInvoice.GetFocusedRow();
                //var barManager = new BarManager { Form = this.gctInvoice };
                //ShowMenuWhenClickMouse(ClickPrintMenuItemButton_Click, barManager);

                ClickPrintInvoiceOrder();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnDeleteInvoiceE_Click(object sender, EventArgs e)
        {
            try
            {
                var dataFocuseRow = (V_HIS_INVOICE)grvInvoice.GetFocusedRow();
                var deleteInvoice = new frmCancelInvoice(dataFocuseRow, delegateSelectData);
                deleteInvoice.currentModule = Module;
                deleteInvoice.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void delegateSelectData()
        {
            try
            {
                SearchInvoice();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion End_Event_Invoice

        #region Event_InvoicDetail--------------------------------------------------------------------------------------------

        private void grvInvoiceDetail_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (!e.IsGetData || e.Column.UnboundType == DevExpress.Data.UnboundColumnType.Bound) return;
                var data = (HIS_INVOICE_DETAIL)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                if (data == null) return;
                switch (e.Column.FieldName)
                {
                    case "NUMBER_ORDER":
                        e.Value = e.ListSourceRowIndex + 1 + startPage3;
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion End_Event_InvoiceDetail



    }
}
