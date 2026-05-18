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
using DevExpress.XtraGrid.Views.Base;
using System;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExpMestChmsUpdate
{
    /// <summary>
    /// PTTK 36619 (BV HAGL) — Đồng bộ 2 chiều:
    ///   Cột AMOUNT_TRANSFER_MEDI/MATE và NOTE_TRANSFER_MEDI/MATE trên LEFT grid
    ///     ⇄
    ///   spinExpAmount và txtNote phía dưới grid.
    ///
    /// Flag isSyncingInputFromGrid / isSyncingGridFromInput chặn recursive update loop.
    /// Các flag này được khai báo trong frmExpMestChmsUpdate.cs.
    /// </summary>
    public partial class frmExpMestChmsUpdate : HIS.Desktop.Utility.FormBase
    {
        // ================================================================
        // GRID → INPUT: user sửa trực tiếp cell trên LEFT grid
        // ================================================================
        private void gridViewMedicine_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                if (isSyncingGridFromInput) return;
                if (e.RowHandle < 0 || e.Column == null) return;
                if (e.Column.FieldName != "AMOUNT_TRANSFER_MEDI"
                    && e.Column.FieldName != "NOTE_TRANSFER_MEDI") return;

                // PTTK 36619: bật btnAddd khi user nhập trên grid mà chưa chọn dòng
                UpdateBtnAddEnabledByTransferColumns();

                // Chỉ đồng bộ nếu dòng đang sửa trùng dòng đang focus
                if (e.RowHandle != gridViewMedicine.FocusedRowHandle) return;

                var row = gridViewMedicine.GetRow(e.RowHandle) as ADO.HisMedicineInStockADO;
                if (row == null) return;

                isSyncingInputFromGrid = true;
                try
                {
                    if (e.Column.FieldName == "AMOUNT_TRANSFER_MEDI")
                    {
                        spinExpAmount.EditValue = row.AMOUNT_TRANSFER_MEDI ?? (decimal?)null;
                    }
                    else
                    {
                        txtNote.Text = row.NOTE_TRANSFER_MEDI ?? "";
                    }
                }
                finally
                {
                    isSyncingInputFromGrid = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewMaterial_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                if (isSyncingGridFromInput) return;
                if (e.RowHandle < 0 || e.Column == null) return;
                if (e.Column.FieldName != "AMOUNT_TRANSFER_MATE"
                    && e.Column.FieldName != "NOTE_TRANSFER_MATE") return;

                // PTTK 36619: bật btnAddd khi user nhập trên grid mà chưa chọn dòng
                UpdateBtnAddEnabledByTransferColumns();

                if (e.RowHandle != gridViewMaterial.FocusedRowHandle) return;

                var row = gridViewMaterial.GetRow(e.RowHandle) as ADO.HisMaterialInStockADO;
                if (row == null) return;

                isSyncingInputFromGrid = true;
                try
                {
                    if (e.Column.FieldName == "AMOUNT_TRANSFER_MATE")
                    {
                        spinExpAmount.EditValue = row.AMOUNT_TRANSFER_MATE ?? (decimal?)null;
                    }
                    else
                    {
                        txtNote.Text = row.NOTE_TRANSFER_MATE ?? "";
                    }
                }
                finally
                {
                    isSyncingInputFromGrid = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// PTTK 36619 (BV HAGL): Bật btnAddd nếu có ≥ 1 dòng AMOUNT_TRANSFER > 0 trên LEFT grid.
        /// Không tắt — để các nhánh cũ (RowCellClick → currentMediMate) tự quản.
        /// </summary>
        private void UpdateBtnAddEnabledByTransferColumns()
        {
            try
            {
                bool hasTransferRow = false;

                if (listMediInStock != null && listMediInStock.Any(o => (o.AMOUNT_TRANSFER_MEDI ?? 0) > 0))
                    hasTransferRow = true;

                if (!hasTransferRow && listMateInStock != null
                    && listMateInStock.Any(o => (o.AMOUNT_TRANSFER_MATE ?? 0) > 0))
                    hasTransferRow = true;

                if (hasTransferRow) btnAddd.Enabled = true;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        // ================================================================
        // GRID FocusedRowChanged: user đổi dòng bằng phím/click khác cell
        // ================================================================
        private void gridViewMedicine_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            try
            {
                var row = gridViewMedicine.GetRow(e.FocusedRowHandle) as ADO.HisMedicineInStockADO;
                if (row == null) return;
                // Đồng bộ giá trị cell xuống input
                SyncRowTransferToInput_Medi(row);
                // Cập nhật currentMediMate để btnAdd mode đơn lẻ cũ vẫn hoạt động
                this.currentMediMate = new ADO.MediMateTypeADO(row, chkHienThiLo.Checked);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewMaterial_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            try
            {
                var row = gridViewMaterial.GetRow(e.FocusedRowHandle) as ADO.HisMaterialInStockADO;
                if (row == null) return;
                SyncRowTransferToInput_Mate(row);
                this.currentMediMate = new ADO.MediMateTypeADO(row, chkHienThiLo.Checked);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        // ================================================================
        // INPUT → GRID: user sửa spinExpAmount / txtNote phía dưới grid
        // ================================================================
        private void spinExpAmount_TransferSync_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (isSyncingInputFromGrid) return;
                SyncInputToFocusedRow();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtNote_TransferSync_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (isSyncingInputFromGrid) return;
                SyncInputToFocusedRow();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Lấy dòng đang focus trên LEFT grid (theo tab hiện tại) → ghi giá trị spin/text xuống.
        /// </summary>
        private void SyncInputToFocusedRow()
        {
            try
            {
                isSyncingGridFromInput = true;

                // Tab Thuốc (index 0)
                if (xtraTabControlMain.SelectedTabPageIndex == 0)
                {
                    var row = gridViewMedicine.GetRow(gridViewMedicine.FocusedRowHandle) as ADO.HisMedicineInStockADO;
                    if (row != null)
                    {
                        row.AMOUNT_TRANSFER_MEDI = ParseSpinDecimal();
                        row.NOTE_TRANSFER_MEDI = txtNote.Text;
                        RefreshCurrentRow(gridViewMedicine);
                    }
                }
                // Tab Vật tư (index 1)
                else if (xtraTabControlMain.SelectedTabPageIndex == 1)
                {
                    var row = gridViewMaterial.GetRow(gridViewMaterial.FocusedRowHandle) as ADO.HisMaterialInStockADO;
                    if (row != null)
                    {
                        row.AMOUNT_TRANSFER_MATE = ParseSpinDecimal();
                        row.NOTE_TRANSFER_MATE = txtNote.Text;
                        RefreshCurrentRow(gridViewMaterial);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            finally
            {
                isSyncingGridFromInput = false;
            }
        }

        /// <summary>
        /// PTTK 36619: Commit cell editor đang mở trên LEFT grid (Thuốc + Vật tư) trước khi
        /// đọc DataSource. Tránh trường hợp giá trị cell chưa post về ADO instance.
        /// </summary>
        private void CommitGridEditors_Update()
        {
            try
            {
                if (gridViewMedicine != null)
                {
                    gridViewMedicine.CloseEditor();
                    gridViewMedicine.UpdateCurrentRow();
                }
                if (gridViewMaterial != null)
                {
                    gridViewMaterial.CloseEditor();
                    gridViewMaterial.UpdateCurrentRow();
                }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private decimal? ParseSpinDecimal()
        {
            try
            {
                if (spinExpAmount.EditValue == null) return null;
                if (spinExpAmount.EditValue is decimal) return (decimal)spinExpAmount.EditValue;
                decimal val;
                if (decimal.TryParse(spinExpAmount.EditValue.ToString(), out val)) return val;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
            return null;
        }

        private void RefreshCurrentRow(DevExpress.XtraGrid.Views.Grid.GridView view)
        {
            try
            {
                if (view != null) view.RefreshRow(view.FocusedRowHandle);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        // ================================================================
        // BATCH ADD: duyệt LEFT grid tab hiện tại, lấy dòng có AMOUNT_TRANSFER > 0
        // → chuyển sang RIGHT grid (dicMediMateAdo). Trả về số dòng đã thêm.
        // ================================================================
        private int TryBatchAddFromGrid()
        {
            int count = 0;
            try
            {
                // PTTK 36619: PostEditor commit cell editor đang mở (nếu có) trước khi đọc data
                CommitGridEditors_Update();

                // Tab Thuốc
                if (xtraTabControlMain.SelectedTabPageIndex == 0 && listMediInStock != null)
                {
                    // PTTK 36619 BR03: Chỉ thêm dòng có AMOUNT_TRANSFER_MEDI hợp lệ (>0). Bỏ qua dòng trống.
                    var rowsToAdd = listMediInStock
                        .Where(o => (o.AMOUNT_TRANSFER_MEDI ?? 0) > 0)
                        .ToList();
                    Inventec.Common.Logging.LogSystem.Info(
                        "PTTK 36619 (Update Medicine) — listMediInStock.Count=" + listMediInStock.Count
                        + " | rowsToAdd.Count=" + rowsToAdd.Count);
                    if (rowsToAdd.Count == 0) return 0;

                    foreach (var row in rowsToAdd)
                    {
                        try
                        {
                            var ado = new ADO.MediMateTypeADO(row, chkHienThiLo.Checked);
                            ado.EXP_AMOUNT = row.AMOUNT_TRANSFER_MEDI ?? 0;
                            ado.NOTE = row.NOTE_TRANSFER_MEDI ?? "";
                            ado.IsPackage = chkHienThiLo.Checked;
                            if (ado.ExpMedicine != null)
                            {
                                ado.ExpMedicine.Amount = ado.EXP_AMOUNT;
                                ado.ExpMedicine.Description = ado.NOTE;
                            }
                            // PTTK 36619: dùng key duy nhất MEDICINE_ID (SERVICE_ID có thể trùng/0)
                            long dictKey = ado.MEDICINE_ID > 0 ? ado.MEDICINE_ID : ado.SERVICE_ID;
                            dicMediMateAdo[dictKey] = ado;
                            count++;

                            Inventec.Common.Logging.LogSystem.Info(
                                "PTTK 36619 (Update) added Medicine: " + ado.MEDI_MATE_TYPE_NAME
                                + " | MEDICINE_ID=" + ado.MEDICINE_ID
                                + " | SERVICE_ID=" + ado.SERVICE_ID
                                + " | dictKey=" + dictKey
                                + " | EXP_AMOUNT=" + ado.EXP_AMOUNT);

                            // Reset cell giá trị để lần sau không thêm lại cùng dòng
                            row.AMOUNT_TRANSFER_MEDI = null;
                            row.NOTE_TRANSFER_MEDI = null;
                        }
                        catch (Exception exItem)
                        {
                            Inventec.Common.Logging.LogSystem.Error(
                                "PTTK 36619 (Update) batch add Medicine failed for: "
                                + (row != null ? row.MEDICINE_TYPE_NAME : "null"), exItem);
                        }
                    }
                    gridViewMedicine.RefreshData();
                }
                // Tab Vật tư
                else if (xtraTabControlMain.SelectedTabPageIndex == 1 && listMateInStock != null)
                {
                    var rowsToAdd = listMateInStock
                        .Where(o => (o.AMOUNT_TRANSFER_MATE ?? 0) > 0)
                        .ToList();
                    Inventec.Common.Logging.LogSystem.Info(
                        "PTTK 36619 (Update Material) — listMateInStock.Count=" + listMateInStock.Count
                        + " | rowsToAdd.Count=" + rowsToAdd.Count);
                    if (rowsToAdd.Count == 0) return 0;

                    foreach (var row in rowsToAdd)
                    {
                        try
                        {
                            var ado = new ADO.MediMateTypeADO(row, chkHienThiLo.Checked);
                            ado.EXP_AMOUNT = row.AMOUNT_TRANSFER_MATE ?? 0;
                            ado.NOTE = row.NOTE_TRANSFER_MATE ?? "";
                            ado.IsPackage = chkHienThiLo.Checked;
                            if (ado.ExpMaterial != null)
                            {
                                ado.ExpMaterial.Amount = ado.EXP_AMOUNT;
                                ado.ExpMaterial.Description = ado.NOTE;
                            }
                            // PTTK 36619: dùng key duy nhất MATERIAL_ID
                            long dictKey = ado.MATERIAL_ID > 0 ? ado.MATERIAL_ID : ado.SERVICE_ID;
                            dicMediMateAdo[dictKey] = ado;
                            count++;

                            Inventec.Common.Logging.LogSystem.Info(
                                "PTTK 36619 (Update) added Material: " + ado.MEDI_MATE_TYPE_NAME
                                + " | MATERIAL_ID=" + ado.MATERIAL_ID
                                + " | SERVICE_ID=" + ado.SERVICE_ID
                                + " | dictKey=" + dictKey
                                + " | EXP_AMOUNT=" + ado.EXP_AMOUNT);

                            row.AMOUNT_TRANSFER_MATE = null;
                            row.NOTE_TRANSFER_MATE = null;
                        }
                        catch (Exception exItem)
                        {
                            Inventec.Common.Logging.LogSystem.Error(
                                "PTTK 36619 (Update) batch add Material failed for: "
                                + (row != null ? row.MATERIAL_TYPE_NAME : "null"), exItem);
                        }
                    }
                    gridViewMaterial.RefreshData();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return count;
        }

        /// <summary>
        /// Đăng ký các event sync — gọi trong Load event của form sau khi controls đã init.
        /// </summary>
        private void RegisterTransferSyncEvents()
        {
            try
            {
                // Tránh đăng ký trùng
                spinExpAmount.EditValueChanged -= spinExpAmount_TransferSync_EditValueChanged;
                spinExpAmount.EditValueChanged += spinExpAmount_TransferSync_EditValueChanged;

                txtNote.EditValueChanged -= txtNote_TransferSync_EditValueChanged;
                txtNote.EditValueChanged += txtNote_TransferSync_EditValueChanged;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
