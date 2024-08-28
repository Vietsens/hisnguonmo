using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.Common.XtraReportExport
{
    class Utils
    {
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

        internal static string GenerateTempFileWithin(string fileName)
        {
            try
            {

                string extension = Path.GetExtension(fileName);

                if (!Directory.Exists(Path.Combine(Path.Combine(Application.StartupPath, "temp"), "Templates")))
                {
                    Directory.CreateDirectory(Path.Combine(Path.Combine(Application.StartupPath, "temp"), "Templates"));
                }
                return Path.Combine(Path.Combine(Path.Combine(Application.StartupPath, "temp"), "Templates"), Guid.NewGuid() + extension);
            }
            catch (IOException exception)
            {
                Logging.LogSystem.Warn("Error create temp file: " + exception.Message, exception);
                return String.Empty;
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
                    case TemplateType.Repx:
                        extension = ".repx";
                        break;
                    default:
                        extension = ".xlsx";
                        break;
                }

                if (!Directory.Exists(Path.Combine(Path.Combine(Application.StartupPath, "temp"), "Templates")))
                {
                    Directory.CreateDirectory(Path.Combine(Path.Combine(Application.StartupPath, "temp"), "Templates"));
                }
                return Path.Combine(Path.Combine(Path.Combine(Application.StartupPath, "temp"), "Templates"), Guid.NewGuid() + extension);
            }
            catch (IOException exception)
            {
                Logging.LogSystem.Warn("Error create temp file: " + exception.Message, exception);
                return String.Empty;
            }
        }

        internal static string GenerateTempFolderWithin()
        {
            try
            {
                if (!Directory.Exists(Path.Combine(Path.Combine(Application.StartupPath, "temp"), "Templates")))
                {
                    Directory.CreateDirectory(Path.Combine(Path.Combine(Application.StartupPath, "temp"), "Templates"));
                }
                return Path.Combine(Path.Combine(Application.StartupPath, "temp"), "Templates");
            }
            catch (IOException exception)
            {
                Logging.LogSystem.Warn("Error create temp file: " + exception.Message, exception);
                return String.Empty;
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
            result = String.Format("{0}\\{1}", Application.StartupPath, filename);

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

    }
}
