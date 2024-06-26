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

namespace HIS.Desktop.ADO
{
    public class ExpMestViewDetailADO
    {
        public long ExpMestId { get; set; }
        public long ExpMestTypeId { get; set; }
        public long ExpMestStt { get; set; }

        /// <summary>
        /// contructor
        /// </summary>
        /// <param name="_ExpMestId">id phieu xuat</param>
        /// <param name="_ExpMestTypeId">id loai xuat</param>
        /// /// <param name="_ExpMestTypeId">id trang thai</param>
        public ExpMestViewDetailADO(long _ExpMestId, long _ExpMestTypeId, long _ExpMestStt)
        {
            try
            {
                this.ExpMestId = _ExpMestId;
                this.ExpMestTypeId = _ExpMestTypeId;
                this.ExpMestStt = _ExpMestStt;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
