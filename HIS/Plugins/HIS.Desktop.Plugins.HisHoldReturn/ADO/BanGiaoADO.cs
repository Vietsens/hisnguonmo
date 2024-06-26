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

namespace HIS.Desktop.Plugins.HisHoldReturn.ADO
{
    internal class BanGiaoADO
    {
        internal BanGiaoADO()
        {
        }
        public long Value { get; set; }
        public string Text { get; set; }

        internal List<BanGiaoADO> BanGiaoADOs
        {
            get
            {
                List<BanGiaoADO> rs = new List<BanGiaoADO>();
                rs.Add(new BanGiaoADO() { Value = 1, Text = "Tất cả" });
                rs.Add(new BanGiaoADO() { Value = 2, Text = "Đang bàn giao" });
                rs.Add(new BanGiaoADO() { Value = 3, Text = "Đã bàn giao" });
                rs.Add(new BanGiaoADO() { Value = 4, Text = "Chưa trả" });
                rs.Add(new BanGiaoADO() { Value = 5, Text = "Đã trả" });
                return rs;
            }
        }

    }
}
