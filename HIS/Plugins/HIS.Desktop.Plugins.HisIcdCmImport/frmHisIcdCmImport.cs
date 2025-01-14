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
using HIS.Desktop.Plugins.HisIcdCmImport.ADO;
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
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisIcdCmImport
{
    public partial class frmHisIcdCmImport : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module _Module { get; set; }
        RefeshReference delegateRefresh;
        List<IcdCmADO> _IcdCmAdos;
        List<IcdCmADO> _CurrentAdos;
        List<HIS_ICD_CM> _ListIcdCms { get; set; }

        public frmHisIcdCmImport()
        {
            InitializeComponent();
        }

        public frmHisIcdCmImport(Inventec.Desktop.Common.Modules.Module _module)
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

        public frmHisIcdCmImport(Inventec.Desktop.Common.Modules.Module _module, RefeshReference _delegateRefresh)
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
        private void frmHisIcdCmImport_Load(object sender, EventArgs e)
        {
            try
            {
                if (this._Module != null)
                {
                    this.Text = this._Module.text;
                }
                SetIcon();
                LoadDataIcdCm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataIcdCm()
        {
            try
            {
                //_ListIcdCms = new List<HIS_ICD_CM>();
                MOS.Filter.HisIcdCmFilter filter = new MOS.Filter.HisIcdCmFilter();
                this._ListIcdCms = new BackendAdapter(new CommonParam()).Get<List<HIS_ICD_CM>>("api/HisIcdCm/Get", ApiConsumers.MosConsumer, filter, null);
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
                string fileName = System.IO.Path.Combine(Application.StartupPath + "\\Tmp\\Imp\\", "IMPORT_ICD_CM.xlsx");
                Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
                param.Messages = new List<string>();
                if (File.Exists(fileName))
                {
                    saveFileDialog.Title = "Save File";
                    saveFileDialog.FileName = "IMPORT_ICD_CM";
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
                        var hisServiceImport = import.GetWithCheck<IcdCmADO>(0);
                        if (hisServiceImport != null && hisServiceImport.Count > 0)
                        {
                            List<IcdCmADO> listAfterRemove = new List<IcdCmADO>();


                            foreach (var item in hisServiceImport)
                            {
                                bool checkNull = string.IsNullOrEmpty(item.ICD_CM_CODE)
                                    && string.IsNullOrEmpty(item.ICD_CM_NAME)
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
                                this._IcdCmAdos = new List<IcdCmADO>();
                                addServiceToProcessList(_CurrentAdos, ref this._IcdCmAdos);
                                SetDataSource(this._IcdCmAdos);
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

        private void addServiceToProcessList(List<IcdCmADO> _service, ref List<IcdCmADO> _bedRoomRef)
        {
            try
            {
                _bedRoomRef = new List<IcdCmADO>();
                long i = 0;
                foreach (var item in _service)
                {
                    i++;
                    string error = "";
                    var serAdo = new IcdCmADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<IcdCmADO>(serAdo, item);

                    if (!string.IsNullOrEmpty(item.ICD_CM_CODE))
                    {
                        if (item.ICD_CM_CODE.Length > 10)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Mã ICD_CM");
                        }

                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Mã ICD_CM");
                    }

                    var checkTrung12 = _service.Where(p => p.ICD_CM_CODE == item.ICD_CM_CODE).ToList();
                    if (checkTrung12 != null && checkTrung12.Count > 1)
                    {
                        error += string.Format(Message.MessageImport.FileImportDaTonTai, item.ICD_CM_CODE);
                    }
                    if (!string.IsNullOrEmpty(item.ICD_CM_CODE))
                    {
                        if (this._ListIcdCms != null)
                        {
                            var bed = this._ListIcdCms.FirstOrDefault(p => p.ICD_CM_CODE == item.ICD_CM_CODE);
                            if (bed != null)
                            {
                                error += string.Format(Message.MessageImport.DaTonTai, "Mã ICD_CM");
                            }
                        }
                        
                    }


                    if (!string.IsNullOrEmpty(item.ICD_CM_NAME))           
                    {
                        if (Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(item.ICD_CM_NAME, 500))

						{
                            error += string.Format(Message.MessageImport.Maxlength, "Tên ICD_CM");
                        }
                    }
                    else
                    {      
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Tên ICD_CM");
                    }

                    if (!string.IsNullOrEmpty(item.ICD_CM_CHAPTER_CODE))
                    {
                        if (item.ICD_CM_CHAPTER_CODE.Length > 10)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Mã chương");
                        }
                    }
                    if (!string.IsNullOrEmpty(item.ICD_CM_GROUP_CODE))
                    {
                        if (item.ICD_CM_GROUP_CODE.Length > 10)
                        {      
                            error += string.Format(Message.MessageImport.Maxlength, "Mã nhóm");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.ICD_CM_SUB_GROUP_CODE))
                    {
                        if (item.ICD_CM_SUB_GROUP_CODE.Length > 10)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Mã nhóm phụ");
                        }
                    }
                    serAdo.ERROR = error;
                    serAdo.ID = i;

                    _bedRoomRef.Add(serAdo);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void SetDataSource(List<IcdCmADO> dataSource)
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

        private void CheckErrorLine(List<IcdCmADO> dataSource)
        {
            try
            {
                var checkError = this._IcdCmAdos.Exists(o => !string.IsNullOrEmpty(o.ERROR));
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
                    var errorLine = this._IcdCmAdos.Where(o => !string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(errorLine);

                }
                else
                {
                    btnShowLineError.Text = "Dòng lỗi";
                    var errorLine = this._IcdCmAdos.Where(o => string.IsNullOrEmpty(o.ERROR)).ToList();
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
                List<HIS_ICD_CM> datas = new List<HIS_ICD_CM>();

                if (this._IcdCmAdos != null && this._IcdCmAdos.Count > 0)
                {
                    foreach (var item in this._IcdCmAdos)
                    {
                        HIS_ICD_CM ado = new HIS_ICD_CM();
                        ado.ICD_CM_CODE = item.ICD_CM_CODE;
                        ado.ICD_CM_NAME = item.ICD_CM_NAME;
                        ado.ICD_CM_CHAPTER_CODE = item.ICD_CM_CHAPTER_CODE;
                        ado.ICD_CM_CHAPTER_NAME = item.ICD_CM_CHAPTER_NAME;
                        ado.ICD_CM_GROUP_CODE = item.ICD_CM_GROUP_CODE;
                        ado.ICD_CM_GROUP_NAME = item.ICD_CM_GROUP_NAME;
                        ado.ICD_CM_SUB_GROUP_CODE = item.ICD_CM_SUB_GROUP_CODE;
                        ado.ICD_CM_SUB_GROUP_NAME = item.ICD_CM_SUB_GROUP_NAME;
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
                var dataImports = new BackendAdapter(param).Post<List<HIS_ICD_CM>>("api/HisIcdCm/CreateList", ApiConsumers.MosConsumer, datas, param);
                WaitingManager.Hide();
               Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataImports), dataImports));
                if (dataImports != null && dataImports.Count > 0)
                {
                    success = true;
                    btnImport.Enabled = false;
                    LoadDataIcdCm();
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

        private void gridViewData_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    IcdCmADO pData = (IcdCmADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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

        private void repositoryItemButton_ER_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (IcdCmADO)gridViewData.GetFocusedRow();
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
                var row = (IcdCmADO)gridViewData.GetFocusedRow();
                if (row != null)
                {
                    if (this._IcdCmAdos != null && this._IcdCmAdos.Count > 0)
                    {
                        this._IcdCmAdos.Remove(row);
                        var dataCheck = this._IcdCmAdos.Where(p => p.ICD_CM_CODE == row.ICD_CM_CODE).ToList();
                        if (dataCheck != null && dataCheck.Count == 1)
                        {
                            if (!string.IsNullOrEmpty(dataCheck[0].ERROR))
                            {
                                string erro = string.Format(Message.MessageImport.FileImportDaTonTai, dataCheck[0].ICD_CM_CODE);
                                //string[] Codes = dataCheck[0].ERROR.Split('|');
                                dataCheck[0].ERROR = dataCheck[0].ERROR.Replace(erro, "");
                            }

                        }
                        SetDataSource(this._IcdCmAdos);
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
                    btnImport_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
