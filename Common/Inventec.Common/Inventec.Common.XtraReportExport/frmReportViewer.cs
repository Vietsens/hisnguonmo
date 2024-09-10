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
