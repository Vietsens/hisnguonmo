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

namespace Inventec.Common.Sqlite
{
    public class SqliteConfig
    {
        internal const string SQLITE_DATA_SOURCE = "InventecSqliteDB.s3db";//Sqlite Datasource name
        internal const string TABLE_SPACE__LOCAL_DATA = "TBL__LOCAL_DATA";//Sqlite local datatable name
        internal const string TABLE_SPACE__BACKEND_DATA = "TBL__BACKEND_DATA";//Sqlite backend datatable name
        internal const string TABLE_SPACE__CONFIG_DATA = "TBL__CONFIG_DATA";//Sqlite config datatable name

        public static string SQLITE_DATA_SOURCE__CUSTOM = "";//Sqlite Datasource name custom
    }
}
