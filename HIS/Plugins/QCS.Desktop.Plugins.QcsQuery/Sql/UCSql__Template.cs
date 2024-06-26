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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.UC.Paging;
using DevExpress.XtraNavBar;
using DevExpress.XtraEditors;
using DevExpress.Utils;
using HIS.Desktop.LibraryMessage;
using QCS.Filter;
using Inventec.Core;
using Inventec.Common.Logging;
using System.Collections;
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using QCS.EFMODEL.DataModels;
using HIS.Desktop.Common;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.Utilities;
using HIS.Desktop.LocalStorage.ConfigApplication;
using QCS.Desktop.Plugins.QcsQuery.SqlSave;
using DevExpress.XtraGrid;

namespace QCS.Desktop.Plugins.QcsQuery.Sql
{
    public partial class UCSql : HIS.Desktop.Utility.UserControlBase, IControlCallBack
    {
        #region Declare
        //DevExpress.XtraNavBar.NavBarGroup navBarGroupGender;
        int rowCountQuery = 0;
        int dataTotalQuery = 0;
        int startPageQuery = 0;
        PagingGrid pagingGridQuery;
        int rowCountQueryLog = 0;
        int dataTotalQueryLog = 0;
        int startPageQueryLog = 0;
        PagingGrid pagingGridQueryLog;
        #endregion

