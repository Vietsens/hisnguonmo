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

namespace Inventec.Common.Sqlite
{
    public class SQLiteManager
    {
        SQLiteDatabase sqliteDatabase;
        string seperate = ",";

        public SQLiteManager(string dbFile)
        {
            try
            {
                this.sqliteDatabase = new SQLiteDatabase(dbFile);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public DataTable GetDataTable(string sql)
        {
            try
            {
                return this.sqliteDatabase.GetDataTable(sql);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }

        public void CreateTable(Table table)
        {
            bool valid = true;
            try
            {
                valid = valid && (table != null);
                valid = valid && (!String.IsNullOrEmpty(table.TableName));
                valid = valid && (table.Columns != null && table.Columns.Count > 0);
                if (valid)
                {
                    DataTable tables = this.sqliteDatabase.GetDataTable("select NAME from SQLITE_MASTER where type='table' and NAME = '" + table.TableName + "';");
                    if (tables == null || tables.Rows.Count == 0)
                    {
                        int dem = 0;
                        string scriptCreateTable = "create table {0} ({1})";
                        string scriptGenerateColumns = "";
                        string tempGenerateColumn = "{0} {1}";
                        foreach (var col in table.Columns)
                        {
                            scriptGenerateColumns += ((dem == 0 ? "" : seperate) + String.Format(tempGenerateColumn, col.ColumnName, col.ColumnType));
                        }
                        int rs = this.sqliteDatabase.ExecuteNonQuery(String.Format(scriptCreateTable, table.TableName, scriptGenerateColumns));
                        if (rs <= -1)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Khong tao duoc bang du lieu luu cache local trong db sqlite" + ", Du lieu truyen vao: scriptCreateTable = " + scriptCreateTable + ", ket qua: rs = " + rs);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
