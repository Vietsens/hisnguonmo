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
using HIS.Desktop.Plugins.ExpMestChmsCreate.ADO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.ExpMestChmsCreate
{
    /// <summary>
    /// PTTK 36619 (BV HAGL) — Batch add nhanh: user nhập AMOUNT_TRANSFER_MEDICINE / AMOUNT_TRANSFER_MATERIAL
    /// trực tiếp trên LEFT grid rồi bấm "Thêm" — chuyển toàn bộ dòng hợp lệ sang RIGHT grid.
    ///
    /// BR03: Chỉ dòng có AMOUNT_TRANSFER > 0 mới được thêm (bỏ qua dòng trống, không báo lỗi).
    /// BR07: Không đụng tới luồng lưu phiếu sau thêm.
    /// </summary>
    public partial class UCExpMestChmsCreate : HIS.Desktop.Utility.UserControlBase
    {
        // ================================================================
        // ENABLE/DISABLE btnAdd theo trạng thái cột AMOUNT_TRANSFER_*
        // (PTTK 36619): user nhập trực tiếp trên grid mà không chọn dòng nào,
        // không bật chkPlanningExport — nút Thêm vẫn phải hoạt động.
        // ================================================================
        private void gridViewMedicine_TransferCellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                if (e.Column == null) return;
                if (e.Column.FieldName != "AMOUNT_TRANSFER_MEDICINE"
                    && e.Column.FieldName != "NOTE_TRANSFER_MEDICINE") return;
                UpdateBtnAddEnabledByTransferColumns();
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private void gridViewMaterial_TransferCellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                if (e.Column == null) return;
                if (e.Column.FieldName != "AMOUNT_TRANSFER_MATERIAL"
                    && e.Column.FieldName != "NOTE_TRANSFER_MATERIAL") return;
                UpdateBtnAddEnabledByTransferColumns();
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Bật btnAdd nếu có ít nhất 1 dòng AMOUNT_TRANSFER > 0 trên grid tab hiện tại.
        /// Không tắt nếu state hiện tại đã true vì lý do khác (chkPlanningExport / currentMediMate).
        /// PTTK 36619: cũng clear validation warning trên spinExpAmount/txtNote khi batch mode active.
        /// </summary>
        private void UpdateBtnAddEnabledByTransferColumns()
        {
            try
            {
                bool hasTransferRow = false;

                var medSrc = gridControlMedicine.DataSource as List<HisMedicineInStockADO>;
                if (medSrc != null && medSrc.Any(o => (o.AMOUNT_TRANSFER_MEDICINE ?? 0) > 0))
                    hasTransferRow = true;

                if (!hasTransferRow)
                {
                    var matSrc = gridControlMaterial.DataSource as List<HisMaterialInStockADO>;
                    if (matSrc != null && matSrc.Any(o => (o.AMOUNT_TRANSFER_MATERIAL ?? 0) > 0))
                        hasTransferRow = true;
                }

                if (hasTransferRow)
                {
                    btnAdd.Enabled = true;
                    // PTTK 36619: clear icon warning trên vùng nhập phía dưới grid khi batch mode active
                    dxValidationProvider2.RemoveControlError(spinExpAmount);
                    dxValidationProvider2.RemoveControlError(txtNote);
                }
                // Không tắt — để các nhánh cũ (chkPlanningExport / currentMediMate) tự quản
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Đăng ký event sync — gọi trong Load event sau khi controls đã init.
        /// </summary>
        private void RegisterTransferBatchEvents()
        {
            try
            {
                gridViewMedicine.CellValueChanged -= gridViewMedicine_TransferCellValueChanged;
                gridViewMedicine.CellValueChanged += gridViewMedicine_TransferCellValueChanged;

                gridViewMaterial.CellValueChanged -= gridViewMaterial_TransferCellValueChanged;
                gridViewMaterial.CellValueChanged += gridViewMaterial_TransferCellValueChanged;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Duyệt LEFT grid của tab hiện tại, tạo MediMateTypeADO cho dòng có AMOUNT_TRANSFER > 0.
        /// Push vào CẢ `dicMediMateAdo` (cho bulk-mode bind) VÀ `currentMediMate_` (cho single-mode bind)
        /// — caller tự chọn nguồn theo `chkPlanningExport.Checked`.
        /// Set `MEDI_STOCK_ID_IPM` theo radioImport / radioExport (cùng quy tắc với luồng cũ — section btnAdd_Click).
        /// Trả về số dòng đã thêm.
        /// </summary>
        private int TryBatchAddFromTransferColumns()
        {
            int count = 0;
            try
            {
                long? impMediStockIdForExport = null;
                if (radioExport.Checked)
                {
                    if (cboImpMediStock.EditValue == null) return 0; // Chưa chọn kho nhập, không thể thêm
                    impMediStockIdForExport = Inventec.Common.TypeConvert.Parse.ToInt64(cboImpMediStock.EditValue.ToString());
                }

                if (dicMediMateAdo == null) dicMediMateAdo = new Dictionary<long, MediMateTypeADO>();
                if (currentMediMate_ == null) currentMediMate_ = new List<MediMateTypeADO>();

                // Tab Thuốc (index 0)
                if (xtraTabControlMain.SelectedTabPageIndex == 0)
                {
                    var src = gridControlMedicine.DataSource as List<HisMedicineInStockADO>;
                    if (src == null || src.Count == 0) return 0;

                    var rowsToAdd = src.Where(o => (o.AMOUNT_TRANSFER_MEDICINE ?? 0) > 0).ToList();
                    if (rowsToAdd.Count == 0) return 0;

                    foreach (var item in rowsToAdd)
                    {
                        var ado = new MediMateTypeADO(item);
                        ado.EXP_AMOUNT = item.AMOUNT_TRANSFER_MEDICINE ?? 0;
                        ado.NOTE = item.NOTE_TRANSFER_MEDICINE ?? "";
                        ado.IsPackage = chkHienThiLo.Checked;
                        if (ado.ExpMedicine != null)
                        {
                            ado.ExpMedicine.Amount = ado.EXP_AMOUNT;
                            ado.ExpMedicine.Description = ado.NOTE;
                        }
                        // Set MEDI_STOCK_ID_IPM theo chiều xuất (cùng quy tắc luồng cũ)
                        if (radioImport.Checked) ado.MEDI_STOCK_ID_IPM = ado.MEDI_STOCK_ID;
                        else if (radioExport.Checked) ado.MEDI_STOCK_ID_IPM = impMediStockIdForExport;

                        dicMediMateAdo[item.SERVICE_ID] = ado;
                        AddOrReplaceInCurrentMediMateList(ado);
                        count++;

                        // Clear cell sau khi thêm để lần sau không thêm lại trùng
                        item.AMOUNT_TRANSFER_MEDICINE = null;
                        item.NOTE_TRANSFER_MEDICINE = null;
                    }
                    gridViewMedicine.RefreshData();
                }
                // Tab Vật tư (index 1)
                else if (xtraTabControlMain.SelectedTabPageIndex == 1)
                {
                    var src = gridControlMaterial.DataSource as List<HisMaterialInStockADO>;
                    if (src == null || src.Count == 0) return 0;

                    var rowsToAdd = src.Where(o => (o.AMOUNT_TRANSFER_MATERIAL ?? 0) > 0).ToList();
                    if (rowsToAdd.Count == 0) return 0;

                    foreach (var item in rowsToAdd)
                    {
                        var ado = new MediMateTypeADO(item);
                        ado.EXP_AMOUNT = item.AMOUNT_TRANSFER_MATERIAL ?? 0;
                        ado.NOTE = item.NOTE_TRANSFER_MATERIAL ?? "";
                        ado.IsPackage = chkHienThiLo.Checked;
                        if (ado.ExpMaterial != null)
                        {
                            ado.ExpMaterial.Amount = ado.EXP_AMOUNT;
                            ado.ExpMaterial.Description = ado.NOTE;
                        }
                        if (radioImport.Checked) ado.MEDI_STOCK_ID_IPM = ado.MEDI_STOCK_ID;
                        else if (radioExport.Checked) ado.MEDI_STOCK_ID_IPM = impMediStockIdForExport;

                        dicMediMateAdo[item.SERVICE_ID] = ado;
                        AddOrReplaceInCurrentMediMateList(ado);
                        count++;

                        item.AMOUNT_TRANSFER_MATERIAL = null;
                        item.NOTE_TRANSFER_MATERIAL = null;
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
        /// Thêm hoặc thay thế item trong currentMediMate_ theo cùng key luồng cũ
        /// (SERVICE_ID + MEDI_MATE_TYPE_NAME + MEDI_STOCK_ID_IPM + MEDICINE_ID/MATERIAL_ID).
        /// </summary>
        private void AddOrReplaceInCurrentMediMateList(MediMateTypeADO ado)
        {
            try
            {
                var existing = currentMediMate_.FirstOrDefault(o =>
                    o.SERVICE_ID == ado.SERVICE_ID
                    && o.MEDI_MATE_TYPE_NAME == ado.MEDI_MATE_TYPE_NAME
                    && o.MEDI_STOCK_ID_IPM == ado.MEDI_STOCK_ID_IPM
                    && (ado.IsMedicine
                        ? o.MEDICINE_ID == ado.MEDICINE_ID
                        : (!ado.IsMedicine && !ado.IsBlood)
                            ? o.MATERIAL_ID == ado.MATERIAL_ID
                            : o.MEDI_MATE_TYPE_NAME == ado.MEDI_MATE_TYPE_NAME));
                if (existing != null)
                {
                    currentMediMate_.RemoveAll(o => o == existing);
                }
                currentMediMate_.Add(ado);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
