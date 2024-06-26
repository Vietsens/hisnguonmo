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
using ACS.EFMODEL.DataModels;
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.HisImport.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
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

namespace HIS.Desktop.Plugins.HisImport
{
    public partial class frmImport : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module _Module { get; set; }
        RefeshReference delegateRefresh;
        List<ImportADO> _ImportAdos;
        List<ImportADO> _CurrentAdos;
        List<HIS_SUPPLIER> _ListSuppliers { get; set; }

        public frmImport()
        {
            InitializeComponent();
        }

        public frmImport(Inventec.Desktop.Common.Modules.Module _module)
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

        public frmImport(Inventec.Desktop.Common.Modules.Module _module, RefeshReference _delegateRefresh)
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

        private void frmImport_Load(object sender, EventArgs e)
        {
            try
            {
                if (this._Module != null)
                {
                    this.Text = this._Module.text;
                }
                SetIcon();
                LoadCurrentDataSupplier();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadCurrentDataSupplier()
        {
            try
            {
                _ListSuppliers = new List<HIS_SUPPLIER>();
                MOS.Filter.HisSupplierFilter filter = new MOS.Filter.HisSupplierFilter();
                _ListSuppliers = new BackendAdapter(new CommonParam()).Get<List<HIS_SUPPLIER>>("api/HisSupplier/Get", ApiConsumers.MosConsumer, filter, null);
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

                string fileName = System.IO.Path.Combine(Application.StartupPath + "\\Tmp\\Imp\\", "IMPORT_SUPPLIER.xlsx");
                Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
                param.Messages = new List<string>();
                if (File.Exists(fileName))
                {
                    saveFileDialog.Title = "Save File";
                    saveFileDialog.FileName = "IMPORT_SUPPLIER";
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
                        var hisServiceImport = import.GetWithCheck<ImportADO>(0);
                        if (hisServiceImport != null && hisServiceImport.Count > 0)
                        {
                            this._CurrentAdos = hisServiceImport.Where(p => checkNull(p)).ToList();

                            //List<ImportADO> listAfterRemove = new List<ImportADO>();
                            //foreach (var item in hisServiceImport)
                            //{
                            //    bool checkNull = string.IsNullOrEmpty(item.LOGINNAME)
                            //        ;
                            //    if (!checkNull)
                            //    {
                            //        listAfterRemove.Add(item);
                            //    }
                            //} 
                            WaitingManager.Hide();

                            //this._CurrentAdos = listAfterRemove;

                            if (this._CurrentAdos != null && this._CurrentAdos.Count > 0)
                            {
                                btnShowLineError.Enabled = true;
                                this._ImportAdos = new List<ImportADO>();
                                addServiceToProcessList(_CurrentAdos, ref this._ImportAdos);
                                SetDataSource(this._ImportAdos);
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
        bool checkNull(ImportADO data)
        {
            bool result = true;

            try
            {
                if (data != null)
                {
                    if (string.IsNullOrEmpty(data.SUPPLIER_CODE)
                        && string.IsNullOrEmpty(data.SUPPLIER_NAME)
                        && string.IsNullOrEmpty(data.SUPPLIER_SHORT_NAME)
                        && string.IsNullOrEmpty(data.EMAIL)
                        && string.IsNullOrEmpty(data.IS_BLOOD_STR)
                        && string.IsNullOrEmpty(data.PHONE)
                        && string.IsNullOrEmpty(data.TAX_CODE)
                        && string.IsNullOrEmpty(data.ADDRESS)
                        )
                    {
                        result = false;
                    }
                }
                else
                    result = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return result;

            }
            return result;

        }


        private void addServiceToProcessList(List<ImportADO> _service, ref List<ImportADO> _importAdoRef)
        {
            try
            {
                _importAdoRef = new List<ImportADO>();
                long i = 0;
                foreach (var item in _service)
                {
                    i++;
                    string error = "";
                    var serAdo = new ImportADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<ImportADO>(serAdo, item);

                    if (!string.IsNullOrEmpty(item.SUPPLIER_CODE))
                    {
                        if (item.SUPPLIER_CODE.Length > 10)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "SUPPLIER_CODE");
                        }
                        else
                        {
                            var dataCheckold = this._ListSuppliers.FirstOrDefault(p => p.SUPPLIER_CODE == item.SUPPLIER_CODE);
                            if (dataCheckold != null)
                            {
                                error += string.Format(Message.MessageImport.DaTonTai, "SUPPLIER_CODE");
                            }
                            else
                            {
                                var checkTrung = _service.Where(p => p.SUPPLIER_CODE == item.SUPPLIER_CODE).ToList();
                                if (checkTrung != null && checkTrung.Count > 1)
                                {
                                    error += string.Format(Message.MessageImport.FileImportDaTonTai, item.SUPPLIER_CODE);
                                }
                            }
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "SUPPLIER_CODE");
                    }
                    if (!string.IsNullOrEmpty(item.SUPPLIER_NAME))
                    {
                        if (Encoding.UTF8.GetByteCount(item.SUPPLIER_NAME.Trim()) > 1000)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "SUPPLIER_NAME");
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "SUPPLIER_NAME");
                    }
                    if (!string.IsNullOrEmpty(item.SUPPLIER_SHORT_NAME))
                    {
                        if (Encoding.UTF8.GetByteCount(item.SUPPLIER_SHORT_NAME.Trim()) > 100)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "SUPPLIER_SHORT_NAME");
                        }
                    }
                    if (!string.IsNullOrEmpty(item.EMAIL))
                    {
                        if (Encoding.UTF8.GetByteCount(item.EMAIL.Trim()) > 100)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "EMAIL");
                        }
                    }
                    if (!string.IsNullOrEmpty(item.IS_BLOOD_STR))
                    {
                        if (item.IS_BLOOD_STR.ToLower().Trim() == "x")
                        {
                            serAdo.IS_BLOOD = 1;

                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Cung cấp máu và chế phẩm");
                        }
                    }
                    if (!string.IsNullOrEmpty(item.PHONE))
                    {
                        if (Encoding.UTF8.GetByteCount(item.PHONE.Trim()) > 20)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "PHONE");
                        }
                    }
                    if (!string.IsNullOrEmpty(item.TAX_CODE))
                    {
                        if (Encoding.UTF8.GetByteCount(item.TAX_CODE.Trim()) > 13)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "TAX_CODE");
                        }
                    }
                    if (!string.IsNullOrEmpty(item.ADDRESS))
                    {
                        if (Encoding.UTF8.GetByteCount(item.ADDRESS.Trim()) > 2000)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "ADDRESS");
                        }
                    }
                    serAdo.ERROR = error;
                    serAdo.ID = i;
                    _importAdoRef.Add(serAdo);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void SetDataSource(List<ImportADO> dataSource)
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

        private void CheckErrorLine(List<ImportADO> dataSource)
        {
            try
            {
                var checkError = this._ImportAdos.Exists(o => !string.IsNullOrEmpty(o.ERROR));
                if (!checkError)
                {
                    btnImport.Enabled = true;
                    btnShowLineError.Enabled = false;
                }
                else
                {
                    btnShowLineError.Enabled = true;
                    btnImport.Enabled = true;
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
                    var errorLine = this._ImportAdos.Where(o => !string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(errorLine);

                }
                else
                {
                    btnShowLineError.Text = "Dòng lỗi";
                    var errorLine = this._ImportAdos.Where(o => string.IsNullOrEmpty(o.ERROR)).ToList();
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
                var row = (ImportADO)gridViewData.GetFocusedRow();
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
                var row = (ImportADO)gridViewData.GetFocusedRow();
                if (row != null)
                {
                    if (this._ImportAdos != null && this._ImportAdos.Count > 0)
                    {
                        this._ImportAdos.Remove(row);
                        var dataCheck = this._ImportAdos.Where(p => p.SUPPLIER_CODE == row.SUPPLIER_CODE).ToList();
                        if (dataCheck != null && dataCheck.Count == 1)
                        {
                            if (!string.IsNullOrEmpty(dataCheck[0].ERROR))
                            {
                                string erro = string.Format(Message.MessageImport.FileImportDaTonTai, dataCheck[0].SUPPLIER_CODE);
                                dataCheck[0].ERROR = dataCheck[0].ERROR.Replace(erro, "");
                            }

                        }
                        SetDataSource(this._ImportAdos);
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
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    ImportADO pData = (ImportADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    if (e.Column.FieldName == "IS_BLOOD_STR_BOOL")
                    {

                        e.Value = pData.IS_BLOOD == 1 ? true : false;

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
                if (!btnImport.Enabled) return;
                btnImport.Focus();
                var lisData = (List<ImportADO>)gridControlData.DataSource;
                if (lisData == null || lisData.Count <= 0) return;
                if (lisData.Exists(o => String.IsNullOrEmpty(o.ERROR))) return;
                bool success = false;
                WaitingManager.Show();
                List<HIS_SUPPLIER> datas = new List<HIS_SUPPLIER>();

                if (this._ImportAdos != null && this._ImportAdos.Count > 0)
                {
                    foreach (var item in this._ImportAdos)
                    {
                        HIS_SUPPLIER ado = new HIS_SUPPLIER();
                        ado.SUPPLIER_CODE = item.SUPPLIER_CODE;
                        ado.SUPPLIER_NAME = item.SUPPLIER_NAME;
                        ado.SUPPLIER_SHORT_NAME = item.SUPPLIER_SHORT_NAME;
                        ado.EMAIL = item.EMAIL;
                        ado.IS_BLOOD = item.IS_BLOOD;
                        ado.PHONE = item.PHONE;
                        ado.TAX_CODE = item.TAX_CODE;
                        ado.ADDRESS = item.ADDRESS;
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
                var dataImports = new BackendAdapter(param).Post<List<HIS_SUPPLIER>>("api/HisSupplier/CreateList", ApiConsumers.MosConsumer, datas, param);
                WaitingManager.Hide();
                if (dataImports != null && dataImports.Count > 0)
                {
                    success = true;
                    btnImport.Enabled = false;
                    if (this.delegateRefresh != null)
                    {
                        LoadCurrentDataSupplier();
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
