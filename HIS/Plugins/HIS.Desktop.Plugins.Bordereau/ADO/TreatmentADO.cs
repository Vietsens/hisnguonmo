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
using MOS.EFMODEL.DataModels;
using AutoMapper;

namespace HIS.Desktop.Plugins.Bordereau.ADO
{
    class TreatmentADO : V_HIS_TREATMENT
    {
        public string timeTreatment { get; set; }
        public string contentShow { get; set; }
        public TreatmentADO(MOS.EFMODEL.DataModels.V_HIS_TREATMENT item)
        {
            Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_TREATMENT, TreatmentADO>();
            Mapper.Map<MOS.EFMODEL.DataModels.V_HIS_TREATMENT, TreatmentADO>(item, this);

            this.timeTreatment = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this.IN_TIME);
            contentShow = string.Format("{0} - {1}", item.TREATMENT_CODE, timeTreatment);
        }
    }
}
