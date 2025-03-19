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

namespace Inventec.UC.UpLoadFile.Design.Template1
{
    internal partial class Template1
    {
        private void btnChoiseFolder_Click(object sender, EventArgs e)
        {

            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "Chọn một folder";
            fbd.RootFolder = Environment.SpecialFolder.MyComputer;
            fbd.ShowNewFolderButton = false;
            if (fbd.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(fbd.SelectedPath))
            {
                string[] fileNames = System.IO.Directory.GetFiles(fbd.SelectedPath);
                ListData.Clear();
                ListStream.Clear();
                for (int i = 0; i < fileNames.Length; i++)
                {
                    FileStream fileStream = new FileStream(fileNames[i], FileMode.Open, FileAccess.Read);
                    if (fileStream.CanRead)
                    {
                        ListStream.Add(fileStream);
                        Data.DataShowGridControl data = new Data.DataShowGridControl() { FILE_NAME = fileStream.Name.Substring(fileStream.Name.LastIndexOf('\\') + 1), FILE_LENGTH = (fileStream.Length / 1024).ToString() + " Kb", FILE_STATUS = 0 };
                        ListData.Add(data);
                    }
                }

                if (ListData.Count > 0)
                {
                    gridControlUpLoadFile.DataSource = ListData;
                    foreach (FileStream fs in ListStream)
                    {
                        byte[] byteArr = new byte[fs.Length];
                        fs.Read(byteArr, 0, System.Convert.ToInt32(fs.Length));
                        MemoryStream memoryStream = new MemoryStream(byteArr);
                        if (_UpLoad != null)
                        {
                            timer1.Enabled = true;
                            _UpLoad((object)memoryStream);
                            if (timer1.Enabled)
                            {
                                timer1.Enabled = false;
                                status = 9;
                                gridViewUpLoadFile.SetRowCellValue(index, "FILE_STATUS", 100);
                                gridViewUpLoadFile.RefreshRowCell(index, gridViewUpLoadFile.Columns["FILE_STATUS"]);
                            }
                        }
                        index++;
                    }
                }
            }
        }

        private void btnChoiseFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "All files (*.*)|*.*";
            ofd.Multiselect = true;
            ofd.Title = "Chọn file.";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string[] fileNames = ofd.FileNames;
                ListData.Clear();
                ListStream.Clear();
                for (int i = 0; i < fileNames.Length; i++)
                {
                    FileStream fileStream = new FileStream(fileNames[i], FileMode.Open, FileAccess.Read);
                    if (fileStream.CanRead)
                    {
                        ListStream.Add(fileStream);
                        Data.DataShowGridControl data = new Data.DataShowGridControl() { FILE_NAME = fileStream.Name.Substring(fileStream.Name.LastIndexOf('\\') + 1), FILE_LENGTH = (fileStream.Length / 1024).ToString() + " Kb", FILE_STATUS = 0 };
                        ListData.Add(data);
                    }
                    
                }

                if (ListData.Count > 0)
                {
                    gridControlUpLoadFile.DataSource = ListData;
                    foreach (FileStream fs in ListStream)
                    {
                        byte[] byteArr = new byte[fs.Length];
                        fs.Read(byteArr, 0, System.Convert.ToInt32(fs.Length));
                        MemoryStream memoryStream = new MemoryStream(byteArr);
                        if (_UpLoad != null)
                        {
                            timer1.Enabled = true;
                            _UpLoad((object)memoryStream);
                            if (timer1.Enabled)
                            {
                                timer1.Enabled = false;
                                status = 9;
                                gridViewUpLoadFile.SetRowCellValue(index, "FILE_STATUS", 100);
                                gridViewUpLoadFile.RefreshRowCell(index, gridViewUpLoadFile.Columns["FILE_STATUS"]);
                            }
                        }
                        index++;
                    }
 
                }
            }
        }
    }
}
