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
using HIS.Desktop.Common;
using HIS.Desktop.Plugins.ImportHisMedicinePaty.ADO;
using System;
using Inventec.Common.Adapter;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOS.EFMODEL.DataModels;
using Inventec.Core;
using HIS.Desktop.ApiConsumer;
using System.IO;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.BackendData;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;

namespace HIS.Desktop.Plugins.ImportHisMedicinePaty
{
    public partial class frmImportHisMedicinePaty : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module _Module { get; set; }
        RefeshReference delegateRefresh;
        List<MedicinePatyADO> _medicinePatyAdos;
        List<MedicinePatyADO> _CurrentAdos;
        List<HIS_MEDICINE_PATY> _ListMedicinePaty { get; set; }
        int checkButtonErrorLine = 0;

        public frmImportHisMedicinePaty()
        {
            InitializeComponent();
        }
        public frmImportHisMedicinePaty(Inventec.Desktop.Common.Modules.Module _module)
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
        public frmImportHisMedicinePaty(Inventec.Desktop.Common.Modules.Module _module, RefeshReference _delegateRefresh)
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

        private void frmImportHisMedicinePaty_Load(object sender, EventArgs e)
        {
            try
            {
                if (this._Module != null)
                {
                    this.Text = this._Module.text;
                }
                SetIcon();
                LoadDataIcd();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataIcd()
        {
            try
            {
                _ListMedicinePaty = new List<HIS_MEDICINE_PATY>();
                MOS.Filter.HisMedicineFilter filter = new MOS.Filter.HisMedicineFilter();
                _ListMedicinePaty = new BackendAdapter(new CommonParam()).Get<List<HIS_MEDICINE_PATY>>("api/HisMedicinePaty/Get", ApiConsumers.MosConsumer, filter, null);
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

                string fileName = System.IO.Path.Combine(Application.StartupPath + "\\Tmp\\Imp\\", "IMPORT_MEDICINE_PATY.xlsx");
                Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
                param.Messages = new List<string>();
                if (File.Exists(fileName))
                {
                    saveFileDialog.Title = "Save File";
                    saveFileDialog.FileName = "IMPORT_MEDICINE_PATY";
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
                        var medicinePatyImport = import.GetWithCheck<MedicinePatyADO>(0);
                        if (medicinePatyImport != null && medicinePatyImport.Count > 0)
                        {
                            List<MedicinePatyADO> listAfterRemove = new List<MedicinePatyADO>();


                            foreach (var item in medicinePatyImport)
                            {
                                bool checkNull =
                                    string.IsNullOrEmpty(item.MEDICINE_TYPE_CODE)
                                    && string.IsNullOrEmpty(item.PACKAGE_NUMBER)
                                    && item.EXP_PRICE.Equals(null)
                                    && item.EXP_VAT_RATIO.Equals(null)
                                    && string.IsNullOrEmpty(item.PATIENT_TYPE_CODE);

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
                                this._medicinePatyAdos = new List<MedicinePatyADO>();
                                addServiceToProcessList(_CurrentAdos, ref this._medicinePatyAdos);
                                SetDataSource(this._medicinePatyAdos);
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

        private void SetDataSource(List<MedicinePatyADO> dataSource)
        {
            try
            {
                gridControl1.BeginUpdate();
                gridControl1.DataSource = null;
                gridControl1.DataSource = dataSource;
                gridControl1.EndUpdate();
                CheckErrorLine(null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CheckErrorLine(List<MedicinePatyADO> dataSource)
        {
            try
            {
                var checkError = this._medicinePatyAdos.Exists(o => !string.IsNullOrEmpty(o.ERROR));
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

        private void addServiceToProcessList(List<MedicinePatyADO> _listMedicinePaty, ref List<MedicinePatyADO> _medicinePatyRef)
        {
            try
            {
                _medicinePatyRef = new List<MedicinePatyADO>();
                long i = 0;
                foreach (var item in _listMedicinePaty)
                {
                    i++;
                    string error = "";
                    var serAdo = new MedicinePatyADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<MedicinePatyADO>(serAdo, item);

                    var listMedicine = new List<V_HIS_MEDICINE>();
                    MOS.Filter.HisMedicineViewFilter filter = new MOS.Filter.HisMedicineViewFilter();
                    filter.MEDICINE_TYPE_CODE = item.MEDICINE_TYPE_CODE;
                    filter.PACKAGE_NUMBER = item.PACKAGE_NUMBER;
                    filter.SUPPLIER_CODE = item.SUPPLIER_CODE;
                    listMedicine = new BackendAdapter(new CommonParam()).Get<List<V_HIS_MEDICINE>>("api/HisMedicine/GetView", ApiConsumers.MosConsumer, filter, null);

                    if (!string.IsNullOrEmpty(item.MEDICINE_TYPE_CODE))
                    {
                        if (Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(item.MEDICINE_TYPE_CODE, 25))
                        {
                            error += string.Format(Message.MessageMedicinePaty.Maxlength, "Mã loại thuốc");
                        }
                        if (listMedicine != null && listMedicine.Count() > 0)
                        {
                            serAdo.MEDICINE_ID = listMedicine.FirstOrDefault().ID;
                            serAdo.MEDICINE_TYPE_CODE = listMedicine.FirstOrDefault().MEDICINE_TYPE_CODE;
                            serAdo.MEDICINE_TYPE_NAME = listMedicine.FirstOrDefault().MEDICINE_TYPE_NAME;
                        }
                        else
                        {
                            error += string.Format(Message.MessageMedicinePaty.KhongHopLe, "Mã loại thuốc");
                        }
                    }
                    if (!string.IsNullOrEmpty(item.PACKAGE_NUMBER))
                    {
                        if (Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(item.PACKAGE_NUMBER, 100))
                        {
                            error += string.Format(Message.MessageMedicinePaty.Maxlength, "Số lô");
                        }
                        if (listMedicine != null && listMedicine.Count() > 0)
                        {
                            serAdo.PACKAGE_NUMBER = listMedicine.FirstOrDefault().PACKAGE_NUMBER;
                        }
                        else
                        {
                            error += string.Format(Message.MessageMedicinePaty.KhongHopLe, "Số lô");
                        }
                    }
                    if (!string.IsNullOrEmpty(item.SUPPLIER_CODE))
                    {
                        if (Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(item.SUPPLIER_CODE, 10))
                        {
                            error += string.Format(Message.MessageMedicinePaty.Maxlength, "Mã nhà cung cấp");
                        }
                        if (listMedicine != null && listMedicine.Count() > 0)
                        {
                            serAdo.SUPPLIER_NAME = listMedicine.FirstOrDefault().SUPPLIER_NAME;
                        }
                        else
                        {
                            error += string.Format(Message.MessageMedicinePaty.KhongHopLe, "Mã nhà cung cấp");
                        }
                    }

                    var checkTrung12 = _listMedicinePaty.Where(p => p.MEDICINE_TYPE_CODE == item.MEDICINE_TYPE_CODE
                        && p.PACKAGE_NUMBER == item.PACKAGE_NUMBER && p.SUPPLIER_CODE == item.SUPPLIER_CODE
                        && p.EXP_PRICE == item.EXP_PRICE && p.EXP_VAT_RATIO == item.EXP_VAT_RATIO
                        && p.PATIENT_TYPE_CODE == item.PATIENT_TYPE_CODE).ToList();
                    if (checkTrung12 != null && checkTrung12.Count > 1)
                    {
                        error += string.Format(Message.MessageMedicinePaty.FileImportDaTonTai, item.MEDICINE_TYPE_CODE);
                    }

                    if (!string.IsNullOrEmpty(item.EXP_PRICE.ToString()))
                    {
                        serAdo.EXP_PRICE = item.EXP_PRICE;
                    }
                    else
                    {
                        error += string.Format(Message.MessageMedicinePaty.KhongHopLe, "Giá");
                    }
                    if (!string.IsNullOrEmpty(item.EXP_VAT_RATIO.ToString()))
                    {
                        serAdo.EXP_VAT_RATIO = item.EXP_VAT_RATIO;
                    }
                    else
                    {
                        error += string.Format(Message.MessageMedicinePaty.KhongHopLe, "VAT");
                    }
                    if (!string.IsNullOrEmpty(item.PATIENT_TYPE_CODE))
                    {
                        var medicinePatyPatient = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(p => p.PATIENT_TYPE_CODE == item.PATIENT_TYPE_CODE);
                        if (medicinePatyPatient != null)
                        {
                            serAdo.PATIENT_TYPE_ID = medicinePatyPatient.ID;
                            serAdo.PATIENT_TYPE_NAME = medicinePatyPatient.PATIENT_TYPE_NAME;
                        }
                        else
                        {
                            error += string.Format(Message.MessageMedicinePaty.KhongHopLe, "Đối tượng");
                        }
                    }

                    if (serAdo.MEDICINE_ID != 0 && serAdo.PATIENT_TYPE_ID != 0)
                    {
                        var checkMedicinePaty = BackendDataWorker.Get<HIS_MEDICINE_PATY>().FirstOrDefault(p => p.MEDICINE_ID == serAdo.MEDICINE_ID && p.PATIENT_TYPE_ID == serAdo.PATIENT_TYPE_ID);
                        if (checkMedicinePaty != null)
                        {
                            error += string.Format(Message.MessageMedicinePaty.DaTonTai, "Chính sách giá");
                        }
                    }
                    serAdo.ERROR = error;
                    serAdo.ID = i;

                    _medicinePatyRef.Add(serAdo);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MedicinePatyADO pData = (MedicinePatyADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    //else if (e.Column.FieldName == "EXP_PRICE_STR")
                    //{
                    //    e.Value = pData.EXP_PRICE;
                    //}
                    //else if (e.Column.FieldName == "EXP_VAT_RATIO_STR")
                    //{
                    //    e.Value = pData.EXP_VAT_RATIO;
                    //}
                    //else if (e.Column.FieldName == "PATIENT_TYPE_CODE_STR")
                    //{
                    //    e.Value = pData.PATIENT_TYPE_CODE;
                    //}
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnShowLineError_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnShowLineError.Text == "Dòng lỗi")
                {
                    btnShowLineError.Text = "Dòng không lỗi";
                    var errorLine = this._medicinePatyAdos.Where(o => !string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(errorLine);
                    checkButtonErrorLine = 1;

                }
                else
                {
                    btnShowLineError.Text = "Dòng lỗi";
                    var errorLine = this._medicinePatyAdos.Where(o => string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(errorLine);
                    checkButtonErrorLine = 2;
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
                List<HIS_MEDICINE_PATY> datas = new List<HIS_MEDICINE_PATY>();

                if (this._medicinePatyAdos != null && this._medicinePatyAdos.Count > 0)
                {
                    foreach (var item in this._medicinePatyAdos)
                    {
                        HIS_MEDICINE_PATY ado = new HIS_MEDICINE_PATY();
                        ado.MEDICINE_ID = item.MEDICINE_ID;
                        ado.EXP_PRICE = item.EXP_PRICE;
                        ado.EXP_VAT_RATIO = item.EXP_VAT_RATIO / 100;
                        ado.PATIENT_TYPE_ID = item.PATIENT_TYPE_ID;
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
                var dataImports = new BackendAdapter(param).Post<List<HIS_MEDICINE_PATY>>("api/HisMedicinePaty/CreateList", ApiConsumers.MosConsumer, datas, param);
                WaitingManager.Hide();
                if (dataImports != null && dataImports.Count > 0)
                {
                    success = true;
                    btnImport.Enabled = false;
                    LoadDataIcd();
                    BackendDataWorker.Reset<HIS_MEDICINE_PATY>();
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

        private void gridView1_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    //BedADO data = (BedADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    string error = (gridView1.GetRowCellValue(e.RowHandle, "ERROR") ?? "").ToString();
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
                var row = (MedicinePatyADO)gridView1.GetFocusedRow();
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
                var row = (MedicinePatyADO)gridView1.GetFocusedRow();
                if (row != null)
                {
                    if (this._medicinePatyAdos != null && this._medicinePatyAdos.Count > 0)
                    {
                        this._medicinePatyAdos.Remove(row);
                        List<MedicinePatyADO> mp = this._medicinePatyAdos;
                        addServiceToProcessList(mp, ref this._medicinePatyAdos);
                        switch (checkButtonErrorLine)
                        {
                            case 0:
                                {
                                    SetDataSource(this._medicinePatyAdos);
                                    CheckErrorLine();
                                    break;
                                }
                            case 1:
                                {
                                    SetDataSource(this._medicinePatyAdos.Where(o => !string.IsNullOrEmpty(o.ERROR)).ToList());
                                    CheckErrorLine();
                                    break;
                                }
                            case 2:
                                {
                                    SetDataSource(this._medicinePatyAdos.Where(o => string.IsNullOrEmpty(o.ERROR)).ToList()); CheckErrorLine();
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
                var checkError = this._medicinePatyAdos.Exists(o => !string.IsNullOrEmpty(o.ERROR));
                if (!checkError)
                {
                    btnImport.Enabled = true;
                    btnShowLineError.Enabled = false;
                    SetDataSource(this._medicinePatyAdos);
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

    }
}
