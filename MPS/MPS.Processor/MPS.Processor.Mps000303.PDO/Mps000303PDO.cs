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

namespace MPS.Processor.Mps000303.PDO
{
    public class Mps000303PDO : RDOBase
    {
        public List<Mps000303BySereServ> _Mps000303BySereServs;
        public List<Mps000303ADO> _Mps000303ADOs;
        public V_HIS_TREATMENT currentTreatment { get; set; }
        public SingleKeys _SingleKeys { get; set; }

        public Mps000303PDO() { }

        public Mps000303PDO(
            V_HIS_TREATMENT currentTreatment,
            List<Mps000303ADO> _mps000225ADO,
            List<Mps000303BySereServ> _mps000225BySSs,
            SingleKeys _singleKeys
            )
        {
            try
            {
                this.currentTreatment = currentTreatment;
                this._Mps000303BySereServs = _mps000225BySSs;
                this._Mps000303ADOs = _mps000225ADO;
                this._SingleKeys = _singleKeys;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
    public class Mps000303BySereServ : V_HIS_SERE_SERV
    {
        public long Service_Type_Id { get; set; }
        public string CONCENTRA { get; set; }
        public string AMOUNT_STRING { get; set; }

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
        public string Day25 { get; set; }
        public string Day26 { get; set; }
        public string Day27 { get; set; }
        public string Day28 { get; set; }
        public string Day29 { get; set; }
        public string Day30 { get; set; }
        public string Day31 { get; set; }
        public string Day32 { get; set; }
        public string Day33 { get; set; }
        public string Day34 { get; set; }
        public string Day35 { get; set; }
        public string Day36 { get; set; }
        public string Day37 { get; set; }
        public string Day38 { get; set; }
        public string Day39 { get; set; }
        public string Day40 { get; set; }
        public string Day41 { get; set; }
        public string Day42 { get; set; }
        public string Day43 { get; set; }
        public string Day44 { get; set; }
        public string Day45 { get; set; }
        public string Day46 { get; set; }
        public string Day47 { get; set; }
        public string Day48 { get; set; }
        public string Day49 { get; set; }
        public string Day50 { get; set; }
        public string Day51 { get; set; }
        public string Day52 { get; set; }
        public string Day53 { get; set; }
        public string Day54 { get; set; }
        public string Day55 { get; set; }
        public string Day56 { get; set; }
        public string Day57 { get; set; }
        public string Day58 { get; set; }
        public string Day59 { get; set; }
        public string Day60 { get; set; }

        public long TYPE_ID { get; set; }
    }

    public class Mps000303ADO
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
        public string Day25 { get; set; }
        public string Day26 { get; set; }
        public string Day27 { get; set; }
        public string Day28 { get; set; }
        public string Day29 { get; set; }
        public string Day30 { get; set; }
        public string Day31 { get; set; }
        public string Day32 { get; set; }
        public string Day33 { get; set; }
        public string Day34 { get; set; }
        public string Day35 { get; set; }
        public string Day36 { get; set; }
        public string Day37 { get; set; }
        public string Day38 { get; set; }
        public string Day39 { get; set; }
        public string Day40 { get; set; }
        public string Day41 { get; set; }
        public string Day42 { get; set; }
        public string Day43 { get; set; }
        public string Day44 { get; set; }
        public string Day45 { get; set; }
        public string Day46 { get; set; }
        public string Day47 { get; set; }
        public string Day48 { get; set; }
        public string Day49 { get; set; }
        public string Day50 { get; set; }
        public string Day51 { get; set; }
        public string Day52 { get; set; }
        public string Day53 { get; set; }
        public string Day54 { get; set; }
        public string Day55 { get; set; }
        public string Day56 { get; set; }
        public string Day57 { get; set; }
        public string Day58 { get; set; }
        public string Day59 { get; set; }
        public string Day60 { get; set; }

        public string DayAndYear1 { get; set; }
        public string DayAndYear2 { get; set; }
        public string DayAndYear3 { get; set; }
        public string DayAndYear4 { get; set; }
        public string DayAndYear5 { get; set; }
        public string DayAndYear6 { get; set; }
        public string DayAndYear7 { get; set; }
        public string DayAndYear8 { get; set; }
        public string DayAndYear9 { get; set; }
        public string DayAndYear10 { get; set; }
        public string DayAndYear11 { get; set; }
        public string DayAndYear12 { get; set; }
        public string DayAndYear13 { get; set; }
        public string DayAndYear14 { get; set; }
        public string DayAndYear15 { get; set; }
        public string DayAndYear16 { get; set; }
        public string DayAndYear17 { get; set; }
        public string DayAndYear18 { get; set; }
        public string DayAndYear19 { get; set; }
        public string DayAndYear20 { get; set; }
        public string DayAndYear21 { get; set; }
        public string DayAndYear22 { get; set; }
        public string DayAndYear23 { get; set; }
        public string DayAndYear24 { get; set; }
        public string DayAndYear25 { get; set; }
        public string DayAndYear26 { get; set; }
        public string DayAndYear27 { get; set; }
        public string DayAndYear28 { get; set; }
        public string DayAndYear29 { get; set; }
        public string DayAndYear30 { get; set; }
        public string DayAndYear31 { get; set; }
        public string DayAndYear32 { get; set; }
        public string DayAndYear33 { get; set; }
        public string DayAndYear34 { get; set; }
        public string DayAndYear35 { get; set; }
        public string DayAndYear36 { get; set; }
        public string DayAndYear37 { get; set; }
        public string DayAndYear38 { get; set; }
        public string DayAndYear39 { get; set; }
        public string DayAndYear40 { get; set; }
        public string DayAndYear41 { get; set; }
        public string DayAndYear42 { get; set; }
        public string DayAndYear43 { get; set; }
        public string DayAndYear44 { get; set; }
        public string DayAndYear45 { get; set; }
        public string DayAndYear46 { get; set; }
        public string DayAndYear47 { get; set; }
        public string DayAndYear48 { get; set; }
        public string DayAndYear49 { get; set; }
        public string DayAndYear50 { get; set; }
        public string DayAndYear51 { get; set; }
        public string DayAndYear52 { get; set; }
        public string DayAndYear53 { get; set; }
        public string DayAndYear54 { get; set; }
        public string DayAndYear55 { get; set; }
        public string DayAndYear56 { get; set; }
        public string DayAndYear57 { get; set; }
        public string DayAndYear58 { get; set; }
        public string DayAndYear59 { get; set; }
        public string DayAndYear60 { get; set; }


        public List<Mps000303BySereServ> Mps000303BySereServADOs { get; set; }
        public long TYPE_ID { get; set; }
    }

    public class SingleKeys
    {
        public string USER_NAME { get; set; }
        public string LOGIN_NAME { get; set; }
    }

    public class Mps000303Deparment
    {
        public long ID { get; set; }
        public string REQUEST_DEPARTMENT_NAME { get; set; }
        public long TYPE_ID { get; set; }
    }
}
