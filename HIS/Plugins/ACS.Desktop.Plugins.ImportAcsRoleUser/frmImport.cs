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
using ACS.Desktop.Plugins.AcsImport.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
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
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace ACS.Desktop.Plugins.AcsImport
{
    public partial class frmImport : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module _Module { get; set; }
        RefeshReference delegateRefresh;
        List<ImportADO> _ImportAdos;
        List<ImportADO> _CurrentAdos;
        List<ACS_USER> _ListAcsUsers { get; set; }
        List<ACS_ROLE> _ListAcsRoles { get; set; }
        List<ACS_ROLE_USER> _ListAcsRoleUsers { get; set; }

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
                //_ListAcsUsers = new List<ACS_USER>();
                //ACS.Filter.AcsUserFilter filter = new ACS.Filter.AcsUserFilter();
                //_ListAcsUsers = new BackendAdapter(new CommonParam()).Get<List<ACS_USER>>("api/AcsUser/Get", ApiConsumers.AcsConsumer, filter, null);

                //_ListAcsRoleUsers = new List<ACS_ROLE_USER>();
                //ACS.Filter.AcsRoleUserFilter RoleUserFilter = new ACS.Filter.AcsRoleUserFilter();
                //_ListAcsRoleUsers = new BackendAdapter(new CommonParam()).Get<List<ACS_ROLE_USER>>("api/AcsRoleUser/Get", ApiConsumers.AcsConsumer, RoleUserFilter, null);

                //_ListAcsRoles = new List<ACS_ROLE>();
                //ACS.Filter.AcsRoleFilter roleFilter = new ACS.Filter.AcsRoleFilter();
                //_ListAcsRoles = new BackendAdapter(new CommonParam()).Get<List<ACS_ROLE>>("api/AcsRole/Get", ApiConsumers.AcsConsumer, roleFilter, null);
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

                string fileName = System.IO.Path.Combine(Application.StartupPath + "\\Tmp\\Imp\\", "IMPORT_ACS_ROLE_USER.xlsx");
                Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
                param.Messages = new List<string>();
                if (File.Exists(fileName))
                {
                    saveFileDialog.Title = "Save File";
                    saveFileDialog.FileName = "IMPORT_ACS_ROLE_USER";
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
            try
            {
                t1.Start();
                t2.Start();
                t3.Start();

                t1.Join();
                t2.Join();
                t3.Join();
                result = true;
            }
            catch (Exception ex)
            {
                t1.Abort();
                t2.Abort();
                t3.Abort();
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void Get1()
        {
            _ListAcsUsers = new List<ACS_USER>();
            ACS.Filter.AcsUserFilter filter = new ACS.Filter.AcsUserFilter();
            _ListAcsUsers = new BackendAdapter(new CommonParam()).Get<List<ACS_USER>>("api/AcsUser/Get", ApiConsumers.AcsConsumer, filter, null);
        }
        private void Get2()
        {
            _ListAcsRoleUsers = new List<ACS_ROLE_USER>();
            ACS.Filter.AcsRoleUserFilter RoleUserFilter = new ACS.Filter.AcsRoleUserFilter();
            _ListAcsRoleUsers = new BackendAdapter(new CommonParam()).Get<List<ACS_ROLE_USER>>("api/AcsRoleUser/Get", ApiConsumers.AcsConsumer, RoleUserFilter, null);
        }
        private void Get3()
        {
            _ListAcsRoles = new List<ACS_ROLE>();
            ACS.Filter.AcsRoleFilter roleFilter = new ACS.Filter.AcsRoleFilter();
            _ListAcsRoles = new BackendAdapter(new CommonParam()).Get<List<ACS_ROLE>>("api/AcsRole/Get", ApiConsumers.AcsConsumer, roleFilter, null);
        }

        bool checkNull(ImportADO data)
        {
            bool result = true;
            try
            {
                if (data != null)
                {
                    if (string.IsNullOrEmpty(data.ROLE_CODE)
                        && string.IsNullOrEmpty(data.LOGINNAME))
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

                    if (!string.IsNullOrEmpty(item.LOGINNAME))
                    {
                        if (item.LOGINNAME.Length > 50)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "LOGINNAME");
                        }
                        else
                        {
                            var dataOld = this._ListAcsUsers.FirstOrDefault(p => p.LOGINNAME == item.LOGINNAME.Trim());
                            if (dataOld != null)
                            {
                                serAdo.USER_ID = dataOld.ID;
                                serAdo.USERNAME = dataOld.USERNAME;
                            }
                            else
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "LOGINNAME");
                            }
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "LOGINNAME");
                    }
                    if (!string.IsNullOrEmpty(item.ROLE_CODE))
                    {
                        if (Encoding.UTF8.GetByteCount(item.ROLE_CODE.Trim()) > 8)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "ROLE_CODE");
                        }
                        else
                        {
                            var dataOld = this._ListAcsRoles.FirstOrDefault(p => p.ROLE_CODE == item.ROLE_CODE.Trim());
                            if (dataOld != null)
                            {
                                serAdo.ROLE_ID = dataOld.ID;
                                serAdo.ROLE_NAME = dataOld.ROLE_NAME;
                            }
                            else
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "ROLE_CODE");
                            }
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "ROLE_CODE");
                    }
                    if (!string.IsNullOrEmpty(item.ROLE_CODE) && !string.IsNullOrEmpty(item.LOGINNAME))
                    {
                        var checkTrungs = _service.Where(p => p.LOGINNAME.Trim() == item.LOGINNAME.Trim() && p.ROLE_CODE.Trim() == item.ROLE_CODE.Trim()).ToList();
                        if (checkTrungs != null && checkTrungs.Count > 1)
                        {
                            error += string.Format(Message.MessageImport.FileImportDaTonTai, item.LOGINNAME.Trim() + " - " + item.ROLE_CODE.Trim());
                        }
                    }
                    if (serAdo.ROLE_ID > 0 && serAdo.USER_ID > 0 && this._ListAcsRoleUsers != null && this._ListAcsRoleUsers.Count > 0)
                    {
                        var dataS = this._ListAcsRoleUsers.FirstOrDefault(p => p.USER_ID == serAdo.USER_ID && p.ROLE_ID == serAdo.ROLE_ID);
                        if (dataS != null)
                        {
                            error += string.Format(Message.MessageImport.DaTonTai, "ROLE_CODE,LOGINNAME");
                        }
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
                        var dataCheck = this._ImportAdos.Where(p => p.LOGINNAME.Trim() == row.LOGINNAME.Trim() && p.ROLE_CODE.Trim() == row.ROLE_CODE.Trim()).ToList();
                        if (dataCheck != null && dataCheck.Count == 1)
                        {
                            if (!string.IsNullOrEmpty(dataCheck[0].ERROR))
                            {
                                string erro = string.Format(Message.MessageImport.FileImportDaTonTai, dataCheck[0].LOGINNAME.Trim() + " - " + dataCheck[0].ROLE_CODE.Trim());
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
                List<ACS_ROLE_USER> datas = new List<ACS_ROLE_USER>();

                if (this._ImportAdos != null && this._ImportAdos.Count > 0)
                {
                    datas.AddRange(this._ImportAdos);
                    //foreach (var item in this._ImportAdos)
                    //{
                    //    ACS_ROLE_USER ado = new ACS_ROLE_USER();
                    //    ado.ROLE_ID = item.ROLE_ID;
                    //    ado.USER_ID = item.USER_ID;
                    //    datas.Add(ado);
                    //}
                }
                else
                {
                    WaitingManager.Hide();
                    DevExpress.XtraEditors.XtraMessageBox.Show("Dữ liệu rỗng", "Thông báo");
                    return;
                }

                CommonParam param = new CommonParam();
                var dataImports = new BackendAdapter(param).Post<List<ACS_ROLE_USER>>("api/AcsRoleUser/CreateList", ApiConsumers.AcsConsumer, datas, param);
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
