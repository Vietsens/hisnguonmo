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

namespace MPS.ADO.TrackingPrint
{
    public class Mps000062ADOMedicines : MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE
    {
        public long TRACKING_ID { get; set; }
        public string TRACKING_TIME_STR { get; set; }
        public string Tracking_Detail { get; set; }
        public long? REMEDY_COUNT { get; set; }
        public long? NUMBER_H_N { get; set; }
        public decimal? Amount_By_Remedy_Count { get; set; }

        public Mps000062ADOMedicines() { }
        public Mps000062ADOMedicines(MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE data)
        {
            try
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<Mps000062ADOMedicines>(this, data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
