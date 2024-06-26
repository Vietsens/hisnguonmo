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

namespace HIS.Desktop.Plugins.EstablishHisWarningFEECFG
{
    class HisRequestUriStore
    {
        internal const string MOSHIS_WARNING_FEE_CFG_CREATE = "api/HisWarningFeeCfg/Create";
        internal const string MOSHIS_WARNING_FEE_CFG_DELETE = "api/HisWarningFeeCfg/Delete";
        internal const string MOSHIS_WARNING_FEE_CFG_UPDATE = "api/HisWarningFeeCfg/Update";
        internal const string MOSHIS_WARNING_FEE_CFG_GET = "api/HisWarningFeeCfg/Get";
        internal const string MOSHIS_WARNING_FEE_CFG_CHANGE_LOCK = "api/HisWarningFeeCfg/ChangeLock";
    }
}
