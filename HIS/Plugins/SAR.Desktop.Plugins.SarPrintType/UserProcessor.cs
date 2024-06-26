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
using SAR.Desktop.Plugins.SarPrintType.ADO;
using Inventec.Core;
using ACS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SAR.Desktop.Plugins.SarPrintType
{
    public class UserProcessor
    {
        public static List<ACS_USER> AcsUsers { get; set; }

        object uc;
        public UserProcessor()
            : base()
        { }


        public UserProcessor(CommonParam paramBusiness, List<ACS_USER> _AcsUsers)
           
        {
            if (_AcsUsers != null && _AcsUsers.Count > 0)
            {
                AcsUsers = _AcsUsers.Where(p => p.IS_ACTIVE == 1).ToList();
            }

        }

      
    }
}
