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
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Controls;
using HIS.Desktop.Plugins.BaseCompensationBillCreate.ValidationRules;
using Inventec.Core;
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
using HIS.Desktop.Plugins.BaseCompensationBillCreate.ADO;
using HIS.Desktop.LocalStorage.LocalData;

namespace HIS.Desktop.Plugins.BaseCompensationBillCreate
{
    public partial class frmBaseCompensationBillCreate : HIS.Desktop.Utility.FormBase
    {
        internal void AddMedicineClick()
        {
            try
            {
                bool valid = true;
                decimal total = 0;
                if (spinAmount.Value <= 0 || (this.HisMedicineTypeInStockSDO == null))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Thiếu trường dữ liệu bắt buộc, vui lòng kiểm tra lại.", "Thông báo");
                    spinAmount.SelectAll();
                    spinAmount.Focus();
                    valid = false;
                }

                if ((spinAmount.Value) > (this.HisMedicineTypeInStockSDO.AvailableAmount ?? 0))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Số lượng vượt quá số lượng cho phép.", "Thông báo");
                    spinAmount.SelectAll();
                    spinAmount.Focus();
                    valid = false;
                }
                if (valid == true)
                {
                    if (ActionBosung == GlobalVariables.ActionAdd)
                    {
                        foreach (var item in this.ListVHisMedicineTypeProcess)
                        {
                            if (this.HisMedicineTypeInStockSDO.Id == item.ID && item.IS_MEDICINE)
                            {
                                total = total + item.AMOUNT;
                            }
                        }
                        foreach (var item in this.ListVHisMedicineTypeProcess)
                        {
                            this.ListVHisMedicineTypeProcess.RemoveAll(o => (o.ID == this.HisMedicineTypeInStockSDO.Id && o.IS_MEDICINE));
                            this.HisChmsExpMestSDO.ExpMedicines.RemoveAll(o => o.MedicineTypeId == this.HisMedicineTypeInStockSDO.Id);
                            break;
                        }
                        this.MedicineTypeModel = new MssMedicineTypeSDO();
                        HisMedicineTypeAmountSDO medicineTypeAmount = new HisMedicineTypeAmountSDO();

                        if (ListVHisMedicineTypeProcess != null && ListVHisMedicineTypeProcess.Count > 0)
                        {
                            var idRowMax = ListVHisMedicineTypeProcess.Max(o => o.IdRow);
                            this.MedicineTypeModel.IdRow = idRowMax + 1;
                        }
                        this.MedicineTypeModel.ID = this.HisMedicineTypeInStockSDO.Id;
                        this.MedicineTypeModel.MEDICINE_TYPE_CODE = this.HisMedicineTypeInStockSDO.MedicineTypeCode;
                        this.MedicineTypeModel.MEDICINE_TYPE_NAME = this.HisMedicineTypeInStockSDO.MedicineTypeName;
                        this.MedicineTypeModel.SERVICE_UNIT_NAME = this.HisMedicineTypeInStockSDO.ServiceUnitName;
                        this.MedicineTypeModel.SERVICE_UNIT_CODE = this.HisMedicineTypeInStockSDO.ServiceUnitCode;
                        this.MedicineTypeModel.SERVICE_UNIT_ID = (this.HisMedicineTypeInStockSDO.ServiceUnitId ?? 0);
                        this.MedicineTypeModel.AMOUNT = total + Inventec.Common.TypeConvert.Parse.ToDecimal(spinAmount.Text);
                        medicineTypeAmount.MedicineTypeId = this.HisMedicineTypeInStockSDO.Id;
                        medicineTypeAmount.Amount = total + Inventec.Common.TypeConvert.Parse.ToDecimal(spinAmount.Text);
                        MedicineTypeModel.IS_MEDICINE = true;
                        ListVHisMedicineTypeProcess.Insert(0, this.MedicineTypeModel);
                        total = 0;
                    }
                    else if (ActionBosung == GlobalVariables.ActionEdit)
                    {
                        foreach (var item in this.ListVHisMedicineTypeProcess)
                        {
                            this.ListVHisMedicineTypeProcess.RemoveAll(o => o.ID == this.HisMedicineTypeInStockSDO.Id);
                            break;
                        }
                        this.MedicineTypeModel = new MssMedicineTypeSDO();
                        if (ListVHisMedicineTypeProcess != null && ListVHisMedicineTypeProcess.Count > 0)
                        {
                            var idRowMax = ListVHisMedicineTypeProcess.Max(o => o.IdRow);
                            this.MedicineTypeModel.IdRow = idRowMax + 1;
                        }
                        this.MedicineTypeModel.ID = this.HisMedicineTypeInStockSDO.Id;
                        this.MedicineTypeModel.MEDICINE_TYPE_CODE = this.HisMedicineTypeInStockSDO.MedicineTypeCode;
                        this.MedicineTypeModel.MEDICINE_TYPE_NAME = this.HisMedicineTypeInStockSDO.MedicineTypeName;
                        this.MedicineTypeModel.SERVICE_UNIT_NAME = this.HisMedicineTypeInStockSDO.ServiceUnitName;
                        this.MedicineTypeModel.SERVICE_UNIT_CODE = this.HisMedicineTypeInStockSDO.ServiceUnitCode;
                        this.MedicineTypeModel.SERVICE_UNIT_ID = (this.HisMedicineTypeInStockSDO.ServiceUnitId ?? 0);
                        this.MedicineTypeModel.AMOUNT = Inventec.Common.TypeConvert.Parse.ToDecimal(spinAmount.Text);
                        this.MedicineTypeModel.IS_MEDICINE = true;
                        ListVHisMedicineTypeProcess.Add(this.MedicineTypeModel);
                    }
                    gridControlDetailMedicineProcess.DataSource = null;
                    gridControlDetailMedicineProcess.DataSource = this.ListVHisMedicineTypeProcess;
                    ActionBosung = GlobalVariables.ActionAdd;
                    spinAmount.Value = 0;
                    txtKeyword__Medicine.Focus();
                    txtKeyword__Medicine.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void MedicineTypeKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var row = (HisMedicineTypeInStockSDO)gridViewMedicineType.GetFocusedRow();
                    if (row != null)
                    {
                        this.focusedRowHandleMedicine = gridViewMedicineType.FocusedRowHandle;
                        //ResetControlAddMedicine();
                        this.HisMedicineTypeInStockSDO = row;
                        txtMedicineTypeCode.Text = row.MedicineTypeCode;
                        txtMedicineTypeName.Text = row.MedicineTypeName;
                        //txtNationalName.Text = row.NationalName;
                        //txtMenufactureName.Text = row.ManufacturerName;  
                        txtServiceUnitName.Text = row.ServiceUnitName;
                        if (sender != null)
                        {
                            spinAmount.SelectAll();
                            spinAmount.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void MedicineTypeRowCellClick(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            try
            {
                this.HisMedicineTypeInStockSDO = (HisMedicineTypeInStockSDO)gridViewMedicineType.GetFocusedRow();
                if (this.HisMedicineTypeInStockSDO != null)
                {
                    this.focusedRowHandleMedicine = gridViewMedicineType.FocusedRowHandle;
                    txtMedicineTypeCode.Text = this.HisMedicineTypeInStockSDO.MedicineTypeCode;
                    txtMedicineTypeName.Text = this.HisMedicineTypeInStockSDO.MedicineTypeName;
                    //txtNationalName.Text = this.HisMedicineTypeInStockSDO.NationalName;
                    //txtMenufactureName.Text = this.HisMedicineTypeInStockSDO.ManufacturerName;
                    txtServiceUnitName.Text = this.HisMedicineTypeInStockSDO.ServiceUnitName;
                    ActionBosung = GlobalVariables.ActionAdd;
                    if (sender != null)
                    {
                        spinAmount.Value = 0;
                        spinAmount.SelectAll();
                        spinAmount.Focus();
                    }

                }
                else
                {
                    txtMedicineTypeCode.Text = "";
                    txtMedicineTypeName.Text = "";
                    //txtNationalName.Text = "";
                    //txtMenufactureName.Text = "";
                    txtServiceUnitName.Text = "";
                    ActionBosung = GlobalVariables.ActionAdd;
                    spinAmount.Value = 0;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
