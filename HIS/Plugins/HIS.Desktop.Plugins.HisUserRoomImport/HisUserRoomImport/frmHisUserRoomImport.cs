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
using HIS.Desktop.Plugins.HisUserRoomImport.ADO;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
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

namespace HIS.Desktop.Plugins.HisUserRoomImport.HisUserRoomImport
{
    public partial class frmHisUserRoomImport : FormBase
    {
        #region Declare
        Inventec.Desktop.Common.Modules.Module _Module { get; set; }
        RefeshReference delegateRefresh;
        List<UserRoomADO> _userRoomAdos;
        List<UserRoomADO> _CurrentAdos;
        int checkButtonErrorLine = 0;
        bool isFirstTime = true;
        CommonParam paramDB = new CommonParam();
        HisUserRoomFilter filterDB = new HisUserRoomFilter();
        List<HIS_USER_ROOM> _ListHisUserName { get; set; }
        List<HIS_USER_ROOM> rsDB = new List<HIS_USER_ROOM>();
        #endregion

        #region Construct
        public frmHisUserRoomImport()
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
        public frmHisUserRoomImport(Inventec.Desktop.Common.Modules.Module moduleData)
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
        public frmHisUserRoomImport(Inventec.Desktop.Common.Modules.Module _module, RefeshReference _delegateRefresh)
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
        private void frmHisUserRoomImport_Load(object sender, EventArgs e)
        {
            try
            {
                if (this._Module != null)
                {
                    this.Text = this._Module.text;
                }
                SetIcon();
                LoadDataUserRoom();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataUserRoom()
        {
            try
            {
                _ListHisUserName = new List<HIS_USER_ROOM>();
                MOS.Filter.HisUserRoomFilter filter = new MOS.Filter.HisUserRoomFilter();
                _ListHisUserName = new BackendAdapter(new CommonParam()).Get<List<HIS_USER_ROOM>>("api/HisUserRoom/Get", ApiConsumers.MosConsumer, filter, null);
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
                string fileName = System.IO.Path.Combine(Application.StartupPath + "\\Tmp\\Imp\\", "IMPORT_USER_ROOM.xlsx");
                Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
                param.Messages = new List<string>();
                if (File.Exists(fileName))
                {
                    saveFileDialog.Title = "Save File";
                    saveFileDialog.FileName = "IMPORT_USER_ROOM";
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
                        var hisUserRoomImport = import.GetWithCheck<UserRoomADO>(0);
                        if (hisUserRoomImport != null && hisUserRoomImport.Count > 0)
                        {
                            List<UserRoomADO> listAfterRemove = new List<UserRoomADO>();


                            foreach (var item in hisUserRoomImport)
                            {
                                bool checkNull =
                                string.IsNullOrEmpty(item.LOGINNAME) && string.IsNullOrEmpty(item.ROOM_CODE);
                                if (!checkNull)
                                {
                                    listAfterRemove.Add(item);
                                }
                            }


                            this._CurrentAdos = listAfterRemove;

                            if (this._CurrentAdos != null && this._CurrentAdos.Count > 0)
                            {
                                btnShowLineError.Enabled = true;
                                this._userRoomAdos = new List<UserRoomADO>();
                                rsDB = new BackendAdapter(paramDB).Get<List<HIS_USER_ROOM>>("api/HisUserRoom/Get", ApiConsumers.MosConsumer, filterDB, paramDB).ToList();
                                addServiceToProcessList(_CurrentAdos, ref this._userRoomAdos);
                                isFirstTime = false;
                                SetDataSource(this._userRoomAdos);
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

        private void SetDataSource(List<UserRoomADO> dataSource)
        {
            try
            {
                gridControlUserRoomImpor.BeginUpdate();
                gridControlUserRoomImpor.DataSource = null;
                gridControlUserRoomImpor.DataSource = dataSource;
                gridControlUserRoomImpor.EndUpdate();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void CheckErrorLine()
        {
            try
            {
                var checkError = this._userRoomAdos.Exists(o => !string.IsNullOrEmpty(o.ERROR));
                if (!checkError)
                {
                    btnSave.Enabled = true;
                    btnShowLineError.Enabled = false;
                    SetDataSource(this._userRoomAdos);
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
        private void addServiceToProcessList(List<UserRoomADO> _CurrentService, ref List<UserRoomADO> _userRoomAdos)
        {
            try
            {
                _userRoomAdos = new List<UserRoomADO>();
                long i = 0;

                foreach (var item in _CurrentService)
                {
                    i++;
                    string error = "";
                    var serAdo = new UserRoomADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<UserRoomADO>(serAdo, item);
                    bool checkTrungDB = false;
                    bool checkTrungTrongFile = false;
                    bool checkPrivate = true;
                    List<V_HIS_ROOM> userRoom = new List<V_HIS_ROOM>();
                    userRoom = null;
                    // check trung trong file import
                    if (!string.IsNullOrEmpty(item.ROOM_CODE) && !string.IsNullOrEmpty(item.LOGINNAME) && item.ROOM_CODE != "" && item.LOGINNAME != "")
                    {
                        var count = _CurrentService.Where(o => o.LOGINNAME == item.LOGINNAME && o.ROOM_CODE == item.ROOM_CODE && o.ROOM_TYPE_CODE == item.ROOM_TYPE_CODE).ToList();
                        if (count.Count > 1)
                            checkTrungTrongFile = true;
                    }
                    if (checkTrungTrongFile)
                    {
                        error += string.Format(Message.MessageImport.TonTaiTrungNhauTrongFileImport);
                    }
                    // check neu du lieu trong file import da ton tai trong DB
                    //if (isFirstTime)

                    if (!string.IsNullOrEmpty(item.ROOM_CODE) && !string.IsNullOrEmpty(item.LOGINNAME) && item.ROOM_CODE != "" && item.LOGINNAME != "" && !string.IsNullOrEmpty(item.ROOM_TYPE_CODE) && item.ROOM_TYPE_CODE != "")
                    {
                        userRoom = BackendDataWorker.Get<V_HIS_ROOM>().Where(p => p.ROOM_CODE == item.ROOM_CODE && p.ROOM_TYPE_CODE == item.ROOM_TYPE_CODE).ToList();
                        if (userRoom != null && userRoom.Count > 0)
                        {
                            List<HIS_USER_ROOM> rs = rsDB.Where(o => o.LOGINNAME == item.LOGINNAME && o.ROOM_ID == userRoom.FirstOrDefault().ID).ToList();
                            if (rs != null && rs.Count > 0)
                                checkTrungDB = true;
                        }
                    }
                    if (checkTrungDB)
                    {
                        if (error != "") error += " | ";
                        error += string.Format(Message.MessageImport.DBDaTonTai, item.LOGINNAME);
                    }

                    // check tinh phu hop cua cac du lieu truyen vao
                    if (!string.IsNullOrEmpty(item.LOGINNAME))
                    {
                        if (item.LOGINNAME.Length > 50)
                        {
                            if (error != "") error += " | ";
                            error += string.Format(Message.MessageImport.Maxlength, "Tên đăng nhập");
                        }

                        var rs = BackendDataWorker.Get<ACS_USER>().FirstOrDefault(p => p.LOGINNAME == item.LOGINNAME);
                        if (rs == null)
                        {
                            if (error != "") error += " | ";
                            error += string.Format(Message.MessageImport.KhongHopLe, "Tên đăng nhập");
                        }
                    }
                    else
                    {
                        if (error != "") error += " | ";
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Tên đăng nhập");
                    }

                    if (!string.IsNullOrEmpty(item.ROOM_CODE))
                    {
                        if (item.ROOM_CODE.Length > 10)
                        {
                            if (error != "") error += " | ";
                            error += string.Format(Message.MessageImport.Maxlength, "Mã phòng");
                            checkPrivate = false;
                        }
                        userRoom = BackendDataWorker.Get<V_HIS_ROOM>().Where(p => p.ROOM_CODE == item.ROOM_CODE).ToList();
                        if (userRoom != null && userRoom.Count > 0)
                        {
                            //var itemCurrent = userRoom.FirstOrDefault();
                            //if (itemCurrent != null)
                            //    if (itemCurrent.IS_ACTIVE == 1)
                            //    {
                            //        serAdo.ROOM_ID = itemCurrent.ID;
                            //    }
                            //    else
                            //    {
                            //        serAdo.ROOM_ID = itemCurrent.ID;
                            //        if (error != "") error += " | ";
                            //        error += string.Format(Message.MessageImport.MaPhongDaKhoa);
                            //    }
                        }
                        else
                        {
                            if (error != "") error += " | ";
                            error += string.Format(Message.MessageImport.KhongHopLe, "Mã phòng");
                            checkPrivate = false;
                        }
                    }
                    else
                    {
                        if (error != "") error += " | ";
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Mã phòng");
                        checkPrivate = false;
                    }

                    if (!string.IsNullOrEmpty(item.ROOM_TYPE_CODE))
                    {
                        if (item.ROOM_TYPE_CODE.Length > 2)
                        {
                            if (error != "") error += " | ";
                            error += string.Format(Message.MessageImport.Maxlength, "Mã loại phòng");
                            checkPrivate = false;
                        }
                        userRoom = BackendDataWorker.Get<V_HIS_ROOM>().Where(p => p.ROOM_TYPE_CODE == item.ROOM_TYPE_CODE).ToList();
                        if (userRoom != null && userRoom.Count > 0)
                        {
                            //var itemCurrent = userRoom.FirstOrDefault();
                            //if (itemCurrent != null)
                            //    if (itemCurrent.IS_ACTIVE == 1)
                            //    {
                            //        serAdo.ROOM_ID = itemCurrent.ID;
                            //    }
                            //    else
                            //    {
                            //        serAdo.ROOM_ID = itemCurrent.ID;
                            //        if (error != "") error += " | ";
                            //        error += string.Format(Message.MessageImport.MaLoaiPhongDaKhoa);
                            //    }
                        }
                        else
                        {
                            if (error != "") error += " | ";
                            error += string.Format(Message.MessageImport.KhongHopLe, "Mã loại phòng");
                            checkPrivate = false;
                        }
                    }
                    else
                    {
                        if (error != "") error += " | ";
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Mã loại phòng");
                        checkPrivate = false;
                    }
                    if (!string.IsNullOrEmpty(item.ROOM_TYPE_CODE) && !string.IsNullOrEmpty(item.ROOM_CODE) && checkPrivate )
                    {

                        userRoom = BackendDataWorker.Get<V_HIS_ROOM>().Where(p => p.ROOM_TYPE_CODE == item.ROOM_TYPE_CODE && p.ROOM_CODE == item.ROOM_CODE).ToList();
                        if (userRoom != null && userRoom.Count > 0)
                        {
                            var itemCurrent = userRoom.FirstOrDefault();
                            if (itemCurrent != null)
                                if (itemCurrent.IS_ACTIVE == 1)
                                {
                                    serAdo.ROOM_ID = itemCurrent.ID;
                                }
                                else
                                {
                                    serAdo.ROOM_ID = itemCurrent.ID;
                                    if (error != "") error += " | ";
                                    error += string.Format(Message.MessageImport.PhongDaKhoa);
                                }
                        }
                        else
                        {
                            if (error != "") error += " | ";
                            error += string.Format(Message.MessageImport.PhongKhongTonTai);
                        }
                        
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

        private void btnShowLineError_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnShowLineError.Text == "Dòng lỗi")
                {
                    btnShowLineError.Text = "Dòng không lỗi";
                    var errorLine = this._userRoomAdos.Where(o => !string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(errorLine);
                    CheckErrorLine();
                    checkButtonErrorLine = 1;

                }
                else
                {
                    btnShowLineError.Text = "Dòng lỗi";
                    var errorLine = this._userRoomAdos.Where(o => string.IsNullOrEmpty(o.ERROR)).ToList();
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
                List<HIS_USER_ROOM> datas = new List<HIS_USER_ROOM>();

                if (this._userRoomAdos != null && this._userRoomAdos.Count > 0)
                {
                    foreach (var item in this._userRoomAdos)
                    {
                        HIS_USER_ROOM ado = new HIS_USER_ROOM();
                        // ado.GROUP_CODE = item.GROUP_CODE;
                        var result = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(p => p.ROOM_CODE == item.ROOM_CODE);
                        if (result != null)
                            ado.ROOM_ID = result.ID;
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
                var dataImports = new BackendAdapter(param).Post<List<HIS_USER_ROOM>>("api/HisUserRoom/CreateList", ApiConsumers.MosConsumer, datas, param);
                WaitingManager.Hide();
                if (dataImports != null && dataImports.Count > 0)
                {
                    success = true;
                    btnSave.Enabled = false;
                    LoadDataUserRoom();
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

        private void gridViewUserRoomImpor_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    //BedADO data = (BedADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    string error = (gridViewUserRoomImpor.GetRowCellValue(e.RowHandle, "ERROR") ?? "").ToString();
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
                var row = (UserRoomADO)gridViewUserRoomImpor.GetFocusedRow();
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
                var row = (UserRoomADO)gridViewUserRoomImpor.GetFocusedRow();
                if (row != null)
                {
                    if (this._userRoomAdos != null && this._userRoomAdos.Count > 0)
                    {
                        this._userRoomAdos.Remove(row);
                        List<UserRoomADO> rs = this._userRoomAdos;
                        addServiceToProcessList(rs, ref this._userRoomAdos);
                        switch (checkButtonErrorLine)
                        {
                            case 0:
                                {
                                    SetDataSource(this._userRoomAdos);
                                    CheckErrorLine();
                                    break;
                                }
                            case 1:
                                {
                                    SetDataSource(this._userRoomAdos.Where(o => !string.IsNullOrEmpty(o.ERROR)).ToList());
                                    CheckErrorLine();
                                    break;
                                }
                            case 2:
                                {
                                    SetDataSource(this._userRoomAdos.Where(o => string.IsNullOrEmpty(o.ERROR)).ToList()); CheckErrorLine();
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

        private void gridViewUserRoomImpor_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    UserRoomADO pData = (UserRoomADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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

        private void bbtnSave_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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
    }
}
