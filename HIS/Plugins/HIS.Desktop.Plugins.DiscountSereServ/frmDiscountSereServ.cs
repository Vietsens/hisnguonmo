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
//using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.DiscountSereServ.Validation;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
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

namespace HIS.Desktop.Plugins.DiscountSereServ
{
    public partial class frmDiscountSereServ : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module currentModule;

        HIS_SERE_SERV sereServ = null;

        decimal discount = 0;
        decimal virTotalPrice = 0;

        int positionHandleControl = -1;

        public frmDiscountSereServ(Inventec.Desktop.Common.Modules.Module module, HIS_SERE_SERV data)
		:base(module)
        {
            InitializeComponent();
            try
            {
                this.Size = new Size(this.Size.Width, this.Size.Height - 25);
                SetIcon();
                Base.ResourceLangManager.InitResourceLanguageManager();
                this.sereServ = data;
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

        private void frmDiscountSereServ_Load(object sender, EventArgs e)
        {
            try
            {
                LoadKeyFrmLanguage();
                if (sereServ != null)
                {
                    virTotalPrice = sereServ.VIR_PATIENT_PRICE ?? 0;
                    discount = sereServ.DISCOUNT ?? 0;
                    lblVirTotalPatientPrice.Text = Inventec.Common.Number.Convert.NumberToString(virTotalPrice);
                    lblDiscount.Text = Inventec.Common.Number.Convert.NumberToString(discount);
                    lblTotalSalePrice.Text = Inventec.Common.Number.Convert.NumberToString(virTotalPrice - discount);
                    if (virTotalPrice == 0)
                    {
                        txtDiscountPrice.Enabled = false;
                        txtDiscountRatio.Enabled = false;
                        btnSave.Enabled = false;
                    }
                    else
                    {
                        ValidControl();
                        txtDiscountPrice.Focus();
                        txtDiscountPrice.SelectAll();
                    }
                }
                else
                {
                    txtDiscountPrice.Enabled = false;
                    txtDiscountRatio.Enabled = false;
                    btnSave.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDiscountPrice_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (txtDiscountPrice.EditValue != txtDiscountPrice.OldEditValue)
                    {
                        ReloadPriceToLabel(false);
                    }
                    else
                    {
                        txtDiscountRatio.Focus();
                        txtDiscountRatio.SelectAll();
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDiscountRatio_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (txtDiscountRatio.EditValue != txtDiscountRatio.OldEditValue)
                    {
                        ReloadPriceToLabel(true);
                    }
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
                positionHandleControl = -1;
                if (!btnSave.Enabled || !dxValidationProvider1.Validate() || this.sereServ == null)
                    return;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                HIS_SERE_SERV data = new HIS_SERE_SERV();
                data = sereServ;

                if (txtDiscountPrice.Value > 0)
                {
                    data.DISCOUNT = txtDiscountPrice.Value;
                }
                else if (txtDiscountRatio.Value > 0)
                {
                    discount = virTotalPrice * (txtDiscountRatio.Value / 100);
                    txtDiscountPrice.Value = discount;
                    data.DISCOUNT = txtDiscountPrice.Value;
                }
                var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_SERE_SERV>(HisRequestUriStore.HIS_SERE_SERV_UPDATEDISCOUNT, ApiConsumers.MosConsumer, data, param);
                if (rs != null)
                {
                    success = true;
                }

                WaitingManager.Hide();
                SessionManager.ProcessTokenLost(param);
                if (success)
                {
                    MessageManager.Show(this, param, success);
                    this.Close();
                }
                else
                {
                    MessageManager.Show(param, success);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ReloadPriceToLabel(bool IsRatio)
        {
            try
            {
                if (IsRatio)
                {
                    discount = virTotalPrice * (txtDiscountRatio.Value / 100);
                    txtDiscountPrice.Value = discount;
                }
                else
                {
                    discount = txtDiscountPrice.Value;
                    if (virTotalPrice > 0)
                    {
                        txtDiscountRatio.Value = (discount / virTotalPrice) * 100;
                    }
                }
                lblDiscount.Text = Inventec.Common.Number.Convert.NumberToString(discount);
                lblTotalSalePrice.Text = Inventec.Common.Number.Convert.NumberToString(virTotalPrice - discount);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnRCSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void LoadKeyFrmLanguage()
        {
            try
            {
                //Button
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_DISCOUNT_SERE_SERV__BTN_SAVE", Base.ResourceLangManager.LanguageFrmDiscountSereServ, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                //Layout
                this.layoutDiscount.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_DISCOUNT_SERE_SERV__LAYOUT_DISCOUNT", Base.ResourceLangManager.LanguageFrmDiscountSereServ, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutDiscountPrice.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_DISCOUNT_SERE_SERV__LAYOUT_DISCOUNT_PRICE", Base.ResourceLangManager.LanguageFrmDiscountSereServ, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutDiscountRatio.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_DISCOUNT_SERE_SERV__LAYOUT_DISCOUNT_RATIO", Base.ResourceLangManager.LanguageFrmDiscountSereServ, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutTotalSalePrice.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_DISCOUNT_SERE_SERV__LAYOUT_TOTAL_SALE_PRICE", Base.ResourceLangManager.LanguageFrmDiscountSereServ, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutVirTotalPatientPrice.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_DISCOUNT_SERE_SERV__LAYOUT_VIR_TOTAL_PRICE", Base.ResourceLangManager.LanguageFrmDiscountSereServ, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                ValidControlDiscountPrice();
                ValidControlDiscountRatio();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlDiscountPrice()
        {
            try
            {
                DiscountPriceValidationRule disPriceRule = new DiscountPriceValidationRule();
                disPriceRule.txtDiscountPrice = txtDiscountPrice;
                disPriceRule.virTotalPrice = virTotalPrice;
                dxValidationProvider1.SetValidationRule(txtDiscountPrice, disPriceRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlDiscountRatio()
        {
            try
            {
                DiscountRatioValidationRule disRatioRule = new DiscountRatioValidationRule();
                disRatioRule.txtDiscountRatio = txtDiscountRatio;
                dxValidationProvider1.SetValidationRule(txtDiscountRatio, disRatioRule);
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
    }
}
