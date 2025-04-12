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
using HIS.Desktop.Plugins.HisMachineImport.ADO;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
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

namespace HIS.Desktop.Plugins.HisMachineImport.HisMachineImport
{
    public partial class frmHisMachineImport : FormBase
    {
        #region Declare
        Inventec.Desktop.Common.Modules.Module _Module { get; set; }
        List<MachineImportADO> _machineAdos;
        RefeshReference delegateRefresh;
        List<MachineImportADO> _CurrentAdos;
        int checkButtonErrorLine = 0;
        bool isFirstTime = true;
        CommonParam paramDB = new CommonParam();
        HisMachineFilter filterDB = new HisMachineFilter();
        List<HIS_MACHINE> _ListHisMachine { get; set; }
        List<HIS_MACHINE> rsDB = new List<HIS_MACHINE>();
        #endregion

        #region Construct
        public frmHisMachineImport()
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
        public frmHisMachineImport(Inventec.Desktop.Common.Modules.Module moduleData)
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
        #endregion

        private void frmHisMachineImport_Load(object sender, EventArgs e)
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
        #region Action

        private void btnDownLoadFile_Click(object sender, EventArgs e)
        {
            try
            {
                string fileName = System.IO.Path.Combine(Application.StartupPath + "\\Tmp\\Imp\\", "IMPORT_MACHINE_CLS.xlsx");
                Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
                param.Messages = new List<string>();
                if (File.Exists(fileName))
                {
                    saveFileDialog.Title = "Save File";
                    saveFileDialog.FileName = "IMPORT_MACHINE_CLS";
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
                        var hisMachineImport = import.GetWithCheck<MachineImportADO>(0);
                        if (hisMachineImport != null && hisMachineImport.Count > 0)
                        {
                            List<MachineImportADO> listAfterRemove = new List<MachineImportADO>();


                            foreach (var item in hisMachineImport)
                            {
                                //bool checkNull =
                                // string.IsNullOrEmpty(item.MACHINE_CODE) 
                                // && string.IsNullOrEmpty(item.MACHINE_NAME) && string.IsNullOrEmpty(item.MACHINE_NAME) 
                                // && string.IsNullOrEmpty(item.MACHINE_GROUP_CODE) && string.IsNullOrEmpty(item.MACHINE_GROUP_CODE) 
                                // && string.IsNullOrEmpty(item.ROOM_IDS) && string.IsNullOrEmpty(item.ROOM_IDS) 
                                // && string.IsNullOrEmpty(item.INTEGRATE_ADDRESS) && string.IsNullOrEmpty(item.INTEGRATE_ADDRESS) 
                                // && string.IsNullOrEmpty(item.SERIAL_NUMBER) && string.IsNullOrEmpty(item.SERIAL_NUMBER) 
                                // && string.IsNullOrEmpty(item.SOURCE_CODE) && string.IsNullOrEmpty(item.SOURCE_CODE)
                                // //Dangth
                                // && string.IsNullOrEmpty(item.SYMBOL) && string.IsNullOrEmpty(item.SYMBOL)
                                // && string.IsNullOrEmpty(item.MANUFACTURER_NAME) && string.IsNullOrEmpty(item.MANUFACTURER_NAME)
                                // && string.IsNullOrEmpty(item.NATIONAL_NAME) && string.IsNullOrEmpty(item.NATIONAL_NAME)
                                // && (!item.MANUFACTURED_YEAR.HasValue) && (!item.MANUFACTURED_YEAR.HasValue)
                                // && (!item.USED_YEAR.HasValue) && (!item.USED_YEAR.HasValue)
                                // && string.IsNullOrEmpty(item.CIRCULATION_NUMBER) && string.IsNullOrEmpty(item.CIRCULATION_NUMBER)
                                // && (!item.MAX_SERVICE_PER_DAY.HasValue);
                                //if (!checkNull)
                                //{
                                //    listAfterRemove.Add(item);
                                //}
                                bool checkNull =
        string.IsNullOrEmpty(item.MACHINE_CODE)
        && string.IsNullOrEmpty(item.MACHINE_NAME)
        && string.IsNullOrEmpty(item.SERIAL_NUMBER)
        && string.IsNullOrEmpty(item.SOURCE_CODE)
        && string.IsNullOrEmpty(item.ROOM_IDS)
        && string.IsNullOrEmpty(item.INTEGRATE_ADDRESS)
        && string.IsNullOrEmpty(item.SYMBOL)
        && string.IsNullOrEmpty(item.MACHINE_GROUP_CODE)
        && string.IsNullOrEmpty(item.MANUFACTURER_NAME)
        && string.IsNullOrEmpty(item.NATIONAL_NAME)
        && string.IsNullOrEmpty(item.CIRCULATION_NUMBER)
        && !item.MANUFACTURED_YEAR.HasValue
        && !item.USED_YEAR.HasValue
        && !item.MAX_SERVICE_PER_DAY.HasValue;

                                if (!checkNull)
                                {
                                    listAfterRemove.Add(item);
                                }
                            }


                            this._CurrentAdos = listAfterRemove;

                            if (this._CurrentAdos != null && this._CurrentAdos.Count > 0)
                            {
                                btnShowLineError.Enabled = true;
                                this._machineAdos = new List<MachineImportADO>();
                                rsDB = new BackendAdapter(paramDB).Get<List<HIS_MACHINE>>("api/HisMachine/Get", ApiConsumers.MosConsumer, filterDB, paramDB).ToList();
                                addServiceToProcessList(_CurrentAdos, ref this._machineAdos);
                                isFirstTime = false;
                                SetDataSource(this._machineAdos);
                                CheckErrorLine();
                            }
                            WaitingManager.Hide();
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

        private void addServiceToProcessList(List<MachineImportADO> _CurrentAdosAdd, ref List<MachineImportADO> _machineAdos)
        {
            try
            {
                _machineAdos = new List<MachineImportADO>();
                long i = 0;

                foreach (var item in _CurrentAdosAdd)
                {
                    i++;
                    string error = "";
                    var machineAdo = new MachineImportADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<MachineImportADO>(machineAdo, item);
                    bool checkTrungDB = false;
                    bool checkTrungTrongFile = false;
                    List<HIS_MACHINE> lstMachineCheck = new List<HIS_MACHINE>();
                    lstMachineCheck = null;
                    // check trung trong file import
                    if (!string.IsNullOrEmpty(item.MACHINE_CODE) && item.MACHINE_CODE != "")
                    {
                        var count = _CurrentAdosAdd.Where(o => o.MACHINE_CODE == item.MACHINE_CODE && 
                                                            o.MACHINE_NAME == item.MACHINE_NAME && 
                                                            o.SERIAL_NUMBER == item.SERIAL_NUMBER && 
                                                            o.SOURCE_CODE == item.SOURCE_CODE && 
                                                            o.ROOM_IDS == item.ROOM_IDS && 
                                                            o.INTEGRATE_ADDRESS == item.INTEGRATE_ADDRESS && 
                                                            o.MAX_SERVICE_PER_DAY == item.MAX_SERVICE_PER_DAY && 
                                                            o.MACHINE_GROUP_CODE == item.MACHINE_GROUP_CODE &&
                                                            o.SYMBOL == item.SYMBOL &&
                                                            o.MANUFACTURER_NAME == item.MANUFACTURER_NAME &&
                                                            o.NATIONAL_NAME == item.NATIONAL_NAME &&
                                                            o.MANUFACTURED_YEAR == item.MANUFACTURED_YEAR &&
                                                            o.USED_YEAR == item.USED_YEAR &&
                                                            o.CIRCULATION_NUMBER == item.CIRCULATION_NUMBER).GroupBy(o => o.MACHINE_CODE).Select(o => o.FirstOrDefault()).ToList();
                        if (count.Count > 1)
                            checkTrungTrongFile = true;
                    }
                    if (checkTrungTrongFile)
                    {
                        error += string.Format(Message.MessageImport.TonTaiTrungNhauTrongFileImport);
                    }
                    // check neu du lieu trong file import da ton tai trong DB
                    {
                        if (!string.IsNullOrEmpty(item.MACHINE_CODE) && item.MACHINE_CODE != "")
                        {
                            //List<HIS_MACHINE> rs = rsDB.Where(o => o.MACHINE_CODE == item.MACHINE_CODE && o.MACHINE_NAME == item.MACHINE_NAME && o.SERIAL_NUMBER == item.SERIAL_NUMBER && o.SOURCE_CODE == item.SOURCE_CODE && o.ROOM_IDS == item.ROOM_IDS && o.INTEGRATE_ADDRESS == item.INTEGRATE_ADDRESS && o.MAX_SERVICE_PER_DAY == item.MAX_SERVICE_PER_DAY && o.MACHINE_GROUP_CODE == item.MACHINE_GROUP_CODE).ToList();
                            List<HIS_MACHINE> rs = rsDB.Where(o => o.MACHINE_CODE == item.MACHINE_CODE ).ToList();
                            // chỉ so sánh UK của 1 bảng thôi
                            if (rs != null && rs.Count > 0)
                                checkTrungDB = true;
                        }
                        if (checkTrungDB)
                        {
                            if (error != "") error += " | ";
                            error += string.Format(Message.MessageImport.DBDaTonTai, item.MACHINE_CODE);
                        }
                    }
                    // check tinh phu hop cua cac du lieu truyen vao
                    if (!string.IsNullOrEmpty(item.MACHINE_CODE) || item.MACHINE_CODE != "")
                    {
                        if (item.MACHINE_CODE.Length > 100)
                        {
                            if (error != "") error += " | ";
                            error += string.Format(Message.MessageImport.Maxlength, "Mã máy CLS", 100);
                        }
                        lstMachineCheck = BackendDataWorker.Get<HIS_MACHINE>().Where(p => p.MACHINE_CODE == item.MACHINE_CODE).ToList();
                        if (lstMachineCheck != null && lstMachineCheck.Count > 0)
                        {
                            var itemCurrent = lstMachineCheck.FirstOrDefault();
                            if (itemCurrent != null)
                                if (itemCurrent.IS_ACTIVE == 1)
                                {
                                    // machineAdo.ID = itemCurrent.ID;
                                }
                                else
                                {
                                    // machineAdo.ID = itemCurrent.ID;
                                    if (error != "") error += " | ";
                                    error += string.Format(Message.MessageImport.MaLoaiMayCLSDaKhoa);
                                }
                        }
                        //else
                        //{
                        //    if (error != "") error += " | ";
                        //    error += string.Format(Message.MessageImport.KhongHopLe, "Mã máy CLS");
                        //}
                    }
                    else
                    {
                        if (error != "") error += " | ";
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Mã máy CLS");
                    }

                    if (!string.IsNullOrEmpty(item.MACHINE_NAME))
                    {
                        if (item.MACHINE_NAME.Length > 200)
                        {
                            if (error != "") error += " | ";
                            error += string.Format(Message.MessageImport.Maxlength, "Tên máy CLS", 200);
                        }
                    }
                    if (!string.IsNullOrEmpty(item.SERIAL_NUMBER))
                    {
                        if (item.SERIAL_NUMBER.Length > 200)
                        {
                            if (error != "") error += " | ";
                            error += string.Format(Message.MessageImport.Maxlength, "Số serial ", 200);
                        }
                    }
                    if (!string.IsNullOrEmpty(item.SOURCE_CODE))
                    {
                        if (item.SOURCE_CODE.Length > 2)
                        {
                            if (error != "") error += " | ";
                            error += string.Format(Message.MessageImport.Maxlength, "Tên nguồn kinh phí", 2);
                        }
                    }
                    if (!string.IsNullOrEmpty(item.INTEGRATE_ADDRESS))
                    {
                        if (Encoding.UTF8.GetByteCount(item.INTEGRATE_ADDRESS) > 500)
                        {
                            if (error != "") error += " | ";
                            error += string.Format(Message.MessageImport.Maxlength, "Địa chỉ tích hợp", 500);
                        }
                    }
                    if (!string.IsNullOrEmpty(item.MACHINE_GROUP_CODE))
                    {
                        if (item.MACHINE_GROUP_CODE.Length > 10)
                        {
                            if (error != "") error += " | ";
                            error += string.Format(Message.MessageImport.Maxlength, "Mã nhóm máy thực hiện", 10);
                        }
                    }
                    if (!string.IsNullOrEmpty(item.SYMBOL))
                    {
                        if (Encoding.UTF8.GetByteCount(item.SYMBOL) > 500)
                        {
                            if (error != "") error += " | ";
                            error += string.Format(Message.MessageImport.Maxlength, "Ký hiệu", 500);
                        }
                    }
                    if (!string.IsNullOrEmpty(item.MANUFACTURER_NAME))
                    {
                        if (Encoding.UTF8.GetByteCount(item.MANUFACTURER_NAME) > 500)
                        {
                            if (error != "") error += " | ";
                            error += string.Format(Message.MessageImport.Maxlength, "Tên công ty sản xuất", 500);
                        }
                    }
                    if (!string.IsNullOrEmpty(item.NATIONAL_NAME))
                    {
                        if (Encoding.UTF8.GetByteCount(item.NATIONAL_NAME) > 500)
                        {
                            if (error != "") error += " | ";
                            error += string.Format(Message.MessageImport.Maxlength, "Tên nước sản xuất", 500);
                        }
                    }
                    if (item.MANUFACTURED_YEAR.HasValue)
                    {
                        var year = item.MANUFACTURED_YEAR.Value;
                        if (year < 0 || year > 9999)
                        {
                            if (error != "") error += " | ";
                            error += "Năm sản xuất vượt quá 4 ký tự";
                        }
                    }

                    if (item.USED_YEAR.HasValue)
                    {
                        var year = item.USED_YEAR.Value;
                        if (year < 0 || year > 9999)
                        {
                            if (error != "") error += " | ";
                            error += "Năm sử dụng vượt quá 4 ký tự";
                        }
                    }
                    if (!string.IsNullOrEmpty(item.CIRCULATION_NUMBER))
                    {
                        if (item.CIRCULATION_NUMBER.Length > 22)
                        {
                            if (error != "") error += " | ";
                            error += string.Format(Message.MessageImport.Maxlength, "Số lưu hành", 22);
                        }
                    }

                    machineAdo.ERROR = error;
                    machineAdo.ID = i;

                    _machineAdos.Add(machineAdo);
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
                    var errorLine = this._machineAdos.Where(o => !string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(errorLine);
                    CheckErrorLine();
                    checkButtonErrorLine = 1;

                }
                else
                {
                    btnShowLineError.Text = "Dòng lỗi";
                    var errorLine = this._machineAdos.Where(o => string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(errorLine);
                    CheckErrorLine();
                    checkButtonErrorLine = 2;
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
                List<HIS_MACHINE> datas = new List<HIS_MACHINE>();

                if (this._machineAdos != null && this._machineAdos.Count > 0)
                {
                    foreach (var item in this._machineAdos)
                    {
                        HIS_MACHINE ado = new HIS_MACHINE();
                        // ado.GROUP_CODE = item.GROUP_CODE;
                        var result = BackendDataWorker.Get<HIS_MACHINE>().FirstOrDefault(p => p.MACHINE_CODE == item.MACHINE_CODE);
                        if (result != null)
                            ado.ID = result.ID;
                        ado.MACHINE_CODE = item.MACHINE_CODE;
                        ado.MACHINE_NAME = item.MACHINE_NAME;
                        ado.SERIAL_NUMBER = item.SERIAL_NUMBER;
                        ado.SOURCE_CODE = item.SOURCE_CODE;
                        ado.ROOM_IDS = item.ROOM_IDS;
                        ado.INTEGRATE_ADDRESS = item.INTEGRATE_ADDRESS;
                        ado.MAX_SERVICE_PER_DAY = item.MAX_SERVICE_PER_DAY;
                        ado.MACHINE_GROUP_CODE = item.MACHINE_GROUP_CODE;
                        ado.SYMBOL = item.SYMBOL;
                        ado.MANUFACTURER_NAME = item.MANUFACTURER_NAME;
                        ado.NATIONAL_NAME = item.NATIONAL_NAME;
                        ado.MANUFACTURED_YEAR = item.MANUFACTURED_YEAR;
                        ado.USED_YEAR = item.USED_YEAR;
                        ado.CIRCULATION_NUMBER = item.CIRCULATION_NUMBER;
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
                var dataImports = new BackendAdapter(param).Post<List<HIS_MACHINE>>("api/HisMachine/CreateList", ApiConsumers.MosConsumer, datas, param);
                WaitingManager.Hide();
                if (dataImports != null && dataImports.Count > 0)
                {
                    success = true;
                    btnSave.Enabled = false;
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


        private void btnDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (MachineImportADO)gridViewHisMachineImport.GetFocusedRow();
                if (row != null)
                {
                    if (this._machineAdos != null && this._machineAdos.Count > 0)
                    {
                        this._machineAdos.Remove(row);
                        List<MachineImportADO> rs = this._machineAdos;
                        addServiceToProcessList(rs, ref this._machineAdos);
                        switch (checkButtonErrorLine)
                        {
                            case 0:
                                {
                                    SetDataSource(this._machineAdos);
                                    CheckErrorLine();
                                    break;
                                }
                            case 1:
                                {
                                    SetDataSource(this._machineAdos.Where(o => !string.IsNullOrEmpty(o.ERROR)).ToList());
                                    CheckErrorLine();
                                    break;
                                }
                            case 2:
                                {
                                    SetDataSource(this._machineAdos.Where(o => string.IsNullOrEmpty(o.ERROR)).ToList()); CheckErrorLine();
                                    break;
                                }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CheckErrorLine()
        {
            try
            {
                var checkError = this._machineAdos.Exists(o => !string.IsNullOrEmpty(o.ERROR));
                if (!checkError)
                {
                    btnSave.Enabled = true;
                    btnShowLineError.Enabled = false;
                    SetDataSource(this._machineAdos);
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

        private void SetDataSource(List<MachineImportADO> dataSource)
        {
            try
            {
                gridControlHisMachineImport.BeginUpdate();
                gridControlHisMachineImport.DataSource = null;
                gridControlHisMachineImport.DataSource = dataSource;
                gridControlHisMachineImport.EndUpdate();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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

        private void gridViewHisMachineImport_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    //BedADO data = (BedADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    string error = (gridViewHisMachineImport.GetRowCellValue(e.RowHandle, "ERROR") ?? "").ToString();
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

        private void gridViewHisMachineImport_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MachineImportADO pData = (MachineImportADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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
        #endregion

        private void repositoryItemButton_ER_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (MachineImportADO)gridViewHisMachineImport.GetFocusedRow();
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
    }
}
