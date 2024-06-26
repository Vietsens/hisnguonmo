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
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.HisImportTestIndexMap.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using LIS.EFMODEL.DataModels;
using LIS.Filter;
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

namespace HIS.Desktop.Plugins.HisImportTestIndexMap.FormLoad
{
    public partial class frmTestIndexMap : HIS.Desktop.Utility.FormBase
    {
        List<TestIndexMapImportADO> testIndexMapAdos;
        List<TestIndexMapImportADO> currentAdos;
        List<V_LIS_TEST_INDEX_MAP> ListTestIndexMap;
        List<LIS_MACHINE_INDEX> ListMachineIndex;
        List<LIS_MACHINE> lisMachine;
        RefeshReference delegateRefresh;
        bool checkClick;
        Inventec.Desktop.Common.Modules.Module currentModule;

        public frmTestIndexMap(Inventec.Desktop.Common.Modules.Module module, RefeshReference _delegate)
            : base(module)
        {
            InitializeComponent();
            try
            {
                this.currentModule = module;
                this.delegateRefresh = _delegate;

                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public frmTestIndexMap(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
            try
            {
                this.currentModule = module;

                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CheckErrorLine(List<TestIndexMapImportADO> dataSource)
        {
            try
            {
                if (testIndexMapAdos != null && testIndexMapAdos.Count > 0)
                {
                    var checkError = testIndexMapAdos.Exists(o => !string.IsNullOrEmpty(o.ERROR));
                    if (!checkError)
                    {
                        btnSave.Enabled = true;
                    }
                    else
                    {
                        btnSave.Enabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void SetDataSource(List<TestIndexMapImportADO> dataSource)
        {
            try
            {
                gridControlTestIndexMap.BeginUpdate();
                gridControlTestIndexMap.DataSource = null;
                gridControlTestIndexMap.DataSource = dataSource;
                gridControlTestIndexMap.EndUpdate();
                CheckErrorLine(null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void convertDateStringToTimeNumber(string date, ref long? dateTime, ref string check)
        {
            try
            {
                if (date.Length > 14)
                {
                    check = Message.MessageImport.Maxlength;
                    return;
                }

                if (date.Length < 14)
                {
                    check = Message.MessageImport.KhongHopLe;
                    return;
                }

                if (date.Length > 0)
                {
                    if (!Inventec.Common.DateTime.Check.IsValidTime(date))
                    {
                        check = Message.MessageImport.KhongHopLe;
                        return;
                    }
                    dateTime = Inventec.Common.TypeConvert.Parse.ToInt64(date);
                }




                //string[] substring = date.Split('/');
                //if (substring != null)
                //{
                //    if (substring.Count() != 3)
                //    {
                //        check = false;
                //        return;
                //    }
                //    if (Inventec.Common.TypeConvert.Parse.ToInt64(substring[0]) < 0 || Inventec.Common.TypeConvert.Parse.ToInt64(substring[0]) > 31)
                //    {
                //        check = false;
                //        return;
                //    }
                //    if (Inventec.Common.TypeConvert.Parse.ToInt64(substring[1]) < 0 || Inventec.Common.TypeConvert.Parse.ToInt64(substring[1]) > 12)
                //    {
                //        check = false;
                //        return;
                //    }
                //    if (Inventec.Common.TypeConvert.Parse.ToInt64(substring[2]) < 0 || Inventec.Common.TypeConvert.Parse.ToInt64(substring[2]) > 9999)
                //    {
                //        check = false;
                //        return;
                //    }
                //}
                //string dateString = substring[2] + substring[1] + substring[0] + "000000";
                //dateTime = Inventec.Common.TypeConvert.Parse.ToInt64(dateString);

                ////date.Replace(" ", "");
                ////int idx = date.LastIndexOf("/");
                ////string year = date.Substring(idx + 1);
                ////string monthdate = date.Substring(0, idx);
                ////idx = monthdate.LastIndexOf("/");
                ////monthdate.Remove(idx);
                ////idx = monthdate.LastIndexOf("/");
                ////string month = monthdate.Substring(idx + 1);
                ////string dateStr = monthdate.Substring(0, idx);
                ////if (month.Length < 2)
                ////{
                ////    month = "0" + month;
                ////}
                ////if (dateStr.Length < 2)
                ////{
                ////    dateStr = "0" + dateStr;
                ////}
                ////datetime = year + month + dateStr;
                ////datetime.Replace("/", "");

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void addTestIndexMapToProcessList(List<TestIndexMapImportADO> _machine, ref List<TestIndexMapImportADO> _machineRef)
        {
            try
            {
                _machineRef = new List<TestIndexMapImportADO>();
                long i = 0;
                foreach (var item in _machine)
                {
                    i++;
                    string error = "";
                    var mateAdo = new TestIndexMapImportADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<TestIndexMapImportADO>(mateAdo, item);

                    if (!string.IsNullOrEmpty(item.MACHINE_CODE))
                    {
                        if (item.MACHINE_CODE.Length > 10)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Mã máy", 10);
                        }

                        if (!string.IsNullOrEmpty(item.MACHINE_INDEX_CODE))
                        {
                            if (item.MACHINE_INDEX_CODE.Length > 20)
                            {
                                error += string.Format(Message.MessageImport.Maxlength, "Mã chỉ số máy", 20);
                            }

                            if (lisMachine != null && lisMachine.Count > 0)
                            {
                                var machineGet = lisMachine.FirstOrDefault(o => o.MACHINE_CODE == item.MACHINE_CODE);
                                if (machineGet != null)
                                {
                                    if (ListMachineIndex != null && ListMachineIndex.Count > 0)
                                    {

                                        var machineIndexGet = ListMachineIndex.FirstOrDefault(o => o.MACHINE_INDEX_CODE == item.MACHINE_INDEX_CODE && o.MACHINE_ID == machineGet.ID);
                                        if (machineIndexGet != null)
                                        {
                                            mateAdo.MACHINE_INDEX_ID = machineIndexGet.ID;
                                            mateAdo.MACHINE_INDEX_NAME = machineIndexGet.MACHINE_INDEX_NAME;
                                        }
                                        else
                                        {
                                            error += string.Format(Message.MessageImport.KhongHopLe, "Mã chỉ số máy và Mã máy");
                                        }
                                    }
                                }
                                else
                                {
                                    error += string.Format(Message.MessageImport.KhongHopLe, "Mã máy");
                                }
                            }
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(item.MACHINE_INDEX_CODE))
                        {
                            if (item.MACHINE_INDEX_CODE.Length > 20)
                            {
                                error += string.Format(Message.MessageImport.Maxlength, "Mã chỉ số máy", 20);
                            }
                            if (ListMachineIndex != null && ListMachineIndex.Count > 0)
                            {
                                var machineIndexGet = ListMachineIndex.FirstOrDefault(o => o.MACHINE_INDEX_CODE == item.MACHINE_INDEX_CODE);
                                if (machineIndexGet != null)
                                {
                                    mateAdo.MACHINE_INDEX_ID = machineIndexGet.ID;
                                    mateAdo.MACHINE_INDEX_NAME = machineIndexGet.MACHINE_INDEX_NAME;
                                }
                                else
                                {
                                    error += string.Format(Message.MessageImport.KhongHopLe, "Mã chỉ số máy");
                                }
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(item.TEST_INDEX_CODE))
                    {
                        if (item.TEST_INDEX_CODE.Length > 20)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Mã ánh xạ",20);
                        }
                       
                        if (!string.IsNullOrEmpty(item.MACHINE_CODE))
                        {
                            if (ListTestIndexMap != null && ListTestIndexMap.Count > 0)
                            {
                                var check = ListTestIndexMap.Exists(o => o.TEST_INDEX_CODE == item.TEST_INDEX_CODE && o.MACHINE_INDEX_CODE == item.MACHINE_INDEX_CODE && o.MACHINE_CODE == item.MACHINE_CODE);
                                if (check)
                                {
                                    error += string.Format(Message.MessageImport.DaTonTai, "Tên ánh xạ");
                                }
                            }

                            var checkExel = _machineRef.FirstOrDefault(o => o.TEST_INDEX_CODE == item.TEST_INDEX_CODE && o.MACHINE_CODE == item.MACHINE_CODE && o.MACHINE_INDEX_CODE == item.MACHINE_INDEX_CODE);
                            if (checkExel != null)
                            {
                                error += string.Format(Message.MessageImport.TonTaiTrungNhauTrongFileImport, "Mã ánh xạ");
                            }
                        }
                        else
                        {
                            if (ListTestIndexMap != null && ListTestIndexMap.Count > 0)
                            {
                                var check = ListTestIndexMap.Exists(o => o.TEST_INDEX_CODE == item.TEST_INDEX_CODE && o.MACHINE_INDEX_CODE == item.MACHINE_INDEX_CODE);
                                if (check)
                                {
                                    error += string.Format(Message.MessageImport.DaTonTai, "Tên ánh xạ");
                                }
                            }

                            var checkExel = _machineRef.FirstOrDefault(o => o.TEST_INDEX_CODE == item.TEST_INDEX_CODE && o.MACHINE_INDEX_CODE == item.MACHINE_INDEX_CODE);
                            if (checkExel != null)
                            {
                                error += string.Format(Message.MessageImport.TonTaiTrungNhauTrongFileImport, "Mã ánh xạ");
                            }
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Mã ánh xạ");
                    }

                    if (!string.IsNullOrEmpty(item.TEST_INDEX_NAME))
                    {
                        if (item.TEST_INDEX_NAME.Length > 200)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Tên ánh xạ",200);
                        }
                    }

                    if (!string.IsNullOrEmpty(item.TEST_INDEX_UNIT_SYMBOL))
                    {
                        if (item.TEST_INDEX_UNIT_SYMBOL.Length > 10)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Ký hiệu",10);
                        }
                    }

                    mateAdo.ERROR = error;
                    mateAdo.ID = i;

                    _machineRef.Add(mateAdo);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void CheckHour(string input, ref string check)
        {
            try
            {
                if (input.Length > 4)
                {
                    check = Message.MessageImport.KhongHopLe;
                    return;
                }
                else
                {
                    if (checkHour(input))
                    {
                        check = Message.MessageImport.KhongHopLe;
                        return;
                    }
                    else
                    {
                        var gio = input.Substring(0, 2);
                        var phut = input.Substring(2, 2);

                        if (Inventec.Common.TypeConvert.Parse.ToInt32(gio) > 24 || Inventec.Common.TypeConvert.Parse.ToInt32(gio) < 0)
                        {
                            check = Message.MessageImport.KhongHopLe;
                            return;
                        }
                        if (Inventec.Common.TypeConvert.Parse.ToInt32(phut) > 60 || Inventec.Common.TypeConvert.Parse.ToInt32(phut) < 0 || Inventec.Common.TypeConvert.Parse.ToInt32(phut) % 5 != 0)
                        {
                            check = Message.MessageImport.KhongHopLe;
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private bool checkDigit(string s)
        {
            bool result = false;
            try
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (!char.IsNumber(s[i]))
                        return result = true;
                }
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return result;
            }
        }

        private bool checkNumber(string s)
        {
            bool result = false;
            try
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (!char.IsNumber(s[i]) && s[i] != ',')
                        return result = true;
                }
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return result;
            }
        }

        private bool checkHour(string s)
        {
            bool result = false;
            try
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (!char.IsNumber(s[i]))
                        return result = true;
                }
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return result;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                bool success = false;
                WaitingManager.Show();
                AutoMapper.Mapper.CreateMap<TestIndexMapImportADO, LIS_TEST_INDEX_MAP>();
                var data = AutoMapper.Mapper.Map<List<LIS_TEST_INDEX_MAP>>(testIndexMapAdos);
                if (data != null && data.Count > 0)
                {
                    foreach (var item in data)
                    {
                        item.ID = 0;
                    }
                }
                CommonParam param = new CommonParam();
                var rs = new BackendAdapter(param).Post<List<LIS_TEST_INDEX_MAP>>("api/LisTestIndexMap/CreateList", ApiConsumers.LisConsumer, data, param);
                if (rs != null)
                {
                    success = true;
                    btnSave.Enabled = false;
                    GetData();
                    if (this.delegateRefresh != null)
                    {
                        this.delegateRefresh();
                    }
                }
                WaitingManager.Hide();
                #region Hien thi message thong bao
                MessageManager.Show(this.ParentForm, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void Btn_Show_Error_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var row = (TestIndexMapImportADO)gridViewTestIndexMap.GetFocusedRow();
                if (row != null)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(row.ERROR);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void Btn_Delete_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var row = (TestIndexMapImportADO)gridViewTestIndexMap.GetFocusedRow();
                if (row != null)
                {
                    if (testIndexMapAdos != null && testIndexMapAdos.Count > 0)
                    {
                        testIndexMapAdos.Remove(row);

                        gridControlTestIndexMap.DataSource = null;
                        gridControlTestIndexMap.DataSource = testIndexMapAdos;
                        CheckErrorLine(null);
                        if (checkClick)
                        {
                            if (btnShowLineError.Text == "Dòng lỗi")
                            {
                                btnShowLineError.Text = "Dòng không lỗi";
                            }
                            else
                            {
                                btnShowLineError.Text = "Dòng lỗi";
                            }
                            btnShowLineError_Click(null, null);
                        }
                        //btnShowLineError_Click(null, null);
                    }
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
                checkClick = true;
                if (btnShowLineError.Text == "Dòng lỗi")
                {
                    btnShowLineError.Text = "Dòng không lỗi";
                    var errorLine = testIndexMapAdos.Where(o => !string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(errorLine);

                }
                else
                {
                    btnShowLineError.Text = "Dòng lỗi";
                    var errorLine = testIndexMapAdos.Where(o => string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(errorLine);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridViewTestIndexMap_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    TestIndexMapImportADO data = (TestIndexMapImportADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "ErrorLine")
                    {
                        if (!string.IsNullOrEmpty(data.ERROR))
                        {
                            e.RepositoryItem = Btn_ErrorLine;
                        }
                    }
                    else if (e.Column.FieldName == "Delete")
                    {
                        e.RepositoryItem = Btn_Delete;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridViewTestIndexMap_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    TestIndexMapImportADO pData = (TestIndexMapImportADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(pData.CREATE_TIME.ToString()));

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao CREATE_TIME", ex);
                        }
                    }
                    //else if (e.Column.FieldName == "ACTIVE_ITEM")
                    //{
                    //    try
                    //    {
                    //        if (status == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    //            e.Value = Inventec.Common.Resource.Get.Value("frmHisMisuTestIndexMapType.HoatDong", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    //        else
                    //            e.Value = Inventec.Common.Resource.Get.Value("frmHisMisuTestIndexMapType.TamKhoa", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        Inventec.Common.Logging.LogSystem.Error(ex);
                    //    }
                    //}
                    else if (e.Column.FieldName == "MODIFY_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(pData.MODIFY_TIME.ToString()));

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua MODIFY_TIME", ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetData()
        {
            try
            {
                ListTestIndexMap = new List<V_LIS_TEST_INDEX_MAP>();
                ListMachineIndex = new List<LIS_MACHINE_INDEX>();
                lisMachine = new List<LIS_MACHINE>();
                CommonParam param = new CommonParam();

                LisTestIndexMapViewFilter testIndexMapFilter = new LisTestIndexMapViewFilter();
                ListTestIndexMap = new BackendAdapter(param).Get<List<V_LIS_TEST_INDEX_MAP>>("api/LisTestIndexMap/GetView", ApiConsumers.LisConsumer, testIndexMapFilter, param);

                LisMachineIndexFilter machineIndexFilter = new LisMachineIndexFilter();
                ListMachineIndex = new BackendAdapter(param).Get<List<LIS_MACHINE_INDEX>>("api/LisMachineIndex/Get", ApiConsumers.LisConsumer, machineIndexFilter, param);

                LisMachineFilter machineFilter = new LisMachineFilter();
                lisMachine = new BackendAdapter(param).Get<List<LIS_MACHINE>>("api/LisMachine/Get", ApiConsumers.LisConsumer, machineFilter, param);
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void frmTestIndexMap_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }
                btnSave.Enabled = false;
                btnShowLineError.Enabled = false;
                GetData();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            try
            {
                var source = System.IO.Path.Combine(Application.StartupPath
                + "/Tmp/Imp", "IMPORT_TEST_INDEX_MAP.xlsx");

                if (File.Exists(source))
                {
                    SaveFileDialog saveFileDialog1 = new SaveFileDialog();

                    saveFileDialog1.Title = "Save File";
                    saveFileDialog1.FileName = "IMPORT_TEST_INDEX_MAP";
                    saveFileDialog1.DefaultExt = "xlsx";
                    saveFileDialog1.Filter = "Excel files (*.xlsx)|All files (*.*)";
                    saveFileDialog1.FilterIndex = 2;
                    saveFileDialog1.RestoreDirectory = true;

                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        File.Copy(source, saveFileDialog1.FileName);
                        DevExpress.XtraEditors.XtraMessageBox.Show("Tải file thành công");
                    }
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Không tìm thấy file import");
                }
            }
            catch (Exception ex)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show("Tải file thất bại");
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                //WaitingManager.Show();
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Multiselect = false;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    WaitingManager.Show();

                    var import = new Inventec.Common.ExcelImport.Import();
                    if (import.ReadFileExcel(ofd.FileName))
                    {
                        var hisTestIndexMapImport = import.GetWithCheck<TestIndexMapImportADO>(0);
                        if (hisTestIndexMapImport != null && hisTestIndexMapImport.Count > 0)
                        {
                            List<TestIndexMapImportADO> listAfterRemove = new List<TestIndexMapImportADO>();
                            foreach (var item in hisTestIndexMapImport)
                            {
                                listAfterRemove.Add(item);
                            }

                            foreach (var item in hisTestIndexMapImport)
                            {
                                bool checkNull = string.IsNullOrEmpty(item.MACHINE_INDEX_CODE)
                                    && string.IsNullOrEmpty(item.TEST_INDEX_CODE)
                                    && string.IsNullOrEmpty(item.TEST_INDEX_NAME)
                                    && string.IsNullOrEmpty(item.TEST_INDEX_UNIT_SYMBOL)
                                    && string.IsNullOrEmpty(item.MACHINE_CODE);
                                if (checkNull)
                                {
                                    listAfterRemove.Remove(item);
                                }
                            }

                            WaitingManager.Hide();

                            this.currentAdos = listAfterRemove;

                            if (this.currentAdos != null && this.currentAdos.Count > 0)
                            {
                                checkClick = false;
                                btnSave.Enabled = true;
                                btnShowLineError.Enabled = true;
                                testIndexMapAdos = new List<TestIndexMapImportADO>();
                                addTestIndexMapToProcessList(currentAdos, ref testIndexMapAdos);
                                SetDataSource(testIndexMapAdos);
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
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnSave.Enabled)
                {
                    btnSave_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
    }
}
