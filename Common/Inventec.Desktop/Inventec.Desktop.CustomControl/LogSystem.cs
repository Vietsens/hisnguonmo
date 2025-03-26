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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.Desktop.CustomControl
{
    internal class LogSystem
    {
        internal static void Log(string messageLog)
        {
            // Example #4: Append new text to an existing file.
            // The using statement automatically flushes AND CLOSES the stream and calls 
            // IDisposable.Dispose on the stream object.
            try
            {
                try
                {
                    if (!System.IO.Directory.Exists(System.IO.Path.Combine(Application.StartupPath, @"Logs")))
                    {
                        System.IO.Directory.CreateDirectory(System.IO.Path.Combine(Application.StartupPath, @"Logs"));
                    }
                }
                catch (Exception exx) { }

                try
                {
                    if (!System.IO.File.Exists(System.IO.Path.Combine(Application.StartupPath, @"Logs\LogSystem_CustomControl.txt")))
                    {
                        System.IO.File.Create(System.IO.Path.Combine(Application.StartupPath, @"Logs\LogSystem_CustomControl.txt"));
                    }
                }
                catch (Exception exx) { }

                using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(System.IO.Path.Combine(Application.StartupPath, @"Logs\LogSystem_CustomControl.txt"), true))
                {
                    file.WriteLine(DateTime.Now.ToString() + " " + messageLog);
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }
    }
}
