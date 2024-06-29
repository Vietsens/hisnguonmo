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
using HIS.UC.ImpMestType.ADO;
using HIS.UC.ImpMestType.Run;
using HIS.UC.ImpMestType.Reload;
using DevExpress.XtraGrid.Columns;
using DevExpress.Data;
using MOS.EFMODEL.DataModels;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using HIS.UC.ImpMestType.Popup;
using DevExpress.XtraBars;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

namespace HIS.UC.ImpMestType
{
    internal partial class  UCImpMestType : UserControl
    {
        #region Declare
        ImpMestTypeInitADO initADO = null;
        Grid_CustomUnboundColumnData gridView_CustomUnboundColumnData = null;
        btn_Radio_Enable_Click btnRadioEnable_Click = null;
        gridView_MouseDown gridView_MouseDown = null;

        GridView_MouseRightClick gridView_MouseRightClick = null;
        HIS.Desktop.ADO.ImpMestTypeADO currentAdo;
        DevExpress.XtraBars.BarManager barManager1;
        PopupMenuProcessor popupMenuProcessor;
        #endregion

        public UCImpMestType(ImpMestTypeInitADO ado)
        {
            InitializeComponent();
            try
            {
                this.initADO = ado;
                this.gridView_CustomUnboundColumnData = ado.ImpMestTypeGrid_CustomUnboundColumnData;
                this.btnRadioEnable_Click = ado.btnRadioEnable_Click;
                this.gridView_MouseDown = ado.gridView_MouseDown;
                this.gridView_MouseRightClick = ado.gridView_MouseRightClick;
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
                if (gridViewImpMestType.IsEditing)
                    gridViewImpMestType.CloseEditor();

                if (gridViewImpMestType.FocusedRowModified)
                    gridViewImpMestType.UpdateCurrentRow();

                result = (List<HIS.Desktop.ADO.ImpMestTypeADO>)gridControlImpMestType.DataSource;
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
                if (this.initADO.ListImpMestTypeColumn != null && this.initADO.ListImpMestTypeColumn.Count > 0)
                {
                    foreach (var item in this.initADO.ListImpMestTypeColumn)
                    {
                        GridColumn col = gridViewImpMestType.Columns.AddField(item.FieldName);
                        col.Visible = item.Visible;
                        col.VisibleIndex = item.VisibleIndex;
                        col.Width = item.ColumnWidth;
                        col.FieldName = item.FieldName;
                        col.OptionsColumn.AllowEdit = item.AllowEdit;
                        col.Caption = item.Caption;
                        if (item.image != null)
                        {
                            col.Image = item.image;
                            col.ImageAlignment = StringAlignment.Center;
                        }
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

        private void gridViewMedicineType_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound && e.ListSourceRowIndex >= 0)
                {
                    var data = (HIS.Desktop.ADO.ImpMestTypeADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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

        internal void Reload(List<HIS.Desktop.ADO.ImpMestTypeADO> data)
        {
            try
            {
                if (this.initADO != null)
                {
                    this.initADO.ListImpMestTypeADO = data;
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
                gridControlImpMestType.BeginUpdate();
                gridControlImpMestType.DataSource = this.initADO.ListImpMestTypeADO;
                gridControlImpMestType.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMedicineType_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                GridView View = sender as GridView;
                if (e.RowHandle >= 0)
                {
                    var data = (HIS.Desktop.ADO.ImpMestTypeADO)gridViewImpMestType.GetRow(e.RowHandle);
                    if (data != null)
                    {
                        if (e.Column.FieldName == "checkImpMestType")
                        {
                            if (data.isKeyChoose)
                            {
                                e.RepositoryItem = CheckD;
                            }
                            else
                            {
                                e.RepositoryItem = CheckE;
                            }
                        }
                        if (e.Column.FieldName == "radioImpMestType")
                        {
                            if (data.isKeyChoose)
                            {
                                e.RepositoryItem = RadioE;
                            }
                            else
                            {
                                e.RepositoryItem = RadioD;
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

        private void UC_MedicineTypeGrid_Load(object sender, EventArgs e)
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

        private void RadioEnable_Click(object sender, EventArgs e)
        {
            try
            {
                var row = (HIS_IMP_MEST_TYPE)gridViewImpMestType.GetFocusedRow();
                foreach (var item in this.initADO.ListImpMestTypeADO)
                {
                    if (item.ID == row.ID)
                    {
                        item.radioImpMestType = true;
                    }
                    else
                    {
                        item.radioImpMestType = false;
                    }
                }

                gridControlImpMestType.RefreshDataSource();

                if (row != null && this.btnRadioEnable_Click != null)
                {
                    this.btnRadioEnable_Click(row);
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMedicineType_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                var impMestTypeFocus = (HIS_IMP_MEST_TYPE)gridViewImpMestType.GetFocusedRow();
                if (impMestTypeFocus != null && gridView_MouseDown != null)
                {
                    this.gridView_MouseDown(sender, e);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            try
            {
                currentAdo = (HIS.Desktop.ADO.ImpMestTypeADO)gridViewImpMestType.GetFocusedRow();

                GridHitInfo hi = e.HitInfo;
                if (hi.InRowCell && this.gridView_MouseRightClick != null)
                {
                    if (this.barManager1 == null && this.gridView_MouseRightClick != null)
                        this.barManager1 = new DevExpress.XtraBars.BarManager();
                    this.barManager1.Form = this;
                    popupMenuProcessor = new PopupMenuProcessor(this.barManager1, GridView_MouseRightClick);
                    this.popupMenuProcessor.InitMenu();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GridView_MouseRightClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if ((e.Item is BarButtonItem) && this.currentAdo != null)
                {
                    var type = (PopupMenuProcessor.ItemType)e.Item.Tag;
                    switch (type)
                    {
                        case PopupMenuProcessor.ItemType.Copy:
                            {
                                this.gridView_MouseRightClick(this.currentAdo, e);
                                break;
                            }
                        case PopupMenuProcessor.ItemType.Paste:
                            {
                                this.gridView_MouseRightClick(this.currentAdo, e);
                                break;
                            }
                        default:
                            break;
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
