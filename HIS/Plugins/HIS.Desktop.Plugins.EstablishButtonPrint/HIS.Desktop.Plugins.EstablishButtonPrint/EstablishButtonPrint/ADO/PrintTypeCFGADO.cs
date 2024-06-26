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

namespace HIS.Desktop.Plugins.EstablishButtonPrint.EstablishButtonPrint.ADO
{
    public class PrintTypeCFGADO
    {
        public short? IS_ACTIVE { get; set; }
        public string APP_CODE { get; set; }
        public string MODULE_LINK { get; set; }
        public string CONTROL_PATH { get; set; }
        public string CONTROL_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
        public PrintTypeIDCaptionADO PRINT_TYPE_CAPTION { get; set; }
        public List<PrintTypeIDCaptionADO> PRINT_TYPE_CAPTIONs { get; set; }
        public string CREATOR { get; set; }
        public long? CREATE_TIME { get; set; }
        public string MODIFIER { get; set; }
        public long? MODIFY_TIME { get; set; }

    }
}
