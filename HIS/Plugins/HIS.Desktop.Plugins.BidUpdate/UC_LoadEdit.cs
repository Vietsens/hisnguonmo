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
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.Plugins.BidUpdate.ADO;
using HIS.Desktop.Plugins.BidUpdate.Base;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.BidUpdate
{
    public partial class UC_LoadEdit : UserControl
    {
        BidEditADO bidEditAdo;
        Delete_ButtonClick deleteButtonClick;
        Grid_Click gridClick;
        List<ADO.MedicineTypeADO> listAdo;
        List<ADO.MedicineTypeADO> listAdoDefault;
        Inventec.Desktop.Common.Modules.Module Module;
        long bid_id;
        public bool IsFirstLoad = true;

        public UC_LoadEdit(BidEditADO ado, Inventec.Desktop.Common.Modules.Module Module, long bid_id)
        {
            InitializeComponent();
            try
            {
                this.bid_id = bid_id;
                this.Module = Module;
                this.bidEditAdo = ado;
                this.deleteButtonClick = ado.delete_ButtonClick;
                this.gridClick = ado.grid_Click;
                this.listAdo = ado.listADOs;

                if (ado.TYPE == GlobalConfig.VATTU)
                {
                    gridCol_ActiveBhyt.Visible = false;
                    gridCol_DosageForm.Visible = false;
                    gridCol_HeinServiceBhytName.Visible = false;
                    gridCol_MediUseForm.Visible = false;
                    gridCol_ParkingType.Visible = false;
                    gridCol_RegisterNumber.Visible = false;
                }
                else if (ado.TYPE == GlobalConfig.THUOC)
                {

                    gridCol_BidMaterialTypeCode.Visible = false;
                    gridCol_BidMaterialTypeName.Visible = false;
                    gridCol_JoinBidMaterialTypeCode.Visible = false;
                }
                else
                {
                    gridCol_ActiveBhyt.Visible = false;
                    gridCol_DosageForm.Visible = false;
                    gridCol_HeinServiceBhytName.Visible = false;
                    gridCol_MediUseForm.Visible = false;
                    gridCol_ParkingType.Visible = false;
                    gridCol_RegisterNumber.Visible = false;
                    gridCol_BidMaterialTypeCode.Visible = false;
                    gridCol_BidMaterialTypeName.Visible = false;
                    gridCol_JoinBidMaterialTypeCode.Visible = false;
                    gridCol_MonthLifespan.Visible = false;
                    gridCol_DayLifespan.Visible = false;
                    gridCol_HourLifespan.Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (ADO.MedicineTypeADO)gridViewEdit.GetFocusedRow();
                if (row != null && this.deleteButtonClick != null)
                {
                    this.deleteButtonClick(row);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridViewEdit_Click(object sender, EventArgs e)
        {
            try
            {
                var row = (ADO.MedicineTypeADO)gridViewEdit.GetFocusedRow();
                if (row != null && this.gridClick != null)
                {
                    this.gridClick(row);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridViewEdit_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (ADO.MedicineTypeADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewEdit_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                var data = (ADO.MedicineTypeADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                if (data != null)
                {
                    if (e.Column.FieldName == "ImpVatRatio")
                    {
                        data.IMP_VAT_RATIO = data.ImpVatRatio / 100;
                    }
                    gridControlEdit.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        public void Reload(List<ADO.MedicineTypeADO> data)
        {
            try
            {
                gridControlEdit.BeginUpdate();
                this.listAdo = data;
                if (IsFirstLoad && data != null && data.Count > 0)
                {
                    this.listAdoDefault = data;
                    IsFirstLoad = false;
                }
                gridControlEdit.DataSource = null;
                gridControlEdit.DataSource = data;
                gridControlEdit.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void UC_LoadEdit_Load(object sender, EventArgs e)
        {
            try
            {
                Reload(this.listAdo);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        public void PostEditor()
        {
            try
            {
                gridViewEdit.PostEditor();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!string.IsNullOrEmpty(txtSearch.Text.Trim()))
                    {
                        Search(txtSearch.Text.Trim().ToUpper());
                    }
                    else
                    {
                        Reload(this.listAdo);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Search(string keyWord)
        {
            try
            {
                if (listAdo != null && listAdo.Count > 0)
                {
                    List<ADO.MedicineTypeADO> ados = new List<MedicineTypeADO>();
                    ados = listAdo.Where(o =>
                        o.MEDICINE_TYPE_CODE.ToUpper().Contains(keyWord)
                        || o.MEDICINE_TYPE_NAME.ToUpper().Contains(keyWord)
                        ).ToList();

                    gridControlEdit.BeginUpdate();
                    gridControlEdit.DataSource = null;
                    gridControlEdit.DataSource = ados;
                    gridControlEdit.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void txtSearch_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtSearch.Text.Trim()))
                {
                    Search(txtSearch.Text.Trim().ToUpper());
                }
                else
                {
                    Reload(this.listAdo);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridViewEdit_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                ADO.MedicineTypeADO data = null;
                if (e.RowHandle > -1)
                {
                    data = (ADO.MedicineTypeADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                }
                if (e.RowHandle >= 0)
                {
                    if (e.Column.FieldName == "ADJUST_AMOUNT")
                    {
                        e.RepositoryItem = data.Type != Base.GlobalConfig.MAU ? listAdoDefault != null && listAdoDefault.Count > 0 && listAdoDefault.FirstOrDefault(o => o.ID == data.ID) != null ? repAdjustAmount : repAdjustAmountDis : spAdjustAmountDisable;
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public void FocusAdjAmount()
        {
            try
            {
                gridViewEdit.FocusedColumn = gridColumn17;
                gridViewEdit.FocusedRowHandle = listAdo.IndexOf(listAdo.FirstOrDefault(o => o.AMOUNT == 0 && o.ADJUST_AMOUNT == null));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repAdjustAmount_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                List<object> listArgs = new List<object>();
                var row = (MedicineTypeADO)gridViewEdit.GetFocusedRow();
                if (listAdoDefault.FirstOrDefault(o => o.ID == row.ID) != null)
                {
                    var objData = listAdoDefault.FirstOrDefault(o => o.ID == row.ID);

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => objData), objData));
                    if (objData.Type == Base.GlobalConfig.THUOC)
                        listArgs.Add(new HIS_BID_MEDICINE_TYPE() { ID = objData.BID_MEDI_MATY_BLO_ID, MEDICINE_TYPE_ID = objData.ID, AMOUNT = objData.AMOUNT ?? 0, ADJUST_AMOUNT = objData.ADJUST_AMOUNT, BID_ID = bid_id });
                    else
                        listArgs.Add(new HIS_BID_MATERIAL_TYPE() { ID = objData.BID_MEDI_MATY_BLO_ID, MATERIAL_TYPE_ID = objData.ID, AMOUNT = objData.AMOUNT ?? 0, ADJUST_AMOUNT = objData.ADJUST_AMOUNT, BID_ID = bid_id });
                    listArgs.Add((HIS.Desktop.Common.DelegateSelectData)((data) =>
                    {
                        objData.ADJUST_AMOUNT = data as decimal?;
                        row.ADJUST_AMOUNT = objData.ADJUST_AMOUNT;
                        gridControlEdit.RefreshDataSource();
                    }));

                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.BidRegulation", this.Module.RoomId, this.Module.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
