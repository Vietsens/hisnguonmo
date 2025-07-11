using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.DrugUsageAnalysis.ADO;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.DrugUsageAnalysis
{
    public partial class UCDrugUsageAnalysis : UserControlBase
    {
        private void LoadDataToTreeList(long treatmentID)
        {
            try
            {
                // Load parent nodes (tracking sheets)
                var trackingFilter = new MOS.Filter.HisTrackingViewFilter { TREATMENT_ID = treatmentID };
                var param = new CommonParam();
                var trackings = new BackendAdapter(param).Get<List<V_HIS_TRACKING>>(
                    "api/HisTracking/GetView", ApiConsumers.MosConsumer, trackingFilter, param);
                // Get list of drug usage analysis by treatment
                var analysisFilter = new MOS.Filter.HisDrugUseAnalysisFilter { TDL_TREATMENT_ID = treatmentID };
                var analyses = new BackendAdapter(param).Get<List<HIS_DRUG_USE_ANALYSIS>>(
                    "api/HisDrugUseAnalysis/Get", ApiConsumers.MosConsumer, analysisFilter, param);
                // Create a HashSet for quick lookup of tracking IDs that have analyses
                var trackingIdsWithAnalysis = new HashSet<long>(analyses
                    .Where(a => a.TRACKING_ID.HasValue)
                    .Select(a => a.TRACKING_ID.Value));
                // Create a list to hold the parent nodes with their child nodes
                var result = new List<TrackingDrugUseAnalysisADO>();
                foreach (var tracking in trackings.OrderByDescending(t => t.TRACKING_TIME))
                {
                    // Create the parent node for the tracking sheet
                    var parentNode = new TrackingDrugUseAnalysisADO(tracking)
                    {
                        IsColorDeepSkyBlue = trackingIdsWithAnalysis.Contains(tracking.ID),
                    };
                    result.Add(parentNode);
                    // Load child nodes (drug services)
                    var serviceFilter = new MOS.Filter.HisSereServView1Filter()
                    {
                        TRACKING_ID = tracking.ID,
                        SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC,
                        IS_INCLUDE_DELETED = false
                    };
                    var services = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV_1>>(
                        "api/HisSereServ/GetView1", ApiConsumers.MosConsumer, serviceFilter, param);
                    foreach (var service in services.Select((s, i) => new { s, i = i++ }))
                    {
                        if (service.i == 0)
                        {
                            var adoChild0 = new ADO.TrackingDrugUseAnalysisADO(tracking, false, 0);
                            result.Add(adoChild0);
                        }
                        var adoChild = new ADO.TrackingDrugUseAnalysisADO(tracking, false, service.i + 1)
                        {
                            ServiceName = service.s.TDL_SERVICE_NAME,
                            Amount = service.s.AMOUNT,
                            ServiceUnitName = service.s.SERVICE_UNIT_NAME,
                        };
                        result.Add(adoChild);
                    }
                }

                treeListDateTime.DataSource = result;
                //if (result != null && result.Count > 0)
                //{
                //    foreach (TreeListColumn item in treeListDateTime.Columns)
                //    {
                //        if (item == treeListColumnSTT || item == treeListColumn_DrugUsageAnalysisDetail)
                //        {
                //            continue;
                //        }
                //        item.BestFit();
                //    }
                //}
                treeListDateTime.ExpandAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
