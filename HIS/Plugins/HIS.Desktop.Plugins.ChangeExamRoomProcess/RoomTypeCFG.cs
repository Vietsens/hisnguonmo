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
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.LocalStorage.SdaConfig;
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ChangeExamRoomProcess
{   
   public class RoomTypeCFG
    {

        private const string ROOM_TYPE_EXECUTE = "DBCODE.HIS_RS.HIS_ROOM_TYPE.ROOM_TYPE_CODE.EXECUTE";//Phòng xử lý dịch vụ

        private static long RoomTypeId;
        public static long ROOM_TYPE
        {
            get
            {
                if (RoomTypeId == 0)
                {
                    RoomTypeId = GetId(SdaConfigs.Get<string>(ROOM_TYPE_EXECUTE));
                }
                return RoomTypeId;
            }
            set
            {
                RoomTypeId = value;
            }
        }



        private static long GetId(string code)
        {
            long result = 0;
            try
            {
                var data = BackendDataWorker.Get<HIS_ROOM_TYPE>().FirstOrDefault(o => o.ROOM_TYPE_CODE == code);
                if (!(data != null && data.ID > 0)) throw new ArgumentNullException(code + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => code), code));
                result = data.ID;
            }
            catch (Exception ex)
            {
                LogSystem.Debug(ex);
                result = 0;
            }
            return result;
        }
    }
}
