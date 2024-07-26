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
    public class IcdADO : MOS.EFMODEL.DataModels.HIS_ICD
    {
        public IcdADO() { }
        public string ICD_NAME_UNSIGNED { get; set; }

        public IcdADO(MOS.EFMODEL.DataModels.HIS_ICD data)
        {
            try
            {
                //Inventec.Common.Mapper.DataObjectMapper.Map<IcdADO>(this, data);
                
                this.BYT_REPORT_CODE = data.BYT_REPORT_CODE;
                this.CHAPTER_CODE = data.CHAPTER_CODE;
                this.CHAPTER_NAME = data.CHAPTER_NAME;
                this.CHAPTER_NAME_EN = data.CHAPTER_NAME_EN;
                this.CREATE_TIME = data.CREATE_TIME;
                this.CREATOR = data.CREATOR;
                this.GROUP_CODE = data.GROUP_CODE;
                this.ICD_CHAPTER_ID = data.ICD_CHAPTER_ID;
                this.ICD_CODE = data.ICD_CODE;
                this.ICD_GROUP_ID = data.ICD_GROUP_ID;
                this.ICD_NAME = data.ICD_NAME;
                this.ICD_NAME_COMMON = data.ICD_NAME_COMMON;
                this.ICD_NAME_EN = data.ICD_NAME_EN;
                this.ID = data.ID;
                this.IS_ACTIVE = data.IS_ACTIVE;
                this.IS_DELETE = data.IS_DELETE;
                this.IS_HEIN_NDS = data.IS_HEIN_NDS;
                this.MODIFIER = data.MODIFIER;
                this.MODIFY_TIME = data.MODIFY_TIME;
                this.SUB_CODE = data.SUB_CODE;
                this.SUB_CODE_1 = data.SUB_CODE_1;
                this.SUB_CODE_2 = data.SUB_CODE_2;
                this.SUB_NAME = data.SUB_NAME;
                this.SUB_NAME_1 = data.SUB_NAME_1;
                this.SUB_NAME_1_EN = data.SUB_NAME_1_EN;
                this.SUB_NAME_2 = data.SUB_NAME_2;
                this.SUB_NAME_2_EN = data.SUB_NAME_2_EN;
                this.TYPE_CODE = data.TYPE_CODE;
                this.TYPE_NAME_EN = data.TYPE_NAME_EN;
                this.IS_REQUIRE_CAUSE = data.IS_REQUIRE_CAUSE;
                this.ICD_NAME_UNSIGNED = Inventec.Common.String.Convert.UnSignVNese2(this.ICD_NAME);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
