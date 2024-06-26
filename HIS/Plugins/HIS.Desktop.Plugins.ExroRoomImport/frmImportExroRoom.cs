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
using HIS.Desktop.Plugins.ExroRoomImport.ADO;
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

namespace HIS.Desktop.Plugins.ExroRoomImport
{
    public partial class frmImportExroRoom : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module _Module { get; set; }
        RefeshReference delegateRefresh;
        List<ExroRoomADO> _ExroRoomAdos;
        List<ExroRoomADO> _CurrentAdos;
        List<HIS_EXRO_ROOM> _ListExroRooms { get; set; }

        public frmImportExroRoom()
        {
            InitializeComponent();
        }

        public frmImportExroRoom(Inventec.Desktop.Common.Modules.Module _module)
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

        public frmImportExroRoom(Inventec.Desktop.Common.Modules.Module _module, RefeshReference _delegateRefresh)
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

        private void frmImportBed_Load(object sender, EventArgs e)
        {
            try
            {
                if (this._Module != null)
                {
                    this.Text = this._Module.text;
                }
                SetIcon();
                LoadDataExroRoom();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataExroRoom()
        {
            try
            {
                _ListExroRooms = new List<HIS_EXRO_ROOM>();
                HisExroRoomFilter filter = new HisExroRoomFilter();
                _ListExroRooms = new BackendAdapter(new CommonParam()).Get<List<HIS_EXRO_ROOM>>("api/HisExroRoom/Get", ApiConsumers.MosConsumer, filter, null);
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

                string fileName = System.IO.Path.Combine(Application.StartupPath + "\\Tmp\\Imp\\", "IMPORT_EXROROOM.xlsx");
                Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
                param.Messages = new List<string>();
                if (File.Exists(fileName))
                {
                    saveFileDialog.Title = "Save File";
                    saveFileDialog.FileName = "IMPORT_EXROROOM";
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
                        var hisServiceImport = import.GetWithCheck<ExroRoomADO>(0);
                        if (hisServiceImport != null && hisServiceImport.Count > 0)
                        {
                            List<ExroRoomADO> listAfterRemove = new List<ExroRoomADO>();


                            foreach (var item in hisServiceImport)
                            {
                                bool checkNull = string.IsNullOrEmpty(item.ROOM_CODE) || string.IsNullOrEmpty(item.EXECUTE_ROOM_CODE);

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
                                this._ExroRoomAdos = new List<ExroRoomADO>();
                                addToProcessList(_CurrentAdos, ref this._ExroRoomAdos);
                                SetDataSource(this._ExroRoomAdos);
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

        private void addToProcessList(List<ExroRoomADO> _service, ref List<ExroRoomADO> _exeRoomRef)
        {
            try
            {
                _exeRoomRef = new List<ExroRoomADO>();
                long i = 0;
                foreach (var item in _service)
                {
                    i++;
                    string error = "";
                    var serAdo = new ExroRoomADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<ExroRoomADO>(serAdo, item);

                    if (!string.IsNullOrEmpty(item.ROOM_CODE))
                    {
                        if (item.ROOM_CODE.Length > 10)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, item.ROOM_CODE);
                        }

                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, item.ROOM_CODE);
                    }
                    if (!string.IsNullOrEmpty(item.EXECUTE_ROOM_CODE))
                    {
                        if (item.EXECUTE_ROOM_CODE.Length > 10)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, item.EXECUTE_ROOM_CODE);
                        }

                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, item.EXECUTE_ROOM_CODE);
                    }

                    /* if (!string.IsNullOrEmpty(item.BED_CODE) && !string.IsNullOrEmpty(item.BED_ROOM_CODE))
                     {
                         var HisbedRoom = BackendDataWorker.Get<HIS_BED_ROOM>().FirstOrDefault(p => p.BED_ROOM_CODE == item.BED_ROOM_CODE);
                         var bed = this._ListBeds.FirstOrDefault(p => p.BED_CODE == item.BED_CODE && p.BED_ROOM_ID == HisbedRoom.ID);
                         if (bed != null)
                         {
                             error += string.Format(Message.MessageImport.DaTonTai, "Mã giường");
                         }
                     }
                     if (!string.IsNullOrEmpty(item.BED_NAME) && !string.IsNullOrEmpty(item.BED_ROOM_CODE))
                     {
                         var HisbedRoom = BackendDataWorker.Get<HIS_BED_ROOM>().FirstOrDefault(p => p.BED_ROOM_CODE == item.BED_ROOM_CODE);
                         var bed = this._ListBeds.FirstOrDefault(p => p.BED_NAME == item.BED_NAME && p.BED_ROOM_ID == HisbedRoom.ID);
                         if (bed != null)
                         {
                             error += string.Format(Message.MessageImport.DaTonTai, "Tên giường");
                         }
                     }
                     if (!string.IsNullOrEmpty(item.BED_NAME) && !string.IsNullOrEmpty(item.BED_TYPE_CODE))
                     {
                         var Hisbedtype = BackendDataWorker.Get<HIS_BED_TYPE>().FirstOrDefault(p => p.BED_TYPE_CODE == item.BED_TYPE_CODE);
                         var bed = this._ListBeds.FirstOrDefault(p => p.BED_NAME == item.BED_NAME && p.BED_ROOM_ID == Hisbedtype.ID);
                         if (bed != null)
                         {
                             error += string.Format(Message.MessageImport.DaTonTaiLoaiGiuong, "Tên giường");
                         }
                     }
                    */
                    //

                    var checkTrung12 = _service.Where(p => (p.ROOM_CODE == item.ROOM_CODE && p.EXECUTE_ROOM_CODE == item.EXECUTE_ROOM_CODE && p.ROOM_CODE == item.ROOM_CODE && p.IS_PRIORITY_REQUIRE == item.IS_PRIORITY_REQUIRE && p.IS_HOLD_ORDER == item.IS_HOLD_ORDER && p.IS_ALLOW_REQUEST == item.IS_ALLOW_REQUEST) || (p.ROOM_CODE == item.ROOM_CODE && p.EXECUTE_ROOM_CODE == item.EXECUTE_ROOM_CODE)).ToList();
                    if (checkTrung12 != null && checkTrung12.Count > 1)
                    {
                        error += string.Format(Message.MessageImport.TonTaiTrungNhauTrongFileImport, item.ROOM_CODE);
                    }


                    if (!string.IsNullOrEmpty(item.EXECUTE_ROOM_CODE))
                    {
                        var exeRoom = BackendDataWorker.Get<HIS_EXECUTE_ROOM>().FirstOrDefault(p => p.EXECUTE_ROOM_CODE == item.EXECUTE_ROOM_CODE.Trim());
                        if (exeRoom != null)
                        {
                            if (exeRoom.IS_ACTIVE == 1)
                            {
                                serAdo.EXECUTE_ROOM_ID = exeRoom.ID;
                            }
                            else
                            {
                                error += string.Format(Message.MessageImport.MaPhongDaKhoa, item.EXECUTE_ROOM_CODE);
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, item.EXECUTE_ROOM_CODE);
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, item.EXECUTE_ROOM_CODE);
                    }

                    if (!string.IsNullOrEmpty(item.ROOM_CODE))
                    {
                        var room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(p => p.ROOM_CODE == item.ROOM_CODE.Trim());
                        if (room != null)
                        {
                            if (room.IS_ACTIVE == 1)
                            {
                                serAdo.ID = room.ID;
                            }
                            else
                            {
                                error += string.Format(Message.MessageImport.MaPhongDaKhoa, item.ROOM_CODE);
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, item.ROOM_CODE);
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, item.ROOM_CODE);
                    }

                    if (string.IsNullOrEmpty(item.IS_HOLD_ORDER.ToString()) && string.IsNullOrEmpty(item.IS_ALLOW_REQUEST.ToString()))
                    {
                        if (!string.IsNullOrEmpty(item.EXECUTE_ROOM_CODE))
                        {
                            var exeRoom = BackendDataWorker.Get<HIS_EXECUTE_ROOM>().FirstOrDefault(p => p.EXECUTE_ROOM_CODE == item.EXECUTE_ROOM_CODE.Trim());
                            if (exeRoom != null)
                            {
                                error += string.Format(Message.MessageImport.NhapMotTrongHai, item.EXECUTE_ROOM_CODE, exeRoom.EXECUTE_ROOM_NAME);
                            }
                            else
                            {
                                error += string.Format(Message.MessageImport.NhapMotTrongHai, item.EXECUTE_ROOM_CODE);
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(item.EXECUTE_ROOM_CODE) && !string.IsNullOrEmpty(item.ROOM_CODE))
                    {
                        var room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(p => p.ROOM_CODE == item.ROOM_CODE.Trim());
                        var exeRoom = BackendDataWorker.Get<HIS_EXECUTE_ROOM>().FirstOrDefault(p => p.EXECUTE_ROOM_CODE == item.EXECUTE_ROOM_CODE.Trim());
                        if (room != null && exeRoom != null)
                        {
                            //var checkExroRoom = BackendDataWorker.Get<HIS_EXRO_ROOM>().FirstOrDefault(p => p.ROOM_ID == room.ID && p.EXECUTE_ROOM_ID == exeRoom.ID);

                            CommonParam paramCommon = new CommonParam();
                            HisExroRoomFilter filter = new HisExroRoomFilter();
                            filter.IS_ACTIVE = 1;

                            var checkExroRoom = new BackendAdapter(paramCommon).Get<List<HIS_EXRO_ROOM>>
                                ("api/HisExroRoom/Get", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, filter, paramCommon);
                            var check = checkExroRoom.Where(o => o.EXECUTE_ROOM_ID == exeRoom.ID && o.ROOM_ID == room.ID).FirstOrDefault();
                            if (check != null)
                            {
                                error += string.Format(Message.MessageImport.DaTonTai, item.ROOM_CODE);
                            }
                        }


                    }

                    serAdo.ERROR = error;
                    serAdo.ID = i;

                    _exeRoomRef.Add(serAdo);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void SetDataSource(List<ExroRoomADO> dataSource)
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

        private void CheckErrorLine(List<ExroRoomADO> dataSource)
        {
            try
            {
                var checkError = this._ExroRoomAdos.Exists(o => !string.IsNullOrEmpty(o.ERROR));
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
                    var errorLine = this._ExroRoomAdos.Where(o => !string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(errorLine);

                }
                else
                {
                    btnShowLineError.Text = "Dòng lỗi";
                    var errorLine = this._ExroRoomAdos.Where(o => string.IsNullOrEmpty(o.ERROR)).ToList();
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
                var row = (ExroRoomADO)gridViewData.GetFocusedRow();
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
                var row = (ExroRoomADO)gridViewData.GetFocusedRow();
                if (row != null)
                {
                    if (this._ExroRoomAdos != null && this._ExroRoomAdos.Count > 0)
                    {
                        this._ExroRoomAdos.Remove(row);
                        var dataCheck =
                        _ExroRoomAdos.Where(p => (p.ROOM_CODE == row.ROOM_CODE && p.EXECUTE_ROOM_CODE == row.EXECUTE_ROOM_CODE && p.ROOM_CODE == row.ROOM_CODE && p.IS_PRIORITY_REQUIRE == row.IS_PRIORITY_REQUIRE && p.IS_HOLD_ORDER == row.IS_HOLD_ORDER && p.IS_ALLOW_REQUEST == row.IS_ALLOW_REQUEST) || (p.ROOM_CODE == row.ROOM_CODE && p.EXECUTE_ROOM_CODE == row.EXECUTE_ROOM_CODE)).ToList();
                        if (dataCheck != null && dataCheck.Count == 1)
                        {
                            if (!string.IsNullOrEmpty(dataCheck[0].ERROR))
                            {
                                string erro = string.Format(Message.MessageImport.TonTaiTrungNhauTrongFileImport, dataCheck[0].ROOM_CODE);
                                dataCheck[0].ERROR = dataCheck[0].ERROR.Replace(erro, "");
                            }

                        }
                        SetDataSource(this._ExroRoomAdos);
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
                    ExroRoomADO pData = (ExroRoomADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    if (e.Column.FieldName == "ROOM_NAME_STR")
                    {
                        var roomName = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ROOM_CODE == pData.ROOM_CODE).ROOM_NAME;
                        e.Value = roomName;
                    }
                    if (e.Column.FieldName == "EXECUTE_ROOM_NAME_STR")
                    {
                        var exeRoomName = BackendDataWorker.Get<HIS_EXECUTE_ROOM>().FirstOrDefault(o => o.EXECUTE_ROOM_CODE == pData.EXECUTE_ROOM_CODE).EXECUTE_ROOM_NAME;
                        e.Value = exeRoomName;
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
                List<ExroRoomADO> datas = new List<ExroRoomADO>();

                if (this._ExroRoomAdos != null && this._ExroRoomAdos.Count > 0)
                {
                    foreach (var item in this._ExroRoomAdos)
                    {
                        var executeRoom = BackendDataWorker.Get<HIS_EXECUTE_ROOM>().FirstOrDefault(p => p.EXECUTE_ROOM_CODE == item.EXECUTE_ROOM_CODE.Trim());
                        var assignRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(p => p.ROOM_CODE == item.ROOM_CODE.Trim());
                        if (executeRoom != null && assignRoom != null)
                        {
                            ExroRoomADO ado = new ExroRoomADO();
                            ado.EXECUTE_ROOM_ID = executeRoom.ID;
                            ado.ROOM_ID = assignRoom.ID;
                            ado.EXECUTE_ROOM_CODE = item.EXECUTE_ROOM_CODE;
                            ado.ROOM_CODE = item.ROOM_CODE;
                            ado.IS_ALLOW_REQUEST = item.IS_ALLOW_REQUEST;
                            ado.IS_PRIORITY_REQUIRE = item.IS_PRIORITY_REQUIRE;
                            ado.IS_HOLD_ORDER = item.IS_HOLD_ORDER;
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
                var dataImports = new BackendAdapter(param).Post<List<HIS_EXRO_ROOM>>("api/HisExroRoom/CreateList", ApiConsumers.MosConsumer, datas, param);
                Inventec.Common.Logging.LogSystem.Debug("Dữ liệu api/HisExroRoom/CreateList trả về: ");
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataImports), dataImports));
                WaitingManager.Hide();
                if (dataImports != null && dataImports.Count > 0)
                {
                    success = true;
                    btnImport.Enabled = false;
                    LoadDataExroRoom();
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
                    //BedADO data = (BedADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
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
