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

namespace Inventec.Desktop.CustomControl
{
    public class CustomGridViewWithFilterMultiColumn : GridView, IGridLookUp
    {       
        public CustomGridViewWithFilterMultiColumn() : base() { }

        protected internal virtual void SetGridControlAccessMetod(GridControl newControl)
        {
            SetGridControl(newControl);
        }

        void IGridLookUp.Show(object editValue, string filterText)
        {
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
            LogSystem.Log("FocusedRowHandle=" + FocusedRowHandle);
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

        protected override string ViewName { get { return "CustomGridViewWithFilterMultiColumn"; } }
        protected virtual internal string GetExtraFilterText { get { return ExtraFilterText; } }
    }

    public class CustomGridControlWithFilterMultiColumn : GridControl
    {
        public CustomGridControlWithFilterMultiColumn()
            : base()
        {
        }

        protected override void RegisterAvailableViewsCore(InfoCollection collection)
        {
            base.RegisterAvailableViewsCore(collection);
            collection.Add(new CustomGridInfoRegistrator());
        }

        protected override BaseView CreateDefaultView()
        {
            return CreateView("CustomGridViewWithFilterMultiColumn");
        }

    }

    public class CustomGridPainter : GridPainter
    {
        public CustomGridPainter(GridView view) : base(view) { }

        public virtual new CustomGridViewWithFilterMultiColumn View { get { return (CustomGridViewWithFilterMultiColumn)base.View; } }

        protected override void DrawRowCell(GridViewDrawArgs e, GridCellInfo cell)
        {
            cell.ViewInfo.MatchedStringUseContains = true;
            cell.ViewInfo.MatchedString = View.GetExtraFilterText;
            cell.State = GridRowCellState.Dirty;
            e.ViewInfo.UpdateCellAppearance(cell);
            base.DrawRowCell(e, cell);
        }
    }

    public class CustomGridInfoRegistrator : GridInfoRegistrator
    {
        public CustomGridInfoRegistrator() : base() { }
        public override BaseViewPainter CreatePainter(BaseView view) { return new CustomGridPainter(view as DevExpress.XtraGrid.Views.Grid.GridView); }
        public override string ViewName { get { return "CustomGridViewWithFilterMultiColumn"; } }
        public override BaseView CreateView(GridControl grid)
        {
            CustomGridViewWithFilterMultiColumn view = new CustomGridViewWithFilterMultiColumn();
            view.SetGridControlAccessMetod(grid);
            return view;
        }

    }

}
