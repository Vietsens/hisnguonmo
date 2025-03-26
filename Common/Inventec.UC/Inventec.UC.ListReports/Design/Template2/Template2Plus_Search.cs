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
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.ListReports.Design.Template2
{
    internal partial class Template2
    {

        private void txtSearch_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    ButtonSearchAndPagingClick(true);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                ButtonSearchAndPagingClick(true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                SetDefaultControl();
                ButtonSearchAndPagingClick(true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnRefresh_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void ButtonSearchAndPagingClick(bool flag)
        {
            try
            {
                waitLoad = new WaitDialogForm(Base.MessageUtil.GetMessage(MessageLang.Message.Enum.HeThongThongBaoMoTaChoWaitDialogForm), Base.MessageUtil.GetMessage(MessageLang.Message.Enum.HeThongThongBaoTieuDeChoWaitDialogForm));
                CommonParam param;
                if (flag)
                {
                    param = new CommonParam(0, Convert.ToInt32(txtPageSize.Text));
                }
                else
                {
                    param = new CommonParam(pagingGrid.RecNo, Convert.ToInt32(txtPageSize.Text));
                }
                SAR.Filter.SarReportViewFilter filter = new SAR.Filter.SarReportViewFilter();
                try
                {
                    filter.FilterMode = SAR.Filter.SarReportViewFilter.FilterModeEnum.ALL;
                    if (checkICreate.Checked)
                    {
                        filter.FilterMode = SAR.Filter.SarReportViewFilter.FilterModeEnum.CREATE;
                    }
                    else if (checkIReceive.Checked)
                    {
                        filter.FilterMode = SAR.Filter.SarReportViewFilter.FilterModeEnum.RECEIVE;
                    }

                    if (dtTimeForm.EditValue != null && dtTimeForm.DateTime != DateTime.MinValue)
                    {
                        filter.CREATE_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeForm.EditValue).ToString("yyyyMMdd") + "000000");
                    }
                    if (dtTimeTo.EditValue != null && dtTimeTo.DateTime != DateTime.MinValue)
                    {
                        filter.CREATE_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeTo.EditValue).ToString("yyyyMMdd") + "235959");
                    }
                    if (checkNoProcess.Checked)
                    {
                        if (filter.REPORT_STT_IDs == null) filter.REPORT_STT_IDs = new List<long>();
                        filter.REPORT_STT_IDs.Add(Config.SarReportSttCFG.REPORT_STT_ID__WAIT);
                    }
                    if (checkProcessing.Checked)
                    {
                        if (filter.REPORT_STT_IDs == null) filter.REPORT_STT_IDs = new List<long>();
                        filter.REPORT_STT_IDs.Add(Config.SarReportSttCFG.REPORT_STT_ID__PROCESSING);
                    }
                    if (checkFinish.Checked)
                    {
                        if (filter.REPORT_STT_IDs == null) filter.REPORT_STT_IDs = new List<long>();
                        filter.REPORT_STT_IDs.Add(Config.SarReportSttCFG.REPORT_STT_ID__DONE);
                    }
                    if (checkCancel.Checked)
                    {
                        if (filter.REPORT_STT_IDs == null) filter.REPORT_STT_IDs = new List<long>();
                        filter.REPORT_STT_IDs.Add(Config.SarReportSttCFG.REPORT_STT_ID__CANCEL);
                    }
                    if (checkError.Checked)
                    {
                        if (filter.REPORT_STT_IDs == null) filter.REPORT_STT_IDs = new List<long>();
                        filter.REPORT_STT_IDs.Add(Config.SarReportSttCFG.REPORT_STT_ID__ERROR);
                    }
                    filter.KEY_WORD = txtSearch.Text.Trim();
                    var Data = new Sar.SarReport.Get.SarReportGet().GetView(filter);
                    if (Data != null)
                    {
                        rowCount = Data.Param.Count ?? 0;
                        ListReport.Clear();
                        if (Data.Data != null) ListReport = (List<SAR.EFMODEL.DataModels.V_SAR_REPORT>)Data.Data;
                        var recordData = (ListReport == null ? 0 : ListReport.Count);
                        if (flag)
                        {
                            pagingGrid.Innitial(lblDisplayPageNo, txtPageSize, txtCurrentPage, lblTotalPage, btnLastPage, btnPreviousPage, btnFirstPage, btnNextPage, rowCount, recordData);
                        }
                    }
                    gridViewListReports.BeginUpdate();
                    gridViewListReports.GridControl.DataSource = ListReport;
                    gridViewListReports.EndUpdate();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
                #region Process Has Exception
                if (_HasException != null) _HasException(param);
                #endregion
                waitLoad.Dispose();
            }
            catch (Exception ex)
            {
                waitLoad.Dispose();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
