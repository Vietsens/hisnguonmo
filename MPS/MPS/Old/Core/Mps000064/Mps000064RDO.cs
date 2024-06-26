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

using MPS;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPS.ADO;

namespace MPS.Core.Mps000064
{
    /// <summary>
    /// .
    /// </summary>
    public class Mps000064RDO : RDOBase
    {
        internal PatientADO Patient { get; set; }
        internal V_HIS_REHA_SERVICE_REQ rehaService { get; set; }
        internal V_HIS_REHA_SUM hisRehaSumRow { get; set; }
        internal List<MPS.ADO.RehaTrainPrintByPageADO> lsRehaTrain { get; set; }
        internal List<MPS.ADO.RehaTrainPrintADO> rehaTrainPrints { get; set; }
        internal HIS_DEPARTMENT department;

        public Mps000064RDO(
            PatientADO patient,
            V_HIS_REHA_SERVICE_REQ rehaService,
            V_HIS_REHA_SUM hisRehaSumRow,
            List<MPS.ADO.RehaTrainPrintByPageADO> lsRehaTrain,
            List<MPS.ADO.RehaTrainPrintADO> rehaTrainPrints,
            HIS_DEPARTMENT department
            )
        {
            try
            {
                this.Patient = patient;
                this.rehaService = rehaService;
                this.hisRehaSumRow = hisRehaSumRow;
                this.lsRehaTrain = lsRehaTrain;
                this.rehaTrainPrints = rehaTrainPrints;
                this.department = department;
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
                foreach (var item in lsRehaTrain)
                {
                    keyValues.Add(new KeyValue(Mps000064ExtendSingleKey.Day1, item.Day1));
                    keyValues.Add(new KeyValue(Mps000064ExtendSingleKey.Day2, item.Day2));
                    keyValues.Add(new KeyValue(Mps000064ExtendSingleKey.Day3, item.Day3));
                    keyValues.Add(new KeyValue(Mps000064ExtendSingleKey.Day4, item.Day4));
                    keyValues.Add(new KeyValue(Mps000064ExtendSingleKey.Day5, item.Day5));
                    keyValues.Add(new KeyValue(Mps000064ExtendSingleKey.Day6, item.Day6));
                    keyValues.Add(new KeyValue(Mps000064ExtendSingleKey.Day7, item.Day7));
                    keyValues.Add(new KeyValue(Mps000064ExtendSingleKey.Day8, item.Day8));
                    keyValues.Add(new KeyValue(Mps000064ExtendSingleKey.Day9, item.Day9));
                    keyValues.Add(new KeyValue(Mps000064ExtendSingleKey.Day10, item.Day10));
                    keyValues.Add(new KeyValue(Mps000064ExtendSingleKey.Day11, item.Day11));
                    keyValues.Add(new KeyValue(Mps000064ExtendSingleKey.Day12, item.Day12));
                    keyValues.Add(new KeyValue(Mps000064ExtendSingleKey.Day13, item.Day13));
                    keyValues.Add(new KeyValue(Mps000064ExtendSingleKey.Day14, item.Day14));
                    keyValues.Add(new KeyValue(Mps000064ExtendSingleKey.Day15, item.Day15));
                    keyValues.Add(new KeyValue(Mps000064ExtendSingleKey.Day16, item.Day16));
                    keyValues.Add(new KeyValue(Mps000064ExtendSingleKey.Day17, item.Day17));
                    keyValues.Add(new KeyValue(Mps000064ExtendSingleKey.Day18, item.Day18));
                    keyValues.Add(new KeyValue(Mps000064ExtendSingleKey.Day19, item.Day19));
                    keyValues.Add(new KeyValue(Mps000064ExtendSingleKey.Day20, item.Day20));
                    keyValues.Add(new KeyValue(Mps000064ExtendSingleKey.Day21, item.Day21));
                    keyValues.Add(new KeyValue(Mps000064ExtendSingleKey.Day22, item.Day22));
                    keyValues.Add(new KeyValue(Mps000064ExtendSingleKey.Day23, item.Day23));
                    keyValues.Add(new KeyValue(Mps000064ExtendSingleKey.Day24, item.Day24));
                }

                GlobalQuery.AddObjectKeyIntoListkey<HIS_DEPARTMENT>(department, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_REHA_SERVICE_REQ>(rehaService, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_REHA_SUM>(hisRehaSumRow, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<PatientADO>(Patient, keyValues);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
