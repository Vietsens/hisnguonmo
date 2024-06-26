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
using HIS.Desktop.Plugins.ExpMestDepaCreate.ADO;
using HIS.Desktop.Utility;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ExpMestDepaCreate
{
    public partial class UCExpMestDepaCreate : UserControlBase
    {

        private void expMestMediGrid__CustomUnboundColumnData(V_HIS_EXP_MEST_MEDICINE data, CustomColumnDataEventArgs e)
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

        private void expMestMateGrid__CustomUnboundColumnData(V_HIS_EXP_MEST_MATERIAL data, CustomColumnDataEventArgs e)
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

        private void medicineInStockTree_RowEnter(UC.MedicineTypeInStock.ADO.MedicineTypeInStockADO data)
        {
            try
            {
                this.currentMediMate = null;
                if (data != null)
                {
                    this.currentMediMate = new ADO.MediMateTypeADO(data);
                }
                SetValueByMediMateADO();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bloodInStockTree_RowEnter(UC.HisBloodTypeInStock.ADO.HisBloodTypeInStockADO data)
        {
            try
            {
                this.currentMediMate = null;
                if (data != null)
                {
                    this.currentMediMate = new ADO.MediMateTypeADO(data);
                }
                SetValueByMediMateADO();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void medicineInStockTree_CLick(UC.MedicineTypeInStock.ADO.MedicineTypeInStockADO data)
        {
            try
            {
                this.currentMediMate = null;
                if (data != null)
                {
                    this.currentMediMate = new ADO.MediMateTypeADO(data);
                }
                SetValueByMediMateADO();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void materialInStockTree_Click(UC.MaterialTypeInStock.ADO.MaterialTypeInStockADO data)
        {
            try
            {
                this.currentMediMate = null;
                if (data != null)
                {
                    this.currentMediMate = new ADO.MediMateTypeADO(data);
                }
                SetValueByMediMateADO();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void bloodInStockTree_Click(UC.HisBloodTypeInStock.ADO.HisBloodTypeInStockADO data)
        {
            try
            {
                this.currentMediMate = null;
                if (data != null)
                {
                    this.currentMediMate = new MediMateTypeADO(data);
                }
                SetValueByMediMateADO();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void materialInStockTree_EnterRow(UC.MaterialTypeInStock.ADO.MaterialTypeInStockADO data)
        {
            try
            {
                this.currentMediMate = null;
                if (data != null)
                {
                    this.currentMediMate = new ADO.MediMateTypeADO(data);
                }
                SetValueByMediMateADO();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void materialInStockTree_Click_HC(UC.MaterialTypeInStock.ADO.MaterialTypeInStockADO data)
        {
            try
            {
                this.currentMediMate = null;
                if (data != null)
                {
                    this.currentMediMate = new ADO.MediMateTypeADO(data);
                    this.currentMediMate.IsHoaChat = true;
                }
                SetValueByMediMateADO();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void materialInStockTree_EnterRow_HC(UC.MaterialTypeInStock.ADO.MaterialTypeInStockADO data)
        {
            try
            {
                this.currentMediMate = null;
                if (data != null)
                {
                    this.currentMediMate = new ADO.MediMateTypeADO(data);
                    this.currentMediMate.IsHoaChat = true;
                }
                SetValueByMediMateADO();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestDetail_CellValueChanged(object sender, CellValueChangedEventArgs e)
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
                    }
                    else
                    {
                        data.ExpMaterial.Amount = data.EXP_AMOUNT;
                    }
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
                        message = Base.ResourceMessageLang.SoLuongXuatPhaiLonHonKhong;
                    }
                    else if (data.EXP_AMOUNT > data.AVAILABLE_AMOUNT)
                    {
                        valid = false;
                        message = Base.ResourceMessageLang.SoLuongXuatLonHonSoLuonKhaDungTrongKho;
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

        private void SetValueByMediMateADO()
        {
            try
            {

                spinExpAmount.Value = 0;
                txtNote.Text = "";
                if (this.currentMediMate != null)
                {
                    btnAdd.Enabled = true;
                    //spinExpAmount.Properties.MaxValue = this.currentMediMate.AVAILABLE_AMOUNT ?? 0;
                    spinExpAmount.Focus();
                    spinExpAmount.SelectAll();
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
