using DevExpress.Office.Crypto.Agile;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.HisExecuteRoom.RoomConfigOption;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.Utility;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static DevExpress.Data.Helpers.ExpressiveSortInfo;
using static HIS.Desktop.Plugins.HisExecuteRoom.RoomConfigOption.RoomConfigOption;

namespace HIS.Desktop.Plugins.HisExecuteRoom.HisExecuteRoom
{
    public partial class frmHisExecuteRoom : HIS.Desktop.Utility.FormBase
    {
        private DevExpress.Utils.ToolTipController toolTipControllerRoomOptionItem;
        List<RoomOptionItem> SelectedOptions;
        List<RoomOptionItem> DepartmentsDataSource;
        private void InitComboDepartment2()
        {
            this.DepartmentsDataSource = (from a in (IList<RoomConfigOption.RoomConfigOption.Option>)Enum.GetValues(typeof(RoomConfigOption.RoomConfigOption.Option))
                                          select new RoomOptionItem(a)).ToList();
            this.InitCombo(cboDepartment,
                DepartmentsDataSource,
                 nameof(RoomOptionItem.Name),
                 nameof(RoomOptionItem.Code),
                cboDepartment_MarksSelection,
                cboDepartment_CustomDisplayText,
                OnViewRowClick,
                OnViewRowStyle
                );
            this.EnsureToolTipService();
        }

        private void cboDepartment_MarksSelection(object sender, EventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    List<RoomOptionItem> sgSelectedNews = new List<RoomOptionItem>();
                    foreach (RoomOptionItem rv in (gridCheckMark).Selection)
                    {
                        if (rv != null)
                        {
                            if (sb.ToString().Length > 0) { sb.Append(", "); }
                            sb.Append(rv.Name.ToString());
                            sgSelectedNews.Add(rv);
                        }
                    }
                    this.SelectedOptions = new List<RoomOptionItem>();
                    this.SelectedOptions.AddRange(sgSelectedNews);
                    //this.cboDepartment.EditValue = sb.ToString();
                    this.cboDepartment.Text = sb.ToString();
                    //
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        protected void cboDepartment_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null || gridCheckMark.Selection == null || gridCheckMark.Selection.Count == 0)
                {
                    e.DisplayText = "";
                    return;
                }
                foreach (RoomOptionItem rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(", "); }

                    sb.Append(rv.Name.ToString());
                }
                //if (SelectedOptions != null && SelectedOptions.Count == this.DepartmentsDataSource.Count)
                //{
                //    sb = new StringBuilder("Tất cả");
                //}
                e.DisplayText = sb.Length > 4000 ? sb.ToString().Substring(0, 4000) : sb.ToString();
                var g = sender as DevExpress.XtraEditors.GridLookUpEdit;
                g.Text = e.DisplayText;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void OnViewRowClick(object s, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            try
            {
                var view = s as DevExpress.XtraGrid.Views.Grid.GridView;
                if (view == null) return;
                RoomOptionItem row = view.GetRow(e.RowHandle) as RoomOptionItem;
                if (row == null) return;
                if (view.FocusedColumn.FieldName == "CheckMarkSelection")
                {
                    if (row.Option == Option.MustBeApprovedSurgery)
                    {
                        if (SelectedOptions == null || !SelectedOptions.Any(o => o.Option == RoomConfigOption.RoomConfigOption.Option.IsSurgery))
                        {
                            GridCheckMarksSelection gridCheckMark = cboDepartment.Properties.Tag as GridCheckMarksSelection;
                            if (gridCheckMark != null)
                            {
                                gridCheckMark.SelectRow(view, e.RowHandle, false);
                            }
                        }
                    }
                    else if (row.Option == Option.IsSurgery)
                    {
                        if (SelectedOptions != null
                        && SelectedOptions.Any(o => o.Option == RoomConfigOption.RoomConfigOption.Option.MustBeApprovedSurgery)
                        && !SelectedOptions.Any(o => o.Option == RoomConfigOption.RoomConfigOption.Option.IsSurgery)
                        )
                        {
                            GridCheckMarksSelection gridCheckMark = cboDepartment.Properties.Tag as GridCheckMarksSelection;
                            if (gridCheckMark != null)
                            {
                                for (int i = 0; i < view.DataRowCount; i++)
                                {
                                    var r = view.GetRow(i) as RoomOptionItem;
                                    if (r != null && r.Option == RoomConfigOption.RoomConfigOption.Option.MustBeApprovedSurgery)
                                    {
                                        gridCheckMark.SelectRow(view, i, false);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void OnViewRowStyle(object s, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            try
            {
                var view = s as DevExpress.XtraGrid.Views.Grid.GridView;
                if (view == null) return;
                RoomOptionItem row = view.GetRow(e.RowHandle) as RoomOptionItem;
                if (row == null) return;
                if (row != null && row.Option == Option.MustBeApprovedSurgery)
                {
                    if (SelectedOptions == null || !SelectedOptions.Any(o => o.Option == RoomConfigOption.RoomConfigOption.Option.IsSurgery))
                    {
                        e.Appearance.ForeColor = System.Drawing.Color.Gray;
                        e.Appearance.BackColor = System.Drawing.Color.WhiteSmoke;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ProcessSelectDepartment(List<RoomOptionItem> selectedItems)
        {
            try
            {
                GridCheckMarksSelection gridCheckMark = cboDepartment.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    cboClearSelection(cboDepartment);
                }
                if (selectedItems != null && selectedItems.Count > 0 && cboDepartment.Properties.Tag != null)
                {
                    List<RoomOptionItem> dataSourceItems = cboDepartment.Properties.DataSource as List<RoomOptionItem>;
                    List<RoomOptionItem> validSelectedItems = new List<RoomOptionItem>();
                    foreach (var item in selectedItems)
                    {
                        var row = dataSourceItems != null ? dataSourceItems.FirstOrDefault(o => o.Code == item.Code) : null;
                        if (row != null)
                        {
                            validSelectedItems.Add(row);
                        }
                    }
                    gridCheckMark.SelectAll(validSelectedItems);
                    if (cboDepartment.Properties.Buttons.Count > 0)
                    {
                        foreach (EditorButton item in cboDepartment.Properties.Buttons)
                        {
                            if (item != null && item.Kind == ButtonPredefines.Delete)
                            {
                                item.Visible = true;
                            }
                        }
                    }
                }
                else
                {
                    cboDepartment.EditValue = null;
                    GridCheckMarksSelection gridCheckMarkBusinessCodes = cboDepartment.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMarkBusinessCodes != null)
                    {
                        cboClearSelection(cboDepartment);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void EnsureToolTipService()
        {
            if (_toolTipAttached)
            {
                return;
            }
            _toolTipService.Attach(
                cboDepartment,
                gridView22,
                rrow => (rrow as RoomOptionItem)?.ToolTip ?? string.Empty);

            _toolTipAttached = true;

            if (_toolTipAttached)
            {
                return;
            }
        }
    }
}
