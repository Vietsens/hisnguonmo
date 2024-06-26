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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Plugins.Library.ElectronicBill.Base;
using HIS.Desktop.Plugins.TransactionEInvoice.Resources;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Resources;

namespace HIS.Desktop.Plugins.TransactionEInvoice.TransactionEInvoice
{
    public partial class frmTransactionEInvoice : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        int ActionType = -1;
        int positionHandle = -1;
        MOS.EFMODEL.DataModels.V_HIS_TRANSACTION HisTransaction;
        Inventec.Desktop.Common.Modules.Module moduleData;
        string InvoiceCode = "";
        string InvoiceSys = "";
        string ErrorMess = "";
        #endregion

        #region Construct
        public frmTransactionEInvoice(Inventec.Desktop.Common.Modules.Module moduleData, MOS.EFMODEL.DataModels.V_HIS_TRANSACTION _HisTransaction)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();
                this.moduleData = moduleData;
                this.HisTransaction = _HisTransaction;

                try
                {
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
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
        #endregion

        #region Private method
        private void frmTransactionEInvoice_Load(object sender, EventArgs e)
        {
            try
            {
                MeShow();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void MeShow()
        {
            try
            {
                SetDefaultValue();
                // kiem tra du lieu nhap vao
                ValidateForm();
                //set ngon ngu
                SetCaptionByLanguagekey();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguagekey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.TransactionEInvoice.Resources.Lang", typeof(HIS.Desktop.Plugins.TransactionEInvoice.TransactionEInvoice.frmTransactionEInvoice).Assembly);

                if (this.moduleData != null && !String.IsNullOrEmpty(this.moduleData.text))
                {
                    this.Text = this.moduleData.text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void ValidateForm()
        {
            ValidationSingleControl(txtInvoiceCode, dxValidationProvider1);
            ValidationSingleControl(txtEinvoiceNumOrder, dxValidationProvider1);
            ValidationSingleControl(dtEinvoiceTime, dxValidationProvider1);

        }

        private void SetDefaultValue()
        {
            try
            {


                Inventec.Common.Logging.LogSystem.Info("HisTransaction: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisTransaction), HisTransaction));
                if (HisTransaction != null)
                {
                    lblTransactionCode.Text = !string.IsNullOrEmpty(HisTransaction.TRANSACTION_CODE) ? HisTransaction.TRANSACTION_CODE : "";
                    lblAmount.Text = Inventec.Common.Number.Convert.NumberToString((HisTransaction.AMOUNT), ConfigApplications.NumberSeperator);
                    lblTdlPatientName.Text = !string.IsNullOrEmpty(HisTransaction.TDL_PATIENT_NAME) ? HisTransaction.TDL_PATIENT_NAME : "";
                    lblTdlPatientDob.Text = HisTransaction.TDL_PATIENT_DOB != null ? Inventec.Common.DateTime.Convert.TimeNumberToDateString((long)HisTransaction.TDL_PATIENT_DOB) : "";
                    lblTdlPatientGenderName.Text = !string.IsNullOrEmpty(HisTransaction.TDL_PATIENT_GENDER_NAME) ? HisTransaction.TDL_PATIENT_GENDER_NAME : "";
                }
                
                if (HisTransaction.EINVOICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EINVOICE_TYPE.ID__BKAV)
                    txtInvoiceCode.Text = HisTransaction.TRANSACTION_CODE;
                else
                    txtInvoiceCode.Text = "";
                txtInvoiceLookupCode.Text = HisTransaction.INVOICE_LOOKUP_CODE;
                txtEinvoiceNumOrder.Text = "";
                dtEinvoiceTime.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                ErrorMess = "";
                this.positionHandle = -1;
                if (!dxValidationProvider1.Validate())
                    return;
                if (MessageBox.Show(ResourceMessage.CanhBaoLuu, "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    ElectronicBillDataInput DataInput = new ElectronicBillDataInput();
                    //DataInput.Transaction = AutoMapper.Mapper.Map<HIS_TRANSACTION>(HisTransaction);
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TRANSACTION>(DataInput.Transaction, this.HisTransaction);
                    //DataInput.Transaction = HisTransaction;
                    DataInput.InvoiceCode = txtInvoiceCode.Text.Trim();
                    DataInput.ENumOrder = txtEinvoiceNumOrder.Text.Trim();
                    DataInput.TemplateCode = HisTransaction.TEMPLATE_CODE;
                    DataInput.SymbolCode = HisTransaction.SYMBOL_CODE;

                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => DataInput), DataInput));

                    bool chek = HIS.Desktop.Plugins.Library.ElectronicBill.ElectronicBillProcessor.GetInvoiceInfo(DataInput, ref InvoiceSys, ref InvoiceCode, ref ErrorMess);

                    if (chek == true)
                    {
                        HisTransactionInvoiceInfoSDO input = new HisTransactionInvoiceInfoSDO();
                        input.EinvoiceNumOrder = txtEinvoiceNumOrder.Text.Trim();
                        input.EInvoiceTime = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtEinvoiceTime.Text).ToString("yyyyMMddHHmmss"));
                        input.InvoiceSys = InvoiceSys;
                        input.InvoiceCode = InvoiceCode;
                        input.Id = this.HisTransaction.ID;
                        input.InvoiceLookupCode = txtInvoiceLookupCode.Text.Trim();

                        CommonParam param = new CommonParam();

                        Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => input), input));

                        var resultData = new BackendAdapter(param).Post<bool>(HisRequestUriStore.HIS_TRANSACTION_UPDATE_INVOICEINFO, ApiConsumers.MosConsumer, input, param);

                        #region Hien thi message thong bao
                        MessageManager.Show(this, param, resultData);
                        #endregion
                    }
                    else
                    {
                        MessageBox.Show(ErrorMess);
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        #endregion

        #region event

        private void txtInvoiceCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtEinvoiceNumOrder.Focus();
                    txtEinvoiceNumOrder.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void txtEinvoiceNumOrder_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtEinvoiceTime.Focus();
                    dtEinvoiceTime.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtEinvoiceTime_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtInvoiceLookupCode.Focus();
                    txtInvoiceLookupCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void bbtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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
                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.Focus();
                        edit.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtInvoiceLookupCode_KeyDown(object sender, KeyEventArgs e)
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
