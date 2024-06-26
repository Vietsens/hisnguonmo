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
using HIS.Desktop.Library.CacheClient;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Common.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace HIS.Desktop.Modules.Main
{
    class SqliteProcess : ProcessBase
    {
        internal SqliteProcess() { }
        internal bool Sync()
        {
            bool success = false;
            var tblSyncTimeInDB = SQLiteDatabaseWorker.SQLiteDatabase.GetDataTable("select " + DataKeyConstan.KEY + "," + DataKeyConstan.LAST_DB_MODIFY_TIME + " from " + TableNameConstan.TableName__SHC_SYNC + " where " + DataKeyConstan.IS_MODIFIED + " = 1");
            if (tblSyncTimeInDB != null && tblSyncTimeInDB.Rows.Count > 0)
            {
                for (int i = 0; i < tblSyncTimeInDB.Rows.Count; i++)
                {
                    string dataKey = tblSyncTimeInDB.Rows[i][DataKeyConstan.KEY].ToString();
                    string tableName = dataKey.Substring(dataKey.LastIndexOf(".") + 1);
                    long lastModifyTime = Inventec.Common.TypeConvert.Parse.ToInt64(tblSyncTimeInDB.Rows[i][DataKeyConstan.LAST_DB_MODIFY_TIME].ToString());
                    Type t = GetTypeByName(dataKey);
                    bool successs = false;

                    IList dataSave = GetDataByKey(t, dataKey, tableName);

                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => t), t) + "____SyncTimeInDB = " + lastModifyTime);
                    //Update dữ liệu từ DB local vào RAM
                    bool valid = (t != null && dataSave != null && dataSave.Count > 0);
                    successs = valid && (BackendDataWorker.UpdateToRam(t, dataSave, lastModifyTime));
                    if (successs)
                    {
                        Inventec.Common.Logging.LogSystem.Info(tableName + " update tu sqlite cache local ve RAM thanh cong____LAST_DB_MODIFY_TIME = " + lastModifyTime + "____Count = " + dataSave.Count);

                        ResetDataExt(dataKey);
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Info(dataKey + " map to Type fail, sync data to ram fail");
                    }

                    //Cập nhật trạng thái của dữ liệu trong DB cache local từ trạng thái có dữ liệu mới sửa cần đồng bộ -> trạng thái không có dữ liệu mới
                    if (!ResetIsModified(dataKey))
                    {
                        Inventec.Common.Logging.LogSystem.Info("Reset IsModified cua key " + dataKey + " trong bang " + TableNameConstan.TableName__SHC_SYNC + " that bai.");
                    }

                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                }
            }
            return success;
        }

        static bool ResetIsModified(string key)
        {
            bool result = false;
            try
            {
                Dictionary<String, Object> dicDataSync = new Dictionary<string, object>();
                dicDataSync.Add(DataKeyConstan.KEY, key);
                dicDataSync.Add(DataKeyConstan.IS_MODIFIED, 0);
                result = SQLiteDatabaseWorker.SQLiteDatabase.Update(TableNameConstan.TableName__SHC_SYNC, dicDataSync, DataKeyConstan.KEY + " = '" + key + "'");
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        IList GetDataByKey(Type t, string key, string tableName)
        {
            IList dataSave = null;
            try
            {
                if (t == typeof(MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY))
                {
                    try
                    {
                        dataSave = SQLiteDatabaseWorker.SQLiteDatabase.GetList<MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY>(tableName);
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Warn("Bang du lieu da bi thay doi ve cau truc, se tao lai bang. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => tableName), tableName));
                        Inventec.Common.Logging.LogSystem.Warn(ex);
                        RecreateTable(tableName);
                        dataSave = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY>();
                    }
                }
                else if (t == typeof(MOS.EFMODEL.DataModels.V_HIS_SERVICE))
                {
                    try
                    {
                        dataSave = SQLiteDatabaseWorker.SQLiteDatabase.GetList<MOS.EFMODEL.DataModels.V_HIS_SERVICE>(tableName);
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Warn("Bang du lieu da bi thay doi ve cau truc, se tao lai bang. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => tableName), tableName));
                        Inventec.Common.Logging.LogSystem.Warn(ex);
                        RecreateTable(tableName);
                        dataSave = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE>();
                    }
                }
                else if (t == typeof(MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM))
                {
                    try
                    {
                        dataSave = SQLiteDatabaseWorker.SQLiteDatabase.GetList<MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM>(tableName);
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Warn("Bang du lieu da bi thay doi ve cau truc, se tao lai bang. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => tableName), tableName));
                        Inventec.Common.Logging.LogSystem.Warn(ex);
                        RecreateTable(tableName);
                        dataSave = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM>();
                    }
                }
                else if (t == typeof(MOS.EFMODEL.DataModels.HIS_MEDI_ORG))
                {
                    try
                    {
                        dataSave = SQLiteDatabaseWorker.SQLiteDatabase.GetList<MOS.EFMODEL.DataModels.HIS_MEDI_ORG>(tableName);
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Warn("Bang du lieu da bi thay doi ve cau truc, se tao lai bang. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => tableName), tableName));
                        Inventec.Common.Logging.LogSystem.Warn(ex);
                        RecreateTable(tableName);
                        dataSave = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDI_ORG>();
                    }
                }
                else if (t == typeof(MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE))
                {
                    try
                    {
                        dataSave = SQLiteDatabaseWorker.SQLiteDatabase.GetList<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE>(tableName);
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Warn("Bang du lieu da bi thay doi ve cau truc, se tao lai bang. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => tableName), tableName));
                        Inventec.Common.Logging.LogSystem.Warn(ex);
                        RecreateTable(tableName);
                        dataSave = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE>();
                    }
                }
                else if (t == typeof(MOS.EFMODEL.DataModels.V_HIS_MATERIAL_TYPE))
                {
                    try
                    {
                        dataSave = SQLiteDatabaseWorker.SQLiteDatabase.GetList<MOS.EFMODEL.DataModels.V_HIS_MATERIAL_TYPE>(tableName);
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Warn("Bang du lieu da bi thay doi ve cau truc, se tao lai bang. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => tableName), tableName));
                        Inventec.Common.Logging.LogSystem.Warn(ex);
                        RecreateTable(tableName);
                        dataSave = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MATERIAL_TYPE>();
                    }
                }
                else if (t == typeof(MOS.EFMODEL.DataModels.HIS_ICD))
                {
                    try
                    {
                        dataSave = SQLiteDatabaseWorker.SQLiteDatabase.GetList<MOS.EFMODEL.DataModels.HIS_ICD>(tableName);
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Warn("Bang du lieu da bi thay doi ve cau truc, se tao lai bang. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => tableName), tableName));
                        Inventec.Common.Logging.LogSystem.Warn(ex);
                        RecreateTable(tableName);
                        dataSave = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_ICD>();
                    }
                }
                else if (t == typeof(MOS.EFMODEL.DataModels.HIS_CACHE_MONITOR))
                {
                    try
                    {
                        dataSave = SQLiteDatabaseWorker.SQLiteDatabase.GetList<MOS.EFMODEL.DataModels.HIS_CACHE_MONITOR>(tableName);
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Warn("Bang du lieu da bi thay doi ve cau truc, se tao lai bang. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => tableName), tableName));
                        Inventec.Common.Logging.LogSystem.Warn(ex);
                        RecreateTable(tableName);
                        dataSave = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_CACHE_MONITOR>();
                    }
                }
                //Các dữ liệu ít thay đổi                      
                else if (t == typeof(SDA.EFMODEL.DataModels.V_SDA_PROVINCE))
                {
                    try
                    {
                        dataSave = SQLiteDatabaseWorker.SQLiteDatabase.GetList<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>(tableName);
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Warn("Bang du lieu da bi thay doi ve cau truc, se tao lai bang. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => tableName), tableName));
                        Inventec.Common.Logging.LogSystem.Warn(ex);
                        RecreateTable(tableName);
                        dataSave = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>();
                    }
                }
                else if (t == typeof(SDA.EFMODEL.DataModels.V_SDA_DISTRICT))
                {
                    try
                    {
                        dataSave = SQLiteDatabaseWorker.SQLiteDatabase.GetList<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>(tableName);
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Warn("Bang du lieu da bi thay doi ve cau truc, se tao lai bang. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => tableName), tableName));
                        Inventec.Common.Logging.LogSystem.Warn(ex);
                        RecreateTable(tableName);
                        dataSave = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>();
                    }
                }
                else if (t == typeof(SDA.EFMODEL.DataModels.V_SDA_COMMUNE))
                {
                    try
                    {
                        dataSave = SQLiteDatabaseWorker.SQLiteDatabase.GetList<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>(tableName);
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Warn("Bang du lieu da bi thay doi ve cau truc, se tao lai bang. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => tableName), tableName));
                        Inventec.Common.Logging.LogSystem.Warn(ex);
                        RecreateTable(tableName);
                        dataSave = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>();
                    }
                }
                else if (t == typeof(SDA.EFMODEL.DataModels.SDA_NATIONAL))
                {
                    try
                    {
                        dataSave = SQLiteDatabaseWorker.SQLiteDatabase.GetList<SDA.EFMODEL.DataModels.SDA_NATIONAL>(tableName);
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Warn("Bang du lieu da bi thay doi ve cau truc, se tao lai bang. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => tableName), tableName));
                        Inventec.Common.Logging.LogSystem.Warn(ex);
                        RecreateTable(tableName);
                        dataSave = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_NATIONAL>();
                    }
                }
                else if (t == typeof(SDA.EFMODEL.DataModels.SDA_ETHNIC))
                {
                    try
                    {
                        dataSave = SQLiteDatabaseWorker.SQLiteDatabase.GetList<SDA.EFMODEL.DataModels.SDA_ETHNIC>(tableName);
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Warn("Bang du lieu da bi thay doi ve cau truc, se tao lai bang. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => tableName), tableName));
                        Inventec.Common.Logging.LogSystem.Warn(ex);
                        RecreateTable(tableName);
                        dataSave = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_ETHNIC>();
                    }
                }
                else
                {
                    bool successDelLostData = SQLiteDatabaseWorker.SQLiteDatabase.Delete(SerivceConfig.TableName__SHC_SYNC, SerivceConfig.KEY + "=" + key);

                    Inventec.Common.Logging.LogSystem.Info("Xoa dong du lieu dong bo thua (xoa key " + key + " trong bang SHC_SYNC) khong co trong danh sach du lieu hop le luu vao cache____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => successDelLostData), successDelLostData));
                }

                Inventec.Common.Logging.LogSystem.Info("SqliteProcess__GetDataByKey____type:" + key + "____" + Inventec.Common.Logging.LogUtil.TraceData("count", (dataSave != null ? dataSave.Count : 0)));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return dataSave;
        }

        void RecreateTable(string tableName)
        {
            try
            {
                SQLiteDatabaseWorker.SQLiteDatabase.DropTable(tableName);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
