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
//using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisMachineImport.ADO
{
    class MachineImportADO : HIS_MACHINE
    {
        private static string ToDdMmYyyy(long? hisTime)
        {
            try
            {
                if (!hisTime.HasValue || hisTime.Value <= 0)
                    return "";

                var str = hisTime.Value.ToString();
                if (str.Length != 8 && str.Length != 14)
                    return "";

                // HIS time number: yyyyMMdd or yyyyMMddHHmmss
                int year = int.Parse(str.Substring(0, 4));
                int month = int.Parse(str.Substring(4, 2));
                int day = int.Parse(str.Substring(6, 2));
                var dt = new DateTime(year, month, day);
                return dt.ToString("dd/MM/yyyy");
            }
            catch
            {
                return "";
            }
        }

        public string ROOM_CODE { get; set; }
        public string ROOM_CODES { get; set; }
        public long ROOM_TYPE_ID { get; set; }
        //public string ROOM_TYPE_CODE { get; set; }
        public string ERROR { get; set; }

        public string CONTRACT_FROM_DMY { get { return ToDdMmYyyy(this.CONTRACT_FROM); } }
        public string CONTRACT_TO_DMY { get { return ToDdMmYyyy(this.CONTRACT_TO); } }
        public string FROM_TIME_DMY { get { return ToDdMmYyyy(this.FROM_TIME); } }
        public string TO_TIME_DMY { get { return ToDdMmYyyy(this.TO_TIME); } }
    }
}
