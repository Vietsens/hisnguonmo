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
using DevExpress.XtraTreeList;
using HIS.Desktop.Plugins.ContentSubclinical.Base;
using Inventec.Desktop.Common.LanguageManager;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ContentSubclinical
{
    /// <summary>
    /// Task 57971 - tree / form events of the result tab mode (tab bar: frmContentSubclinical__Pluss__ClsTab.cs).
    /// </summary>
    public partial class frmContentSubclinical : HIS.Desktop.Utility.FormBase
    {
        private void treeListServiceReq_BeforeCollapse(object sender, BeforeCollapseEventArgs e)
        {
            try
            {
                // Tab mode: results stay expanded (no expand / collapse)
                if (this.IsClsTabActive)
                    e.CanCollapse = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Tab mode keeps the designed tree (parent field of the tabs, SRI column driven by the tab): the layout saved
        /// for the tab-mode tree (HIS.Desktop.ApplyRestoreLayout.ModuleLinks) is not restored.
        /// </summary>
        private void treeListServiceReq_BeforeLoadLayout(object sender, DevExpress.Utils.LayoutAllowEventArgs e)
        {
            try
            {
                if (this.IsClsTabActive)
                    e.Allow = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeListServiceReq_CustomDrawEmptyArea(object sender, CustomDrawEmptyAreaEventArgs e)
        {
            try
            {
                if (!this.IsClsTabActive || this.treeListServiceReq.Nodes.Count > 0)
                    return;
                // Test tab emptied by a test filter: say so instead of blaming the date range
                bool isFiltered = this.clsTabCurrentCode == ClsTabGroupStore.GROUP_XN && (this.chkAbove.Checked || this.chkBelow.Checked || this.chkImportant.Checked);
                string text = Inventec.Common.Resource.Get.Value(isFiltered ? "frmContentSubclinical.ClsTab.EmptyFilteredText" : "frmContentSubclinical.ClsTab.EmptyText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                if (String.IsNullOrEmpty(text))
                    return;
                Color backColor = this.treeListServiceReq.Appearance.Empty.BackColor;
                if (backColor.IsEmpty || backColor == Color.Transparent)
                    backColor = SystemColors.Window;
                using (SolidBrush backBrush = new SolidBrush(backColor))
                {
                    e.Graphics.FillRectangle(backBrush, e.Bounds);
                }
                Rectangle textBounds = new Rectangle(e.Bounds.X, e.Bounds.Y + 12, e.Bounds.Width, Math.Max(20, e.Bounds.Height - 12));
                using (StringFormat format = new StringFormat())
                {
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Near;
                    e.Graphics.DrawString(text, this.treeListServiceReq.Font, SystemBrushes.GrayText, textBounds, format);
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
