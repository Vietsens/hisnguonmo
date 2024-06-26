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
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.Plugins.ExpMestChmsCreate.ADO;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ExpMestChmsCreate
{
    public partial class UCExpMestChmsCreate : HIS.Desktop.Utility.UserControlBase
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

        private void gridViewExpMestChmsDetail_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
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
                gridControlExpMestChmsDetail.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestChmsDetail_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var data = (MediMateTypeADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "EXP_AMOUNT")
                        {
                            if (data.IsSupplement)
                            {
                                e.RepositoryItem = repositoryItemSpinExpAmountDisable;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemSpinExpAmount;
                            }
                        }
                        else if (e.Column.FieldName == "BLOOD_ABO_ID")
                        {
                            if (data.IsBlood)
                            {
                                e.RepositoryItem = cboABO;
                            }
                        }
                        else if (e.Column.FieldName == "BLOOD_RH_ID")
                        {
                            if (data.IsBlood)
                            {
                                e.RepositoryItem = cboRH;
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

        private void gridViewExpMestChmsDetail_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MediMateTypeADO data = (MediMateTypeADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                  
                    if (data != null)
                    {
                        if (e.Column.FieldName == "VOLUME_DISPLAY")
                        {
                            if (data.IsBlood)
                            {
                                e.Value = data.VOLUME;
                            }
                            else
                            {
                                e.Value = "";
                            }
                        }
                        if (e.Column.FieldName == "MEDI_STOCK_STR")
                        {

                            //Nhập đc r 
                            if (radioImport.Checked)
                            {
                                if (data.MEDI_STOCK_ID.HasValue)
                                {
                                    e.Value = listExpMediStock.FirstOrDefault(o => o.ID == data.MEDI_STOCK_ID_IPM).MEDI_STOCK_NAME;
                                //e.Value = listExpMediStock.FirstOrDefault(o => o.ID == (long)cboExpMediStock.EditValue).MEDI_STOCK_NAME;
                                 }
                            }

                           
                            if (radioExport.Checked)
                            {
                                    e.Value = listImpMediStock.FirstOrDefault(o => o.ID == data.MEDI_STOCK_ID_IPM).MEDI_STOCK_NAME;
                                   // e.Value = listImpMediStock.FirstOrDefault(o => o.ID == (long)cboImpMediStock.EditValue).MEDI_STOCK_NAME;
                                    //e.Value = txtImpMediStock.EditValue;
                                
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

        private void gridViewExpMestChmsDetail_InvalidRowException(object sender, DevExpress.XtraGrid.Views.Base.InvalidRowExceptionEventArgs e)
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

        private void gridViewExpMestChmsDetail_ShownEditor(object sender, EventArgs e)
        {
            try
            {
                if (gridViewExpMestChmsDetail.FocusedRowHandle < 0 || gridViewExpMestChmsDetail.FocusedColumn.FieldName != "EXP_AMOUNT")
                    return;
                var data = (MediMateTypeADO)((IList)((BaseView)sender).DataSource)[gridViewExpMestChmsDetail.FocusedRowHandle];
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
                        message = Base.ResourceMessageLang.SoLuongXuatLonHonSoLuongKhaDungTrongKho;
                    }
                    if (!valid)
                    {
                        gridViewExpMestChmsDetail.SetColumnError(gridViewExpMestChmsDetail.FocusedColumn, message);
                    }
                    else
                    {
                        gridViewExpMestChmsDetail.SetColumnError(gridViewExpMestChmsDetail.FocusedColumn, "", DevExpress.XtraEditors.DXErrorProvider.ErrorType.None);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
