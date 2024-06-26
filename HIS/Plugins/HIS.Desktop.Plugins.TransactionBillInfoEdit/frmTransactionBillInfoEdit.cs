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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TransactionBillInfoEdit
{
    public partial class frmTransactionBillInfoEdit : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module currentModule = null;
        V_HIS_INVOICE _HisInvoice = null;
        HIS.Desktop.Common.DelegateRefreshData _dlg = null;

        public frmTransactionBillInfoEdit(Inventec.Desktop.Common.Modules.Module module, V_HIS_INVOICE _Invoice)
            : base(module)
        {
            InitializeComponent();
            try
            {
                SetIcon();

                this.currentModule = module;
                this._HisInvoice = _Invoice;
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

        public frmTransactionBillInfoEdit(Inventec.Desktop.Common.Modules.Module module, V_HIS_INVOICE _transaction, HIS.Desktop.Common.DelegateRefreshData dlg)
            : base(module)
        {
            InitializeComponent();
            try
            {
                SetIcon();

                this.currentModule = module;
                this._HisInvoice = _transaction;
                this._dlg = dlg;
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

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmTransactionBillInfoEdit_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                SetDataDefault();
                SetValidate();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDataDefault()
        {
            try
            {
                if (this._HisInvoice != null)
                {
                    this.txtNguoiMua.Text = this._HisInvoice.BUYER_NAME;
                    this.txtDiaChiNguoiMua.Text = this._HisInvoice.BUYER_ADDRESS;
                    this.txtSTKNguoiMua.Text = this._HisInvoice.BUYER_ACCOUNT_NUMBER;
                    this.txtMaSoThue.Text = this._HisInvoice.BUYER_TAX_CODE;
                    this.txtDonVi.Text = this._HisInvoice.BUYER_ORGANIZATION;
                }
                else
                {
                    this.txtNguoiMua.Text = "";
                    this.txtDiaChiNguoiMua.Text = "";
                    this.txtSTKNguoiMua.Text = "";
                    this.txtMaSoThue.Text = "";
                    this.txtDonVi.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtNguoiMua_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDiaChiNguoiMua.Focus();
                    txtDiaChiNguoiMua.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDiaChiNguoiMua_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtSTKNguoiMua.Focus();
                    txtSTKNguoiMua.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSTKNguoiMua_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtMaSoThue.Focus();
                    txtMaSoThue.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtMaSoThue_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDonVi.Focus();
                    txtDonVi.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDonVi_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSave.Focus();
                }
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
                btnSave.Focus();

                this.positionHandle = -1;
                if (!dxValidationProvider1.Validate())
                    return;

                bool success = false;
                WaitingManager.Show();

                MOS.SDO.HisInvoiceUpdateInfoSDO ado = new MOS.SDO.HisInvoiceUpdateInfoSDO();

                // Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TRANSACTION>(ado, this._HisTransaction);

                ado.BuyerName = this.txtNguoiMua.Text;
                ado.BuyerAddress = this.txtDiaChiNguoiMua.Text;
                ado.BuyerAccountNumber = this.txtSTKNguoiMua.Text;
                ado.BuyerTaxCode = this.txtMaSoThue.Text;
                ado.BuyerOrganization = this.txtDonVi.Text;
                ado.InvoiceId = this._HisInvoice.ID;

                CommonParam param = new CommonParam();
                var dataUpdate = new BackendAdapter(param).Post<HIS_INVOICE>("api/HisInvoice/UpdateInfo", ApiConsumers.MosConsumer, ado, param);
                if (dataUpdate != null)
                {
                    success = true;
                    if (this._dlg != null)
                    {
                        this._dlg();
                    }
                    this.Close();
                }
                WaitingManager.Hide();

                MessageManager.Show(this.ParentForm, param, success);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem__Save_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        int positionHandle = -1;

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo viewInfo = edit.GetViewInfo() as DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetValidate()
        {
            try
            {
                SetMaxlength(txtNguoiMua, 200);
                SetMaxlength(txtDiaChiNguoiMua, 500);
                SetMaxlength(txtSTKNguoiMua, 50);
                SetMaxlength(txtMaSoThue, 14);
                SetMaxlength(txtDonVi, 200);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetMaxlength(BaseEdit control, int maxlenght)
        {
            try
            {
                ControlMaxLengthValidationRule validate = new ControlMaxLengthValidationRule();
                validate.editor = control;
                validate.maxLength = maxlenght;
                validate.IsRequired = false;
                validate.ErrorText = string.Format("Nhập quá kí tự cho phép", maxlenght);
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
