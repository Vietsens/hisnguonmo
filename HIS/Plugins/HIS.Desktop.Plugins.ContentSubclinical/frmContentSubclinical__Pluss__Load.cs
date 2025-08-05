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
using DevExpress.XtraTreeList.Nodes;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.ContentSubclinical.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using LIS.EFMODEL.DataModels;
using LIS.Filter;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ContentSubclinical
{
    public partial class frmContentSubclinical : HIS.Desktop.Utility.FormBase
    {
        internal List<HisSereServADONumOrder> rsSereServ { get; set; }
        Dictionary<long, HIS_SERE_SERV_EXT> dicSereServExt { get; set; }
        long _DOB = 0;
        long _GENDER_ID = 0;
        NumberStyles style;

        void LoadDataSS()
        {
            try
            {
                if (chkOtherTreatment.Checked)
                {
                    this.treatmentIdSearch = Inventec.Common.TypeConvert.Parse.ToInt64((this.cboHisTreatment.EditValue ?? "0").ToString());
                }
                else
                    this.treatmentIdSearch = this.treatmentId;

                if (this.treatmentIdSearch <= 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Bắt buộc chọn Hồ sơ điều trị", "Thông báo", MessageBoxButtons.OK);
                    return;
                }
                WaitingManager.Show();
                CommonParam param = new CommonParam();

                MOS.Filter.HisServiceReqFilter serviceReqFilter = new MOS.Filter.HisServiceReqFilter();

                serviceReqFilter.TREATMENT_ID = this.treatmentIdSearch;
                if (ShowResultWhenReqComplete == "1")
                    serviceReqFilter.SERVICE_REQ_STT_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT;
                serviceReqFilter.SERVICE_REQ_STT_IDs = new List<long>();
                serviceReqFilter.SERVICE_REQ_STT_IDs.AddRange(new List<long>
                    {
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT,
                     IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL
                    });
                if (dtTimeFrom.EditValue != null && dtTimeFrom.DateTime != DateTime.MinValue)
                {
                    serviceReqFilter.INTRUCTION_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeFrom.EditValue).ToString("yyyyMMdd") + "000000");
                }
                if (dtTimeTo.EditValue != null && dtTimeTo.DateTime != DateTime.MinValue)
                {
                    serviceReqFilter.INTRUCTION_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeTo.EditValue).ToString("yyyyMMdd") + "235959");
                }

                var rsServiceReq = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumers.MosConsumer, serviceReqFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (ShowResultWhenReqComplete == "2" && rsServiceReq != null)
                {
                    rsServiceReq = rsServiceReq.Where(o => o.SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN || (o.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN && o.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)).ToList();
                }
                this.listServiceReq = rsServiceReq ?? new List<HIS_SERVICE_REQ>();

                rsSereServ = new List<HisSereServADONumOrder>();
                List<long> _ServiceReqIds = new List<long>();
                if (rsServiceReq != null && rsServiceReq.Count > 0)
                {
                    this._GENDER_ID = rsServiceReq.FirstOrDefault().TDL_PATIENT_GENDER_ID ?? 0;
                    this._DOB = rsServiceReq.FirstOrDefault().TDL_PATIENT_DOB;
                    _ServiceReqIds = rsServiceReq.Select(p => p.ID).Distinct().ToList();
                    MOS.Filter.HisSereServFilter ssFilter = new MOS.Filter.HisSereServFilter();
                    ssFilter.SERVICE_REQ_IDs = _ServiceReqIds;
                    ssFilter.TDL_SERVICE_TYPE_IDs = new List<long>();
                    ssFilter.TDL_SERVICE_TYPE_IDs.AddRange(new List<long>
                    {
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__GPBL
                    });

                    var dataSereServs = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GET, ApiConsumers.MosConsumer, ssFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);

                    List<HIS_SERE_SERV> dataTests = new List<HIS_SERE_SERV>();
                    dicSereServExt = new Dictionary<long, HIS_SERE_SERV_EXT>();
                    List<HIS_SERE_SERV_EXT> dataSereServExts = new List<HIS_SERE_SERV_EXT>();
                    if (dataSereServs != null && dataSereServs.Count > 0)
                    {
                        dataTests = dataSereServs.Where(p => p.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN || p.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__GPBL).ToList();

                        MOS.Filter.HisSereServExtFilter extFilter = new HisSereServExtFilter();
                        extFilter.SERE_SERV_IDs = dataSereServs.Select(p => p.ID).Distinct().ToList();

                        dataSereServExts = new BackendAdapter(param).Get<List<HIS_SERE_SERV_EXT>>("/api/HisSereServExt/Get", ApiConsumers.MosConsumer, extFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);

                        if (dataSereServExts != null && dataSereServExts.Count > 0)
                        {
                            foreach (var item in dataSereServExts)
                            {
                                if (dicSereServExt.ContainsKey(item.SERE_SERV_ID))
                                {
                                    dicSereServExt[item.SERE_SERV_ID] = item;
                                }
                                else
                                {
                                    dicSereServExt.Add(item.SERE_SERV_ID, item);
                                }
                            }
                        }

                        rsSereServ.AddRange((from r in dataSereServs select new HisSereServADONumOrder(r)).ToList());
                    }
                    List<V_HIS_SERE_SERV_TEIN> _SereServTeins = new List<V_HIS_SERE_SERV_TEIN>();
                    if (dataTests != null && dataTests.Count > 0)
                    {
                        HisSereServTeinViewFilter sereSerTeinFilter = new HisSereServTeinViewFilter();
                        sereSerTeinFilter.SERE_SERV_IDs = dataTests.Select(p => p.ID).Distinct().ToList();
                        sereSerTeinFilter.ORDER_FIELD = "NUM_ORDER";
                        sereSerTeinFilter.ORDER_DIRECTION = "DESC";
                        sereSerTeinFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                        _SereServTeins = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV_TEIN>>(HisRequestUriStore.HIS_SERE_SERV_TEIN_GET, ApiConsumers.MosConsumer, sereSerTeinFilter, param);
                    }

                    List<LIS_SAMPLE> _LisSamples = new List<LIS_SAMPLE>();
                    List<LIS_SAMPLE_SERVICE> _LisSampleServices = new List<LIS_SAMPLE_SERVICE>();
                    List<LIS_RESULT> _LisResults = new List<LIS_RESULT>();
                    if (!chkImportant.Checked && chkShowMicrobiological.Checked)
                    {
                        if (dataTests != null && dataTests.Count > 0)
                        {
                            LisSampleFilter samFilter = new LisSampleFilter();
                            samFilter.SERVICE_REQ_CODEs = dataTests.Select(p => p.TDL_SERVICE_REQ_CODE).Distinct().ToList();
                            _LisSamples = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<LIS_SAMPLE>>("api/LisSample/Get", ApiConsumers.LisConsumer, samFilter, null);
                            if (_LisSamples != null && _LisSamples.Count > 0)
                            {
                                LisSampleServiceFilter sampleServiceFilter = new LisSampleServiceFilter();
                                sampleServiceFilter.SAMPLE_IDs = _LisSamples.Select(p => p.ID).Distinct().ToList();
                                _LisSampleServices = new BackendAdapter(param).Get<List<LIS_SAMPLE_SERVICE>>("/api/LisSampleService/Get", ApiConsumer.ApiConsumers.LisConsumer, sampleServiceFilter, param);
                                if (_LisSampleServices != null && _LisSampleServices.Count > 0)
                                {
                                    LIS.Filter.LisResultFilter resultFilter = new LisResultFilter();
                                    resultFilter.SAMPLE_SERVICE_IDs = _LisSampleServices.Select(p => p.ID).Distinct().ToList();
                                    _LisResults = new BackendAdapter(param).Get<List<LIS_RESULT>>("api/LisResult/Get", ApiConsumer.ApiConsumers.LisConsumer, resultFilter, param);
                                }
                            }
                        }
                    }
                    List<V_HIS_SERVICE> listServiceXN = BackendDataWorker.Get<V_HIS_SERVICE>().Where(p => p.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN).ToList();
                    #region ---CreateTree---
                    List<TreeSereServADO> SereServADOs = new List<TreeSereServADO>();
                    treeListServiceReq.DataSource = null;
                    var listRootSety = (rsSereServ != null && rsSereServ.Count > 0) ? rsSereServ.GroupBy(g => g.TDL_INTRUCTION_DATE).ToList() : null;
                    foreach (var rootSety in listRootSety)
                    {
                        var listBySety = rootSety.ToList<HisSereServADONumOrder>().GroupBy(p => p.TDL_SERVICE_TYPE_ID).ToList();
                        TreeSereServADO ssInTime = new TreeSereServADO();
                        ssInTime.CONCRETE_ID__IN_SETY = rootSety.First().TDL_INTRUCTION_DATE + "";
                        ssInTime.SERVICE_REQ_CODE = Inventec.Common.DateTime.Convert.TimeNumberToDateString(rootSety.First().TDL_INTRUCTION_DATE.ToString());
                        ssInTime.TDL_INTRUCTION_DATE = rootSety.First().TDL_INTRUCTION_DATE;
                        SereServADOs.Add(ssInTime);
                        foreach (var itemSS in listBySety)
                        {
                            TreeSereServADO ssServiceType = new TreeSereServADO();
                            ssServiceType.CONCRETE_ID__IN_SETY = ssInTime.CONCRETE_ID__IN_SETY + "_" + itemSS.First().TDL_SERVICE_TYPE_ID + "";
                            ssServiceType.PARENT_ID__IN_SETY = ssInTime.CONCRETE_ID__IN_SETY;
                            var serviceType = BackendDataWorker.Get<HIS_SERVICE_TYPE>().FirstOrDefault(p => p.ID == itemSS.First().TDL_SERVICE_TYPE_ID);
                            ssServiceType.SERVICE_REQ_CODE = (serviceType != null ? serviceType.SERVICE_TYPE_NAME : "");
                            List<IGrouping<long, HisSereServADONumOrder>> listBySetyParent = null;
                            if (itemSS.First().TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN)
                            {
                                ssServiceType.NUM_ORDER = 1;
                                if (chkShowParentServiceGroup.Checked)
                                {
                                    itemSS.ToList().ForEach(o => o.PARENT_ID = listServiceXN.FirstOrDefault(p => p.ID == o.SERVICE_ID).PARENT_ID);
                                    listBySetyParent = itemSS.ToList<HisSereServADONumOrder>().GroupBy(p => p.PARENT_ID ?? 0).ToList();
                                }
                            }
                            else
                            {
                                ssServiceType.NUM_ORDER = 2;
                            }
                            ssServiceType.TDL_SERVICE_TYPE_ID = itemSS.First().TDL_SERVICE_TYPE_ID;
                            SereServADOs.Add(ssServiceType);
                            if (chkShowParentServiceGroup.Checked && listBySetyParent != null && listBySetyParent.Count > 0)
                            {
                                foreach (var itemParent in listBySetyParent)
                                {
                                    if (itemParent.Key > 0)
                                    {
                                        TreeSereServADO ssServiceParent = new TreeSereServADO();
                                        ssServiceParent.CONCRETE_ID__IN_SETY = ssServiceType.CONCRETE_ID__IN_SETY + "_" + itemParent.First().PARENT_ID;
                                        ssServiceParent.PARENT_ID__IN_SETY = ssServiceType.CONCRETE_ID__IN_SETY;
                                        ssServiceParent.NUM_ORDER = ssServiceType.NUM_ORDER;
                                        ssServiceParent.TDL_SERVICE_TYPE_ID = ssServiceType.TDL_SERVICE_TYPE_ID;
                                        var serviceParent = itemParent.First().PARENT_ID != null ? listServiceXN.FirstOrDefault(p => p.ID == itemParent.First().PARENT_ID) : null;
                                        ssServiceParent.SERVICE_REQ_CODE = (serviceParent != null ? serviceParent.SERVICE_NAME : "");
                                        ssServiceParent.IsParentServiceType = true;
                                        SereServADOs.Add(ssServiceParent);
                                        if (chkImportant.Checked || !chkShowMicrobiological.Checked || _LisSampleServices == null || _LisSampleServices.Count == 0 || _LisResults == null || _LisResults.Count == 0)
                                        {
                                            int t = 0;
                                            foreach (var item in itemParent)
                                            {
                                                LoadDataService(item, ssServiceParent, rsServiceReq, dicSereServExt, _SereServTeins, _LisSamples, ref SereServADOs, ref t);
                                            }
                                        }
                                        else
                                            LoadDataMicrobiologicalResult(itemParent, ssServiceParent, rsServiceReq, dicSereServExt, _SereServTeins, _LisSamples, _LisSampleServices, _LisResults, ref SereServADOs);
                                    }
                                    else
                                    {
                                        if (chkImportant.Checked || !chkShowMicrobiological.Checked || _LisSampleServices == null || _LisSampleServices.Count == 0 || _LisResults == null || _LisResults.Count == 0)
                                        {
                                            int t = 0;
                                            foreach (var item in itemParent)
                                            {
                                                LoadDataService(item, ssServiceType, rsServiceReq, dicSereServExt, _SereServTeins, _LisSamples, ref SereServADOs, ref t);
                                            }
                                        }
                                        else
                                        {
                                            LoadDataMicrobiologicalResult(itemParent, ssServiceType, rsServiceReq, dicSereServExt, _SereServTeins, _LisSamples, _LisSampleServices, _LisResults, ref SereServADOs);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (chkImportant.Checked || !chkShowMicrobiological.Checked || _LisSampleServices == null || _LisSampleServices.Count == 0 || _LisResults == null || _LisResults.Count == 0)
                                {
                                    int t = 0;
                                    foreach (var item in itemSS)
                                    {
                                        LoadDataService(item, ssServiceType, rsServiceReq, dicSereServExt, _SereServTeins, _LisSamples, ref SereServADOs, ref t);
                                    }
                                }
                                else
                                    LoadDataMicrobiologicalResult(itemSS, ssServiceType, rsServiceReq, dicSereServExt, _SereServTeins, _LisSamples, _LisSampleServices, _LisResults, ref SereServADOs);
                            }
                        }
                    }
                    if (chkImportant.Checked)
                    {
                        var listSsImportant = SereServADOs.Where(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN
                            && o.IsLeaf && o.IS_IMPORTANT).ToList();
                        if (listSsImportant == null || listSsImportant.Count == 0)
                            SereServADOs = new List<TreeSereServADO>();
                        var listSsImportantP = listSsImportant.Select(o => o.PARENT_ID__IN_SETY).Distinct().ToList();
                        var listDV = SereServADOs.Where(o => listSsImportantP.Contains(o.CONCRETE_ID__IN_SETY)).Select(o => o.PARENT_ID__IN_SETY).Distinct().ToList();
                        List<string> listDVC = new List<string>();
                        List<string> listLoaiDV = new List<string>();
                        if (chkShowParentServiceGroup.Checked)
                        {

                            listDVC = SereServADOs.Where(o => listDV.Contains(o.CONCRETE_ID__IN_SETY)).Select(o => o.PARENT_ID__IN_SETY).Distinct().ToList();
                            listLoaiDV = SereServADOs.Where(o => listDVC.Contains(o.CONCRETE_ID__IN_SETY)).Select(o => o.PARENT_ID__IN_SETY).Distinct().ToList();
                        }
                        else
                        {
                            listLoaiDV = SereServADOs.Where(o => listDV.Contains(o.CONCRETE_ID__IN_SETY)).Select(o => o.PARENT_ID__IN_SETY).Distinct().ToList();
                        }
                        List<string> listSum = new List<string>();
                        listSum.AddRange(listSsImportant.Select(o => o.CONCRETE_ID__IN_SETY).ToList());
                        listSum.AddRange(listSsImportantP);
                        listSum.AddRange(listDV);
                        listSum.AddRange(listDVC);
                        listSum.AddRange(listLoaiDV);
                        SereServADOs = SereServADOs.Where(o => listSum.Contains(o.CONCRETE_ID__IN_SETY)).ToList();

                    }
                    if (SereServADOs.Count > 0)
                    {
                        if (chkAbove.Checked && chkBelow.Checked)
                        {
                            try
                            {
                                foreach (var item in SereServADOs)
                                {

                                    if (item.VALUE_RANGE != null)
                                    {
                                        try
                                        {
                                            item.VALUE_RANGE_LONG = Decimal.Parse(item.VALUE_RANGE, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
                                        }
                                        catch (Exception ex)
                                        {
                                            item.VALUE_RANGE_LONG = null;
                                            Inventec.Common.Logging.LogSystem.Error("item.VALUE_RANGE_______" + item.VALUE_RANGE);
                                        }
                                    }
                                }
                                SereServADOs = SereServADOs.Where(o => o.VALUE_RANGE_LONG > o.MAX_VALUE || o.VALUE_RANGE_LONG < o.MIN_VALUE).ToList();
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (chkAbove.Checked)
                        {
                            try
                            {
                                foreach (var item in SereServADOs)
                                {

                                    if (item.VALUE_RANGE != null)
                                    {
                                        try
                                        {
                                            item.VALUE_RANGE_LONG = Decimal.Parse(item.VALUE_RANGE, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
                                        }
                                        catch (Exception ex)
                                        {
                                            item.VALUE_RANGE_LONG = null;
                                            Inventec.Common.Logging.LogSystem.Error("item.VALUE_RANGE_______" + item.VALUE_RANGE);
                                        }
                                    }
                                }
                                SereServADOs = SereServADOs.Where(o => o.VALUE_RANGE_LONG > o.MAX_VALUE).ToList();
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (chkBelow.Checked)
                        {
                            try
                            {
                                foreach (var item in SereServADOs)
                                {
                                    if (item.VALUE_RANGE != null)
                                    {
                                        try
                                        {
                                            item.VALUE_RANGE_LONG = Decimal.Parse(item.VALUE_RANGE, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
                                        }
                                        catch (Exception ex)
                                        {
                                            item.VALUE_RANGE_LONG = null;
                                            Inventec.Common.Logging.LogSystem.Error("item.VALUE_RANGE_______" + item.VALUE_RANGE);
                                        }
                                    }

                                }
                                SereServADOs = SereServADOs.Where(o => o.VALUE_RANGE_LONG < o.MIN_VALUE).ToList();
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                    }


                    SereServADOs = SereServADOs.OrderByDescending(o => o.TDL_INTRUCTION_DATE).ThenBy(p => p.NUM_ORDER).ToList();
                    BindingList<TreeSereServADO> records = new BindingList<TreeSereServADO>(SereServADOs);
                    treeListServiceReq.DataSource = records;
                    treeListServiceReq.ExpandAll();
                    treeSereServ_CheckAllNode(treeListServiceReq.Nodes, true, 1);
                    #endregion
                    LoadImage(SereServADOs);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataMicrobiologicalResult(IGrouping<long, HisSereServADONumOrder> itemParent, TreeSereServADO ssServiceParent, List<HIS_SERVICE_REQ> rsServiceReq, Dictionary<long, HIS_SERE_SERV_EXT> dicSereServExt, List<V_HIS_SERE_SERV_TEIN> _SereServTeins, List<LIS_SAMPLE> lisSamples, List<LIS_SAMPLE_SERVICE> lisSampleServices, List<LIS_RESULT> lisResults, ref List<TreeSereServADO> SereServADOs)
        {
            try
            {
                int t = 0;
                foreach (var item in itemParent)
                {
                    var lstSamples = lisSamples.Where(p => p.SERVICE_REQ_CODE == item.TDL_SERVICE_REQ_CODE && p.IS_ANTIBIOTIC_RESISTANCE == 1).ToList();
                    if (lstSamples != null && lstSamples.Count > 0)
                    {
                        foreach (var sample in lstSamples)
                        {
                            var lstSampleService = lisSampleServices.Where(o => o.SAMPLE_ID == sample.ID && o.SERVICE_CODE == item.TDL_SERVICE_CODE).ToList();
                            if (lstSampleService != null && lstSampleService.Count > 0)
                            {
                                foreach (var sammpleService in lstSampleService)
                                {
                                    var lstResult = lisResults.Where(o => o.SAMPLE_SERVICE_ID == sammpleService.ID).ToList();
                                    if (lstResult != null && lstResult.Count > 0)
                                    {
                                        TreeSereServADO leaff = new TreeSereServADO();
                                        leaff.CONCRETE_ID__IN_SETY = ssServiceParent.CONCRETE_ID__IN_SETY + "_" + sammpleService.ID;
                                        leaff.PARENT_ID__IN_SETY = ssServiceParent.CONCRETE_ID__IN_SETY;
                                        leaff.SERVICE_REQ_CODE = sammpleService.SERVICE_NAME;
                                        leaff.NUM_ORDER = ssServiceParent.NUM_ORDER;
                                        leaff.TDL_SERVICE_TYPE_ID = ssServiceParent.TDL_SERVICE_TYPE_ID;
                                        leaff.TDL_SERVICE_CODE = sammpleService.SERVICE_CODE;
                                        leaff.TDL_SERVICE_NAME = sammpleService.SERVICE_NAME;
                                        leaff.VALUE_RANGE = sammpleService.MICROBIOLOGICAL_RESULT;
                                        leaff.DESCRIPTION = !string.IsNullOrEmpty(sample.NOTE) ? "Ghi chú: " + sample.NOTE : "";
                                        leaff.IS_SERE_SERV_HAS_MIC = true;
                                        SereServADOs.Add(leaff);
                                        int q = 0;
                                        var ssResult = lstResult.GroupBy(o => o.BACTERIUM_CODE).ToList();
                                        foreach (var ssBac in ssResult)
                                        {
                                            q++;
                                            TreeSereServADO bacterium = new TreeSereServADO();
                                            bacterium.CONCRETE_ID__IN_SETY = leaff.CONCRETE_ID__IN_SETY + "_" + q;
                                            bacterium.PARENT_ID__IN_SETY = leaff.CONCRETE_ID__IN_SETY;
                                            bacterium.SERVICE_REQ_CODE = ssBac.First().BACTERIUM_NAME;
                                            bacterium.NUM_ORDER = ssServiceParent.NUM_ORDER;
                                            bacterium.IS_BACTERIUM = true;
                                            SereServADOs.Add(bacterium);
                                            foreach (var ssAnti in ssBac)
                                            {
                                                if (string.IsNullOrEmpty(ssAnti.ANTIBIOTIC_CODE))
                                                    continue;
                                                TreeSereServADO antibiotic = new TreeSereServADO();
                                                antibiotic.CONCRETE_ID__IN_SETY = bacterium.CONCRETE_ID__IN_SETY + "_" + ssAnti.ID;
                                                antibiotic.PARENT_ID__IN_SETY = bacterium.CONCRETE_ID__IN_SETY;
                                                antibiotic.SERVICE_REQ_CODE = ssAnti.ANTIBIOTIC_NAME;
                                                antibiotic.VALUE_RANGE = ssAnti.MIC;
                                                antibiotic.SRI_CODE = ssAnti.SRI_CODE;
                                                antibiotic.DESCRIPTION = ssAnti.DESCRIPTION;
                                                antibiotic.NUM_ORDER = ssServiceParent.NUM_ORDER;
                                                antibiotic.IS_ANTIBIOTIC = true;
                                                SereServADOs.Add(antibiotic);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        LoadDataService(item, ssServiceParent, rsServiceReq, dicSereServExt, _SereServTeins, lisSamples, ref SereServADOs, ref t);
                                    }
                                }
                            }
                            else
                            {
                                LoadDataService(item, ssServiceParent, rsServiceReq, dicSereServExt, _SereServTeins, lisSamples, ref SereServADOs, ref t);
                            }
                        }
                    }
                    else
                    {
                        LoadDataService(item, ssServiceParent, rsServiceReq, dicSereServExt, _SereServTeins, lisSamples, ref SereServADOs, ref t);
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataService(HisSereServADONumOrder item, TreeSereServADO ssServiceParent, List<HIS_SERVICE_REQ> rsServiceReq, Dictionary<long, HIS_SERE_SERV_EXT> dicSereServExt, List<V_HIS_SERE_SERV_TEIN> _SereServTeins, List<LIS_SAMPLE> lisSamples, ref List<TreeSereServADO> SereServADOs, ref int t)
        {
            try
            {
                t++;
                var serviceUnit = BackendDataWorker.Get<HIS_SERVICE_UNIT>().FirstOrDefault(p => p.ID == item.TDL_SERVICE_UNIT_ID);
                TreeSereServADO leaf = new TreeSereServADO(item);
                leaf.CONCRETE_ID__IN_SETY = ssServiceParent.CONCRETE_ID__IN_SETY + "_" + t;//ssServiceReq
                leaf.PARENT_ID__IN_SETY = ssServiceParent.CONCRETE_ID__IN_SETY;//ssServiceReq
                leaf.SERVICE_REQ_CODE = item.TDL_SERVICE_NAME;
                leaf.NUM_ORDER = ssServiceParent.NUM_ORDER;
                leaf.TDL_SERVICE_TYPE_ID = ssServiceParent.TDL_SERVICE_TYPE_ID;
                leaf.SERVICE_ID = item.SERVICE_ID;
                leaf.TDL_SERVICE_CODE = item.TDL_SERVICE_CODE;
                leaf.TDL_SERVICE_NAME = item.TDL_SERVICE_NAME;
                if (item.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN)
                {
                    if (!chkImportant.Checked && chkShowMicrobiological.Checked)
                    {
                        var sample = lisSamples.FirstOrDefault(o => o.SERVICE_REQ_CODE == item.TDL_SERVICE_REQ_CODE);
                        if (sample != null)
                        {
                            string note = !string.IsNullOrEmpty(sample.NOTE) ? "Ghi chú: " + sample.NOTE : "";
                            note += !string.IsNullOrEmpty(sample.DESCRIPTION) ? "\n\r" + "Nhận xét: " + sample.DESCRIPTION : "";
                            note += !string.IsNullOrEmpty(sample.CONCLUDE) ? "\n\r" + "Kết luận: " + sample.CONCLUDE : "";
                            leaf.DESCRIPTION = note;
                        }
                    }
                    var dataSer = rsServiceReq.FirstOrDefault(p => p.ID == item.SERVICE_REQ_ID);
                    if (dataSer != null && dataSer.ID > 0 && dataSer.EXE_SERVICE_MODULE_ID > 0)
                    {
                        List<HIS_EXE_SERVICE_MODULE> exeServiceModules = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EXE_SERVICE_MODULE>();
                        HIS_EXE_SERVICE_MODULE exeServiceModule = exeServiceModules != null && exeServiceModules.Count > 0 ?
                            exeServiceModules.FirstOrDefault(o => o.ID == dataSer.EXE_SERVICE_MODULE_ID.Value) : null;
                        if (exeServiceModule != null && exeServiceModule.MODULE_LINK != "HIS.Desktop.Plugins.TestServiceReqExcute" && dicSereServExt.ContainsKey(leaf.ID))
                        {
                            leaf.VALUE_RANGE = dicSereServExt[leaf.ID].CONCLUDE;
                            leaf.DESCRIPTION = dicSereServExt[leaf.ID].DESCRIPTION;
                            leaf.SERVICE_UNIT_NAME = (serviceUnit != null ? serviceUnit.SERVICE_UNIT_NAME : "");
                            SereServADOs.Add(leaf);
                            return;
                        }
                    }

                    if (_SereServTeins != null && _SereServTeins.Count > 0)
                    {
                        var dtTests = _SereServTeins.Where(p => p.SERE_SERV_ID == item.ID).ToList();
                        if (dtTests != null && dtTests.Count > 0)
                        {
                            int k = 0;
                            foreach (var itemT in dtTests)
                            {
                                k++;
                                TreeSereServADO leafXN = new TreeSereServADO();
                                leafXN.CONCRETE_ID__IN_SETY = leaf.CONCRETE_ID__IN_SETY + "_" + k;
                                leafXN.PARENT_ID__IN_SETY = leaf.CONCRETE_ID__IN_SETY;
                                leafXN.SERVICE_REQ_CODE = itemT.TEST_INDEX_NAME;
                                leafXN.VALUE_RANGE = itemT.VALUE;
                                leafXN.DESCRIPTION = itemT.DESCRIPTION;
                                leafXN.TEST_INDEX_CODE = itemT.TEST_INDEX_CODE;
                                leafXN.TEST_INDEX_NAME = itemT.TEST_INDEX_NAME;
                                leafXN.IsLeaf = true;
                                leafXN.NUM_ORDER = ssServiceParent.NUM_ORDER;
                                leafXN.IS_IMPORTANT = (itemT.IS_IMPORTANT == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);
                                leafXN.TDL_SERVICE_TYPE_ID = leaf.TDL_SERVICE_TYPE_ID;
                                leafXN.SERVICE_ID = leaf.SERVICE_ID;
                                leafXN.TDL_SERVICE_CODE = leaf.TDL_SERVICE_CODE;
                                leafXN.TDL_SERVICE_NAME = leaf.TDL_SERVICE_NAME;
                                leafXN.ID = item.ID;

                                leafXN.TEST_INDEX_UNIT_CODE = itemT.TEST_INDEX_UNIT_CODE;
                                leafXN.TEST_INDEX_UNIT_NAME = itemT.TEST_INDEX_UNIT_NAME;
                                ProcessCheckNormal(ref leafXN);
                                SereServADOs.Add(leafXN);
                            }
                        }
                    }

                }
                else
                {
                    if (dicSereServExt.ContainsKey(leaf.ID))
                    {
                        leaf.DESCRIPTION = dicSereServExt[leaf.ID].DESCRIPTION;
                        leaf.VALUE_RANGE = dicSereServExt[leaf.ID].CONCLUDE;
                    }
                }
                leaf.SERVICE_UNIT_NAME = (serviceUnit != null ? serviceUnit.SERVICE_UNIT_NAME : "");
                SereServADOs.Add(leaf);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void LoadImage(List<TreeSereServADO> SereServADOs)
        {
            try
            {
                ImageADOs = new List<ImageADO>();
                CommonParam param = new CommonParam();

                HisSereServFileFilter filter = new HisSereServFileFilter();
                filter.SERE_SERV_IDs = SereServADOs.Select(o => o.ID).Distinct().ToList();

                var SereServFile = new BackendAdapter(param).Get<List<HIS_SERE_SERV_FILE>>(HisRequestUriStore.HIS_SERE_SERV_FILE_GET, HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);

                foreach (var item in SereServFile)
                {
                    var data = SereServADOs.FirstOrDefault(o => o.ID == item.SERE_SERV_ID);
                    var stream = Inventec.Fss.Client.FileDownload.GetFile(item.URL);

                    ImageADO ado = new ImageADO();
                    ado.SERVICE_REQ_NAME = data != null ? data.SERVICE_REQ_CODE : "";
                    ado.IMAGE_DISPLAY = Image.FromStream(stream);

                    ImageADOs.Add(ado);
                }

                gridControl1.DataSource = ImageADOs;
                gridControl1.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessCheckNormal(ref TreeSereServADO hisSereServTeinSDO)
        {
            try
            {
                V_HIS_TEST_INDEX_RANGE testIndexRange = new V_HIS_TEST_INDEX_RANGE();

                var testIndexRangeAll = BackendDataWorker.Get<V_HIS_TEST_INDEX_RANGE>();
                testIndexRange = GetTestIndexRange(this._DOB, this._GENDER_ID, hisSereServTeinSDO.TEST_INDEX_CODE, ref testIndexRangeAll);
                if (testIndexRange != null)
                {
                    AssignNormal(ref hisSereServTeinSDO, testIndexRange);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void AssignNormal(ref TreeSereServADO ti, V_HIS_TEST_INDEX_RANGE testIndexRange)
        {
            try
            {
                Decimal minValue = 0, maxValue = 0, value = 0;
                if (ti != null && testIndexRange != null)
                {
                    ti.DESCRIPTION = "";
                    if (!String.IsNullOrWhiteSpace(testIndexRange.MIN_VALUE))
                    {
                        if (Decimal.TryParse((testIndexRange.MIN_VALUE ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out minValue))
                        {
                            ti.MIN_VALUE = minValue;
                        }
                        else
                        {
                            ti.MIN_VALUE = null;
                        }
                    }
                    if (!String.IsNullOrWhiteSpace(testIndexRange.MAX_VALUE))
                    {
                        if (Decimal.TryParse((testIndexRange.MAX_VALUE ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out maxValue))
                        {
                            ti.MAX_VALUE = maxValue;
                        }
                        else
                        {
                            ti.MAX_VALUE = null;
                        }
                    }

                    if (!String.IsNullOrWhiteSpace(ti.VALUE_RANGE))
                    {
                        if (Decimal.TryParse((ti.VALUE_RANGE ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out value))
                        {
                            ti.VALUE = value;
                        }
                        else
                        {
                            ti.VALUE = null;
                        }
                    }

                    ti.IS_NORMAL = null;
                    ti.IS_HIGHER = null;
                    ti.IS_LOWER = null;
                    if (!String.IsNullOrEmpty(testIndexRange.NORMAL_VALUE))
                    {
                        ti.DESCRIPTION = testIndexRange.NORMAL_VALUE;
                        if (!String.IsNullOrWhiteSpace(ti.DESCRIPTION) && !String.IsNullOrWhiteSpace(ti.VALUE_RANGE) && ti.VALUE_RANGE.ToUpper() == ti.DESCRIPTION.ToUpper())
                        {
                            ti.IS_NORMAL = true;
                        }
                        else if (!String.IsNullOrWhiteSpace(ti.DESCRIPTION) && !String.IsNullOrWhiteSpace(ti.VALUE_RANGE) && ti.VALUE_RANGE.ToUpper() != ti.DESCRIPTION.ToUpper())
                        {
                            ti.IS_LOWER = true;
                            ti.IS_HIGHER = true;
                        }
                    }
                    else
                    {
                        if (testIndexRange.IS_ACCEPT_EQUAL_MIN == 1 && testIndexRange.IS_ACCEPT_EQUAL_MAX == null)
                        {
                            if (testIndexRange.MIN_VALUE != null)
                            {
                                ti.DESCRIPTION += testIndexRange.MIN_VALUE + "<= ";
                            }

                            ti.DESCRIPTION += "X";

                            if (testIndexRange.MAX_VALUE != null)
                            {
                                ti.DESCRIPTION += " < " + testIndexRange.MAX_VALUE;
                            }

                            if (ti.VALUE != null && ti.MIN_VALUE != null && ti.MIN_VALUE <= ti.VALUE && ti.MAX_VALUE != null && ti.VALUE < ti.MAX_VALUE)
                            {
                                ti.IS_NORMAL = true;
                            }
                            else if (ti.VALUE != null && ti.MIN_VALUE != null && ti.VALUE < ti.MIN_VALUE)
                            {
                                ti.IS_LOWER = true;
                            }
                            else if (ti.VALUE != null && ti.MAX_VALUE != null && ti.MAX_VALUE <= ti.VALUE)
                            {
                                ti.IS_HIGHER = true;
                            }
                        }
                        else if (testIndexRange.IS_ACCEPT_EQUAL_MIN == 1 && testIndexRange.IS_ACCEPT_EQUAL_MAX == 1)
                        {
                            if (testIndexRange.MIN_VALUE != null)
                            {
                                ti.DESCRIPTION += testIndexRange.MIN_VALUE + "<= ";
                            }

                            ti.DESCRIPTION += "X";

                            if (testIndexRange.MAX_VALUE != null)
                            {
                                ti.DESCRIPTION += " <= " + testIndexRange.MAX_VALUE;
                            }

                            if (ti.VALUE != null && ti.MIN_VALUE != null && ti.MIN_VALUE <= ti.VALUE && ti.MAX_VALUE != null && ti.VALUE <= ti.MAX_VALUE)
                            {
                                ti.IS_NORMAL = true;
                            }
                            else if (ti.VALUE != null && ti.MIN_VALUE != null && ti.VALUE < ti.MIN_VALUE)
                            {
                                ti.IS_LOWER = true;
                            }
                            else if (ti.VALUE != null && ti.MAX_VALUE != null && ti.MAX_VALUE < ti.VALUE)
                            {
                                ti.IS_HIGHER = true;
                            }
                        }
                        else if (testIndexRange.IS_ACCEPT_EQUAL_MIN == null && testIndexRange.IS_ACCEPT_EQUAL_MAX == 1)
                        {
                            if (testIndexRange.MIN_VALUE != null)
                            {
                                ti.DESCRIPTION += testIndexRange.MIN_VALUE + "< ";
                            }

                            ti.DESCRIPTION += "X";

                            if (testIndexRange.MAX_VALUE != null)
                            {
                                ti.DESCRIPTION += " <= " + testIndexRange.MAX_VALUE;
                            }

                            if (ti.VALUE != null && ti.MIN_VALUE != null && ti.MIN_VALUE < ti.VALUE && ti.MAX_VALUE != null && ti.VALUE <= ti.MAX_VALUE)
                            {
                                ti.IS_NORMAL = true;
                            }
                            else if (ti.VALUE != null && ti.MIN_VALUE != null && ti.VALUE < ti.MIN_VALUE)
                            {
                                ti.IS_LOWER = true;
                            }
                            else if (ti.VALUE != null && ti.MAX_VALUE != null && ti.MAX_VALUE < ti.VALUE)
                            {
                                ti.IS_HIGHER = true;
                            }
                        }
                        else if (testIndexRange.IS_ACCEPT_EQUAL_MIN == null && testIndexRange.IS_ACCEPT_EQUAL_MAX == null)
                        {
                            if (testIndexRange.MIN_VALUE != null)
                            {
                                ti.DESCRIPTION += testIndexRange.MIN_VALUE + "< ";
                            }

                            ti.DESCRIPTION += "X";

                            if (testIndexRange.MAX_VALUE != null)
                            {
                                ti.DESCRIPTION += " < " + testIndexRange.MAX_VALUE;
                            }
                            if (ti.VALUE != null && ti.MIN_VALUE != null && ti.MIN_VALUE < ti.VALUE && ti.MAX_VALUE != null && ti.VALUE < ti.MAX_VALUE)
                            {
                                ti.IS_NORMAL = true;
                            }
                            else if (ti.VALUE != null && ti.MIN_VALUE != null && ti.VALUE <= ti.MIN_VALUE)
                            {
                                ti.IS_LOWER = true;
                            }
                            else if (ti.VALUE != null && ti.MAX_VALUE != null && ti.MAX_VALUE <= ti.VALUE)
                            {
                                ti.IS_HIGHER = true;
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        MOS.EFMODEL.DataModels.V_HIS_TEST_INDEX_RANGE GetTestIndexRange(long dob, long genderCode, string testIndexId, ref List<V_HIS_TEST_INDEX_RANGE> testIndexRanges)
        {
            MOS.EFMODEL.DataModels.V_HIS_TEST_INDEX_RANGE testIndexRange = new V_HIS_TEST_INDEX_RANGE();
            try
            {
                if (testIndexRanges != null && testIndexRanges.Count > 0)
                {
                    double age = 0;
                    List<V_HIS_TEST_INDEX_RANGE> query = new List<V_HIS_TEST_INDEX_RANGE>();
                    var ageDate = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(dob);
                    foreach (var item in testIndexRanges)
                    {
                        if (item.TEST_INDEX_CODE == testIndexId)
                        {
                            if (item.AGE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_AGE_TYPE.ID__YEAR) // Năm
                            {
                                age = (DateTime.Now - (ageDate ?? DateTime.Now)).TotalDays / 365;
                            }
                            else if (item.AGE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_AGE_TYPE.ID__MONTH) // Tháng
                            {
                                age = (DateTime.Now - (ageDate ?? DateTime.Now)).TotalDays / 30;
                            }
                            else if (item.AGE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_AGE_TYPE.ID__DAY) // Ngày
                            {
                                age = (DateTime.Now - (ageDate ?? DateTime.Now)).TotalDays;
                            }
                            else if (item.AGE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_AGE_TYPE.ID__HOUR) // Giờ
                            {
                                age = (DateTime.Now - (ageDate ?? DateTime.Now)).TotalHours;
                            }
                            Inventec.Common.Logging.LogSystem.Debug(age + "______" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dob), dob));

                            if (((item.AGE_FROM.HasValue && item.AGE_FROM.Value <= age) || !item.AGE_FROM.HasValue)
                                 && ((item.AGE_TO.HasValue && item.AGE_TO.Value >= age) || !item.AGE_TO.HasValue))
                            {
                                query.Add(item);
                            }
                        }
                    }
                    HIS_GENDER gender = BackendDataWorker.Get<HIS_GENDER>().FirstOrDefault(p => p.ID == genderCode);
                    if (gender != null && gender.ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__MALE)
                    {
                        query = query.Where(o => o.IS_MALE == 1).ToList();
                    }
                    else if (gender != null && gender.ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__FEMALE)
                    {
                        query = query.Where(o => o.IS_FEMALE == 1).ToList();
                    }
                    testIndexRange = query.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                testIndexRange = new V_HIS_TEST_INDEX_RANGE();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return testIndexRange;
        }
    }
}
