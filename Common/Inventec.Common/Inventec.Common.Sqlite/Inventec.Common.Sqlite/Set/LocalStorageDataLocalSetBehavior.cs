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
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.Sqlite
{
    class LocalStorageDataLocalSetBehavior : BussinessBase, IDelegacyT
    {
        string key;
        object value;
        string tableNameOrType;
        internal LocalStorageDataLocalSetBehavior(CommonParam param, string _key, object _value, string _tableNameOrType)
            : base()
        {
            this.key = _key;
            this.value = _value;
            this.tableNameOrType = _tableNameOrType;
        }

        T IDelegacyT.Execute<T>()
        {
            try
            {
                T result = default(T);

                SQLiteDatabase sQLiteDatabase;
                if (!String.IsNullOrEmpty(SqliteConfig.SQLITE_DATA_SOURCE__CUSTOM))
                    sQLiteDatabase = new SQLiteDatabase(SqliteConfig.SQLITE_DATA_SOURCE__CUSTOM);
                else
                    sQLiteDatabase = new SQLiteDatabase();

                DataTable resultData = sQLiteDatabase.GetDataTable(
                    "select * from " + this.tableNameOrType +
                    " where " + ColumnConfig.COLUMN_NAME__KEY + " = " + key
                    );

                if (resultData != null && resultData.Rows.Count > 0)
                {
                    result = (T)resultData.Rows[0][ColumnConfig.COLUMN_NAME__VALUE];
                }

                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                param.HasException = true;
                return default(T);
            }
        }
    }
}
