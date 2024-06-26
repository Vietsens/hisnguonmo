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

namespace HIS.Desktop.ApiConsumer
{
    public class HtcRequestUriStore
    {
        public const string HTC_EXPENSE__GET = "api/HtcExpense/Get";
        public const string HTC_EXPENSE__GETVIEW = "api/HtcExpense/GetView";
        public const string HTC_EXPENSE__CHANGELOCK = "api/HtcExpense/ChangeLock";
        public const string HTC_EXPENSE__CREATE = "api/HtcExpense/Create";
        public const string HTC_EXPENSE__DELETE = "api/HtcExpense/Delete";
        public const string HTC_EXPENSE__UPDATE = "api/HtcExpense/Update";

        public const string HTC_EXPENSE_TYPE__GET = "api/HtcExpenseType/Get";
        public const string HTC_EXPENSE_TYPE__CREATE = "api/HtcExpenseType/Create";
        public const string HTC_EXPENSE_TYPE__UPDATE = "api/HtcExpenseType/Update";

        public const string HTC_PERIOD__GET = "api/HtcPeriod/Get";
        public const string HTC_PERIOD__CHANGELOCK = "api/HtcPeriod/ChangeLock";
        public const string HTC_PERIOD__CREATE = "api/HtcPeriod/Create";
        public const string HTC_PERIOD__DELETE = "api/HtcPeriod/Delete";
        public const string HTC_PERIOD__UPDATE = "api/HtcPeriod/Update";
        public const string HTC_PERIOD__ALLOCATION_INDRECT_FEE = "api/HtcPeriod/AllocationIndrectFee";

        public const string HTC_PERIOD_DEPARTMENT__GET = "api/HtcPeriodDepartment/Get";
        public const string HTC_PERIOD_DEPARTMENT__GETVIEW = "api/HtcPeriodDepartment/GetView";
        public const string HTC_PERIOD_DEPARTMENT__CHANGELOCK = "api/HtcPeriodDepartment/ChangeLock";
        public const string HTC_PERIOD_DEPARTMENT__CREATE = "api/HtcPeriodDepartment/Create";
        public const string HTC_PERIOD_DEPARTMENT__DELETE = "api/HtcPeriodDepartment/Delete";
        public const string HTC_PERIOD_DEPARTMENT__UPDATE = "api/HtcPeriodDepartment/Update";

        public const string HTC_REVENUE__GET = "api/HtcRevenue/Get";
        public const string HTC_REVENUE__CHANGELOCK = "api/HtcRevenue/ChangeLock";
        public const string HTC_REVENUE__CREATE = "api/HtcRevenue/Create";
        public const string HTC_REVENUE__CREATET = "api/HtcRevenue/CreateT";
        public const string HTC_REVENUE__DELETE = "api/HtcRevenue/Delete";
        public const string HTC_REVENUE__UPDATE = "api/HtcRevenue/Update";
    }
}
