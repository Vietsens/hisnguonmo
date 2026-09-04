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
        private List<HeinServiceTypeADO> heinServiceTypeADOs { get; set; }
        // Bộ loại dịch vụ gom theo KHOA (KHÔNG tách phòng) - template gom theo khoa bind HeinServiceTypeByDepa, tránh nhân đôi.
        private List<HeinServiceTypeADO> heinServiceTypeADOs_ByDepa { get; set; }
        // Bộ gom THUẦN theo loại dịch vụ (bỏ cả khoa lẫn phòng) - mẫu KHÔNG có band khoa/phòng bind các tên *LoaiDV, tránh nhân đôi.
        private List<HeinServiceTypeADO> heinServiceTypeADOs_LoaiDV { get; set; }
        private List<MedicineLineADO> medicineLineADOs { get; set; }
        private List<MedicineLineADO> medicineLineADOs_Depa { get; set; }
        private List<MedicineLineADO> medicineLineADOs_LoaiDV { get; set; }
        private List<HeinServiceTypeADO> HeinServiceTypeBeds { get; set; }
        private List<HeinServiceTypeADO> HeinServiceTypeBeds_Depa { get; set; }
        private List<HeinServiceTypeADO> HeinServiceTypeBeds_LoaiDV { get; set; }

        /// <summary>
        /// Dựng 3 bộ master gom theo loại hình DV / dòng thuốc / giường (port từ Mps000281,
        /// bỏ chiều KEY_PATY_ALTER vì 510 không tách bảng theo thẻ BHYT).
        /// Thứ tự bắt buộc: gom loại hình DV -> dồn giường về nhóm "Giường" -> dòng thuốc -> giường chi tiết.
        /// </summary>
        internal void GroupDisplayProcess()
        {
            try
            {
                // Dựng SẴN 3 grain của bộ loại dịch vụ; template bind tên tương ứng (không cần biết mẫu là gì): 
                //  - HeinServiceType        = {loại DV + khoa + phòng} -> mẫu gom khoa/phòng (TongHop).
                //  - HeinServiceTypeByDepa  = {loại DV + khoa}         -> mẫu gom theo khoa.
                //  - HeinServiceTypeLoaiDV  = {loại DV}                -> mẫu gom THUẦN theo loại DV (không band khoa/phòng), tránh nhân đôi.
                // Cả 3 gọi TRƯỚC bước gộp giường để cùng input.
                heinServiceTypeADOs = HeinServiceTypeProcess(1);
                heinServiceTypeADOs_ByDepa = HeinServiceTypeProcess(2);
                heinServiceTypeADOs_LoaiDV = HeinServiceTypeProcess(3);

                // Dồn các loại "giường" về 1 nhóm cha "Giường" trên chính dòng Service.
                // Áp cho CẢ 3 list: sau khi 3 bộ merge đã clone (object độc lập), việc mutate
                // KHÔNG còn tự lan giữa các bộ nữa, nên phải gộp giường riêng cho từng list —
                // nếu không dòng con "Giường" ở bộ 2 (theo khoa) / bộ 3 (loại DV) sẽ đứt quan hệ.
                Action<SereServADO> lumpBed = o =>
                {
                    if (o.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__GI_NGT
                        || o.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__GI_NT
                        || o.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__GI_BN
                        || o.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__GI_L)
                    {
                        o.HEIN_SERVICE_TYPE_PARENT_1_ID = o.HEIN_SERVICE_TYPE_ID;
                        o.HEIN_SERVICE_TYPE_ID = HeinServiceTypeExt.BED__ID;
                    }
                };
                sereServADOs.ForEach(lumpBed);
                sereServADOsByDepa.ForEach(lumpBed);
                sereServADOsLoaiDV.ForEach(lumpBed);

                // Dòng thuốc & giường: dựng cả bản có khoa/phòng (mẫu gom khoa/phòng) lẫn bản thuần loại DV (*LoaiDV).
                medicineLineADOs = MedicineLineProcess();
                medicineLineADOs_Depa = MedicineLineProcessByDepa();
                medicineLineADOs_LoaiDV = MedicineLineProcessDV();
                HeinServiceTypeBeds = HeinServiceTypeBedProcess();
                HeinServiceTypeBeds_Depa = HeinServiceTypeBedProcessDepa();
                HeinServiceTypeBeds_LoaiDV = HeinServiceTypeBedProcessDepaDV();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <param name="type">
        /// 1 = {loại DV + khoa + phòng} đọc sereServADOs      (bộ 1, mẫu TongHop).
        /// 2 = {loại DV + khoa}        đọc sereServADOsByDepa (bộ 2, mẫu gom theo khoa).
        /// 3 = {loại DV} thuần         đọc sereServADOsLoaiDV (bộ 3, không band khoa/phòng, tránh nhân đôi).
        /// </param>
        private List<HeinServiceTypeADO> HeinServiceTypeProcess(int type)
        {
            List<HeinServiceTypeADO> result = new List<HeinServiceTypeADO>();
            try
            {
                var groups = Enumerable.Empty<IGrouping<object, SereServADO>>();
                if (type == 1)          // gom theo KHOA + PHÒNG
                {
                    groups = sereServADOs
                    .OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999999)
                    .GroupBy(o => new { o.HEIN_SERVICE_TYPE_ID, o.GROUP_DEPARTMENT_ID, o.GROUP_ROOM_ID }).ToList();
                }

                else if (type == 2)     // gom theo KHOA
                {
                    groups = sereServADOsByDepa
                    .OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999999)
                    .GroupBy(o => new { o.HEIN_SERVICE_TYPE_ID, o.GROUP_DEPARTMENT_ID }).ToList();
                }
                else if (type == 3)     // gom theo LOẠI DỊCH VỤ
                {
                    groups = sereServADOsLoaiDV
                    .OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999999)
                    .GroupBy(o => new { o.HEIN_SERVICE_TYPE_ID }).ToList();
                }

                foreach (var g in groups)
                {
                    SereServADO first = g.First();
                    HeinServiceTypeADO h = new HeinServiceTypeADO();
                    // Chỉ mang theo chiều mà type đó gom, chiều bỏ = 0 (khớp key nối ServiceGroupByDepa/ByRoom
                    // và để gộp giường so đúng): type 3 bỏ khoa; chỉ type 1 giữ phòng.
                    h.GROUP_DEPARTMENT_ID = (type == 3) ? 0L : first.GROUP_DEPARTMENT_ID;
                    h.GROUP_ROOM_ID = (type == 1) ? first.GROUP_ROOM_ID : 0L;
                    h.HEIN_SERVICE_TYPE_CHILD_NUM_ORDER = first.HEIN_SERVICE_TYPE_CHILD_NUM_ORDER;
                    h.TOTAL_PRICE_HEIN_SERVICE_TYPE = g.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND ?? 0);
                    h.TOTAL_PRICE_BHYT_HEIN_SERVICE_TYPE = g.Sum(o => o.TOTAL_PRICE_BHYT);
                    h.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE = g.Sum(o => o.VIR_TOTAL_HEIN_PRICE ?? 0);
                    h.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE = g.Sum(o => o.VIR_TOTAL_PATIENT_PRICE_BHYT ?? 0);
                    h.TOTAL_PATIENT_PRICE_SELF_HEIN_SERVICE_TYPE = g.Sum(o => o.TOTAL_PRICE_PATIENT_SELF);
                    h.OTHER_SOURCE_PRICE = g.Sum(o => o.OTHER_SOURCE_PRICE ?? 0);
                    h.TOTAL_PATIENT_PRICE_LEFT = g.Sum(o => o.TOTAL_PATIENT_PRICE_LEFT);
                    h.TOTAL_PRICE_VP = g.Sum(o => o.TOTAL_PRICE_VP);
                    h.TOTAL_HEIN_PRICE = h.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE;

                    if (first.HEIN_SERVICE_TYPE_ID.HasValue)
                    {
                        h.ID = first.HEIN_SERVICE_TYPE_ID.Value;
                        h.HEIN_SERVICE_TYPE_NAME = first.HEIN_SERVICE_TYPE_NAME;
                        h.HEIN_SERVICE_TYPE_NAME_697 = first.HEIN_SERVICE_TYPE_NAME_697;
                        h.NUM_ORDER = first.HEIN_SERVICE_TYPE_NUM_ORDER;
                    }
                    else
                    {
                        h.HEIN_SERVICE_TYPE_NAME = "Khác";
                    }

                    // Gộp mọi loại giường vào 1 dòng "Giường" duy nhất TRONG TỪNG khoa/phòng
                    if (first.HEIN_SERVICE_TYPE_ID.HasValue
                        && (first.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__GI_NGT
                            || first.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__GI_NT
                            || first.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__GI_BN
                            || first.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__GI_L))
                    {
                        HeinServiceTypeADO existingBed = result.FirstOrDefault(o => o.ID == HeinServiceTypeExt.BED__ID
                            && o.GROUP_DEPARTMENT_ID == h.GROUP_DEPARTMENT_ID
                            && (type != 1 || o.GROUP_ROOM_ID == h.GROUP_ROOM_ID));
                        if (existingBed != null)
                        {
                            // type 2 (theo khoa) & type 3 (loại DV): 1 khoa có nhiều loại giường ở các phòng khác nhau
                            // -> cộng dồn tổng vào 1 dòng "Giường" để không mất tiền.
                            // type 1 (theo phòng): giữ nguyên hành vi cũ - chỉ giữ dòng đầu, không cộng dồn (tránh hồi quy).
                            if (type != 1)
                            {
                                existingBed.TOTAL_PRICE_HEIN_SERVICE_TYPE += h.TOTAL_PRICE_HEIN_SERVICE_TYPE;
                                existingBed.TOTAL_PRICE_BHYT_HEIN_SERVICE_TYPE += h.TOTAL_PRICE_BHYT_HEIN_SERVICE_TYPE;
                                existingBed.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE += h.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE;
                                existingBed.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE += h.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE;
                                existingBed.TOTAL_PATIENT_PRICE_SELF_HEIN_SERVICE_TYPE += h.TOTAL_PATIENT_PRICE_SELF_HEIN_SERVICE_TYPE;
                                existingBed.OTHER_SOURCE_PRICE += h.OTHER_SOURCE_PRICE;
                                existingBed.TOTAL_PATIENT_PRICE_LEFT += h.TOTAL_PATIENT_PRICE_LEFT;
                                existingBed.TOTAL_PRICE_VP += h.TOTAL_PRICE_VP;
                                existingBed.TOTAL_HEIN_PRICE += h.TOTAL_HEIN_PRICE;
                            }
                            continue;
                        }
                        h.ID = HeinServiceTypeExt.BED__ID;
                        h.HEIN_SERVICE_TYPE_NAME = HeinServiceTypeExt.BED__NAME;
                        h.NUM_ORDER = first.HEIN_SERVICE_TYPE_NUM_ORDER;
                    }

                    result.Add(h);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private List<MedicineLineADO> MedicineLineProcess()
        {
            List<MedicineLineADO> result = new List<MedicineLineADO>();
            try
            {
                var groups = sereServADOs
                    .OrderBy(o => o.MEDICINE_LINE_ID)
                    .GroupBy(o => new { o.MEDICINE_LINE_ID, o.HEIN_SERVICE_TYPE_ID,o.GROUP_DEPARTMENT_ID,o.GROUP_ROOM_ID }).ToList();

                foreach (var g in groups)
                {
                    SereServADO first = g.First();
                    MedicineLineADO ado = new MedicineLineADO();
                    ado.ID = first.MEDICINE_LINE_ID;
                    ado.HEIN_SERVICE_TYPE_ID = first.HEIN_SERVICE_TYPE_ID;
                    ado.GROUP_DEPARTMENT_ID = first.GROUP_DEPARTMENT_ID;
                    ado.GROUP_ROOM_ID = first.GROUP_ROOM_ID;
                    ado.MEDICINE_LINE_CODE = first.MEDICINE_LINE_CODE;
                    ado.MEDICINE_LINE_NAME = first.MEDICINE_LINE_NAME;
                    // Khớp 281: null MEDICINE_LINE_ID -> không vào nhánh "Chưa xác định"
                    // (so sánh nullable: null <= 0 trả về false). Chỉ kích hoạt khi LÀ 0 thật sự.
                    if (first.MEDICINE_LINE_ID <= 0 && first.HEIN_SERVICE_TYPE_ID > 0)
                    {
                        ado.MEDICINE_LINE_CODE = "Chưa xác định";
                        ado.MEDICINE_LINE_NAME = "Chưa xác định";
                    }

                    // Số tháng cấp thuốc
                    if (rdo.ServiceReqs != null && rdo.ServiceReqs.Count > 0)
                    {
                        List<long> serviceReqIds = g.Select(o => o.SERVICE_REQ_ID ?? 0).ToList();
                        List<long> donTypeIds = new List<long> { IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT, IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK, IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONM, IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT };
                        List<HIS_SERVICE_REQ> serviceReqTemps = rdo.ServiceReqs.Where(o => serviceReqIds.Contains(o.ID) && o.REMEDY_COUNT.HasValue && donTypeIds.Contains(o.SERVICE_REQ_TYPE_ID)).ToList();
                        if (serviceReqTemps.Count > 0)
                            ado.REMEDY_COUNT = serviceReqTemps.Sum(o => o.REMEDY_COUNT ?? 0);
                    }

                    result.Add(ado);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private List<MedicineLineADO> MedicineLineProcessByDepa()
        {
            List<MedicineLineADO> result = new List<MedicineLineADO>();
            try
            {
                // Bộ 2 gom theo KHOA: KHÔNG đưa GROUP_ROOM_ID vào key. Nguồn sereServADOsByDepa giữ
                // room của dòng first (clone) nên 2 dịch vụ cùng MEDICINE_LINE_ID khác phòng trong cùng
                // khoa sẽ sinh 2 dòng MedicineLineDepa TRÙNG ID -> quan hệ khóa đơn ID=MEDICINE_LINE_ID
                // nhân đôi Service. (Bộ 2 không có quan hệ nào theo phòng.)
                var groups = sereServADOsByDepa
                    .OrderBy(o => o.MEDICINE_LINE_ID)
                    .GroupBy(o => new { o.MEDICINE_LINE_ID, o.HEIN_SERVICE_TYPE_ID, o.GROUP_DEPARTMENT_ID }).ToList();

                foreach (var g in groups)
                {
                    SereServADO first = g.First();
                    MedicineLineADO ado = new MedicineLineADO();
                    ado.ID = first.MEDICINE_LINE_ID;
                    ado.HEIN_SERVICE_TYPE_ID = first.HEIN_SERVICE_TYPE_ID;
                    ado.GROUP_DEPARTMENT_ID = first.GROUP_DEPARTMENT_ID;
                    ado.MEDICINE_LINE_CODE = first.MEDICINE_LINE_CODE;
                    ado.MEDICINE_LINE_NAME = first.MEDICINE_LINE_NAME;
                    // Khớp 281: null MEDICINE_LINE_ID -> không vào nhánh "Chưa xác định"
                    // (so sánh nullable: null <= 0 trả về false). Chỉ kích hoạt khi LÀ 0 thật sự.
                    if (first.MEDICINE_LINE_ID <= 0 && first.HEIN_SERVICE_TYPE_ID > 0)
                    {
                        ado.MEDICINE_LINE_CODE = "Chưa xác định";
                        ado.MEDICINE_LINE_NAME = "Chưa xác định";
                    }

                    // Số tháng cấp thuốc
                    if (rdo.ServiceReqs != null && rdo.ServiceReqs.Count > 0)
                    {
                        List<long> serviceReqIds = g.Select(o => o.SERVICE_REQ_ID ?? 0).ToList();
                        List<long> donTypeIds = new List<long> { IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT, IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK, IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONM, IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT };
                        List<HIS_SERVICE_REQ> serviceReqTemps = rdo.ServiceReqs.Where(o => serviceReqIds.Contains(o.ID) && o.REMEDY_COUNT.HasValue && donTypeIds.Contains(o.SERVICE_REQ_TYPE_ID)).ToList();
                        if (serviceReqTemps.Count > 0)
                            ado.REMEDY_COUNT = serviceReqTemps.Sum(o => o.REMEDY_COUNT ?? 0);
                    }

                    result.Add(ado);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private List<MedicineLineADO> MedicineLineProcessDV()
        {
            List<MedicineLineADO> result = new List<MedicineLineADO>();
            try
            {
                // Bộ 3 gom THUẦN theo loại DV: KHÔNG đưa khoa/phòng vào key (giống bed builder LoaiDV
                // dòng 415), tránh sinh nhiều dòng MedicineLineLoaiDV trùng MEDICINE_LINE_ID -> nhân đôi Service.
                var groups = sereServADOsLoaiDV
                    .OrderBy(o => o.MEDICINE_LINE_ID)
                    .GroupBy(o => new { o.MEDICINE_LINE_ID, o.HEIN_SERVICE_TYPE_ID }).ToList();

                foreach (var g in groups)
                {
                    SereServADO first = g.First();
                    MedicineLineADO ado = new MedicineLineADO();
                    ado.ID = first.MEDICINE_LINE_ID;
                    ado.HEIN_SERVICE_TYPE_ID = first.HEIN_SERVICE_TYPE_ID;
                    ado.MEDICINE_LINE_CODE = first.MEDICINE_LINE_CODE;
                    ado.MEDICINE_LINE_NAME = first.MEDICINE_LINE_NAME;
                    // Khớp 281: null MEDICINE_LINE_ID -> không vào nhánh "Chưa xác định" 
                    // (so sánh nullable: null <= 0 trả về false). Chỉ kích hoạt khi LÀ 0 thật sự.
                    if (first.MEDICINE_LINE_ID <= 0 && first.HEIN_SERVICE_TYPE_ID > 0)
                    {
                        ado.MEDICINE_LINE_CODE = "Chưa xác định";
                        ado.MEDICINE_LINE_NAME = "Chưa xác định";
                    }

                    // Số tháng cấp thuốc
                    if (rdo.ServiceReqs != null && rdo.ServiceReqs.Count > 0)
                    {
                        List<long> serviceReqIds = g.Select(o => o.SERVICE_REQ_ID ?? 0).ToList();
                        List<long> donTypeIds = new List<long> { IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT, IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK, IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONM, IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT };
                        List<HIS_SERVICE_REQ> serviceReqTemps = rdo.ServiceReqs.Where(o => serviceReqIds.Contains(o.ID) && o.REMEDY_COUNT.HasValue && donTypeIds.Contains(o.SERVICE_REQ_TYPE_ID)).ToList();
                        if (serviceReqTemps.Count > 0)
                            ado.REMEDY_COUNT = serviceReqTemps.Sum(o => o.REMEDY_COUNT ?? 0);
                    }

                    result.Add(ado);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private List<HeinServiceTypeADO> HeinServiceTypeBedProcess()
        {
            List<HeinServiceTypeADO> result = new List<HeinServiceTypeADO>();
            try
            {
                var groups = sereServADOs
                    .OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999999)
                    .GroupBy(o => new { o.HEIN_SERVICE_TYPE_ID, o.MEDICINE_LINE_ID, o.HEIN_SERVICE_TYPE_PARENT_1_ID, o.GROUP_DEPARTMENT_ID, o.GROUP_ROOM_ID}).ToList();

                foreach (var g in groups)
                {
                    SereServADO first = g.First();
                    HeinServiceTypeADO h = new HeinServiceTypeADO();
                    h.PARENT_ID = first.HEIN_SERVICE_TYPE_ID;
                    h.ID = first.HEIN_SERVICE_TYPE_PARENT_1_ID;
                    h.MEDICINE_LINE_ID = first.MEDICINE_LINE_ID;
                    h.GROUP_DEPARTMENT_ID = first.GROUP_DEPARTMENT_ID;
                    h.GROUP_ROOM_ID = first.GROUP_ROOM_ID;

                    if (h.PARENT_ID.HasValue && h.PARENT_ID == HeinServiceTypeExt.BED__ID)
                    {
                        h.HEIN_SERVICE_TYPE_NAME = first.HEIN_SERVICE_TYPE_NAME;
                        h.HEIN_SERVICE_TYPE_NAME_697 = first.HEIN_SERVICE_TYPE_NAME_697;
                        h.NUM_ORDER = first.HEIN_SERVICE_TYPE_CHILD_NUM_ORDER;
                        h.TOTAL_PRICE_HEIN_SERVICE_TYPE = g.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND ?? 0);
                        h.TOTAL_PRICE_BHYT_HEIN_SERVICE_TYPE = g.Sum(o => o.TOTAL_PRICE_BHYT);
                        h.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE = g.Sum(o => o.VIR_TOTAL_HEIN_PRICE ?? 0);
                        h.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE = g.Sum(o => o.VIR_TOTAL_PATIENT_PRICE_BHYT ?? 0);
                        h.TOTAL_PATIENT_PRICE_SELF_HEIN_SERVICE_TYPE = g.Sum(o => o.TOTAL_PRICE_PATIENT_SELF);
                        h.OTHER_SOURCE_PRICE = g.Sum(o => o.OTHER_SOURCE_PRICE ?? 0);
                        h.TOTAL_PATIENT_PRICE_LEFT = g.Sum(o => o.TOTAL_PATIENT_PRICE_LEFT);
                        h.TOTAL_PRICE_VP = g.Sum(o => o.TOTAL_PRICE_VP);
                    }

                    result.Add(h);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private List<HeinServiceTypeADO> HeinServiceTypeBedProcessDepa()
        {
            List<HeinServiceTypeADO> result = new List<HeinServiceTypeADO>();
            try
            {
                var groups = sereServADOsByDepa
                    .OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999999)
                    .GroupBy(o => new { o.HEIN_SERVICE_TYPE_ID, o.MEDICINE_LINE_ID, o.HEIN_SERVICE_TYPE_PARENT_1_ID, o.GROUP_DEPARTMENT_ID}).ToList();

                foreach (var g in groups)
                {
                    SereServADO first = g.First();
                    HeinServiceTypeADO h = new HeinServiceTypeADO();
                    h.PARENT_ID = first.HEIN_SERVICE_TYPE_ID;
                    h.ID = first.HEIN_SERVICE_TYPE_PARENT_1_ID;
                    h.MEDICINE_LINE_ID = first.MEDICINE_LINE_ID;
                    h.GROUP_DEPARTMENT_ID = first.GROUP_DEPARTMENT_ID;

                    if (h.PARENT_ID.HasValue && h.PARENT_ID == HeinServiceTypeExt.BED__ID)
                    {
                        h.HEIN_SERVICE_TYPE_NAME = first.HEIN_SERVICE_TYPE_NAME;
                        h.HEIN_SERVICE_TYPE_NAME_697 = first.HEIN_SERVICE_TYPE_NAME_697;
                        h.NUM_ORDER = first.HEIN_SERVICE_TYPE_CHILD_NUM_ORDER;
                        h.TOTAL_PRICE_HEIN_SERVICE_TYPE = g.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND ?? 0);
                        h.TOTAL_PRICE_BHYT_HEIN_SERVICE_TYPE = g.Sum(o => o.TOTAL_PRICE_BHYT);
                        h.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE = g.Sum(o => o.VIR_TOTAL_HEIN_PRICE ?? 0);
                        h.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE = g.Sum(o => o.VIR_TOTAL_PATIENT_PRICE_BHYT ?? 0);
                        h.TOTAL_PATIENT_PRICE_SELF_HEIN_SERVICE_TYPE = g.Sum(o => o.TOTAL_PRICE_PATIENT_SELF);
                        h.OTHER_SOURCE_PRICE = g.Sum(o => o.OTHER_SOURCE_PRICE ?? 0);
                        h.TOTAL_PATIENT_PRICE_LEFT = g.Sum(o => o.TOTAL_PATIENT_PRICE_LEFT);
                        h.TOTAL_PRICE_VP = g.Sum(o => o.TOTAL_PRICE_VP);
                    }

                    result.Add(h);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private List<HeinServiceTypeADO> HeinServiceTypeBedProcessDepaDV()
        {
            List<HeinServiceTypeADO> result = new List<HeinServiceTypeADO>();
            try
            {
                var groups = sereServADOsLoaiDV
                    .OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999999)
                    .GroupBy(o => new { o.HEIN_SERVICE_TYPE_ID, o.MEDICINE_LINE_ID, o.HEIN_SERVICE_TYPE_PARENT_1_ID}).ToList();

                foreach (var g in groups)
                {
                    SereServADO first = g.First();
                    HeinServiceTypeADO h = new HeinServiceTypeADO();
                    h.PARENT_ID = first.HEIN_SERVICE_TYPE_ID;
                    h.ID = first.HEIN_SERVICE_TYPE_PARENT_1_ID;
                    h.MEDICINE_LINE_ID = first.MEDICINE_LINE_ID;

                    if (h.PARENT_ID.HasValue && h.PARENT_ID == HeinServiceTypeExt.BED__ID)
                    {
                        h.HEIN_SERVICE_TYPE_NAME = first.HEIN_SERVICE_TYPE_NAME;
                        h.HEIN_SERVICE_TYPE_NAME_697 = first.HEIN_SERVICE_TYPE_NAME_697;
                        h.NUM_ORDER = first.HEIN_SERVICE_TYPE_CHILD_NUM_ORDER;
                        h.TOTAL_PRICE_HEIN_SERVICE_TYPE = g.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND ?? 0);
                        h.TOTAL_PRICE_BHYT_HEIN_SERVICE_TYPE = g.Sum(o => o.TOTAL_PRICE_BHYT);
                        h.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE = g.Sum(o => o.VIR_TOTAL_HEIN_PRICE ?? 0);
                        h.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE = g.Sum(o => o.VIR_TOTAL_PATIENT_PRICE_BHYT ?? 0);
                        h.TOTAL_PATIENT_PRICE_SELF_HEIN_SERVICE_TYPE = g.Sum(o => o.TOTAL_PRICE_PATIENT_SELF);
                        h.OTHER_SOURCE_PRICE = g.Sum(o => o.OTHER_SOURCE_PRICE ?? 0);
                        h.TOTAL_PATIENT_PRICE_LEFT = g.Sum(o => o.TOTAL_PATIENT_PRICE_LEFT);
                        h.TOTAL_PRICE_VP = g.Sum(o => o.TOTAL_PRICE_VP);
                    }

                    result.Add(h);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
    }
}
