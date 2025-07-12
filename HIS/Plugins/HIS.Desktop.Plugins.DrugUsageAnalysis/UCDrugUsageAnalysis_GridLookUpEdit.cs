using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.Utility;
using System;

namespace HIS.Desktop.Plugins.DrugUsageAnalysis
{
    public partial class UCDrugUsageAnalysis : UserControlBase
    {
        private void InitCombo(
            GridLookUpEdit cbo, 
            object data, 
            string displayMember, 
            string valueMember,
            GridCheckMarksSelection.SelectionChangedEventHandler eventHandlerMarksSelection,
            DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler eventHandlerCustomDisplayText
            )
        {
            try
            {
                // Marks selection
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cbo.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(eventHandlerMarksSelection);
                cbo.Properties.Tag = gridCheck;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;
                //
                cbo.Properties.View.ColumnFilterChanged += (s, e) =>
                {
                    var view = s as DevExpress.XtraGrid.Views.Grid.GridView;
                    if (view == null) return;

                    // Lấy filter text của cột đầu tiên có filter (hoặc tuỳ ý)
                    string filterText = null;
                    foreach (var col in view.Columns)
                    {
                        var column = col as DevExpress.XtraGrid.Columns.GridColumn;
                        if (column != null && !string.IsNullOrEmpty(column.FilterInfo?.Value as string))
                        {
                            filterText = column.FilterInfo.Value as string;
                            break;
                        }
                    }
                    // Gán filterText cho FindPanelText để highlight
                    view.ApplyFindFilter(!string.IsNullOrEmpty(filterText) ? $"\"{filterText}\"" : string.Empty);
                };
                // Combo properties
                cbo.Properties.Closed += (s, e) =>
                {
                    GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                    cbo.Properties.Buttons[1].Visible = gridCheckMark != null && gridCheckMark.Selection.Count > 0;
                    var view = cbo.Properties.View;
                    if (view != null)
                    {
                        view.ClearColumnsFilter();
                        view.ApplyFindFilter(string.Empty);
                    }
                };
                cbo.Properties.View.CustomDrawCell += View_CustomDrawCell_ShowPlaceholder;
                cbo.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(eventHandlerCustomDisplayText);
                cbo.Properties.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboProperties_ButtonClick);
                cbo.Properties.DataSource = data;
                cbo.Properties.DisplayMember = displayMember;
                cbo.Properties.ValueMember = valueMember;
                if (cbo.Properties.View.Columns.Count > 0)
                {
                    var checkCol = cbo.Properties.View.Columns[0];
                    checkCol.Width = 30;
                    checkCol.MinWidth = 30;
                    checkCol.MaxWidth = 30;
                    checkCol.OptionsColumn.FixedWidth = true;
                }
                DevExpress.XtraGrid.Columns.GridColumn col2 = cbo.Properties.View.Columns.AddField(displayMember);
                col2.VisibleIndex = 2;
                col2.Width = 325;
                col2.Caption = "Chọn tất cả";
                col2.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
                cbo.Properties.PopupFormWidth = 350;
                cbo.Properties.View.OptionsView.ShowColumnHeaders = true;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;
                cbo.Properties.View.OptionsView.ShowAutoFilterRow = true;
                cbo.Properties.View.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
                cbo.Properties.View.BestFitColumns();
                // Clear selection
                this.cboClearSelection(cbo);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void View_CustomDrawCell_ShowPlaceholder(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            if (view == null) return;
            if (e.RowHandle == DevExpress.XtraGrid.GridControl.AutoFilterRowHandle)
            {
                var filterValue = view.GetRowCellValue(e.RowHandle, e.Column);
                if (filterValue == null || string.IsNullOrEmpty(filterValue.ToString()))
                {
                    e.DisplayText = "Từ khóa tìm kiếm ...";
                    e.Appearance.ForeColor = System.Drawing.Color.Gray;
                }
            }
        }
        private void cboClearSelection(GridLookUpEdit gridLookUpEdit)
        {
            try
            {
                GridCheckMarksSelection gridCheckMark = gridLookUpEdit.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(gridLookUpEdit.Properties.View);
                }
                if (gridLookUpEdit.Properties.Buttons.Count > 0)
                {
                    foreach (EditorButton item in gridLookUpEdit.Properties.Buttons)
                    {
                        if (item != null && item.Kind == ButtonPredefines.Delete)
                        {
                            item.Visible = false;
                        }
                    }
                }
                gridLookUpEdit.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboProperties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    var cbo = sender as DevExpress.XtraEditors.GridLookUpEdit;
                    this.cboClearSelection(cbo);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
