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
using DevExpress.Data;
using DevExpress.Spreadsheet;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.HisImportServiceRetyCat.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using SAR.EFMODEL.DataModels;
using SAR.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisImportServiceRetyCat
{
    public partial class FormImportServiceRetyCat : HIS.Desktop.Utility.FormBase
    {
        private Inventec.Desktop.Common.Modules.Module moduleData;
        private System.Globalization.CultureInfo cultureLang;
        private List<ADO.ServiceRetyCatAdo> ListDataImport;
        private bool checkClick;
        private List<HIS_REPORT_TYPE_CAT> ListReportTypeCat;
        private List<V_HIS_SERVICE> ListService;
        private List<SAR_REPORT_TYPE> ListReportType;
        HIS.Desktop.Common.RefeshReference RefeshReference = null;
        public FormImportServiceRetyCat()
        {
            InitializeComponent();
        }
        public FormImportServiceRetyCat(Inventec.Desktop.Common.Modules.Module moduleData, HIS.Desktop.Common.RefeshReference RefeshReference)
        //: base(moduleData)
        {
            InitializeComponent();
            try
            {
                this.moduleData = moduleData;
                this.Text = moduleData.text;
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationManager.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                this.RefeshReference = RefeshReference;
                this.cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public FormImportServiceRetyCat(Inventec.Desktop.Common.Modules.Module moduleData)
        //: base(moduleData)
        {
            InitializeComponent();
            try
            {
                this.moduleData = moduleData;
                this.Text = moduleData.text;
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationManager.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);

                this.cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FormImportServiceRetyCat_Load(object sender, EventArgs e)
        {
            try
            {
                //Gan ngon ngu
                LoadKeysFromlanguage();

                LoadDataDeafault();

                BtnSave.Enabled = false;
                BtnLineError.Enabled = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataDeafault()
        {
            try
            {
                CommonParam paramCommon = new CommonParam();
                WaitingManager.Show();
                MOS.Filter.HisReportTypeCatFilter reportTypeCatFilter = new MOS.Filter.HisReportTypeCatFilter();
                this.ListReportTypeCat = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<HIS_REPORT_TYPE_CAT>>("api/HisReportTypeCat/Get", ApiConsumer.ApiConsumers.MosConsumer, reportTypeCatFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                if (this.ListReportTypeCat == null) this.ListReportTypeCat = new List<HIS_REPORT_TYPE_CAT>();
                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion

                this.ListService = BackendDataWorker.Get<V_HIS_SERVICE>();
                if (this.ListService == null) this.ListService = new List<V_HIS_SERVICE>();
                SarReportTypeFilter reportTypeFilter = new SarReportTypeFilter();
                this.ListReportType = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<SAR_REPORT_TYPE>>("api/SarReportType/Get", ApiConsumer.ApiConsumers.SarConsumer, reportTypeFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion

                if (this.ListReportType == null) this.ListReportType = new List<SAR_REPORT_TYPE>();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadKeysFromlanguage()
        {
            try
            {
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("FormImportServiceRetyCat.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_Stt.Caption = Inventec.Common.Resource.Get.Value("FormImportServiceRetyCat.Gc_Stt.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_Stt.ToolTip = Inventec.Common.Resource.Get.Value("FormImportServiceRetyCat.Gc_Stt.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_LineError.Caption = Inventec.Common.Resource.Get.Value("FormImportServiceRetyCat.Gc_LineError.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_Delete.Caption = Inventec.Common.Resource.Get.Value("FormImportServiceRetyCat.Gc_Delete.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_ServiceTypeCode.Caption = Inventec.Common.Resource.Get.Value("FormImportServiceRetyCat.Gc_ServiceTypeCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_ServiceTypeCode.ToolTip = Inventec.Common.Resource.Get.Value("FormImportServiceRetyCat.Gc_ServiceTypeCode.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_ServiceTypeName.Caption = Inventec.Common.Resource.Get.Value("FormImportServiceRetyCat.Gc_ServiceTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_ServiceTypeName.ToolTip = Inventec.Common.Resource.Get.Value("FormImportServiceRetyCat.Gc_ServiceTypeName.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_CategoryCode.Caption = Inventec.Common.Resource.Get.Value("FormImportServiceRetyCat.Gc_CategoryCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_CategoryCode.ToolTip = Inventec.Common.Resource.Get.Value("FormImportServiceRetyCat.Gc_CategoryCode.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_CategoryName.Caption = Inventec.Common.Resource.Get.Value("FormImportServiceRetyCat.Gc_CategoryName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_CategoryName.ToolTip = Inventec.Common.Resource.Get.Value("FormImportServiceRetyCat.Gc_CategoryName.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_ServiceCode.Caption = Inventec.Common.Resource.Get.Value("FormImportServiceRetyCat.Gc_ServiceCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_ServiceCode.ToolTip = Inventec.Common.Resource.Get.Value("FormImportServiceRetyCat.Gc_ServiceCode.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_ServiceName.Caption = Inventec.Common.Resource.Get.Value("FormImportServiceRetyCat.Gc_ServiceName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_ServiceName.ToolTip = Inventec.Common.Resource.Get.Value("FormImportServiceRetyCat.Gc_ServiceName.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.BtnSave.Text = Inventec.Common.Resource.Get.Value("FormImportServiceRetyCat.BtnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.BtnLineError.Text = Inventec.Common.Resource.Get.Value("FormImportServiceRetyCat.BtnLineError.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.BtnImport.Text = Inventec.Common.Resource.Get.Value("FormImportServiceRetyCat.BtnImport.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.BtnDownloadTemplate.Text = Inventec.Common.Resource.Get.Value("FormImportServiceRetyCat.BtnDownloadTemplate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("FormImportServiceRetyCat.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonSave.Caption = Inventec.Common.Resource.Get.Value("FormImportServiceRetyCat.barButtonSave.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_ReportTypeCode.Caption = Inventec.Common.Resource.Get.Value("FormImportServiceRetyCat.Gc_ReportTypeCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_ReportTypeCode.ToolTip = Inventec.Common.Resource.Get.Value("FormImportServiceRetyCat.Gc_ReportTypeCode.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_ReportTypeName.Caption = Inventec.Common.Resource.Get.Value("FormImportServiceRetyCat.Gc_ReportTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_ReportTypeName.ToolTip = Inventec.Common.Resource.Get.Value("FormImportServiceRetyCat.Gc_ReportTypeName.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("FormImportServiceRetyCat.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string GetLanguageControl(string key)
        {
            return Inventec.Common.Resource.Get.Value(key, Resources.ResourceLanguageManager.LanguageResource, cultureLang);
        }

        private void BtnDownloadTemplate_Click(object sender, EventArgs e)
        {
            try
            {

                string fileName = System.IO.Path.Combine(Application.StartupPath + "\\Tmp\\Imp\\", "IMPORT_SERVICE_RETY_CAT.xlsx");
                Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
                param.Messages = new List<string>();
                if (File.Exists(fileName))
                {
                    saveFileDialog.Title = "Save File";
                    saveFileDialog.FileName = "IMPORT_SERVICE_RETY_CAT";
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

        private void BtnImport_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Multiselect = false;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    WaitingManager.Show();
                    var import = new Inventec.Common.ExcelImport.Import();
                    if (import.ReadFileExcel(ofd.FileName))
                    {
                        ListDataImport = import.Get<ADO.ServiceRetyCatAdo>(0);
                        if (ListDataImport != null && ListDataImport.Count > 0)
                        {
                            AddListServiceRetyCatToProcessList(ListDataImport);

                            if (this.ListDataImport != null && this.ListDataImport.Count > 0)
                            {
                                SetDataSource(ListDataImport);

                                checkClick = false;
                                //BtnSave.Enabled = true;
                                BtnLineError.Enabled = true;
                            }
                            WaitingManager.Hide();
                        }
                        else
                        {
                            WaitingManager.Hide();
                            DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceLanguageManager.HeThongTBKQXLYCCuaFrontendThatBai, Resources.ResourceLanguageManager.ThongBao, DevExpress.Utils.DefaultBoolean.True);
                        }
                    }
                    else
                    {
                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceLanguageManager.HeThongTBKQXLYCCuaFrontendThatBai, Resources.ResourceLanguageManager.ThongBao, DevExpress.Utils.DefaultBoolean.True);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDataSource(List<ADO.ServiceRetyCatAdo> ListDataImport)
        {
            try
            {
                gridControl.BeginUpdate();
                gridControl.DataSource = null;
                gridControl.DataSource = ListDataImport;
                gridControl.EndUpdate();
                CheckErrorLine(ListDataImport);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CheckErrorLine(List<ADO.ServiceRetyCatAdo> dataSource)
        {
            try
            {
                if (dataSource != null && dataSource.Count > 0)
                {
                    var checkError = dataSource.Exists(o => !string.IsNullOrEmpty(o.ERROR));
                    if (!checkError)
                    {
                        BtnSave.Enabled = true;
                    }
                    else
                    {
                        BtnSave.Enabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void AddListServiceRetyCatToProcessList(List<ADO.ServiceRetyCatAdo> ImpListProcessor)
        {
            try
            {
                if (ImpListProcessor != null && ImpListProcessor.Count > 0)
                {
                    var ListServiceRetyCat = this.GetServiceRetyCat(ImpListProcessor);
                    long i = 0;
                    foreach (var item in ImpListProcessor)
                    {
                        i++;
                        List<string> errors = new List<string>();
                        item.IdRow = i;

                        if (string.IsNullOrWhiteSpace(item.CATEGORY_CODE))
                        {
                            errors.Add(string.Format(Resources.ResourceLanguageManager.KhongHopLe, GetLanguageControl("FormImportServiceRetyCat.Gc_CategoryCode.Caption")));
                        }
                        //else
                        //{
                        //    if (item.CATEGORY_CODE.Length > 10)
                        //    {
                        //        errors.Add(string.Format(Resources.ResourceLanguageManager.Maxlength, GetLanguageControl("FormImportServiceRetyCat.Gc_CategoryCode.Caption")));
                        //    }
                        //}
                        if (string.IsNullOrWhiteSpace(item.REPORT_TYPE_CODE))
                        {
                            errors.Add(string.Format(Resources.ResourceLanguageManager.KhongHopLe, GetLanguageControl("FormImportServiceRetyCat.Gc_ReportTypeCode.Caption")));
                        }
                        //else
                        //{
                        //    if (item.REPORT_TYPE_CODE.Length > 10)
                        //    {
                        //        errors.Add(string.Format(Resources.ResourceLanguageManager.Maxlength, GetLanguageControl("FormImportServiceRetyCat.Gc_ReportTypeCode.Caption")));
                        //    }
                        //}

                        if (string.IsNullOrWhiteSpace(item.SERVICE_CODE))
                        {
                            errors.Add(string.Format(Resources.ResourceLanguageManager.KhongHopLe, GetLanguageControl("FormImportServiceRetyCat.Gc_ServiceCode.Caption")));
                        }
                        //else
                        //{
                        //    if (item.SERVICE_CODE.Length > 10)
                        //    {
                        //        errors.Add(string.Format(Resources.ResourceLanguageManager.Maxlength, GetLanguageControl("FormImportServiceRetyCat.Gc_ServiceCode.Caption")));
                        //    }
                        //}
                        if (ListServiceRetyCat != null)
                        {
                            var serviceRetyCat = ListServiceRetyCat.FirstOrDefault(o => ListReportTypeCat.Exists(p => p.CATEGORY_CODE == item.CATEGORY_CODE && p.REPORT_TYPE_CODE == item.REPORT_TYPE_CODE && p.ID == o.REPORT_TYPE_CAT_ID) && ListService.Exists(q => q.SERVICE_CODE == item.SERVICE_CODE && q.ID == o.SERVICE_ID));
                            var check = (serviceRetyCat != null);
                            if (check)
                            {
                                errors.Add(string.Format(Resources.ResourceLanguageManager.DaTonTai, "Dịch vụ nhóm báo cáo "));
                            }

                            var checkExel = ImpListProcessor.Count(o => o.CATEGORY_CODE == item.CATEGORY_CODE && o.REPORT_TYPE_CODE == item.REPORT_TYPE_CODE && o.SERVICE_CODE == item.SERVICE_CODE);
                            if (checkExel > 1)
                            {
                                errors.Add(string.Format(Resources.ResourceLanguageManager.BiTrung, "Dịch vụ nhóm báo cáo "));
                            }

                        }
                        var reportTypeCat = this.ListReportTypeCat.FirstOrDefault(o => o.CATEGORY_CODE == item.CATEGORY_CODE && o.REPORT_TYPE_CODE == item.REPORT_TYPE_CODE);
                        if (reportTypeCat != null)
                        {
                            item.CATEGORY_NAME = reportTypeCat.CATEGORY_NAME;
                        }
                        else if (!this.ListReportType.Exists(o => o.REPORT_TYPE_CODE == item.REPORT_TYPE_CODE))
                        {
                            errors.Add(string.Format(Resources.ResourceLanguageManager.ChuaTonTai, GetLanguageControl("FormImportServiceRetyCat.Gc_ReportTypeCode.Caption")));
                        }
                        else
                        {
                            errors.Add(string.Format(Resources.ResourceLanguageManager.ChuaTonTai, GetLanguageControl("FormImportServiceRetyCat.Gc_CategoryCode.Caption")));
                        }
                        var reportType = this.ListReportType.FirstOrDefault(o => o.REPORT_TYPE_CODE == item.REPORT_TYPE_CODE);
                        if (reportType != null)
                        {
                            item.REPORT_TYPE_NAME = reportType.REPORT_TYPE_NAME;
                        }
                        var service = this.ListService.FirstOrDefault(o => o.SERVICE_CODE == item.SERVICE_CODE);
                        if (service != null)
                        {
                            item.SERVICE_NAME = service.SERVICE_NAME;
                            item.SERVICE_TYPE_NAME = service.SERVICE_TYPE_NAME;
                            item.SERVICE_CODE = service.SERVICE_CODE;
                            item.SERVICE_TYPE_CODE = service.SERVICE_TYPE_CODE;
                        }
                        else
                        {
                            errors.Add(string.Format(Resources.ResourceLanguageManager.ChuaTonTai, GetLanguageControl("FormImportServiceRetyCat.Gc_ServiceCode.Caption")));
                        }
                        item.ERROR = string.Join(";", errors);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<HIS_SERVICE_RETY_CAT> GetServiceRetyCat(List<ServiceRetyCatAdo> listServiceRetyCatAdo)
        {
            List<HIS_SERVICE_RETY_CAT> result = null;
            try
            {
                CommonParam paramCommon = new CommonParam();
                var listReportTypeCatSub = this.ListReportTypeCat.Where(o => listServiceRetyCatAdo.Exists(p => p.REPORT_TYPE_CODE == o.REPORT_TYPE_CODE && p.CATEGORY_CODE == o.CATEGORY_CODE)).ToList();

                var listServiceSub = this.ListService.Where(o => listServiceRetyCatAdo.Exists(p => p.SERVICE_CODE == o.SERVICE_CODE)).ToList();

                if (listReportTypeCatSub != null && listServiceSub != null && listReportTypeCatSub.Count > 0 && listServiceSub.Count > 0)
                {
                    HisServiceRetyCatFilter HisServiceRetyCatfilter = new HisServiceRetyCatFilter();
                    HisServiceRetyCatfilter.SERVICE_IDs = listServiceSub.Select(o => o.ID).ToList();
                    HisServiceRetyCatfilter.REPORT_TYPE_CAT_IDs = listReportTypeCatSub.Select(o => o.ID).ToList();
                    result = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<HIS_SERVICE_RETY_CAT>>("api/HisServiceRetyCat/Get", ApiConsumer.ApiConsumers.MosConsumer, HisServiceRetyCatfilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                    #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                    SessionManager.ProcessTokenLost(paramCommon);
                    #endregion

                }

            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
                throw new NullReferenceException("Loi khi lay ServiceId hoac ReportTypeId");
            }
            return result;
        }

        private void convertDateStringToTimeNumber(string date, ref long? dateTime, ref string check)
        {
            try
            {
                if (date.Length > 14)
                {
                    check = Resources.ResourceLanguageManager.Maxlength;
                    return;
                }

                if (date.Length < 14)
                {
                    check = Resources.ResourceLanguageManager.KhongHopLe;
                    return;
                }

                if (date.Length > 0)
                {
                    if (!Inventec.Common.DateTime.Check.IsValidTime(date))
                    {
                        check = Resources.ResourceLanguageManager.KhongHopLe;
                        return;
                    }
                    dateTime = Inventec.Common.TypeConvert.Parse.ToInt64(date);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void BtnLineError_Click(object sender, EventArgs e)
        {
            try
            {
                if (!BtnLineError.Enabled) return;

                checkClick = true;
                if (BtnLineError.Text == GetLanguageControl("FormImportServiceRetyCat.Gc_LineError_Error"))
                {
                    BtnLineError.Text = GetLanguageControl("FormImportServiceRetyCat.Gc_LineError_Ok");
                    var data = ListDataImport.Where(o => !string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(data);
                }
                else
                {
                    BtnLineError.Text = GetLanguageControl("FormImportServiceRetyCat.Gc_LineError_Error");
                    var data = ListDataImport.Where(o => string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(data);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!BtnSave.Enabled) return;
                BtnSave.Focus();
                var listData = (List<ADO.ServiceRetyCatAdo>)gridControl.DataSource;

                if (listData == null || listData.Count <= 0) return;

                bool success = false;
                WaitingManager.Show();

                List<HIS_SERVICE_RETY_CAT> listServiceRetyCat = new List<HIS_SERVICE_RETY_CAT>();

                foreach (var item in listData)
                {
                    HIS_SERVICE_RETY_CAT src = new HIS_SERVICE_RETY_CAT();
                    var service = ListService.FirstOrDefault(o => o.SERVICE_CODE == item.SERVICE_CODE);
                    var reportTypeCat = ListReportTypeCat.FirstOrDefault(o => o.CATEGORY_CODE == item.CATEGORY_CODE && o.REPORT_TYPE_CODE == item.REPORT_TYPE_CODE);
                    if (service != null && reportTypeCat != null)
                    {
                        src.SERVICE_ID = service.ID;
                        src.REPORT_TYPE_CAT_ID = reportTypeCat.ID;
                        listServiceRetyCat.Add(src);
                    }
                }
                CommonParam param = new CommonParam();

                if (listServiceRetyCat != null && listServiceRetyCat.Count > 0)
                {
                    var rs = new BackendAdapter(param).Post<List<HIS_SERVICE_RETY_CAT>>("api/HisServiceRetyCat/CreateList", ApiConsumer.ApiConsumers.MosConsumer, listServiceRetyCat, SessionManager.ActionLostToken, param);
                    if (rs != null)
                    {
                        success = true;
                        BtnSave.Enabled = false;
                        if (this.RefeshReference != null)
                        {
                            this.RefeshReference();
                        }
                    }
                }
                WaitingManager.Hide();
                #region Hien thi message thong bao
                MessageManager.Show(this, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                BtnSave_Click(null, null);
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
                    ADO.ServiceRetyCatAdo data = (ADO.ServiceRetyCatAdo)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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

        private void repositoryItemBtnLineError_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (ADO.ServiceRetyCatAdo)gridView.GetFocusedRow();
                if (row != null)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(row.ERROR);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtnDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (ADO.ServiceRetyCatAdo)gridView.GetFocusedRow();
                if (row != null)
                {
                    if (ListDataImport != null && ListDataImport.Count > 0)
                    {
                        ListDataImport.Remove(row);
                        AddListServiceRetyCatToProcessList(ListDataImport);
                        SetDataSource(ListDataImport);

                        if (checkClick)
                        {
                            if (BtnLineError.Text == GetLanguageControl("FormImportServiceRetyCat.Gc_LineError_Error"))
                            {
                                BtnLineError.Text = GetLanguageControl("FormImportServiceRetyCat.Gc_LineError_Ok");
                            }
                            else
                            {
                                BtnLineError.Text = GetLanguageControl("FormImportServiceRetyCat.Gc_LineError_Error");
                            }
                            BtnLineError_Click(null, null);
                        }
                    }
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
                    string ERROR = (view.GetRowCellValue(e.RowHandle, "ERROR") ?? "").ToString().Trim();
                    if (e.Column.FieldName == "ErrorLine")
                    {
                        if (!string.IsNullOrEmpty(ERROR))
                        {
                            e.RepositoryItem = repositoryItemBtnLineError;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
