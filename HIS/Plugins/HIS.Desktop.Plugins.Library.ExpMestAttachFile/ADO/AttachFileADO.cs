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

namespace HIS.Desktop.Plugins.Library.ExpMestAttachFile.ADO
{
    /// <summary>
    /// One file (image or PDF) waiting to be attached to a sale export ticket.
    /// Kept in memory until the ticket is saved, then merged into a single PDF document on EMR.
    /// </summary>
    public class AttachFileADO
    {
        /// <summary>Display name shown in the file grid</summary>
        public string FileName { get; set; }

        /// <summary>Full path of a PDF file on disk (null for images)</summary>
        public string PdfPath { get; set; }

        /// <summary>Image content (null for PDF)</summary>
        public System.Drawing.Image Image { get; set; }

        /// <summary>Size in bytes used for the size validation</summary>
        public long FileSize { get; set; }

        /// <summary>True when the file came from the camera module</summary>
        public bool IsCapture { get; set; }

        public bool IsPdf
        {
            get { return !String.IsNullOrEmpty(this.PdfPath); }
        }

        public AttachFileADO() { }
    }
}
