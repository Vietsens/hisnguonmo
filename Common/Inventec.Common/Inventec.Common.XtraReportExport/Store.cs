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
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.XtraReportExport
{
    public class Store
    {
        string templatePath;
        public Dictionary<string, object> DictionaryTemplateKey { get; set; }
        internal XtraReport xtraReport;
        private System.Drawing.Printing.PaperSize PaperSizeDefault { get; set; }

        public Store()
        {
        }

        public bool ReadTemplate(string path)
        {
            bool result = false;
            try
            {
                this.templatePath = path;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => templatePath), templatePath));
                xtraReport = new XtraReport();
                xtraReport.LoadLayout(this.templatePath);

                result = true;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = false;
                this.templatePath = "";
                this.xtraReport = null;
            }
            return result;
        }

        public bool ReadTemplate(MemoryStream inputStream, TemplateType templateType)
        {
            bool result = false;
            try
            {
                this.templatePath = Utils.GenerateTempFileWithin(templateType);
                Utils.ByteToFile(Utils.StreamToByte(inputStream), this.templatePath);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => templatePath), templatePath));
                if (File.Exists(this.templatePath))
                {
                    xtraReport = new XtraReport();
                    xtraReport.LoadLayout(this.templatePath);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = false;
                this.templatePath = "";
                this.xtraReport = null;
            }
            return result;
        }

        public XtraReport OutFile()
        {
            try
            {
                this.SetPaperSize();
                return this.xtraReport;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return null;
        }

        /// <summary>
        /// Return MemoryStream of Xlsx
        /// </summary>
        /// <returns></returns>
        public MemoryStream OutStream()
        {
            MemoryStream result = new MemoryStream();
            try
            {
                this.SetPaperSize();
                this.xtraReport.ExportToXlsx(result);
                result.Position = 0;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        public MemoryStream OutPdfStream()
        {
            MemoryStream result = new MemoryStream();
            try
            {
                this.SetPaperSize();
                this.xtraReport.ExportToPdf(result);
                result.Position = 0;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        public void ShowPreview(Dictionary<string, object> dicParam, Inventec.Common.SignLibrary.ADO.InputADO emrInputADO)
        {
            try
            {
                ShowPreview(this.xtraReport, dicParam, emrInputADO);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        public void ShowPreview(XtraReport xtraReport, Dictionary<string, object> dicParam, Inventec.Common.SignLibrary.ADO.InputADO emrInputADO)
        {
            try
            {
                frmReportViewer frmReportViewer = new frmReportViewer(xtraReport, dicParam, emrInputADO);
                frmReportViewer.ShowDialog();

            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        public System.Drawing.Printing.PaperSize GetPaperSizeDefault()
        {
            return PaperSizeDefault;
        }

        public bool SetCommonFunctions()
        {
            return true;
        }

        private void SetPaperSize()
        {
            try
            {
                PaperSizeDefault = new System.Drawing.Printing.PaperSize(this.xtraReport.PaperName, this.xtraReport.PageWidth, this.xtraReport.PageHeight);
                PaperSizeDefault.RawKind = (int)this.xtraReport.PaperKind;
            }
            catch (Exception exx)
            {
                LogSystem.Warn(exx);
            }
        }
    }
}
