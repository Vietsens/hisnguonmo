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
using HIS.UC.ConflictActiveIngredient.ADO;
using DevExpress.XtraGrid.Columns;
using DevExpress.Data;
using MOS.EFMODEL.DataModels;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Desktop.CustomControl;
using DevExpress.XtraEditors.DXErrorProvider;

namespace HIS.UC.ConflictActiveIngredient
{
    public partial class UC_ConflictActiveIngredient : UserControl
    {
        ConflictActiveIngredientInitADO initADO = null;
        Grid_CustomUnboundColumnData gridView_CustomUnboundColumnData = null;
        Grid_CellValueChanged gridView_CellValueChanged = null;
        btn_Radio_Enable_Click1 btn_Radio_Enable_Click1 = null;
        repositoryItemButtonEdit_HuongXuLy_ButtonClick1 repositoryItemButtonEdit_HuongXuLy_ButtonClick1 = null;
        repositoryItemButtonEdit_CoChe_ButtonClick1 repositoryItemButtonEdit_CoChe_1 = null;
        repositoryItemButtonEdit_HauQua_ButtonClick1 repositoryItemButtonEdit_HauQua_1 = null;
        repositoryItemButtonEdit_CoChe_EditValueChanged1 repositoryItemButtonEdit_CoChe_EditValueChanged_1 = null;
        repositoryItemButtonEdit_HauQua_EditValueChanged1 repositoryItemButtonEdit_HauQua_EditValueChanged_1 = null;
        repositoryItemGrid repGrid = null;
        Grid_MouseDown gridView_MouseDown = null;
		private List<HIS_INTERACTIVE_GRADE> dtInteractiveGrade;

