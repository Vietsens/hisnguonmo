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
using HIS.Desktop.Plugins.HisCarerCardImport.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisCarerCardImport
{
    public partial class frmHisCarerCardImport : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module _Module { get; set; }
        RefeshReference delegateRefresh;
        List<CareRADO> _CareRADOs;
        List<CareRADO> _CurrentAdos;
        int checkButtonErrorLine = 0;
        List<HIS_CARER_CARD> _ListBeds { get; set; }
        List<HIS_SERVICE> hissv = new List<HIS_SERVICE>();
        public frmHisCarerCardImport()
        {
            InitializeComponent();
        }

        public frmHisCarerCardImport(Inventec.Desktop.Common.Modules.Module _module)
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

        public frmHisCarerCardImport(Inventec.Desktop.Common.Modules.Module _module, RefeshReference _delegateRefresh)
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
                Thread thread = new Thread(() =>
                {
                    LoadDataBed();

                });
                thread.Start();
                Thread thread_ = new Thread(() =>
                {
                    hissv = new List<HIS_SERVICE>();
                    MOS.Filter.HisServiceFilter filter = new MOS.Filter.HisServiceFilter();
                    hissv = new BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE>>("api/HisService/Get", ApiConsumers.MosConsumer, filter, null);
                   // hissv = BackendDataWorker.Get<HIS_SERVICE>().Where(p => p.IS_ACTIVE == 1).ToList();
                });
                thread_.Start();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataBed()
        {
            try
            {
                _ListBeds = new List<HIS_CARER_CARD>();
                MOS.Filter.HisCarerCardFilter filter = new MOS.Filter.HisCarerCardFilter();
                _ListBeds = new BackendAdapter(new CommonParam()).Get<List<HIS_CARER_CARD>>("api/HisCarerCard/Get", ApiConsumers.MosConsumer, filter, null);
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

                string fileName = System.IO.Path.Combine(Application.StartupPath + "\\Tmp\\Imp\\", "IMPORT_CARER_CARD.xlsx");
                Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
                param.Messages = new List<string>();
                if (File.Exists(fileName))
                {
                    saveFileDialog.Title = "Save File";
                    saveFileDialog.FileName = "IMPORT_CARER_CARD";
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
                        var hisServiceImport = import.GetWithCheck<CareRADO>(0);
                        if (hisServiceImport != null && hisServiceImport.Count > 0)
                        {
                            List<CareRADO> listAfterRemove = new List<CareRADO>();


                            foreach (var item in hisServiceImport)
                            {
                                bool checkNull = string.IsNullOrEmpty(item.CARER_CARD_NUMBER)
                                    && string.IsNullOrEmpty(item.SERVICE_CODE);

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
                                this._CareRADOs = new List<CareRADO>();

                                Inventec.Common.Logging.LogSystem.Debug("+++++++++++++++ lần 1 ++++++++++" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this._CareRADOs), this._CareRADOs));
                                addServiceToProcessList(_CurrentAdos, ref this._CareRADOs);
                                Inventec.Common.Logging.LogSystem.Debug("+++++++++++++++ lần 2 ++++++++++" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this._CareRADOs), this._CareRADOs));
                                SetDataSource(this._CareRADOs);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_service"></param>
        /// <param name="_bedRoomRef"></param>
        private void addServiceToProcessList(List<CareRADO> _service, ref List<CareRADO> _bedRoomRef)
        {
            try
            {
                _bedRoomRef = new List<CareRADO>();
                long i = 0;
                foreach (var item in _service)
                {
                    i++;
                    string error = "";
                    var serAdo = new CareRADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<CareRADO>(serAdo, item);

                    if (!string.IsNullOrEmpty(item.CARER_CARD_NUMBER))
                    {
                        if (item.CARER_CARD_NUMBER.Length > 20)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Số thẻ");
                        }
                        serAdo.CARER_CARD_NUMBER = item.CARER_CARD_NUMBER;
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Số thẻ");
                    }

                    if (!string.IsNullOrEmpty(item.SERVICE_CODE))
                    {
                        if (item.SERVICE_CODE.Length > 100)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Mã dịch vụ");
                        }
                        if (hissv.Exists(o => o.SERVICE_CODE == item.SERVICE_CODE) == false)
                        {
                            error += string.Format(Message.MessageImport.CheckSevice, "Mã dịch vụ");
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Mã dịch vụ");
                    }

                    #region Đã tồn tại 
                    //  Đã tồn tại
                    if (!string.IsNullOrEmpty(item.CARER_CARD_NUMBER) && !string.IsNullOrEmpty(item.SERVICE_CODE))
                    {
                        var HisSvcheck = hissv.FirstOrDefault(p => p.SERVICE_CODE == item.SERVICE_CODE);
                        if (HisSvcheck != null)
                        {
                            var bed = this._ListBeds.FirstOrDefault(p => p.CARER_CARD_NUMBER == item.CARER_CARD_NUMBER && p.SERVICE_ID == HisSvcheck.ID);
                            if (bed != null)
                            {
                                error += string.Format(Message.MessageImport.DaTonTai, "Số thẻ");
                            }
                        }

                    }

                    #endregion
                    var checkTrung12 = _service.Where(p => p.CARER_CARD_NUMBER == item.CARER_CARD_NUMBER && p.SERVICE_CODE == item.SERVICE_CODE).ToList();
                    if (checkTrung12 != null && checkTrung12.Count > 1)
                    {
                        error += string.Format(Message.MessageImport.FileImportDaTonTai, item.CARER_CARD_NUMBER);
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

        private void SetDataSource(List<CareRADO> dataSource)
        {
            try
            {
                gridControlData.BeginUpdate();
                gridControlData.DataSource = null;
                gridControlData.DataSource = dataSource;
                gridControlData.EndUpdate();
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CheckErrorLine(List<CareRADO> dataSource)
        {
            try
            {
                var checkError = this._CareRADOs.Exists(o => !string.IsNullOrEmpty(o.ERROR));
                if (!checkError)
                {
                    btnImport.Enabled = true;
                    btnShowLineError.Enabled = false;
                    SetDataSource(this._CareRADOs);
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
                    var errorLine = this._CareRADOs.Where(o => !string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(errorLine);
                    checkButtonErrorLine = 1;

                }
                else
                {
                    btnShowLineError.Text = "Dòng lỗi";
                    var errorLine = this._CareRADOs.Where(o => string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(errorLine);
                    checkButtonErrorLine = 2;
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
                var row = (CareRADO)gridViewData.GetFocusedRow();
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
                var row = (CareRADO)gridViewData.GetFocusedRow();
                if (row != null)
                {
                    if (this._CareRADOs != null && this._CareRADOs.Count > 0)
                    {
                        this._CareRADOs.Remove(row);
                        var dataCheck = this._CareRADOs.Where(p => p.CARER_CARD_NUMBER == row.CARER_CARD_NUMBER && p.SERVICE_CODE == row.SERVICE_CODE).ToList();
                        if (dataCheck != null && dataCheck.Count == 1)
                        {
                            if (!string.IsNullOrEmpty(dataCheck[0].ERROR))
                            {
                                string erro = string.Format(Message.MessageImport.FileImportDaTonTai, dataCheck[0].CARER_CARD_NUMBER);
                                //string[] Codes = dataCheck[0].ERROR.Split('|');
                                dataCheck[0].ERROR = dataCheck[0].ERROR.Replace(erro, "");
                            }

                        }
                        switch (checkButtonErrorLine)
                        {
                            case 0:
                                {
                                    SetDataSource(this._CareRADOs);
                                    CheckErrorLine(null);
                                    break;
                                }
                            case 1:
                                {
                                    SetDataSource(this._CareRADOs.Where(o => !string.IsNullOrEmpty(o.ERROR)).ToList());
                                    CheckErrorLine(null);
                                    break;
                                }
                            case 2:
                                {
                                    SetDataSource(this._CareRADOs.Where(o => string.IsNullOrEmpty(o.ERROR)).ToList());
                                    CheckErrorLine(null);
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

        private void gridViewData_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    CareRADO pData = (CareRADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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
                List<HIS_CARER_CARD> datas = new List<HIS_CARER_CARD>();

                if (this._CareRADOs != null && this._CareRADOs.Count > 0)
                {
                    foreach (var item in this._CareRADOs)
                    {
                        HIS_CARER_CARD ado = new HIS_CARER_CARD();
                        ado.CARER_CARD_NUMBER = item.CARER_CARD_NUMBER;

                        ado.SERVICE_ID = hissv.FirstOrDefault(o => o.SERVICE_CODE == item.SERVICE_CODE).ID;

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
                var dataImports = new BackendAdapter(param).Post<List<HIS_CARER_CARD>>("api/HisCarerCard/CreateList", ApiConsumers.MosConsumer, datas, param);
                WaitingManager.Hide();
                if (dataImports != null && dataImports.Count > 0)
                {
                    success = true;
                    btnImport.Enabled = false;
                    LoadDataBed();
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
                    //CareRADO data = (CareRADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
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
