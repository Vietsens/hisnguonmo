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
using HIS.Desktop.Plugins.HisImportTestIndex.ADO;
using Inventec.Common.Adapter;
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

namespace HIS.Desktop.Plugins.HisImportTestIndex.FormLoad
{
    public partial class frmTestIndex : HIS.Desktop.Utility.FormBase
    {
        List<TestIndexImportADO> testIndexAdos;
        List<TestIndexImportADO> currentAdos;
        List<HIS_TEST_INDEX> listTestIndex;
        List<HIS_TEST_INDEX_UNIT> listTestIndexUnit;
        RefeshReference delegateRefresh;
        bool checkClick;
        Inventec.Desktop.Common.Modules.Module currentModule;

        public frmTestIndex(Inventec.Desktop.Common.Modules.Module module, RefeshReference _delegate)
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

        public frmTestIndex(Inventec.Desktop.Common.Modules.Module module)
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

        private void CheckErrorLine(List<TestIndexImportADO> dataSource)
        {
            try
            {
                if (testIndexAdos != null && testIndexAdos.Count > 0)
                {
                    var checkError = testIndexAdos.Exists(o => !string.IsNullOrEmpty(o.ERROR));
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

        private void SetDataSource(List<TestIndexImportADO> dataSource)
        {
            try
            {
                gridControlTestIndex.BeginUpdate();
                gridControlTestIndex.DataSource = null;
                gridControlTestIndex.DataSource = dataSource;
                gridControlTestIndex.EndUpdate();
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

        private void addTestIndexToProcessList(List<TestIndexImportADO> _testIndex, ref List<TestIndexImportADO> _testIndexRef)
        {
            try
            {
                _testIndexRef = new List<TestIndexImportADO>();
                long i = 0;
                foreach (var item in _testIndex)
                {
                    i++;
                    string error = "";
                    var mateAdo = new TestIndexImportADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<TestIndexImportADO>(mateAdo, item);

                    if (!string.IsNullOrEmpty(item.SERVICE_CODE))
                    {
                        if (item.SERVICE_CODE.Length > 25)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Mã dịch vụ");
                        }
                        var serviceXN = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN).ToList();
                        if (serviceXN != null && serviceXN.Count > 0)
                        {
                            var serviceGet = serviceXN.FirstOrDefault(o => o.SERVICE_CODE == item.SERVICE_CODE);
                            if (serviceGet != null)
                            {
                                mateAdo.TEST_SERVICE_TYPE_ID = serviceGet.ID;
                                mateAdo.SERVICE_NAME = serviceGet.SERVICE_NAME;
                            }
                            else
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "Mã dịch vụ");
                            }
                        }
                        else
                        {
                            error += "Dịch vụ không phải là dịch vụ xét nghiệm";
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Mã dịch vụ");
                    }

                    if (!string.IsNullOrEmpty(item.TEST_INDEX_UNIT_CODE))
                    {
                        if (item.TEST_INDEX_UNIT_CODE.Length > 2)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Mã ĐVT chỉ số xét nghiệm");
                        }
                        if (listTestIndexUnit != null && listTestIndexUnit.Count > 0)
                        {
                            var testUnitGet = listTestIndexUnit.FirstOrDefault(o => o.TEST_INDEX_UNIT_CODE == item.TEST_INDEX_UNIT_CODE);
                            if (testUnitGet != null)
                            {
                                mateAdo.TEST_INDEX_UNIT_ID = testUnitGet.ID;
                                mateAdo.TEST_INDEX_UNIT_NAME = testUnitGet.TEST_INDEX_UNIT_NAME;
                            }
                            else
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "Mã ĐVT chỉ số xét nghiệm");
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(item.TEST_INDEX_CODE))
                    {
                        if (item.TEST_INDEX_CODE.Length > 20)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Mã chỉ số xét nghiệm");
                        }
                        if (listTestIndex != null && listTestIndex.Count > 0)
                        {
                            var check = listTestIndex.Exists(o => o.TEST_INDEX_CODE == item.TEST_INDEX_CODE);
                            if (check)
                            {
                                error += string.Format(Message.MessageImport.DaTonTai, "Mã chỉ số xét nghiệm");
                            }
                        }
                        var checkExel = _testIndexRef.FirstOrDefault(o => o.TEST_INDEX_CODE == item.TEST_INDEX_CODE);
                        if (checkExel != null)
                        {
                            error += string.Format(Message.MessageImport.TonTaiTrungNhauTrongFileImport, "Mã chỉ số xét nghiệm");
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Mã chỉ số xét nghiệm");
                    }

                    if (!string.IsNullOrEmpty(item.TEST_INDEX_NAME))
                    {
                        if (item.TEST_INDEX_NAME.Length > 300)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Tên chỉ số xét nghiệm");
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Tên chỉ số xét nghiệm");
                    }

                    if (!string.IsNullOrEmpty(item.NUM_ORDER_STR))
                    {
                        if (Inventec.Common.Number.Check.IsNumber(item.NUM_ORDER_STR))
                        {
                            mateAdo.NUM_ORDER = Inventec.Common.TypeConvert.Parse.ToInt64(item.NUM_ORDER_STR);
                            if (mateAdo.NUM_ORDER.ToString().Length > 19 || mateAdo.NUM_ORDER < 0)
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "STT");
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "STT");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.DEFAULT_VALUE))
                    {
                        if (item.DEFAULT_VALUE.Length > 100)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Giá trị mặc định");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.BHYT_CODE))
                    {
                        if (item.BHYT_CODE.Length > 20)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Mã BHYT");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.BHYT_NAME))
                    {
                        if (Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(item.BHYT_NAME, 300))
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Tên BHYT");
                        }
                    }

                    mateAdo.ERROR = error;
                    mateAdo.ID = i;

                    _testIndexRef.Add(mateAdo);
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
                AutoMapper.Mapper.CreateMap<TestIndexImportADO, HIS_TEST_INDEX>();
                var data = AutoMapper.Mapper.Map<List<HIS_TEST_INDEX>>(testIndexAdos);
                if (data != null && data.Count > 0)
                {
                    foreach (var item in data)
                    {
                        item.ID = 0;
                    }
                }
                CommonParam param = new CommonParam();
                var rs = new BackendAdapter(param).Post<List<HIS_TEST_INDEX>>("api/HisTestIndex/CreateList", ApiConsumers.MosConsumer, data, param);
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
                var row = (TestIndexImportADO)gridViewTestIndex.GetFocusedRow();
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
                var row = (TestIndexImportADO)gridViewTestIndex.GetFocusedRow();
                if (row != null)
                {
                    if (testIndexAdos != null && testIndexAdos.Count > 0)
                    {
                        testIndexAdos.Remove(row);
                        gridControlTestIndex.DataSource = null;
                        gridControlTestIndex.DataSource = testIndexAdos;
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
                    var errorLine = testIndexAdos.Where(o => !string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(errorLine);

                }
                else
                {
                    btnShowLineError.Text = "Dòng lỗi";
                    var errorLine = testIndexAdos.Where(o => string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(errorLine);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridViewTestIndex_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    TestIndexImportADO data = (TestIndexImportADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
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

        private void gridViewTestIndex_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    TestIndexImportADO pData = (TestIndexImportADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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
                    //            e.Value = Inventec.Common.Resource.Get.Value("frmHisMisuTestIndexType.HoatDong", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    //        else
                    //            e.Value = Inventec.Common.Resource.Get.Value("frmHisMisuTestIndexType.TamKhoa", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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
                listTestIndex = new List<HIS_TEST_INDEX>();
                listTestIndexUnit = new List<HIS_TEST_INDEX_UNIT>();
                CommonParam param = new CommonParam();

                HisTestIndexFilter testIndexFilter = new HisTestIndexFilter();
                listTestIndex = new BackendAdapter(param).Get<List<HIS_TEST_INDEX>>("api/HisTestIndex/Get", ApiConsumers.MosConsumer, testIndexFilter, param);

                HisTestIndexUnitFilter testIndexUnitFilter = new HisTestIndexUnitFilter();
                listTestIndexUnit = new BackendAdapter(param).Get<List<HIS_TEST_INDEX_UNIT>>("api/HisTestIndexUnit/Get", ApiConsumers.MosConsumer, testIndexUnitFilter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void frmTestIndex_Load(object sender, EventArgs e)
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
                + "/Tmp/Imp", "IMPORT_TEST_INDEX.xlsx");

                if (File.Exists(source))
                {
                    SaveFileDialog saveFileDialog1 = new SaveFileDialog();

                    saveFileDialog1.Title = "Save File";
                    saveFileDialog1.FileName = "IMPORT_TEST_INDEX";
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
                        var hisTestIndexImport = import.GetWithCheck<TestIndexImportADO>(0);
                        if (hisTestIndexImport != null && hisTestIndexImport.Count > 0)
                        {
                            List<TestIndexImportADO> listAfterRemove = new List<TestIndexImportADO>();
                            foreach (var item in hisTestIndexImport)
                            {
                                listAfterRemove.Add(item);
                            }

                            foreach (var item in hisTestIndexImport)
                            {
                                bool checkNull = string.IsNullOrEmpty(item.SERVICE_CODE)
                                    && string.IsNullOrEmpty(item.TEST_INDEX_UNIT_CODE)
                                    && string.IsNullOrEmpty(item.TEST_INDEX_NAME)
                                    && string.IsNullOrEmpty(item.NUM_ORDER_STR)
                                    && string.IsNullOrEmpty(item.DEFAULT_VALUE)
                                    && string.IsNullOrEmpty(item.BHYT_CODE)
                                    && string.IsNullOrEmpty(item.BHYT_NAME)
                                    && string.IsNullOrEmpty(item.TEST_INDEX_CODE);

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
                                testIndexAdos = new List<TestIndexImportADO>();
                                addTestIndexToProcessList(currentAdos, ref testIndexAdos);
                                SetDataSource(testIndexAdos);
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
