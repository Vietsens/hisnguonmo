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
using HIS.Desktop.Plugins.HisImportBedRoom.ADO;
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

namespace HIS.Desktop.Plugins.HisImportBedRoom
{
    public partial class frmImportBedRoom : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module _Module { get; set; }
        RefeshReference delegateRefresh;
        List<BedRoomADO> _BedRoomAdos;
        List<BedRoomADO> _CurrentAdos;
        List<HIS_BED_ROOM> _ListBedRooms { get; set; }

        public frmImportBedRoom()
        {
            InitializeComponent();
        }

        public frmImportBedRoom(Inventec.Desktop.Common.Modules.Module _module)
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

        public frmImportBedRoom(Inventec.Desktop.Common.Modules.Module _module, RefeshReference _delegateRefresh)
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

        private void frmImportBedRoom_Load(object sender, EventArgs e)
        {
            try
            {
                if (this._Module != null)
                {
                    this.Text = this._Module.text;
                }
                SetIcon();
                LoadDataBeRoom();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataBeRoom()
        {
            try
            {
                _ListBedRooms = new List<HIS_BED_ROOM>();
                MOS.Filter.HisBedRoomFilter filter = new MOS.Filter.HisBedRoomFilter();
                _ListBedRooms = new BackendAdapter(new CommonParam()).Get<List<HIS_BED_ROOM>>("api/HisBedRoom/Get", ApiConsumers.MosConsumer, filter, null);
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

                string fileName = System.IO.Path.Combine(Application.StartupPath + "\\Tmp\\Imp\\", "IMPORT_BED_ROOM.xlsx");
                Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
                param.Messages = new List<string>();
                if (File.Exists(fileName))
                {
                    saveFileDialog.Title = "Save File";
                    saveFileDialog.FileName = "IMPORT_BED_ROOM";
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
                        var hisServiceImport = import.GetWithCheck<BedRoomADO>(0);
                        if (hisServiceImport != null && hisServiceImport.Count > 0)
                        {
                            List<BedRoomADO> listAfterRemove = new List<BedRoomADO>();


                            foreach (var item in hisServiceImport)
                            {
                                bool checkNull = string.IsNullOrEmpty(item.BED_ROOM_CODE)
                                    && string.IsNullOrEmpty(item.BED_ROOM_NAME)
                                    && string.IsNullOrEmpty(item.DEPARTMENT_CODE)
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
                                this._BedRoomAdos = new List<BedRoomADO>();
                                addServiceToProcessList(_CurrentAdos, ref this._BedRoomAdos);
                                SetDataSource(this._BedRoomAdos);
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

        private void addServiceToProcessList(List<BedRoomADO> _service, ref List<BedRoomADO> _bedRoomRef)
        {
            try
            {
                _bedRoomRef = new List<BedRoomADO>();
                long i = 0;
                foreach (var item in _service)
                {
                    i++;
                    string error = "";
                    var serAdo = new BedRoomADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BedRoomADO>(serAdo, item);

                    if (!string.IsNullOrEmpty(item.BED_ROOM_CODE))
                    {
                        if (item.BED_ROOM_CODE.Length > 20)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Mã buồng bệnh");
                        }
                        else
                        {
                            var bedRoom = this._ListBedRooms.FirstOrDefault(p => p.BED_ROOM_CODE == item.BED_ROOM_CODE);
                            if (bedRoom != null)
                            {
                                error += string.Format(Message.MessageImport.DaTonTai, "Mã buồng bệnh");
                            }
                            else
                            {
                                var checkTrung = _service.Where(p => p.BED_ROOM_CODE == item.BED_ROOM_CODE).ToList();
                                if (checkTrung != null && checkTrung.Count > 1)
                                {
                                    error += string.Format(Message.MessageImport.FileImportDaTonTai, item.BED_ROOM_CODE);
                                }
                            }
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Mã buồng bệnh");
                    }
                    if (!string.IsNullOrEmpty(item.BED_ROOM_NAME))
                    {
                        if (item.BED_ROOM_NAME.Length > 100)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Tên buồng bệnh");
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Tên buồng bệnh");
                    }
                    if (!string.IsNullOrEmpty(item.DEPARTMENT_CODE))
                    {
                        if (item.DEPARTMENT_CODE.Length > 10)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Khoa");
                        }
                        else
                        {
                            var department = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(p => p.DEPARTMENT_CODE == item.DEPARTMENT_CODE.Trim());
                            if (department != null)
                            {
                                if (department.IS_ACTIVE == 1)
                                {
                                    serAdo.DEPARTMENT_ID = department.ID;
                                    serAdo.DEPARTMENT_NAME = department.DEPARTMENT_NAME;
                                }
                                else
                                {
                                    error += string.Format(Message.MessageImport.MaKhoaDaKhoa, "Khoa");
                                }
                            }
                            else
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "Khoa");
                            }
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Khoa");
                    }
                    if (!string.IsNullOrEmpty(item.IS_SURGERY_STR))
                    {
                        if (item.IS_SURGERY_STR == "x")
                        {
                            serAdo.IS_SURGERY_DISPLAY = true;
                            serAdo.IS_SURGERY = (short)1;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Buồng mổ");
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

        private void SetDataSource(List<BedRoomADO> dataSource)
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

        private void CheckErrorLine(List<BedRoomADO> dataSource)
        {
            try
            {
                var checkError = this._BedRoomAdos.Exists(o => !string.IsNullOrEmpty(o.ERROR));
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
                    var errorLine = this._BedRoomAdos.Where(o => !string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(errorLine);

                }
                else
                {
                    btnShowLineError.Text = "Dòng lỗi";
                    var errorLine = this._BedRoomAdos.Where(o => string.IsNullOrEmpty(o.ERROR)).ToList();
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
                var row = (BedRoomADO)gridViewData.GetFocusedRow();
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
                var row = (BedRoomADO)gridViewData.GetFocusedRow();
                if (row != null)
                {
                    if (this._BedRoomAdos != null && this._BedRoomAdos.Count > 0)
                    {
                        this._BedRoomAdos.Remove(row);
                        var dataCheck = this._BedRoomAdos.Where(p => p.BED_ROOM_CODE == row.BED_ROOM_CODE).ToList();
                        if (dataCheck != null && dataCheck.Count == 1)
                        {
                            if (!string.IsNullOrEmpty(dataCheck[0].ERROR))
                            {
                                string erro = string.Format(Message.MessageImport.FileImportDaTonTai, dataCheck[0].BED_ROOM_CODE);
                                //string[] Codes = dataCheck[0].ERROR.Split('|');
                                dataCheck[0].ERROR = dataCheck[0].ERROR.Replace(erro, "");
                            }

                        }
                        SetDataSource(this._BedRoomAdos);
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
                    BedRoomADO pData = (BedRoomADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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
                List<HisBedRoomSDO> datas = new List<HisBedRoomSDO>();

                if (this._BedRoomAdos != null && this._BedRoomAdos.Count > 0)
                {
                    foreach (var item in this._BedRoomAdos)
                    {
                        HisBedRoomSDO ado = new HisBedRoomSDO();
                        HIS_BED_ROOM bedRoom = new HIS_BED_ROOM();
                        bedRoom.BED_ROOM_CODE = item.BED_ROOM_CODE;
                        bedRoom.BED_ROOM_NAME = item.BED_ROOM_NAME;
                        bedRoom.IS_SURGERY = item.IS_SURGERY;
                        ado.HisBedRoom = bedRoom;
                        HIS_ROOM room = new HIS_ROOM();
                        room.DEPARTMENT_ID = item.DEPARTMENT_ID;
                        room.ROOM_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__BUONG;
                        ado.HisRoom = room;
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
                var dataImports = new BackendAdapter(param).Post<List<HisBedRoomSDO>>("api/HisBedRoom/CreateList", ApiConsumers.MosConsumer, datas, param);
                WaitingManager.Hide();
                if (dataImports != null && dataImports.Count > 0)
                {
                    success = true;
                    btnImport.Enabled = false;
                    LoadDataBeRoom();
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
