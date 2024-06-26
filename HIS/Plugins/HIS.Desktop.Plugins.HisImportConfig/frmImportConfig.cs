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
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.HisImportConfig.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System.Windows.Forms;
using System.IO;
using System.ComponentModel;
using Inventec.Common.Logging;
using System.Collections;

namespace HIS.Desktop.Plugins.HisImportConfig
{
    public partial class frmImportConfig : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module _Module { get; set; }
        RefeshReference delegateRefresh;
        List<HisConfigADO> HisConfigAdos;
        List<HisConfigADO> CurrentAdos;
        List<HIS_CONFIG> ListConFig { get; set; }


        public frmImportConfig()
        {
            InitializeComponent();
        }
        public frmImportConfig(Inventec.Desktop.Common.Modules.Module _module)
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
        public frmImportConfig(Inventec.Desktop.Common.Modules.Module _module, RefeshReference _delegateRefresh)
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

        private void gridControl1_Click(object sender, EventArgs e)
        {

        }

        private void frmImportConfig_Load(object sender, EventArgs e)
        {
            try
            {
                if (this._Module != null)
                {
                    this.Text = this._Module.text;
                }
                SetIcon();
                LoadCurrentDataConfig();
            }
            catch (Exception ex)
            {

                LogSession.Error(ex);
            }
        }
        private void LoadCurrentDataConfig()
        {

            try
            {


                ListConFig = new List<HIS_CONFIG>();
                MOS.Filter.HisConfigFilter filter = new MOS.Filter.HisConfigFilter();
                ListConFig = new BackendAdapter(new CommonParam()).Get<List<HIS_CONFIG>>("api/HisConfig/Get", ApiConsumers.MosConsumer, filter, null);

            }
            catch (Exception ex)
            {

                LogSession.Error(ex);
            }
        }

