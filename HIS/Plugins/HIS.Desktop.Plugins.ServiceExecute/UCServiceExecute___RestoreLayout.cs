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
using DevExpress.XtraLayout;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.Location;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ServiceExecute
{
    /// <summary>
    /// Save/Restore "Customize Layout" cho UCServiceExecute (DevExpress 15.2).
    /// Hành vi:
    ///   - Khi UC Load:
    ///       + Snapshot layout designer GỐC vào RAM (dùng để reset).
    ///       + Nếu có file ModuleDesign/{ModuleLink}/{layoutControlName}.xml → RestoreLayoutFromXml.
    ///   - Khi user kéo thả LayoutControlItem (qua chuột phải > Customize Layout, hoặc kéo splitter runtime):
    ///       + MouseUp / MouseEnter trên LayoutControl → so sánh snapshot XML hiện tại với snapshot trước đó
    ///       + Nếu khác → auto save vào file XML chuẩn (KHÔNG hỏi user, KHÔNG dùng Save As dialog của DevExpress).
    ///   - Phím tắt Ctrl+Shift+R: khôi phục layout về mặc định + xóa file XML đã lưu.
    /// Bật/tắt qua HIS_CONFIG key "HIS.Desktop.ApplyRestoreLayout.ModuleLinks" (CSV ModuleLink).
    /// Note: DevExpress 15.2 LayoutControl KHÔNG có CustomizationVisibleChanged hay LayoutChanged event public,
    ///       nên dùng MouseUp + MouseEnter + so sánh XML để bắt thay đổi cả 2 luồng:
    ///       (1) kéo splitter inline; (2) customize trong Customization Form rồi quay lại UC.
    /// </summary>
    public partial class UCServiceExecute
    {
        #region Declare RestoreLayout
        private const string CONFIG_KEY__APPLY_RESTORE_LAYOUT = "HIS.Desktop.ApplyRestoreLayout.ModuleLinks";
        private const string MODULE_DESIGN_FOLDER_NAME = "ModuleDesign";

        private bool isAllowRestoreLayout;
        // Khi true: đang khởi tạo/reset → bỏ qua MouseUp/MouseEnter để KHÔNG ghi đè file XML
        private bool isInitializingLayout;

        // Layout designer GỐC — dùng cho phím tắt Ctrl+Shift+R reset
        private readonly Dictionary<LayoutControl, byte[]> defaultLayoutSnapshot
            = new Dictionary<LayoutControl, byte[]>();
        // Snapshot XML mới nhất đã lưu — dùng để so sánh tránh save trùng
        private readonly Dictionary<LayoutControl, string> lastSavedLayoutXml
            = new Dictionary<LayoutControl, string>();
        #endregion

        /// <summary>
        /// Gọi cuối UCServiceExecute_Load (sau ProcessCustomizeUI).
        /// </summary>
        private void InitRestoreLayout()
        {
            try
            {
                isAllowRestoreLayout = CheckAllowRestoreLayout();
                Inventec.Common.Logging.LogSystem.Debug(
                    Inventec.Common.Logging.LogUtil.TraceData(
                        Inventec.Common.Logging.LogUtil.GetMemberName(() => isAllowRestoreLayout), isAllowRestoreLayout));
                if (!isAllowRestoreLayout) return;

                EnsureModuleDesignDirectory();

                HookLayoutControl(this.layoutControl1);
                HookLayoutControl(this.layoutControl2);
                HookLayoutControl(this.layoutControl3);
                HookLayoutControl(this.layoutControl4);
                HookLayoutControl(this.layoutControl5);
                HookLayoutControl(this.lciContentLibrary);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool CheckAllowRestoreLayout()
        {
            try
            {
                string moduleLinksApplys = HisConfigs.Get<string>(CONFIG_KEY__APPLY_RESTORE_LAYOUT);
                if (string.IsNullOrEmpty(moduleLinksApplys)) return false;

                var moduleLinkApplys = moduleLinksApplys.Split(
                    new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in moduleLinkApplys)
                {
                    if (string.Equals(item.Trim(), moduleLink, StringComparison.OrdinalIgnoreCase))
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
                Path.Combine(ApplicationStoreLocation.ApplicationStartupPath, MODULE_DESIGN_FOLDER_NAME),
                moduleLink);
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

        private string GetLayoutFileName(LayoutControl layoutControl)
        {
            return Path.Combine(
                GetModuleDesignDirectory(),
                string.Format("{0}.xml", layoutControl.Name));
        }

        private void HookLayoutControl(LayoutControl layoutControl)
        {
            if (layoutControl == null) return;

            isInitializingLayout = true;
            try
            {
                CaptureDefaultLayoutSnapshot(layoutControl);

                string fileName = GetLayoutFileName(layoutControl);
                if (File.Exists(fileName))
                {
                    layoutControl.RestoreLayoutFromXml(fileName);
                    Inventec.Common.Logging.LogSystem.Debug(
                        "InitRestoreLayout: restored " + layoutControl.Name + " from " + fileName);
                }

                lastSavedLayoutXml[layoutControl] = ReadLayoutAsString(layoutControl);

                layoutControl.MouseUp -= LayoutControl_MouseUp;
                layoutControl.MouseUp += LayoutControl_MouseUp;
                layoutControl.MouseEnter -= LayoutControl_MouseEnter;
                layoutControl.MouseEnter += LayoutControl_MouseEnter;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            finally
            {
                isInitializingLayout = false;
            }
        }

        private void CaptureDefaultLayoutSnapshot(LayoutControl layoutControl)
        {
            try
            {
                using (var ms = new MemoryStream())
                {
                    layoutControl.SaveLayoutToStream(ms);
                    defaultLayoutSnapshot[layoutControl] = ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string ReadLayoutAsString(LayoutControl layoutControl)
        {
            try
            {
                using (var ms = new MemoryStream())
                {
                    layoutControl.SaveLayoutToStream(ms);
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

        private void LayoutControl_MouseUp(object sender, MouseEventArgs e)
        {
            TrySaveLayoutIfChanged(sender as LayoutControl);
        }

        private void LayoutControl_MouseEnter(object sender, EventArgs e)
        {
            TrySaveLayoutIfChanged(sender as LayoutControl);
        }

        /// <summary>
        /// So sánh snapshot XML hiện tại với snapshot lần lưu trước. Nếu khác → save vào file XML.
        /// Cho phép bắt cả 2 luồng: kéo splitter inline (MouseUp) và customize trong Customization Form
        /// rồi di chuột quay lại UC (MouseEnter).
        /// </summary>
        private void TrySaveLayoutIfChanged(LayoutControl layoutControl)
        {
            try
            {
                if (layoutControl == null) return;
                if (isInitializingLayout || !isAllowRestoreLayout) return;

                string current = ReadLayoutAsString(layoutControl);
                if (current == null) return;

                string previous;
                if (lastSavedLayoutXml.TryGetValue(layoutControl, out previous)
                    && string.Equals(previous, current, StringComparison.Ordinal))
                {
                    return;
                }

                EnsureModuleDesignDirectory();
                string fileName = GetLayoutFileName(layoutControl);
                layoutControl.SaveLayoutToXml(fileName);
                lastSavedLayoutXml[layoutControl] = current;

                Inventec.Common.Logging.LogSystem.Info(
                    "Customize layout auto-saved: " + layoutControl.Name + " -> " + fileName);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Khôi phục layout về MẶC ĐỊNH designer + xóa file XML đã lưu.
        /// Gọi qua phím tắt Ctrl+Shift+R (KeyboardWorker).
        /// PUBLIC để KeyboardAction reflect ra method này.
        /// </summary>
        public void ResetLayoutToDefault()
        {
            try
            {
                if (XtraMessageBox.Show(
                        ResourceMessage.BanCoMuonKhoiPhucLayoutMacDinh,
                        ResourceMessage.ThongBao,
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return;
                }

                ResetSingleLayoutControl(this.layoutControl1);
                ResetSingleLayoutControl(this.layoutControl2);
                ResetSingleLayoutControl(this.layoutControl3);
                ResetSingleLayoutControl(this.layoutControl4);
                ResetSingleLayoutControl(this.layoutControl5);
                ResetSingleLayoutControl(this.lciContentLibrary);

                Inventec.Common.Logging.LogSystem.Info("ResetLayoutToDefault: completed");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ResetSingleLayoutControl(LayoutControl layoutControl)
        {
            if (layoutControl == null) return;

            isInitializingLayout = true;
            try
            {
                byte[] bytes;
                if (defaultLayoutSnapshot.TryGetValue(layoutControl, out bytes) && bytes != null)
                {
                    using (var ms = new MemoryStream(bytes))
                    {
                        ms.Position = 0;
                        layoutControl.RestoreLayoutFromStream(ms);
                    }
                }

                lastSavedLayoutXml[layoutControl] = ReadLayoutAsString(layoutControl);

                string fileName = GetLayoutFileName(layoutControl);
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                    Inventec.Common.Logging.LogSystem.Info(
                        "ResetLayout: deleted " + fileName);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            finally
            {
                isInitializingLayout = false;
            }
        }
    }
}
