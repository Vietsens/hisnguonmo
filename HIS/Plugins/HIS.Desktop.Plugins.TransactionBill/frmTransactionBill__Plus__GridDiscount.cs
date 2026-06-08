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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.TransactionBill.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.TransactionBill
{
    public partial class frmTransactionBill : HIS.Desktop.Utility.FormBase
    {
        #region Discount grid state

        private bool isDiscountGridWiredUp = false;
        private bool isCellValueChangedFromCode = false;
        private System.Windows.Forms.BindingSource bindingSourceDiscount;
        private List<long> discountDeletedIds = new List<long>();

        // Stub controls — controls cũ (txtDiscount/Ratio + txtReason) đã bị xóa khỏi Designer
        // khi user chuyển sang grid Chiết khấu. Các đoạn code legacy còn tham chiếu được
        // redirect sang stub để KHÔNG vỡ compile + KHÔNG ảnh hưởng UI (stubs không add
        // vào form, không hiển thị). Logic chiết khấu THẬT đã chạy qua grid.
        private DevExpress.XtraEditors.SpinEdit _stubTxtDiscount;
        private DevExpress.XtraEditors.SpinEdit _stubTxtDiscountRatio;
        private DevExpress.XtraEditors.TextEdit _stubTxtReason;

        internal DevExpress.XtraEditors.SpinEdit txtDiscount
        {
            get
            {
                if (_stubTxtDiscount == null) _stubTxtDiscount = new DevExpress.XtraEditors.SpinEdit();
                return _stubTxtDiscount;
            }
        }

        internal DevExpress.XtraEditors.SpinEdit txtDiscountRatio
        {
            get
            {
                if (_stubTxtDiscountRatio == null) _stubTxtDiscountRatio = new DevExpress.XtraEditors.SpinEdit();
                return _stubTxtDiscountRatio;
            }
        }

        internal DevExpress.XtraEditors.TextEdit txtReason
        {
            get
            {
                if (_stubTxtReason == null) _stubTxtReason = new DevExpress.XtraEditors.TextEdit();
                return _stubTxtReason;
            }
        }

        #endregion

        /// <summary>
        /// Cấu hình runtime cho grid Chiết khấu sẵn có trong Designer
        /// (gridControlDiscount / gridViewDiscount / gcDiscount..7 / repoSpinDiscountAmount+2 /
        /// repoTxtDiscountReason / repoBtnDeleteDiscount / lciDiscountGrid).
        /// Ẩn 3 control đơn cũ (Chiết khấu đ/%, Lý do) và đổi layout Ngân hàng / Ghi chú.
        /// </summary>
        private void WireUpDiscountGrid()
        {
            try
            {
                if (this.isDiscountGridWiredUp) return;
                if (this.gridControlDiscount == null || this.gridViewDiscount == null) return;

                this.isDiscountGridWiredUp = true;

                ConfigureDiscountRepositoryItems();
                ConfigureDiscountColumns();
                ConfigureDiscountGridView();
                HookDiscountGridEvents();
                HideDiscountLeftoverControls();
                ApplyDiscountLayoutChanges();
                InitDiscountDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ConfigureDiscountRepositoryItems()
        {
            try
            {
                // repoSpinDiscountAmount -> Chiết khấu (đ)
                if (this.repoSpinDiscountAmount != null)
                {
                    this.repoSpinDiscountAmount.AutoHeight = false;
                    this.repoSpinDiscountAmount.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
                    this.repoSpinDiscountAmount.DisplayFormat.FormatString = "#,##0";
                    this.repoSpinDiscountAmount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    this.repoSpinDiscountAmount.EditFormat.FormatString = "#,##0";
                    this.repoSpinDiscountAmount.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    this.repoSpinDiscountAmount.MaxValue = 9999999999m;
                    this.repoSpinDiscountAmount.MinValue = 0m;
                    this.repoSpinDiscountAmount.ReadOnly = false;
                }

                // repoSpinDiscountRatio -> Chiết khấu (%)
                if (this.repoSpinDiscountRatio != null)
                {
                    this.repoSpinDiscountRatio.AutoHeight = false;
                    this.repoSpinDiscountRatio.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
                    this.repoSpinDiscountRatio.DisplayFormat.FormatString = "#,##0.##";
                    this.repoSpinDiscountRatio.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    this.repoSpinDiscountRatio.EditFormat.FormatString = "#,##0.##";
                    this.repoSpinDiscountRatio.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    this.repoSpinDiscountRatio.MaxValue = 100m;
                    this.repoSpinDiscountRatio.MinValue = 0m;
                    this.repoSpinDiscountRatio.ReadOnly = false;
                }

                // repoTxtDiscountReason -> Lý do
                if (this.repoTxtDiscountReason != null)
                {
                    this.repoTxtDiscountReason.AutoHeight = false;
                    // MaxLength tính theo CHAR; validate thực sự dùng UTF-8 byte length 250
                    // trong gridView_ValidateRow + ValidateDiscountGridBeforeSave.
                    this.repoTxtDiscountReason.MaxLength = 1000;
                    this.repoTxtDiscountReason.ReadOnly = false;
                }

                // repoBtnDeleteDiscount -> nút X xóa dòng. Reuse glyph từ Quỹ hỗ trợ.
                if (this.repoBtnDeleteDiscount != null)
                {
                    this.repoBtnDeleteDiscount.AutoHeight = false;
                    this.repoBtnDeleteDiscount.ReadOnly = false;
                    this.repoBtnDeleteDiscount.TextEditStyle = TextEditStyles.HideTextEditor;

                    System.Drawing.Image deleteImage = null;
                    try
                    {
                        if (this.repositoryItemBtnDeleteFund != null && this.repositoryItemBtnDeleteFund.Buttons.Count > 0)
                            deleteImage = this.repositoryItemBtnDeleteFund.Buttons[0].Image;
                    }
                    catch (Exception exImg) { Inventec.Common.Logging.LogSystem.Warn(exImg); }

                    if (deleteImage != null && this.repoBtnDeleteDiscount.Buttons.Count > 0)
                    {
                        this.repoBtnDeleteDiscount.Buttons[0].Image = deleteImage;
                        this.repoBtnDeleteDiscount.Buttons[0].IsLeft = true;
                        this.repoBtnDeleteDiscount.Buttons[0].Enabled = true;
                    }

                    this.repoBtnDeleteDiscount.ButtonClick -= this.repoBtnDeleteDiscount_ButtonClick;
                    this.repoBtnDeleteDiscount.ButtonClick += this.repoBtnDeleteDiscount_ButtonClick;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ConfigureDiscountColumns()
        {
            try
            {
                // gcDiscount -> DISCOUNT (Chiết khấu đ)
                if (this.gcDiscount != null)
                {
                    this.gcDiscount.Caption = "Chiết khấu (đ)";
                    this.gcDiscount.FieldName = "DISCOUNT";
                    this.gcDiscount.AppearanceCell.Options.UseTextOptions = true;
                    this.gcDiscount.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                    this.gcDiscount.AppearanceHeader.Options.UseTextOptions = true;
                    this.gcDiscount.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    this.gcDiscount.OptionsColumn.AllowEdit = true;
                    this.gcDiscount.OptionsColumn.AllowFocus = true;
                    this.gcDiscount.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
                    this.gcDiscount.OptionsColumn.ShowCaption = true;
                    this.gcDiscount.UnboundType = DevExpress.Data.UnboundColumnType.Bound;
                    this.gcDiscount.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.None;
                    this.gcDiscount.ColumnEdit = this.repoSpinDiscountAmount;
                    this.gcDiscount.Visible = true;
                    this.gcDiscount.VisibleIndex = 0;
                    this.gcDiscount.Width = 130;
                }

                // gcDiscountRatio -> DISCOUNT_RATIO (%)
                if (this.gcDiscountRatio != null)
                {
                    this.gcDiscountRatio.Caption = "Chiết khấu (%)";
                    this.gcDiscountRatio.FieldName = "DISCOUNT_RATIO";
                    this.gcDiscountRatio.AppearanceCell.Options.UseTextOptions = true;
                    this.gcDiscountRatio.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                    this.gcDiscountRatio.AppearanceHeader.Options.UseTextOptions = true;
                    this.gcDiscountRatio.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    this.gcDiscountRatio.OptionsColumn.AllowEdit = true;
                    this.gcDiscountRatio.OptionsColumn.AllowFocus = true;
                    this.gcDiscountRatio.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
                    this.gcDiscountRatio.OptionsColumn.ShowCaption = true;
                    this.gcDiscountRatio.UnboundType = DevExpress.Data.UnboundColumnType.Bound;
                    this.gcDiscountRatio.ColumnEdit = this.repoSpinDiscountRatio;
                    this.gcDiscountRatio.Visible = true;
                    this.gcDiscountRatio.VisibleIndex = 1;
                    this.gcDiscountRatio.Width = 110;
                }

                // gcDiscountReason -> REASON (Lý do)
                if (this.gcDiscountReason != null)
                {
                    this.gcDiscountReason.Caption = "Lý do";
                    this.gcDiscountReason.FieldName = "REASON";
                    this.gcDiscountReason.AppearanceCell.Options.UseTextOptions = true;
                    this.gcDiscountReason.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
                    this.gcDiscountReason.AppearanceHeader.Options.UseTextOptions = true;
                    this.gcDiscountReason.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    this.gcDiscountReason.OptionsColumn.AllowEdit = true;
                    this.gcDiscountReason.OptionsColumn.AllowFocus = true;
                    this.gcDiscountReason.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
                    this.gcDiscountReason.OptionsColumn.ShowCaption = true;
                    this.gcDiscountReason.UnboundType = DevExpress.Data.UnboundColumnType.Bound;
                    this.gcDiscountReason.ColumnEdit = this.repoTxtDiscountReason;
                    this.gcDiscountReason.Visible = true;
                    this.gcDiscountReason.VisibleIndex = 2;
                    this.gcDiscountReason.Width = 275;
                }

                // gcDiscountDelete -> Delete button column
                if (this.gcDiscountDelete != null)
                {
                    this.gcDiscountDelete.Caption = "Xóa";
                    this.gcDiscountDelete.FieldName = "DELETE";
                    this.gcDiscountDelete.ColumnEdit = this.repoBtnDeleteDiscount;
                    this.gcDiscountDelete.UnboundType = DevExpress.Data.UnboundColumnType.Object;
                    this.gcDiscountDelete.OptionsColumn.ShowCaption = false;
                    this.gcDiscountDelete.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
                    this.gcDiscountDelete.Visible = true;
                    this.gcDiscountDelete.VisibleIndex = 3;
                    this.gcDiscountDelete.Width = 41;
                }

                // Ẩn các columns thừa user kéo từ template Quỹ hỗ trợ
                // (STT/checkbox đầu, FUND_BUDGET, FUND_ID) — bảng Chiết khấu KHÔNG có cột này.
                if (this.gridColumn2 != null)
                {
                    this.gridColumn2.Visible = false;
                    this.gridColumn2.VisibleIndex = -1;
                    this.gridColumn2.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.None;
                    this.gridColumn2.Width = 0;
                    this.gridColumn2.OptionsColumn.ShowInCustomizationForm = false;
                }
                if (this.gridColumn6 != null)
                {
                    this.gridColumn6.Visible = false;
                    this.gridColumn6.VisibleIndex = -1;
                    this.gridColumn6.OptionsColumn.ShowInCustomizationForm = false;
                }
                if (this.gridColumn8 != null)
                {
                    this.gridColumn8.Visible = false;
                    this.gridColumn8.VisibleIndex = -1;
                    this.gridColumn8.OptionsColumn.ShowInCustomizationForm = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ConfigureDiscountGridView()
        {
            try
            {
                this.gridViewDiscount.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.True;
                this.gridViewDiscount.OptionsBehavior.Editable = true;
                this.gridViewDiscount.OptionsCustomization.AllowColumnMoving = false;
                this.gridViewDiscount.OptionsCustomization.AllowColumnResizing = false;
                this.gridViewDiscount.OptionsMenu.EnableColumnMenu = false;
                this.gridViewDiscount.OptionsNavigation.AutoFocusNewRow = true;
                this.gridViewDiscount.OptionsNavigation.EnterMoveNextColumn = true;
                this.gridViewDiscount.OptionsView.ColumnAutoWidth = false;
                this.gridViewDiscount.OptionsView.NewItemRowPosition = NewItemRowPosition.Bottom;
                this.gridViewDiscount.OptionsView.ShowGroupPanel = false;
                this.gridViewDiscount.OptionsView.ShowIndicator = false;
                // Force indicator column width = 0 để không hiện cột * (new-row marker / checkbox đầu)
                this.gridViewDiscount.IndicatorWidth = 0;
                // Tắt selection multi-select (không hiện checkbox/select column ở đầu)
                this.gridViewDiscount.OptionsSelection.MultiSelect = false;
                this.gridViewDiscount.OptionsSelection.EnableAppearanceFocusedCell = false;
                // Tắt filter panel ở đầu (nếu có)
                this.gridViewDiscount.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
                this.gridViewDiscount.OptionsFind.AlwaysVisible = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void HookDiscountGridEvents()
        {
            try
            {
                this.gridViewDiscount.CellValueChanged -= this.gridViewDiscount_CellValueChanged;
                this.gridViewDiscount.InvalidRowException -= this.gridViewDiscount_InvalidRowException;
                this.gridViewDiscount.ValidateRow -= this.gridViewDiscount_ValidateRow;

                this.gridViewDiscount.CellValueChanged += this.gridViewDiscount_CellValueChanged;
                this.gridViewDiscount.InvalidRowException += this.gridViewDiscount_InvalidRowException;
                this.gridViewDiscount.ValidateRow += this.gridViewDiscount_ValidateRow;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void HideDiscountLeftoverControls()
        {
            try
            {
                // customGridControl1 + customGridView1 + label1 + layoutControlItem68
                // là 3 control kéo thừa khi designer Designer — ẩn để không ảnh hưởng UI.
                if (this.customGridControl1 != null) this.customGridControl1.Visible = false;
                if (this.label1 != null) this.label1.Visible = false;
                if (this.layoutControlItem68 != null)
                    this.layoutControlItem68.Visibility = LayoutVisibility.Never;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ApplyDiscountLayoutChanges()
        {
            try
            {
                // 3 control đơn lẻ cũ (layoutDiscount/Ratio/Reason) đã bị bỏ trong Designer
                // — không cần ẩn nữa. Chiết khấu đã chuyển hoàn toàn sang grid.

                // Đẩy Ngân hàng nằm cạnh Ghi chú (vì 3 control trên đã bỏ, dành chỗ cho Ngân hàng)
                try
                {
                    if (this.layoutBank != null && this.layoutDescription != null)
                        this.layoutBank.Move(this.layoutDescription, InsertType.Left);
                }
                catch (Exception exMove)
                {
                    Inventec.Common.Logging.LogSystem.Warn(exMove);
                }

                // Set caption cho item67 = "Chiết khấu:" (label trái của grid)
                if (this.lciDiscountGrid != null)
                {
                    // QUAN TRONG: gan grid control vao layout item. Truoc day KHONG gan ->
                    // gridControlDiscount hien o Location cung (102,613) trong Designer => bay xuong day,
                    // tach roi khoi label "Chiet khau:". Gan Control thi grid nam dung vi tri cua item.
                    if (this.gridControlDiscount != null && this.lciDiscountGrid.Control != this.gridControlDiscount)
                        this.lciDiscountGrid.Control = this.gridControlDiscount;

                    this.lciDiscountGrid.AppearanceItemCaption.Options.UseTextOptions = true;
                    this.lciDiscountGrid.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                    this.lciDiscountGrid.AppearanceItemCaption.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
                    this.lciDiscountGrid.Text = "Chiết khấu:";
                    this.lciDiscountGrid.TextAlignMode = TextAlignModeItem.CustomSize;
                    this.lciDiscountGrid.TextSize = new System.Drawing.Size(90, 0);
                    this.lciDiscountGrid.TextToControlDistance = 5;
                    this.lciDiscountGrid.TextVisible = true;

                    // Đặt 2 grid (Chiết khấu + Quỹ hỗ trợ) cân bằng nhau
                    if (this.LciBillFund != null)
                    {
                        int gridHeight = 70;

                        // CHI ep chieu CAO (de grid du cao hien dong). KHONG ep chieu RONG cung
                        // (truoc day ep Width = LciBillFund.Size.Width doc luc form chua layout xong
                        // -> sai/stale -> grid bi tran/bay ra cho khac). Width = 0 => layout tu dan full group.
                        this.lciDiscountGrid.SizeConstraintsType = SizeConstraintsType.Custom;
                        this.lciDiscountGrid.MinSize = new System.Drawing.Size(0, gridHeight);
                        this.lciDiscountGrid.MaxSize = new System.Drawing.Size(0, gridHeight);
                    }

                    // Đặt grid Chiết khấu nằm trên grid Quỹ hỗ trợ
                    try
                    {
                        if (this.LciBillFund != null)
                            this.lciDiscountGrid.Move(this.LciBillFund, InsertType.Top);
                    }
                    catch (Exception exMove2)
                    {
                        Inventec.Common.Logging.LogSystem.Warn(exMove2);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitDiscountDataSource()
        {
            try
            {
                this.bindingSourceDiscount = new System.Windows.Forms.BindingSource();
                this.bindingSourceDiscount.DataSource = new System.ComponentModel.BindingList<HisTransactionDiscountADO>();
                this.gridControlDiscount.DataSource = this.bindingSourceDiscount;
                this.gridControlDiscount.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Wrapper được gọi từ Load — giữ tên cũ để chỗ khác trong code khỏi đổi.</summary>
        private void InitDiscountGridBinding()
        {
            WireUpDiscountGrid();
        }

        /// <summary>Reload binding với danh sách chiết khấu hiện tại.</summary>
        internal void LoadDiscountGridDataSource(List<HisTransactionDiscountADO> data)
        {
            try
            {
                if (this.gridControlDiscount == null || !this.isDiscountGridWiredUp) return;
                if (data == null) data = new List<HisTransactionDiscountADO>();

                var bindingList = new System.ComponentModel.BindingList<HisTransactionDiscountADO>(data);

                this.gridViewDiscount.BeginUpdate();
                this.bindingSourceDiscount.DataSource = bindingList;
                this.gridControlDiscount.RefreshDataSource();
                this.gridViewDiscount.EndUpdate();
                this.discountDeletedIds.Clear();
                RecalculateTotalDiscountFromGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Lấy danh sách dòng chiết khấu hiện tại trên grid (đã loại các dòng trống).</summary>
        internal List<HisTransactionDiscountADO> GetDiscountGridData()
        {
            List<HisTransactionDiscountADO> result = new List<HisTransactionDiscountADO>();
            try
            {
                if (this.gridControlDiscount == null || !this.isDiscountGridWiredUp) return result;
                this.gridViewDiscount.CloseEditor();
                this.gridViewDiscount.UpdateCurrentRow();

                var data = this.bindingSourceDiscount.DataSource as IEnumerable<HisTransactionDiscountADO>;
                if (data == null) return result;

                foreach (var item in data)
                {
                    if (item == null) continue;
                    if ((item.DISCOUNT ?? 0) > 0 || (item.DISCOUNT_RATIO ?? 0) > 0 || !string.IsNullOrWhiteSpace(item.REASON))
                    {
                        result.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>this.totalDiscount = tổng cột DISCOUNT trên grid, sau đó gọi CalcuCanThu.</summary>
        private void RecalculateTotalDiscountFromGrid()
        {
            try
            {
                var data = this.bindingSourceDiscount != null
                    ? this.bindingSourceDiscount.DataSource as IEnumerable<HisTransactionDiscountADO>
                    : null;

                decimal sum = 0;
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        if (item != null && item.DISCOUNT.HasValue)
                            sum += item.DISCOUNT.Value;
                    }
                }
                this.totalDiscount = sum;
                CalcuCanThu();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewDiscount_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("gridViewDiscount_CellValueChanged FieldName=" + (e.Column != null ? e.Column.FieldName : "null")
                    + " RowHandle=" + e.RowHandle
                    + " Value=" + (e.Value == null ? "null" : e.Value.ToString())
                    + " totalPatientPrice=" + this.totalPatientPrice);

                if (isCellValueChangedFromCode) return;

                var view = sender as GridView;
                if (view == null) return;

                isCellValueChangedFromCode = true;
                try
                {
                    if (e.Column.FieldName == "DISCOUNT")
                    {
                        decimal discount = e.Value == null ? 0 : Convert.ToDecimal(e.Value);
                        decimal ratio = 0;
                        if (this.totalPatientPrice > 0)
                            ratio = Math.Round((discount / this.totalPatientPrice) * 100m, 4);
                        view.SetRowCellValue(e.RowHandle, this.gcDiscountRatio, ratio);
                    }
                    else if (e.Column.FieldName == "DISCOUNT_RATIO")
                    {
                        decimal ratio = e.Value == null ? 0 : Convert.ToDecimal(e.Value);
                        decimal discount = Math.Round((ratio * this.totalPatientPrice) / 100m, 4);
                        view.SetRowCellValue(e.RowHandle, this.gcDiscount, discount);
                    }
                }
                finally
                {
                    isCellValueChangedFromCode = false;
                }

                RecalculateTotalDiscountFromGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewDiscount_InvalidRowException(object sender, InvalidRowExceptionEventArgs e)
        {
            try
            {
                e.ExceptionMode = ExceptionMode.NoAction;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewDiscount_ValidateRow(object sender, ValidateRowEventArgs e)
        {
            try
            {
                var view = sender as GridView;
                if (view == null) return;
                var reason = view.GetRowCellValue(e.RowHandle, this.gcDiscountReason) as string;
                if (!string.IsNullOrEmpty(reason))
                {
                    int byteLen = System.Text.Encoding.UTF8.GetByteCount(reason);
                    if (byteLen > 250)
                    {
                        e.Valid = false;
                        view.SetColumnError(this.gcDiscountReason, "Lý do tối đa 250 ký tự");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repoBtnDeleteDiscount_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (this.gridViewDiscount == null) return;
                int rowHandle = this.gridViewDiscount.FocusedRowHandle;
                if (rowHandle < 0) return;

                var row = this.gridViewDiscount.GetRow(rowHandle) as HisTransactionDiscountADO;
                if (row != null && row.ID > 0)
                {
                    if (!this.discountDeletedIds.Contains(row.ID))
                        this.discountDeletedIds.Add(row.ID);
                }

                this.gridViewDiscount.DeleteRow(rowHandle);
                RecalculateTotalDiscountFromGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Gọi sau Lưu thành công: gửi xóa các dòng đã bị user xóa trên grid.</summary>
        internal void ProcessDeletedDiscountRows()
        {
            try
            {
                if (this.discountDeletedIds == null || this.discountDeletedIds.Count == 0) return;

                foreach (var id in this.discountDeletedIds.ToList())
                {
                    try
                    {
                        CommonParam param = new CommonParam();
                        var ok = new BackendAdapter(param).Post<bool>(
                            RequestUriStore.HIS_TRANSACTION_DISCOUNT_DELETE,
                            ApiConsumers.MosConsumer,
                            id,
                            param);
                        if (ok)
                            this.discountDeletedIds.Remove(id);
                    }
                    catch (Exception exi)
                    {
                        Inventec.Common.Logging.LogSystem.Warn(exi);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Validate grid trước khi Save: chặn nếu Lý do > 250 byte UTF-8.
        /// Trả về false + errorMsg nếu invalid.
        /// </summary>
        internal bool ValidateDiscountGridBeforeSave(out string errorMsg)
        {
            errorMsg = null;
            try
            {
                if (this.gridControlDiscount == null || !this.isDiscountGridWiredUp) return true;
                this.gridViewDiscount.CloseEditor();
                this.gridViewDiscount.UpdateCurrentRow();

                var rows = GetDiscountGridData();
                int rowIdx = 0;
                foreach (var row in rows)
                {
                    rowIdx++;
                    if (!string.IsNullOrEmpty(row.REASON))
                    {
                        int byteLen = System.Text.Encoding.UTF8.GetByteCount(row.REASON);
                        if (byteLen > 250)
                        {
                            errorMsg = string.Format(
                                "Dòng chiết khấu số {0}: Lý do không được vượt quá 250 ký tự (hiện tại {1} ký tự)",
                                rowIdx, byteLen);
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return true;
        }

        /// <summary>Build danh sách HIS_TRANSACTION_DISCOUNT để gắn vào SDO khi Lưu.</summary>
        internal List<HIS_TRANSACTION_DISCOUNT> BuildTransactionDiscountListForSDO(long? treatmentId, long? transactionId)
        {
            List<HIS_TRANSACTION_DISCOUNT> result = new List<HIS_TRANSACTION_DISCOUNT>();
            try
            {
                var rows = GetDiscountGridData();
                foreach (var row in rows)
                {
                    HIS_TRANSACTION_DISCOUNT td = new HIS_TRANSACTION_DISCOUNT();
                    td.ID = row.ID;
                    td.TRANSACTION_ID = transactionId ?? row.TRANSACTION_ID ?? 0;
                    td.TREATMENT_ID = treatmentId ?? row.TREATMENT_ID ?? 0;
                    td.DISCOUNT = row.DISCOUNT ?? 0;
                    td.DISCOUNT_RATIO = (long)Math.Round(row.DISCOUNT_RATIO ?? 0, 0);
                    td.REASON = row.REASON;
                    result.Add(td);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>Build chuỗi EXEMPTION_REASON = các Lý do cách bằng dấu ';' (tối đa 4000 ký tự).</summary>
        internal string BuildExemptionReasonFromGrid()
        {
            try
            {
                var rows = GetDiscountGridData();
                if (rows == null || rows.Count == 0) return string.Empty;
                var reasons = rows
                    .Select(o => o.REASON ?? string.Empty)
                    .Where(o => !string.IsNullOrWhiteSpace(o))
                    .ToList();
                string joined = string.Join(";", reasons);
                if (joined.Length > 4000) joined = joined.Substring(0, 4000);
                return joined;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return string.Empty;
            }
        }

        /// <summary>Load chiết khấu của transaction đang xem (edit mode).</summary>
        internal void LoadDiscountByTransactionId(long transactionId)
        {
            try
            {
                if (this.gridControlDiscount == null || !this.isDiscountGridWiredUp) return;

                CommonParam param = new CommonParam();
                var filter = new
                {
                    TRANSACTION_ID = transactionId,
                    IS_ACTIVE = (short?)1
                };

                var rawData = new BackendAdapter(param).Post<List<HIS_TRANSACTION_DISCOUNT>>(
                    RequestUriStore.HIS_TRANSACTION_DISCOUNT_GET,
                    ApiConsumers.MosConsumer,
                    filter,
                    param);

                // BE chưa có API hoặc TRANSACTION chưa có discount nào -> giữ nguyên grid hiện tại, không wipe
                if (rawData == null || rawData.Count == 0) return;

                var data = new List<HisTransactionDiscountADO>();
                if (rawData != null)
                {
                    foreach (var item in rawData)
                    {
                        data.Add(new HisTransactionDiscountADO
                        {
                            ID = item.ID,
                            TRANSACTION_ID = item.TRANSACTION_ID,
                            TREATMENT_ID = item.TREATMENT_ID,
                            DISCOUNT = item.DISCOUNT,
                            DISCOUNT_RATIO = item.DISCOUNT_RATIO,
                            REASON = item.REASON,
                        });
                    }
                }
                LoadDiscountGridDataSource(data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Reset grid về rỗng (sau Mới).</summary>
        internal void ResetDiscountGrid()
        {
            try
            {
                if (this.gridControlDiscount == null || !this.isDiscountGridWiredUp) return;
                this.discountDeletedIds.Clear();
                LoadDiscountGridDataSource(new List<HisTransactionDiscountADO>());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
