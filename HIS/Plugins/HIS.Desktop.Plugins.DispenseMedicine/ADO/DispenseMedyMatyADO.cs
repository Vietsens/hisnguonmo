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

namespace HIS.Desktop.Plugins.DispenseMedicine.ADO
{
    public class DispenseMedyMatyADO
    {
        public decimal? OldAmount { get; set; }
        public decimal Amount { get; set; }
        public decimal? CFGAmount { get; set; }
        public long PreparationMediMatyTypeId { get; set; }
        public string PreparationMediMatyTypeName { get; set; }
        public long ServiceTypeId { get; set; }
        public string ServiceUnitName { get; set; }
        public bool IsNotAvaliable { get; set; }
        public decimal? ProductAmount { get; set; }

        public DispenseMedyMatyADO(DispenseMedyMatyADO r)
        {
            try
            {
                System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get<DispenseMedyMatyADO>();
                foreach (var item in pi)
                {
                    item.SetValue(this, (item.GetValue(r)));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public DispenseMedyMatyADO()
        { 
        }
    }
}
