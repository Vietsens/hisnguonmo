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
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.TransactionDepositCancel.Validation;
using Inventec.Common.LocalStorage.SdaConfig;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
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

namespace HIS.Desktop.Plugins.TransactionDepositCancel
{
    public partial class frmTransactionDepositCancel : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module currentModule = null;

        V_HIS_TRANSACTION transaction;

        private int positionHandleControl = -1;

        public frmTransactionDepositCancel(Inventec.Desktop.Common.Modules.Module module, V_HIS_TRANSACTION data)
		:base(module)
        {
            InitializeComponent();
            try
            {
                this.SetIcon();
                Base.ResourceLangManager.InitResourceLanguageManager();
                this.transaction = data;
                this.currentModule = module;
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
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationStartupPath, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmTransactionDepositCancel_Load(object sender, EventArgs e)
        {
            try
            {
                this.LoadKeyFrmLanguage();
                this.ValidControl();
                if (this.transaction != null && this.transaction.TRANSACTION_TYPE_CODE == SdaConfigs.Get<string>(ConfigKeys.DBCODE__HIS_RS__HIS_TRANSACTION_TYPE__TRANSACTION_TYPE_CODE__DEPOSIT))
                {
                    this.lblTransactionCode.Text = this.transaction.TRANSACTION_CODE;
                    this.lblAmount.Text = Inventec.Common.Number.Convert.NumberToString(transaction.AMOUNT);
                    this.lblTreatmentCode.Text = this.transaction.TREATMENT_CODE;
                    this.lblVirPatientName.Text = this.transaction.TDL_PATIENT_NAME;
                    txtCancelReason.Focus();
                    txtCancelReason.SelectAll();
                }
                else
                {
                    this.btnSave.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControl()
        {
            try
            {
                this.ValidControlCancelReason();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlCancelReason()
        {
            try
            {
                CancelReasonValidationRule reasonRule = new CancelReasonValidationRule();
                reasonRule.txtCancelReason = txtCancelReason;
                dxValidationProvider1.SetValidationRule(txtCancelReason, reasonRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtCancelReason_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
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
                this.positionHandleControl = -1;
                if (!this.btnSave.Enabled || !this.dxValidationProvider1.Validate() || this.transaction == null)
                    return;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                HisTransactionCancelSDO sdo = new HisTransactionCancelSDO();
                sdo.CancelReason = this.txtCancelReason.Text;
                sdo.TransactionId = this.transaction.ID;
                var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<V_HIS_TRANSACTION>("api/HisDeposit/Cancel", ApiConsumers.MosConsumer, sdo, param);
                if (rs != null)
                {
                    success = true;
                }
                WaitingManager.Hide();
                if (success)
                {
                    MessageManager.Show(this, param, success);
                }
                else
                {
                    MessageManager.Show(param, success);
                }
                SessionManager.ProcessTokenLost(param);
                if (success)
                {
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnRCSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSave_Click(null, null);
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
                if (positionHandleControl == -1)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControl > edit.TabIndex)
                {
                    positionHandleControl = edit.TabIndex;
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

        private void LoadKeyFrmLanguage()
        {
            try
            {
                var cul = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                var lang = Base.ResourceLangManager.LanguageFrmTransactionDepositCancel;

                this.btnSave.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEPOSIT_CANCEL__BTN_SAVE", lang, cul);

                this.layoutAmount.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEPOSIT_CANCEL__LAYOUT_AMOUNT", lang, cul);
                this.layoutCancelReason.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEPOSIT_CANCEL__LAYOUT_CANCEL_REASON", lang, cul);
                this.layoutTransactionCode.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEPOSIT_CANCEL__LAYOUT_TRANSACTION_CODE", lang, cul);
                this.layoutTreatmentCode.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEPOSIT_CANCEL__LAYOUT_TREATMENT_CODE", lang, cul);
                this.layoutVirPatientName.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEPOSIT_CANCEL__LAYOUT_VIR_PATIENT_NAME", lang, cul);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
