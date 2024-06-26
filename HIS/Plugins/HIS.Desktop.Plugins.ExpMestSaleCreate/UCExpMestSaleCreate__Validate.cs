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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.UC.MedicineTypeInStock;
using HIS.UC.MaterialTypeInStock;
using HIS.UC.ExpMestMedicineGrid;
using HIS.UC.ExpMestMaterialGrid;
using HIS.UC.ExpMestMedicineGrid.ADO;
using HIS.UC.ExpMestMaterialGrid.ADO;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;
using DevExpress.XtraEditors.Controls;
using MOS.SDO;
using MOS.Filter;
using Inventec.Core;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.ExpMestSaleCreate.ADO;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.Plugins.ExpMestSaleCreate.Validation;
using DevExpress.Utils.Menu;
using Inventec.Desktop.Common.Controls.ValidationRule;
using DevExpress.XtraEditors.DXErrorProvider;
using System.Runtime.InteropServices;
using HIS.Desktop.Utility;

namespace HIS.Desktop.Plugins.ExpMestSaleCreate
{
    public partial class UCExpMestSaleCreate : UserControlBase
    {
        private void ValidControl()
        {
            try
            {
                ValidControlExpMediStock();
                ValidControlPatientType();
                ValidControlPatyPrice();
                ValidControlExpAmount();
                ValidControlExpPrice();
                ValidControlExpVatRatio();
                ValidControlDiscount();
                //ValidControlPrescriptionCode();
                ValidControlIntructionTime();
                ValidControlCashierRoom();
                ValidControlAccountBook();
                ValidationControlMaxLength(txtVirPatientName, 150, dxValidationProvider_Save, true);
                ValidationControlMaxLength(txtAddress, 600, dxValidationProvider_Save);
                ValidationControlMaxLength(txtDescription, 500, dxValidationProvider_Save);
                ValidationControlMaxLength(txtNote, 200, dxValidationProvider_Add);
                ValidationControlMaxLength(txtTutorial, 1000, dxValidationProvider_Add);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidationControlMaxLength(BaseEdit control, int? maxLength, DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dx, [Optional] bool IsRequest)
        {
            ControlMaxLengthValidationRule validate = new ControlMaxLengthValidationRule();
            validate.editor = control;
            validate.maxLength = maxLength;
            validate.IsRequired = IsRequest;
            validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
            dx.SetValidationRule(control, validate);
        }

        private void ValidControlExpMediStock()
        {
            try
            {
                ExpMediStockValidationRule mediStockRule = new ExpMediStockValidationRule();
                mediStockRule.txtExpMediStockName = txtExpMediStock;
                dxValidationProvider_Save.SetValidationRule(txtExpMediStock, mediStockRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        ///// <summary>
        ///// không validate theo mã y lệnh, mã điều trị
        ///// </summary>
        //private void ValidControlPrescriptionCode()
        //{
        //    try
        //    {
        //        if (!checkIsVisitor.Checked)
        //        {
        //            layoutPrescriptionCode.AppearanceItemCaption.ForeColor = Color.Maroon;

        //            CodePrescriptionValidationRule controlEdit = new CodePrescriptionValidationRule();
        //            controlEdit.ServiceReqCode = txtPrescriptionCode;
        //            controlEdit.TreatmentCode = txtTreatmentCode;
        //            dxValidationProvider_Save.SetValidationRule(txtPrescriptionCode, controlEdit);
        //            txtPrescriptionCode.Focus();
        //            txtPrescriptionCode.SelectAll();
        //        }
        //        else
        //        {
        //            layoutPrescriptionCode.AppearanceItemCaption.ForeColor = Color.Empty;
        //            dxValidationProvider_Save.SetValidationRule(txtPrescriptionCode, null);
        //            txtPatientCode.Focus();
        //            txtPatientCode.SelectAll();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        private void ValidControlPatientName()
        {
            try
            {
                ControlEditValidationRule controlEdit = new ControlEditValidationRule();
                controlEdit.editor = txtVirPatientName;
                controlEdit.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongTBTruongDuLieuBatBuocPhaiNhap);
                controlEdit.ErrorType = ErrorType.Warning;
                this.dxValidationProvider_Save.SetValidationRule(txtVirPatientName, controlEdit);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidControlIntructionTime()
        {
            try
            {
                ControlEditValidationRule controlEdit = new ControlEditValidationRule();
                controlEdit.editor = dtIntructionTime;
                controlEdit.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongTBTruongDuLieuBatBuocPhaiNhap);
                controlEdit.ErrorType = ErrorType.Warning;
                this.dxValidationProvider_Save.SetValidationRule(dtIntructionTime, controlEdit);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidControlPatientType()
        {
            try
            {
                PatientTypeValidationRule patientTypeRule = new PatientTypeValidationRule();
                patientTypeRule.cboPatientType = cboPatientType;
                dxValidationProvider_Save.SetValidationRule(cboPatientType, patientTypeRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlCashierRoom()
        {
            try
            {
                BillCashierRoomValidationRule rule = new BillCashierRoomValidationRule();
                rule.cboBillCashierRoom = cboBillCashierRoom;
                rule.chkCreateBill = chkCreateBill;
                dxValidationProvider_Save.SetValidationRule(cboBillCashierRoom, rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlAccountBook()
        {
            try
            {
                BillAccountBookValidationRule rule = new BillAccountBookValidationRule();
                rule.cboBillAccountBook = cboBillAccountBook;
                rule.chkCreateBill = chkCreateBill;
                dxValidationProvider_Save.SetValidationRule(cboBillAccountBook, rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlPatyPrice()
        {
            try
            {
                PatientTypePriceValidationRule patyPriceRule = new PatientTypePriceValidationRule();
                patyPriceRule.checkImpExpPrice = checkImpExpPrice;
                patyPriceRule.cboPatientType = cboPatientType;
                dxValidationProvider_Add.SetValidationRule(cboPatientType, patyPriceRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlExpAmount()
        {
            try
            {
                ExpAmountValidationRule amountRule = new ExpAmountValidationRule();
                amountRule.spinAmount = spinAmount;
                dxValidationProvider_Add.SetValidationRule(spinAmount, amountRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlExpPrice()
        {
            try
            {
                ExpPriceValidationRule priceRule = new ExpPriceValidationRule();
                priceRule.checkImpExpPrice = checkImpExpPrice;
                priceRule.spinExpPrice = spinExpPrice;
                dxValidationProvider_Add.SetValidationRule(spinExpPrice, priceRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlExpVatRatio()
        {
            try
            {
                ExpVatRatioValidationRule vatRatioRule = new ExpVatRatioValidationRule();
                vatRatioRule.checkImpExpPrice = checkImpExpPrice;
                vatRatioRule.spinExpVatRatio = spinExpVatRatio;
                dxValidationProvider_Add.SetValidationRule(spinExpVatRatio, vatRatioRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlDiscount()
        {
            try
            {
                DiscountValidationRule discountRule = new DiscountValidationRule();
                discountRule.spinDiscount = spinDiscount;
                discountRule.checkImpExpPrice = checkImpExpPrice;
                discountRule.spinAmount = spinAmount;
                discountRule.spinExpPrice = spinExpPrice;
                dxValidationProvider_Add.SetValidationRule(spinDiscount, discountRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
