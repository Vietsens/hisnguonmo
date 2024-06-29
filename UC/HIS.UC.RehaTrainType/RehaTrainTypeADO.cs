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

namespace HIS.UC.RehaTrainType
{
    public class RehaTrainTypeADO : MOS.EFMODEL.DataModels.V_HIS_REHA_TRAIN_TYPE
    {
        public RehaTrainTypeADO() { }
        public RehaTrainTypeADO(V_HIS_REHA_TRAIN_TYPE data)
        {
            if (data != null)
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<RehaTrainTypeADO>(this, data);
            }
        }
        public bool checkRehaTrainType { get; set; }
        public bool isKeyChooseRehaTrainType { get; set; }
        public bool radioRehaTrainType { get; set; }
    }
}
