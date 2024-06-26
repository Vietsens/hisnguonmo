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

namespace HIS.Desktop.Plugins.SecondaryIcd.ADO
{
    public class IcdADO : MOS.EFMODEL.DataModels.HIS_ICD
    {
        public bool IsChecked { get; set; }

        public IcdADO(MOS.EFMODEL.DataModels.HIS_ICD icd, string icdCodes)
        {
            try
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<IcdADO>(this, icd);
                if (!String.IsNullOrEmpty(icdCodes) && icdCodes.Contains(SecondaryIcdUtil.AddSeperateToKey(this.ICD_CODE)))
                {
                    this.IsChecked = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public IcdADO(MOS.EFMODEL.DataModels.HIS_ICD_CM icd, string icdCodes)
        {
            try
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<IcdADO>(this, icd);
                this.ICD_CODE = icd.ICD_CM_CODE;
                this.ICD_NAME = icd.ICD_CM_NAME;
                this.CHAPTER_CODE = icd.ICD_CM_CHAPTER_CODE;
                this.CHAPTER_NAME = icd.ICD_CM_CHAPTER_NAME;
                this.GROUP_CODE = icd.ICD_CM_GROUP_CODE;
                //this.GROUP_NAME = icd.ICD_CM_GROUP_NAME;

                if (!String.IsNullOrEmpty(icdCodes) && icdCodes.Contains(SecondaryIcdUtil.AddSeperateToKey(this.ICD_CODE)))
                {
                    this.IsChecked = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
