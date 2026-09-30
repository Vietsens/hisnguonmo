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
using HIS.Desktop.Plugins.Library.ExpMestAttachFile.Config;
using System;
using System.Collections.Generic;
using System.IO;

namespace HIS.Desktop.Plugins.Library.ExpMestAttachFile.Base
{
    /// <summary>
    /// v57853 - Validate format/size BEFORE a file is read into memory (requirement #3, scenario 2).
    /// </summary>
    internal class AttachFileValidator
    {
        /// <summary>
        /// Build AttachFileADO for each valid path; invalid paths produce one warning line each.
        /// </summary>
        internal static List<AttachFileADO> LoadFiles(string[] paths, List<string> errors)
        {
            List<AttachFileADO> result = new List<AttachFileADO>();
            try
            {
                if (paths == null) return result;
                long maxBytes = AttachFileConfig.MaxFileSizeBytes;
                foreach (string path in paths)
                {
                    string fileName = Path.GetFileName(path);
                    string extension = Path.GetExtension(path) ?? "";
                    if (!AttachFileConfig.ALLOWED_EXTENSIONS.Contains(extension))
                    {
                        errors.Add(String.Format(Resources.ResourceMessage.FileKhongDungDinhDang, fileName));
                        continue;
                    }

                    FileInfo info = new FileInfo(path);
                    if (!info.Exists) continue;
                    if (info.Length > maxBytes)
                    {
                        errors.Add(BuildSizeError(fileName, info.Length));
                        continue;
                    }

                    AttachFileADO ado = new AttachFileADO();
                    ado.FileName = fileName;
                    ado.FileSize = info.Length;
                    if (extension.Equals(".pdf", StringComparison.OrdinalIgnoreCase))
                    {
                        ado.PdfPath = path;
                    }
                    else
                    {
                        ado.Image = LoadImageWithoutLock(path);
                        if (ado.Image == null)
                        {
                            errors.Add(String.Format(Resources.ResourceMessage.FileKhongDungDinhDang, fileName));
                            continue;
                        }
                    }
                    result.Add(ado);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>
        /// Validate an image captured by the camera module (size measured as JPEG).
        /// Returns null when valid, otherwise the warning message.
        /// </summary>
        internal static string ValidateCapture(System.Drawing.Image image, string fileName, out long size)
        {
            size = 0;
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    size = ms.Length;
                }
                if (size > AttachFileConfig.MaxFileSizeBytes)
                    return BuildSizeError(fileName, size);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return null;
        }

        private static string BuildSizeError(string fileName, long size)
        {
            return String.Format(Resources.ResourceMessage.FileVuotDungLuong,
                fileName,
                Math.Round((decimal)size / (1024 * 1024), 2),
                AttachFileConfig.MaxFileSizeMB);
        }

        /// <summary>Image.FromFile keeps the file locked until dispose -> copy into a Bitmap instead</summary>
        private static System.Drawing.Image LoadImageWithoutLock(string path)
        {
            try
            {
                byte[] bytes = File.ReadAllBytes(path);
                using (MemoryStream ms = new MemoryStream(bytes))
                using (System.Drawing.Image img = System.Drawing.Image.FromStream(ms))
                {
                    return new System.Drawing.Bitmap(img);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return null;
        }
    }
}
