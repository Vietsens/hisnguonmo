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
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraNavBar;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using Inventec.Desktop.Common.Controls.ValidationRule;
using DevExpress.XtraEditors.DXErrorProvider;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;
using ACS.EFMODEL.DataModels;
using ACS.Filter;
using ACS.Desktop.Plugins.AcsRoleBase;

namespace ACS.Desktop.Plugins.AcsRoleBase
{
    public partial class frmAcsRoleBase : HIS.Desktop.Utility.FormBase
    {
        #region Declare

        List<long> listbase = new List<long>();
        List<ACS_ROLE_BASE> ListRoleBase = new List<ACS_ROLE_BASE>();
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        PagingGrid pagingGrid;
        int ActionType = -1;
        int positionHandle = -1;
        long RoleID = 0;
        ACS_ROLE currentData;
        List<string> arrControlEnableNotChange = new List<string>();
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        Inventec.Desktop.Common.Modules.Module moduleData;
        Action<Type> delegateRefresh;
        List<RoleBaseADO> ListData = new List<RoleBaseADO>();
        List<RoleBaseADO> ListDataOld = new List<RoleBaseADO>();
        #endregion

        #region Construct
        public frmAcsRoleBase(Inventec.Desktop.Common.Modules.Module moduleData, Inventec.Common.WebApiClient.ApiConsumer sdaConsumer, Inventec.Common.WebApiClient.ApiConsumer acsConsumer, long numPageSize, string applicationCode, string iconPath, long roleID)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();

                this.moduleData = moduleData;
                this.delegateRefresh = delegateRefresh;
                ConfigApplications.NumPageSize = numPageSize;
                GlobalVariables.APPLICATION_CODE = applicationCode;
                ApiConsumers.SdaConsumer = sdaConsumer;
                ApiConsumers.AcsConsumer = acsConsumer;
                this.RoleID = roleID;
                pagingGrid = new PagingGrid();
                this.moduleData = moduleData;
                gridControlFormList.ToolTipController = toolTipControllerGrid;

