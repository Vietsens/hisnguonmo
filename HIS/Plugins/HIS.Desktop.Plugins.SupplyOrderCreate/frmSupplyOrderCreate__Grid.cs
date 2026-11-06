using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.Plugins.SupplyOrderCreate.ADO;
using HIS.Desktop.Plugins.SupplyOrderCreate.Resources;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.SupplyOrderCreate
{
    public partial class frmSupplyOrderCreate
    {
        private const string FORMAT_MONEY = "#,##0.####";
        private const string FORMAT_AMOUNT = "#,##0.####";

        private GridColumn colDelete;
        private GridColumn colSupplier;
        private GridColumn colSupplierName;
        private GridColumn colApproved;
        private GridColumn colOrdered;
        private GridColumn colRemain;
        private GridColumn colNew;
        private GridColumn colImpUnitAmount;
        private GridColumn colAmount;
        private GridColumn colTotal;
        private GridColumn colVat;
        private GridColumn colTotalWithVat;
        private GridColumn colArrived;
        private GridColumn colImported;
        private GridColumn colWait;
        private RepositoryItemSpinEdit repositorySpinAmount;
        private RepositoryItemLookUpEdit repositorySupplier;
        private RepositoryItemButtonEdit repositoryDelete;

        private void InitGrid()
        {
            try
            {
                this.repositorySpinAmount = new RepositoryItemSpinEdit();
                this.repositorySpinAmount.Buttons.Clear();
                this.repositorySpinAmount.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
                this.repositorySpinAmount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                this.repositorySpinAmount.DisplayFormat.FormatString = FORMAT_AMOUNT;
                this.repositorySpinAmount.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                this.repositorySpinAmount.EditFormat.FormatString = FORMAT_AMOUNT;
                this.repositorySpinAmount.MinValue = 0;
                this.repositorySpinAmount.MaxValue = 999999999999m;

                this.repositorySupplier = new RepositoryItemLookUpEdit();
                this.repositorySupplier.DataSource = this.suppliers;
                this.repositorySupplier.ValueMember = "ID";
                this.repositorySupplier.DisplayMember = "SUPPLIER_NAME";
                this.repositorySupplier.NullText = "";
                this.repositorySupplier.ShowHeader = false;
                this.repositorySupplier.Columns.Add(new LookUpColumnInfo("SUPPLIER_CODE", "", 70));
                this.repositorySupplier.Columns.Add(new LookUpColumnInfo("SUPPLIER_NAME", "", 250));
                this.repositorySupplier.PopupWidth = 330;
                this.repositorySupplier.SearchMode = SearchMode.AutoFilter;

                this.repositoryDelete = new RepositoryItemButtonEdit();
                this.repositoryDelete.TextEditStyle = TextEditStyles.HideTextEditor;
                this.repositoryDelete.Buttons.Clear();
                this.repositoryDelete.Buttons.Add(new EditorButton(ButtonPredefines.Delete));
                this.repositoryDelete.ButtonClick += this.repositoryDelete_ButtonClick;

                this.gridControlLine.RepositoryItems.AddRange(new RepositoryItem[] { this.repositorySpinAmount, this.repositorySupplier, this.repositoryDelete });

                GridView view = this.gridViewLine;
                view.Columns.Clear();
                int i = 0;
                this.colDelete = this.AddColumn(view, "DELETE", "", 30, i++, null, this.repositoryDelete);
                this.colDelete.UnboundType = DevExpress.Data.UnboundColumnType.Object;
                this.colDelete.OptionsColumn.AllowEdit = true;
                this.colDelete.OptionsColumn.ShowCaption = false;
                this.AddColumn(view, "STT", "frmSupplyOrderCreate.Column.Stt", 40, i++, null, null);
                this.AddColumn(view, "TYPE_CODE", "frmSupplyOrderCreate.Column.TypeCode", 90, i++, null, null);
                this.AddColumn(view, "TYPE_NAME", "frmSupplyOrderCreate.Column.TypeName", 220, i++, null, null);
                this.AddColumn(view, "SERVICE_UNIT_NAME", "frmSupplyOrderCreate.Column.ServiceUnit", 60, i++, null, null);
                this.AddColumn(view, "PACKING", "frmSupplyOrderCreate.Column.Packing", 110, i++, null, null);
                this.colSupplier = this.AddColumn(view, "SUPPLIER_ID", "frmSupplyOrderCreate.Column.Supplier", 180, i++, null, this.repositorySupplier);
                this.colApproved = this.AddColumn(view, "APPROVED_AMOUNT", "frmSupplyOrderCreate.Column.Approved", 85, i++, FORMAT_AMOUNT, null);
                this.colOrdered = this.AddColumn(view, "ORDERED_AMOUNT", "frmSupplyOrderCreate.Column.Ordered", 85, i++, FORMAT_AMOUNT, null);
                this.colNew = this.AddColumn(view, "NEW_AMOUNT", "frmSupplyOrderCreate.Column.New", 85, i++, FORMAT_AMOUNT, null);
                this.colRemain = this.AddColumn(view, "REMAIN_AMOUNT", "frmSupplyOrderCreate.Column.Remain", 85, i++, FORMAT_AMOUNT, null);
                this.colImpUnitAmount = this.AddColumn(view, "IMP_UNIT_AMOUNT", "frmSupplyOrderCreate.Column.ImpUnitAmount", 75, i++, FORMAT_AMOUNT, this.repositorySpinAmount);
                this.AddColumn(view, "IMP_UNIT_NAME", "frmSupplyOrderCreate.Column.ImpUnit", 60, i++, null, null);
                this.colAmount = this.AddColumn(view, "AMOUNT", "frmSupplyOrderCreate.Column.Amount", 85, i++, FORMAT_AMOUNT, this.repositorySpinAmount);
                this.AddColumn(view, "IMP_PRICE", "frmSupplyOrderCreate.Column.ImpPrice", 90, i++, FORMAT_MONEY, null);
                this.AddColumn(view, "VAT_PERCENT", "frmSupplyOrderCreate.Column.VatPercent", 55, i++, FORMAT_AMOUNT, null);
                this.colTotal = this.AddColumn(view, "TOTAL_PRICE", "frmSupplyOrderCreate.Column.TotalPrice", 100, i++, FORMAT_MONEY, null);
                this.colVat = this.AddColumn(view, "VAT_PRICE", "frmSupplyOrderCreate.Column.VatPrice", 90, i++, FORMAT_MONEY, null);
                this.colTotalWithVat = this.AddColumn(view, "TOTAL_PRICE_WITH_VAT", "frmSupplyOrderCreate.Column.TotalPriceWithVat", 110, i++, FORMAT_MONEY, null);
                this.colArrived = this.AddColumn(view, "ARRIVED_AMOUNT", "frmSupplyOrderCreate.Column.Arrived", 85, i++, FORMAT_AMOUNT, null);
                this.colImported = this.AddColumn(view, "IMPORTED_AMOUNT", "frmSupplyOrderCreate.Column.Imported", 85, i++, FORMAT_AMOUNT, null);
                this.colWait = this.AddColumn(view, "WAIT_AMOUNT", "frmSupplyOrderCreate.Column.Wait", 85, i++, FORMAT_AMOUNT, null);
                this.AddColumn(view, "SUPPLY_ORDER_CODE", "frmSupplyOrderCreate.Column.OrderCode", 100, i++, null, null);
                this.AddColumn(view, "KIND_NAME", "frmSupplyOrderCreate.Column.Kind", 70, i++, null, null);
                this.colSupplierName = this.AddColumn(view, "SUPPLIER_NAME", "frmSupplyOrderCreate.Column.Supplier", 180, i++, null, null);
                this.colSupplierName.Visible = false;

                this.colApproved.ToolTip = ResourceLanguageManager.Get("frmSupplyOrderCreate.Column.Approved.ToolTip");
                this.colOrdered.ToolTip = ResourceLanguageManager.Get("frmSupplyOrderCreate.Column.Ordered.ToolTip");
                this.colNew.ToolTip = ResourceLanguageManager.Get("frmSupplyOrderCreate.Column.New.ToolTip");
                this.colRemain.ToolTip = ResourceLanguageManager.Get("frmSupplyOrderCreate.Column.Remain.ToolTip");
                this.colImpUnitAmount.ToolTip = ResourceLanguageManager.Get("frmSupplyOrderCreate.Column.ImpUnitAmount.ToolTip");
                this.colAmount.ToolTip = ResourceLanguageManager.Get("frmSupplyOrderCreate.Column.Amount.ToolTip");
                this.colImpUnitAmount.AppearanceHeader.Font = new Font(this.colImpUnitAmount.AppearanceHeader.Font, FontStyle.Bold);
                this.colAmount.AppearanceHeader.Font = new Font(this.colAmount.AppearanceHeader.Font, FontStyle.Bold);

                foreach (GridColumn sumCol in new GridColumn[] { this.colTotal, this.colVat, this.colTotalWithVat })
                {
                    sumCol.SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
                    sumCol.SummaryItem.DisplayFormat = "{0:" + FORMAT_MONEY + "}";
                    view.GroupSummary.Add(new DevExpress.XtraGrid.GridGroupSummaryItem(DevExpress.Data.SummaryItemType.Sum, sumCol.FieldName, sumCol, "{0:" + FORMAT_MONEY + "}"));
                }
                view.GroupSummary.Add(new DevExpress.XtraGrid.GridGroupSummaryItem(DevExpress.Data.SummaryItemType.Count, "TYPE_CODE", null, ResourceLanguageManager.Get("frmSupplyOrderCreate.GroupCount")));

                view.OptionsView.ColumnAutoWidth = false;
                view.OptionsView.ShowFooter = true;
                view.OptionsView.ShowGroupPanel = false;
                view.OptionsView.ShowIndicator = false;
                view.OptionsView.GroupFooterShowMode = GroupFooterShowMode.VisibleIfExpanded;
                view.OptionsCustomization.AllowSort = false;
                view.OptionsCustomization.AllowGroup = false;
                view.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseDownFocused;
                view.ShowingEditor += this.gridViewLine_ShowingEditor;
                view.ShownEditor += this.gridViewLine_ShownEditor;
                view.CellValueChanged += this.gridViewLine_CellValueChanged;
                view.RowCellStyle += this.gridViewLine_RowCellStyle;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private GridColumn AddColumn(GridView view, string fieldName, string captionKey, int width, int visibleIndex, string format, RepositoryItem editor)
        {
            GridColumn col = new GridColumn();
            col.FieldName = fieldName;
            col.Caption = String.IsNullOrEmpty(captionKey) ? "" : ResourceLanguageManager.Get(captionKey);
            col.Width = width;
            col.Visible = true;
            col.VisibleIndex = visibleIndex;
            col.OptionsColumn.AllowEdit = false;
            col.OptionsFilter.AllowFilter = false;
            if (!String.IsNullOrEmpty(format))
            {
                col.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                col.DisplayFormat.FormatString = format;
                col.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            }
            if (editor != null)
            {
                col.ColumnEdit = editor;
            }
            view.Columns.Add(col);
            return col;
        }

        private void RefreshGrid()
        {
            try
            {
                int stt = 1;
                foreach (OrderLineADO line in this.lines)
                {
                    line.STT = stt++;
                    line.SUPPLIER_NAME = this.GetSupplierName(line.SUPPLIER_ID);
                }
                this.colImpUnitAmount.OptionsColumn.AllowEdit = !this.IsViewMode;
                this.colAmount.OptionsColumn.AllowEdit = !this.IsViewMode;
                this.colSupplier.OptionsColumn.AllowEdit = !this.IsViewMode;
                this.gridControlLine.BeginUpdate();
                this.gridControlLine.DataSource = null;
                this.gridControlLine.DataSource = this.lines;
                this.gridControlLine.EndUpdate();
                if (this.chkPreview.Checked)
                {
                    this.gridViewLine.ExpandAllGroups();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewLine_ShowingEditor(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                GridColumn col = this.gridViewLine.FocusedColumn;
                OrderLineADO line = this.gridViewLine.GetFocusedRow() as OrderLineADO;
                if (this.IsViewMode || line == null)
                {
                    e.Cancel = true;
                }
                else if (col == this.colImpUnitAmount)
                {
                    // k empty or <= 0: only SL le can be entered
                    e.Cancel = !line.HasRatio;
                }
                else
                {
                    e.Cancel = !(col == this.colDelete || col == this.colAmount || col == this.colSupplier);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Supplier list of the item (catalog SUPPLIER_IDS); empty = whole supplier catalog
        /// </summary>
        private void gridViewLine_ShownEditor(object sender, EventArgs e)
        {
            try
            {
                if (this.gridViewLine.FocusedColumn != this.colSupplier)
                {
                    return;
                }
                LookUpEdit editor = this.gridViewLine.ActiveEditor as LookUpEdit;
                OrderLineADO line = this.gridViewLine.GetFocusedRow() as OrderLineADO;
                if (editor == null || line == null)
                {
                    return;
                }
                if (line.SUPPLIER_IDS != null && line.SUPPLIER_IDS.Count > 0)
                {
                    editor.Properties.DataSource = this.suppliers.Where(o => line.SUPPLIER_IDS.Contains(o.ID) || o.ID == line.SUPPLIER_ID).ToList();
                }
                else
                {
                    editor.Properties.DataSource = this.suppliers;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// SL x k = SL le; SL le / k = SL (no rounding to whole packs)
        /// </summary>
        private void gridViewLine_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                if (this.isBinding)
                {
                    return;
                }
                OrderLineADO line = this.gridViewLine.GetRow(e.RowHandle) as OrderLineADO;
                if (line == null)
                {
                    return;
                }
                if (e.Column == this.colImpUnitAmount)
                {
                    line.AMOUNT = line.IMP_UNIT_AMOUNT.HasValue && line.HasRatio
                        ? (decimal?)Math.Round(line.IMP_UNIT_AMOUNT.Value * line.IMP_UNIT_CONVERT_RATIO.Value, 4)
                        : null;
                }
                else if (e.Column == this.colAmount)
                {
                    line.IMP_UNIT_AMOUNT = line.AMOUNT.HasValue && line.HasRatio
                        ? (decimal?)Math.Round(line.AMOUNT.Value / line.IMP_UNIT_CONVERT_RATIO.Value, 6)
                        : null;
                }
                else if (e.Column == this.colSupplier)
                {
                    line.SUPPLIER_NAME = this.GetSupplierName(line.SUPPLIER_ID);
                }
                else
                {
                    return;
                }
                this.isDirty = true;
                if (e.Column == this.colSupplier && this.chkPreview.Checked)
                {
                    this.RefreshGrid();
                    return;
                }
                this.gridViewLine.RefreshRow(e.RowHandle);
                this.gridViewLine.UpdateTotalSummary();
                this.gridViewLine.UpdateGroupSummary();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewLine_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {
                OrderLineADO line = this.gridViewLine.GetRow(e.RowHandle) as OrderLineADO;
                if (line == null)
                {
                    return;
                }
                if (e.Column == this.colRemain && line.REMAIN_AMOUNT.HasValue && line.REMAIN_AMOUNT.Value < 0)
                {
                    e.Appearance.ForeColor = Color.Red;
                    e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                }
                else if (e.Column == this.colAmount && !this.IsViewMode)
                {
                    e.Appearance.BackColor = line.IsOverRemain ? Color.Moccasin : Color.Honeydew;
                }
                else if (e.Column == this.colImpUnitAmount && !this.IsViewMode)
                {
                    e.Appearance.BackColor = line.HasRatio ? Color.Honeydew : Color.Gainsboro;
                }
                if ((e.Column.FieldName == "IMP_PRICE" && !line.IMP_PRICE.HasValue) || (e.Column.FieldName == "VAT_PERCENT" && !line.IMP_VAT_RATIO.HasValue))
                {
                    e.Appearance.BackColor = Color.MistyRose;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryDelete_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (this.IsViewMode)
                {
                    return;
                }
                OrderLineADO line = this.gridViewLine.GetFocusedRow() as OrderLineADO;
                if (line == null)
                {
                    return;
                }
                this.gridViewLine.CloseEditor();
                this.lines.Remove(line);
                this.isDirty = true;
                this.RefreshGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Pick an item: add a line with the first catalog supplier, amount left empty, load F/O/R/N
        /// </summary>
        private void cboItem_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.isBinding || this.cboItem.EditValue == null)
                {
                    return;
                }
                string key = this.cboItem.EditValue.ToString();
                ItemTypeADO item;
                if (!this.dicItemType.TryGetValue(key, out item))
                {
                    return;
                }
                OrderLineADO existed = this.lines.FirstOrDefault(o => o.KEY == key);
                if (existed != null)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(this, string.Format(ResourceLanguageManager.Get("Message.ItemExisted"), item.TYPE_CODE, item.TYPE_NAME), ResourceLanguageManager.Get("Message.Title"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.FocusLine(existed);
                }
                else
                {
                    OrderLineADO line = this.NewLine(item);
                    this.lines.Add(line);
                    this.isDirty = true;
                    this.RefreshGrid();
                    this.LoadBalance(new List<OrderLineADO> { line });
                    this.FocusLine(line);
                    this.gridViewLine.FocusedColumn = line.HasRatio ? this.colImpUnitAmount : this.colAmount;
                }
                this.BeginInvoke(new MethodInvoker(() =>
                {
                    this.isBinding = true;
                    this.cboItem.EditValue = null;
                    this.isBinding = false;
                }));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FocusLine(OrderLineADO line)
        {
            int index = this.lines.IndexOf(line);
            if (index >= 0)
            {
                this.gridViewLine.FocusedRowHandle = this.gridViewLine.GetRowHandle(index);
                this.gridControlLine.Focus();
            }
        }

        private OrderLineADO NewLine(ItemTypeADO item)
        {
            OrderLineADO line = new OrderLineADO();
            line.KEY = item.KEY;
            line.TYPE_ID = item.TYPE_ID;
            line.IS_MATERIAL = item.IS_MATERIAL;
            line.TYPE_CODE = item.TYPE_CODE;
            line.TYPE_NAME = item.TYPE_NAME;
            line.SERVICE_UNIT_NAME = item.SERVICE_UNIT_NAME;
            line.IMP_UNIT_NAME = item.IMP_UNIT_NAME;
            line.IMP_UNIT_CONVERT_RATIO = item.IMP_UNIT_CONVERT_RATIO;
            line.PACKING = item.PACKING;
            line.KIND_NAME = item.KIND_NAME;
            line.SUPPLIER_IDS = item.SUPPLIER_IDS;
            line.IMP_PRICE = item.IMP_PRICE;
            line.IMP_VAT_RATIO = item.IMP_VAT_RATIO;
            line.SUPPLIER_ID = item.SUPPLIER_IDS != null && item.SUPPLIER_IDS.Count > 0 && this.suppliers.Any(o => o.ID == item.SUPPLIER_IDS[0]) ? (long?)item.SUPPLIER_IDS[0] : null;
            return line;
        }
    }
}
