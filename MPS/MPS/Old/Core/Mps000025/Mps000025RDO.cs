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

namespace MPS.Core.Mps000025
{
    /// <summary>
    /// .
    /// </summary>
    public class Mps000025RDO : RDOBase
    {
        internal V_HIS_SERVICE_REQ ServiceReqPrint { get; set; }
        internal List<V_HIS_SERE_SERV> sereServs { get; set; }
        internal PatyAlterBhytADO PatyAlterBhyt { get; set; }

        public Mps000025RDO(
            V_HIS_SERVICE_REQ ServiceReqPrint,
            List<V_HIS_SERE_SERV> sereServs,
            PatyAlterBhytADO PatyAlterBhyt
            )
        {
            try
            {
                this.ServiceReqPrint = ServiceReqPrint;
                this.sereServs = sereServs;
                this.PatyAlterBhyt = PatyAlterBhyt;
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
                if (ServiceReqPrint != null)
                {
                    keyValues.Add(new KeyValue(Mps000025ExtendSingleKey.EXECUTE_ROOM_NAME, ServiceReqPrint.EXECUTE_ROOM_NAME));
                    keyValues.Add(new KeyValue(Mps000025ExtendSingleKey.INSTRUCTION_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(ServiceReqPrint.INTRUCTION_TIME)));
                    var tuoi = AgeUtil.CalculateFullAge(ServiceReqPrint.DOB);
                    keyValues.Add(new KeyValue(Mps000025ExtendSingleKey.AGE, tuoi));
                }
                else
                {
                    keyValues.Add(new KeyValue(Mps000025ExtendSingleKey.EXECUTE_ROOM_NAME, ""));
                    keyValues.Add(new KeyValue(Mps000025ExtendSingleKey.INSTRUCTION_TIME_STR, ""));
                }
               
                GlobalQuery.AddObjectKeyIntoListkey<PatyAlterBhytADO>(PatyAlterBhyt, keyValues,false);
                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_SERVICE_REQ>(ServiceReqPrint, keyValues);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


    }
}
