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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.Plugins.TransDepartment.Config;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TransDepartment
{
    public partial class frmDepartmentTran : HIS.Desktop.Utility.FormBase
    {
        /// <summary>
        /// Viec 59683: kiem tra thoi gian chuyen khoa nguoi dung nhap. Chi chay khi bat key
        /// MOS.HIS_DEPARTMENT_TRAN.ALLOW_EDIT_TRANSFER_TIME va dang tao moi luot chuyen khoa.
        /// - Chan: lon hon thoi gian hien tai; nho hon thoi gian vao khoa hien tai;
        ///   nho hon thoi gian bat dau giuong cua benh nhan tai khoa hien tai.
        /// - Canh bao Co/Khong: nho hon thoi gian ket thuc giuong moi nhat tai khoa hien tai.
        ///   Chon Co -> set isConfirmBedFinishTime de BE bo qua dung kiem tra nay khi roi buong.
        /// Khong bat try/catch tai day de exception noi len btnSave_Click -> huy luon viec luu.
        /// </summary>
        /// <returns>false = da hien thi thong bao, khong cho luu.</returns>
        private bool IsValidTransferTime()
        {
            this.isConfirmBedFinishTime = false;
            if (!ConfigKey.AllowEditTransferTime || this.departmentTran != null)
                return true;

            if (dtLogTime.EditValue == null)
            {
                XtraMessageBox.Show("Bạn chưa nhập thời gian chuyển khoa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dtLogTime.Focus();
                return false;
            }

            long transferTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtLogTime.DateTime) ?? 0;
            LogSystem.Debug("59683 - Kiem tra thoi gian chuyen khoa. treatmentId=" + this.treatmentId + "; departmentId=" + this.departmentId + "; transferTime=" + transferTime);
            if (transferTime > Inventec.Common.DateTime.Get.Now())
            {
                XtraMessageBox.Show("Thời gian chuyển khoa không được lớn hơn thời gian hiện tại.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dtLogTime.Focus();
                return false;
            }

            CommonParam param = new CommonParam();

            // Thoi gian vao khoa hien tai = luot gan nhat benh nhan duoc tiep nhan vao khoa nay
            HisDepartmentTranFilter departmentTranFilter = new HisDepartmentTranFilter();
            departmentTranFilter.TREATMENT_ID = this.treatmentId;
            departmentTranFilter.DEPARTMENT_ID = this.departmentId;
            var departmentTrans = new BackendAdapter(param).Get<List<HIS_DEPARTMENT_TRAN>>(
                "api/HisDepartmentTran/Get", ApiConsumers.MosConsumer, departmentTranFilter, SessionManager.ActionLostToken, param);
            var currentDepartmentTran = departmentTrans != null
                ? departmentTrans.Where(o => o.DEPARTMENT_IN_TIME.HasValue).OrderByDescending(o => o.DEPARTMENT_IN_TIME).ThenByDescending(o => o.ID).FirstOrDefault()
                : null;
            if (currentDepartmentTran != null && transferTime < currentDepartmentTran.DEPARTMENT_IN_TIME.Value)
            {
                XtraMessageBox.Show(
                    string.Format("Thời gian chuyển khoa không được nhỏ hơn thời gian vào khoa hiện tại ({0}).",
                        Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(currentDepartmentTran.DEPARTMENT_IN_TIME.Value)),
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dtLogTime.Focus();
                return false;
            }

            // Lich su giuong cua benh nhan tai khoa hien tai (tinh ca buong da roi trong khoa)
            HisBedLogViewFilter bedLogFilter = new HisBedLogViewFilter();
            bedLogFilter.TREATMENT_ID = this.treatmentId;
            bedLogFilter.DEPARTMENT_IDs = new List<long>() { this.departmentId };
            var bedLogs = new BackendAdapter(param).Get<List<V_HIS_BED_LOG>>(
                "api/HisBedLog/GetView", ApiConsumers.MosConsumer, bedLogFilter, SessionManager.ActionLostToken, param);
            LogSystem.Debug(LogUtil.TraceData(LogUtil.GetMemberName(() => bedLogs), bedLogs));
            if (bedLogs == null || bedLogs.Count == 0)
                return true;

            var startAfters = bedLogs.Where(o => o.START_TIME > transferTime).ToList();
            if (startAfters.Count > 0)
            {
                string info = string.Join("; ", startAfters.Select(o => string.Format("{0} ({1})", o.BED_NAME,
                    Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(o.START_TIME))).ToList());
                XtraMessageBox.Show(
                    string.Format("Thời gian chuyển khoa nhỏ hơn thời gian bắt đầu giường {0}.", info),
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dtLogTime.Focus();
                return false;
            }

            var lastFinish = bedLogs.Where(o => o.FINISH_TIME.HasValue).OrderByDescending(o => o.FINISH_TIME).FirstOrDefault();
            if (lastFinish != null && transferTime < lastFinish.FINISH_TIME.Value)
            {
                if (XtraMessageBox.Show(
                        string.Format("Thời gian ra khoa nhỏ hơn thời gian kết thúc giường {0} ({1}). Bạn có muốn tiếp tục?", lastFinish.BED_NAME,
                            Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(lastFinish.FINISH_TIME.Value)),
                        "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                {
                    dtLogTime.Focus();
                    return false;
                }
                this.isConfirmBedFinishTime = true;
            }

            return true;
        }
    }
}
