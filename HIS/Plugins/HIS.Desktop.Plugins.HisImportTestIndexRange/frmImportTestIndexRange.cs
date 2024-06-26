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
using HIS.Desktop.Plugins.HisImportTestIndexRange.ADO;
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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisImportTestIndexRange
{
    public partial class frmImportTestIndexRange : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module _Module { get; set; }
        RefeshReference delegateRefresh;
        List<TestIndexRangeADO> _TestIndexRangeAdos;
        List<TestIndexRangeADO> _CurrentAdos;

        internal List<HIS_TEST_INDEX> _TestIndexs { get; set; }

        public frmImportTestIndexRange()
        {
            InitializeComponent();
        }

        public frmImportTestIndexRange(Inventec.Desktop.Common.Modules.Module _module)
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

        public frmImportTestIndexRange(Inventec.Desktop.Common.Modules.Module _module, RefeshReference _delegateRefresh)
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

        private void frmImportTestIndexRange_Load(object sender, EventArgs e)
        {
            try
            {
                if (this._Module != null)
                {
                    this.Text = this._Module.text;
                }
                SetIcon();
                LoadDataTestIndex();
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

        private void LoadDataTestIndex()
        {
            try
            {
                _TestIndexs = new List<HIS_TEST_INDEX>();
                MOS.Filter.HisTestIndexFilter filter = new MOS.Filter.HisTestIndexFilter();
                _TestIndexs = new BackendAdapter(new CommonParam()).Get<List<HIS_TEST_INDEX>>("api/HisTestIndex/Get", ApiConsumers.MosConsumer, filter, null);
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

                string fileName = System.IO.Path.Combine(Application.StartupPath + "\\Tmp\\Imp\\", "IMPORT_TEST_INDEX_RANGE.xlsx");
                Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
                param.Messages = new List<string>();
                if (File.Exists(fileName))
                {
                    saveFileDialog.Title = "Save File";
                    saveFileDialog.FileName = "IMPORT_TEST_INDEX_RANGE";
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
                        var hisServiceImport = import.GetWithCheck<TestIndexRangeADO>(0);
                        if (hisServiceImport != null && hisServiceImport.Count > 0)
                        {
                            List<TestIndexRangeADO> listAfterRemove = new List<TestIndexRangeADO>();
                            //foreach (var item in hisServiceImport)
                            //{
                            //    listAfterRemove.Add(item);
                            //}

                            foreach (var item in hisServiceImport)
                            {
                                bool checkNull = string.IsNullOrEmpty(item.TEST_INDEX_CODE)
                                    && string.IsNullOrEmpty(item.AGE_FROM_STR)
                                    && string.IsNullOrEmpty(item.AGE_TO_STR)
                                    && string.IsNullOrEmpty(item.IS_FEMALE_STR)
                                    && string.IsNullOrEmpty(item.IS_MALE_STR)
                                    && string.IsNullOrEmpty(item.MAX_VALUE)
                                     && string.IsNullOrEmpty(item.MIN_VALUE)
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
                                this._TestIndexRangeAdos = new List<TestIndexRangeADO>();
                                addServiceToProcessList(_CurrentAdos, ref this._TestIndexRangeAdos);
                                SetDataSource(this._TestIndexRangeAdos);
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

        private void addServiceToProcessList(List<TestIndexRangeADO> _service, ref List<TestIndexRangeADO> _testIndexRangeRef)
        {
            try
            {
                _testIndexRangeRef = new List<TestIndexRangeADO>();
                long i = 0;
                foreach (var item in _service)
                {
                    i++;
                    string error = "";
                    var serAdo = new TestIndexRangeADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<TestIndexRangeADO>(serAdo, item);

                    if (!string.IsNullOrEmpty(item.AGE_FROM_STR))
                    {
                        serAdo.AGE_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(item.AGE_FROM_STR);
                        if (item.AGE_FROM_STR.Length > 19)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Tuổi từ");
                        }
                        else if (serAdo.AGE_FROM <= 0)
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, " '" + "Tuổi từ: " + item.AGE_FROM_STR + "' ");
                        }
                    }
                    if (!string.IsNullOrEmpty(item.AGE_TO_STR))
                    {
                        serAdo.AGE_TO = Inventec.Common.TypeConvert.Parse.ToInt64(item.AGE_TO_STR);
                        if (item.AGE_TO_STR.Trim().Length > 19)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Tuổi đến");
                        }
                        else if (serAdo.AGE_TO <= 0)
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, " '" + "Tuổi đến: " + item.AGE_TO_STR + "' ");
                        }
                    }
                    if (!string.IsNullOrEmpty(item.TEST_INDEX_CODE))
                    {
                        if (item.TEST_INDEX_CODE.Trim().Length > 20)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Mã chỉ số", 20);
                        }
                        else
                        {
                            var testIndex = this._TestIndexs.FirstOrDefault(p => p.TEST_INDEX_CODE.Trim() == item.TEST_INDEX_CODE.Trim());
                            if (testIndex != null)
                            {
                                if (testIndex.IS_ACTIVE == 1)
                                {
                                    serAdo.TEST_INDEX_ID = testIndex.ID;
                                    serAdo.TEST_INDEX_NAME = testIndex.TEST_INDEX_NAME;
                                }
                                else
                                {
                                    error += string.Format(Message.MessageImport.DaKhoa, "Mã chỉ số");
                                }
                            }
                            else
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "Mã chỉ số");
                            }
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Mã chỉ số");
                    }
                    if (!string.IsNullOrEmpty(item.IS_FEMALE_STR))
                    {
                        if (item.IS_FEMALE_STR.Trim() == "x")
                        {
                            serAdo.IS_FEMALE = (short)1;
                            serAdo.IS_FEMALE_B = true;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Nữ");
                        }
                    }
                    if (!string.IsNullOrEmpty(item.IS_MALE_STR))
                    {
                        if (item.IS_MALE_STR.Trim() == "x")
                        {
                            serAdo.IS_MALE = (short)1;
                            serAdo.IS_MALE_B = true;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Nam");
                        }
                    }

                    if (!serAdo.IS_FEMALE_B && !serAdo.IS_MALE_B)
                    {
                        error += "Bắt buộc nhập giới tính|";
                    }


                    if (!string.IsNullOrEmpty(item.VALUE_RANGE_CHECK))
                    {
                        if (item.VALUE_RANGE_CHECK.Trim() == "x")
                        {
                            if (!string.IsNullOrEmpty(item.NORMAL_VALUE))
                            {
                                error += "Dải giá trị không được nhập giá trị|";
                            }
                            if (!string.IsNullOrEmpty(item.MAX_VALUE))
                            {
                                if (item.MAX_VALUE.Trim().Length > 100)
                                {
                                    error += string.Format(Message.MessageImport.Maxlength, "Lớn nhất");
                                }
                                if (!CheckNumber(item.MAX_VALUE.Trim()))
                                {
                                    error += string.Format(Message.MessageImport.KhongHopLe, "Lớn nhất");
                                }
                            }
                            if (!string.IsNullOrEmpty(item.MIN_VALUE))
                            {
                                if (item.MIN_VALUE.Trim().Length > 100)
                                {
                                    error += string.Format(Message.MessageImport.Maxlength, "Nhỏ nhất");
                                }
                                if (!CheckNumber(item.MIN_VALUE.Trim()))
                                {
                                    error += string.Format(Message.MessageImport.KhongHopLe, "Nhỏ nhất");
                                }
                            }
                            if (!string.IsNullOrEmpty(item.IS_ACCEPT_EQUAL_MAX_STR))
                            {
                                if (item.IS_ACCEPT_EQUAL_MAX_STR.Trim() == "x")
                                {
                                    serAdo.IS_ACCEPT_EQUAL_MAX = (short)1;
                                    serAdo.IS_ACCEPT_EQUAL_MAX_B = true;
                                }
                                else
                                {
                                    error += string.Format(Message.MessageImport.KhongHopLe, "Có lấy giá trị = lớn nhất");
                                }
                            }
                            if (!string.IsNullOrEmpty(item.IS_ACCEPT_EQUAL_MIN_STR))
                            {
                                if (item.IS_ACCEPT_EQUAL_MIN_STR.Trim() == "x")
                                {
                                    serAdo.IS_ACCEPT_EQUAL_MIN = (short)1;
                                    serAdo.IS_ACCEPT_EQUAL_MIN_B = true;
                                }
                                else
                                {
                                    error += string.Format(Message.MessageImport.KhongHopLe, "Có lấy giá trị = nhỏ nhất");
                                }
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Dải giá trị");
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(item.MAX_VALUE.Trim()) || !string.IsNullOrEmpty(item.MIN_VALUE.Trim()) || !string.IsNullOrEmpty(item.IS_ACCEPT_EQUAL_MAX_STR.Trim()) || !string.IsNullOrEmpty(item.IS_ACCEPT_EQUAL_MIN_STR.Trim()))
                        {
                            error += "Không được nhập dữ liệu của dải giá trị|";
                        }

                        if (!string.IsNullOrEmpty(item.NORMAL_VALUE))
                        {

                            //serAdo.MAX_VALUE = "";
                            //serAdo.MIN_VALUE = "";
                            //serAdo.IS_ACCEPT_EQUAL_MAX = null;
                            //serAdo.IS_ACCEPT_EQUAL_MAX_B = false;
                            //serAdo.IS_ACCEPT_EQUAL_MAX_STR = "";
                            //serAdo.IS_ACCEPT_EQUAL_MIN = null;
                            //serAdo.IS_ACCEPT_EQUAL_MIN_B = false;
                            //serAdo.IS_ACCEPT_EQUAL_MIN_STR = "";
                            if (item.NORMAL_VALUE.Trim().Length > 100)
                            {
                                error += string.Format(Message.MessageImport.Maxlength, "Giá trị");
                            }
                        }
                    }

                    serAdo.ERROR = error;
                    serAdo.ID = i;

                    _testIndexRangeRef.Add(serAdo);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        public bool CheckNumber(string pText)
        {
            bool check = false;
            try
            {
                Regex regex = new Regex(@"^[-+]?[0-9]*\.?[0-9]+$");
                check = regex.IsMatch(pText);
            }
            catch (Exception ex)
            {
                check = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return check;
        }

        private void SetDataSource(List<TestIndexRangeADO> dataSource)
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

        private void CheckErrorLine(List<TestIndexRangeADO> dataSource)
        {
            try
            {
                var checkError = this._TestIndexRangeAdos.Exists(o => !string.IsNullOrEmpty(o.ERROR));
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
                    var errorLine = this._TestIndexRangeAdos.Where(o => !string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(errorLine);

                }
                else
                {
                    btnShowLineError.Text = "Dòng lỗi";
                    var errorLine = this._TestIndexRangeAdos.Where(o => string.IsNullOrEmpty(o.ERROR)).ToList();
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
                var row = (TestIndexRangeADO)gridViewData.GetFocusedRow();
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
                var row = (TestIndexRangeADO)gridViewData.GetFocusedRow();
                if (row != null)
                {
                    if (this._TestIndexRangeAdos != null && this._TestIndexRangeAdos.Count > 0)
                    {
                        this._TestIndexRangeAdos.Remove(row);
                        SetDataSource(this._TestIndexRangeAdos);
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
                    TestIndexRangeADO pData = (TestIndexRangeADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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
                List<HIS_TEST_INDEX_RANGE> datas = new List<HIS_TEST_INDEX_RANGE>();
                if (this._TestIndexRangeAdos != null && this._TestIndexRangeAdos.Count > 0)
                {
                    foreach (var item in this._TestIndexRangeAdos)
                    {
                        HIS_TEST_INDEX_RANGE ado = new HIS_TEST_INDEX_RANGE();
                        ado = item;
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
                var dataImports = new BackendAdapter(param).Post<List<HIS_TEST_INDEX_RANGE>>("api/HisTestIndexRange/CreateList", ApiConsumers.MosConsumer, datas, param);
                WaitingManager.Hide();
                if (dataImports != null && dataImports.Count > 0)
                {
                    success = true;
                    btnImport.Enabled = false;
                    LoadDataTestIndex();
                    if (this.delegateRefresh != null)
                    {
                        this.delegateRefresh();
                    }
                }


                #region Hien thi message thong bao
                MessageManager.Show(this.ParentForm, param, success);
                #endregion
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
                // DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
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

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
