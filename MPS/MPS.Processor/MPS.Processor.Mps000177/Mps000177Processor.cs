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
using Inventec.Core;
using MPS.Processor.Mps000177.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000177
{
    public class Mps000177Processor : AbstractProcessor
    {
        Mps000177PDO rdo;
        List<Mps000177MediMate> medicine = null;
        List<Mps000177MediMate> material = null;

        public Mps000177Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000177PDO)rdoBase;
        }

        public override bool ProcessData()
        {
            bool result = true;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                SetData();

                singleTag.ProcessData(store, singleValueDictionary);
                objectTag.AddObjectData(store, "list", rdo.currentPatient);
                objectTag.AddObjectData(store, "medicine", medicine);
                objectTag.AddObjectData(store, "material", material);
                objectTag.AddRelationship(store, "list", "medicine", "treatment_id", "treatment_id");
                objectTag.AddRelationship(store, "list", "material", "treatment_id", "treatment_id");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void SetData()
        {
            try
            {
                if (rdo.Mps000177MediMate != null && rdo.Mps000177MediMate.Count > 0)
                {
                    medicine = new List<Mps000177MediMate>();
                    material = new List<Mps000177MediMate>();
                    foreach (var item in rdo.Mps000177MediMate)
                    {
                        if (item.type == 1)
                        {
                            medicine.Add(item);
                        }
                        else
                        {
                            material.Add(item);
                        }
                    }
                }

                if (rdo.Mps000177DAY != null && rdo.Mps000177DAY.Count > 0)
                {
                    foreach (var patient in rdo.currentPatient)
                    {
                        if (rdo.bedRoomName.ContainsKey(patient.treatment_id))
                        {
                            patient.ROOM_NAME = rdo.bedRoomName[patient.treatment_id].BED_ROOM_NAME;
                            patient.BED_NAME = rdo.bedRoomName[patient.treatment_id].BED_NAME;
                        }
                        var item = rdo.Mps000177DAY.FirstOrDefault(o => o.treatment_id == patient.treatment_id);
                        if (item == null) continue;
                        patient.Day1 = item.Day1;
                        patient.Day2 = item.Day2;
                        patient.Day3 = item.Day3;
                        patient.Day4 = item.Day4;
                        patient.Day5 = item.Day5;
                        patient.Day6 = item.Day6;
                        patient.Day7 = item.Day7;
                        patient.Day8 = item.Day8;
                        patient.Day9 = item.Day9;
                        patient.Day10 = item.Day10;
                        patient.Day11 = item.Day11;
                        patient.Day12 = item.Day12;
                        patient.Day13 = item.Day13;
                        patient.Day14 = item.Day14;
                        patient.Day15 = item.Day15;
                        patient.Day16 = item.Day16;
                        patient.Day17 = item.Day17;
                        patient.Day18 = item.Day18;
                        patient.Day19 = item.Day19;
                        patient.Day20 = item.Day20;
                        patient.Day21 = item.Day21;
                        patient.Day22 = item.Day22;
                        patient.Day23 = item.Day23;
                        patient.Day24 = item.Day24;
                    }
                }

                if (!String.IsNullOrEmpty(rdo.departmentName))
                {
                    SetSingleKey(new KeyValue(Mps000177ExtendSingleKey.DEPARTMENT_NAME, rdo.departmentName));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
