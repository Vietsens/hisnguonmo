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
using Inventec.Common.Repository;
using Inventec.Core;
using RDCACHE.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Library.CacheClient
{
    public class SqliteSet
    {
        internal SqliteTableCreate SqliteTableCreate { get { return (SqliteTableCreate)Worker.Get<SqliteTableCreate>(); } }
        const string seperate = ",";
        public SqliteSet() { }

        public void Create<T>(List<T> datas, bool isWaitingSync)
        {
            try
            {
                bool isOK = false;
                string dataKey = typeof(T).ToString();
                string tableName = dataKey.Substring(dataKey.LastIndexOf(".") + 1);
                if (datas != null && datas.Count > 0)
                {
                    if (SqliteTableCreate.CreateTableNew<T>(tableName))
                    {
                        try
                        {
                            SQLiteDatabaseWorker.SQLiteDatabase.ClearTable(tableName);
                        }
                        catch (Exception exx)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi clear du lieu bang " + tableName + " trong sqlite DB", exx);
                        }

                        try
                        {
                            isOK = SQLiteDatabaseWorker.SQLiteDatabase.ExecuteWithData(datas, tableName, true);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn(ex);
                            SQLiteDatabaseWorker.SQLiteDatabase.DropTable(tableName);
                            if (SqliteTableCreate.CreateTableNew<T>(tableName))
                            {
                                //isOK = SQLiteDatabaseWorker.SQLiteDatabase.ExecuteWithData(datas, tableName, true);
                                Delete("Library.CacheClient.Sqlites.SHC_SYNC", SerivceConfig.KEY + " = " + dataKey);
                                isOK = false;
                            }
                        }

                        if (!isOK)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("SqliteSet.Create fail. Loi them du lieu vao DB cache local____"
                            + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataKey), dataKey)
                            + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => tableName), tableName)
                            + "____" + Inventec.Common.Logging.LogUtil.TraceData("count", datas.Count));
                        }
                        else
                        {
                            if (dataKey.Contains("HIS_CACHE_MONITOR"))
                                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => datas), datas));

                            long? modifyTimeNew = Utils.GetModifyTimeMax<T>(datas);
                            Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => modifyTimeNew), modifyTimeNew));
                            Dictionary<String, Object> dicDataSync = new Dictionary<string, object>();
                            dicDataSync.Add(SerivceConfig.KEY, dataKey);
                            dicDataSync.Add(SerivceConfig.LAST_DB_MODIFY_TIME, modifyTimeNew);
                            dicDataSync.Add(SerivceConfig.LAST_SYNC_MODIFY_TIME, Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss")));
                            dicDataSync.Add(SerivceConfig.IS_MODIFIED, (isWaitingSync ? 1 : 0));
                            if (SqliteCheck.ExistsKeyInTable(SerivceConfig.TableName__SHC_SYNC, SerivceConfig.KEY, dataKey))
                            {
                                Inventec.Common.Logging.LogSystem.Debug("SqliteSet=>1");
                                isOK = isOK && SQLiteDatabaseWorker.SQLiteDatabase.Update(SerivceConfig.TableName__SHC_SYNC, dicDataSync, SerivceConfig.KEY + " = '" + dataKey + "'");
                            }
                            else
                            {
                                Inventec.Common.Logging.LogSystem.Debug("SqliteSet=>2");
                                isOK = isOK && SQLiteDatabaseWorker.SQLiteDatabase.Insert(SerivceConfig.TableName__SHC_SYNC, dicDataSync);
                            }
                            Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => isOK), isOK));
                        }
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        GC.Collect();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal bool Delete(string dataKey, string condition)
        {
            bool success = false;
            try
            {
                string tableName = dataKey.Substring(dataKey.LastIndexOf(".") + 1);
                success = SQLiteDatabaseWorker.SQLiteDatabase.Delete(tableName, (String.IsNullOrEmpty(condition) ? "1 = 1" : condition));
                Inventec.Common.Logging.LogSystem.Info("SqliteWorker.Delete " + success + ". table = " + tableName);
                success = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataKey), dataKey));
                Inventec.Common.Logging.LogSystem.Warn(ex);
                success = false;
            }

            return success;
        }

        long? GetModifyTimeMax<T>(List<T> datas)
        {
            long? modifyTimeNew = 0;
            try
            {
                Type type = typeof(T);
                System.Reflection.PropertyInfo propertyInfoOrderField = type.GetProperty("MODIFY_TIME");
                if (propertyInfoOrderField != null)
                {
                    var tbl = datas.ListToDataTable<T>();
                    var drSort = tbl.Select("1 = 1", "MODIFY_TIME DESC").FirstOrDefault();
                    modifyTimeNew = (drSort != null ? long.Parse((drSort["MODIFY_TIME"] ?? "").ToString()) : 0);
                    modifyTimeNew = ((modifyTimeNew.HasValue && modifyTimeNew > 0) ? modifyTimeNew : 0);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return modifyTimeNew;
        }
    }
}
