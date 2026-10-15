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
using HIS.Desktop.Plugins.WaitingScreenPttt.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.WaitingScreenPttt
{
    internal class WaitingPatientLoader
    {
        /// <summary>
        /// Danh sách BN đang nằm trong các buồng đã chọn và có dịch vụ PT/TT đã hoàn thành
        /// </summary>
        internal static List<WaitingPatientADO> Load(long departmentId, List<long> bedRoomIds)
        {
            List<WaitingPatientADO> result = new List<WaitingPatientADO>();
            if (bedRoomIds == null || bedRoomIds.Count == 0)
                return result;

            CommonParam param = new CommonParam();

            // 1. BN đang nằm trong buồng tại thời điểm hiện tại
            HisTreatmentBedRoomView1Filter bedRoomFilter = new HisTreatmentBedRoomView1Filter();
            bedRoomFilter.DEPARTMENT_ID = departmentId;
            bedRoomFilter.IS_IN_ROOM = true;
            List<V_HIS_TREATMENT_BED_ROOM_1> treatmentBedRooms = new BackendAdapter(param).Get<List<V_HIS_TREATMENT_BED_ROOM_1>>("api/HisTreatmentBedRoom/GetView1", ApiConsumers.MosConsumer, bedRoomFilter, param);
            if (treatmentBedRooms == null || treatmentBedRooms.Count == 0)
                return result;

            // 1 hồ sơ chỉ 1 dòng: nếu đang có mặt ở nhiều buồng thì lấy buồng vào sau cùng
            List<V_HIS_TREATMENT_BED_ROOM_1> inRooms = treatmentBedRooms
                .Where(o => bedRoomIds.Contains(o.BED_ROOM_ID))
                .GroupBy(o => o.TREATMENT_ID)
                .Select(g => g.OrderByDescending(o => o.ADD_TIME).First())
                .ToList();
            if (inRooms.Count == 0)
                return result;

            // 2. Dịch vụ PT/TT đã hoàn thành của các hồ sơ đó
            List<V_HIS_SERE_SERV_1> sereServs = new List<V_HIS_SERE_SERV_1>();
            List<long> treatmentIds = inRooms.Select(o => o.TREATMENT_ID).Distinct().ToList();
            for (int i = 0; i < treatmentIds.Count; i += 100)
            {
                HisSereServView1Filter sereServFilter = new HisSereServView1Filter();
                sereServFilter.TREATMENT_IDs = treatmentIds.Skip(i).Take(100).ToList();
                sereServFilter.SERVICE_TYPE_IDs = new List<long> { IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT, IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT };
                sereServFilter.SERVICE_REQ_STT_IDs = new List<long> { IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT };
                sereServFilter.HAS_EXECUTE = true;
                List<V_HIS_SERE_SERV_1> batch = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV_1>>("api/HisSereServ/GetView1", ApiConsumers.MosConsumer, sereServFilter, param);
                if (batch != null)
                    sereServs.AddRange(batch);
            }
            if (sereServs.Count == 0)
                return result;

            // 3. Mỗi hồ sơ chọn 1 dịch vụ: ưu tiên dịch vụ chính (không phải dịch vụ gắn kèm),
            //    nhiều dịch vụ chính (tách dịch vụ) thì lấy dịch vụ mới nhất
            Dictionary<long, V_HIS_SERE_SERV_1> dicChosen = new Dictionary<long, V_HIS_SERE_SERV_1>();
            // Kíp mới nhất của y lệnh chứa dịch vụ được chọn
            Dictionary<long, long> dicEkipId = new Dictionary<long, long>();
            foreach (var group in sereServs.Where(o => o.TDL_TREATMENT_ID.HasValue).GroupBy(o => o.TDL_TREATMENT_ID.Value))
            {
                List<V_HIS_SERE_SERV_1> mains = group.Where(o => o.PARENT_ID == null).ToList();
                List<V_HIS_SERE_SERV_1> candidates = mains.Count > 0 ? mains : group.ToList();
                V_HIS_SERE_SERV_1 chosen = candidates.OrderByDescending(o => o.TDL_INTRUCTION_TIME).ThenByDescending(o => o.ID).First();
                dicChosen[group.Key] = chosen;

                long? newestEkipId = group.Where(o => o.SERVICE_REQ_ID == chosen.SERVICE_REQ_ID && o.EKIP_ID.HasValue).Select(o => o.EKIP_ID).Max();
                if (newestEkipId.HasValue)
                    dicEkipId[group.Key] = newestEkipId.Value;
            }

            // 4. Bác sĩ thực hiện chính trong kíp
            Dictionary<long, string> dicDoctor = new Dictionary<long, string>();
            List<long> ekipIds = dicEkipId.Values.Distinct().ToList();
            for (int i = 0; i < ekipIds.Count; i += 100)
            {
                HisEkipUserViewFilter ekipUserFilter = new HisEkipUserViewFilter();
                ekipUserFilter.EKIP_IDs = ekipIds.Skip(i).Take(100).ToList();
                ekipUserFilter.IS_SURG_MAIN = true;
                List<V_HIS_EKIP_USER> ekipUsers = new BackendAdapter(param).Get<List<V_HIS_EKIP_USER>>("api/HisEkipUser/GetView", ApiConsumers.MosConsumer, ekipUserFilter, param);
                if (ekipUsers == null)
                    continue;
                foreach (var group in ekipUsers.GroupBy(o => o.EKIP_ID))
                {
                    dicDoctor[group.Key] = group.OrderBy(o => o.ID).First().USERNAME;
                }
            }

            // 5. Ghép dữ liệu, sắp theo buồng rồi theo thời gian vào buồng
            foreach (var inRoom in inRooms)
            {
                V_HIS_SERE_SERV_1 chosen;
                if (!dicChosen.TryGetValue(inRoom.TREATMENT_ID, out chosen))
                    continue;

                WaitingPatientADO ado = new WaitingPatientADO();
                ado.TREATMENT_ID = inRoom.TREATMENT_ID;
                ado.PATIENT_NAME = inRoom.TDL_PATIENT_NAME;
                ado.DOB_YEAR = inRoom.TDL_PATIENT_DOB > 0 ? inRoom.TDL_PATIENT_DOB.ToString().Substring(0, 4) : "";
                ado.BED_ROOM_NAME = inRoom.BED_ROOM_NAME;
                ado.ADD_TIME = inRoom.ADD_TIME;
                ado.SERVICE_NAME = chosen.TDL_SERVICE_NAME;

                long ekipId;
                string doctorName;
                if (dicEkipId.TryGetValue(inRoom.TREATMENT_ID, out ekipId) && dicDoctor.TryGetValue(ekipId, out doctorName))
                    ado.DOCTOR_NAME = doctorName;

                result.Add(ado);
            }

            return result.OrderBy(o => o.BED_ROOM_NAME).ThenBy(o => o.ADD_TIME).ToList();
        }
    }
}
