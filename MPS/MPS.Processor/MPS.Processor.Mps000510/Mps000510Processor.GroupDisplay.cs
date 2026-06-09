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
        private List<MedicineLineADO> medicineLineADOs { get; set; }
        private List<HeinServiceTypeADO> HeinServiceTypeBeds { get; set; }

        /// <summary>
        /// Dựng 3 bộ master gom theo loại hình DV / dòng thuốc / giường (port từ Mps000281,
        /// bỏ chiều KEY_PATY_ALTER vì 510 không tách bảng theo thẻ BHYT).
        /// Thứ tự bắt buộc: gom loại hình DV -> dồn giường về nhóm "Giường" -> dòng thuốc -> giường chi tiết.
        /// </summary>
        internal void GroupDisplayProcess()
        {
            try
            {
                HeinServiceTypeProcess();

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

                MedicineLineProcess();
                HeinServiceTypeBedProcess();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void HeinServiceTypeProcess()
        {
            heinServiceTypeADOs = new List<HeinServiceTypeADO>();
            try
            {
                var groups = sereServADOs
                    .OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999999)
                    .GroupBy(o => o.HEIN_SERVICE_TYPE_ID).ToList();

                foreach (var g in groups)
                {
                    SereServADO first = g.First();
                    HeinServiceTypeADO h = new HeinServiceTypeADO();
                    h.TOTAL_PRICE_HEIN_SERVICE_TYPE = g.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND ?? 0);
                    h.TOTAL_PRICE_BHYT_HEIN_SERVICE_TYPE = g.Sum(o => o.TOTAL_PRICE_BHYT);
                    h.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE = g.Sum(o => o.VIR_TOTAL_HEIN_PRICE ?? 0);
                    h.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE = g.Sum(o => o.VIR_TOTAL_PATIENT_PRICE_BHYT ?? 0);
                    h.TOTAL_PATIENT_PRICE_SELF_HEIN_SERVICE_TYPE = g.Sum(o => o.TOTAL_PRICE_PATIENT_SELF);
                    h.OTHER_SOURCE_PRICE = g.Sum(o => o.OTHER_SOURCE_PRICE ?? 0);
                    h.TOTAL_PATIENT_PRICE_LEFT = g.Sum(o => o.TOTAL_PATIENT_PRICE_LEFT);
                    h.TOTAL_PRICE_VP = g.Sum(o => o.TOTAL_PRICE_VP);

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

                    // Gộp mọi loại giường vào 1 dòng "Giường" duy nhất
                    if (first.HEIN_SERVICE_TYPE_ID.HasValue
                        && (first.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__GI_NGT
                            || first.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__GI_NT
                            || first.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__GI_BN
                            || first.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__GI_L))
                    {
                        if (heinServiceTypeADOs.Any(o => o.ID == HeinServiceTypeExt.BED__ID))
                            continue;
                        h.ID = HeinServiceTypeExt.BED__ID;
                        h.HEIN_SERVICE_TYPE_NAME = HeinServiceTypeExt.BED__NAME;
                        h.NUM_ORDER = first.HEIN_SERVICE_TYPE_NUM_ORDER;
                    }

                    heinServiceTypeADOs.Add(h);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void MedicineLineProcess()
        {
            medicineLineADOs = new List<MedicineLineADO>();
            try
            {
                var groups = sereServADOs
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
                    if ((first.MEDICINE_LINE_ID ?? 0) <= 0 && (first.HEIN_SERVICE_TYPE_ID ?? 0) > 0)
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

                    medicineLineADOs.Add(ado);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void HeinServiceTypeBedProcess()
        {
            HeinServiceTypeBeds = new List<HeinServiceTypeADO>();
            try
            {
                var groups = sereServADOs
                    .OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999999)
                    .GroupBy(o => new { o.HEIN_SERVICE_TYPE_ID, o.MEDICINE_LINE_ID, o.HEIN_SERVICE_TYPE_PARENT_1_ID }).ToList();

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

                    HeinServiceTypeBeds.Add(h);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
