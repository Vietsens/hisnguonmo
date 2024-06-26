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
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.BaseCompensationBillCreate.ADO;
using HIS.Desktop.Plugins.BaseCompensationBillCreate.ValidationRules;
using Inventec.Common.Adapter;
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

namespace HIS.Desktop.Plugins.BaseCompensationBillCreate
{
    public partial class frmBaseCompensationBillCreate : HIS.Desktop.Utility.FormBase
    {
        internal void AddMaterialClick()
        {
            try
            {
                bool valid = true;
                decimal total = 0;

                if (spinAmount.Value <= 0 || (this.HisMaterialTypeInStockSDO == null))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Thiếu trường dữ liệu bắt buộc, vui lòng kiểm tra lại.", "Thông báo");
                    spinAmount.SelectAll();
                    spinAmount.Focus();
                    valid = false;
                }

                if ((spinAmount.Value) > (this.HisMaterialTypeInStockSDO.AvailableAmount ?? 0))
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
                            if ((!item.IS_MEDICINE) && this.HisMaterialTypeInStockSDO.Id == item.ID)
                            {
                                total = total + item.AMOUNT;
                            }
                        }
                        foreach (var item in this.ListVHisMedicineTypeProcess)
                        {
                            this.ListVHisMedicineTypeProcess.RemoveAll(o => (o.ID == this.HisMaterialTypeInStockSDO.Id && !o.IS_MEDICINE));
                            break;
                        }
                        this.MedicineTypeModel = new MssMedicineTypeSDO();

