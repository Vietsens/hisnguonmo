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

namespace Inventec.UC.Login.Set.SetBranch
{
    class SetBranch : ISetBranch
    {
        Object obj;
        long? id;

        internal SetBranch(Object obj, long? id)
            : base()
        {
            this.obj = obj;
            this.id = id;
        }

        void ISetBranch.Run(System.Windows.Forms.UserControl UC)
        {
            try
            {
                if (UC.GetType() == typeof(Design.Template3.Template3))
                {
                    Design.Template3.Template3 UCLogin = (Design.Template3.Template3)UC;
                    UCLogin.SetBranch(obj, id);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
