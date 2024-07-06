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

namespace His.UC.UCHein.Data
{
    public class MediOrgADO : MOS.EFMODEL.DataModels.HIS_MEDI_ORG
    {
        public MediOrgADO() { }
        public string MEDI_ORG_NAME_UNSIGNED { get; set; }

        public MediOrgADO(MOS.EFMODEL.DataModels.HIS_MEDI_ORG data)
        {
            try
            {
                //Inventec.Common.Mapper.DataObjectMapper.Map<MediOrgADO>(this, data);

                this.ADDRESS = data.ADDRESS;
                this.CREATE_TIME = data.CREATE_TIME;
                this.CREATOR = data.CREATOR;
                this.GROUP_CODE = data.GROUP_CODE;
                this.ID = data.ID;
                this.IS_ACTIVE = data.IS_ACTIVE;
                this.IS_DELETE = data.IS_DELETE;
                this.LEVEL_CODE = data.LEVEL_CODE;
                this.MEDI_ORG_CODE = data.MEDI_ORG_CODE;
                this.MEDI_ORG_NAME = data.MEDI_ORG_NAME;
                this.MODIFIER = data.MODIFIER;
                this.MODIFY_TIME = data.MODIFY_TIME;
                this.PROVINCE_CODE = data.PROVINCE_CODE;
                this.RANK_CODE = data.RANK_CODE;

                this.MEDI_ORG_NAME_UNSIGNED = Inventec.Common.String.Convert.UnSignVNese2(this.MEDI_ORG_NAME);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
