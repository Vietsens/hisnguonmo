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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisMedicalContractCreate.Run
{
    public partial class FormHisMedicalContractCreate : FormBase
    {
        decimal currentAMOUNT;

        private void RefreshSupplierData(object data)
        {
            try
            {
                if (data != null && data.GetType() == typeof(HIS_SUPPLIER))
                {
                    HIS_SUPPLIER newData = data as HIS_SUPPLIER;
                    BackendDataWorker.Reset<HIS_SUPPLIER>();
                    //Task task =
                    LoadAsyncDataToCboSupplier();
                    //task.Wait();
                    if (cboSupplierNum == 1)
                    {
                        cboSupplier.EditValue = newData.ID;
                    }
                    else if (cboSupplierNum == 2)
                    {
                        cboDocumentSupplier.EditValue = newData.ID;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetValueForAdd()
        {
            try
            {
                ClearForAdd();

                // thiet lap mac dinh
                if (xtraTabMetyMate.SelectedTabPage == tabMedicineType && this.medicineType != null && !String.IsNullOrEmpty(this.medicineType.MEDICINE_TYPE_CODE))
                {
                    long supplierId = Inventec.Common.TypeConvert.Parse.ToInt64((cboSupplier.EditValue ?? 0).ToString());
                    long bidId = Inventec.Common.TypeConvert.Parse.ToInt64((cboBid.EditValue ?? 0).ToString());
                    V_HIS_BID_MEDICINE_TYPE bidMedicine = null;
                    if (bidId > 0)
                    {
                        if (DicBidMedicineType.ContainsKey(bidId))
                            bidMedicine = this.DicBidMedicineType[bidId].FirstOrDefault(o => o.SUPPLIER_ID == supplierId && o.MEDICINE_TYPE_ID == this.medicineType.ID && o.BID_GROUP_CODE == this.medicineType.BidGroupCode);
                        if (bidMedicine != null)
                        {
                            spMonthLifespan.EditValue = bidMedicine.MONTH_LIFESPAN;
                            spDayLifespan.EditValue = bidMedicine.DAY_LIFESPAN;
                            spHourLifespan.EditValue = bidMedicine.HOUR_LIFESPAN;
                            spAmount.EditValue = bidMedicine.AMOUNT;
                            spImpVat.EditValue = bidMedicine.IMP_VAT_RATIO * 100;


                            spImpPrice.EditValue = bidMedicine.IMP_PRICE;
                            spPriceVat.EditValue = spContractPrice.Value;
                        }
                        spImpVat.Enabled = false;
                        spMonthLifespan.Enabled = false;
                        spDayLifespan.Enabled = false;
                        spHourLifespan.Enabled = false;
                        lciHour.Enabled = false;
                        lciPnNationalName.Enabled = false;
                        dtExpiredDate.Enabled = false;
                        txtRegisterNumber.Enabled = false;
                        txtNationalName.Enabled = false;
                        chkEditNational.Enabled = false;
                        txtConcentra.Enabled = false;
                        txtManufacturerCode.Enabled = false;
                        cboManufacturerName.Enabled = false;

                        txtQDThau.Enabled = false;
                        txtNhomThau.Enabled = false;
                        txtGhiChu.Enabled = false;
                    }

                    if (bidMedicine != null)
                    {
                        var national = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_NATIONAL>().Where(o => o.NATIONAL_NAME == bidMedicine.NATIONAL_NAME).ToList();
                        if (national != null && national.Count() > 0)
                        {
                            txtNationalName.Text = national[0].NATIONAL_NAME;
                            cboNationalName.EditValue = national[0].ID;
                            txtNationalName.Visible = false;
                            cboNationalName.Visible = true;
                            chkEditNational.CheckState = System.Windows.Forms.CheckState.Unchecked;
                        }
                        else
                        {
                            txtNationalName.Text = this.medicineType.NATIONAL_NAME;
                            cboNationalName.EditValue = null;
                            txtNationalName.Visible = true;
                            cboNationalName.Visible = false;
                            chkEditNational.CheckState = System.Windows.Forms.CheckState.Checked;
                        }

                        if (bidMedicine.EXPIRED_DATE.HasValue)
                        {
                            dtExpiredDate.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(bidMedicine.EXPIRED_DATE.Value);
                        }


                        cboManufacturerName.EditValue = bidMedicine.MANUFACTURER_ID;
                        txtManufacturerCode.Text = bidMedicine.MANUFACTURER_CODE;
                        txtRegisterNumber.Text = bidMedicine.REGISTER_NUMBER;
                        txtConcentra.Text = bidMedicine.CONCENTRA;

                        txtQDThau.Text = bidMedicine.BID_NUMBER;
                        txtNhomThau.Text = bidMedicine.BID_GROUP_CODE;
                        txtGhiChu.Text = bidMedicine.NOTE;

                    }
                    else
                    {
                        var national = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_NATIONAL>().Where(o => o.NATIONAL_NAME == this.medicineType.NATIONAL_NAME).ToList();
                        txtRegisterNumber.Text = this.medicineType.REGISTER_NUMBER;
                        if (national != null && national.Count() > 0)
                        {
                            txtNationalName.Text = national[0].NATIONAL_NAME;
                            cboNationalName.EditValue = national[0].ID;
                            txtNationalName.Visible = false;
                            cboNationalName.Visible = true;
                            chkEditNational.CheckState = System.Windows.Forms.CheckState.Unchecked;
                        }
                        else
                        {
                            txtNationalName.Text = this.medicineType.NATIONAL_NAME;
                            cboNationalName.EditValue = null;
                            txtNationalName.Visible = true;
                            cboNationalName.Visible = false;
                            chkEditNational.CheckState = System.Windows.Forms.CheckState.Checked;
                        }

                        if (this.medicineType.LAST_EXPIRED_DATE.HasValue)
                        {
                            dtExpiredDate.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.medicineType.LAST_EXPIRED_DATE.Value);
                        }


                        cboManufacturerName.EditValue = this.medicineType.MANUFACTURER_ID;
                        txtManufacturerCode.Text = this.medicineType.MANUFACTURER_CODE;
                        txtConcentra.Text = this.medicineType.CONCENTRA;



                        spImpVat.EditValue = this.medicineType.IMP_VAT_RATIO * 100;
                    }



                }
                else if (xtraTabMetyMate.SelectedTabPage == tabMaterialType && this.materialType != null && !String.IsNullOrEmpty(this.materialType.MATERIAL_TYPE_CODE))
                {
                    txtRegisterNumber.Text = null;
                    txtRegisterNumber.Enabled = false;
                    long supplierId = Inventec.Common.TypeConvert.Parse.ToInt64((cboSupplier.EditValue ?? 0).ToString());
                    long bidId = Inventec.Common.TypeConvert.Parse.ToInt64((cboBid.EditValue ?? 0).ToString());
                    V_HIS_BID_MATERIAL_TYPE bidMaterial = null;
                    if (bidId > 0)
                    {
                        if (DicBidMaterialType.ContainsKey(bidId))
                            bidMaterial = this.DicBidMaterialType[bidId].FirstOrDefault(o => o.SUPPLIER_ID == supplierId && o.MATERIAL_TYPE_ID == this.materialType.ID && o.BID_GROUP_CODE == this.materialType.BidGroupCode);
                        if (bidMaterial != null)
                        {
                            spMonthLifespan.EditValue = bidMaterial.MONTH_LIFESPAN;
                            spDayLifespan.EditValue = bidMaterial.DAY_LIFESPAN;
                            spHourLifespan.EditValue = bidMaterial.HOUR_LIFESPAN;
                            spAmount.EditValue = bidMaterial.AMOUNT;
                            spImpVat.EditValue = bidMaterial.IMP_VAT_RATIO * 100;


                            spImpPrice.EditValue = bidMaterial.IMP_PRICE;
                            spPriceVat.EditValue = spContractPrice.Value;
                        }
                        spImpVat.Enabled = false;
                        spMonthLifespan.Enabled = false;
                        spDayLifespan.Enabled = false;
                        spHourLifespan.Enabled = false;
                        lciHour.Enabled = false;
                        lciPnNationalName.Enabled = false;
                        dtExpiredDate.Enabled = false;
                        txtRegisterNumber.Enabled = false;
                        txtNationalName.Enabled = false;
                        chkEditNational.Enabled = false;
                        txtConcentra.Enabled = false;
                        txtManufacturerCode.Enabled = false;
                        cboManufacturerName.Enabled = false;
                        txtNhomThau.Enabled = false;
                        txtQDThau.Enabled = false;
                        txtGhiChu.Enabled = false;
                    }

                    if (bidMaterial != null)
                    {
                        var national = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_NATIONAL>().Where(o => o.NATIONAL_NAME == bidMaterial.NATIONAL_NAME).ToList();
                        if (national != null && national.Count() > 0)
                        {
                            txtNationalName.Text = national[0].NATIONAL_NAME;
                            cboNationalName.EditValue = national[0].ID;
                            txtNationalName.Visible = false;
                            cboNationalName.Visible = true;
                            chkEditNational.CheckState = System.Windows.Forms.CheckState.Unchecked;
                        }
                        else
                        {
                            txtNationalName.Text = this.materialType.NATIONAL_NAME;
                            cboNationalName.EditValue = null;
                            txtNationalName.Visible = true;
                            cboNationalName.Visible = false;
                            chkEditNational.CheckState = System.Windows.Forms.CheckState.Checked;
                        }

                        cboManufacturerName.EditValue = bidMaterial.MANUFACTURER_ID;
                        txtManufacturerCode.Text = bidMaterial.MANUFACTURER_CODE;
                        txtConcentra.Text = bidMaterial.CONCENTRA;

                        txtQDThau.Text = bidMaterial.BID_NUMBER;
                        txtNhomThau.Text = bidMaterial.BID_GROUP_CODE;
                        txtGhiChu.Text = bidMaterial.NOTE;

                        if (bidMaterial.EXPIRED_DATE.HasValue)
                        {
                            dtExpiredDate.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(bidMaterial.EXPIRED_DATE.Value);
                        }

                    }
                    else
                    {
                        var national = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_NATIONAL>().Where(o => o.NATIONAL_NAME == this.materialType.NATIONAL_NAME).ToList();
                        if (national != null && national.Count() > 0)
                        {
                            txtNationalName.Text = national[0].NATIONAL_NAME;
                            cboNationalName.EditValue = national[0].ID;
                            txtNationalName.Visible = false;
                            cboNationalName.Visible = true;
                            chkEditNational.CheckState = System.Windows.Forms.CheckState.Unchecked;
                        }
                        else
                        {
                            txtNationalName.Text = this.materialType.NATIONAL_NAME;
                            cboNationalName.EditValue = null;
                            txtNationalName.Visible = true;
                            cboNationalName.Visible = false;
                            chkEditNational.CheckState = System.Windows.Forms.CheckState.Checked;
                        }

                        cboManufacturerName.EditValue = this.materialType.MANUFACTURER_ID;
                        txtManufacturerCode.Text = this.materialType.MANUFACTURER_CODE;
                        txtConcentra.Text = this.materialType.CONCENTRA;
                        spImpVat.EditValue = this.materialType.IMP_VAT_RATIO * 100;

                        if (this.materialType.LAST_EXPIRED_DATE.HasValue)
                        {
                            dtExpiredDate.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.materialType.LAST_EXPIRED_DATE.Value);
                        }

                    }

                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Error("DEAULT _______________________");
                    EnableLeftControl(false);
                    SetDefaultValueControlDetail();
                }

                spAmount.Focus();
                spAmount.SelectAll();

                dxValidationProviderMetyMate.RemoveControlError(spAmount);
                dxValidationProviderMetyMate.RemoveControlError(spContractPrice);
                //dxValidationProviderMetyMate.RemoveControlError(spImpVat);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ClearForAdd()
        {
            try
            {
                spAmount.Value = 0;
                spImpVat.Value = 0;
                spContractPrice.Value = 0;
                //spImpPrice.Value = 0;
                //spPriceVat.Value = 0;
                dtExpiredDate.EditValue = null;
                dtImportDate.EditValue = null;
                txtRegisterNumber.EditValue = null;
                txtConcentra.EditValue = null;
                txtManufacturerCode.EditValue = null;
                cboManufacturerName.EditValue = null;
                cboNationalName.EditValue = null;
                txtNationalName.EditValue = null;

                //  if (chkClearControl.Checked)
                //  {
                spDayLifespan.EditValue = null;
                spMonthLifespan.EditValue = null;
                spHourLifespan.EditValue = null;

                spDayLifespan.Enabled = true;
                spMonthLifespan.Enabled = true;
                spHourLifespan.Enabled = true;
                lciHour.Enabled = true;


                txtGhiChu.EditValue = null;
                txtQDThau.EditValue = null;
                txtNhomThau.EditValue = null;
                dteDiscountFromDate.EditValue = null;
                dteDiscountToDate.EditValue = null;
                //  }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void EnableLeftControl(bool Enable)
        {
            try
            {
                txtConcentra.Enabled = Enable;
                txtManufacturerCode.Enabled = Enable;
                cboNationalName.Enabled = Enable;
                cboManufacturerName.Enabled = Enable;
                txtNationalName.Enabled = Enable;
                txtRegisterNumber.Enabled = Enable;
                txtQDThau.Enabled = Enable;
                txtGhiChu.Enabled = Enable;
                txtNhomThau.Enabled = Enable;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool addMedicine()
        {
            bool result = false;
            try
            {
                ADO.MetyMatyADO ado = new ADO.MetyMatyADO();
                long bidId = Inventec.Common.TypeConvert.Parse.ToInt64((cboBid.EditValue ?? 0).ToString());
                V_HIS_BID_MEDICINE_TYPE bidMedicine = null;
                string bidGroupCode = null;
                if (this.ActionType == GlobalVariables.ActionEdit)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<ADO.MetyMatyADO>(ado, this.EditMedicine);
                    if (DicBidMedicineType.ContainsKey(bidId))
                        bidMedicine = this.DicBidMedicineType[bidId].FirstOrDefault(o => o.ID == this.EditMedicine.BID_METY_MATY_ID);
                    bidGroupCode = this.EditMedicine.BID_GROUP_CODE;

                }
                else if (this.ActionType == GlobalVariables.ActionAdd)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<ADO.MetyMatyADO>(ado, this.medicineType);

                    long supplierId = Inventec.Common.TypeConvert.Parse.ToInt64((cboSupplier.EditValue ?? 0).ToString());
                    if (DicBidMedicineType.ContainsKey(bidId))
                        bidMedicine = this.DicBidMedicineType[bidId].FirstOrDefault(o => o.SUPPLIER_ID == supplierId && o.MEDICINE_TYPE_ID == this.medicineType.ID && o.BID_GROUP_CODE == medicineType.BidGroupCode);
                    if (bidMedicine != null)
                    {
                        ado.BID_METY_MATY_ID = bidMedicine.ID;
                    }
                    bidGroupCode = this.medicineType.BidGroupCode;
                }
                #region
                if (bidMedicine != null)
                {
                    // lấy ra dữ liệu khi mới mở form Edit
                    ADO.MetyMatyADO metyMatyADO = new ADO.MetyMatyADO();
                    if (this.ActionType == GlobalVariables.ActionEdit)
                        metyMatyADO = ListMedicineADOTemp.FirstOrDefault(o => o.ID == this.EditMedicine.ID);
                    // lưu rồi thì mới trừ số lượng.                   
                    decimal checkAmount = spAmount.Value - (this.ActionType == GlobalVariables.ActionEdit && metyMatyADO != null && metyMatyADO.AMOUNT.HasValue && metyMatyADO.CONTRACT_MATY_METY_ID > 0 ? metyMatyADO.AMOUNT.Value : 0);
                    decimal VariableAmount = bidMedicine.AMOUNT * (1 + (bidMedicine.IMP_MORE_RATIO ?? 0)) - (bidMedicine.TDL_CONTRACT_AMOUNT ?? 0) + (bidMedicine.ADJUST_AMOUNT ?? 0);
                    if (VariableAmount < checkAmount)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceLanguageManager.SoLuongHopDongVuotQuaSoLuongThau, Resources.ResourceLanguageManager.ThongBao);
                        return result;
                    }

                    if (spContractPrice.Value > (bidMedicine.IMP_PRICE ?? 0) * (1 + (bidMedicine.IMP_VAT_RATIO ?? 0)))
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceLanguageManager.GiaHopDongVuotQuaThau, Resources.ResourceLanguageManager.ThongBao);
                        return result;
                    }

                    if (bidMedicine.IMP_VAT_RATIO.HasValue && bidMedicine.IMP_VAT_RATIO.Value < (spImpVat.Value / 100))
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceLanguageManager.VatNhapHopDongVuotQuaThau, Resources.ResourceLanguageManager.ThongBao);
                        return result;
                    }
                }
                ado.IMP_PRICE = spImpPrice.Value;
                ado.IMP_VAT_RATIO = spImpVat.Value / 100;
                ado.ImpVatRatio = spImpVat.Value;
                ado.AMOUNT = spAmount.Value;
                ado.NATIONAL_NAME = txtNationalName.Text.Trim();
                ado.CONCENTRA = txtConcentra.Text.Trim();
                if (!string.IsNullOrEmpty(this.bidGroupCodeSelected))
                {
                    ado.BID_GROUP_CODE = this.bidGroupCodeSelected;
                }
                else if (!string.IsNullOrEmpty(txtNhomThau.Text.Trim()))
                {
                    ado.BID_GROUP_CODE = txtNhomThau.Text.Trim();
                }
                else
                {
                    ado.BID_GROUP_CODE = null;
                }
                ado.NOTE = txtGhiChu.Text;
                ado.BID_NUMBER = txtQDThau.Text;


                ado.REGISTER_NUMBER = txtRegisterNumber.Text.Trim();
                ado.CONTRACT_PRICE = spContractPrice.Value;


                if (spMonthLifespan.Value > 0)
                {
                    ado.MonthLifespan = (long)spMonthLifespan.Value;
                }
                else
                {
                    ado.MonthLifespan = null;
                }

                if (spDayLifespan.Value > 0)
                {
                    ado.DayLifespan = (long)spDayLifespan.Value;
                }
                else
                {
                    ado.DayLifespan = null;
                }
                if (spHourLifespan.Value > 0)
                {
                    ado.HourLifespan = (long)spHourLifespan.Value;
                }
                else
                {
                    ado.HourLifespan = null;
                }

                if (cboBid.EditValue != null)
                {
                    ado.BID_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboBid.EditValue.ToString());
                }
                else
                {
                    ado.BID_ID = 0;
                }

                if (dtExpiredDate.EditValue != null)
                {
                    if (dtExpiredDate.DateTime < DateTime.Today)
                    {
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceLanguageManager.HanSuDungKhongDuocNhoHonNgayHienTai, Resources.ResourceLanguageManager.ThongBao, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return result;
                    }

                    ado.EXPIRED_DATE = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtExpiredDate.DateTime);
                }
                else
                {
                    ado.EXPIRED_DATE = null;
                }
                if (dtImportDate.EditValue != null)
                {
                    ado.IMP_EXPIRED_DATE = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtImportDate.DateTime);
                }
                else
                {
                    ado.IMP_EXPIRED_DATE = null;
                }


                if (cboManufacturerName.EditValue != null)
                {
                    var manu = BackendDataWorker.Get<HIS_MANUFACTURER>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt32(cboManufacturerName.EditValue.ToString()));
                    if (manu != null)
                    {
                        ado.MANUFACTURER_CODE = manu.MANUFACTURER_CODE;
                        ado.MANUFACTURER_ID = manu.ID;
                        ado.MANUFACTURER_NAME = manu.MANUFACTURER_NAME;
                    }
                }
                else
                {
                    ado.MANUFACTURER_ID = null;
                    ado.MANUFACTURER_NAME = null;
                }

                ado.DISCOUNT_FROM_DATE = DiscountFromDate;
                ado.DISCOUNT_TO_DATE = DiscountToDate;
                #endregion
                var aMedicineType = this.ListMedicineADO.FirstOrDefault(o => o.ID == this.medicineType.ID && o.CONTRACT_PRICE == ado.CONTRACT_PRICE && (o.BID_GROUP_CODE ?? "") == (ado.BID_GROUP_CODE ?? "") && (o.BID_NUMBER ?? "") == (ado.BID_NUMBER ?? ""));

                if (aMedicineType != null && aMedicineType.ID > 0)
                {
                    if (this.ActionType == GlobalVariables.ActionAdd)
                    {
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(string.Format(Resources.ResourceLanguageManager.DaTonTaiThuocVatTu, string.Format(Resources.ResourceLanguageManager.CanhBaoThuoc, ado.MEDICINE_TYPE_NAME)), Resources.ResourceLanguageManager.ThongBao, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return result;

                    }
                    this.ListMedicineADO.Remove(aMedicineType);

                }
                this.ListMedicineADO.Add(ado);
                this.xtraTabContractMetyMaty.SelectedTabPage = tabMedicalContractMety;

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private bool addMaterial()
        {
            bool result = false;
            try
            {
                ADO.MetyMatyADO ado = new ADO.MetyMatyADO();

                V_HIS_BID_MATERIAL_TYPE bidMaterial = null;
                string bidGroupCode = null;

                long bidId = Inventec.Common.TypeConvert.Parse.ToInt64((cboBid.EditValue ?? 0).ToString());
                if (this.ActionType == GlobalVariables.ActionEdit)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<ADO.MetyMatyADO>(ado, this.EditMaterial);
                    if (DicBidMaterialType.ContainsKey(bidId))
                        bidMaterial = this.DicBidMaterialType[bidId].FirstOrDefault(o => o.ID == this.EditMaterial.BID_METY_MATY_ID);
                    bidGroupCode = this.EditMaterial.BID_GROUP_CODE;

                }
                else if (this.ActionType == GlobalVariables.ActionAdd)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<ADO.MetyMatyADO>(ado, this.materialType);
                    ado.MEDICINE_TYPE_CODE = materialType.MATERIAL_TYPE_CODE;
                    ado.MEDICINE_TYPE_NAME = materialType.MATERIAL_TYPE_NAME;
                    ado.BID_GROUP_CODE = materialType.BidGroupCode;
                    long supplierId = Inventec.Common.TypeConvert.Parse.ToInt64((cboSupplier.EditValue ?? 0).ToString());
                    if (DicBidMaterialType.ContainsKey(bidId))
                        bidMaterial = this.DicBidMaterialType[bidId].FirstOrDefault(o => o.SUPPLIER_ID == supplierId && o.MATERIAL_TYPE_ID == this.materialType.ID && o.BID_GROUP_CODE == materialType.BidGroupCode);
                    if (bidMaterial != null)
                    {
                        ado.BID_METY_MATY_ID = bidMaterial.ID;
                    }
                    bidGroupCode = this.materialType.BidGroupCode;
                }

                if (bidMaterial != null)
                {
                    // lấy ra dữ liệu khi mới mở form Edit
                    ADO.MetyMatyADO metyMatyADO = new ADO.MetyMatyADO();
                    if (this.ActionType == GlobalVariables.ActionEdit)
                        metyMatyADO = ListMaterialADOTemp.FirstOrDefault(o => o.ID == this.EditMaterial.ID);
                    // lưu rồi thì mới trừ số lượng.                   
                    decimal checkAmount = spAmount.Value - (this.ActionType == GlobalVariables.ActionEdit && metyMatyADO != null && metyMatyADO.AMOUNT.HasValue && metyMatyADO.CONTRACT_MATY_METY_ID > 0 ? metyMatyADO.AMOUNT.Value : 0);
                    decimal VariableAmount = bidMaterial.AMOUNT * (1 + (bidMaterial.IMP_MORE_RATIO ?? 0)) - (bidMaterial.TDL_CONTRACT_AMOUNT ?? 0) + (bidMaterial.ADJUST_AMOUNT ?? 0);
                    if (VariableAmount < checkAmount)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceLanguageManager.SoLuongHopDongVuotQuaSoLuongThau, Resources.ResourceLanguageManager.ThongBao);
                        return result;
                    }

                    if (spContractPrice.Value > (bidMaterial.IMP_PRICE ?? 0) * (1 + (bidMaterial.IMP_VAT_RATIO ?? 0)))
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceLanguageManager.GiaHopDongVuotQuaThau, Resources.ResourceLanguageManager.ThongBao);
                        return result;
                    }

                    if (bidMaterial.IMP_VAT_RATIO.HasValue && bidMaterial.IMP_VAT_RATIO.Value < (spImpVat.Value / 100))
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceLanguageManager.VatNhapHopDongVuotQuaThau, Resources.ResourceLanguageManager.ThongBao);
                        return result;
                    }
                }

                ado.IMP_PRICE = spImpPrice.Value;
                ado.IMP_VAT_RATIO = spImpVat.Value / 100;
                ado.ImpVatRatio = spImpVat.Value;
                ado.AMOUNT = spAmount.Value;
                ado.NATIONAL_NAME = txtNationalName.Text.Trim();
                ado.CONCENTRA = txtConcentra.Text.Trim();




                ado.REGISTER_NUMBER = txtRegisterNumber.Text.Trim();
                ado.CONTRACT_PRICE = spContractPrice.Value;

                ado.NOTE = txtGhiChu.Text;

                ado.BID_NUMBER = txtQDThau.Text;
                if (!string.IsNullOrEmpty(this.bidGroupCodeSelected))
                {
                    ado.BID_GROUP_CODE = this.bidGroupCodeSelected;
                }
                else if (!string.IsNullOrEmpty(txtNhomThau.Text.Trim()))
                {
                    ado.BID_GROUP_CODE = txtNhomThau.Text.Trim();
                }
                else
                {
                    ado.BID_GROUP_CODE = null;
                }

                if (spMonthLifespan.Value > 0)
                {
                    ado.MonthLifespan = (long)spMonthLifespan.Value;
                }
                else
                {
                    ado.MonthLifespan = null;
                }


                if (spDayLifespan.Value > 0)
                {
                    ado.DayLifespan = (long)spDayLifespan.Value;
                }
                else
                {
                    ado.DayLifespan = null;
                }
                if (spHourLifespan.Value > 0)
                {
                    ado.HourLifespan = (long)spHourLifespan.Value;
                }
                else
                {
                    ado.HourLifespan = null;
                }

                if (dtExpiredDate.EditValue != null)
                {
                    if (dtExpiredDate.DateTime < DateTime.Today)
                    {
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceLanguageManager.HanSuDungKhongDuocNhoHonNgayHienTai, Resources.ResourceLanguageManager.ThongBao, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return result;
                    }

                    ado.EXPIRED_DATE = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtExpiredDate.DateTime);
                }
                else
                {
                    ado.EXPIRED_DATE = null;
                }
                if (dtImportDate.EditValue != null)
                {
                    ado.IMP_EXPIRED_DATE = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtImportDate.DateTime);
                }
                else
                {
                    ado.IMP_EXPIRED_DATE = null;
                }

                if (cboManufacturerName.EditValue != null)
                {
                    var manu = BackendDataWorker.Get<HIS_MANUFACTURER>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt32(cboManufacturerName.EditValue.ToString()));
                    if (manu != null)
                    {
                        ado.MANUFACTURER_CODE = manu.MANUFACTURER_CODE;
                        ado.MANUFACTURER_ID = manu.ID;
                        ado.MANUFACTURER_NAME = manu.MANUFACTURER_NAME;
                    }
                }
                else
                    ado.MANUFACTURER_ID = null;

                ado.DISCOUNT_FROM_DATE = DiscountFromDate;
                ado.DISCOUNT_TO_DATE = DiscountToDate;
                var aMaterialType = this.ListMaterialADO.FirstOrDefault(o => o.ID == this.materialType.ID && o.CONTRACT_PRICE == ado.CONTRACT_PRICE && (o.BID_GROUP_CODE ?? "") == (ado.BID_GROUP_CODE ?? "") && (o.BID_NUMBER ?? "") == (ado.BID_NUMBER ?? ""));

                if (aMaterialType != null && aMaterialType.ID > 0)
                {
                    if (this.ActionType == GlobalVariables.ActionAdd)
                    {
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(string.Format(Resources.ResourceLanguageManager.DaTonTaiThuocVatTu, string.Format(Resources.ResourceLanguageManager.CanhBaoVatTu, ado.MEDICINE_TYPE_NAME)), Resources.ResourceLanguageManager.ThongBao, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                            return result;
                    }
                    this.ListMaterialADO.Remove(aMaterialType);

                }

                this.ListMaterialADO.Add(ado);
                this.xtraTabContractMetyMaty.SelectedTabPage = tabMedicalContractMaty;
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void UpdateGrid()
        {
            try
            {
                ucContractMaty.Reload(this.ListMaterialADO);
                ucContractMety.Reload(this.ListMedicineADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool CheckValidDataInGridService(ref CommonParam param, List<ADO.MetyMatyADO> medicines, List<ADO.MetyMatyADO> materials)
        {
            bool valid = true;
            try
            {
                if ((medicines != null && medicines.Count > 0) || (materials != null && materials.Count > 0))
                {
                    if (param.Messages == null)
                    {
                        param.Messages = new List<string>();
                    }

                    long supplierId = Inventec.Common.TypeConvert.Parse.ToInt64((cboSupplier.EditValue ?? 0).ToString());

                    long bidId = Inventec.Common.TypeConvert.Parse.ToInt64((cboBid.EditValue ?? 0).ToString());

                    bool IsHasMessageAdjustAmount = false;

                    if (medicines != null && medicines.Count > 0)
                    {
                        bool CheckAmount = false;
                        foreach (var item in medicines)
                        {
                            string messageErr = String.Format(Resources.ResourceLanguageManager.CanhBaoThuoc, item.MEDICINE_TYPE_NAME);
                            bool result = true;

                            if (item.AMOUNT <= 0)
                            {
                                CheckAmount = true;
                                result = false;
                                messageErr += Resources.ResourceLanguageManager.SoLuongKhongDuocAm;
                            }

                            if (!result)
                            {
                                param.Messages.Add(messageErr + ";");
                            }
                        }
                        if (!CheckAmount && DicBidMedicineType != null && DicBidMedicineType.Count > 0)
                        {
                            var dt = medicines.GroupBy(o => o.ID).ToList();
                            List<string> message = new List<string>();
                            foreach (var item in dt)
                            {
                                var AmountTotal = item.ToList().Sum(o => o.AMOUNT);
                                V_HIS_BID_MEDICINE_TYPE bidMedicine = null;
                                if (this.ActionType == GlobalVariables.ActionEdit)
                                {
                                    if (DicBidMedicineType.ContainsKey(bidId))
                                        bidMedicine = this.DicBidMedicineType[bidId].FirstOrDefault(o => o.ID == item.FirstOrDefault().BID_METY_MATY_ID);

                                }
                                else if (this.ActionType == GlobalVariables.ActionAdd)
                                {
                                    if (DicBidMedicineType.ContainsKey(bidId))
                                        bidMedicine = this.DicBidMedicineType[bidId].FirstOrDefault(o => o.SUPPLIER_ID == supplierId && o.MEDICINE_TYPE_ID == item.FirstOrDefault().ID && o.BID_GROUP_CODE == item.FirstOrDefault().BID_GROUP_CODE);
                                }
                                if (bidMedicine != null)
                                {
                                    decimal VariableAmount = bidMedicine.AMOUNT * (1 + (bidMedicine.IMP_MORE_RATIO ?? 0)) + (bidMedicine.ADJUST_AMOUNT ?? 0);
                                    if (AmountTotal > VariableAmount)
                                    {
                                        message.Add(string.Format("Thuốc {0} (Hợp đồng: {1} - Thầu: {2})", item.ToList().FirstOrDefault().MEDICINE_TYPE_NAME, AmountTotal, VariableAmount));
                                    }
                                }
                            }
                            if (message != null && message.Count > 0)
                            {
                                IsHasMessageAdjustAmount = true;
                                param.Messages.Add("Thuốc/vật tư có tổng số lượng hợp đồng lớn hơn số lượng trong thầu." + string.Join(", ", message));
                            }
                        }
                    }

                    if (materials != null && materials.Count > 0)
                    {
                        bool CheckAmount = false;
                        foreach (var item in materials)
                        {
                            string messageErr = String.Format(Resources.ResourceLanguageManager.CanhBaoVatTu, item.MEDICINE_TYPE_NAME);
                            bool result = true;

                            if (item.AMOUNT <= 0)
                            {
                                result = false;
                                CheckAmount = true;
                                messageErr += Resources.ResourceLanguageManager.SoLuongKhongDuocAm;
                            }

                            if (!result)
                            {
                                param.Messages.Add(messageErr + ";");
                            }
                        }
                        if (!CheckAmount && DicBidMaterialType != null && DicBidMaterialType.Count > 0)
                        {
                            var dt = materials.GroupBy(o => o.ID).ToList();
                            List<string> message = new List<string>();
                            foreach (var item in dt)
                            {
                                var AmountTotal = item.ToList().Sum(o => o.AMOUNT);
                                V_HIS_BID_MATERIAL_TYPE bidMaterial = null;
                                if (this.ActionType == GlobalVariables.ActionEdit)
                                {
                                    if (DicBidMaterialType.ContainsKey(bidId))
                                        bidMaterial = this.DicBidMaterialType[bidId].FirstOrDefault(o => o.ID == item.FirstOrDefault().BID_METY_MATY_ID);

                                }
                                else if (this.ActionType == GlobalVariables.ActionAdd)
                                {
                                    if (DicBidMaterialType.ContainsKey(bidId))
                                        bidMaterial = this.DicBidMaterialType[bidId].FirstOrDefault(o => o.SUPPLIER_ID == supplierId && o.MATERIAL_TYPE_ID == item.FirstOrDefault().ID && o.BID_GROUP_CODE == item.FirstOrDefault().BID_GROUP_CODE);
                                }
                                if (bidMaterial != null)
                                {
                                    decimal VariableAmount = bidMaterial.AMOUNT * (1 + (bidMaterial.IMP_MORE_RATIO ?? 0)) + (bidMaterial.ADJUST_AMOUNT ?? 0);
                                    if (AmountTotal > VariableAmount)
                                    {
                                        message.Add(string.Format("Vật tư {0} (Hợp đồng: {1} - Thầu: {2})", item.ToList().FirstOrDefault().MEDICINE_TYPE_NAME, AmountTotal, VariableAmount));
                                    }
                                }
                            }
                            if (message != null && message.Count > 0)
                            {
                                IsHasMessageAdjustAmount = true;
                                param.Messages.Add(string.Join(", ", message));
                            }
                        }
                    }
                }
                else
                {
                    param.Messages.Add(Resources.ResourceLanguageManager.ThieuTruongDuLieuBatBuoc);
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => medicines), medicines));
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => materials), materials));
                }

                if (param.Messages.Count > 0)
                {
                    param.Messages = param.Messages.Distinct().ToList();
                    valid = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                valid = false;
            }
            return valid;
        }

        private void GetDataForSave()
        {
            try
            {
                this.MedicalContractSDO = new MOS.SDO.HisMedicalContractSDO();
                this.MedicalContractSDO.MaterialTypes = new List<MOS.SDO.HisMediContractMatySDO>();
                this.MedicalContractSDO.MedicineTypes = new List<MOS.SDO.HisMediContractMetySDO>();
                this.MedicalContractSDO.MedicalContractCode = !string.IsNullOrEmpty(txtMedicalContractCode.Text.Trim()) ? txtMedicalContractCode.Text.Trim() : null;
                this.MedicalContractSDO.MedicalContractName = !string.IsNullOrEmpty(txtMedicalContractName.Text.Trim()) ? txtMedicalContractName.Text.Trim() : null;
                this.MedicalContractSDO.VentureAgreening = !string.IsNullOrEmpty(txtVentureAgreening.Text.Trim()) ? txtVentureAgreening.Text.Trim() : null;
                this.MedicalContractSDO.Note = !string.IsNullOrEmpty(txtNote.Text.Trim()) ? txtNote.Text.Trim() : null;

                if (cboSupplier.EditValue != null)
                {
                    this.MedicalContractSDO.SupplierId = Inventec.Common.TypeConvert.Parse.ToInt64(cboSupplier.EditValue.ToString());
                }
                else
                {
                    this.MedicalContractSDO.SupplierId = 0;
                }

                if (cboBid.EditValue != null)
                {
                    this.MedicalContractSDO.BidId = Inventec.Common.TypeConvert.Parse.ToInt64(cboBid.EditValue.ToString());
                }
                else
                {
                    this.MedicalContractSDO.BidId = null;
                }

                if (cboDocumentSupplier.EditValue != null)
                {
                    this.MedicalContractSDO.DocumentSupplierId = Inventec.Common.TypeConvert.Parse.ToInt64(cboDocumentSupplier.EditValue.ToString());
                }
                else
                {
                    this.MedicalContractSDO.DocumentSupplierId = null;
                }

                if (dtValidFromDate.EditValue != null && dtValidFromDate.DateTime != DateTime.MinValue)
                {
                    this.MedicalContractSDO.ValidFromDate = Inventec.Common.TypeConvert.Parse.ToInt64(dtValidFromDate.DateTime.ToString("yyyyMMdd") + "000000");
                }
                else
                {
                    this.MedicalContractSDO.ValidFromDate = null;
                }

                if (dtValidToDate.EditValue != null && dtValidToDate.DateTime != DateTime.MinValue)
                {
                    this.MedicalContractSDO.ValidToDate = Inventec.Common.TypeConvert.Parse.ToInt64(dtValidToDate.DateTime.ToString("yyyyMMdd") + "000000");
                }
                else
                {
                    this.MedicalContractSDO.ValidToDate = null;
                }

                if (this.ListMaterialADO != null && this.ListMaterialADO.Count > 0)
                {
                    foreach (var item in this.ListMaterialADO)
                    {
                        var maty = new MOS.SDO.HisMediContractMatySDO();
                        maty.Id = item.CONTRACT_MATY_METY_ID;
                        maty.Amount = (decimal)(item.AMOUNT ?? 0);
                        maty.ImpPrice = (decimal)(item.IMP_PRICE ?? 0);
                        maty.ImpVatRatio = item.IMP_VAT_RATIO;
                        maty.MaterialTypeId = item.ID;
                        maty.BidMaterialTypeId = item.BID_METY_MATY_ID;
                        maty.ExpiredDate = item.EXPIRED_DATE;
                        maty.Concentra = !string.IsNullOrEmpty(item.CONCENTRA) ? item.CONCENTRA : null;
                        maty.ManufacturerId = item.MANUFACTURER_ID;
                        maty.NationalName = !string.IsNullOrEmpty(item.NATIONAL_NAME) ? item.NATIONAL_NAME : null;

                        maty.MonthLifespan = item.MonthLifespan;
                        maty.DayLifespan = item.DayLifespan;
                        maty.HourLifespan = item.HourLifespan;

                        maty.Note = !string.IsNullOrEmpty(item.NOTE) ? item.NOTE : null;
                        maty.BidGroupCode = !string.IsNullOrEmpty(item.BID_GROUP_CODE) ? item.BID_GROUP_CODE : null;
                        maty.BidNumber = !string.IsNullOrEmpty(item.BID_NUMBER) ? item.BID_NUMBER : null;

                        maty.ContractPrice = item.CONTRACT_PRICE ?? 0;
                        maty.ImpExpiredDate = item.IMP_EXPIRED_DATE;
                        maty.DiscountFromDate = item.DISCOUNT_FROM_DATE;
                        maty.DiscountToDate = item.DISCOUNT_TO_DATE;
                        this.MedicalContractSDO.MaterialTypes.Add(maty);
                    }
                }

                if (this.ListMedicineADO != null && this.ListMedicineADO.Count > 0)
                {
                    foreach (var item in this.ListMedicineADO)
                    {
                        var mety = new MOS.SDO.HisMediContractMetySDO();
                        mety.Id = item.CONTRACT_MATY_METY_ID;
                        mety.Amount = (decimal)(item.AMOUNT ?? 0);
                        mety.ImpPrice = (decimal)(item.IMP_PRICE ?? 0);
                        mety.ImpVatRatio = item.IMP_VAT_RATIO;
                        mety.MedicineTypeId = item.ID;
                        mety.BidMedicineTypeId = item.BID_METY_MATY_ID;
                        mety.ExpiredDate = item.EXPIRED_DATE;
                        mety.Concentra = !string.IsNullOrEmpty(item.CONCENTRA) ? item.CONCENTRA : null;
                        mety.ManufacturerId = item.MANUFACTURER_ID;
                        mety.NationalName = !string.IsNullOrEmpty(item.NATIONAL_NAME) ? item.NATIONAL_NAME : null;

                        mety.MonthLifespan = item.MonthLifespan;
                        mety.DayLifespan = item.DayLifespan;
                        mety.HourLifespan = item.HourLifespan;

                        mety.ContractPrice = item.CONTRACT_PRICE ?? 0;
                        mety.MedicineRegisterNumber = item.REGISTER_NUMBER;
                        mety.ImpExpiredDate = item.IMP_EXPIRED_DATE;

                        mety.Note = !string.IsNullOrEmpty(item.NOTE) ? item.NOTE : null;
                        mety.BidGroupCode = !string.IsNullOrEmpty(item.BID_GROUP_CODE) ? item.BID_GROUP_CODE : null;
                        mety.BidNumber = !string.IsNullOrEmpty(item.BID_NUMBER) ? item.BID_NUMBER : null;

                        mety.DiscountFromDate = item.DISCOUNT_FROM_DATE;
                        mety.DiscountToDate = item.DISCOUNT_TO_DATE;
                        this.MedicalContractSDO.MedicineTypes.Add(mety);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void addListMaterialTypeToProcessList(List<ADO.MetyMatyADO> listMaterial, ref List<ADO.MetyMatyADO> listError)
        {
            try
            {
                if (listMaterial != null && listMaterial.Count > 0)
                {
                    foreach (var materialTypeImport in listMaterial)
                    {
                        Inventec.Common.Logging.LogSystem.Error(materialTypeImport.BID_METY_MATY_ID + "____________");
                        var materialTypeNotExist = this.ListHisMaterialType.FirstOrDefault(o => o.MATERIAL_TYPE_CODE == materialTypeImport.MEDICINE_TYPE_CODE);
                        if (materialTypeNotExist == null) continue;

                        var materialType = new ADO.MetyMatyADO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<ADO.MetyMatyADO>(materialType, materialTypeImport);
                        Inventec.Common.Mapper.DataObjectMapper.Map<ADO.MetyMatyADO>(materialType, materialTypeNotExist);

                        materialType.MEDICINE_TYPE_CODE = materialTypeNotExist.MATERIAL_TYPE_CODE;
                        materialType.MEDICINE_TYPE_NAME = materialTypeNotExist.MATERIAL_TYPE_NAME;
                        materialType.IMP_PRICE = materialTypeImport.IMP_PRICE;
                        materialType.AMOUNT = materialTypeImport.AMOUNT;
                        materialType.EXPIRED_DATE = materialTypeImport.EXPIRED_DATE;
                        materialType.IMP_EXPIRED_DATE = materialTypeImport.IMP_EXPIRED_DATE;

                        materialType.BID_METY_MATY_ID = materialTypeImport.BID_METY_MATY_ID;

                        materialType.MonthLifespan = materialTypeImport.MonthLifespan;
                        materialType.DayLifespan = materialTypeImport.DayLifespan;
                        materialType.HourLifespan = materialTypeImport.HourLifespan;

                        if (!String.IsNullOrWhiteSpace(materialTypeImport.SERVICE_UNIT_CODE))
                        {
                            var serrviceUnit = BackendDataWorker.Get<HIS_SERVICE_UNIT>().FirstOrDefault(o => o.SERVICE_UNIT_CODE == materialTypeImport.SERVICE_UNIT_CODE);
                            if (serrviceUnit != null)
                            {
                                materialType.SERVICE_UNIT_ID = serrviceUnit.ID;
                                materialType.SERVICE_UNIT_NAME = serrviceUnit.SERVICE_UNIT_NAME;
                            }
                        }

                        if (materialTypeImport.IMP_VAT_RATIO != null)
                        {
                            materialType.ImpVatRatio = materialTypeImport.IMP_VAT_RATIO;
                            materialType.IMP_VAT_RATIO = materialTypeImport.IMP_VAT_RATIO / 100;
                        }
                        else
                        {
                            materialType.ImpVatRatio = 0;
                            materialType.IMP_VAT_RATIO = 0;
                        }

                        if (!String.IsNullOrWhiteSpace(materialTypeImport.MANUFACTURER_CODE))
                        {
                            var manufacturer = BackendDataWorker.Get<HIS_MANUFACTURER>().FirstOrDefault(o => o.MANUFACTURER_CODE == materialTypeImport.MANUFACTURER_CODE);
                            if (manufacturer != null)
                            {
                                materialType.MANUFACTURER_CODE = manufacturer.MANUFACTURER_CODE;
                                materialType.MANUFACTURER_ID = manufacturer.ID;
                                materialType.MANUFACTURER_NAME = manufacturer.MANUFACTURER_NAME;
                            }
                            else
                            {
                                materialType.MANUFACTURER_CODE = null;
                            }
                        }

                        if (!String.IsNullOrWhiteSpace(materialTypeImport.EXPIRED_DATE_STR))
                        {
                            materialType.EXPIRED_DATE = Inventec.Common.TypeConvert.Parse.ToInt64(materialTypeImport.EXPIRED_DATE_STR);
                        }
                        if (!string.IsNullOrEmpty(materialTypeImport.BID_GROUP_CODE))
                        {
                            materialType.BID_GROUP_CODE = materialTypeImport.BID_GROUP_CODE;
                        }

                        this.ListMaterialADO.Insert(0, materialType);
                    }

                    if (this.ListMaterialADO.Count > 0)
                    {
                        var group = ListMaterialADO.GroupBy(o => o.ID).ToList();
                        foreach (var item in group)
                        {
                            if (item.Count() > 1)
                            {
                                listError.Add(item.First());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void addListMedicineTypeToProcessList(List<ADO.MetyMatyADO> listMedicine, ref List<ADO.MetyMatyADO> listError)
        {
            try
            {
                if (listMedicine != null && listMedicine.Count > 0)
                {
                    foreach (var medicineTypeImport in listMedicine)
                    {
                        Inventec.Common.Logging.LogSystem.Error(medicineTypeImport.BID_METY_MATY_ID + "____________");
                        var medicineTypeNotExist = ListHisMedicineType.FirstOrDefault(o => o.MEDICINE_TYPE_CODE == medicineTypeImport.MEDICINE_TYPE_CODE);
                        if (medicineTypeNotExist == null) continue;

                        var medicineType = new ADO.MetyMatyADO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<ADO.MetyMatyADO>(medicineType, medicineTypeImport);
                        Inventec.Common.Mapper.DataObjectMapper.Map<ADO.MetyMatyADO>(medicineType, medicineTypeNotExist);

                        medicineType.EXPIRED_DATE = medicineTypeImport.EXPIRED_DATE;
                        medicineType.IMP_PRICE = medicineTypeImport.IMP_PRICE;
                        medicineType.AMOUNT = medicineTypeImport.AMOUNT;
                        medicineType.IMP_EXPIRED_DATE = medicineTypeImport.IMP_EXPIRED_DATE;

                        medicineType.BID_METY_MATY_ID = medicineTypeImport.BID_METY_MATY_ID;

                        medicineType.MonthLifespan = medicineTypeImport.MonthLifespan;
                        medicineType.DayLifespan = medicineTypeImport.DayLifespan;
                        medicineType.HourLifespan = medicineTypeImport.HourLifespan;



                        if (medicineTypeImport.SERVICE_UNIT_CODE != null)
                        {
                            var serrviceUnit = BackendDataWorker.Get<HIS_SERVICE_UNIT>().FirstOrDefault(o => o.SERVICE_UNIT_CODE == medicineTypeImport.SERVICE_UNIT_CODE);
                            if (serrviceUnit != null)
                            {
                                medicineType.SERVICE_UNIT_ID = serrviceUnit != null ? serrviceUnit.ID : 0;
                                medicineType.SERVICE_UNIT_NAME = serrviceUnit != null ? serrviceUnit.SERVICE_UNIT_NAME : null;
                            }
                        }

                        if (medicineTypeImport.IMP_VAT_RATIO != null)
                        {
                            medicineType.ImpVatRatio = medicineTypeImport.IMP_VAT_RATIO;
                            medicineType.IMP_VAT_RATIO = medicineTypeImport.IMP_VAT_RATIO / 100;
                        }
                        else
                        {
                            medicineType.ImpVatRatio = 0;
                            medicineType.IMP_VAT_RATIO = 0;
                        }

                        if (!String.IsNullOrWhiteSpace(medicineTypeImport.MANUFACTURER_CODE))
                        {
                            var manufacturer = BackendDataWorker.Get<HIS_MANUFACTURER>().FirstOrDefault(o => o.MANUFACTURER_CODE == medicineTypeImport.MANUFACTURER_CODE);
                            if (manufacturer != null)
                            {
                                medicineType.MANUFACTURER_CODE = manufacturer.MANUFACTURER_CODE;
                                medicineType.MANUFACTURER_ID = manufacturer.ID;
                                medicineType.MANUFACTURER_NAME = manufacturer.MANUFACTURER_NAME;
                            }
                            else
                            {
                                medicineType.MANUFACTURER_CODE = null;
                            }
                        }

                        if (!String.IsNullOrWhiteSpace(medicineTypeImport.EXPIRED_DATE_STR))
                        {
                            medicineType.EXPIRED_DATE = Inventec.Common.TypeConvert.Parse.ToInt64(medicineTypeImport.EXPIRED_DATE_STR);
                        }
                        if (!string.IsNullOrEmpty(medicineTypeImport.BID_GROUP_CODE))
                        {
                            medicineType.BID_GROUP_CODE = medicineTypeImport.BID_GROUP_CODE;
                        }

                        this.ListMedicineADO.Add(medicineType);
                    }

                    if (this.ListMedicineADO.Count > 0)
                    {
                        var group = ListMedicineADO.GroupBy(o => o.ID).ToList();
                        foreach (var item in group)
                        {
                            if (item.Count() > 1)
                            {
                                listError.Add(item.First());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UpdateDetailId()
        {
            try
            {
                LoadMedicineType();
                LoadMaterialType();

                ucContractMaty.Reload(this.ListMaterialADO);
                ucContractMety.Reload(this.ListMedicineADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
