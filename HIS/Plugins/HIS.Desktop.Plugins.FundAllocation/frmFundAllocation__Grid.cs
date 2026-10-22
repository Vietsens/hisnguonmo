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
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.Plugins.FundAllocation.ADO;
using HIS.Desktop.Plugins.FundAllocation.Base;
using HIS.Desktop.Plugins.FundAllocation.Resources;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.FundAllocation
{
    public partial class frmFundAllocation
    {
        private const string FORMAT_MONEY = "#,##0.####";
        private const string FORMAT_PRICE = "#,##0.###";
        private const string FORMAT_AMOUNT = "#,##0.######";

        private RepositoryItemSpinEdit repositorySpinAmount;
        private RepositoryItemSpinEdit repositorySpinMoney;
        private RepositoryItemCheckEdit repositoryCheckReadOnly;
        private Font strikeoutFont;

        private void InitRepositories()
        {
            if (this.repositorySpinAmount != null)
            {
                return;
            }
            this.repositorySpinAmount = this.CreateSpin(FORMAT_AMOUNT);
            this.repositorySpinMoney = this.CreateSpin(FORMAT_MONEY);
            this.repositoryCheckReadOnly = new RepositoryItemCheckEdit();
            this.repositoryCheckReadOnly.ReadOnly = true;
            this.gridControlFund.RepositoryItems.AddRange(new RepositoryItem[] { this.repositorySpinAmount, this.repositorySpinMoney, this.repositoryCheckReadOnly });
            this.gridControlPatient.RepositoryItems.AddRange(new RepositoryItem[] { this.repositorySpinAmount, this.repositorySpinMoney, this.repositoryCheckReadOnly });
        }

        private RepositoryItemSpinEdit CreateSpin(string format)
        {
            RepositoryItemSpinEdit spin = new RepositoryItemSpinEdit();
            spin.Buttons.Clear();
            spin.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
            spin.DisplayFormat.FormatString = format;
            spin.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            spin.EditFormat.FormatString = format;
            spin.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            spin.MaxValue = 999999999999999m;
            spin.MinValue = -999999999999999m;
            return spin;
        }

        private void InitGridFund()
        {
            this.InitLineGrid(this.gridViewFund, AllocationSide.Fund);
        }

        private void InitGridPatient()
        {
            this.InitLineGrid(this.gridViewPatient, AllocationSide.Patient);
        }

        private void InitLineGrid(GridView view, AllocationSide side)
        {
            try
            {
                this.InitRepositories();
                view.Columns.Clear();
                string prefix = side == AllocationSide.Fund ? "FUND" : "PATIENT";
                int i = 0;
                this.AddColumn(view, "TDL_SERVICE_CODE", "frmFundAllocation.Column.ServiceCode", 80, i++, null, null);
                this.AddColumn(view, "TDL_SERVICE_NAME", "frmFundAllocation.Column.ServiceName", 180, i++, null, null);
                GridColumn colAmount = this.AddColumn(view, prefix + "_AMOUNT", "frmFundAllocation.Column.Amount", 70, i++, FORMAT_AMOUNT, this.repositorySpinAmount);
                GridColumn colPrice = this.AddColumn(view, prefix + "_PRICE", "frmFundAllocation.Column.Price", 90, i++, FORMAT_PRICE, this.repositorySpinMoney);
                GridColumn colTotal = this.AddColumn(view, prefix + "_TOTAL_PRICE", "frmFundAllocation.Column.TotalPrice", 100, i++, FORMAT_MONEY, this.repositorySpinMoney);
                this.AddColumn(view, "SERVICE_UNIT_NAME", "frmFundAllocation.Column.ServiceUnit", 80, i++, null, null);
                GridColumn colPackage = this.AddColumn(view, "IsInPackage", "frmFundAllocation.Column.InPackage", 70, i++, null, this.repositoryCheckReadOnly);
                colPackage.OptionsColumn.AllowEdit = false;
                this.AddColumn(view, "NOTE", "frmFundAllocation.Column.Note", 140, i++, null, null);
                this.AddColumn(view, "SERVICE_TYPE_NAME", "frmFundAllocation.Column.ServiceType", 120, i++, null, null);
                this.AddColumn(view, "INTRUCTION_TIME_DISPLAY", "frmFundAllocation.Column.IntructionTime", 125, i++, null, null);
                this.AddColumn(view, "REQUEST_ROOM_NAME", "frmFundAllocation.Column.RequestRoom", 150, i++, null, null);
                colAmount.OptionsColumn.AllowEdit = true;
                colPrice.OptionsColumn.AllowEdit = true;
                colTotal.OptionsColumn.AllowEdit = true;
                colTotal.SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Custom;
                colTotal.SummaryItem.DisplayFormat = "{0:" + FORMAT_MONEY + "}";

                view.OptionsView.ColumnAutoWidth = false;
                view.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseDownFocused;
                view.CustomSummaryCalculate += this.LineView_CustomSummaryCalculate;
                view.ShowingEditor += this.LineView_ShowingEditor;
                view.ValidatingEditor += this.LineView_ValidatingEditor;
                view.InvalidValueException += this.LineView_InvalidValueException;
                view.RowCellStyle += this.LineView_RowCellStyle;
                view.RowStyle += this.LineView_RowStyle;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitGridHistory()
        {
            try
            {
                GridView view = this.gridViewHistory;
                view.Columns.Clear();
                int i = 0;
                this.AddColumn(view, "VERSION_NUMBER", "frmFundAllocation.History.Version", 75, i++, null, null);
                this.AddColumn(view, "STATUS_DISPLAY", "frmFundAllocation.History.Status", 170, i++, null, null);
                this.AddColumn(view, "SAVE_USERNAME", "frmFundAllocation.History.SaveUser", 130, i++, null, null);
                this.AddColumn(view, "FINISH_USERNAME", "frmFundAllocation.History.FinishUser", 130, i++, null, null);
                this.AddColumn(view, "FINISH_TIME_DISPLAY", "frmFundAllocation.History.FinishTime", 125, i++, null, null);
                this.AddColumn(view, "TOTAL_FUND_PRICE", "frmFundAllocation.History.TotalFundPrice", 110, i++, FORMAT_MONEY, null);
                this.AddColumn(view, "SURCHARGE_AMOUNT", "frmFundAllocation.History.SurchargeAmount", 100, i++, FORMAT_MONEY, null);
                this.AddColumn(view, "TOTAL_FUND_PAY", "frmFundAllocation.History.TotalFundPay", 125, i++, FORMAT_MONEY, null);
                this.AddColumn(view, "CANCEL_USERNAME", "frmFundAllocation.History.CancelUser", 130, i++, null, null);
                this.AddColumn(view, "CANCEL_TIME_DISPLAY", "frmFundAllocation.History.CancelTime", 125, i++, null, null);
                this.AddColumn(view, "CANCEL_REASON", "frmFundAllocation.History.CancelReason", 200, i++, null, null);
                this.AddColumn(view, "TRANSACTION_CODES", "frmFundAllocation.History.Transactions", 260, i++, null, null);
                view.OptionsView.ColumnAutoWidth = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private GridColumn AddColumn(GridView view, string fieldName, string captionKey, int width, int visibleIndex, string numberFormat, RepositoryItem editor)
        {
            GridColumn col = new GridColumn();
            col.FieldName = fieldName;
            col.Name = "gc" + view.Name + fieldName;
            col.Caption = ResourceLanguageManager.Get(captionKey);
            col.ToolTip = col.Caption;
            col.Width = width;
            col.Visible = true;
            col.VisibleIndex = visibleIndex;
            col.OptionsColumn.AllowEdit = false;
            if (!String.IsNullOrEmpty(numberFormat))
            {
                col.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                col.DisplayFormat.FormatString = numberFormat;
                col.AppearanceCell.Options.UseTextOptions = true;
                col.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            }
            if (editor != null)
            {
                col.ColumnEdit = editor;
            }
            view.Columns.Add(col);
            return col;
        }

        private AllocationSide GetSide(GridView view)
        {
            return view == this.gridViewFund ? AllocationSide.Fund : AllocationSide.Patient;
        }

        private AllocationField? GetField(GridColumn column)
        {
            if (column == null)
            {
                return null;
            }
            if (column.FieldName.EndsWith("_TOTAL_PRICE"))
            {
                return AllocationField.TotalPrice;
            }
            if (column.FieldName.EndsWith("_AMOUNT"))
            {
                return AllocationField.Amount;
            }
            if (column.FieldName.EndsWith("_PRICE"))
            {
                return AllocationField.Price;
            }
            return null;
        }

        /// <summary>
        /// Footer total of the line grids: removed lines (struck out) are shown for reference only and must not be summed,
        /// so the footer matches the totals panel (B, N).
        /// </summary>
        private void LineView_CustomSummaryCalculate(object sender, DevExpress.Data.CustomSummaryEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                if (view == null)
                {
                    return;
                }
                if (e.SummaryProcess == DevExpress.Data.CustomSummaryProcess.Start)
                {
                    e.TotalValue = 0m;
                }
                else if (e.SummaryProcess == DevExpress.Data.CustomSummaryProcess.Calculate)
                {
                    FundAllocationLineADO line = view.GetRow(e.RowHandle) as FundAllocationLineADO;
                    if (line != null && !line.IsRemoved && e.FieldValue != null)
                    {
                        e.TotalValue = Convert.ToDecimal(e.TotalValue) + Convert.ToDecimal(e.FieldValue);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LineView_ShowingEditor(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                FundAllocationLineADO line = view != null ? view.GetFocusedRow() as FundAllocationLineADO : null;
                if (line == null || this.IsFinished() || line.IsRemoved || line.IsWholeOnly || !this.GetField(view.FocusedColumn).HasValue)
                {
                    e.Cancel = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LineView_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                FundAllocationLineADO line = view != null ? view.GetFocusedRow() as FundAllocationLineADO : null;
                AllocationField? field = view != null ? this.GetField(view.FocusedColumn) : null;
                if (line == null || !field.HasValue)
                {
                    return;
                }
                decimal value = e.Value != null ? Convert.ToDecimal(e.Value) : 0;
                string error = null;
                if (!AllocationCalculator.TryApply(line, this.GetSide(view), field.Value, value, out error))
                {
                    e.Valid = false;
                    e.ErrorText = error;
                    return;
                }
                e.Value = view.FocusedColumn.FieldName.StartsWith("FUND")
                    ? (field.Value == AllocationField.Amount ? line.FUND_AMOUNT : (field.Value == AllocationField.Price ? line.FUND_PRICE : line.FUND_TOTAL_PRICE))
                    : (field.Value == AllocationField.Amount ? line.PATIENT_AMOUNT : (field.Value == AllocationField.Price ? line.PATIENT_PRICE : line.PATIENT_TOTAL_PRICE));
                this.isDirty = true;
                this.BeginInvoke(new MethodInvoker(this.BindGrids));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LineView_InvalidValueException(object sender, InvalidValueExceptionEventArgs e)
        {
            try
            {
                DevExpress.XtraEditors.XtraMessageBox.Show(this, e.ErrorText, ResourceLanguageManager.Get("frmFundAllocation.Message.ThongBao"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.ExceptionMode = DevExpress.XtraEditors.Controls.ExceptionMode.Ignore;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LineView_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                FundAllocationLineADO line = view != null ? view.GetRow(e.RowHandle) as FundAllocationLineADO : null;
                if (line == null || e.Column == null)
                {
                    return;
                }
                if (e.Column.FieldName == "FUND_PRICE" && AllocationCalculator.IsOddPrice(line.FUND_PRICE))
                {
                    e.Appearance.ForeColor = Color.Red;
                }
                else if (e.Column.FieldName == "PATIENT_PRICE" && AllocationCalculator.IsOddPrice(line.PATIENT_PRICE))
                {
                    e.Appearance.ForeColor = Color.Red;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LineView_RowStyle(object sender, RowStyleEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                FundAllocationLineADO line = view != null ? view.GetRow(e.RowHandle) as FundAllocationLineADO : null;
                if (line == null)
                {
                    return;
                }
                if (line.IsRemoved)
                {
                    if (this.strikeoutFont == null)
                    {
                        this.strikeoutFont = new Font(e.Appearance.Font, FontStyle.Strikeout);
                    }
                    e.Appearance.ForeColor = Color.Gray;
                    e.Appearance.Font = this.strikeoutFont;
                }
                else if (line.IsNewOrChanged)
                {
                    e.Appearance.BackColor = Color.LightYellow;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
