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