		public UC_ConflictActiveIngredient(ConflictActiveIngredientInitADO ado)
        {
            InitializeComponent();
            try
            {
                this.initADO = ado;
                this.gridView_CustomUnboundColumnData = ado.ConflictActiveIngredientGrid_CustomUnboundColumnData;
                btn_Radio_Enable_Click1 = ado.btn_Radio_Enable_Click1;
                //get_Value_Instruction = ado.get
                repositoryItemButtonEdit_HuongXuLy_ButtonClick1 = ado.repositoryItemButtonEdit_HuongXuLy_ButtonClick1;
                repositoryItemButtonEdit_CoChe_1 = ado.repositoryItemButtonEdit_CoChe_ButtonClick1;
                repositoryItemButtonEdit_HauQua_1 = ado.repositoryItemButtonEdit_HauQua_ButtonClick1;
                repositoryItemButtonEdit_CoChe_EditValueChanged_1 = ado.repositoryItemButtonEdit_CoChe_EditValueChanged1;
                repositoryItemButtonEdit_HauQua_EditValueChanged_1 = ado.repositoryItemButtonEdit_HauQua_EditValueChanged1;
                repGrid = ado.repositoryItemGrid;
                this.gridView_MouseDown = ado.ConflictActiveIngredientGrid_MouseDown;
                this.gridView_CellValueChanged = ado.ConflictActiveIngredientGrid_CellValueChanged;
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
                result = (List<HIS.UC.ConflictActiveIngredient.ConflictActiveIngredientADO>)gridControlConflictActiveIngredient.DataSource;
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
                result = this.gridControlConflictActiveIngredient;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
            return result;
        }

        private void UC_ConflictActiveIngredient_Load(object sender, EventArgs e)
        {
            try
            {
                if (this.initADO != null)
                {
                    initCombo();
                    ProcessColumn();
                    UpdateDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SetIsKeyChooseTrue()
        {
            try
            {
                gridViewConflictActiveIngredient.Columns[0].Visible = false;
                gridViewConflictActiveIngredient.Columns[1].Visible = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SetIsKeyChooseFalse()
        {
            try
            {
                gridViewConflictActiveIngredient.Columns[1].Visible = false;
                gridViewConflictActiveIngredient.Columns[0].Visible = true;
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
                gridControlConflictActiveIngredient.BeginUpdate();
                gridControlConflictActiveIngredient.DataSource = this.initADO.ListConflictActiveIngredient;
                gridControlConflictActiveIngredient.EndUpdate();
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
                if (this.initADO.ListConflictActiveIngredientColumn != null && this.initADO.ListConflictActiveIngredientColumn.Count > 0)
                {
                    foreach (var item in this.initADO.ListConflictActiveIngredientColumn)
                    {
                        GridColumn col = gridViewConflictActiveIngredient.Columns.AddField(item.FieldName);
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

        private void gridViewConflictActiveIngredient_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
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

        internal void Reload(List<ConflictActiveIngredientADO> data)
        {
            try
            {
                if (this.initADO != null)
                {
                    this.initADO.ListConflictActiveIngredient = data;
                    UpdateDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void initCombo()
		{
			try
			{
                dtInteractiveGrade = BackendDataWorker.Get<HIS_INTERACTIVE_GRADE>().Where(o=>o.IS_ACTIVE == 1).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("INTERACTIVE_GRADE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("INTERACTIVE_GRADE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("INTERACTIVE_GRADE", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(repositoryItemGridLookUpEdit1, dtInteractiveGrade.ToList(), controlEditorADO);
            }
			catch (Exception ex)
			{

				throw;
			}
		}
        private void gridViewConflictActiveIngredient_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                GridView View = sender as GridView;
                if (e.RowHandle >= 0)
                {
                    var data = (ConflictActiveIngredientADO)gridViewConflictActiveIngredient.GetRow(e.RowHandle);
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
                        if (e.Column.FieldName == "check2")
                        {
                            if (data.isKeyChoose1)
                            {
                                e.RepositoryItem = CheckDisable;
                            }
                            else
                            {
                                e.RepositoryItem = CheckEnable;

                            }
                        }
                        if (e.Column.FieldName == "radio2")
                        {
                            if (data.isKeyChoose1)
                            {
                                e.RepositoryItem = RadioEnable;
                            }
                            else
                            {
                                e.RepositoryItem = RadioDisable;
                            }

                        }

                        if (e.Column.FieldName == "INTERACTIVE_GRADE_ID")
                        {
                            e.RepositoryItem = repositoryItemGridLookUpEdit1;
                        }
                        if (e.Column.FieldName == "INSTRUCTION")
                        {
                            e.RepositoryItem = repositoryItemButtonEdit_HuongXuLy;
                        }
                        if (e.Column.FieldName == "MECHANISM")
                        {
                            e.RepositoryItem = repositoryItemButtonEdit_CoChe;
                        }
                        if (e.Column.FieldName == "CONSEQUENCE")
                        {
                            e.RepositoryItem = repositoryItemButtonEdit_HauQua;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void setValueColumn(string Fieldname, string data)
        {
            //try
            //{
            //    var dataGrid = (HIS_ACTIVE_INGREDIENT)gridViewConflictActiveIngredient.GetFocusedRow();
            //    dataGrid.. = data;
            //    gridViewConflictActiveIngredient.UpdateCurrentRow();
            //    gridControlConflictActiveIngredient.RefreshDataSource();
            //    //gridViewConflictActiveIngredient.SetFocusedRowCellValue(Fieldname, data);
            //}
            //catch (Exception ex)
            //{
            //       Inventec.Common.Logging.LogSystem.Error(ex);
            //}
        }

        private void RadioEnable_Click(object sender, EventArgs e)
        {
            try
            {
                var data = (ConflictActiveIngredientADO)gridViewConflictActiveIngredient.GetFocusedRow();
                var row = (HIS_ACTIVE_INGREDIENT)gridViewConflictActiveIngredient.GetFocusedRow();
                foreach (var item in this.initADO.ListConflictActiveIngredient)
                {
                    if (item.ID == row.ID)
                    {
                        item.radio2 = true;
                    }
                    else
                    {
                        item.radio2 = false;
                    }
                }

                gridControlConflictActiveIngredient.RefreshDataSource();

                if (row != null && this.btn_Radio_Enable_Click1 != null)
                {
                    this.btn_Radio_Enable_Click1(row, data);
                }
                gridViewConflictActiveIngredient.LayoutChanged();
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

        private void gridViewConflictActiveIngredient_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                try
                {
                    var ado = (ConflictActiveIngredientADO)this.gridViewConflictActiveIngredient.GetFocusedRow();
                    if (ado != null)
                    {
                        if (e.Column.FieldName == "CONSEQUENCE"
                            || e.Column.FieldName == "MECHANISM"
                            )
                        {
                            Valid(ado);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

		private void Valid(ConflictActiveIngredientADO conflictActiveIngredientADO)
		{
            try
            {
                if (conflictActiveIngredientADO != null)
                {
                    if (!String.IsNullOrEmpty(conflictActiveIngredientADO.CONSEQUENCE) && Encoding.UTF8.GetByteCount((conflictActiveIngredientADO.CONSEQUENCE)) > 1000)
                    {

                        conflictActiveIngredientADO.ErrorMessageConsequence = "Vượt quá độ dài cho phép 1000 ký tự";
                        conflictActiveIngredientADO.ErrorTypeConsequence = ErrorType.Warning;
                    }
                    else
                    {
                        conflictActiveIngredientADO.ErrorMessageConsequence = "";
                        conflictActiveIngredientADO.ErrorTypeConsequence = ErrorType.None;
                    }
                    if (!String.IsNullOrEmpty(conflictActiveIngredientADO.MECHANISM) && Encoding.UTF8.GetByteCount(conflictActiveIngredientADO.MECHANISM) > 1000)
                    {

                        conflictActiveIngredientADO.ErrorMessageMechanism = "Vượt quá độ dài cho phép 1000 ký tự";
                        conflictActiveIngredientADO.ErrorTypeMechanism = ErrorType.Warning;
                    }
                    else
                    {
                        conflictActiveIngredientADO.ErrorMessageMechanism = "";
                        conflictActiveIngredientADO.ErrorTypeMechanism = ErrorType.None;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

		private void gridViewConflictActiveIngredient_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                try
                {
                    var row = (HIS_ACTIVE_INGREDIENT)gridViewConflictActiveIngredient.GetFocusedRow();
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
                //var data = (ConflictActiveIngredentADO)gridViewConflictActiveIngredient.GetRow(0);
                //if (data != null && (bool)data.isKeyChoose == false)
                //{
                //    if (CheckAll1.Checked == true && CheckAll1.Enabled == true)
                //    {
                //        for (int i = 0; i < gridViewConflictActiveIngredient.RowCount; i++)
                //        {

                //            gridViewConflictActiveIngredient.SetRowCellValue(i, "check1", true);

                //        }
                //    }
                //    else if (CheckAll1.Checked == false)
                //    {
                //        for (int i = 0; i < gridViewConflictActiveIngredient.RowCount; i++)
                //        {

                //            gridViewConflictActiveIngredient.SetRowCellValue(i, "check1", false);

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

        private void gridViewConflictActiveIngredient_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {
                GridView View = sender as GridView;
                if (e.RowHandle >= 0)
                {
                    var data = (ConflictActiveIngredientADO)gridViewConflictActiveIngredient.GetRow(e.RowHandle);
                    if (data != null && !data.isKeyChoose1)
                    {

                        var color = dtInteractiveGrade.Find(o => o.ID == data.INTERACTIVE_GRADE_ID);
                        if (color != null)
                        {
                            if (color.INTERACTIVE_GRADE == 1)
                                e.Appearance.ForeColor = Color.Green;
                            else if (color.INTERACTIVE_GRADE == 2)
                                e.Appearance.ForeColor = Color.Orange;
                            else if (color.INTERACTIVE_GRADE > 2)
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

        private void repositoryItemButtonEdit_HuongXuLy_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var data = (ConflictActiveIngredientADO)gridViewConflictActiveIngredient.GetFocusedRow();
                var row = (HIS_ACTIVE_INGREDIENT)gridViewConflictActiveIngredient.GetFocusedRow();

                //if (row != null && data != null && this.repositoryItemButtonEdit_HuongXuLy_ButtonClick1 != null)
                //{
                //    this.repositoryItemButtonEdit_HuongXuLy_ButtonClick1(sender,row, data);
                //}
                //ktxtHuongXuLy.Text = row.INSTRUCTION;
                if (data != null && row != null && repositoryItemButtonEdit_HuongXuLy_ButtonClick1 != null)
                {
                    repositoryItemButtonEdit_HuongXuLy_ButtonClick1(sender, row, data);
                }


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

		private void repositoryItemButtonEdit_CoChe_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
		{
            try
            {
                var data = (ConflictActiveIngredientADO)gridViewConflictActiveIngredient.GetFocusedRow();
                var row = (HIS_ACTIVE_INGREDIENT)gridViewConflictActiveIngredient.GetFocusedRow();
                if (data != null && row != null && repositoryItemButtonEdit_CoChe_1 != null)
                {
                    repositoryItemButtonEdit_CoChe_1(sender, row, data);
                }


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

		private void repositoryItemButtonEdit_HauQua_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
		{
            try
            {
                var data = (ConflictActiveIngredientADO)gridViewConflictActiveIngredient.GetFocusedRow();
                var row = (HIS_ACTIVE_INGREDIENT)gridViewConflictActiveIngredient.GetFocusedRow();
                if (data != null && row != null && repositoryItemButtonEdit_HauQua_1 != null)
                {
                    repositoryItemButtonEdit_HauQua_1(sender, row, data);
                }


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

		private void gridViewConflictActiveIngredient_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
		{
		
		}

		private void gridViewConflictActiveIngredient_CustomRowColumnError(object sender, Inventec.Desktop.CustomControl.RowColumnErrorEventArgs e)
		{
            try
            {
                if (e.ColumnName == "CONSEQUENCE" || e.ColumnName == "MECHANISM")
                {
                    this.gridView_CustomRowError(sender, e);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

		private void gridView_CustomRowError(object sender, RowColumnErrorEventArgs e)
		{

            try
            {
                var index = this.gridViewConflictActiveIngredient.GetDataSourceRowIndex(e.RowHandle);
                if (index < 0)
                {
                    e.Info.ErrorType = ErrorType.None;
                    e.Info.ErrorText = "";
                    return;
                }
                var listDatas = this.gridControlConflictActiveIngredient.DataSource as List<ConflictActiveIngredientADO>;
                var row = listDatas[index];
                if (e.ColumnName == "CONSEQUENCE")
                {
                    if (row.ErrorTypeConsequence == ErrorType.Warning)
                    {
                        e.Info.ErrorType = (ErrorType)(row.ErrorTypeConsequence);
                        e.Info.ErrorText = (string)(row.ErrorMessageConsequence);
                    }
                    else
                    {
                        e.Info.ErrorType = (ErrorType)(ErrorType.None);
                        e.Info.ErrorText = "";
                    }
                }
                else if (e.ColumnName == "MECHANISM")
                {
                    if (row.ErrorTypeMechanism == ErrorType.Warning)
                    {
                        e.Info.ErrorType = (ErrorType)(row.ErrorTypeMechanism);
                        e.Info.ErrorText = (string)(row.ErrorMessageMechanism);
                    }
                    else
                    {
                        e.Info.ErrorType = (ErrorType)(ErrorType.None);
                        e.Info.ErrorText = "";
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

		private void repositoryItemButtonEdit_CoChe_EditValueChanged(object sender, EventArgs e)
		{
			
		}

		private void repositoryItemButtonEdit_HauQua_EditValueChanged(object sender, EventArgs e)
		{
            
        }

		private void repositoryItemButtonEdit_CoChe_Leave(object sender, EventArgs e)
		{
            try
            {
                var data = (ConflictActiveIngredientADO)gridViewConflictActiveIngredient.GetFocusedRow();
                var row = (HIS_ACTIVE_INGREDIENT)gridViewConflictActiveIngredient.GetFocusedRow();
                if (data != null && row != null && repositoryItemButtonEdit_CoChe_EditValueChanged_1 != null)
                {
                    repositoryItemButtonEdit_CoChe_EditValueChanged_1(sender, row, data);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

		private void repositoryItemButtonEdit_HauQua_Leave(object sender, EventArgs e)
		{
            try
            {
                var data = (ConflictActiveIngredientADO)gridViewConflictActiveIngredient.GetFocusedRow();
                var row = (HIS_ACTIVE_INGREDIENT)gridViewConflictActiveIngredient.GetFocusedRow();
                if (data != null && row != null && repositoryItemButtonEdit_HauQua_EditValueChanged_1 != null)
                {
                    repositoryItemButtonEdit_HauQua_EditValueChanged_1(sender, row, data);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
	}
}
