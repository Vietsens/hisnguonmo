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

namespace HIS.Desktop.Plugins.RepartitionRatioCreate.ADO
{
    public class HtcRepartitionRatioADO
    {
        public long? Id { get; set; }
        public string DepartmentCode { get; set; }

        public string TypeName { get; set; }
        public decimal? Ratio { get; set; }
        public string CreateTime { get; set; }
        public string Creator { get; set; }
        public string ModifyTime { get; set; }
        public string Modifier { get; set; }

        public long RepartitionTypeId { get; set; }
        public long? ParentId { get; set; }

        public string AdoId { get; set; }
        public string AdoParentId { get; set; }

        public bool? IsUpdate { get; set; }
    }
}
