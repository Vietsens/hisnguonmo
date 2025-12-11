using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.Plugins.HisExecuteRoom.ToolTipService;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Markup;
using static HIS.Desktop.Plugins.HisExecuteRoom.RoomConfigOption.RoomConfigOption;

namespace HIS.Desktop.Plugins.HisExecuteRoom.HisExecuteRoom
{

    public partial class frmHisExecuteRoom : HIS.Desktop.Utility.FormBase
    {
        private readonly GridBubbleToolTipService _toolTipService = new GridBubbleToolTipService();
        private bool _toolTipAttached;
        private void InitCombo(
            GridLookUpEdit cbo, 
            object data, 
            string displayMember, 
            string valueMember,
            GridCheckMarksSelection.SelectionChangedEventHandler eventHandlerMarksSelection,
            DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler eventHandlerCustomDisplayText,
            DevExpress.XtraGrid.Views.Grid.RowClickEventHandler eventHandlerRowClick,
            DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler eventHandlerRowStyle
            )
        {
            try
            {
                //cbo.Properties.View.RowClick += (s, e) =>
                //{
                //    try
                //    {
                //        var view = s as DevExpress.XtraGrid.Views.Grid.GridView;
                //        if (view == null) return;
                //        RoomOptionItem row = view.GetRow(e.RowHandle) as RoomOptionItem;
                //        if (row == null) return;
                //        if (view.FocusedColumn.FieldName == "CheckMarkSelection")
                //        {
                //            if (row.Option == Option.MustBeApprovedSurgery)
                //            {
                //                if (SelectedOptions == null || !SelectedOptions.Any(o => o.Option == RoomConfigOption.RoomConfigOption.Option.IsSurgery))
                //                {
                //                    GridCheckMarksSelection gridCheckMark = cboDepartment.Properties.Tag as GridCheckMarksSelection;
                //                    if (gridCheckMark != null)
                //                    {
                //                        gridCheckMark.SelectRow(view, e.RowHandle, false);
                //                    }
                //                }
                //            }
                //            else if (row.Option == Option.IsSurgery)
                //            {
                //                if (SelectedOptions != null 
                //                && SelectedOptions.Any(o => o.Option == RoomConfigOption.RoomConfigOption.Option.MustBeApprovedSurgery)
                //                && !SelectedOptions.Any(o => o.Option == RoomConfigOption.RoomConfigOption.Option.IsSurgery)
                //                )
                //                {
                //                    GridCheckMarksSelection gridCheckMark = cboDepartment.Properties.Tag as GridCheckMarksSelection;
                //                    if (gridCheckMark != null)
                //                    {
                //                        for (int i = 0; i < view.DataRowCount; i++)
                //                        {
                //                            var r = view.GetRow(i) as RoomOptionItem;
                //                            if (r != null && r.Option == RoomConfigOption.RoomConfigOption.Option.MustBeApprovedSurgery)
                //                            {
                //                                gridCheckMark.SelectRow(view, i, false);
                //                                break;
                //                            }
                //                        }
                //                    }
                //                }
                //            }
                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //        Inventec.Common.Logging.LogSystem.Warn(ex);
                //    }
                //};

                //cbo.Properties.View.RowStyle += (s, e) =>
                //{
                //    try
                //    {
                //        var view = s as DevExpress.XtraGrid.Views.Grid.GridView;
                //        if (view == null) return;
                //        RoomOptionItem row = view.GetRow(e.RowHandle) as RoomOptionItem;
                //        if (row == null) return;
                //        if (row != null && row.Option == Option.MustBeApprovedSurgery)
                //        {
                //            if (SelectedOptions == null || !SelectedOptions.Any(o => o.Option == RoomConfigOption.RoomConfigOption.Option.IsSurgery))
                //            {
                //                e.Appearance.ForeColor = System.Drawing.Color.Gray;
                //                e.Appearance.BackColor = System.Drawing.Color.WhiteSmoke;
                //            }
                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //        Inventec.Common.Logging.LogSystem.Warn(ex);
                //    }
                //};

                // Marks selection
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cbo.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(eventHandlerMarksSelection);
                cbo.Properties.Tag = gridCheck;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;
                // attach external handlers
                cbo.Properties.View.RowClick += eventHandlerRowClick;
                cbo.Properties.View.RowStyle += eventHandlerRowStyle;
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
                    //cbo.Properties.Buttons[1].Visible = gridCheckMark != null && gridCheckMark.Selection.Count > 0;
                    if (cbo.Properties.Buttons.Count > 0 && gridCheckMark != null && gridCheckMark.Selection.Count > 0)
                    {
                        foreach (EditorButton item in cbo.Properties.Buttons)
                        {
                            if (item != null && item.Kind == ButtonPredefines.Delete)
                            {
                                item.Visible = true;
                            }
                        }
                    }
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
                cbo.Properties.View.OptionsView.RowAutoHeight = true;
                if (cbo.Properties.View.Columns.Count > 0)
                {
                    var checkCol = cbo.Properties.View.Columns[0];
                    checkCol.Width = 30;
                    checkCol.MinWidth = 30;
                    checkCol.MaxWidth = 30;
                    checkCol.OptionsColumn.FixedWidth = true;
                }
                var memoEdit = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
                memoEdit.WordWrap = true;
                DevExpress.XtraGrid.Columns.GridColumn col2 = cbo.Properties.View.Columns.AddField(displayMember);
                col2.VisibleIndex = 2;
                col2.Width = 475;
                col2.Caption = "Thiết lập chi tiết";
                col2.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
                col2.ColumnEdit = memoEdit;
                col2.AppearanceCell.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;


                cbo.Properties.PopupFormWidth = 500;
                cbo.Properties.View.OptionsView.ShowColumnHeaders = true;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;
                cbo.Properties.View.OptionsView.ShowAutoFilterRow = true;
                cbo.Properties.View.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
                cbo.Properties.View.BestFitColumns();
                // Clear selection
                this.cboClearSelection(cbo);
                //
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
            else
            {
                if (e.Column.FieldName == "CheckMarkSelection")
                {
                    var row = view.GetRow(e.RowHandle) as RoomOptionItem;
                    // Điều kiện để Indeterminate
                    if (row != null)
                    {
                        if (row != null && row.Option == Option.MustBeApprovedSurgery)
                        {
                            if (SelectedOptions == null || !SelectedOptions.Any(o => o.Option == RoomConfigOption.RoomConfigOption.Option.IsSurgery))
                            {
                                int boxSize = 12; // Đặt kích thước mong muốn
                                int x = e.Bounds.Left + (e.Bounds.Width - boxSize) / 2;
                                int y = e.Bounds.Top + (e.Bounds.Height - boxSize) / 2;
                                var boxRect = new System.Drawing.Rectangle(x, y, boxSize, boxSize);
                                // Vẽ nền xám nhạt cho ô vuông nhỏ
                                e.Graphics.FillRectangle(System.Drawing.Brushes.LightGray, boxRect);
                                // Vẽ viền ô vuông
                                e.Graphics.DrawRectangle(System.Drawing.Pens.Gray, boxRect);
                                // Vẽ dấu gạch ngang ở giữa ô vuông nhỏ
                                int lineY = y + boxSize / 2;
                                e.Graphics.DrawLine(System.Drawing.Pens.Gray, x + 3, lineY, x + boxSize - 3, lineY);
                                e.Handled = true;
                                return;
                            }
                        }
                    }
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
