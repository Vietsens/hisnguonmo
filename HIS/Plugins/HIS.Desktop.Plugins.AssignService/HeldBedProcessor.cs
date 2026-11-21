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
using HIS.Desktop.LocalStorage.HisConfig;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.AssignService
{
    /// <summary>
    /// Viec 63227 (62909): giu giuong cho nguoi benh tu luc vao buong toi khi roi buong, chuyen khoa hoac ra vien.
    /// Bat khi khoa MOS.HIS_SERVICE_REQ.DEFAULT_BED_BY_LAST_ASSIGNED = 2.
    /// Nguoi dang giu giuong duoc tinh la dang nam giuong, ke ca khi chua ke giuong cho ngay tiep theo.
    /// Backend (api/HisBedLog/TakeBedsHeld) quyet dinh ai dang giu giuong, man hinh chi cong vao so nguoi dang nam.
    /// </summary>
    internal static class HeldBedProcessor
    {
        internal const string CONFIG_KEY__DEFAULT_BED_BY_LAST_ASSIGNED = "MOS.HIS_SERVICE_REQ.DEFAULT_BED_BY_LAST_ASSIGNED";
        private const string API__TAKE_BEDS_HELD = "api/HisBedLog/TakeBedsHeld";

        //Doc khoa toi da moi 10 giay mot lan: cac man goi theo tung giuong (vd HisBedADO), ma thieu dong cau hinh thi moi lan
        //doc HisConfigs ghi mot canh bao kem stack. Cau hinh nap lai trong luc chay (man cau hinh, doi chi nhanh) co hieu luc
        //sau toi da 10 giay.
        private static string keyValueCache;
        private static DateTime keyValueCacheTime = DateTime.MinValue;

        private static string GetKeyValue()
        {
            //Gio UTC; tuoi am (gio may bi chinh lui) coi la het han de gia tri cu khong bi giu qua 10 giay
            double age = (DateTime.UtcNow - keyValueCacheTime).TotalSeconds;
            if (keyValueCacheTime != DateTime.MinValue && age >= 0 && age < 10)
                return keyValueCache;
            string value = null;
            try
            {
                value = HisConfigs.Get<string>(CONFIG_KEY__DEFAULT_BED_BY_LAST_ASSIGNED);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            keyValueCache = value;
            keyValueCacheTime = DateTime.UtcNow;
            return value;
        }

        /// <summary>
        /// So lan tra cuu (nguoi dang giu giuong, ho so cua chinh nguoi benh) bi loi. Man hinh so truoc va sau mot lan nap de
        /// biet lan nap do co tin duoc khong (tra cuu loi lam thieu hoac thua so nguoi dang nam).
        /// </summary>
        internal static int HeldLookupFailCount;

        /// <summary>
        /// Khoa 01412 = 1 hoac 2: tu dien giuong gan nhat, hoi xac nhan khi doi giuong
        /// </summary>
        internal static bool IsDefaultBedByLastAssignedOn
        {
            get
            {
                string value = GetKeyValue();
                return value == "1" || value == "2";
            }
        }

        /// <summary>
        /// Khoa 01412 = 2: giu giuong, chan nguoi benh khac ke vao giuong da du nguoi
        /// </summary>
        internal static bool IsHoldBedOn
        {
            get
            {
                return GetKeyValue() == "2";
            }
        }

        /// <summary>
        /// So nguoi nam toi da dung de to mau va chan chon giuong.
        /// Khoa = 2 thi giuong chua khai bao so nguoi nam toi da coi la 1 nguoi, giong Backend.
        /// </summary>
        internal static long? GetCapacity(long? maxCapacity)
        {
            if (maxCapacity.HasValue)
                return maxCapacity;
            return IsHoldBedOn ? (long?)1 : null;
        }

        /// <summary>
        /// Lay nguoi dang giu giuong theo danh sach giuong hoac danh sach ho so. Loi thi tra ve danh sach rong.
        /// </summary>
        internal static List<HisBedHeldSDO> TakeBedsHeld(List<long> bedIds, List<long> treatmentIds)
        {
            List<HisBedHeldSDO> result = new List<HisBedHeldSDO>();
            try
            {
                if ((bedIds == null || bedIds.Count == 0) && (treatmentIds == null || treatmentIds.Count == 0))
                    return result;

                TakeBedsHeldSDO sdo = new TakeBedsHeldSDO();
                sdo.BedIds = bedIds != null && bedIds.Count > 0 ? bedIds.Distinct().ToList() : null;
                sdo.TreatmentIds = treatmentIds != null && treatmentIds.Count > 0 ? treatmentIds.Distinct().ToList() : null;
                CommonParam param = new CommonParam();
                Inventec.Common.Logging.LogSystem.Debug("Du lieu goi den api: " + API__TAKE_BEDS_HELD + ". " + Inventec.Common.Logging.LogUtil.TraceData("TakeBedsHeldSDO", sdo));
                var data = new BackendAdapter(param).Post<List<HisBedHeldSDO>>(API__TAKE_BEDS_HELD, HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                if (data != null)
                    result = data;
                else
                    HeldLookupFailCount++;
                Inventec.Common.Logging.LogSystem.Debug("Du lieu api " + API__TAKE_BEDS_HELD + " tra ve: " + Inventec.Common.Logging.LogUtil.TraceData("HisBedHeldSDO", result));
            }
            catch (Exception ex)
            {
                HeldLookupFailCount++;
                Inventec.Common.Logging.LogSystem.Warn(ex);
                result = new List<HisBedHeldSDO>();
            }
            return result;
        }

        /// <summary>
        /// Khoa = 2: them nguoi dang giu giuong vao danh sach ban ghi nam giuong de dem so nguoi dang nam, theo cac quy tac
        /// cua Backend (HisBedCapacityCheck) ve moc thoi gian va loai tru (cac man van dem theo so ban ghi, Backend dem theo
        /// ho so):
        /// - Moc cham nhau khong tinh la nam cung luc: bo ban ghi ket thuc dung luc (hoac truoc) gio bat dau va ban ghi bat dau
        ///   dung luc (hoac sau) gio ket thuc cua khoang dang chon. Gio ket thuc khong sau gio bat dau (dang sua do gio) thi chi
        ///   xet dung thoi diem bat dau.
        /// - Khong dem cac ho so dang thao tac va cac ho so khac cua cung nguoi benh: bo moi ban ghi cua cac ho so nay.
        /// - Moi nguoi dang giu giuong la mot ban ghi chep tu ban ghi gan nhat cua ho (giuong, lan vao buong, gio bat dau) voi
        ///   gio ket thuc rong, tuc la dang nam; ban ghi that cung ID trong danh sach duoc thay the de khong dem trung.
        /// ID cua ban ghi chep: ban ghi that cua nguoi giu con nam trong khoang dang chon thi giu ID THAT (cac man dung ID de
        /// tinh so nguoi nam ghep - api/HisBedLog/UpdateShareCount - van thay nguoi dang nam cung); ban ghi gan nhat cua ho da
        /// ket thuc truoc khoang thi dung ID AM: cac man bo qua ID am khi ghi so nguoi nam ghep nhung van dem nguoi nay.
        /// Tra ve danh sach moi, khong sua danh sach va doi tuong dau vao. Khoa khac 2 thi tra ve nguyen danh sach.
        /// </summary>
        /// <param name="bedLogs">Ban ghi nam giuong man hinh da lay</param>
        /// <param name="bedIds">Cac giuong dang hien thi</param>
        /// <param name="excludeTreatmentIds">Ho so dang thao tac</param>
        /// <param name="startTime">Gio bat dau cua khoang dang chon (rong = khong gioi han)</param>
        /// <param name="finishTime">Gio ket thuc cua khoang dang chon (rong = khong gioi han; khong sau gio bat dau = chi xet dung thoi diem bat dau)</param>
        internal static List<HIS_BED_LOG> AppendHeldBedLogs(List<HIS_BED_LOG> bedLogs, List<long> bedIds, List<long> excludeTreatmentIds, long? startTime, long? finishTime)
        {
            List<HIS_BED_LOG> result = bedLogs != null ? new List<HIS_BED_LOG>(bedLogs) : new List<HIS_BED_LOG>();
            try
            {
                if (!IsHoldBedOn || bedIds == null || bedIds.Count == 0)
                    return result;

                long? start = startTime.HasValue && startTime.Value > 0 ? startTime : null;
                long? finish = finishTime.HasValue && finishTime.Value > 0 && (!start.HasValue || finishTime.Value > start.Value) ? finishTime : null;
                //Gio ket thuc khong sau gio bat dau (vd vua sua gio bat dau, chua sua gio ket thuc): chi xet dung thoi diem bat dau.
                //Khong coi la khong gioi han vi se tinh ca nguoi bat dau nam sau do va ghi sai so nguoi nam ghep.
                bool atStartOnly = start.HasValue && !finish.HasValue && finishTime.HasValue && finishTime.Value > 0;

                if (start.HasValue)
                    result.RemoveAll(o => o.FINISH_TIME.HasValue && o.FINISH_TIME.Value <= start.Value);
                if (finish.HasValue)
                    result.RemoveAll(o => o.START_TIME >= finish.Value);
                if (atStartOnly)
                    result.RemoveAll(o => o.START_TIME > start.Value);

                List<HisBedHeldSDO> helds = TakeBedsHeld(bedIds, null);

                List<long> excludeIds = excludeTreatmentIds != null ? excludeTreatmentIds.Where(o => o > 0).Distinct().ToList() : new List<long>();
                //Khong ai nam hoac giu cac giuong nay trong khoang thi khong can tra ho so cua chinh nguoi benh
                if (excludeIds.Count > 0 && (result.Count > 0 || helds.Count > 0))
                {
                    //Ham nay them ho so khac cua cung nguoi benh vao excludeIds
                    List<long> occupantTbrIds = result.Select(o => o.TREATMENT_BED_ROOM_ID).Concat(helds.Select(o => o.TreatmentBedRoomId)).Where(o => o > 0).Distinct().ToList();
                    List<long> ownTreatmentBedRoomIds = GetOwnTreatmentBedRoomIds(excludeIds, occupantTbrIds);
                    ownTreatmentBedRoomIds.AddRange(helds.Where(o => excludeIds.Contains(o.TreatmentId)).Select(o => o.TreatmentBedRoomId));
                    if (ownTreatmentBedRoomIds.Count > 0)
                        result.RemoveAll(o => ownTreatmentBedRoomIds.Contains(o.TREATMENT_BED_ROOM_ID));
                    helds = helds.Where(o => !excludeIds.Contains(o.TreatmentId) && !ownTreatmentBedRoomIds.Contains(o.TreatmentBedRoomId)).ToList();
                }

                foreach (var held in helds)
                {
                    //Khoang dang chon ket thuc truoc (hoac dung luc) nguoi nay nam giuong: khong tinh. Ban ghi that cua ho
                    //bat dau tu moc do da bi bo o tren.
                    if (finish.HasValue && finish.Value <= held.StartTime)
                        continue;
                    if (atStartOnly && held.StartTime > start.Value)
                        continue;

                    HIS_BED_LOG real = result.FirstOrDefault(o => o.ID == held.BedLogId);
                    HIS_BED_LOG log = new HIS_BED_LOG();
                    log.ID = real != null ? real.ID : -held.BedLogId;
                    log.BED_ID = held.BedId;
                    log.TREATMENT_BED_ROOM_ID = held.TreatmentBedRoomId;
                    log.START_TIME = held.StartTime;
                    log.FINISH_TIME = null;
                    if (real != null)
                    {
                        log.SHARE_COUNT = real.SHARE_COUNT;
                        log.BED_SERVICE_TYPE_ID = real.BED_SERVICE_TYPE_ID;
                        log.PATIENT_TYPE_ID = real.PATIENT_TYPE_ID;
                        log.PRIMARY_PATIENT_TYPE_ID = real.PRIMARY_PATIENT_TYPE_ID;
                        log.SERVICE_REQ_ID = real.SERVICE_REQ_ID;
                    }

                    result.RemoveAll(o => o.ID == held.BedLogId || o.ID == -held.BedLogId);
                    result.Add(log);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>
        /// Lan vao buong (dang mo hoac da dong) cua cac ho so dang thao tac, cong cac lan vao buong trong occupantTbrIds (nguoi
        /// dang nam, dang giu cac giuong) thuoc ho so khac cua cung nguoi benh: Backend khong tinh nguoi benh voi chinh minh.
        /// Ho so khac cua cung nguoi benh duoc them vao excludeIds.
        /// - Luon lay lan vao buong cua chinh cac ho so dang thao tac (api/HisTreatmentBedRoom/Get theo TREATMENT_IDs), ke ca
        ///   khi cac buoc sau loi.
        /// - Mot nguoi benh: moi lan vao buong cua nguoi benh theo ma nguoi benh (api/HisTreatmentBedRoom/GetView1, 1 lan goi;
        ///   so dong theo so lan vao buong cua nguoi benh, khong theo so ho so hay so nguoi dang nam).
        /// - Nhieu nguoi benh (chi dinh nhieu nguoi benh cung luc): tra nguoi benh cua tung nguoi dang nam, dang giu giuong
        ///   (api/HisTreatmentBedRoom/Get theo IDs roi api/HisTreatment/GetDynamic cot ID, PATIENT_ID): so lan goi khong theo so
        ///   nguoi benh.
        /// Khong con ai khac ngoai chinh ho so dang thao tac thi khong tra them. Moi lan goi loi thi tang HeldLookupFailCount.
        /// </summary>
        private static List<long> GetOwnTreatmentBedRoomIds(List<long> excludeIds, List<long> occupantTbrIds)
        {
            List<long> ids = new List<long>();
            List<long> ownIds = excludeIds.ToList();
            try
            {
                for (int skip = 0; skip < ownIds.Count; skip += 500)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisTreatmentBedRoomFilter ownRoomFilter = new MOS.Filter.HisTreatmentBedRoomFilter();
                    ownRoomFilter.TREATMENT_IDs = ownIds.Skip(skip).Take(500).ToList();
                    var ownRooms = new BackendAdapter(param).Get<List<HIS_TREATMENT_BED_ROOM>>("api/HisTreatmentBedRoom/Get", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, ownRoomFilter, param);
                    if (ownRooms != null)
                        ids.AddRange(ownRooms.Select(o => o.ID));
                    else
                        HeldLookupFailCount++;
                }
            }
            catch (Exception ex)
            {
                HeldLookupFailCount++;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            try
            {
                List<long> otherTbrIds = occupantTbrIds != null ? occupantTbrIds.Where(o => o > 0 && !ids.Contains(o)).Distinct().ToList() : new List<long>();
                if (otherTbrIds.Count == 0)
                    return ids;

                //Ma nguoi benh cua cac ho so dang thao tac
                CommonParam param = new CommonParam();
                MOS.Filter.HisTreatmentFilter ownFilter = new MOS.Filter.HisTreatmentFilter();
                ownFilter.IDs = ownIds;
                ownFilter.ColumnParams = new List<string> { "ID", "PATIENT_ID", "TDL_PATIENT_CODE" };
                var owns = new BackendAdapter(param).Get<List<HIS_TREATMENT>>("api/HisTreatment/GetDynamic", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, ownFilter, param);
                if (owns == null)
                {
                    HeldLookupFailCount++;
                    return ids;
                }
                List<long> patientIds = owns.Where(o => o != null && o.PATIENT_ID > 0).Select(o => o.PATIENT_ID).Distinct().ToList();
                List<string> patientCodes = owns.Where(o => o != null && !String.IsNullOrWhiteSpace(o.TDL_PATIENT_CODE)).Select(o => o.TDL_PATIENT_CODE).Distinct().ToList();
                if (patientIds.Count == 0)
                    return ids;

                List<long> samePatientTbrIds = new List<long>();
                List<long> samePatientTreatmentIds = new List<long>();
                if (patientIds.Count == 1 && patientCodes.Count == 1)
                {
                    //Mot nguoi benh: moi lan vao buong cua nguoi benh theo ma nguoi benh
                    MOS.Filter.HisTreatmentBedRoomView1Filter roomFilter = new MOS.Filter.HisTreatmentBedRoomView1Filter();
                    roomFilter.TDL_PATIENT_CODE__EXACT = patientCodes[0];
                    var rooms = new BackendAdapter(param).Get<List<V_HIS_TREATMENT_BED_ROOM_1>>("api/HisTreatmentBedRoom/GetView1", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, roomFilter, param);
                    if (rooms == null)
                    {
                        HeldLookupFailCount++;
                        return ids;
                    }
                    foreach (var room in rooms)
                    {
                        if (room == null || !patientIds.Contains(room.PATIENT_ID))
                            continue;
                        samePatientTbrIds.Add(room.ID);
                        samePatientTreatmentIds.Add(room.TREATMENT_ID);
                    }
                }
                else
                {
                    //Nhieu nguoi benh: lan vao buong cua nguoi dang nam, dang giu giuong -> ho so -> nguoi benh
                    List<HIS_TREATMENT_BED_ROOM> otherRooms = new List<HIS_TREATMENT_BED_ROOM>();
                    for (int skip = 0; skip < otherTbrIds.Count; skip += 500)
                    {
                        MOS.Filter.HisTreatmentBedRoomFilter otherRoomFilter = new MOS.Filter.HisTreatmentBedRoomFilter();
                        otherRoomFilter.IDs = otherTbrIds.Skip(skip).Take(500).ToList();
                        var data = new BackendAdapter(param).Get<List<HIS_TREATMENT_BED_ROOM>>("api/HisTreatmentBedRoom/Get", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, otherRoomFilter, param);
                        if (data == null)
                        {
                            HeldLookupFailCount++;
                            return ids;
                        }
                        otherRooms.AddRange(data.Where(o => o != null));
                    }
                    List<long> otherTreatmentIds = otherRooms.Select(o => o.TREATMENT_ID).Distinct().ToList();
                    List<long> sameTreatmentIds = new List<long>();
                    for (int skip = 0; skip < otherTreatmentIds.Count; skip += 500)
                    {
                        MOS.Filter.HisTreatmentFilter otherFilter = new MOS.Filter.HisTreatmentFilter();
                        otherFilter.IDs = otherTreatmentIds.Skip(skip).Take(500).ToList();
                        otherFilter.ColumnParams = new List<string> { "ID", "PATIENT_ID" };
                        var data = new BackendAdapter(param).Get<List<HIS_TREATMENT>>("api/HisTreatment/GetDynamic", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, otherFilter, param);
                        if (data == null)
                        {
                            HeldLookupFailCount++;
                            return ids;
                        }
                        sameTreatmentIds.AddRange(data.Where(o => o != null && patientIds.Contains(o.PATIENT_ID)).Select(o => o.ID));
                    }
                    foreach (var room in otherRooms)
                    {
                        if (!sameTreatmentIds.Contains(room.TREATMENT_ID))
                            continue;
                        samePatientTbrIds.Add(room.ID);
                        samePatientTreatmentIds.Add(room.TREATMENT_ID);
                    }
                }

                foreach (long tbrId in samePatientTbrIds)
                {
                    if (!ids.Contains(tbrId))
                        ids.Add(tbrId);
                }
                foreach (long treatmentId in samePatientTreatmentIds)
                {
                    if (!excludeIds.Contains(treatmentId))
                        excludeIds.Add(treatmentId);
                }
            }
            catch (Exception ex)
            {
                HeldLookupFailCount++;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return ids;
        }
    }
}
