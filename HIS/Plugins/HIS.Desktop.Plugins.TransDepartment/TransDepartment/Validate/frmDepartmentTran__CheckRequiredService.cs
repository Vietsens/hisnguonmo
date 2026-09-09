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
using HIS.Desktop.LocalStorage.BackendData;
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
        /// Chan chuyen khoa khi benh nhan con dich vu chua hoan thanh (SERVICE_REQ_STT = CXL/DXL)
        /// thuoc cac loai dich vu khai bao o key HIS.Desktop.Plugins.TransDepartment.CheckRequiredService.
        /// Khong bat try/catch tai day de exception noi len btnSave_Click -> huy luon viec luu,
        /// tranh truong hop nuot loi roi cho chuyen khoa nhu chua tung kiem tra.
        /// </summary>
        /// <returns>false = da hien thi thong bao, khong cho luu.</returns>
        private bool IsAllowTransByRequiredService()
        {
            if (ConfigKey.CheckRequiredServiceTypeCodes == null || ConfigKey.CheckRequiredServiceTypeCodes.Count == 0)
                return true;

            List<long> serviceTypeIds = BackendDataWorker.Get<HIS_SERVICE_TYPE>()
                .Where(o => o != null && !string.IsNullOrEmpty(o.SERVICE_TYPE_CODE)
                    && ConfigKey.CheckRequiredServiceTypeCodes.Contains(o.SERVICE_TYPE_CODE.Trim().ToUpper()))
                .Select(o => o.ID)
                .Distinct()
                .ToList();

            if (serviceTypeIds.Count == 0)
            {
                LogSystem.Warn("Khong tim thay loai dich vu nao ung voi ma khai bao o key "
                    + ConfigKey.KEY__CHECK_REQUIRED_SERVICE + ": " + ConfigKey.CheckRequiredService);
                return true;
            }

            CommonParam param = new CommonParam();
            HisSereServView1Filter filter = new HisSereServView1Filter();
            filter.TREATMENT_ID = this.treatmentId;
            filter.SERVICE_TYPE_IDs = serviceTypeIds;
            filter.SERVICE_REQ_STT_IDs = new List<long>()
            {
                IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL,
                IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL
            };
            filter.HAS_EXECUTE = true;

            var unfinisheds = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV_1>>(
                "api/HisSereServ/GetView1", ApiConsumers.MosConsumer, filter, SessionManager.ActionLostToken, param);

            LogSystem.Debug(LogUtil.TraceData(LogUtil.GetMemberName(() => unfinisheds), unfinisheds));

            if (unfinisheds == null || unfinisheds.Count == 0)
                return true;

            string serviceNames = string.Join(", ", unfinisheds.Select(o => o.TDL_SERVICE_NAME).Where(o => !string.IsNullOrEmpty(o)).Distinct().ToList());
            string serviceReqCodes = string.Join(", ", unfinisheds.Select(o => o.TDL_SERVICE_REQ_CODE).Where(o => !string.IsNullOrEmpty(o)).Distinct().ToList());

            XtraMessageBox.Show(
                string.Format("Dịch vụ {0} (mã y lệnh: {1}) chưa hoàn thành. Không cho phép thực hiện chuyển khoa", serviceNames, serviceReqCodes),
                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }
    }
}
