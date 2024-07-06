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
using ACS.EFMODEL.DataModels;
using ACS.Filter;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraGrid.Views.Grid;
using ACS.Desktop.Plugins.AcsUser.ADO;
using ACS.Desktop.Plugins.AcsUser.RoleUser;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.Plugins.HisBornResult.Resources;
using System.Resources;
using ACS.Desktop.Plugins.AcsUser.Popup;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraBars;
using ACS.SDO;

namespace ACS.Desktop.Plugins.AcsUser
{
    public partial class frmAcsUser : Form
    {
        public frmAcsUser(Inventec.Desktop.Common.Modules.Module moduleData, Inventec.Common.WebApiClient.ApiConsumer sdaConsumer, Inventec.Common.WebApiClient.ApiConsumer acsConsumer, Action<Type> delegateRefresh, long numPageSize, string applicationCode, string iconPath)
        {
            InitializeComponent();
            this.moduleData = moduleData;
            SetIcon(iconPath);
            this.moduleData = moduleData;
            this.delegateRefresh = delegateRefresh;
            ConfigApplications.NumPageSize = numPageSize;
            GlobalVariables.APPLICATION_CODE = applicationCode;
            ApiConsumers.SdaConsumer = sdaConsumer;
            ApiConsumers.AcsConsumer = acsConsumer;
        }

        #region global
        ACS.EFMODEL.DataModels.ACS_USER currentData;
        ACS.EFMODEL.DataModels.ACS_USER currentCopyData;
        ACS.EFMODEL.DataModels.ACS_USER resultData;
        List<AcsUserADO> listAcsUserAdos;
        int positionHandle = -1;
        int ActionType = -1;
        private const short IS_ACTIVE_TRUE = 1;
        private const short IS_ACTIVE_FALSE = 0;
        int rowCount;
        int dataTotal;
        internal long id;
        int startPage;
        int limit;
        List<SDA.EFMODEL.DataModels.SDA_GROUP> selectGcode;
        DevExpress.XtraBars.BarManager barManager1;
        PopupMenuProcessor popupMenuProcessor;
        Action<Type> delegateRefresh;
        Inventec.Desktop.Common.Modules.Module moduleData;
        #endregion

        #region private first
        private void FillDataToGridControl()
        {
            WaitingManager.Show();

            int numPageSize = 0;
            if (ucPaging.pagingGrid != null)
            {
                numPageSize = ucPaging.pagingGrid.PageSize;
            }
            else
            {
                numPageSize = (int)ConfigApplications.NumPageSize;
            }

            LoadPaging(new CommonParam(0, numPageSize));

            CommonParam param = new CommonParam();
            param.Limit = rowCount;
            param.Count = dataTotal;
            //ucPaging.Init(loadPaging, param);
            ucPaging.Init(LoadPaging, param, numPageSize, this.gridControl1);

            WaitingManager.Hide();
        }

