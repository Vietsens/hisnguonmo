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

namespace HIS.UC.SecondaryIcd.ADO
{
    public class IcdADO : MOS.EFMODEL.DataModels.V_HIS_ICD
    {
        public IcdADO(MOS.EFMODEL.DataModels.HIS_ICD icd, string[] icdCodes)
        {
            try
            {
                if (icd != null)
                {
                    this.ICD_CODE = icd.ICD_CODE;
                    this.ICD_CHAPTER_ID = icd.ICD_CHAPTER_ID;
                    this.ICD_GROUP_ID = icd.ICD_GROUP_ID;
                    this.ICD_NAME = icd.ICD_NAME;
                    this.ICD_NAME_COMMON = icd.ICD_NAME_COMMON;
                    this.ICD_NAME_EN = icd.ICD_NAME_EN;
                    this.ID = icd.ID;
                    this.IS_HEIN_NDS = icd.IS_HEIN_NDS;
                    if (icdCodes != null && icdCodes.Count() > 0 && icdCodes.Contains(this.ICD_CODE))
                    {
                        this.IsChecked = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public IcdADO(MOS.EFMODEL.DataModels.V_HIS_ICD icd, string[] icdCodes)
        {
            try
            {
                if (icd != null)
                {
                    this.ICD_CODE = icd.ICD_CODE;
                    this.ICD_CHAPTER_ID = icd.ICD_CHAPTER_ID;
                    this.ICD_GROUP_ID = icd.ICD_GROUP_ID;
                    this.ICD_NAME = icd.ICD_NAME;
                    this.ICD_NAME_COMMON = icd.ICD_NAME_COMMON;
                    this.ICD_NAME_EN = icd.ICD_NAME_EN;
                    this.ID = icd.ID;
                    this.IS_HEIN_NDS = icd.IS_HEIN_NDS;
                    this.ICD_GROUP_NAME = icd.ICD_GROUP_NAME;
                    if (icdCodes != null && icdCodes.Count() > 0 && icdCodes.Contains(this.ICD_CODE))
                    {
                        this.IsChecked = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public bool IsChecked { get; set; }
    }
}
