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

namespace MPS.Processor.Mps000485
{
    public class SevereIllInfoADO : MOS.EFMODEL.DataModels.HIS_SEVERE_ILLNESS_INFO
    {
        public string ICD_NAME_MOTHER_SEVERE_ILLNESS_INFO { get; set; }
        public string ICD_CODE_MOTHER_SEVERE_ILLNESS_INFO { get; set; }
        public string ICD_NAME_CONCLU_SEVERE_ILLNESS_INFO { get; set; }
        public string ICD_CODE_CONCLU_SEVERE_ILLNESS_INFO { get; set; }
        public SevereIllInfoADO(MOS.EFMODEL.DataModels.HIS_SEVERE_ILLNESS_INFO data)
        {
            try
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<SevereIllInfoADO>(this, data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
