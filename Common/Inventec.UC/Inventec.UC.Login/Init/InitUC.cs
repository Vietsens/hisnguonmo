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
using Inventec.UC.Login.Design.Template2;
using Inventec.UC.Login.Design.Template3;
using Inventec.UC.Login.UCD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.Login.Init
{
    class InitUC : IInitUC
    {
        public System.Windows.Forms.UserControl Init(MainLogin.EnumTemplate Template, InitUCD Data)
        {
            System.Windows.Forms.UserControl result = null;
            try
            {
                if (Template == MainLogin.EnumTemplate.TEMPLATE1)
                {
                    result = new Template1(Data);
                }
                else if (Template == MainLogin.EnumTemplate.TEMPLATE2)
                {
                    result = new Template2(Data);
                }
                else if (Template == MainLogin.EnumTemplate.TEMPLATE3)
                {
                    result = new Template3(Data);
                }
            }
            catch
            {
                result = null;
            }
            return result;
        }
    }
}
