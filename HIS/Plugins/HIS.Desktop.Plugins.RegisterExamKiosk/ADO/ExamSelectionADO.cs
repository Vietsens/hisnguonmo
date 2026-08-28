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

namespace HIS.Desktop.Plugins.RegisterExamKiosk.ADO
{
    /// <summary>
    /// Mot cong kham nguoi benh da chon tai kiosk: mot phong kham va mot dich vu kham cua phong do.
    /// Nguoi benh co the chon nhieu cong kham cho cung mot luot dang ky.
    /// </summary>
    public class ExamSelectionADO
    {
        public ExamSelectionADO()
        {
        }

        public ExamSelectionADO(V_HIS_EXECUTE_ROOM_1 executeRoom, V_HIS_SERVICE service)
        {
            this.ExecuteRoom = executeRoom;
            this.Service = service;
        }

        /// <summary>Phong kham nguoi benh da chon</summary>
        public V_HIS_EXECUTE_ROOM_1 ExecuteRoom { get; set; }

        /// <summary>Dich vu kham cua phong da chon</summary>
        public V_HIS_SERVICE Service { get; set; }

        public long RoomId
        {
            get { return this.ExecuteRoom != null ? this.ExecuteRoom.ROOM_ID : 0; }
        }

        public long ServiceId
        {
            get { return this.Service != null ? this.Service.ID : 0; }
        }

        public string RoomName
        {
            get { return this.ExecuteRoom != null ? this.ExecuteRoom.EXECUTE_ROOM_NAME : ""; }
        }

        public string ServiceName
        {
            get { return this.Service != null ? this.Service.SERVICE_NAME : ""; }
        }
    }
}
