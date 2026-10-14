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
using System;
using System.Drawing;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.BedRoomPartial
{
    /// <summary>
    /// Viec 57971: support for the paraclinical group tabs of the Bed room screen.
    /// </summary>
    public partial class UCTreeListService : UserControl
    {
        /// <summary>Text drawn in the empty tree (group tabs only); empty = nothing drawn</summary>
        string emptyText;
        bool isEmptyTextHooked = false;

        /// <summary>
        /// Creates the window handle of the tree. A TreeList that never had a handle (its tab page was never shown)
        /// keeps the node structure of its first binding on later DataSource changes; owning a handle makes every
        /// rebind correct, so the screen no longer needs to cycle through all tab pages after each load.
        /// </summary>
        public void EnsureTreeHandle()
        {
            try
            {
                if (this.treeSereServ != null && !this.treeSereServ.IsHandleCreated)
                {
                    IntPtr handle = this.treeSereServ.Handle;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Text shown when the tree has no row, e.g. "Không có y lệnh trong khoảng ngày đã chọn".
        /// </summary>
        public void SetEmptyText(string text)
        {
            try
            {
                this.emptyText = text;
                if (!this.isEmptyTextHooked && this.treeSereServ != null)
                {
                    this.treeSereServ.CustomDrawEmptyArea += new DevExpress.XtraTreeList.CustomDrawEmptyAreaEventHandler(this.treeSereServ_CustomDrawEmptyArea);
                    this.isEmptyTextHooked = true;
                }
                if (this.treeSereServ != null)
                    this.treeSereServ.Invalidate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeSereServ_CustomDrawEmptyArea(object sender, DevExpress.XtraTreeList.CustomDrawEmptyAreaEventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(this.emptyText) || this.treeSereServ.Nodes.Count > 0)
                    return;

                Color backColor = this.treeSereServ.Appearance.Empty.BackColor;
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
                    e.Graphics.DrawString(this.emptyText, this.treeSereServ.Font, SystemBrushes.GrayText, textBounds, format);
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Unhooks the group tab handlers (called from DisposeData).
        /// </summary>
        private void DisposeClsTabData()
        {
            try
            {
                if (this.isEmptyTextHooked && this.treeSereServ != null)
                    this.treeSereServ.CustomDrawEmptyArea -= new DevExpress.XtraTreeList.CustomDrawEmptyAreaEventHandler(this.treeSereServ_CustomDrawEmptyArea);
                this.isEmptyTextHooked = false;
                this.emptyText = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
