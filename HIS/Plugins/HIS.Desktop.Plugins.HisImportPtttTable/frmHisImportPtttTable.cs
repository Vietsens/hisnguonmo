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
using HIS.Desktop.Plugins.HisImportPtttTable.ADO;
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
using HIS.Desktop.Plugins.HisImportPtttTable.Resources;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;

namespace HIS.Desktop.Plugins.HisImportPtttTable
{

    public partial class frmHisImportPtttTable : FormBase
    {
        #region Reclare variable
        Module Module { get; set; }
        RefeshReference delegateRefresh;
        List<PtttTableADO> CurrentADO;
        List<PtttTableADO> PtttTableADO;
        List<HIS_PTTT_TABLE> HisPtttTable { get; set; }
        PagingGrid pagingGrid;
        Module moduleData;
        #endregion
        public frmHisImportPtttTable(Inventec.Desktop.Common.Modules.Module moduleData)
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

        private void frmHisImportPtttTable_Load(object sender, EventArgs e)
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
                ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisImportPtttTable.Resources.Lang", typeof(HIS.Desktop.Plugins.HisImportPtttTable.frmHisImportPtttTable).Assembly);
                this.btnDowloadFile.Text = Inventec.Common.Resource.Get.Value("frmHisPtttTable.btnDowloadFile.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnChooseFile.Text = Inventec.Common.Resource.Get.Value("frmHisPtttTable.btnChooseFile.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnShowLineError.Text = Inventec.Common.Resource.Get.Value("frmHisPtttTable.btnShowLineError.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnImport.Text = Inventec.Common.Resource.Get.Value("frmHisPtttTable.btnImport.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPtttTableCode.Caption = Inventec.Common.Resource.Get.Value("frmHisPtttTable.grdColPtttTableCode.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPtttTableName.Caption = Inventec.Common.Resource.Get.Value("frmHisPtttTable.grdColPtttTableName.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdExecuterRoomId.Caption = Inventec.Common.Resource.Get.Value("frmHisPtttTable.grdExecuterRoomId.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.Caption = Inventec.Common.Resource.Get.Value("frmHisPtttTable.STT.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPtttTableCode.ToolTip = Inventec.Common.Resource.Get.Value("frmHisPtttTable.grdColPtttTableCode.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPtttTableName.ToolTip = Inventec.Common.Resource.Get.Value("frmHisPtttTable.grdColPtttTableName.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdExecuterRoomId.ToolTip = Inventec.Common.Resource.Get.Value("frmHisPtttTable.grdExecuterRoomId.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.ToolTip = Inventec.Common.Resource.Get.Value("frmHisPtttTable.STT.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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
                    saveFileDialog1.FileName = "IMPORT_PTTT_TABLE";
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
                        var hisServiceImport = import.GetWithCheck<PtttTableADO>(0);
                        if (hisServiceImport != null && hisServiceImport.Count > 0)
                        {
                            List<PtttTableADO> listAfterRemove = new List<PtttTableADO>();
                            foreach (var item in hisServiceImport)
                            {
                                bool checkNull = string.IsNullOrEmpty(item.PTTT_TABLE_CODE)
                                    && string.IsNullOrEmpty(item.PTTT_TABLE_NAME)
                                    && string.IsNullOrEmpty(item.EXECUTE_ROOM_CODE)
                                   ;
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
                                this.PtttTableADO = new List<PtttTableADO>();
                                AddServiceToProcessList(CurrentADO, ref this.PtttTableADO);
                                setDataSource(this.PtttTableADO);
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

        private void setDataSource(List<PtttTableADO> datasource)
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

        private void CheckErrorLine(List<PtttTableADO> datasource)
        {
            try
            {
                var checkError = this.PtttTableADO.Exists(o => !string.IsNullOrEmpty(o.ERROR));
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

        private void AddServiceToProcessList(List<PtttTableADO> Service, ref List<PtttTableADO> PtttTableRef)
        {
            try
            {
                PtttTableRef = new List<PtttTableADO>();
                long i = 0;
                foreach (var item in Service)
                {
                    i++;
                    string error = "";
                    var serADO = new PtttTableADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<PtttTableADO>(serADO, item);
                    if (!string.IsNullOrEmpty(item.PTTT_TABLE_CODE))
                    {
                        if (item.PTTT_TABLE_CODE.Length > 4)
                        {
                            error += string.Format(Message.MessImport.Maxlength, "Mã bàn mổ");
                        }
                        else
                        {
                            var PtttTable = this.HisPtttTable.FirstOrDefault(p => p.PTTT_TABLE_CODE == item.PTTT_TABLE_CODE);
                            if (PtttTable != null)
                            {
                                error += string.Format(Message.MessImport.DaTonTai, "Mã bàn mổ");
                            }
                            else
                            {
                                var checkTrung = Service.Where(p => p.PTTT_TABLE_CODE == item.PTTT_TABLE_CODE).ToList();
                                if (checkTrung != null && checkTrung.Count > 1)
                                {
                                    error += string.Format(Message.MessImport.FileImportDaTonTai, item.PTTT_TABLE_CODE);

                                }
                            }
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessImport.ThieuTruongDL, "Mã bàn mổ");
                    }
                    if (!string.IsNullOrEmpty(item.PTTT_TABLE_NAME))
                    {
                        if (item.PTTT_TABLE_NAME.Length > 100)
                        {
                            error += string.Format(Message.MessImport.Maxlength, "Tên bàn mổ");

                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessImport.ThieuTruongDL, "Tên bàn mổ");
                    }
                    if(!string.IsNullOrEmpty(item.EXECUTE_ROOM_CODE))
                    {
                        var RoomID = BackendDataWorker.Get<HIS_EXECUTE_ROOM>().FirstOrDefault(p => p.EXECUTE_ROOM_CODE == item.EXECUTE_ROOM_CODE.Trim());
                        if (RoomID != null)
                        {
                            if (RoomID.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                            {
                                serADO.EXECUTE_ROOM_ID = RoomID.ID;
                            }
                            else
                            {
                                error += string.Format(Message.MessImport.MaPhongKhamDaKhoa, "Mã phòng khám");

                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessImport.KhongHopLe, "Mã phòng khám");
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessImport.ThieuTruongDL, "Mã phòng khám");

                    }
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
                    var errorLine = this.PtttTableADO.Where(o => !string.IsNullOrEmpty(o.ERROR)).ToList();
                    setDataSource(errorLine);
                }
                else
                {
                    btnShowLineError.Text = "Dòng lỗi";
                    var errorLine = this.PtttTableADO.Where(o => string.IsNullOrEmpty(o.ERROR)).ToList();
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
                List<HIS_PTTT_TABLE> data = new List<HIS_PTTT_TABLE>();
                if (this.PtttTableADO != null && this.PtttTableADO.Count > 0)
                {
                    foreach (var item in this.PtttTableADO)
                    {
                        HIS_PTTT_TABLE ado = new HIS_PTTT_TABLE();
                        ado.PTTT_TABLE_CODE = item.PTTT_TABLE_CODE;
                        ado.PTTT_TABLE_NAME = item.PTTT_TABLE_NAME;
                        ado.EXECUTE_ROOM_ID = item.EXECUTE_ROOM_ID;
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
                var dataImport = new BackendAdapter(param).Post<List<HIS_PTTT_TABLE>>("api/HisPtttTable/CreateList", ApiConsumer.ApiConsumers.MosConsumer, data, param);
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
                this.HisPtttTable = new List<HIS_PTTT_TABLE>();
                MOS.Filter.HisPtttTableFilter filter = new MOS.Filter.HisPtttTableFilter();
                this.HisPtttTable=new BackendAdapter(new CommonParam()).Get<List<HIS_PTTT_TABLE>>("api/HisPtttTable/Get",ApiConsumer.ApiConsumers.MosConsumer,filter,null);
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
                var row = (PtttTableADO)grdViewData.GetFocusedRow();
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
                var row = (PtttTableADO)grdViewData.GetFocusedRow();
                if (row != null)
                {
                    if (this.PtttTableADO != null && this.PtttTableADO.Count > 0)
                    {
                        this.PtttTableADO.Remove(row);
                        var datacheck = this.PtttTableADO.Where(p => p.PTTT_TABLE_CODE == row.PTTT_TABLE_CODE).ToList();
                        
                        if (datacheck != null && datacheck.Count == 1)
                        {
                            if (!string.IsNullOrEmpty(datacheck[0].ERROR))
                            {
                                string erro = string.Format(Message.MessImport.FileImportDaTonTai, datacheck[0].PTTT_TABLE_CODE);
                                datacheck[0].ERROR = datacheck[0].ERROR.Replace(erro, "");
                            }
                        }
                        setDataSource(this.PtttTableADO);
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
                    PtttTableADO pData = (PtttTableADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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
