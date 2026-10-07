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
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.Base
{
    /// <summary>
    /// Gives every button of the footer row the same width, filling the row.
    /// The footer buttons are designed with different min/max widths, so when the form is stretched each one
    /// grows to a different width (and the print buttons built by HIS.UC.MenuPrint are sized by their text length).
    /// This helper first relaxes the constraints so the row takes its natural width, then fixes every visible
    /// button - including the print buttons inside the nested print layout controls - to one common width.
    /// </summary>
    internal class FooterButtonWidthEqualizer
    {
        internal const int MIN_BUTTON_WIDTH = 60;
        internal const int MAX_BUTTON_WIDTH = 200;

        LayoutControl hostLayout;
        BaseLayoutItem leftSpaceItem;
        int leftSpaceWidth;
        List<LayoutControlItem> buttonItems;
        List<BaseLayoutItem> fixedItems;
        List<KeyValuePair<LayoutControlItem, LayoutControl>> printContainers;
        bool isProcessing;

        /// <param name="hostLayout">Layout control holding the footer row</param>
        /// <param name="leftSpaceItem">Empty space at the start of the row</param>
        /// <param name="leftSpaceWidth">Width the leading empty space should keep (its design width)</param>
        /// <param name="buttonItems">Footer button items, all get the same width</param>
        /// <param name="fixedItems">Items of the row keeping their own width (e.g. icon button)</param>
        /// <param name="printContainers">Row items hosting a nested layout control filled by HIS.UC.MenuPrint</param>
        internal FooterButtonWidthEqualizer(LayoutControl hostLayout, BaseLayoutItem leftSpaceItem, int leftSpaceWidth, List<LayoutControlItem> buttonItems, List<BaseLayoutItem> fixedItems, List<KeyValuePair<LayoutControlItem, LayoutControl>> printContainers)
        {
            this.hostLayout = hostLayout;
            this.leftSpaceItem = leftSpaceItem;
            this.leftSpaceWidth = leftSpaceWidth;
            this.buttonItems = buttonItems ?? new List<LayoutControlItem>();
            this.fixedItems = fixedItems ?? new List<BaseLayoutItem>();
            this.printContainers = printContainers ?? new List<KeyValuePair<LayoutControlItem, LayoutControl>>();
        }

        internal void Apply()
        {
            if (this.isProcessing || this.hostLayout == null || this.leftSpaceItem == null)
                return;
            this.isProcessing = true;
            try
            {
                List<LayoutControlItem> buttons = this.buttonItems.Where(IsShown).ToList();
                List<BaseLayoutItem> fixeds = this.fixedItems.Where(IsShown).ToList();
                var prints = this.printContainers
                    .Where(o => IsShown(o.Key) && o.Value != null)
                    .Select(o => new { Container = o.Key, Layout = o.Value, Items = GetPrintButtonItems(o.Value) })
                    .ToList();
                var printsWithButton = prints.Where(o => o.Items.Count > 0).ToList();

                int buttonCount = buttons.Count + printsWithButton.Sum(o => o.Items.Count);
                if (buttonCount == 0)
                    return;

                // 1. Relax the constraints so the row takes its natural width.
                this.hostLayout.BeginUpdate();
                try
                {
                    foreach (var item in buttons)
                    {
                        SetWidthRange(item, MIN_BUTTON_WIDTH, 0);
                    }
                    foreach (var print in printsWithButton)
                    {
                        SetWidthRange(print.Container, MIN_BUTTON_WIDTH * print.Items.Count, 0);
                    }
                }
                finally
                {
                    this.hostLayout.EndUpdate();
                }

                // 2. Measure the row and share it between the buttons.
                int fixedWidth = fixeds.Sum(o => o.Width) + prints.Where(o => o.Items.Count == 0).Sum(o => o.Container.Width);
                int rowWidth = this.leftSpaceItem.Width + buttons.Sum(o => o.Width) + printsWithButton.Sum(o => o.Container.Width) + fixedWidth;
                int available = rowWidth - this.leftSpaceWidth - fixedWidth;
                int width = Math.Max(MIN_BUTTON_WIDTH, Math.Min(MAX_BUTTON_WIDTH, available / buttonCount));

                // 3. Fix every button to the same width; the remainder goes back to the leading empty space.
                //    Captions wrap to a second line when the common width is narrower than the text (small screens).
                this.hostLayout.BeginUpdate();
                try
                {
                    foreach (var item in buttons)
                    {
                        SetWidthRange(item, width, width);
                        AllowCaptionWrap(item);
                    }
                    foreach (var print in printsWithButton)
                    {
                        print.Layout.BeginUpdate();
                        try
                        {
                            foreach (var item in print.Items)
                            {
                                SetWidthRange(item, width, width);
                                AllowCaptionWrap(item);
                            }
                        }
                        finally
                        {
                            print.Layout.EndUpdate();
                        }
                        SetWidthRange(print.Container, width * print.Items.Count, width * print.Items.Count);
                    }
                }
                finally
                {
                    this.hostLayout.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            finally
            {
                this.isProcessing = false;
            }
        }

        static bool IsShown(BaseLayoutItem item)
        {
            return item != null && item.Visibility == LayoutVisibility.Always;
        }

        /// <summary>HIS.UC.MenuPrint adds one item per print button into the nested layout root</summary>
        static List<LayoutControlItem> GetPrintButtonItems(LayoutControl layout)
        {
            return layout.Root.Items.OfType<LayoutControlItem>()
                .Where(o => o.Control != null && IsShown(o) && !String.IsNullOrWhiteSpace(o.Control.Text))
                .ToList();
        }

        static void AllowCaptionWrap(LayoutControlItem item)
        {
            DevExpress.XtraEditors.BaseButton button = item.Control as DevExpress.XtraEditors.BaseButton;
            if (button != null)
            {
                button.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            }
        }

        /// <summary>Changes only the width constraints, maxWidth 0 = no limit</summary>
        static void SetWidthRange(LayoutControlItem item, int minWidth, int maxWidth)
        {
            int minHeight = item.MinSize.Height;
            int maxHeight = item.MaxSize.Height;
            item.SizeConstraintsType = SizeConstraintsType.Custom;
            item.MaxSize = new Size(0, maxHeight);
            item.MinSize = new Size(minWidth, minHeight);
            item.MaxSize = new Size(maxWidth, maxHeight);
        }
    }
}
