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
using HIS.UC.ExpMestMaterialGrid.ADO;
using DevExpress.XtraGrid.Columns;
using DevExpress.Data;
using MOS.EFMODEL.DataModels;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;

namespace HIS.UC.ExpMestMaterialGrid
{
    public partial class UCExpMestMaterial : UserControl
    {
        ExpMestMaterialInitADO initADO = null;

        Grid_CustomUnboundColumnData gridView_CustomUnboundColumnData = null;

        bool isShowSearchPanel;

        public UCExpMestMaterial(ExpMestMaterialInitADO ado)
        {
            InitializeComponent();
            try
            {
                this.initADO = ado;
                this.gridView_CustomUnboundColumnData = ado.ExpMestMaterialGrid_CustomUnboundColumnData;
                if (ado.IsShowSearchPanel.HasValue)
                {
                    this.isShowSearchPanel = ado.IsShowSearchPanel.Value;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCExpMestMaterial_Load(object sender, EventArgs e)
        {
            try
            {
                if (this.initADO != null)
                {
                    ProcessColumn();
                    SetVisibleSearchPanel();
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
                gridControlExpMestMaterial.BeginUpdate();
                gridControlExpMestMaterial.DataSource = this.initADO.ListExpMestMaterial;
                gridControlExpMestMaterial.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessColumn()
        {
            try
            {
                if (this.initADO.ListExpMestMaterialColumn != null && this.initADO.ListExpMestMaterialColumn.Count > 0)
                {
                    foreach (var item in this.initADO.ListExpMestMaterialColumn)
                    {
                        GridColumn col = gridViewExpMestMaterial.Columns.AddField(item.FieldName);
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
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetVisibleSearchPanel()
        {
            try
            {
                if (this.isShowSearchPanel)
                {
                    layoutKeyword.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }
                else
                {
                    layoutKeyword.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestMaterial_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound && e.ListSourceRowIndex >= 0)
                {
                    var data = (V_HIS_EXP_MEST_MATERIAL)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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

        private void txtKeyword_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                SearchClick(strValue);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SearchClick(string strValue)
        {
            try
            {
                if (this.initADO != null && this.initADO.ListExpMestMaterial != null)
                {
                    List<V_HIS_EXP_MEST_MATERIAL> listData = new List<V_HIS_EXP_MEST_MATERIAL>();
                    gridControlExpMestMaterial.DataSource = null;
                    if (!String.IsNullOrEmpty(strValue.Trim()))
                    {
                        string key = strValue.Trim().ToLower();

                        listData = this.initADO.ListExpMestMaterial.Where(o => o.MATERIAL_TYPE_CODE.ToLower().Contains(key) || o.MATERIAL_TYPE_NAME.ToLower().Contains(key)).ToList();
                    }
                    else
                    {
                        listData = this.initADO.ListExpMestMaterial;
                    }
                    gridControlExpMestMaterial.BeginUpdate();
                    gridControlExpMestMaterial.DataSource = listData;
                    gridControlExpMestMaterial.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void Reload(List<V_HIS_EXP_MEST_MATERIAL> data)
        {
            try
            {
                if (this.initADO != null)
                {
                    this.initADO.ListExpMestMaterial = data;
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
