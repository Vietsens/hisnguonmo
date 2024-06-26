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
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.ImportDepartment.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
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

namespace HIS.Desktop.Plugins.ImportDepartment
{
    public partial class frmImportDepartment : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module _Module { get; set; }
        RefeshReference delegateRefresh;
        List<DepartmentADO> listDepartmentADO { get; set; }

        public frmImportDepartment()
        {
            InitializeComponent();
        }

        public frmImportDepartment(Inventec.Desktop.Common.Modules.Module _module)
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

        public frmImportDepartment(Inventec.Desktop.Common.Modules.Module _module, RefeshReference _delegateRefresh)
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

        private void frmImportDepartment_Load(object sender, EventArgs e)
        {
            try
            {
                SetIcon();
                if (this._Module != null)
                {
                    this.Text = this._Module.text;
                }
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

                string fileName = System.IO.Path.Combine(Application.StartupPath + "\\Tmp\\Imp\\", "IMPORT_DEPARTMENT.xlsx");
                Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
                param.Messages = new List<string>();
                if (File.Exists(fileName))
                {
                    saveFileDialog.Title = "Save File";
                    saveFileDialog.FileName = "IMPORT_DEPARTMENT";
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
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Multiselect = false;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    WaitingManager.Show();
                    var import = new Inventec.Common.ExcelImport.Import();
                    if (import.ReadFileExcel(openFileDialog.FileName))
                    {
                        listDepartmentADO = new List<DepartmentADO>();
                        WaitingManager.Show();
                        var listDepartmentTemp = import.GetWithCheck<DepartmentADO>(0);
                        if (listDepartmentTemp != null && listDepartmentTemp.Count > 0)
                        {
                            listDepartmentADO = this.ProcessFormatData(listDepartmentTemp);
                        }
                        else
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show("Import thất bại");
                        }

                        WaitingManager.Hide();
                        gridControlDepartment.DataSource = listDepartmentADO;
                        btnSave.Enabled = true;
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

        private void gridViewData_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    DepartmentADO department = (DepartmentADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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

                List<HIS_DEPARTMENT> listDepartment = new List<HIS_DEPARTMENT>();
                List<HIS_DEPARTMENT> listDepartmentError = new List<HIS_DEPARTMENT>();
                this.MakeDataDepartment(ref listDepartment, ref listDepartmentError);

                if (listDepartment == null || listDepartment.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy dữ liệu khoa chuẩn!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (listDepartmentError != null && listDepartmentError.Count > 0)
                {
                    DialogResult myResult;
                    myResult = MessageBox.Show("Tồn tại dữ liệu lỗi. Bạn có muốn tiếp tục không?", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                    if (myResult != DialogResult.OK)
                    {
                        return;
                    }
                }

                WaitingManager.Show();
                CommonParam param = new CommonParam();
                var result = new BackendAdapter(param)
                    .Post<List<MOS.EFMODEL.DataModels.HIS_DEPARTMENT>>("api/HisDepartment/CreateList", ApiConsumers.MosConsumer, listDepartment, param);
                WaitingManager.Hide();
                if (result != null && result.Count > 0)
                {
                    success = true;
                    btnSave.Enabled = false;
                    if (delegateRefresh != null)
                    {
                        delegateRefresh();
                        this.Close();
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
                btnSave.Focus();
                if (btnSave.Enabled)
                    btnImport_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboLocDuLieu_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (cboLocDuLieu.EditValue != null && listDepartmentADO != null && listDepartmentADO.Count > 0)
                {
                    int index = cboLocDuLieu.SelectedIndex;
                    List<DepartmentADO> listDepartmentTemp = listDepartmentADO;
                    switch (index)
                    {
                        case 0:
                            listDepartmentTemp = listDepartmentADO;
                            break;
                        case 1:
                            listDepartmentTemp = listDepartmentADO.Where(o => String.IsNullOrEmpty(o.Error)).ToList();
                            break;
                        case 2:
                            listDepartmentTemp = listDepartmentADO.Where(o => !String.IsNullOrEmpty(o.Error)).ToList();
                            break;
                    }

                    gridControlDepartment.DataSource = listDepartmentTemp;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewDepartment_CustomRowColumnError(object sender, Inventec.Desktop.CustomControl.RowColumnErrorEventArgs e)
        {
            try
            {
                if (e.ColumnName == "BRANCH_CODE")
                {
                    var index = this.gridViewDepartment.GetDataSourceRowIndex(e.RowHandle);
                    if (index < 0)
                    {
                        e.Info.ErrorType = ErrorType.None;
                        e.Info.ErrorText = "";
                        return;
                    }

                    var listDatas = this.gridViewDepartment.DataSource as List<DepartmentADO>;
                    var row = listDatas[index];
                    if (!String.IsNullOrEmpty(row.Error))
                    {
                        e.Info.ErrorType = ErrorType.Warning;
                        e.Info.ErrorText = row.Error;
                    }
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
                DepartmentADO department = gridViewDepartment.GetFocusedRow() as DepartmentADO;
                if (department != null)
                {
                    listDepartmentADO.Remove(department);

                    DepartmentADO departmentTemp = listDepartmentADO.FirstOrDefault(o => o.DEPARTMENT_CODE.Trim() == department.DEPARTMENT_CODE.Trim());
                    if (departmentTemp != null)
                    {
                        departmentTemp.Error = departmentTemp.Error.Replace("Mã khoa trong file Import trùng nhau | ", "");
                    }

                    gridControlDepartment.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewDepartment_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewDepartment_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                DepartmentADO department = (DepartmentADO)gridViewDepartment.GetRow(e.RowHandle);
                if (department != null && !String.IsNullOrEmpty(department.Error))
                    e.Appearance.ForeColor = Color.Red;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
