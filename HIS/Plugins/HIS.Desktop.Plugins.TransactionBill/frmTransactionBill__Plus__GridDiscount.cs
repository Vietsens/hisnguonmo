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
        #region Discount grid fields (programmatic)

        internal GridControl gridControlDiscount;
        internal GridView gridViewDiscount;
        private GridColumn gcDiscount;
        private GridColumn gcDiscountRatio;
        private GridColumn gcDiscountReason;
        private GridColumn gcDiscountDelete;
        private RepositoryItemSpinEdit repoSpinDiscountAmount;
        private RepositoryItemSpinEdit repoSpinDiscountRatio;
        private RepositoryItemTextEdit repoTxtDiscountReason;
        private RepositoryItemButtonEdit repoBtnDeleteDiscount;
        private LayoutControlItem lciDiscountGrid;
        private System.Windows.Forms.BindingSource bindingSourceDiscount;

        private bool isCellValueChangedFromCode = false;
        private List<long> discountDeletedIds = new List<long>();

        #endregion

        /// <summary>
        /// Build grid Chiết khấu programmatically + ẩn 3 ô đơn cũ + đặt grid trên Quỹ hỗ trợ.
        /// </summary>
        private void InitDiscountGridBinding()
        {
            try
            {
                if (this.gridControlDiscount != null) return;

                this.bindingSourceDiscount = new System.Windows.Forms.BindingSource();
                this.gridControlDiscount = new GridControl();
                this.gridViewDiscount = new GridView();
                this.gcDiscount = new GridColumn();
                this.gcDiscountRatio = new GridColumn();
                this.gcDiscountReason = new GridColumn();
                this.gcDiscountDelete = new GridColumn();
                this.repoSpinDiscountAmount = new RepositoryItemSpinEdit();
                this.repoSpinDiscountRatio = new RepositoryItemSpinEdit();
                this.repoTxtDiscountReason = new RepositoryItemTextEdit();
                this.repoBtnDeleteDiscount = new RepositoryItemButtonEdit();
                this.lciDiscountGrid = new LayoutControlItem();

                ((System.ComponentModel.ISupportInitialize)(this.gridControlDiscount)).BeginInit();
                ((System.ComponentModel.ISupportInitialize)(this.gridViewDiscount)).BeginInit();
                ((System.ComponentModel.ISupportInitialize)(this.repoSpinDiscountAmount)).BeginInit();
                ((System.ComponentModel.ISupportInitialize)(this.repoSpinDiscountRatio)).BeginInit();
                ((System.ComponentModel.ISupportInitialize)(this.repoTxtDiscountReason)).BeginInit();
                ((System.ComponentModel.ISupportInitialize)(this.repoBtnDeleteDiscount)).BeginInit();
                ((System.ComponentModel.ISupportInitialize)(this.lciDiscountGrid)).BeginInit();

                this.repoSpinDiscountAmount.AutoHeight = false;
                this.repoSpinDiscountAmount.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
                this.repoSpinDiscountAmount.DisplayFormat.FormatString = "#,##0";
                this.repoSpinDiscountAmount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                this.repoSpinDiscountAmount.EditFormat.FormatString = "#,##0";
                this.repoSpinDiscountAmount.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                this.repoSpinDiscountAmount.MaxValue = 9999999999m;
                this.repoSpinDiscountAmount.Name = "repoSpinDiscountAmount";

                this.repoSpinDiscountRatio.AutoHeight = false;
                this.repoSpinDiscountRatio.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
                this.repoSpinDiscountRatio.DisplayFormat.FormatString = "#,##0.##";
                this.repoSpinDiscountRatio.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                this.repoSpinDiscountRatio.EditFormat.FormatString = "#,##0.##";
                this.repoSpinDiscountRatio.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                this.repoSpinDiscountRatio.MaxValue = 100m;
                this.repoSpinDiscountRatio.Name = "repoSpinDiscountRatio";

                this.repoTxtDiscountReason.AutoHeight = false;
                // MaxLength tính theo CHAR, KHÔNG khớp với DB byte limit khi tiếng Việt có dấu
                // (mỗi char Việt có dấu = 2-3 bytes UTF-8). Đặt rộng để không khoá typing,
                // validate thực sự dùng UTF-8 byte length trong ValidateRow / Save.
                this.repoTxtDiscountReason.MaxLength = 1000;
                this.repoTxtDiscountReason.Name = "repoTxtDiscountReason";

                this.repoBtnDeleteDiscount.AutoHeight = false;
                this.repoBtnDeleteDiscount.Buttons.Clear();
                // Reuse cùng icon X glyph với grid Quỹ hỗ trợ cho thống nhất visual
                System.Drawing.Image deleteImage = null;
                try
                {
                    if (this.repositoryItemBtnDeleteFund != null && this.repositoryItemBtnDeleteFund.Buttons.Count > 0)
                        deleteImage = this.repositoryItemBtnDeleteFund.Buttons[0].Image;
                }
                catch (Exception exImg) { Inventec.Common.Logging.LogSystem.Warn(exImg); }
                if (deleteImage != null)
                {
                    this.repoBtnDeleteDiscount.Buttons.AddRange(new EditorButton[] {
                        new EditorButton(ButtonPredefines.Glyph, "", -1, true, true, false,
                            DevExpress.XtraEditors.ImageLocation.MiddleCenter, deleteImage,
                            new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None),
                            null, null, null, null, "Xóa", null, null, true)
                    });
                }
                else
                {
                    this.repoBtnDeleteDiscount.Buttons.AddRange(new EditorButton[] {
                        new EditorButton(ButtonPredefines.Delete)
                    });
                }
                this.repoBtnDeleteDiscount.Name = "repoBtnDeleteDiscount";
                this.repoBtnDeleteDiscount.TextEditStyle = TextEditStyles.HideTextEditor;
                this.repoBtnDeleteDiscount.ButtonClick += this.repoBtnDeleteDiscount_ButtonClick;

                this.gcDiscount.Caption = "Chiết khấu (đ)";
                this.gcDiscount.FieldName = "DISCOUNT";
                this.gcDiscount.Name = "gcDiscount";
                this.gcDiscount.ColumnEdit = this.repoSpinDiscountAmount;
                this.gcDiscount.Visible = true;
                this.gcDiscount.VisibleIndex = 0;
                this.gcDiscount.Width = 130;

                this.gcDiscountRatio.Caption = "Chiết khấu (%)";
                this.gcDiscountRatio.FieldName = "DISCOUNT_RATIO";
                this.gcDiscountRatio.Name = "gcDiscountRatio";
                this.gcDiscountRatio.ColumnEdit = this.repoSpinDiscountRatio;
                this.gcDiscountRatio.Visible = true;
                this.gcDiscountRatio.VisibleIndex = 1;
                this.gcDiscountRatio.Width = 110;

                this.gcDiscountReason.Caption = "Lý do";
                this.gcDiscountReason.FieldName = "REASON";
                this.gcDiscountReason.Name = "gcDiscountReason";
                this.gcDiscountReason.ColumnEdit = this.repoTxtDiscountReason;
                this.gcDiscountReason.Visible = true;
                this.gcDiscountReason.VisibleIndex = 2;
                this.gcDiscountReason.Width = 259;

                this.gcDiscountDelete.FieldName = "DELETE";
                this.gcDiscountDelete.Name = "gcDiscountDelete";
                this.gcDiscountDelete.ColumnEdit = this.repoBtnDeleteDiscount;
                this.gcDiscountDelete.UnboundType = DevExpress.Data.UnboundColumnType.Object;
                this.gcDiscountDelete.OptionsColumn.ShowCaption = false;
                this.gcDiscountDelete.Visible = true;
                this.gcDiscountDelete.VisibleIndex = 3;
                this.gcDiscountDelete.Width = 41;

                this.gridViewDiscount.Name = "gridViewDiscount";
                this.gridViewDiscount.GridControl = this.gridControlDiscount;
                this.gridViewDiscount.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.True;
                this.gridViewDiscount.OptionsCustomization.AllowColumnMoving = false;
                this.gridViewDiscount.OptionsCustomization.AllowColumnResizing = false;
                this.gridViewDiscount.OptionsMenu.EnableColumnMenu = false;
                this.gridViewDiscount.OptionsNavigation.AutoFocusNewRow = true;
                this.gridViewDiscount.OptionsNavigation.EnterMoveNextColumn = true;
                this.gridViewDiscount.OptionsView.ColumnAutoWidth = false;
                this.gridViewDiscount.OptionsView.NewItemRowPosition = NewItemRowPosition.Bottom;
                this.gridViewDiscount.OptionsView.ShowGroupPanel = false;
                this.gridViewDiscount.OptionsView.ShowIndicator = false;
                this.gridViewDiscount.Columns.AddRange(new GridColumn[] {
                    this.gcDiscount, this.gcDiscountRatio, this.gcDiscountReason, this.gcDiscountDelete
                });
                this.gridViewDiscount.CellValueChanged += this.gridViewDiscount_CellValueChanged;
                this.gridViewDiscount.InvalidRowException += this.gridViewDiscount_InvalidRowException;
                this.gridViewDiscount.ValidateRow += this.gridViewDiscount_ValidateRow;

                this.gridControlDiscount.Name = "gridControlDiscount";
                this.gridControlDiscount.MainView = this.gridViewDiscount;
                this.gridControlDiscount.RepositoryItems.AddRange(new RepositoryItem[] {
                    this.repoSpinDiscountAmount,
                    this.repoSpinDiscountRatio,
                    this.repoTxtDiscountReason,
                    this.repoBtnDeleteDiscount
                });
                this.gridControlDiscount.ViewCollection.AddRange(new BaseView[] { this.gridViewDiscount });

                this.layoutControl1.Controls.Add(this.gridControlDiscount);

                this.lciDiscountGrid.Name = "lciDiscountGrid";
                this.lciDiscountGrid.Control = this.gridControlDiscount;
                this.lciDiscountGrid.AppearanceItemCaption.Options.UseTextOptions = true;
                this.lciDiscountGrid.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                this.lciDiscountGrid.AppearanceItemCaption.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
                this.lciDiscountGrid.Text = "Chiết khấu:";
                this.lciDiscountGrid.TextAlignMode = TextAlignModeItem.CustomSize;
                this.lciDiscountGrid.TextSize = new System.Drawing.Size(90, 0);
                this.lciDiscountGrid.TextToControlDistance = 5;
                // Đặt 2 grid (Chiết khấu + Quỹ hỗ trợ) cân bằng nhau — cùng width, cùng height
                int gridHeight = 70;
                int gridWidth = this.LciBillFund.Size.Width;

                this.lciDiscountGrid.SizeConstraintsType = SizeConstraintsType.Custom;
                this.lciDiscountGrid.MinSize = new System.Drawing.Size(gridWidth, gridHeight);
                this.lciDiscountGrid.MaxSize = new System.Drawing.Size(0, gridHeight);
                this.lciDiscountGrid.Size = new System.Drawing.Size(gridWidth, gridHeight);

                // Force Quỹ hỗ trợ cũng cùng kích thước để 2 grid trông giống nhau
                this.LciBillFund.SizeConstraintsType = SizeConstraintsType.Custom;
                this.LciBillFund.MinSize = new System.Drawing.Size(gridWidth, gridHeight);
                this.LciBillFund.MaxSize = new System.Drawing.Size(0, gridHeight);
                this.LciBillFund.Size = new System.Drawing.Size(gridWidth, gridHeight);

                this.layoutControlGroup1.AddItem(this.lciDiscountGrid);
                try
                {
                    this.lciDiscountGrid.Move(this.LciBillFund, InsertType.Top);
                }
                catch (Exception exMove1)
                {
                    Inventec.Common.Logging.LogSystem.Warn(exMove1);
                }

                this.layoutDiscount.Visibility = LayoutVisibility.Never;
                this.layoutDiscountRatio.Visibility = LayoutVisibility.Never;
                this.layoutReason.Visibility = LayoutVisibility.Never;

                try
                {
                    this.layoutBank.Move(this.layoutDescription, InsertType.Left);
                }
                catch (Exception exMove2)
                {
                    Inventec.Common.Logging.LogSystem.Warn(exMove2);
                }

                ((System.ComponentModel.ISupportInitialize)(this.lciDiscountGrid)).EndInit();
                ((System.ComponentModel.ISupportInitialize)(this.repoBtnDeleteDiscount)).EndInit();
                ((System.ComponentModel.ISupportInitialize)(this.repoTxtDiscountReason)).EndInit();
                ((System.ComponentModel.ISupportInitialize)(this.repoSpinDiscountRatio)).EndInit();
                ((System.ComponentModel.ISupportInitialize)(this.repoSpinDiscountAmount)).EndInit();
                ((System.ComponentModel.ISupportInitialize)(this.gridViewDiscount)).EndInit();
                ((System.ComponentModel.ISupportInitialize)(this.gridControlDiscount)).EndInit();

                this.gridControlDiscount.DataSource = this.bindingSourceDiscount;
                this.bindingSourceDiscount.DataSource = new System.ComponentModel.BindingList<HisTransactionDiscountADO>();
                this.gridControlDiscount.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Reload binding với danh sách chiết khấu hiện tại.</summary>
        internal void LoadDiscountGridDataSource(List<HisTransactionDiscountADO> data)
        {
            try
            {
                if (this.gridControlDiscount == null) return;
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
                if (this.gridControlDiscount == null) return result;
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
        /// Validate grid trước khi Save: chặn nếu Lý do > 250 ký tự.
        /// Trả về false + errorMsg nếu invalid.
        /// </summary>
        internal bool ValidateDiscountGridBeforeSave(out string errorMsg)
        {
            errorMsg = null;
            try
            {
                if (this.gridControlDiscount == null) return true;
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
                if (this.gridControlDiscount == null) return;

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
                if (this.gridControlDiscount == null) return;
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
