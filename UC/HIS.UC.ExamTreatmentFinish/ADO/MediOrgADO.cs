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

namespace HIS.UC.ExamTreatmentFinish.ADO
{
    class MediOrgADO : HIS_MEDI_ORG
    {
        public string MEDI_ORG_NAME_UNSIGN { get; set; }

        public MediOrgADO(HIS_MEDI_ORG data)
        {
            Inventec.Common.Mapper.DataObjectMapper.Map<MediOrgADO>(this, data);
            this.MEDI_ORG_NAME_UNSIGN = Inventec.Common.String.Convert.UnSignVNese(data.MEDI_ORG_NAME);
        }
    }
}