                        if (ListVHisMedicineTypeProcess != null && ListVHisMedicineTypeProcess.Count > 0)
                        {
                            var idRowMax = ListVHisMedicineTypeProcess.Max(o => o.IdRow);
                            this.MedicineTypeModel.IdRow = idRowMax + 1;
                        }
                        this.MedicineTypeModel.ID = this.HisMaterialTypeInStockSDO.Id;
                        this.MedicineTypeModel.MEDICINE_TYPE_CODE = this.HisMaterialTypeInStockSDO.MaterialTypeCode;
                        this.MedicineTypeModel.MEDICINE_TYPE_NAME = this.HisMaterialTypeInStockSDO.MaterialTypeName;
                        this.MedicineTypeModel.SERVICE_UNIT_NAME = this.HisMaterialTypeInStockSDO.ServiceUnitName;
                        this.MedicineTypeModel.SERVICE_UNIT_CODE = this.HisMaterialTypeInStockSDO.ServiceUnitCode;
                        this.MedicineTypeModel.SERVICE_UNIT_ID = (this.HisMaterialTypeInStockSDO.ServiceUnitId ?? 0);
                        this.MedicineTypeModel.AMOUNT = total + Inventec.Common.Number.Get.RoundCurrency(spinAmount.Value, 2);
                        MedicineTypeModel.IS_MEDICINE = false;
                        ListVHisMedicineTypeProcess.Insert(0, this.MedicineTypeModel);
                        total = 0;
                    }
                    else if (ActionBosung == GlobalVariables.ActionEdit)
                    {
                        foreach (var item in this.ListVHisMedicineTypeProcess)
                        {
                            this.ListVHisMedicineTypeProcess.RemoveAll(o => o.ID == this.HisMaterialTypeInStockSDO.Id);
                            break;
                        }
                        this.MedicineTypeModel = new MssMedicineTypeSDO();
                        if (ListVHisMedicineTypeProcess != null && ListVHisMedicineTypeProcess.Count > 0)
                        {
                            var idRowMax = ListVHisMedicineTypeProcess.Max(o => o.IdRow);
                            this.MedicineTypeModel.IdRow = idRowMax + 1;
                        }
                        this.MedicineTypeModel.ID = this.HisMaterialTypeInStockSDO.Id;
                        this.MedicineTypeModel.MEDICINE_TYPE_CODE = this.HisMaterialTypeInStockSDO.MaterialTypeCode;
                        this.MedicineTypeModel.MEDICINE_TYPE_NAME = this.HisMaterialTypeInStockSDO.MaterialTypeName;
                        this.MedicineTypeModel.SERVICE_UNIT_NAME = this.HisMaterialTypeInStockSDO.ServiceUnitName;
                        this.MedicineTypeModel.SERVICE_UNIT_CODE = this.HisMaterialTypeInStockSDO.ServiceUnitCode;
                        this.MedicineTypeModel.SERVICE_UNIT_ID = (this.HisMaterialTypeInStockSDO.ServiceUnitId ?? 0);
                        this.MedicineTypeModel.AMOUNT = Inventec.Common.Number.Get.RoundCurrency(spinAmount.Value, 2);
                        MedicineTypeModel.IS_MEDICINE = false;
                        ListVHisMedicineTypeProcess.Add(this.MedicineTypeModel);
                    }
                    gridControlDetailMedicineProcess.DataSource = null;
                    gridControlDetailMedicineProcess.DataSource = this.ListVHisMedicineTypeProcess;
                    ActionBosung = GlobalVariables.ActionAdd;
                    spinAmount.Value = 0;
                    txtKeyword__Material.Focus();
                    txtKeyword__Material.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void MaterialTypeKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var row = (HisMaterialTypeInStockSDO)gridViewMaterialType.GetFocusedRow();
                    if (row != null)
                    {
                        this.focusedRowHandleMaterial = gridViewMaterialType.FocusedRowHandle;
                        this.HisMaterialTypeInStockSDO = row;
                        txtMedicineTypeCode.Text = row.MaterialTypeCode;
                        txtMedicineTypeName.Text = row.MaterialTypeName;
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void MaterialTypeRowCellClick(object sender, RowCellClickEventArgs e)
        {
            CommonParam param = new CommonParam();
            try
            {
                this.HisMaterialTypeInStockSDO = (HisMaterialTypeInStockSDO)gridViewMaterialType.GetFocusedRow();
                if (this.HisMaterialTypeInStockSDO != null)
                {
                    this.focusedRowHandleMaterial = gridViewMaterialType.FocusedRowHandle;
                    txtMedicineTypeCode.Text = this.HisMaterialTypeInStockSDO.MaterialTypeCode;
                    txtMedicineTypeName.Text = this.HisMaterialTypeInStockSDO.MaterialTypeName;
                    //txtNationalName.Text = this.HisMaterialTypeInStockSDO.NationalName;
                    //txtMenufactureName.Text = this.HisMaterialTypeInStockSDO.ManufacturerName;     
                    txtServiceUnitName.Text = this.HisMaterialTypeInStockSDO.ServiceUnitName;
                    ActionBosung = GlobalVariables.ActionAdd;
                    if (sender != null)
                    {
                        spinAmount.Value = 0;
                        spinAmount.SelectAll();
                        spinAmount.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void RefeshDataAfterSaveToDbMaterial()
        {
            try
            {
                gridControlDetailMaterialExpView.DataSource = null;
                gridControlDetailMaterialExpView.DataSource = dataResult.ExpMaterials;
                LoadDataToGridMaterialTypeInStock();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void LoadDataToGridMaterialTypeInStock()
        {
            CommonParam param = new CommonParam();
            try
            {
                int currentFocusRowIndex = gridViewMaterialType.FocusedRowHandle;
                MOS.Filter.HisMaterialTypeStockViewFilter filter = new MOS.Filter.HisMaterialTypeStockViewFilter();
                filter.IS_LEAF = true;
                //filter.KEY_WORD = txtKeyword__Material.Text.Trim();
                filter.MEDI_STOCK_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboMediStockExport.EditValue ?? 0).ToString());
                currentMaterialTypeInStockSDOs = new BackendAdapter(param).Get<List<MOS.SDO.HisMaterialTypeInStockSDO>>(HisRequestUriStore.HIS_METERIAL_TYPE_GET_IN_STOCK_MATERIAL_TYPE, ApiConsumers.MosConsumer, filter, param);
                gridControlMaterialType.DataSource = currentMaterialTypeInStockSDOs;
                gridViewMaterialType.FocusedRowHandle = currentFocusRowIndex;

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
