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

namespace HIS.UC.FormType
{
    public class FormTypeDelegate
    {
        private static ProcessFormType processFormType;
        public static ProcessFormType ProcessFormType
        {
            get
            {
                return processFormType;
            }
            set
            {
                processFormType = value;
            }
        }

        private static GetCashierUser getCashierUser;
        public static GetCashierUser GetCashierUser
        {
            get
            {
                return getCashierUser;
            }
            set
            {
                getCashierUser = value;
            }
        }

        private static GetSaleUser getSaleUser;
        public static GetSaleUser GetSaleUser
        {
            get
            {
                return getSaleUser;
            }
            set
            {
                getSaleUser = value;
            }
        }

        private static PagingGetDelegate pagingGet;
        public static PagingGetDelegate PagingGet
        {
            get
            {
                return pagingGet;
            }
            set
            {
                pagingGet = value;
            }
        }

    }
}
