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
using HIS.Desktop.Plugins.HisImportMediRecordBorrow.ADO;
using HIS.Desktop.Plugins.HisImportMediRecordBorrow.Message;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisImportMediRecordBorrow
{
    public partial class frmHisImportMediRecordBorrow : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module _Module { get; set; }
        RefeshReference delegateRefresh;
        List<MediRecordBorrowImportADO> _Rows;
        int _ShowMode = 0;

        private const string APPOINTMENT_FORMAT = "yyyyMMddHHmmss";

        public frmHisImportMediRecordBorrow()
        {
            InitializeComponent();
        }

        public frmHisImportMediRecordBorrow(Inventec.Desktop.Common.Modules.Module _module)
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

        public frmHisImportMediRecordBorrow(Inventec.Desktop.Common.Modules.Module _module, RefeshReference _delegateRefresh)
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

        private void frmHisImportMediRecordBorrow_Load(object sender, EventArgs e)
        {
            try
            {
                if (this._Module != null)
                {
                    this.Text = this._Module.text;
                }
                SetIcon();
                btnImport.Enabled = false;
                btnShowLineError.Enabled = false;
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
                string iconPath = System.IO.Path.Combine(
                    HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath,
                    System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
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
                string fileName = System.IO.Path.Combine(Application.StartupPath + "\\Tmp\\Imp\\", "IMPORT_MEDI_RECORD_BORROW.xlsx");
                CommonParam param = new CommonParam();
                param.Messages = new List<string>();

                if (!File.Exists(fileName))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(
                        "Không tìm thấy file mẫu: " + fileName, "Thông báo");
                    return;
                }

                saveFileDialog.Title = "Save File";
                saveFileDialog.FileName = "IMPORT_MEDI_RECORD_BORROW";
                saveFileDialog.DefaultExt = "xlsx";
                saveFileDialog.Filter = "Excel files (*.xlsx)|All files (*.*)";
                saveFileDialog.FilterIndex = 1;
                saveFileDialog.RestoreDirectory = true;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    File.Copy(fileName, saveFileDialog.FileName, true);
                    MessageManager.Show(this.ParentForm, param, true);
                    if (DevExpress.XtraEditors.XtraMessageBox.Show(
                            "Bạn có muốn mở file ngay?", "Xác nhận",
                            MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(saveFileDialog.FileName);
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
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Multiselect = false;
                ofd.Filter = "Excel files (*.xlsx;*.xls)|*.xlsx;*.xls";
                if (ofd.ShowDialog() != DialogResult.OK) return;

                WaitingManager.Show();
                _ShowMode = 0;
                btnShowLineError.Text = "Dòng lỗi";

                var import = new Inventec.Common.ExcelImport.Import();
                if (!import.ReadFileExcel(ofd.FileName))
                {
                    WaitingManager.Hide();
                    DevExpress.XtraEditors.XtraMessageBox.Show("Không đọc được file");
                    return;
                }

                var rowsRaw = import.GetWithCheck<MediRecordBorrowImportADO>(0);
                if (rowsRaw == null || rowsRaw.Count == 0)
                {
                    WaitingManager.Hide();
                    DevExpress.XtraEditors.XtraMessageBox.Show("Không có dữ liệu trong file");
                    ClearGrid();
                    return;
                }

                var rowsClean = rowsRaw
                    .Where(o => !(string.IsNullOrEmpty(o.MEDI_RECORD_STORE_CODE)
                                  && string.IsNullOrEmpty(o.DEPARTMENT_CODE)
                                  && string.IsNullOrEmpty(o.BORROW_LOGINNAME)
                                  && string.IsNullOrEmpty(o.APPOINTMENT_TIME_STR)))
                    .ToList();

                _Rows = new List<MediRecordBorrowImportADO>();
                long stt = 0;
                foreach (var r in rowsClean)
                {
                    stt++;
                    r.STT = stt;
                    r.ERROR = "";
                    _Rows.Add(r);
                }

                LookupMediRecords(_Rows);
                LookupDepartments(_Rows);
                LookupBorrowers(_Rows);
                ValidateRows(_Rows);

                SetDataSource(_Rows);
                UpdateButtonState();

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LookupMediRecords(List<MediRecordBorrowImportADO> rows)
        {
            try
            {
                var storeCodes = rows
                    .Where(r => !string.IsNullOrEmpty(r.MEDI_RECORD_STORE_CODE))
                    .Select(r => r.MEDI_RECORD_STORE_CODE.Trim())
                    .Distinct()
                    .ToList();

                if (storeCodes.Count == 0) return;

                foreach (var code in storeCodes)
                {
                    try
                    {
                        HisMediRecordFilter filter = new HisMediRecordFilter();
                        filter.STORE_CODE = code;
                        var list = new BackendAdapter(new CommonParam()).Get<List<HIS_MEDI_RECORD>>(
                            "api/HisMediRecord/Get", ApiConsumers.MosConsumer, filter, null);
                        if (list == null || list.Count == 0) continue;

                        var mr = list.First();
                        foreach (var r in rows.Where(x => !string.IsNullOrEmpty(x.MEDI_RECORD_STORE_CODE)
                                                          && x.MEDI_RECORD_STORE_CODE.Trim() == code))
                        {
                            r.MEDI_RECORD_ID = mr.ID;
                        }
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Warn(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LookupDepartments(List<MediRecordBorrowImportADO> rows)
        {
            try
            {
                var departments = BackendDataWorker.Get<HIS_DEPARTMENT>();
                if (departments == null) return;

                foreach (var r in rows)
                {
                    if (string.IsNullOrEmpty(r.DEPARTMENT_CODE)) continue;
                    var dep = departments.FirstOrDefault(o => o.DEPARTMENT_CODE == r.DEPARTMENT_CODE.Trim());
                    if (dep != null)
                    {
                        r.DEPARTMENT_ID = dep.ID;
                        r.DEPARTMENT_NAME = dep.DEPARTMENT_NAME;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LookupBorrowers(List<MediRecordBorrowImportADO> rows)
        {
            try
            {
                var employees = BackendDataWorker.Get<HIS_EMPLOYEE>();
                if (employees == null) return;

                foreach (var r in rows)
                {
                    if (string.IsNullOrEmpty(r.BORROW_LOGINNAME)) continue;
                    var emp = employees.FirstOrDefault(o => o.LOGINNAME == r.BORROW_LOGINNAME.Trim());
                    if (emp != null)
                    {
                        r.BORROW_USERNAME = emp.TDL_USERNAME;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidateRows(List<MediRecordBorrowImportADO> rows)
        {
            try
            {
                long nowLong = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));

                foreach (var r in rows)
                { 
                    string error = "";

                    if (string.IsNullOrEmpty(r.MEDI_RECORD_STORE_CODE))
                    {
                        error += string.Format(MessageImport.ThieuTruongDL, "mã lưu trữ");
                    }
                    else if (!r.MEDI_RECORD_ID.HasValue)
                    {
                        error += string.Format(MessageImport.KhongTimThayBenhAn, r.MEDI_RECORD_STORE_CODE.Trim());
                    }

                    if (!string.IsNullOrEmpty(r.DEPARTMENT_CODE) && !r.DEPARTMENT_ID.HasValue)
                    {
                        error += string.Format(MessageImport.KhongTimThayKhoa, r.DEPARTMENT_CODE.Trim());
                    }

                    if (string.IsNullOrEmpty(r.BORROW_LOGINNAME))
                    {
                        error += string.Format(MessageImport.ThieuTruongDL, "tài khoản mượn");
                    }
                    else if (string.IsNullOrEmpty(r.BORROW_USERNAME))
                    {
                        error += string.Format(MessageImport.KhongTimThayTaiKhoan, r.BORROW_LOGINNAME.Trim());
                    }

                    if (string.IsNullOrEmpty(r.APPOINTMENT_TIME_STR))
                    {
                        error += string.Format(MessageImport.ThieuTruongDL, "thời gian hẹn trả");
                    }
                    else
                    {
                        DateTime parsed;
                        if (!DateTime.TryParseExact(r.APPOINTMENT_TIME_STR.Trim(), APPOINTMENT_FORMAT,
                                CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
                        {
                            error += MessageImport.SaiDinhDangThoiGian;
                        }
                        else
                        {
                            long apptLong = Convert.ToInt64(parsed.ToString("yyyyMMddHHmmss"));
                            if (apptLong <= nowLong)
                            {
                                error += MessageImport.ThoiGianHenTraKhongHopLe;
                            }
                            else
                            {
                                r.APPOINTMENT_TIME = apptLong;
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(r.MEDI_RECORD_STORE_CODE)
                        && !string.IsNullOrEmpty(r.BORROW_LOGINNAME))
                    {
                        var dup = rows.FirstOrDefault(o =>
                            o.STT < r.STT
                            && !string.IsNullOrEmpty(o.MEDI_RECORD_STORE_CODE)
                            && !string.IsNullOrEmpty(o.BORROW_LOGINNAME)
                            && o.MEDI_RECORD_STORE_CODE.Trim() == r.MEDI_RECORD_STORE_CODE.Trim()
                            && o.BORROW_LOGINNAME.Trim() == r.BORROW_LOGINNAME.Trim());
                        if (dup != null)
                        {
                            error += string.Format(MessageImport.TrungDongTrongFile, dup.STT);
                        }
                    }

                    r.ERROR = error.TrimEnd('|');
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDataSource(List<MediRecordBorrowImportADO> rows)
        {
            try
            {
                gridControlData.BeginUpdate();
                gridControlData.DataSource = null;
                gridControlData.DataSource = rows;
                gridControlData.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ClearGrid()
        {
            try
            {
                _Rows = null;
                gridControlData.BeginUpdate();
                gridControlData.DataSource = null;
                gridControlData.EndUpdate();
                btnImport.Enabled = false;
                btnShowLineError.Enabled = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateButtonState()
        {
            try
            {
                bool hasData = _Rows != null && _Rows.Count > 0;
                bool hasError = hasData && _Rows.Any(o => !string.IsNullOrEmpty(o.ERROR));
                btnImport.Enabled = hasData && !hasError;
                btnShowLineError.Enabled = hasError;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ApplyDataSourceForMode()
        {
            if (_Rows == null) return;
            switch (_ShowMode)
            {
                case 1:
                    SetDataSource(_Rows.Where(o => !string.IsNullOrEmpty(o.ERROR)).ToList());
                    break;
                case 2:
                    SetDataSource(_Rows.Where(o => string.IsNullOrEmpty(o.ERROR)).ToList());
                    break;
                default:
                    SetDataSource(_Rows);
                    break;
            }
        }

        private void btnShowLineError_Click(object sender, EventArgs e)
        {
            try
            {
                if (_Rows == null) return;
                if (btnShowLineError.Text == "Dòng lỗi")
                {
                    btnShowLineError.Text = "Tất cả";
                    _ShowMode = 1;
                    SetDataSource(_Rows.Where(o => !string.IsNullOrEmpty(o.ERROR)).ToList());
                }
                else
                {
                    btnShowLineError.Text = "Dòng lỗi";
                    _ShowMode = 0;
                    SetDataSource(_Rows);
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
                var row = (MediRecordBorrowImportADO)gridViewData.GetFocusedRow();
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
                var row = (MediRecordBorrowImportADO)gridViewData.GetFocusedRow();
                if (row == null || _Rows == null) return;

                _Rows.Remove(row);
                long stt = 0;
                foreach (var r in _Rows)
                {
                    stt++;
                    r.STT = stt;
                }
                ValidateRows(_Rows);
                ApplyDataSourceForMode();
                UpdateButtonState();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewData_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    if (e.Column.FieldName == "STT_")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewData_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "BORROW_USERNAME" && e.ListSourceRowIndex >= 0)
                {
                    var list = gridControlData.DataSource as IList<MediRecordBorrowImportADO>;
                    if (list != null && e.ListSourceRowIndex < list.Count)
                    {
                        var row = list[e.ListSourceRowIndex];
                        if (!string.IsNullOrEmpty(row.BORROW_LOGINNAME) && !string.IsNullOrEmpty(row.BORROW_USERNAME))
                        {
                            e.DisplayText = row.BORROW_LOGINNAME.Trim() + " - " + row.BORROW_USERNAME;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewData_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle < 0) return;
                if (e.Column.FieldName == "ERROR_")
                {
                    string error = (gridViewData.GetRowCellValue(e.RowHandle, "ERROR") ?? "").ToString();
                    if (!string.IsNullOrEmpty(error))
                    {
                        e.RepositoryItem = repositoryItemButton_ER;
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
                if (_Rows == null || _Rows.Count == 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Dữ liệu rỗng", "Thông báo");
                    return;
                }
                if (_Rows.Any(o => !string.IsNullOrEmpty(o.ERROR)))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Vẫn còn dòng lỗi, vui lòng sửa file Excel và chọn lại", "Thông báo");
                    return;
                }

                WaitingManager.Show();

                var datas = new List<HIS_MEDI_RECORD_BORROW>();
                foreach (var r in _Rows)
                {
                    var entity = new HIS_MEDI_RECORD_BORROW();
                    if (r.MEDI_RECORD_ID.HasValue) entity.MEDI_RECORD_ID = r.MEDI_RECORD_ID.Value;
                    if (r.DEPARTMENT_ID.HasValue) entity.DEPARTMENT_ID = r.DEPARTMENT_ID.Value;
                    entity.BORROW_LOGINNAME = r.BORROW_LOGINNAME != null ? r.BORROW_LOGINNAME.Trim() : null;
                    entity.BORROW_USERNAME = r.BORROW_USERNAME;
                    if (r.APPOINTMENT_TIME.HasValue) entity.APPOINTMENT_TIME = r.APPOINTMENT_TIME.Value;
                    datas.Add(entity);
                }

                CommonParam param = new CommonParam();
                var created = new BackendAdapter(param).Post<List<HIS_MEDI_RECORD_BORROW>>(
                    "api/HisMediRecordBorrow/CreateList", ApiConsumers.MosConsumer, datas, param);

                WaitingManager.Hide();

                bool success = created != null && created.Count > 0;
                if (success)
                {
                    btnImport.Enabled = false;
                    DevExpress.XtraEditors.XtraMessageBox.Show(
                        string.Format("Nhập khẩu thành công {0}/{1} phiếu mượn.", created.Count, datas.Count),
                        "Thông báo");
                    if (this.delegateRefresh != null)
                    {
                        this.delegateRefresh();
                    }
                }

                MessageManager.Show(this.ParentForm, param, success);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnImport.Enabled) btnImport_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
