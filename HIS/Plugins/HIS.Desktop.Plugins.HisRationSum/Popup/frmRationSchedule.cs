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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.HisRationSum.Validtion;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisRationSum.Popup
{
    public delegate void GetTimeResult(long nFrom, long nTo);
    public partial class frmRationSchedule : Form
    {
        long roomId;
        GetTimeResult dlgRsult;
        private int positionHandleControl;

        public frmRationSchedule(long roomId, GetTimeResult dlgRsult)
        {
            InitializeComponent();
            try
            {
                this.roomId = roomId;
                this.dlgRsult = dlgRsult;
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!dxValidationProvider1.Validate())
                    return;
                CommonParam param = new CommonParam();
                long nFrom = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dteFrom.DateTime) ?? 0;
                long nTo = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dteTo.DateTime) ?? 0;
                RationScheduleExecuteSDO rationScheduleExecuteSDO = new RationScheduleExecuteSDO();
                rationScheduleExecuteSDO.ReqRoomId = roomId;
                rationScheduleExecuteSDO.DateFrom = nFrom;
                rationScheduleExecuteSDO.DateTo = nTo;
                WaitingManager.Show();
                var apiresult = new Inventec.Common.Adapter.BackendAdapter
                   (param).Post<List<RationScheduleExecuteSDO>>
                   ("api/HisRationSchedule/Execute", ApiConsumers.MosConsumer, rationScheduleExecuteSDO, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                WaitingManager.Hide();
                #region Show message
                if (apiresult != null)
                {
                    if (dlgRsult != null)
                        dlgRsult(nFrom, nTo);
                    this.Close();
                }
                MessageManager.Show(this.ParentForm, param, apiresult != null);
                #endregion

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void frmRationSchedule_Load(object sender, EventArgs e)
        {
            try
            {
                dteFrom.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.StartDay() ?? 0) ?? DateTime.Now;
                dteTo.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.EndDay() ?? 0) ?? DateTime.Now;
                TimeFromValidationRule rul1 = new TimeFromValidationRule();
                rul1.dtFrom = dteFrom;
                rul1.dtTo = dteTo;
                dxValidationProvider1.SetValidationRule(dteFrom,rul1);
                TimeToValidationRule rul2 = new TimeToValidationRule();
                rul2.dtFrom = dteFrom;
                rul2.dtTo = dteTo;
                dxValidationProvider1.SetValidationRule(dteTo, rul1);
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (this.positionHandleControl == -1)
                {
                    this.positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (this.positionHandleControl > edit.TabIndex)
                {
                    this.positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
