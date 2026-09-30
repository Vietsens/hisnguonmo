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
using HIS.Desktop.Plugins.Library.ExpMestAttachFile.ADO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;

namespace HIS.Desktop.Plugins.Library.ExpMestAttachFile.Base
{
    /// <summary>
    /// v57853 - Merge every image/PDF page into ONE PDF (one EMR document per attach action),
    /// same behaviour as the import-ticket attach feature (v42244).
    /// </summary>
    internal class PdfMergeUtil
    {
        /// <summary>Return the merged PDF bytes, or null when nothing could be merged</summary>
        internal static byte[] MergeToPdf(List<AttachFileADO> files)
        {
            byte[] result = null;
            try
            {
                if (files == null || files.Count == 0) return null;
                using (MemoryStream output = new MemoryStream())
                {
                    Document document = new Document(PageSize.A4);
                    PdfCopy writer = new PdfCopy(document, output);
                    document.Open();
                    int pageCount = 0;
                    foreach (AttachFileADO file in files)
                    {
                        PdfReader reader = null;
                        try
                        {
                            reader = file.IsPdf ? new PdfReader(file.PdfPath) : new PdfReader(ImageToSinglePagePdf(file.Image));
                            reader.ConsolidateNamedDestinations();
                            for (int i = 1; i <= reader.NumberOfPages; i++)
                            {
                                writer.AddPage(writer.GetImportedPage(reader, i));
                                pageCount++;
                            }
                        }
                        finally
                        {
                            if (reader != null) reader.Close();
                        }
                    }
                    // Document.Close also closes the PdfCopy writer and the stream (ToArray still works on a closed MemoryStream).
                    // Closing a document without pages throws "The document has no pages" -> only close when merged something.
                    if (pageCount > 0)
                    {
                        document.Close();
                        result = output.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        /// <summary>Put one image on one A4 page, scaled to fit inside the margins (JPEG to keep the file small)</summary>
        private static byte[] ImageToSinglePagePdf(System.Drawing.Image image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Rectangle pageSize = image.Width > image.Height ? PageSize.A4.Rotate() : PageSize.A4;
                Document doc = new Document(pageSize, 20, 20, 20, 20);
                PdfWriter.GetInstance(doc, ms).CloseStream = false;
                doc.Open();
                iTextSharp.text.Image pdfImage = iTextSharp.text.Image.GetInstance(image, System.Drawing.Imaging.ImageFormat.Jpeg);
                pdfImage.ScaleToFit(pageSize.Width - 40, pageSize.Height - 40);
                pdfImage.Alignment = Element.ALIGN_CENTER;
                doc.Add(pdfImage);
                doc.Close();
                return ms.ToArray();
            }
        }
    }
}
