using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.Plugins.ApprovaleDebate.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HIS.Desktop.Plugins.ApprovaleDebate.Subclinical
{
    /// <summary>
    /// Lay ket qua can lam sang cua ho so cho tab Ket qua CLS, theo cach chuc nang
    /// HIS.Desktop.Plugins.ContentSubclinical dang lam.
    /// </summary>
    internal static class SubclinicalResultLoader
    {
        private const string CONFIG_KEY__SHOW_RESULT_WHEN_REQ_COMPLETE = "HIS.Desktop.Plugins.ContentSubclinical.ShowResultWhenReqComplete";
        private const string URI__HIS_SERE_SERV_EXT_GET = "/api/HisSereServExt/Get";
        private const string NO_RESULT_TEXT = "Chưa có kết quả";
        //Chia nho danh sach ID de tranh ORA-01795 (IN qua 1000 phan tu)
        private const int ID_BATCH_SIZE = 500;

        private static readonly List<long> SUBCLINICAL_SERVICE_TYPE_IDS = new List<long>
        {
            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN,
            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA,
            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA,
            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS,
            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN,
            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__GPBL
        };

        internal static List<SubclinicalResultADO> Load(long treatmentId)
        {
            List<SubclinicalResultADO> result = new List<SubclinicalResultADO>();
            try
            {
                if (treatmentId <= 0)
                    return result;

                CommonParam param = new CommonParam();
                string showOption = HisConfigs.Get<string>(CONFIG_KEY__SHOW_RESULT_WHEN_REQ_COMPLETE);

                HisServiceReqFilter reqFilter = new HisServiceReqFilter();
                reqFilter.TREATMENT_ID = treatmentId;
                reqFilter.SERVICE_REQ_STT_IDs = showOption == "1"
                    ? new List<long> { IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT }
                    : new List<long> { IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL, IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT };
                List<HIS_SERVICE_REQ> serviceReqs = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumers.MosConsumer, reqFilter, param);
                if (serviceReqs == null || serviceReqs.Count == 0)
                    return result;
                if (showOption == "2")
                {
                    serviceReqs = serviceReqs.Where(o => o.SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN
                        || o.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT).ToList();
                }
                Dictionary<long, HIS_SERVICE_REQ> dicServiceReq = serviceReqs.GroupBy(o => o.ID).ToDictionary(g => g.Key, g => g.First());

                List<HIS_SERE_SERV> sereServs = new List<HIS_SERE_SERV>();
                foreach (List<long> ids in Batch(dicServiceReq.Keys.ToList()))
                {
                    HisSereServFilter ssFilter = new HisSereServFilter();
                    ssFilter.SERVICE_REQ_IDs = ids;
                    ssFilter.TDL_SERVICE_TYPE_IDs = SUBCLINICAL_SERVICE_TYPE_IDS;
                    var rs = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GET, ApiConsumers.MosConsumer, ssFilter, param);
                    if (rs != null)
                        sereServs.AddRange(rs);
                }
                sereServs = sereServs.Where(o => o.IS_DELETE != 1 && o.IS_NO_EXECUTE != 1
                    && o.SERVICE_REQ_ID.HasValue && dicServiceReq.ContainsKey(o.SERVICE_REQ_ID.Value)).ToList();
                if (sereServs.Count == 0)
                    return result;

                Dictionary<long, HIS_SERE_SERV_EXT> dicExt = new Dictionary<long, HIS_SERE_SERV_EXT>();
                foreach (List<long> ids in Batch(sereServs.Select(o => o.ID).Distinct().ToList()))
                {
                    HisSereServExtFilter extFilter = new HisSereServExtFilter();
                    extFilter.SERE_SERV_IDs = ids;
                    var rs = new BackendAdapter(param).Get<List<HIS_SERE_SERV_EXT>>(URI__HIS_SERE_SERV_EXT_GET, ApiConsumers.MosConsumer, extFilter, param);
                    if (rs != null)
                    {
                        foreach (var ext in rs)
                            dicExt[ext.SERE_SERV_ID] = ext;
                    }
                }

                List<V_HIS_SERE_SERV_TEIN> teins = new List<V_HIS_SERE_SERV_TEIN>();
                List<long> testIds = sereServs.Where(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN).Select(o => o.ID).Distinct().ToList();
                foreach (List<long> ids in Batch(testIds))
                {
                    HisSereServTeinViewFilter teinFilter = new HisSereServTeinViewFilter();
                    teinFilter.SERE_SERV_IDs = ids;
                    teinFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    var rs = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV_TEIN>>(HisRequestUriStore.HIS_SERE_SERV_TEIN_GET, ApiConsumers.MosConsumer, teinFilter, param);
                    if (rs != null)
                        teins.AddRange(rs);
                }
                //Bo khang sinh do (xem o tab Xet nghiem) va chi so chua co gia tri.
                //Thu tu chi so: NUM_ORDER giam dan nhu man xem chi so (HIS.Desktop.Plugins.SereServTein)
                Dictionary<long, List<V_HIS_SERE_SERV_TEIN>> dicTein = teins
                    .Where(o => String.IsNullOrWhiteSpace(o.BACTERIUM_CODE) && !String.IsNullOrWhiteSpace(o.VALUE))
                    .GroupBy(o => o.SERE_SERV_ID)
                    .ToDictionary(g => g.Key, g => g.OrderByDescending(o => o.NUM_ORDER ?? 0).ToList());

                List<HIS_SERVICE_TYPE> serviceTypes = BackendDataWorker.Get<HIS_SERVICE_TYPE>();

                //Cap 1: moc thoi gian chi dinh (den phut), moi nhat o tren
                var groupByTime = sereServs
                    .GroupBy(o => dicServiceReq[o.SERVICE_REQ_ID.Value].INTRUCTION_TIME / 100)
                    .OrderByDescending(g => g.Key)
                    .ToList();
                int timeIndex = 0;
                foreach (var timeGroup in groupByTime)
                {
                    timeIndex++;
                    long intructionTime = timeGroup.Key * 100;
                    string timeText = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(intructionTime);
                    string dateText = Inventec.Common.DateTime.Convert.TimeNumberToDateString(intructionTime);
                    SubclinicalResultADO timeNode = new SubclinicalResultADO();
                    timeNode.KEY_ID = timeIndex.ToString();
                    timeNode.NODE_LEVEL = SubclinicalResultADO.LEVEL__INTRUCTION_TIME;
                    timeNode.NAME = timeText;
                    timeNode.INTRUCTION_DATE = dateText;
                    result.Add(timeNode);

                    foreach (HIS_SERE_SERV ss in timeGroup.OrderBy(o => o.TDL_SERVICE_TYPE_ID).ThenBy(o => o.TDL_SERVICE_NAME))
                    {
                        HIS_SERVICE_REQ serviceReq = dicServiceReq[ss.SERVICE_REQ_ID.Value];
                        HIS_SERE_SERV_EXT ext = null;
                        dicExt.TryGetValue(ss.ID, out ext);
                        bool isTest = ss.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN;
                        HIS_SERVICE_TYPE serviceType = serviceTypes != null ? serviceTypes.FirstOrDefault(o => o.ID == ss.TDL_SERVICE_TYPE_ID) : null;

                        SubclinicalResultADO serviceNode = new SubclinicalResultADO();
                        serviceNode.KEY_ID = timeNode.KEY_ID + "_" + ss.ID;
                        serviceNode.PARENT_KEY_ID = timeNode.KEY_ID;
                        serviceNode.NODE_LEVEL = SubclinicalResultADO.LEVEL__SERVICE;
                        serviceNode.NAME = serviceType != null ? String.Format("{0} ({1})", ss.TDL_SERVICE_NAME, serviceType.SERVICE_TYPE_NAME) : ss.TDL_SERVICE_NAME;
                        serviceNode.INTRUCTION_DATE = dateText;

                        //Thoi gian co ket qua: y lenh hoan thanh lay FINISH_TIME; dang xu ly (khong phai XN) lay END_TIME cua EXT
                        long? resultTime = null;
                        if (serviceReq.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
                            resultTime = serviceReq.FINISH_TIME;
                        else if (!isTest && ext != null)
                            resultTime = ext.END_TIME;
                        serviceNode.RESULT_TIME = resultTime.HasValue ? Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(resultTime.Value) : null;

                        List<V_HIS_SERE_SERV_TEIN> serviceTeins = null;
                        if (isTest)
                        {
                            dicTein.TryGetValue(ss.ID, out serviceTeins);
                        }
                        else if (ext != null)
                        {
                            serviceNode.RESULT = !String.IsNullOrWhiteSpace(ext.CONCLUDE) ? ext.CONCLUDE : ext.DESCRIPTION;
                        }
                        bool hasResult = isTest ? (serviceTeins != null && serviceTeins.Count > 0) : !String.IsNullOrWhiteSpace(serviceNode.RESULT);
                        if (!hasResult)
                        {
                            serviceNode.RESULT = NO_RESULT_TEXT;
                            serviceNode.IS_NO_RESULT = true;
                        }
                        serviceNode.SEARCH_TEXT = serviceNode.IS_NO_RESULT ? ss.TDL_SERVICE_NAME : ss.TDL_SERVICE_NAME + " " + serviceNode.RESULT;
                        result.Add(serviceNode);

                        if (serviceTeins != null)
                        {
                            foreach (V_HIS_SERE_SERV_TEIN tein in serviceTeins)
                            {
                                SubclinicalResultADO indexNode = new SubclinicalResultADO();
                                indexNode.KEY_ID = serviceNode.KEY_ID + "_" + tein.ID;
                                indexNode.PARENT_KEY_ID = serviceNode.KEY_ID;
                                indexNode.NODE_LEVEL = SubclinicalResultADO.LEVEL__TEST_INDEX;
                                indexNode.NAME = tein.TEST_INDEX_NAME;
                                indexNode.RESULT = tein.VALUE;
                                indexNode.UNIT = tein.TEST_INDEX_UNIT_NAME;
                                indexNode.NORMAL_RANGE = tein.DESCRIPTION;
                                indexNode.RESULT_CODE = tein.RESULT_CODE;
                                indexNode.INTRUCTION_DATE = dateText;
                                indexNode.SEARCH_TEXT = tein.TEST_INDEX_NAME;
                                result.Add(indexNode);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private static IEnumerable<List<long>> Batch(List<long> ids)
        {
            for (int i = 0; i < ids.Count; i += ID_BATCH_SIZE)
            {
                yield return ids.Skip(i).Take(ID_BATCH_SIZE).ToList();
            }
        }
    }
}
