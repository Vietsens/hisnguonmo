/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 */
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000510.ADO;
using MPS.Processor.Mps000510.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MPS.Processor.Mps000510
{
    public partial class Mps000510Processor : AbstractProcessor
    {
        /// <summary>
        /// Xử lý dữ liệu đầu vào (tối ưu):
        ///  1) Dựng các Dictionary tra cứu MỘT LẦN  -> tra tên O(1), bỏ FirstOrDefault O(n²) của Mps000281.
        ///  2) Duyệt 1 lượt list view V_HIS_SERE_SERV_2 -> SereServADO (view đã denormalize sẵn,
        ///     không cần join tay Services/HeinServiceTypes/Rooms/MedicineLines).
        ///  3) Lọc theo khoa nếu cấu hình (FilterDepartmentId).
        ///  4) Gom dòng trùng (viện phí + đồng chi trả BHYT), khoá gom phụ thuộc GroupType.
        ///  5) Sắp xếp + tính nguồn khác.
        /// </summary>   
        internal void DataInputProcess()
        {
            try
            {
                // 1) Dựng dictionary tra cứu một lần
                Dictionary<long, HIS_HEIN_SERVICE_TYPE> heinTypeById = BuildDict(rdo.HeinServiceTypes, o => o.ID);
                Dictionary<long, V_HIS_ROOM> roomById = BuildDict(rdo.Rooms, o => o.ID);
                Dictionary<long, HIS_DEPARTMENT> deptById = BuildDict(rdo.Departments, o => o.ID);
                Dictionary<long, HIS_MEDICINE_LINE> medLineById = BuildDict(rdo.MedicineLines, o => o.ID);
                Dictionary<long, HIS_SERVICE_UNIT> unitById = BuildDict(rdo.HisServiceUnit, o => o.ID);
                Dictionary<long, V_HIS_SERVICE> serviceById = BuildDict(rdo.Services, o => o.ID);
                // medicineTypes key theo SERVICE_ID (giống FirstOrDefault của 281) -> fallback dòng thuốc khi view null
                Dictionary<long, HIS_MEDICINE_TYPE> medicineTypeByServiceId = new Dictionary<long, HIS_MEDICINE_TYPE>();
                if (rdo.medicineTypes != null)
                {
                    foreach (HIS_MEDICINE_TYPE mt in rdo.medicineTypes)
                    {
                        if (mt.SERVICE_ID > 0 && !medicineTypeByServiceId.ContainsKey(mt.SERVICE_ID))
                            medicineTypeByServiceId[mt.SERVICE_ID] = mt;
                    }
                }

                // [DIAG] TODO XÓA SAU KHI FIX: xác nhận DLL mới đang chạy + số lượng phòng/khoa nạp được
                Inventec.Common.Logging.LogSystem.Warn(string.Format(
                    "[Mps000510][DIAG] DataInputProcess START (build: room-from-306-ExeRoom) roomById={0} deptById={1} SereServs={2}",
                    roomById.Count, deptById.Count, rdo.SereServs != null ? rdo.SereServs.Count : -1));

                // 2) Map 1 lượt
                List<SereServADO> all = new List<SereServADO>();
                if (rdo.SereServs != null)
                {
                    foreach (V_HIS_SERE_SERV_2 r in rdo.SereServs)
                    {
                        all.Add(new SereServADO(r, heinTypeById, roomById, deptById, medLineById, unitById, serviceById, medicineTypeByServiceId));
                    }
                }

                // 3) Lọc theo khoa (nếu người dùng yêu cầu bảng kê 1 khoa)
                if (rdo.FilterDepartmentId.HasValue && rdo.FilterDepartmentId.Value > 0)
                {
                    all = all.Where(o => o.GROUP_DEPARTMENT_ID == rdo.FilterDepartmentId.Value).ToList();
                }

                // 4) Nguồn khác: tính TRƯỚC khi gom (gom ghi đè OTHER_SOURCE_PRICE của phần tử đầu nhóm) 
                ProcessOtherSource(all.Where(IsDisplayable).ToList());

                // 5) Gom dòng trùng: CHỈ phần viện phí (non-BHYT) — giống Mps000306.
                //    (Bỏ lượt bhytCoPayment:true để không lấy dịch vụ của thẻ BHYT phần quỹ không chi trả.)
                this.sereServADOs = new List<SereServADO>();
                this.sereServADOs.AddRange(MergeDuplicate(all, bhytCoPayment: false));

                // 6) Sắp xếp hiển thị
                this.sereServADOs = this.sereServADOs
                    .OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999)
                    .ThenBy(o => o.HEIN_SERVICE_TYPE_CHILD_NUM_ORDER ?? 99999)
                    .ThenBy(o => o.SERVICE_NAME)
                    .ToList();

                // [DIAG] TODO XÓA SAU KHI FIX: tổng hợp bao nhiêu dòng có phòng
                Inventec.Common.Logging.LogSystem.Warn(string.Format(
                    "[Mps000510][DIAG] SUMMARY total={0} withRoom={1} withoutRoom={2}",
                    this.sereServADOs.Count,
                    this.sereServADOs.Count(o => o.GROUP_ROOM_ID > 0),
                    this.sereServADOs.Count(o => o.GROUP_ROOM_ID <= 0)));

                // 7) Gom master theo loại hình DV / dòng thuốc / giường (có mutate dòng giường trên Service)
                GroupDisplayProcess();

                // 8) Dựng master gom theo khoa / phòng (Cách B)
                BuildDepartmentRoomGroups(deptById);

                // 9) Dựng bộ key PatyAlterBHYT (port từ Mps000306) cho các tag tổng đầu/cuối trang
                PatyAlterProcess();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private static Dictionary<long, T> BuildDict<T>(IEnumerable<T> source, Func<T, long> keySelector)
        {
            Dictionary<long, T> dic = new Dictionary<long, T>();
            if (source != null)
            {
                foreach (T item in source)
                {
                    long key = keySelector(item);
                    if (!dic.ContainsKey(key))
                        dic[key] = item;
                }
            }
            return dic;
        }

        private bool IsDisplayable(SereServADO o)
        {
            return o.AMOUNT > 0
                && o.PRICE > 0
                && o.IS_EXPEND != 1
                && o.IS_NO_EXECUTE != 1
                && ((o.PATIENT_TYPE_ID == rdo.PatientTypeCFG.PATIENT_TYPE__BHYT && (o.VIR_TOTAL_HEIN_PRICE ?? 0) <= 0)
                    || o.PATIENT_TYPE_ID != rdo.PatientTypeCFG.PATIENT_TYPE__BHYT);
        }

        /// <summary>
        /// Gom các dòng dịch vụ giống nhau. Khoá gom thêm chiều khoa/phòng khi GroupType bật,
        /// để dịch vụ ở khoa/phòng khác nhau không bị nhập làm một.
        /// </summary>
        private List<SereServADO> MergeDuplicate(List<SereServADO> source, bool bhytCoPayment)
        {
            List<SereServADO> result = new List<SereServADO>();
            try
            {
                IEnumerable<SereServADO> rows = source.Where(o =>
                    o.AMOUNT > 0 && o.PRICE > 0 && o.IS_EXPEND != 1 && o.IS_NO_EXECUTE != 1);

                if (bhytCoPayment)
                    rows = rows.Where(o => o.PATIENT_TYPE_ID == rdo.PatientTypeCFG.PATIENT_TYPE__BHYT && (o.VIR_TOTAL_HEIN_PRICE ?? 0) <= 0);
                else
                    rows = rows.Where(o => o.PATIENT_TYPE_ID != rdo.PatientTypeCFG.PATIENT_TYPE__BHYT);

                Func<SereServADO, object> keySelector;
                switch (rdo.GroupType)
                {
                    case GroupServiceType.Department:
                        keySelector = o => new { o.SERVICE_ID, o.PRICE, o.GROUP_DEPARTMENT_ID };
                        break;
                    case GroupServiceType.Room:
                        keySelector = o => new { o.SERVICE_ID, o.PRICE, o.GROUP_DEPARTMENT_ID, o.GROUP_ROOM_ID };
                        break;
                    default:
                        keySelector = o => new { o.SERVICE_ID, o.PRICE };
                        break;
                }

                foreach (var g in rows.GroupBy(keySelector))
                {
                    SereServADO s = g.First();
                    s.AMOUNT = g.Sum(o => o.AMOUNT);
                    s.VIR_TOTAL_PRICE_NO_EXPEND = g.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND);
                    s.OTHER_SOURCE_PRICE = g.Sum(o => o.OTHER_SOURCE_PRICE);
                    s.TOTAL_PRICE_PATIENT_SELF = (s.VIR_TOTAL_PRICE_NO_EXPEND ?? 0) - (s.OTHER_SOURCE_PRICE ?? 0);
                    s.TOTAL_PATIENT_PRICE_LEFT = g.Sum(o => o.TOTAL_PATIENT_PRICE_LEFT);
                    s.TOTAL_PRICE_VP = g.Sum(o => o.TOTAL_PRICE_VP);
                    result.Add(s);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void ProcessOtherSource(List<SereServADO> displayServices)
        {
            try
            {
                this.ListOtherSource = new List<OtherSourceADO>();
                if (displayServices == null || displayServices.Count == 0
                    || rdo.ListOtherPaySource == null || rdo.ListOtherPaySource.Count == 0)
                    return;

                Dictionary<long, HIS_OTHER_PAY_SOURCE> sourceById = BuildDict(rdo.ListOtherPaySource, o => o.ID);
                foreach (var g in displayServices.GroupBy(o => o.OTHER_PAY_SOURCE_ID))
                {
                    if (!g.Key.HasValue)
                        continue;

                    HIS_OTHER_PAY_SOURCE source;
                    if (!sourceById.TryGetValue(g.Key.Value, out source) || source == null)
                        continue;

                    OtherSourceADO ado = new OtherSourceADO();
                    ado.OTHER_PAY_SOURCE_CODE = source.OTHER_PAY_SOURCE_CODE;
                    ado.OTHER_PAY_SOURCE_NAME = source.OTHER_PAY_SOURCE_NAME;
                    ado.TOTAL_PRICE = g.Sum(s => s.OTHER_SOURCE_PRICE ?? 0);
                    ado.TOTAL_PRICE_STR = Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(ado.TOTAL_PRICE).ToString());
                    this.ListOtherSource.Add(ado);
                }
            }
            catch (Exception ex)
            {
                this.ListOtherSource = new List<OtherSourceADO>();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
