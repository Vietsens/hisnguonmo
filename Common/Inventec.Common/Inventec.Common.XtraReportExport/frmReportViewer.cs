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
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.UserDesigner;
using Inventec.Common.SignLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.Common.XtraReportExport
{
    public partial class frmReportViewer : Form
    {
        XtraReport report;
        Dictionary<string, object> dicParam;
        Inventec.Common.SignLibrary.ADO.InputADO emrInputADO;

        public frmReportViewer(XtraReport report, Dictionary<string, object> dicParam, Inventec.Common.SignLibrary.ADO.InputADO emrInputADO)
        {
            InitializeComponent();
            this.report = report;
            this.dicParam = dicParam;
            this.emrInputADO = emrInputADO;
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            bbtnEMR.Glyph = imageCollection1.Images[0];
            documentViewer1.DocumentSource = report;
            report.CreateDocument();
        }

        private void bbtnOpenTemplate_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                ReportDesignTool dt = new ReportDesignTool(report);

                // Access the report's properties.
                dt.Report.DrawGrid = false;

                // Access the Designer form's properties.
                dt.DesignForm.SetWindowVisibility(DesignDockPanelType.FieldList |
                    DesignDockPanelType.PropertyGrid, false);

                // Show the Designer form, modally.
                dt.ShowDesignerDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnEMR_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                SignLibraryGUIProcessor libraryProcessor = new SignLibraryGUIProcessor();
                string fileTemp = Inventec.Common.SignLibrary.Utils.GenerateTempFileWithin();
                report.ExportToPdf(fileTemp);
                if (File.Exists(fileTemp))
                {
                    libraryProcessor.ShowPopup(fileTemp, this.emrInputADO);
                    try
                    {
                        File.Delete(fileTemp);
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnTemplateKey_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.dicParam != null && dicParam.Count > 0)
                {
                    TemplateKey.PreviewTemplateKey previewTemplateKey = new TemplateKey.PreviewTemplateKey(this.dicParam);
                    previewTemplateKey.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
