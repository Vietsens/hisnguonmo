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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.BedRoomPartial.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.BedRoomPartial
{
    /// <summary>
    /// Hien thi don thuoc du tru theo ngay du tru tren man Buong benh.
    /// Quy tac nghiep vu QT-01..QT-13 — xem tai lieu
    /// docs/A_NghiepVu_DonDuTruTheoNgayDuTru_BuongBenh.docx
    /// </summary>
    public partial class UCBedRoomPartial
    {
        #region Declare

        /// <summary>
        /// Loai y lenh duoc coi la "don thuoc" (QT-02).
        /// DONM (don MAU) bi loai tru co chu dich — xem GlobalStore cua AssignServiceEdit
        /// map DONM sang HIS_SERVICE_TYPE.ID__MAU.
        /// </summary>
        private static readonly HashSet<long> MEDICINE_SERVICE_REQ_TYPE_IDS = new HashSet<long>
        {
            IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK,
            IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT,
            IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT
        };

        /// <summary>Nguong so ngay dieu tri — vuot qua thi khong cache toan dot (fallback theo ngay).</summary>
        private const int ANTICIPATE_MAX_DAYS = 90;

        private const string URI__HIS_SERVICE_REQ_GET = "api/HisServiceReq/Get";
        private const string URI__HIS_SERE_SERV_GET_D2 = "api/HisSereServ/GetDHisSereServ2";

        /// <summary>Tien to khoa nhom don du tru o tab "Tat ca" (QT-09).</summary>
        private const string ANTICIPATE_GROUP_PREFIX = "A_";

        /// <summary>Cau hinh bat/tat — doc 1 lan trong Load (QT-11).</summary>
        private bool isShowAnticipateByUseDate;

        /// <summary>Toan bo y lenh cua dot dieu tri. Key = HIS_SERVICE_REQ.ID.</summary>
        private Dictionary<long, HIS_SERVICE_REQ> dictServiceReqOfTreatment;

        /// <summary>serviceReqId -> ngay hieu luc (yyyyMMdd000000). Moi y lenh dung 1 ngay (QT-03).</summary>
        private Dictionary<long, long> dictEffectiveDateByReqId;

        /// <summary>Id cac y lenh la don thuoc du tru — dung danh dau mau va dien 2 cot (QT-07, QT-08).</summary>
        private HashSet<long> anticipateReqIds;

        /// <summary>
        /// Toan bo dong dich vu cua dot dieu tri.
        /// Null khi cau hinh tat HOAC khi dot dieu tri vuot ANTICIPATE_MAX_DAYS (fallback).
        /// </summary>
        private List<DHisSereServ2> allSereServOfTreatment;

        /// <summary>Ma dieu tri dang cache — tranh nap lai khi chua doi benh nhan.</summary>
        private long cachedTreatmentId;

        #endregion

        #region Helper

        /// <summary>Chuyen thoi gian dang yyyyMMddHHmmss ve dau ngay yyyyMMdd000000.</summary>
        private static long ToDateNumber(long timeNumber)
        {
            if (timeNumber <= 0) return 0;
            return (timeNumber / 1000000) * 1000000;
        }

        /// <summary>
        /// True neu y lenh la don THUOC du tru (QT-01 + QT-02).
        /// Ve so sanh ngay la BAT BUOC: mot so luong ke don gan USE_TIME = INTRUCTION_TIME
        /// khi nguoi dung bo trong o "Du tru" — chi kiem tra HasValue se hieu nham don thuong.
        /// </summary>
        private bool IsAnticipateMedicinePres(HIS_SERVICE_REQ req)
        {
            try
            {
                if (req == null || !req.USE_TIME.HasValue) return false;
                if (!MEDICINE_SERVICE_REQ_TYPE_IDS.Contains(req.SERVICE_REQ_TYPE_ID)) return false;

                return ToDateNumber(req.USE_TIME.Value) != ToDateNumber(req.INTRUCTION_TIME);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
        }

        /// <summary>
        /// Ngay hieu luc cua 1 y lenh (QT-03).
        /// Don thuoc du tru -> ngay du tru. Con lai -> ngay ke.
        /// </summary>
        private long GetEffectiveDate(HIS_SERVICE_REQ req)
        {
            try
            {
                if (req == null) return 0;
                if (IsAnticipateMedicinePres(req)) return ToDateNumber(req.USE_TIME.Value);
                return ToDateNumber(req.INTRUCTION_TIME);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return 0;
            }
        }

        /// <summary>
        /// Dung Dictionary tra cuu y lenh theo Id tu danh sach da co.
        /// Thay cho cac doan goi dataServiceReq.Where(...) lap nhieu lan trong vong lap.
        /// </summary>
        private static Dictionary<long, HIS_SERVICE_REQ> BuildServiceReqDictionary(List<HIS_SERVICE_REQ> serviceReqs)
        {
            try
            {
                if (serviceReqs == null || serviceReqs.Count == 0)
                    return new Dictionary<long, HIS_SERVICE_REQ>();

                return serviceReqs.GroupBy(o => o.ID).ToDictionary(g => g.Key, g => g.First());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return new Dictionary<long, HIS_SERVICE_REQ>();
            }
        }

        #endregion

        /// <summary>
        /// Bat/an 2 cot "Ngay ke" va "Ngay du tru" tren ca 4 tab theo cau hinh (QT-07, QT-11).
        /// Goi trong Load, sau khi doc cau hinh va sau AddUc.
        /// </summary>
        private void SetAnticipateColumnsVisible()
        {
            try
            {
                if (ucAll != null) ucAll.SetAnticipateColumnsVisible(isShowAnticipateByUseDate);
                if (ucCLS != null) ucCLS.SetAnticipateColumnsVisible(isShowAnticipateByUseDate);
                if (ucMediMate != null) ucMediMate.SetAnticipateColumnsVisible(isShowAnticipateByUseDate);
                if (ucOrther != null) ucOrther.SetAnticipateColumnsVisible(isShowAnticipateByUseDate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Cache

        /// <summary>Xoa cache — goi khi doi benh nhan hoac cau hinh tat.</summary>
        private void ClearAnticipateCache()
        {
            dictServiceReqOfTreatment = null;
            dictEffectiveDateByReqId = null;
            anticipateReqIds = null;
            allSereServOfTreatment = null;
            cachedTreatmentId = 0;
        }

        /// <summary>
        /// Nap toan bo y lenh cua dot dieu tri, dung map ngay hieu luc.
        /// BAT BUOC goi trong SelectPatient, TRUOC LoadDataDateByTreatmentToTreeList.
        /// Khong lam gi khi cau hinh tat (QT-11).
        /// </summary>
        private void LoadAnticipateCache(long treatmentId)
        {
            try
            {
                if (!isShowAnticipateByUseDate)
                {
                    ClearAnticipateCache();
                    return;
                }

                if (treatmentId <= 0)
                {
                    ClearAnticipateCache();
                    return;
                }

                // Chua doi benh nhan -> dung lai cache
                if (cachedTreatmentId == treatmentId && dictEffectiveDateByReqId != null) return;

                ClearAnticipateCache();

                CommonParam param = new CommonParam();
                HisServiceReqFilter filter = new HisServiceReqFilter();
                filter.TREATMENT_ID = treatmentId;

                Inventec.Common.Logging.LogSystem.Debug(
                    Inventec.Common.Logging.LogUtil.TraceData(
                        Inventec.Common.Logging.LogUtil.GetMemberName(() => treatmentId), treatmentId));

                var serviceReqs = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>(
                    URI__HIS_SERVICE_REQ_GET, ApiConsumers.MosConsumer, filter, param);

                if (serviceReqs == null || serviceReqs.Count == 0)
                {
                    // Khong co y lenh -> giu cache rong, cay ngay se dung tu API group-by-date
                    cachedTreatmentId = treatmentId;
                    dictServiceReqOfTreatment = new Dictionary<long, HIS_SERVICE_REQ>();
                    dictEffectiveDateByReqId = new Dictionary<long, long>();
                    anticipateReqIds = new HashSet<long>();
                    return;
                }

                dictServiceReqOfTreatment = BuildServiceReqDictionary(serviceReqs);
                dictEffectiveDateByReqId = new Dictionary<long, long>(dictServiceReqOfTreatment.Count);
                anticipateReqIds = new HashSet<long>();

                foreach (var req in dictServiceReqOfTreatment.Values)
                {
                    long effDate = GetEffectiveDate(req);
                    if (effDate <= 0) continue;

                    dictEffectiveDateByReqId[req.ID] = effDate;
                    if (IsAnticipateMedicinePres(req)) anticipateReqIds.Add(req.ID);
                }

                cachedTreatmentId = treatmentId;

                LoadAllSereServOfTreatment(treatmentId);

                Inventec.Common.Logging.LogSystem.Debug(String.Format(
                    "LoadAnticipateCache: treatmentId={0}, serviceReq={1}, anticipate={2}, effectiveDate={3}, sereServCached={4}",
                    treatmentId,
                    dictServiceReqOfTreatment.Count,
                    anticipateReqIds.Count,
                    dictEffectiveDateByReqId.Values.Distinct().Count(),
                    allSereServOfTreatment != null ? allSereServOfTreatment.Count : -1));
            }
            catch (Exception ex)
            {
                ClearAnticipateCache();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Nap toan bo dong dich vu cua dot dieu tri de doi ngay khong phai goi API.
        /// Bo qua khi dot dieu tri qua dai — luc do dung che do goi theo ngay (fallback).
        /// </summary>
        private void LoadAllSereServOfTreatment(long treatmentId)
        {
            try
            {
                allSereServOfTreatment = null;
                if (dictEffectiveDateByReqId == null || dictEffectiveDateByReqId.Count == 0) return;

                int dayCount = dictEffectiveDateByReqId.Values.Distinct().Count();
                if (dayCount > ANTICIPATE_MAX_DAYS)
                {
                    Inventec.Common.Logging.LogSystem.Debug(String.Format(
                        "LoadAllSereServOfTreatment: bo qua cache toan dot, chuyen che do goi theo ngay. treatmentId={0}, soNgay={1}, nguong={2}",
                        treatmentId, dayCount, ANTICIPATE_MAX_DAYS));
                    return;
                }

                CommonParam param = new CommonParam();
                DHisSereServ2Filter filter = new DHisSereServ2Filter();
                filter.TREATMENT_ID = treatmentId;

                allSereServOfTreatment = new BackendAdapter(param).Get<List<DHisSereServ2>>(
                    URI__HIS_SERE_SERV_GET_D2, ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                allSereServOfTreatment = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region Loc theo ngay hieu luc

        /// <summary>
        /// Lay dong dich vu hien thi cho 1 ngay (QT-03).
        /// Co cache toan dot -> loc RAM, khong goi API.
        /// Khong co cache (dot dai) -> goi API theo tung ngay ke nguon roi loc lai.
        /// </summary>
        private List<DHisSereServ2> LoadSereServByEffectiveDate(long treatmentId, long dateNumber)
        {
            try
            {
                if (allSereServOfTreatment != null)
                    return FilterByEffectiveDate(allSereServOfTreatment, dateNumber);

                // Fallback: goi API cho tung ngay ke co y lenh hien thi o ngay dang chon
                List<DHisSereServ2> result = new List<DHisSereServ2>();
                CommonParam param = new CommonParam();

                foreach (long sourceDate in GetSourceInstructionDates(dateNumber))
                {
                    DHisSereServ2Filter filter = new DHisSereServ2Filter();
                    filter.TREATMENT_ID = treatmentId;
                    filter.INTRUCTION_DATE = sourceDate;

                    var part = new BackendAdapter(param).Get<List<DHisSereServ2>>(
                        URI__HIS_SERE_SERV_GET_D2, ApiConsumers.MosConsumer, filter, param);

                    if (part != null && part.Count > 0) result.AddRange(part);
                }

                return FilterByEffectiveDate(result, dateNumber);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return new List<DHisSereServ2>();
            }
        }

        /// <summary>Loc danh sach dong dich vu theo ngay hieu luc cua y lenh cha — O(n), tra cuu O(1).</summary>
        private List<DHisSereServ2> FilterByEffectiveDate(List<DHisSereServ2> source, long dateNumber)
        {
            try
            {
                if (source == null || source.Count == 0) return new List<DHisSereServ2>();
                if (dictEffectiveDateByReqId == null) return source;

                long effDate;
                return source
                    .Where(o => dictEffectiveDateByReqId.TryGetValue(o.SERVICE_REQ_ID ?? 0, out effDate)
                                && effDate == dateNumber)
                    .ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return new List<DHisSereServ2>();
            }
        }

        /// <summary>
        /// Cac ngay KE can goi API de lay du y lenh hien thi o ngay dang chon.
        /// Luon gom chinh ngay dang chon (don thuong) va ngay ke cua cac don du tru chuyen den.
        /// Chi dung cho che do fallback.
        /// </summary>
        private List<long> GetSourceInstructionDates(long dateNumber)
        {
            HashSet<long> dates = new HashSet<long>();
            try
            {
                dates.Add(dateNumber);
                if (dictServiceReqOfTreatment == null || dictEffectiveDateByReqId == null) return dates.ToList();

                foreach (var pair in dictEffectiveDateByReqId)
                {
                    if (pair.Value != dateNumber) continue;

                    HIS_SERVICE_REQ req;
                    if (dictServiceReqOfTreatment.TryGetValue(pair.Key, out req) && req != null)
                        dates.Add(ToDateNumber(req.INTRUCTION_TIME));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return dates.ToList();
        }

        #endregion

        #region Dung cay ngay

        /// <summary>
        /// Danh sach ngay dung cay ben trai, giam dan (QT-03, QT-04, QT-06, QT-10).
        /// Don du tru mang ngay du tru, don thuong mang ngay ke -> ngay ke chi con toan don
        /// du tru se TU DONG bien mat khoi cay.
        /// Van hop them ngay tu API group-by-date ma cache khong biet, de khong mat du lieu.
        /// </summary>
        private List<long> BuildTreeDates(List<HisServiceReqGroupByDateSDO> rs)
        {
            List<long> result = new List<long>();
            try
            {
                HashSet<long> dates = new HashSet<long>();

                if (dictEffectiveDateByReqId != null)
                {
                    foreach (long d in dictEffectiveDateByReqId.Values)
                    {
                        if (d > 0) dates.Add(d);
                    }
                }

                // Duong lui: ngay ma cache y lenh khong co ban ghi nao -> van giu lai
                if (rs != null && rs.Count > 0)
                {
                    HashSet<long> knownInstructionDates = new HashSet<long>();
                    if (dictServiceReqOfTreatment != null)
                    {
                        foreach (var req in dictServiceReqOfTreatment.Values)
                            knownInstructionDates.Add(ToDateNumber(req.INTRUCTION_TIME));
                    }

                    foreach (var item in rs)
                    {
                        long d = ToDateNumber(item.InstructionDate ?? 0);
                        if (d > 0 && !knownInstructionDates.Contains(d)) dates.Add(d);
                    }
                }

                result = dates.OrderByDescending(o => o).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// Dung node cay ngay theo ngay hieu luc: node cha = ngay, node con = moc to dieu tri.
        /// Chi dung khi cau hinh bat.
        /// </summary>
        private List<ServiceReqGroupByDateADO> BuildTreeNodesByEffectiveDate(
            long treatmentId, List<HisServiceReqGroupByDateSDO> rs, List<HIS_TRACKING> listTracking)
        {
            List<ServiceReqGroupByDateADO> result = new List<ServiceReqGroupByDateADO>();
            try
            {
                List<long> listDates = BuildTreeDates(rs);
                if (listDates.Count == 0) return result;

                List<long> listTreeListIDs = new List<long>();

                foreach (long dateNumber in listDates)
                {
                    ServiceReqGroupByDateADO adoParent = new ServiceReqGroupByDateADO(dateNumber, treatmentId);
                    listTreeListIDs.Add(adoParent.TREELIST_ID);
                    result.Add(adoParent);

                    if (listTracking == null || listTracking.Count == 0) continue;

                    string dateStr = dateNumber.ToString().Substring(0, 8);
                    foreach (var tracking in listTracking)
                    {
                        if (tracking.TRACKING_TIME.ToString().Substring(0, 8) != dateStr) continue;

                        ServiceReqGroupByDateADO adoChild = new ServiceReqGroupByDateADO(
                            adoParent, true, tracking.TRACKING_TIME, listTreeListIDs);
                        listTreeListIDs.Add(adoChild.TREELIST_ID);
                        adoChild.isParent = false;
                        adoChild.InstructionDate = tracking.TRACKING_TIME;
                        adoChild.TRACKING_ID = tracking.ID;
                        adoChild.TRACKING_TIME = tracking.TRACKING_TIME;
                        if (adoChild.TREELIST_ID > 0) result.Add(adoChild);
                    }
                }
            }
            catch (Exception ex)
            {
                result = new List<ServiceReqGroupByDateADO>();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        #endregion

        #region Dien du lieu hien thi

        /// <summary>
        /// Dien 2 cot Ngay ke / Ngay du tru va co danh dau cho 1 y lenh (QT-07, QT-08).
        /// Tra ve true neu y lenh la don thuoc du tru — khi do NGUOI GOI khong duoc de chuoi
        /// "Du tru: ..." vao cot Khoa yeu cau nua.
        /// </summary>
        private bool FillAnticipateDisplay(SereServADO ado, long? serviceReqId)
        {
            try
            {
                if (ado == null) return false;
                if (!isShowAnticipateByUseDate) return false;
                if (dictServiceReqOfTreatment == null || anticipateReqIds == null) return false;

                HIS_SERVICE_REQ req;
                if (!dictServiceReqOfTreatment.TryGetValue(serviceReqId ?? 0, out req) || req == null) return false;

                ado.INSTRUCTION_DATE_STR =
                    Inventec.Common.DateTime.Convert.TimeNumberToDateString(req.INTRUCTION_TIME);

                if (!anticipateReqIds.Contains(req.ID)) return false;

                ado.IS_ANTICIPATE = true;
                ado.USE_DATE_STR =
                    Inventec.Common.DateTime.Convert.TimeNumberToDateString(req.USE_TIME ?? 0);
                return true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
        }

        /// <summary>
        /// True neu y lenh la don du tru DEN TU NGAY KHAC ngay dang xem — dung de tach nhom
        /// rieng o tab "Tat ca" (QT-09).
        /// </summary>
        private bool IsAnticipateFromOtherDate(long? serviceReqId)
        {
            try
            {
                if (!isShowAnticipateByUseDate) return false;
                if (anticipateReqIds == null || serviceReqId == null) return false;
                return anticipateReqIds.Contains(serviceReqId.Value);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
        }

        /// <summary>
        /// Khoa nhom cap 1 cua tab "Tat ca" (QT-09).
        /// Don du tru den tu ngay khac -> nhom rieng theo ngay ke ("A_" + ngay ke).
        /// Con lai -> nhom theo to dieu tri, GIU NGUYEN dinh dang cu de config tat khong doi gi.
        /// </summary>
        private string BuildAllTabGroupKey(DHisSereServ2 item)
        {
            try
            {
                if (item == null) return "";

                if (IsAnticipateFromOtherDate(item.SERVICE_REQ_ID))
                {
                    HIS_SERVICE_REQ req;
                    if (dictServiceReqOfTreatment != null
                        && dictServiceReqOfTreatment.TryGetValue(item.SERVICE_REQ_ID ?? 0, out req)
                        && req != null)
                    {
                        return ANTICIPATE_GROUP_PREFIX + ToDateNumber(req.INTRUCTION_TIME);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return Convert.ToString(item != null ? item.TRACKING_TIME : null);
        }

        /// <summary>True neu khoa nhom la nhom don du tru rieng (QT-09).</summary>
        private static bool IsAnticipateGroupKey(string groupKey)
        {
            return !String.IsNullOrEmpty(groupKey) && groupKey.StartsWith(ANTICIPATE_GROUP_PREFIX);
        }

        /// <summary>Ten nhom "Du tru — ke ngay dd/MM/yyyy" o tab "Tat ca" (QT-09).</summary>
        private string BuildAnticipateGroupName(long? serviceReqId)
        {
            try
            {
                HIS_SERVICE_REQ req;
                if (dictServiceReqOfTreatment != null
                    && dictServiceReqOfTreatment.TryGetValue(serviceReqId ?? 0, out req)
                    && req != null)
                {
                    return String.Format(
                        Inventec.Common.Resource.Get.Value(
                            "IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__TREE__GROUP__ANTICIPATE",
                            Resources.ResourceLanguageManager.LanguageResource,
                            Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()),
                        Inventec.Common.DateTime.Convert.TimeNumberToDateString(req.INTRUCTION_TIME));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return "";
        }

        #endregion
    }
}
