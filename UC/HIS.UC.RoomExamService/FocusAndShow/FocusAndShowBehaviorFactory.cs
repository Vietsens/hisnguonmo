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

namespace HIS.UC.RoomExamService.FocusAndShow
{
    class FocusAndShowBehaviorFactory
    {
        internal static IFocusAndShow MakeIFocusAndShow(object data)
        {
            IFocusAndShow result = null;
            try
            {
                if (data is UCRoomExamService)
                {
                    result = new FocusAndShowBehavior((UCRoomExamService)data);
                }
                else if (data is UCRoomExamService1)
                {
                    result = new FocusAndShow11Behavior((UCRoomExamService1)data);
                }
                else if (data is UCRoomExamService2)
                {
                    result = new FocusAndShow01Behavior((UCRoomExamService2)data);
                }
                if (result == null) throw new NullReferenceException();
            }
            catch (NullReferenceException ex)
            {
                Inventec.Common.Logging.LogSystem.Error("Factory khong khoi tao duoc doi tuong." + data.GetType().ToString() + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data), ex);
                result = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }
    }
}
