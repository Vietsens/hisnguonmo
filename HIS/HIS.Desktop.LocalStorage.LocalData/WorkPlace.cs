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

namespace HIS.Desktop.LocalStorage.LocalData
{
    public class WorkPlace
    {
        public static MOS.SDO.WorkInfoSDO WorkInfoSDO { get; set; }
        private static List<MOS.SDO.WorkPlaceSDO> workPlaceSDO;
        public static List<MOS.SDO.WorkPlaceSDO> WorkPlaceSDO
        {
            get
            {
                if (workPlaceSDO == null)
                {
                    workPlaceSDO = new List<MOS.SDO.WorkPlaceSDO>();
                }
                return workPlaceSDO;
            }
            set
            {
                workPlaceSDO = value;
            }
        }

        public static MOS.SDO.WorkPlaceSDO GetWorkPlace(Inventec.Desktop.Common.Modules.Module module)
        {
            MOS.SDO.WorkPlaceSDO roomNameParam = null;
            try
            {
                if (module != null)
                    roomNameParam = (WorkPlaceSDO == null ? null : WorkPlaceSDO.Where(o => o.RoomId == module.RoomId && o.RoomTypeId == module.RoomTypeId).FirstOrDefault());
            }
            catch (Exception)
            {
            }
            return roomNameParam;
        }

        public static string GetRoomNames()
        {
            string roomNameParam = "";
            try
            {
                roomNameParam = (WorkPlaceSDO == null ? null : String.Join(";", WorkPlaceSDO.Select(o => o.RoomName + " - " + o.DepartmentName).ToArray()));
            }
            catch (Exception)
            {
            }
            return roomNameParam;
        }

        public static string GetRoomName()
        {
            string roomNameParam = "";
            try
            {
                roomNameParam = (WorkPlaceSDO == null ? null : WorkPlaceSDO.Select(o => o.RoomName).FirstOrDefault());
            }
            catch (Exception)
            {
            }
            return roomNameParam;
        }

        public static List<long> GetDepartmentIds()
        {
            List<long> departmentIdParam = null;
            try
            {
                departmentIdParam = (WorkPlaceSDO == null ? null : (WorkPlaceSDO.Select(o => o.DepartmentId).ToList()));
            }
            catch (Exception)
            {
            }
            return departmentIdParam;
        }

        public static long GetDepartmentId(long roomTypeId)
        {
            long departmentIdParam = 0;
            try
            {
                departmentIdParam = (WorkPlaceSDO == null ? 0 : (WorkPlaceSDO.FirstOrDefault(o => o.RoomTypeId == roomTypeId).DepartmentId));
            }
            catch (Exception)
            {
            }
            return departmentIdParam;
        }

        public static long GetDepartmentId()
        {
            long departmentIdParam = 0;
            try
            {
                departmentIdParam = (WorkPlaceSDO == null ? 0 : (WorkPlaceSDO.FirstOrDefault().DepartmentId));
            }
            catch (Exception)
            {
            }
            return departmentIdParam;
        }

        public static string GetDepartmentName()
        {
            string departmentNameParam = null;
            try
            {
                departmentNameParam = (WorkPlaceSDO == null ? null : (WorkPlaceSDO.Select(o => o.DepartmentName).FirstOrDefault()));
            }
            catch (Exception)
            {
            }
            return departmentNameParam;
        }

        public static List<string> GetDepartmentNames()
        {
            List<string> departmentNameParam = null;
            try
            {
                departmentNameParam = (WorkPlaceSDO == null ? null : (WorkPlaceSDO.Select(o => o.DepartmentName).ToList()));
            }
            catch (Exception)
            {
            }
            return departmentNameParam;
        }

        public static List<long> GetRoomIds()
        {
            List<long> roomIdParam = null;
            try
            {
                roomIdParam = (WorkPlaceSDO == null ? null : (WorkPlaceSDO.Select(o => o.RoomId).ToList()));
            }
            catch (Exception)
            {
            }
            return roomIdParam;
        }

        public static long GetRoomId(long roomTypeId)
        {
            long roomIdParam = 0;
            try
            {
                roomIdParam = (WorkPlaceSDO == null ? 0 : (WorkPlaceSDO.FirstOrDefault(o => o.RoomTypeId == roomTypeId).RoomId));
            }
            catch (Exception)
            {
            }
            return roomIdParam;
        }

        public static long GetRoomId()
        {
            long roomIdParam = 0;
            try
            {
                roomIdParam = (WorkPlaceSDO == null ? 0 : (WorkPlaceSDO.FirstOrDefault().RoomId));
            }
            catch (Exception)
            {
            }
            return roomIdParam;
        }

        public static List<long> GetRoomTypeIds()
        {
            List<long> romTypeIdParam = null;
            try
            {
                romTypeIdParam = (WorkPlaceSDO == null ? null : (WorkPlaceSDO.Select(o => o.RoomTypeId).ToList()));
            }
            catch (Exception)
            {
            }
            return romTypeIdParam;
        }

        public static long GetBranchId()
        {
            long branchIdParam = 0;
            try
            {
                branchIdParam = (WorkPlaceSDO == null ? 0 : (WorkPlaceSDO.FirstOrDefault().BranchId));
            }
            catch (Exception)
            {
            }
            return branchIdParam;
        }

        public static string GetBranchName()
        {
            string branchNameParam = "";
            try
            {
                branchNameParam = (WorkPlaceSDO == null ? "" : WorkPlaceSDO.Select(o => o.BranchName).FirstOrDefault());
            }
            catch (Exception)
            {
            }
            return branchNameParam;
        }
    }
}
