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
using HIS.Desktop.Plugins.ImportHisBedBsty.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using SDA.EFMODEL.DataModels;
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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ImportHisBedBsty
{
    public partial class frmImport : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module _Module { get; set; }
        RefeshReference delegateRefresh;
        List<ImportADO> _ImportAdos;
        List<ImportADO> _CurrentAdos;
        List<V_HIS_BED> _ListHisBed { get; set; }
        List<HIS_SERVICE> _ListHisServices { get; set; }
        List<HIS_DEPARTMENT> _ListHisDepartment { get; set; }
        List<HIS_BED_BSTY> _ListHisBedBstys { get; set; }

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
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadCurrentData()
        {
            try
            {
                ProcessGetData();
                //_ListHisBed = new List<HIS_BED>();
                //ACS.Filter.AcsUserFilter filter = new ACS.Filter.AcsUserFilter();
                //_ListHisBed = new BackendAdapter(new CommonParam()).Get<List<HIS_BED>>("api/AcsUser/Get", ApiConsumers.AcsConsumer, filter, null);

                //_ListHisBedBstys = new List<HIS_BED_BSTY>();
                //ACS.Filter.AcsRoleUserFilter RoleUserFilter = new ACS.Filter.AcsRoleUserFilter();
                //_ListHisBedBstys = new BackendAdapter(new CommonParam()).Get<List<HIS_BED_BSTY>>("api/AcsRoleUser/Get", ApiConsumers.AcsConsumer, RoleUserFilter, null);

                //_ListHisServices = new List<HIS_SERVICE>();
                //ACS.Filter.AcsRoleFilter roleFilter = new ACS.Filter.AcsRoleFilter();
                //_ListHisServices = new BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE>>("api/AcsRole/Get", ApiConsumers.AcsConsumer, roleFilter, null);
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

                string fileName = System.IO.Path.Combine(Application.StartupPath + "\\Tmp\\Imp\\", "IMPORT_BED_BSTY.xlsx");
                Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
                param.Messages = new List<string>();
                if (File.Exists(fileName))
                {
                    saveFileDialog.Title = "Save File";
                    saveFileDialog.FileName = "IMPORT_BED_BSTY";
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
                    LoadCurrentData();
                    var import = new Inventec.Common.ExcelImport.Import();
                    if (import.ReadFileExcel(ofd.FileName))
                    {
                        var hisServiceImport = import.GetWithCheck<ImportADO>(0);
                        if (hisServiceImport != null && hisServiceImport.Count > 0)
                        {
                            WaitingManager.Hide();

                            this._CurrentAdos = hisServiceImport.Where(p => checkNull(p)).ToList();

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

        private bool ProcessGetData()
        {
            bool result = false;
            Thread t1 = new Thread(Get1);
            Thread t2 = new Thread(Get2);
            Thread t3 = new Thread(Get3);
            Thread t4 = new Thread(Get4);
            try
            {
                t1.Start();
                t2.Start();
                t3.Start();
                t4.Start();

                t1.Join();
                t2.Join();
                t3.Join();
                t4.Join();
                result = true;
            }
            catch (Exception ex)
            {
                t1.Abort();
                t2.Abort();
                t3.Abort();
                t4.Abort();
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void Get1()
        {
            _ListHisBed = new List<V_HIS_BED>();
            HisBedFilter filter = new HisBedFilter();
            _ListHisBed = new BackendAdapter(new CommonParam()).Get<List<V_HIS_BED>>("api/HisBed/GetView", ApiConsumers.MosConsumer, filter, null);
        }
        private void Get2()
        {
            _ListHisBedBstys = new List<HIS_BED_BSTY>();
            HisBedBstyFilter bedBstyFilter = new HisBedBstyFilter();
            _ListHisBedBstys = new BackendAdapter(new CommonParam()).Get<List<HIS_BED_BSTY>>("api/HisBedBsty/Get", ApiConsumers.MosConsumer, bedBstyFilter, null);
        }
        private void Get3()
        {
            _ListHisServices = new List<HIS_SERVICE>();
            HisServiceFilter serviceFilter = new HisServiceFilter();
            _ListHisServices = new BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE>>("api/HisService/Get", ApiConsumers.MosConsumer, serviceFilter, null);
        }

        private void Get4()
        {
            _ListHisDepartment = new List<HIS_DEPARTMENT>();
            HisDepartmentFilter departmentFilter = new HisDepartmentFilter();
            _ListHisDepartment = new BackendAdapter(new CommonParam()).Get<List<HIS_DEPARTMENT>>("api/HisDepartment/Get", ApiConsumers.MosConsumer, departmentFilter, null);
        }
        bool checkNull(ImportADO data)
        {
            bool result = true;
            try
            {
                if (data != null)
                {
                    if (string.IsNullOrEmpty(data.BED_CODE)
                        && string.IsNullOrEmpty(data.SERVICE_CODE) && string.IsNullOrEmpty(data.DEPARTMENT_CODE))
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
                result = false;
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

                    //Mã giường
                    if (!string.IsNullOrEmpty(item.BED_CODE))
                    {
                        if (item.BED_CODE.Length > 19)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Mã giường");
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(item.DEPARTMENT_CODE))
                                error += string.Format(Message.MessageImport.ThieuTruongDL, "Mã khoa");
                            else
                            {
                                var dataOld = this._ListHisBed.FirstOrDefault(p => p.BED_CODE == item.BED_CODE.Trim() && p.DEPARTMENT_CODE == item.DEPARTMENT_CODE.Trim());
                                if (dataOld != null)
                                {
                                    if (dataOld.IS_ACTIVE == 1)
                                    {
                                        serAdo.BED_ID = dataOld.ID;
                                        serAdo.BED_CODE = dataOld.BED_CODE;
                                        serAdo.BED_NAME = dataOld.BED_NAME;
                                    }
                                    else
                                        error += string.Format(Message.MessageImport.DulieuBiKhoa, "Mã giường");
                                }
                                else
                                    error += string.Format(Message.MessageImport.KhongHopLe, "Mã giường");
                            }

                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Mã giường");
                    }

                    //Mã dịch vụ
                    if (!string.IsNullOrEmpty(item.SERVICE_CODE))
                    {
                        if (item.SERVICE_CODE.Length > 19)
                            error += string.Format(Message.MessageImport.Maxlength, "Mã dịch vụ");

                        else
                        {
                            var dataOld = this._ListHisServices.FirstOrDefault(p => p.SERVICE_CODE == item.SERVICE_CODE.Trim());
                            if (dataOld != null)
                            {
                                if (dataOld.IS_ACTIVE == 1)
                                {
                                    if (dataOld.SERVICE_TYPE_ID == 8) //mã loại dịch vụ giường (HIS_SERVICE_TYPE)
                                    {

                                        serAdo.BED_SERVICE_TYPE_ID = dataOld.ID;
                                        serAdo.SERVICE_CODE = dataOld.SERVICE_CODE;
                                        serAdo.SERVICE_NAME = dataOld.SERVICE_NAME;
                                    }

                                    else
                                        error += string.Format(Message.MessageImport.KhongPhaiDichVuGiuong, "Mã dịch vụ");
                                }
                                else
                                    error += string.Format(Message.MessageImport.DulieuBiKhoa, "Mã dịch vụ");
                            }
                            else

                                error += string.Format(Message.MessageImport.KhongHopLe, "Mã dịch vụ");

                        }
                    }
                    else
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Mã dịch vụ");


                    //Mã khoa
                    if (!string.IsNullOrEmpty(item.DEPARTMENT_CODE))
                    {
                        if (item.DEPARTMENT_CODE.Length > 10)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Mã khoa");
                        }
                        else
                        {
                            var dataOld = this._ListHisDepartment.FirstOrDefault(p => p.DEPARTMENT_CODE == item.DEPARTMENT_CODE.Trim());
                            if (dataOld != null)
                            {
                                if (dataOld.IS_ACTIVE == 1)
                                {
                                    serAdo.DEPARTMENT_ID = dataOld.ID;
                                    serAdo.DEPARTMENT_CODE = dataOld.DEPARTMENT_CODE;
                                    serAdo.DEPARTMENT_NAME = dataOld.DEPARTMENT_NAME;
                                }
                                else
                                    error += string.Format(Message.MessageImport.DulieuBiKhoa, "Mã khoa");
                            }
                            else
                                error += string.Format(Message.MessageImport.KhongHopLe, "Mã khoa");

                        }
                    }


                    if (!string.IsNullOrEmpty(item.BED_CODE) && !string.IsNullOrEmpty(item.SERVICE_CODE) && !string.IsNullOrEmpty(item.DEPARTMENT_CODE))
                    {
                        var checkTrungs = _service.Where(p => p.BED_CODE.Trim() == item.BED_CODE.Trim() && p.SERVICE_CODE.Trim() == item.SERVICE_CODE.Trim() && p.DEPARTMENT_CODE.Trim() == item.DEPARTMENT_CODE.Trim()).ToList();
                        if (checkTrungs != null && checkTrungs.Count > 1)
                            error += string.Format(Message.MessageImport.FileImportDaTonTai, item.BED_CODE.Trim() + " - " + item.SERVICE_CODE.Trim() + " - " + item.DEPARTMENT_CODE.Trim());

                    }
                    if (serAdo.BED_ID > 0 && serAdo.BED_SERVICE_TYPE_ID >0  && this._ListHisBedBstys != null && this._ListHisBedBstys.Count > 0)
                    {
                        var dataS = this._ListHisBedBstys.FirstOrDefault(p => p.BED_ID == serAdo.BED_ID && p.BED_SERVICE_TYPE_ID == serAdo.BED_SERVICE_TYPE_ID);
                        if (dataS != null)
                            error += string.Format(Message.MessageImport.DaTonTai, "Mã giường, Mã dịch vụ");

                    }


                    serAdo.ERROR = error;
                    //serAdo.ID = i;

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
                        var dataCheck = this._ImportAdos.Where(p => p.BED_CODE.Trim() == row.BED_CODE.Trim() && p.SERVICE_CODE.Trim() == row.SERVICE_CODE.Trim() && p.DEPARTMENT_CODE.Trim() == row.DEPARTMENT_CODE.Trim()).ToList();
                        if (dataCheck != null && dataCheck.Count == 1)
                        {
                            if (!string.IsNullOrEmpty(dataCheck[0].ERROR))
                            {
                                string erro = string.Format(Message.MessageImport.FileImportDaTonTai, dataCheck[0].BED_CODE.Trim() + " - " + dataCheck[0].SERVICE_CODE.Trim() + " - " + dataCheck[0].DEPARTMENT_CODE.Trim());
                                //string[] Codes = dataCheck[0].ERROR.Split('|');
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
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    ImportADO pData = (ImportADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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
                List<HIS_BED_BSTY> datas = new List<HIS_BED_BSTY>();

                if (this._ImportAdos != null && this._ImportAdos.Count > 0)
                {
                    //datas.AddRange(this._ImportAdos);
                    foreach (var item in this._ImportAdos)
                    {
                        HIS_BED_BSTY ado = new HIS_BED_BSTY();
                        ado.BED_ID = item.BED_ID;
                        ado.BED_SERVICE_TYPE_ID = item.BED_SERVICE_TYPE_ID;
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
                var dataImports = new BackendAdapter(param).Post<List<HIS_BED_BSTY>>("api/HisBedBsty/CreateList", ApiConsumers.MosConsumer, datas, param);
                WaitingManager.Hide();
                if (dataImports != null && dataImports.Count > 0)
                {
                    success = true;
                    btnImport.Enabled = false;
                    LoadCurrentData();
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
                    //BedRoomADO data = (BedRoomADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
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
