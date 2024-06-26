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
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ConfigPrinter.ADO
{
    public class PrintTypeADO : SAR_PRINT_TYPE
    {
        public string PRINTER_NAME { get; set; }
        public int Action { get; set; }
        public long? PRINT_TYPE_ID { get; set; }

        public PrintTypeADO() { }

        public PrintTypeADO(SAR_PRINT_TYPE type)
        {
            try
            {
                if (type != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<PrintTypeADO>(this, type);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