        #region Construct
        public UCSql(Inventec.Desktop.Common.Modules.Module _moduleData)
            : base(_moduleData)
        {
            try
            {
                InitializeComponent();
                pagingGridQuery = new PagingGrid();
                pagingGridQueryLog = new PagingGrid();
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
                //navTitleCreateTime.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LANGUAGEUCPATIENTLIST_NAV_TITLE_CREATE_TIME", HIS.Desktop.Resources.ResourceLanguageManager.LanguageUCPatientList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                //lciFromTime.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LANGUAGEUCPATIENTLIST_LCI_FROM_TIME", HIS.Desktop.Resources.ResourceLanguageManager.LanguageUCPatientList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                //lciToTime.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LANGUAGEUCPATIENTLIST_LCI_TO_TIME", HIS.Desktop.Resources.ResourceLanguageManager.LanguageUCPatientList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());                                 
                //btnSearch.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LANGUAGEUCPATIENTLIST_BTN_SEARCH", HIS.Desktop.Resources.ResourceLanguageManager.LanguageUCPatientList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                //btnRefesh.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LANGUAGEUCPATIENTLIST_BTN_REFESH", HIS.Desktop.Resources.ResourceLanguageManager.LanguageUCPatientList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());              
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
                txtSql.Text = "";

                //DateTime FirstDayOfNow = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                //dtFromTime.DateTime = FirstDayOfNow;
                //dtToTime.DateTime = DateTime.Now;
                //dtBirthdayFrom.EditValue = null;
                //dtBirthdayTo.EditValue = null;
                //txtKeyWord.Text = "";
                //if (navBarFilter.Controls.Count > 0)
                //{
                //    for (int i = 0; i < navBarFilter.Controls.Count; i++)
                //    {
                //        if (navBarFilter.Controls[i] is DevExpress.XtraNavBar.NavBarGroupControlContainer)
                //        {
                //            continue;
                //        }
                //        if (navBarFilter.Controls[i] is DevExpress.XtraNavBar.NavBarGroupControlContainerWrapper)
                //        {
                //            foreach (DevExpress.XtraNavBar.NavBarGroupControlContainer group in navBarFilter.Controls[i].Controls)
                //            {
                //                foreach (var itemCheckEdit in group.Controls)
                //                {
                //                    if (itemCheckEdit is CheckEdit)
                //                    {
                //                        var checkEdit = itemCheckEdit as CheckEdit;
                //                        checkEdit.Checked = false;
                //                    }
                //                }
                //            }
                //        }
                //    }
                //}

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void MeShow()
        {
            try
            {
                //Gan gia tri mac dinh
                SetDefaultValueControl();

                //Load du lieu
                FillDataToGridControlQuery();

                FillDataToGridControlQueryLog();

                //Gan ngon ngu
                SetCaptionByLanguageKey();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Ham lay du lieu theo dieu kien tim kiem va gan du lieu vao danh sach
        /// </summary>
        public void FillDataToGridControlQuery()
        {
            try
            {
                WaitingManager.Show();
                int pageSize = 0;
                if (ucPaging1.pagingGrid != null)
                {
                    pageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    pageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }
                LoadPagingQuery(new CommonParam(0, pageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCountQuery;
                param.Count = dataTotalQuery;
                ucPaging1.Init(LoadPagingQuery, param, pageSize, this.gCQuery);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void LoadPagingQuery(object param)
        {
            try
            {
                startPageQuery = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPageQuery, limit);
                Inventec.Core.ApiResultObject<List<QCS.EFMODEL.DataModels.QCS_QUERY>> apiResult = null;
                QcsQueryFilter filter = new QcsQueryFilter();
                SetFilterNavBarQuery(ref filter);
                gCQuery.DataSource = null;
                gVQuery.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<QCS.EFMODEL.DataModels.QCS_QUERY>>(QCS.URI.QcsQuery.GET, ApiConsumers.QcsConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<QCS.EFMODEL.DataModels.QCS_QUERY>)apiResult.Data;
                    if (data != null)
                    {
                        gVQuery.GridControl.DataSource = data;
                        rowCountQuery = (data == null ? 0 : data.Count);
                        dataTotalQuery = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                gVQuery.EndUpdate();

                #region Process has exception
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetFilterNavBarQuery(ref QcsQueryFilter filter)
        {
            try
            {
                filter.CREATOR = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                filter.ORDER_FIELD = "CREATE_TIME";
                filter.ORDER_DIRECTION = "DESC";
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        /// <summary>
        /// Ham lay du lieu theo dieu kien tim kiem va gan du lieu vao danh sach
        /// </summary>
        public void FillDataToGridControlQueryLog()
        {
            try
            {
                WaitingManager.Show();
                int pageSize = 0;
                if (ucPaging2.pagingGrid != null)
                {
                    pageSize = ucPaging2.pagingGrid.PageSize;
                }
                else
                {
                    pageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }
                LoadPagingQueryLog(new CommonParam(0, pageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCountQueryLog;
                param.Count = dataTotalQueryLog;
                ucPaging2.Init(LoadPagingQueryLog, param, pageSize, this.gCQueryLog);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void LoadPagingQueryLog(object param)
        {
            try
            {
                startPageQueryLog = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPageQueryLog, limit);
                Inventec.Core.ApiResultObject<List<QCS.EFMODEL.DataModels.QCS_QUERY_LOG>> apiResult = null;
                QcsQueryLogFilter filter = new QcsQueryLogFilter();
                SetFilterNavBarQueryLog(ref filter);
                gCQueryLog.DataSource = null;
                gVQueryLog.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<QCS.EFMODEL.DataModels.QCS_QUERY_LOG>>(QCS.URI.QcsQueryLog.GET, ApiConsumers.QcsConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<QCS.EFMODEL.DataModels.QCS_QUERY_LOG>)apiResult.Data;
                    if (data != null)
                    {
                        gVQueryLog.GridControl.DataSource = data;
                        rowCountQueryLog = (data == null ? 0 : data.Count);
                        dataTotalQueryLog = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                gVQueryLog.EndUpdate();

                #region Process has exception
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetFilterNavBarQueryLog(ref QcsQueryLogFilter filter)
        {
            try
            {
                //filter.KEY_WORD = txtKeyWord.Text.Trim();
                //if (dtFromTime != null && dtFromTime.DateTime != DateTime.MinValue)
                //    filter.CREATE_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtFromTime.EditValue).ToString("yyyyMMdd") + "000000");
                //if (dtToTime != null && dtToTime.DateTime != DateTime.MinValue)
                //filter.CREATE_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtToTime.EditValue).ToString("yyyyMMdd") + "232359");
                filter.CREATOR = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                filter.ORDER_FIELD = "CREATE_TIME";
                filter.ORDER_DIRECTION = "DESC";

            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        //private void txtKeyWord_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        //{
        //    try
        //    {
        //        if (e.KeyCode == Keys.Enter)
        //        {
        //            btnSearch_Click(null, null);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        //private void btnSearch_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        FillDataToGridControl();
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        private void btnRefesh_Click(object sender, EventArgs e)
        {
            try
            {
                SetDefaultValueControl();
                FillDataToGridControlQuery();
                FillDataToGridControlQueryLog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                QCS.EFMODEL.DataModels.QCS_QUERY dataSave = new QCS_QUERY();
                dataSave.SQL = this.txtSql.Text;
                frmSqlSave form = new frmSqlSave(dataSave, refeshData);
                form.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void refeshData()
        {
            this.FillDataToGridControlQuery();
        }

        #endregion

        #region Public method
        public void Search()
        {
            try
            {
                //btnSearch_Click(null, null);
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

        private void repositoryItemBE__Use_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                var rowData = (QCS.EFMODEL.DataModels.QCS_QUERY)gVQuery.GetFocusedRow();

                if (rowData != null)
                {
                    txtSql.Text = rowData.SQL;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemBE__Delete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                var rowData = (QCS.EFMODEL.DataModels.QCS_QUERY)gVQuery.GetFocusedRow();
                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    QcsQueryFilter filter = new QcsQueryFilter();
                    filter.ID = rowData.ID;
                    var data = new BackendAdapter(param).Get<List<QCS.EFMODEL.DataModels.QCS_QUERY>>("api/QcsQuery/Get", ApiConsumers.QcsConsumer, filter, param).FirstOrDefault();

                    if (rowData != null)
                    {
                        bool success = false;
                        success = new BackendAdapter(param).Post<bool>("api/QcsQuery/Delete", ApiConsumers.QcsConsumer, data.ID, param);
                        if (success)
                        {
                            FillDataToGridControlQuery();
                            //currentData = ((List<SAR_REPORT_TYPE>)gridControlFormList.DataSource).FirstOrDefault();
                        }
                        MessageManager.Show(this.ParentForm, param, success);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonEdit1_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                var rowData = (QCS.EFMODEL.DataModels.QCS_QUERY_LOG)gVQueryLog.GetFocusedRow();

                if (rowData != null)
                {
                    txtSql.Text = rowData.SQL;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonEdit2_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                var rowData = (QCS.EFMODEL.DataModels.QCS_QUERY_LOG)gVQueryLog.GetFocusedRow();
                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    QcsQueryLogFilter filter = new QcsQueryLogFilter();
                    filter.ID = rowData.ID;
                    var data = new BackendAdapter(param).Get<List<QCS.EFMODEL.DataModels.QCS_QUERY_LOG>>("api/QcsQueryLog/Get", ApiConsumers.QcsConsumer, filter, param).FirstOrDefault();

                    if (rowData != null)
                    {
                        bool success = false;
                        success = new BackendAdapter(param).Post<bool>("api/QcsQueryLog/Delete", ApiConsumers.QcsConsumer, data.ID, param);
                        if (success)
                        {
                            FillDataToGridControlQuery();
                            //currentData = ((List<SAR_REPORT_TYPE>)gridControlFormList.DataSource).FirstOrDefault();
                        }
                        MessageManager.Show(this.ParentForm, param, success);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnExeStringCmd_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                if (this.txtSql != null && this.txtSql.Text != "")
                {
                    WaitingManager.Show();
                    QCS.TDO.ExeStringTDO stringTDO = new TDO.ExeStringTDO();
                    stringTDO.Query = this.txtSql.Text;
                    stringTDO.IsAll = false;
                    var resultData = new BackendAdapter(param).Post<QCS.SDO.ExeResultSDO>("api/QcsQueryLog/ExeStringCmd", ApiConsumers.QcsConsumer, stringTDO, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);

                    if (resultData != null && (param.Messages == null || param.Messages.Count == 0))
                    {
                        success = true;
                        gVResult.GridControl.DataSource = null;
                        gVResult.Columns.Clear();
                        AddNumOrder(resultData.QueryResult);
                        gVResult.GridControl.DataSource = resultData.QueryResult;
                        gVResult.BestFitColumns();
                        if (resultData.QueryResult != null)
                        {
                            this.lcRowCount.Text = string.Format("{0}/{1}", resultData.QueryResult.Rows.Count, resultData.CountTotal ?? 0);
                        }
                        FillDataToGridControlQueryLog();
                        WaitingManager.Hide();
                    }

                    WaitingManager.Hide();
                    #region Show message
                    MessageManager.Show(this.ParentForm, param, success);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        private void AddNumOrder(DataTable dataTable)
        {
            DataColumn col = dataTable.Columns.Add("STT", typeof(System.Int32));
            col.SetOrdinal(0);

            int i = 1;
            foreach (DataRow row in dataTable.Rows)
            {
                //need to set value to NewColumn column
                row[col] = i++;   // or set it to some other value
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                ShowGridPreview(this.gCResult);
                //PrintGrid(this.gCResult);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ShowGridPreview(GridControl grid)
        {
            // Check whether the GridControl can be previewed. 
            if (!grid.IsPrintingAvailable)
            {
                MessageBox.Show("The 'DevExpress.XtraPrinting' library is not found", "Error");
                return;
            }

            // Open the Preview window. 
            grid.ShowPrintPreview();
        }

        private void PrintGrid(GridControl grid)
        {
            // Check whether the GridControl can be printed. 
            if (!grid.IsPrintingAvailable)
            {
                MessageBox.Show("The 'DevExpress.XtraPrinting' library is not found", "Error");
                return;
            }

            // Print. 
            grid.Print();
        }
        private void btnExcel_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.gCResult != null)
                {
                    SaveFileDialog saveFile = new SaveFileDialog();
                    saveFile.Filter = "Excel file|*.xlsx|All file|*.*";
                    if (saveFile.ShowDialog() == DialogResult.OK)
                    {
                        this.gCResult.ExportToXlsx(saveFile.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void gVQuery_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    QCS.EFMODEL.DataModels.QCS_QUERY pData = (QCS.EFMODEL.DataModels.QCS_QUERY)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    short status = Inventec.Common.TypeConvert.Parse.ToInt16((pData.IS_ACTIVE ?? -1).ToString());
                    if (e.Column.FieldName == "NUM_ORDER")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPageQuery; //+ ((pagingGrid.CurrentPage - 1) * pagingGrid.PageSize);
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString((long)pData.CREATE_TIME);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    //else if (e.Column.FieldName == "MODIFY_TIME_STR")
                    //{
                    //    try
                    //    {
                    //        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString((long)pData.MODIFY_TIME);
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        Inventec.Common.Logging.LogSystem.Error(ex);
                    //    }
                    //}
                    //else if (e.Column.FieldName == "REPORT_TYPE_GROUP_NAME" && pData.REPORT_TYPE_GROUP_ID.HasValue)
                    //{
                    //    e.Value = (this.reportTypeGroups.FirstOrDefault(o => o.ID == pData.REPORT_TYPE_GROUP_ID) ?? new SAR_REPORT_TYPE_GROUP()).REPORT_TYPE_GROUP_NAME;

                    //}
                }

                gCQuery.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gVQueryLog_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    QCS.EFMODEL.DataModels.QCS_QUERY_LOG pData = (QCS.EFMODEL.DataModels.QCS_QUERY_LOG)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    short status = Inventec.Common.TypeConvert.Parse.ToInt16((pData.IS_ACTIVE ?? -1).ToString());
                    if (e.Column.FieldName == "NUM_ORDER")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPageQueryLog; //+ ((pagingGrid.CurrentPage - 1) * pagingGrid.PageSize);
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString((long)pData.CREATE_TIME);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    //else if (e.Column.FieldName == "MODIFY_TIME_STR")
                    //{
                    //    try
                    //    {
                    //        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString((long)pData.MODIFY_TIME);
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        Inventec.Common.Logging.LogSystem.Error(ex);
                    //    }
                    //}
                    //else if (e.Column.FieldName == "REPORT_TYPE_GROUP_NAME" && pData.REPORT_TYPE_GROUP_ID.HasValue)
                    //{
                    //    e.Value = (this.reportTypeGroups.FirstOrDefault(o => o.ID == pData.REPORT_TYPE_GROUP_ID) ?? new SAR_REPORT_TYPE_GROUP()).REPORT_TYPE_GROUP_NAME;

                    //}
                }

                gCQueryLog.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private void bbtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        //{
        //    btnSave_Click(null, null);
        //}

        //private void bbtnExeCmdString_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        //{
        //    btnExeStringCmd_Click(null, null);
        //}

        //private void bbtnPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        //{
        //    btnPrint_Click(null,null);
        //}

        //private void bbtnExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        //{
        //    btnExcel_Click(null, null);
        //}

        private void btnAll_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                if (this.txtSql != null && this.txtSql.Text != "")
                {
                    WaitingManager.Show();
                    QCS.TDO.ExeStringTDO stringTDO = new TDO.ExeStringTDO();
                    stringTDO.Query = this.txtSql.Text;
                    stringTDO.IsAll = true;
                    var resultData = new BackendAdapter(param).Post<QCS.SDO.ExeResultSDO>("api/QcsQueryLog/ExeStringCmd", ApiConsumers.QcsConsumer, stringTDO, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    if (resultData != null)
                    {
                        if (resultData != null && (param.Messages == null || param.Messages.Count == 0))
                        {
                            success = true;
                            gVResult.GridControl.DataSource = null;
                            gVResult.Columns.Clear();
                            AddNumOrder(resultData.QueryResult);
                            gVResult.GridControl.DataSource = resultData.QueryResult;
                            gVResult.BestFitColumns();
                            if (resultData.QueryResult != null)
                            {
                                this.lcRowCount.Text = string.Format("{0}/{1}", resultData.QueryResult.Rows.Count, resultData.CountTotal ?? 0);
                            }
                            FillDataToGridControlQueryLog();
                            WaitingManager.Hide();
                        }
                    }
                    WaitingManager.Hide();
                    #region Show message
                    MessageManager.Show(this.ParentForm, param, success);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        //private void bbtnAll_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        //{
        //    btnAll_Click(null,null);
        //}

        public void bbtnCtrlS_ItemClick()
        {
            try
            {
                if (btnSave.Enabled)
                {
                    btnSave.Focus();
                    btnSave_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void bbtnCtrlE_ItemClick()
        {
            try
            {
                if (btnExeStringCmd.Enabled)
                {
                    btnExeStringCmd.Focus();
                    btnExeStringCmd_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void bbtnCtrlP_ItemClick()
        {
            try
            {
                if (btnPrint.Enabled)
                {
                    btnPrint.Focus();
                    btnPrint_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void bbtnCtrlX_ItemClick()
        {
            try
            {
                if (this.txtSql.SelectionLength != 0)
                {
                    this.txtSql.Cut();
                }
                else
                {
                    btnExcel_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void bbtnCtrlA_ItemClick()
        {
            try
            {
                if (this.txtSql.SelectionLength != 0)
                {
                    this.txtSql.SelectAll();
                }
                else
                {
                    btnAll_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        public void bbtnCtrlC_ItemClick()
        {
            try
            {
                if (this.txtSql.SelectionLength != 0)
                {
                    this.txtSql.Copy();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void bbtnCtrlV_ItemClick()
        {
            try
            {
                if (this.txtSql.Enabled)
                {
                    this.txtSql.Paste();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }



    }
}

