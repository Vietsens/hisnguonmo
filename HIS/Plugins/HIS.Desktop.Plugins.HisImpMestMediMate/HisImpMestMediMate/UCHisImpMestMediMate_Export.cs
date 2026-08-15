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
using DevExpress.XtraGrid.Columns;
using Inventec.Desktop.Common.Message;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisImpMestMediMate.HisImpMestMediMate
{
    public partial class UCHisImpMestMediMate : HIS.Desktop.Utility.UserControlBase
    {
        /// <summary>
        /// Xuat Excel dung tap du lieu dang hien thi, bo sung 5 cot gia:
        /// Don gia / Thanh tien / VAT / Don gia (sau VAT) / Thanh tien (sau VAT).
        /// 5 cot nay chi hien khi xuat, sau do an lai de man hinh giu dung bo cot da thong nhat.
        /// </summary>
        public void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                if (GetCurrentRowCount() <= 0)
                {
                    return;
                }

                this.saveFileDialog1.FileName = BuildDefaultFileName();
                if (this.saveFileDialog1.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                var priceColumns = GetPriceColumns();
                WaitingManager.Show();
                bool success = false;
                try
                {
                    ShowPriceColumns(priceColumns, true);
                    this.gridControlData.ExportToXlsx(this.saveFileDialog1.FileName);
                    success = true;
                }
                finally
                {
                    ShowPriceColumns(priceColumns, false);
                    WaitingManager.Hide();
                }

                if (!success)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(
                        Resources.ResourceMessage.XuLyThatBai,
                        Resources.ResourceMessage.ThongBao,
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (DevExpress.XtraEditors.XtraMessageBox.Show(
                        Resources.ResourceMessage.XuatFileThanhCongBanCoMuonMoFile,
                        Resources.ResourceMessage.ThongBao,
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start(this.saveFileDialog1.FileName);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                DevExpress.XtraEditors.XtraMessageBox.Show(
                    Resources.ResourceMessage.XuLyThatBai,
                    Resources.ResourceMessage.ThongBao,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private List<GridColumn> GetPriceColumns()
        {
            if (IsMedicineMode)
            {
                return new List<GridColumn>
                {
                    this.colMedImpPrice,
                    this.colMedTotalPrice,
                    this.colMedVatRatio,
                    this.colMedImpPriceVat,
                    this.colMedTotalPriceVat
                };
            }

            return new List<GridColumn>
            {
                this.colMatImpPrice,
                this.colMatTotalPrice,
                this.colMatVatRatio,
                this.colMatImpPriceVat,
                this.colMatTotalPriceVat
            };
        }

        private void ShowPriceColumns(List<GridColumn> columns, bool visible)
        {
            try
            {
                if (columns == null || columns.Count == 0) return;

                var view = IsMedicineMode
                    ? (DevExpress.XtraGrid.Views.Grid.GridView)this.gridViewMedicine
                    : (DevExpress.XtraGrid.Views.Grid.GridView)this.gridViewMaterial;

                view.BeginUpdate();
                try
                {
                    int nextIndex = view.VisibleColumns.Count;
                    foreach (var column in columns)
                    {
                        if (visible)
                        {
                            column.Visible = true;
                            column.VisibleIndex = nextIndex++;
                        }
                        else
                        {
                            column.Visible = false;
                        }
                    }
                }
                finally
                {
                    view.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string BuildDefaultFileName()
        {
            try
            {
                string prefix = IsMedicineMode ? "TraCuuThuoc" : "TraCuuVatTu";
                string code = IsMedicineMode
                    ? GetSelectedTypeCode(this.cboMedicineType, "MEDICINE_TYPE_CODE")
                    : GetSelectedTypeCode(this.cboMaterialType, "MATERIAL_TYPE_CODE");
                if (!string.IsNullOrWhiteSpace(code))
                {
                    prefix = prefix + "_" + code;
                }
                return prefix + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return "TraCuuThuocVatTu.xlsx";
            }
        }

        private string GetSelectedTypeCode(
            Inventec.Desktop.CustomControl.CustomGridLookUpEditWithFilterMultiColumn cbo, string fieldName)
        {
            try
            {
                var row = cbo.GetSelectedDataRow();
                if (row == null) return "";
                var property = row.GetType().GetProperty(fieldName);
                if (property == null) return "";
                var value = property.GetValue(row, null);
                return value == null ? "" : value.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return "";
            }
        }
    }
}
