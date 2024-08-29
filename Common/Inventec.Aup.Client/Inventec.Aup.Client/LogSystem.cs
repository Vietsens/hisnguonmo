using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Inventec.Aup.Client
{
    internal class LogSystem
    {
        internal static void Debug(string messageLog)
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
                    if (!System.IO.File.Exists(System.IO.Path.Combine(Application.StartupPath, @"Logs\LogSystem1.txt")))
                    {
                        System.IO.File.Create(System.IO.Path.Combine(Application.StartupPath, @"Logs\LogSystem1.txt"));
                    }
                }
                catch (Exception exx) { }

                
                using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(System.IO.Path.Combine(Application.StartupPath, @"Logs\LogSystem1.txt"), true))
                {
                    file.WriteLine(messageLog);
                    file.Close();
                }

            }
            catch (Exception ex)
            {
                LogSystem.W(ex);
                //MessageBox.Show(ex.Message);
            }
        }
    }
}
