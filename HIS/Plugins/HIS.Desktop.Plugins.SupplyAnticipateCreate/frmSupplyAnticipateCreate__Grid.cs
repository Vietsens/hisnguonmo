using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.Plugins.SupplyAnticipateCreate.ADO;
using HIS.Desktop.Plugins.SupplyAnticipateCreate.Resources;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.SupplyAnticipateCreate
{
    public partial class frmSupplyAnticipateCreate
    {
        private const string FORMAT_MONEY = "#,##0.####";
        private const string FORMAT_AMOUNT = "#,##0.####";

        private GridColumn colDelete;
        private GridColumn colSupplier;
        private GridColumn colAmount;
        private GridColumn colApprovalAmount;
        private GridColumn colNote;
        private GridColumn colExpMonth1;
        private GridColumn colExpMonth2;
        private GridColumn colExpMonth3;
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
                this.AddColumn(view, "STT", "frmSupplyAnticipateCreate.Column.Stt", 40, i++, null, null);
                this.AddColumn(view, "TYPE_CODE", "frmSupplyAnticipateCreate.Column.TypeCode", 90, i++, null, null);
                this.AddColumn(view, "TYPE_NAME", "frmSupplyAnticipateCreate.Column.TypeName", 220, i++, null, null);
                this.AddColumn(view, "SERVICE_UNIT_NAME", "frmSupplyAnticipateCreate.Column.ServiceUnit", 60, i++, null, null);
                this.AddColumn(view, "PACKING", "frmSupplyAnticipateCreate.Column.Packing", 110, i++, null, null);
                this.colSupplier = this.AddColumn(view, "SUPPLIER_ID", "frmSupplyAnticipateCreate.Column.Supplier", 180, i++, null, this.repositorySupplier);
                GridColumn colStock = this.AddColumn(view, "TOTAL_IN_STOCK", "frmSupplyAnticipateCreate.Column.TotalInStock", 80, i++, FORMAT_AMOUNT, null);
                colStock.ToolTip = ResourceLanguageManager.Get("frmSupplyAnticipateCreate.Column.TotalInStock.ToolTip");
                this.colExpMonth1 = this.AddColumn(view, "EXP_MONTH_1", "frmSupplyAnticipateCreate.Column.ExpMonth", 80, i++, FORMAT_AMOUNT, null);
                this.colExpMonth2 = this.AddColumn(view, "EXP_MONTH_2", "frmSupplyAnticipateCreate.Column.ExpMonth", 80, i++, FORMAT_AMOUNT, null);
                this.colExpMonth3 = this.AddColumn(view, "EXP_MONTH_3", "frmSupplyAnticipateCreate.Column.ExpMonth", 80, i++, FORMAT_AMOUNT, null);
                GridColumn colLast = this.AddColumn(view, "LAST_APPROVED_AMOUNT", "frmSupplyAnticipateCreate.Column.LastApproved", 90, i++, FORMAT_AMOUNT, null);
                colLast.ToolTip = ResourceLanguageManager.Get("frmSupplyAnticipateCreate.Column.LastApproved.ToolTip");
                this.AddColumn(view, "LAST_UNAPPROVED_AMOUNT", "frmSupplyAnticipateCreate.Column.LastUnapproved", 90, i++, FORMAT_AMOUNT, null);
                this.colAmount = this.AddColumn(view, "AMOUNT", "frmSupplyAnticipateCreate.Column.Amount", 80, i++, FORMAT_AMOUNT, this.repositorySpinAmount);
                this.colApprovalAmount = this.AddColumn(view, "APPROVAL_AMOUNT", "frmSupplyAnticipateCreate.Column.ApprovalAmount", 80, i++, FORMAT_AMOUNT, this.repositorySpinAmount);
                this.AddColumn(view, "IMP_PRICE", "frmSupplyAnticipateCreate.Column.ImpPrice", 90, i++, FORMAT_MONEY, null);
                this.AddColumn(view, "VAT_PERCENT", "frmSupplyAnticipateCreate.Column.VatPercent", 55, i++, FORMAT_AMOUNT, null);
                GridColumn colTotal = this.AddColumn(view, "TOTAL_PRICE", "frmSupplyAnticipateCreate.Column.TotalPrice", 100, i++, FORMAT_MONEY, null);
                GridColumn colVat = this.AddColumn(view, "VAT_PRICE", "frmSupplyAnticipateCreate.Column.VatPrice", 90, i++, FORMAT_MONEY, null);
                GridColumn colTotalVat = this.AddColumn(view, "TOTAL_PRICE_WITH_VAT", "frmSupplyAnticipateCreate.Column.TotalPriceWithVat", 110, i++, FORMAT_MONEY, null);
                this.colNote = this.AddColumn(view, "NOTE", "frmSupplyAnticipateCreate.Column.Note", 150, i++, null, null);
                this.AddColumn(view, "KIND_NAME", "frmSupplyAnticipateCreate.Column.Kind", 70, i++, null, null);
                this.AddColumn(view, "STOCK_TIME_DISPLAY", "frmSupplyAnticipateCreate.Column.StockTime", 115, i++, null, null);
                foreach (GridColumn sumCol in new GridColumn[] { colTotal, colVat, colTotalVat })
                {
                    sumCol.SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
                    sumCol.SummaryItem.DisplayFormat = "{0:" + FORMAT_MONEY + "}";
                }
                this.colAmount.AppearanceHeader.Font = new Font(this.colAmount.AppearanceHeader.Font, FontStyle.Bold);
                this.colApprovalAmount.AppearanceHeader.Font = new Font(this.colApprovalAmount.AppearanceHeader.Font, FontStyle.Bold);
                this.colAmount.ToolTip = ResourceLanguageManager.Get("frmSupplyAnticipateCreate.Column.Amount.ToolTip");
                this.colApprovalAmount.ToolTip = ResourceLanguageManager.Get("frmSupplyAnticipateCreate.Column.ApprovalAmount.ToolTip");

                view.OptionsView.ColumnAutoWidth = false;
                view.OptionsView.ShowFooter = true;
                view.OptionsView.ShowGroupPanel = false;
                view.OptionsView.ShowIndicator = false;
                view.OptionsCustomization.AllowSort = false;
                view.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseDownFocused;
                view.ShowingEditor += this.gridViewLine_ShowingEditor;
                view.ShownEditor += this.gridViewLine_ShownEditor;
                view.CellValueChanged += this.gridViewLine_CellValueChanged;
                view.RowCellStyle += this.gridViewLine_RowCellStyle;
                this.SetExpMonthCaptions(null, null, null);
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

        /// <summary>
        /// Month labels, farthest month on the left
        /// </summary>
        private void SetExpMonthCaptions(long? month1, long? month2, long? month3)
        {
            string format = ResourceLanguageManager.Get("frmSupplyAnticipateCreate.Column.ExpMonth");
            long? period = this.GetSelectedPeriod();
            DateTime? basePeriod = period.HasValue ? Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(period.Value) : null;
            this.colExpMonth1.Caption = string.Format(format, MonthText(month1, basePeriod, -3));
            this.colExpMonth2.Caption = string.Format(format, MonthText(month2, basePeriod, -2));
            this.colExpMonth3.Caption = string.Format(format, MonthText(month3, basePeriod, -1));
        }

        /// <summary>
        /// Accept yyyyMM or yyyyMMddHHmmss from server, else compute from the period
        /// </summary>
        internal static string MonthText(long? month, DateTime? basePeriod, int offset)
        {
            if (month.HasValue && month.Value > 0)
            {
                string s = month.Value.ToString();
                if (s.Length >= 6)
                {
                    return s.Substring(4, 2) + "/" + s.Substring(0, 4);
                }
            }
            if (basePeriod.HasValue)
            {
                return basePeriod.Value.AddMonths(offset).ToString("MM/yyyy");
            }
            return "";
        }

        private void RefreshGrid()
        {
            try
            {
                int stt = 1;
                foreach (SupplyLineADO line in this.lines)
                {
                    line.STT = stt++;
                }
                this.gridControlLine.BeginUpdate();
                this.gridControlLine.DataSource = null;
                this.gridControlLine.DataSource = this.lines;
                this.gridControlLine.EndUpdate();
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
                if (col == this.colDelete || col == this.colAmount || col == this.colSupplier || col == this.colNote)
                {
                    e.Cancel = this.mode != FormMode.Edit;
                }
                else if (col == this.colApprovalAmount)
                {
                    e.Cancel = this.mode != FormMode.Approve;
                }
                else
                {
                    e.Cancel = true;
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
                SupplyLineADO line = this.gridViewLine.GetFocusedRow() as SupplyLineADO;
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

        private void gridViewLine_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                if (this.isBinding)
                {
                    return;
                }
                if (e.Column == this.colAmount || e.Column == this.colSupplier || e.Column == this.colNote || e.Column == this.colApprovalAmount)
                {
                    this.isDirty = true;
                    this.gridViewLine.RefreshRow(e.RowHandle);
                    this.gridViewLine.UpdateTotalSummary();
                }
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
                SupplyLineADO line = this.gridViewLine.GetRow(e.RowHandle) as SupplyLineADO;
                if (line == null)
                {
                    return;
                }
                if (e.Column == this.colApprovalAmount && line.IsApprovalDifferent)
                {
                    e.Appearance.BackColor = Color.LightGoldenrodYellow;
                    e.Appearance.ForeColor = Color.Blue;
                    e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                }
                else if ((e.Column == this.colAmount && this.mode == FormMode.Edit) || (e.Column == this.colApprovalAmount && this.mode == FormMode.Approve))
                {
                    e.Appearance.BackColor = Color.Honeydew;
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
                if (this.mode != FormMode.Edit)
                {
                    return;
                }
                SupplyLineADO line = this.gridViewLine.GetFocusedRow() as SupplyLineADO;
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
        /// Pick an item: add a line (no auto amount), load its reference numbers
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
                SupplyLineADO existed = this.lines.FirstOrDefault(o => o.KEY == key);
                if (existed != null)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(this, string.Format(ResourceLanguageManager.Get("Message.ItemExisted"), item.TYPE_CODE, item.TYPE_NAME), ResourceLanguageManager.Get("Message.Title"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.FocusLine(existed);
                }
                else
                {
                    SupplyLineADO line = this.NewLine(item);
                    line.SUPPLIER_ID = item.SUPPLIER_IDS != null && item.SUPPLIER_IDS.Count > 0 && this.suppliers.Any(o => o.ID == item.SUPPLIER_IDS[0]) ? (long?)item.SUPPLIER_IDS[0] : null;
                    this.lines.Add(line);
                    this.isDirty = true;
                    this.RefreshGrid();
                    this.LoadReference(new List<SupplyLineADO> { line });
                    this.FocusLine(line);
                    this.gridViewLine.FocusedColumn = this.colAmount;
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

        private void FocusLine(SupplyLineADO line)
        {
            int index = this.lines.IndexOf(line);
            if (index >= 0)
            {
                this.gridViewLine.FocusedRowHandle = this.gridViewLine.GetRowHandle(index);
                this.gridControlLine.Focus();
            }
        }

        private SupplyLineADO NewLine(ItemTypeADO item)
        {
            SupplyLineADO line = new SupplyLineADO();
            line.KEY = item.KEY;
            line.TYPE_ID = item.TYPE_ID;
            line.IS_MATERIAL = item.IS_MATERIAL;
            line.TYPE_CODE = item.TYPE_CODE;
            line.TYPE_NAME = item.TYPE_NAME;
            line.SERVICE_UNIT_NAME = item.SERVICE_UNIT_NAME;
            line.PACKING = item.PACKING;
            line.KIND_NAME = item.KIND_NAME;
            line.SUPPLIER_IDS = item.SUPPLIER_IDS;
            line.IMP_PRICE = item.IMP_PRICE;
            line.IMP_VAT_RATIO = item.IMP_VAT_RATIO;
            return line;
        }

        private void cboStock_EditValueChanged(object sender, EventArgs e)
        {
            this.HeaderChanged();
        }

        private void dtPeriod_EditValueChanged(object sender, EventArgs e)
        {
            this.HeaderChanged();
        }

        private void txtDescription_EditValueChanged(object sender, EventArgs e)
        {
            if (!this.isBinding)
            {
                this.isDirty = true;
            }
        }

        /// <summary>
        /// Stock or period changed: reference numbers of every line depend on both
        /// </summary>
        private void HeaderChanged()
        {
            try
            {
                if (this.isBinding)
                {
                    return;
                }
                this.isDirty = true;
                this.SetExpMonthCaptions(null, null, null);
                this.LoadReference(this.lines);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
