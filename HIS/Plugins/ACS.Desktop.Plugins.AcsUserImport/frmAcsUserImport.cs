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
using Inventec.Desktop.Common.Modules;
using HIS.Desktop.Common;
using MOS.EFMODEL.DataModels;
using System.IO;
using Inventec.Desktop.Common.Message;
using Inventec.Common.Logging;
using ACS.Desktop.Plugins.AcsUserImport.ADO;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Core;
using Inventec.Common.Adapter;
using Inventec.Desktop.Common.Message;
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using System.Collections;
using HIS.Desktop.Utility;
using Inventec.UC.Paging;
using HIS.Desktop.LocalStorage.Location;
using ACS.Desktop.Plugins.AcsUserImport.Resources;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using ACS.EFMODEL.DataModels;

namespace ACS.Desktop.Plugins.AcsUserImport
{

    public partial class frmAcsUserImport : FormBase
    {
        #region Reclare variable
        Module Module { get; set; }
        RefeshReference delegateRefresh;
        List<AcsUserImportADO> CurrentADO;
        List<AcsUserImportADO> AcsUserImportADO;
        List<ACS_USER> AcsUser { get; set; }
        PagingGrid pagingGrid;
        Module moduleData;
        #endregion
        public frmAcsUserImport(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();

                pagingGrid = new PagingGrid();
                this.moduleData = moduleData;
                grdControlData.ToolTipController = toolTipController1;

                try
                {
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
           
        }

        private void frmAcsUserImport_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                LoadDataPtttTable();
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                if (this.moduleData != null && !String.IsNullOrEmpty(this.moduleData.text))
                {
                    this.Text = this.moduleData.text;
                }
                ResourceLanguageManager.LanguageResource = new ResourceManager("ACS.Desktop.Plugins.AcsUserImport.Resources.Lang", typeof(ACS.Desktop.Plugins.AcsUserImport.frmAcsUserImport).Assembly);
                this.btnDowloadFile.Text = Inventec.Common.Resource.Get.Value("frmAcsUser.btnDowloadFile.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnChooseFile.Text = Inventec.Common.Resource.Get.Value("frmAcsUser.btnChooseFile.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnShowLineError.Text = Inventec.Common.Resource.Get.Value("frmAcsUser.btnShowLineError.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnImport.Text = Inventec.Common.Resource.Get.Value("frmAcsUser.btnImport.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPtttTableCode.Caption = Inventec.Common.Resource.Get.Value("frmAcsUser.grdColPtttTableCode.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPtttTableName.Caption = Inventec.Common.Resource.Get.Value("frmAcsUser.grdColPtttTableName.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdExecuterRoomId.Caption = Inventec.Common.Resource.Get.Value("frmAcsUser.grdExecuterRoomId.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.Caption = Inventec.Common.Resource.Get.Value("frmAcsUser.STT.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPtttTableCode.ToolTip = Inventec.Common.Resource.Get.Value("frmAcsUser.grdColPtttTableCode.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPtttTableName.ToolTip = Inventec.Common.Resource.Get.Value("frmAcsUser.grdColPtttTableName.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdExecuterRoomId.ToolTip = Inventec.Common.Resource.Get.Value("frmAcsUser.grdExecuterRoomId.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.ToolTip = Inventec.Common.Resource.Get.Value("frmAcsUser.STT.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void btnDowloadFile_Click(object sender, EventArgs e)
        {
            try
            {
                string fileName = System.IO.Path.Combine(Application.StartupPath + "\\Tmp\\Imp\\", "IMPORT_PTTT_TABLE.xlsx");
                Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
                param.Messages = new List<string>();
                if (File.Exists(fileName))
                {
                    saveFileDialog1.Title = "Save File";
                    saveFileDialog1.FileName = "IMPORT_ACS_USER_DANGNHAP";
                    saveFileDialog1.DefaultExt = "xlsx";
                    saveFileDialog1.Filter = "Excel files (*.xlsx)|All files (*.*)";
                    saveFileDialog1.FilterIndex = 2;
                    saveFileDialog1.RestoreDirectory = true;
                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        File.Copy(fileName, saveFileDialog1.FileName);
                        MessageManager.Show(this.ParentForm, param, true);
                        if (DevExpress.XtraEditors.XtraMessageBox.Show("Bạn có muốn mở file ngay?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
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
                        var hisServiceImport = import.GetWithCheck<AcsUserImportADO>(0);
                        if (hisServiceImport != null && hisServiceImport.Count > 0)
                        {
                            List<AcsUserImportADO> listAfterRemove = new List<AcsUserImportADO>();
                            foreach (var item in hisServiceImport)
                            {
                                bool checkNull = string.IsNullOrEmpty(item.LOGINNAME)
                                    && string.IsNullOrEmpty(item.USERNAME);
                                if (!checkNull)
                                {
                                    listAfterRemove.Add(item);
                                }
                            }
                            WaitingManager.Hide();
                            this.CurrentADO = listAfterRemove;
                            if (this.CurrentADO != null && this.CurrentADO.Count > 0)
                            {
                                btnShowLineError.Enabled = true;
                                this.AcsUserImportADO = new List<AcsUserImportADO>();
                                AddServiceToProcessList(CurrentADO, ref this.AcsUserImportADO);
                                setDataSource(this.AcsUserImportADO);
                            }

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

                LogSystem.Error(ex);
            }
        }

        private void setDataSource(List<AcsUserImportADO> datasource)
        {
            try
            {
                grdControlData.BeginUpdate();
                grdControlData.DataSource = null;
                grdControlData.DataSource = datasource;
                grdControlData.EndUpdate();
                CheckErrorLine(null);
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void CheckErrorLine(List<AcsUserImportADO> datasource)
        {
            try
            {
                var checkError = this.AcsUserImportADO.Exists(o => !string.IsNullOrEmpty(o.ERROR));
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

                LogSystem.Warn(ex);
            }
        }

        private void AddServiceToProcessList(List<AcsUserImportADO> Service, ref List<AcsUserImportADO> PtttTableRef)
        {
            try
            {
                PtttTableRef = new List<AcsUserImportADO>();
                long i = 0;
                foreach (var item in Service)
                {
                    i++;
                    string error = "";
                    var serADO = new AcsUserImportADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<AcsUserImportADO>(serADO, item);
                    if (!string.IsNullOrEmpty(item.LOGINNAME))
                    {
                        if (item.LOGINNAME.Length > 50)
                        {
                            error += string.Format(Message.MessImport.Maxlength, "Tài khoản");
                        }
                        else
                        {
                            var PtttTable = this.AcsUser.FirstOrDefault(p => p.LOGINNAME == item.LOGINNAME);
                            if (PtttTable != null)
                            {
                                error += string.Format(Message.MessImport.DaTonTai, "Tài khoản");
                            }
                            else
                            {
                                var checkTrung = Service.Where(p => p.LOGINNAME == item.LOGINNAME).ToList();
                                if (checkTrung != null && checkTrung.Count > 1)
                                {
                                    error += string.Format(Message.MessImport.FileImportDaTonTai, item.LOGINNAME);

                                }
                            }
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessImport.ThieuTruongDL, "Tài khoản");
                    }

                    if (!string.IsNullOrEmpty(item.USERNAME))
                    {
                        if (item.USERNAME.Length > 100)
                        {
                            error += string.Format(Message.MessImport.Maxlength, "Tên tài khoản");

                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessImport.ThieuTruongDL, "Tên tài khoản");
                    }


                    if (!string.IsNullOrEmpty(item.EMAIL))
                    {
                        if (item.USERNAME.Length > 100)
                        {
                            error += string.Format(Message.MessImport.Maxlength, "Mail");

                        }
                    }

                    if (!string.IsNullOrEmpty(item.MOBILE))
                    {
                        if (item.USERNAME.Length > 20)
                        {
                            error += string.Format(Message.MessImport.Maxlength, "Số điện thoại");

                        }
                    }

                    if (!string.IsNullOrEmpty(item.G_CODE))
                    {
                        if (item.USERNAME.Length > 50)
                        {
                            error += string.Format(Message.MessImport.Maxlength, "Mã đơn vị");

                        }
                    }


                    //if(!string.IsNullOrEmpty(item.EXECUTE_ROOM_CODE))
                    //{
                    //    var RoomID = BackendDataWorker.Get<HIS_EXECUTE_ROOM>().FirstOrDefault(p => p.EXECUTE_ROOM_CODE == item.EXECUTE_ROOM_CODE.Trim());
                    //    if (RoomID != null)
                    //    {
                    //        if (RoomID.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    //        {
                    //            serADO.EXECUTE_ROOM_ID = RoomID.ID;
                    //        }
                    //        else
                    //        {
                    //            error += string.Format(Message.MessImport.MaPhongKhamDaKhoa, "Mã phòng khám");

                    //        }
                    //    }
                    //    else
                    //    {
                    //        error += string.Format(Message.MessImport.KhongHopLe, "Mã phòng khám");
                    //    }
                    //}
                    //else
                    //{
                    //    error += string.Format(Message.MessImport.ThieuTruongDL, "Mã phòng khám");

                    //}
                    serADO.ERROR = error;
                    serADO.ID = i;
                    PtttTableRef.Add(serADO);
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void btnShowLineError_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnShowLineError.Text == "Dòng lỗi")
                {
                    btnShowLineError.Text = "Dòng không lỗi";
                    var errorLine = this.AcsUserImportADO.Where(o => !string.IsNullOrEmpty(o.ERROR)).ToList();
                    setDataSource(errorLine);
                }
                else
                {
                    btnShowLineError.Text = "Dòng lỗi";
                    var errorLine = this.AcsUserImportADO.Where(o => string.IsNullOrEmpty(o.ERROR)).ToList();
                    setDataSource(errorLine);
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                bool success = false;
                WaitingManager.Show();
                List<ACS_USER> data = new List<ACS_USER>();
                if (this.AcsUserImportADO != null && this.AcsUserImportADO.Count > 0)
                {
                    foreach (var item in this.AcsUserImportADO)
                    {
                        ACS_USER ado = new ACS_USER();
                        ado.LOGINNAME = item.LOGINNAME;
                        ado.USERNAME = item.USERNAME;
                       
                        if (!string.IsNullOrEmpty(item.EMAIL))
                        {
                        ado.EMAIL = item.EMAIL;
                        }
                        if (!string.IsNullOrEmpty(item.MOBILE))
                        {
                            ado.MOBILE = item.MOBILE;
                        }
                        if (!string.IsNullOrEmpty(item.G_CODE))
                        {
                            ado.G_CODE = item.G_CODE;
                        }
                       

                        data.Add(ado);
                    }
                }
                else
                {
                    WaitingManager.Hide();
                    DevExpress.XtraEditors.XtraMessageBox.Show("Dữ liệu rỗng", "Thông báo");
                    return;
                }
                CommonParam param = new CommonParam();
                var dataImport = new BackendAdapter(param).Post<List<ACS_USER>>("api/AcsUser/CreateList", HIS.Desktop.ApiConsumer.ApiConsumers.AcsConsumer, data, param);
                WaitingManager.Hide();
                if (dataImport != null && dataImport.Count > 0)
                {
                    success = true;
                    btnImport.Enabled = false;
                    LoadDataPtttTable();
                    if (this.delegateRefresh != null)
                    {
                        this.delegateRefresh();
                    }
                }
                MessageManager.Show(this.ParentForm, param, success);
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void LoadDataPtttTable()
        {
            try
            {
                this.AcsUser = new List<ACS_USER>();
                ACS.Filter.AcsUserFilter filter = new  ACS.Filter.AcsUserFilter();
                this.AcsUser=new BackendAdapter(new CommonParam()).Get<List<ACS_USER>>("api/AcsUser/Get",HIS.Desktop.ApiConsumer.ApiConsumers.AcsConsumer,filter,null);
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void btnError_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (AcsUserImportADO)grdViewData.GetFocusedRow();
                if (row != null && !string.IsNullOrEmpty(row.ERROR))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(row.ERROR, "Thông báo");
                }
            }
            catch (Exception ex)
            {

                LogSession.Warn(ex);
            }
        }

        private void btnDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (AcsUserImportADO)grdViewData.GetFocusedRow();
                if (row != null)
                {
                    if (this.AcsUserImportADO != null && this.AcsUserImportADO.Count > 0)
                    {
                        this.AcsUserImportADO.Remove(row);
                        var datacheck = this.AcsUserImportADO.Where(p => p.LOGINNAME == row.LOGINNAME).ToList();
                        
                        if (datacheck != null && datacheck.Count == 1)
                        {
                            if (!string.IsNullOrEmpty(datacheck[0].ERROR))
                            {
                                string erro = string.Format(Message.MessImport.FileImportDaTonTai, datacheck[0].LOGINNAME);
                                datacheck[0].ERROR = datacheck[0].ERROR.Replace(erro, "");
                            }
                        }
                        setDataSource(this.AcsUserImportADO);
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void grdViewData_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    AcsUserImportADO pData = (AcsUserImportADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void grdViewData_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    string error = (grdViewData.GetRowCellValue(e.RowHandle, "ERROR") ?? "").ToString();
                    if (e.Column.FieldName == "ERROR")
                    {
                        if (!string.IsNullOrEmpty(error))
                        {
                            e.RepositoryItem = btnError;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void bbtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnImport.Enabled)
                {
                    btnImport_Click(null, null);
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }
    }
}
