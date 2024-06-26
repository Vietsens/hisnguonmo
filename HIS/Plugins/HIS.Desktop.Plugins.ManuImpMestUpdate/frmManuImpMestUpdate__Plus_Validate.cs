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
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.ManuImpMestUpdate.ADO;
using HIS.Desktop.Plugins.ManuImpMestUpdate.Config;
using HIS.Desktop.Plugins.ManuImpMestUpdate.Resources;
using HIS.Desktop.Plugins.ManuImpMestUpdate.Validation;
using HIS.Desktop.Utility;
using HIS.UC.MaterialType;
using HIS.UC.MedicineType;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Threading;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ManuImpMestUpdate
{
    public partial class frmManuImpMestUpdate : HIS.Desktop.Utility.FormBase
    {
        private void ValidControls()
        {
            try
            {
                HIS_IMP_MEST_TYPE impMestType = null;
                if (cboImpMestType.EditValue != null)
                {
                    impMestType = BackendDataWorker.Get<HIS_IMP_MEST_TYPE>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboImpMestType.EditValue));
                }
                if (impMestType.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC)
                {
                    ValidControlSupplier();
                }
                ValidControlImpMestType();
                ValidControlMediStock();
                ValidControlImpAmount();
                ValidControlImPrice();
                ValidControlImpVatRatio();
                ValidControlExpiredDate();
                ValidControlSpinDiscount();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlImpMestType()
        {
            try
            {
                ImpMestTypeValidationRule impMestTypeRule = new ImpMestTypeValidationRule();
                impMestTypeRule.cboImpMestType = cboImpMestType;
                dxValidationProvider1.SetValidationRule(cboImpMestType, impMestTypeRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlSupplier()
        {
            try
            {
                SupplierValidationRule supplierValidationRule = new SupplierValidationRule();
                supplierValidationRule.cboSupplier = cboSupplier;
                dxValidationProvider1.SetValidationRule(cboSupplier, supplierValidationRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlMediStock()
        {
            try
            {
                MediStockValidationRule mediStockRule = new MediStockValidationRule();
                mediStockRule.cboMediStock = cboMediStock;
                dxValidationProvider1.SetValidationRule(cboMediStock, mediStockRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlImpAmount()
        {
            try
            {
                ImpAmountValidationRule impAmountRule = new ImpAmountValidationRule();
                impAmountRule.spinImpAmount = spinAmount;
                dxValidationProviderLeft.SetValidationRule(spinAmount, impAmountRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlImPrice()
        {
            try
            {
                ImpPriceValidationRule impPriceRule = new ImpPriceValidationRule();
                impPriceRule.spinImpPrice = spinImpPrice;
                dxValidationProviderLeft.SetValidationRule(spinImpPrice, impPriceRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlImpVatRatio()
        {
            try
            {
                ImpVatRatioValidationRule impVatRule = new ImpVatRatioValidationRule();
                impVatRule.spinImpVatRatio = spinImpVatRatio;
                dxValidationProviderLeft.SetValidationRule(spinImpVatRatio, impVatRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlSpinDiscount()
        {
            try
            {
                ImpVatRatioValidationRule impVatRule = new ImpVatRatioValidationRule();
                impVatRule.spinImpVatRatio = spinDiscountRatio;
                dxValidationProvider1.SetValidationRule(spinDiscountRatio, impVatRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlExpiredDate()
        {
            try
            {
                ExpiredDateValidationRule expiredDateRule = new ExpiredDateValidationRule();
                expiredDateRule.txtExpiredDate = txtExpiredDate;
                expiredDateRule.dtExpiredDate = dtExpiredDate;
                dxValidationProviderLeft.SetValidationRule(txtExpiredDate, expiredDateRule);
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

        private void dxValidationProvider2_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
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

        private void ValidControlExpiredDate1(BaseEdit control)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider2.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
    }
}
