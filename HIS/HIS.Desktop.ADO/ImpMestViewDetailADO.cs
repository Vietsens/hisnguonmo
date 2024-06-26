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
    public class ImpMestViewDetailADO
    {
        public long ImpMestId { get; set; }
        public long ImpMestSttId { get; set; }
        public long IMP_MEST_TYPE_ID { get; set; }

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="_ImpMestId">Id phieu nhap</param>
        /// <param name="_IMP_MEST_TYPE_ID">id loai nhap</param>
        /// /// <param name="_ImpMestSttId">id trang thai nhap</param>
        public ImpMestViewDetailADO(long _ImpMestId, long _IMP_MEST_TYPE_ID, long _ImpMestSttId)
        {
            try
            {
                this.ImpMestId = _ImpMestId;
                this.ImpMestSttId = _ImpMestSttId;
                this.IMP_MEST_TYPE_ID = _IMP_MEST_TYPE_ID;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
