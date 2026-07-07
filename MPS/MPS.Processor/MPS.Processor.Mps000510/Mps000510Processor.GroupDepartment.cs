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
        private List<GroupDepartmentADO> ServiceGroupByDepa { get; set; }
        private List<GroupDepartmentADO> ServiceGroupByRoom { get; set; }

        /// <summary>
        /// Dựng 2 bộ master gom theo khoa xử lý và phòng xử lý từ bộ Service đã gom.
        /// - ServiceGroupByDepa: 1 dòng / khoa, sắp xếp khoa lâm sàng lên trước rồi theo tên.
        /// - ServiceGroupByRoom: 1 dòng / (khoa, phòng).
        /// </summary>
        private void BuildDepartmentRoomGroups(Dictionary<long, HIS_DEPARTMENT> deptById)
        {
            this.ServiceGroupByDepa = new List<GroupDepartmentADO>();
            this.ServiceGroupByRoom = new List<GroupDepartmentADO>();
            try
            {
                if (this.sereServADOs == null || this.sereServADOs.Count == 0)
                    return;

                // ===== Gom theo khoa =====
                foreach (var g in this.sereServADOs.GroupBy(o =>  o.GROUP_DEPARTMENT_ID))
                {
                    SereServADO first = g.First();
                    GroupDepartmentADO ado = NewGroupTotals(g);
                    ado.GROUP_DEPARTMENT_ID = g.Key;
                    ado.DEPARTMENT_CODE = first.GROUP_DEPARTMENT_CODE;
                    ado.DEPARTMENT_NAME = first.GROUP_DEPARTMENT_NAME;
                    // NUM_ORDER nhỏ nhất của loại dịch vụ trong khoa (null -> 99999 xuống cuối).
                    ado.MIN_NUM_ORDER = g.Min(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999);

                    HIS_DEPARTMENT dept;
                    if (g.Key > 0 && deptById != null && deptById.TryGetValue(g.Key, out dept) && dept != null)
                        ado.IS_CLINICAL = dept.IS_CLINICAL;

                    this.ServiceGroupByDepa.Add(ado);
                }
                // Sắp khoa theo loại dịch vụ: khoa chứa loại có NUM_ORDER nhỏ nhất (vd khám) lên trước; đồng hạng mới xét lâm sàng rồi tên khoa.
                this.ServiceGroupByDepa = this.ServiceGroupByDepa
                    .OrderBy(o => o.MIN_NUM_ORDER)
                    .ThenBy(o => o.IS_CLINICAL == 1 ? 0 : 1)   // đồng NUM_ORDER: khoa lâm sàng lên trước
                    .ThenBy(o => o.DEPARTMENT_NAME)
                    .ToList();

                // ===== Gom theo phòng (trong từng khoa) =====
                foreach (var g in this.sereServADOs.GroupBy(o => new { o.GROUP_DEPARTMENT_ID, o.GROUP_ROOM_ID }))
                {
                    SereServADO first = g.First();
                    GroupDepartmentADO ado = NewGroupTotals(g);
                    ado.GROUP_DEPARTMENT_ID = g.Key.GROUP_DEPARTMENT_ID;
                    ado.DEPARTMENT_CODE = first.GROUP_DEPARTMENT_CODE;
                    ado.DEPARTMENT_NAME = first.GROUP_DEPARTMENT_NAME;
                    ado.GROUP_ROOM_ID = g.Key.GROUP_ROOM_ID;
                    ado.ROOM_CODE = first.GROUP_ROOM_CODE;
                    ado.GROUP_ROOM_CODE = first.GROUP_ROOM_CODE;
                    ado.ROOM_NAME = first.GROUP_ROOM_NAME;
                    // NUM_ORDER nhỏ nhất của loại dịch vụ trong phòng.
                    ado.MIN_NUM_ORDER = g.Min(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999);

                    this.ServiceGroupByRoom.Add(ado);
                }
                // Phòng nằm lồng trong khoa (template nối theo GROUP_DEPARTMENT_ID) nên sắp theo NUM_ORDER của khoa trước
                // để phòng không nhảy khoa, rồi trong khoa sắp theo NUM_ORDER của phòng, cuối cùng là tên phòng.
                // 510 chỉ có 1 đối tượng BHYT -> dict chỉ key theo GROUP_DEPARTMENT_ID (không có KEY_PATY_ALTER như 512/508).
                var depaMinOrder = this.ServiceGroupByDepa
                    .ToDictionary(o => o.GROUP_DEPARTMENT_ID, o => o.MIN_NUM_ORDER);
                this.ServiceGroupByRoom = this.ServiceGroupByRoom
                    .OrderBy(o => depaMinOrder.ContainsKey(o.GROUP_DEPARTMENT_ID) ? depaMinOrder[o.GROUP_DEPARTMENT_ID] : 99999)
                    .ThenBy(o => o.MIN_NUM_ORDER)
                    .ThenBy(o => o.ROOM_NAME)
                    .ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private static GroupDepartmentADO NewGroupTotals(IEnumerable<SereServADO> g)
        {
            decimal totalPriceBhyt = g.Sum(o => o.TOTAL_PRICE_BHYT);
            decimal totalHeinPrice = g.Sum(o => o.VIR_TOTAL_HEIN_PRICE ?? 0);
            decimal totalPatientPrice = g.Sum(o => o.VIR_TOTAL_PATIENT_PRICE_BHYT ?? 0);
            return new GroupDepartmentADO
            {
                TOTAL_PRICE = g.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND ?? 0),
                TOTAL_PRICE_BHYT = totalPriceBhyt,
                TOTAL_HEIN_PRICE = totalHeinPrice,
                TOTAL_PATIENT_PRICE = totalPatientPrice,
                TOTAL_PATIENT_PRICE_SELF = g.Sum(o => o.TOTAL_PRICE_PATIENT_SELF),
                OTHER_SOURCE_PRICE = g.Sum(o => o.OTHER_SOURCE_PRICE ?? 0),
                TOTAL_PRICE_VP = g.Sum(o => o.TOTAL_PRICE_VP),
                TOTAL_PATIENT_PRICE_LEFT = g.Sum(o => o.TOTAL_PATIENT_PRICE_LEFT),
                // Alias trùng tên với bộ HeinServiceType để template dùng chung key
                TOTAL_PRICE_BHYT_HEIN_SERVICE_TYPE = totalPriceBhyt,
                TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE = totalHeinPrice,
                TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE = totalPatientPrice
            };
        }

        /// <summary>
        /// Dựng bộ key PatyAlterBHYT (port từ Mps000306). 510 chỉ có 1 đối tượng BHYT
        /// (CurrentPatyAlter) nên bộ này là 1 dòng tổng của cả điều trị, dùng cho các tag
        /// đầu/cuối trang: số thẻ, tỷ lệ, ngày thẻ, các cột tổng tiền.
        /// </summary>
        private void PatyAlterProcess()
        {
            this.patyAlterBHYTADOs = new List<PatyAlterBhytADO>();
            try
            {
                // 510 là bảng kê VIỆN PHÍ: KHÔNG lấy thông tin thẻ BHYT (mã thẻ, ngày thẻ);
                // mức hưởng để 0%. Bộ key này chỉ mang số tiền tổng hợp cho tag đầu/cuối trang.
                PatyAlterBhytADO ado = new PatyAlterBhytADO();
                ado.RATIO_STR = "0%";

                if (this.sereServADOs != null && this.sereServADOs.Count > 0)
                {
                    ado.TOTAL_PRICE = this.sereServADOs.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND ?? 0);
                    ado.TOTAL_PRICE_BHYT = this.sereServADOs.Sum(o => o.TOTAL_PRICE_BHYT);
                    ado.TOTAL_PRICE_HEIN = this.sereServADOs.Sum(o => o.VIR_TOTAL_HEIN_PRICE ?? 0);
                    ado.TOTAL_PRICE_PATIENT = this.sereServADOs.Sum(o => o.VIR_TOTAL_PATIENT_PRICE_BHYT ?? 0);
                    ado.TOTAL_PRICE_PATIENT_SELF = this.sereServADOs.Sum(o => o.TOTAL_PRICE_PATIENT_SELF);
                    ado.TOTAL_PRICE_OTHER = this.sereServADOs.Sum(o => o.OTHER_SOURCE_PRICE ?? 0);
                    ado.TOTAL_PATIENT_PRICE_LEFT = this.sereServADOs.Sum(o => o.TOTAL_PATIENT_PRICE_LEFT);
                    ado.TOTAL_PRICE_VP = this.sereServADOs.Sum(o => o.TOTAL_PRICE_VP);
                }

                this.patyAlterBHYTADOs.Add(ado);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