                try
                {
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

        #region Private method
        private void frmAcsRoleBase_Load(object sender, EventArgs e)
        {
            try
            {
                MeShow();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {  ////Khoi tao doi tuong resource

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

        /// <summary>
        /// Gan focus vao control mac dinh
        /// </summary>
        private void SetDefaultFocus()
        {
            try
            {
                txtKeyword.Focus();
                txtKeyword.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void InitTabIndex()
        {
            try
            {
                //dicOrderTabIndexControl.Add("txtBedCode", 0);
                //dicOrderTabIndexControl.Add("txtBedName", 1);
                //dicOrderTabIndexControl.Add("lkRoomId", 2);


                //if (dicOrderTabIndexControl != null)
                //{
                //    foreach (KeyValuePair<string, int> itemOrderTab in dicOrderTabIndexControl)
                //    {
                //        SetTabIndexToControl(itemOrderTab, lcEditorInfo);
                //    }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool SetTabIndexToControl(KeyValuePair<string, int> itemOrderTab, DevExpress.XtraLayout.LayoutControl layoutControlEditor)
        {
            bool success = false;
            try
            {
                if (!layoutControlEditor.IsInitialized) return success;
                layoutControlEditor.BeginUpdate();
                try
                {
                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in layoutControlEditor.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null)
                        {
                            BaseEdit be = lci.Control as BaseEdit;
                            if (be != null)
                            {
                                //Cac control dac biet can fix khong co thay doi thuoc tinh enable
                                if (itemOrderTab.Key.Contains(be.Name))
                                {
                                    be.TabIndex = itemOrderTab.Value;
                                }
                            }
                        }
                    }
                }
                finally
                {
                    layoutControlEditor.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return success;
        }

        private void FillDataToControlsForm()
        {
            try
            {
                //InitComboBedTypeId();
                //InitComboBedRoomId();

                //TODO
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Init combo



        #endregion

        /// <summary>
        /// Ham lay du lieu theo dieu kien tim kiem va gan du lieu vao danh sach
        /// </summary>
        public void FillDataToGridControl()
        {
            try
            {
                try
                {
                    CommonParam paramCommon = new CommonParam();
                    AcsRoleFilter filter = new AcsRoleFilter();

                    SetFilterNavBar(ref filter);
                    filter.ORDER_DIRECTION = "DESC";
                    filter.ORDER_FIELD = "MODIFY_TIME";
                    gridviewFormList.BeginUpdate();
                    var apiResult = new BackendAdapter(paramCommon).Get<List<ACS_ROLE>>(AcsRequestUriStore.ACS_ROLE_GET, ApiConsumers.AcsConsumer, filter, paramCommon);

                    ListData = new List<RoleBaseADO>();
                    if (apiResult != null)
                    {
                        foreach (var item in apiResult)
                        {
                            RoleBaseADO x = new RoleBaseADO();
                            x.ID = item.ID;
                            x.IsBase = false;
                            x.ROLE_CODE = item.ROLE_CODE;
                            x.ROLE_NAME = item.ROLE_NAME;
                            ListData.Add(x);

                            foreach (var item2 in listbase)
                            {
                                if (item.ID == item2)
                                {
                                    x.IsBase = true;
                                }
                            }
                        }
                        if (ListData != null)
                        {
                            gridviewFormList.GridControl.DataSource = ListData.OrderByDescending(o => o.IsBase);
                        }
                    }
                    gridviewFormList.EndUpdate();

                }
                catch (Exception ex)
                {
                    LogSystem.Error(ex);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void LoadAcsEoleBase(ref List<long> listbase)
        {
            CommonParam paramCommon = new CommonParam();
            AcsRoleBaseFilter filter = new AcsRoleBaseFilter();
            filter.ROLE_ID = RoleID;
            ListRoleBase = new BackendAdapter(paramCommon).Get<List<ACS_ROLE_BASE>>("api/AcsRoleBase/Get", ApiConsumers.AcsConsumer, filter, paramCommon);
            foreach (var item in ListRoleBase)
            {
                listbase.Add(item.ROLE_BASE_ID);
            }
        }

        private void SetFilterNavBar(ref AcsRoleFilter filter)
        {
            try
            {
                filter.KEY_WORD = txtKeyword.Text.Trim();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void txtKeyWord_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                }
                else if (e.KeyCode == Keys.Down)
                {
                    gridviewFormList.Focus();
                    gridviewFormList.FocusedRowHandle = 0;
                    var rowData = (ACS_ROLE)gridviewFormList.GetFocusedRow();
                    if (rowData != null)
                    {
                        ChangedDataRow(rowData);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeyword_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                }
                else if (e.KeyCode == Keys.Down)
                {
                    gridviewFormList.Focus();
                    gridviewFormList.FocusedRowHandle = 0;
                    var rowData = (ACS_ROLE)gridviewFormList.GetFocusedRow();
                    if (rowData != null)
                    {
                        ChangedDataRow(rowData);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridviewFormList_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    RoleBaseADO pData = (RoleBaseADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    short status = Inventec.Common.TypeConvert.Parse.ToInt16((pData.IS_ACTIVE ?? -1).ToString());
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage; //+ ((pagingGrid.CurrentPage - 1) * pagingGrid.PageSize);
                    }
                    else if (e.Column.FieldName == "grclCheck")
                    {
                        if (pData.IsBase)
                        {
                            e.Value = true;
                        }
                    }

                    gridControlFormList.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridControlFormList_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var rowData = (ACS_ROLE)gridviewFormList.GetFocusedRow();
                if (rowData != null)
                {
                    currentData = rowData;
                    ChangedDataRow(rowData);

                    //Set focus vào control editor đầu tiên
                    SetFocusEditor();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridviewFormList_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var rowData = (ACS_ROLE)gridviewFormList.GetFocusedRow();
                    if (rowData != null)
                    {
                        ChangedDataRow(rowData);

                        //Set focus vào control editor đầu tiên
                        SetFocusEditor();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dnNavigation_PositionChanged(object sender, EventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangedDataRow(ACS_ROLE data)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToEditorControl(ACS_ROLE data)
        {
            try
            {
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Gan focus vao control mac dinh
        /// </summary>
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



        private void LoadCurrent(long currentId, ref ACS_ROLE currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                AcsRoleFilter filter = new AcsRoleFilter();
                filter.ID = currentId;
                currentDTO = new BackendAdapter(param).Get<List<ACS_ROLE>>(AcsRequestUriStore.ACS_ROLE_GET, ApiConsumers.AcsConsumer, filter, param).FirstOrDefault();
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
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProvider_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Button handler
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
        private void SaveProcess()
        {
            try
            {

                #region Hien thi message thong bao
                // MessageManager.Show(this, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                // SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateRowDataAfterEdit(ACS_ROLE data)
        {
            try
            {
                if (data == null)
                    throw new ArgumentNullException("data(ACS_ROLE) is null");
                var rowData = (ACS_ROLE)gridviewFormList.GetFocusedRow();
                if (rowData != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<ACS_ROLE>(rowData, data);
                    gridviewFormList.RefreshRow(gridviewFormList.FocusedRowHandle);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        #endregion

        #region Validate
        private void ValidateForm()
        {
            try
            {


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateLookupWithTextEdit(LookUpEdit cbo, TextEdit textEdit)
        {
            try
            {
                LookupEditWithTextEditValidationRule validRule = new LookupEditWithTextEditValidationRule();
                validRule.txtTextEdit = textEdit;
                validRule.cbo = cbo;
                validRule.ErrorText = "Trường dữ liệu bắt buộc";
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(textEdit, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private void ValidateGridLookupWithTextEdit(GridLookUpEdit cbo, TextEdit textEdit)
        //{
        //    try
        //    {
        //        GridLookupEditWithTextEditValidationRule validRule = new GridLookupEditWithTextEditValidationRule();
        //        validRule.txtTextEdit = textEdit;
        //        validRule.cbo = cbo;
        //        validRule.ErrorText = MessageUtil.GetMessage(THE.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
        //        validRule.ErrorType = ErrorType.Warning;
        //        dxValidationProviderEditorInfo.SetValidationRule(textEdit, validRule);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        private void ValidationSingleControl(BaseEdit control)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = "Trường dữ liệu bắt buộc";
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Tooltip
        private void toolTipControllerGrid_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                //TODO

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #endregion

        #region Public method
        public void MeShow()
        {
            try
            {
                LoadAcsEoleBase(ref listbase);
                //Set enable control default
                EnableControlChanged(this.ActionType);

                //Fill data into datasource combo
                FillDataToControlsForm();

                //Load du lieu
                FillDataToGridControl();

                //Load ngon ngu label control
                SetCaptionByLanguageKey();

                //Set tabindex control
                InitTabIndex();

                //Set validate rule
                ValidateForm();

                //Focus default
                SetDefaultFocus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Shortcut
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






        private void bbtnFocusDefault_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                txtKeyword.Focus();
                txtKeyword.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void unLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            ACS_ROLE success = new ACS_ROLE();
            //bool notHandler = false;
            try
            {

                ACS_ROLE data = (ACS_ROLE)gridviewFormList.GetFocusedRow();
                if (MessageBox.Show("Bạn có muốn khóa dữ liệu không", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    ACS_ROLE data1 = new ACS_ROLE();
                    data1.ID = data.ID;
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<ACS_ROLE>(AcsRequestUriStore.ACS_ROLE_CHANGELOCK, ApiConsumers.AcsConsumer, data1, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        FillDataToGridControl();
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Lock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            ACS_ROLE success = new ACS_ROLE();
            //bool notHandler = false;
            try
            {

                ACS_ROLE data = (ACS_ROLE)gridviewFormList.GetFocusedRow();
                if (MessageBox.Show("Bạn có muốn bỏ khóa dữ liệu không", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    ACS_ROLE data1 = new ACS_ROLE();
                    data1.ID = data.ID;
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<ACS_ROLE>(AcsRequestUriStore.ACS_ROLE_CHANGELOCK, ApiConsumers.AcsConsumer, data1, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        FillDataToGridControl();
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridviewFormList_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {

                    ACS_ROLE data = (ACS_ROLE)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "IS_LOCK")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? unLock : Lock);

                    }

                    if (e.Column.FieldName == "Delete")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? btnGEdit : Delete);

                    }
                    if (e.Column.FieldName == "IS_FULL_CH")
                    {
                        e.RepositoryItem = Check;
                    }
                    if (e.Column.FieldName == "ROLE_BASE_bt")
                    {
                        e.RepositoryItem = btnShowBase;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridviewFormList_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    ACS_ROLE data = (ACS_ROLE)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "IS_ACTIVE_STR")
                    {
                        if (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
                            e.Appearance.ForeColor = Color.Red;
                        else
                            e.Appearance.ForeColor = Color.Green;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void txtSampleRoomCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                }//e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            FillDataToGridControl();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            //listbase
            var datacheck = (from o in ListData where o.IsBase == true select o.ID).ToList();
            var datauncheck = (from o in ListData where o.IsBase == false select o.ID).ToList();
            //tạo danh tạo
            var datacreate = (from o in datacheck where !listbase.Contains(o) select o).ToList();
            //tạo danh sách xóa
            var datadelete = (from o in datauncheck where listbase.Contains(o) select o).ToList();
            bool succes = false;



            if (datacreate.Count > 0)
            {
                List<ACS_ROLE_BASE> Listcreate = new List<ACS_ROLE_BASE>();
                foreach (var item in datacreate)
                {
                    ACS_ROLE_BASE RoleBase = new ACS_ROLE_BASE();
                    RoleBase.ROLE_ID = RoleID;
                    RoleBase.ROLE_BASE_ID = item;
                    Listcreate.Add(RoleBase);
                }
                var apiresult = new BackendAdapter(param).Post<bool>("api/AcsRoleBase/CreateList", ApiConsumers.AcsConsumer, Listcreate, param);
                succes = true;
            }
            if (datadelete.Count > 0)
            {
                List<long> ListDelete = new List<long>();
                foreach (var item in ListRoleBase)
                {
                    foreach (var item2 in datadelete)
                    {
                        if (item.ROLE_BASE_ID == item2)
                        {
                            ListDelete.Add(item.ID);
                        }
                    }
                }
                var apiresult = new BackendAdapter(param).Post<bool>("api/AcsRoleBase/DeleteList", ApiConsumers.AcsConsumer, ListDelete, param);
                if (apiresult)
                {
                    succes = true;
                }
            }
            if (datadelete.Count == 0 && datacreate.Count == 0)
            {
                MessageManager.Show("Chưa có thay đôi.");
                return;
            }
            if (succes)
            {
                this.Close();

            }
            MessageManager.Show(this, param, succes);
        }

        private void Check_Click(object sender, EventArgs e)
        {
            var row = (RoleBaseADO)gridviewFormList.GetFocusedRow();
            if (row.IsBase)
            {
                row.IsBase = false;
            }
            else
            {
                row.IsBase = true;
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSave.Focus();
            btnSave_Click(null, null);
        }
    }
}
