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

namespace HIS.Desktop.Plugins.AssignService.ADO
{
    /// <summary>
    /// Mot dong tren man hinh xac nhan phong xu ly truoc khi luu chi dinh:
    /// ten phong xu ly va so luong dich vu duoc chi dinh ve phong do trong lan chi dinh hien tai.
    /// Cac dong trung phong da duoc gop lai truoc khi dua vao day.
    /// </summary>
    public class ExecuteRoomConfirmADO
    {
        /// <summary>Phong xu ly hien thi cho nguoi dung theo dang "MA - Ten phong"</summary>
        public string EXECUTE_ROOM_DISPLAY { get; set; }

        /// <summary>So luong dich vu dang chi dinh ve phong xu ly nay</summary>
        public int SERVICE_COUNT { get; set; }

        /// <summary>Danh sach ma dich vu chi dinh ve phong nay, ngan cach bang dau phay</summary>
        public string SERVICE_CODES { get; set; }
    }
}
