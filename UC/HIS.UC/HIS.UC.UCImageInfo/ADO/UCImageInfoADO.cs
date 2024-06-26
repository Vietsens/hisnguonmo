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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HIS.Desktop.DelegateRegister;

namespace HIS.UC.UCImageInfo.ADO
{
    public class UCImageInfoADO
    {
        public UCImageInfoADO() { }

        public List<ImageInfoADO> ListImageData { get; set; }

        public Action<object> _FocusNextUserControl { get; set; }

        public Action<object> _ReloadDataByCmndBefore { get; set; }
        public Action<object> _ReloadDataByCmndAfter { get; set; }
    }
}
