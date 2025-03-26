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

namespace Inventec.Common.Sqlite
{
    public enum LocalDataType
    {
        BackendData,
        LocalData,
    }

    public class LocalDataWorker : BussinessBase
    {
        public LocalDataWorker(CommonParam param)
            : base(param)
        {

        }

        public LocalDataWorker()
            : base()
        {

        }

        public bool GenerateDBFile(string dbFilename)
        {
            bool success = false;
            try
            {

                success = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                success = false;
            }
            return success;
        }

        public T Get<T>(string key, object tableName)
        {
            T result = default(T);
            try
            {
                IDelegacyT delegacy = new LocalStorageDataGet(param, key, tableName);
                result = delegacy.Execute<T>();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                result = default(T);
            }
            return result;
        }

        public T Set<T>(object value, object tableName)
        {
            T result = default(T);
            try
            {
                IDelegacyT delegacy = new LocalStorageDataBackendSet(param, value, tableName);
                result = delegacy.Execute<T>();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                result = default(T);
            }
            return result;
        }
    }
}
