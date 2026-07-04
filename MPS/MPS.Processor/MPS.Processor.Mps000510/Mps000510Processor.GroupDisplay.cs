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
                heinServiceTypeADOs = HeinServiceTypeProcess(groupByRoom: true, keepDepa: true);
                heinServiceTypeADOs_ByDepa = HeinServiceTypeProcess(groupByRoom: false, keepDepa: true);
                heinServiceTypeADOs_LoaiDV = HeinServiceTypeProcess(groupByRoom: false, keepDepa: false);

                // Dồn các loại "giường" về 1 nhóm cha "Giường" trên chính dòng Service
                sereServADOs.ForEach(o =>
                {
                    if (o.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__GI_NGT
                        || o.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__GI_NT
                        || o.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__GI_BN
                        || o.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__GI_L)
                    {
                        o.HEIN_SERVICE_TYPE_PARENT_1_ID = o.HEIN_SERVICE_TYPE_ID;
                        o.HEIN_SERVICE_TYPE_ID = HeinServiceTypeExt.BED__ID;
                    }
                });

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

        /// <param name="groupByRoom">true: có chiều PHÒNG trong key (GROUP_ROOM_ID). false: bỏ phòng.</param>
        /// <param name="keepDepa">true: có chiều KHOA trong key (GROUP_DEPARTMENT_ID). false: bỏ khoa -> gom THUẦN theo loại DV (mẫu không có band khoa/phòng), tránh nhân đôi.</param>
        private List<HeinServiceTypeADO> HeinServiceTypeProcess(bool groupByRoom, bool keepDepa)
        {
            List<HeinServiceTypeADO> result = new List<HeinServiceTypeADO>();
            try
            {
                var groups = sereServADOs
                    .OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999999)
                    .GroupBy(o => new { o.HEIN_SERVICE_TYPE_ID, GROUP_DEPARTMENT_ID = (keepDepa ? o.GROUP_DEPARTMENT_ID : 0L), GROUP_ROOM_ID = (groupByRoom ? o.GROUP_ROOM_ID : 0L) }).ToList();

                foreach (var g in groups)
                {
                    SereServADO first = g.First();
                    HeinServiceTypeADO h = new HeinServiceTypeADO();
                    // Khóa gom theo khoa / phòng để nối với ServiceGroupByDepa / ServiceGroupByRoom.
                    // Ép 0 khi bỏ chiều tương ứng để cột ADO khớp key (logic gộp giường bên dưới so theo 2 cột này).
                    h.GROUP_DEPARTMENT_ID = keepDepa ? first.GROUP_DEPARTMENT_ID : 0L;
                    h.GROUP_ROOM_ID = groupByRoom ? first.GROUP_ROOM_ID : 0L;
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
                            && (!groupByRoom || o.GROUP_ROOM_ID == h.GROUP_ROOM_ID));
                        if (existingBed != null)
                        {
                            // Gom theo KHOA (groupByRoom=false): 1 khoa có nhiều loại giường ở các phòng khác nhau -> cộng dồn tổng vào 1 dòng "Giường"
                            // để không mất tiền (khác nhánh theo phòng vốn chỉ giữ dòng đầu, giữ nguyên hành vi cũ để không hồi quy).
                            if (!groupByRoom)
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

        /// <param name="keepDepaRoom">true: giữ chiều khoa + phòng trong key (mẫu gom khoa/phòng). false: bỏ -> gom thuần theo dòng thuốc + loại DV, tránh nhân đôi ở mẫu không band khoa/phòng.</param>
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
                        List<HIS_SERVICE_REQ> serviceReqTemps = rdo.ServiceReqs.Where(o => serviceReqIds.Contains(o.ID) && o.REMEDY_COUNT.HasValue).ToList();
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
                var groups = sereServADOsByDepa
                    .OrderBy(o => o.MEDICINE_LINE_ID)
                    .GroupBy(o => new { o.MEDICINE_LINE_ID, o.HEIN_SERVICE_TYPE_ID,o.GROUP_DEPARTMENT_ID, o.GROUP_ROOM_ID}).ToList();

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
                        List<HIS_SERVICE_REQ> serviceReqTemps = rdo.ServiceReqs.Where(o => serviceReqIds.Contains(o.ID) && o.REMEDY_COUNT.HasValue).ToList();
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
                var groups = sereServADOsLoaiDV
                    .OrderBy(o => o.MEDICINE_LINE_ID)
                    .GroupBy(o => new { o.MEDICINE_LINE_ID, o.HEIN_SERVICE_TYPE_ID, o.GROUP_DEPARTMENT_ID, o.GROUP_ROOM_ID }).ToList();

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
                        List<HIS_SERVICE_REQ> serviceReqTemps = rdo.ServiceReqs.Where(o => serviceReqIds.Contains(o.ID) && o.REMEDY_COUNT.HasValue).ToList();
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

        /// <param name="keepDepaRoom">true: giữ chiều khoa + phòng trong key. false: bỏ -> gom thuần theo loại giường, tránh nhân đôi ở mẫu không band khoa/phòng.</param>
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
