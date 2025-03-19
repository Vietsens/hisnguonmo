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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.VoiceCommand
{
    public class CommandUtils
    {
        internal static string GenerateTempFileWithin()
        {
            try
            {
                string pathFolderTemp = GenerateTempFolderWithin();
                return Path.Combine(pathFolderTemp, Guid.NewGuid().ToString() + ".wav");
            }
            catch (IOException exception)
            {
                Console.WriteLine("Error create temp file: " + exception.Message);
                return "";
            }
        }

        internal static void ResetVoiceCacheWithinTempFolder()
        {
            try
            {
                string tempFolderParent = GenerateTempFolderWithin();
                System.IO.DirectoryInfo di = new DirectoryInfo(tempFolderParent);
                di.Delete(true);
            }
            catch (IOException exception)
            {
                Console.WriteLine("Error ResetVoiceCacheWithinTempFolder: " + exception.Message);                
            }
        }

        internal static string GenerateTempFolderWithin()
        {
            try
            {
                string pathFolderTemp = GenerateTempFolderWithinByDate();
                if (!Directory.Exists(pathFolderTemp))
                {
                    Directory.CreateDirectory(pathFolderTemp);
                }
                return pathFolderTemp;
            }
            catch (IOException exception)
            {
                Console.WriteLine("Error create temp file: " + exception.Message);
                return "";
            }
        }

        internal static string GenerateTempFolderWithinByDate()
        {
            try
            {
                string pathFolderTemp = Path.Combine(Path.Combine(ParentTempFolder(), DateTime.Now.ToString("ddMMyyyy")), "AudioFile");
                if (!Directory.Exists(pathFolderTemp))
                {
                    Directory.CreateDirectory(pathFolderTemp);
                }
                return pathFolderTemp;
            }
            catch (IOException exception)
            {
                Console.WriteLine("Error create temp file: " + exception.Message);
                return "";
            }
        }

        internal static string ParentTempFolder()
        {
            try
            {
                string pathFolderTemp = Path.Combine(Application.StartupPath, "temp");
                return pathFolderTemp;
            }
            catch (IOException exception)
            {
                Console.WriteLine("Error create temp file: " + exception.Message);
                return Application.StartupPath;
            }
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
    }
}
