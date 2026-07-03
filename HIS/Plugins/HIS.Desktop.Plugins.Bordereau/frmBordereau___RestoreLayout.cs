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
using DevExpress.XtraGrid.Columns;
using HIS.Desktop.LocalStorage.HisConfig;
using Inventec.Desktop.Common.LibraryMessage;
using Inventec.Desktop.Common.LocalStorage.Location;
using System;
using System.IO;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Bordereau
{
    /// <summary>
    /// Lưu/khôi phục trạng thái layout cột của gridViewBordereau theo người dùng
    /// (tương tự cơ chế màn Chỉ định dịch vụ kỹ thuật - UCServiceExecute).
    /// - Khi Load: snapshot layout GỐC vào RAM; nếu có file ModuleDesign/{ModuleLink}/gridViewBordereau.xml
    ///   → RestoreLayoutFromXml (giữ lại cột mới chưa có trong file: RemoveOldColumns=false).
    /// - Khi user kéo/resize/ẩn-hiện cột → auto-save vào file XML (so sánh snapshot tránh ghi trùng).
    /// - Phím tắt Ctrl+Shift+R: khôi phục layout về mặc định + xóa file đã lưu.
    /// Bật/tắt qua HIS_CONFIG key "HIS.Desktop.ApplyRestoreLayout.ModuleLinks" (CSV ModuleLink).
    /// </summary>
    public partial class frmBordereau
    {
        #region Declare RestoreLayout
        private const string CONFIG_KEY__APPLY_RESTORE_LAYOUT = "HIS.Desktop.ApplyRestoreLayout.ModuleLinks";
        private const string MODULE_DESIGN_FOLDER_NAME = "ModuleDesign";

        private bool isAllowRestoreLayoutGrid;
        // true khi đang khởi tạo/reset → bỏ qua sự kiện đổi cột để KHÔNG ghi đè file XML
        private bool isInitializingGridLayout;

        // Layout GỐC (designer + cột runtime) — dùng cho Ctrl+Shift+R reset
        private byte[] defaultGridLayoutSnapshot;
        // Snapshot XML đã lưu gần nhất — so sánh tránh save trùng
        private string lastSavedGridLayoutXml;
        #endregion

        /// <summary>
        /// Gọi trong frmBordereau_Load (SAU InitPaymentNoteColumn để snapshot/khôi phục đã gồm cột mới).
        /// </summary>
        private void InitRestoreLayoutGrid()
        {
            try
            {
                this.isAllowRestoreLayoutGrid = CheckAllowRestoreLayout();
                Inventec.Common.Logging.LogSystem.Debug(
                    Inventec.Common.Logging.LogUtil.TraceData(
                        Inventec.Common.Logging.LogUtil.GetMemberName(() => isAllowRestoreLayoutGrid), isAllowRestoreLayoutGrid));
                if (!this.isAllowRestoreLayoutGrid)
                    return;

                EnsureModuleDesignDirectory();

                this.isInitializingGridLayout = true;
                try
                {
                    // Giữ lại cột không có trong file layout cũ (vd cột Ghi chú thanh toán mới thêm).
                    this.gridViewBordereau.OptionsLayout.Columns.RemoveOldColumns = false;
                    this.gridViewBordereau.OptionsLayout.Columns.AddNewColumns = true;

                    CaptureDefaultGridLayoutSnapshot();

                    string fileName = GetGridLayoutFileName();
                    if (File.Exists(fileName))
                    {
                        this.gridViewBordereau.RestoreLayoutFromXml(fileName);
                        Inventec.Common.Logging.LogSystem.Debug(
                            "InitRestoreLayoutGrid: restored gridViewBordereau from " + fileName);
                    }

                    this.lastSavedGridLayoutXml = ReadGridLayoutAsString();

                    this.gridViewBordereau.ColumnWidthChanged -= GridViewBordereau_LayoutChanged;
                    this.gridViewBordereau.ColumnWidthChanged += GridViewBordereau_LayoutChanged;
                    this.gridViewBordereau.ColumnPositionChanged -= GridViewBordereau_LayoutChanged_Obj;
                    this.gridViewBordereau.ColumnPositionChanged += GridViewBordereau_LayoutChanged_Obj;
                }
                finally
                {
                    this.isInitializingGridLayout = false;
                }
            }
            catch (Exception ex)
            {
                this.isInitializingGridLayout = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool CheckAllowRestoreLayout()
        {
            try
            {
                string moduleLinksApplys = HisConfigs.Get<string>(CONFIG_KEY__APPLY_RESTORE_LAYOUT);
                if (string.IsNullOrEmpty(moduleLinksApplys))
                    return false;

                var moduleLinkApplys = moduleLinksApplys.Split(
                    new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in moduleLinkApplys)
                {
                    if (string.Equals(item.Trim(), this.moduleLink, StringComparison.OrdinalIgnoreCase))
                        return true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return false;
        }

        private string GetModuleDesignDirectory()
        {
            return Path.Combine(
                Path.Combine(ApplicationStoreLocation.ApplicationDirectory, MODULE_DESIGN_FOLDER_NAME),
                this.moduleLink);
        }

        private void EnsureModuleDesignDirectory()
        {
            try
            {
                string dir = GetModuleDesignDirectory();
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string GetGridLayoutFileName()
        {
            return Path.Combine(GetModuleDesignDirectory(), "gridViewBordereau.xml");
        }

        private void CaptureDefaultGridLayoutSnapshot()
        {
            try
            {
                using (var ms = new MemoryStream())
                {
                    this.gridViewBordereau.SaveLayoutToStream(ms);
                    this.defaultGridLayoutSnapshot = ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string ReadGridLayoutAsString()
        {
            try
            {
                using (var ms = new MemoryStream())
                {
                    this.gridViewBordereau.SaveLayoutToStream(ms);
                    ms.Position = 0;
                    using (var sr = new StreamReader(ms))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }

        // ColumnWidthChanged: ColumnEventHandler
        private void GridViewBordereau_LayoutChanged(object sender, DevExpress.XtraGrid.Views.Base.ColumnEventArgs e)
        {
            TrySaveGridLayoutIfChanged();
        }

        // ColumnPositionChanged: EventHandler
        private void GridViewBordereau_LayoutChanged_Obj(object sender, EventArgs e)
        {
            TrySaveGridLayoutIfChanged();
        }

        /// <summary>
        /// So sánh snapshot XML hiện tại với lần lưu trước. Nếu khác → save vào file XML.
        /// </summary>
        private void TrySaveGridLayoutIfChanged()
        {
            try
            {
                if (this.isInitializingGridLayout || !this.isAllowRestoreLayoutGrid)
                    return;

                string current = ReadGridLayoutAsString();
                if (current == null)
                    return;

                if (this.lastSavedGridLayoutXml != null
                    && string.Equals(this.lastSavedGridLayoutXml, current, StringComparison.Ordinal))
                {
                    return;
                }

                EnsureModuleDesignDirectory();
                string fileName = GetGridLayoutFileName();
                this.gridViewBordereau.SaveLayoutToXml(fileName);
                this.lastSavedGridLayoutXml = current;

                Inventec.Common.Logging.LogSystem.Info(
                    "Grid layout auto-saved: gridViewBordereau -> " + fileName);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Khôi phục layout grid về MẶC ĐỊNH + xóa file XML đã lưu. Gọi qua Ctrl+Shift+R.
        /// </summary>
        private void ResetGridLayoutToDefault()
        {
            try
            {
                if (!this.isAllowRestoreLayoutGrid)
                    return;

                if (XtraMessageBox.Show(
                        GetPaymentNoteRes("frmBordereau.Message.RestoreDefaultLayout", "Bạn có muốn khôi phục lại bố cục cột mặc định không?"),
                        MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return;
                }

                this.isInitializingGridLayout = true;
                try
                {
                    if (this.defaultGridLayoutSnapshot != null)
                    {
                        using (var ms = new MemoryStream(this.defaultGridLayoutSnapshot))
                        {
                            ms.Position = 0;
                            this.gridViewBordereau.RestoreLayoutFromStream(ms);
                        }
                    }

                    this.lastSavedGridLayoutXml = ReadGridLayoutAsString();

                    string fileName = GetGridLayoutFileName();
                    if (File.Exists(fileName))
                    {
                        File.Delete(fileName);
                        Inventec.Common.Logging.LogSystem.Info("ResetGridLayout: deleted " + fileName);
                    }
                }
                finally
                {
                    this.isInitializingGridLayout = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, Keys keyData)
        {
            try
            {
                if (keyData == (Keys.Control | Keys.Shift | Keys.R))
                {
                    ResetGridLayoutToDefault();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
