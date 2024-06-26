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
using Inventec.Common.Logging;
using Inventec.Common.SignLibrary.ADO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MPS.ProcessorBase
{
    class Utils
    {
        internal static string GenerateTempFileWithin(string fileName, string extension)
        {
            try
            {
                if (String.IsNullOrEmpty(extension))
                    extension = Path.GetExtension(fileName);

                if (!Directory.Exists(Path.Combine(Path.Combine(Path.Combine(Application.StartupPath, "temp"), DateTime.Now.ToString("ddMMyyyy")), "Templates")))
                {
                    Directory.CreateDirectory(Path.Combine(Path.Combine(Path.Combine(Application.StartupPath, "temp"), DateTime.Now.ToString("ddMMyyyy")), "Templates"));
                }
                return Path.Combine(Path.Combine(Path.Combine(Path.Combine(Application.StartupPath, "temp"), DateTime.Now.ToString("ddMMyyyy")), "Templates"), Guid.NewGuid().ToString() + extension);
            }
            catch (IOException exception)
            {
                Inventec.Common.Logging.LogSystem.Warn("Error create temp file: " + exception.Message, exception);
                return String.Empty;
            }
        }

        internal static PrintConfig.TemplateType GetTemplateTypeFromFile(string fileName)
        {
            PrintConfig.TemplateType templateType = PrintConfig.TemplateType.Excel;
            try
            {
                string extension = Path.GetExtension(fileName);
                switch (extension)
                {
                    case ".xlsx":
                        templateType = PrintConfig.TemplateType.Excel;
                        break;
                    case ".docx":
                        templateType = PrintConfig.TemplateType.Word;
                        break;
                    case ".doc":
                        templateType = PrintConfig.TemplateType.Word;
                        break;
                    case ".repx":
                        templateType = PrintConfig.TemplateType.XtraReport;
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }

            return templateType;
        }

        internal static string GetTempFolder()
        {
            try
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string pathFolder = Path.Combine(path, "SharpDocxReport");
                if (!Directory.Exists(pathFolder))
                {
                    Directory.CreateDirectory(pathFolder);
                }
                return pathFolder;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return System.String.Empty;
        }

        internal static byte[] StreamToByte(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        internal static byte[] FileToByte(string input)
        {
            return File.ReadAllBytes(input);
        }

        internal static void ByteToFile(byte[] arrInFile, string saveFile)
        {
            File.WriteAllBytes(saveFile, arrInFile);
        }
    }
}
