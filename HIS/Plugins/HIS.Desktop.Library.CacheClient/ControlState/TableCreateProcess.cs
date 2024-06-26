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
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Library.CacheClient.ControlState
{
    class TableCreateProcess
    {
        internal const string seperate = ",";

        public TableCreateProcess() { }

        internal string GetSqliteType(Type type)
        {
            string t = "";
            if (type == typeof(short) || type == typeof(int) || type == typeof(short?) || type == typeof(int?))
            {
                t = "INTEGER";
            }
            else if (type == typeof(long) || type == typeof(long?))
            {
                t = "BIGINT";
            }
            else if (type == typeof(decimal) || type == typeof(decimal?))
            {
                t = "NUMERIC";
            }
            else
            {
                t = "TEXT";
            }
            return t;
        }

        internal bool CreateTable(string scriptCreateTable)
        {
            bool valid = false;
            try
            {
                int rs = DatabaseCSWorker.DatabaseCS.ExecuteNonQuery(scriptCreateTable);
                if (rs <= -1)
                {
                    Inventec.Common.Logging.LogSystem.Warn("Khong tao duoc bang du lieu luu cache local trong db sqlite" + ", Du lieu truyen vao: scriptCreateTable = " + scriptCreateTable + ", ket qua: rs = " + rs);
                }
                else
                {
                    valid = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }
                       
        bool DropTable(string tableName)
        {
            bool success = false;
            try
            {
                DatabaseCSWorker.DatabaseCS.DropTable(tableName);
                success = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return success;
        }
    }
}
