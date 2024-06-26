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

namespace HIS.Desktop.LocalStorage.BackendData
{
    class GetDataBehaviorFactory
    {
        internal static IGetDataT MakeIGetList(CommonParam param, Type type, object filterQuery)
        {
            IGetDataT result = null;
            try
            {
                #region ACS
                if (type == typeof(ACS.EFMODEL.DataModels.ACS_USER))
                {
                    AcsUserFilter filter = new AcsUserFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.ACS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new AcsGetListBehavior(param, (filterQuery == null ? filter : filterQuery), AcsRequestUriStore.ACS_USER_GET);
                }
                else if (type == typeof(ACS.EFMODEL.DataModels.ACS_APPLICATION))
                {
                    AcsApplicationFilter filter = new AcsApplicationFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.ACS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new AcsGetListBehavior(param, (filterQuery == null ? filter : filterQuery), "api/AcsApplication/Get");
                }
                #endregion

                #region SDA
                else if (type == typeof(CommuneADO))
                {
                    result = new SdaCommuneADOGetListBehavior(param);
                }
                else if (type == typeof(SDA.EFMODEL.DataModels.SDA_COMMUNE))
                {
                    SdaCommuneFilter filter = new SdaCommuneFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new SdaGetListBehavior(param, (filterQuery == null ? filter : filterQuery), SdaRequestUriStore.SDA_COMMUNE_GETVIEW);
                }
                else if (type == typeof(SDA.EFMODEL.DataModels.SDA_DISTRICT))
                {
                    SdaDistrictFilter filter = new SdaDistrictFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new SdaGetListBehavior(param, (filterQuery == null ? filter : filterQuery), SdaRequestUriStore.SDA_DISTRICT_GETVIEW);
                }
                else if (type == typeof(SDA.EFMODEL.DataModels.SDA_PROVINCE))
                {
                    SdaProvinceFilter filter = new SdaProvinceFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new SdaGetListBehavior(param, (filterQuery == null ? filter : filterQuery), SdaRequestUriStore.SDA_PROVINCE_GETVIEW);
                }
                else if (type == typeof(SDA.EFMODEL.DataModels.V_SDA_COMMUNE))
                {
                    if (!HisConfigCFG.IsUseZip)
                    {
                        SdaCommuneViewFilter filter = new SdaCommuneViewFilter();
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
                        SdaCommuneViewFilter filter = new SdaCommuneViewFilter();
                        //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                        result = new SdaGetZipListBehavior(param, (filterQuery == null ? filter : filterQuery), "api/SdaCommune/GetViewZip");
                    }
                }
                else if (type == typeof(SDA.EFMODEL.DataModels.V_SDA_DISTRICT))
                {
                    if (!HisConfigCFG.IsUseZip)
                    {
                        SdaDistrictViewFilter filter = new SdaDistrictViewFilter();
                        //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                        if (HisConfigCFG.IsUseRedisCacheServer)
                            result = new RdCacheGetListBehavior(param, (filterQuery == null ? filter : filterQuery), RDCACHE.URI.RdCache.GET__DISTRICT, "api/SdaDistrict/GetView", HIS.Desktop.ApiConsumer.ApiConsumers.SdaConsumer);
                        else
                            result = new SdaGetListBehavior(param, (filterQuery == null ? filter : filterQuery), "api/SdaDistrict/GetView");
                    }
                    else
                    {
                        SdaDistrictViewFilter filter = new SdaDistrictViewFilter();
                        //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                        result = new SdaGetZipListBehavior(param, (filterQuery == null ? filter : filterQuery), "api/SdaDistrict/GetViewZip");
                    }
                }
                else if (type == typeof(SDA.EFMODEL.DataModels.V_SDA_PROVINCE))
                {
                    if (!HisConfigCFG.IsUseZip)
                    {
                        SdaProvinceViewFilter filter = new SdaProvinceViewFilter();
                        //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                        if (HisConfigCFG.IsUseRedisCacheServer)
                            result = new RdCacheGetListBehavior(param, (filterQuery == null ? filter : filterQuery), RDCACHE.URI.RdCache.GET__PROVINCE, "api/SdaProvince/GetView", HIS.Desktop.ApiConsumer.ApiConsumers.SdaConsumer);
                        else
                            result = new SdaGetListBehavior(param, (filterQuery == null ? filter : filterQuery), "api/SdaProvince/GetView");
                    }
                    else
                    {
                        SdaProvinceViewFilter filter = new SdaProvinceViewFilter();
                        //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                        result = new SdaGetZipListBehavior(param, (filterQuery == null ? filter : filterQuery), "api/SdaProvince/GetViewZip");
                    }
                }
                else if (type == typeof(SDA.EFMODEL.DataModels.SDA_NATIONAL))
                {
                    SdaNationalFilter filter = new SdaNationalFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;    
                    if (HisConfigCFG.IsUseRedisCacheServer)
                        result = new RdCacheGetListBehavior(param, (filterQuery == null ? filter : filterQuery), RDCACHE.URI.RdCache.GET__NATIONAL, "api/SdaNational/Get", HIS.Desktop.ApiConsumer.ApiConsumers.SdaConsumer);
                    else
                        result = new SdaGetListBehavior(param, (filterQuery == null ? filter : filterQuery), "api/SdaNational/Get");
                }
                else if (type == typeof(SDA.EFMODEL.DataModels.SDA_GROUP))
                {
                    SdaGroupFilter filter = new SdaGroupFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new SdaGetListBehavior(param, (filterQuery == null ? filter : filterQuery), SdaRequestUriStore.SDA_GROUP_GET);
                }
                else if (type == typeof(SDA.EFMODEL.DataModels.SDA_ETHNIC))
                {
                    SdaEthnicFilter filter = new SdaEthnicFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new SdaGetListBehavior(param, (filterQuery == null ? filter : filterQuery), SdaRequestUriStore.SDA_ETHNIC_GET);
                }
                else if (type == typeof(SDA.EFMODEL.DataModels.SDA_LANGUAGE))
                {
                    SdaLanguageFilter filter = new SdaLanguageFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new SdaGetListBehavior(param, (filterQuery == null ? filter : filterQuery), SdaRequestUriStore.SDA_LANGUAGE_GET);
                }
                //else if (type == typeof(SDA.EFMODEL.DataModels.SDA_TRANSLATE))
                //{
                //    SdaTranslateFilter filter = new SdaTranslateFilter();
                //    //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                //    var language = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_LANGUAGE>();
                //    var lang = language.FirstOrDefault(o => o.LANGUAGE_CODE.ToUpper() == Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage().ToUpper() && o.IS_BASE != 1);
                //    if (lang != null)
                //    {
                //        filter.LANGUAGE_ID = lang.ID;
                //    }
                //    else
                //    {
                //        filter.LANGUAGE_ID = -1;
                //    }
                //    result = new SdaGetListBehavior(param, filter, SdaRequestUriStore.SDA_TRANSLATE_GET);
                //}
                else if (type == typeof(SDA.EFMODEL.DataModels.SDA_MODULE_FIELD))
                {
                    SdaModuleFieldFilter filter = new SdaModuleFieldFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new SdaGetListBehavior(param, (filterQuery == null ? filter : filterQuery), SdaRequestUriStore.SDA_MODULE_FIELD_GET);
                }
                #endregion

                #region SAR
                else if (type == typeof(SAR.EFMODEL.DataModels.SAR_REPORT_TEMPLATE))
                {
                    SAR.Filter.SarReportTemplateFilter filter = new SAR.Filter.SarReportTemplateFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new SarGetListBehavior(param, (filterQuery == null ? filter : filterQuery), SarRequestUriStore.SAR_REPORT_TEMPLATE_GET);
                }
                else if (type == typeof(SAR.EFMODEL.DataModels.SAR_REPORT_TYPE))
                {
                    SAR.Filter.SarReportTypeFilter filter = new SAR.Filter.SarReportTypeFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new SarGetListBehavior(param, (filterQuery == null ? filter : filterQuery), SarRequestUriStore.SAR_REPORT_TYPE_GET);
                }
                else if (type == typeof(SAR.EFMODEL.DataModels.SAR_REPORT_STT))
                {
                    SAR.Filter.SarReportSttFilter filter = new SAR.Filter.SarReportSttFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new SarGetListBehavior(param, (filterQuery == null ? filter : filterQuery), SarRequestUriStore.SAR_REPORT_STT_GET);
                }
                else if (type == typeof(SAR.EFMODEL.DataModels.SAR_PRINT_TYPE))
                {
                    SAR.Filter.SarPrintTypeFilter filter = new SAR.Filter.SarPrintTypeFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new SarGetListBehavior(param, (filterQuery == null ? filter : filterQuery), SarRequestUriStore.SAR_PRINT_TYPE_GET);
                }
                else if (type == typeof(SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI))
                {
                    SAR.Filter.SarRetyFofiViewFilter filter = new SAR.Filter.SarRetyFofiViewFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new SarGetListBehavior(param, (filterQuery == null ? filter : filterQuery), SarRequestUriStore.SAR_RETY_FOFI_GETVIEW);
                }
                else if (type == typeof(SAR.EFMODEL.DataModels.SAR_FORM_FIELD))
                {
                    SAR.Filter.SarFormFieldFilter filter = new SAR.Filter.SarFormFieldFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new SarGetListBehavior(param, (filterQuery == null ? filter : filterQuery), SarRequestUriStore.SAR_FORM_FIELD_GET);
                }
                #endregion

                #region MOS
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_PTTT_HIGH_TECH))
                {
                    HisPtttHighTechFilter filter = new HisPtttHighTechFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), "api/HisPtttHighTech/Get");
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_EMOTIONLESS_RESULT))
                {
                    HisEmotionlessResultFilter filter = new HisEmotionlessResultFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), "api/HisEmotionlessResult/Get");
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_PTTT_PRIORITY))
                {
                    HisPtttPriorityFilter filter = new HisPtttPriorityFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), "api/HisPtttPriority/Get");
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_EXE_SERVICE_MODULE))
                {
                    HisExeServiceModuleFilter filter = new HisExeServiceModuleFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), "api/HisExeServiceModule/Get");
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_EQUIPMENT_SET))
                {
                    HisEquipmentSetFilter filter = new HisEquipmentSetFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), "api/HisEquipmentSet/Get");
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_MACHINE))
                {
                    HisMachineFilter filter = new HisMachineFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), "api/HisMachine/Get");
                }
                //else if (type == typeof(MOS.EFMODEL.DataModels.HIS_ROOM_MACHINE))
                //{
                //    HisRoomMachineFilter filter = new HisRoomMachineFilter();
                //    //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                //    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), "api/HisRoomMachine/Get");
                //}
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_SERVICE_MACHINE))
                {
                    HisServiceMachineFilter filter = new HisServiceMachineFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), "api/HisServiceMachine/Get");
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_EMPLOYEE))
                {
                    HisEmployeeFilter filter = new HisEmployeeFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), "api/HisEmployee/Get");
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_PACKAGE))
                {
                    HisPackageFilter filter = new HisPackageFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_PACKAGE_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_MEDI_ORG))
                {
                    if (!HisConfigCFG.IsUseZip)
                    {
                        HisMediOrgFilter filter = new HisMediOrgFilter();
                        //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                        if (HisConfigCFG.IsUseRedisCacheServer)
                            result = new RdCacheGetListBehavior(param, (filterQuery == null ? filter : filterQuery), RDCACHE.URI.RdCache.GET__MEDI_ORG, "api/HisMediOrg/Get", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer);
                        else
                            result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), "api/HisMediOrg/Get");
                    }
                    else
                    {
                        HisMediOrgFilter filter = new HisMediOrgFilter();
                        //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                        result = new MosGetZipListBehavior(param, (filterQuery == null ? filter : filterQuery), "api/HisMediOrg/GetZip");
                    }
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_OWE_TYPE))
                {
                    HisOweTypeFilter filter = new HisOweTypeFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_OWE_TYPE_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_EXP_MEST_REASON))
                {
                    HisExpMestReasonFilter filter = new HisExpMestReasonFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_EXP_MEST_REASON_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_MEDICINE_TYPE_TUT))
                {
                    HisMedicineTypeTutFilter filter = new HisMedicineTypeTutFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_MEDICINE_TYPE_TUT_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_MEDICINE_PATY))
                {
                    if (!HisConfigCFG.IsUseZip)
                    {
                        HisMedicinePatyFilter filter = new HisMedicinePatyFilter();
                        //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                        result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), "/api/HisMedicinePaty/Get");
                    }
                    else
                    {
                        HisMedicinePatyFilter filter = new HisMedicinePatyFilter();
                        //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                        result = new MosGetZipListBehavior(param, (filterQuery == null ? filter : filterQuery), "/api/HisMedicinePaty/GetZip");
                    }
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_MEDICINE_GROUP))
                {
                    HisMedicineGroupFilter filter = new HisMedicineGroupFilter();
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_MEDICINE_GROUP_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_ICD_CM))
                {
                    HisIcdCmFilter filter = new HisIcdCmFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_ICD_CM_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_MEST_METY_DEPA))
                {
                    HisMestMetyDepaFilter filter = new HisMestMetyDepaFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_MEST_METY_DEPA_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_MEDI_STOCK_MATY))
                {
                    HisMediStockMatyFilter filter = new HisMediStockMatyFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_MEDI_STOCK_MATY_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK_MATY))
                {
                    HisMediStockMatyViewFilter filter = new HisMediStockMatyViewFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_MEDI_STOCK_MATY_GETVIEW);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_MEDI_STOCK_METY))
                {
                    HisMediStockMetyFilter filter = new HisMediStockMetyFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_MEDI_STOCK_METY_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK_METY))
                {
                    HisMediStockMetyViewFilter filter = new HisMediStockMetyViewFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_MEDI_STOCK_METY_GETVIEW);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_ACIN_INTERACTIVE))
                {
                    HisAcinInteractiveFilter filter = new HisAcinInteractiveFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_ACIN_INTERACTIVE_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.V_HIS_ACIN_INTERACTIVE))
                {
                    HisAcinInteractiveViewFilter filter = new HisAcinInteractiveViewFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_ACIN_INTERACTIVE_GETVIEW);
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
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_HTU))
                {
                    HisHtuFilter filter = new HisHtuFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_HTU_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_BRANCH))
                {
                    HisBranchFilter filter = new HisBranchFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_BRANCH_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.V_HIS_USER_ROOM))
                {
                    if (!HisConfigCFG.IsUseZip)
                    {
                        HisUserRoomViewFilter filter = new HisUserRoomViewFilter();
                        //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                        //filter.IS_PAUSE = IMSys.DbConfig.HIS_RS.HIS_ROOM.IS_PAUSE__TRUE;
                        filter.LOGINNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                        result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), "api/HisUserRoom/GetView");
                    }
                    else
                    {
                        HisUserRoomViewFilter filter = new HisUserRoomViewFilter();
                        //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                        //filter.IS_PAUSE = IMSys.DbConfig.HIS_RS.HIS_ROOM.IS_PAUSE__TRUE;
                        filter.LOGINNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                        result = new MosGetZipListBehavior(param, (filterQuery == null ? filter : filterQuery), "api/HisUserRoom/GetViewZip");
                    }
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_MEST_PATIENT_TYPE))
                {
                    MOS.Filter.HisMestPatientTypeFilter filter = new MOS.Filter.HisMestPatientTypeFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_MEST_PATIENT_TYPE_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.V_HIS_BED_ROOM))
                {
                    MOS.Filter.HisBedRoomViewFilter filter = new MOS.Filter.HisBedRoomViewFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_BED_ROOM_GETVIEW);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.V_HIS_BED))
                {
                    MOS.Filter.HisBedViewFilter filter = new MOS.Filter.HisBedViewFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_BED_GETVIEW);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_EXP_MEST_TEMPLATE))
                {
                    MOS.Filter.HisExpMestTemplateFilter filter = new MOS.Filter.HisExpMestTemplateFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_EXP_MEST_TEMPLATE_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM))
                {
                    MOS.Filter.HisExecuteRoomViewFilter filter = new MOS.Filter.HisExecuteRoomViewFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_EXECUTE_ROOM_GETVIEW);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_EXECUTE_ROOM))
                {
                    MOS.Filter.HisExecuteRoomFilter filter = new MOS.Filter.HisExecuteRoomFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_EXECUTE_ROOM_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_DEPARTMENT))
                {
                    MOS.Filter.HisDepartmentFilter filter = new MOS.Filter.HisDepartmentFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_DEPARTMENT_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_KSK_CONTRACT))
                {
                    MOS.Filter.HisKskContractFilter filter = new MOS.Filter.HisKskContractFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_KSK_CONTRACT_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_CAREER))
                {
                    MOS.Filter.HisCareerFilter filter = new MOS.Filter.HisCareerFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_CAREER_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_DEATH_CAUSE))
                {
                    MOS.Filter.HisDeathCauseFilter filter = new MOS.Filter.HisDeathCauseFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_DEATH_CAUSE_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_DEATH_WITHIN))
                {
                    MOS.Filter.HisDeathWithinFilter filter = new MOS.Filter.HisDeathWithinFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_DEATH_WITHIN_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.V_HIS_ROOM))
                {
                    MOS.Filter.HisRoomViewFilter filter = new MOS.Filter.HisRoomViewFilter();
                    //filter.IS_PAUSE = IMSys.DbConfig.HIS_RS.HIS_ROOM.IS_PAUSE__TRUE;
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_ROOM_GETVIEW);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE))
                {
                    MOS.Filter.HisPatientTypeFilter filter = new MOS.Filter.HisPatientTypeFilter();
                    filter.ORDER_FIELD = "PATIENT_TYPE_CODE";
                    filter.ORDER_DIRECTION = "ASC";
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_PATIENT_TYPE_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_IMP_MEST_TYPE))
                {
                    MOS.Filter.HisImpMestTypeFilter filter = new MOS.Filter.HisImpMestTypeFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    filter.ORDER_FIELD = "IMP_MEST_TYPE_CODE";
                    filter.ORDER_DIRECTION = "ASC";
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_IMP_MEST_TYPE_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_IMP_MEST_STT))
                {
                    MOS.Filter.HisImpMestSttFilter filter = new MOS.Filter.HisImpMestSttFilter();
                    filter.ORDER_FIELD = "IMP_MEST_STT_CODE";
                    filter.ORDER_DIRECTION = "ASC";
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_IMP_MEST_STT_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_ROOM_TYPE))
                {
                    MOS.Filter.HisRoomTypeFilter filter = new MOS.Filter.HisRoomTypeFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_ROOM_TYPE_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALLOW))
                {
                    MOS.Filter.HisPatientTypeAllowViewFilter filter = new MOS.Filter.HisPatientTypeAllowViewFilter();
                    filter.ORDER_FIELD = "PATIENT_TYPE_CODE";
                    filter.ORDER_DIRECTION = "ASC";
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_PATIENT_TYPE_ALLOW_GETVIEW);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALLOW))
                {
                    MOS.Filter.HisPatientTypeAllowFilter filter = new MOS.Filter.HisPatientTypeAllowFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_PATIENT_TYPE_ALLOW_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_ICD))
                {
                    if (!HisConfigCFG.IsUseZip)
                    {
                        MOS.Filter.HisIcdFilter filter = new MOS.Filter.HisIcdFilter();
                        //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                        if (HisConfigCFG.IsUseRedisCacheServer)
                            result = new RdCacheGetListBehavior(param, (filterQuery == null ? filter : filterQuery), RDCACHE.URI.RdCache.GET__ICD, "api/HisIcd/Get", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer);
                        else
                            result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), "api/HisIcd/Get");
                    }
                    else
                    {
                        MOS.Filter.HisIcdFilter filter = new MOS.Filter.HisIcdFilter();
                        //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                        result = new MosGetZipListBehavior(param, (filterQuery == null ? filter : filterQuery), "api/HisIcd/GetZip");
                    }
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_EXECUTE_GROUP))
                {
                    MOS.Filter.HisExecuteGroupFilter filter = new MOS.Filter.HisExecuteGroupFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_EXECUTE_GROUP_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM))
                {
                    MOS.Filter.HisTranPatiFormFilter filter = new MOS.Filter.HisTranPatiFormFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_TRAN_PATI_FORM_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_TRAN_PATI_REASON))
                {
                    MOS.Filter.HisTranPatiReasonFilter filter = new MOS.Filter.HisTranPatiReasonFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_TRAN_PATI_REASON_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_TRAN_PATI_TECH))
                {
                    MOS.Filter.HisTranPatiTechFilter filter = new MOS.Filter.HisTranPatiTechFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_TRAN_PATI_TECH_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY))
                {
                    if (!HisConfigCFG.IsUseZip)
                    {
                        MOS.Filter.HisServicePatyViewFilter filter = new MOS.Filter.HisServicePatyViewFilter();
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
                        MOS.Filter.HisServicePatyViewFilter filter = new MOS.Filter.HisServicePatyViewFilter();
                        //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                        //Neu da chon chi nhanh thi chi lay chinh sach gia theo chi nhanh da chon
                        if (BranchDataWorker.GetCurrentBranchId() > 0)
                            filter.BRANCH_ID = BranchDataWorker.GetCurrentBranchId();
                        result = new MosGetZipListBehavior(param, (filterQuery == null ? filter : filterQuery), "api/HisServicePaty/GetViewZip");
                    }
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_STT))
                {
                    MOS.Filter.HisServiceReqSttFilter filter = new MOS.Filter.HisServiceReqSttFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_SERVICE_REQ_STT_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_TYPE))
                {
                    MOS.Filter.HisServiceReqTypeFilter filter = new MOS.Filter.HisServiceReqTypeFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_SERVICE_REQ_TYPE_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_SERVICE_UNIT))
                {
                    MOS.Filter.HisServiceUnitFilter filter = new MOS.Filter.HisServiceUnitFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_SERVICE_UNIT_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_GENDER))
                {
                    MOS.Filter.HisGenderFilter filter = new MOS.Filter.HisGenderFilter();
                    filter.ORDER_FIELD = "GENDER_CODE";
                    filter.ORDER_DIRECTION = "ASC";
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_GENDER_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_TYPE))
                {
                    MOS.Filter.HisHeinServiceTypeFilter filter = new HisHeinServiceTypeFilter();
                    filter.ORDER_FIELD = "HEIN_SERVICE_TYPE_CODE";
                    filter.ORDER_DIRECTION = "ASC";
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_HEIN_SERVICE_TYPE_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_MEDICINE_LINE))
                {
                    MOS.Filter.HisMedicineLineFilter filter = new MOS.Filter.HisMedicineLineFilter();
                    filter.ORDER_FIELD = "MEDICINE_LINE_CODE";
                    filter.ORDER_DIRECTION = "ASC";
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_MEDICINE_LINE_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_MILITARY_RANK))
                {
                    MOS.Filter.HisMaterialTypeViewFilter filter = new MOS.Filter.HisMaterialTypeViewFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_MILITARY_RANK_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_MEDICINE_USE_FORM))
                {
                    MOS.Filter.HisMedicineUseFormFilter filter = new MOS.Filter.HisMedicineUseFormFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_MEDICINE_USE_FORM_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_MEDI_REACT_TYPE))
                {
                    MOS.Filter.HisMediReactTypeFilter filter = new MOS.Filter.HisMediReactTypeFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_MEDI_REACT_TYPE_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_EXP_MEST_STT))
                {
                    MOS.Filter.HisExpMestSttFilter filter = new MOS.Filter.HisExpMestSttFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    filter.ORDER_FIELD = "EXP_MEST_STT_CODE";
                    filter.ORDER_DIRECTION = "ASC";
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_EXP_MEST_STT_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_TREATMENT_END_TYPE))
                {
                    MOS.Filter.HisTreatmentEndTypeFilter filter = new MOS.Filter.HisTreatmentEndTypeFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_TREATMENT_END_TYPE_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_TREATMENT_RESULT))
                {
                    MOS.Filter.HisTreatmentResultFilter filter = new MOS.Filter.HisTreatmentResultFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_TREATMENT_RESULT_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.V_HIS_SERVICE_PACKAGE))
                {
                    MOS.Filter.HisServicePackageViewFilter filter = new MOS.Filter.HisServicePackageViewFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_SERVICE_PACKAGE_GETVIEW);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.V_HIS_SERVICE))
                {
                    if (!HisConfigCFG.IsUseZip)
                    {
                        MOS.Filter.HisServiceViewFilter filter = new MOS.Filter.HisServiceViewFilter();
                        ////filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                        if (HisConfigCFG.IsUseRedisCacheServer)
                            result = new RdCacheGetListBehavior(param, (filterQuery == null ? filter : filterQuery), RDCACHE.URI.RdCache.GET__SERVICE, "api/HisService/GetView", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer);
                        else
                            result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), "api/HisService/GetView");
                    }
                    else
                    {
                        MOS.Filter.HisServiceViewFilter filter = new MOS.Filter.HisServiceViewFilter();
                        ////filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                        result = new MosGetZipListBehavior(param, (filterQuery == null ? filter : filterQuery), "api/HisService/GetViewZip");
                    }
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM))
                {
                    if (!HisConfigCFG.IsUseZip)
                    {
                        MOS.Filter.HisServiceRoomViewFilter filter = new MOS.Filter.HisServiceRoomViewFilter();
                        //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                        if (HisConfigCFG.IsUseRedisCacheServer)
                            result = new RdCacheGetListBehavior(param, (filterQuery == null ? filter : filterQuery), RDCACHE.URI.RdCache.GET__SERVICE_ROOM, "api/HisServiceRoom/GetView", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer);
                        else
                            result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), "api/HisServiceRoom/GetView");
                    }
                    else
                    {
                        MOS.Filter.HisServiceRoomViewFilter filter = new MOS.Filter.HisServiceRoomViewFilter();
                        //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                        result = new MosGetZipListBehavior(param, (filterQuery == null ? filter : filterQuery), "api/HisServiceRoom/GetViewZip");
                    }
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_SERVICE_GROUP))
                {
                    MOS.Filter.HisServiceGroupFilter filter = new MOS.Filter.HisServiceGroupFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_SERVICE_GROUP_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.V_HIS_SERV_SEGR))
                {
                    MOS.Filter.HisServSegrViewFilter filter = new MOS.Filter.HisServSegrViewFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_SERV_SEGR_GETVIEW);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_SERVICE_TYPE))
                {
                    MOS.Filter.HisServiceTypeFilter filter = new MOS.Filter.HisServiceTypeFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_SERVICE_TYPE_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_MEDI_STOCK))
                {
                    MOS.Filter.HisMediStockFilter filter = new MOS.Filter.HisMediStockFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_MEDI_STOCK_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK))
                {
                    MOS.Filter.HisMediStockViewFilter filter = new MOS.Filter.HisMediStockViewFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_MEDI_STOCK_GETVIEW);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_EXP_MEST_TYPE))
                {
                    MOS.Filter.HisExpMestTypeFilter filter = new MOS.Filter.HisExpMestTypeFilter();
                    filter.ORDER_FIELD = "EXP_MEST_TYPE_CODE";
                    filter.ORDER_DIRECTION = "ASC";
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_EXP_MEST_TYPE_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_MEDICINE_TYPE))
                {
                    MOS.Filter.HisMedicineTypeFilter filter = new MOS.Filter.HisMedicineTypeFilter();
                    filter.ORDER_FIELD = "MEDICINE_TYPE_CODE";
                    filter.ORDER_DIRECTION = "ASC";
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_MEDICINE_TYPE_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE))
                {
                    if (!HisConfigCFG.IsUseZip)
                    {
                        MOS.Filter.HisMedicineTypeViewFilter filter = new MOS.Filter.HisMedicineTypeViewFilter();
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
                        MOS.Filter.HisMedicineTypeViewFilter filter = new MOS.Filter.HisMedicineTypeViewFilter();
                        filter.ORDER_FIELD = "MEDICINE_TYPE_CODE";
                        filter.ORDER_DIRECTION = "ASC";
                        //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                        result = new MosGetZipListBehavior(param, (filterQuery == null ? filter : filterQuery), "api/HisMedicineType/GetViewZip");
                    }
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_MANUFACTURER))
                {
                    MOS.Filter.HisManufacturerFilter filter = new MOS.Filter.HisManufacturerFilter();
                    filter.ORDER_FIELD = "MANUFACTURER_CODE";
                    filter.ORDER_DIRECTION = "ASC";
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_MANUFACTURER_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE_ACIN))
                {
                    MOS.Filter.HisMedicineTypeAcinViewFilter filter = new MOS.Filter.HisMedicineTypeAcinViewFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_MEDICINE_TYPE_ACIN_GETVIEW);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_PACKING_TYPE))
                {
                    MOS.Filter.HisPackingTypeFilter filter = new MOS.Filter.HisPackingTypeFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_PACKING_TYPE_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.V_HIS_TEST_INDEX_RANGE))
                {
                    MOS.Filter.HisTestIndexRangeViewFilter filter = new MOS.Filter.HisTestIndexRangeViewFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_TEST_INDEX_RANGE_GETVIEW);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.V_HIS_TEST_INDEX))
                {
                    MOS.Filter.HisTestIndexViewFilter filter = new MOS.Filter.HisTestIndexViewFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_TEST_INDEX_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE))
                {
                    MOS.Filter.HisTreatmentTypeFilter filter = new MOS.Filter.HisTreatmentTypeFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_TREATMENT_TYPE_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.V_HIS_MATERIAL_TYPE))
                {
                    if (!HisConfigCFG.IsUseZip)
                    {
                        MOS.Filter.HisMaterialTypeViewFilter filter = new MOS.Filter.HisMaterialTypeViewFilter();
                        //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                        if (HisConfigCFG.IsUseRedisCacheServer)
                            result = new RdCacheGetListBehavior(param, (filterQuery == null ? filter : filterQuery), RDCACHE.URI.RdCache.GET__MATERIAL_TYPE, "api/HisMaterialType/GetView", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer);
                        else
                            result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), "api/HisMaterialType/GetView");
                    }
                    else
                    {
                        MOS.Filter.HisMaterialTypeViewFilter filter = new MOS.Filter.HisMaterialTypeViewFilter();
                        //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                        result = new MosGetZipListBehavior(param, (filterQuery == null ? filter : filterQuery), "api/HisMaterialType/GetViewZip");
                    }
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_MATERIAL_TYPE))
                {
                    MOS.Filter.HisMaterialTypeFilter filter = new MOS.Filter.HisMaterialTypeFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_MATERIAL_TYPE_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_MILITARY_RANK))
                {
                    MOS.Filter.HisMilitaryRankFilter filter = new MOS.Filter.HisMilitaryRankFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_MILITARY_RANK_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_WORK_PLACE))
                {
                    MOS.Filter.HisWorkPlaceFilter filter = new MOS.Filter.HisWorkPlaceFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_WORK_PLACE_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_EMERGENCY_WTIME))
                {
                    MOS.Filter.HisEmergencyWtimeFilter filter = new MOS.Filter.HisEmergencyWtimeFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_EMERGENCY_WTIME_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_REHA_TRAIN_TYPE))
                {
                    MOS.Filter.HisRehaTrainTypeFilter filter = new MOS.Filter.HisRehaTrainTypeFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_REHA_TRAIN_TYPE_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_BLOOD_ABO))
                {
                    MOS.Filter.HisBloodAboFilter filter = new MOS.Filter.HisBloodAboFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_BLOOD_ABO__GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_BLOOD_RH))
                {
                    MOS.Filter.HisBloodRhFilter filter = new MOS.Filter.HisBloodRhFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_BLOOD_RH__GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_PAY_FORM))
                {
                    MOS.Filter.HisPayFormFilter filter = new MOS.Filter.HisPayFormFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_PAY_FORM_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_BLOOD_TYPE))
                {
                    MOS.Filter.HisBloodTypeFilter filter = new MOS.Filter.HisBloodTypeFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_BLOOD_TYPE_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.V_HIS_BLOOD_TYPE))
                {
                    MOS.Filter.HisBloodTypeViewFilter filter = new MOS.Filter.HisBloodTypeViewFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_BLOOD_TYPE_GETVIEW);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_SUPPLIER))
                {
                    MOS.Filter.HisSupplierFilter filter = new MOS.Filter.HisSupplierFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_SUPPLIER_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_IMP_SOURCE))
                {
                    MOS.Filter.HisImpSourceFilter filter = new MOS.Filter.HisImpSourceFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_IMP_SOURCE_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_TRANSACTION_TYPE))
                {
                    HisTransactionTypeFilter filter = new HisTransactionTypeFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_TRANSACTION_TYPE_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_BORN_TYPE))
                {
                    HisBornTypeFilter filter = new HisBornTypeFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_BORN_TYPE_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_BORN_POSITION))
                {
                    HisBornPositionFilter filter = new HisBornPositionFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_BORN_POSITION_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_BORN_RESULT))
                {
                    HisBornResultFilter filter = new HisBornResultFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_BORN_RESULT_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM))
                {
                    HisMestRoomViewFilter filter = new HisMestRoomViewFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), "api/HisMestRoom/GetView");
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_ANTICIPATE))
                {
                    HisAnticipateFilter filter = new HisAnticipateFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_ANTICIPATE_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_ACCIDENT_HURT_TYPE))
                {
                    HisAccidentHurtTypeFilter filter = new HisAccidentHurtTypeFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_ACCIDENT_HURT_TYPE_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.V_HIS_CASHIER_ROOM))
                {
                    HisCashierRoomViewFilter filter = new HisCashierRoomViewFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_CASHIER_ROOM_GETVIEW);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_CASHIER_ROOM))
                {
                    HisCashierRoomFilter filter = new HisCashierRoomFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_CASHIER_ROOM_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_FUND))
                {
                    HisFundFilter filter = new HisFundFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_FUND_GET);
                }
                //else if (type == typeof(MOS.EFMODEL.DataModels.V_HIS_TREATMENT_BED_ROOM))
                //{
                //    HisTreatmentBedRoomViewFilter filter = new HisTreatmentBedRoomViewFilter();
                //    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                //    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_TREATMENT_BED_ROOM_GETVIEW);
                //}
                //else if (type == typeof(MOS.EFMODEL.DataModels.V_HIS_ROOM_COUNTER))
                //{
                //    HisRoomCounterViewFilter filter = new HisRoomCounterViewFilter();
                //    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                //    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_ROOM_GETVIEW_COUNTER);
                //}
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_SERVICE_FOLLOW))
                {
                    HisServiceFollowFilter filter = new HisServiceFollowFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_SERVICE_FOLLOW_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.V_HIS_SERVICE_FOLLOW))
                {
                    HisServiceFollowViewFilter filter = new HisServiceFollowViewFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_SERVICE_FOLLOW_GETVIEW);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_PTTT_METHOD))
                {
                    HisPtttMethodFilter filter = new HisPtttMethodFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_PTTT_METHOD_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_PTTT_GROUP))
                {
                    HisPtttGroupFilter filter = new HisPtttGroupFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_PTTT_GROUP_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_EMOTIONLESS_METHOD))
                {
                    HisEmotionlessMethodFilter filter = new HisEmotionlessMethodFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_EMOTIONLESS_METHOD_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_PTTT_CONDITION))
                {
                    HisPtttConditionFilter filter = new HisPtttConditionFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_PTTT_CONDITION_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_PTTT_CATASTROPHE))
                {
                    HisPtttCatastropheFilter filter = new HisPtttCatastropheFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_PTTT_CATASTROPHE_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE))
                {
                    HisExecuteRoleFilter filter = new HisExecuteRoleFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_EXECUTE_ROLE_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_BHYT_BLACKLIST))
                {
                    HisBhytBlacklistFilter filter = new HisBhytBlacklistFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_BHYT_BLACKLIST_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_BHYT_WHITELIST))
                {
                    HisBhytWhitelistFilter filter = new HisBhytWhitelistFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_BHYT_WHITELIST_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_PAAN_POSITION))
                {
                    HisPaanPositionFilter filter = new HisPaanPositionFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), "api/HisPaanPosition/Get");
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_PAAN_LIQUID))
                {
                    HisPaanLiquidFilter filter = new HisPaanLiquidFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), "api/HisPaanLiquid/Get");
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_REPAY_REASON))
                {
                    HisRepayReasonFilter filter = new HisRepayReasonFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), "api/HisRepayReason/Get");
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_MEDICINE_TYPE_ROOM))
                {
                    HisMedicineTypeRoomFilter filter = new HisMedicineTypeRoomFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), "api/HisMedicineTypeRoom/Get");
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_SERE_SERV_TEMP))
                {
                    MOS.Filter.HisSereServTempFilter filter = new MOS.Filter.HisSereServTempFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), "api/HisSereServTemp/Get");
                }
                #endregion

                #region HTC
                else if (type == typeof(HTC.EFMODEL.DataModels.HTC_PERIOD))
                {
                    HTC.Filter.HtcPeriodFilter filter = new HTC.Filter.HtcPeriodFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new HtcGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HtcRequestUriStore.HTC_PERIOD__GET);
                }
                else if (type == typeof(HTC.EFMODEL.DataModels.HTC_EXPENSE_TYPE))
                {
                    HTC.Filter.HtcExpenseTypeFilter filter = new HTC.Filter.HtcExpenseTypeFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new HtcGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HtcRequestUriStore.HTC_EXPENSE_TYPE__GET);
                }
                #endregion

                #region LIS
                else if (type == typeof(LIS.EFMODEL.DataModels.LIS_SAMPLE_STT))
                {
                    LIS.Filter.LisSampleSttFilter filter = new LIS.Filter.LisSampleSttFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new LisGetListBehavior(param, (filterQuery == null ? filter : filterQuery), LisRequestUriStore.LIS_SAMPLE_STT_GET);
                }
                //else if (type == typeof(HTC.EFMODEL.DataModels.HTC_EXPENSE_TYPE))
                //{
                //    HTC.Filter.HtcExpenseTypeFilter filter = new HTC.Filter.HtcExpenseTypeFilter();
                //    result = new HtcGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HtcRequestUriStore.HTC_EXPENSE_TYPE__GET);
                //}
                //else if (type == typeof(MOS.EFMODEL.DataModels.V_HIS_DIIM_SERVICE_TYPE))
                //{
                //    HisDiimServiceTypeViewFilter filter = new HisDiimServiceTypeViewFilter();
                //    result = new MosGetListBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_DIIM_SERVICE_TYPE_GETVIEW);
                //}
                #endregion

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

        internal static IGetDataTAsync MakeIGetListAsync(CommonParam param, Type type, object filterQuery)
        {
            IGetDataTAsync result = null;
            try
            {
                #region ACS
                if (type == typeof(ACS.EFMODEL.DataModels.ACS_USER))
                {
                    AcsUserFilter filter = new AcsUserFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.ACS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new AcsGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), AcsRequestUriStore.ACS_USER_GET);
                }

                else if (type == typeof(ACS.EFMODEL.DataModels.ACS_APPLICATION))
                {
                    AcsApplicationFilter filter = new AcsApplicationFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.ACS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new AcsGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), "api/AcsApplication/Get");
                }
                #endregion

                #region SDA
                //else if (type == typeof(CommuneADO))
                //{
                //    result = new SdaCommuneADOGetListBehavior(param);
                //}
                //else if (type == typeof(SDA.EFMODEL.DataModels.SDA_COMMUNE))
                //{
                //    SdaCommuneFilter filter = new SdaCommuneFilter();
                //    //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                //    result = new SdaGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), SdaRequestUriStore.SDA_COMMUNE_GETVIEW);
                //}
                //else if (type == typeof(SDA.EFMODEL.DataModels.SDA_DISTRICT))
                //{
                //    SdaDistrictFilter filter = new SdaDistrictFilter();
                //    //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                //    result = new SdaGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), SdaRequestUriStore.SDA_DISTRICT_GETVIEW);
                //}
                //else if (type == typeof(SDA.EFMODEL.DataModels.SDA_PROVINCE))
                //{
                //    SdaProvinceFilter filter = new SdaProvinceFilter();
                //    //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                //    result = new SdaGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), SdaRequestUriStore.SDA_PROVINCE_GETVIEW);
                //}
                else if (type == typeof(SDA.EFMODEL.DataModels.V_SDA_COMMUNE))
                {
                    SdaCommuneViewFilter filter = new SdaCommuneViewFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new SdaGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), SdaRequestUriStore.SDA_COMMUNE_GETVIEW);
                }
                else if (type == typeof(SDA.EFMODEL.DataModels.V_SDA_DISTRICT))
                {
                    SdaDistrictViewFilter filter = new SdaDistrictViewFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new SdaGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), SdaRequestUriStore.SDA_DISTRICT_GETVIEW);
                }
                else if (type == typeof(SDA.EFMODEL.DataModels.V_SDA_PROVINCE))
                {
                    SdaProvinceViewFilter filter = new SdaProvinceViewFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new SdaGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), SdaRequestUriStore.SDA_PROVINCE_GETVIEW);
                }
                else if (type == typeof(SDA.EFMODEL.DataModels.SDA_NATIONAL))
                {
                    SdaNationalFilter filter = new SdaNationalFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new SdaGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), SdaRequestUriStore.SDA_NATIONAL_GET);
                }
                else if (type == typeof(SDA.EFMODEL.DataModels.SDA_GROUP))
                {
                    SdaGroupFilter filter = new SdaGroupFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new SdaGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), SdaRequestUriStore.SDA_GROUP_GET);
                }
                else if (type == typeof(SDA.EFMODEL.DataModels.SDA_ETHNIC))
                {
                    SdaEthnicFilter filter = new SdaEthnicFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new SdaGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), SdaRequestUriStore.SDA_ETHNIC_GET);
                }
                //else if (type == typeof(SDA.EFMODEL.DataModels.SDA_LANGUAGE))
                //{
                //    SdaLanguageFilter filter = new SdaLanguageFilter();
                //    //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                //    result = new SdaGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), SdaRequestUriStore.SDA_LANGUAGE_GET);
                //}
                //else if (type == typeof(SDA.EFMODEL.DataModels.SDA_TRANSLATE))
                //{
                //    SdaTranslateFilter filter = new SdaTranslateFilter();
                //    //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                //    var language = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_LANGUAGE>();
                //    var lang = language.FirstOrDefault(o => o.LANGUAGE_CODE.ToUpper() == Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage().ToUpper() && o.IS_BASE != 1);
                //    if (lang != null)
                //    {
                //        filter.LANGUAGE_ID = lang.ID;
                //    }
                //    else
                //    {
                //        filter.LANGUAGE_ID = -1;
                //    }
                //    result = new SdaGetListAsyncBehavior(param, filter, SdaRequestUriStore.SDA_TRANSLATE_GET);
                //}
                else if (type == typeof(SDA.EFMODEL.DataModels.SDA_MODULE_FIELD))
                {
                    SdaModuleFieldFilter filter = new SdaModuleFieldFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new SdaGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), SdaRequestUriStore.SDA_MODULE_FIELD_GET);
                }
                #endregion

                #region SAR
                else if (type == typeof(SAR.EFMODEL.DataModels.SAR_REPORT_TEMPLATE))
                {
                    SAR.Filter.SarReportTemplateFilter filter = new SAR.Filter.SarReportTemplateFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new SarGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), SarRequestUriStore.SAR_REPORT_TEMPLATE_GET);
                }
                else if (type == typeof(SAR.EFMODEL.DataModels.SAR_REPORT_TYPE))
                {
                    SAR.Filter.SarReportTypeFilter filter = new SAR.Filter.SarReportTypeFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new SarGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), SarRequestUriStore.SAR_REPORT_TYPE_GET);
                }
                else if (type == typeof(SAR.EFMODEL.DataModels.SAR_REPORT_STT))
                {
                    SAR.Filter.SarReportSttFilter filter = new SAR.Filter.SarReportSttFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new SarGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), SarRequestUriStore.SAR_REPORT_STT_GET);
                }
                else if (type == typeof(SAR.EFMODEL.DataModels.SAR_PRINT_TYPE))
                {
                    SAR.Filter.SarPrintTypeFilter filter = new SAR.Filter.SarPrintTypeFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new SarGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), SarRequestUriStore.SAR_PRINT_TYPE_GET);
                }
                else if (type == typeof(SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI))
                {
                    SAR.Filter.SarRetyFofiViewFilter filter = new SAR.Filter.SarRetyFofiViewFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new SarGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), SarRequestUriStore.SAR_RETY_FOFI_GETVIEW);
                }
                else if (type == typeof(SAR.EFMODEL.DataModels.SAR_FORM_FIELD))
                {
                    SAR.Filter.SarFormFieldFilter filter = new SAR.Filter.SarFormFieldFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new SarGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), SarRequestUriStore.SAR_FORM_FIELD_GET);
                }
                #endregion

                #region MOS
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_EQUIPMENT_SET))
                {
                    HisEquipmentSetFilter filter = new HisEquipmentSetFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), "api/HisEquipmentSet/Get");
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_MACHINE))
                {
                    HisMachineFilter filter = new HisMachineFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), "api/HisMachine/Get");
                }
                //else if (type == typeof(MOS.EFMODEL.DataModels.HIS_ROOM_MACHINE))
                //{
                //    HisRoomMachineFilter filter = new HisRoomMachineFilter();
                //    //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                //    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), "api/HisRoomMachine/Get");
                //}
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_SERVICE_MACHINE))
                {
                    HisServiceMachineFilter filter = new HisServiceMachineFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), "api/HisServiceMachine/Get");
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_EMPLOYEE))
                {
                    HisEmployeeFilter filter = new HisEmployeeFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), "api/HisEmployee/Get");
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_PACKAGE))
                {
                    HisPackageFilter filter = new HisPackageFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_PACKAGE_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_MEDI_ORG))
                {
                    HisMediOrgFilter filter = new HisMediOrgFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_MEDI_ORG_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_OWE_TYPE))
                {
                    HisOweTypeFilter filter = new HisOweTypeFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_OWE_TYPE_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_EXP_MEST_REASON))
                {
                    HisExpMestReasonFilter filter = new HisExpMestReasonFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_EXP_MEST_REASON_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_MEDICINE_TYPE_TUT))
                {
                    HisMedicineTypeTutFilter filter = new HisMedicineTypeTutFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_MEDICINE_TYPE_TUT_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_MEDICINE_PATY))
                {
                    HisMedicinePatyFilter filter = new HisMedicinePatyFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_MEDICINE_PATY_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_MEDICINE_GROUP))
                {
                    HisMedicineGroupFilter filter = new HisMedicineGroupFilter();
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_MEDICINE_GROUP_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_ICD_CM))
                {
                    HisIcdCmFilter filter = new HisIcdCmFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_ICD_CM_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_MEST_METY_DEPA))
                {
                    HisMestMetyDepaFilter filter = new HisMestMetyDepaFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_MEST_METY_DEPA_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_MEDI_STOCK_MATY))
                {
                    HisMediStockMatyFilter filter = new HisMediStockMatyFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_MEDI_STOCK_MATY_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK_MATY))
                {
                    HisMediStockMatyViewFilter filter = new HisMediStockMatyViewFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_MEDI_STOCK_MATY_GETVIEW);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_MEDI_STOCK_METY))
                {
                    HisMediStockMetyFilter filter = new HisMediStockMetyFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_MEDI_STOCK_METY_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK_METY))
                {
                    HisMediStockMetyViewFilter filter = new HisMediStockMetyViewFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_MEDI_STOCK_METY_GETVIEW);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_ACIN_INTERACTIVE))
                {
                    HisAcinInteractiveFilter filter = new HisAcinInteractiveFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_ACIN_INTERACTIVE_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.V_HIS_ACIN_INTERACTIVE))
                {
                    HisAcinInteractiveViewFilter filter = new HisAcinInteractiveViewFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_ACIN_INTERACTIVE_GETVIEW);
                }
                //else if (type == typeof(AgeADO))
                //{
                //    result = new HisAgeGetListBehavior(param);
                //}
                //else if (type == typeof(ServiceComboADO))
                //{
                //    result = new HisServiceComboGetBehavior(param);
                //}
                //else if (type == typeof(MedicineMaterialTypeComboADO))
                //{
                //    result = new MedicineMaterialTypeComboGetBehavior(param);
                //}
                //else if (type == typeof(MOS.LibraryHein.Bhyt.HeinLiveArea.HeinLiveAreaData))
                //{
                //    result = new HeinLiveAreaGetListBehavior(param);
                //}
                //else if (type == typeof(MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeData))
                //{
                //    result = new HeinRightRouteTypeGetListBehavior(param);
                //}
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_HTU))
                {
                    HisHtuFilter filter = new HisHtuFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_HTU_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_BRANCH))
                {
                    HisBranchFilter filter = new HisBranchFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_BRANCH_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.V_HIS_USER_ROOM))
                {
                    HisUserRoomViewFilter filter = new HisUserRoomViewFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    //filter.IS_PAUSE = IMSys.DbConfig.HIS_RS.HIS_ROOM.IS_PAUSE__TRUE;
                    filter.LOGINNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_USER_ROOM_GETVIEW);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_MEST_PATIENT_TYPE))
                {
                    MOS.Filter.HisMestPatientTypeFilter filter = new MOS.Filter.HisMestPatientTypeFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_MEST_PATIENT_TYPE_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.V_HIS_BED_ROOM))
                {
                    MOS.Filter.HisBedRoomViewFilter filter = new MOS.Filter.HisBedRoomViewFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_BED_ROOM_GETVIEW);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.V_HIS_BED))
                {
                    MOS.Filter.HisBedViewFilter filter = new MOS.Filter.HisBedViewFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_BED_GETVIEW);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_EXP_MEST_TEMPLATE))
                {
                    MOS.Filter.HisExpMestTemplateFilter filter = new MOS.Filter.HisExpMestTemplateFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_EXP_MEST_TEMPLATE_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM))
                {
                    MOS.Filter.HisExecuteRoomViewFilter filter = new MOS.Filter.HisExecuteRoomViewFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_EXECUTE_ROOM_GETVIEW);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_EXECUTE_ROOM))
                {
                    MOS.Filter.HisExecuteRoomFilter filter = new MOS.Filter.HisExecuteRoomFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_EXECUTE_ROOM_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_DEPARTMENT))
                {
                    MOS.Filter.HisDepartmentFilter filter = new MOS.Filter.HisDepartmentFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_DEPARTMENT_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_KSK_CONTRACT))
                {
                    MOS.Filter.HisKskContractFilter filter = new MOS.Filter.HisKskContractFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_KSK_CONTRACT_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_CAREER))
                {
                    MOS.Filter.HisCareerFilter filter = new MOS.Filter.HisCareerFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_CAREER_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_DEATH_CAUSE))
                {
                    MOS.Filter.HisDeathCauseFilter filter = new MOS.Filter.HisDeathCauseFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_DEATH_CAUSE_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_DEATH_WITHIN))
                {
                    MOS.Filter.HisDeathWithinFilter filter = new MOS.Filter.HisDeathWithinFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_DEATH_WITHIN_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.V_HIS_ROOM))
                {
                    MOS.Filter.HisRoomViewFilter filter = new MOS.Filter.HisRoomViewFilter();
                    //filter.IS_PAUSE = IMSys.DbConfig.HIS_RS.HIS_ROOM.IS_PAUSE__TRUE;
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_ROOM_GETVIEW);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE))
                {
                    MOS.Filter.HisPatientTypeFilter filter = new MOS.Filter.HisPatientTypeFilter();
                    filter.ORDER_FIELD = "PATIENT_TYPE_CODE";
                    filter.ORDER_DIRECTION = "ASC";
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_PATIENT_TYPE_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_IMP_MEST_TYPE))
                {
                    MOS.Filter.HisImpMestTypeFilter filter = new MOS.Filter.HisImpMestTypeFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    filter.ORDER_FIELD = "IMP_MEST_TYPE_CODE";
                    filter.ORDER_DIRECTION = "ASC";
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_IMP_MEST_TYPE_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_IMP_MEST_STT))
                {
                    MOS.Filter.HisImpMestSttFilter filter = new MOS.Filter.HisImpMestSttFilter();
                    filter.ORDER_FIELD = "IMP_MEST_STT_CODE";
                    filter.ORDER_DIRECTION = "ASC";
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_IMP_MEST_STT_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_ROOM_TYPE))
                {
                    MOS.Filter.HisRoomTypeFilter filter = new MOS.Filter.HisRoomTypeFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_ROOM_TYPE_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALLOW))
                {
                    MOS.Filter.HisPatientTypeAllowViewFilter filter = new MOS.Filter.HisPatientTypeAllowViewFilter();
                    filter.ORDER_FIELD = "PATIENT_TYPE_CODE";
                    filter.ORDER_DIRECTION = "ASC";
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_PATIENT_TYPE_ALLOW_GETVIEW);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALLOW))
                {
                    MOS.Filter.HisPatientTypeAllowFilter filter = new MOS.Filter.HisPatientTypeAllowFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_PATIENT_TYPE_ALLOW_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_ICD))
                {
                    MOS.Filter.HisIcdFilter filter = new MOS.Filter.HisIcdFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_ICD_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_EXECUTE_GROUP))
                {
                    MOS.Filter.HisExecuteGroupFilter filter = new MOS.Filter.HisExecuteGroupFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_EXECUTE_GROUP_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM))
                {
                    MOS.Filter.HisTranPatiFormFilter filter = new MOS.Filter.HisTranPatiFormFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_TRAN_PATI_FORM_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_TRAN_PATI_REASON))
                {
                    MOS.Filter.HisTranPatiReasonFilter filter = new MOS.Filter.HisTranPatiReasonFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_TRAN_PATI_REASON_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY))
                {
                    MOS.Filter.HisServicePatyViewFilter filter = new MOS.Filter.HisServicePatyViewFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    //Neu da chon chi nhanh thi chi lay chinh sach gia theo chi nhanh da chon
                    if (BranchDataWorker.GetCurrentBranchId() > 0)
                        filter.BRANCH_ID = BranchDataWorker.GetCurrentBranchId();
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_SERVICE_PATY_GETVIEW);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_STT))
                {
                    MOS.Filter.HisServiceReqSttFilter filter = new MOS.Filter.HisServiceReqSttFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_SERVICE_REQ_STT_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_TYPE))
                {
                    MOS.Filter.HisServiceReqTypeFilter filter = new MOS.Filter.HisServiceReqTypeFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_SERVICE_REQ_TYPE_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_SERVICE_UNIT))
                {
                    MOS.Filter.HisServiceUnitFilter filter = new MOS.Filter.HisServiceUnitFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_SERVICE_UNIT_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_GENDER))
                {
                    MOS.Filter.HisGenderFilter filter = new MOS.Filter.HisGenderFilter();
                    filter.ORDER_FIELD = "GENDER_CODE";
                    filter.ORDER_DIRECTION = "ASC";
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_GENDER_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_TYPE))
                {
                    MOS.Filter.HisHeinServiceTypeFilter filter = new HisHeinServiceTypeFilter();
                    filter.ORDER_FIELD = "HEIN_SERVICE_TYPE_CODE";
                    filter.ORDER_DIRECTION = "ASC";
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_HEIN_SERVICE_TYPE_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_MEDICINE_LINE))
                {
                    MOS.Filter.HisMedicineLineFilter filter = new MOS.Filter.HisMedicineLineFilter();
                    filter.ORDER_FIELD = "MEDICINE_LINE_CODE";
                    filter.ORDER_DIRECTION = "ASC";
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_MEDICINE_LINE_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_MILITARY_RANK))
                {
                    MOS.Filter.HisMaterialTypeViewFilter filter = new MOS.Filter.HisMaterialTypeViewFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_MILITARY_RANK_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_MEDICINE_USE_FORM))
                {
                    MOS.Filter.HisMedicineUseFormFilter filter = new MOS.Filter.HisMedicineUseFormFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_MEDICINE_USE_FORM_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_MEDI_REACT_TYPE))
                {
                    MOS.Filter.HisMediReactTypeFilter filter = new MOS.Filter.HisMediReactTypeFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_MEDI_REACT_TYPE_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_EXP_MEST_STT))
                {
                    MOS.Filter.HisExpMestSttFilter filter = new MOS.Filter.HisExpMestSttFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    filter.ORDER_FIELD = "EXP_MEST_STT_CODE";
                    filter.ORDER_DIRECTION = "ASC";
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_EXP_MEST_STT_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_TREATMENT_END_TYPE))
                {
                    MOS.Filter.HisTreatmentEndTypeFilter filter = new MOS.Filter.HisTreatmentEndTypeFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_TREATMENT_END_TYPE_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_TREATMENT_RESULT))
                {
                    MOS.Filter.HisTreatmentResultFilter filter = new MOS.Filter.HisTreatmentResultFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_TREATMENT_RESULT_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.V_HIS_SERVICE_PACKAGE))
                {
                    MOS.Filter.HisServicePackageViewFilter filter = new MOS.Filter.HisServicePackageViewFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_SERVICE_PACKAGE_GETVIEW);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.V_HIS_SERVICE))
                {
                    MOS.Filter.HisServiceViewFilter filter = new MOS.Filter.HisServiceViewFilter();
                    ////filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_SERVICE_GETVIEW);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM))
                {
                    MOS.Filter.HisServiceRoomViewFilter filter = new MOS.Filter.HisServiceRoomViewFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_SERVICE_ROOM_GETVIEW);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_SERVICE_GROUP))
                {
                    MOS.Filter.HisServiceGroupFilter filter = new MOS.Filter.HisServiceGroupFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_SERVICE_GROUP_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.V_HIS_SERV_SEGR))
                {
                    MOS.Filter.HisServSegrViewFilter filter = new MOS.Filter.HisServSegrViewFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_SERV_SEGR_GETVIEW);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_SERVICE_TYPE))
                {
                    MOS.Filter.HisServiceTypeFilter filter = new MOS.Filter.HisServiceTypeFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_SERVICE_TYPE_GET);
                }
                //else if (type == typeof(MOS.EFMODEL.DataModels.HIS_MEDI_STOCK))
                //{
                //    MOS.Filter.HisMediStockFilter filter = new MOS.Filter.HisMediStockFilter();
                //    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                //    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_MEDI_STOCK_GETVIEW);
                //}
                else if (type == typeof(MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK))
                {
                    MOS.Filter.HisMediStockViewFilter filter = new MOS.Filter.HisMediStockViewFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_MEDI_STOCK_GETVIEW);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_EXP_MEST_TYPE))
                {
                    MOS.Filter.HisExpMestTypeFilter filter = new MOS.Filter.HisExpMestTypeFilter();
                    filter.ORDER_FIELD = "EXP_MEST_TYPE_CODE";
                    filter.ORDER_DIRECTION = "ASC";
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_EXP_MEST_TYPE_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_MEDICINE_TYPE))
                {
                    MOS.Filter.HisMedicineTypeFilter filter = new MOS.Filter.HisMedicineTypeFilter();
                    filter.ORDER_FIELD = "MEDICINE_TYPE_CODE";
                    filter.ORDER_DIRECTION = "ASC";
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_MEDICINE_TYPE_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE))
                {
                    MOS.Filter.HisMedicineTypeViewFilter filter = new MOS.Filter.HisMedicineTypeViewFilter();
                    filter.ORDER_FIELD = "MEDICINE_TYPE_CODE";
                    filter.ORDER_DIRECTION = "ASC";
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_MEDICINE_TYPE_GETVIEW);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_MANUFACTURER))
                {
                    MOS.Filter.HisManufacturerFilter filter = new MOS.Filter.HisManufacturerFilter();
                    filter.ORDER_FIELD = "MANUFACTURER_CODE";
                    filter.ORDER_DIRECTION = "ASC";
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_MANUFACTURER_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE_ACIN))
                {
                    MOS.Filter.HisMedicineTypeAcinViewFilter filter = new MOS.Filter.HisMedicineTypeAcinViewFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_MEDICINE_TYPE_ACIN_GETVIEW);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_PACKING_TYPE))
                {
                    MOS.Filter.HisPackingTypeFilter filter = new MOS.Filter.HisPackingTypeFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_PACKING_TYPE_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.V_HIS_TEST_INDEX_RANGE))
                {
                    MOS.Filter.HisTestIndexRangeViewFilter filter = new MOS.Filter.HisTestIndexRangeViewFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_TEST_INDEX_RANGE_GETVIEW);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.V_HIS_TEST_INDEX))
                {
                    MOS.Filter.HisTestIndexViewFilter filter = new MOS.Filter.HisTestIndexViewFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_TEST_INDEX_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE))
                {
                    MOS.Filter.HisTreatmentTypeFilter filter = new MOS.Filter.HisTreatmentTypeFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_TREATMENT_TYPE_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.V_HIS_MATERIAL_TYPE))
                {
                    MOS.Filter.HisMaterialTypeViewFilter filter = new MOS.Filter.HisMaterialTypeViewFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_MATERIAL_TYPE_GETVIEW);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_MATERIAL_TYPE))
                {
                    MOS.Filter.HisMaterialTypeFilter filter = new MOS.Filter.HisMaterialTypeFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_MATERIAL_TYPE_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_MILITARY_RANK))
                {
                    MOS.Filter.HisMilitaryRankFilter filter = new MOS.Filter.HisMilitaryRankFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_MILITARY_RANK_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_WORK_PLACE))
                {
                    MOS.Filter.HisWorkPlaceFilter filter = new MOS.Filter.HisWorkPlaceFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_WORK_PLACE_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_EMERGENCY_WTIME))
                {
                    MOS.Filter.HisEmergencyWtimeFilter filter = new MOS.Filter.HisEmergencyWtimeFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_EMERGENCY_WTIME_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_REHA_TRAIN_TYPE))
                {
                    MOS.Filter.HisRehaTrainTypeFilter filter = new MOS.Filter.HisRehaTrainTypeFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_REHA_TRAIN_TYPE_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_BLOOD_ABO))
                {
                    MOS.Filter.HisBloodAboFilter filter = new MOS.Filter.HisBloodAboFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_BLOOD_ABO__GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_BLOOD_RH))
                {
                    MOS.Filter.HisBloodRhFilter filter = new MOS.Filter.HisBloodRhFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_BLOOD_RH__GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_PAY_FORM))
                {
                    MOS.Filter.HisPayFormFilter filter = new MOS.Filter.HisPayFormFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_PAY_FORM_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_BLOOD_TYPE))
                {
                    MOS.Filter.HisBloodTypeFilter filter = new MOS.Filter.HisBloodTypeFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_BLOOD_TYPE_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.V_HIS_BLOOD_TYPE))
                {
                    MOS.Filter.HisBloodTypeViewFilter filter = new MOS.Filter.HisBloodTypeViewFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_BLOOD_TYPE_GETVIEW);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_SUPPLIER))
                {
                    MOS.Filter.HisSupplierFilter filter = new MOS.Filter.HisSupplierFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_SUPPLIER_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_IMP_SOURCE))
                {
                    MOS.Filter.HisImpSourceFilter filter = new MOS.Filter.HisImpSourceFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_IMP_SOURCE_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_TRANSACTION_TYPE))
                {
                    HisTransactionTypeFilter filter = new HisTransactionTypeFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_TRANSACTION_TYPE_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_BORN_TYPE))
                {
                    HisBornTypeFilter filter = new HisBornTypeFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_BORN_TYPE_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_BORN_POSITION))
                {
                    HisBornPositionFilter filter = new HisBornPositionFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_BORN_POSITION_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_BORN_RESULT))
                {
                    HisBornResultFilter filter = new HisBornResultFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_BORN_RESULT_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM))
                {
                    HisMestRoomViewFilter filter = new HisMestRoomViewFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_MEST_ROOM_GETVIEW);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_ANTICIPATE))
                {
                    HisAnticipateFilter filter = new HisAnticipateFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_ANTICIPATE_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_ACCIDENT_HURT_TYPE))
                {
                    HisAccidentHurtTypeFilter filter = new HisAccidentHurtTypeFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_ACCIDENT_HURT_TYPE_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.V_HIS_CASHIER_ROOM))
                {
                    HisCashierRoomViewFilter filter = new HisCashierRoomViewFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_CASHIER_ROOM_GETVIEW);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_CASHIER_ROOM))
                {
                    HisCashierRoomFilter filter = new HisCashierRoomFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_CASHIER_ROOM_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_FUND))
                {
                    HisFundFilter filter = new HisFundFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_FUND_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.V_HIS_TREATMENT_BED_ROOM))
                {
                    HisTreatmentBedRoomViewFilter filter = new HisTreatmentBedRoomViewFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_TREATMENT_BED_ROOM_GETVIEW);
                }
                //else if (type == typeof(MOS.EFMODEL.DataModels.V_HIS_ROOM_COUNTER))
                //{
                //    HisRoomCounterViewFilter filter = new HisRoomCounterViewFilter();
                //    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                //    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_ROOM_GETVIEW_COUNTER);
                //}
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_SERVICE_FOLLOW))
                {
                    HisServiceFollowFilter filter = new HisServiceFollowFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_SERVICE_FOLLOW_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.V_HIS_SERVICE_FOLLOW))
                {
                    HisServiceFollowViewFilter filter = new HisServiceFollowViewFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_SERVICE_FOLLOW_GETVIEW);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_PTTT_METHOD))
                {
                    HisPtttMethodFilter filter = new HisPtttMethodFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_PTTT_METHOD_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_PTTT_GROUP))
                {
                    HisPtttGroupFilter filter = new HisPtttGroupFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_PTTT_GROUP_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_EMOTIONLESS_METHOD))
                {
                    HisEmotionlessMethodFilter filter = new HisEmotionlessMethodFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_EMOTIONLESS_METHOD_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_PTTT_CONDITION))
                {
                    HisPtttConditionFilter filter = new HisPtttConditionFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_PTTT_CONDITION_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_PTTT_CATASTROPHE))
                {
                    HisPtttCatastropheFilter filter = new HisPtttCatastropheFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_PTTT_CATASTROPHE_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE))
                {
                    HisExecuteRoleFilter filter = new HisExecuteRoleFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_EXECUTE_ROLE_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_BHYT_BLACKLIST))
                {
                    HisBhytBlacklistFilter filter = new HisBhytBlacklistFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_BHYT_BLACKLIST_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_BHYT_WHITELIST))
                {
                    HisBhytWhitelistFilter filter = new HisBhytWhitelistFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HisRequestUriStore.HIS_BHYT_WHITELIST_GET);
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_PAAN_POSITION))
                {
                    HisPaanPositionFilter filter = new HisPaanPositionFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), "api/HisPaanPosition/Get");
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_PAAN_LIQUID))
                {
                    HisPaanLiquidFilter filter = new HisPaanLiquidFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), "api/HisPaanLiquid/Get");
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_REPAY_REASON))
                {
                    HisRepayReasonFilter filter = new HisRepayReasonFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), "api/HisRepayReason/Get");
                }
                else if (type == typeof(MOS.EFMODEL.DataModels.HIS_MEDICINE_TYPE_ROOM))
                {
                    HisMedicineTypeRoomFilter filter = new HisMedicineTypeRoomFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new MosGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), "api/HisMedicineTypeRoom/Get");
                }
                #endregion

                #region HTC
                else if (type == typeof(HTC.EFMODEL.DataModels.HTC_PERIOD))
                {
                    HTC.Filter.HtcPeriodFilter filter = new HTC.Filter.HtcPeriodFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new HtcGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HtcRequestUriStore.HTC_PERIOD__GET);
                }
                else if (type == typeof(HTC.EFMODEL.DataModels.HTC_EXPENSE_TYPE))
                {
                    HTC.Filter.HtcExpenseTypeFilter filter = new HTC.Filter.HtcExpenseTypeFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new HtcGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), HtcRequestUriStore.HTC_EXPENSE_TYPE__GET);
                }
                #endregion

                #region LIS
                else if (type == typeof(LIS.EFMODEL.DataModels.LIS_SAMPLE_STT))
                {
                    LIS.Filter.LisSampleSttFilter filter = new LIS.Filter.LisSampleSttFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    result = new LisGetListAsyncBehavior(param, (filterQuery == null ? filter : filterQuery), LisRequestUriStore.LIS_SAMPLE_STT_GET);
                }
                #endregion
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
    }
}
