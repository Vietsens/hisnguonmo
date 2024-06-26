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
using Inventec.Common.RichEditor.Base;
using HIS.Desktop.Controls.Session;
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using HIS.Desktop.ApiConsumer;
using Inventec.Desktop.Common.Controls.ValidationRule;
using DevExpress.XtraEditors;
using HIS.Desktop.LibraryMessage;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;


namespace HIS.Desktop.Plugins.InvoiceBook.Popup.CancelInvoice
{
    public partial class frmCancelInvoice : Form
    {
        V_HIS_INVOICE _invoiceWithUC = new V_HIS_INVOICE();
        HIS.Desktop.Plugins.InvoiceBook.UCInvoiceBook ucInvoiceBook;
        int positionHandleControlEditor = -1;
        HIS.Desktop.Common.DelegateRefreshData refreshData;
        public Inventec.Desktop.Common.Modules.Module currentModule;

        public frmCancelInvoice(V_HIS_INVOICE vHisInvoice, HIS.Desktop.Common.DelegateRefreshData _refreshData)
        {
            try
            {
                InitializeComponent();
                this._invoiceWithUC = vHisInvoice;
                this.refreshData = _refreshData;
                ValidateForm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.InvoiceBook.Resources.Lang", typeof(HIS.Desktop.Plugins.InvoiceBook.Popup.CancelInvoice.frmCancelInvoice).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmCancelInvoice.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmCancelInvoice.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("frmCancelInvoice.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("frmCancelInvoice.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem4.Text = Inventec.Common.Resource.Get.Value("frmCancelInvoice.layoutControlItem4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("frmCancelInvoice.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("frmCancelInvoice.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("frmCancelInvoice.layoutControlItem7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem9.Text = Inventec.Common.Resource.Get.Value("frmCancelInvoice.layoutControlItem9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmCancelInvoice.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barbtnSave.Caption = Inventec.Common.Resource.Get.Value("frmCancelInvoice.barbtnSave.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmCancelInvoice.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            bool result = false;
            try
            {
                positionHandleControlEditor = -1;
                if (!dxValidationProvider1.Validate())
                    return;
                WaitingManager.Show();
                V_HIS_INVOICE devHisInvoice = new V_HIS_INVOICE();
                Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_INVOICE>(devHisInvoice, this._invoiceWithUC);
                devHisInvoice.CANCEL_REASON = txtReason.Text;
                //xem lai.........
                var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<V_HIS_INVOICE>("api/HisInvoice/Cancel", ApiConsumers.MosConsumer, devHisInvoice, param);
                WaitingManager.Hide();
                if (rs != null)
                {
                    result = true;
                    UCInvoiceBook ucinvoicebook = new UCInvoiceBook(currentModule);
                    ucinvoicebook.SearchInvoice();
                    this.refreshData();
                    this.Close();
                    //ucInvoiceBook.MeShow();
                }
                #region Show message

                MessageManager.Show(this, param, result);
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


        private void frmCancelInvoice_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                labelControl1.Text = _invoiceWithUC.NUM_ORDER.ToString();
                labelControl3.Text = _invoiceWithUC.SYMBOL_CODE.ToString();
                labelControl4.Text = _invoiceWithUC.TEMPLATE_CODE.ToString();
                labelControl5.Text = _invoiceWithUC.SELLER_NAME.ToString();
                labelControl6.Text = _invoiceWithUC.BUYER_NAME.ToString();
                labelControl7.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(_invoiceWithUC.INVOICE_TIME);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }

        }

        private void barbtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSave_Click(null, null);
        }

        private void ValidateForm()
        {
            try
            {
                ValidateSingleControls(txtReason);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateSingleControls(BaseEdit control)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, ValidationFailedEventArgs e)
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



    }
}
