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
using HIS.Desktop.Library.CacheClient.Redis;
using HIS.Desktop.LocalStorage.BackendData.Core;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Common.Logging;
using Inventec.Common.Repository;
using Inventec.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace HIS.Desktop.LocalStorage.BackendData
{
    public partial class BackendDataWorker
    {
        internal static ConcurrentDictionary<Type, object> dic = new ConcurrentDictionary<Type, object>();
        internal static ConcurrentDictionary<Type, string> dicTimeSync = new ConcurrentDictionary<Type, string>();
        internal static Object thisLock = new Object();
        internal const string seperate = ",";
        internal static CacheWorker CacheWorker { get { return (CacheWorker)Worker.Get<CacheWorker>(); } }
        internal static CacheMonitorGet CacheMonitorGet { get { return (CacheMonitorGet)Worker.Get<CacheMonitorGet>(); } }
        internal static RamMonitorGet RamMonitorGet { get { return (RamMonitorGet)Worker.Get<RamMonitorGet>(); } }
        internal static CacheMonitorSync CacheMonitorSync { get { return (CacheMonitorSync)Worker.Get<CacheMonitorSync>(); } }
        internal static BackendDataReset BackendDataReset { get { return (BackendDataReset)Worker.Get<BackendDataReset>(); } }

        public static void InitDataCacheLocal()
        {
            try
            {
                CacheWorker.InitDataCacheLocal();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public static ConcurrentDictionary<Type, object> GetAll()
        {
            try
            {
                return dic;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dic), dic));
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }

        public static List<T> Get<T>() where T : class
        {
            return Get<T>(false, false);
        }

        public static List<T> Get<T>(bool isTranslate, bool isNotGetInCache)
        {
            return Get<T>(isTranslate, isNotGetInCache, false, true);
        }

        public static List<T> Get<T>(bool isTranslate, bool isNotGetInCache, bool islock, bool isSaveToRam)
        {
            List<T> result = null;
            object resultTemp = null;
            try
            {
                Process proc = Process.GetCurrentProcess();
                Type type = typeof(T);
                //Inventec.Common.Logging.LogAction.Debug("Dung lượng RAM trước khi gọi/lưu vào BackendDataWorker get data " + type.ToString() + " trong cache local là:" + ((decimal)proc.PrivateMemorySize64 / (1024 * 1024)) + "MB");
                if (
                    !isSaveToRam
                    //|| (
                    //    !CacheMonitorGet.IsExistsCode(type.ToString())
                    //    && !RamMonitorGet.IsExistsCode(type.ToString())
                    //)
                    )
                {
                    resultTemp = GetDataByType<T>(null, isNotGetInCache);
                    //if (isTranslate)
                    TranslateWorker.TranslateData(resultTemp as List<T>);
                    result = ((List<T>)resultTemp);
                }
                else
                {
                    if (!dic.ContainsKey(type))
                    {
                        LogSystem.Debug("dic khong ContainsKey Key: " + type.ToString());
                        if (islock)
                        {
                            lock (thisLock)
                            {
                                resultTemp = GetDataByType<T>(null, isNotGetInCache);
                                //if (isTranslate)
                                TranslateWorker.TranslateData(resultTemp as List<T>);

                                if (!dic.TryAdd(type, resultTemp))
                                {
                                    LogSystem.Info("Khong Add duoc cau hinh vao dictionary Key: " + type.ToString());
                                }

                                SetSyncTime<T>(DateTime.Now.ToString("yyyyMMddHHmmss"));
                                result = ((List<T>)resultTemp);
                            }
                        }
                        else
                        {
                            resultTemp = GetDataByType<T>(null, isNotGetInCache);

                            //if (isTranslate)
                                TranslateWorker.TranslateData(resultTemp as List<T>);

                            if (!dic.TryAdd(type, resultTemp))
                            {
                                LogSystem.Info("Khong Add duoc cau hinh vao dictionary Key: " + type.ToString());
                            }

                            SetSyncTime<T>(DateTime.Now.ToString("yyyyMMddHHmmss"));
                            result = ((List<T>)resultTemp);

                        }
                        Inventec.Common.Logging.LogSystem.Debug("BackendDataWorker => Loaded type " + type.ToString() + " success. Data Count = " + (result != null ? result.Count : 0));
                        //Inventec.Common.Logging.LogAction.Debug("Tổng số dữ liệu đang lưu BackendDataWorker trong cache RAM local Dictionary.count=" + dic.Count);
                        //Inventec.Common.Logging.LogAction.Debug("Chi tiết các dữ liệu đang lưu BackendDataWorker trong cache RAM local: " + String.Join(", ", dic.Keys.Select(k => k.ToString()).ToArray()));

                    }
                    else
                    {
                        if (!dic.TryGetValue(type, out resultTemp))
                        {
                            LogSystem.Debug("Khong get duoc cau hinh trong dictionary Key: " + type.ToString());
                            resultTemp = GetDataByType<T>(null, isNotGetInCache);

                            //if (isTranslate)
                            TranslateWorker.TranslateData(resultTemp as List<T>);

                            if (!dic.TryAdd(type, resultTemp))
                            {
                                LogSystem.Debug("Khong Add duoc cau hinh vao dictionary Key: " + type.ToString());
                            }

                            SetSyncTime<T>(DateTime.Now.ToString("yyyyMMddHHmmss"));

                            Inventec.Common.Logging.LogSystem.Debug("BackendDataWorker => Loaded type " + type.ToString() + " success. Data Count = " + (((List<T>)resultTemp) != null ? ((List<T>)resultTemp).Count : 0));
                        }
                       
                        result = ((List<T>)resultTemp);
                        //if (isTranslate)

                        if (result == null || result.Count == 0)
                        {
                            Inventec.Common.Logging.LogSystem.Debug("BackendDataWorker => List<T> Get<T> => result == null || result.Count == 0" + ", type = " + type.ToString() + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => resultTemp), resultTemp));
                        }
                    }
                }
                //long memoryUsageusers = (GC.GetTotalMemory(true) - startBytes) / (1024 * 1024);
                //Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => startBytes), startBytes) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => memoryUsageusers), memoryUsageusers));
                //Inventec.Common.Logging.LogSystem.Info("BackendDataWorker.Get: " + type.ToString() + "____1");
                //long startBytes = GC.GetTotalMemory(true);
                //decimal memoryUsageusers = (decimal)(startBytes) / (1024 * 1024);

                //Inventec.Common.Logging.LogSystem.Info("CalculateMemoryRam____Dung luong PM( GC.GetTotalMemory):" + ((decimal)startBytes / (1024 * 1024)) + "MB");
                //Inventec.Common.Logging.LogSystem.Info("CalculateMemoryRam____Dung luong PM(proc.PrivateMemorySize64):" + ((decimal)proc.PrivateMemorySize64 / (1024 * 1024)) + "MB");
                //Inventec.Common.Logging.LogAction.Debug("Dung lượng RAM sau khi gọi/lưu vào BackendDataWorker get data " + type.ToString() + " trong cache local là:" + ((decimal)proc.PrivateMemorySize64 / (1024 * 1024)) + "MB");

            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        static object GetDataByType<T>(object filter, bool isNotGetInCache)
        {
            string dataKey = typeof(T).ToString();
            long? modifyTimeNew = 0;
            List<T> datas = null;
            try
            {
                //Nếu có cấu hình sử dụng cache local lưu trữ dữ liệu tải về và dữ liệu cache đó có trong khai báo file CacheMonitorConfig.xml
                if (!isNotGetInCache
                    && !String.IsNullOrEmpty(dataKey)
                    && CacheMonitorGet.IsExistsCode(dataKey))
                {
                    datas = CacheWorker.Get<T>(dataKey);
                    if (datas != null && datas.Count > 0)
                    {
                        return datas;
                    }
                }
                CommonParam param = new CommonParam();
                SetOtherFilterQuery<T>(ref filter);
                IGetDataT delegacy = new GetData(param, filter);
                datas = (delegacy != null ? delegacy.Execute<T>() as List<T> : null);
                bool findNow = (filter != null);

                if (!findNow
                    && !isNotGetInCache
                    && datas != null && datas.Count > 0
                    && CacheMonitorGet.IsExistsCode(dataKey))
                {
                    CacheWorker.DeleteWithState(dataKey);
                    CacheWorker.Set<T>(datas, dataKey, false);
                }
                return datas;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Info("GetDataByType fail. Kiem tra cho xu ly them du lieu vao cache local, hoac ham get data tu api____"
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataKey), dataKey)
                    + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => modifyTimeNew), modifyTimeNew)
                    + "____" + Inventec.Common.Logging.LogUtil.TraceData("count", (datas != null ? datas.Count : 0)));
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }

        static void SetOtherFilterQuery<T>(ref object filter)
        {
            try
            {
                if (typeof(T) is MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY)
                {
                    if (filter == null)
                    {
                        dynamic dfilter = new System.Dynamic.ExpandoObject();
                        dfilter.BRANCH_ID = BranchDataWorker.GetCurrentBranchId();
                        dfilter.CustomColumns = new List<string>() {
                            "ID",
                            "PATIENT_TYPE_ID",
                            "PATIENT_TYPE_CODE",
                            "SERVICE_ID",
                            "INSTR_NUM_BY_TYPE_FROM",
                            "INSTR_NUM_BY_TYPE_TO",
                            "PRICE",
                            "VAT_RATIO",
                            "BRANCH_ID",
                            "SERVICE_TYPE_ID",
                            "IS_ACTIVE",
                            "MODIFY_TIME",
                            "PRIORITY",
                            "FROM_TIME",
                            "TO_TIME",
                            "TREATMENT_FROM_TIME",
                            "TREATMENT_TO_TIME",
                            "INTRUCTION_NUMBER_FROM",
                            "INTRUCTION_NUMBER_TO",
                            "HOUR_FROM",
                            "HOUR_TO",
                            "DAY_FROM",
                            "DAY_TO",
                            "REQUEST_ROOM_IDS",
                            "EXECUTE_ROOM_IDS",
                            "REQUEST_DEPARMENT_IDS",
                            "SERVICE_CONDITION_ID",
                            "SERVICE_CONDITION_CODE",
                            "SERVICE_CONDITION_NAME",
                            "HEIN_RATIO",
                            "BASE_PATIENT_TYPE_ID",
                            "INHERIT_PATIENT_TYPE_IDS",
                            "PACKAGE_ID"
                        };
                        filter = dfilter;
                    }
                    else
                    {
                        PropertyInfo pi = typeof(T).GetProperty("BRANCH_ID");
                        if (pi != null && BranchDataWorker.GetCurrentBranchId() > 0)
                        {
                            pi.SetValue(filter, BranchDataWorker.GetCurrentBranchId());
                        }
                    }
                }
                else if (typeof(T) == typeof(MOS.EFMODEL.DataModels.V_HIS_USER_ROOM)
                    || typeof(T) == typeof(MOS.EFMODEL.DataModels.HIS_USER_ROOM)
                    || typeof(T) == typeof(MOS.EFMODEL.DataModels.HIS_MEDICINE_TYPE_TUT))
                {
                    if (filter == null)
                    {
                        dynamic dfilter = new System.Dynamic.ExpandoObject();
                        dfilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                        string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                        dfilter.LOGINNAME__EXACT = loginName;
                        dfilter.LOGINNAME = loginName;
                        filter = dfilter;
                    }
                    else
                    {
                        string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                        PropertyInfo piLoginNameExact = typeof(T).GetProperty("LOGINNAME__EXACT");
                        if (piLoginNameExact != null)
                        {
                            piLoginNameExact.SetValue(filter, loginName);
                        }
                        PropertyInfo piLoginName = typeof(T).GetProperty("LOGINNAME");
                        if (piLoginName != null)
                        {
                            piLoginName.SetValue(filter, loginName);
                        }
                        PropertyInfo piIsActive = typeof(T).GetProperty("IS_ACTIVE");
                        if (piIsActive != null)
                        {
                            piIsActive.SetValue(filter, IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public static bool Reset<T>() where T : class
        {
            bool result = true;
            try
            {
                result = Reset(typeof(T));
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        public static bool Reset(Type type)
        {
            try
            {
                return BackendDataReset.Reset(type, dic, dicTimeSync);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return false;
        }

        public static bool UpdateToRam(Type key, object values, long timeSync)
        {
            bool result = false;
            try
            {
                if (key != null)
                {
                    object outValue = null;
                    if (dic.TryGetValue(key, out outValue))
                    {
                        if (!dic.TryUpdate(key, values, outValue))
                        {
                            if (!dic.TryRemove(key, out outValue))
                            {
                                LogSystem.Info("Khong Remove duoc cau hinh trong dictionary Key: " + key.ToString());
                                dic.TryUpdate(key, values, null);
                            }
                            else
                            {
                                if (!dic.TryAdd(key, values))
                                {
                                    LogSystem.Info("Khong Add duoc cau hinh vao dictionary Key: " + key.ToString());
                                }
                            }
                        }
                    }
                    else
                    {
                        if (!dic.TryAdd(key, values))
                        {
                            LogSystem.Info("Khong Add duoc cau hinh vao dictionary Key: " + key.ToString());
                        }
                    }

                    SetSyncTime(key, DateTime.Now.ToString("yyyyMMddHHmmss"));
                    result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        public static bool ResetAll()
        {
            bool result = false;
            try
            {
                dic.Clear();
                dicTimeSync.Clear();

                var cmos = new CacheMonitorGet().Get();
                if (cmos != null && cmos.Count > 0)
                {
                    foreach (var cm in cmos)
                    {
                        CacheWorker.Delete(cm.CacheMonitorKeyCode);
                    }
                }

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        public static bool ResetForLogout()
        {
            bool result = false;
            try
            {
                result = Reset(typeof(MOS.EFMODEL.DataModels.V_HIS_USER_ROOM));
                HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ExpmestSaleCreate__CashierRoomId = 0;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        public static bool IsExistsKey<T>()
        {
            bool result = false;
            try
            {
                result = dic.ContainsKey(typeof(T));
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }
    }
}
