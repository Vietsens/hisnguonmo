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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.HisServiceConditionImport.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisServiceConditionImport.HisServiceConditionImport
{
    public partial class frmHisServiceConditionImport : HIS.Desktop.Utility.FormBase
    {
        #region declare
        Inventec.Desktop.Common.Modules.Module _Module { get; set; }
        RefeshReference delegateRefresh;
        List<ServiceConditionADO> _ServiceConditionAdos;
        List<ServiceConditionADO> _CurrentAdos;
        List<HIS_SERVICE_CONDITION> _ListServiceConditions { get; set; }
        #endregion

        public frmHisServiceConditionImport()
        {
            InitializeComponent();
        }
        public frmHisServiceConditionImport(Inventec.Desktop.Common.Modules.Module _module)
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

        public frmHisServiceConditionImport(Inventec.Desktop.Common.Modules.Module _module, RefeshReference _delegateRefresh)
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

        private void frmHisServiceConditionImport_Load(object sender, EventArgs e)
        {
            try
            {
                if (this._Module != null)
                {
                    this.Text = this._Module.text;
                }
                SetIcon();
                LoadDataServiceCondition();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataServiceCondition()
        {
            try
            {
                _ListServiceConditions = new List<HIS_SERVICE_CONDITION>();
                MOS.Filter.HisBedFilter filter = new MOS.Filter.HisBedFilter();
                _ListServiceConditions = new BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_CONDITION>>("api/HisServiceCondition/Get", ApiConsumers.MosConsumer, filter, null);
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
                string fileName = System.IO.Path.Combine(Application.StartupPath + "\\Tmp\\Imp\\", "IMPORT_SERVICE_CONDITION.xlsx");
                Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
                param.Messages = new List<string>();
                if (File.Exists(fileName))
                {
                    saveFileDialog.Title = "Save File";
                    saveFileDialog.FileName = "IMPORT_SERVICE_CONDITION";
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
                        var hisServiceImport = import.GetWithCheck<ServiceConditionADO>(0);
                        if (hisServiceImport != null && hisServiceImport.Count > 0)
                        {
                            List<ServiceConditionADO> listAfterRemove = new List<ServiceConditionADO>();


                            foreach (var item in hisServiceImport)
                            {
                                bool checkNull = string.IsNullOrEmpty(item.SERVICE_CONDITION_CODE)
                                    && string.IsNullOrEmpty(item.SERVICE_CONDITION_NAME)
                                    && string.IsNullOrEmpty(item.SERVICE_CODE)
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
                                this._ServiceConditionAdos = new List<ServiceConditionADO>();
                                addServiceToProcessList(_CurrentAdos, ref this._ServiceConditionAdos);
                                SetDataSource(this._ServiceConditionAdos);
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

        private void SetDataSource(List<ServiceConditionADO> dataSource)
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

        private void addServiceToProcessList(List<ServiceConditionADO> _service, ref List<ServiceConditionADO> _serviceRef)
        {
            try
            {
                _serviceRef = new List<ServiceConditionADO>();
                long i = 0;
                foreach (var item in _service)
                {
                    i++;
                    string error = "";
                    var serAdo = new ServiceConditionADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<ServiceConditionADO>(serAdo, item);

                    if (!string.IsNullOrEmpty(item.SERVICE_CONDITION_CODE))
                    {
                        if (item.SERVICE_CONDITION_CODE.Length > 20)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Mã");
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Mã");
                    }

                    if (!string.IsNullOrEmpty(item.SERVICE_CONDITION_CODE))
                    {
                        if (_ListServiceConditions != null)
                        {
                            var serviceConditions = this._ListServiceConditions.FirstOrDefault(p => p.SERVICE_CONDITION_CODE == item.SERVICE_CONDITION_CODE && p.SERVICE_CONDITION_NAME == item.SERVICE_CONDITION_NAME && p.SERVICE_ID == item.SERVICE_ID && p.HEIN_RATIO == item.HEIN_RATIO && p.HEIN_PRICE == item.HEIN_PRICE);
                            if (serviceConditions != null)
                            {
                                error += string.Format(Message.MessageImport.DaTonTai, "Mã");
                            }
                        }

                    }
                    var checkTrung12 = _service.Where(p => p.SERVICE_CONDITION_CODE == item.SERVICE_CONDITION_CODE).ToList();
                    if (checkTrung12 != null && checkTrung12.Count > 1)
                    {
                        error += string.Format(Message.MessageImport.FileImportDaTonTai, item.SERVICE_CONDITION_CODE);
                    }
                    if (!string.IsNullOrEmpty(item.SERVICE_CONDITION_NAME))
                    {
                        var checkLength = Inventec.Common.String.CountVi.Count(item.SERVICE_CONDITION_NAME);
                        //Inventec.Common.Logging.LogSystem.Debug("checkLength " + checkLength);
                        if (checkLength > 1000)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Tên");
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Tên");
                    }
                    if (!string.IsNullOrEmpty(item.SERVICE_CODE))
                    {
                        if (item.SERVICE_CODE.Length > 19)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Mã dịch vụ");
                        }
                        var service = BackendDataWorker.Get<HIS_SERVICE>().FirstOrDefault(p => p.SERVICE_CODE == item.SERVICE_CODE.Trim());
                        if (service != null)
                        {
                            if (service.IS_ACTIVE == 1)
                            {
                                serAdo.SERVICE_ID = service.ID;
                                serAdo.SERVICE_CODE = service.SERVICE_CODE;
                            }
                            else
                            {
                                error += string.Format(Message.MessageImport.MaDichVuDaKhoa, "Mã dịch vụ");
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Mã dịch vụ");
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Mã dịch vụ");
                    }
                    if (!string.IsNullOrEmpty(item.HEIN_PRICE_STR))
                    {
                        decimal price = 0;
                        if (!Decimal.TryParse(item.HEIN_PRICE_STR, NumberStyles.Integer, CultureInfo.InvariantCulture, out price))
                            error += string.Format(Message.MessageImport.KhongHopLe, "Giá trần BHYT");
                        if (price < 0)
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Giá trần BHYT");
                        }
                        else
                        {
                            serAdo.HEIN_PRICE = price;
                        }
                    }
                    if (!string.IsNullOrEmpty(item.HEIN_RATIO_STR))
                    {
                        decimal price = 0;
                        if (!Decimal.TryParse(item.HEIN_RATIO_STR, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out price))
                            error += string.Format(Message.MessageImport.KhongHopLe, "Tỷ lệ thanh toán");
                        var priceLong = (long)(price * 100);

                        if ((price * 100) - priceLong > 0)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Tỷ lệ thanh toán");
                        }
                        if (price > 100 || price < 0)
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Tỷ lệ thanh toán");
                        }
                        else
                        {
                            serAdo.HEIN_RATIO = price / 100;
                        }
                    }
                    serAdo.ERROR = error;
                    serAdo.ID = i;

                    _serviceRef.Add(serAdo);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CheckErrorLine(List<ServiceConditionADO> dataSource)
        {
            try
            {
                var checkError = this._ServiceConditionAdos.Exists(o => !string.IsNullOrEmpty(o.ERROR));
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
                    var errorLine = this._ServiceConditionAdos.Where(o => !string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(errorLine);

                }
                else
                {
                    btnShowLineError.Text = "Dòng lỗi";
                    var errorLine = this._ServiceConditionAdos.Where(o => string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(errorLine);
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
                List<HIS_SERVICE_CONDITION> datas = new List<HIS_SERVICE_CONDITION>();

                if (this._ServiceConditionAdos != null && this._ServiceConditionAdos.Count > 0)
                {
                    foreach (var item in this._ServiceConditionAdos)
                    {
                        HIS_SERVICE_CONDITION ado = new HIS_SERVICE_CONDITION();
                        ado.SERVICE_CONDITION_CODE = item.SERVICE_CONDITION_CODE;
                        ado.SERVICE_CONDITION_NAME = item.SERVICE_CONDITION_NAME;
                        ado.SERVICE_ID = item.SERVICE_ID;
                        ado.HEIN_RATIO = item.HEIN_RATIO;
                        ado.HEIN_PRICE = item.HEIN_PRICE;
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
                var dataImports = new BackendAdapter(param).Post<List<HIS_SERVICE_CONDITION>>("api/HisServiceCondition/CreateList", ApiConsumers.MosConsumer, datas, param);
                WaitingManager.Hide();
                if (dataImports != null && dataImports.Count > 0)
                {
                    success = true;
                    btnImport.Enabled = false;
                    LoadDataServiceCondition();
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

        private void bbtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void gridViewData_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    ServiceConditionADO pData = (ServiceConditionADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    else if (e.Column.FieldName == "HEIN_PRICE_BHYT_STR")
                    {
                        Inventec.Common.Logging.LogSystem.Debug("FieldName HEIN_PRICE_STR ");
                        if (pData != null && string.IsNullOrEmpty(pData.ERROR))
                        {
                            e.Value = Tien(pData.HEIN_PRICE_STR);
                        }
                        else 
                        {
                            e.Value = pData.HEIN_PRICE_STR;
                        }
                    }
                }
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

        private void repositoryItemButton_ER_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var row = (ServiceConditionADO)gridViewData.GetFocusedRow();
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

        private void repositoryEnableDelete_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var row = (ServiceConditionADO)gridViewData.GetFocusedRow();
                if (row != null)
                {
                    if (this._ServiceConditionAdos != null && this._ServiceConditionAdos.Count > 0)
                    {
                        this._ServiceConditionAdos.Remove(row);
                        var dataCheck = this._ServiceConditionAdos.Where(p => p.SERVICE_CONDITION_CODE == row.SERVICE_CONDITION_CODE).ToList();
                        if (dataCheck != null && dataCheck.Count == 1)
                        {
                            if (!string.IsNullOrEmpty(dataCheck[0].ERROR))
                            {
                                string erro = string.Format(Message.MessageImport.FileImportDaTonTai, dataCheck[0].SERVICE_CONDITION_CODE);
                                //string[] Codes = dataCheck[0].ERROR.Split('|');
                                dataCheck[0].ERROR = dataCheck[0].ERROR.Replace(erro, "");
                            }

                        }
                        SetDataSource(this._ServiceConditionAdos);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string Tien(string XXX)
        {
            string KetQua = "";
            int DoDai = XXX.Length;
            for (int i = DoDai - 1; i > -1; i--)
            {
                KetQua = XXX[i] + KetQua;
                if ((DoDai - i == 3 && DoDai > 3) || (DoDai - i == 6 && DoDai > 6))
                    KetQua = "." + KetQua;
            }
            return KetQua;
        }
    }
}


