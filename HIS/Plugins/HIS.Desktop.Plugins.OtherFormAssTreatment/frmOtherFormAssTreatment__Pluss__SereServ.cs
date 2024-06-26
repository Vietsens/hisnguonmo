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
using HIS.Desktop.Plugins.OtherFormAssTreatment.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
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

namespace HIS.Desktop.Plugins.OtherFormAssTreatment
{
    public partial class frmOtherFormAssTreatment : HIS.Desktop.Utility.FormBase
    {
        internal List<HisSereServADONumOrder> rsSereServ { get; set; }

        long _DOB = 0;
        long _GENDER_ID = 0;
        NumberStyles style;

        List<TreeSereServADO> LoadDataSS()
        {
            List<TreeSereServADO> SereServADOs = new List<TreeSereServADO>();
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();

                MOS.Filter.HisServiceReqFilter serviceReqFilter = new MOS.Filter.HisServiceReqFilter();

                serviceReqFilter.TREATMENT_ID = this.TreatmentId;
                //serviceReqFilter.SERVICE_REQ_STT_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT;
                serviceReqFilter.SERVICE_REQ_STT_IDs = new List<long>();
                serviceReqFilter.SERVICE_REQ_STT_IDs.AddRange(new List<long>
                    {
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT,
                     IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL
                    });
                //if (dtTimeFrom.EditValue != null && dtTimeFrom.DateTime != DateTime.MinValue)
                //{
                //    serviceReqFilter.INTRUCTION_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeFrom.EditValue).ToString("yyyyMMdd") + "000000");
                //}
                //if (dtTimeTo.EditValue != null && dtTimeTo.DateTime != DateTime.MinValue)
                //{
                //    serviceReqFilter.INTRUCTION_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeTo.EditValue).ToString("yyyyMMdd") + "235959");
                //}

                var rsServiceReq = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumers.MosConsumer, serviceReqFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);


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
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT
                    });

                    var dataSereServs = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GET, ApiConsumers.MosConsumer, ssFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);

                    List<HIS_SERE_SERV> dataTests = new List<HIS_SERE_SERV>();
                    Dictionary<long, HIS_SERE_SERV_EXT> dicSereServExt = new Dictionary<long, HIS_SERE_SERV_EXT>();
                    List<HIS_SERE_SERV_EXT> dataSereServExts = new List<HIS_SERE_SERV_EXT>();
                    if (dataSereServs != null && dataSereServs.Count > 0)
                    {
                        dataTests = dataSereServs.Where(p => p.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN).ToList();

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

                    #region ---CreateTree---
                    
                    //treeListServiceReq.DataSource = null;
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
                            // var listBySety234 = itemSS.ToList<HisSereServADONumOrder>().GroupBy(p => p.SERVICE_REQ_ID).ToList();
                            TreeSereServADO ssServiceType = new TreeSereServADO();
                            ssServiceType.CONCRETE_ID__IN_SETY = ssInTime.CONCRETE_ID__IN_SETY + "_" + itemSS.First().TDL_SERVICE_TYPE_ID + "";
                            ssServiceType.PARENT_ID__IN_SETY = ssInTime.CONCRETE_ID__IN_SETY;
                            var serviceType = BackendDataWorker.Get<HIS_SERVICE_TYPE>().FirstOrDefault(p => p.ID == itemSS.First().TDL_SERVICE_TYPE_ID);
                            ssServiceType.SERVICE_REQ_CODE = (serviceType != null ? serviceType.SERVICE_TYPE_NAME : "");
                            if (itemSS.First().TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN)
                            {
                                ssServiceType.NUM_ORDER = 1;
                            }
                            else
                            {
                                ssServiceType.NUM_ORDER = 2;
                            }
                            ssServiceType.TDL_SERVICE_TYPE_ID = itemSS.First().TDL_SERVICE_TYPE_ID;
                            SereServADOs.Add(ssServiceType);
                            int d = 0;
                            foreach (var item in itemSS)
                            {
                                d++;
                                var serviceUnit = BackendDataWorker.Get<HIS_SERVICE_UNIT>().FirstOrDefault(p => p.ID == item.TDL_SERVICE_UNIT_ID);
                                TreeSereServADO leaf = new TreeSereServADO(item);
                                leaf.CONCRETE_ID__IN_SETY = ssServiceType.CONCRETE_ID__IN_SETY + "_" + d;//ssServiceReq
                                leaf.PARENT_ID__IN_SETY = ssServiceType.CONCRETE_ID__IN_SETY;//ssServiceReq
                                leaf.SERVICE_REQ_CODE = item.TDL_SERVICE_NAME;
                                leaf.NUM_ORDER = ssServiceType.NUM_ORDER;
                                if (item.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN)
                                {
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
                                            continue;
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
                                                leafXN.IsLeaf = true;
                                                leafXN.NUM_ORDER = ssServiceType.NUM_ORDER;
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
                                        leaf.IsLeaf = true;
                                        leaf.VALUE_RANGE = dicSereServExt[leaf.ID].CONCLUDE;
                                    }
                                }
                                leaf.SERVICE_UNIT_NAME = (serviceUnit != null ? serviceUnit.SERVICE_UNIT_NAME : "");
                                SereServADOs.Add(leaf);
                            }
                        }
                    }

                    SereServADOs = SereServADOs.OrderByDescending(o => o.TDL_INTRUCTION_DATE).ThenBy(p => p.NUM_ORDER).ToList();

                    //treeListServiceReq.DataSource = records;
                    //treeListServiceReq.ExpandAll();
                    //treeSereServ_CheckAllNode(treeListServiceReq.Nodes);
                    #endregion
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                SereServADOs = new List<TreeSereServADO>();
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return SereServADOs;
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
                    long age = Inventec.Common.DateTime.Calculation.Age(dob);

                    var query = testIndexRanges.Where(o => o.TEST_INDEX_CODE == testIndexId
                            && ((o.AGE_FROM.HasValue && o.AGE_FROM.Value <= age) || !o.AGE_FROM.HasValue)
                            && ((o.AGE_TO.HasValue && o.AGE_TO.Value >= age) || !o.AGE_TO.HasValue));
                    HIS_GENDER gender = BackendDataWorker.Get<HIS_GENDER>().FirstOrDefault(p => p.ID == genderCode);
                    if (gender != null && gender.ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__MALE)
                    {
                        query = query.Where(o => o.IS_MALE == 1);
                    }
                    else if (gender != null && gender.ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__FEMALE)
                    {
                        query = query.Where(o => o.IS_FEMALE == 1);
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