        private void SaveProcess()
        {
            if (!btnEdit.Enabled && !btnAdd.Enabled)
                return;

            positionHandle = -1;
            if (!dxValidationProvider.Validate())
                return;

            CommonParam param = new CommonParam();
            bool success = false;
            ACS_USER updateDTO = new ACS_USER();

            WaitingManager.Show();

            //bool str = txtCode.Text.IsNormalized(NormalizationForm.FormD);
            //if (!str)
            //{
            //	MessageBox.Show("Mã sai định dạng" + "\n" + "Không được nhập có dấu", "Thông báo");
            //	txtCode.Focus();
            //	txtCode.SelectAll();
            //	return;
            //}

            if (this.currentData != null && this.currentData.ID > 0)
            {
                AcsUserFilter filter = new AcsUserFilter();
                filter.ID = currentData.ID;
                updateDTO = new BackendAdapter(param).Get<List<ACS_USER>>
                  (HisRequestUriStore.ACS_USER_GET, ApiConsumers.AcsConsumer, filter, param).FirstOrDefault();
            }


            updateDTO.LOGINNAME = txtLoginName.Text.Trim();
            updateDTO.USERNAME = txtUserName.Text.Trim();
            updateDTO.EMAIL = txtEmail.Text.Trim();
            updateDTO.MOBILE = txtMobile.Text.Trim();
            //updateDTO.G_CODE = listAcsUserAdos.FirstOrDefault(o=>o.CHECKEDIT==true).G_CODE;
            updateDTO.G_CODE = (string)cbbGCode.EditValue;

            if (ActionType == GlobalVariables.ActionAdd)
            {
                updateDTO.IS_ACTIVE = IS_ACTIVE_TRUE;
                resultData = new BackendAdapter(param).Post<ACS_USER>
                  (HisRequestUriStore.ACS_USER_CREATE, ApiConsumers.AcsConsumer, updateDTO, param);
                if (resultData != null)
                {
                    success = true;
                    FillDataToGridControl();
                    ResetFormData();

                    if (this.delegateRefresh != null)
                    {
                        ACS_USER acsUser = new ACS_USER();
                        this.delegateRefresh(acsUser.GetType());
                    }
                }
            }

            else if (updateDTO != null)
            {
                resultData = new BackendAdapter(param).Post<ACS_USER>
                  (HisRequestUriStore.ACS_USER_UPDATE, ApiConsumers.AcsConsumer, updateDTO, param);
                if (resultData != null)
                {
                    success = true;
                    FillDataToGridControl();
                    if (this.delegateRefresh != null)
                    {
                        ACS_USER acsUser= new ACS_USER();
                        this.delegateRefresh(acsUser.GetType());
                    }
                }
            }

            WaitingManager.Hide();
            MessageManager.Show(this, param, success);
            txtLoginName.Focus();
            txtLoginName.SelectAll();
        }

        private void ChangedDataRow()
        {

            currentData = (ACS_USER)gridviewFormList.GetFocusedRow();
            //var sda = (SDA.EFMODEL.DataModels.SDA_GROUP)treeSdaGroup.GetDataRecordByNode(treeSdaGroup.FocusedNode);
            //var sda = selectGcode.Select(o => o.G_CODE).FirstOrDefault();      
            if (currentData != null)
            {
                txtLoginName.Text = currentData.LOGINNAME;
                txtUserName.Text = currentData.USERNAME;
                txtMobile.Text = currentData.MOBILE;
                txtEmail.Text = currentData.EMAIL;
                cbbGCode.EditValue = currentData.G_CODE;

                //if (click != null)
                //{
                //  LoadtreeSdaGroup(click);
                //}

                this.ActionType = GlobalVariables.ActionEdit;
                EnableControlChanged(this.ActionType);

                btnEdit.Enabled = (currentData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);
                positionHandle = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError
                  (dxValidationProvider, dxErrorProvider);
            }
            txtLoginName.Focus();
        }

        private void EnableControlChanged(int action)
        {
            btnAdd.Enabled = (action == GlobalVariables.ActionAdd);
            btnEdit.Enabled = (action == GlobalVariables.ActionEdit);
        }

        private void SetDefaultValue()
        {
            txtLoginName.Focus();
            txtLoginName.SelectAll();
            txtSearch.Text = "";
        }

