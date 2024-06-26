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
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000064.PDO
{
    public class Mps000064PDO : RDOBase
    {
        public V_HIS_PATIENT Patient { get; set; }
        public V_HIS_REHA_SERVICE_REQ rehaService { get; set; }
        public V_HIS_REHA_SUM hisRehaSumRow { get; set; }
        public List<RehaTrainPrintByPageADO> lsRehaTrain { get; set; }
        public List<RehaTrainPrintADO> rehaTrainPrints { get; set; }
        public HIS_DEPARTMENT department;

        public Mps000064PDO(
            V_HIS_PATIENT patient,
            V_HIS_REHA_SERVICE_REQ rehaService,
            V_HIS_REHA_SUM hisRehaSumRow,
            List<RehaTrainPrintByPageADO> lsRehaTrain,
            List<RehaTrainPrintADO> rehaTrainPrints,
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

        public class RehaTrainPrintByPageADO
        {
            public string Day1 { get; set; }
            public string Day2 { get; set; }
            public string Day3 { get; set; }
            public string Day4 { get; set; }
            public string Day5 { get; set; }
            public string Day6 { get; set; }
            public string Day7 { get; set; }
            public string Day8 { get; set; }
            public string Day9 { get; set; }
            public string Day10 { get; set; }
            public string Day11 { get; set; }
            public string Day12 { get; set; }
            public string Day13 { get; set; }
            public string Day14 { get; set; }
            public string Day15 { get; set; }
            public string Day16 { get; set; }
            public string Day17 { get; set; }
            public string Day18 { get; set; }
            public string Day19 { get; set; }
            public string Day20 { get; set; }
            public string Day21 { get; set; }
            public string Day22 { get; set; }
            public string Day23 { get; set; }
            public string Day24 { get; set; }
            public List<RehaTrainPrintADO> RehaTrainPrintSDOs { get; set; }
        }

        public class RehaTrainPrintADO
        {
            public string RehaTrainTypeName { get; set; }
            public string Day1 { get; set; }
            public string Day2 { get; set; }
            public string Day3 { get; set; }
            public string Day4 { get; set; }
            public string Day5 { get; set; }
            public string Day6 { get; set; }
            public string Day7 { get; set; }
            public string Day8 { get; set; }
            public string Day9 { get; set; }
            public string Day10 { get; set; }
            public string Day11 { get; set; }
            public string Day12 { get; set; }
            public string Day13 { get; set; }
            public string Day14 { get; set; }
            public string Day15 { get; set; }
            public string Day16 { get; set; }
            public string Day17 { get; set; }
            public string Day18 { get; set; }
            public string Day19 { get; set; }
            public string Day20 { get; set; }
            public string Day21 { get; set; }
            public string Day22 { get; set; }
            public string Day23 { get; set; }
            public string Day24 { get; set; }
        }


    }
}
