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
using System.Threading;
using System.Threading.Tasks;

namespace HIS.Desktop.Library.CacheClient
{
    public class SqliteCheck
    {
        public static bool ExistsDataInTable(string table)
        {
            bool result = false;
            try
            {
                //Thread.Sleep(200);
                string cmd = "select count(*) from " + table + ";";
                DataTable oldData = SQLiteDatabaseWorker.SQLiteDatabase.GetDataTable(cmd);
                result = (oldData != null && oldData.Rows.Count > 0 && Convert.ToInt64(oldData.Rows[0][0].ToString()) > 0);

                if (!result)
                {
                    Inventec.Common.Logging.LogSystem.Info("cmd = " + cmd + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => oldData), oldData));
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Info("ExistsKeyInTable: table " + table + ", result = " + result);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        public static bool ExistsKeyInTable(string table, string keyName, string value)
        {
            bool result = false;
            try
            {
                Thread.Sleep(200);
                string cmd = "select count(*) from " + table + " where " + keyName + " = '" + value + "';";
                DataTable oldData = SQLiteDatabaseWorker.SQLiteDatabase.GetDataTable(cmd);
                result = (oldData != null && oldData.Rows.Count > 0 && Convert.ToInt64(oldData.Rows[0][0].ToString()) > 0);

                if (!result)
                {
                    Inventec.Common.Logging.LogSystem.Info("cmd = " + cmd + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => oldData), oldData));
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Info("ExistsKeyInTable: table " + table + ", result = " + result);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        public static bool? ExistsModifiedInTable(string table, string key)
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

        public static bool CheckExistsTable(string tableName)
        {
            bool valid = false;
            try
            {
                DataTable tables = SQLiteDatabaseWorker.SQLiteDatabase.GetDataTable("select NAME from SQLITE_MASTER where type='table' and NAME = '" + tableName + "'");
                valid = (tables != null && tables.Rows.Count > 0);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }


    }
}
