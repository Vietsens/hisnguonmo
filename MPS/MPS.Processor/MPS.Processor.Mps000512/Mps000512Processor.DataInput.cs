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
using MPS.Processor.Mps000512.ADO;
using MPS.Processor.Mps000512.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000512
{
    public partial class Mps000512Processor : AbstractProcessor
    {
        /// <summary>
        /// Dựng dữ liệu đầu vào. Gom logic lặp 3 lần của Mps000302 (main / suất ăn / không-khám-0đ)
        /// về 1 hàm dùng chung <see cref="BuildAndGroupSereServ"/> (đã bỏ nhánh NoExamZero).
        /// Hai bộ main + suất ăn chạy song song như Mps000302.
        /// </summary>
        private void DataInputProcess()
        {
            try
            {
                patientADO = DataRawProcess.PatientRawToADO(rdo.Treatment);
                List<HIS_PATIENT_TYPE_ALTER> ListPta = rdo.PatientTypeAlterAlls.OrderByDescending(o => o.LOG_TIME).ToList();

                // Dựng bộ tra cứu danh mục MỘT LẦN (O(1)) dùng chung cho cả 2 bộ (main + suất ăn).
                SereServLookup lk = new SereServLookup(
                    rdo.Services, rdo.HeinServiceTypes, rdo.Rooms, rdo.Departments,
                    rdo.medicineTypes, rdo.MedicineLines, rdo.ListPatientType, rdo.HisServiceUnit, rdo.ServiceReqs,
                    rdo.SereServExts, rdo.ListSereServBills, rdo.ListSereServDeposits, rdo.ListSeseDepoRepays);

                List<Task> taskall = new List<Task>();

                Task tsMain = Task.Factory.StartNew(() =>
                {
                    this.sereServADOs = BuildAndGroupSereServ(lk, ListPta, groupSuatAn: false);
                });
                taskall.Add(tsMain);

                Task tsSuatAn = Task.Factory.StartNew(() =>
                {
                    this.sereServADOSAs = BuildAndGroupSereServ(lk, ListPta, groupSuatAn: true);
                });
                taskall.Add(tsSuatAn);

                Task.WaitAll(taskall.ToArray());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Map HIS_SERE_SERV -> SereServADO, lọc theo khoa (nếu cấu hình), rồi gom dòng trùng.
        /// Khoá gom thêm chiều khoa/phòng khi GroupType bật để dịch vụ ở khoa/phòng khác nhau không bị nhập làm một.
        /// Khi GroupType = None thì DEPT/ROOM = 0 -> gom GIỐNG HỆT Mps000302.
        /// </summary>
        private List<SereServADO> BuildAndGroupSereServ(SereServLookup lk, List<HIS_PATIENT_TYPE_ALTER> ListPta, bool groupSuatAn)
        {
            List<SereServADO> result = new List<SereServADO>();
            try
            {
                List<SereServADO> temps = (from r in rdo.SereServs
                                           select new SereServADO(r, lk, rdo.PatientTypeCFG, rdo.HisConfigValue,
                                               rdo.Treatment, ListPta, groupSuatAn)).ToList();

                // ==== Mục tiêu 2: LỌC theo khoa (chỉ 1 điều kiện) ====
                if (rdo.FilterDepartmentId.HasValue && rdo.FilterDepartmentId.Value > 0)
                {
                    temps = temps.Where(o => o.GROUP_DEPARTMENT_ID == rdo.FilterDepartmentId.Value).ToList();
                }

                IEnumerable<SereServADO> rows = temps.Where(o =>
                    o.AMOUNT > 0
                    && o.IS_NO_EXECUTE != 1
                    && (!rdo.HisConfigValue.IsNotIncludeIsExpend || (rdo.HisConfigValue.IsNotIncludeIsExpend && o.IS_EXPEND != 1)));

                if (groupSuatAn)
                    rows = rows.Where(o => !o.IsHide);

                var groups = rows
                    .OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999)
                    .GroupBy(o => new
                    {
                        o.SERVICE_ID,
                        o.PRIMARY_PRICE,
                        o.PRICE_BHYT,
                        o.SERVICE_PAY_RATE,
                        o.BHYT_PAY_RATE,
                        o.IS_EXPEND,
                        o.NUMBER_OF_FILM,
                        o.KEY_PATY_ALTER,
                        o.HEIN_SERVICE_TYPE_ID,
                        o.STENT_ORDER,
                        DEPT = rdo.GroupType == GroupServiceType.None ? 0L : o.GROUP_DEPARTMENT_ID,
                        ROOM = rdo.GroupType == GroupServiceType.Room ? o.GROUP_ROOM_ID : 0L
                    }).ToList();

                foreach (var g in groups)
                {
                    SereServADO sereServ = g.FirstOrDefault();
                    sereServ.AMOUNT = g.Sum(o => o.AMOUNT);
                    sereServ.VIR_TOTAL_HEIN_PRICE = g.Sum(o => o.VIR_TOTAL_HEIN_PRICE);
                    sereServ.VIR_TOTAL_PATIENT_PRICE_BHYT = g.Sum(o => o.VIR_TOTAL_PATIENT_PRICE_BHYT);
                    sereServ.TOTAL_PRICE_BHYT = g.Sum(o => o.TOTAL_PRICE_BHYT);
                    sereServ.VIR_TOTAL_PATIENT_PRICE = g.Sum(o => o.VIR_TOTAL_PATIENT_PRICE);
                    sereServ.VIR_TOTAL_PRICE_NO_EXPEND = g.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND);
                    sereServ.TOTAL_PRICE_PATIENT_SELF = g.Sum(o => o.TOTAL_PRICE_PATIENT_SELF);
                    sereServ.OTHER_SOURCE_PRICE = g.Sum(o => o.OTHER_SOURCE_PRICE);
                    sereServ.TOTAL_PATIENT_PRICE_LEFT = g.Sum(o => o.TOTAL_PATIENT_PRICE_LEFT);
                    sereServ.TOTAL_PRICE_VP = g.Sum(o => o.TOTAL_PRICE_VP);
                    sereServ.IS_PAID = g.Min(o => o.IS_PAID);//tất cả thanh toán min sẽ là 1 nếu có 1 dv chưa thanh toán min sẽ là 0
                    result.Add(sereServ);

                    if (sereServ.STENT_ORDER.HasValue && sereServ.STENT_ORDER.Value > 1)
                    {
                        decimal quyBHTT = sereServ.VIR_TOTAL_HEIN_PRICE ?? 0;
                        decimal bnCungChiTra = sereServ.VIR_TOTAL_PATIENT_PRICE_BHYT ?? 0;
                        decimal nguonKhac = sereServ.OTHER_SOURCE_PRICE ?? 0;

                        decimal bnHoacNguonKhac = bnCungChiTra > 0 ? bnCungChiTra : nguonKhac;

                        sereServ.TOTAL_PRICE_BHYT = quyBHTT + bnHoacNguonKhac;
                    }
                }

                //không có stent lên đầu.
                result = result.OrderBy(o => o.STENT_ORDER ?? 0).ThenBy(o => o.SERVICE_NAME).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
