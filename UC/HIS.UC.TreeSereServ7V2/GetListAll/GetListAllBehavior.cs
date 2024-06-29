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
using HIS.UC.TreeSereServ7V2.Run;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.UC.TreeSereServ7V2.GetListAll
{
    class GetListAllBehavior : IGetListAll
    {
        UserControl entity;

        internal GetListAllBehavior(UserControl control)
            : base()
        {
            this.entity = control;
        }

        List<SereServADO> IGetListAll.Run()
        {
            List<SereServADO> result = null;
            try
            {
                if (this.entity.GetType() == typeof(UCTreeSereServ7V2))
                {
                    result = ((UCTreeSereServ7V2)this.entity).GetListAll();
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
