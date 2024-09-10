using System;
using System.IO;

namespace Inventec.Common.ElectronicBill.Misa
{
    class ProcessFile
    {
        public static string GenerateTempFile()
        {
            try
            {
                string pathFolderTemp = GenerateTempFolder();
                return Path.Combine(pathFolderTemp, Guid.NewGuid().ToString() + ".pdf");
            }
            catch (IOException exception)
            {
                Inventec.Common.Logging.LogSystem.Warn("Error create temp file: " + exception.Message);
                return "";
            }
        }

        private static string GenerateTempFolder()
        {
            try
            {
                string pathFolderTemp = GenerateTempFolderByDate();
                if (!Directory.Exists(pathFolderTemp))
                {
                    Directory.CreateDirectory(pathFolderTemp);
                }
                return pathFolderTemp;
            }
            catch (IOException exception)
            {
                Inventec.Common.Logging.LogSystem.Warn("Error create temp file: " + exception.Message);
                return "";
            }
        }

        private static string GenerateTempFolderByDate()
        {
            try
            {
                string pathFolderTemp = Path.Combine(Path.GetTempPath(), DateTime.Now.ToString("ddMMyyyy"));
                if (!Directory.Exists(pathFolderTemp))
                {
                    Directory.CreateDirectory(pathFolderTemp);
                }
                return pathFolderTemp;
            }
            catch (IOException exception)
            {
                Inventec.Common.Logging.LogSystem.Warn("Error create temp file: " + exception.Message);
                return "";
            }
        }
    }
}
