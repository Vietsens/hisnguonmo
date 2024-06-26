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
using Inventec.Core;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.LocalStorage.BackendData.Get;
using MOS.Filter;
using SDA.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData.Core.Get;
using HIS.Desktop.Library.CacheClient;
using HIS.Desktop.LocalStorage.BackendData.Core.CFG;
using HIS.Desktop.LocalStorage.BackendData.Core;

namespace HIS.Desktop.LocalStorage.BackendData
{
    class GetDataBehaviorFactory
    {
        internal static IGetDataT MakeIGetList(CommonParam param, Type type, object filterQuery)
        {
            IGetDataT result = null;
            try
            {
                //if (!TypeCollection.AcceptTypes.Contains(type))
                //    throw new NullReferenceException();

                dynamic filter = new System.Dynamic.ExpandoObject();
                //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;  
                #region Extend data
                if (type == typeof(CommuneADO))
                {
                    bool isSearchOrderByXHT = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS_DESKTOP_REGISTER__SEARCH_CODE__X/H/T") == "1" ? true : false;
                    if (isSearchOrderByXHT)
                    {
                        result = new SdaCommuneADOGetList1Behavior(param);
                    }
                    else
                    {
                        result = new SdaCommuneADOGetListBehavior(param);
                    }
                }
                else if (type == typeof(AgeADO))
                {
                    result = new HisAgeGetListBehavior(param);
                }
                else if (type == typeof(MOS.LibraryHein.Bhyt.HeinLiveArea.HeinLiveAreaData))
                {
                    result = new HeinLiveListAreaBehavior(param);
                }
                else if (type == typeof(MedicineMaterialTypeComboADO))
                {
                    result = new MedicineMaterialTypeComboGetBehavior(param);
                }
                #endregion

                #region Special data (cache)
                else if (type == typeof(SDA.EFMODEL.DataModels.V_SDA_COMMUNE))
                {
                    if (!HisConfigCFG.IsUseZip)
                    {
                        //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;  
                        if (HisConfigCFG.IsUseRedisCacheServer)
                        {
                            result = new RdCacheGetListBehavior(param, (filterQuery == null ? filter : filterQuery), RDCACHE.URI.RdCache.GET__COMMUNE, "api/SdaCommune/GetView", HIS.Desktop.ApiConsumer.ApiConsumers.SdaConsumer);
                        }
                        else
                            result = new SdaGetListBehavior(param, (filterQuery == null ? filter : filterQuery), "api/SdaCommune/GetView");
                    }
                    else
                    {
                        //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                        result = new SdaGetZipListBehavior(param, (filterQuery == null ? filter : filterQuery), "api/SdaCommune/GetViewZip");
                    }
                }
                else if (type == typeof(SDA.EFMODEL.DataModels.V_SDA_DISTRICT))
                {
                    if (!HisConfigCFG.IsUseZip)
                    {
                        //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                        if (HisConfigCFG.IsUseRedisCacheServer)
                            result = new RdCacheGetListBehavior(param, (filterQuery == null ? filter : filterQuery), RDCACHE.URI.RdCache.GET__DISTRICT, "api/SdaDistrict/GetView", HIS.Desktop.ApiConsumer.ApiConsumers.SdaConsumer);
                        else
                            result = new SdaGetListBehavior(param, (filterQuery == null ? filter : filterQuery), "api/SdaDistrict/GetView");
                    }
                    else
                    {
                        //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                        result = new SdaGetZipListBehavior(param, (filterQuery == null ? filter : filterQuery), "api/SdaDistrict/GetViewZip");
                    }
                }
                else if (type == typeof(SDA.EFMODEL.DataModels.V_SDA_PROVINCE))
                {
                    if (!HisConfigCFG.IsUseZip)
                    {
                        //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                        if (HisConfigCFG.IsUseRedisCacheServer)
                            result = new RdCacheGetListBehavior(param, (filterQuery == null ? filter : filterQuery), RDCACHE.URI.RdCache.GET__PROVINCE, "api/SdaProvince/GetView", HIS.Desktop.ApiConsumer.ApiConsumers.SdaConsumer);
                        else
                            result = new SdaGetListBehavior(param, (filterQuery == null ? filter : filterQuery), "api/SdaProvince/GetView");
                    }
                    else
                    {
                        //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                        result = new SdaGetZipListBehavior(param, (filterQuery == null ? filter : filterQuery), "api/SdaProvince/GetViewZip");
                    }
                }
                else if (type == typeof(SDA.EFMODEL.DataModels.SDA_NATIONAL))
                {
                    //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;    
                    if (HisConfigCFG.IsUseRedisCacheServer)
                        result = new RdCacheGetListBehavior(param, (filterQuery == null ? filter : filterQuery), RDCACHE.URI.RdCache.GET__NATIONAL, "api/SdaNational/Get", HIS.Desktop.ApiConsumer.ApiConsumers.SdaConsumer);
                    else
                        result = new SdaGetListBehavior(param, (filterQuery == null ? filter : filterQuery), "api/SdaNational/Get");
                }
                else if (type == typeof(SDA.EFMODEL.DataModels.SDA_TRANSLATE))
                {
                    filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                    //var language = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_LANGUAGE>();
                    //var lang = language.FirstOrDefault(o => o.LANGUAGE_CODE.ToUpper() == Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage().ToUpper() && o.IS_BASE != 1);
                    //if (lang != null)
                    //{
                    //    filter.LANGUAGE_ID = lang.ID;
                    //}
                    //else
                    //{
                    //    filter.LANGUAGE_ID = -1;
                    //}
                    result = new SdaGetListBehavior(param, filter, SdaRequestUriStore.SDA_TRANSLATE_GET);
                }

                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_MEDI_ORG))
                {
                    if (!HisConfigCFG.IsUseZip)
                    {
                        //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                        if (HisConfigCFG.IsUseRedisCacheServer)
                            result = new RdCacheGetListBehavior(param, (filterQuery == null ? filter : filterQuery), RDCACHE.URI.RdCache.GET__MEDI_ORG, "api/HisMediOrg/Get", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer);
                        else
                            result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), "api/HisMediOrg/Get");
                    }
                    else
                    {
                        //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                        result = new MosGetZipListBehavior(param, (filterQuery == null ? filter : filterQuery), "api/HisMediOrg/GetZip");
                    }
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_MEDICINE_PATY))
                {
                    if (!HisConfigCFG.IsUseZip)
                    {
                        //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                        result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), "/api/HisMedicinePaty/Get");
                    }
                    else
                    {
                        //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                        result = new MosGetZipListBehavior(param, (filterQuery == null ? filter : filterQuery), "/api/HisMedicinePaty/GetZip");
                    }
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.V_HIS_USER_ROOM))
                {
                    if (!HisConfigCFG.IsUseZip)
                    {
                        //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                        //filter.IS_PAUSE = IMSys.DbConfig.HIS_RS.HIS_ROOM.IS_PAUSE__TRUE;
                        filter.LOGINNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                        result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), "api/HisUserRoom/GetView");
                    }
                    else
                    {
                        //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                        //filter.IS_PAUSE = IMSys.DbConfig.HIS_RS.HIS_ROOM.IS_PAUSE__TRUE;
                        filter.LOGINNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                        result = new MosGetZipListBehavior(param, (filterQuery == null ? filter : filterQuery), "api/HisUserRoom/GetViewZip");
                    }
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_ICD))
                {
                    if (!HisConfigCFG.IsUseZip)
                    {
                        //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                        if (HisConfigCFG.IsUseRedisCacheServer)
                            result = new RdCacheGetListBehavior(param, (filterQuery == null ? filter : filterQuery), RDCACHE.URI.RdCache.GET__ICD, "api/HisIcd/Get", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer);
                        else
                            result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), "api/HisIcd/Get");
                    }
                    else
                    {
                        //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                        result = new MosGetZipListBehavior(param, (filterQuery == null ? filter : filterQuery), "api/HisIcd/GetZip");
                    }
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY))
                {
                    if (!HisConfigCFG.IsUseZip)
                    {
                        //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                        //Neu da chon chi nhanh thi chi lay chinh sach gia theo chi nhanh da chon
                        if (BranchDataWorker.GetCurrentBranchId() > 0)
                            filter.BRANCH_ID = BranchDataWorker.GetCurrentBranchId();
                        if (HisConfigCFG.IsUseRedisCacheServer)
                            result = new RdCacheGetListBehavior(param, (filterQuery == null ? filter : filterQuery), RDCACHE.URI.RdCache.GET__SERVICE_PATY, "api/HisServicePaty/GetView", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer);
                        else
                            result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), "api/HisServicePaty/GetView");
                    }
                    else
                    {
                        //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                        //Neu da chon chi nhanh thi chi lay chinh sach gia theo chi nhanh da chon
                        if (BranchDataWorker.GetCurrentBranchId() > 0)
                            filter.BRANCH_ID = BranchDataWorker.GetCurrentBranchId();
                        result = new MosGetZipListBehavior(param, (filterQuery == null ? filter : filterQuery), "api/HisServicePaty/GetViewZip");
                    }
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.V_HIS_SERVICE))
                {
                    if (!HisConfigCFG.IsUseZip)
                    {
                        ////filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                        if (HisConfigCFG.IsUseRedisCacheServer)
                            result = new RdCacheGetListBehavior(param, (filterQuery == null ? filter : filterQuery), RDCACHE.URI.RdCache.GET__SERVICE, "api/HisService/GetView", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer);
                        else
                            result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), "api/HisService/GetView");
                    }
                    else
                    {
                        ////filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                        result = new MosGetZipListBehavior(param, (filterQuery == null ? filter : filterQuery), "api/HisService/GetViewZip");
                    }
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM))
                {
                    if (!HisConfigCFG.IsUseZip)
                    {
                        //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                        if (HisConfigCFG.IsUseRedisCacheServer)
                            result = new RdCacheGetListBehavior(param, (filterQuery == null ? filter : filterQuery), RDCACHE.URI.RdCache.GET__SERVICE_ROOM, "api/HisServiceRoom/GetView", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer);
                        else
                            result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), "api/HisServiceRoom/GetView");
                    }
                    else
                    {
                        //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                        result = new MosGetZipListBehavior(param, (filterQuery == null ? filter : filterQuery), "api/HisServiceRoom/GetViewZip");
                    }
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE))
                {
                    if (!HisConfigCFG.IsUseZip)
                    {
                        filter.ORDER_FIELD = "MEDICINE_TYPE_CODE";
                        filter.ORDER_DIRECTION = "ASC";
                        //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                        if (HisConfigCFG.IsUseRedisCacheServer)
                            result = new RdCacheGetListBehavior(param, (filterQuery == null ? filter : filterQuery), RDCACHE.URI.RdCache.GET__MEDICINE_TYPE, "api/HisMedicineType/GetView", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer);
                        else
                            result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), "api/HisMedicineType/GetView");
                    }
                    else
                    {
                        filter.ORDER_FIELD = "MEDICINE_TYPE_CODE";
                        filter.ORDER_DIRECTION = "ASC";
                        //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                        result = new MosGetZipListBehavior(param, (filterQuery == null ? filter : filterQuery), "api/HisMedicineType/GetViewZip");
                    }
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.V_HIS_MATERIAL_TYPE))
                {
                    if (!HisConfigCFG.IsUseZip)
                    {
                        //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                        if (HisConfigCFG.IsUseRedisCacheServer)
                            result = new RdCacheGetListBehavior(param, (filterQuery == null ? filter : filterQuery), RDCACHE.URI.RdCache.GET__MATERIAL_TYPE, "api/HisMaterialType/GetView", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer);
                        else
                            result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), "api/HisMaterialType/GetView");
                    }
                    else
                    {
                        //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                        result = new MosGetZipListBehavior(param, (filterQuery == null ? filter : filterQuery), "api/HisMaterialType/GetViewZip");
                    }
                }
                #endregion

                else
                    result = ExecuteType(param, filterQuery, filter, type);

                if (result == null) throw new NullReferenceException();
            }
            catch (NullReferenceException ex)
            {
                Inventec.Common.Logging.LogSystem.Warn("Factory khong khoi tao duoc doi tuong." + type.GetType().ToString(), ex);
                result = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        internal static IGetDataTAsync MakeIGetListAsync(CommonParam param, Type type, object filterQuery)
        {
            IGetDataTAsync result = null;
            try
            {
                //if (!TypeCollection.AcceptTypes.Contains(type))
                //    throw new NullReferenceException();

                dynamic filter = new System.Dynamic.ExpandoObject();
                //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;  
                result = ExecuteTypeAsync(param, filterQuery, filter, type);

                if (result == null) throw new NullReferenceException();
            }
            catch (NullReferenceException ex)
            {
                Inventec.Common.Logging.LogSystem.Error("Factory khong khoi tao duoc doi tuong." + type.GetType().ToString() + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => type), type), ex);
                result = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        static string GetUriByType(string typeName)
        {
            string uri = "";
            try
            {
                var nameSplits = typeName.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                if (nameSplits != null && nameSplits.Length > 0)
                {
                    string tableViewName = nameSplits[3];
                    string tableViewNameFix = (tableViewName.StartsWith("V_") ? tableViewName.Substring(2) : tableViewName);
                    string methodApi = (tableViewName.StartsWith("V_") ? "GetView" : "Get");
                    uri = String.Format("/api/{0}/{1}", Util.FirstCharUpper(Util.ToCamelCase(tableViewNameFix)), methodApi);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return uri;
        }

        static IGetDataT ExecuteType(CommonParam param, object filterQuery, object filter, Type type)
        {
            IGetDataT result = null;
            string typeName = "", uriForType = "";
            try
            {
                typeName = type.ToString();
                uriForType = GetUriByType(typeName);
                if (typeName.StartsWith("MOS."))
                {
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), uriForType);
                }
                else if (typeName.StartsWith("SDA."))
                {
                    result = new SdaGetListBehavior(param, (filterQuery == null ? filter : filterQuery), uriForType);
                }
                else if (typeName.StartsWith("SAR."))
                {
                    result = new SarGetListBehavior(param, (filterQuery == null ? filter : filterQuery), uriForType);
                }
                else if (typeName.StartsWith("HTC."))
                {
                    result = new HtcGetListBehavior(param, (filterQuery == null ? filter : filterQuery), uriForType);
                }
                else if (typeName.StartsWith("LIS."))
                {
                    result = new LisGetListBehavior(param, (filterQuery == null ? filter : filterQuery), uriForType);
                }
                else if (typeName.StartsWith("ACS."))
                {
                    result = new AcsGetListBehavior(param, (filterQuery == null ? filter : filterQuery), uriForType);
                }
                else if (typeName.StartsWith("EMR."))
                {
                    result = new EmrGetListBehavior(param, (filterQuery == null ? filter : filterQuery), uriForType);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("typeName", typeName) + "____" + Inventec.Common.Logging.LogUtil.TraceData("GetUriByType", uriForType));
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        static IGetDataTAsync ExecuteTypeAsync(CommonParam param, object filterQuery, object filter, Type type)
        {
            IGetDataTAsync result = null;
            string typeName = "", uriForType = "";
            try
            {
                typeName = type.ToString();
                uriForType = GetUriByType(typeName);
                if (typeName.StartsWith("MOS."))
                {
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), uriForType);
                }
                else if (typeName.StartsWith("SDA."))
                {
                    result = new SdaGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), uriForType);
                }
                else if (typeName.StartsWith("SAR."))
                {
                    result = new SarGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), uriForType);
                }
                else if (typeName.StartsWith("HTC."))
                {
                    result = new HtcGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), uriForType);
                }
                else if (typeName.StartsWith("LIS."))
                {
                    result = new LisGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), uriForType);
                }
                else if (typeName.StartsWith("ACS."))
                {
                    result = new AcsGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), uriForType);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("typeName", typeName) + "____" + Inventec.Common.Logging.LogUtil.TraceData("GetUriByType", uriForType));
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
