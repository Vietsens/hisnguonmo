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
using HIS.UC.MedicineTypeAcinGrid.ADO;
using HIS.UC.MedicineTypeAcinGrid.Run;
using HIS.UC.MedicineTypeAcinGrid.Reload;
using DevExpress.XtraGrid.Columns;
using DevExpress.Data;
using MOS.EFMODEL.DataModels;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace HIS.UC.MedicineTypeAcinGrid
{
    internal partial class UC_MedicineTypeAcinGrid : UserControl
    {
        #region Declare
        MedicineTypeAcinGridInitADO initADO = null;
        Grid_CustomUnboundColumnData gridView_CustomUnboundColumnData = null;       
        #endregion


        public UC_MedicineTypeAcinGrid(MedicineTypeAcinGridInitADO ado)
        {
            InitializeComponent();
            try
            {
                this.initADO = ado;
                this.gridView_CustomUnboundColumnData = ado.MedicineTypeAcinGrid_CustomUnboundColumnData;               
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        public object GetDataGridView()
        {
            object result = null;
            try
            {
                result = (List<HIS.Desktop.ADO.MedicineTypeAcinADO>)gridControlMedicineTypeAcin.DataSource;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
            return result;
        }


        private void ProcessColumn()
        {
            try
            {
                if (this.initADO.ListMedicineTypeAcinColumn != null && this.initADO.ListMedicineTypeAcinColumn.Count > 0)
                {
                    foreach (var item in this.initADO.ListMedicineTypeAcinColumn)
                    {
                        GridColumn col = gridViewMedicineTypeAcin.Columns.AddField(item.FieldName);
                        col.Visible = item.Visible;
                        col.VisibleIndex = item.VisibleIndex;
                        col.Width = item.ColumnWidth;
                        col.FieldName = item.FieldName;
                        col.OptionsColumn.AllowEdit = item.AllowEdit;
                        col.Caption = item.Caption;                      
                        if (item.UnboundColumnType != null && item.UnboundColumnType != UnboundColumnType.Bound)
                            col.UnboundType = item.UnboundColumnType;
                        if (item.Format != null)
                        {
                            col.DisplayFormat.FormatString = item.Format.FormatString;
                            col.DisplayFormat.FormatType = item.Format.FormatType;
                        }

                        col.OptionsColumn.ShowCaption = item.ShowCaption;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void gridViewMedicineTypeAcin_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound && e.ListSourceRowIndex >= 0)
                {
                    var data = (V_HIS_MEDICINE_TYPE_ACIN)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null && this.gridView_CustomUnboundColumnData != null)
                    {
                        this.gridView_CustomUnboundColumnData(data, e);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void Reload(List<HIS.Desktop.ADO.MedicineTypeAcinADO> data)
        {
            try
            {
                if (this.initADO != null)
                {
                    this.initADO.ListMedicineTypeAcinADO = data;
                    UpdateDataSource();                 
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UpdateDataSource()
        {
            try
            {
                gridControlMedicineTypeAcin.BeginUpdate();
                gridControlMedicineTypeAcin.DataSource = this.initADO.ListMedicineTypeAcinADO;
                gridControlMedicineTypeAcin.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMedicineTypeAcin_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                GridView View = sender as GridView;
                if (e.RowHandle >= 0)
                {
                    var data = (HIS.Desktop.ADO.MedicineTypeAcinADO)gridViewMedicineTypeAcin.GetRow(e.RowHandle);
                    if (data != null)
                    {
                        if (e.Column.FieldName == "check2")
                        {
                            if (data.isKeyChoose1)
                            {
                                e.RepositoryItem = CheckEA;
                            }
                            else
                            {
                                e.RepositoryItem = CheckDA;
                            }
                        }
                        if (e.Column.FieldName == "radio2")
                        {
                            if (data.isKeyChoose1)
                            {
                                e.RepositoryItem = RadioEA;
                            }
                            else
                            {
                                e.RepositoryItem = RadioDA;
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

        private void UC_MedicineTypeAcinGrid_Load(object sender, EventArgs e)
        {
            try
            {
                if (this.initADO != null)
                {
                    ProcessColumn();
                    UpdateDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
