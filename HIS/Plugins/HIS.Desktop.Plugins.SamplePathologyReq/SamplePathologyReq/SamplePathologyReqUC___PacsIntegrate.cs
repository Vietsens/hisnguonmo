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
using System.Linq;
using System.Windows.Forms;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.Plugins.SamplePathologyReq.ADO;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;

namespace HIS.Desktop.Plugins.SamplePathologyReq.SamplePathologyReq
{
    public partial class SamplePathologyReqUC
    {
        private void UpdateBtnSendIntegrateState()
        {
            try
            {
                var row = gridView1.GetFocusedRow() as ServiceReqADO;
                btnSendIntegrate.Enabled = IsSendIntegrateEligible(row);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool IsSendIntegrateEligible(ServiceReqADO row)
        {
            if (row == null) return false;
            if (string.IsNullOrWhiteSpace(row.BLOCK)) return false;
            if (row.IS_SENT_EXT != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE) return false;
            if (row.IS_NO_EXECUTE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE) return false;
            return true;
        }

        private void gridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            try
            {
                UpdateBtnSendIntegrateState();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSendIntegrate_Click(object sender, EventArgs e)
        {
            try
            {
                var row = gridView1.GetFocusedRow() as ServiceReqADO;
                if (row == null) return;

                if (string.IsNullOrWhiteSpace(row.BLOCK))
                {
                    MessageBox.Show(Resources.ResourceMessage.ChuaNhapBlock);
                    return;
                }

                if (!IsSendIntegrateEligible(row)) return;

                long serviceReqId = row.ID;

                SaveBeforeSendIntegrate();

                var refreshedRow = (gridView1.DataSource as List<ServiceReqADO>)
                    ?.FirstOrDefault(o => o.ID == serviceReqId) ?? row;
                if (!IsSendIntegrateEligible(refreshedRow)) return;

                CallPacsRequestOrder(serviceReqId);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SaveBeforeSendIntegrate()
        {
            try
            {
                var data = gridView1.DataSource as List<ServiceReqADO>;
                if (data == null || data.Count == 0) return;
                ProcessUpdateBlock(data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CallPacsRequestOrder(long serviceReqId)
        {
            CommonParam param = new CommonParam();
            try
            {
                Inventec.Common.Logging.LogSystem.Debug(
                    Inventec.Common.Logging.LogUtil.TraceData(
                        Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceReqId), serviceReqId));

                WaitingManager.Show();

                bool success = new BackendAdapter(param).Post<bool>(
                    HisRequestUriStore.HIS_SERVICE_REQ_REQUEST_ORDER,
                    ApiConsumers.MosConsumer,
                    serviceReqId,
                    SessionManager.ActionLostToken,
                    param);

                WaitingManager.Hide();

                if (success) FillDataToGridControl();
                MessageManager.Show(this.ParentForm, param, success);

                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(
                    "btnSendIntegrate_Click failed."
                    + Inventec.Common.Logging.LogUtil.TraceData(
                        Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceReqId), serviceReqId),
                    ex);
            }
        }
    }
}
