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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.ADO
{
    public class Mps000146ADO : V_HIS_INFUSION
    {
        public string START_TIME_STR { get; set; }
        public string FINISH_TIME_STR { get; set; }
        public string CREATE_TIME_STR { get; set; }
        public string EXECUTE_DEPARTMENT_NAME { get; set; }
        public string EXECUTE_ROOM_NAME { get; set; }
        public string EXECUTE_DEPARTMENT_CODE { get; set; }
        public string EXECUTE_ROOM_CODE { get; set; }

        public string MODIFY_TIME_STR { get; set; }

        public Mps000146ADO() { }

        public Mps000146ADO(V_HIS_INFUSION data)
        {
            try
            {
                if (data != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<Mps000146ADO>(this, data);
                    this.CREATE_TIME_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.CREATE_TIME ?? 0);
                    if (this.START_TIME.HasValue)
                    {
                        this.START_TIME_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this.START_TIME.Value);
                    }

                    if (this.FINISH_TIME.HasValue)
                    {
                        this.FINISH_TIME_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this.FINISH_TIME.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
