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
using HIS.UC.ActiveIngredent.ADO;
using DevExpress.XtraGrid.Columns;
using DevExpress.Data;
using MOS.EFMODEL.DataModels;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;

namespace HIS.UC.ActiveIngredent
{
    public partial class UC_ActiveIngredent : UserControl
    {
        ActiveIngredentInitADO initADO = null;
        Grid_CustomUnboundColumnData gridView_CustomUnboundColumnData = null;
        Grid_CellValueChanged gridView_CellValueChanged = null;
        btn_Radio_Enable_Click btn_Radio_Enable_Click = null;
        Grid_MouseDown gridView_MouseDown = null;

        public UC_ActiveIngredent(ActiveIngredentInitADO ado)
        {
            InitializeComponent();
            try
            {
                this.initADO = ado;
                this.gridView_CustomUnboundColumnData = ado.ActiveIngredentGrid_CustomUnboundColumnData;
                btn_Radio_Enable_Click = ado.btn_Radio_Enable_Click;
                this.gridView_MouseDown = ado.ActiveIngredentGrid_MouseDown;
                this.gridView_CellValueChanged = ado.ActiveIngredentGrid_CellValueChanged;
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
                result = (List<HIS.UC.ActiveIngredent.ActiveIngredentADO>)gridControlActiveIngredent.DataSource;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
            return result;
        }

        public object GetGridControl()
        {
            object result = null;
            try
            {
                result = this.gridControlActiveIngredent;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
            return result;
        }

        private void UC_ActiveIngredent_Load(object sender, EventArgs e)
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

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.UC.ActiveIngredent.Resources.Lang", typeof(HIS.UC.ActiveIngredent.UC_ActiveIngredent).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UC_ActiveIngredent.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        public void SetIsKeyChooseTrue()
        {
            try
            {
                gridViewActiveIngredent.Columns[0].Visible = false;
                gridViewActiveIngredent.Columns[1].Visible = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //public void SetIsKeyChooseFalse()
        //{
        //    try
        //    {
        //        gridViewActiveIngredent.Columns[1].Visible = false;
        //        gridViewActiveIngredent.Columns[0].Visible = true;

        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}
        private void UpdateDataSource()
        {
            try
            {
                gridControlActiveIngredent.BeginUpdate();
                gridControlActiveIngredent.DataSource = this.initADO.ListActiveIngredent;
                gridControlActiveIngredent.EndUpdate();
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
                if (this.initADO.ListActiveIngredentColumn != null && this.initADO.ListActiveIngredentColumn.Count > 0)
                {
                    foreach (var item in this.initADO.ListActiveIngredentColumn)
                    {
                        GridColumn col = gridViewActiveIngredent.Columns.AddField(item.FieldName);
                        col.Visible = item.Visible;
                        col.VisibleIndex = item.VisibleIndex;
                        col.Width = item.ColumnWidth;
                        col.Image = item.image;
                        col.ImageAlignment = item.imageAlignment;
                        col.FieldName = item.FieldName;
                        col.OptionsColumn.AllowEdit = item.AllowEdit;
                        col.Caption = item.Caption;
                        col.ToolTip = item.ToolTip;
                        //col.Visible=visible;
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

        private void gridViewActiveIngredent_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound && e.ListSourceRowIndex >= 0)
                {
                    var data = (HIS_ACTIVE_INGREDIENT)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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

        internal void Reload(List<ActiveIngredentADO> data)
        {
            try
            {
                if (this.initADO != null)
                {
                    this.initADO.ListActiveIngredent = data;
                    UpdateDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void gridViewActiveIngredent_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                GridView View = sender as GridView;
                if (e.RowHandle >= 0)
                {
                    var data = (ActiveIngredentADO)gridViewActiveIngredent.GetRow(e.RowHandle);
                    if (data != null)
                    {
                        //if (data.isKeyChoose)
                        //{
                        //    CheckAll1.Enabled = false;
                        //}
                        //else
                        //{
                        //    CheckAll1.Enabled = true;
                        //}
                        if (e.Column.FieldName == "check1")
                        {
                            if (data.isKeyChoose)
                            {
                                e.RepositoryItem = CheckDisable;
                            }
                            else
                            {
                                e.RepositoryItem = CheckEnable;

                            }
                        }
                        if (e.Column.FieldName == "radio1")
                        {
                            if (data.isKeyChoose)
                            {
                                e.RepositoryItem = RadioEnable;
                            }
                            else
                            {
                                e.RepositoryItem = RadioDisable;
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

        private void RadioEnable_Click(object sender, EventArgs e)
        {
            try
            {
                var data = (ActiveIngredentADO)gridViewActiveIngredent.GetFocusedRow();
                var row = (HIS_ACTIVE_INGREDIENT)gridViewActiveIngredent.GetFocusedRow();
                foreach (var item in this.initADO.ListActiveIngredent)
                {
                    if (item.ID == row.ID)
                    {
                        item.radio1 = true;
                    }
                    else
                    {
                        item.radio1 = false;
                    }
                }

                gridControlActiveIngredent.RefreshDataSource();

                if (row != null && this.btn_Radio_Enable_Click != null)
                {
                    this.btn_Radio_Enable_Click(row,data);
                }
                gridViewActiveIngredent.LayoutChanged();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CheckEnable_Click(object sender, EventArgs e)
        {
        }

        private void CheckEnable_CheckedChanged(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewActiveIngredent_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                //state = Inventec.Common.TypeConvert.Parse.ToBoolean((gridViewActiveIngredent.GetRowCellValue(e.RowHandle, "check1") ?? "").ToString());
                //listState = new List<bool>();
                //listState.Add(state);
                //var data = (ActiveIngredentADO)gridViewActiveIngredent.GetFocusedRow();
                //if (data != null && data is ActiveIngredentADO)
                //{
                //    this.gridView_CellValueChanged(data, e);
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewActiveIngredent_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                try
                {
                    var row = (HIS_ACTIVE_INGREDIENT)gridViewActiveIngredent.GetFocusedRow();
                    if (row != null && this.gridView_MouseDown != null)
                    {
                        this.gridView_MouseDown(sender, e);
                    }
                }
                catch (Exception ex)
                {

                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CheckAll1_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                //var data = (ActiveIngredentADO)gridViewActiveIngredent.GetRow(0);
                //if (data != null && (bool)data.isKeyChoose == false)
                //{
                //    if (CheckAll1.Checked == true && CheckAll1.Enabled == true)
                //    {
                //        for (int i = 0; i < gridViewActiveIngredent.RowCount; i++)
                //        {

                //            gridViewActiveIngredent.SetRowCellValue(i, "check1", true);

                //        }
                //    }
                //    else if (CheckAll1.Checked == false)
                //    {
                //        for (int i = 0; i < gridViewActiveIngredent.RowCount; i++)
                //        {

                //            gridViewActiveIngredent.SetRowCellValue(i, "check1", false);

                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CheckAll1_Click(object sender, EventArgs e)
        {
        }
    }
}
