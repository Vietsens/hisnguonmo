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
using DevExpress.Utils;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.UC.ListReports.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ListReports.Design.Template3
{
    internal partial class Template3
    {
        public void MeShow()
        {
            try
            {
                gridColumnCopyReport.Visible = (this._ReportCopy != null);
                FillDataToGridLookupEdit();
                SetDefaultControl();
                //txtPageSize.EditValue = GlobalStore.NumberPage;
                //ButtonSearchAndPagingClick(true);
                SetInitPaging();
                txtSearch.Text = "";
                txtSearch.Focus();
                txtSearch.SelectAll();

                //initGridLookUpSelectOne();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        void FillDataToGridLookupEdit()
        {
            try
            {
                List<SearchADO> listData = new List<SearchADO>();
                listData.Add(new SearchADO(1, "Tôi tạo, tôi nhận hoặc được chia sẻ"));
                listData.Add(new SearchADO(2, "Tôi tạo"));
                listData.Add(new SearchADO(3, "Tôi nhận"));
                if (GlobalStore.isAdmin)
                {
                    listData.Add(new SearchADO(4, "Toàn viện"));
                }
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("Name", "", 300, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("Name", "Id", columnInfos, false, 300);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(this.cboSTT, listData, controlEditorADO);
                this.cboSTT.EditValue = 1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
        internal void SetInitPaging()
        {
            try
            {
                int pageSize = 0;
                //WaitingManager.Show();
                if (ucPaging1.pagingGrid != null)
                {
                    pageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    pageSize = (int)GlobalStore.NumberPage;
                }
                FillDataToGridControl(new CommonParam(0, pageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridControl, param, pageSize, this.gridControlListReports);
                //WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void FillDataToGridControl(object Commonparam)
        {
            try
            {
                //waitLoad = new WaitDialogForm(Base.MessageUtil.GetMessage(MessageLang.Message.Enum.HeThongThongBaoMoTaChoWaitDialogForm), Base.MessageUtil.GetMessage(MessageLang.Message.Enum.HeThongThongBaoTieuDeChoWaitDialogForm));
                //CommonParam param;
                startPage = ((CommonParam)Commonparam).Start ?? 0;
                int limit = ((CommonParam)Commonparam).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                SAR.Filter.SarReportViewFilter filter = new SAR.Filter.SarReportViewFilter();
                try
                {
                    if (cboSTT.EditValue.ToString() == "1")
                    {
                        filter.FilterMode = SAR.Filter.SarReportViewFilter.FilterModeEnum.ALL;
                    }
                    else if (cboSTT.EditValue.ToString() == "2")
                    {
                        filter.FilterMode = SAR.Filter.SarReportViewFilter.FilterModeEnum.CREATE;
                    }
                    else if (cboSTT.EditValue.ToString() == "3")
                    {
                        filter.FilterMode = SAR.Filter.SarReportViewFilter.FilterModeEnum.RECEIVE;
                    }
                    else if (cboSTT.EditValue.ToString() == "4")
                    {
                        filter.IS_EXCLUDE_FILTER_BY_LOGINNAME = true;
                    }

                    if (dtTimeForm.EditValue != null && dtTimeForm.DateTime != DateTime.MinValue)
                    {
                        filter.CREATE_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeForm.EditValue).ToString("yyyyMMdd") + "000000");
                    }
                    if (dtTimeTo.EditValue != null && dtTimeTo.DateTime != DateTime.MinValue)
                    {
                        filter.CREATE_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeTo.EditValue).ToString("yyyyMMdd") + "235959");
                    }
                    //if (checkNoProcess.Checked)
                    //{
                    //    if (filter.REPORT_STT_IDs == null) filter.REPORT_STT_IDs = new List<long>();
                    //    filter.REPORT_STT_IDs.Add(Config.SarReportSttCFG.REPORT_STT_ID__WAIT);
                    //}
                    //if (checkProcessing.Checked)
                    //{
                    //    if (filter.REPORT_STT_IDs == null) filter.REPORT_STT_IDs = new List<long>();
                    //    filter.REPORT_STT_IDs.Add(Config.SarReportSttCFG.REPORT_STT_ID__PROCESSING);
                    //}
                    //if (checkFinish.Checked)
                    //{
                    //    if (filter.REPORT_STT_IDs == null) filter.REPORT_STT_IDs = new List<long>();
                    //    filter.REPORT_STT_IDs.Add(Config.SarReportSttCFG.REPORT_STT_ID__DONE);
                    //}
                    //if (checkCancel.Checked)
                    //{
                    //    if (filter.REPORT_STT_IDs == null) filter.REPORT_STT_IDs = new List<long>();
                    //    filter.REPORT_STT_IDs.Add(Config.SarReportSttCFG.REPORT_STT_ID__CANCEL);
                    //}
                    //if (checkError.Checked)
                    //{
                    //    if (filter.REPORT_STT_IDs == null) filter.REPORT_STT_IDs = new List<long>();
                    //    filter.REPORT_STT_IDs.Add(Config.SarReportSttCFG.REPORT_STT_ID__ERROR);
                    //}
                    filter.KEY_WORD = txtSearch.Text.Trim();
                    var Data = new Sar.SarReport.Get.SarReportGet(paramCommon).GetView(filter);
                    if (Data != null)
                    {
                        rowCount = Data.Param.Count ?? 0;
                        ListReport.Clear();
                        if (Data.Data != null) ListReport = (List<SAR.EFMODEL.DataModels.V_SAR_REPORT>)Data.Data;
                        //var recordData = (ListReport == null ? 0 : ListReport.Count);
                        var recordData = Data.Data;
                        if (recordData != null && recordData.Count > 0)
                        {
                            gridControlListReports.DataSource = recordData;
                            rowCount = (recordData == null ? 0 : recordData.Count);
                            dataTotal = (Data.Param == null ? 0 : Data.Param.Count ?? 0);
                        }
                        else
                        {
                            gridControlListReports.DataSource = null;
                            rowCount = (recordData == null ? 0 : recordData.Count);
                            dataTotal = (Data.Param == null ? 0 : Data.Param.Count ?? 0);
                        }
                    }
                    //gridViewListReports.BeginUpdate();
                    //gridViewListReports.GridControl.DataSource = ListReport;
                    //gridViewListReports.EndUpdate();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
                #region Process Has Exception
                //if (_HasException != null) _HasException(param);
                #endregion
                //waitLoad.Dispose();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void SetDefaultControl()
        {
            try
            {
                DateTime dtStart = DateTime.Now;
                dtTimeForm.EditValue = dtStart;
                dtTimeTo.EditValue = DateTime.Now;
                txtSearch.Text = "";
                txtSearch.Focus();
                txtSearch.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
