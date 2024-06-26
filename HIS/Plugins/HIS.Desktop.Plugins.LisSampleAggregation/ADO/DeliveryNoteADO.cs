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
using LIS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.LisSampleAggregation.ADO
{
    public class DeliveryNoteADO : LIS_DELIVERY_NOTE
    {
        public string RECEIVING_TIME_ForDisplay { get; set; }

        public DeliveryNoteADO()
        { }

        public DeliveryNoteADO(LIS_DELIVERY_NOTE data)
        {
            try
            {
                if (data != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<DeliveryNoteADO>(this, data);
                    if (data.RECEIVING_TIME != null)
                    {
                        this.RECEIVING_TIME_ForDisplay = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.RECEIVING_TIME ?? 0);
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
