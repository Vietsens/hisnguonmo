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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.Fss.Utility;
using SAR.Desktop.Plugins.SarReportTemplate.ADO;
using SAR.EFMODEL.DataModels;
using SAR.Filter;
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

namespace SAR.Desktop.Plugins.SarReportTemplate
{
    public partial class frmup : Form
    {
        SAR_REPORT_TEMPLATE ReportTemplate = null;
        DelegateRefreshData deleResultFileUpload;

        public frmup(DelegateRefreshData _deleResultFileUpload, SAR_REPORT_TEMPLATE reporttemplate)
        {
            InitializeComponent();
            this.deleResultFileUpload = _deleResultFileUpload;
            this.ReportTemplate = reporttemplate;
        }

        private void frmup_Load(object sender, EventArgs e)
        {

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                bool success = false;
                CommonParam param = new CommonParam();
                if (txtFile.Text == "")
                {
                    MessageBox.Show("chọn file");
                }
                else
                {
                    if (UpdateReportTemplateURL(true))
                    {
                        var resultData = new BackendAdapter(param).Post<SAR.EFMODEL.DataModels.SAR_REPORT_TEMPLATE>(SarRequestUriStore.SAR_REPORT_TEMPLATE_UPDATE, ApiConsumers.SarConsumer, ReportTemplate, param);
                        if (resultData != null)
                        {
                            success = true;
                            if (deleResultFileUpload != null)
                            {
                                this.deleResultFileUpload();
                            }

                            this.Close();
                        }
                    }
                    else
                    {
                        if (UpdateReportTemplateURL(false))
                        {
                            var resultData = new BackendAdapter(param).Post<SAR.EFMODEL.DataModels.SAR_REPORT_TEMPLATE>(SarRequestUriStore.SAR_REPORT_TEMPLATE_UPDATE, ApiConsumers.SarConsumer, ReportTemplate, param);
                            if (resultData != null)
                            {
                                success = true;
                                if (deleResultFileUpload != null)
                                {
                                    this.deleResultFileUpload();
                                }

                                this.Close();
                            }
                        }
                    }

                    #region Hien thi message thong bao
                    MessageManager.Show(this, param, success);
                    #endregion

                    #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                    SessionManager.ProcessTokenLost(param);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool UpdateReportTemplateURL(bool keepOriginalFileName)
        {
            bool result = true;
            try
            {
                //{"FILE_NAME":"mrs 200 bao cao xuat nhap ton kho toan vien.xlsx.xlsx","URL":"\\\\Upload\\\\SAR\\\\ReportTemplate\\\\mrs 200 bao cao xuat nhap ton kho toan vien.xlsx.xlsx","EXTENSION":""}

                string filename = txtFile.Text.Split('\\').LastOrDefault();

                using (MemoryStream TemplateStream = new MemoryStream())
                {
                    byte[] byteArray = ReadAllBytes2(openFileDialog.FileName);
                    if (byteArray.Length > 0)
                    {
                        TemplateStream.Write(byteArray, 0, (int)byteArray.Length);
                        TemplateStream.Position = 0;
                    }
                    SarReportTypeFilter filter = new SarReportTypeFilter();
                    filter.ID = ReportTemplate.REPORT_TYPE_ID;
                    SAR_REPORT_TYPE reportType = new BackendAdapter(new CommonParam()).Get<List<SAR_REPORT_TYPE>>("api/SarReportType/Get", ApiConsumers.SarConsumer, filter, new CommonParam()).FirstOrDefault();
                    string duoi = filename.Split('.').LastOrDefault();
                    string FILE_NAME = "";
                    if (reportType != null)
                    {
                        FILE_NAME = reportType.REPORT_TYPE_CODE + "_" + reportType.REPORT_TYPE_NAME + "_" + ReportTemplate.REPORT_TEMPLATE_CODE + "_" + ReportTemplate.REPORT_TEMPLATE_NAME + "." + duoi;
                    }
                    else
                    {
                        FILE_NAME = "" + "_" + "" + "_" + ReportTemplate.REPORT_TEMPLATE_CODE + "_" + ReportTemplate.REPORT_TEMPLATE_NAME + "." + duoi;
                    }
                    FileUploadInfo fileupload = Inventec.Fss.Client.FileUpload.UploadFile("SAR", "ReportTemplate", TemplateStream, FILE_NAME, keepOriginalFileName);

                    if (fileupload != null)
                    {
                        TemplateADO url = new TemplateADO();
                        url.FILE_NAME = FILE_NAME;
                        url.URL = fileupload.Url;
                        url.EXTENSION = duoi;
                        //string test = fileupload.Url.Split('\\').LastOrDefault();
                        //string Url = "{\"FILE_NAME\":\"" + h + "\",\"URL\":\"\\\\\\\\Upload\\\\\\\\SAR\\\\\\\\ReportTemplate\\\\\\\\" + h + "\",\"EXTENSION\":\"\"}";
                        var temUrl = Newtonsoft.Json.JsonConvert.SerializeObject(url);
                        ReportTemplate.REPORT_TEMPLATE_URL = temUrl;
                    }
                    else
                    {
                        result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }
        private byte[] ReadAllBytes2(string filePath)
        {
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (var ms = new MemoryStream())
                {
                    fs.CopyTo(ms);
                    return ms.ToArray();
                }
            }
        }
        private void btnChonFile_Click(object sender, EventArgs e)
        {
            try
            {
                openFileDialog.Filter = "Excel 2007 later file (*.xlsx)|*.xlsx|Excel 97-2003 file(*.xls)|*.xls|Pdf file(*.pdf)|*.pdf";
                openFileDialog.RestoreDirectory = true;
                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    txtFile.Text = openFileDialog.FileName;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