        private void btnDownLoadFile_Click(object sender, EventArgs e)
        {
            try
            {
                string fileName = System.IO.Path.Combine(Application.StartupPath + "\\Tmp\\Imp\\", "IMPORT_HIS_Config.xlsx");
                CommonParam param = new CommonParam();
                param.Messages = new List<string>();
                if (System.IO.File.Exists(fileName))
                {
                    saveFileDialog1.Title = "Save File";
                    saveFileDialog1.FileName = "IMPORT_HIS_Config";
                    saveFileDialog1.DefaultExt = "xlsx";
                    saveFileDialog1.Filter = "Excel files (*.xlsx)|All files (*.*)";
                    saveFileDialog1.FilterIndex = 2;
                    saveFileDialog1.RestoreDirectory = true;

                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        System.IO.File.Copy(fileName, saveFileDialog1.FileName);
                        MessageManager.Show(this.ParentForm, param, true);
                        if (DevExpress.XtraEditors.XtraMessageBox.Show("Bạn có muốn mở file ngay ?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            System.Diagnostics.Process.Start(saveFileDialog1.FileName);
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
                        var hisServiceImport = import.GetWithCheck<HisConfigADO>(0);
                        if (hisServiceImport != null && hisServiceImport.Count > 0)
                        {
                            List<HisConfigADO> listAfterRemove = new List<HisConfigADO>();
                            foreach (var item in hisServiceImport)
                            {
                                bool checkNull = string.IsNullOrEmpty(item.CONFIG_CODE) && string.IsNullOrEmpty(item.KEY) && string.IsNullOrEmpty(item.BRANCH_CODE) && string.IsNullOrEmpty(item.VALUE) && string.IsNullOrEmpty(item.DEFAULT_VALUE);
                                if (!checkNull)
                                {
                                    listAfterRemove.Add(item);
                                }
                            }
                            WaitingManager.Hide();
                            this.CurrentAdos = listAfterRemove;
                            if (this.CurrentAdos != null && this.CurrentAdos.Count > 0)
                            {
                                btnShowLineError.Enabled = true;
                                this.HisConfigAdos = new List<HisConfigADO>();
                                addServiceToProcessList(CurrentAdos, ref this.HisConfigAdos);
                                SetDataSource(this.HisConfigAdos);

                            }
                        }
                        else
                        {
                            WaitingManager.Hide();
                            DevExpress.XtraEditors.XtraMessageBox.Show(" Import thất bại");

                        }
                    }
                    else
                    {

                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show("Không dọc được file");

                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);

            }
        }
        private void addServiceToProcessList(List<HisConfigADO> service, ref List<HisConfigADO> hisConfigRef)
        {
            try
            {
                hisConfigRef = new List<HisConfigADO>();
                long i = 0;
                foreach (var item in service)
                {
                    i++;
                    string error = "";
                    var serAdo = new HisConfigADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HisConfigADO>(serAdo, item);
                    if (!string.IsNullOrEmpty(item.KEY))
                    {
                        if (item.KEY.Length > 100)
                        {
                            error += string.Format(Message.MessageImport.MaxLength, " Key cấu hình");
                        }
                        else
                        {
                            //var key = this.ListConFig.FirstOrDefault(p=>p.KEY ==  item.KEY && p.BRANCH_ID == item.BRANCH_ID);
                            //if (key != null && key.ID >0)
                            //{
                            //    item.ID = key.ID;
                            //}
                            var key = this.ListConFig.FirstOrDefault(p => p.KEY == item.KEY);
                            if (key == null)
                            {
                                error += string.Format(Message.MessageImport.KhongTonTai, "Key cấu hình");
                            }
                        }
                    }
                    else error += string.Format(Message.MessageImport.ThieuTruongDl, "Key cấu hình");

                    //Error CONFIG_CODE
                    if (!string.IsNullOrEmpty(item.CONFIG_CODE))
                    {
                        if (item.CONFIG_CODE.Length > 20)
                        {
                            error += string.Format(Message.MessageImport.MaxLength, " Mã cấu hình");
                        }
                        else
                        {
                            var code = this.ListConFig.FirstOrDefault(p => p.CONFIG_CODE == item.CONFIG_CODE);
                            var key = this.ListConFig.FirstOrDefault(p => p.KEY == item.KEY);
                            if (code == null)
                            {
                                if (key != null)
                                {
                                    error += string.Format(Message.MessageImport.KhongTonTai, "Mã cấu hình");
                                }
                            }
                            else
                            {
                                if(key != null && key.CONFIG_CODE != item.CONFIG_CODE)
                                {
                                    error += string.Format(Message.MessageImport.KhongHopLe, "Mã cấu hình");
                                }    
                                    
                            }
                        }
                    }
                    else error += string.Format(Message.MessageImport.ThieuTruongDl, "Mã cấu hình");

                    // Error value
                    //if (!string.IsNullOrEmpty(item.VALUE))
                    //{
                    //    if (Encoding.UTF8.GetByteCount(item.VALUE.Trim()) > 4000 )
                    //    {
                    //        error += string.Format(Message.MessageImport.MaxLength, "Giá trị");
                    //    }
                    //}
                    //// Error value_default
                    //if (!string.IsNullOrEmpty(item.DEFAULT_VALUE))
                    //{
                    //    if (Encoding.UTF8.GetByteCount(item.DEFAULT_VALUE.Trim()) > 4000)
                    //    {
                    //        error += string.Format(Message.MessageImport.MaxLength, "Giá trị mặc định");

                    //    }
                    //}
                    //// Error Mô tả
                    //if (!string.IsNullOrEmpty(item.DESCRIPTION))
                    //{
                    //    if (Encoding.UTF8.GetByteCount(item.DESCRIPTION.Trim()) > 4000)
                    //    {
                    //        error += string.Format(Message.MessageImport.MaxLength, "Mô tả");

                    //    }
                    //}
                    // Error Nhóm cấu hình
                    if (!string.IsNullOrEmpty(item.CONFIG_GROUP_CODES))
                    {
                        if (Encoding.UTF8.GetByteCount(item.CONFIG_GROUP_CODES.Trim()) > 6)
                        {
                            error += string.Format(Message.MessageImport.MaxLength, "Nhóm cấu hình");
                        }
                        var configGroupGet = BackendDataWorker.Get<HIS_CONFIG_GROUP>().FirstOrDefault(o => o.CONFIG_GROUP_CODE == item.CONFIG_GROUP_CODES);
                        if (configGroupGet != null)
                        {
                            serAdo.CONFIG_GROUP_CODES = configGroupGet.CONFIG_GROUP_CODE;
                            serAdo.CONFIG_GROUP_NAMES = configGroupGet.CONFIG_GROUP_NAME;

                        }

                        if (!CheckConfigGroupCode(item.CONFIG_GROUP_CODES.Trim()))
                        {
                            error += string.Format(Message.MessageImport.MaNhomCauHinhKhongTonTai, "Nhóm cấu hình");
                        }
                    }
                    // Error Chi nhánh

                    if (!string.IsNullOrEmpty(item.BRANCH_CODE))
                    {
                        if (Encoding.UTF8.GetByteCount(item.BRANCH_CODE.Trim()) > 6)
                        {
                            error += string.Format(Message.MessageImport.MaxLength, "Chi nhánh");
                        }
                        var branchGet = BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.BRANCH_CODE == item.BRANCH_CODE);
                        if (branchGet != null)
                        {
                            serAdo.BRANCH_ID = branchGet.ID;
                            serAdo.BRANCH_NAME = branchGet.BRANCH_NAME;

                        }

                        if (!CheckBranchCode(item.BRANCH_CODE.Trim()))
                        {
                            error += string.Format(Message.MessageImport.MaChiNhanhKhongTonTai, "Chi nhánh");
                        }
                    }





                    serAdo.ERROR = error;
                    serAdo.ID = i;
                    hisConfigRef.Add(serAdo);
                }
            }

            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private bool CheckConfigGroupCode(string Text)
        {
            bool success = false;
            try
            {
                var data = BackendDataWorker.Get<HIS_CONFIG_GROUP>();
                foreach (var item in data)
                {
                    if (item.CONFIG_GROUP_CODE == Text)
                    {
                        success = true;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
            return success;
        }
        private void SetDataSource(List<HisConfigADO> dataSource)
        {
            gridControl1.BeginUpdate();
            gridControl1.DataSource = null;
            gridControl1.DataSource = dataSource;
            gridControl1.EndUpdate();
            checkErrorLine(null);
        }
        private void checkErrorLine(List<HisConfigADO> dataSource)
        {
            try
            {
                var checkerror = this.HisConfigAdos.Exists(o => !string.IsNullOrEmpty(o.ERROR));
                if (!checkerror)
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
                    var errorLine = this.HisConfigAdos.Where(o => !string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(errorLine);

                }
                else
                {
                    btnShowLineError.Text = "Dòng lỗi";
                    var errorLine = this.HisConfigAdos.Where(o => string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(errorLine);
                }

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void Config()
        {
            foreach (var item in this.HisConfigAdos)
            {
                var data = BackendDataWorker.Get<HIS_CONFIG>().FirstOrDefault(o => o.KEY == item.KEY);
                item.ID = data.ID;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            try
            {
                bool success = false;
                WaitingManager.Show();
                Inventec.Common.Logging.LogSystem.Error("HIS_CONFIG---------------------------1");
                // Config();
                List<HIS_CONFIG> datas = new List<HIS_CONFIG>();

                if (this.HisConfigAdos != null && this.HisConfigAdos.Count > 0)
                {
                    foreach (var item in this.HisConfigAdos)
                    {
                        HIS_CONFIG ado = new HIS_CONFIG();
                        var key = this.ListConFig.FirstOrDefault(p => p.KEY == item.KEY);
                        if (key != null && key.ID > 0)
                        {
                            ado.ID = key.ID;
                            ado.CONFIG_CODE = item.CONFIG_CODE;
                            ado.KEY = item.KEY;
                            ado.VALUE = item.VALUE;
                            ado.DEFAULT_VALUE = item.DEFAULT_VALUE;
                            ado.DESCRIPTION = item.DESCRIPTION;
                            ado.CONFIG_GROUP_CODES = item.CONFIG_GROUP_CODES;
                            ado.BRANCH_ID = item.BRANCH_ID;
                            datas.Add(ado);
                        }
                    }
                }
                else
                {
                    WaitingManager.Hide();
                    DevExpress.XtraEditors.XtraMessageBox.Show("Dữ liệu rỗng", "Thông báo");
                    return;
                }

                CommonParam param = new CommonParam();


                var dataImports = new BackendAdapter(param).Post<List<HIS_CONFIG>>("api/HisConfig/UpdateList", ApiConsumers.MosConsumer, datas, param);
                WaitingManager.Hide();
                if (dataImports != null && dataImports.Count > 0)
                {
                    success = true;
                    btnSave.Enabled = false;
                    LoadCurrentDataConfig();
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
        private bool CheckBranchCode(String Text)
        {
            bool success = false;
            try
            {
                var data = BackendDataWorker.Get<HIS_BRANCH>();
                foreach (var item in data)
                {
                    if (item.BRANCH_CODE == Text)
                    {
                        success = true;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
            return success;
        }
        private void gridView1_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    HisConfigADO pData = (HisConfigADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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

        private void gridView1_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    string error = (gridView1.GetRowCellValue(e.RowHandle, "ERROR") ?? "").ToString();
                    if (e.Column.FieldName == "Error")
                    {
                        if (!string.IsNullOrEmpty(error))
                        {
                            e.RepositoryItem = btnERROR;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnERROR_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (HisConfigADO)gridView1.GetFocusedRow();
                if (row != null && !string.IsNullOrEmpty(row.ERROR))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(row.ERROR, "Thông báo ");
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnDELETE_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (HisConfigADO)gridView1.GetFocusedRow();
                if (row != null)
                {

                    if (this.HisConfigAdos != null && this.HisConfigAdos.Count > 0)
                    {
                        this.HisConfigAdos.Remove(row);
                        var dataCheck = this.HisConfigAdos.Where(p => p.KEY == row.KEY).ToList();
                        if (dataCheck != null && dataCheck.Count == 1)
                        {
                            if (!string.IsNullOrEmpty(dataCheck[0].ERROR))
                            {
                                string errro = string.Format(Message.MessageImport.FileImportDaTonTai, dataCheck[0].KEY);
                                dataCheck[0].ERROR = dataCheck[0].ERROR.Replace(errro, "");

                            }
                        }
                        SetDataSource(this.HisConfigAdos);
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnLuu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave.Focus();
                btnSave_Click(null, null);

            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }


    }
}
