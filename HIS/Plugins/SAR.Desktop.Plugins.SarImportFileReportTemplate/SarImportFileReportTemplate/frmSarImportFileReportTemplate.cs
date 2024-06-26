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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.Core;
using Inventec.Desktop.Common.Modules;
using HIS.Desktop.Utility;
using HIS.Desktop.Common;
using Inventec.Common.Logging;
using SAR.EFMODEL.DataModels;
using SAR.Filter;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using System.IO;
using Inventec.Fss.Utility;
using SAR.Desktop.Plugins.SarImportFileReportTemplate.ADO;
using Inventec.Common.String;
using Inventec.Desktop.Common.Message;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;

namespace SAR.Desktop.Plugins.SarImportFileReportTemplate.SarImportFileReportTemplate
{
    public partial class frmSarImportFileReportTemplate : FormBase
    {
        #region Declare
        Module _Module { get; set; }
        RefeshReference delegateRefresh;
        List<SAR_REPORT_TEMPLATE> ReportTemp;
        private MemoryStream TemplateStream;
        List<SarImportFileReportTemplateADO> ImportListFileReportTempADO;
        DelegateRefreshData deleResultFileUpload;
        string pathFoder;
        #endregion
        public frmSarImportFileReportTemplate()
        {
            InitializeComponent();
        }
        public frmSarImportFileReportTemplate(Module module)
            : base(module)
        {
            InitializeComponent();
            this._Module = module;
        }
        public frmSarImportFileReportTemplate(Module module, RefeshReference delegateRefresh)
            : base(module)
        {
            InitializeComponent();
            this._Module = module;
            this.delegateRefresh = delegateRefresh;
        }

