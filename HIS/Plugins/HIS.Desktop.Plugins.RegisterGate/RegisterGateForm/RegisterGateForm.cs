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
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;

using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.RegisterGate.RegisterGateForm
{
    public partial class RegisterGateForm : FormBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        int positionHandler = -1;
        PagingGrid pagingGrid;
        Inventec.Desktop.Common.Modules.Module currentModule;
        MOS.EFMODEL.DataModels.HIS_REGISTER_GATE currentData;
        DelegateSelectData delegateSelect = null;
        int ActionType = -1;
        #endregion

        #region contruct
        public RegisterGateForm(Inventec.Desktop.Common.Modules.Module module, DelegateSelectData delegateData)
            : base(module)
        {

            InitializeComponent();
            currentModule = module;
            this.delegateSelect = delegateData;
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

        public RegisterGateForm(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {

            try
            {
                InitializeComponent();
                //pagingGrid = new PagingGrid();
                currentModule = module;
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
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void RegisterGateForm_Load(object sender, EventArgs e)
        {
            try
            {
                FormShow();
            }
            catch (Exception ex) { }
        }

        private void FormShow()
        {
            SetCaptionByLanguageKey();
            SetDefaultFocus();
            //load dữ liệu
            FillDataToGridControl();
            ValidateForm();
        }

        private void ValidateForm()
        {
            try
            {
                ValidMaxlengthTextBox(txtCode, 2);
                ValidMaxlengthTextBox(txtName, 100);
                //ValidationSingleControl(txtName);
                //ValidationSingleControl(txtCode);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidMaxlengthTextBox(TextEdit txtEdit, int? maxLength)
        {
            try
            {
                ValidateMaxLength validateMaxLength = new ValidateMaxLength();
                validateMaxLength.textEdit = txtEdit;
                validateMaxLength.maxLength = maxLength;
                dxValidationProvider1.SetValidationRule(txtEdit, validateMaxLength);

                //ValidateCheckZero vali = new ValidateCheckZero();
                //vali.textEdit = txtFORMAT;
                //dxValidationProvider1.SetValidationRule(txtFORMAT, vali);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationSingleControl(BaseEdit control)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);

                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToGridControl()
        {
            try
            {
                WaitingManager.Show();

                int numPageSize = 0;
                if (ucPaging.pagingGrid != null)
                {
                    numPageSize = ucPaging.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                LoadPaging(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging.Init(LoadPaging, param, numPageSize, this.gridControl1);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void LoadPaging(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.HIS_REGISTER_GATE>> apiResult = null;
                HisRegisterGateFilter filter = new HisRegisterGateFilter();
                SetFilterNavBar(ref filter);
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                gridViewRegisterGate.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.HIS_REGISTER_GATE>>(HisRequestUriStore.MOSHIS_REGISTER_GATE_GETVIEW, ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<MOS.EFMODEL.DataModels.HIS_REGISTER_GATE>)apiResult.Data;
                    if (data != null)
                    {

                        gridViewRegisterGate.GridControl.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                gridViewRegisterGate.EndUpdate();

                #region Process has exception
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetFilterNavBar(ref HisRegisterGateFilter filter)
        {
            try
            {
                filter.KEY_WORD = txtSearch.Text.Trim();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {

            try
            {
                ////Khoi tao doi tuong resource
                HIS.Desktop.Plugins.RegisterGate.Resource.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.RegisterGate.Resource.Lang", typeof(HIS.Desktop.Plugins.RegisterGate.RegisterGateForm.RegisterGateForm).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("RegisterGateForm.layoutControl1.Text", HIS.Desktop.Plugins.RegisterGate.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcEditorInfo.Text = Inventec.Common.Resource.Get.Value("RegisterGateForm.layoutControl3.Text", HIS.Desktop.Plugins.RegisterGate.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grlSTT.Caption = Inventec.Common.Resource.Get.Value("RegisterGateForm.grlSTT.Caption", HIS.Desktop.Plugins.RegisterGate.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gclCode.Caption = Inventec.Common.Resource.Get.Value("RegisterGateForm.gclCode.Caption", HIS.Desktop.Plugins.RegisterGate.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gclName.Caption = Inventec.Common.Resource.Get.Value("RegisterGateForm.gclName.Caption", HIS.Desktop.Plugins.RegisterGate.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("RegisterGateForm.btnEdit.Text", HIS.Desktop.Plugins.RegisterGate.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnReset.Text = Inventec.Common.Resource.Get.Value("RegisterGateForm.btnReset.Text", HIS.Desktop.Plugins.RegisterGate.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("RegisterGateForm.btnAdd.Text", HIS.Desktop.Plugins.RegisterGate.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("RegisterGateForm.layoutControl2.Text", HIS.Desktop.Plugins.RegisterGate.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("RegisterGateForm.btnSearch.Text", HIS.Desktop.Plugins.RegisterGate.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("RegisterGateForm.layoutControlItem6.Text", HIS.Desktop.Plugins.RegisterGate.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("RegisterGateForm.layoutControlItem7.Text", HIS.Desktop.Plugins.RegisterGate.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("RegisterGateForm.Text", HIS.Desktop.Plugins.RegisterGate.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                if (this.currentModule != null && !string.IsNullOrEmpty(currentModule.text))
                {
                    this.Text = this.currentModule.text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void SetDefaultFocus()
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                txtSearch.Text = "";
                ResetFormData();
                EnableControlChanged(this.ActionType);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void EnableControlChanged(int p)
        {
            btnAdd.Enabled = (p == GlobalVariables.ActionAdd);

            btnEdit.Enabled = (p == GlobalVariables.ActionEdit);
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

                            ckbReset.Properties.AllowGrayed = false;
                            ckbReset.Checked = false;
                        }
                    }
                    txtCode.Focus();
                    txtCode.SelectAll();
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

        private void bbtnGLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            HIS_REGISTER_GATE success = new HIS_REGISTER_GATE();
            bool notHandler = false;
            try
            {
                HIS_REGISTER_GATE data = (HIS_REGISTER_GATE)gridViewRegisterGate.GetFocusedRow();
                if (
                    //DevExpress.XtraEditors.XtraMessageBox.Show("Bạn có chắc muốn mở khóa dữ liệu",
                    //"", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes
                    MessageBox.Show("Bạn có chắc muốn mở khóa dữ liệu?", "",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes
                    )
                {
                    HIS_REGISTER_GATE data1 = new HIS_REGISTER_GATE();
                    data1.ID = data.ID;
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).
                        Post<HIS_REGISTER_GATE>(HisRequestUriStore.MOSHIS_REGISTER_GATE_CHANGELOCK, ApiConsumers.MosConsumer, data1.ID, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        notHandler = true;
                        FillDataToGridControl();
                    }
                    MessageManager.Show(this, param, notHandler);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnGUnLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            HIS_REGISTER_GATE success = new HIS_REGISTER_GATE();
            bool notHandler = false;
            try
            {
                HIS_REGISTER_GATE data = (HIS_REGISTER_GATE)gridViewRegisterGate.GetFocusedRow();
                if (
                    //DevExpress.XtraEditors.XtraMessageBox.Show("Bạn có chắc muốn khóa dữ liệu",
                    //"", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes
                    MessageBox.Show("Bạn có chắc muốn khóa dữ liệu?", "",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes
                    )
                {
                    HIS_REGISTER_GATE data1 = new HIS_REGISTER_GATE();
                    data1.ID = data.ID;
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).
                        Post<HIS_REGISTER_GATE>(HisRequestUriStore.MOSHIS_REGISTER_GATE_CHANGELOCK, ApiConsumers.MosConsumer, data1.ID, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        notHandler = true;
                        FillDataToGridControl();
                    }
                    MessageManager.Show(this, param, notHandler);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            Delete();
        }

        private void Delete()
        {
            btnEdit.Enabled = false;
            CommonParam param = new CommonParam();
            try
            {
                HIS_REGISTER_GATE rowData = (HIS_REGISTER_GATE)gridViewRegisterGate.GetFocusedRow();
                if (
                    //DevExpress.XtraEditors.XtraMessageBox.Show("Bạn có muốn xóa dữ liệu",
                    //"", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes
                    MessageBox.Show("Bạn chắc muốn xóa dữ liệu?", "",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes
                )
                {
                    if (rowData != null)
                    {
                        bool success = false;
                        //hàm success bên dưới là kiểm tra có xóa thành công hay không?
                        //nếu thành công thì fill dữ liệu vào grid và load lại grid.
                        //gọi đến api: api/mosLanguage/Delete bên backend SDA
                        //api này xử lý ntn thì chưa rõ :))
                        success = new Inventec.Common.Adapter.BackendAdapter(param).
                        Post<bool>(HisRequestUriStore.MOSHIS_REGISTER_GATE_DELETE, ApiConsumers.MosConsumer, rowData.ID, param);

                        if (success != null)
                        {
                            FillDataToGridControl();
                            currentData = ((List<HIS_REGISTER_GATE>)gridControl1.DataSource).FirstOrDefault();
                        }
                        //hàm show ra thông tin có thành công hay không? 
                        MessageManager.Show(this, param, success);
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewRegisterGate_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    HIS_REGISTER_GATE data = (HIS_REGISTER_GATE)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "actIS_ACTIVE")
                    {
                        if (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
                            e.Appearance.ForeColor = Color.Red;

                        else
                            e.Appearance.ForeColor = Color.Green;
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        //private void gridViewRegisterGate_RowCountChanged(object sender, EventArgs e)
        //{
        //    var rowData = (MOS.EFMODEL.DataModels.HIS_REGISTER_GATE)gridViewRegisterGate.GetFocusedRow();
        //    if (rowData != null)
        //    {
        //        currentData = rowData;
        //        ChangeDataRow(rowData);
        //    }
        //}

        private void ChangeDataRow(HIS_REGISTER_GATE rowData)
        {
            try
            {
                if (rowData != null)
                {
                    //FillDataEditorControl(data);
                    txtCode.Text = currentData.REGISTER_GATE_CODE;
                    txtName.Text = currentData.REGISTER_GATE_NAME;
                    txtFORMAT.Text = currentData.FORMAT;
                    if (currentData.IS_RESET_AFTER_NOON == 1)
                    {
                        ckbReset.Checked = true;
                    }
                    else
                        ckbReset.Checked = false;
                    this.ActionType = GlobalVariables.ActionEdit;
                    EnableControlChanged(this.ActionType);
                    //btnEdit.Enabled = (this.currentData.IS_BASE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE);
                    btnEdit.Enabled = true;
                    positionHandler = -1;
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewRegisterGate_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    HIS_REGISTER_GATE data = (HIS_REGISTER_GATE)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "LOCK")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE ? btnGLock : btnGUnLock);
                    }
                    if (e.Column.FieldName == "DELETE")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? btnGDelete : btnGDeleteDisable);
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewRegisterGate_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var rowData = (MOS.EFMODEL.DataModels.HIS_REGISTER_GATE)gridViewRegisterGate.GetFocusedRow();
                    if (rowData != null)
                    {
                        currentData = rowData;
                        ChangeDataRow(rowData);
                    }
                }
                if (e.KeyCode == Keys.Down)
                {
                    (gridControl1.FocusedView as ColumnView).MoveNext();
                    var a = (gridControl1.FocusedView as ColumnView).FocusedRowHandle;
                    e.Handled = true;
                    var nextRow = (MOS.EFMODEL.DataModels.HIS_REGISTER_GATE)gridViewRegisterGate.GetFocusedRow();
                    if (nextRow != null)
                    {
                        currentData = nextRow;
                        ChangeDataRow(nextRow);
                    }
                }
                if (e.KeyCode == Keys.Up)
                {
                    (gridControl1.FocusedView as ColumnView).MovePrev();
                    var a = (gridControl1.FocusedView as ColumnView).FocusedRowHandle;
                    e.Handled = true;
                    var nextRow = (MOS.EFMODEL.DataModels.HIS_REGISTER_GATE)gridViewRegisterGate.GetFocusedRow();
                    if (nextRow != null)
                    {
                        currentData = nextRow;
                        ChangeDataRow(nextRow);
                    }
                }


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                SaveProcess();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                SaveProcess();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SaveProcess()
        {
            CommonParam param = new CommonParam();
            try
            {
                bool success = false;
                if (!btnAdd.Enabled && !btnEdit.Enabled)
                {
                    return;
                }
                positionHandler = -1;
                if (!dxValidationProvider1.Validate())
                {
                    return;
                }
                WaitingManager.Show();
                MOS.EFMODEL.DataModels.HIS_REGISTER_GATE updateDTO = new MOS.EFMODEL.DataModels.HIS_REGISTER_GATE();
                if (this.currentData != null && this.currentData.ID > 0)
                {
                    LoadCurrent(this.currentData.ID, ref updateDTO);
                }

                //ném ra thằng đã đc sửa(tương tự với hàm update, set giá trị khi sửa )
                //sau đó thằng mới sẽ đc gắn vào api create,upadte
                UpdateDTOFromDataForm(ref updateDTO);

                if (!string.IsNullOrEmpty(txtFORMAT.Text))
                {
                    updateDTO.FORMAT = txtFORMAT.Text;
                }
                else
                {
                    updateDTO.FORMAT = null;
                }
                if (ckbReset.Checked)
                    updateDTO.IS_RESET_AFTER_NOON = 1;
                else
                    updateDTO.IS_RESET_AFTER_NOON = null;
                if (ActionType == GlobalVariables.ActionAdd)
                {
                    updateDTO.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    //gọi ra api create
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_REGISTER_GATE>
                        (HisRequestUriStore.MOSHIS_REGISTER_GATE_CREATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToGridControl();
                        ResetFormData();
                    }
                }
                else
                {
                    updateDTO.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    //gọi ra api update
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_REGISTER_GATE>
                        (HisRequestUriStore.MOSHIS_REGISTER_GATE_UPDATE, ApiConsumers.MosConsumer, updateDTO, param);

                    if (resultData != null)
                    {
                        success = true;
                        FillDataToGridControl();
                    }

                }
                if (success)
                {
                    SetFocusEditor();
                }
                WaitingManager.Hide();
                #region Hien thi thong bao
                MessageManager.Show(this, param, success);
                #endregion

                #region Nếu phiên làm việc bị mất thì phần mềm tự động logout và trở về trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetFocusEditor()
        {
            try
            {
                //TODO

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void UpdateDTOFromDataForm(ref HIS_REGISTER_GATE updateDTO)
        {
            try
            {
                updateDTO.REGISTER_GATE_CODE = txtCode.Text.Trim();
                updateDTO.REGISTER_GATE_NAME = txtName.Text.Trim();
                updateDTO.FORMAT = txtCode.Text;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCurrent(long currentId, ref HIS_REGISTER_GATE updateDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisRegisterGateFilter filter = new HisRegisterGateFilter();
                filter.ID = currentId;
                updateDTO = new BackendAdapter(param).
                    Get<List<MOS.EFMODEL.DataModels.HIS_REGISTER_GATE>>(HisRequestUriStore.MOSHIS_REGISTER_GATE_GET, ApiConsumers.MosConsumer, filter, param)
                    .FirstOrDefault();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                EnableControlChanged(this.ActionType);
                positionHandler = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                ResetFormData();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewRegisterGate_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.HIS_REGISTER_GATE pData =
                        (MOS.EFMODEL.DataModels.HIS_REGISTER_GATE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    short status = Inventec.Common.TypeConvert.Parse.ToInt16((pData.IS_ACTIVE ?? -1).ToString());
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage;
                    }
                    else if (e.Column.FieldName == "actIS_ACTIVE")
                    {
                        try
                        {
                            if (status == 1)
                            {
                                e.Value = "Hoạt động";
                            }
                            else
                            {
                                e.Value = "Tạm khóa";
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn(ex);
                        }
                    }
                    else if (e.Column.FieldName == "IS_RESET_AFTER_NOON_STR")
                    {
                        e.Value = pData.IS_RESET_AFTER_NOON == 1 ? true : false;
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        try
                        {
                            //e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString((long)pData.CREATE_TIME);
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.CREATE_TIME ?? 0);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn(ex);
                        }
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.MODIFY_TIME ?? 0);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn(ex);
                        }
                    }
                }
                gridControl1.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewRegisterGate_Click(object sender, EventArgs e)
        {
            try
            {
                var rowData = (MOS.EFMODEL.DataModels.HIS_REGISTER_GATE)gridViewRegisterGate.GetFocusedRow();
                if (rowData != null)
                {
                    currentData = rowData;
                    ChangeDataRow(rowData);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnAdd_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void barButtonEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnEdit_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnReset_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnReset_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Down)
                {
                    txtName.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Down)
                {
                    ckbReset.Focus();
                }
                if (e.KeyCode == Keys.Up)
                {
                    txtCode.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ckbReset_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtFORMAT.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private void txtFORMAT_KeyPress(object sender, KeyPressEventArgs e)
        //{
        //    try
        //    {
        //        if (Char.GetNumericValue(e.KeyChar) == 0 || e.KeyChar == (char)Keys.Back)
        //        {
        //            e.Handled = true;
        //        }
                
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }

        //}

        private void txtFORMAT_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }

        private void txtFORMAT_KeyDown(object sender, KeyEventArgs e)
        {

            try
            {
                if (e.KeyCode != Keys.Control && e.KeyCode != Keys.NumPad0 && e.KeyCode != Keys.D0 && e.KeyCode != Keys.Delete && e.KeyCode != Keys.Back)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        //private void txtFORMAT_KeyUp(object sender, KeyEventArgs e)
        //{
        //    try
        //    {
        //        if (e.KeyCode != Keys.Control && e.KeyCode != Keys.NumPad0 && e.KeyCode != Keys.D0 && e.KeyCode != Keys.Delete)
        //        {
        //            e.Handled = true;
        //            e.SuppressKeyPress = true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}
    }
}
