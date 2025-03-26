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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.Common.TemplaterExport
{
    class Utils
    {
        internal static bool DocToDocx(MemoryStream inputStream, string inputFile, MemoryStream outputStream, string outputFile, string extension = "")
        {
            bool success = false;
            try
            {
                if ((inputStream == null || inputStream.Length == 0) && System.String.IsNullOrEmpty(inputFile))
                    throw new ArgumentNullException("inStream & inFile is null");

                if (outputStream == null && System.String.IsNullOrEmpty(outputFile))
                {
                    throw new ArgumentNullException("outStream & outFile is null");
                }
                License.LicenceProcess.SetLicenseForAspose();
                Aspose.Words.Document pdfDocument = null;

                if (!System.String.IsNullOrEmpty(inputFile))
                {
                    // Open the source PDF document
                    pdfDocument = new Aspose.Words.Document(inputFile);
                                  
                }
                else if (inputStream != null && inputStream.Length > 0)
                {
                    pdfDocument = new Aspose.Words.Document(inputStream);
                }

                if (outputStream != null)
                {
                    // Save document in docx format
                    pdfDocument.Save(outputStream, Aspose.Words.SaveFormat.Docx);
                    outputStream.Position = 0;
                }
                if (!System.String.IsNullOrEmpty(outputFile))
                {
                    pdfDocument.Save(outputFile, Aspose.Words.SaveFormat.Docx);
                }

                success = true;
            }
            catch (Exception ex)
            {
                LogSystem.Error("Convert to docx fail____inputFile=" + inputFile + "____outputFile=" + outputFile, ex);
            }

            return success;
        }

        internal static string GetImgFolder()
        {
            try
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string pathFolder = Path.Combine(Path.Combine(path, "TemplaterExport"), "ImgTemp");
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

        internal static string GenerateTempFile()
        {
            try
            {
                return Path.GetTempFileName();
            }
            catch (IOException exception)
            {
                Logging.LogSystem.Warn("Error create temp file: " + exception.Message, exception);
                return "";
            }
        }

        internal static string GenerateTempFileWithin(string fileName, string _extension = "")
        {
            try
            {
                string extension = !System.String.IsNullOrEmpty(_extension) ? _extension : Path.GetExtension(fileName);
                string pathDic = Path.Combine(Path.Combine(Path.Combine(ApplicationLocationStore.ApplicationPathLocal, "temp"), DateTime.Now.ToString("ddMMyyyy")), "Templates");
                if (!Directory.Exists(pathDic))
                {
                    Directory.CreateDirectory(pathDic);
                }
                return Path.Combine(pathDic, Guid.NewGuid().ToString() + extension);
            }
            catch (IOException exception)
            {
                Logging.LogSystem.Warn("Error create temp file: " + exception.Message, exception);
                return System.String.Empty;
            }
        }

        internal static string GenerateTempFileWithin(TemplateType templateType)
        {
            try
            {
                string extension = "";
                switch (templateType)
                {
                    case TemplateType.Excel:
                        extension = ".xlsx";
                        break;
                    case TemplateType.Docx:
                        extension = ".docx";
                        break;
                    case TemplateType.Pptx:
                        extension = ".pptx";
                        break;
                    default:
                        extension = ".xlsx";
                        break;
                }
                string pathDic = Path.Combine(Path.Combine(Path.Combine(ApplicationLocationStore.ApplicationPathLocal, "temp"), DateTime.Now.ToString("ddMMyyyy")), "Templates");
                if (!Directory.Exists(pathDic))
                {
                    Directory.CreateDirectory(pathDic);
                }
                return Path.Combine(pathDic, Guid.NewGuid() + extension);
            }
            catch (IOException exception)
            {
                Logging.LogSystem.Warn("Error create temp file: " + exception.Message, exception);
                return System.String.Empty;
            }
        }

        internal static string ParentTempFolder()
        {
            try
            {
                string pathDic = Path.Combine(Path.Combine(ApplicationLocationStore.ApplicationPathLocal, "temp"), "Templates");
                if (!Directory.Exists(pathDic))
                {
                    Directory.CreateDirectory(pathDic);
                }
                return pathDic;
            }
            catch (IOException exception)
            {
                Logging.LogSystem.Warn("Error create temp file: " + exception.Message, exception);
                return System.String.Empty;
            }
        }

        internal static string GenerateTempFolderWithin()
        {
            try
            {
                string pathDic = Path.Combine(Path.Combine(Path.Combine(ApplicationLocationStore.ApplicationPathLocal, "temp"), DateTime.Now.ToString("ddMMyyyy")), "Templates");
                if (!Directory.Exists(pathDic))
                {
                    Directory.CreateDirectory(pathDic);
                }
                return pathDic;
            }
            catch (IOException exception)
            {
                Logging.LogSystem.Warn("Error create temp file: " + exception.Message, exception);
                return System.String.Empty;
            }
        }

        internal static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        internal static string Base64Encode(string dataEncode)
        {
            return System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(dataEncode));
        }

        internal static string GetFullPathFile(string filename)
        {
            string result = "";
            result = System.String.Format("{0}\\{1}", ApplicationLocationStore.ApplicationPathLocal, filename);

            return result;
        }

        internal static MemoryStream GetStreamFromFile(string pathFile)
        {
            MemoryStream result = new MemoryStream();
            try
            {
                result = new MemoryStream();
                using (FileStream file = new FileStream(pathFile, FileMode.Open, FileAccess.Read))
                {
                    byte[] bytes = new byte[file.Length];
                    file.Read(bytes, 0, (int)file.Length);
                    result.Write(bytes, 0, (int)file.Length);
                    result.Position = 0;

                    return result;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = null;
            }
            return result;
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

        internal static string FileToBase64String(string input)
        {
            return System.Convert.ToBase64String(File.ReadAllBytes(input));
        }

        internal static void ByteToFile(byte[] arrInFile, string saveFile)
        {
            File.WriteAllBytes(saveFile, arrInFile);
        }

        internal static FileStream Open(string path)
        {
            try
            {
                return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            }
            catch
            {
                return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            }
        }

    }
}
