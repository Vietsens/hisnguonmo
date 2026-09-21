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
using HIS.Desktop.Plugins.Library.CheckRequireMediMate.ADO;
using HIS.Desktop.Plugins.Library.CheckRequireMediMate.Resources;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Library.CheckRequireMediMate
{
    /// <summary>
    /// Thu vien dung chung cho viec 3353 (PT-56272): canh bao "Dich vu chua co thuoc, vat tu di kem"
    /// khi KET THUC THUC HIEN dich vu da bat co HIS_SERVICE.IS_REQUIRE_MEDI_MATE = 1.
    ///
    /// Muc NHAC, KHONG CHAN: hien hop thoai Yes/No (mac dinh No). Yes = tiep tuc ket thuc, No = dung lai de ke bo sung.
    /// Loi ky thuat (mat mang, backend cu chua co cot...) -> ghi log va KHONG chan (fail-open).
    ///
    /// Cach dung tai man ket thuc (dat SAU cac kiem tra san co, TRUOC loi goi api/HisServiceReq/Finish|FinishWithTime):
    ///   if (!CheckRequireMediMateManager.CheckBeforeFinish(listSereServ)) return;
    /// Man ket thuc nhieu y lenh cung luc (Tra ket qua tong hop):
    ///   if (!CheckRequireMediMateManager.CheckBeforeFinishByServiceReqIds(serviceReqIds)) return;
    ///
    /// "Da co thuoc, vat tu di kem" = ton tai dong HIS_SERE_SERV con (PARENT_ID = ID dich vu) loai Thuoc/Vat tu,
    /// chua huy (API tu loai IS_DELETE = 1) va IS_NO_EXECUTE != 1. Khong xet so luong / chung loai / dinh muc.
    /// Co dich vu duoc doc "song" qua api/HisService/Get (khong dung cache RAM) de co hieu luc ngay sau khi sua danh muc.
    /// </summary>
    public static class CheckRequireMediMateManager
    {
        #region API cong khai - moi man ket thuc chi goi 1 dong

        /// <summary>Dung cho man co danh sach HIS_SERE_SERV (ServiceExecute, TestServiceReqExcute...)</summary>
        /// <returns>true = duoc phep ket thuc; false = nguoi dung chon dung lai de ke bo sung</returns>
        public static bool CheckBeforeFinish(IEnumerable<HIS_SERE_SERV> sereServs)
        {
            return CheckBeforeFinish(ConvertList(sereServs, SereServCheckADO.From));
        }

        /// <summary>Dung cho man co danh sach V_HIS_SERE_SERV</summary>
        public static bool CheckBeforeFinish(IEnumerable<V_HIS_SERE_SERV> sereServs)
        {
            return CheckBeforeFinish(ConvertList(sereServs, SereServCheckADO.From));
        }

        /// <summary>Dung cho man PTTT (SurgServiceReqExecute) co danh sach V_HIS_SERE_SERV_5</summary>
        public static bool CheckBeforeFinish(IEnumerable<V_HIS_SERE_SERV_5> sereServs)
        {
            return CheckBeforeFinish(ConvertList(sereServs, SereServCheckADO.From));
        }

        /// <summary>
        /// Dung cho man ket thuc NHIEU y lenh cung luc (Tra ket qua tong hop): thu vien tu lay dich vu cua cac y lenh
        /// qua api/HisSereServ/Get theo SERVICE_REQ_IDs, hoi MOT LAN cho tat ca.
        /// </summary>
        public static bool CheckBeforeFinishByServiceReqIds(List<long> serviceReqIds)
        {
            try
            {
                if (serviceReqIds == null || serviceReqIds.Count == 0)
                {
                    return true;
                }

                List<HIS_SERE_SERV> sereServs = GetSereServByServiceReqIds(serviceReqIds.Distinct().ToList());
                return CheckBeforeFinish(sereServs);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return true;
            }
        }

        /// <summary>Kieu chung: tim dich vu thieu thuoc/vat tu roi hoi nguoi dung.</summary>
        public static bool CheckBeforeFinish(List<SereServCheckADO> sereServs)
        {
            try
            {
                List<SereServCheckADO> missing = GetMissingMediMateServices(sereServs);
                if (missing == null || missing.Count == 0)
                {
                    return true;
                }
                return ConfirmFinish(missing);
            }
            catch (Exception ex)
            {
                // Loi ky thuat thi khong chan nguoi dung ket thuc (muc nhac)
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return true;
            }
        }

        #endregion

        #region Tim dich vu thieu thuoc, vat tu di kem

        /// <summary>
        /// Tra ve cac dich vu bat co "Co thuoc, vat tu di kem" nhung chua co dong thuoc/vat tu con hieu luc gan voi no.
        /// Luon tra ve list khac null. Loi API -> list rong (fail-open) + ghi log.
        /// </summary>
        public static List<SereServCheckADO> GetMissingMediMateServices(List<SereServCheckADO> sereServs)
        {
            List<SereServCheckADO> result = new List<SereServCheckADO>();
            try
            {
                if (sereServs == null || sereServs.Count == 0)
                {
                    return result;
                }

                // Quy tac 6: bo dich vu "khong thuc hien" / da huy / thieu khoa
                List<SereServCheckADO> candidates = sereServs
                    .Where(o => o != null && o.ID > 0 && o.SERVICE_ID > 0 && o.IS_DELETE != 1 && o.IS_NO_EXECUTE != 1)
                    .GroupBy(o => o.ID).Select(g => g.First())
                    .ToList();
                if (candidates.Count == 0)
                {
                    return result;
                }

                // Buoc 1: dich vu nao bat co (doc song tu backend, khong dung cache RAM)
                List<long> serviceIds = candidates.Select(o => o.SERVICE_ID).Distinct().ToList();
                HashSet<long> requireServiceIds = GetRequireServiceIds(serviceIds);
                if (requireServiceIds == null || requireServiceIds.Count == 0)
                {
                    // Quy tac 1: khong dich vu nao bat co -> khong goi them gi, ket thuc nhu cu
                    return result;
                }

                List<SereServCheckADO> requires = candidates.Where(o => requireServiceIds.Contains(o.SERVICE_ID)).ToList();
                if (requires.Count == 0)
                {
                    return result;
                }

                // Buoc 2: dich vu nao da co dong thuoc/vat tu con hieu luc gan voi no (PARENT_ID = ID dich vu)
                HashSet<long> parentIdsHavingMediMate = GetParentIdsHavingMediMate(requires.Select(o => o.ID).Distinct().ToList());
                if (parentIdsHavingMediMate == null)
                {
                    // Loi API -> khong ket luan duoc -> khong canh bao (fail-open)
                    return result;
                }

                result = requires.Where(o => !parentIdsHavingMediMate.Contains(o.ID)).ToList();

                Inventec.Common.Logging.LogSystem.Info("CheckRequireMediMateManager: "
                    + Inventec.Common.Logging.LogUtil.TraceData("requireServiceIds", requireServiceIds)
                    + Inventec.Common.Logging.LogUtil.TraceData("parentIdsHavingMediMate", parentIdsHavingMediMate)
                    + Inventec.Common.Logging.LogUtil.TraceData("missing", result.Select(o => new { o.ID, o.SERVICE_REQ_ID, o.TDL_SERVICE_CODE, o.TDL_SERVICE_NAME }).ToList()));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                result = new List<SereServCheckADO>();
            }
            return result;
        }

        #endregion

        #region Hop thoai

        /// <summary>
        /// Hien hop thoai Yes/No (mac dinh No) liet ke du cac dich vu thieu thuoc/vat tu (quy tac 5: hoi MOT lan cho tat ca).
        /// </summary>
        /// <returns>true = nguoi dung chon tiep tuc ket thuc</returns>
        public static bool ConfirmFinish(List<SereServCheckADO> missing)
        {
            try
            {
                if (missing == null || missing.Count == 0)
                {
                    return true;
                }

                string message = BuildMessage(missing);
                Inventec.Common.Logging.LogSystem.Info("CheckRequireMediMateManager.ConfirmFinish: " + message
                    + Inventec.Common.Logging.LogUtil.TraceData("serviceReqIds", missing.Select(o => o.SERVICE_REQ_ID).Distinct().ToList()));

                DialogResult dialogResult = DevExpress.XtraEditors.XtraMessageBox.Show(
                    message,
                    ResourceMessage.ThongBao,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2);

                Inventec.Common.Logging.LogSystem.Info("CheckRequireMediMateManager.ConfirmFinish: nguoi dung chon " + dialogResult);
                return dialogResult == DialogResult.Yes;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return true;
            }
        }

        /// <summary>"Dịch vụ chưa có thuốc, vật tư đi kèm:" + moi dich vu 1 dong "- Ma - Ten" + cau hoi tiep tuc</summary>
        public static string BuildMessage(List<SereServCheckADO> missing)
        {
            StringBuilder lines = new StringBuilder();
            try
            {
                if (missing != null)
                {
                    foreach (var item in missing.GroupBy(o => o.ID).Select(g => g.First()))
                    {
                        if (lines.Length > 0)
                        {
                            lines.Append(Environment.NewLine);
                        }
                        lines.Append("- ");
                        if (!String.IsNullOrWhiteSpace(item.TDL_SERVICE_CODE))
                        {
                            lines.Append(item.TDL_SERVICE_CODE).Append(" - ");
                        }
                        lines.Append(item.TDL_SERVICE_NAME);
                    }
                }
                return String.Format(ResourceMessage.DichVuChuaCoThuocVatTuDiKem, lines.ToString());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return lines.ToString();
        }

        #endregion

        #region Goi backend

        /// <summary>
        /// api/HisService/Get theo IDs, nhan vao ADO cuc bo de doc cot moi IS_REQUIRE_MEDI_MATE.
        /// Tra ve tap SERVICE_ID bat co; null khi loi API.
        /// </summary>
        private static HashSet<long> GetRequireServiceIds(List<long> serviceIds)
        {
            try
            {
                if (serviceIds == null || serviceIds.Count == 0)
                {
                    return new HashSet<long>();
                }

                CommonParam param = new CommonParam();
                HisServiceFilter filter = new HisServiceFilter();
                filter.IDs = serviceIds;
                // Khong loc IS_ACTIVE: dich vu bi khoa sau khi da chi dinh van phai kiem tra

                List<HisServiceRequireADO> services = new BackendAdapter(param).Get<List<HisServiceRequireADO>>(
                    HisRequestUriStore.MOSHIS_SERVICE_GET,
                    HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                    filter,
                    param);

                if (services == null)
                {
                    Inventec.Common.Logging.LogSystem.Warn("CheckRequireMediMateManager.GetRequireServiceIds: api/HisService/Get tra ve null"
                        + Inventec.Common.Logging.LogUtil.TraceData("serviceIds", serviceIds)
                        + Inventec.Common.Logging.LogUtil.TraceData("param", param));
                    return null;
                }

                return new HashSet<long>(services.Where(o => o != null && o.IS_REQUIRE_MEDI_MATE == 1).Select(o => o.ID));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }

        /// <summary>
        /// api/HisSereServ/Get theo PARENT_IDs + loai Thuoc/Vat tu. API tu loai IS_DELETE = 1; loc them IS_NO_EXECUTE.
        /// Tra ve tap PARENT_ID (= ID dich vu) da co thuoc/vat tu; rong = chua dich vu nao co; null khi loi API.
        /// </summary>
        private static HashSet<long> GetParentIdsHavingMediMate(List<long> parentIds)
        {
            try
            {
                if (parentIds == null || parentIds.Count == 0)
                {
                    return new HashSet<long>();
                }

                CommonParam param = new CommonParam();
                HisSereServFilter filter = new HisSereServFilter();
                filter.PARENT_IDs = parentIds;
                filter.TDL_SERVICE_TYPE_IDs = new List<long>()
                {
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT
                };

                List<HIS_SERE_SERV> children = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>(
                    HisRequestUriStore.MOSHIS_SERE_SERV_GET,
                    HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                    filter,
                    param);

                if (children == null)
                {
                    if (param.HasException)
                    {
                        Inventec.Common.Logging.LogSystem.Warn("CheckRequireMediMateManager.GetParentIdsHavingMediMate: api/HisSereServ/Get loi"
                            + Inventec.Common.Logging.LogUtil.TraceData("parentIds", parentIds)
                            + Inventec.Common.Logging.LogUtil.TraceData("param", param));
                        return null;
                    }
                    // Khong co du lieu -> chua dich vu nao co thuoc/vat tu
                    return new HashSet<long>();
                }

                return new HashSet<long>(children
                    .Where(o => o != null && o.PARENT_ID.HasValue && o.IS_DELETE != 1 && o.IS_NO_EXECUTE != 1)
                    .Select(o => o.PARENT_ID.Value));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }

        /// <summary>Dich vu cua cac y lenh (man Tra ket qua tong hop). API tu loai IS_DELETE = 1.</summary>
        private static List<HIS_SERE_SERV> GetSereServByServiceReqIds(List<long> serviceReqIds)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisSereServFilter filter = new HisSereServFilter();
                filter.SERVICE_REQ_IDs = serviceReqIds;

                List<HIS_SERE_SERV> result = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>(
                    HisRequestUriStore.MOSHIS_SERE_SERV_GET,
                    HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                    filter,
                    param);
                if (result == null && param.HasException)
                {
                    Inventec.Common.Logging.LogSystem.Warn("CheckRequireMediMateManager.GetSereServByServiceReqIds: api/HisSereServ/Get loi"
                        + Inventec.Common.Logging.LogUtil.TraceData("serviceReqIds", serviceReqIds)
                        + Inventec.Common.Logging.LogUtil.TraceData("param", param));
                }
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return null;
        }

        #endregion

        #region Tien ich

        private static List<SereServCheckADO> ConvertList<T>(IEnumerable<T> items, Func<T, SereServCheckADO> map)
        {
            List<SereServCheckADO> result = new List<SereServCheckADO>();
            try
            {
                if (items == null)
                {
                    return result;
                }
                foreach (T item in items)
                {
                    SereServCheckADO ado = map(item);
                    if (ado != null)
                    {
                        result.Add(ado);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        #endregion
    }
}
