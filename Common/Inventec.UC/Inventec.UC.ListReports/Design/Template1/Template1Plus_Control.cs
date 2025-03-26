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
using DevExpress.XtraGrid.Views.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ListReports.Design.Template1
{
    internal partial class Template1
    {

        private void gridViewListReports_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    SAR.EFMODEL.DataModels.V_SAR_REPORT data = (SAR.EFMODEL.DataModels.V_SAR_REPORT)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            try
                            {
                                e.Value = e.ListSourceRowIndex + 1 + ((pagingGrid.CurrentPage - 1) * pagingGrid.PageSize);

                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "REPORT_START_TIME")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.START_TIME ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "REPORT_FINISH_TIME")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.FINISH_TIME ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "REPORT_CREATE_TIME")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "REPORT_MODIFY_TIME")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewListReports_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                SAR.EFMODEL.DataModels.V_SAR_REPORT data = (SAR.EFMODEL.DataModels.V_SAR_REPORT)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                if (data != null)
                {
                    string loginName = Base.TokenClientStore.ClientTokenManager.GetLoginName();
                    bool enable = (data.CREATOR.Equals(loginName) || Config.GlobalStore.isAdmin);
                    if (enable)
                    {
                        if (e.Column.FieldName == "BtnPublicReport")
                        {
                            if (data.IS_PUBLIC == 1)
                            {
                                e.RepositoryItem = btnPrivateReport;
                            }
                            else
                            {
                                e.RepositoryItem = btnPublicReport;
                            }
                        }
                        else if (e.Column.FieldName == "BtnEditReport")
                        {
                            e.RepositoryItem = btnEditReport;
                        }
                        else if (e.Column.FieldName == "BtnDeleteReport")
                        {
                            e.RepositoryItem = btnDeleteReport;
                        }
                        else if (e.Column.FieldName == "BtnSendReport")
                        {
                            e.RepositoryItem = btnSendReport;
                        }
                        else if (e.Column.FieldName == "BtnDownloadReport")
                        {
                            if (data.REPORT_STT_ID == Config.SarReportSttCFG.REPORT_STT_ID__DONE)
                            {
                                e.RepositoryItem = btnDownloadReport;
                            }
                            else
                            {
                                e.RepositoryItem = btnDownloadReportDisable;
                            }
                        }
                        else if (e.Column.FieldName == "BtnPrintReport")
                        {
                            if (data.REPORT_STT_ID == Config.SarReportSttCFG.REPORT_STT_ID__DONE)
                            {
                                e.RepositoryItem = btnPrintReport;
                            }
                            else
                            {
                                e.RepositoryItem = btnPrintReportDisable;
                            }
                        }
                    }
                    else
                    {
                        if (e.Column.FieldName == "BtnPublicReport")
                        {
                            if (data.IS_PUBLIC == 1)
                            {
                                e.RepositoryItem = btnPrivateReportDisable;
                            }
                            else
                            {
                                e.RepositoryItem = btnPublicReportDisable;
                            }
                        }
                        else if (e.Column.FieldName == "BtnEditReport")
                        {
                            e.RepositoryItem = btnEditReportDisable;
                        }
                        else if (e.Column.FieldName == "BtnDeleteReport")
                        {
                            e.RepositoryItem = btnDeleteReportDisable;
                        }
                        else if (e.Column.FieldName == "BtnSendReport")
                        {
                            e.RepositoryItem = btnSendReportDisable;
                        }
                        else if (e.Column.FieldName == "BtnDownloadReport")
                        {
                            e.RepositoryItem = btnDownloadReportDisable;
                        }
                        else if (e.Column.FieldName == "BtnPrintReport")
                        {
                            e.RepositoryItem = btnPrintReportDisable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewListReports_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {

        }

    }
}
