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
using HIS.Desktop.Utility;
using HIS.Desktop.Common;
using Inventec.Desktop.Common.Modules;
using Inventec.Common.Logging;
using System.IO;
using Inventec.Desktop.Common.Message;
using SAR.EFMODEL.DataModels;
using SAR.Filter;
using SAR.Desktop.Plugins.SarImportReportTemplate.ADO;
using Inventec.Core;
using Inventec.Common.Adapter;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.String;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using System.Resources;
using Inventec.Common.Resource;
using Inventec.Desktop.Common.LanguageManager;

namespace SAR.Desktop.Plugins.SarImportReportTemplate.SarImportReportTemplate
{
    public partial class frmSarImportReportTemplate : FormBase
    {
        #region Declare
        Module _Module { get; set; }
        RefeshReference delegateRefresh;
        List<ImportReportTemplateADO> _CurrentAdos;
        List<ImportReportTemplateADO> ImportReportTemplateAdos;
        List<SAR_REPORT_TEMPLATE> ReportTemp;
        List<SAR_REPORT_TYPE> ReportType;

        #endregion
        public frmSarImportReportTemplate()
        {
            InitializeComponent();
        }

        public frmSarImportReportTemplate(Module module):base(module)
        {
            InitializeComponent();
            try
            {
                this._Module = module;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public frmSarImportReportTemplate(Module module, RefeshReference delegateRefresh)
            : base(module)
        {
            InitializeComponent();
            try
            {
                this._Module = module;
                this.delegateRefresh = delegateRefresh;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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

        private void frmSarImportReportTemplate_Load(object sender, EventArgs e)
        {
            try
            {
                SetIcon();
                LoadDataDefaut();
                SetCaptionByLanguageKey();
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }
        private void SetCaptionByLanguageKey()
        {
            try
            {
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("SAR.Desktop.Plugins.SarImportReportTemplate.Resources.Lang", typeof(SAR.Desktop.Plugins.SarImportReportTemplate.SarImportReportTemplate.frmSarImportReportTemplate).Assembly);
                 this.btnDownLoadFile.Text = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.btnDownLoadFile.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                 this.btnChooseFile.Text = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.btnChooseFile.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnShowLineError.Text = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.btnShowLineError.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                 this.btnImport.Text = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.btnImport.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColReportCode.Caption = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.grdColReportCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                 this.grdColReportCode.ToolTip = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.grdColReportCode.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                 this.grdColReportName.Caption = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.grdColReportName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                 this.grdColReportName.ToolTip = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.grdColReportName.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                 this.grdColReportTypeCode.Caption = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.grdColReportTypeCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                 this.grdColReportTypeCode.ToolTip = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.grdColReportTypeCode.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                 this.grdColExtension.Caption = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.grdColExtension.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                 this.grdColExtension.ToolTip = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.grdColExtension.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                 this.grdColTutorial.Caption = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.grdColTutorial.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                 this.grdColTutorial.ToolTip = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.grdColTutorial.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
               
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        private void LoadDataDefaut()
        {
            try
            {
               
                CommonParam param=new CommonParam();
                SarReportTemplateViewFilter filter=new SarReportTemplateViewFilter();
                SarReportTypeFilter typeFilter=new SarReportTypeFilter();
                this.ReportTemp = new BackendAdapter(param).Get <List<SAR_REPORT_TEMPLATE>>(SarRequestUriStore.SAR_REPORT_TEMPLATE_GET, ApiConsumers.SarConsumer,filter,param);
                param = new CommonParam();
                this.ReportType = new BackendAdapter(param).Get<List<SAR_REPORT_TYPE>>(SarRequestUriStore.SAR_REPORT_TYPE_GET, ApiConsumers.SarConsumer, typeFilter, param);
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void btnDownLoadFile_Click(object sender, EventArgs e)
        {
            try
            {

                string fileName = System.IO.Path.Combine(Application.StartupPath + "\\Tmp\\Imp\\", "IMPORT_REPORT_TEMPLATE.xlsx");
                Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
                param.Messages = new List<string>();
                if (File.Exists(fileName))
                {
                    saveFileDialog.Title = "Save File";
                    saveFileDialog.FileName = "IMPORT_REPORT_TEMPLATE";
                    saveFileDialog.DefaultExt = "xlsx";
                    saveFileDialog.Filter = "Excel files (*.xlsx)|All files (*.*)";
                    saveFileDialog.FilterIndex = 2;
                    saveFileDialog.RestoreDirectory = true;

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        File.Copy(fileName, saveFileDialog.FileName);
                        MessageManager.Show(this.ParentForm, param, true);
                        if (DevExpress.XtraEditors.XtraMessageBox.Show("Bạn có muốn mở file ngay?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            System.Diagnostics.Process.Start(saveFileDialog.FileName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnChooseFile_Click(object sender, EventArgs e)
        {
             try
            {
                btnShowLineError.Text = "Dòng lỗi";

                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Multiselect = false;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    WaitingManager.Show();
                    
                    var import = new Inventec.Common.ExcelImport.Import();
                    if (import.ReadFileExcel(ofd.FileName))
                    {
                        
                        var hisServiceImport = import.GetWithCheck<ImportReportTemplateADO>(0);
                        if (hisServiceImport != null && hisServiceImport.Count > 0)
                        {
                            List<ImportReportTemplateADO> listAfterRemove = new List<ImportReportTemplateADO>();


                            foreach (var item in hisServiceImport)
                            {
                                bool checkNull = string.IsNullOrEmpty(item.REPORT_TEMPLATE_CODE)
                                    && string.IsNullOrEmpty(item.REPORT_TEMPLATE_NAME)
                                    && string.IsNullOrEmpty(item.REPORT_TYPE_CODE)
                                    && string.IsNullOrEmpty(item.EXTENSION_RECEIVE)
                                    && string.IsNullOrEmpty(item.TUTORIAL)
                                    ;

                                if (!checkNull)
                                {
                                    listAfterRemove.Add(item);
                                }
                            }
                            WaitingManager.Hide();

                            this._CurrentAdos = listAfterRemove;

                            if (this._CurrentAdos != null && this._CurrentAdos.Count > 0)
                            {
                                btnShowLineError.Enabled = true;
                                this.ImportReportTemplateAdos = new List<ImportReportTemplateADO>();
                                addServiceToProcessList(_CurrentAdos, ref this.ImportReportTemplateAdos);
                                SetDataSource(this.ImportReportTemplateAdos);
                            }

                            //btnSave.Enabled = true;
                        }
                        else
                        {
                            WaitingManager.Hide();
                            DevExpress.XtraEditors.XtraMessageBox.Show("Import thất bại");
                        }
                    }
                    else
                    {
                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show("Không đọc được file");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void addServiceToProcessList(List<ImportReportTemplateADO> _service, ref List<ImportReportTemplateADO> ImportReportTemplateRef)
        {
            try
            {
                ImportReportTemplateRef = new List<ImportReportTemplateADO>();
                long i = 0;
                foreach (var item in _service)
                {
                    i++;
                    string error = "";
                    var serAdo = new ImportReportTemplateADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<ImportReportTemplateADO>(serAdo, item);

                    if (!string.IsNullOrEmpty(item.REPORT_TEMPLATE_CODE))
                    {
                        var checkTempCode = this.ReportTemp.FirstOrDefault(o => o.REPORT_TEMPLATE_CODE == item.REPORT_TEMPLATE_CODE);
                        if (CountVi.Count(item.REPORT_TEMPLATE_CODE) > 100)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Mã mẫu báo cáo");
                        }
                        if (checkTempCode != null)
                        {
                            error += string.Format(Message.MessageImport.DaTonTai, item.REPORT_TEMPLATE_CODE);
                        }

                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Mã mẫu báo cáo");
                    }

                
                    var checkTrung = _service.Where(p => p.REPORT_TEMPLATE_CODE == item.REPORT_TEMPLATE_CODE && p.REPORT_TEMPLATE_NAME == item.REPORT_TEMPLATE_NAME && p.REPORT_TYPE_CODE == item.REPORT_TYPE_CODE && p.EXTENSION_RECEIVE == item.EXTENSION_RECEIVE && p.TUTORIAL == item.TUTORIAL).ToList();
                    if (checkTrung != null && checkTrung.Count > 1)
                    {
                        error += string.Format(Message.MessageImport.FileImportDaTonTai, item.REPORT_TEMPLATE_CODE);
                    }
                    var checktrungMaBC = _service.Where(p => p.REPORT_TEMPLATE_CODE == item.REPORT_TEMPLATE_CODE).ToList();
                    if (checktrungMaBC != null && checktrungMaBC.Count > 1)
                    {
                        error += string.Format(Message.MessageImport.FileImportDaTonTaiMaBaoCao, item.REPORT_TEMPLATE_CODE);
                    }

                    if (!string.IsNullOrEmpty(item.REPORT_TEMPLATE_NAME))
                    {
                        if (CountVi.Count(item.REPORT_TEMPLATE_NAME) > 200)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Tên mẫu báo cáo");
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Tên mẫu báo cáo");
                    }
                    if (!string.IsNullOrEmpty(item.REPORT_TYPE_CODE))
                    {
                        var ReportTempType = this.ReportType.FirstOrDefault(o => o.REPORT_TYPE_CODE == item.REPORT_TYPE_CODE);
                        if (ReportTempType != null)
                        {
                            if (ReportTempType.IS_ACTIVE == 1)
                            {
                                serAdo.REPORT_TYPE_ID = ReportTempType.ID;
                            }
                            else
                            {
                                error += string.Format(Message.MessageImport.MaLoaiBaoCaoDaKhoa, item.REPORT_TYPE_CODE);
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Mã loại báo cáo");
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Mã loại báo cáo");
                    }

                    if (!string.IsNullOrEmpty(item.EXTENSION_RECEIVE))
                    {
                        if (CountVi.Count(item.EXTENSION_RECEIVE) > 20)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Định dạng nhập");
                        }
                    }
                   
                    if (!string.IsNullOrEmpty(item.TUTORIAL))
                    {
                        if (CountVi.Count(item.TUTORIAL) > 2000)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Hướng dẫn");
                        }
                    }
                    serAdo.ERROR = error;
                    serAdo.ID = i;

                    ImportReportTemplateRef.Add(serAdo);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void SetDataSource(List<ImportReportTemplateADO> dataSource)
        {
            try
            {
                gridControlImport.BeginUpdate();
                gridControlImport.DataSource = null;
                gridControlImport.DataSource = dataSource;
                gridControlImport.EndUpdate();
                CheckErrorLine(null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CheckErrorLine(List<ImportReportTemplateADO> dataSource)
        {
            try
            {
                var checkError = this.ImportReportTemplateAdos.Exists(o => !string.IsNullOrEmpty(o.ERROR));
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
                    var errorLine = this.ImportReportTemplateAdos.Where(o => !string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(errorLine);

                }
                else
                {
                    btnShowLineError.Text = "Dòng lỗi";
                    var errorLine = this.ImportReportTemplateAdos.Where(o => string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(errorLine);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                bool success = false;
                WaitingManager.Show();
                List<SAR_REPORT_TEMPLATE> UpdateDTO = new List<SAR_REPORT_TEMPLATE>();

                if (this.ImportReportTemplateAdos != null && this.ImportReportTemplateAdos.Count > 0)
                {
                    foreach (var item in this.ImportReportTemplateAdos)
                    {
                        SAR_REPORT_TEMPLATE ado = new SAR_REPORT_TEMPLATE();
                        ado.REPORT_TEMPLATE_CODE = item.REPORT_TEMPLATE_CODE;
                        ado.REPORT_TEMPLATE_NAME = item.REPORT_TEMPLATE_NAME;
                        ado.REPORT_TYPE_ID = item.REPORT_TYPE_ID;
                        ado.EXTENSION_RECEIVE = item.EXTENSION_RECEIVE;
                        ado.TUTORIAL = item.TUTORIAL;
                        UpdateDTO.Add(ado);
                    }
                }
                else
                {
                    WaitingManager.Hide();
                    DevExpress.XtraEditors.XtraMessageBox.Show("Dữ liệu rỗng", "Thông báo");
                    return;
                }

                CommonParam param = new CommonParam();
                var dataImports = new BackendAdapter(param).Post<List<SAR_REPORT_TEMPLATE>>("api/SarReportTemplate/CreateList", ApiConsumers.SarConsumer, UpdateDTO, param);
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

                MessageManager.Show(this.ParentForm, param, success);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewImport_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    ImportReportTemplateADO pData = (ImportReportTemplateADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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

        private void gridViewImport_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    string error = (gridViewImport.GetRowCellValue(e.RowHandle, "ERROR") ?? "").ToString();
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

        private void btnDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (ImportReportTemplateADO)gridViewImport.GetFocusedRow();
                if (row != null)
                {
                    if (this.ImportReportTemplateAdos != null && this.ImportReportTemplateAdos.Count > 0)
                    {
                        this.ImportReportTemplateAdos.Remove(row);
                        var dataCheck = this.ImportReportTemplateAdos.Where(p => p.REPORT_TEMPLATE_CODE == row.REPORT_TEMPLATE_CODE && p.REPORT_TEMPLATE_NAME == row.REPORT_TEMPLATE_NAME && p.REPORT_TYPE_CODE == row.REPORT_TYPE_CODE && p.EXTENSION_RECEIVE == row.EXTENSION_RECEIVE && p.TUTORIAL == row.TUTORIAL).ToList();
                        if (dataCheck != null && dataCheck.Count == 1)
                        {
                            if (!string.IsNullOrEmpty(dataCheck[0].ERROR))
                            {
                                string erro = string.Format(Message.MessageImport.FileImportDaTonTai, dataCheck[0].REPORT_TEMPLATE_CODE);
                                dataCheck[0].ERROR = dataCheck[0].ERROR.Replace(erro, "");
                            }

                        }
                        var datacheckCode = this.ImportReportTemplateAdos.Where(p => p.REPORT_TEMPLATE_CODE == row.REPORT_TEMPLATE_CODE).ToList();
                        if (datacheckCode != null && datacheckCode.Count == 1)
                        {
                            if (!string.IsNullOrEmpty(datacheckCode[0].ERROR))
                            {
                                string erro = string.Format(Message.MessageImport.FileImportDaTonTaiMaBaoCao, datacheckCode[0].REPORT_TEMPLATE_CODE);
                                datacheckCode[0].ERROR = datacheckCode[0].ERROR.Replace(erro, "");
                            }

                        }
                        SetDataSource(this.ImportReportTemplateAdos);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnImport.Enabled)
                {
                    btnImport_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                
                throw;
            }
        }

        private void btnError_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (ImportReportTemplateADO)gridViewImport.GetFocusedRow();
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
