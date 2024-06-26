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
using HIS.Desktop.Common;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
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

namespace HIS.Desktop.Plugins.ApprovalSurgery.CreateCalendar
{
    public partial class frmCreateCalendar : FormBase
    {
        public int positionHandle = -1;
        public DelegateSelectData refeshDataResult { get; set; }
        public long roomId { get; set; }
        public V_HIS_PTTT_CALENDAR ptttCalendar { get; set; }

        public frmCreateCalendar(long _roomId, DelegateSelectData _refeshDataResult)
        {
            InitializeComponent();
            try
            {
                this.roomId = _roomId;
                this.refeshDataResult = _refeshDataResult;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public frmCreateCalendar(long _roomId, V_HIS_PTTT_CALENDAR _ptttCalendar, DelegateSelectData _refeshDataResult)
        {
            InitializeComponent();
            try
            {
                this.roomId = _roomId;
                this.refeshDataResult = _refeshDataResult;
                this.ptttCalendar = _ptttCalendar;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItemCtrlS_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (btnSave.Enabled)
            {
                btnSave_Click(null, null);
            }
        }

        private void frmCreateCalendar_Load(object sender, EventArgs e)
        {
            try
            {
                //Load icon
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(Inventec.Desktop.Common.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));

                ValidateControl();

                if (ptttCalendar != null)
                {
                    dtTimeFrom.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(ptttCalendar.TIME_FROM).Value;
                    dtTimeTo.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(ptttCalendar.TIME_TO).Value;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            bool valid = true;
            bool success = false;
            try
            {
                this.positionHandle = -1;
                if (!dxValidationProvider1.Validate())
                    return;
                if (valid)
                {
                    WaitingManager.Show();
                    CommonParam param = new CommonParam();
                    HisPtttCalendarSDO ptttCalendarSDO = new HisPtttCalendarSDO();
                    long timeFrom = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeFrom.EditValue).ToString("yyyyMMdd") + "000000");
                    long timeTo = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeTo.EditValue).ToString("yyyyMMdd") + "235959");
                    ptttCalendarSDO.TimeFrom = timeFrom;
                    ptttCalendarSDO.TimeTo = timeTo;
                    ptttCalendarSDO.WorkingRoomId = this.roomId;
                    ptttCalendarSDO.Id = this.ptttCalendar != null ? (long?)ptttCalendar.ID : null;

                    string requestUri = this.ptttCalendar != null ? "api/HisPtttCalendar/UpdateInfo" : "api/HisPtttCalendar/Create";
                    var result = new BackendAdapter(param)
                    .Post<HIS_PTTT_CALENDAR>(requestUri, ApiConsumers.MosConsumer, ptttCalendarSDO, param);
                    if (result != null)
                    {
                        success = true;
                        if (refeshDataResult != null)
                        {
                            refeshDataResult(result);
                            this.Close();
                        }
                    }
                    WaitingManager.Hide();
                    MessageManager.Show(this, param, success);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
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

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
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
