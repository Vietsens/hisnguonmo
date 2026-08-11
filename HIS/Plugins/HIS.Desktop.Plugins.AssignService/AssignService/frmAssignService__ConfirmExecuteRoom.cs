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
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.Plugins.AssignService.ADO;
using HIS.Desktop.Plugins.AssignService.Config;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.AssignService.AssignService
{
    public partial class frmAssignService : HIS.Desktop.Utility.FormBase
    {
        /// <summary>
        /// Hien thi man hinh xac nhan danh sach phong xu ly truoc khi luu chi dinh.
        /// Chi chay khi cau hinh HIS.Desktop.Plugins.AssignService.ConfirmExecuteRoomWhenSave = 1.
        /// Chi mang tinh xac nhan: khong kiem tra tinh hop le giua dich vu va phong xu ly,
        /// khong thay doi logic gan phong xu ly mac dinh.
        /// </summary>
        /// <param name="serviceCheckeds__Send">Cac dong dich vu dang duoc chon de luu trong lan chi dinh hien tai</param>
        /// <returns>true: duoc phep tiep tuc luu. false: bac si chon Khong dong y, dung thao tac luu</returns>
        private bool ConfirmExecuteRoomBeforeSave(List<SereServADO> serviceCheckeds__Send)
        {
            try
            {
                if (!HisConfigCFG.IsConfirmExecuteRoomWhenSave) return true;
                if (serviceCheckeds__Send == null || !serviceCheckeds__Send.Any()) return true;

                Dictionary<long, string> executeRoomDisplays = GetExecuteRoomDisplayDictionary();
                Dictionary<long, ExecuteRoomConfirmADO> dicRoomConfirm = new Dictionary<long, ExecuteRoomConfirmADO>();
                Dictionary<long, List<string>> dicServiceCodeByRoom = new Dictionary<long, List<string>>();
                List<ExecuteRoomConfirmADO> roomConfirms = new List<ExecuteRoomConfirmADO>();
                List<string> serviceNamesWithoutRoom = new List<string>();

                foreach (var item in serviceCheckeds__Send)
                {
                    if (item == null) continue;

                    long executeRoomId = Convert.ToInt64(item.TDL_EXECUTE_ROOM_ID);
                    if (executeRoomId <= 0)
                    {
                        serviceNamesWithoutRoom.Add(BuildServiceDisplay(item));
                        continue;
                    }

                    ExecuteRoomConfirmADO roomConfirm = null;
                    if (dicRoomConfirm.TryGetValue(executeRoomId, out roomConfirm))
                    {
                        // Gop cac dong trung phong: tang so luong dich vu + bo sung ma dich vu
                        roomConfirm.SERVICE_COUNT++;
                        AddServiceCode(dicServiceCodeByRoom, executeRoomId, item.TDL_SERVICE_CODE);
                        continue;
                    }

                    string executeRoomDisplay = null;
                    if (!executeRoomDisplays.TryGetValue(executeRoomId, out executeRoomDisplay)) executeRoomDisplay = "";

                    roomConfirm = new ExecuteRoomConfirmADO()
                    {
                        EXECUTE_ROOM_DISPLAY = executeRoomDisplay,
                        SERVICE_COUNT = 1
                    };
                    dicRoomConfirm.Add(executeRoomId, roomConfirm);
                    roomConfirms.Add(roomConfirm);
                    AddServiceCode(dicServiceCodeByRoom, executeRoomId, item.TDL_SERVICE_CODE);
                }

                // Ghep ma dich vu cua tung phong sau khi da gom xong
                foreach (var roomEntry in dicRoomConfirm)
                {
                    List<string> serviceCodes = null;
                    if (dicServiceCodeByRoom.TryGetValue(roomEntry.Key, out serviceCodes) && serviceCodes != null)
                    {
                        roomEntry.Value.SERVICE_CODES = String.Join(", ", serviceCodes);
                    }
                }

                // Vien bat co che he thong tu phan phong xu ly: cot Phong xu ly de trong la dung thiet ke,
                // khong liet ke de tranh canh bao nhieu moi lan luu.
                if (HisConfigCFG.IsAssignRoomByLoadBalance) serviceNamesWithoutRoom.Clear();

                if (roomConfirms.Count == 0 && serviceNamesWithoutRoom.Count == 0) return true;

                roomConfirms = roomConfirms
                    .OrderByDescending(o => o.SERVICE_COUNT)
                    .ThenBy(o => o.EXECUTE_ROOM_DISPLAY)
                    .ToList();

                bool isAgreed = false;
                using (frmConfirmExecuteRoom frmConfirm = new frmConfirmExecuteRoom(this.currentModule, roomConfirms, serviceNamesWithoutRoom))
                {
                    frmConfirm.ShowDialog(this);
                    isAgreed = frmConfirm.IsAgreed;
                }

                Inventec.Common.Logging.LogSystem.Info("ConfirmExecuteRoomBeforeSave____IsAgreed:" + isAgreed);
                return isAgreed;
            }
            catch (Exception ex)
            {
                // Loi khi hien thi xac nhan khong duoc chan luong luu hien tai
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return true;
        }

        /// <summary>
        /// Gom ma dich vu theo tung phong xu ly, giu nguyen thu tu dong tren luoi.
        /// Khong loai trung de so ma khop voi cot So dich vu (1 dich vu chi dinh nhieu lan = nhieu dong).
        /// </summary>
        private static void AddServiceCode(Dictionary<long, List<string>> dicServiceCodeByRoom, long executeRoomId, string serviceCode)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(serviceCode)) return;

                List<string> serviceCodes = null;
                if (!dicServiceCodeByRoom.TryGetValue(executeRoomId, out serviceCodes))
                {
                    serviceCodes = new List<string>();
                    dicServiceCodeByRoom.Add(executeRoomId, serviceCodes);
                }
                serviceCodes.Add(serviceCode);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Chuoi hien thi 1 dich vu chua chon phong xu ly, dang "MA - Ten dich vu".
        /// </summary>
        private static string BuildServiceDisplay(SereServADO sereServ)
        {
            string result = "";
            try
            {
                result = sereServ.TDL_SERVICE_NAME;
                if (!String.IsNullOrWhiteSpace(sereServ.TDL_SERVICE_CODE))
                {
                    result = String.IsNullOrWhiteSpace(result)
                        ? sereServ.TDL_SERVICE_CODE
                        : sereServ.TDL_SERVICE_CODE + " - " + sereServ.TDL_SERVICE_NAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>
        /// Lay chuoi hien thi phong xu ly theo ma phong, dang "MA - Ten phong".
        /// Uu tien nguon dang gan vao cot Phong xu ly tren luoi de thong tin trung voi cai bac si nhin thay
        /// (nguon nay da bao gom ca buong benh duoc bo sung khi dieu tri tai buong).
        /// </summary>
        private Dictionary<long, string> GetExecuteRoomDisplayDictionary()
        {
            Dictionary<long, string> result = new Dictionary<long, string>();
            try
            {
                List<V_HIS_EXECUTE_ROOM> sources = new List<V_HIS_EXECUTE_ROOM>();

                var gridSource = this.repositoryItemcboExcuteRoom_TabService != null
                    ? this.repositoryItemcboExcuteRoom_TabService.DataSource as IEnumerable<V_HIS_EXECUTE_ROOM>
                    : null;
                if (gridSource != null) sources.AddRange(gridSource);
                if (this.allDataExecuteRooms != null) sources.AddRange(this.allDataExecuteRooms);

                foreach (var room in sources)
                {
                    if (room == null) continue;

                    long roomId = Convert.ToInt64(room.ROOM_ID);
                    if (roomId <= 0 || result.ContainsKey(roomId)) continue;

                    string display = room.EXECUTE_ROOM_NAME;
                    if (!String.IsNullOrWhiteSpace(room.EXECUTE_ROOM_CODE))
                    {
                        display = String.IsNullOrWhiteSpace(display)
                            ? room.EXECUTE_ROOM_CODE
                            : room.EXECUTE_ROOM_CODE + " - " + room.EXECUTE_ROOM_NAME;
                    }

                    result.Add(roomId, display);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
    }
}
