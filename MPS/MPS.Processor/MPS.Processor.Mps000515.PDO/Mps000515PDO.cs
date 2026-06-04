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
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MPS.Processor.Mps000515.PDO
{
    public partial class Mps000515PDO : RDOBase
    {
        public Mps000515PDO() { }

        public Mps000515PDO(
            V_HIS_PATIENT currentPatient,
            V_HIS_PATIENT_TYPE_ALTER patyAlterBhyt,
            HIS_TREATMENT treatment,
            List<V_HIS_SERVICE_REQ> serviceReqs,
            List<V_HIS_SERE_SERV> sereServs)
            : this(currentPatient, patyAlterBhyt, treatment, serviceReqs, sereServs, null)
        {
        }

        public Mps000515PDO(
            V_HIS_PATIENT currentPatient,
            V_HIS_PATIENT_TYPE_ALTER patyAlterBhyt,
            HIS_TREATMENT treatment,
            List<V_HIS_SERVICE_REQ> serviceReqs,
            List<V_HIS_SERE_SERV> sereServs,
            string gate)
        {
            try
            {
                this.currentPatient = currentPatient;
                this.PatyAlterBhyt = patyAlterBhyt;
                this.currentTreatment = treatment;
                this.Gate = gate;
                this.ExamRooms = BuildExamRooms(serviceReqs, sereServs);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Chuyển danh sách yêu cầu khám đã đăng ký thành danh sách dòng phòng khám.
        /// Tên dịch vụ khám lấy từ sere_serv tương ứng theo SERVICE_REQ_ID.
        /// </summary>
        private List<Mps000515_ExamRoomRow> BuildExamRooms(
            List<V_HIS_SERVICE_REQ> serviceReqs,
            List<V_HIS_SERE_SERV> sereServs)
        {
            List<Mps000515_ExamRoomRow> result = new List<Mps000515_ExamRoomRow>();
            try
            {
                if (serviceReqs == null || serviceReqs.Count == 0)
                {
                    return result;
                }

                ILookup<long?, V_HIS_SERE_SERV> sereServLookup = (sereServs != null && sereServs.Count > 0)
                    ? sereServs.ToLookup(o => o.SERVICE_REQ_ID)
                    : null;

                List<V_HIS_SERVICE_REQ> orderedReqs = serviceReqs
                    .OrderBy(o => o.NUM_ORDER ?? long.MaxValue)
                    .ThenBy(o => o.ID)
                    .ToList();

                int stt = 1;
                foreach (var req in orderedReqs)
                {
                    Mps000515_ExamRoomRow row = new Mps000515_ExamRoomRow();
                    row.STT = stt++;
                    row.ROOM_CODE = req.EXECUTE_ROOM_CODE;
                    row.ROOM_NAME = req.EXECUTE_ROOM_NAME;
                    row.DEPARTMENT_NAME = req.EXECUTE_DEPARTMENT_NAME;
                    row.ROOM_ADDRESS = req.EXECUTE_ROOM_ADDRESS;
                    row.NUM_ORDER = req.NUM_ORDER;
                    row.NOTE = req.NOTE;

                    if (sereServLookup != null)
                    {
                        List<string> serviceNames = sereServLookup[req.ID]
                            .Select(o => o.TDL_SERVICE_NAME)
                            .Where(o => !string.IsNullOrWhiteSpace(o))
                            .Distinct()
                            .ToList();
                        row.SERVICE_NAME = string.Join(", ", serviceNames);
                    }

                    result.Add(row);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