        private void frmSarImportFileReportTemplate_Load(object sender, EventArgs e)
        {
            try
            {
                SetIcon();
                LoadDataDefaut();
                SetCaptionByLanguageKey();
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("SAR.Desktop.Plugins.SarImportFileReportTemplate.Resources.Lang", typeof(SAR.Desktop.Plugins.SarImportFileReportTemplate.SarImportFileReportTemplate.frmSarImportFileReportTemplate).Assembly);
                this.btnChooseFile.Text = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.btnChooseFile.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnShowLineError.Text = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.btnShowLineError.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnImport.Text = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.btnImport.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void SetIcon()
        {
            try
            {
                if (this._Module != null)
                {
                    this.Text = this._Module.text;
                }
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataDefaut()
        {
            try
            {

                CommonParam param = new CommonParam();
                SarReportTemplateViewFilter filter = new SarReportTemplateViewFilter();
                this.ReportTemp = new BackendAdapter(param).Get<List<SAR_REPORT_TEMPLATE>>("api/SarReportTemplate/Get", ApiConsumers.SarConsumer, filter, param);

            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void btnInport_Click(object sender, EventArgs e)
        {
            try
            {
                bool success = false;
                WaitingManager.Show();
                List<SAR_REPORT_TEMPLATE> UpdateDTO = new List<SAR_REPORT_TEMPLATE>();

                if (this.ImportListFileReportTempADO != null && this.ImportListFileReportTempADO.Count > 0)
                {
                    foreach (var item in this.ImportListFileReportTempADO)
                    {
                        
                        SAR_REPORT_TEMPLATE dataDto = new SAR_REPORT_TEMPLATE();
                        Inventec.Common.Mapper.DataObjectMapper.Map<SAR_REPORT_TEMPLATE>(dataDto, item);
                        byte[] byteArray = File.ReadAllBytes(item.Path);
                        if (byteArray.Length > 0)
                        {
                            TemplateStream = new MemoryStream();
                            TemplateStream.Write(byteArray, 0, (int)byteArray.Length);
                            TemplateStream.Position = 0;
                        }
                        try
                        {
                            FileUploadInfo fileupload = Inventec.Fss.Client.FileUpload.UploadFile("SAR", "ReportTemplate", TemplateStream, item.FileName, true);
                            if (fileupload != null)
                            {
                                string Url = "{\"FILE_NAME\":\"" + item.FileName + "\",\"URL\":\"\\\\\\\\Upload\\\\\\\\SAR\\\\\\\\ReportTemplate\\\\\\\\" + item.FileName + "\",\"EXTENSION\":\"\"}";
                                dataDto.REPORT_TEMPLATE_URL = Url;
                            }
                        }
                        catch (Exception ex)
                        {

                            LogSystem.Warn(ex);
                        }
                        UpdateDTO.Add(dataDto);

                    }
                }
                else
                {
                    WaitingManager.Hide();
                    DevExpress.XtraEditors.XtraMessageBox.Show("Dữ liệu rỗng", "Thông báo");
                    return;
                }

                CommonParam param = new CommonParam();
                var dataImports = new BackendAdapter(param).Post<List<SAR_REPORT_TEMPLATE>>("api/SarReportTemplate/UpdateList", ApiConsumers.SarConsumer, UpdateDTO, param);
                WaitingManager.Hide();
                if (dataImports != null && dataImports.Count > 0)
                {
                    success = true;
                    btnImport.Enabled = false;
                    LoadDataDefaut();
                    if (this.delegateRefresh != null)
                    {
                        this.delegateRefresh();
                    }
                }
                WaitingManager.Hide();
                MessageManager.Show(this.ParentForm, param, success);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

      
        private void bbtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnImport.Enabled)
                {
                    btnInport_Click(null, null);
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void btnChooseFile_Click(object sender, EventArgs e)
        {
            try
            {
                ImportListFileReportTempADO = new List<SarImportFileReportTemplateADO>();
                if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    pathFoder = folderBrowserDialog.SelectedPath.ToString();
                    string[] filePahts = Directory.GetFiles(pathFoder);
                    if (filePahts != null && filePahts.Count() > 0)
                    {
                        List<SarImportFileReportTemplateADO> CurrentAdos = new List<SarImportFileReportTemplateADO>();
                        for (int i = 0; i < filePahts.Count(); i++)
                        {
                            SarImportFileReportTemplateADO reportItem = new SarImportFileReportTemplateADO();
                            string x = filePahts[i].Split('\\').LastOrDefault();

                            reportItem.FileName = x;
                            string ReportCode = x.Split('.').FirstOrDefault();
                            reportItem.REPORT_TEMPLATE_CODE = ReportCode;
                            reportItem.Path = filePahts[i];
                            CurrentAdos.Add(reportItem);
                        }
                        addServiceToProcessList(CurrentAdos, ref this.ImportListFileReportTempADO);
                        SetDataSource(this.ImportListFileReportTempADO);
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void addServiceToProcessList(List<SarImportFileReportTemplateADO> currentAdos, ref List<SarImportFileReportTemplateADO> importListFileReportTempADO)
        {
            try
            {
                if (currentAdos != null && currentAdos.Count > 0)
                {
                    importListFileReportTempADO = new List<SarImportFileReportTemplateADO>();
                    foreach (var item in currentAdos)
                    {
                        string CheckExcel = item.FileName.Split('.').LastOrDefault();
                        string error = "";
                        SarImportFileReportTemplateADO dataADO = new SarImportFileReportTemplateADO();
                        var checkCode = this.ReportTemp.FirstOrDefault(p => p.REPORT_TEMPLATE_CODE == item.REPORT_TEMPLATE_CODE);
                        
                        if (checkCode != null)
                        {
                            Inventec.Common.Mapper.DataObjectMapper.Map<SarImportFileReportTemplateADO>(dataADO, checkCode);
                            dataADO.FileName = item.FileName;
                        }
                        else
                        {
                            dataADO.FileName = item.FileName;
                            dataADO.REPORT_TEMPLATE_CODE = item.REPORT_TEMPLATE_CODE;
                            error += string.Format(Message.MessageImport.KoTonTai, item.REPORT_TEMPLATE_CODE);
                        }
                        string Url = "{\"FILE_NAME\":\"" + item.FileName + "\",\"URL\":\"\\\\\\\\Upload\\\\\\\\SAR\\\\\\\\ReportTemplate\\\\\\\\" + item.FileName + "\",\"EXTENSION\":\"\"}";
                        if (CheckExcel != "xlsx" && CheckExcel != "xls")
                        {
                            error += string.Format(Message.MessageImport.DinhDangFileKoDung, item.REPORT_TEMPLATE_CODE);
                        }
                        if (CountVi.Count(Url) > 2000)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Tên File");
                        }
                        var checkFileImp = currentAdos.Where(o => o.FileName == item.FileName && o.REPORT_TEMPLATE_CODE == item.REPORT_TEMPLATE_CODE).ToList();
                        var checkReportCode = currentAdos.Where(o => o.REPORT_TEMPLATE_CODE == item.REPORT_TEMPLATE_CODE).ToList();
                        if (checkFileImp != null && checkFileImp.Count > 1)
                        {
                            error += string.Format(Message.MessageImport.FileImportDaTonTai, item.REPORT_TEMPLATE_CODE);
                        }
                        if (checkReportCode != null && checkReportCode.Count > 1)
                        {
                            error += string.Format(Message.MessageImport.FileImportDaTonTaiMaBaoCao, item.REPORT_TEMPLATE_CODE);
                        }
                        dataADO.ERROR = error;
                        dataADO.Path = item.Path;
                        importListFileReportTempADO.Add(dataADO);
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void SetDataSource(List<SarImportFileReportTemplateADO> importListFileReportTempADO)
        {
            try
            {
                gridControl.BeginUpdate();
                gridControl.DataSource = null;
                gridControl.DataSource = importListFileReportTempADO;
                gridControl.EndUpdate();
                CheckErrorLine();
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void CheckErrorLine()
        {
            try
            {
                var checkError = this.ImportListFileReportTempADO.Exists(o => !string.IsNullOrEmpty(o.ERROR));
                if (!checkError)
                {
                    btnImport.Enabled = true;
                    btnShowLineError.Enabled = false;
                }
                else
                {
                    btnShowLineError.Enabled = true;
                    btnImport.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnShowLineError_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnShowLineError.Text == "Dòng lỗi")
                {
                    btnShowLineError.Text = "Dòng không lỗi";
                    var errorLine = this.ImportListFileReportTempADO.Where(o => !string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(errorLine);

                }
                else
                {
                    btnShowLineError.Text = "Dòng lỗi";
                    var errorLine = this.ImportListFileReportTempADO.Where(o => string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(errorLine);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    string error = (gridView.GetRowCellValue(e.RowHandle, "ERROR") ?? "").ToString();
                    if (e.Column.FieldName == "ERROR")
                    {
                        if (!string.IsNullOrEmpty(error))
                        {
                            e.RepositoryItem = btnError;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    SarImportFileReportTemplateADO pData = (SarImportFileReportTemplateADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (SarImportFileReportTemplateADO)gridView.GetFocusedRow();
                if (row != null)
                {
                    if (this.ImportListFileReportTempADO != null && this.ImportListFileReportTempADO.Count > 0)
                    {
                        this.ImportListFileReportTempADO.Remove(row);
                        var dataCheck = this.ImportListFileReportTempADO.Where(p => p.REPORT_TEMPLATE_CODE == row.REPORT_TEMPLATE_CODE && p.FileName == row.FileName).ToList();
                        if (dataCheck != null && dataCheck.Count == 1)
                        {
                            if (!string.IsNullOrEmpty(dataCheck[0].ERROR))
                            {
                                string erro = string.Format(Message.MessageImport.FileImportDaTonTai, dataCheck[0].REPORT_TEMPLATE_CODE);
                                dataCheck[0].ERROR = dataCheck[0].ERROR.Replace(erro, "");
                            }

                        }
                        var datacheckCode = this.ImportListFileReportTempADO.Where(p => p.REPORT_TEMPLATE_CODE == row.REPORT_TEMPLATE_CODE).ToList();
                        if (datacheckCode != null && datacheckCode.Count == 1)
                        {
                            if (!string.IsNullOrEmpty(datacheckCode[0].ERROR))
                            {
                                string erro = string.Format(Message.MessageImport.FileImportDaTonTaiMaBaoCao, datacheckCode[0].REPORT_TEMPLATE_CODE);
                                datacheckCode[0].ERROR = datacheckCode[0].ERROR.Replace(erro, "");
                            }

                        }
                        SetDataSource(this.ImportListFileReportTempADO);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnError_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (SarImportFileReportTemplateADO)gridView.GetFocusedRow();
                if (row != null && !string.IsNullOrEmpty(row.ERROR))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(row.ERROR, "Thông báo");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
