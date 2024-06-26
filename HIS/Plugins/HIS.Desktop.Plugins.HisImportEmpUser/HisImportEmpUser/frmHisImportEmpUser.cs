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
using DevExpress.Data;
using DevExpress.XtraBars;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.HisImportEmpUser;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using ACS.EFMODEL.DataModels;
using SDA.EFMODEL.DataModels;
using HIS.Desktop.Plugins.HisImportEmpUser.ADO;
using System.Collections;
using System.Text.RegularExpressions;
using MOS.Filter;
using SDA.Filter;


namespace HIS.Desktop.Plugins.HisImportEmpUser.HisImportEmpUser
{
    public partial class frmHisImportEmpUser : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module _Module { get; set; }
        HIS.Desktop.Common.RefeshReference delegateRefresh;
        List<HIS_EMPLOYEE> ListEmp { get; set; }
        List<ACS_USER> ListUser { get; set; }
        List<EmpUserADO> EmpUserAdos;
        List<EmpUserADO> CurrentAdos;
        //List<SDA_GROUP> ListSdaGroups { get; set; }
        //List<HIS_DEPARTMENT> ListHisDepartment { get; set; }

        public frmHisImportEmpUser()
        {
            InitializeComponent();
        }
        public frmHisImportEmpUser(Inventec.Desktop.Common.Modules.Module _module)
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
        public frmHisImportEmpUser(Inventec.Desktop.Common.Modules.Module _module, RefeshReference _delegateRefresh)
            : base(_module)
        {
            InitializeComponent();
            try
            {
                this._Module = _module;
                this.delegateRefresh = _delegateRefresh;
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
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

        private void frmHisImportEmpUser_Load(object sender, EventArgs e)
        {
            try
            {
                if (this._Module != null)
                {
                    this.Text = this._Module.text;
                }
                SetIcon();
                GetData();
                //LoadCurrentDataEmployee();
                //LoadCurrentDataUser();
                //LoadCurrentDataDepartment();
                //LoadCurrentSdaGroup();

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void GetData()
        {
            ListUser = BackendDataWorker.Get<ACS_USER>();
            ListEmp = BackendDataWorker.Get<HIS_EMPLOYEE>();
            //LoadCurrentDataEmployee();
            //LoadCurrentDataUser();
        }
        private void LoadCurrentDataUser()
        {
            try
            {
                ListUser = new List<ACS_USER>();
                ACS.Filter.AcsUserFilter filter = new ACS.Filter.AcsUserFilter();
                ListUser = new BackendAdapter(new CommonParam()).Get<List<ACS_USER>>("api/AcsUser/Get", ApiConsumers.AcsConsumer, filter, null);

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadCurrentDataEmployee()
        {
            try
            {
                ListEmp = new List<HIS_EMPLOYEE>();
                MOS.Filter.HisEmployeeFilter filter = new MOS.Filter.HisEmployeeFilter();
                ListEmp = new BackendAdapter(new CommonParam()).Get<List<HIS_EMPLOYEE>>("api/HisEmployee/Get", ApiConsumers.MosConsumer, filter, null);
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
                string fileName = System.IO.Path.Combine(Application.StartupPath + "\\Tmp\\Imp\\", "IMPORT_ACS_USER.xlsx");
                CommonParam param = new CommonParam();
                param.Messages = new List<string>();
                if (System.IO.File.Exists(fileName))
                {
                    saveFileDialog1.Title = "Save File";
                    saveFileDialog1.FileName = "IMPORT_ASC_USER";
                    saveFileDialog1.DefaultExt = "xlsx";
                    saveFileDialog1.Filter = "Excel files (*.xlsx)|All files (*.*)";
                    saveFileDialog1.FilterIndex = 2;
                    saveFileDialog1.RestoreDirectory = true;

                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        System.IO.File.Copy(fileName, saveFileDialog1.FileName);
                        MessageManager.Show(this.ParentForm, param, true);
                        if (DevExpress.XtraEditors.XtraMessageBox.Show("Bạn có muốn mở file ngay ?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
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
                        var hisServiceImport = import.GetWithCheck<EmpUserADO>(0);
                        if (hisServiceImport != null && hisServiceImport.Count > 0)
                        {
                            List<EmpUserADO> listAfterRemove = new List<EmpUserADO>();
                            foreach (var item in hisServiceImport)
                            {
                                bool checkNull = string.IsNullOrEmpty(item.LOGINNAME);
                                if (!checkNull)
                                {
                                    listAfterRemove.Add(item);
                                }
                            }
                            WaitingManager.Hide();
                            this.CurrentAdos = listAfterRemove;
                            if (this.CurrentAdos != null && this.CurrentAdos.Count > 0)
                            {
                                btnShowLineError.Enabled = true;
                                this.EmpUserAdos = new List<EmpUserADO>();
                                addServiceToProcessList(CurrentAdos, ref this.EmpUserAdos);
                                SetDataSource(this.EmpUserAdos);

                            }
                        }
                        else
                        {
                            WaitingManager.Hide();
                            DevExpress.XtraEditors.XtraMessageBox.Show(" Import thất bại");

                        }
                    }
                    else
                    {

                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show("Không dọc được file");

                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);

            }
        }
        private void checkErrorLine(List<EmpUserADO> dataSource)
        {
            try
            {
                var checkerror = this.EmpUserAdos.Exists(o => !string.IsNullOrEmpty(o.ERROR));
                if (!checkerror)
                {
                    btnSave.Enabled = true;
                    btnShowLineError.Enabled = false;

                }
                else
                {
                    btnShowLineError.Enabled = true;
                    btnSave.Enabled = false;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void SetDataSource(List<EmpUserADO> dataSource)
        {
            gridControl1.BeginUpdate();
            gridControl1.DataSource = null;
            gridControl1.DataSource = dataSource;
            gridControl1.EndUpdate();
            checkErrorLine(null);

        }
        public bool IsNumber(string pText)
        {
            Regex regex = null;
            try
            {
                regex = new Regex(@"^[-+]?[0-9]*\.?[0-9]+$"); return regex.IsMatch(pText);
            }
            catch (Exception ex)
            {
                return regex.IsMatch(pText);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void addServiceToProcessList(List<EmpUserADO> service, ref List<EmpUserADO> empUserRef)
        {
            try
            {
                empUserRef = new List<EmpUserADO>();
                long i = 0;
                foreach (var item in service)
                {
                    i++;
                    string error = "";
                    var serAdo = new EmpUserADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<EmpUserADO>(serAdo, item);
                    if (!string.IsNullOrEmpty(item.LOGINNAME))
                    {
                        if (item.LOGINNAME.Length > 50)
                        {
                            error += string.Format(Message.MessageImport.MaxLength, "Tên đăng nhập ");
                        }
                        else
                        {
                            var user = this.ListUser.FirstOrDefault(p => p.LOGINNAME == item.LOGINNAME);
                            if (user != null)
                            {
                                error += string.Format(Message.MessageImport.DaTonTai, "Tên đăng nhập");

                            }
                            else
                            {
                                var checkTrung = service.Where(p => p.LOGINNAME == item.LOGINNAME).ToList();
                                if (checkTrung != null && checkTrung.Count > 1)
                                {
                                    error += string.Format(Message.MessageImport.FileImportDaTonTai, item.LOGINNAME);

                                }

                            }
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDl, "Tên đăng nhập");

                    }
                    if (!string.IsNullOrEmpty(item.USERNAME))
                    {
                        if (Encoding.UTF8.GetByteCount(item.USERNAME.Trim()) > 100)
                        {
                            error += string.Format(Message.MessageImport.MaxLength, "Họ tên");
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDl, "Họ tên");
                    }
                    if (!string.IsNullOrEmpty(item.EMAIL))
                    {
                        if (Encoding.UTF8.GetByteCount(item.EMAIL.Trim()) > 100)
                        {
                            error += string.Format(Message.MessageImport.MaxLength, "Email");
                        }
                    }
                    if (!string.IsNullOrEmpty(item.MOBILE))
                    {
                        if (Encoding.UTF8.GetByteCount(item.MOBILE.Trim()) > 20)
                        {
                            error += string.Format(Message.MessageImport.MaxLength, "Số điện thoại");
                        }
                    }
                    if (!string.IsNullOrEmpty(item.G_CODE))
                    {
                        if (Encoding.UTF8.GetByteCount(item.G_CODE.Trim()) > 20)
                        {
                            error += string.Format(Message.MessageImport.MaxLength, "Mã đơn vị");

                        }
                        else
                        {

                            var check = BackendDataWorker.Get<SDA_GROUP>().FirstOrDefault(p => p.G_CODE == item.G_CODE);
                            if (check != null)
                            { }
                            else error += string.Format(Message.MessageImport.KhongHopLe, "Mã đơn vị");

                        }
                    }
                    if (!string.IsNullOrEmpty(item.DIPLOMA))
                    {
                        if (Encoding.UTF8.GetByteCount(item.DIPLOMA.Trim()) > 50)
                        {
                            error += string.Format(Message.MessageImport.MaxLength, "Chứng chỉ hành nghề");
                        }
                    }
                    if (!string.IsNullOrEmpty(item.MEDICINE_TYPE_RANK.Trim()))
                    {
                        if (IsNumber(item.MEDICINE_TYPE_RANK.Trim()))
                        {
                            if (Encoding.UTF8.GetByteCount(item.MEDICINE_TYPE_RANK.Trim()) > 19)
                            {
                                error += string.Format(Message.MessageImport.MaxLength, "Hạng thuốc kê đơn");
                            }
                            if (Convert.ToInt32(item.MEDICINE_TYPE_RANK.Trim()) < 0)
                                error += string.Format(Message.MessageImport.KhongTheAm, "Hạng thuốc kê đơn");
                            if (Convert.ToInt32(item.MEDICINE_TYPE_RANK.Trim()) < 1 || Convert.ToInt32(item.MEDICINE_TYPE_RANK.Trim()) > 5)
                                error += string.Format(Message.MessageImport.KhongTheLonHon, " ");
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Hạng thuốc kê đơn");
                        }
                    }
                    if (!string.IsNullOrEmpty(item.MAX_BHYT.Trim()))
                    {
                        if (IsNumber(item.MAX_BHYT.Trim()))
                        {
                            if (Encoding.UTF8.GetByteCount(item.MAX_BHYT.Trim()) > 19)
                            {
                                error += string.Format(Message.MessageImport.MaxLength, "Số xử lý chỉ định BHYT tối đa trong ngày");
                            }
                            if (Convert.ToInt32(item.MAX_BHYT.Trim()) < 0)
                                error += string.Format(Message.MessageImport.KhongTheAm, "Số xử lý chỉ định BHYT tối đa trong ngày");

                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Số xử lý chỉ định BHYT tối đa trong ngày");
                        }
                    }
                    if (!string.IsNullOrEmpty(item.ACCOUNT_NUMBER))
                    {
                        if (IsNumber(item.ACCOUNT_NUMBER.Trim()))
                        {
                            if (Encoding.UTF8.GetByteCount(item.ACCOUNT_NUMBER.Trim()) > 50)
                            {
                                error += string.Format(Message.MessageImport.MaxLength, "Số tài khoản");
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Số tài khoản");
                        }

                    }
                    if (!string.IsNullOrEmpty(item.BANK))
                    {
                        if (Encoding.UTF8.GetByteCount(item.BANK.Trim()) > 200)
                        {
                            error += string.Format(Message.MessageImport.MaxLength, "Ngân hàng");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.DEPARTMENT_CODE))
                    {
                        if (Encoding.UTF8.GetByteCount(item.DEPARTMENT_CODE.Trim()) > 10)
                        {
                            error += string.Format(Message.MessageImport.MaxLength, "Mã khoa");

                        }
                        else
                        {

                            var check = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(p => p.DEPARTMENT_CODE == item.DEPARTMENT_CODE);
                            if (check != null)
                            { }
                            else error += string.Format(Message.MessageImport.KhongHopLe, "Mã khoa");

                        }
                    }

                    if (!string.IsNullOrEmpty(item.IS_ADMIN_STR))
                    {
                        if (item.IS_ADMIN_STR.Trim().ToUpper() != "X")
                        {
                            error += string.Format(Message.MessageImport.SaiDinhDang, "Là quản trị");

                        }
                        else { serAdo.CONG_VIEC = "Quản trị"; }
                    }

                    if (!string.IsNullOrEmpty(item.IS_DOCTOR_STR))
                    {
                        if (item.IS_DOCTOR_STR.Trim().ToUpper() != "X")
                        {
                            error += string.Format(Message.MessageImport.SaiDinhDang, "Là bác sĩ");
                        }
                        else
                        {
                            serAdo.CONG_VIEC += !string.IsNullOrEmpty(serAdo.CONG_VIEC) ? " và Bác sĩ" : "Bác sĩ";
                        }
                    }
                    if (!string.IsNullOrEmpty(item.IS_NURSE_STR))
                    {
                        if (item.IS_NURSE_STR.Trim().ToUpper() != "X")
                        {
                            error += string.Format(Message.MessageImport.SaiDinhDang, "Là y tá");
                        }
                        else
                        {
                            serAdo.CONG_VIEC += !string.IsNullOrEmpty(serAdo.CONG_VIEC) ? " và Y tá" : "Y tá";
                        }
                    }
                    //if (!string.IsNullOrEmpty(item.IS_NURSE_STR))
                    //{
                    //    if (item.IS_NURSE_STR.Trim().ToUpper() != "X")
                    //    {
                    //        error += string.Format(Message.MessageImport.SaiDinhDang, "Là Y tá");
                    //    }
                    //    else
                    //    {
                    //        serAdo.IS_NURSE_STR += !string.IsNullOrEmpty(serAdo.IS_NURSE_STR) ? " và Y tá" : "Y tá";
                    //    }
                    //}
                    serAdo.ERROR = error;
                    serAdo.ID = i;
                    empUserRef.Add(serAdo);

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
                    var errorLine = this.EmpUserAdos.Where(o => !string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(errorLine);

                }
                else
                {
                    btnShowLineError.Text = "Dòng lỗi";
                    var errorLine = this.EmpUserAdos.Where(o => string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(errorLine);

                }

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                bool success = false;
                WaitingManager.Show();
                List<HIS_EMPLOYEE> datas = new List<HIS_EMPLOYEE>();
                CommonParam param = new CommonParam();
                if (this.EmpUserAdos != null && this.EmpUserAdos.Count > 0)
                {
                    foreach (var item in this.EmpUserAdos)
                    {
                        HIS_EMPLOYEE ado = new HIS_EMPLOYEE();
                        ado.LOGINNAME = item.LOGINNAME;
                        if (!string.IsNullOrEmpty(item.IS_ADMIN_STR))
                        {
                            ado.IS_ADMIN = 1;
                        }
                        if (!string.IsNullOrEmpty(item.IS_DOCTOR_STR))
                        {
                            ado.IS_DOCTOR = 1;

                        }
                        ado.DIPLOMA = item.DIPLOMA;
                        if (!string.IsNullOrEmpty(item.IS_NURSE_STR))
                        {
                            ado.IS_NURSE = 1;

                        }
                        if (!string.IsNullOrEmpty(item.MAX_BHYT.Trim()))
                        {
                            ado.MAX_BHYT_SERVICE_REQ_PER_DAY = Convert.ToInt64(item.MAX_BHYT);
                        }
                        else
                            ado.MAX_BHYT_SERVICE_REQ_PER_DAY = null;
                        if (!string.IsNullOrEmpty(item.MEDICINE_TYPE_RANK.Trim()))
                        {
                            ado.MEDICINE_TYPE_RANK = Convert.ToInt64(item.MEDICINE_TYPE_RANK);
                        }
                        else
                            ado.MEDICINE_TYPE_RANK = null;
                        ado.ACCOUNT_NUMBER = item.ACCOUNT_NUMBER;
                        ado.BANK = item.BANK;
                        if (string.IsNullOrEmpty(item.DEPARTMENT_CODE))
                            ado.DEPARTMENT_ID = null;
                        else
                            ado.DEPARTMENT_ID = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.DEPARTMENT_CODE == item.DEPARTMENT_CODE).First().ID;
                        ado.TDL_USERNAME = item.USERNAME;
                        ado.TDL_EMAIL = item.EMAIL;
                        ado.TDL_MOBILE = item.MOBILE;
                        datas.Add(ado);
                    }
                }
                else
                {
                    WaitingManager.Hide();
                    DevExpress.XtraEditors.XtraMessageBox.Show("Dữ liệu rỗng", "Thông báo");
                    return;
                }
                var emp = new BackendAdapter(param).Post<List<HIS_EMPLOYEE>>("api/HisEmployee/CreateList", ApiConsumers.MosConsumer, datas, param);
                WaitingManager.Hide();
                if (emp != null && emp.Count > 0)
                {
                    success = true;
                    btnSave.Enabled = false;
                    BackendDataWorker.Reset<HIS_EMPLOYEE>();
                    BackendDataWorker.Reset<ACS_USER>();
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

        private void gridView1_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    EmpUserADO pData = (EmpUserADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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

        private void gridView1_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    string error = (gridView1.GetRowCellValue(e.RowHandle, "ERROR") ?? "").ToString();
                    if (e.Column.FieldName == "Error")
                    {
                        if (!string.IsNullOrEmpty(error))
                        {
                            e.RepositoryItem = btnLoi;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnLoi_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (EmpUserADO)gridView1.GetFocusedRow();
                if (row != null && !string.IsNullOrEmpty(row.ERROR))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(row.ERROR, "Thông báo ");
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void btnDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (EmpUserADO)gridView1.GetFocusedRow();
                if (row != null)
                {

                    if (this.EmpUserAdos != null && this.EmpUserAdos.Count > 0)
                    {
                        this.EmpUserAdos.Remove(row);
                        var dataCheck = this.EmpUserAdos.Where(p => p.LOGINNAME == row.LOGINNAME).ToList();
                        if (dataCheck != null && dataCheck.Count == 1)
                        {
                            if (!string.IsNullOrEmpty(dataCheck[0].ERROR))
                            {
                                string errro = string.Format(Message.MessageImport.FileImportDaTonTai, dataCheck[0].LOGINNAME);
                                dataCheck[0].ERROR = dataCheck[0].ERROR.Replace(errro, "");

                            }
                        }
                        SetDataSource(this.EmpUserAdos);
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnLuu_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {

                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


    }
}
