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
using Inventec.UC.Login.Design.Template1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.Login.Set.SetDelegateButtonConfig
{
    class SetDelefateButtonConfig : ISetDelefateButtonConfig
    {
        public bool SetDelegateConfig(System.Windows.Forms.UserControl UC, EventButtonConfig btnConfig)
        {
            bool valid = false;
            try
            {
                if (UC.GetType() == typeof(Template1))
                {
                    Template1 UCLogin = (Template1)UC;
                    valid = UCLogin.SetDelegateButtonConfig(btnConfig);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                valid = false;
            }
            return valid;
        }
    }
}
