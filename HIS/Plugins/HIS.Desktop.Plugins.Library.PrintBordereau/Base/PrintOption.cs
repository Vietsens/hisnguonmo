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

namespace HIS.Desktop.Plugins.Library.PrintBordereau.Base
{
    public class PrintOption
    {
        public enum Value
        {
            PRINT_NOW = 1,
            INIT_MENU = 2,
            PRINT_NOW_AND_INIT_MENU = 3,
            PRINT_NOW_AND_EMR_SIGN_NOW = 4,
            EMR_SIGN_NOW = 5,
            EMR_SIGN_AND_PRINT_PREVIEW = 6,
            SHOW_DIALOG = 7,
        }

        public enum PayType
        {
            ALL = 1,
            DEPOSIT = 2,
            NOT_DEPOSIT = 3,
            BILL = 4,
            NOT_BILL = 5,
            NOT_BILL_OR_DEPOSIT = 6
        }
    }
}
