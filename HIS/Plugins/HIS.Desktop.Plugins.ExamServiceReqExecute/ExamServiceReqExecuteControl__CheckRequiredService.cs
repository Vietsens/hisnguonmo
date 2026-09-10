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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.ExamServiceReqExecute.Config;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExamServiceReqExecute
{
    public partial class ExamServiceReqExecuteControl
    {
        /// <summary>
        /// Chan chuyen khoa khi benh nhan con dich vu chua hoan thanh (SERVICE_REQ_STT = CXL/DXL)
        /// thuoc cac loai dich vu khai bao o key HIS.Desktop.Plugins.TransDepartment.CheckRequiredService.
        ///
        /// Duong chuyen khoa o day la "kham them" (chkExamServiceAdd) co tich chuyen khoa
        /// (ExamAdditionSDO.IsChangeDepartment) - luu xong backend se chuyen khoa, nen phai chan
        /// giong het form Chuyen khoa cua plugin HIS.Desktop.Plugins.TransDepartment.
        ///
        /// Khong bat try/catch tai day de exception noi len ProcessExamServiceReqExecute -> huy luon
        /// viec luu, tranh nuot loi roi cho chuyen khoa nhu chua tung kiem tra.
        /// </summary>
        /// <summary>
        /// Nhap vien vao khoa KHAC khoa dang lam viec = co chuyen khoa -> phai chan giong form Chuyen khoa.
        /// Nhap vien vao chinh khoa hien tai thi khong phai chuyen khoa, khong kiem tra.
        /// Khong tra duoc phong lam viec thi coi nhu CO chuyen khoa (kiem tra cho chac), vi neu bo qua
        /// ma thuc te co chuyen khoa thi lot mat dich vu chua hoan thanh.
        /// </summary>
        private bool IsChangeDepartmentWhenHospitalize(long hospitalizeDepartmentId)
        {
            if (hospitalizeDepartmentId <= 0)
                return false;

            var room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.moduleData.RoomId);
            if (room == null)
            {
                LogSystem.Warn("Khong tra duoc phong lam viec RoomId=" + this.moduleData.RoomId
                    + " de so khoa hien tai voi khoa nhap vien, coi nhu co chuyen khoa.");
                return true;
            }

            return room.DEPARTMENT_ID != hospitalizeDepartmentId;
        }

        /// <returns>false = da hien thi thong bao, khong cho luu.</returns>
        private bool IsAllowTransByRequiredService()
        {
            if (HisConfigCFG.TransDepartmentCheckRequiredServiceTypeCodes == null
                || HisConfigCFG.TransDepartmentCheckRequiredServiceTypeCodes.Count == 0)
                return true;

            if (this.treatment == null || this.treatment.ID <= 0)
                return true;

            List<long> serviceTypeIds = BackendDataWorker.Get<HIS_SERVICE_TYPE>()
                .Where(o => o != null && !string.IsNullOrEmpty(o.SERVICE_TYPE_CODE)
                    && HisConfigCFG.TransDepartmentCheckRequiredServiceTypeCodes.Contains(o.SERVICE_TYPE_CODE.Trim().ToUpper()))
                .Select(o => o.ID)
                .Distinct()
                .ToList();

            if (serviceTypeIds.Count == 0)
            {
                LogSystem.Warn("Khong tim thay loai dich vu nao ung voi ma khai bao o key "
                    + HisConfigCFG.KEY_TransDepartmentCheckRequiredService + ": "
                    + HisConfigCFG.TransDepartmentCheckRequiredService);
                return true;
            }

            CommonParam param = new CommonParam();
            HisSereServView1Filter filter = new HisSereServView1Filter();
            filter.TREATMENT_ID = this.treatment.ID;
            filter.SERVICE_TYPE_IDs = serviceTypeIds;
            filter.SERVICE_REQ_STT_IDs = new List<long>()
            {
                IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL,
                IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL
            };
            filter.HAS_EXECUTE = true;

            var unfinisheds = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV_1>>(
                "api/HisSereServ/GetView1", ApiConsumers.MosConsumer, filter, param);

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
