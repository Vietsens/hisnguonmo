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
using EMR.EFMODEL.DataModels;
using EMR.Filter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.IsAdmin;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.BedRoomPartial.ADO;
using HIS.Desktop.Plugins.BedRoomPartial.Key;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.BedRoomPartial
{
    /// <summary>
    /// Viec 55703 - Xoa y lenh do nguoi khac chi dinh trong cung khoa chi dinh
    /// (key MOS.HIS_SERVICE_REQ.ALLOW_DELETE_BY_SAME_REQUEST_DEPARTMENT = 1).
    /// Nut Xoa tren cay y lenh bat them khi y lenh chua xu ly va khoa chi dinh trung khoa dang lam viec;
    /// luc bam Xoa, y lenh chi xoa duoc nho quy tac nay phai khong co van ban da ky so tren EMR.
    /// </summary>
    public partial class UCBedRoomPartial : UserControlBase
    {
        /// <summary>
        /// Khoa cua phong dang lam viec - cung cach tinh departmentId khi dung cay y lenh.
        /// </summary>
        private long GetWorkingDepartmentId()
        {
            try
            {
                var room = this.currentModule != null ? BackendDataWorker.Get<HIS_ROOM>().FirstOrDefault(p => p.ID == this.currentModule.RoomId) : null;
                return room != null ? room.DEPARTMENT_ID : 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return 0;
            }
        }

        /// <summary>
        /// Y lenh CHI xoa duoc nho quy tac cung khoa (tai khoan khong co quyen xoa san co) - phai kiem tra van ban ky
        /// tren EMR truoc khi xoa. Y lenh co quyen san co giu nguyen luong xoa cu.
        /// </summary>
        private bool IsDeleteOnlyBySameDepartment(SereServADO data)
        {
            try
            {
                if (data == null)
                    return false;

                long workingDepartmentId = GetWorkingDepartmentId();
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                return Base.SameDepartmentDeleteChecker.IsAllowedBySameDepartment(
                        HisConfigCFG.IsAllowDeleteBySameRequestDepartment,
                        data.REQUEST_DEPARTMENT_ID ?? 0,
                        data.SERVICE_REQ_STT_ID ?? 0,
                        workingDepartmentId)
                    && !Base.SameDepartmentDeleteChecker.HasOwnDeleteRight(
                        data.SERVICE_REQ_CREATOR,
                        data.REQUEST_LOGINNAME,
                        data.SERVICE_REQ_TYPE_ID,
                        data.REQUEST_DEPARTMENT_ID ?? 0,
                        loginName,
                        CheckLoginAdmin.IsAdmin(loginName),
                        this.hasDeleteBedPermission,
                        workingDepartmentId);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
        }

        /// <summary>
        /// Viec 55703: truoc khi xoa y lenh chi duoc xoa theo quy tac cung khoa, kiem tra van ban EMR cua y lenh.
        /// Chan khi van ban da ky so, hoac van ban chua ky do tai khoan khac tao, hoac khong kiem tra duoc EMR.
        /// Tra ve false khi phai dung xoa (da hien thong bao).
        /// </summary>
        private bool CheckEmrDocumentBeforeSameDepartmentDelete(SereServADO data)
        {
            string serviceReqCode = null;
            try
            {
                if (data == null || !HisConfigCFG.HasConnectionEmr || !IsDeleteOnlyBySameDepartment(data))
                    return true;

                serviceReqCode = !String.IsNullOrWhiteSpace(data.SERVICE_REQ_CODE) ? data.SERVICE_REQ_CODE : data.SERVICE_CODE;
                if (String.IsNullOrWhiteSpace(this.treatmentCode) || String.IsNullOrWhiteSpace(serviceReqCode))
                {
                    ShowCannotCheckEmrDocument(serviceReqCode);
                    return false;
                }

                CommonParam paramEmr = new CommonParam();
                EmrDocumentFilter filter = new EmrDocumentFilter();
                filter.TREATMENT_CODE__EXACT = this.treatmentCode;
                List<EMR_DOCUMENT> documents = new BackendAdapter(paramEmr).Get<List<EMR_DOCUMENT>>(Base.UriApi.EMR_DOCUMENT_GET, ApiConsumers.EmrConsumer, filter, paramEmr);
                if (documents == null && IsFailedParam(paramEmr))
                {
                    Inventec.Common.Logging.LogSystem.Warn("Viec 55703: khong lay duoc van ban EMR de kiem tra xoa y lenh cung khoa"
                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceReqCode), serviceReqCode)
                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => paramEmr), paramEmr));
                    ShowCannotCheckEmrDocument(serviceReqCode);
                    return false;
                }

                List<string> blockedLines = Base.SameDepartmentDeleteChecker.GetBlockedLines(documents, new List<string>() { serviceReqCode },
                    Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(),
                    Resources.ResourceMessage.XoaCungKhoaYLenhDaKySo,
                    Resources.ResourceMessage.XoaCungKhoaVanBanDoNguoiKhacTao);
                if (blockedLines.Count == 0)
                    return true;

                DevExpress.XtraEditors.XtraMessageBox.Show(
                    Resources.ResourceMessage.XoaCungKhoaKhongXoaDuoc + Environment.NewLine + String.Join(Environment.NewLine, blockedLines),
                    Resources.ResourceMessage.ThongBao,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                ShowCannotCheckEmrDocument(serviceReqCode);
                return false;
            }
        }

        private void ShowCannotCheckEmrDocument(string serviceReqCode)
        {
            try
            {
                DevExpress.XtraEditors.XtraMessageBox.Show(
                    String.Format(Resources.ResourceMessage.XoaCungKhoaKhongKiemTraDuocVanBanKy, serviceReqCode),
                    Resources.ResourceMessage.ThongBao,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private static bool IsFailedParam(CommonParam param)
        {
            return param != null
                && (param.HasException
                || (param.Messages != null && param.Messages.Count > 0)
                || (param.BugCodes != null && param.BugCodes.Count > 0));
        }
    }
}
