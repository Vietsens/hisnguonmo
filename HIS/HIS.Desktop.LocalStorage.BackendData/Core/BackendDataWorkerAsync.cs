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
using ACS.Filter;
using Inventec.Common.Adapter;
using Inventec.Core;
using HIS.Desktop.ApiConsumer;
using SDA.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using System.Data;
using System.Threading.Tasks;
using System.Configuration;
using HIS.Desktop.LocalStorage.ConfigApplication;
using System.Reflection;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Common.Repository;
using System.Diagnostics;
using System.Collections.Concurrent;
using Inventec.Common.Logging;
using HIS.Desktop.Library.CacheClient;

namespace HIS.Desktop.LocalStorage.BackendData
{
    public partial class BackendDataWorkerAsync
    {
        internal static Object thisLock = new Object();
        internal const string seperate = ",";
        internal static CacheWorker CacheWorker { get { return (CacheWorker)Worker.Get<CacheWorker>(); } }
        internal static CacheMonitorGet CacheMonitorGet { get { return (CacheMonitorGet)Worker.Get<CacheMonitorGet>(); } }
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

        public static async Task<List<T>> Get<T>(bool isTranslate, bool isRamOnly) where T : class
        {
            List<T> result = null;
            object resultTemp = null;
            try
            {
                Type type = typeof(T);

                if (!BackendDataWorker.dic.ContainsKey(type))
                {
                    resultTemp = await GetDataByType<T>(null, isRamOnly);

                    if (!BackendDataWorker.dic.TryAdd(type, resultTemp))
                    {
                        LogSystem.Info("Khong Add duoc cau hinh vao dictionary Key: " + type.ToString());
                    }

                    BackendDataWorker.SetSyncTime<T>(DateTime.Now.ToString("yyyyMMddHHmmss"));

                    //Copy temp to result                
                    result = ((List<T>)resultTemp);
                }
                else
                {
                    if (!BackendDataWorker.dic.TryGetValue(type, out resultTemp))
                    {
                        LogSystem.Info("Khong get duoc cau hinh trong dictionary Key: " + type.ToString());
                        resultTemp = await GetDataByType<T>(null, isRamOnly);
                        if (!BackendDataWorker.dic.TryAdd(type, resultTemp))
                        {
                            LogSystem.Info("Khong Add duoc cau hinh vao dictionary Key: " + type.ToString());
                        }
                        BackendDataWorker.SetSyncTime<T>(DateTime.Now.ToString("yyyyMMddHHmmss"));
                    }
                    result = ((List<T>)resultTemp);
                }
                Inventec.Common.Logging.LogSystem.Debug("BackendDataWorkerAsync => Loaded type " + (type != null ? type.ToString() : "") + ". Data Count = " + (result != null ? result.Count : 0));
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        public static async Task<List<T>> Get<T>() where T : class
        {
            return await Get<T>(false, false);
        }

        /// <summary>
        ///Duyệt đa ngôn ngữ
        ///Kiểm tra ngôn ngữ đang chọn có phải là ngôn ngữ cơ sơ không
        ///Nếu là cơ sở: 
        ///------- Bỏ qua
        ///------- Nếu không phải là cơ sở: 
        ///-------------- duyệt trong bảng SdaTranslate theo key -> lấy giá trị của của dữ liệu tương ứng với ngôn ngữ được chọn
        ///-------------- update vào ram giá trị của dữ liệu đó vào Ram
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataRaw"></param>
        public static async Task TranslateData<T>(List<T> dataRaw)
        {
            try
            {
                if (TranslateDataWorker.Language != null && TranslateDataWorker.Language.IS_BASE != 1)
                {
                    string typeName = typeof(T).ToString();
                    var arrName = typeName.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    if (arrName != null && arrName.Count >= 3)
                    {
                        //MOS.SDO.WorkPlaceSDO
                        List<SDA.EFMODEL.DataModels.SDA_TRANSLATE> translates;

                        var ts = await Get<SDA.EFMODEL.DataModels.SDA_TRANSLATE>();
                        translates = ts.Where(o => o.LANGUAGE_ID == TranslateDataWorker.Language.ID
                            //&& o.SCHEMA == GetSchemaName(arrName[0])
                                                && o.TABLE_NAME == arrName[3]).ToList();

                        if (translates != null && translates.Count > 0)
                        {
                            dataRaw.ForEach(o => UpdateLanguage(o, translates));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        static void UpdateLanguage<T>(T t, List<SDA.EFMODEL.DataModels.SDA_TRANSLATE> translates)
        {
            try
            {
                Type type = t.GetType();
                var propers = type.GetProperties();

                foreach (var tra in translates)
                {
                    var ppName1 = (!String.IsNullOrEmpty(tra.FIND_COLUMN_NAME_ONE) ? propers.FirstOrDefault(o => o.Name == tra.FIND_COLUMN_NAME_ONE && (o.GetValue(t, null) ?? "").ToString() == tra.FIND_DATA_CODE_ONE) : null);
                    var ppName2 = (!String.IsNullOrEmpty(tra.FIND_COLUMN_NAME_TWO) ? propers.FirstOrDefault(o => o.Name == tra.FIND_COLUMN_NAME_TWO && (o.GetValue(t, null) ?? "").ToString() == tra.FIND_DATA_CODE_TWO) : null);
                    var ppNameUpdate = propers.FirstOrDefault(o => tra.UPDATE_COLUMN_NAME == o.Name);
                    if ((!String.IsNullOrEmpty(tra.FIND_COLUMN_NAME_ONE)
                        && !String.IsNullOrEmpty(tra.FIND_COLUMN_NAME_TWO)
                        && ppName1 != null
                        && ppName2 != null)
                        || (ppName1 != null || ppName2 != null))
                    {
                        ppNameUpdate.SetValue(t, tra.UPDATE_DATA_VALUE);
                        BackendDataWorker.SetSyncTime(type, DateTime.Now.ToString("yyyyMMddHHmmss"));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        static async Task<object> GetDataByType<T>(object filter, bool isOnlyRam)
        {
            string dataKey = typeof(T).ToString();
            long? modifyTimeNew = 0;
            List<T> datas = null;
            try
            {
                bool findNow = (filter != null);

                //Nếu có cấu hình sử dụng cache local lưu trữ dữ liệu tải về và dữ liệu cache đó có trong khai báo file CacheMonitorConfig.xml
                //--> Thì mới kiểm tra và lấy dữ liệu trong file sqlite DB local     
                if (!isOnlyRam && !String.IsNullOrEmpty(dataKey) && CacheMonitorGet.IsExistsCode(dataKey))
                {
                    datas = CacheWorker.Get<T>(dataKey);
                    if (datas != null && datas.Count > 0)
                    {
                        return datas;
                    }
                }

                CommonParam param = new CommonParam();
                SetOtherFilterQuery<T>(ref filter);
                Type type = typeof(T);
                if (type == typeof(AgeADO)
                    || type == typeof(ServiceComboADO)
                    || type == typeof(MedicineMaterialTypeComboADO)
                    || type == typeof(CommuneADO))
                {
                    IGetDataT delegacy = new GetData(param, filter);
                    datas = (delegacy != null ? delegacy.Execute<T>() as List<T> : null);
                }
                else
                {
                    IGetDataTAsync delegacy = new GetDataAsync(param, filter);
                    datas = (delegacy != null ? await delegacy.ExecuteAsync<T>() as List<T> : null);
                }

                if (!findNow
                    && !isOnlyRam
                    && datas != null && datas.Count > 0
                    && CacheMonitorGet.IsExistsCode(dataKey))
                {
                    try
                    {
                        CacheWorker.Set<T>(datas, dataKey);
                    }
                    catch (Exception exx) { Inventec.Common.Logging.LogSystem.Error(exx); }
                }
                return datas;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Info("InsertDataInToDBCache fail. Loi them du lieu vao DB cache local____"
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataKey), dataKey)
                    + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => modifyTimeNew), modifyTimeNew)
                    + "____" + Inventec.Common.Logging.LogUtil.TraceData("EncodeData(datas)", EncryptUtil.EncodeData(datas)));
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
    }
}
