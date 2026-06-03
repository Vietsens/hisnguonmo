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
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MPS.Processor.Mps000512.ADO
{
    /// <summary>
    /// Bộ tra cứu danh mục dựng MỘT LẦN cho cả hai bộ (main + suất ăn),
    /// thay cho FirstOrDefault O(n) lặp trên từng dòng dịch vụ (O(n*m)) của Mps000302.
    /// Tra cứu O(1) qua Dictionary/Lookup; chỉ đọc nên dùng song song an toàn.
    /// Khoá luôn ép về long qua Convert.ToInt64 để không phụ thuộc kiểu null/non-null của cột.
    /// </summary>
    public class SereServLookup
    {
        public Dictionary<long, V_HIS_SERVICE> ServiceById { get; private set; }
        public Dictionary<long, HIS_HEIN_SERVICE_TYPE> HeinTypeById { get; private set; }
        public Dictionary<long, V_HIS_ROOM> RoomById { get; private set; }
        public Dictionary<long, HIS_DEPARTMENT> DeptById { get; private set; }
        public Dictionary<long, HIS_MEDICINE_TYPE> MedicineTypeByServiceId { get; private set; }
        public Dictionary<long, HIS_MEDICINE_LINE> MedicineLineById { get; private set; }
        public Dictionary<long, HIS_PATIENT_TYPE> PatientTypeById { get; private set; }
        public Dictionary<long, HIS_SERVICE_UNIT> ServiceUnitById { get; private set; }
        public Dictionary<long, HIS_SERVICE_REQ> ServiceReqById { get; private set; }
        public ILookup<long, HIS_SERE_SERV_EXT> SereServExtBySereServId { get; private set; }
        public ILookup<long, HIS_SERE_SERV_DEPOSIT> DepositBySereServId { get; private set; }

        /// <summary>SERE_SERV_ID có bill chưa huỷ (IS_CANCEL != 1).</summary>
        public HashSet<long> PaidBillSereServIds { get; private set; }
        /// <summary>SERE_SERV_DEPOSIT_ID có ít nhất 1 bản ghi hoàn ứng (bất kỳ trạng thái).</summary>
        public HashSet<long> DepositIdsWithRepay { get; private set; }
        /// <summary>SERE_SERV_DEPOSIT_ID có hoàn ứng đã huỷ (IS_CANCEL == 1).</summary>
        public HashSet<long> DepositIdsWithCanceledRepay { get; private set; }

        public SereServLookup(
            List<V_HIS_SERVICE> services,
            List<HIS_HEIN_SERVICE_TYPE> heinServiceTypes,
            List<V_HIS_ROOM> rooms,
            List<HIS_DEPARTMENT> departments,
            List<HIS_MEDICINE_TYPE> medicineTypes,
            List<HIS_MEDICINE_LINE> medicineLines,
            List<HIS_PATIENT_TYPE> patientTypes,
            List<HIS_SERVICE_UNIT> serviceUnits,
            List<HIS_SERVICE_REQ> serviceReqs,
            List<HIS_SERE_SERV_EXT> sereServExts,
            List<HIS_SERE_SERV_BILL> sereServBills,
            List<HIS_SERE_SERV_DEPOSIT> sereServDeposits,
            List<HIS_SESE_DEPO_REPAY> seseDepoRepays)
        {
            this.ServiceById = ToDict(services, o => o.ID);
            this.HeinTypeById = ToDict(heinServiceTypes, o => o.ID);
            this.RoomById = ToDict(rooms, o => o.ID);
            this.DeptById = ToDict(departments, o => o.ID);
            this.MedicineTypeByServiceId = ToDict(medicineTypes, o => o.SERVICE_ID);
            this.MedicineLineById = ToDict(medicineLines, o => o.ID);
            this.PatientTypeById = ToDict(patientTypes, o => o.ID);
            this.ServiceUnitById = ToDict(serviceUnits, o => o.ID);
            this.ServiceReqById = ToDict(serviceReqs, o => o.ID);

            this.SereServExtBySereServId = (sereServExts ?? new List<HIS_SERE_SERV_EXT>())
                .ToLookup(o => Convert.ToInt64(o.SERE_SERV_ID));
            this.DepositBySereServId = (sereServDeposits ?? new List<HIS_SERE_SERV_DEPOSIT>())
                .ToLookup(o => Convert.ToInt64(o.SERE_SERV_ID));

            this.PaidBillSereServIds = ToIdSet(sereServBills != null ? sereServBills.Where(s => s.IS_CANCEL != 1) : null, s => s.SERE_SERV_ID);
            this.DepositIdsWithRepay = ToIdSet(seseDepoRepays, e => e.SERE_SERV_DEPOSIT_ID);
            this.DepositIdsWithCanceledRepay = ToIdSet(seseDepoRepays != null ? seseDepoRepays.Where(e => e.IS_CANCEL == 1) : null, e => e.SERE_SERV_DEPOSIT_ID);
        }

        private static Dictionary<long, T> ToDict<T>(IEnumerable<T> src, Func<T, object> key)
        {
            Dictionary<long, T> d = new Dictionary<long, T>();
            if (src != null)
            {
                foreach (T o in src)
                {
                    long k = Convert.ToInt64(key(o));
                    if (!d.ContainsKey(k))
                        d[k] = o;
                }
            }
            return d;
        }

        private static HashSet<long> ToIdSet<T>(IEnumerable<T> src, Func<T, object> key)
        {
            HashSet<long> h = new HashSet<long>();
            if (src != null)
            {
                foreach (T o in src)
                    h.Add(Convert.ToInt64(key(o)));
            }
            return h;
        }

        /// <summary>Tra cứu O(1); trả null nếu khoá rỗng/không có. Ép khoá qua Convert.ToInt64 để dung mọi kiểu.</summary>
        public static T Get<T>(Dictionary<long, T> d, object key) where T : class
        {
            if (d == null || key == null)
                return null;
            long k;
            try { k = Convert.ToInt64(key); }
            catch { return null; }
            T v;
            return d.TryGetValue(k, out v) ? v : null;
        }
    }
}
