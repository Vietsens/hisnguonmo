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

namespace HIS.Desktop.Library.CacheClient
{
    class SqliteDataBaseCreate
    {
        internal const string seperate = ",";

        public SqliteDataBaseCreate() { }

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

        internal void InitDataCacheLocal()
        {
            try
            {
                CheckConstructCacheDB();

                string scriptCreateTable = "create table if not exists {0} ({1})";
                string scriptGenerateColumns = "";
                string tempGenerateColumn = "{0} {1}";

                if (ExistsModifiedInTable(SerivceConfig.TableName__SHC_SYNC, "") == null)
                {
                    DropTable(SerivceConfig.TableName__SHC_SYNC);
                }

                if (!SqliteCheck.CheckExistsTable(SerivceConfig.TableName__SHC_SYNC))
                {
                    scriptGenerateColumns = "";
                    scriptGenerateColumns += String.Format(tempGenerateColumn, SerivceConfig.KEY, "TEXT");
                    scriptGenerateColumns += (seperate + String.Format(tempGenerateColumn, SerivceConfig.LAST_DB_MODIFY_TIME, "BIGINT"));
                    scriptGenerateColumns += (seperate + String.Format(tempGenerateColumn, SerivceConfig.LAST_SYNC_MODIFY_TIME, "BIGINT"));
                    scriptGenerateColumns += (seperate + String.Format(tempGenerateColumn, SerivceConfig.IS_MODIFIED, "INT"));
                    CreateTable(String.Format(scriptCreateTable, SerivceConfig.TableName__SHC_SYNC, scriptGenerateColumns));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        
        internal bool CreateTable(string scriptCreateTable)
        {
            bool valid = false;
            try
            {
                int rs = SQLiteDatabaseWorker.SQLiteDatabase.ExecuteNonQuery(scriptCreateTable);
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

        bool? ExistsModifiedInTable(string table, string key)
        {
            bool? result = null;
            try
            {
                string cmd = "select " + SerivceConfig.IS_MODIFIED + " from " + table;
                if (!String.IsNullOrEmpty(key))
                {
                    cmd += " where " + SerivceConfig.KEY + " = '" + key + "'";
                }
                cmd += " order by 1;";
                DataTable oldData = SQLiteDatabaseWorker.SQLiteDatabase.GetDataTable(cmd);
                if (oldData != null && oldData.Rows.Count > 0)
                {
                    result = (int.Parse((oldData.Rows[0][0].ToString() ?? "0")) == 1);
                }

                if (result == null)
                {
                    Inventec.Common.Logging.LogSystem.Info("Cau truc bang " + table + " da thay doi, can cap nhat cau truc bang moi de chay duoc" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => table), table) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => key), key));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        void CheckConstructCacheDB()
        {
            try
            {
                //try
                //{
                //    var oldData = SQLiteDatabaseWorker.SQLiteDatabase.GetDataTable("select " + SerivceConfig.KEY + " from " + SerivceConfig.TableName__SHC_SYNC + " order by " + SerivceConfig.LAST_DB_MODIFY_TIME + " desc");
                //}
                //catch (Exception ex)
                //{
                //    Inventec.Common.Logging.LogSystem.Warn("Cau truc file cache local da bi sua, phai xoa cac bang cu trogn DB di va khoi tao lai____" + ex);
                //    SQLiteDatabaseWorker.SQLiteDatabase.DropTable(SerivceConfig.TableName__SHC_DATA);
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool DropTable(string tableName)
        {
            bool success = false;
            try
            {
                SQLiteDatabaseWorker.SQLiteDatabase.DropTable(tableName);
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
