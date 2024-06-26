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
using HIS.Desktop.Plugins.ImpHisMediStockMaty.ADO;
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

namespace HIS.Desktop.Plugins.ImpHisMediStockMaty
{
    public partial class frmImport : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module _Module { get; set; }
        RefeshReference delegateRefresh;
        List<ImportADO> _ImportAdos;
        List<ImportADO> _CurrentAdos;
        List<V_HIS_MEDI_STOCK> _ListHisMediStock;
        List<V_HIS_MATERIAL_TYPE> _LisMaterialType { get; set; }
        //List<HIS_MEDI_STOCK_MATY> _ListKskServices { get; set; }

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
                //LoadCurrentData();
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
                _ListHisMediStock = new List<V_HIS_MEDI_STOCK>();
                HisMediStockViewFilter mediStockFilter = new HisMediStockViewFilter();
                _ListHisMediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>();

                _LisMaterialType = new List<V_HIS_MATERIAL_TYPE>();
                HisMaterialTypeViewFilter materialTypeFilter = new HisMaterialTypeViewFilter();
                _LisMaterialType = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>();

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

                string fileName = System.IO.Path.Combine(Application.StartupPath + "\\Tmp\\Imp\\", "IMPORT_MEDI_STOCK_MATY.xlsx");
                Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
                param.Messages = new List<string>();
                if (File.Exists(fileName))
                {
                    saveFileDialog.Title = "Save File";
                    saveFileDialog.FileName = "IMPORT_MEDI_STOCK_MATY";
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
                        var hisMediStockMatyImport = import.GetWithCheck<ImportADO>(0);
                        if (hisMediStockMatyImport != null && hisMediStockMatyImport.Count > 0)
                        {
                            WaitingManager.Hide();

                            this._CurrentAdos = hisMediStockMatyImport.Where(p => checkNull(p)).ToList();

                            if (this._CurrentAdos != null && this._CurrentAdos.Count > 0)
                            {
                                btnShowLineError.Enabled = true;
                                this._ImportAdos = new List<ImportADO>();
                                addMediStockMatyToProcessList(_CurrentAdos, ref this._ImportAdos);
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
                    if (string.IsNullOrEmpty(data.MEDI_STOCK_CODE)
                        && string.IsNullOrEmpty(data.MATERIAL_TYPE_CODE))
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

        private void addMediStockMatyToProcessList(List<ImportADO> _mediStockMaty, ref List<ImportADO> _importAdoRef)
        {
            try
            {
                _importAdoRef = new List<ImportADO>();
                long i = 0;
                foreach (var item in _mediStockMaty)
                {
                    i++;
                    string error = "";
                    var serAdo = new ImportADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<ImportADO>(serAdo, item);
                    //Mã kho
                    if (!string.IsNullOrEmpty(item.MEDI_STOCK_CODE))
                    {
                        if (item.MEDI_STOCK_CODE.Length > 10)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Mã kho");
                        }

                        var getData = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.MEDI_STOCK_CODE == item.MEDI_STOCK_CODE);
                        if (getData != null)
                        {
                            serAdo.MEDI_STOCK_ID = getData.ID;
                            serAdo.MEDI_STOCK_CODE = getData.MEDI_STOCK_CODE;
                            serAdo.MEDI_STOCK_NAME = getData.MEDI_STOCK_NAME;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Mã kho");
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Mã kho");
                    }


                    // check trung

                    var checkTrung = _mediStockMaty.Where(p => p.MEDI_STOCK_CODE.ToUpper().Trim() == item.MEDI_STOCK_CODE.ToUpper().Trim() && p.MATERIAL_TYPE_CODE.ToUpper().Trim() == item.MATERIAL_TYPE_CODE.ToUpper().Trim()).ToList();
                    if (checkTrung != null && checkTrung.Count > 1)
                    {
                        error += string.Format(Message.MessageImport.FileImportDaTonTai, item.MEDI_STOCK_CODE);
                    }

                    //Mã kho xuất
                    if (!string.IsNullOrEmpty(item.EXP_MEDI_STOCK_CODE))
                    {
                        if (item.MEDI_STOCK_CODE.Length > 10)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Mã kho xuất");
                        }
                        var getData = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.MEDI_STOCK_CODE == item.EXP_MEDI_STOCK_CODE);
                        if (getData != null)
                        {
                            serAdo.EXP_MEDI_STOCK_ID = getData.ID;
                            serAdo.EXP_MEDI_STOCK_CODE = getData.MEDI_STOCK_CODE;
                            serAdo.EXP_MEDI_STOCK_NAME = getData.MEDI_STOCK_NAME;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Mã kho xuất");
                        }
                    }



                    //Mã loại vật tư
                    if (!string.IsNullOrEmpty(item.MATERIAL_TYPE_CODE))
                    {
                        if (item.MATERIAL_TYPE_CODE.Length > 25)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Mã loại vật tư");
                        }
                        var getData = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.MATERIAL_TYPE_CODE == item.MATERIAL_TYPE_CODE);
                        if (getData != null)
                        {
                            serAdo.MATERIAL_TYPE_ID = getData.ID;
                            serAdo.MATERIAL_TYPE_CODE = getData.MATERIAL_TYPE_CODE;
                            serAdo.MATERIAL_TYPE_NAME = getData.MATERIAL_TYPE_NAME;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Mã loại vật tư");
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Mã loại vật tư");
                    }
                    //if (item.ALERT_MAX_IN_STOCK != null && item.REAL_BASE_AMOUNT == null)
                    //{
                    //    error += string.Format(Message.MessageImport.NhapCoSoPhaiNhapCoSoThucTe);
                    //}


                    //if (!string.IsNullOrEmpty(item.REAL_BASE_AMOUNT.ToString()))
                    //{
                    //    var realBaseAmount = Inventec.Common.TypeConvert.Parse.ToDecimal(item.REAL_BASE_AMOUNT.ToString());
                    //    var max = Inventec.Common.TypeConvert.Parse.ToDecimal(item.ALERT_MAX_IN_STOCK.ToString());
                    //    if (realBaseAmount > 99999999999999 || realBaseAmount < 0)
                    //    {
                    //        error += string.Format(Message.MessageImport.KhongHopLe, "Cơ số thực ");
                    //    }

                    //    if (realBaseAmount > max)
                    //    {
                    //        error += string.Format(Message.MessageImport.KhongHopLe, "Cơ số thực không thể lớn hơn cơ số tối đa");
                    //    }

                    //    else
                    //    {
                    //        serAdo.REAL_BASE_AMOUNT = realBaseAmount;
                    //    }
                    //}

                    //else serAdo.REAL_BASE_AMOUNT = null;


                    if (!string.IsNullOrEmpty(item.ALERT_MAX_IN_STOCK.ToString()))
                    {
                        var max = Inventec.Common.TypeConvert.Parse.ToDecimal(item.ALERT_MAX_IN_STOCK.ToString());
                        if (max > 99999999999999 || max < 0)
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Cơ số tối đa");
                        }
                        else
                        {
                            serAdo.ALERT_MAX_IN_STOCK = max;
                        }
                    }

                    else serAdo.ALERT_MAX_IN_STOCK = null;




                    if (!string.IsNullOrEmpty(item.ALERT_MIN_IN_STOCK.ToString()))
                    {
                        var min = Inventec.Common.TypeConvert.Parse.ToDecimal(item.ALERT_MIN_IN_STOCK.ToString());
                        if (min > 99999999999999 || min < 0)
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Số lượng sàn");
                        }
                        else
                        {
                            serAdo.ALERT_MIN_IN_STOCK = min;
                        }
                    }


