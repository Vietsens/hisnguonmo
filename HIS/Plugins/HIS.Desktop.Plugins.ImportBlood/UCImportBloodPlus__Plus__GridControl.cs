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
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.ImportBlood.ADO;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ImportBlood
{
    public partial class UCImportBloodPlus
    {

        private void gridViewImpMestBlood_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                if (e.RowHandle < 0)
                {
                    return;
                }
                var data = (VHisBloodADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                if (data != null)
                {
                    if (e.Column.FieldName == "ImpVatRatio")
                    {
                        data.IMP_VAT_RATIO = data.ImpVatRatio / 100;
                    }
                    gridControlImpMestBlood.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public string GetError(int rowHandle, DevExpress.XtraGrid.Columns.GridColumn column)
        {
            try
            {

                if (column.FieldName == "ImpVatRatio")
                {

                    VHisBloodADO data = (VHisBloodADO)gridViewImpMestBlood.GetRow(rowHandle);
                    if (data != null && (data.ImpVatRatio < 0 || data.ImpVatRatio > 100))
                            return "Giá trị không hợp lệ.";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return string.Empty;
        }

        void SetError(BaseEditViewInfo cellInfo, string errorIconText)
        {
            try
            {
                if (errorIconText == string.Empty)
                {
                    cellInfo.ErrorIconText = null;
                    cellInfo.ShowErrorIcon = false;
                    return;
                }
                cellInfo.ErrorIconText = errorIconText;
                cellInfo.ShowErrorIcon = true;
                cellInfo.FillBackground = true;
                cellInfo.ErrorIcon = DXErrorProvider.GetErrorIconInternal(ErrorType.Warning);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewImpMestBlood_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (VHisBloodADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            try
                            {
                                e.Value = e.ListSourceRowIndex + 1;
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "PACKING_TIME_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.PACKING_TIME ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "Expired_Date_Str")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.EXPIRED_DATE ?? 0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridBloodType_RowEnter(UC.BloodType.ADO.BloodTypeADO data)
        {
            try
            {
                WaitingManager.Show();
                this.ProcessChoiceBloodTypeADO(data);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void gridBloodType_RowClick(UC.BloodType.ADO.BloodTypeADO data)
        {
            try
            {
                WaitingManager.Show();
                this.ChoiceBloodTypeADO(data);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridBloodType_Click(UC.BloodType.ADO.BloodTypeADO data)
        {
            try
            {
                WaitingManager.Show();
                this.ProcessChoiceBloodTypeADO(data);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void RemoveControlDxError1()
        {
            try
            {
                dxValidationProvider1.RemoveControlError(txtBloodAboCode);
                dxValidationProvider1.RemoveControlError(spinImpPrice);
                dxValidationProvider1.RemoveControlError(spinImpVatRatio);
                dxValidationProvider1.RemoveControlError(txtPackingTime);
                dxValidationProvider1.RemoveControlError(dtPackingTime);
                dxValidationProvider1.RemoveControlError(dtExpiredDate);
                dxValidationProvider1.RemoveControlError(txtBloodCode);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void RemoveControlDxError2()
        {
            try
            {
                dxValidationProvider2.RemoveControlError(txtImpMestType);
                dxValidationProvider2.RemoveControlError(txtMediStock);
                dxValidationProvider2.RemoveControlError(cboSupplier);
                dxValidationProvider2.RemoveControlError(txtDocumentDate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessChoiceBloodTypeADO(UC.BloodType.ADO.BloodTypeADO data)
        {
            try
            {
                //không ProcessChoiceBloodType với TH Hiến Máu
                if (this._isHienMau)
                {
                    if (this.bloodGiverActionType == ActionType.Update && this.updatingBloodGiverADO != null)
                    {
                        this.SetEnableButtonAdd(true);
                        this.currentBlood_BloodGiver_ForAdd = null;
                        if (data != null && data.IS_LEAF == 1)
                        {
                            this.currentBlood_BloodGiver_ForAdd = new VHisBloodADO(data);
                            currentBlood_BloodGiver_ForAdd.IsBloodDonation = true;
                            currentBlood_BloodGiver_ForAdd.BloodDonationCode = updatingBloodGiverADO.GIVE_CODE;
                            currentBlood_BloodGiver_ForAdd.BloodDonationStr_ForDisplayGroup = String.Format("Hồ sơ hiến máu: {0} - {1} - {2} - {3}", updatingBloodGiverADO.GIVE_CODE, updatingBloodGiverADO.GIVE_NAME, updatingBloodGiverADO.DOB_ForDisplay, updatingBloodGiverADO.GENDER_ForDisplay);
                            currentBlood_BloodGiver_ForAdd.GIVE_CODE = updatingBloodGiverADO.GIVE_CODE;
                            currentBlood_BloodGiver_ForAdd.GIVE_NAME = updatingBloodGiverADO.GIVE_NAME;
                            currentBlood_BloodGiver_ForAdd.DOB_ForDisplay = updatingBloodGiverADO.DOB_ForDisplay;
                            currentBlood_BloodGiver_ForAdd.GENDER_ForDisplay = updatingBloodGiverADO.GENDER_ForDisplay;
                            currentBlood_BloodGiver_ForAdd.BLOOD_ABO_ID = updatingBloodGiverADO.BLOOD_ABO_ID ?? 0;
                            currentBlood_BloodGiver_ForAdd.BLOOD_RH_ID = updatingBloodGiverADO.BLOOD_RH_ID;
                            currentBlood_BloodGiver_ForAdd.EXPIRED_DATE = updatingBloodGiverADO.EXECUTE_TIME != null ? (long?)(updatingBloodGiverADO.EXECUTE_TIME + (data.ALERT_EXPIRED_DATE ?? 0) * 1000000) : null;
                            currentBlood_BloodGiver_ForAdd.BLOOD_CODE = (data.BLOOD_TYPE_CODE ?? "") + (updatingBloodGiverADO.GIVE_CODE ?? "");
                        }
                        this.SetControlValueByBloodType(true);
                        RemoveControlDxError1();
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    this.SetEnableButtonAdd(true);
                    this.currentBlood = null;
                    if (data != null && data.IS_LEAF == 1)
                    {
                        this.currentBlood = new VHisBloodADO(data);
                    }

                    this.CheckBloodTypeInBid();
                    this.SetControlValueByBloodType(true);
                    RemoveControlDxError1();
                    txtBloodAboCode.Focus();
                    txtBloodAboCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ChoiceBloodTypeADO(UC.BloodType.ADO.BloodTypeADO data)
        {
            try
            {
                //long packingTime = 0;
                //long expiredDate = 0;
                //if (this.currentBlood.PACKING_TIME != null)
                //{
                //    packingTime = (long)this.currentBlood.PACKING_TIME;
                //}
                //if (this.currentBlood.EXPIRED_DATE != null)
                //{
                //    expiredDate = (long)this.currentBlood.EXPIRED_DATE;
                //}
                 decimal price = 0;
                 decimal impVatRatio = 0;
                 string giveCode = "";
                 string giveName = "";
                 string packageNumber = "";
                 bool isInfect = false;
                 if (this.currentBlood != null)
                 {
                     if (this.currentBlood.IMP_PRICE != null)
                     {
                         price = this.currentBlood.IMP_PRICE;
                     }
                     if (this.currentBlood.IMP_VAT_RATIO != null)
                     {
                         impVatRatio = this.currentBlood.IMP_VAT_RATIO;
                     }

                     giveCode = this.currentBlood.GIVE_CODE;
                     giveName = this.currentBlood.GIVE_NAME;
                     isInfect = (this.currentBlood.IS_INFECT == 1);
                     packageNumber = this.currentBlood.PACKAGE_NUMBER;

                     if (data != null && data.IS_LEAF == 1)
                     {
                         this.currentBlood = new VHisBloodADO(data);
                     }
                 }
                 
                this.CheckBloodTypeInBid();
                this.SetBloodTypeValue();
                spinImpPrice.Value = price;
                spinImpVatRatio.Value = impVatRatio*100;
                txtGiveCode.Text = giveCode;
                txtGiveName.Text = giveName;
                checkIsInfect.Checked = isInfect;
                txtPackageNumber.Text = packageNumber;
                //dtPackingTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(packingTime) ?? DateTime.Now;

                //dtExpiredDate.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(expiredDate) ?? DateTime.Now;

                this.SetEnableButtonAdd(true);
                RemoveControlDxError1();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CheckBloodTypeInBid()
        {
            try
            {
                if (this.currentBlood != null)
                {
                    bool inBid = false;
                    if (this.listBidBlood != null)
                    {
                        var bidBlood = listBidBlood.FirstOrDefault(o => o.BLOOD_TYPE_ID == this.currentBlood.BLOOD_TYPE_ID);
                        if (bidBlood != null)
                        {
                            inBid = true;
                            //lblBidInfo.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_BLOOD__LBL_BID_INFO__HAS_IN_BID", Base.ResourceLangManager.LanguageUCImportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                            //txtBidNumOrder.Text = bidBlood.BID_NUM_ORDER;
                        }
                    }
                    if (!inBid)
                    {
                        //lblBidInfo.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_BLOOD__LBL_BID_INFO__NOT_HAS_IN_BID", Base.ResourceLangManager.LanguageUCImportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetControlValueByBloodType(bool isAdd)
        {
            try
            {
                txtBloodAboCode.Text = "";
                cboBloodAbo.EditValue = null;
                //txtBloodRhCode.Text = "";
                cboBloodRh.EditValue = null;
                txtGiveCode.Text = "";
                txtGiveName.Text = "";
                checkIsInfect.Checked = false;
                txtBloodCode.Text = "";
                txtPackageNumber.Text = "";
                spinImpPrice.Value = 0;
                spinImpVatRatio.Value = 0;
                dtPackingTime.EditValue = null;
                dtExpiredDate.EditValue = null;
                if (this._isHienMau)
                {
                    if (this.currentBlood_BloodGiver_ForAdd != null)
                    {
                        //txtGiveCode.Text = this.currentBlood.GIVE_CODE;
                        //txtGiveName.Text = this.currentBlood.GIVE_NAME;
                        checkIsInfect.Checked = (this.currentBlood_BloodGiver_ForAdd.IS_INFECT == 1);
                        txtBloodCode.Text = this.currentBlood_BloodGiver_ForAdd.BLOOD_CODE;
                        if (this.currentBlood_BloodGiver_ForAdd.BLOOD_ABO_ID > 0)
                        {
                            cboBloodAbo.EditValue = this.currentBlood_BloodGiver_ForAdd.BLOOD_ABO_ID;
                        }
                        if (this.currentBlood_BloodGiver_ForAdd.BLOOD_RH_ID.HasValue && this.currentBlood_BloodGiver_ForAdd.BLOOD_RH_ID.Value > 0)
                        {
                            cboBloodRh.EditValue = this.currentBlood_BloodGiver_ForAdd.BLOOD_RH_ID.Value;
                        }
                        spinImpPrice.Value = this.currentBlood_BloodGiver_ForAdd.IMP_PRICE;
                        spinImpVatRatio.Value = this.currentBlood_BloodGiver_ForAdd.ImpVatRatio;
                        if (isAdd)
                        {
                            btnAdd.Enabled = true;
                        }
                        else
                        {
                            btnUpdate.Enabled = true;
                        }
                        txtPackageNumber.Text = this.currentBlood_BloodGiver_ForAdd.PACKAGE_NUMBER;
                        if (this.currentBlood_BloodGiver_ForAdd.PACKING_TIME != null)
                        {
                            dtPackingTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.currentBlood_BloodGiver_ForAdd.PACKING_TIME ?? 0) ?? DateTime.Now;
                        }
                        if (this.currentBlood_BloodGiver_ForAdd.EXPIRED_DATE != null)
                        {
                            dtExpiredDate.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.currentBlood_BloodGiver_ForAdd.EXPIRED_DATE ?? 0) ?? DateTime.Now;
                        }
                    }
                    else
                    {
                        if (isAdd)
                        {
                            btnAdd.Enabled = false;
                        }
                        else
                        {
                            btnUpdate.Enabled = false;
                        }
                    }
                }
                else
                {
                    if (this.currentBlood != null)
                    {
                        txtGiveCode.Text = this.currentBlood.GIVE_CODE;
                        txtGiveName.Text = this.currentBlood.GIVE_NAME;
                        checkIsInfect.Checked = (this.currentBlood.IS_INFECT == 1);
                        txtBloodCode.Text = this.currentBlood.BLOOD_CODE;
                        if (this.currentBlood.BLOOD_ABO_ID > 0)
                        {
                            cboBloodAbo.EditValue = this.currentBlood.BLOOD_ABO_ID;
                        }
                        if (this.currentBlood.BLOOD_RH_ID.HasValue && this.currentBlood.BLOOD_RH_ID.Value > 0)
                        {
                            cboBloodRh.EditValue = this.currentBlood.BLOOD_RH_ID.Value;
                        }
                        spinImpPrice.Value = this.currentBlood.IMP_PRICE;
                        spinImpVatRatio.Value = this.currentBlood.ImpVatRatio;
                        if (isAdd)
                        {
                            btnAdd.Enabled = true;
                        }
                        else
                        {
                            btnUpdate.Enabled = true;
                        }
                        txtPackageNumber.Text = this.currentBlood.PACKAGE_NUMBER;
                        if (this.currentBlood.PACKING_TIME != null)
                        {
                            dtPackingTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.currentBlood.PACKING_TIME ?? 0) ?? DateTime.Now;
                        }
                        if (this.currentBlood.EXPIRED_DATE != null)
                        {
                            dtExpiredDate.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.currentBlood.EXPIRED_DATE ?? 0) ?? DateTime.Now;
                        }
                    }
                    else
                    {
                        if (isAdd)
                        {
                            btnAdd.Enabled = false;
                        }
                        else
                        {
                            btnUpdate.Enabled = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetBloodTypeValue() 
        {
            txtGiveCode.Text = this.currentBlood.GIVE_CODE;
            txtGiveName.Text = this.currentBlood.GIVE_NAME;
            checkIsInfect.Checked = (this.currentBlood.IS_INFECT == 1);
            txtBloodCode.Text = this.currentBlood.BLOOD_CODE;
            if (this.currentBlood.BLOOD_ABO_ID > 0)
            {
                cboBloodAbo.EditValue = this.currentBlood.BLOOD_ABO_ID;
            }
            if (this.currentBlood.BLOOD_RH_ID.HasValue && this.currentBlood.BLOOD_RH_ID.Value > 0)
            {
                cboBloodRh.EditValue = this.currentBlood.BLOOD_RH_ID.Value;
            }
            spinImpPrice.Value = this.currentBlood.IMP_PRICE;
            spinImpVatRatio.Value = this.currentBlood.ImpVatRatio;
            txtPackageNumber.Text = this.currentBlood.PACKAGE_NUMBER;
        }
    }
}
