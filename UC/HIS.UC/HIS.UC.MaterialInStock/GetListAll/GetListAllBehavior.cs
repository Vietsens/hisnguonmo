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
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.UC.MaterialInStock.GetListAll
{
    class GetListAllBehavior : IGetListAll
    {
        UserControl entity;

        internal GetListAllBehavior(UserControl control)
            : base()
        {
            this.entity = control;
        }

        List<HisMaterialInStockSDO> IGetListAll.Run()
        {
            List<HisMaterialInStockSDO> result = null;
            try
            {
                if (this.entity.GetType() == typeof(HIS.UC.HisMaterialInStock.Run.UCHisMaterialInStock))
                {
                    result = ((HIS.UC.HisMaterialInStock.Run.UCHisMaterialInStock)this.entity).GetListAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }
    }
}
