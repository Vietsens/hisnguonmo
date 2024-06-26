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
using Inventec.Core;
using Inventec.Desktop.Common.Modules;
using Inventec.Common.Logging;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LibraryMessage;
using HIS.Desktop.LibraryMessage;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.BackendData;
using EMR.EFMODEL.DataModels;
using EMR.Filter;
using System.Collections;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Adapter;
using Inventec.Common.WebApiClient;
using HIS.Desktop.Utility;

namespace EMR.Desktop.Plugins.EmrConfig.EmrConfig
{
    public partial class frmEmrConfig : HIS.Desktop.Utility.FormBase
    {
        #region ---Declare---
        Module moduleCurrent;
        string workingModuleLink = "";
        int ActionType = -1;
        EMR_CONFIG ConfigCurrent = new EMR_CONFIG();
        #endregion

        #region ---Constructor---
        public frmEmrConfig(Module module)
        {
            InitializeComponent();
            this.moduleCurrent = module;
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        public frmEmrConfig(Module module, string _workingModuleLink)
        {
            InitializeComponent();
            this.moduleCurrent = module;
            this.workingModuleLink = _workingModuleLink;
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
        #endregion

        #region ---Method Private---
        private void frmEmrConfig_Load(object sender, EventArgs e)
        {
            try
            {
                txtValueDefault.Properties.ReadOnly = true;
                txtKey.Properties.ReadOnly = true;
                txtDescription.Properties.ReadOnly = true;
                // set ngon ngu
                SetCaptionByLanguagesKey();

                //gan giá trị mặc định
                SetValueDefault();

                //Validate 
                ValidationSingleControl(txtKey);

                //Load du lieu vao gridcontrol
                FillDataToGridControl();
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void FillDataToGridControl()
        {
            try
            {
                WaitingManager.Show();
                BackendDataWorker.Reset<EMR_CONFIG>();

                List<EMR_CONFIG> dataSource = BackendDataWorker.Get<EMR_CONFIG>();

                if (!String.IsNullOrWhiteSpace(txtKeyWord.Text))
                {
                    string keyword = txtKeyWord.Text.Trim().ToLower();
                    dataSource = dataSource
                        .Where(o => (!String.IsNullOrWhiteSpace(o.KEY) && o.KEY.ToLower().Contains(keyword))
                        || (!String.IsNullOrWhiteSpace(o.VALUE) && o.VALUE.ToLower().Contains(keyword))
                        || (!String.IsNullOrWhiteSpace(o.DESCRIPTION) && o.DESCRIPTION.ToLower().Contains(keyword))
                        || (!String.IsNullOrWhiteSpace(o.DEFAULT_VALUE) && o.DEFAULT_VALUE.ToLower().Contains(keyword))
                        ).OrderByDescending(o => o.MODIFY_TIME).ToList();
                }

                if (!String.IsNullOrWhiteSpace(txtModuleLink.Text) && dataSource != null && dataSource.Count() > 0)
                {
                    dataSource = dataSource
                       .Where(o => !String.IsNullOrWhiteSpace(o.MODULE_LINKS)
                           && o.MODULE_LINKS.ToLower().Contains(txtModuleLink.Text.Trim().ToLower())
                       ).OrderByDescending(o => o.MODIFY_TIME).ToList();
                }


                gridViewConfig.BeginUpdate();
                gridViewConfig.GridControl.DataSource = null;
                gridViewConfig.GridControl.DataSource = dataSource != null ? dataSource.OrderByDescending(o => o.MODIFY_TIME).ToList() : dataSource;
                gridViewConfig.EndUpdate();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void SetValueDefault()
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                dxValidationProvider1.RemoveControlError(txtKey);
                ResetFormData();
                EnableControlChanged(this.ActionType);
                txtKeyWord.Text = "";
                chkRefreshConfig.Checked = true;
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void ResetFormData()
        {
            try
            {
                if (!lcEditorInfo.IsInitialized) return;
                lcEditorInfo.BeginUpdate();
                try
                {
                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in lcEditorInfo.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null && lci.Control is DevExpress.XtraEditors.BaseEdit)
                        {
                            DevExpress.XtraEditors.BaseEdit fomatFrm = lci.Control as DevExpress.XtraEditors.BaseEdit;

                            fomatFrm.ResetText();
                            fomatFrm.EditValue = null;
                            txtKeyWord.Focus();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                finally
                {
                    lcEditorInfo.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void EnableControlChanged(int action)
        {
            try
            {
                btnEdit.Enabled = (action == GlobalVariables.ActionEdit);
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguagesKey()
        {
            try
            {
                Resources.ResourceLanguageManager.LanguageResource = new System.Resources.ResourceManager("EMR.Desktop.Plugins.EmrConfig.Resources.Lang", typeof(EMR.Desktop.Plugins.EmrConfig.EmrConfig.frmEmrConfig).Assembly);

                this.Text = (this.moduleCurrent != null ? this.moduleCurrent.text : "");

                this.lcKey.Text = SetKeyLanguage("frmEmrConfig.lcKey.Text");
                this.lcValue.Text = SetKeyLanguage("frmEmrConfig.lcValue.Text");
                this.lcValueDefault.Text = SetKeyLanguage("frmEmrConfig.lcValueDefault.Text");
                this.lcDescription.Text = SetKeyLanguage("frmEmrConfig.lcDescription.Text");
                this.lcRefreshConfig.Text = SetKeyLanguage("frmEmrConfig.lcRefreshConfig.Text");

                this.btnSearch.Text = SetKeyLanguage("frmEmrConfig.btnSearch.Text");
                this.btnRest.Text = SetKeyLanguage("frmEmrConfig.btnRest.Text");
                this.btnImport.Text = SetKeyLanguage("frmEmrConfig.btnImport.Text");
                this.btnExport.Text = SetKeyLanguage("frmEmrConfig.btnExport.Text");
                this.btnEdit.Text = SetKeyLanguage("frmEmrConfig.btnEdit.Text");
                this.btnCancel.Text = SetKeyLanguage("frmEmrConfig.btnCancel.Text");

                this.grdColKey.Caption = SetKeyLanguage("frmEmrConfig.grdColKey.Caption");
                this.grdColValue.Caption = SetKeyLanguage("frmEmrConfig.grdColValue.Caption");
                this.grdColValueDefault.Caption = SetKeyLanguage("frmEmrConfig.grdColValueDefault.Caption");
                this.grdColDesciption.Caption = SetKeyLanguage("frmEmrConfig.grdColDesciption.Caption");
                this.grdColModuleLink.Caption = SetKeyLanguage("frmEmrConfig.grdColModuleLink.Caption");
                this.grdColIsAction.Caption = SetKeyLanguage("frmEmrConfig.grdColIsAction.Caption");
                this.grdColCreateTime.Caption = SetKeyLanguage("frmEmrConfig.grdColCreateTime.Caption");
                this.grdColCreator.Caption = SetKeyLanguage("frmEmrConfig.grdColCreator.Caption");
                this.grdColModifyTime.Caption = SetKeyLanguage("frmEmrConfig.grdColModifyTime.Caption");
                this.grdColModifier.Caption = SetKeyLanguage("frmEmrConfig.grdColModifier.Caption");

            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private string SetKeyLanguage(string Key)
        {
            string Result = "";
            try
            {
                Result = Inventec.Common.Resource.Get.Value(Key, Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
            return Result;
        }

        private void ValidationSingleControl(DevExpress.XtraEditors.BaseEdit control)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SaveProcessor()
        {
            try
            {
                if (!btnEdit.Enabled)
                    return;
                if (!dxValidationProvider1.Validate())
                    return;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                if (this.ConfigCurrent != null && this.ConfigCurrent.ID > 0)
                {
                    EMR_CONFIG dataUpdate = new EMR_CONFIG();
                    dataUpdate = BackendDataWorker.Get<EMR_CONFIG>().FirstOrDefault(o => o.ID == ConfigCurrent.ID);
                    dataUpdate.VALUE = txtValue.Text.Trim();
                    var resutl = new BackendAdapter(param).Post<EMR_CONFIG>(EMR.URI.EmrConfig.UPDATE, ApiConsumers.EmrConsumer, dataUpdate, param);
                    if (resutl != null)
                    {
                        success = true;
                        BackendDataWorker.Reset<EMR_CONFIG>();
                        Inventec.Common.SignLibrary.GlobalStore.EmrConfigs = null;
                        FillDataToGridControl();
                        if (chkRefreshConfig.Checked)
                        {
                            CommonParam paramrest = new CommonParam();
                            var ResultRest = new BackendAdapter(paramrest).Post<bool>(EMR.URI.EmrConfig.RESET_ALL, ApiConsumers.EmrConsumer, null, paramrest);
                            success = ResultRest;
                            if (paramrest.Messages != null && paramrest.Messages.Count > 0)
                                param.Messages.Add(paramrest.GetMessage());
                        }
                    }
                }
                WaitingManager.Hide();
                MessageManager.Show(this.ParentForm, param, success);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        private void ProcessData(List<EMR_CONFIG> data, ref Inventec.Common.FlexCellExport.Store store)
        {
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.SetCommonFunctions();
                objectTag.AddObjectData(store, "ExportResult", data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                store = null;
            }
        }
        #endregion

        #region ---Click---
        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                SetValueDefault();
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                SaveProcessor();
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToGridControl();
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void btnRest_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Bạn có muốn reset lại dữ liệu không?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    bool sucssess = false;
                    CommonParam param = new CommonParam();
                    var lsData = (List<EMR_CONFIG>)gridControlConfig.DataSource;
                    if (lsData != null && lsData.Count > 0)
                    {
                        lsData.ForEach(o => o.VALUE = o.DEFAULT_VALUE);
                        var Result = new BackendAdapter(param).Post<List<EMR_CONFIG>>(EMR.URI.EmrConfig.UPDATE_LIST, HIS.Desktop.ApiConsumer.ApiConsumers.EmrConsumer, lsData, param);
                        if (Result != null && Result.Count > 0)
                        {
                            sucssess = true;
                            BackendDataWorker.Reset<EMR_CONFIG>();
                            FillDataToGridControl();
                        }
                    }
                    WaitingManager.Hide();
                    MessageManager.Show(this.ParentForm, param, sucssess);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "EMR.Desktop.Plugins.EmrImportConfig").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'EMR.Desktop.Plugins.EmrImportConfig'");

                List<object> listArgs = new List<object>();
                listArgs.Add(this.moduleCurrent);

                var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleCurrent != null ? this.moduleCurrent.RoomId : 0, this.moduleCurrent != null ? this.moduleCurrent.RoomTypeId : 0), listArgs);
                if (extenceInstance == null) throw new NullReferenceException("extenceInstance is null");

                WaitingManager.Hide();
                ((System.Windows.Forms.Form)extenceInstance).ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                List<string> expCode = new List<string>();

                Inventec.Common.FlexCellExport.Store store = new Inventec.Common.FlexCellExport.Store(true);

                string templateFile = System.IO.Path.Combine(Application.StartupPath + "\\Tmp\\Exp", "EXPORT_EMR_CONFIG.xlsx");

                //chọn đường dẫn
                saveFileDialog1.Filter = "Excel 2007 later file (*.xlsx)|*.xlsx|Excel 97-2003 file(*.xls)|*.xls";
                if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {

                    //getdata
                    WaitingManager.Show();

                    if (String.IsNullOrEmpty(templateFile))
                    {
                        store = null;
                        DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Không tìm thấy file", templateFile));
                        return;
                    }

                    store.ReadTemplate(System.IO.Path.GetFullPath(templateFile));
                    if (store.TemplatePath == "")
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Biểu mẫu đang mở hoặc không tồn tại file template. Vui lòng kiểm tra lại. (" + templateFile + ")");
                        return;
                    }

                    EmrConfigFilter filter = new EmrConfigFilter();
                    var EmrConfigs = new BackendAdapter(new CommonParam()).Get<List<EMR_CONFIG>>(EMR.URI.EmrConfig.GET, HIS.Desktop.ApiConsumer.ApiConsumers.EmrConsumer, filter, null);

                    ProcessData(EmrConfigs, ref store);
                    WaitingManager.Hide();

                    if (store != null)
                    {
                        try
                        {
                            if (store.OutFile(saveFileDialog1.FileName))
                            {
                                DevExpress.XtraEditors.XtraMessageBox.Show("Xuất file thành công");

                                if (MessageBox.Show("Bạn có muốn mở file?",
                                    "Thông báo", MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Question) == DialogResult.Yes)
                                    System.Diagnostics.Process.Start(saveFileDialog1.FileName);
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn(ex);
                        }
                    }
                    else
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Xử lý thất bại");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #region ---ItemClick---
        private void bbtnSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnSearch.Enabled)
                    btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void bbtnImport_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnImport.Enabled)
                    btnImport_Click(null, null);
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void bbtnExport_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnExport.Enabled)
                    btnExport_Click(null, null);
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void bbtnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.ActionType == GlobalVariables.ActionEdit && btnEdit.Enabled)
                {
                    btnEdit_Click(null, null);
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void bbtCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnCancel.Enabled)
                    btnCancel_Click(null, null);
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void F2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                txtValue.Focus();
                txtValue.SelectAll();
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        #endregion

        #region ---ButtonClick---
        private void btnLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    bool success = false;
                    CommonParam param = new CommonParam();
                    var DataRow = (EMR_CONFIG)gridViewConfig.GetFocusedRow();
                    if (DataRow != null)
                    {
                        var Result = new BackendAdapter(param).Post<EMR_CONFIG>(EMR.URI.EmrConfig.LOCK, ApiConsumers.EmrConsumer, DataRow, param);

                        if (Result != null)
                        {
                            success = true;
                            BackendDataWorker.Reset<EMR_CONFIG>();
                            FillDataToGridControl();
                            btnCancel_Click(null, null);
                        }
                    }

                    WaitingManager.Hide();
                    MessageManager.Show(this.ParentForm, param, success);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        private void btnUnlock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    bool success = false;
                    CommonParam param = new CommonParam();
                    var DataRow = (EMR_CONFIG)gridViewConfig.GetFocusedRow();
                    if (DataRow != null)
                    {
                        var Result = new BackendAdapter(param).Post<EMR_CONFIG>(EMR.URI.EmrConfig.UNLOCK, ApiConsumers.EmrConsumer, DataRow, param);
                        if (Result != null)
                        {
                            success = true;
                            BackendDataWorker.Reset<EMR_CONFIG>();
                            FillDataToGridControl();
                            btnCancel_Click(null, null);
                        }
                    }

                    WaitingManager.Hide();
                    MessageManager.Show(this.ParentForm, param, success);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        private void btnEnableRest_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Bạn có muốn reset lại dữ liệu không?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    bool success = false;
                    CommonParam param = new CommonParam();
                    var DataRow = (EMR_CONFIG)gridViewConfig.GetFocusedRow();
                    if (DataRow != null)
                    {
                        EMR_CONFIG dataUpdate = new EMR_CONFIG();
                        dataUpdate = BackendDataWorker.Get<EMR_CONFIG>().FirstOrDefault(o => o.ID == DataRow.ID);
                        dataUpdate.VALUE = dataUpdate.DEFAULT_VALUE;
                        var Result = new BackendAdapter(param).Post<EMR_CONFIG>(EMR.URI.EmrConfig.UPDATE, ApiConsumers.EmrConsumer, dataUpdate, param);
                        if (Result != null)
                        {
                            success = true;
                            BackendDataWorker.Reset<EMR_CONFIG>();
                            FillDataToGridControl();
                            btnCancel_Click(null, null);
                        }
                    }

                    WaitingManager.Hide();
                    MessageManager.Show(this.ParentForm, param, success);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }
        #endregion
        #endregion

        #region ---PreviewKeyDown---
        private void txtKeyWord_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToGridControl();
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void txtModuleLink_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToGridControl();
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }

        }
        #endregion

        #region ---Even Gridcontrol---
        private void gridViewConfig_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    var datarow = (EMR_CONFIG)gridViewConfig.GetRow(e.RowHandle);
                    if (datarow != null)
                    {
                        if (e.Column.FieldName == "LOCK")
                        {
                            e.RepositoryItem = (datarow.IS_ACTIVE == 1 ? btnLock : btnUnlock);
                        }
                        if (e.Column.FieldName == "REST")
                        {
                            e.RepositoryItem = (datarow.IS_ACTIVE == 1 ? btnEnableRest : btnDisableRest);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void gridViewConfig_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    var dataRow = (EMR_CONFIG)gridViewConfig.GetRow(e.RowHandle);
                    if (dataRow != null)
                    {
                        if (e.Column.FieldName == "IS_ACTIVE_STR")
                        {
                            e.Appearance.ForeColor = (dataRow.IS_ACTIVE == 1 ? System.Drawing.Color.Green : System.Drawing.Color.Red);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void gridViewConfig_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    EMR_CONFIG pData = (EMR_CONFIG)((IList)((DevExpress.XtraGrid.Views.Base.BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    else if (e.Column.FieldName == "IS_ACTIVE_STR")
                    {
                        e.Value = (pData.IS_ACTIVE == 1 ? "Hoạt động" : "Tạm khóa");
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString((long)pData.CREATE_TIME);
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString((long)pData.MODIFY_TIME);
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }
        #endregion

        private void gridViewConfig_Click(object sender, EventArgs e)
        {
            try
            {
                var dataRow = (EMR_CONFIG)gridViewConfig.GetFocusedRow();
                if (dataRow != null)
                {
                    this.ConfigCurrent = new EMR_CONFIG();
                    this.ConfigCurrent = dataRow;
                    SetDataEditcontrol(dataRow);
                    this.ActionType = GlobalVariables.ActionEdit;
                    EnableControlChanged(this.ActionType);
                    btnEdit.Enabled = (dataRow.IS_ACTIVE == 1);
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void SetDataEditcontrol(EMR_CONFIG dataRow)
        {
            try
            {
                if (dataRow != null)
                {
                    txtKey.Text = dataRow.KEY;
                    txtValue.Text = dataRow.VALUE;
                    txtValueDefault.Text = dataRow.DEFAULT_VALUE;
                    txtDescription.Text = dataRow.DESCRIPTION;
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
    }
}
