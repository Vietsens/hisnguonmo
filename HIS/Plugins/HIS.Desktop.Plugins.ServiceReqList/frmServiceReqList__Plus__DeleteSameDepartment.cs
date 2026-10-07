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
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ServiceReqList
{
    public partial class frmServiceReqList : HIS.Desktop.Utility.FormBase
    {
        /// <summary>
        /// Khoa cua phong dang lam viec (khoa duoc xoa y lenh theo quy tac cung khoa - viec 55703).
        /// </summary>
        private long GetWorkingDepartmentId()
        {
            return this.currentRoom != null ? this.currentRoom.DEPARTMENT_ID : 0;
        }

        /// <summary>
        /// Viec 55703: y lenh duoc xoa theo quy tac cung khoa (key bat, chua xu ly, cung khoa chi dinh).
        /// Dung de bat nut Xoa tren luoi va menu Xoa chuot phai.
        /// </summary>
        private bool IsDeleteAllowedBySameDepartment(HIS_SERVICE_REQ data)
        {
            try
            {
                return data != null && Base.SameDepartmentDeleteChecker.IsAllowedBySameDepartment(
                    HisConfigCFG.IsAllowDeleteBySameRequestDepartment,
                    data.REQUEST_DEPARTMENT_ID,
                    data.SERVICE_REQ_STT_ID,
                    GetWorkingDepartmentId());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
        }

        /// <summary>
        /// Y lenh CHI xoa duoc nho quy tac cung khoa (tai khoan khong co quyen xoa san co) - phai kiem tra van ban ky
        /// tren EMR truoc khi xoa. Y lenh co quyen san co giu nguyen luong xoa cu.
        /// </summary>
        private bool IsDeleteOnlyBySameDepartment(HIS_SERVICE_REQ data)
        {
            try
            {
                return IsDeleteAllowedBySameDepartment(data)
                    && !Base.SameDepartmentDeleteChecker.HasOwnDeleteRight(
                        data.CREATOR,
                        data.REQUEST_LOGINNAME,
                        data.SERVICE_REQ_TYPE_ID,
                        data.REQUEST_DEPARTMENT_ID,
                        this.loginName,
                        CheckLoginAdmin.IsAdmin(this.loginName),
                        this.hasDeleteBedPermission,
                        GetWorkingDepartmentId());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
        }

        /// <summary>
        /// Viec 55703: truoc khi xoa cac y lenh chi duoc xoa theo quy tac cung khoa, kiem tra van ban EMR cua y lenh.
        /// Chan khi van ban da ky so, hoac van ban chua ky do tai khoan khac tao, hoac khong kiem tra duoc EMR.
        /// Tra ve false khi phai dung xoa (da hien thong bao).
        /// </summary>
        private bool CheckEmrDocumentBeforeSameDepartmentDelete(List<ADO.ServiceReqADO> serviceReqs)
        {
            List<string> checkCodes = null;
            try
            {
                if (serviceReqs == null || serviceReqs.Count == 0 || !HisConfigCFG.HasConnectionEmr)
                    return true;

                List<ADO.ServiceReqADO> checkReqs = serviceReqs.Where(o => o != null && IsDeleteOnlyBySameDepartment(o)).ToList();
                if (checkReqs.Count == 0)
                    return true;

                checkCodes = checkReqs.Select(o => o.SERVICE_REQ_CODE).Distinct().ToList();
                List<string> blockedLines = new List<string>();
                foreach (var treatmentGroup in checkReqs.GroupBy(o => o.TDL_TREATMENT_CODE))
                {
                    List<string> serviceReqCodes = treatmentGroup.Select(o => o.SERVICE_REQ_CODE).Distinct().ToList();
                    if (String.IsNullOrWhiteSpace(treatmentGroup.Key))
                    {
                        ShowCannotCheckEmrDocument(serviceReqCodes);
                        return false;
                    }

                    CommonParam paramEmr = new CommonParam();
                    EmrDocumentFilter filter = new EmrDocumentFilter();
                    filter.TREATMENT_CODE__EXACT = treatmentGroup.Key;
                    List<EMR_DOCUMENT> documents = new BackendAdapter(paramEmr).Get<List<EMR_DOCUMENT>>(RequestUriStore.EMR_DOCUMENT_GET, ApiConsumers.EmrConsumer, filter, paramEmr);
                    if (documents == null && IsFailedParam(paramEmr))
                    {
                        Inventec.Common.Logging.LogSystem.Warn("Viec 55703: khong lay duoc van ban EMR de kiem tra xoa y lenh cung khoa"
                            + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceReqCodes), serviceReqCodes)
                            + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => paramEmr), paramEmr));
                        ShowCannotCheckEmrDocument(serviceReqCodes);
                        return false;
                    }

                    blockedLines.AddRange(Base.SameDepartmentDeleteChecker.GetBlockedLines(documents, serviceReqCodes, this.loginName,
                        Resources.ResourceMessage.XoaCungKhoaYLenhDaKySo,
                        Resources.ResourceMessage.XoaCungKhoaVanBanDoNguoiKhacTao));
                }

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
                ShowCannotCheckEmrDocument(checkCodes);
                return false;
            }
        }

        private void ShowCannotCheckEmrDocument(List<string> serviceReqCodes)
        {
            try
            {
                DevExpress.XtraEditors.XtraMessageBox.Show(
                    String.Format(Resources.ResourceMessage.XoaCungKhoaKhongKiemTraDuocVanBanKy, serviceReqCodes != null ? String.Join(", ", serviceReqCodes) : ""),
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
