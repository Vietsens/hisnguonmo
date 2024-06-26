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
using Inventec.Common.Logging;
using Inventec.Core;
using System.IO;
using Inventec.Desktop.Common.Message;
using ACS.EFMODEL.DataModels;
using ACS.Filter;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using HIS.Desktop.Common;

namespace HIS.Desktop.Plugins.ExecuteRoleUserImport
{
    public partial class frmExecuteRoleUserImport : HIS.Desktop.Utility.FormBase
    {
        #region --Declare variable---
        Module currentModule;
        List<ADO.ExcuteRoleUserADO> CurrentExcuterRoleUser;
        List<ADO.ExcuteRoleUserADO> ExcuteRoleUserImport;
        List<ACS_USER> lsAcsUserCurrent;
        List<HIS_EXECUTE_ROLE> lsExcuterRole;
        List<HIS_EXECUTE_ROLE_USER> lsExcuteRoleUser;
        RefeshReference delegateRefresh;
        #endregion
        public frmExecuteRoleUserImport(Module module)
            : base(module)
        {

            InitializeComponent();
            try
            {
                this.currentModule = module;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);

            }
        }

        public frmExecuteRoleUserImport(Module module, RefeshReference _delegateRefresh)
            : base(module)
        {

            InitializeComponent();
            try
            {
                this.currentModule = module;
                this.delegateRefresh = _delegateRefresh;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);

            }
        }

        private void frmExecuteRoleUserImport_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                SetCationByKeyLanguage();
                LoadDataCurrent();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }

        }

        private void LoadDataCurrent()
        {
            try
            {

                CommonParam param1 = new CommonParam();
                HisExecuteRoleFilter filer = new HisExecuteRoleFilter();
                this.lsExcuterRole = new Inventec.Common.Adapter.BackendAdapter(param1).Get<List<HIS_EXECUTE_ROLE>>(HIS.Desktop.ApiConsumer.HisRequestUriStore.HIS_EXECUTE_ROLE_GET, HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, filer, param1);
                CommonParam param2 = new CommonParam();
                MOS.Filter.HisExecuteRoleUserFilter filter = new HisExecuteRoleUserFilter();
                this.lsExcuteRoleUser = new Inventec.Common.Adapter.BackendAdapter(param2).Get<List<HIS_EXECUTE_ROLE_USER>>(
                   "api/HisExecuteRoleUser/Get",
                   HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                   filter,
                   param2);
                this.lsAcsUserCurrent = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS_USER>().Where(o => o.IS_ACTIVE == 1).ToList();
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void SetCationByKeyLanguage()
        {
            try
            {
                this.Text = (this.currentModule != null ? this.currentModule.text : "");
                Resources.ResourceLanguageManager.LanguageResource = new System.Resources.ResourceManager("HIS.Desktop.Plugins.ExecuteRoleUserImport.Resources.Lang", typeof(HIS.Desktop.Plugins.ExecuteRoleUserImport.frmExecuteRoleUserImport).Assembly);
                this.btnDowloasFile.Text = SetKey("frmExecuteRoleUserImport.btnDowloasFile.Text");
                this.btnImportFile.Text = SetKey("frmExecuteRoleUserImport.btnImportFile.Text");
                this.btnLineError.Text = SetKey("frmExecuteRoleUserImport.btnLineError.Text");
                this.btnSave.Text = SetKey("frmExecuteRoleUserImport.btnSave.Text");
                this.grdColExcuteRoleCode.Caption = SetKey("frmExecuteRoleUserImport.grdColExcuteRoleCode.Caption");
                this.grdColExcuteRoleName.Caption = SetKey("frmExecuteRoleUserImport.grdColExcuteRoleName.Caption");
                this.grdColLoginName.Caption = SetKey("frmExecuteRoleUserImport.grdColLoginName.Caption");
                this.grdColUserName.Caption = SetKey("frmExecuteRoleUserImport.grdColUserName.Caption");
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private string SetKey(string key)
        {
            string result = "";
            try
            {
                result = Inventec.Common.Resource.Get.Value(key, Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                result = "";
                LogSystem.Warn(ex);
            }
            return result;
        }

        private void gridViewExcuteRoleUser_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    ADO.ExcuteRoleUserADO pData = (ADO.ExcuteRoleUserADO)((System.Collections.IList)((DevExpress.XtraGrid.Views.Base.BaseView)sender).DataSource)[e.ListSourceRowIndex];
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

        private void btnDowloasFile_Click(object sender, EventArgs e)
        {
            try
            {
                string filename = System.IO.Path.Combine(Application.StartupPath + "\\Tmp\\Imp\\", "IMPORT_EXCUTE_ROLE_USER.xlsx");
                CommonParam param = new CommonParam();
                if (System.IO.File.Exists(filename))
                {
                    saveFileDialog.Title = "Save File";
                    saveFileDialog.FileName = "IMPORT_EXCUTE_ROLE_USER";
                    saveFileDialog.DefaultExt = "xlsx";
                    saveFileDialog.Filter = "Excel files (*.xlsx)|All files (*.*)";
                    saveFileDialog.FilterIndex = 2;
                    saveFileDialog.RestoreDirectory = true;
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        File.Copy(filename, saveFileDialog.FileName);
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

        private void btnImportFile_Click(object sender, EventArgs e)
        {
            try
            {
                btnLineError.Text = SetKey("frmExecuteRoleUserImport.btnLineError.Text");

                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Multiselect = false;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    WaitingManager.Show();

                    var import = new Inventec.Common.ExcelImport.Import();
                    if (import.ReadFileExcel(ofd.FileName))
                    {
                        var ExcuterRoleUserImport = import.GetWithCheck<ADO.ExcuteRoleUserADO>(0);
                        if (ExcuterRoleUserImport != null && ExcuterRoleUserImport.Count > 0)
                        {
                            List<ADO.ExcuteRoleUserADO> listAfterRemove = new List<ADO.ExcuteRoleUserADO>();


                            foreach (var item in ExcuterRoleUserImport)
                            {
                                bool checkNull = string.IsNullOrEmpty(item.LogiName) && string.IsNullOrEmpty(item.ExcuteRoleCode)
                                    ;
                                if (!checkNull)
                                {
                                    listAfterRemove.Add(item);
                                }
                            }
                            WaitingManager.Hide();

                            this.CurrentExcuterRoleUser = listAfterRemove;

                            if (this.CurrentExcuterRoleUser != null && this.CurrentExcuterRoleUser.Count > 0)
                            {
                                btnLineError.Enabled = true;
                                this.ExcuteRoleUserImport = new List<ADO.ExcuteRoleUserADO>();
                                addExcuterRoleUserToProcessList(CurrentExcuterRoleUser, ref this.ExcuteRoleUserImport);
                                SetDataSource(this.ExcuteRoleUserImport);
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

        private void SetDataSource(List<ADO.ExcuteRoleUserADO> listdata)
        {
            try
            {
                gridControlExcuteRoleUser.BeginUpdate();
                gridControlExcuteRoleUser.DataSource = null;
                gridControlExcuteRoleUser.DataSource = listdata;
                gridControlExcuteRoleUser.EndUpdate();
                CheckErrorLine();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CheckErrorLine()
        {
            try
            {
                var checkError = this.ExcuteRoleUserImport.Exists(o => !string.IsNullOrEmpty(o.Error));
                if (!checkError)
                {
                    btnSave.Enabled = true;
                    btnLineError.Enabled = false;
                }
                else
                {
                    btnLineError.Enabled = true;
                    btnSave.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void addExcuterRoleUserToProcessList(List<ADO.ExcuteRoleUserADO> CurrentExcuterRoleUser, ref List<ADO.ExcuteRoleUserADO> excuterRoleUser)
        {
            try
            {
                excuterRoleUser = new List<ADO.ExcuteRoleUserADO>();
                foreach (var item in CurrentExcuterRoleUser)
                {
                    string Error = "";
                    ADO.ExcuteRoleUserADO excuteRoleUseritem = new ADO.ExcuteRoleUserADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<ADO.ExcuteRoleUserADO>(excuteRoleUseritem, item);
                    if (!string.IsNullOrEmpty(item.ExcuteRoleCode))
                    {
                        if (Inventec.Common.String.CountVi.Count(item.ExcuteRoleCode) > 2)
                        {
                            Error += string.Format(Message.Maxlength, "Mã vai trò");
                        }
                        else
                        {
                            if (lsExcuterRole != null && lsExcuterRole.Count > 0)
                            {
                                var checkExcuterRole = this.lsExcuterRole.FirstOrDefault<HIS_EXECUTE_ROLE>(o => o.EXECUTE_ROLE_CODE == item.ExcuteRoleCode);
                                if (checkExcuterRole != null)
                                {
                                    excuteRoleUseritem.ExcuteRoleCode = checkExcuterRole.EXECUTE_ROLE_CODE;
                                    excuteRoleUseritem.ExcuteRoleName = checkExcuterRole.EXECUTE_ROLE_NAME;
                                    excuteRoleUseritem.ExcuterRole_ID = checkExcuterRole.ID;
                                }
                                else
                                {
                                    Error += string.Format(Message.KhongTonTai, "Mã vai trò");
                                }
                            }
                        }
                    }
                    else
                    {
                        Error += string.Format(Message.ThieuTruongDL, "Mã vai trò");
                    }
                    if (!string.IsNullOrEmpty(item.LogiName))
                    {
                        if (Inventec.Common.String.CountVi.Count(item.LogiName) > 50)
                        {
                            Error += string.Format(Message.Maxlength, "Tên đăng nhập");
                        }
                        else
                        {
                            var acsUser = this.lsAcsUserCurrent.FirstOrDefault<ACS_USER>(o => o.LOGINNAME.Trim().ToUpper() == item.LogiName.Trim().ToUpper());
                            if (acsUser != null)
                            {
                                excuteRoleUseritem.LogiName = acsUser.LOGINNAME;
                                excuteRoleUseritem.UserName = acsUser.USERNAME;
                            }
                            else
                            {
                                Error += string.Format(Message.KhongTonTai, "Tên đăng nhập");
                            }
                        }
                    }
                    else
                    {
                        Error += string.Format(Message.ThieuTruongDL, "tên đăng nhập");
                    }

                    var datacheck = CurrentExcuterRoleUser.Where(o => o.ExcuteRoleCode == item.ExcuteRoleCode && o.LogiName == item.LogiName).ToList();
                    if (datacheck != null && datacheck.Count > 1)
                    {
                        Error += string.Format(Message.TonTaiTrungNhauTrongFileImport);
                    }
                    if (!string.IsNullOrEmpty(item.LogiName) && !string.IsNullOrEmpty(item.ExcuteRoleCode))
                    {
                        var checkExcuterRole = this.lsExcuterRole.FirstOrDefault<HIS_EXECUTE_ROLE>(o => o.EXECUTE_ROLE_CODE == item.ExcuteRoleCode);
                        var acsUser = this.lsAcsUserCurrent.FirstOrDefault<ACS_USER>(o => o.LOGINNAME.Trim().ToUpper() == item.LogiName.Trim().ToUpper());
                        if (checkExcuterRole != null && acsUser != null)
                        {
                            var data = this.lsExcuteRoleUser.FirstOrDefault(o => o.EXECUTE_ROLE_ID == checkExcuterRole.ID && o.LOGINNAME == acsUser.LOGINNAME);
                            if (data != null)
                            {
                                Error += string.Format(Message.DuLieuDaTonTai);
                            }
                        }
                    }
                    excuteRoleUseritem.Error = Error;
                    excuterRoleUser.Add(excuteRoleUseritem);
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void btnLineError_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnLineError.Text == "Dòng lỗi")
                {
                    btnLineError.Text = SetKey("frmExecuteRoleUserImport.btnLineNotError.Text");
                    var errorLine = this.ExcuteRoleUserImport.Where(o => !string.IsNullOrEmpty(o.Error)).ToList();
                    SetDataSource(errorLine);

                }
                else
                {
                    btnLineError.Text = SetKey("frmExecuteRoleUserImport.btnLineError.Text");
                    var errorLine = this.ExcuteRoleUserImport.Where(o => string.IsNullOrEmpty(o.Error)).ToList();
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
                WaitingManager.Show();
                bool success = false;
                List<HIS_EXECUTE_ROLE_USER> lsExcuteRoleUser = new List<HIS_EXECUTE_ROLE_USER>();
                if (this.ExcuteRoleUserImport != null && this.ExcuteRoleUserImport.Count > 0)
                {
                    foreach (var item in this.ExcuteRoleUserImport)
                    {
                        HIS_EXECUTE_ROLE_USER excuterRoleUserItem = new HIS_EXECUTE_ROLE_USER();
                        excuterRoleUserItem.EXECUTE_ROLE_ID = item.ExcuterRole_ID;
                        excuterRoleUserItem.LOGINNAME = item.LogiName;
                        lsExcuteRoleUser.Add(excuterRoleUserItem);
                    }
                }
                if (lsExcuteRoleUser != null && lsExcuteRoleUser.Count > 0)
                {
                    CommonParam param = new CommonParam();
                    var dataImports = new Inventec.Common.Adapter.BackendAdapter(param).Post<List<HIS_EXECUTE_ROLE_USER>>("api/HisExecuteRoleUser/CreateList", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, lsExcuteRoleUser, param);
                    if (dataImports != null && dataImports.Count > 0)
                    {
                        success = true;
                        btnSave.Enabled = false;
                        if (this.delegateRefresh != null)
                        {
                            this.delegateRefresh();
                        }
                    }
                    WaitingManager.Hide();
                    MessageManager.Show(this.ParentForm, param, success);
                }
                else
                {
                    WaitingManager.Hide();
                    DevExpress.XtraEditors.XtraMessageBox.Show("Dữ liệu rỗng", "Thông báo");
                    return;
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (ADO.ExcuteRoleUserADO)gridViewExcuteRoleUser.GetFocusedRow();
                if (row != null)
                {
                    if (this.ExcuteRoleUserImport != null && this.ExcuteRoleUserImport.Count > 0)
                    {
                        this.ExcuteRoleUserImport.Remove(row);
                        var dataCheck = this.ExcuteRoleUserImport.Where(p => p.ExcuteRoleCode.ToUpper().Trim() == row.ExcuteRoleCode.ToUpper().Trim() && p.ExcuterRole_ID == row.ExcuterRole_ID).ToList();
                        if (dataCheck != null && dataCheck.Count == 1)
                        {
                            if (!string.IsNullOrEmpty(dataCheck[0].Error))
                            {
                                string erro = string.Format(Message.TonTaiTrungNhauTrongFileImport);
                                dataCheck[0].Error = dataCheck[0].Error.Replace(erro, "");
                            }

                        }
                        SetDataSource(this.ExcuteRoleUserImport);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonError_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (ADO.ExcuteRoleUserADO)gridViewExcuteRoleUser.GetFocusedRow();
                if (row != null && !string.IsNullOrEmpty(row.Error))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(row.Error, "Thông báo");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewExcuteRoleUser_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    string error = (gridViewExcuteRoleUser.GetRowCellValue(e.RowHandle, "Error") ?? "").ToString();
                    if (e.Column.FieldName == "Error")
                    {
                        if (!string.IsNullOrEmpty(error))
                        {
                            e.RepositoryItem = repositoryItemButtonError;
                        }
                    }
                }
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
                if (btnSave.Enabled)
                    btnSave_Click(null, null);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


    }
}
