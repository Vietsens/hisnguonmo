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
using MPS.Processor.Mps000028.PDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000028
{
    class SereServQrADO : SereServADO
    {
        public byte[] QrCT540 { get; set; }
        public byte[] AccessNo { get; set; }

        public SereServQrADO(SereServADO data)
        {
            if (data != null)
            {
                System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get<SereServADO>();
                foreach (var item in pi)
                {
                    item.SetValue(this, (item.GetValue(data)));
                }
            }
        }
    }
}
