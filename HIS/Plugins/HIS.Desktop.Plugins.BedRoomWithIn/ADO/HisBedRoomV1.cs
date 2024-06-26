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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.BedRoomWithIn.ADO
{
    class HisBedRoomV1 : MOS.EFMODEL.DataModels.V_HIS_BED_ROOM_1
    {
        public long TT_PATIENT_BED { get; set; }
        public string TT_PATIENT_BED_STR { get; set; }
        public long IsKey_ { get; set; }

        public long? BILL_PATIENT_TYPE_ID { get; set; }

        public bool? CHECK_IS_FULL { get; set; }
        public HisBedRoomV1() { }

        public HisBedRoomV1(MOS.EFMODEL.DataModels.V_HIS_BED_ROOM_1 data)
        {
            try
            {
                if (data != null)
                {
                    Inventec.Common.TypeConvert.Parse.ToUInt64(this.PATIENT_COUNT.ToString());
                    Inventec.Common.Mapper.DataObjectMapper.Map<HisBedRoomV1>(this, data);
                    this.TT_PATIENT_BED_STR = (int)this.PATIENT_COUNT + "/" + (int)this.BED_COUNT;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

