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


namespace HIS.Desktop.Plugins.DebateDiagnostic.ADO
{
    class HisDebateADO : HIS_DEBATE
    {
        public string HisDebateTimeString { get; set; }
        public string ContentTypeName { get; set; }
        public HisDebateADO(MOS.EFMODEL.DataModels.HIS_DEBATE item)
        {
            Mapper.CreateMap<MOS.EFMODEL.DataModels.HIS_DEBATE, HisDebateADO>();
            Mapper.Map<MOS.EFMODEL.DataModels.HIS_DEBATE, HisDebateADO>(item, this);
            if (this.CONTENT_TYPE == 1)
            {
                ContentTypeName = "Hội chẩn khác";
            }
            else if (this.CONTENT_TYPE == 2)
            {
                ContentTypeName = "Hội chẩn thuốc";
            }
            else
            {
                ContentTypeName = "Hội chẩn trước phẫu thuật";
            }
            this.HisDebateTimeString = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this.DEBATE_TIME ?? 0);
        }
    }
}
