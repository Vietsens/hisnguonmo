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
using System.Text;
using DevExpress.XtraGrid.Columns;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Data.Filtering;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Registrator;
using DevExpress.Data;
using DevExpress.XtraEditors.Controls;

namespace Inventec.Desktop.CustomControl.NoFocus
{
    public class CustomGridViewWithFilterMultiColumnNoFocus : GridView, IGridLookUp
    {
        public CustomGridViewWithFilterMultiColumnNoFocus() : base() { }

        protected internal virtual void SetGridControlAccessMetod(GridControl newControl)
        {
            SetGridControl(newControl);
        }

        void IGridLookUp.Show(object editValue, string filterText)
        {
            //LogSystem.Log("editValue=" + editValue + ", filterText=" + filterText + ", FocusedRowHandle=" + FocusedRowHandle);
            if (editValue == null || String.IsNullOrEmpty(filterText))
            {
                FocusedRowHandle = -1;
                MakeRowVisible(FocusedRowHandle, false);
                return;
            }

            System.Reflection.FieldInfo field = typeof(GridView).GetField("firstMouseEnter", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            field.SetValue(this, true);
            if (LookUpOwner == null) return;
            if (Columns.Count == 0 && AllowAutoPopulateColumns) PopulateColumns();
            ((IGridLookUp)this).SetDisplayFilter(filterText);
            if (LookUpOwner.TextEditStyle == TextEditStyles.DisableTextEditor && RowCount == 0 && ExtraFilterText != string.Empty) ((IGridLookUp)this).SetDisplayFilter(string.Empty);
            int rowHandle = DataController.FindRowByValue(LookUpOwner.ValueMember, editValue, delegate(object args)
            {
                int row = (int)args;
                if (row >= 0)
                {
                    FocusedRowHandle = row;
                    MakeRowVisible(FocusedRowHandle, false);
                }
                else
                {
                    FocusedRowHandle = InvalidRowHandle;
                }
            });
            if (rowHandle == AsyncServerModeDataController.OperationInProgress)
            {
                rowHandle = FocusedRowHandle;
            }
            if (!this.IsValidRowHandle(rowHandle))
                rowHandle = 0;
            BeginUpdate();
            try
            {
                TopRowIndex = 0;
                FocusedRowHandle = rowHandle;
            }
            finally
            {
                EndUpdate();
            }
            MakeRowVisible(FocusedRowHandle, false);
        }

        protected override string OnCreateLookupDisplayFilter(string text, string displayMember)
        {
            List<CriteriaOperator> subStringOperators = new List<CriteriaOperator>();
            foreach (string sString in text.Split(' '))
            {
                string exp = LikeData.CreateContainsPattern(sString);
                List<CriteriaOperator> columnsOperators = new List<CriteriaOperator>();
                foreach (GridColumn col in Columns)
                {
                    if (col.ColumnType == typeof(string))//if (col.Visible && col.ColumnType == typeof(string))
                        columnsOperators.Add(new BinaryOperator(col.FieldName, exp, BinaryOperatorType.Like));
                }
                subStringOperators.Add(new GroupOperator(GroupOperatorType.Or, columnsOperators));
            }
            return new GroupOperator(GroupOperatorType.And, subStringOperators).ToString();
        }

        protected override void OnApplyColumnsFilterComplete()
        {
            base.OnApplyColumnsFilterComplete();

            if (true)
            {
                //GridControl.BeginInvoke((Action)(() => MoveTo(0)));
            }
        }

        protected override string ViewName { get { return "CustomGridViewWithFilterMultiColumnNoFocus"; } }
        protected virtual internal string GetExtraFilterText { get { return ExtraFilterText; } }
    }

    public class CustomGridControlWithFilterMultiColumnNoFocus : GridControl
    {
        public CustomGridControlWithFilterMultiColumnNoFocus()
            : base()
        {
        }

        protected override void RegisterAvailableViewsCore(InfoCollection collection)
        {
            base.RegisterAvailableViewsCore(collection);
            collection.Add(new CustomGridInfoRegistratorNoFocus());
        }

        protected override BaseView CreateDefaultView()
        {
            return CreateView("CustomGridViewWithFilterMultiColumnNoFocus");
        }

    }

    public class CustomGridPainterNoFocus : GridPainter
    {
        public CustomGridPainterNoFocus(GridView view) : base(view) { }

        public virtual new CustomGridViewWithFilterMultiColumnNoFocus View { get { return (CustomGridViewWithFilterMultiColumnNoFocus)base.View; } }

        protected override void DrawRowCell(GridViewDrawArgs e, GridCellInfo cell)
        {
            cell.ViewInfo.MatchedStringUseContains = true;
            cell.ViewInfo.MatchedString = View.GetExtraFilterText;
            cell.State = GridRowCellState.Dirty;
            e.ViewInfo.UpdateCellAppearance(cell);
            base.DrawRowCell(e, cell);
        }
    }

    public class CustomGridInfoRegistratorNoFocus : GridInfoRegistrator
    {
        public CustomGridInfoRegistratorNoFocus() : base() { }
        public override BaseViewPainter CreatePainter(BaseView view) { return new CustomGridPainterNoFocus(view as DevExpress.XtraGrid.Views.Grid.GridView); }
        public override string ViewName { get { return "CustomGridViewWithFilterMultiColumnNoFocus"; } }
        public override BaseView CreateView(GridControl grid)
        {
            CustomGridViewWithFilterMultiColumnNoFocus view = new CustomGridViewWithFilterMultiColumnNoFocus();
            view.SetGridControlAccessMetod(grid);
            return view;
        }

    }

}
