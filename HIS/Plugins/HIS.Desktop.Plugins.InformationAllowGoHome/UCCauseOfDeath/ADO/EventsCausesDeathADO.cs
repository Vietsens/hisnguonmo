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

namespace HIS.Desktop.Plugins.InformationAllowGoHome.UCCauseOfDeath.ADO
{
    public class EventsCausesDeathADO : HIS_EVENTS_CAUSES_DEATH
    {
        public int actionType { get; set; }
        public string ICD_NAME { get; set; }
        public DateTime? Date { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
        public EventsCausesDeathADO() { }
        public EventsCausesDeathADO(HIS_EVENTS_CAUSES_DEATH data) {
            try
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<EventsCausesDeathADO>(this, data);
                this.actionType = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                this.Date = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.HAPPEN_TIME ?? 0);
                this.SERVICE_UNIT_NAME = data.UNIT_NAME;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
