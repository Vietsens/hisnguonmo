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
using MPS.ADO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Core.Mps000088
{
    /// <summary>
    /// In yeu cau kham.
    /// Dau vao:
    /// PatyAlterBhyt: doi tuong the bhyt
    /// ServiceReq: yeu cau dich vu
    /// PatientType: doi tuong benh nhan
    /// </summary>
    public class Mps000088RDO : RDOBase
    {
        internal List<MPS.ADO.Mps000088ByMediEndMate> mps000088ByMediEndMate;
        internal List<MPS.ADO.Mps000088ADO> mps000088ADO;
        internal V_HIS_TREATMENT currentTreatment { get; set; }
        HIS_DEPARTMENT department { get; set; }
        string bedRoomName;

        public Mps000088RDO(
            V_HIS_TREATMENT currentTreatment,
            List<MPS.ADO.Mps000088ADO> mps000088ADO,
            List<MPS.ADO.Mps000088ByMediEndMate> mps000088ByMediEndMate,
            HIS_DEPARTMENT department,
            string bedRoomName
            )
        {
            try
            {
                this.currentTreatment = currentTreatment;
                this.mps000088ByMediEndMate = mps000088ByMediEndMate;
                this.mps000088ADO = mps000088ADO;
                this.department = department;
                this.bedRoomName = bedRoomName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal override void SetSingleKey()
        {
            try
            {
                if (mps000088ADO != null && mps000088ADO.Count > 0)
                {
                    foreach (var item in mps000088ADO)
                    {
                        keyValues.Add(new KeyValue(Mps000088ExtendSingleKey.Day1, item.Day1));
                        keyValues.Add(new KeyValue(Mps000088ExtendSingleKey.Day2, item.Day2));
                        keyValues.Add(new KeyValue(Mps000088ExtendSingleKey.Day3, item.Day3));
                        keyValues.Add(new KeyValue(Mps000088ExtendSingleKey.Day4, item.Day4));
                        keyValues.Add(new KeyValue(Mps000088ExtendSingleKey.Day5, item.Day5));
                        keyValues.Add(new KeyValue(Mps000088ExtendSingleKey.Day6, item.Day6));
                        keyValues.Add(new KeyValue(Mps000088ExtendSingleKey.Day7, item.Day7));
                        keyValues.Add(new KeyValue(Mps000088ExtendSingleKey.Day8, item.Day8));
                        keyValues.Add(new KeyValue(Mps000088ExtendSingleKey.Day9, item.Day9));
                        keyValues.Add(new KeyValue(Mps000088ExtendSingleKey.Day10, item.Day10));
                        keyValues.Add(new KeyValue(Mps000088ExtendSingleKey.Day11, item.Day11));
                        keyValues.Add(new KeyValue(Mps000088ExtendSingleKey.Day12, item.Day12));
                        keyValues.Add(new KeyValue(Mps000088ExtendSingleKey.Day13, item.Day13));
                        keyValues.Add(new KeyValue(Mps000088ExtendSingleKey.Day14, item.Day14));
                        keyValues.Add(new KeyValue(Mps000088ExtendSingleKey.Day15, item.Day15));
                        keyValues.Add(new KeyValue(Mps000088ExtendSingleKey.Day16, item.Day16));
                        keyValues.Add(new KeyValue(Mps000088ExtendSingleKey.Day17, item.Day17));
                        keyValues.Add(new KeyValue(Mps000088ExtendSingleKey.Day18, item.Day18));
                        keyValues.Add(new KeyValue(Mps000088ExtendSingleKey.Day19, item.Day19));
                        keyValues.Add(new KeyValue(Mps000088ExtendSingleKey.Day20, item.Day20));
                        keyValues.Add(new KeyValue(Mps000088ExtendSingleKey.Day21, item.Day21));
                        keyValues.Add(new KeyValue(Mps000088ExtendSingleKey.Day22, item.Day22));
                        keyValues.Add(new KeyValue(Mps000088ExtendSingleKey.Day23, item.Day23));
                        keyValues.Add(new KeyValue(Mps000088ExtendSingleKey.Day24, item.Day24));
                    }
                    keyValues.Add(new KeyValue(Mps000088ExtendSingleKey.REQUEST_BED_ROOM_NAME, bedRoomName));
                    keyValues.Add(new KeyValue(Mps000088ExtendSingleKey.REQUEST_DEPARTMENT_NAME, department.DEPARTMENT_NAME));
                }
                if (currentTreatment != null)
                {
                    keyValues.Add((new KeyValue(Mps000088ExtendSingleKey.IN_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(currentTreatment.IN_TIME))));
                    if (currentTreatment.OUT_TIME != null)
                        keyValues.Add((new KeyValue(Mps000088ExtendSingleKey.OUT_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(currentTreatment.OUT_TIME ?? 0))));
                    if (currentTreatment.CLINICAL_IN_TIME != null)
                        keyValues.Add((new KeyValue(Mps000088ExtendSingleKey.CLINICAL_IN_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(currentTreatment.CLINICAL_IN_TIME ?? 0))));
                    keyValues.Add((new KeyValue(Mps000088ExtendSingleKey.AGE, AgeUtil.CalculateFullAge(currentTreatment.DOB))));
                }
                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_TREATMENT>(currentTreatment, keyValues);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