                    if (!string.IsNullOrEmpty(item.IS_PREVENT_MAX_STR))
                    {
                        if (item.IS_PREVENT_MAX_STR.ToLower().Trim() == "x")
                        {
                            serAdo.IS_PREVENT_MAX = 1;

                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Chặn nhập quá trần");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.IS_GOODS_RESTRICT_STR))
                    {
                        if (item.IS_GOODS_RESTRICT_STR.ToLower().Trim() == "x")
                        {
                            serAdo.IS_GOODS_RESTRICT = 1;

                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Vật tư giới hạn");
                        }
                    }
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
                        var dataCheck = this._ImportAdos.Where(p => p.MEDI_STOCK_CODE.ToUpper().Trim() == row.MEDI_STOCK_CODE.ToUpper().Trim() && p.MATERIAL_TYPE_CODE.ToUpper().Trim() == row.MATERIAL_TYPE_CODE.ToUpper().Trim()).ToList();
                        if (dataCheck != null && dataCheck.Count == 1)
                        {
                            if (!string.IsNullOrEmpty(dataCheck[0].ERROR))
                            {
                                string erro = string.Format(Message.MessageImport.FileImportDaTonTai, dataCheck[0].MEDI_STOCK_CODE);
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
                    if (e.Column.FieldName == "IS_GOODS_RESTRICT_STR_BOOL")
                    {

                        e.Value = pData.IS_GOODS_RESTRICT == 1 ? true : false;

                    }
                    if (e.Column.FieldName == "IS_PREVENT_MAX_STR_BOOL")
                    {

                        e.Value = pData.IS_PREVENT_MAX == 1 ? true : false;

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
                List<HIS_MEDI_STOCK_MATY> datas = new List<HIS_MEDI_STOCK_MATY>();
                List<HIS_MEDI_STOCK_MATY> dataUpdates = new List<HIS_MEDI_STOCK_MATY>();
                CommonParam comParam = new CommonParam();
                HisMediStockMatyFilter filter = new HisMediStockMatyFilter();
                var DatacheckTon = new BackendAdapter(comParam).Get<List<HIS_MEDI_STOCK_MATY>>("api/HisMediStockMaty/Get", ApiConsumers.MosConsumer, filter, comParam);
                if (this._ImportAdos != null && this._ImportAdos.Count > 0)
                {
                    //datas.AddRange(this._ImportAdos);
                    foreach (var item in this._ImportAdos)
                    {

                        HIS_MEDI_STOCK_MATY ado = new HIS_MEDI_STOCK_MATY();
                        ado.MEDI_STOCK_ID = item.MEDI_STOCK_ID;
                        ado.MATERIAL_TYPE_ID = item.MATERIAL_TYPE_ID;
                        ado.ALERT_MIN_IN_STOCK = item.ALERT_MIN_IN_STOCK;
                        ado.ALERT_MAX_IN_STOCK = item.ALERT_MAX_IN_STOCK;
                        ado.IS_PREVENT_MAX = item.IS_PREVENT_MAX;
                        ado.IS_GOODS_RESTRICT = item.IS_GOODS_RESTRICT;
                        //ado.REAL_BASE_AMOUNT = item.REAL_BASE_AMOUNT;
                        ado.EXP_MEDI_STOCK_ID = item.EXP_MEDI_STOCK_ID;
                        var dataCheck = DatacheckTon.FirstOrDefault(o => o.MATERIAL_TYPE_ID == item.MATERIAL_TYPE_ID && o.MEDI_STOCK_ID == item.MEDI_STOCK_ID);
                        if (dataCheck != null)
                        {
                            ado.ID = dataCheck.ID;
                            dataUpdates.Add(ado);
                        }
                        else
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
                if (datas != null && datas.Count > 0)
                {
                    var dataImports = new BackendAdapter(param).Post<List<HIS_MEDI_STOCK_MATY>>("/api/HisMediStockMaty/Import", ApiConsumers.MosConsumer, datas, param);
                    if (dataImports != null && dataImports.Count > 0)
                    {
                        success = true;
                    }
                }
                if (dataUpdates != null && dataUpdates.Count > 0)
                {
                    CommonParam pramUpdate = new CommonParam();
                    var DataUpdates = new BackendAdapter(pramUpdate).Post<List<HIS_MEDI_STOCK_MATY>>("/api/HisMediStockMaty/UpdateList", ApiConsumers.MosConsumer, dataUpdates, pramUpdate);
                    if (DataUpdates != null && DataUpdates.Count > 0)
                    {
                        success = true;

                    }
                    else if ((param.Messages != null && param.Messages.Count > 0)
                       || (param.BugCodes != null && param.BugCodes.Count > 0))
                    {
                        param.Messages.AddRange(pramUpdate.Messages);
                        param.BugCodes.AddRange(pramUpdate.BugCodes);
                    }

                }
                if (this.delegateRefresh != null)
                {
                    this.delegateRefresh();
                }
                btnImport.Enabled = false;
                WaitingManager.Hide();
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
