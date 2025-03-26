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
using Inventec.UC.Login.Design.Template3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.Login.Set.SetDelegateLoginInfor
{
    class SetDelegateLoginInfor : ISetDelegateLoginInfor
    {
        public bool SetDelegateLogin(System.Windows.Forms.UserControl UC, LoginInfor Infor)
        {
            bool valid = false;
            try
            {
                if (UC.GetType() == typeof(Template1))
                {
                    Template1 UCLogin = (Template1)UC;
                    valid = UCLogin.SetDelegateLoginInfor(Infor);

                }
                else if (UC.GetType() == typeof(Template3))
                {
                    Template3 UCLogin = (Template3)UC;
                    valid = UCLogin.SetDelegateLoginInfor(Infor);

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
