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
using AutoMapper;
using ClearCanvas.ImageViewer.StudyManagement;
using Inventec.DicomViewer.ADO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.DicomViewer
{
    public class DicomProcessor
    {
        /// <summary>
        /// Show dicom viewer by list path file
        /// </summary>
        /// <param name="files"></param>
        public static void ShowViewer(List<string> files)
        {
            try
            {
                if (files == null || files.Count == 0)
                    throw new ArgumentNullException("files input is null or count = 0");
                string paramfiles = "";

                paramfiles = String.Join("####@####", files);

                var process = new System.Diagnostics.Process();
                process.StartInfo.FileName = "ClearCanvas.Desktop.Executable.exe"; // Path to pacs application.
                process.StartInfo.Arguments = paramfiles;   // Your arguments
                process.Start();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Show dicom viewer by path of one file or path folder of some files
        /// </summary>
        /// <param name="pathFile"></param>
        public static void ShowViewer(string pathFile)
        {
            try
            {
                if (String.IsNullOrEmpty(pathFile))
                    throw new ArgumentNullException("pathFile input is null or count = 0");
                string paramfiles = "";
                List<string> fileList = new List<string>();

                if (System.IO.File.Exists(pathFile))
                    fileList.Add(pathFile);
                else if (System.IO.Directory.Exists(pathFile))
                    fileList.AddRange(System.IO.Directory.GetFiles(pathFile, "*.dcm", System.IO.SearchOption.AllDirectories));

                paramfiles = String.Join("####@####", fileList);

                var process = new System.Diagnostics.Process();
                process.StartInfo.FileName = "ClearCanvas.Desktop.Executable.exe"; // Path to pacs application.
                process.StartInfo.Arguments = paramfiles;   // Your arguments
                process.Start();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Show dicom viewer by path of one file or path folder of some files
        /// </summary>
        /// <param name="pathFile"></param>
        public static void ShowViewer(MemoryStream stream)
        {
            try
            {
                if (stream == null || stream.Length == 0)
                    throw new ArgumentNullException("stream input is null or Length = 0");

                stream.Position = 0;
                ClearCanvas.Dicom.DicomFile dicom = new ClearCanvas.Dicom.DicomFile();
                dicom.Load(stream);

                ISopDataSource dataSource = new LocalSopDataSource(dicom);

                string paramfiles = "";
                List<string> fileList = new List<string>();

                //if (System.IO.File.Exists(pathFile))
                //    fileList.Add(pathFile);
                //else if (System.IO.Directory.Exists(pathFile))
                //    fileList.AddRange(System.IO.Directory.GetFiles(pathFile, "*.dcm", System.IO.SearchOption.AllDirectories));
                //paramfiles = String.Join("####@####", fileList);
                string sopDataSourceAsString = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(dataSource);
                paramfiles = "$$$$@$$$$" + sopDataSourceAsString;
                var process = new System.Diagnostics.Process();
                process.StartInfo.FileName = "ClearCanvas.Desktop.Executable.exe"; // Path to pacs application.
                process.StartInfo.Arguments = paramfiles;   // Your arguments
                process.Start();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Get sop info by dicom file
        /// </summary>
        /// <param name="file">path file dicom</param>
        /// <returns>SopADO</returns>
        public static SopADO GetSop(string file)
        {
            SopADO result = null;
            try
            {
                Sop sop = Sop.Create(file);

                Mapper.CreateMap<Sop, SopADO>();
                result = Mapper.Map<Sop, SopADO>(sop);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        public static SopADO GetSop(ClearCanvas.Dicom.DicomFile file)
        {
            SopADO result = null;
            try
            {
                ISopDataSource dataSource = new LocalSopDataSource(file);
                Sop sop = Sop.Create(dataSource);

                Mapper.CreateMap<Sop, SopADO>();
                result = Mapper.Map<Sop, SopADO>(sop);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
    }
}
