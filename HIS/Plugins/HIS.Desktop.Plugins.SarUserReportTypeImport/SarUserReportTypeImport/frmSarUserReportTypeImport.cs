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
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.SarUserReportTypeImport.ADO;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using SAR.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.Plugins.SarUserReportTypeImport.Message;
using System.IO;
using DevExpress.Data;
using SAR.Filter;

namespace HIS.Desktop.Plugins.SarUserReportTypeImport
{
    public partial class frmSarUserReportTypeImport : FormBase
    {
        #region Declare
        Inventec.Desktop.Common.Modules.Module _Module { get; set; }
        RefeshReference delegateRefresh;
        List<SarUserReportTypeADO> _sarUserReportTypeAdos;
        List<SarUserReportTypeADO> _CurrentAdos;
        List<SAR_USER_REPORT_TYPE> _ListUserReportType { get; set; }
        #endregion

        #region Construct
        public frmSarUserReportTypeImport()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public frmSarUserReportTypeImport(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();

                this._Module = moduleData;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public frmSarUserReportTypeImport(Inventec.Desktop.Common.Modules.Module _module, RefeshReference _delegateRefresh)
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
        #endregion
        private void frmSarUserReportTypeImport_Load(object sender, EventArgs e)
        {
            try
            {
                if (this._Module != null)
                {
                    this.Text = this._Module.text;
                }
                SetIcon();
                LoadDataSarUserReportType();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataSarUserReportType()
        {
            try
            {
                _ListUserReportType = new List<SAR_USER_REPORT_TYPE>();
                SAR.Filter.SarUserReportTypeFilter filter = new SAR.Filter.SarUserReportTypeFilter();
                _ListUserReportType = new BackendAdapter(new CommonParam()).Get<List<SAR_USER_REPORT_TYPE>>("api/SarUserReportType/Get", ApiConsumers.MosConsumer, filter, null);
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
        private void SetDataSource(List<SarUserReportTypeADO> dataSource)
        {
            try
            {
                gridControlSarUserReportTypeImport.BeginUpdate();
                gridControlSarUserReportTypeImport.DataSource = null;
                gridControlSarUserReportTypeImport.DataSource = dataSource;
                gridControlSarUserReportTypeImport.EndUpdate();
                CheckErrorLine(null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void CheckErrorLine(List<SarUserReportTypeADO> dataSource)
        {
            try
            {
                var checkError = this._sarUserReportTypeAdos.Exists(o => !string.IsNullOrEmpty(o.ERROR));
                if (!checkError)
                {
                    btnSave.Enabled = true;
                    btnShowLineError.Enabled = false;
                }
                else
                {
                    btnShowLineError.Enabled = true;
                    btnSave.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
        private void addServiceToProcessList(List<SarUserReportTypeADO> _CurrentService, ref List<SarUserReportTypeADO> _userRoomAdos)
        {
            try
            {
                _userRoomAdos = new List<SarUserReportTypeADO>();
                long i = 0;
                foreach (var item in _CurrentService)
                {
                    i++;
                    string error = "";
                    var serAdo = new SarUserReportTypeADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<SarUserReportTypeADO>(serAdo, item);

                    //if (!string.IsNullOrEmpty(item.GROUP_CODE))
                    //{
                    //    if (item.GROUP_CODE.Length > 50)
                    //    {
                    //        error += string.Format(Message.MessageImport.Maxlength, "Mã nhóm");
                    //    }
                    //}
                    //else
                    //{
                    //    error += string.Format(Message.MessageImport.ThieuTruongDL, "Mã nhóm");
                    //}

                    var checkTrung12 = _CurrentService.Where(p => p.GROUP_CODE == item.GROUP_CODE && p.REPORT_TYPE_ID == item.REPORT_TYPE_ID && p.REPORT_TYPE_CODE == item.REPORT_TYPE_CODE).ToList();
                    if (checkTrung12 != null && checkTrung12.Count > 1)
                    {
                        error += string.Format(Message.MessageImport.FileImportDaTonTai, item.REPORT_TYPE_ID);
                    }
                    //if (!string.IsNullOrEmpty(item.REPORT_TYPE_CODE))
                    //{
                    //    if (item.REPORT_TYPE_CODE.Length > 10)
                    //    {
                    //        error += string.Format(Message.MessageImport.Maxlength, "Mã báo cáo");
                    //    }
                    //}
                    //else
                    //{
                    //    error += string.Format(Message.MessageImport.ThieuTruongDL, "Mã báo cáo");
                    //}

                    //if (!string.IsNullOrEmpty(item.REPORT_TYPE_NAME))
                    //{
                    //    if (item.REPORT_TYPE_NAME.Length > 100)
                    //    {
                    //        error += string.Format(Message.MessageImport.Maxlength, "Tên báo cáo");
                    //    }
                    //}
                    //else
                    //{
                    //    error += string.Format(Message.MessageImport.ThieuTruongDL, "Tên báo cáo");
                    //}

                    if (!string.IsNullOrEmpty(item.LOGINNAME))
                    {
                        if (item.LOGINNAME.Length > 50)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Tên đăng nhập");
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Tên đăng nhập");
                    }

                    if (!string.IsNullOrEmpty(item.REPORT_TYPE_CODE))
                    {
                        if (item.REPORT_TYPE_CODE.Length > 10)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Mã phòng");
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Mã phòng");
                    }

                    if (!string.IsNullOrEmpty((item.REPORT_TYPE_CODE).ToString()))
                    {
                        var userRoom = BackendDataWorker.Get<SAR_USER_REPORT_TYPE>().FirstOrDefault(p => p.REPORT_TYPE_ID == item.REPORT_TYPE_ID);
                        if (userRoom != null)
                        {
                            if (userRoom.IS_ACTIVE == 1)
                            {
                                serAdo.REPORT_TYPE_ID = userRoom.REPORT_TYPE_ID;
                            }
                            else
                            {
                                error += string.Format(Message.MessageImport.MaLoaiBaoCaoDaKhoa, "Loại báo cáo");
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Loại báo cáo");
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Mã phòng");
                    }

                    serAdo.ERROR = error;
                    serAdo.ID = i;

                    _userRoomAdos.Add(serAdo);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnDownLoadFile_Click(object sender, EventArgs e)
        {
            try
            {
                string fileName = System.IO.Path.Combine(Application.StartupPath + "\\Tmp\\Imp\\", "IMPORT_SAR_USER_REPORT_TYPE.xlsx");
                Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
                param.Messages = new List<string>();
                if (File.Exists(fileName))
                {
                    saveFileDialog.Title = "Save File";
                    saveFileDialog.FileName = "IMPORT_SAR_USER_REPORT_TYPE";
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
                        var sarUserReportTypeImport = import.GetWithCheck<SarUserReportTypeADO>(0);
                        if (sarUserReportTypeImport != null && sarUserReportTypeImport.Count > 0)
                        {
                            List<SarUserReportTypeADO> listAfterRemove = new List<SarUserReportTypeADO>();


                            foreach (var item in sarUserReportTypeImport)
                            {
                                bool checkNull = string.IsNullOrEmpty(item.REPORT_TYPE_CODE)
                                    && string.IsNullOrEmpty(item.GROUP_CODE)
                                && string.IsNullOrEmpty(item.LOGINNAME);
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
                                this._sarUserReportTypeAdos = new List<SarUserReportTypeADO>();
                                addServiceToProcessList(_CurrentAdos, ref this._sarUserReportTypeAdos);
                                SetDataSource(this._sarUserReportTypeAdos);
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

        private void btnShowLineError_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnShowLineError.Text == "Dòng lỗi")
                {
                    btnShowLineError.Text = "Dòng không lỗi";
                    var errorLine = this._sarUserReportTypeAdos.Where(o => !string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(errorLine);

                }
                else
                {
                    btnShowLineError.Text = "Dòng lỗi";
                    var errorLine = this._sarUserReportTypeAdos.Where(o => string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(errorLine);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                bool success = false;
                WaitingManager.Show();
                List<SAR_USER_REPORT_TYPE> datas = new List<SAR_USER_REPORT_TYPE>();

                if (this._sarUserReportTypeAdos != null && this._sarUserReportTypeAdos.Count > 0)
                {
                    foreach (var item in this._sarUserReportTypeAdos)
                    {
                        SAR_USER_REPORT_TYPE ado = new SAR_USER_REPORT_TYPE();
                        ado.GROUP_CODE = item.GROUP_CODE;
                        //SarReportTypeFilter filter = new SarReportTypeFilter();
                        //filter.REPORT_TYPE_CODE = item.REPORT_TYPE_CODE;
                        //var result =  new BackendAdapter(new CommonParam()).Get<List<SAR_REPORT_TYPE>>("api/SarReportType/Get", ApiConsumers.MosConsumer, filter, null).ToList();
                        var result = BackendDataWorker.Get<SAR_REPORT_TYPE>().FirstOrDefault(p => p.REPORT_TYPE_CODE == item.REPORT_TYPE_CODE);
                        if (result != null)
                            ado.REPORT_TYPE_ID = result.ID;
                        ado.LOGINNAME = item.LOGINNAME;

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
                var dataImports = new BackendAdapter(param).Post<List<SAR_USER_REPORT_TYPE>>("api/SarUserReportType/CreateList", ApiConsumers.MosConsumer, datas, param);
                WaitingManager.Hide();
                if (dataImports != null && dataImports.Count > 0)
                {
                    success = true;
                    btnSave.Enabled = false;
                    LoadDataSarUserReportType();
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

        private void gridViewSarUserReportTypeImport_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    SarUserReportTypeADO pData = (SarUserReportTypeADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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

        private void gridViewSarUserReportTypeImport_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    //BedADO data = (BedADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    string error = (gridViewSarUserReportTypeImport.GetRowCellValue(e.RowHandle, "ERROR") ?? "").ToString();
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

        private void bbtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnSave.Enabled)
                    btnSave_Click(null, null);
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
                var row = (SarUserReportTypeADO)gridViewSarUserReportTypeImport.GetFocusedRow();
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
                var row = (SarUserReportTypeADO)gridViewSarUserReportTypeImport.GetFocusedRow();
                if (row != null)
                {
                    if (this._sarUserReportTypeAdos != null && this._sarUserReportTypeAdos.Count > 0)
                    {
                        this._sarUserReportTypeAdos.Remove(row);
                        var dataCheck = this._sarUserReportTypeAdos.Where(p => p.LOGINNAME == row.LOGINNAME && p.GROUP_CODE == row.GROUP_CODE && p.REPORT_TYPE_CODE == row.REPORT_TYPE_CODE && p.REPORT_TYPE_ID == row.REPORT_TYPE_ID).ToList();
                        if (dataCheck != null && dataCheck.Count == 1)
                        {
                            if (!string.IsNullOrEmpty(dataCheck[0].ERROR))
                            {
                                string erro = string.Format(Message.MessageImport.FileImportDaTonTai, dataCheck[0].REPORT_TYPE_CODE);
                                //string[] Codes = dataCheck[0].ERROR.Split('|');
                                dataCheck[0].ERROR = dataCheck[0].ERROR.Replace(erro, "");
                            }

                        }
                        SetDataSource(this._sarUserReportTypeAdos);
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
