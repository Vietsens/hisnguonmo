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

namespace HIS.Desktop.Plugins.TreatmentHistory.ADO
{
    /// <summary>
    /// Mở rộng V_HIS_SERE_SERV_7 thêm NOTE_ADO để cây Grid 3 (TreeSereServ7) hiển thị
    /// cột "Ghi chú / Kết quả" = ghi chú y lệnh (INSTRUCTION_NOTE).
    /// V_HIS_SERE_SERV_7 không có field ghi chú nên cột (FieldName "NOTE_ADO") luôn trống;
    /// ADO này bổ sung property NOTE_ADO để UC map sang được.
    /// </summary>
    public class SereServ7MergeADO : V_HIS_SERE_SERV_7
    {
        /// <summary>Ghi chú y lệnh (lấy từ DHisSereServ2.INSTRUCTION_NOTE).</summary>
        public string NOTE_ADO { get; set; }
    }
}