        private void ResetFormData()
        {
            try
            {
                if (!lcEditInfo.IsInitialized) return;
                lcEditInfo.BeginUpdate();

                foreach (DevExpress.XtraLayout.BaseLayoutItem item in lcEditInfo.Items)
                {
                    DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                    if (lci != null && lci.Control != null && lci.Control is BaseEdit)
                    {
                        DevExpress.XtraEditors.BaseEdit formatFrm = lci.Control as DevExpress.XtraEditors.BaseEdit;
                        formatFrm.ResetText();
                        formatFrm.EditValue = null;
                        txtLoginName.Focus();
                        txtLoginName.Text = "";
                        txtUserName.Text = "";
                        txtEmail.Text = "";
                        txtMobile.Text = "";
                        FillDataToGridControl();
                        this.ActionType = GlobalVariables.ActionAdd;
                        EnableControlChanged(this.ActionType);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            finally
            {
                lcEditInfo.EndUpdate();
            }
        }

        private void ValidateForm()
        {
            try
            {
                ValidationSingleControl(txtLoginName);
                ValidationSingleControl(txtUserName);
                ValidationEmail(txtEmail);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void initCombo(Control cbbGCod)
        {
            try
            {
                CommonParam param = new CommonParam();
                SDA.Filter.SdaGroupFilter filter = new SDA.Filter.SdaGroupFilter();
                filter.KEY_WORD = cbbGCode.Text;

                var obj = new BackendAdapter(param).Get<List<SDA.EFMODEL.DataModels.SDA_GROUP>>
                  (HisRequestUriStore.SDA_GROUP_GET, ApiConsumers.SdaConsumer, filter, param);

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();

                columnInfos.Add(new ColumnInfo("G_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("GROUP_NAME", "", 250, 2));

                ControlEditorADO controlEditorADO = new ControlEditorADO
                  ("GROUP_NAME", "G_CODE", columnInfos, false, 350);
                ControlEditorLoader.Load(cbbGCod, obj, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void DefaultPass()
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                ACS_USER reset = (ACS_USER)gridviewFormList.GetFocusedRow();
                if (reset != null)
                {
                    //update.PASSWORD = update.LOGINNAME;

                    if (reset != null)
                    {
                        var obi = new BackendAdapter(param).Post<bool>
                          (HisRequestUriStore.ACS_USER_RESET, ApiConsumers.AcsConsumer, reset, param);

                        if (obi)
                        {
                            success = true;
                            FillDataToGridControl();
                        }
                    }
                }

                WaitingManager.Hide();
                MessageManager.Show(this, param, success);
                txtLoginName.Focus();
                txtLoginName.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                HIS.Desktop.Plugins.HisBornResult.Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("ACS.Desktop.Plugins.AcsUser.Resources.Lang", typeof(ACS.Desktop.Plugins.AcsUser.frmAcsUser).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmAcsUser.layoutControl1.Text", HIS.Desktop.Plugins.HisBornResult.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcEditInfo.Text = Inventec.Common.Resource.Get.Value("frmAcsUser.lcEditInfo.Text", HIS.Desktop.Plugins.HisBornResult.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cbbGCode.Properties.NullText = Inventec.Common.Resource.Get.Value("frmAcsUser.cbbGCode.Properties.NullText", HIS.Desktop.Plugins.HisBornResult.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar2.Text = Inventec.Common.Resource.Get.Value("frmAcsUser.bar2.Text", HIS.Desktop.Plugins.HisBornResult.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("frmAcsUser.barButtonItem1.Caption", HIS.Desktop.Plugins.HisBornResult.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem2.Caption = Inventec.Common.Resource.Get.Value("frmAcsUser.barButtonItem2.Caption", HIS.Desktop.Plugins.HisBornResult.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem3.Caption = Inventec.Common.Resource.Get.Value("frmAcsUser.barButtonItem3.Caption", HIS.Desktop.Plugins.HisBornResult.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem4.Caption = Inventec.Common.Resource.Get.Value("frmAcsUser.barButtonItem4.Caption", HIS.Desktop.Plugins.HisBornResult.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value("frmAcsUser.btnRefresh.Text", HIS.Desktop.Plugins.HisBornResult.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmAcsUser.btnAdd.Text", HIS.Desktop.Plugins.HisBornResult.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmAcsUser.btnEdit.Text", HIS.Desktop.Plugins.HisBornResult.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("frmAcsUser.layoutControlItem5.Text", HIS.Desktop.Plugins.HisBornResult.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("frmAcsUser.layoutControlItem6.Text", HIS.Desktop.Plugins.HisBornResult.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("frmAcsUser.layoutControlItem7.Text", HIS.Desktop.Plugins.HisBornResult.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("frmAcsUser.layoutControlItem8.Text", HIS.Desktop.Plugins.HisBornResult.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem19.Text = Inventec.Common.Resource.Get.Value("frmAcsUser.layoutControlItem19.Text", HIS.Desktop.Plugins.HisBornResult.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmAcsUser.layoutControl2.Text", HIS.Desktop.Plugins.HisBornResult.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmAcsUser.btnSearch.Text", HIS.Desktop.Plugins.HisBornResult.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn14.Caption = Inventec.Common.Resource.Get.Value("frmAcsUser.gridColumn14.Caption", HIS.Desktop.Plugins.HisBornResult.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmAcsUser.gridColumn1.Caption", HIS.Desktop.Plugins.HisBornResult.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmAcsUser.gridColumn2.Caption", HIS.Desktop.Plugins.HisBornResult.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmAcsUser.gridColumn3.Caption", HIS.Desktop.Plugins.HisBornResult.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmAcsUser.gridColumn4.Caption", HIS.Desktop.Plugins.HisBornResult.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmAcsUser.gridColumn5.Caption", HIS.Desktop.Plugins.HisBornResult.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("frmAcsUser.gridColumn6.Caption", HIS.Desktop.Plugins.HisBornResult.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("frmAcsUser.gridColumn7.Caption", HIS.Desktop.Plugins.HisBornResult.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("frmAcsUser.gridColumn8.Caption", HIS.Desktop.Plugins.HisBornResult.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("frmAcsUser.gridColumn9.Caption", HIS.Desktop.Plugins.HisBornResult.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("frmAcsUser.gridColumn10.Caption", HIS.Desktop.Plugins.HisBornResult.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("frmAcsUser.gridColumn11.Caption", HIS.Desktop.Plugins.HisBornResult.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn12.Caption = Inventec.Common.Resource.Get.Value("frmAcsUser.gridColumn12.Caption", HIS.Desktop.Plugins.HisBornResult.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn13.Caption = Inventec.Common.Resource.Get.Value("frmAcsUser.gridColumn13.Caption", HIS.Desktop.Plugins.HisBornResult.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                if (this.moduleData != null && !String.IsNullOrEmpty(this.moduleData.text))
                {
                    this.Text = this.moduleData.text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetIcon(string iconPath)
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon
                  (iconPath);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
        #endregion

        #region private second
        private void LoadPaging(object param)
        {
            startPage = ((CommonParam)param).Start ?? 0;
            limit = ((CommonParam)param).Limit ?? 0;

            CommonParam paramCommon = new CommonParam(startPage, limit);

            ACS.Filter.AcsUserFilter filterSearch = new AcsUserFilter();
            filterSearch.KEY_WORD = txtSearch.Text.Trim();
            filterSearch.ORDER_DIRECTION = "DESC";
            filterSearch.ORDER_FIELD = "MODIFY_TIME";

            Inventec.Core.ApiResultObject<List<ACS.EFMODEL.DataModels.ACS_USER>> apiResult = null;
            apiResult = new BackendAdapter(paramCommon).GetRO<List<ACS.EFMODEL.DataModels.ACS_USER>>
              (HisRequestUriStore.ACS_USER_GET, ApiConsumers.AcsConsumer, filterSearch, paramCommon);
            if (apiResult != null)
            {
                var data = (List<ACS.EFMODEL.DataModels.ACS_USER>)apiResult.Data;
                gridviewFormList.GridControl.DataSource = data;
                rowCount = (data == null ? 0 : data.Count);
                dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
            }
        }

        private void ValidationSingleControl(BaseEdit control)
        {
            ControlEditValidationRule validRule = new ControlEditValidationRule();
            validRule.editor = control;
            validRule.ErrorText = "Trường dữ liệu bắt buộc";
            validRule.ErrorType = ErrorType.Warning;
            dxValidationProvider.SetValidationRule(control, validRule);
        }

        private void ValidationEmail(TextEdit control)
        {
            try
            {
                ValidateEmail validRule = new ValidateEmail();
                validRule.txt = control;
                validRule.ErrorText = "E-mail sai định dạng";
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region event
        private void frmAcsUser_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                FillDataToGridControl();
                initCombo(cbbGCode);
                this.ActionType = GlobalVariables.ActionAdd;
                EnableControlChanged(this.ActionType);
                ValidateForm();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridviewFormList_Click(object sender, EventArgs e)
        {
            try
            {
                ChangedDataRow();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridviewFormList_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    ACS_USER AcsUser = (ACS_USER)gridviewFormList.GetRow(e.RowHandle);
                    if (e.Column.FieldName == "LOCK")
                    {
                        e.RepositoryItem = AcsUser.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? btnUnlock :
                          btnLock;
                    }
                    if (e.Column.FieldName == "DEFAULTPASS")
                    {
                        e.RepositoryItem = btnDefaultPass;
                    }
                    if (e.Column.FieldName == "ROLEUSER")
                    {
                        e.RepositoryItem = btnRoleUser;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridviewFormList_CustomUnboundColumnData(object sender,
          DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                var data = (ACS_USER)gridviewFormList.GetRow(e.ListSourceRowIndex);
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;

                if (e.Column.FieldName == "STT")
                    e.Value = e.ListSourceRowIndex + 1 + startPage;
                if (e.Column.FieldName == "STATUS")
                    e.Value = data.IS_ACTIVE == IS_ACTIVE_TRUE ? "đang hoạt động" : "đã khóa";
                if (e.Column.FieldName == "CREATE_TIME_STR")
                {
                    string createTime = (view.GetRowCellValue(e.ListSourceRowIndex, "CREATE_TIME") ?? "").ToString();
                    e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString
                      (Inventec.Common.TypeConvert.Parse.ToInt64(createTime));
                }
                if (e.Column.FieldName == "MODIFY_TIME_STR")
                {
                    string mobdifyTime = (view.GetRowCellValue(e.ListSourceRowIndex, "MODIFY_TIME") ?? "").ToString();
                    e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString
                      (Inventec.Common.TypeConvert.Parse.ToInt64(mobdifyTime));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridviewFormList_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                var data = (ACS_USER)gridviewFormList.GetRow(e.RowHandle);
                if (e.Column.FieldName == "STATUS")
                    e.Appearance.ForeColor = data.IS_ACTIVE == IS_ACTIVE_TRUE ? Color.Green : Color.Red;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void btnLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool notHandler = false;

                var currentLock = (ACS_USER)gridviewFormList.GetFocusedRow();
                if (MessageBox.Show("Bạn có muốn bỏ khóa dữ liệu không?",
                  "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    var resultLock = new BackendAdapter(param).Post<ACS_USER>
                      (HisRequestUriStore.ACS_USER_CHANGELOCK, ApiConsumers.AcsConsumer, currentLock, param);

                    if (resultLock != null)
                    {
                        notHandler = true;
                        FillDataToGridControl();
                        if (this.delegateRefresh != null)
                        {
                            ACS_USER acsUser = new ACS_USER();
                            this.delegateRefresh(acsUser.GetType());
                        }
                    }

                    MessageManager.Show(this.ParentForm, param, notHandler);
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnUnlock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool notHandler = false;

                var currentUnlock = (ACS_USER)gridviewFormList.GetFocusedRow();
                if (MessageBox.Show("Bạn có muốn khóa dữ liệu không?",
                  "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    var resultUnlock = new BackendAdapter(param).Post<ACS_USER>(HisRequestUriStore.ACS_USER_CHANGELOCK,
                      ApiConsumers.AcsConsumer,
                      currentUnlock, param);

                    if (resultUnlock != null)
                    {
                        notHandler = true;
                        FillDataToGridControl();
                        if (this.delegateRefresh != null)
                        {
                            ACS_USER acsUser = new ACS_USER();
                            this.delegateRefresh(acsUser.GetType());
                        }
                    }

                    MessageManager.Show(this.ParentForm, param, notHandler);
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnDefaultPass_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Bạn có muốn reset mật khẩu không?",
                  "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    DefaultPass();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                ResetFormData();
                //positionHandle = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError
                  (dxValidationProvider, dxErrorProvider);
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

        private void btnRoleUser_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                currentData = (ACS_USER)gridviewFormList.GetFocusedRow();
                frmRoleUsers frm = new frmRoleUsers(currentData);
                frm.Show();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void barButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnRefresh_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtLoginName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtUserName.Focus();
                    txtUserName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtUserName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtEmail.Focus();
                    txtEmail.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtEmail_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtMobile.Focus();
                    txtMobile.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMobile_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cbbGCode.Focus();

                    cbbGCode.ShowPopup();

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSearch_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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

        private void cbbGCode_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (this.ActionType == GlobalVariables.ActionAdd)
                {
                    btnAdd.Focus();
                }
                else
                {
                    btnEdit.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cbbGCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.ActionType == GlobalVariables.ActionAdd)
                    {
                        btnAdd.Focus();
                    }
                    else
                    {
                        btnEdit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridviewFormList_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            try
            {

                GridHitInfo hi = e.HitInfo;
                if (hi.InRowCell)
                {
                    if (this.barManager1 == null)
                        this.barManager1 = new DevExpress.XtraBars.BarManager();
                    this.barManager1.Form = this;
                    popupMenuProcessor = new PopupMenuProcessor(this.barManager1, GridView_MouseRightClick);
                    this.popupMenuProcessor.InitMenu();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GridView_MouseRightClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if ((e.Item is BarButtonItem))
                {
                    var type = (PopupMenuProcessor.ItemType)e.Item.Tag;
                    switch (type)
                    {
                        case PopupMenuProcessor.ItemType.Copy:
                            {
                                this.currentCopyData = (ACS_USER)gridviewFormList.GetFocusedRow();
                                break;
                            }
                        case PopupMenuProcessor.ItemType.Paste:
                            {
                                Paste();
                                break;
                            }
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Paste()
        {
            try
            {
                var pasteLoginname = (ACS_USER)gridviewFormList.GetFocusedRow();
                if (this.currentCopyData == null)
                {
                    MessageManager.Show("Chưa copy!");
                    return;
                }

                if (this.currentCopyData != null && pasteLoginname != null && this.currentCopyData.LOGINNAME == pasteLoginname.LOGINNAME)
                {
                    MessageManager.Show("Trùng dữ liệu copy và paste!");
                    return;
                }
                WaitingManager.Show();
                bool succces = false;
                CommonParam param = new CommonParam();
                UserRoleCopySDO userRoleCopySDO = new UserRoleCopySDO();
                userRoleCopySDO.CopyLoginname = this.currentCopyData.LOGINNAME;
                userRoleCopySDO.PasteLoginname = pasteLoginname.LOGINNAME;
                var result = new BackendAdapter(param).Post<List<ACS_ROLE_USER>>("api/AcsUser/CopyRole", ApiConsumers.AcsConsumer, userRoleCopySDO, param);

                if (result != null)
                {
                    succces = true;
                    FillDataToGridControl();
                    ResetFormData();
                    if (this.delegateRefresh != null)
                    {
                        ACS_USER acsUser = new ACS_USER();
                        this.delegateRefresh(acsUser.GetType());
                    }
                }
                WaitingManager.Hide();
                MessageManager.Show(this, param, succces);
                txtLoginName.Focus();
                txtLoginName.SelectAll();
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
                DevExpress.XtraEditors.XtraMessageBox.Show("Chức năng chưa được hoàn thiện", "Thông báo");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
    }
}
