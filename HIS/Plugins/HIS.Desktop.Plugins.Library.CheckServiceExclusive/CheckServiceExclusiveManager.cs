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
using HIS.Desktop.Plugins.Library.CheckServiceExclusive.ADO;
using HIS.Desktop.Plugins.Library.CheckServiceExclusive.Form;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HIS.Desktop.Plugins.Library.CheckServiceExclusive
{
    /// <summary>
    /// Thu vien dung chung kiem tra "dich vu khong duoc phep chi dinh dong thoi" (viec 57452 / TTMB-TK-56258).
    /// Dung o cac man chi dinh dich vu: AssignService, AssignBed, AssignNoneMediService, AssignServiceEdit...
    ///
    /// Cach dung tai man chi dinh:
    ///   1. Luc Load form:  checkServiceExclusiveManager = new CheckServiceExclusiveManager(this.currentModule);
    ///   2. Truoc khi Luu:  isValid = isValid &amp;&amp; checkServiceExclusiveManager.ProcessCheck(assigningIds, assignedIds, ref mess);
    ///   3. (Tuy chon) canh bao mem khi tich chon: GetWarningMessage(...) roi gan vao icon loi tren luoi.
    ///
    /// Pham vi doi chieu: cac dich vu DA chi dinh trong CUNG 1 LAN DIEU TRI + cac dich vu DANG tich chon.
    /// Quan he loai tru la DOI XUNG: chi khai 1 ban ghi (A -&gt; B) nhung bat ca 2 chieu.
    /// </summary>
    public class CheckServiceExclusiveManager
    {
        private Inventec.Desktop.Common.Modules.Module currentModule;

        public CheckServiceExclusiveManager(Inventec.Desktop.Common.Modules.Module currentModule)
        {
            this.currentModule = currentModule;
        }

        /// <summary>
        /// Vien chua khai bao danh muc -> bo qua toan bo viec kiem tra.
        /// Nguoi goi nen kiem tra co nay truoc de khong phai dung du lieu dau vao.
        /// </summary>
        public static bool IsEmptyCatalog
        {
            get
            {
                try
                {
                    return ServiceExclusiveDataWorker.IsEmpty;
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                    return true;
                }
            }
        }

        /// <summary>Nap truoc danh muc vao RAM (goi luc Load form de khong tre luc bam Luu)</summary>
        public static void LoadData()
        {
            try
            {
                var temp = ServiceExclusiveDataWorker.DATA;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Xoa cache danh muc (goi sau khi sua danh muc o man Danh muc)</summary>
        public static void ResetData()
        {
            try
            {
                ServiceExclusiveDataWorker.Reset();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region Lay danh sach dich vu "da duoc chi dinh" dung quy tac nghiep vu

        /// <summary>
        /// Loc danh sach dich vu DA chi dinh con hieu luc trong lan dieu tri (quy tac 7, 8, 9 cua thiet ke 57452):
        ///  - bo dich vu da xoa (IS_DELETE = 1 hoac SERVICE_REQ_ID null)
        ///  - bo dich vu khong thuc hien (IS_NO_EXECUTE = 1) -- gom ca dich vu da bi DOI sang dich vu khac
        ///  - bo dong hao phi (IS_EXPEND = 1)
        ///  - bo cac dich vu thuoc phieu dang sua (excludeServiceReqIds)
        /// </summary>
        public static List<long> BuildAssignedServiceIds(IEnumerable<HIS_SERE_SERV> sereServs, List<long> excludeServiceReqIds = null)
        {
            List<long> result = new List<long>();
            try
            {
                if (sereServs == null)
                {
                    return result;
                }

                foreach (var item in sereServs)
                {
                    if (item == null) continue;
                    if (item.IS_DELETE == 1) continue;
                    if (!item.SERVICE_REQ_ID.HasValue) continue;
                    if (item.IS_NO_EXECUTE == 1) continue;
                    if (item.IS_EXPEND == 1) continue;
                    if (excludeServiceReqIds != null && excludeServiceReqIds.Contains(item.SERVICE_REQ_ID.Value)) continue;

                    result.Add(item.SERVICE_ID);
                }

                result = result.Distinct().ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>Ban dung cho view V_HIS_SERE_SERV (xem overload tren)</summary>
        public static List<long> BuildAssignedServiceIds(IEnumerable<V_HIS_SERE_SERV> sereServs, List<long> excludeServiceReqIds = null)
        {
            List<long> result = new List<long>();
            try
            {
                if (sereServs == null)
                {
                    return result;
                }

                foreach (var item in sereServs)
                {
                    if (item == null) continue;
                    if (item.IS_DELETE == 1) continue;
                    if (!item.SERVICE_REQ_ID.HasValue) continue;
                    if (item.IS_NO_EXECUTE == 1) continue;
                    if (item.IS_EXPEND == 1) continue;
                    if (excludeServiceReqIds != null && excludeServiceReqIds.Contains(item.SERVICE_REQ_ID.Value)) continue;

                    result.Add(item.SERVICE_ID);
                }

                result = result.Distinct().ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// Lay danh sach dich vu da chi dinh CUA CA LAN DIEU TRI truc tiep tu backend
        /// (api/HisSereServ/Get theo TREATMENT_ID), roi loc theo dung quy tac nghiep vu.
        ///
        /// Dung cho cac man hinh ma danh sach sere serv san co da bi thu hep (vi du chi trong ngay
        /// chi dinh, hoac chi cung loai y lenh) — pham vi kiem tra cua viec 57452 la CA LAN DIEU TRI.
        /// API nay da tu loai IS_DELETE = 1 va SERVICE_REQ_ID null o tang DAO.
        /// </summary>
        public static List<long> GetAssignedServiceIdsByTreatment(long treatmentId, List<long> excludeServiceReqIds = null)
        {
            List<long> result = new List<long>();
            try
            {
                if (treatmentId <= 0)
                {
                    return result;
                }

                CommonParam param = new CommonParam();
                MOS.Filter.HisSereServFilter filter = new MOS.Filter.HisSereServFilter();
                filter.TREATMENT_ID = treatmentId;

                List<HIS_SERE_SERV> sereServs = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_SERE_SERV>>(
                    HisRequestUriStore.MOSHIS_SERE_SERV_GET,
                    HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                    filter,
                    param);

                result = BuildAssignedServiceIds(sereServs, excludeServiceReqIds);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        #endregion

        #region Kiem tra truoc khi Luu

        /// <summary>
        /// Kiem tra truoc khi luu chi dinh.
        /// </summary>
        /// <param name="assigningServiceIds">SERVICE_ID cac dich vu nguoi dung dang tich chon de chi dinh</param>
        /// <param name="assignedServiceIds">SERVICE_ID cac dich vu da chi dinh con hieu luc trong lan dieu tri
        /// (nen lay qua BuildAssignedServiceIds)</param>
        /// <param name="messageError">Thong diep mo ta vi pham (de nguoi goi ghi log neu can)</param>
        /// <returns>true = duoc phep luu; false = khong cho luu (muc Chan, hoac muc Canh bao nhung nguoi dung chon Huy)</returns>
        public bool ProcessCheck(List<long> assigningServiceIds, List<long> assignedServiceIds, ref string messageError)
        {
            bool result = true;
            try
            {
                List<ServiceExclusiveViolationADO> violations = GetViolations(assigningServiceIds, assignedServiceIds);
                if (violations.Count == 0)
                {
                    return true;
                }

                messageError = BuildMessage(violations);
                Inventec.Common.Logging.LogSystem.Warn("CheckServiceExclusiveManager.ProcessCheck: " + messageError);

                bool hasBlock = violations.Any(o => o.HANDLE_TYPE_ID == (short)HandleType.Block);
                if (hasBlock)
                {
                    // Muc Chan: khong truyen callback -> form chi co nut Dong -> khong cho luu
                    using (frmServiceExclusive form = new frmServiceExclusive(this.currentModule, violations))
                    {
                        form.ShowDialog();
                    }
                    return false;
                }

                // Muc Canh bao: hien nut Tiep tuc, nguoi dung tu quyet dinh
                bool isContinue = false;
                using (frmServiceExclusive form = new frmServiceExclusive(this.currentModule, violations, o => isContinue = o))
                {
                    form.ShowDialog();
                }
                result = isContinue;
            }
            catch (Exception ex)
            {
                // Loi ky thuat thi khong chan nguoi dung lam viec (giong cach xu ly cua kiem tra tuong tac thuoc)
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = true;
            }
            return result;
        }

        #endregion

        #region Canh bao mem tren luoi

        /// <summary>
        /// Thong diep canh bao cho 1 dong tren luoi chi dinh (hien icon canh bao ngay khi tich chon).
        /// Tra ve null neu dich vu nay khong vi pham.
        /// </summary>
        public string GetWarningMessage(long assigningServiceId, List<long> assigningServiceIds, List<long> assignedServiceIds)
        {
            try
            {
                List<ServiceExclusiveViolationADO> violations = GetViolations(
                    new List<long> { assigningServiceId }, assignedServiceIds, assigningServiceIds);

                if (violations.Count == 0)
                {
                    return null;
                }
                return BuildMessage(violations);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }

        #endregion

        #region Core

        private List<ServiceExclusiveViolationADO> GetViolations(List<long> assigningServiceIds, List<long> assignedServiceIds)
        {
            return GetViolations(assigningServiceIds, assignedServiceIds, assigningServiceIds);
        }

        /// <summary>
        /// Dò quan he loai tru 2 chieu cho tung dich vu dang chi dinh, doi chieu voi:
        ///  (a) dich vu da chi dinh truoc do trong lan dieu tri
        ///  (b) cac dich vu khac dang duoc tich chon cung luc
        /// </summary>
        /// <param name="checkServiceIds">Cac dich vu can kiem tra</param>
        /// <param name="assignedServiceIds">Dich vu da chi dinh trong lan dieu tri</param>
        /// <param name="allAssigningServiceIds">Toan bo dich vu dang tich chon tren luoi</param>
        private List<ServiceExclusiveViolationADO> GetViolations(List<long> checkServiceIds,
            List<long> assignedServiceIds, List<long> allAssigningServiceIds)
        {
            List<ServiceExclusiveViolationADO> result = new List<ServiceExclusiveViolationADO>();
            try
            {
                if (checkServiceIds == null || checkServiceIds.Count == 0)
                {
                    return result;
                }

                List<V_HIS_SERVICE_EXCLUSIVE> catalog = ServiceExclusiveDataWorker.DATA;
                if (catalog.Count == 0)
                {
                    return result;
                }

                HashSet<long> assignedIds = new HashSet<long>(assignedServiceIds ?? new List<long>());
                HashSet<long> assigningIds = new HashSet<long>(allAssigningServiceIds ?? new List<long>());

                // Chong lap: 1 cap (A,B) chi bao 1 lan du dò duoc tu ca 2 phia
                HashSet<string> addedKeys = new HashSet<string>();

                foreach (long serviceId in checkServiceIds.Distinct())
                {
                    foreach (var pair in catalog)
                    {
                        long otherId;
                        if (pair.SERVICE_ID == serviceId && pair.EXCLUSIVE_ID != serviceId)
                        {
                            otherId = pair.EXCLUSIVE_ID;
                        }
                        else if (pair.EXCLUSIVE_ID == serviceId && pair.SERVICE_ID != serviceId)
                        {
                            otherId = pair.SERVICE_ID;
                        }
                        else
                        {
                            continue;
                        }

                        // (a) dich vu loai tru DA duoc chi dinh truoc do
                        if (assignedIds.Contains(otherId))
                        {
                            AddViolation(result, addedKeys, pair, otherId, serviceId, ViolationSource.Assigned);
                            continue;
                        }

                        // (b) dich vu loai tru DANG duoc tich chon cung luc
                        if (assigningIds.Contains(otherId))
                        {
                            AddViolation(result, addedKeys, pair, otherId, serviceId, ViolationSource.Assigning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void AddViolation(List<ServiceExclusiveViolationADO> result, HashSet<string> addedKeys,
            V_HIS_SERVICE_EXCLUSIVE pair, long assignedServiceId, long assigningServiceId, ViolationSource source)
        {
            try
            {
                // Khoa chuan hoa theo cap (khong phan biet chieu) de khong bao trung
                string key = (Math.Min(assignedServiceId, assigningServiceId)) + "_"
                           + (Math.Max(assignedServiceId, assigningServiceId));
                if (addedKeys.Contains(key))
                {
                    // Cung 1 cap nhung danh muc co nhieu ban ghi (vi du khai ca A->B lan B->A, muc khac nhau):
                    // rang buoc UK1 cua bang chi chan trung THEO CHIEU nen truong hop nay van xay ra duoc.
                    // Lay muc xu ly NANG NHAT (Chan thang Canh bao) de khong bo lot truong hop phai chan.
                    ServiceExclusiveViolationADO existed = result.FirstOrDefault(o =>
                        (Math.Min(o.ASSIGNED_SERVICE_ID, o.ASSIGNING_SERVICE_ID)) + "_"
                        + (Math.Max(o.ASSIGNED_SERVICE_ID, o.ASSIGNING_SERVICE_ID)) == key);
                    if (existed != null)
                    {
                        // Nang muc xu ly len muc nang nhat trong cac ban ghi cua cap
                        if (pair.HANDLE_TYPE_ID > existed.HANDLE_TYPE_ID)
                        {
                            existed.HANDLE_TYPE_ID = pair.HANDLE_TYPE_ID;
                        }
                        // Ghi chu la thong tin bo sung: ban ghi nao co thi hien, khong phu thuoc muc xu ly
                        if (String.IsNullOrWhiteSpace(existed.NOTE) && !String.IsNullOrWhiteSpace(pair.NOTE))
                        {
                            existed.NOTE = pair.NOTE;
                        }
                    }
                    return;
                }
                addedKeys.Add(key);

                ServiceExclusiveViolationADO ado = new ServiceExclusiveViolationADO();
                ado.ASSIGNED_SERVICE_ID = assignedServiceId;
                ado.ASSIGNING_SERVICE_ID = assigningServiceId;
                ado.HANDLE_TYPE_ID = pair.HANDLE_TYPE_ID;
                ado.NOTE = pair.NOTE;
                ado.SOURCE = source;

                // Ten dich vu lay thang tu view danh muc, khong phai tra lai BackendDataWorker
                if (pair.SERVICE_ID == assignedServiceId)
                {
                    ado.ASSIGNED_SERVICE_NAME = pair.SERVICE_NAME;
                    ado.ASSIGNING_SERVICE_NAME = pair.EXCLUSIVE_NAME;
                }
                else
                {
                    ado.ASSIGNED_SERVICE_NAME = pair.EXCLUSIVE_NAME;
                    ado.ASSIGNING_SERVICE_NAME = pair.SERVICE_NAME;
                }

                ado.ASSIGNED_SERVICE_NAME = GetServiceName(assignedServiceId, ado.ASSIGNED_SERVICE_NAME);
                ado.ASSIGNING_SERVICE_NAME = GetServiceName(assigningServiceId, ado.ASSIGNING_SERVICE_NAME);

                result.Add(ado);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Du phong khi view danh muc chua co ten dich vu</summary>
        private string GetServiceName(long serviceId, string nameFromCatalog)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(nameFromCatalog))
                {
                    return nameFromCatalog;
                }

                var service = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker
                    .Get<V_HIS_SERVICE>()
                    .FirstOrDefault(o => o.ID == serviceId);
                return service != null ? service.SERVICE_NAME : String.Empty;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return nameFromCatalog;
        }

        /// <summary>
        /// Gom thong diep theo dich vu da co, dung tung chu theo tai lieu 3342:
        /// "Bệnh nhân đã được chỉ định dịch vụ A không được phép chỉ định dịch vụ B, C"
        /// Neu cap khai bao co Ghi chu thi hien kem sau cau thong bao.
        /// </summary>
        private string BuildMessage(List<ServiceExclusiveViolationADO> violations)
        {
            StringBuilder builder = new StringBuilder();
            try
            {
                var groups = violations.GroupBy(o => o.ASSIGNED_SERVICE_NAME);
                foreach (var group in groups)
                {
                    string assigningNames = String.Join(", ", group.Select(o => o.ASSIGNING_SERVICE_NAME).Distinct().ToArray());
                    if (builder.Length > 0)
                    {
                        builder.Append(Environment.NewLine);
                    }
                    builder.Append(String.Format(
                        "Bệnh nhân đã được chỉ định dịch vụ {0} không được phép chỉ định dịch vụ {1}",
                        group.Key, assigningNames));

                    List<string> notes = group
                        .Where(o => !String.IsNullOrWhiteSpace(o.NOTE))
                        .Select(o => o.NOTE.Trim())
                        .Distinct()
                        .ToList();
                    if (notes.Count > 0)
                    {
                        builder.Append(". Ghi chú: ").Append(String.Join("; ", notes.ToArray()));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return builder.ToString();
        }

        #endregion
    }
}
