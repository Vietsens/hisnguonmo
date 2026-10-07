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
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.ServiceReqList.Base
{
    /// <summary>
    /// Viec 55703 - Xoa y lenh do nguoi khac chi dinh trong cung khoa chi dinh
    /// (key MOS.HIS_SERVICE_REQ.ALLOW_DELETE_BY_SAME_REQUEST_DEPARTMENT = 1).
    /// Chi chua logic thuan (khong goi API, khong hien thong bao) de dung chung cho nut Xoa tren luoi,
    /// menu chuot phai 1 y lenh va xoa nhieu y lenh da tich.
    /// </summary>
    internal static class SameDepartmentDeleteChecker
    {
        private const string SERVICE_REQ_CODE_PREFIX = "SERVICE_REQ_CODE:";
        private const string PATIENT_SIGNER_PREFIX = "#@!@#";

        /// <summary>
        /// Quyen xoa san co truoc viec 55703 (khong phu thuoc key): quan tri, nguoi tao, nguoi chi dinh,
        /// y lenh giuong + quyen HIS000053, y lenh kham cung khoa chi dinh.
        /// Y lenh co quyen nay giu nguyen luong xoa cu, khong kiem tra them van ban ky.
        /// </summary>
        internal static bool HasOwnDeleteRight(string creator, string requestLoginName, long serviceReqTypeId, long requestDepartmentId,
            string loginName, bool isAdmin, bool hasDeleteBedPermission, long workingDepartmentId)
        {
            try
            {
                if (isAdmin)
                    return true;
                if (!String.IsNullOrWhiteSpace(loginName) && (loginName == creator || loginName == requestLoginName))
                    return true;
                if (serviceReqTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__G && hasDeleteBedPermission)
                    return true;
                if (serviceReqTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH && workingDepartmentId > 0 && requestDepartmentId == workingDepartmentId)
                    return true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return false;
        }

        /// <summary>
        /// Quy tac cung khoa: key bat, y lenh chua xu ly, khoa chi dinh trung khoa cua phong dang lam viec.
        /// </summary>
        internal static bool IsAllowedBySameDepartment(bool isConfigOn, long requestDepartmentId, long serviceReqSttId, long workingDepartmentId)
        {
            return isConfigOn
                && workingDepartmentId > 0
                && requestDepartmentId == workingDepartmentId
                && serviceReqSttId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL;
        }

        /// <summary>
        /// Van ban EMR thuoc y lenh: HIS_CODE chua "SERVICE_REQ_CODE:ma y lenh" va ky tu ngay sau ma khong phai chu so
        /// (tranh ma 123 khop nham ma 1234) - cung quy uoc voi ExamServiceReqExecute.
        /// </summary>
        internal static bool IsDocumentOfServiceReq(string hisCode, string serviceReqCode)
        {
            if (String.IsNullOrEmpty(hisCode) || String.IsNullOrWhiteSpace(serviceReqCode))
                return false;

            string token = SERVICE_REQ_CODE_PREFIX + serviceReqCode.Trim();
            int index = hisCode.IndexOf(token, StringComparison.OrdinalIgnoreCase);
            while (index >= 0)
            {
                int next = index + token.Length;
                if (next >= hisCode.Length || !Char.IsDigit(hisCode[next]))
                    return true;

                index = hisCode.IndexOf(token, index + 1, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        /// <summary>
        /// Danh gia van ban EMR cua cac y lenh chi duoc xoa theo quy tac cung khoa. Tra ve cac dong thong bao chan
        /// (danh sach rong = duoc xoa):
        ///  - Van ban da co nguoi ky (SIGNERS khac rong): y lenh da ky so -> chi nguoi chi dinh/nguoi tao/quan tri duoc xoa.
        ///  - Van ban chua ky nhung do tai khoan khac tao: EMR chi cho nguoi tao xoa van ban, neu van xoa y lenh se de lai
        ///    van ban mo coi -> nguoi tao van ban phai xoa van ban truoc.
        /// signedFormat: {0} ma y lenh, {1} ten van ban, {2} nguoi ky. otherCreatorFormat: {0} ma y lenh, {1} ten van ban, {2} nguoi tao.
        /// </summary>
        internal static List<string> GetBlockedLines(List<EMR_DOCUMENT> documents, List<string> serviceReqCodes, string loginName,
            string signedFormat, string otherCreatorFormat)
        {
            List<string> result = new List<string>();
            if (documents == null || documents.Count == 0 || serviceReqCodes == null || serviceReqCodes.Count == 0)
                return result;

            List<EMR_DOCUMENT> activeDocuments = documents.Where(o => o != null && o.IS_DELETE != 1).ToList();
            foreach (string serviceReqCode in serviceReqCodes.Where(o => !String.IsNullOrWhiteSpace(o)).Distinct())
            {
                foreach (EMR_DOCUMENT document in activeDocuments.Where(o => IsDocumentOfServiceReq(o.HIS_CODE, serviceReqCode)))
                {
                    if (!String.IsNullOrWhiteSpace(document.SIGNERS))
                    {
                        result.Add(String.Format(signedFormat, serviceReqCode, document.DOCUMENT_NAME, FormatSigners(document.SIGNERS)));
                    }
                    else if (document.IS_CAPTURE != 1 && document.CREATOR != loginName)
                    {
                        result.Add(String.Format(otherCreatorFormat, serviceReqCode, document.DOCUMENT_NAME, document.CREATOR));
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// SIGNERS cua EMR la danh sach tai khoan da ky, ngan cach dau phay; chu ky benh nhan luu dang "#@!@#ma benh nhan".
        /// </summary>
        private static string FormatSigners(string signers)
        {
            return String.Join(", ", signers.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(o => o.Trim())
                .Select(o => o.StartsWith(PATIENT_SIGNER_PREFIX) ? o.Substring(PATIENT_SIGNER_PREFIX.Length) : o));
        }
    }
}
