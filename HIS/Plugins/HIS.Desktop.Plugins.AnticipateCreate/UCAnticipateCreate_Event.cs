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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.AnticipateCreate
{
    public partial class UCAnticipateCreate : HIS.Desktop.Utility.UserControlBase
    {
        #region click
        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (lciBtnAdd.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Never) return;
                if (!btnAdd.Enabled && this.ActionType == GlobalVariables.ActionEdit) return;
                btnAdd.Focus();
                positionHandleLeft = -1;
                if (!dxValidationProviderLeftPanel.Validate()) return;

                if (xtraTabControl1.SelectedTabPageIndex == 0) // thuoc
                {
                    if (chkItemType.Checked)
                    {
                        if (!ProcessAddMedicine(cboSupplier, cboBid))
                            return;
                    }
                    else
                    {
                        if (!ProcessAddMedicine(cboSupplier1, cboBid1))
                            return;
                    }
                }

                else if (xtraTabControl1.SelectedTabPageIndex == 1) // vat tu
                {
                    if (chkItemType.Checked)
                    {
                        if (!ProcessAddMaterial(cboSupplier, cboBid))
                            return;
                    }
                    else
                    {
                        if (!ProcessAddMaterial(cboSupplier1, cboBid1))
                            return;
                    }
                }
                else if (xtraTabControl1.SelectedTabPageIndex == 2) // Mau
                {
                    ADO.MedicineTypeADO aBloodType = null;
                    if (chkItemType.Checked)
                    {
                        if (cboSupplier.EditValue != null)
                        {
                            aBloodType = this.ListMedicineTypeAdoProcess.FirstOrDefault(o => o.ID == this.bloodType.ID &&
                                o.Type == Base.GlobalConfig.MAU &&
                                o.SUPPLIER_ID == (long)cboSupplier.EditValue);
                        }
                        else
                        {
                            aBloodType = this.ListMedicineTypeAdoProcess.FirstOrDefault(o => o.ID == this.bloodType.ID &&
                                o.Type == Base.GlobalConfig.MAU);
                        }
                    }
                    else
                    {
                        if (cboSupplier1.EditValue != null)
                        {
                            aBloodType = this.ListMedicineTypeAdoProcess.FirstOrDefault(o => o.ID == this.bloodType.ID &&
                                o.Type == Base.GlobalConfig.MAU &&
                                o.SUPPLIER_ID == (long)cboSupplier1.EditValue);
                        }
                        else
                        {
                            aBloodType = this.ListMedicineTypeAdoProcess.FirstOrDefault(o => o.ID == this.bloodType.ID &&
                                o.Type == Base.GlobalConfig.MAU);
                        }
                    }

                    if (aBloodType != null && aBloodType.ID > 0)
                    {
                        this.ListMedicineTypeAdoProcess.RemoveAll(o => o == aBloodType);
                        addBlood();
                    }
                    else addBlood();
                }
                gridControlProcess.BeginUpdate();
                gridControlProcess.DataSource = null;
                gridControlProcess.DataSource = this.ListMedicineTypeAdoProcess;
                gridControlProcess.EndUpdate();
                gridViewProcess.FocusedRowHandle = (gridViewProcess.RowCount - 1);
                setValueAfterAdd();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool ProcessAddMedicine(GridLookUpEdit cbosupplier, GridLookUpEdit cbobid)
        {
            try
            {
                ADO.MedicineTypeADO aMedicineType = null;

                decimal? BidSupplierAmount = 0, SupplierAmount = 0, BidAmount = 0;

                List<V_HIS_BID_MEDICINE_TYPE> BidMedicine = new List<V_HIS_BID_MEDICINE_TYPE>();

                if (this.ListBidMedicineType != null && this.ListBidMedicineType.Count > 0)
                {
                    BidMedicine = ListBidMedicineType.Where(o => o.MEDICINE_TYPE_ID == this.medicineType.ID).ToList();
                    SupplierAmount = BidAmount = (BidMedicine != null && BidMedicine.Count > 0) ? BidMedicine.Sum(o => o.AMOUNT) + BidMedicine.Sum(o => o.ADJUST_AMOUNT) + BidMedicine.Sum(o => o.AMOUNT * o.IMP_MORE_RATIO) - (BidMedicine.Sum(o => o.IN_AMOUNT) != null ? BidMedicine.Sum(o => o.IN_AMOUNT) : 0) : 0;
                    if (cbosupplier.EditValue != null && cbobid.EditValue != null)
                    {
                        var MedicineOfSupplierAndBid = ListBidMedicineType.Where(o => o.SUPPLIER_ID == (long)cbosupplier.EditValue && o.BID_ID == (long)cbobid.EditValue && o.MEDICINE_TYPE_ID == this.medicineType.ID).ToList();
                        BidSupplierAmount = (MedicineOfSupplierAndBid != null && MedicineOfSupplierAndBid.Count > 0) ? MedicineOfSupplierAndBid.Sum(o => o.AMOUNT) + MedicineOfSupplierAndBid.Sum(o => o.ADJUST_AMOUNT) + MedicineOfSupplierAndBid.Sum(o => o.AMOUNT * o.IMP_MORE_RATIO) - (MedicineOfSupplierAndBid.Sum(o => o.IN_AMOUNT) != null ? MedicineOfSupplierAndBid.Sum(o => o.IN_AMOUNT) : 0) : 0;
                    }
                    else if (cbosupplier.EditValue != null)
                    {
                        var BidMedicineOfSupplier = ListBidMedicineType.Where(o => o.SUPPLIER_ID == (long)cbosupplier.EditValue && o.MEDICINE_TYPE_ID == this.medicineType.ID).ToList();
                        SupplierAmount = (BidMedicineOfSupplier != null && BidMedicineOfSupplier.Count > 0) ? BidMedicineOfSupplier.Sum(o => o.AMOUNT) + BidMedicineOfSupplier.Sum(o => o.ADJUST_AMOUNT) + BidMedicineOfSupplier.Sum(o => o.AMOUNT * o.IMP_MORE_RATIO) - (BidMedicineOfSupplier.Sum(o => o.IN_AMOUNT) != null ? BidMedicineOfSupplier.Sum(o => o.IN_AMOUNT) : 0) : 0;
                    }

                    else if (cbobid.EditValue != null)
                    {
                        var BidMedicineOfBid = ListBidMedicineType.Where(o => o.BID_ID == (long)cbobid.EditValue && o.MEDICINE_TYPE_ID == this.medicineType.ID).ToList();
                        BidAmount = (BidMedicineOfBid != null && BidMedicineOfBid.Count > 0) ? (BidMedicineOfBid.Sum(o => o.AMOUNT) + BidMedicineOfBid.Sum(o => o.ADJUST_AMOUNT) + BidMedicineOfBid.Sum(o => o.AMOUNT * o.IMP_MORE_RATIO) - (BidMedicineOfBid.Sum(o => o.IN_AMOUNT) != null ? BidMedicineOfBid.Sum(o => o.IN_AMOUNT) : 0)) : 0;
                    }

                }
                if (cbosupplier.EditValue != null)
                {
                    if (cbobid.EditValue != null)
                    {
                        if ((decimal)spinAmount.EditValue > BidSupplierAmount && isBusiness == false)
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show("Số lượng không được lớn hơn số lượng nhập còn lại theo nhà thầu và gói thầu của thuốc", "Thông báo");
                            return false;
                        }

                        aMedicineType = this.ListMedicineTypeAdoProcess.FirstOrDefault(o => o.ID == this.medicineType.ID &&
                       o.Type == Base.GlobalConfig.THUOC &&
                       o.SUPPLIER_ID == (long)cbosupplier.EditValue &&
                       o.BID_ID == (long)cbobid.EditValue);
                    }
                    else
                    {
                        if ((decimal)spinAmount.EditValue > SupplierAmount && isBusiness == false)
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show("Số lượng không được lớn hơn số lượng nhập còn lại theo nhà thầu của thuốc", "Thông báo");
                            return false;
                        }

                        aMedicineType = this.ListMedicineTypeAdoProcess.FirstOrDefault(o => o.ID == this.medicineType.ID &&
                       o.Type == Base.GlobalConfig.THUOC &&
                       o.SUPPLIER_ID == (long)cbosupplier.EditValue &&
                       o.BID_ID == null);
                    }
                }
                else
                {
                    if (cbobid.EditValue != null)
                    {
                        if ((decimal)spinAmount.EditValue > BidAmount && isBusiness == false)
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show("Số lượng không được lớn hơn số lượng nhập còn lại theo gói thầu của thuốc", "Thông báo");
                            return false;
                        }

                        aMedicineType = this.ListMedicineTypeAdoProcess.FirstOrDefault(o => o.ID == this.medicineType.ID &&
                        o.Type == Base.GlobalConfig.THUOC &&
                        o.SUPPLIER_ID == null &&
                        o.BID_ID == (long)cbobid.EditValue);
                    }
                    else
                    {
                        if ((decimal)spinAmount.EditValue > SupplierAmount && isBusiness == false)
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show("Số lượng không được lớn hơn số lượng nhập còn lại của thuốc", "Thông báo");
                            return false;
                        }

                        aMedicineType = this.ListMedicineTypeAdoProcess.FirstOrDefault(o => o.ID == this.medicineType.ID &&
                        o.Type == Base.GlobalConfig.THUOC &&
                        o.SUPPLIER_ID == null &&
                        o.BID_ID == null);
                    }
                }

                if (aMedicineType != null && aMedicineType.ID > 0)
                {
                    this.ListMedicineTypeAdoProcess.RemoveAll(o => o == aMedicineType);
                    addMedicine();
                }
                else addMedicine();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
            return true;
        }

        private bool ProcessAddMaterial(GridLookUpEdit cbosupplier, GridLookUpEdit cbobid)
        {
            try
            {
                ADO.MedicineTypeADO aMaterialType = null;

                decimal? BidSupplierAmount = 0, SupplierAmount = 0, BidAmount = 0;

                List<V_HIS_BID_MATERIAL_TYPE> BidMaterial = new List<V_HIS_BID_MATERIAL_TYPE>();


                if (this.ListBidMaterialType != null && ListBidMaterialType.Count > 0)
                {
                    BidMaterial = ListBidMaterialType.Where(o => o.MATERIAL_TYPE_ID == this.materialType.ID).ToList();
                    SupplierAmount = BidAmount = (BidMaterial != null && BidMaterial.Count > 0) ? BidMaterial.Sum(o => o.AMOUNT) + BidMaterial.Sum(o => o.ADJUST_AMOUNT) + BidMaterial.Sum(o => o.AMOUNT * o.IMP_MORE_RATIO) - (BidMaterial.Sum(o => o.IN_AMOUNT) != null ? BidMaterial.Sum(o => o.IN_AMOUNT) : 0) : 0;
                    if (cbosupplier.EditValue != null && cbobid.EditValue != null)
                    {
                        var MaterialOfSupplierAndBid = ListBidMaterialType.Where(o => o.SUPPLIER_ID == (long)cbosupplier.EditValue && o.BID_ID == (long)cbobid.EditValue && o.MATERIAL_TYPE_ID == this.materialType.ID).ToList();
                        BidSupplierAmount = (MaterialOfSupplierAndBid != null && MaterialOfSupplierAndBid.Count > 0) ? MaterialOfSupplierAndBid.Sum(o => o.AMOUNT) + MaterialOfSupplierAndBid.Sum(o => o.ADJUST_AMOUNT) + MaterialOfSupplierAndBid.Sum(o => o.AMOUNT * o.IMP_MORE_RATIO) - (MaterialOfSupplierAndBid.Sum(o => o.IN_AMOUNT) != null ? MaterialOfSupplierAndBid.Sum(o => o.IN_AMOUNT) : 0) : 0;
                    }
                    else if (cbosupplier.EditValue != null)
                    {
                        var BidMaterialOfSupplier = ListBidMaterialType.Where(o => o.SUPPLIER_ID == (long)cbosupplier.EditValue && o.MATERIAL_TYPE_ID == this.materialType.ID).ToList();
                        SupplierAmount = (BidMaterialOfSupplier != null && BidMaterialOfSupplier.Count > 0) ? BidMaterialOfSupplier.Sum(o => o.AMOUNT) + BidMaterialOfSupplier.Sum(o => o.ADJUST_AMOUNT) + BidMaterialOfSupplier.Sum(o => o.AMOUNT * o.IMP_MORE_RATIO) - (BidMaterialOfSupplier.Sum(o => o.IN_AMOUNT) != null ? BidMaterialOfSupplier.Sum(o => o.IN_AMOUNT) : 0) : 0;
                    }
                    if (cbobid.EditValue != null)
                    {
                        var BidMedicineOfBid = ListBidMaterialType.Where(o => o.BID_ID == (long)cbobid.EditValue && o.MATERIAL_TYPE_ID == this.materialType.ID).ToList();
                        BidAmount = (BidMedicineOfBid != null && BidMedicineOfBid.Count > 0) ? BidMedicineOfBid.Sum(o => o.AMOUNT) + BidMedicineOfBid.Sum(o => o.ADJUST_AMOUNT) + BidMedicineOfBid.Sum(o => o.AMOUNT * o.IMP_MORE_RATIO) - (BidMedicineOfBid.Sum(o => o.IN_AMOUNT) != null ? BidMedicineOfBid.Sum(o => o.IN_AMOUNT) : 0) : 0;
                    }
                }
                if (cbosupplier.EditValue != null)
                {
                    if (cbobid.EditValue != null)
                    {
                        if ((decimal)spinAmount.EditValue > BidSupplierAmount && isBusiness == false)
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show("Số lượng không được lớn hơn số lượng nhập còn lại theo nhà thầu và gói thầu của vật tư", "Thông báo");
                            return false;
                        }

                        aMaterialType = this.ListMedicineTypeAdoProcess.FirstOrDefault(o => o.ID == this.materialType.ID &&
                       o.Type == Base.GlobalConfig.VATTU &&
                       o.SUPPLIER_ID == (long)cbosupplier.EditValue &&
                       o.BID_ID == (long)cbobid.EditValue);
                    }
                    else
                    {
                        if ((decimal)spinAmount.EditValue > SupplierAmount && isBusiness == false)
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show("Số lượng không được lớn hơn số lượng nhập còn lại theo nhà thầu của vật tư", "Thông báo");
                            return false;
                        }

                        aMaterialType = this.ListMedicineTypeAdoProcess.FirstOrDefault(o => o.ID == this.materialType.ID &&
                        o.Type == Base.GlobalConfig.VATTU &&
                        o.SUPPLIER_ID == (long)cbosupplier.EditValue &&
                        o.BID_ID == null);
                    }

                }
                else
                {
                    if (cbobid.EditValue != null)
                    {
                        if ((decimal)spinAmount.EditValue > BidAmount && isBusiness == false)
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show("Số lượng không được lớn hơn số lượng nhập còn lại theo gói thầu của vật tư", "Thông báo");
                            return false;
                        }

                        aMaterialType = this.ListMedicineTypeAdoProcess.FirstOrDefault(o => o.ID == this.materialType.ID &&
                        o.Type == Base.GlobalConfig.VATTU &&
                        o.SUPPLIER_ID == null &&
                        o.BID_ID == (long)cbobid.EditValue);
                    }
                    else
                    {
                        if ((decimal)spinAmount.EditValue > SupplierAmount && isBusiness == false)
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show("Số lượng không được lớn hơn số lượng nhập còn lại của vật tư", "Thông báo");
                            return false;
                        }

                        aMaterialType = this.ListMedicineTypeAdoProcess.FirstOrDefault(o => o.ID == this.materialType.ID &&
                        o.Type == Base.GlobalConfig.VATTU &&
                        o.SUPPLIER_ID == null &&
                        o.BID_ID == null);
                    }
                }

                if (aMaterialType != null && aMaterialType.ID > 0)
                {
                    this.ListMedicineTypeAdoProcess.RemoveAll(o => o == aMaterialType);
                    addMaterial();
                }
                else addMaterial();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
            return true;
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                positionHandleLeft = -1;
                if (lciBtnUpdate.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Never) return;
                if (!dxValidationProviderLeftPanel.Validate())
                    return;

                btnUpdate.Focus();
                if (this.editMedicineTypeAdo != null)
                {
                    this.ListMedicineTypeAdoProcess.RemoveAll(o => o == this.editMedicineTypeAdo);
                    if (this.editMedicineTypeAdo.Type == Base.GlobalConfig.THUOC)
                    {
                        if (chkItemType.Checked)
                        {
                            if (!ProcessUpdateMedicine(this.cboSupplier, this.cboBid))
                                return;
                        }
                        else
                        {
                            if (!ProcessUpdateMedicine(this.cboSupplier1, this.cboBid1))
                                return;
                        }
                    }
                    else if (this.editMedicineTypeAdo.Type == Base.GlobalConfig.VATTU)
                    {
                        if (chkItemType.Checked)
                        {
                            if (!ProcessUpdateMaterial(this.cboSupplier, this.cboBid))
                                return;
                        }
                        else
                        {
                            if (!ProcessUpdateMaterial(this.cboSupplier1, this.cboBid1))
                                return;
                        }
                    }
                    else if (this.editMedicineTypeAdo.Type == Base.GlobalConfig.MAU) addBlood();
                }
                this.ActionType = GlobalVariables.ActionAdd;
                VisibleButton(this.ActionType);
                gridControlProcess.BeginUpdate();
                gridControlProcess.DataSource = null;
                gridControlProcess.DataSource = this.ListMedicineTypeAdoProcess;
                gridControlProcess.EndUpdate();

                txtSupplierCode.Text = "";
                cboSupplier.EditValue = null;
                cboSupplier.Properties.Buttons[1].Visible = false;
                spinAmount.Value = 0;
                spinImpPrice.Value = 0;
                EnableButton(this.ActionType);
                VisibleButton(this.ActionType);
                //FocusTab();
                showBlood = true;
                showMaterial = true;
                this.editMedicineTypeAdo = null;
                spinAmount.Value = 0;
                spinImpPrice.Value = 0;
                cboBid.EditValue = null;
                cboBid.Properties.Buttons[1].Visible = false;
                btnSave.Enabled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool ProcessUpdateMedicine(GridLookUpEdit cbosupplier, GridLookUpEdit cbobid)
        {
            try
            {
                decimal? BidSupplierAmount = 0, SupplierAmount = 0, BidAmount = 0;
                if (this.ListBidMedicineType != null)
                {
                    if (cbosupplier.EditValue != null && cbobid.EditValue != null)
                    {
                        var MedicineOfSupplierAndBid = ListBidMedicineType.Where(o => o.SUPPLIER_ID == (long)cbosupplier.EditValue && o.BID_ID == (long)cbobid.EditValue && o.MEDICINE_TYPE_ID == this.medicineType.ID).ToList();
                        BidSupplierAmount = (MedicineOfSupplierAndBid != null && MedicineOfSupplierAndBid.Count > 0) ? MedicineOfSupplierAndBid.Sum(o => o.AMOUNT) + MedicineOfSupplierAndBid.Sum(o => o.ADJUST_AMOUNT) + MedicineOfSupplierAndBid.Sum(o => o.AMOUNT * o.IMP_MORE_RATIO) - (MedicineOfSupplierAndBid.Sum(o => o.IN_AMOUNT) != null ? MedicineOfSupplierAndBid.Sum(o => o.IN_AMOUNT) : 0) : 0;
                    }
                    else if (cbosupplier.EditValue != null)
                    {
                        var BidMedicineOfSupplier = ListBidMedicineType.Where(o => o.SUPPLIER_ID == (long)cbosupplier.EditValue && o.MEDICINE_TYPE_ID == this.medicineType.ID).ToList();
                        SupplierAmount = (BidMedicineOfSupplier != null && BidMedicineOfSupplier.Count > 0) ? BidMedicineOfSupplier.Sum(o => o.AMOUNT) + BidMedicineOfSupplier.Sum(o => o.ADJUST_AMOUNT) + BidMedicineOfSupplier.Sum(o => o.AMOUNT * o.IMP_MORE_RATIO) - (BidMedicineOfSupplier.Sum(o => o.IN_AMOUNT) != null ? BidMedicineOfSupplier.Sum(o => o.IN_AMOUNT) : 0) : 0;
                    }
                    else if (cbobid.EditValue != null)
                    {
                        var BidMedicineOfBid = ListBidMedicineType.Where(o => o.BID_ID == (long)cbobid.EditValue && o.MEDICINE_TYPE_ID == this.medicineType.ID).ToList();
                        BidAmount = (BidMedicineOfBid != null && BidMedicineOfBid.Count > 0) ? BidMedicineOfBid.Sum(o => o.AMOUNT) + BidMedicineOfBid.Sum(o => o.ADJUST_AMOUNT) + BidMedicineOfBid.Sum(o => o.AMOUNT * o.IMP_MORE_RATIO) - (BidMedicineOfBid.Sum(o => o.IN_AMOUNT) != null ? BidMedicineOfBid.Sum(o => o.IN_AMOUNT) : 0) : 0;
                    }
                }

                if (cbosupplier.EditValue != null)
                {
                    if (cbobid.EditValue != null)
                    {
                        if (spinAmount.Value > BidSupplierAmount && isBusiness == false)
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show("Số lượng không được lớn hơn số lượng nhập còn lại theo nhà thầu và gói thầu của thuốc", "Thông báo");
                            return false;
                        }

                    }
                    else
                    {
                        if (spinAmount.Value > SupplierAmount && isBusiness == false)
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show("Số lượng không được lớn hơn số lượng nhập còn lại theo nhà thầu của thuốc", "Thông báo");
                            return false;
                        }
                    }
                }
                else
                {
                    if (cbobid.EditValue != null)
                    {
                        if (spinAmount.Value > BidAmount && isBusiness == false)
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show("Số lượng không được lớn hơn số lượng nhập còn lại theo gói thầu của thuốc", "Thông báo");
                            return false;
                        }
                    }
                    else
                    {
                        if (this.editMedicineTypeAdo != null && this.editMedicineTypeAdo.AllowAmount != null && spinAmount.Value > this.editMedicineTypeAdo.AllowAmount && isBusiness == false)
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show("Số lượng không được lớn hơn số lượng nhập còn lại của thuốc", "Thông báo");
                            return false;
                        }

                    }
                }
                addMedicine();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
            return true;
        }

        private bool ProcessUpdateMaterial(GridLookUpEdit cbosupplier, GridLookUpEdit cbobid)
        {
            try
            {
                decimal? BidSupplierAmount = 0, SupplierAmount = 0, BidAmount = 0;
                if (this.ListBidMaterialType != null)
                {
                    if (cbosupplier.EditValue != null && cbobid.EditValue != null)
                    {
                        var MedicineOfSupplierAndBid = ListBidMedicineType.Where(o => o.SUPPLIER_ID == (long)cbosupplier.EditValue && o.BID_ID == (long)cbobid.EditValue && o.MEDICINE_TYPE_ID == this.medicineType.ID).ToList();
                        BidSupplierAmount = (MedicineOfSupplierAndBid != null && MedicineOfSupplierAndBid.Count > 0) ? MedicineOfSupplierAndBid.Sum(o => o.AMOUNT) + MedicineOfSupplierAndBid.Sum(o => o.ADJUST_AMOUNT) + MedicineOfSupplierAndBid.Sum(o => o.AMOUNT * o.IMP_MORE_RATIO) - (MedicineOfSupplierAndBid.Sum(o => o.IN_AMOUNT) != null ? MedicineOfSupplierAndBid.Sum(o => o.IN_AMOUNT) : 0) : 0;
                    }
                    else if (cbosupplier.EditValue != null)
                    {
                        var BidMedicineOfSupplier = ListBidMaterialType.Where(o => o.SUPPLIER_ID == (long)cbosupplier.EditValue && o.MATERIAL_TYPE_ID == this.materialType.ID).ToList();
                        SupplierAmount = (BidMedicineOfSupplier != null && BidMedicineOfSupplier.Count > 0) ? BidMedicineOfSupplier.Sum(o => o.AMOUNT) + BidMedicineOfSupplier.Sum(o => o.ADJUST_AMOUNT) + BidMedicineOfSupplier.Sum(o => o.AMOUNT * o.IMP_MORE_RATIO) - (BidMedicineOfSupplier.Sum(o => o.IN_AMOUNT) != null ? BidMedicineOfSupplier.Sum(o => o.IN_AMOUNT) : 0) : 0;
                    }
                    else if (cbobid.EditValue != null)
                    {
                        var BidMedicineOfBid = ListBidMaterialType.Where(o => o.BID_ID == (long)cbobid.EditValue && o.MATERIAL_TYPE_ID == this.materialType.ID).ToList();
                        BidAmount = (BidMedicineOfBid != null && BidMedicineOfBid.Count > 0) ? BidMedicineOfBid.Sum(o => o.AMOUNT) + BidMedicineOfBid.Sum(o => o.ADJUST_AMOUNT) + BidMedicineOfBid.Sum(o => o.AMOUNT * o.IMP_MORE_RATIO) - (BidMedicineOfBid.Sum(o => o.IN_AMOUNT) != null ? BidMedicineOfBid.Sum(o => o.IN_AMOUNT) : 0) : 0;
                    }
                }

                if (cbosupplier.EditValue != null)
                {
                    if (cbobid.EditValue != null)
                    {
                        if (spinAmount.Value > BidSupplierAmount && isBusiness == false)
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show("Số lượng không được lớn hơn số lượng nhập còn lại theo nhà thầu và gói thầu của vật tư", "Thông báo");
                            return false;
                        }
                    }
                    else
                    {
                        if (spinAmount.Value > SupplierAmount && isBusiness == false)
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show("Số lượng không được lớn hơn số lượng nhập còn lại theo nhà thầu của vật tư", "Thông báo");
                            return false;
                        }
                    }
                }
                else
                {
                    if (cbobid.EditValue != null)
                    {
                        if (spinAmount.Value > BidAmount && isBusiness == false)
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show("Số lượng không được lớn hơn số lượng nhập còn lại theo gói thầu của vật tư", "Thông báo");
                            return false;
                        }
                    }
                    else
                    {
                        if (this.editMedicineTypeAdo != null && this.editMedicineTypeAdo.AllowAmount != null && spinAmount.Value > this.editMedicineTypeAdo.AllowAmount && isBusiness == false)
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show("Số lượng không được lớn hơn số lượng nhập còn lại của vật tư", "Thông báo");
                            return false;
                        }
                    }
                }
                addMaterial();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
            return true;
        }
        private void btnDiscard_Click(object sender, EventArgs e)
        {
            try
            {
                if (lciBtnDiscard.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Never) return;
                SetDefaultValueControlLeft();
                this.ActionType = GlobalVariables.ActionAdd;
                VisibleButton(this.ActionType);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            gridViewProcess.PostEditor();
            CommonParam paramCommon = new CommonParam();
            bool success = false;
            try
            {
                if (!btnSave.Enabled && this.ActionType == GlobalVariables.ActionView) return;

                //if (this.ActionType == GlobalVariables.ActionAdd && CheckValidDataInGridService(ref paramCommon, ListMedicineTypeAdoProcess))
                if (this.ActionType == GlobalVariables.ActionAdd)
                {

                    getDataForProcess();
                    if (this.anticipateModel == null ||
                        this.anticipateModel.HIS_ANTICIPATE_MATY == null ||
                        this.anticipateModel.HIS_ANTICIPATE_METY == null ||
                        this.anticipateModel.HIS_ANTICIPATE_BLTY == null ||
                        (this.anticipateModel.HIS_ANTICIPATE_MATY.Count <= 0 &&
                        this.anticipateModel.HIS_ANTICIPATE_METY.Count <= 0 &&
                        this.anticipateModel.HIS_ANTICIPATE_BLTY.Count <= 0))
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceMessage.ThieuTruongDuLieuBatBuoc);
                        return;
                    }
                    WaitingManager.Show();
                    if (this.currentAnticipate == null)
                    {
                        anticipatePrint = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Post<MOS.EFMODEL.DataModels.HIS_ANTICIPATE>(ApiConsumer.HisRequestUriStore.HIS_ANTICIPATE_CREATE, ApiConsumer.ApiConsumers.MosConsumer, anticipateModel, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                    }
                    else
                    {
                        anticipatePrint = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Post<MOS.EFMODEL.DataModels.HIS_ANTICIPATE>("api/HisAnticipate/Update", ApiConsumer.ApiConsumers.MosConsumer, anticipateModel, paramCommon);
                    }
                    if (anticipatePrint != null)
                    {
                        success = true;
                        if (this.delegateRefresh != null)
                        {
                            this.delegateRefresh();
                        }
                        this.ActionType = GlobalVariables.ActionView;
                        EnableButton(this.ActionType);
                        txtSupplierCode.Text = "";
                        cboSupplier.EditValue = null;
                        spinAmount.Value = 0;
                        spinImpPrice.Value = 0;
                    }
                    WaitingManager.Hide();
                }
                if (success)
                {
                    BackendDataWorker.Reset<HIS_ANTICIPATE>();
                }
                #region Show message
                Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, paramCommon, success);
                #endregion

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnNew.Enabled) return;
                SetDefaultValueControl();
                EnableButton(this.ActionType);
                VisibleButton(this.ActionType);
                FillDataToGridMedicineType();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void setValueAfterAdd()
        {
            try
            {
                spinAmount.Value = 0;
                spinImpPrice.Value = 0;
                //cboSupplier.EditValue = null;
                cboBid.EditValue = null;
                //FocusTab();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void getDataForProcess()
        {
            try
            {
                this.anticipateModel = new MOS.EFMODEL.DataModels.HIS_ANTICIPATE();
                if (this.currentAnticipate != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_ANTICIPATE>(this.anticipateModel, this.currentAnticipate);
                }
                this.anticipateModel.HIS_ANTICIPATE_MATY = new List<MOS.EFMODEL.DataModels.HIS_ANTICIPATE_MATY>();
                this.anticipateModel.HIS_ANTICIPATE_METY = new List<MOS.EFMODEL.DataModels.HIS_ANTICIPATE_METY>();
                this.anticipateModel.HIS_ANTICIPATE_BLTY = new List<MOS.EFMODEL.DataModels.HIS_ANTICIPATE_BLTY>();
                foreach (var item in this.ListMedicineTypeAdoProcess)
                {
                    if (!item.IsNotSave)
                    {
                        if (item.Type == Base.GlobalConfig.THUOC)
                        {
                            var anticipateMety = new MOS.EFMODEL.DataModels.HIS_ANTICIPATE_METY();
                            anticipateMety.AMOUNT = (decimal)(item.AMOUNT ?? 0);
                            anticipateMety.IMP_PRICE = (decimal)(item.IMP_PRICE ?? 0);
                            anticipateMety.MEDICINE_TYPE_ID = item.ID;
                            anticipateMety.SUPPLIER_ID = item.SUPPLIER_ID;
                            anticipateMety.BID_ID = item.BID_ID;
                            this.anticipateModel.HIS_ANTICIPATE_METY.Add(anticipateMety);

                        }
                        else if (item.Type == Base.GlobalConfig.VATTU)
                        {
                            var anticipateMaty = new MOS.EFMODEL.DataModels.HIS_ANTICIPATE_MATY();
                            anticipateMaty.AMOUNT = (decimal)(item.AMOUNT ?? 0);
                            anticipateMaty.IMP_PRICE = (decimal)(item.IMP_PRICE ?? 0);
                            anticipateMaty.MATERIAL_TYPE_ID = item.ID;
                            anticipateMaty.SUPPLIER_ID = item.SUPPLIER_ID;
                            anticipateMaty.BID_ID = item.BID_ID;
                            this.anticipateModel.HIS_ANTICIPATE_MATY.Add(anticipateMaty);
                        }
                        else if (item.Type == Base.GlobalConfig.MAU)
                        {
                            var anticipateBlty = new MOS.EFMODEL.DataModels.HIS_ANTICIPATE_BLTY();
                            anticipateBlty.AMOUNT = (decimal)(item.AMOUNT ?? 0);
                            anticipateBlty.IMP_PRICE = (decimal)(item.IMP_PRICE ?? 0);
                            anticipateBlty.BLOOD_TYPE_ID = item.ID;
                            anticipateBlty.SUPPLIER_ID = item.SUPPLIER_ID;
                            anticipateBlty.BID_ID = item.BID_ID;
                            this.anticipateModel.HIS_ANTICIPATE_BLTY.Add(anticipateBlty);
                        }
                    }
                }
                if (cboReceiveMediStock.EditValue != null)
                {
                    this.anticipateModel.RECEIVE_MEDI_STOCK_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboReceiveMediStock.EditValue.ToString());
                }
                else
                {
                    this.anticipateModel.RECEIVE_MEDI_STOCK_ID = null;
                }
                this.anticipateModel.DESCRIPTION = txtDescription.Text;
                this.anticipateModel.USE_TIME = txtUseTime.Text;
                this.anticipateModel.REQUEST_ROOM_ID = RoomId;
                this.anticipateModel.REQUEST_DEPARTMENT_ID = DepartmentId;
                this.anticipateModel.REQUEST_LOGINNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                this.anticipateModel.REQUEST_USERNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool CheckValidDataInGridService(ref CommonParam param, List<ADO.MedicineTypeADO> MedicineCheckeds__Send)
        {
            bool valid = true;
            try
            {
                if (MedicineCheckeds__Send != null && MedicineCheckeds__Send.Count > 0)
                {
                    //if (this.currentAnticipate != null)// && this.currentAnticipate.BID_ID != null)
                    //{
                    string error = "";
                    var amount = MedicineCheckeds__Send.Where(o => o.isAmount && !o.IsNotSave).ToList();
                    var delete = MedicineCheckeds__Send.Where(o => o.isDelete && !o.IsNotSave).ToList();
                    if (amount != null && amount.Count > 0)
                    {
                        string messageErr = "";
                        foreach (var item in amount)
                        {
                            if (item.Type == Base.GlobalConfig.THUOC)
                            {
                                messageErr = String.Format(Resources.ResourceMessage.CanhBaoThuoc, item.MEDICINE_TYPE_NAME);
                            }
                            else if (item.Type == Base.GlobalConfig.VATTU)
                            {
                                messageErr = String.Format(Resources.ResourceMessage.CanhBaoVatTu, item.MEDICINE_TYPE_NAME);
                            }
                            else if (item.Type == Base.GlobalConfig.MAU)
                            {
                                messageErr = String.Format(Resources.ResourceMessage.CanhBaoMau, item.MEDICINE_TYPE_NAME);
                            }

                            messageErr += "không được lớn hơn số lượng còn lại";
                            error += messageErr + "; ";
                        }

                    }
                    if (delete != null && delete.Count > 0)
                    {
                        string messageErr = "";
                        foreach (var item in delete)
                        {
                            if (item.Type == Base.GlobalConfig.THUOC)
                            {
                                messageErr = String.Format(Resources.ResourceMessage.CanhBaoThuoc, item.MEDICINE_TYPE_NAME);
                            }
                            else if (item.Type == Base.GlobalConfig.VATTU)
                            {
                                messageErr = String.Format(Resources.ResourceMessage.CanhBaoVatTu, item.MEDICINE_TYPE_NAME);
                            }
                            else if (item.Type == Base.GlobalConfig.MAU)
                            {
                                messageErr = String.Format(Resources.ResourceMessage.CanhBaoMau, item.MEDICINE_TYPE_NAME);
                            }

                            messageErr += "không có trong gói thầu";
                            error += messageErr + "; ";
                        }
                    }

                    if (!string.IsNullOrEmpty(error))
                    {
                        param.Messages.Add(error + ";");
                    }
                    //}

                    foreach (var item in MedicineCheckeds__Send)
                    {
                        if (!item.IsNotSave)
                        {
                            string messageErr = "";
                            bool result = true;
                            if (item.Type == Base.GlobalConfig.THUOC)
                            {
                                messageErr = String.Format(Resources.ResourceMessage.CanhBaoThuoc, item.MEDICINE_TYPE_NAME);
                            }
                            else if (item.Type == Base.GlobalConfig.VATTU)
                            {
                                messageErr = String.Format(Resources.ResourceMessage.CanhBaoVatTu, item.MEDICINE_TYPE_NAME);
                            }
                            else if (item.Type == Base.GlobalConfig.MAU)
                            {
                                messageErr = String.Format(Resources.ResourceMessage.CanhBaoMau, item.MEDICINE_TYPE_NAME);
                            }

                            if (item.AMOUNT <= 0)
                            {
                                result = false;
                                messageErr += Resources.ResourceMessage.SoLuongKhongDuocAm;
                            }

                            var listItem = MedicineCheckeds__Send.Where(o => o.MEDICINE_TYPE_CODE == item.MEDICINE_TYPE_CODE).ToList();
                            if (listItem != null && listItem.Count > 1)
                            {
                                foreach (var i in listItem)
                                {
                                    if (i.SUPPLIER_ID == item.SUPPLIER_ID && i.IdRow != item.IdRow)
                                    {
                                        result = false;
                                        messageErr += Resources.ResourceMessage.BiTrung;
                                        break;
                                    }
                                }
                            }

                            if (!result)
                            {
                                param.Messages.Add(messageErr + ";");
                            }
                        }
                    }
                }
                else
                {
                    param.Messages.Add(Resources.ResourceMessage.ThieuTruongDuLieuBatBuoc);
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
        #endregion

        #region add
        private void addMedicine()
        {
            try
            {
                ADO.MedicineTypeADO ado = new ADO.MedicineTypeADO(this.medicineType);

                ado.IdRow = setIdRow(this.ListMedicineTypeAdoProcess);

                //this.medicineType.IdRow = setIdRow(this.ListMedicineTypeAdoProcess);

                ado.AMOUNT = spinAmount.Value;
                ado.IMP_PRICE = spinImpPrice.Value;
                ado.Type = Base.GlobalConfig.THUOC;
                if (chkItemType.Checked)
                {
                    if (cboSupplier.EditValue != null)
                    {
                        long supplierId = Inventec.Common.TypeConvert.Parse.ToInt32(cboSupplier.EditValue.ToString());
                        var supplier = this.ListSupplier.FirstOrDefault(o => o.ID == supplierId);
                        if (supplier != null)
                        {
                            ado.SUPPLIER_ID = supplier.ID;
                            ado.SUPPLIER_NAME = supplier.SUPPLIER_NAME;
                        }

                        var BidMedicineTypes = ListBidMedicineType.Where(o => o.SUPPLIER_ID == supplierId).ToList();
                        ado.AllowAmount = BidMedicineTypes.Count > 0 ? BidMedicineTypes.Sum(o => o.AMOUNT) - BidMedicineTypes.Sum(o => o.IN_AMOUNT) + BidMedicineTypes.Sum(o => o.ADJUST_AMOUNT) + BidMedicineTypes.Sum(o => o.AMOUNT * o.IMP_MORE_RATIO) : 0;
                    }
                    else
                    {
                        ado.SUPPLIER_ID = null;
                        ado.SUPPLIER_NAME = null;
                    }

                    if (cboBid.EditValue != null)
                    {
                        var bid = this.ListBid.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt32(cboBid.EditValue.ToString()));
                        if (bid != null)
                        {
                            ado.BID_ID = bid.ID;
                            ado.BID_NUMBER = bid.BID_NUMBER;
                            ado.BID_YEAR = bid.BID_YEAR;
                        }

                        var BidMedicineTypes = ListBidMedicineType.Where(o => o.BID_ID == Inventec.Common.TypeConvert.Parse.ToInt32(cboBid.EditValue.ToString())).ToList();
                        ado.AllowAmount = BidMedicineTypes.Count > 0 ? BidMedicineTypes.Sum(o => o.AMOUNT) - BidMedicineTypes.Sum(o => o.IN_AMOUNT) + BidMedicineTypes.Sum(o => o.ADJUST_AMOUNT) + BidMedicineTypes.Sum(o => o.AMOUNT * o.IMP_MORE_RATIO) : 0;
                    }
                    else
                    {
                        ado.BID_ID = null;
                        ado.BID_NUMBER = null;
                        ado.BID_YEAR = null;
                    }
                }
                else
                {
                    if (cboSupplier1.EditValue != null)
                    {
                        long supplierId = Inventec.Common.TypeConvert.Parse.ToInt32(cboSupplier1.EditValue.ToString());
                        var supplier = this.ListSupplier.FirstOrDefault(o => o.ID == supplierId);
                        if (supplier != null)
                        {
                            ado.SUPPLIER_ID = supplier.ID;
                            ado.SUPPLIER_NAME = supplier.SUPPLIER_NAME;
                        }

                        var BidMedicineTypes = ListBidMedicineType.Where(o => o.SUPPLIER_ID == supplierId).ToList();
                        ado.AllowAmount = BidMedicineTypes.Count > 0 ? BidMedicineTypes.Sum(o => o.AMOUNT) - BidMedicineTypes.Sum(o => o.IN_AMOUNT) + BidMedicineTypes.Sum(o => o.ADJUST_AMOUNT) + BidMedicineTypes.Sum(o => o.AMOUNT * o.IMP_MORE_RATIO) : 0;
                    }
                    else
                    {
                        ado.SUPPLIER_ID = null;
                        ado.SUPPLIER_NAME = null;
                    }

                    if (cboBid1.EditValue != null)
                    {
                        var bid = this.ListBid.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt32(cboBid1.EditValue.ToString()));
                        if (bid != null)
                        {
                            ado.BID_ID = bid.ID;
                            ado.BID_NUMBER = bid.BID_NUMBER;
                            ado.BID_YEAR = bid.BID_YEAR;
                        }

                        var BidMedicineTypes = ListBidMedicineType.Where(o => o.BID_ID == Inventec.Common.TypeConvert.Parse.ToInt32(cboBid1.EditValue.ToString())).ToList();
                        ado.AllowAmount = BidMedicineTypes.Count > 0 ? BidMedicineTypes.Sum(o => o.AMOUNT) - BidMedicineTypes.Sum(o => o.IN_AMOUNT) + BidMedicineTypes.Sum(o => o.ADJUST_AMOUNT) + BidMedicineTypes.Sum(o => o.AMOUNT * o.IMP_MORE_RATIO) : 0;
                    }
                    else
                    {
                        ado.BID_ID = null;
                        ado.BID_NUMBER = null;
                        ado.BID_YEAR = null;
                    }
                }

                ado.IsNotSave = false;
                //this.ListMedicineTypeAdoProcess.Add(this.medicineType);

                ListMedicineTypeAdoProcess.Add(ado);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void addMedicine(List<V_HIS_BID_MEDICINE_TYPE> listBidData)
        {
            try
            {
                if (this.listAticipateMetys != null && this.listAticipateMetys.Count > 0)
                {
                    foreach (var item in this.listAticipateMetys)
                    {
                        var checkBid = listBidData.FirstOrDefault(o => o.MEDICINE_TYPE_ID == item.MEDICINE_TYPE_ID);
                        ADO.MedicineTypeADO medicineADO = new ADO.MedicineTypeADO();
                        var medicineAdd = Base.GlobalConfig.HisMedicineTypes.FirstOrDefault(o => o.ID == item.MEDICINE_TYPE_ID);
                        if (medicineAdd != null)
                        {
                            Inventec.Common.Mapper.DataObjectMapper.Map<ADO.MedicineTypeADO>(medicineADO, medicineAdd);
                        }
                        medicineADO.Type = Base.GlobalConfig.THUOC;
                        medicineADO.IdRow = setIdRow(this.ListMedicineTypeAdoProcess);
                        medicineADO.AMOUNT = item.AMOUNT;
                        medicineADO.IMP_PRICE = item.IMP_PRICE;
                        medicineADO.SUPPLIER_ID = item.SUPPLIER_ID;
                        medicineADO.SUPPLIER_NAME = item.SUPPLIER_NAME;
                        medicineADO.BID_ID = item.BID_ID;
                        if (checkBid != null)
                        {
                            medicineADO.AllowAmount = checkBid.AMOUNT - (checkBid.IN_AMOUNT != null ? checkBid.IN_AMOUNT : 0) + (checkBid.ADJUST_AMOUNT ?? 0) + (checkBid.AMOUNT * (checkBid.IMP_MORE_RATIO ?? 0));
                            medicineADO.isDelete = false;
                        }
                        else
                        {
                            medicineADO.AllowAmount = null;
                            medicineADO.isDelete = true;
                        }

                        //if (medicineADO.AllowAmount != null)
                        //{
                        //    medicineADO.isAmount = item.AMOUNT > medicineADO.AllowAmount;
                        //}

                        this.ListMedicineTypeAdoProcess.Add(medicineADO);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void addMedicine(V_HIS_ANTICIPATE_METY anticipateMetyData, V_HIS_MEDICINE_TYPE medicineData)
        {
            try
            {
                ADO.MedicineTypeADO medicineADO = new ADO.MedicineTypeADO();
                Inventec.Common.Mapper.DataObjectMapper.Map<ADO.MedicineTypeADO>(medicineADO, medicineData);
                medicineADO.Type = Base.GlobalConfig.THUOC;
                medicineADO.IdRow = setIdRow(this.ListMedicineTypeAdoProcess);
                medicineADO.AMOUNT = anticipateMetyData.AMOUNT;
                medicineADO.IMP_PRICE = anticipateMetyData.IMP_PRICE;
                medicineADO.SUPPLIER_ID = anticipateMetyData.SUPPLIER_ID;
                medicineADO.SUPPLIER_NAME = anticipateMetyData.SUPPLIER_NAME;
                medicineADO.BID_ID = anticipateMetyData.BID_ID;
                medicineADO.AllowAmount = null;
                this.ListMedicineTypeAdoProcess.Add(medicineADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void addMaterial()
        {
            try
            {
                this.materialType.AMOUNT = spinAmount.Value;
                this.materialType.IMP_PRICE = spinImpPrice.Value;
                ADO.MedicineTypeADO aMedicineAdo = new ADO.MedicineTypeADO();
                //AutoMapper.Mapper.CreateMap<ADO.MaterialTypeADO, ADO.MedicineTypeADO>();
                //aMedicineAdo = AutoMapper.Mapper.Map<ADO.MaterialTypeADO, ADO.MedicineTypeADO>(this.materialType);

                Inventec.Common.Mapper.DataObjectMapper.Map<ADO.MedicineTypeADO>(aMedicineAdo, this.materialType);

                aMedicineAdo.MEDICINE_TYPE_CODE = this.materialType.MATERIAL_TYPE_CODE;
                aMedicineAdo.MEDICINE_TYPE_NAME = this.materialType.MATERIAL_TYPE_NAME;
                aMedicineAdo.Type = Base.GlobalConfig.VATTU;
                aMedicineAdo.IdRow = setIdRow(this.ListMedicineTypeAdoProcess);
                if (chkItemType.Checked)
                {
                    if (cboSupplier.EditValue != null)
                    {
                        aMedicineAdo.SUPPLIER_ID = Inventec.Common.TypeConvert.Parse.ToInt32(cboSupplier.EditValue.ToString());
                        var supplier = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SUPPLIER>().FirstOrDefault(o => o.ID == aMedicineAdo.SUPPLIER_ID);
                        aMedicineAdo.SUPPLIER_NAME = supplier.SUPPLIER_NAME;

                        var BidMaterialTypes = ListBidMaterialType.Where(o => o.SUPPLIER_ID == aMedicineAdo.SUPPLIER_ID).ToList();
                        aMedicineAdo.AllowAmount = BidMaterialTypes.Count > 0 ? BidMaterialTypes.Sum(o => o.AMOUNT) - BidMaterialTypes.Sum(o => o.IN_AMOUNT) + BidMaterialTypes.Sum(o => o.ADJUST_AMOUNT) + BidMaterialTypes.Sum(o => o.AMOUNT * o.IMP_MORE_RATIO) : 0;
                    }
                    else
                    {
                        aMedicineAdo.SUPPLIER_ID = null;
                        aMedicineAdo.SUPPLIER_NAME = null;
                    }

                    if (cboBid.EditValue != null)
                    {
                        var bid = this.ListBid.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt32(cboBid.EditValue.ToString()));
                        if (bid != null)
                        {
                            aMedicineAdo.BID_ID = bid.ID;
                            aMedicineAdo.BID_NUMBER = bid.BID_NUMBER;
                            aMedicineAdo.BID_YEAR = bid.BID_YEAR;
                        }
                        var BidMaterialTypes = ListBidMaterialType.Where(o => o.BID_ID == Inventec.Common.TypeConvert.Parse.ToInt32(cboBid.EditValue.ToString())).ToList();
                        aMedicineAdo.AllowAmount = BidMaterialTypes.Count > 0 ? BidMaterialTypes.Sum(o => o.AMOUNT) - BidMaterialTypes.Sum(o => o.IN_AMOUNT) + BidMaterialTypes.Sum(o => o.ADJUST_AMOUNT) + BidMaterialTypes.Sum(o => o.AMOUNT * o.IMP_MORE_RATIO) : 0;
                    }
                    else
                    {
                        aMedicineAdo.BID_ID = null;
                        aMedicineAdo.BID_NUMBER = null;
                        aMedicineAdo.BID_YEAR = null;
                    }
                }
                else
                {
                    if (cboSupplier1.EditValue != null)
                    {
                        aMedicineAdo.SUPPLIER_ID = Inventec.Common.TypeConvert.Parse.ToInt32(cboSupplier1.EditValue.ToString());
                        var supplier = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SUPPLIER>().FirstOrDefault(o => o.ID == aMedicineAdo.SUPPLIER_ID);
                        aMedicineAdo.SUPPLIER_NAME = supplier.SUPPLIER_NAME;

                        var BidMaterialTypes = ListBidMaterialType.Where(o => o.SUPPLIER_ID == aMedicineAdo.SUPPLIER_ID).ToList();
                        aMedicineAdo.AllowAmount = BidMaterialTypes.Count > 0 ? BidMaterialTypes.Sum(o => o.AMOUNT) - BidMaterialTypes.Sum(o => o.IN_AMOUNT) + BidMaterialTypes.Sum(o => o.ADJUST_AMOUNT) + BidMaterialTypes.Sum(o => o.AMOUNT * o.IMP_MORE_RATIO) : 0;
                    }
                    else
                    {
                        aMedicineAdo.SUPPLIER_ID = null;
                        aMedicineAdo.SUPPLIER_NAME = null;
                    }

                    if (cboBid1.EditValue != null)
                    {
                        var bid = this.ListBid.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt32(cboBid1.EditValue.ToString()));
                        if (bid != null)
                        {
                            aMedicineAdo.BID_ID = bid.ID;
                            aMedicineAdo.BID_NUMBER = bid.BID_NUMBER;
                            aMedicineAdo.BID_YEAR = bid.BID_YEAR;
                        }
                        var BidMaterialTypes = ListBidMaterialType.Where(o => o.BID_ID == Inventec.Common.TypeConvert.Parse.ToInt32(cboBid1.EditValue.ToString())).ToList();
                        aMedicineAdo.AllowAmount = BidMaterialTypes.Count > 0 ? BidMaterialTypes.Sum(o => o.AMOUNT) - BidMaterialTypes.Sum(o => o.IN_AMOUNT) + BidMaterialTypes.Sum(o => o.ADJUST_AMOUNT) + BidMaterialTypes.Sum(o => o.AMOUNT * o.IMP_MORE_RATIO) : 0;
                    }
                    else
                    {
                        aMedicineAdo.BID_ID = null;
                        aMedicineAdo.BID_NUMBER = null;
                        aMedicineAdo.BID_YEAR = null;
                    }
                }

                aMedicineAdo.IsNotSave = false;
                this.ListMedicineTypeAdoProcess.Add(aMedicineAdo);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void addMaterial(List<V_HIS_BID_MATERIAL_TYPE> listBidData)
        {
            try
            {
                if (this.listAticipateMatys != null && this.listAticipateMatys.Count > 0)
                {
                    foreach (var item in this.listAticipateMatys)
                    {
                        var checkBid = listBidData.FirstOrDefault(o => o.MATERIAL_TYPE_ID == item.MATERIAL_TYPE_ID);
                        var materialAdd = Base.GlobalConfig.HisMaterialTypes.FirstOrDefault(o => o.ID == item.MATERIAL_TYPE_ID);

                        ADO.MedicineTypeADO materialADO = new ADO.MedicineTypeADO();
                        if (materialAdd != null)
                        {
                            Inventec.Common.Mapper.DataObjectMapper.Map<ADO.MedicineTypeADO>(materialADO, materialAdd);
                        }
                        materialADO.Type = Base.GlobalConfig.VATTU;
                        materialADO.IdRow = setIdRow(this.ListMedicineTypeAdoProcess);
                        materialADO.AMOUNT = item.AMOUNT;
                        if (checkBid != null)
                        {
                            materialADO.AllowAmount = checkBid.AMOUNT - (checkBid.IN_AMOUNT != null ? checkBid.IN_AMOUNT : 0) + (checkBid.ADJUST_AMOUNT ?? 0) + (checkBid.AMOUNT * (checkBid.IMP_MORE_RATIO ?? 0));
                            materialADO.isDelete = false;
                        }
                        else
                        {
                            materialADO.AllowAmount = null;
                            materialADO.isDelete = true;
                        }
                        materialADO.IMP_PRICE = item.IMP_PRICE;
                        materialADO.SUPPLIER_ID = item.SUPPLIER_ID;
                        materialADO.SUPPLIER_NAME = item.SUPPLIER_NAME;
                        materialADO.MEDICINE_TYPE_CODE = item.MATERIAL_TYPE_CODE;
                        materialADO.MEDICINE_TYPE_NAME = item.MATERIAL_TYPE_NAME;
                        materialADO.BID_ID = item.BID_ID;

                        //if (materialADO.AllowAmount != null)
                        //{
                        //    materialADO.isAmount = item.AMOUNT > materialADO.AllowAmount;
                        //}

                        this.ListMedicineTypeAdoProcess.Add(materialADO);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void addMaterial(V_HIS_ANTICIPATE_MATY anticipateMatyData, V_HIS_MATERIAL_TYPE materialData)
        {
            try
            {
                ADO.MedicineTypeADO materialADO = new ADO.MedicineTypeADO();
                Inventec.Common.Mapper.DataObjectMapper.Map<ADO.MedicineTypeADO>(materialADO, materialData);
                materialADO.Type = Base.GlobalConfig.VATTU;
                materialADO.IdRow = setIdRow(this.ListMedicineTypeAdoProcess);
                materialADO.AMOUNT = anticipateMatyData.AMOUNT;
                materialADO.AllowAmount = null;
                materialADO.IMP_PRICE = anticipateMatyData.IMP_PRICE;
                materialADO.SUPPLIER_ID = anticipateMatyData.SUPPLIER_ID;
                materialADO.SUPPLIER_NAME = anticipateMatyData.SUPPLIER_NAME;
                materialADO.MEDICINE_TYPE_CODE = materialData.MATERIAL_TYPE_CODE;
                materialADO.MEDICINE_TYPE_NAME = materialData.MATERIAL_TYPE_NAME;

                this.ListMedicineTypeAdoProcess.Add(materialADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void addBlood()
        {
            try
            {
                this.bloodType.AMOUNT = spinAmount.Value;
                this.bloodType.IMP_PRICE = spinImpPrice.Value;
                ADO.MedicineTypeADO aMedicineAdo = new ADO.MedicineTypeADO();
                AutoMapper.Mapper.CreateMap<ADO.BloodTypeADO, ADO.MedicineTypeADO>();
                aMedicineAdo = AutoMapper.Mapper.Map<ADO.BloodTypeADO, ADO.MedicineTypeADO>(this.bloodType);
                aMedicineAdo.MEDICINE_TYPE_CODE = this.bloodType.BLOOD_TYPE_CODE;
                aMedicineAdo.MEDICINE_TYPE_NAME = this.bloodType.BLOOD_TYPE_NAME;
                aMedicineAdo.Type = Base.GlobalConfig.MAU;
                aMedicineAdo.IdRow = setIdRow(this.ListMedicineTypeAdoProcess);
                if (cboSupplier.EditValue != null)
                {
                    aMedicineAdo.SUPPLIER_ID = Inventec.Common.TypeConvert.Parse.ToInt32(cboSupplier.EditValue.ToString());
                    var supplier = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SUPPLIER>().FirstOrDefault(o => o.ID == aMedicineAdo.SUPPLIER_ID);
                    aMedicineAdo.SUPPLIER_NAME = supplier.SUPPLIER_NAME;
                }
                else
                {
                    aMedicineAdo.SUPPLIER_ID = null;
                    aMedicineAdo.SUPPLIER_NAME = null;
                }

                if (cboBid.EditValue != null)
                {
                    var bid = this.ListBid.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt32(cboBid.EditValue.ToString()));
                    if (bid != null)
                    {
                        aMedicineAdo.BID_ID = bid.ID;
                        aMedicineAdo.BID_NUMBER = bid.BID_NUMBER;
                        aMedicineAdo.BID_YEAR = bid.BID_YEAR;
                    }
                }
                else
                {
                    aMedicineAdo.BID_ID = null;
                    aMedicineAdo.BID_NUMBER = null;
                    aMedicineAdo.BID_YEAR = null;
                }

                aMedicineAdo.IsNotSave = false;
                this.ListMedicineTypeAdoProcess.Add(aMedicineAdo);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void addBlood(List<V_HIS_BID_BLOOD_TYPE> listBidData)
        {
            try
            {
                if (this.listAticipateBltys != null && this.listAticipateBltys.Count > 0)
                {
                    foreach (var item in this.listAticipateBltys)
                    {
                        var checkBid = listBidData.FirstOrDefault(o => o.BLOOD_TYPE_ID == item.BLOOD_TYPE_ID);
                        var bloodAdd = Base.GlobalConfig.HisBloodTypes.FirstOrDefault(o => o.ID == item.BLOOD_TYPE_ID);
                        ADO.MedicineTypeADO bloodADO = new ADO.MedicineTypeADO();
                        if (bloodAdd != null)
                        {
                            Inventec.Common.Mapper.DataObjectMapper.Map<ADO.MedicineTypeADO>(bloodADO, bloodAdd);
                        }
                        bloodADO.Type = Base.GlobalConfig.MAU;
                        bloodADO.IdRow = setIdRow(this.ListMedicineTypeAdoProcess);
                        bloodADO.AMOUNT = item.AMOUNT;

                        if (checkBid != null)
                        {
                            bloodADO.AllowAmount = checkBid.AMOUNT - (checkBid.IN_AMOUNT != null ? checkBid.IN_AMOUNT : 0);
                            bloodADO.isDelete = false;
                        }
                        else
                        {
                            bloodADO.AllowAmount = null;
                            bloodADO.isDelete = true;
                        }

                        //if (bloodADO.AllowAmount != null)
                        //{
                        //    bloodADO.isAmount = item.AMOUNT > bloodADO.AllowAmount;
                        //}

                        bloodADO.IMP_PRICE = item.IMP_PRICE;
                        bloodADO.SUPPLIER_ID = item.SUPPLIER_ID;
                        bloodADO.SUPPLIER_NAME = item.SUPPLIER_NAME;
                        bloodADO.ID = item.BLOOD_TYPE_ID;
                        bloodADO.MEDICINE_TYPE_CODE = item.BLOOD_TYPE_CODE;
                        bloodADO.MEDICINE_TYPE_NAME = item.BLOOD_TYPE_NAME;
                        bloodADO.BID_ID = item.BID_ID;
                        this.ListMedicineTypeAdoProcess.Add(bloodADO);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void addBlood(V_HIS_ANTICIPATE_BLTY anticipateBltyData, V_HIS_BLOOD_TYPE bloodData)
        {
            try
            {
                ADO.MedicineTypeADO bloodADO = new ADO.MedicineTypeADO();
                Inventec.Common.Mapper.DataObjectMapper.Map<ADO.MedicineTypeADO>(bloodADO, bloodData);
                bloodADO.Type = Base.GlobalConfig.MAU;
                bloodADO.IdRow = setIdRow(this.ListMedicineTypeAdoProcess);
                bloodADO.AMOUNT = anticipateBltyData.AMOUNT;
                bloodADO.AllowAmount = null;
                bloodADO.IMP_PRICE = anticipateBltyData.IMP_PRICE;
                bloodADO.SUPPLIER_ID = anticipateBltyData.SUPPLIER_ID;
                bloodADO.SUPPLIER_NAME = anticipateBltyData.SUPPLIER_NAME;
                bloodADO.ID = anticipateBltyData.BLOOD_TYPE_ID;
                bloodADO.MEDICINE_TYPE_CODE = bloodData.BLOOD_TYPE_CODE;
                bloodADO.MEDICINE_TYPE_NAME = bloodData.BLOOD_TYPE_NAME;
                this.ListMedicineTypeAdoProcess.Add(bloodADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private double setIdRow(List<ADO.MedicineTypeADO> medicineTypes)
        {
            double currentIdRow = 0;
            try
            {
                if (medicineTypes != null && medicineTypes.Count > 0)
                {
                    var maxIdRow = medicineTypes.Max(o => o.IdRow);
                    currentIdRow = ++maxIdRow;
                }
                else
                {
                    currentIdRow = 1;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return currentIdRow;
        }
        #endregion

        #region Delete Edit
        private void cboSupplier_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboSupplier.Properties.Buttons[1].Visible = false;
                    cboSupplier.EditValue = null;
                    txtSupplierCode.Text = "";
                    txtSupplierCode.Focus();
                    txtSupplierCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboSupplier_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboSupplier.EditValue != null)
                {
                    var data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SUPPLIER>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboSupplier.EditValue ?? 0).ToString()));
                    if (data != null)
                    {
                        txtSupplierCode.Text = data.SUPPLIER_CODE;
                        cboSupplier.Properties.Buttons[1].Visible = true;
                        LoadDataToCboBid(lstBidAdo.Where(o => o.SUPPLIER_ID == data.ID).ToList());

                    }
                }
                else
                {
                    LoadDataToCboBid(lstBidAdo);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboSupplier1_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboSupplier1.Properties.Buttons[1].Visible = false;
                    cboSupplier1.EditValue = null;
                    txtSupplierCode1.Text = "";
                    txtSupplierCode1.Focus();
                    txtSupplierCode1.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboBid1_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboBid1.Properties.Buttons[1].Visible = false;
                    cboBid1.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboBid1_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboBid1.EditValue != null)
                {
                    cboBid1.Properties.Buttons[1].Visible = true;
                    var data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BID>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboBid1.EditValue.ToString()));
                    if (data != null)
                    {
                        this.currentBidId = data.ID;
                        List<long> supplierIds = new List<long>();
                        if (xtraTabControl1.SelectedTabPageIndex == 0)
                        {
                            if (ListBidMedicineType != null && ListBidMedicineType.Count > 0)
                            {
                                var lst = ListBidMedicineType.Where(o => o.BID_ID == data.ID).ToList();
                                supplierIds = (lst != null && lst.Count > 0) ? lst.Select(o => o.SUPPLIER_ID).ToList() : null;
                            }

                        }
                        else if (xtraTabControl1.SelectedTabPageIndex == 1)
                        {
                            if (ListBidMaterialType != null && ListBidMaterialType.Count > 0)
                            {
                                var lst = ListBidMaterialType.Where(o => o.BID_ID == data.ID).ToList();
                                supplierIds = (lst != null && lst.Count > 0) ? lst.Select(o => o.SUPPLIER_ID).ToList() : null;
                            }
                        }
                        cboSupplier1.Properties.DataSource = supplierIds != null && supplierIds.Count > 0 ? ListSupplier.Where(o => supplierIds.Contains(o.ID)).ToList() : null;
                    }
                }
                else
                {
                    this.currentBidId = 0;
                    cboSupplier1.Properties.DataSource = ListSupplier;
                }
                FillDataToGridMedicineType();
                FillDataToGridMaterialType();
                gridControlBid.DataSource = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ButtonEdit_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (this.ActionType == GlobalVariables.ActionView) return;
                this.medicineType = new ADO.MedicineTypeADO();

                editMedicineTypeAdo = (ADO.MedicineTypeADO)gridViewProcess.GetFocusedRow();
                if (editMedicineTypeAdo != null)
                {
                    this.medicineType = editMedicineTypeAdo;
                    if (editMedicineTypeAdo.Type == Base.GlobalConfig.THUOC)
                    {
                        xtraTabControl1.SelectedTabPageIndex = 0;
                        this.medicineType = editMedicineTypeAdo;
                    }
                    else if (editMedicineTypeAdo.Type == Base.GlobalConfig.VATTU)
                    {
                        xtraTabControl1.SelectedTabPageIndex = 1;
                        this.materialType = new ADO.MaterialTypeADO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<ADO.MaterialTypeADO>(this.materialType, editMedicineTypeAdo);
                        this.materialType.MATERIAL_TYPE_CODE = editMedicineTypeAdo.MEDICINE_TYPE_CODE;
                        this.materialType.MATERIAL_TYPE_NAME = editMedicineTypeAdo.MEDICINE_TYPE_NAME;
                    }
                    else if (editMedicineTypeAdo.Type == Base.GlobalConfig.MAU)
                    {
                        xtraTabControl1.SelectedTabPageIndex = 2;
                        this.bloodType = new ADO.BloodTypeADO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<ADO.BloodTypeADO>(this.bloodType, editMedicineTypeAdo);
                        this.bloodType.BLOOD_TYPE_CODE = editMedicineTypeAdo.MEDICINE_TYPE_CODE;
                        this.bloodType.BLOOD_TYPE_NAME = editMedicineTypeAdo.MEDICINE_TYPE_NAME;
                    }
                    this.ActionType = GlobalVariables.ActionEdit;
                    VisibleButton(this.ActionType);
                    var supplier = Base.GlobalConfig.ListSupplier.FirstOrDefault(o => o.ID == editMedicineTypeAdo.SUPPLIER_ID);
                    if (chkItemType.Checked)
                    {
                        if (supplier != null)
                        {
                            txtSupplierCode.Text = supplier.SUPPLIER_CODE;
                            cboSupplier.EditValue = supplier.ID;
                            cboSupplier.Properties.Buttons[1].Visible = true;
                        }
                        else
                        {
                            txtSupplierCode.Text = "";
                            cboSupplier.EditValue = null;
                            cboSupplier.Properties.Buttons[1].Visible = false;
                        }

                        if (editMedicineTypeAdo.BID_ID.HasValue)
                        {
                            cboBid.EditValue = editMedicineTypeAdo.BID_ID;
                            cboBid.Properties.Buttons[1].Visible = true;
                        }
                        else
                        {
                            cboBid.EditValue = null;
                            cboBid.Properties.Buttons[1].Visible = false;
                        }
                    }
                    else
                    {
                        if (supplier != null)
                        {
                            txtSupplierCode1.Text = supplier.SUPPLIER_CODE;
                            cboSupplier1.EditValue = supplier.ID;
                            cboSupplier1.Properties.Buttons[1].Visible = true;
                        }
                        else
                        {
                            txtSupplierCode1.Text = "";
                            cboSupplier1.EditValue = null;
                            cboSupplier1.Properties.Buttons[1].Visible = false;
                        }

                        if (editMedicineTypeAdo.BID_ID.HasValue)
                        {
                            cboBid1.EditValue = editMedicineTypeAdo.BID_ID;
                            cboBid1.Properties.Buttons[1].Visible = true;
                        }
                        else
                        {
                            cboBid1.EditValue = null;
                            cboBid1.Properties.Buttons[1].Visible = false;
                        }
                    }

                    spinAmount.EditValue = editMedicineTypeAdo.AMOUNT;
                    spinImpPrice.EditValue = editMedicineTypeAdo.IMP_PRICE;
                    spinAmount.Focus();
                    spinAmount.SelectAll();
                    btnSave.Enabled = false;
                    dxValidationProviderLeftPanel.RemoveControlError(spinImpPrice);
                    dxValidationProviderLeftPanel.RemoveControlError(spinAmount);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ButtonDelete_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var row = (ADO.MedicineTypeADO)gridViewProcess.GetFocusedRow();
                idRow = row.IdRow;
                foreach (var item in this.ListMedicineTypeAdoProcess)
                {
                    if (idRow == item.IdRow)
                    {
                        this.ListMedicineTypeAdoProcess.RemoveAll(o => o.IdRow == idRow);
                        idRow = -1;
                        break;
                    }
                }
                gridControlProcess.BeginUpdate();
                gridControlProcess.DataSource = null;
                gridControlProcess.DataSource = this.ListMedicineTypeAdoProcess;
                gridControlProcess.EndUpdate();
                this.ActionType = GlobalVariables.ActionAdd;
                SetDefaultValueControlLeft();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region enter
        private void spinAmount_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinImpPrice.Focus();
                    spinImpPrice.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinImpPrice_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtSupplierCode.Focus();
                    txtSupplierCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSupplierCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool showCbo = true;
                    if (!String.IsNullOrEmpty(txtSupplierCode.Text.Trim()))
                    {
                        string code = txtSupplierCode.Text.Trim();
                        var listData = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SUPPLIER>().Where(o => o.SUPPLIER_CODE.Contains(code)).ToList();
                        List<MOS.EFMODEL.DataModels.HIS_SUPPLIER> result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.SUPPLIER_CODE == code).ToList() : listData) : null;
                        if (result != null && result.Count == 1)
                        {
                            showCbo = false;
                            txtSupplierCode.Text = result.First().SUPPLIER_CODE;
                            cboSupplier.EditValue = result.First().ID;
                            cboSupplier.Properties.Buttons[1].Visible = true;
                        }
                    }
                    if (showCbo)
                    {
                        cboSupplier.Focus();
                        cboSupplier.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboSupplier_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboSupplier.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_SUPPLIER data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SUPPLIER>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboSupplier.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtSupplierCode.Text = data.SUPPLIER_CODE;
                            cboSupplier.Properties.Buttons[1].Visible = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtUseTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDescription.Focus();
                    txtDescription.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboSupplier_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboSupplier.EditValue != null)
                    {
                        var data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SUPPLIER>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboSupplier.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtSupplierCode.Text = data.SUPPLIER_CODE;
                            cboSupplier.Properties.Buttons[1].Visible = true;
                            LoadDataToCboBid(lstBidAdo.Where(o => o.SUPPLIER_ID == data.ID).ToList());

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSupplierCode1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool showCbo = true;
                    if (!String.IsNullOrEmpty(txtSupplierCode1.Text.Trim()))
                    {
                        string code = txtSupplierCode1.Text.Trim();
                        var listData = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SUPPLIER>().Where(o => o.SUPPLIER_CODE.Contains(code)).ToList();
                        List<MOS.EFMODEL.DataModels.HIS_SUPPLIER> result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.SUPPLIER_CODE == code).ToList() : listData) : null;
                        if (result != null && result.Count == 1)
                        {
                            showCbo = false;
                            txtSupplierCode1.Text = result.First().SUPPLIER_CODE;
                            cboSupplier1.EditValue = result.First().ID;
                        }
                    }
                    if (showCbo)
                    {
                        cboSupplier1.Focus();
                        cboSupplier1.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboSupplier1_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboSupplier1.EditValue != null)
                {
                    var data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SUPPLIER>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboSupplier1.EditValue ?? 0).ToString()));
                    if (data != null)
                    {
                        this.currentSupplierId = data.ID;
                        txtSupplierCode1.Text = data.SUPPLIER_CODE;
                        cboSupplier1.Properties.Buttons[1].Visible = true;
                        List<long> bidIds = new List<long>();
                        if (xtraTabControl1.SelectedTabPageIndex == 0)
                        {
                            if (ListBidMedicineType != null && ListBidMedicineType.Count > 0)
                            {
                                var lst = ListBidMedicineType.Where(o => o.SUPPLIER_ID == data.ID).ToList();
                                bidIds = (lst != null && lst.Count > 0) ? lst.Select(o => o.BID_ID).ToList() : null;
                            }
                        }
                        else if (xtraTabControl1.SelectedTabPageIndex == 1)
                        {
                            if (ListBidMaterialType != null && ListBidMaterialType.Count > 0)
                            {
                                var lst = ListBidMaterialType.Where(o => o.SUPPLIER_ID == data.ID).ToList();
                                bidIds = (lst != null && lst.Count > 0) ? lst.Select(o => o.BID_ID).ToList() : null;
                            }
                        }
                        cboBid1.Properties.DataSource = bidIds != null && bidIds.Count > 0 ? ListBid.Where(o => bidIds.Contains(o.ID)).ToList() : null;
                    }
                }
                else
                {
                    this.currentSupplierId = 0;
                    cboBid1.Properties.DataSource = ListBid;
                }
                FillDataToGridMedicineType();
                FillDataToGridMaterialType();
                gridControlBid.DataSource = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboBid_EditValueChanged(object sender, EventArgs e)
        {

            try
            {
                if (cboBid.EditValue != null)
                {
                    cboBid.Properties.Buttons[1].Visible = true;
                    if (xtraTabControl1.SelectedTabPageIndex == 0)
                    {
                        var lst = bidMedicineTypes.Where(o => o.BID_ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboBid.EditValue.ToString())).ToList();
                        if (lst != null && lst.Count > 0)
                        {
                            LoadDataToCboSupplier(lst.Select(o => o.SUPPLIER_ID).ToList());
                        }
                    }
                    else if (xtraTabControl1.SelectedTabPageIndex == 1)
                    {
                        var lst = bidMaterialTypes.Where(o => o.BID_ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboBid.EditValue.ToString())).ToList();
                        if (lst != null && lst.Count > 0)
                        {
                            LoadDataToCboSupplier(lst.Select(o => o.SUPPLIER_ID).ToList());
                        }
                    }
                }
                else
                {
                    if (xtraTabControl1.SelectedTabPageIndex == 0)
                    {
                        LoadDataToCboSupplier(bidMedicineTypes.Select(o => o.SUPPLIER_ID).ToList());
                    }
                    else if (xtraTabControl1.SelectedTabPageIndex == 1)
                    {
                        LoadDataToCboSupplier(bidMaterialTypes.Select(o => o.SUPPLIER_ID).ToList());
                    }
                }


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion
    }
}
