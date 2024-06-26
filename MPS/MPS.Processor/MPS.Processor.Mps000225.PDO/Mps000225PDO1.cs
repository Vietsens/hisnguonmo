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

namespace MPS.Processor.Mps000225.PDO
{
    public class Mps000225PDO : RDOBase
    {
        public List<Mps000225BySereServ> _Mps000225BySereServs;
        public List<Mps000225ADO> _Mps000225ADOs;
        public V_HIS_TREATMENT currentTreatment { get; set; }
        public SingleKeys _SingleKeys { get; set; }
        public List<HIS_PATIENT_TYPE> ListPatientType { get; set; }
        public List<HIS_SERVICE_TYPE> ListServiceType { get; set; }

        public Mps000225PDO() { }

        public Mps000225PDO(
            V_HIS_TREATMENT currentTreatment,
            List<Mps000225ADO> _mps000225ADO,
            List<Mps000225BySereServ> _mps000225BySSs,
            SingleKeys _singleKeys,
            List<HIS_PATIENT_TYPE> _listPatientType,
            List<HIS_SERVICE_TYPE> _listServiceType
            )
        {
            try
            {
                this.currentTreatment = currentTreatment;
                this._Mps000225BySereServs = _mps000225BySSs;
                this._Mps000225ADOs = _mps000225ADO;
                this._SingleKeys = _singleKeys;
                this.ListPatientType = _listPatientType;
                this.ListServiceType = _listServiceType;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }

    public class Mps000225BySereServ : V_HIS_SERE_SERV
    {
        public long Service_Type_Id { get; set; }
        public string CONCENTRA { get; set; }
        public string AMOUNT_STRING { get; set; }
        public string INSTRUCTION_NOTE { get; set; }

<<<<<<< .mine
||||||| .r110322
        public string NOON_STR { get; set; }
        public string MORNING_STR { get; set; }
        public string AFTERNOON_STR { get; set; }
        public string EVENING_STR { get; set; }
=======
        public string NOON { get; set; }
        public string MORNING { get; set; }
        public string AFTERNOON { get; set; }
        public string EVENING { get; set; }

>>>>>>> .r110410
        #region ---Day---
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
        public short? TypeExpend { get; set; } //null:All;0:no expend; 1: expend
        #endregion

        #region Morning_Day
        public string MORNING_Day1 { get; set; }
        public string MORNING_Day2 { get; set; }
        public string MORNING_Day3 { get; set; }
        public string MORNING_Day4 { get; set; }
        public string MORNING_Day5 { get; set; }
        public string MORNING_Day6 { get; set; }
        public string MORNING_Day7 { get; set; }
        public string MORNING_Day8 { get; set; }
        public string MORNING_Day9 { get; set; }
        public string MORNING_Day10 { get; set; }
        public string MORNING_Day11 { get; set; }
        public string MORNING_Day12 { get; set; }
        public string MORNING_Day13 { get; set; }
        public string MORNING_Day14 { get; set; }
        public string MORNING_Day15 { get; set; }
        public string MORNING_Day16 { get; set; }
        public string MORNING_Day17 { get; set; }
        public string MORNING_Day18 { get; set; }
        public string MORNING_Day19 { get; set; }
        public string MORNING_Day20 { get; set; }
        public string MORNING_Day21 { get; set; }
        public string MORNING_Day22 { get; set; }
        public string MORNING_Day23 { get; set; }
        public string MORNING_Day24 { get; set; }
        public string MORNING_Day25 { get; set; }
        public string MORNING_Day26 { get; set; }
        public string MORNING_Day27 { get; set; }
        public string MORNING_Day28 { get; set; }
        public string MORNING_Day29 { get; set; }
        public string MORNING_Day30 { get; set; }
        public string MORNING_Day31 { get; set; }
        public string MORNING_Day32 { get; set; }
        public string MORNING_Day33 { get; set; }
        public string MORNING_Day34 { get; set; }
        public string MORNING_Day35 { get; set; }
        public string MORNING_Day36 { get; set; }
        public string MORNING_Day37 { get; set; }
        public string MORNING_Day38 { get; set; }
        public string MORNING_Day39 { get; set; }
        public string MORNING_Day40 { get; set; }
        public string MORNING_Day41 { get; set; }
        public string MORNING_Day42 { get; set; }
        public string MORNING_Day43 { get; set; }
        public string MORNING_Day44 { get; set; }
        public string MORNING_Day45 { get; set; }
        public string MORNING_Day46 { get; set; }
        public string MORNING_Day47 { get; set; }
        public string MORNING_Day48 { get; set; }
        public string MORNING_Day49 { get; set; }
        public string MORNING_Day50 { get; set; }
        public string MORNING_Day51 { get; set; }
        public string MORNING_Day52 { get; set; }
        public string MORNING_Day53 { get; set; }
        public string MORNING_Day54 { get; set; }
        public string MORNING_Day55 { get; set; }
        public string MORNING_Day56 { get; set; }
        public string MORNING_Day57 { get; set; }
        public string MORNING_Day58 { get; set; }
        public string MORNING_Day59 { get; set; }
        public string MORNING_Day60 { get; set; }
        #endregion

        #region Noon_day
        public string NOON_Day1 { get; set; }
        public string NOON_Day2 { get; set; }
        public string NOON_Day3 { get; set; }
        public string NOON_Day4 { get; set; }
        public string NOON_Day5 { get; set; }
        public string NOON_Day6 { get; set; }
        public string NOON_Day7 { get; set; }
        public string NOON_Day8 { get; set; }
        public string NOON_Day9 { get; set; }
        public string NOON_Day10 { get; set; }
        public string NOON_Day11 { get; set; }
        public string NOON_Day12 { get; set; }
        public string NOON_Day13 { get; set; }
        public string NOON_Day14 { get; set; }
        public string NOON_Day15 { get; set; }
        public string NOON_Day16 { get; set; }
        public string NOON_Day17 { get; set; }
        public string NOON_Day18 { get; set; }
        public string NOON_Day19 { get; set; }
        public string NOON_Day20 { get; set; }
        public string NOON_Day21 { get; set; }
        public string NOON_Day22 { get; set; }
        public string NOON_Day23 { get; set; }
        public string NOON_Day24 { get; set; }
        public string NOON_Day25 { get; set; }
        public string NOON_Day26 { get; set; }
        public string NOON_Day27 { get; set; }
        public string NOON_Day28 { get; set; }
        public string NOON_Day29 { get; set; }
        public string NOON_Day30 { get; set; }
        public string NOON_Day31 { get; set; }
        public string NOON_Day32 { get; set; }
        public string NOON_Day33 { get; set; }
        public string NOON_Day34 { get; set; }
        public string NOON_Day35 { get; set; }
        public string NOON_Day36 { get; set; }
        public string NOON_Day37 { get; set; }
        public string NOON_Day38 { get; set; }
        public string NOON_Day39 { get; set; }
        public string NOON_Day40 { get; set; }
        public string NOON_Day41 { get; set; }
        public string NOON_Day42 { get; set; }
        public string NOON_Day43 { get; set; }
        public string NOON_Day44 { get; set; }
        public string NOON_Day45 { get; set; }
        public string NOON_Day46 { get; set; }
        public string NOON_Day47 { get; set; }
        public string NOON_Day48 { get; set; }
        public string NOON_Day49 { get; set; }
        public string NOON_Day50 { get; set; }
        public string NOON_Day51 { get; set; }
        public string NOON_Day52 { get; set; }
        public string NOON_Day53 { get; set; }
        public string NOON_Day54 { get; set; }
        public string NOON_Day55 { get; set; }
        public string NOON_Day56 { get; set; }
        public string NOON_Day57 { get; set; }
        public string NOON_Day58 { get; set; }
        public string NOON_Day59 { get; set; }
        public string NOON_Day60 { get; set; }
        #endregion

        #region Afternoon_day
        public string AFTERNOON_Day1 { get; set; }
        public string AFTERNOON_Day2 { get; set; }
        public string AFTERNOON_Day3 { get; set; }
        public string AFTERNOON_Day4 { get; set; }
        public string AFTERNOON_Day5 { get; set; }
        public string AFTERNOON_Day6 { get; set; }
        public string AFTERNOON_Day7 { get; set; }
        public string AFTERNOON_Day8 { get; set; }
        public string AFTERNOON_Day9 { get; set; }
        public string AFTERNOON_Day10 { get; set; }
        public string AFTERNOON_Day11 { get; set; }
        public string AFTERNOON_Day12 { get; set; }
        public string AFTERNOON_Day13 { get; set; }
        public string AFTERNOON_Day14 { get; set; }
        public string AFTERNOON_Day15 { get; set; }
        public string AFTERNOON_Day16 { get; set; }
        public string AFTERNOON_Day17 { get; set; }
        public string AFTERNOON_Day18 { get; set; }
        public string AFTERNOON_Day19 { get; set; }
        public string AFTERNOON_Day20 { get; set; }
        public string AFTERNOON_Day21 { get; set; }
        public string AFTERNOON_Day22 { get; set; }
        public string AFTERNOON_Day23 { get; set; }
        public string AFTERNOON_Day24 { get; set; }
        public string AFTERNOON_Day25 { get; set; }
        public string AFTERNOON_Day26 { get; set; }
        public string AFTERNOON_Day27 { get; set; }
        public string AFTERNOON_Day28 { get; set; }
        public string AFTERNOON_Day29 { get; set; }
        public string AFTERNOON_Day30 { get; set; }
        public string AFTERNOON_Day31 { get; set; }
        public string AFTERNOON_Day32 { get; set; }
        public string AFTERNOON_Day33 { get; set; }
        public string AFTERNOON_Day34 { get; set; }
        public string AFTERNOON_Day35 { get; set; }
        public string AFTERNOON_Day36 { get; set; }
        public string AFTERNOON_Day37 { get; set; }
        public string AFTERNOON_Day38 { get; set; }
        public string AFTERNOON_Day39 { get; set; }
        public string AFTERNOON_Day40 { get; set; }
        public string AFTERNOON_Day41 { get; set; }
        public string AFTERNOON_Day42 { get; set; }
        public string AFTERNOON_Day43 { get; set; }
        public string AFTERNOON_Day44 { get; set; }
        public string AFTERNOON_Day45 { get; set; }
        public string AFTERNOON_Day46 { get; set; }
        public string AFTERNOON_Day47 { get; set; }
        public string AFTERNOON_Day48 { get; set; }
        public string AFTERNOON_Day49 { get; set; }
        public string AFTERNOON_Day50 { get; set; }
        public string AFTERNOON_Day51 { get; set; }
        public string AFTERNOON_Day52 { get; set; }
        public string AFTERNOON_Day53 { get; set; }
        public string AFTERNOON_Day54 { get; set; }
        public string AFTERNOON_Day55 { get; set; }
        public string AFTERNOON_Day56 { get; set; }
        public string AFTERNOON_Day57 { get; set; }
        public string AFTERNOON_Day58 { get; set; }
        public string AFTERNOON_Day59 { get; set; }
        public string AFTERNOON_Day60 { get; set; }
        #endregion

        #region Evening_day
        public string EVENING_Day1 { get; set; }
        public string EVENING_Day2 { get; set; }
        public string EVENING_Day3 { get; set; }
        public string EVENING_Day4 { get; set; }
        public string EVENING_Day5 { get; set; }
        public string EVENING_Day6 { get; set; }
        public string EVENING_Day7 { get; set; }
        public string EVENING_Day8 { get; set; }
        public string EVENING_Day9 { get; set; }
        public string EVENING_Day10 { get; set; }
        public string EVENING_Day11 { get; set; }
        public string EVENING_Day12 { get; set; }
        public string EVENING_Day13 { get; set; }
        public string EVENING_Day14 { get; set; }
        public string EVENING_Day15 { get; set; }
        public string EVENING_Day16 { get; set; }
        public string EVENING_Day17 { get; set; }
        public string EVENING_Day18 { get; set; }
        public string EVENING_Day19 { get; set; }
        public string EVENING_Day20 { get; set; }
        public string EVENING_Day21 { get; set; }
        public string EVENING_Day22 { get; set; }
        public string EVENING_Day23 { get; set; }
        public string EVENING_Day24 { get; set; }
        public string EVENING_Day25 { get; set; }
        public string EVENING_Day26 { get; set; }
        public string EVENING_Day27 { get; set; }
        public string EVENING_Day28 { get; set; }
        public string EVENING_Day29 { get; set; }
        public string EVENING_Day30 { get; set; }
        public string EVENING_Day31 { get; set; }
        public string EVENING_Day32 { get; set; }
        public string EVENING_Day33 { get; set; }
        public string EVENING_Day34 { get; set; }
        public string EVENING_Day35 { get; set; }
        public string EVENING_Day36 { get; set; }
        public string EVENING_Day37 { get; set; }
        public string EVENING_Day38 { get; set; }
        public string EVENING_Day39 { get; set; }
        public string EVENING_Day40 { get; set; }
        public string EVENING_Day41 { get; set; }
        public string EVENING_Day42 { get; set; }
        public string EVENING_Day43 { get; set; }
        public string EVENING_Day44 { get; set; }
        public string EVENING_Day45 { get; set; }
        public string EVENING_Day46 { get; set; }
        public string EVENING_Day47 { get; set; }
        public string EVENING_Day48 { get; set; }
        public string EVENING_Day49 { get; set; }
        public string EVENING_Day50 { get; set; }
        public string EVENING_Day51 { get; set; }
        public string EVENING_Day52 { get; set; }
        public string EVENING_Day53 { get; set; }
        public string EVENING_Day54 { get; set; }
        public string EVENING_Day55 { get; set; }
        public string EVENING_Day56 { get; set; }
        public string EVENING_Day57 { get; set; }
        public string EVENING_Day58 { get; set; }
        public string EVENING_Day59 { get; set; }
        public string EVENING_Day60 { get; set; }
        #endregion
    }

    public class Mps000225ADO
    {
        #region ---Day---
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
        #endregion

        #region ---Day and year---
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

        #endregion
        public List<Mps000225BySereServ> Mps000225BySereServADOs { get; set; }
    }

    public class SingleKeys
    {
        public string REQUEST_DEPARTMENT_NAME { get; set; }
        public string BED_ROOM_NAME { get; set; }
        public string BED_CODE { get; set; }
        public string BED_NAME { get; set; }
        public string USER_NAME { get; set; }
        public string LOGIN_NAME { get; set; }
    }
}
