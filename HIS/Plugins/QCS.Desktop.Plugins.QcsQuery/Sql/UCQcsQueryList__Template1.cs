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
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraNavBar;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using QCS.Desktop.ApiConsumer;
using QCS.Desktop.Common;
using QCS.Desktop.Controls.Session;
using QCS.Desktop.LibraryMessage;
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
        DevExpress.XtraNavBar.NavBarGroup navBarGroupGender;
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        PagingGrid pagingGrid;
        #endregion

        #region Construct
        public UCQcsQueryList()
        {
            try
            {
                InitializeComponent();
                pagingGrid = new PagingGrid();
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

        private void SetDefaultValueControl()
        {
            try
            {
                DateTime FirstDayOfNow = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                dtFromTime.DateTime = FirstDayOfNow;
                dtToTime.DateTime = DateTime.Now;
                txtKeyWord.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Ham lay du lieu theo dieu kien tim kiem va gan du lieu vao danh sach
        /// </summary>
        private void FillDataToGridControl()
        {
            try
            {
                WaitingManager.Show();

                LoadPaging(new CommonParam(0, Convert.ToInt32(ConfigApplications.NumPageSize)));

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

                gridviewQcsQueryList.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<V_QCS_QUERY>>(QcsRequestUriStore.QCS_QUERY_GETVIEW, ApiConsumers.QcsConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<QCS.EFMODEL.DataModels.V_QCS_QUERY>)apiResult.Data;
                    if (data != null)
                    {
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
                filter.KEY_WORD = txtKeyWord.Text.Trim();
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

        private void txtKeyWord_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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

        private void btnRefesh_Click(object sender, EventArgs e)
        {
            try
            {
                SetDefaultValueControl();
                FillDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnGEdit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (QCS.EFMODEL.DataModels.V_QCS_QUERY)gridviewQcsQueryList.GetFocusedRow();
                if (row != null)
                {
                    Desktop.Plugins.QcsQuery.QcsQueryUpdate.frmQcsQueryUpdate infoQcsQuery = new Desktop.Plugins.QcsQuery.QcsQueryUpdate.frmQcsQueryUpdate(row.ID, FillDataToGridControl);
                    infoQcsQuery.ShowDialog();
                }
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
        #endregion

        #region Public method
        public void MeShow()
        {
            try
            {
                //Gan gia tri mac dinh
                SetDefaultValueControl();

                //Load du lieu
                FillDataToGridControl();

                //Gan ngon ngu
                SetCaptionByLanguageKey();
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

