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

namespace Inventec.UC.EventLogControl.Design.Template1
{
    internal partial class Template1
    {

        private void txtKeyWord_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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
                SetDefaultValueControl();
                ButtonSearchAndPagingClick(true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtTimeFrom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    dtTimeTo.Focus();
                    dtTimeTo.ShowPopup();
                }
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
                WaitDialogForm waitLoad = new WaitDialogForm(Base.MessageUtil.GetMessage(MessageLang.Message.Enum.HeThongThongBaoMoTaChoWaitDialogForm), Base.MessageUtil.GetMessage(MessageLang.Message.Enum.HeThongThongBaoTieuDeChoWaitDialogForm));
                SDA.Filter.SdaEventLogFilter filter = new SDA.Filter.SdaEventLogFilter();
                CommonParam param;
                if (flag)
                {
                    param = new CommonParam(0, Inventec.Common.TypeConvert.Parse.ToInt32(txtPageSize.Text));
                }
                else
                {
                    param = new CommonParam(pagingGrid.RecNo, Inventec.Common.TypeConvert.Parse.ToInt32(txtPageSize.Text));
                }

                try
                {
                    filter.ORDER_FIELD = "CREATE_TIME";
                    filter.ORDER_DIRECTION = "DESC";

                    filter.KEY_WORD = txtKeyWord.Text.Trim();

                    if (dtTimeFrom.EditValue != null && dtTimeFrom.DateTime != DateTime.MinValue)
                    {
                        filter.CREATE_DATE_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime((dtTimeFrom.EditValue ?? "").ToString()).ToString("yyyyMMdd") + "000000");
                    }
                    if (dtTimeTo.EditValue != null  && dtTimeTo.DateTime != DateTime.MinValue)
                    {
                        filter.CREATE_DATE_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime((dtTimeTo.EditValue ?? "").ToString()).ToString("yyyyMMdd") + "000000");
                    }
                    //if (!String.IsNullOrEmpty(currentData.loginName))
                    //{
                    //    filter.LOGIN_NAME = currentData.loginName;
                    //}
                    Inventec.Common.Logging.LogSystem.Debug("SdaEventLogUC Get for paging start time=" + DateTime.Now.ToString("dd/MM/yyyy HHmmss"));
                    var dataEventLog = new Sda.EventLog.Get.SdaEventLogGet(param).Get(filter);
                    Inventec.Common.Logging.LogSystem.Debug("SdaEventLogUC Get for paging finish time=" + DateTime.Now.ToString("dd/MM/yyyy HHmmss"));

                    if (dataEventLog != null)
                    {
                        int rowCount = dataEventLog.Total ?? 0;
                        ListEventLog = (List<SDA.EFMODEL.DataModels.SDA_EVENT_LOG>)dataEventLog.Data;
                        if (flag)
                        {
                            var recordData = ListEventLog == null ? 0 : ListEventLog.Count;
                            pagingGrid.Innitial(lblDisplayPageNo, txtPageSize, txtCurrentPage, lblTotalPage, btnLastPage, btnPreviousPage, btnFirstPage, btnNextPage, rowCount, recordData);
                        }
                        gridViewEventLog.BeginUpdate();
                        gridViewEventLog.GridControl.DataSource = ListEventLog;
                        gridViewEventLog.EndDataUpdate();
                        Inventec.Common.Logging.LogSystem.Debug("SdaEventLogUC Get for paging finish Update=" + DateTime.Now.ToString("dd/MM/yyyy HHmmss"));
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }

                #region has exception
                if (_HasException != null) _HasException(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                waitLoad.Dispose();
            }
        }

    }
}
