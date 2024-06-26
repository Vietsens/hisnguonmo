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

namespace HIS.Desktop.Plugins.HisIcdImport.ADO
{
    public class IcdADO : MOS.EFMODEL.DataModels.HIS_ICD
    {
        public string ERROR { get; set; }
        public string ICD_GROUP_CODE { get; set; }
        public string ICD_GROUP_NAME { get; set; }
        public string MAX_CAPACITY_STR { get; set; }

        public string IS_CAUSE_STR { get; set; }

        public string IS_HEIN_NDS_STR { get; set; }

        public string IS_REQUIRE_CAUSE_STR { get; set; }

        public string IS_TRADITIONAL_STR { get; set; }

        public string GENDER_CODE_STR { get; set; }
        public string GENDER_NAME_STR { get; set; }
        public string ATTACH_ICD_CODES_STR { get; set; }
        public string ATTACH_ICD_NAMES_STR { get; set; }
        public string AGE_FROM_STR { get; set; }
        public string AGE_FROM_DISPLAY_STR { get; set; }
        public string AGE_TO_STR { get; set; }
        public string AGE_TO_DISPLAY_STR { get; set; }
        public string AGE_TYPE_CODE_STR { get; set; }
        public string AGE_TYPE_NAME_STR { get; set; }
        public string IS_SUBCODE_STR { get; set; }
        public string IS_SWORD_STR { get; set; }
        public string IS_COVID_STR { get; set; }

        
    }
}
