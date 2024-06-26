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
using QCS.Desktop.ApiConsumer;
using QCS.Desktop.Common;
using QCS.Desktop.Controls.Session;
using QCS.Desktop.LibraryMessage;
using QCS.Desktop.LocalStorage.BackendData;
using QCS.Desktop.LocalStorage.ConfigApplication;
using QCS.Desktop.LocalStorage.LocalData;
using QCS.Desktop.Utilities;
using QCS.EFMODEL.DataModels;
using QCS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace QCS.Desktop.Plugins.QcsQuery.QcsQueryList
{
    public partial class UCQcsQueryList : UserControl, IControlCallBack
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        PagingGrid pagingGrid;
        int ActionType = -1;
        int positionHandle = -1;
        QCS.EFMODEL.DataModels.V_QCS_QUERY currentQcsQuery;
        List<string> arrControlEnableNotChange = new List<string>();
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        #endregion

        #region Construct
        public UCQcsQueryList()
        {
            try
            {
                InitializeComponent();
                pagingGrid = new PagingGrid();
                gridControlQcsQueryList.ToolTipController = toolTipControllerGrid;
                MeShow();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Private method

        void SetCaptionByLanguageKey()
        {
            try
            {
                //navTitleCreateTime.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LANGUAGEUCPATIENTLIST_NAV_TITLE_CREATE_TIME", QCS.Desktop.Resources.ResourceLanguageManager.LanguageUCPatientList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                //lciFromTime.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LANGUAGEUCPATIENTLIST_LCI_FROM_TIME", QCS.Desktop.Resources.ResourceLanguageManager.LanguageUCPatientList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                //lciToTime.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LANGUAGEUCPATIENTLIST_LCI_TO_TIME", QCS.Desktop.Resources.ResourceLanguageManager.LanguageUCPatientList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());                                 
                //btnSearch.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LANGUAGEUCPATIENTLIST_BTN_SEARCH", QCS.Desktop.Resources.ResourceLanguageManager.LanguageUCPatientList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                //btnRefesh.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LANGUAGEUCPATIENTLIST_BTN_REFESH", QCS.Desktop.Resources.ResourceLanguageManager.LanguageUCPatientList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());              
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultValue()
        {
            try
            {
                this.ActionType = GlobalVariables.ActionView;

                DateTime FirstDayOfNow = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                dtFromTime.DateTime = FirstDayOfNow;
                dtToTime.DateTime = DateTime.Now;
                txtKeyword.Text = "";
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
				//TODO
               
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
				dicOrderTabIndexControl.Add("txtSql", 0);
dicOrderTabIndexControl.Add("txtDescription", 1);

                //dicOrderTabIndexControl.Add("txtPatientCode", 0);
                //dicOrderTabIndexControl.Add("txtPatientName", 1);

                if (dicOrderTabIndexControl != null)
                {
                    foreach (KeyValuePair<string, int> itemOrderTab in dicOrderTabIndexControl)
                    {
                        SetTabIndexToControl(itemOrderTab, lcEditorInfo);
                    }
                }
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
				txtSql.Text = data.SQL;
txtDescription.Text = data.DESCRIPTION;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Ham lay du lieu theo dieu kien tim kiem va gan du lieu vao danh sach
        /// </summary>
        public void FillDataToGridControl()
        {
            try
            {
                WaitingManager.Show();

                LoadPaging(new CommonParam(0, (int)ConfigApplications.NumPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPagingForQcsQuery.Init(LoadPaging, param);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        /// <summary>
        /// Ham goi api lay du lieu phan trang
        /// </summary>
        /// <param name="param"></param>
        private void LoadPaging(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                Inventec.Core.ApiResultObject<List<QCS.EFMODEL.DataModels.V_QCS_QUERY>> apiResult = null;
                QcsQueryViewFilter filter = new QcsQueryViewFilter();
                SetFilterTimeFromToNavBar(ref filter);
                dnNavigationQcsQuery.DataSource = null;
                gridviewQcsQueryList.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<V_QCS_QUERY>>(QcsRequestUriStore.QCS_QUERY_GETVIEW, ApiConsumers.QcsConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                     var data = (List<QCS.EFMODEL.DataModels.V_QCS_QUERY>)apiResult.Data;
                    if (data != null)
                    {
                        dnNavigationQcsQuery.DataSource = data;
                        gridviewQcsQueryList.GridControl.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                gridviewQcsQueryList.EndUpdate();

                #region Process has exception
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetFilterTimeFromToNavBar(ref QcsQueryViewFilter filter)
        {
            try
            {
                filter.KEY_WORD = txtKeyword.Text.Trim();
                if (dtFromTime != null && dtFromTime.DateTime != DateTime.MinValue)
                    filter.CREATE_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtFromTime.EditValue).ToString("yyyyMMdd") + "000000");
                if (dtToTime != null && dtToTime.DateTime != DateTime.MinValue)
                    filter.CREATE_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtToTime.EditValue).ToString("yyyyMMdd") + "232359");
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        //private void SetFilterFromGenderNavBar(NavBarControl navBarFilter, ref SdcPatientViewFilter filter)
        //{
        //    try
        //    {
        //        if (navBarFilter.Controls.Count > 0)
        //        {
        //            for (int i = 0; i < navBarFilter.Controls.Count; i++)
        //            {
        //                if (navBarFilter.Controls[i] is DevExpress.XtraNavBar.NavBarGroupControlContainer)
        //                {
        //                    continue;
        //                }
        //                if (navBarFilter.Controls[i] is DevExpress.XtraNavBar.NavBarGroupControlContainerWrapper)
        //                {
        //                    var groupWrapper = navBarFilter.Controls[i] as DevExpress.XtraNavBar.NavBarGroupControlContainerWrapper;
        //                    foreach (DevExpress.XtraNavBar.NavBarGroupControlContainer group in groupWrapper.Controls)
        //                    {
        //                        foreach (var itemCheckEdit in group.Controls)
        //                        {
        //                            if (itemCheckEdit is CheckEdit)
        //                            {
        //                                var checkEdit = itemCheckEdit as CheckEdit;
        //                                if (checkEdit.Checked)
        //                                {
        //                                    if (filter.GENDER_IDs == null)
        //                                        filter.GENDER_IDs = new List<long>();
        //                                    filter.GENDER_IDs.Add(Inventec.Common.TypeConvert.Parse.ToInt64(checkEdit.Name));
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogSystem.Error(ex);
        //    }
        //}

        private void txtKeyword_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridviewQcsQueryList_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    QCS.EFMODEL.DataModels.V_QCS_QUERY pData = (QCS.EFMODEL.DataModels.V_QCS_QUERY)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage; //+ ((pagingGrid.CurrentPage - 1) * pagingGrid.PageSize);
                    }

                    
                    gridControlQcsQueryList.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dnNavigationQcsQuery_PositionChanged(object sender, EventArgs e)
        {
            try
            {
                this.currentQcsQuery = (QCS.EFMODEL.DataModels.V_QCS_QUERY)(gridControlQcsQueryList.DataSource as List<QCS.EFMODEL.DataModels.V_QCS_QUERY>)[dnNavigationQcsQuery.Position];
                if (this.currentQcsQuery != null)
                {
                    FillDataToEditorControl(this.currentQcsQuery);
                    this.ActionType = GlobalVariables.ActionView;
                    EnableControlChanged(this.ActionType);
                    btnEdit.Enabled = (this.currentQcsQuery.IS_ACTIVE == IMSys.DbConfig.QCS_RS.COMMON.IS_ACTIVE__TRUE);
                    positionHandle = -1;
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderab1, dxErrorProviderab1);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToEditorControl(QCS.EFMODEL.DataModels.V_QCS_QUERY patientData)
        {
            try
            {
                txtSql.Text = data.SQL;
txtDescription.Text = data.DESCRIPTION;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCurrent(long patientId, ref QCS.EFMODEL.DataModels.V_QCS_QUERY currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                QcsQueryViewFilter filter = new QcsQueryViewFilter();
                filter.ID = patientId;
                currentDTO = new BackendAdapter(param).Get<List<V_QCS_QUERY>>(QcsRequestUriStore.QCS_QUERY_GETVIEW, ApiConsumers.QcsConsumer, filter, param).FirstOrDefault();
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
                UpdateItemsReadOnly(lcEditorInfo);
                btnEdit.Enabled = (action == GlobalVariables.ActionView);
                btnCancel.Enabled = (action == GlobalVariables.ActionEdit);
                btnSave.Enabled = (action == GlobalVariables.ActionAdd || action == GlobalVariables.ActionEdit);
                btnAdd.Enabled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateItemsReadOnly(Control controlContainer)
        {
            try
            {
                if (controlContainer is DevExpress.XtraLayout.LayoutControl)
                {
                    DevExpress.XtraLayout.LayoutControl layoutControlEditor = controlContainer as DevExpress.XtraLayout.LayoutControl;
                    bool isAllowEditing = !(ActionType == GlobalVariables.ActionView);
                    if (!layoutControlEditor.IsInitialized) return;
                    layoutControlEditor.BeginUpdate();
                    try
                    {
                        foreach (DevExpress.XtraLayout.BaseLayoutItem item in layoutControlEditor.Items)
                        {
                            DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                            if (lci != null && lci.Control != null)
                            {
                                if (lci.Control != null)
                                {
                                    //Cac control dac biet can fix khong co thay doi thuoc tinh enable
                                    if (arrControlEnableNotChange != null && arrControlEnableNotChange.Contains(lci.Control.Name))
                                    {
                                        //NOTHING
                                    }
                                    //Neu o trang thai xem (view) -> set thuoc tinh disable cac control khong cho sua
                                    //Nguoc lai set thuoc tinh enable cac control cho phep sua
                                    else
                                    {
                                        lci.Control.Enabled = isAllowEditing;
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
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProviderab1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
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

        private void btnRefesh_Click(object sender, EventArgs e)
        {
            try
            {
                SetDefaultValue();
                FillDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnGDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var rowData = (QCS.EFMODEL.DataModels.V_QCS_QUERY)gridviewQcsQueryList.GetFocusedRow();
                if (rowData != null)
                {
                    bool success = false;
                    CommonParam param = new CommonParam();
                    success = new BackendAdapter(param).Post<bool>(QcsRequestUriStore.QCS_QUERY_DELETE, ApiConsumers.QcsConsumer, rowData.ID, param);
                    if (success)
                    {
                        FillDataToGridControl();
                    }
                    MessageManager.Show(param, success);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                this.ActionType = GlobalVariables.ActionView;
                EnableControlChanged(GlobalVariables.ActionView);
                positionHandle = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderab1, dxErrorProviderab1);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            try
            {
                bool success = false;
                if (!btnSave.Enabled)
                    return;

                positionHandle = -1;
                if (!dxValidationProviderab1.Validate())
                    return;

                WaitingManager.Show();
                QCS.EFMODEL.DataModels.V_QCS_QUERY updateDTO = new QCS.EFMODEL.DataModels.V_QCS_QUERY();

                if (this.currentQcsQuery != null && this.currentQcsQuery.ID > 0)
                {
                    LoadCurrent(this.currentQcsQuery.ID, ref updateDTO);
                }
                UpdateDTOFromDataForm(ref updateDTO);
                if (ActionType == GlobalVariables.ActionAdd)
                {
                    var resultData = new BackendAdapter(param).Post<V_QCS_QUERY>(QcsRequestUriStore.QCS_QUERY_CREATE, ApiConsumers.QcsConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToGridControl();
                    }
                }
                else
                {
                    var resultData = new BackendAdapter(param).Post<V_QCS_QUERY>(QcsRequestUriStore.QCS_QUERY_UPDATE, ApiConsumers.QcsConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToGridControl();
                    }
                }

                if (success)
                {
                    SetDefaultFocus();
                    this.ActionType = GlobalVariables.ActionView;
                    EnableControlChanged(this.ActionType);
                }

                WaitingManager.Hide();

                #region Hien thi message thong bao
                MessageManager.Show(param, success);
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

        private void UpdateDTOFromDataForm(ref QCS.EFMODEL.DataModels.V_QCS_QUERY currentDTO)
        {
            try
            {
				currentDTO.SQL = txtSql.Text.Trim();
currentDTO.DESCRIPTION = txtDescription.Text.Trim();

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
                this.ActionType = GlobalVariables.ActionAdd;
                EnableControlChanged(GlobalVariables.ActionAdd);
                FillDataToEditorControl(null);
                gridControlQcsQueryList.DataSource = null;
                dnNavigationQcsQuery.DataSource = null;
                positionHandle = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderab1, dxErrorProviderab1);
                SetDefaultFocus();
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
                this.ActionType = GlobalVariables.ActionEdit;
                EnableControlChanged(GlobalVariables.ActionEdit);

                SetDefaultFocus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region InitComboDataSource
		txtSql.Text = data.SQL;
txtDescription.Text = data.DESCRIPTION;
        
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
                //Gan gia tri mac dinh
                SetDefaultValue();

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
				
				//Focus default
				SetDefaultFocus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void Search()
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

        public void Refesh()
        {
            try
            {
                btnRefesh_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion
    }
}

