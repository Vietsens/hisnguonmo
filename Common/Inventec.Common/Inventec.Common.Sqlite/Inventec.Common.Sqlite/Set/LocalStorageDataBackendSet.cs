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
using Inventec.Common.Sqlite.Base;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.Sqlite
{
    class LocalStorageDataBackendSet : BussinessBase, IDelegacyT
    {
        object data;
        object tableNameOrType;

        internal LocalStorageDataBackendSet(CommonParam param, object _data, object tablenameOrType)
            : base(param)
        {
            data = _data;
            tableNameOrType = tablenameOrType;
        }

        T IDelegacyT.Execute<T>()
        {
            T result = default(T);
            try
            {
                IDelegacyT behavior = LocalStorageDataBackendSetBehaviorFactory.MakeIGet(param, data, tableNameOrType);
                result = behavior != null ? (T)System.Convert.ChangeType(behavior.Execute<T>(), typeof(T)) : default(T);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = default(T);
            }
            return result;
        }
    }
}
