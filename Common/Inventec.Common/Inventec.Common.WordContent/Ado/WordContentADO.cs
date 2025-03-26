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

namespace Inventec.Common.WordContent
{
    public class WordContentADO
    {
        public WordContentADO() { }
        public string FileName { get; set; }
        public string TemplateFileName { get; set; }
        public Dictionary<string, object> TemplateKey { get; set; }
        public Inventec.Common.SignLibrary.ADO.InputADO EmrInputADO { get; set; }
        public Action<SAR.EFMODEL.DataModels.SAR_PRINT> ActUpdateReference { get; set; }
        public SAR.EFMODEL.DataModels.SAR_PRINT OldSarPrint { get; set; }
        public SAR.EFMODEL.DataModels.SAR_PRINT_TYPE SarPrintType { get; set; }
        public bool? IsViewOnly { get; set; }
    }
}
