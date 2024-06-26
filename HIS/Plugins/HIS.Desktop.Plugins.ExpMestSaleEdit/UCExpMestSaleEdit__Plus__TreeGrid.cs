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
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.Plugins.ExpMestSaleEdit.ADO;
using HIS.UC.MaterialTypeInStock.ADO;
using HIS.UC.MedicineTypeInStock.ADO;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExpMestSaleEdit
{
    public partial class UCExpMestSaleEdit
    {

        private void expMestMediGrid__CustomUnboundColumnData(V_HIS_EXP_MEST_MEDICINE data, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (data != null)
                {
                    if (e.Column.FieldName == "EXPIRED_DATE_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.EXPIRED_DATE ?? 0);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void expMestMateGrid__CustomUnboundColumnData(V_HIS_EXP_MEST_MATERIAL data, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (data != null)
                {
                    if (e.Column.FieldName == "EXPIRED_DATE_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.EXPIRED_DATE ?? 0);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void medicineInStockTree_RowEnter(MedicineTypeInStockADO data)
        {
            try
            {
                WaitingManager.Show();
                this.currentMediMate = null;
                if (data != null)
                {
                    this.currentMediMate = new ADO.MediMateTypeADO(data);
                }
                SetEnableControlPriceByCheckBox();
                SetValueByMediMateADO();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void medicineInStockTree_CLick(MedicineTypeInStockADO data)
        {
            try
            {
                WaitingManager.Show();
                this.currentMediMate = null;
                if (data != null)
                {
                    this.currentMediMate = new ADO.MediMateTypeADO(data);
                }
                SetEnableControlPriceByCheckBox();
                SetValueByMediMateADO();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void materialInStockTree_EnterRow(MaterialTypeInStockADO data)
        {
            try
            {
                WaitingManager.Show();
                this.currentMediMate = null;
                if (data != null)
                {
                    this.currentMediMate = new ADO.MediMateTypeADO(data);
                }
                SetEnableControlPriceByCheckBox();
                SetValueByMediMateADO();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void materialInStockTree_Click(MaterialTypeInStockADO data)
        {
            try
            {
                WaitingManager.Show();
                this.currentMediMate = null;
                if (data != null)
                {
                    this.currentMediMate = new ADO.MediMateTypeADO(data);
                }
                SetEnableControlPriceByCheckBox();
                SetValueByMediMateADO();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestDetail_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.ListSourceRowIndex >= 0 && e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound && e.Column.FieldName == "STT")
                {
                    var data = (MediMateTypeADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestDetail_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                if (e.RowHandle < 0 || e.Column.FieldName != "EXP_AMOUNT")
                    return;
                var data = (MediMateTypeADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                if (data != null)
                {
                    if (data.IsMedicine)
                    {
                        data.ExpMedicine.Amount = data.EXP_AMOUNT;
                        data.ADVISORY_TOTAL_PRICE = (data.EXP_AMOUNT * (data.ADVISORY_PRICE ?? 0) * (1 + (data.EXP_VAT_RATIO ?? 0) / 100));
                    }
                    else
                    {
                        data.ExpMaterial.Amount = data.EXP_AMOUNT;
                    }
                    if (data.EXP_AMOUNT > data.AVAILABLE_AMOUNT)
                    {
                        data.IsGreatAvailable = true;
                    }
                    else
                    {
                        data.IsGreatAvailable = false;
                    }
                    this.SetTotalPrice();
                }
                gridControlExpMestDetail.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestDetail_InvalidRowException(object sender, DevExpress.XtraGrid.Views.Base.InvalidRowExceptionEventArgs e)
        {
            try
            {
                e.ExceptionMode = DevExpress.XtraEditors.Controls.ExceptionMode.NoAction;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestDetail_ShownEditor(object sender, EventArgs e)
        {
            try
            {
                if (gridViewExpMestDetail.FocusedRowHandle < 0 || gridViewExpMestDetail.FocusedColumn.FieldName != "EXP_AMOUNT")
                    return;
                var data = (MediMateTypeADO)((IList)((BaseView)sender).DataSource)[gridViewExpMestDetail.FocusedRowHandle];
                if (data != null)
                {
                    bool valid = true;
                    string message = "";
                    if (data.EXP_AMOUNT <= 0)
                    {
                        valid = false;
                        message = Base.ResourceMessageLang.SoLuongXuatPhaiLonHongKhong;
                    }
                    else if (data.EXP_AMOUNT > data.AVAILABLE_AMOUNT)
                    {
                        valid = false;
                        message = Base.ResourceMessageLang.SoLuongXuatLonHonSoLuongKhaDungTrongKho;
                    }
                    if (!valid)
                    {
                        gridViewExpMestDetail.SetColumnError(gridViewExpMestDetail.FocusedColumn, message);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestDetail_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var data = (MediMateTypeADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (data != null)
                    {
                        if (data.IsNotHasMest)
                        {
                            e.Appearance.ForeColor = Color.Red;
                        }
                        else if (data.IsGreatAvailable)
                        {
                            e.Appearance.ForeColor = Color.Red;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetValueByMediMateADO()
        {
            try
            {
                spinAmount.Value = 0;
                txtNote.Text = "";
                if (this.currentMediMate != null)
                {
                    btnAdd.Enabled = true;
                    //spinAmount.Properties.MaxValue = this.currentMediMate.AVAILABLE_AMOUNT ?? 0;
                    spinAmount.Focus();
                    spinAmount.SelectAll();
                }
                else
                {
                    btnAdd.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
