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
using HIS.UC.RoomExamService.ADO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.RoomExamService.Run
{
    class RunBehavior : IRun
    {
        RoomExamServiceInitADO entity;

        internal RunBehavior(RoomExamServiceInitADO data)
        {
            this.entity = data;
        }

        object IRun.Run()
        {
            object result = null;
            switch (entity.TemplateDesign)
            {
                case TemplateDesign.T01:
                    result = new UCRoomExamService2(entity);
                    break;
                case TemplateDesign.T11:
                    result = new UCRoomExamService(entity);
                    break;
                case TemplateDesign.T20:
                    result = new UCRoomExamService1(entity);
                    break;
                default:
                    result = new UCRoomExamService(entity);
                    break;
            }
            return result;
        }
    }
}
