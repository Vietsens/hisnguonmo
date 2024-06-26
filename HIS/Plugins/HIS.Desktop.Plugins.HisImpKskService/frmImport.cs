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
using HIS.Desktop.Plugins.HisImpKskService.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using SDA.EFMODEL.DataModels;
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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisImpKskService
{
    public partial class frmImport : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module _Module { get; set; }
        RefeshReference delegateRefresh;
        List<ImportADO> _ImportAdos;
        List<ImportADO> _CurrentAdos;
        List<V_HIS_ROOM> _ListHisRoom;
        List<HIS_SERVICE> _ListHisServices { get; set; }
        List<HIS_KSK> _ListHisKsk { get; set; }
        List<HIS_KSK_SERVICE> _ListKskServices { get; set; }

        public frmImport()
        {
            InitializeComponent();
        }

        public frmImport(Inventec.Desktop.Common.Modules.Module _module)
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

        public frmImport(Inventec.Desktop.Common.Modules.Module _module, RefeshReference _delegateRefresh)
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

        private void frmImport_Load(object sender, EventArgs e)
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

        private void LoadCurrentData()
        {
            try
            {
                //ProcessGetData();
                _ListHisRoom = new List<V_HIS_ROOM>();
                HisRoomFilter roomFilter = new HisRoomFilter();
                _ListHisRoom = new BackendAdapter(new CommonParam()).Get<List<V_HIS_ROOM>>("api/HisRoom/Get", ApiConsumers.MosConsumer, roomFilter, null);

                _ListKskServices = new List<HIS_KSK_SERVICE>();
                HisKskServiceFilter kskServiceFilter = new HisKskServiceFilter();
                _ListKskServices = new BackendAdapter(new CommonParam()).Get<List<HIS_KSK_SERVICE>>("api/HisKskService/Get", ApiConsumers.MosConsumer, kskServiceFilter, null);

                _ListHisServices = new List<HIS_SERVICE>();
                HisServiceFilter serviceFilter = new HisServiceFilter();
                _ListHisServices = new BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE>>("api/HisService/Get", ApiConsumers.MosConsumer, serviceFilter, null);

                _ListHisKsk = new List<HIS_KSK>();
                HisKskFilter kskFilter = new HisKskFilter();
                _ListHisKsk = new BackendAdapter(new CommonParam()).Get<List<HIS_KSK>>("api/HisKsk/Get", ApiConsumers.MosConsumer, kskFilter, null);


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

                string fileName = System.IO.Path.Combine(Application.StartupPath + "\\Tmp\\Imp\\", "IMPORT_SERVICE_KSK.xlsx");
                Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
                param.Messages = new List<string>();
                if (File.Exists(fileName))
                {
                    saveFileDialog.Title = "Save File";
                    saveFileDialog.FileName = "IMPORT_SERVICE_KSK";
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
                    LoadCurrentData();
                    var import = new Inventec.Common.ExcelImport.Import();
                    if (import.ReadFileExcel(ofd.FileName))
                    {
                        var hisServiceImport = import.GetWithCheck<ImportADO>(0);
                        if (hisServiceImport != null && hisServiceImport.Count > 0)
                        {
                            WaitingManager.Hide();

                            this._CurrentAdos = hisServiceImport.Where(p => checkNull(p)).ToList();

                            if (this._CurrentAdos != null && this._CurrentAdos.Count > 0)
                            {
                                btnShowLineError.Enabled = true;
                                this._ImportAdos = new List<ImportADO>();
                                addServiceToProcessList(_CurrentAdos, ref this._ImportAdos);
                                SetDataSource(this._ImportAdos);
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

  
        bool checkNull(ImportADO data)
        {
            bool result = true;
            try
            {
                if (data != null)
                {
                    if (string.IsNullOrEmpty(data.ROOM_CODE)
                        && string.IsNullOrEmpty(data.SERVICE_CODE) && string.IsNullOrEmpty(data.KSK_CODE))
                    {
                        result = false;
                    }
                }
                else
                    result = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void addServiceToProcessList(List<ImportADO> _service, ref List<ImportADO> _importAdoRef)
        {
            try
            {
                _importAdoRef = new List<ImportADO>();
                long i = 0;
                foreach (var item in _service)
                {
                    i++;
                    string error = "";
                    var serAdo = new ImportADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<ImportADO>(serAdo, item);

                    //Mã phòng
                    if (!string.IsNullOrEmpty(item.ROOM_CODE))
                    {
                        if (item.ROOM_CODE.Length > 19)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Mã phòng");
                        }
                        var getData = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ROOM_CODE == item.ROOM_CODE);
                        if (getData != null)
                        {
                            serAdo.ROOM_ID = getData.ID;
                            serAdo.SERVICE_CODE= getData.ROOM_CODE;
                            serAdo.ROOM_NAME = getData.ROOM_NAME;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Mã phòng");
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Mã phòng");
                    }

                    //Mã dịch vụ
                    if (!string.IsNullOrEmpty(item.SERVICE_CODE))
                    {
                        if (item.SERVICE_CODE.Length > 19)
                            error += string.Format(Message.MessageImport.Maxlength, "Mã dịch vụ");

                        else
                        {
                            var dataOld = this._ListHisServices.FirstOrDefault(p => p.SERVICE_CODE == item.SERVICE_CODE.Trim());
                            if (dataOld != null)
                            {
                                if (dataOld.IS_ACTIVE == 1)
                                {              
                                        serAdo.SERVICE_ID = dataOld.ID;
                                        serAdo.SERVICE_CODE = dataOld.SERVICE_CODE;
                                        serAdo.SERVICE_NAME = dataOld.SERVICE_NAME;
         
                                }
                                else
                                    error += string.Format(Message.MessageImport.DulieuBiKhoa, "Mã dịch vụ");
                            }
                            else

                                error += string.Format(Message.MessageImport.KhongHopLe, "Mã dịch vụ");

                        }
                    }
                    else
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Mã dịch vụ");


                    //Mã khám sức khỏe
                    if (!string.IsNullOrEmpty(item.KSK_CODE))
                    {
                        if (item.KSK_CODE.Length > 10)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Mã khám sức khỏe");
                        }
                        else
                        {
                            var dataOld = this._ListHisKsk.FirstOrDefault(p => p.KSK_CODE == item.KSK_CODE.Trim());
                            if (dataOld != null)
                            {
                                if (dataOld.IS_ACTIVE == 1)
                                {
                                    serAdo.KSK_ID = dataOld.ID;
                                    serAdo.KSK_CODE = dataOld.KSK_CODE;
                                    serAdo.KSK_NAME = dataOld.KSK_NAME;
                                }
                                else
                                    error += string.Format(Message.MessageImport.DulieuBiKhoa, "Mã khám sức khỏe");
                            }
                            else
                                error += string.Format(Message.MessageImport.KhongHopLe, "Mã khám sức khỏe");

                        }
                    }
                    else
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Mã khám sức khỏe");


                    if (!string.IsNullOrEmpty(item.ROOM_CODE) && !string.IsNullOrEmpty(item.SERVICE_CODE) && !string.IsNullOrEmpty(item.KSK_CODE))
                    {
                        var checkTrungs = _service.Where(p => p.ROOM_CODE.Trim() == item.ROOM_CODE.Trim() && p.SERVICE_CODE.Trim() == item.SERVICE_CODE.Trim() && p.KSK_CODE.Trim() == item.KSK_CODE.Trim()).ToList();
                        if (checkTrungs != null && checkTrungs.Count > 1)
                            error += string.Format(Message.MessageImport.FileImportDaTonTai, item.ROOM_CODE.Trim() + " - " + item.SERVICE_CODE.Trim() + " - " + item.KSK_CODE.Trim());

                    }
                    if(!string.IsNullOrEmpty(item.AMOUNT_STR))
                        
                    {
                        var amount = Inventec.Common.TypeConvert.Parse.ToDecimal(item.AMOUNT_STR);
                        if (amount > 99999999999999 || amount < 0)
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Số lượng");
                        }
                        else
                        {
                            serAdo.AMOUNT = amount;
                        }
 
                    }
                    else
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Số lượng");

                    if (!string.IsNullOrEmpty(item.IMP_PRICE))
                    {
                        var price = Inventec.Common.TypeConvert.Parse.ToDecimal(item.IMP_PRICE);
                        if (price > 99999999999999 || price < 0)
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Giá ");
                        }
                        else
                        {
                            serAdo.PRICE = price;
                        }
                    }

                    else serAdo.PRICE = null;
                 
                        if (!string.IsNullOrEmpty(item.ImpVatRatio))
                        {
                            var vat = Inventec.Common.TypeConvert.Parse.ToDecimal(item.ImpVatRatio)/100;
                            if (vat > 1 || vat < 0)
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "VAT ");
                            }
                            else
                            {
                                serAdo.VAT_RATIO = vat;
                            }
                        }
   
                    else serAdo.VAT_RATIO= null;

                    //if (serAdo.ROOM_ID > 0 && serAdo.KSK_ID >0  && this._ListKskServices != null && this._ListKskServices.Count > 0)
                    //{
                    //    var dataS = this._ListKskServices.FirstOrDefault(p => p.ROOM_ID == serAdo.ROOM_ID && p.KSK_ID== serAdo.KSK_ID);
                    //    if (dataS != null)
                    //        error += string.Format(Message.MessageImport.DaTonTai, "Mã phòng, Mã khám sức khỏe");

                    //}


                    serAdo.ERROR = error;
                    //serAdo.ID = i;

                    _importAdoRef.Add(serAdo);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void SetDataSource(List<ImportADO> dataSource)
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

        private void CheckErrorLine(List<ImportADO> dataSource)
        {
            try
            {
                var checkError = this._ImportAdos.Exists(o => !string.IsNullOrEmpty(o.ERROR));
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
                    var errorLine = this._ImportAdos.Where(o => !string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(errorLine);

                }
                else
                {
                    btnShowLineError.Text = "Dòng lỗi";
                    var errorLine = this._ImportAdos.Where(o => string.IsNullOrEmpty(o.ERROR)).ToList();
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
                var row = (ImportADO)gridViewData.GetFocusedRow();
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
                var row = (ImportADO)gridViewData.GetFocusedRow();
                if (row != null)
                {
                    if (this._ImportAdos != null && this._ImportAdos.Count > 0)
                    {
                        this._ImportAdos.Remove(row);
                        var dataCheck = this._ImportAdos.Where(p => p.ROOM_CODE.Trim() == row.ROOM_CODE.Trim() && p.SERVICE_CODE.Trim() == row.SERVICE_CODE.Trim() && p.KSK_CODE.Trim() == row.KSK_CODE.Trim()).ToList();
                        if (dataCheck != null && dataCheck.Count == 1)
                        {
                            if (!string.IsNullOrEmpty(dataCheck[0].ERROR))
                            {
                                string erro = string.Format(Message.MessageImport.FileImportDaTonTai, dataCheck[0].ROOM_CODE.Trim() + " - " + dataCheck[0].SERVICE_CODE.Trim() + " - " + dataCheck[0].KSK_CODE.Trim());
                                //string[] Codes = dataCheck[0].ERROR.Split('|');
                                dataCheck[0].ERROR = dataCheck[0].ERROR.Replace(erro, "");
                            }

                        }
                        SetDataSource(this._ImportAdos);
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
                    ImportADO pData = (ImportADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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
                List<HIS_KSK_SERVICE> datas = new List<HIS_KSK_SERVICE>();

                if (this._ImportAdos != null && this._ImportAdos.Count > 0)
                {
                    //datas.AddRange(this._ImportAdos);
                    foreach (var item in this._ImportAdos)
                    {
                  
                        HIS_KSK_SERVICE ado = new HIS_KSK_SERVICE();
                        ado.KSK_ID = item.KSK_ID;
                        ado.SERVICE_ID = item.SERVICE_ID;
                        ado.ROOM_ID = item.ROOM_ID;
                        ado.AMOUNT = Inventec.Common.TypeConvert.Parse.ToDecimal(item.AMOUNT_STR);
                        if (!string.IsNullOrEmpty(item.IMP_PRICE))
                        {
                            ado.PRICE = Inventec.Common.TypeConvert.Parse.ToDecimal(item.IMP_PRICE);

                        }
                        else
                            ado.PRICE = null;
                        if (!string.IsNullOrEmpty(item.ImpVatRatio))
                        {
                            ado.VAT_RATIO = Inventec.Common.TypeConvert.Parse.ToDecimal(item.ImpVatRatio);

                        }
                        else
                            ado.VAT_RATIO = null;

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
                var dataImports = new BackendAdapter(param).Post<List<HIS_KSK_SERVICE>>("api/HisKskService/CreateList", ApiConsumers.MosConsumer, datas, param);
                WaitingManager.Hide();
                if (dataImports != null && dataImports.Count > 0)
                {
                    success = true;
                    btnImport.Enabled = false;
                    LoadCurrentData();
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
