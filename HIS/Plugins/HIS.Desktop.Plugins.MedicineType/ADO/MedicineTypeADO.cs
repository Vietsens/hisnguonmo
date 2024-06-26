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
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.MedicineType.ADO
{
    public class MedicineTypeADO : V_HIS_MEDICINE_TYPE
    {
        public bool IsAddictive { get; set; }
        public bool IsAntibiotic { get; set; }
        public bool IsNeurological { get; set; }
        public bool IsStopImp { get; set; }
        public bool IsFood { get; set; }
        public bool IsCPNG { get; set; }
        public bool IsAutoExpend { get; set; }
        public bool? IsMustPrepare { get; set; }

        public MedicineTypeADO(V_HIS_MEDICINE_TYPE _data)
        {
            try
            {
                if (_data != null)
                {

                    System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get<V_HIS_MEDICINE_TYPE>();

                    foreach (var item in pi)
                    {
                        item.SetValue(this, (item.GetValue(_data)));
                    }
                }

            }

            catch (Exception)
            {

            }
        }
    }
}
