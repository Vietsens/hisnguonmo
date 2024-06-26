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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.PackingMaterial.ADO
{
    public class ExpImpMestADO
    {
        public string TYPE_NAME { get; set; }
        public string ExpImpCode { get; set; }

        public ExpImpMestADO() { }

        public ExpImpMestADO(HIS_EXP_MEST expMest)
        {
            this.TYPE_NAME = "Phiếu xuất";
            this.ExpImpCode = expMest.EXP_MEST_CODE;
        }

        public ExpImpMestADO(HIS_IMP_MEST impMest)
        {
            this.TYPE_NAME = "Phiếu nhập";
            this.ExpImpCode = impMest.IMP_MEST_CODE;
        }
    }
}
