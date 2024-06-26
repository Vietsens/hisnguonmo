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
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.BackendData;
using SAR.Desktop.Plugins.SarImportRetyFofi.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.SDO;
using SAR.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SAR.Desktop.Plugins.SarImportRetyFofi
{
    public partial class frmSarImportRetyFofi : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module _Module { get; set; }
        RefeshReference delegateRefresh;
        List<RetyFofiADO> _RetyFofiAdos;
        List<RetyFofiADO> _CurrentAdos;
        List<SAR_RETY_FOFI> _ListRetyFofis { get; set; }

        public frmSarImportRetyFofi()
        {
            InitializeComponent();
        }

        public frmSarImportRetyFofi(Inventec.Desktop.Common.Modules.Module _module)
            : base(_module)
        {
            InitializeComponent();
            try
            {
                this._Module = _module;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public frmSarImportRetyFofi(Inventec.Desktop.Common.Modules.Module _module, RefeshReference _delegateRefresh)
            : base(_module)
        {
            InitializeComponent();
            try
            {
                this._Module = _module;
                this.delegateRefresh = _delegateRefresh;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmImportBed_Load(object sender, EventArgs e)
        {
            try
            {
                if (this._Module != null)
                {
                    this.Text = this._Module.text;
                }
                SetIcon();
                LoadDataRetyFofi();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataRetyFofi()
        {
            try
            {
                _ListRetyFofis = new List<SAR_RETY_FOFI>();
                MOS.Filter.HisBedFilter filter = new MOS.Filter.HisBedFilter();
                _ListRetyFofis = new BackendAdapter(new CommonParam()).Get<List<SAR_RETY_FOFI>>("api/SarRetyFofi/Get", ApiConsumers.SarConsumer, filter, null);
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
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnDownLoadFile_Click(object sender, EventArgs e)
        {
            try
            {

                string fileName = System.IO.Path.Combine(Application.StartupPath + "\\Tmp\\Imp\\", "IMPORT_RETY_FOFI.xlsx");
                Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
                param.Messages = new List<string>();
                if (File.Exists(fileName))
                {
                    saveFileDialog.Title = "Save File";
                    saveFileDialog.FileName = "IMPORT_RETY_FOFI";
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
                        var hisServiceImport = import.GetWithCheck<RetyFofiADO>(0);
                        if (hisServiceImport != null && hisServiceImport.Count > 0)
                        {
                            List<RetyFofiADO> listAfterRemove = new List<RetyFofiADO>();


                            foreach (var item in hisServiceImport)
                            {
                                bool checkNull = string.IsNullOrEmpty(item.FORM_FIELD_CODE)
                                    && string.IsNullOrEmpty(item.REPORT_TYPE_CODE)
                                    && string.IsNullOrEmpty(item.NUM_ORDER_STR)
                                    && string.IsNullOrEmpty(item.JSON_OUTPUT)
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
                                this._RetyFofiAdos = new List<RetyFofiADO>();
                                addServiceToProcessList(_CurrentAdos, ref this._RetyFofiAdos);
                                SetDataSource(this._RetyFofiAdos);
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

        private void addServiceToProcessList(List<RetyFofiADO> _service, ref List<RetyFofiADO> _retyFofiRef)
        {
            try
            {
                _retyFofiRef = new List<RetyFofiADO>();
                long i = 0;
                foreach (var item in _service)
                {
                    i++;
                    string error = "";
                    var serAdo = new RetyFofiADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<RetyFofiADO>(serAdo, item);

                    if (!string.IsNullOrEmpty(item.REPORT_TYPE_CODE) && !string.IsNullOrEmpty(item.FORM_FIELD_CODE))
                    {
                        var reportType = BackendDataWorker.Get<SAR_REPORT_TYPE>().FirstOrDefault(p => p.REPORT_TYPE_CODE == item.REPORT_TYPE_CODE.Trim());
                        var formField = BackendDataWorker.Get<SAR_FORM_FIELD>().FirstOrDefault(p => p.FORM_FIELD_CODE == item.FORM_FIELD_CODE.Trim());
                        SAR_RETY_FOFI SarRetyFifo = new SAR_RETY_FOFI();
                        long number = 0;
                        if (reportType != null && formField != null)
                        {
                            if (!string.IsNullOrEmpty(item.NUM_ORDER_STR))
                            {
                                if (Int64.TryParse(item.NUM_ORDER_STR, out number))
                                    SarRetyFifo = this._ListRetyFofis.FirstOrDefault(p => p.REPORT_TYPE_ID == reportType.ID && p.FORM_FIELD_ID == formField.ID && p.NUM_ORDER == number);

                            }
                            else
                                SarRetyFifo = this._ListRetyFofis.FirstOrDefault(p => p.REPORT_TYPE_ID == reportType.ID && p.FORM_FIELD_ID == formField.ID);
                        }
                        if (SarRetyFifo != null)
                        {
                            error += string.Format(Message.MessageImport.DaTonTai, "Biểu mẫu đã có trường lọc và thứ tự hiển thị");
                        }
                    }
                    if (!string.IsNullOrEmpty(item.REPORT_TYPE_CODE))
                    {
                        var reportType = BackendDataWorker.Get<SAR_REPORT_TYPE>().FirstOrDefault(p => p.REPORT_TYPE_CODE == item.REPORT_TYPE_CODE.Trim());
                        if (reportType != null)
                        {
                            if (reportType.IS_ACTIVE == 1)
                            {
                                serAdo.REPORT_TYPE_ID = reportType.ID;
                                serAdo.REPORT_TYPE_CODE = reportType.REPORT_TYPE_CODE;
                                serAdo.REPORT_TYPE_NAME = reportType.REPORT_TYPE_NAME;
                            }
                            else
                            {
                                error += string.Format(Message.MessageImport.MaLoaiBaoCao, "Mã loại báo cáo");
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

                    if (!string.IsNullOrEmpty(item.FORM_FIELD_CODE))
                    {
                        var formField = BackendDataWorker.Get<SAR_FORM_FIELD>().FirstOrDefault(p => p.FORM_FIELD_CODE == item.FORM_FIELD_CODE.Trim());
                        if (formField != null)
                        {
                            if (formField.IS_ACTIVE == 1)
                            {
                                serAdo.FORM_FIELD_ID = formField.ID;
                                serAdo.FORM_FIELD_CODE = formField.FORM_FIELD_CODE;
                            }
                            else
                            {
                                error += string.Format(Message.MessageImport.MaTruongLoc, "Mã trường lọc");
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Mã trường lọc");
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Mã trường lọc");
                    }
                    if (string.IsNullOrEmpty(item.JSON_OUTPUT))
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Đầu ra");
                    if (string.IsNullOrEmpty(item.NUM_ORDER_STR))
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Số thứ tự");

                    if (!string.IsNullOrEmpty(item.IS_REQUIRE_STR))
                    {
                        if (item.IS_REQUIRE_STR == "x")
                        {
                            serAdo.IS_REQUIRE_DISPLAY = true;
                            serAdo.IS_REQUIRE = (short)1;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Trường bắt buộc truyền vào");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.NUM_ORDER_STR))
                    {
                        long MaxCapacity = 0;
                        if (Int64.TryParse(item.NUM_ORDER_STR, out MaxCapacity))
                        {
                            serAdo.NUM_ORDER = MaxCapacity;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Số thứ tự");
                        }
                    }

                    

                    serAdo.ERROR = error;
                    serAdo.ID = i;

                    _retyFofiRef.Add(serAdo);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void SetDataSource(List<RetyFofiADO> dataSource)
        {
            try
            {
                gridControlData.BeginUpdate();
                gridControlData.DataSource = null;
                gridControlData.DataSource = dataSource;
                gridControlData.EndUpdate();
                CheckErrorLine(null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CheckErrorLine(List<RetyFofiADO> dataSource)
        {
            try
            {
                var checkError = this._RetyFofiAdos.Exists(o => !string.IsNullOrEmpty(o.ERROR));
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
                    var errorLine = this._RetyFofiAdos.Where(o => !string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(errorLine);

                }
                else
                {
                    btnShowLineError.Text = "Dòng lỗi";
                    var errorLine = this._RetyFofiAdos.Where(o => string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(errorLine);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButton_ER_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (RetyFofiADO)gridViewData.GetFocusedRow();
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

        private void repositoryItemButton_Delete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (RetyFofiADO)gridViewData.GetFocusedRow();
                if (row != null)
                {
                    if (this._RetyFofiAdos != null && this._RetyFofiAdos.Count > 0)
                    {
                        this._RetyFofiAdos.Remove(row);
                        var dataCheck = this._RetyFofiAdos.Where(p => p.REPORT_TYPE_CODE == row.REPORT_TYPE_CODE && p.FORM_FIELD_CODE == row.FORM_FIELD_CODE).ToList();
                        if (dataCheck != null && dataCheck.Count == 1)
                        {
                            if (!string.IsNullOrEmpty(dataCheck[0].ERROR))
                            {
                                string erro = string.Format(Message.MessageImport.FileImportDaTonTai, dataCheck[0]);
                                //string[] Codes = dataCheck[0].ERROR.Split('|');
                                dataCheck[0].ERROR = dataCheck[0].ERROR.Replace(erro, "");
                            }

                        }
                        SetDataSource(this._RetyFofiAdos);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewData_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    RetyFofiADO pData = (RetyFofiADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                bool success = false;
                WaitingManager.Show();
                List<SAR_RETY_FOFI> datas = new List<SAR_RETY_FOFI>();

                if (this._RetyFofiAdos != null && this._RetyFofiAdos.Count > 0)
                {
                    foreach (var item in this._RetyFofiAdos)
                    {
                        SAR_RETY_FOFI ado = new SAR_RETY_FOFI();
                        ado.REPORT_TYPE_ID = item.REPORT_TYPE_ID;
                        ado.FORM_FIELD_ID = item.FORM_FIELD_ID;
                        ado.NUM_ORDER = item.NUM_ORDER;
                        ado.DESCRIPTION = item.DESCRIPTION;
                        ado.JSON_OUTPUT = item.JSON_OUTPUT;
                        ado.IS_REQUIRE = item.IS_REQUIRE;
                        datas.Add(ado);
                    }
                }
                else
                {
                    WaitingManager.Hide();
                    DevExpress.XtraEditors.XtraMessageBox.Show("Dữ liệu rỗng", "Thông báo");
                    return;
                }

                CommonParam param = new CommonParam();
                var dataImports = new BackendAdapter(param).Post<List<SAR_RETY_FOFI>>("api/SarRetyFofi/CreateList", ApiConsumers.SarConsumer, datas, param);
                WaitingManager.Hide();
                if (dataImports != null && dataImports.Count > 0)
                {
                    success = true;
                    btnImport.Enabled = false;
                    LoadDataRetyFofi();
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

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnImport.Enabled)
                    btnImport_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewData_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    //BedADO data = (BedADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    string error = (gridViewData.GetRowCellValue(e.RowHandle, "ERROR") ?? "").ToString();
                    if (e.Column.FieldName == "ERROR_")
                    {
                        if (!string.IsNullOrEmpty(error))
                        {
                            e.RepositoryItem = repositoryItemButton_ER;
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
