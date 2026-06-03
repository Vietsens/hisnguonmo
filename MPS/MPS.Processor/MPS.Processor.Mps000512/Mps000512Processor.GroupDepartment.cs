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
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MPS.Processor.Mps000512
{
    public partial class Mps000512Processor : AbstractProcessor
    {
        private List<GroupDepartmentADO> ServiceGroupByDepa { get; set; }
        private List<GroupDepartmentADO> ServiceGroupByRoom { get; set; }

        /// <summary>
        /// Dựng 2 bộ master gom theo khoa xử lý và phòng xử lý từ bộ Service đã gom (port từ Mps000510).
        /// - ServiceGroupByDepa: 1 dòng / khoa, khoa lâm sàng lên trước rồi theo tên.
        /// - ServiceGroupByRoom: 1 dòng / (khoa, phòng).
        /// </summary>
        private void BuildDepartmentRoomGroups()
        {
            this.ServiceGroupByDepa = new List<GroupDepartmentADO>();
            this.ServiceGroupByRoom = new List<GroupDepartmentADO>();
            try
            {
                if (this.sereServADOs == null || this.sereServADOs.Count == 0)
                    return;

                Dictionary<long, HIS_DEPARTMENT> deptById = new Dictionary<long, HIS_DEPARTMENT>();
                if (rdo.Departments != null)
                {
                    foreach (HIS_DEPARTMENT d in rdo.Departments)
                    {
                        if (!deptById.ContainsKey(d.ID))
                            deptById[d.ID] = d;
                    }
                }

                // ===== Gom theo khoa =====
                foreach (var g in this.sereServADOs.GroupBy(o => o.GROUP_DEPARTMENT_ID))
                {
                    SereServADO first = g.First();
                    GroupDepartmentADO ado = NewGroupTotals(g);
                    ado.GROUP_DEPARTMENT_ID = g.Key;
                    ado.DEPARTMENT_CODE = first.GROUP_DEPARTMENT_CODE;
                    ado.DEPARTMENT_NAME = first.GROUP_DEPARTMENT_NAME;

                    HIS_DEPARTMENT dept;
                    if (g.Key > 0 && deptById.TryGetValue(g.Key, out dept) && dept != null)
                        ado.IS_CLINICAL = dept.IS_CLINICAL;

                    this.ServiceGroupByDepa.Add(ado);
                }
                this.ServiceGroupByDepa = this.ServiceGroupByDepa
                    .OrderBy(o => o.IS_CLINICAL == 1 ? 0 : 1)   // khoa lâm sàng lên trước
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
                    ado.ROOM_NAME = first.GROUP_ROOM_NAME;

                    this.ServiceGroupByRoom.Add(ado);
                }
                this.ServiceGroupByRoom = this.ServiceGroupByRoom
                    .OrderBy(o => o.ROOM_NAME)
                    .ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private static GroupDepartmentADO NewGroupTotals(IEnumerable<SereServADO> g)
        {
            return new GroupDepartmentADO
            {
                TOTAL_PRICE = g.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND ?? 0),
                TOTAL_PRICE_BHYT = g.Sum(o => o.TOTAL_PRICE_BHYT),
                TOTAL_HEIN_PRICE = g.Sum(o => o.VIR_TOTAL_HEIN_PRICE ?? 0),
                TOTAL_PATIENT_PRICE = g.Sum(o => o.VIR_TOTAL_PATIENT_PRICE_BHYT ?? 0),
                TOTAL_PATIENT_PRICE_SELF = g.Sum(o => o.TOTAL_PRICE_PATIENT_SELF),
                OTHER_SOURCE_PRICE = g.Sum(o => o.OTHER_SOURCE_PRICE ?? 0),
                TOTAL_PRICE_VP = g.Sum(o => o.TOTAL_PRICE_VP),
                TOTAL_PATIENT_PRICE_LEFT = g.Sum(o => o.TOTAL_PATIENT_PRICE_LEFT)
            };
        }
    }
}
