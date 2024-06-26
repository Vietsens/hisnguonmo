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
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.Common;
using HIS.Desktop.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using PacsOne.AppControl.AppControl;
using System.IO;
using System.Text;

namespace HIS.Desktop.Plugins.HisPacsOne.HisPacsOne
{
    public partial class frmHisPacsOne : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        DelegateSelectData delegateSelect;
        private readonly ApplicationControl _appControl = new ApplicationControl();
        string IntegrateFolder = "Integrate";
        string PacsOneFolder = "PacsOne";
        string RadiantViewerFolder = "RadiantViewer";
        string RadiAntViewer64bitFolder = "RadiAntViewer64bit";
        string COMMONFolder = "COMMON";
        string DicomImagesFolder = "DicomImages";
        string RadiAntViewerFileExe = "RadiAntViewer.exe";

        string[] inputFilePaths;

        #endregion

        #region Construct
        public frmHisPacsOne(Inventec.Desktop.Common.Modules.Module moduleData, DelegateSelectData _delegateSelect, string[] _inputFilePaths)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();
                delegateSelect = _delegateSelect;
                inputFilePaths = _inputFilePaths;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Private method

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisPacsOne.Resources.Lang", typeof(HIS.Desktop.Plugins.HisPacsOne.HisPacsOne.frmHisPacsOne).Assembly);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Init()
        {
            try
            {
                // copy các file option 
                StringBuilder programDataFolder = new StringBuilder();
                StringBuilder programFolder = new StringBuilder();
                programFolder.Append(Application.StartupPath).Append(Path.DirectorySeparatorChar).Append(IntegrateFolder).Append(Path.DirectorySeparatorChar).Append(PacsOneFolder);
                programDataFolder.Append(programFolder.ToString()).Append(Path.DirectorySeparatorChar).Append(RadiantViewerFolder);
                if (!Directory.Exists(programDataFolder.ToString()))
                    Directory.CreateDirectory(programDataFolder.ToString());
                var xzx = String.Format("{0}{1}{2}{3}{4}", programFolder.ToString(), Path.DirectorySeparatorChar, RadiAntViewer64bitFolder, Path.DirectorySeparatorChar, COMMONFolder);
                var x = Directory.GetFiles(xzx);
                foreach (var f in x)
                    File.Copy(f, programDataFolder.ToString() + Path.DirectorySeparatorChar + Path.GetFileName(f), true);
                string agruments = "";
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => inputFilePaths), inputFilePaths));
                if (inputFilePaths != null && inputFilePaths.Length > 0)
                {
                    agruments = inputFilePaths.Aggregate(" -lang vi -f ", (current, p) => current + "\"" + p + "\" ");
                }
                else
                {
                    // Gán đường dẫn file ảnh test (full path)
                    var pathList = Directory.GetFiles(programFolder.ToString() + Path.DirectorySeparatorChar + DicomImagesFolder);
                    agruments = pathList.Aggregate(" -lang vi -f ", (current, p) => current + "\"" + p + "\" ");
                }
                // Gán hàm xử lý sự kiện khi đóng form view
                _appControl.OnClosed += () => { Dispose(true); };

                // Truyền tham số
                _appControl.ExeName = String.Format("{0}{1}{2}{3}{4}", programFolder.ToString(), Path.DirectorySeparatorChar, RadiAntViewer64bitFolder, Path.DirectorySeparatorChar, RadiAntViewerFileExe);
                _appControl.Agruments = agruments;

                // Add lên giao diện
                Controls.Add(_appControl);
                _appControl.Dock = DockStyle.Fill;

                // Show ảnh
                _appControl.ShowViewer();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void frmHisPacsOne_Load(object sender, EventArgs e)
        {
            try
            {
                Init();
                SetCaptionByLanguageKey();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
    }
}
