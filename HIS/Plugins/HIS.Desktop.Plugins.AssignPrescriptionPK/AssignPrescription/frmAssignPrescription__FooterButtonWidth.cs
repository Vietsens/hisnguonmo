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
using HIS.Desktop.Plugins.AssignPrescriptionPK.Base;
using System;
using System.Collections.Generic;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.AssignPrescription
{
    public partial class frmAssignPrescription : HIS.Desktop.Utility.FormBase
    {
        /// <summary>Design width of emptySpaceItem10, the leading space of the footer button row</summary>
        const int FOOTER_LEFT_SPACE_WIDTH = 11;
        FooterButtonWidthEqualizer footerButtonWidthEqualizer;
        bool isFooterButtonWidthRequested;

        /// <summary>
        /// Footer buttons (and the print buttons built by HIS.UC.MenuPrint) get the same width filling the row,
        /// recomputed when the form is resized and whenever the print buttons are rebuilt.
        /// </summary>
        private void InitFooterButtonWidth()
        {
            try
            {
                this.footerButtonWidthEqualizer = new FooterButtonWidthEqualizer(
                    this.layoutControl6,
                    this.emptySpaceItem10,
                    FOOTER_LEFT_SPACE_WIDTH,
                    new List<LayoutControlItem>
                    {
                        this.layoutControlItem33,       // test result
                        this.lcibtnSaveTemplate,        // save template
                        this.layoutControlItem11,       // appointment services
                        this.layoutControlItem10,       // add treatment regimen
                        this.layoutControlItem9,        // order list
                        this.layoutControlItem44,       // AI prescription (hidden when not configured)
                        this.lcibtnSaveAndPrint,        // save and print
                        this.lcibtnSave,                // save
                        this.lbibtnformNew,             // new
                        this.layoutControlItem30,       // create medical record cover
                        this.layoutControlItem34,       // confirm TB treatment
                        this.layoutControlItem59        // deposit QR
                    },
                    new List<BaseLayoutItem> { this.layoutControlItem36 }, // config (icon) button keeps its width
                    new List<KeyValuePair<LayoutControlItem, LayoutControl>>
                    {
                        new KeyValuePair<LayoutControlItem, LayoutControl>(this.lciPrintAssignPrescription, this.layoutControlPrintAssignPrescription),
                        new KeyValuePair<LayoutControlItem, LayoutControl>(this.lciPrintAssignPrescriptionExt, this.layoutControlPrintAssignPrescriptionExt)
                    });
                this.layoutControl6.SizeChanged += this.layoutControl6_SizeChangedForFooterButton;
                this.RequestEqualFooterButtonWidth();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void layoutControl6_SizeChangedForFooterButton(object sender, EventArgs e)
        {
            try
            {
                this.RequestEqualFooterButtonWidth();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Runs the equalizer after the current call stack finishes, so it measures the row once any
        /// pending layoutControl6.BeginUpdate (e.g. around InitMenuToButtonPrint after saving) is closed.
        /// </summary>
        private void RequestEqualFooterButtonWidth()
        {
            try
            {
                if (this.footerButtonWidthEqualizer == null || this.isFooterButtonWidthRequested || this.IsDisposed || !this.IsHandleCreated)
                    return;
                this.isFooterButtonWidthRequested = true;
                this.BeginInvoke(new Action(this.EqualFooterButtonWidth));
            }
            catch (Exception ex)
            {
                this.isFooterButtonWidthRequested = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void EqualFooterButtonWidth()
        {
            try
            {
                this.isFooterButtonWidthRequested = false;
                if (this.footerButtonWidthEqualizer != null && !this.IsDisposed)
                {
                    this.footerButtonWidthEqualizer.Apply();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
