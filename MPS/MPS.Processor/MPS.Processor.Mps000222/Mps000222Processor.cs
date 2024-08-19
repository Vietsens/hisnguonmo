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
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000222.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000222
{
    public class Mps000222Processor : AbstractProcessor
    {
        Mps000222PDO rdo;
        List<Mps000222ADO> GroupByParentServiceTest = new List<Mps000222ADO>();
        List<Mps000222ADO> GroupByParentServiceAndServiceReq = new List<Mps000222ADO>();
        List<Mps000222ADO> ExesereServs = new List<Mps000222ADO>();
        List<Mps000222ADO> Service = new List<Mps000222ADO>();
        List<Mps000222ADO_CLS> ServiceCLS = new List<Mps000222ADO_CLS>();
        List<Mps000222ADO_CLS> ServiceCLSFull = new List<Mps000222ADO_CLS>();
        NumberStyles style = NumberStyles.Any;

        const string config = "_Once";
        bool isOnce = false;

        public Mps000222Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            isOnce = printData.fileName.Contains(config);
            rdo = (Mps000222PDO)rdoBase;
        }

        public override bool ProcessData()
        {
            bool result = true;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));

                ProcessSingleKey();
                //SetData();
                //SetBarcode();
                //SetService();

                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);
                objectTag.AddObjectData(store, "ServiceCLS", ServiceCLS);
                objectTag.AddObjectData(store, "ServiceCLSFull", ServiceCLSFull);
                objectTag.AddObjectData(store, "ServiceTest", ExesereServs);
                objectTag.AddObjectData(store, "Service", Service);
                objectTag.AddObjectData(store, "ExpMediMate", rdo.ListExpMestMedcine);
                objectTag.AddRelationship(store, "Service", "ServiceTest", "SERVICE_ID", "SERVICE_ID");
                objectTag.AddObjectData(store, "Service1", GroupByParentServiceAndServiceReq);
                objectTag.AddObjectData(store, "ServiceTestIndex", GroupByParentServiceTest);
                objectTag.AddRelationship(store, "ServiceTestIndex", "ServiceTest", new string[] { "SERVICE_CODE_1", "INTRUCTION_TIME_1" }, new string[] { "SERVICE_CODE_1", "INTRUCTION_TIME_1" });
                objectTag.AddRelationship(store, "Service1", "ServiceTest", new string[] { "SERVICE_ID", "SERVICE_REQ_ID" }, new string[] { "SERVICE_ID", "SERVICE_REQ_ID" });
                objectTag.AddRelationship(store, "ServiceCLS", "ServiceCLSFull", new string[] { "SERVICE_TYPE_ID", "DIIM_TYPE_ID", "FUEX_TYPE_ID", "INTRUCTION_TIME" }, new string[] { "SERVICE_TYPE_ID", "DIIM_TYPE_ID", "FUEX_TYPE_ID", "INTRUCTION_TIME" });
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void ProcessSingleKey()
        {
            Thread service = new Thread(SetService);
            Thread barCode = new Thread(SetBarcode);
            Thread data = new Thread(SetData);

            try
            {
                service.Start();
                barCode.Start();
                data.Start();

                service.Join();
                barCode.Join();
                data.Join();
            }
            catch (Exception ex)
            {
                service.Abort();
                barCode.Abort();
                data.Abort();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void SetDataGroupServiceTest()
		{
			try
			{
                if(ExesereServs!=null && ExesereServs.Count > 0)
				{
                    var group = ExesereServs.GroupBy(o => new { o.SERVICE_CODE_1, o.INTRUCTION_TIME_1 }).ToList();
					foreach (var item in group)
					{
                        Mps000222ADO ado = new Mps000222ADO();
                        ado.SERVICE_CODE_1 = item.FirstOrDefault().SERVICE_CODE_1;
                        ado.SERVICE_NAME = rdo.HisServices.FirstOrDefault(o=>o.SERVICE_CODE == item.FirstOrDefault().SERVICE_CODE_1).SERVICE_NAME;
                        ado.INTRUCTION_TIME_1 = item.FirstOrDefault().INTRUCTION_TIME_1;
                        GroupByParentServiceTest.Add(ado);
                    }
                }                    
			}
			catch (Exception ex)
			{
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
		}
        private void SetService()
        {
            try
            {
                if (rdo.VHisSereServTeins != null && rdo.HisSereServs != null)
                {
                    var dicSereServTeins = new Dictionary<long, List<V_HIS_SERE_SERV_TEIN>>();
                    if (rdo.VHisSereServTeins.Count > 0)
                    {
                        //sắp xếp lại danh sách thông số
                        rdo.VHisSereServTeins = rdo.VHisSereServTeins.OrderByDescending(o => o.NUM_ORDER).ToList();
                        foreach (var item in rdo.VHisSereServTeins)
                        {
                            if (!dicSereServTeins.ContainsKey(item.SERE_SERV_ID))
                                dicSereServTeins[item.SERE_SERV_ID] = new List<V_HIS_SERE_SERV_TEIN>();
                            dicSereServTeins[item.SERE_SERV_ID].Add(item);
                        }
                    }

                    if (rdo.HisSereServs.Count > 0)
                    {
                        var testParent = rdo.HisServices.Where(o => !o.PARENT_ID.HasValue || (o.PARENT_ID.HasValue && o.IS_LEAF != 1)).ToList();
                        if (testParent != null && testParent.Count > 0)
                        {
                            foreach (var serviceParent in testParent)
                            {
                                var parent = new Mps000222ADO();
                                parent.SERVICE_ID = serviceParent.ID;
                                parent.SERVICE_NAME = serviceParent.SERVICE_NAME;

                                List<Mps000222ADO> exeSeseTein = new List<Mps000222ADO>();
                                var listService = rdo.HisServices.Where(o => o.PARENT_ID == serviceParent.ID).ToList();
                                if (listService != null && listService.Count > 0)
                                {
                                    var serviceId = listService.Select(s => s.ID).ToList();
                                    var listSereServ = rdo.HisSereServs.Where(o => serviceId.Contains(o.SERVICE_ID)).ToList();
                                    if (listSereServ != null && listSereServ.Count > 0)
                                    {
                                        //sắp xếp theo thứ tự chỉ định
                                        var lstServId = listSereServ.Select(o => o.SERVICE_ID).ToList();
                                        var lstServiceOrder = listService.Where(o => lstServId.Contains(o.ID)).OrderByDescending(o => o.NUM_ORDER).ThenBy(o => o.SERVICE_NAME).ToList();
                                        for (int i = 0; i < lstServiceOrder.Count; i++)
                                        {
                                            var seses = listSereServ.Where(o => o.SERVICE_ID == lstServiceOrder[i].ID).ToList();
                                            if (seses != null && seses.Count > 0)
                                            {
                                                foreach (var sese in seses)
                                                {
                                                    Mps000222ADO ReqAndParentSv = new Mps000222ADO();
                                                    V_HIS_SERVICE_REQ serviceReq = rdo.VHisServiceReqTests.FirstOrDefault(o => o.ID == sese.SERVICE_REQ_ID);
                                                    if (serviceReq != null && serviceReq.START_TIME > 0)
                                                    {
                                                        ReqAndParentSv.FINISH_TIME_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(serviceReq.FINISH_TIME ?? 0);
                                                        ReqAndParentSv.START_TIME_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(serviceReq.START_TIME ?? 0);
                                                        ReqAndParentSv.REQUEST_DEPARTMENT_NAME = serviceReq.REQUEST_DEPARTMENT_NAME;
                                                        ReqAndParentSv.SERVICE_ID = serviceParent.ID;
                                                        ReqAndParentSv.SERVICE_NAME = serviceParent.SERVICE_NAME;
                                                        ReqAndParentSv.SERVICE_REQ_ID = serviceReq.ID;
                                                        ReqAndParentSv.INTRUCTION_TIME_1 = serviceReq.INTRUCTION_TIME;
                                                        if (!this.GroupByParentServiceAndServiceReq.Exists(o => o.SERVICE_ID == ReqAndParentSv.SERVICE_ID && o.SERVICE_REQ_ID == ReqAndParentSv.SERVICE_REQ_ID))
                                                        {
                                                            this.GroupByParentServiceAndServiceReq.Add(ReqAndParentSv);
                                                        }
                                                    }

                                                    if (dicSereServTeins.ContainsKey(sese.ID))
                                                    {
                                                        foreach (var sereServTein in dicSereServTeins[sese.ID])
                                                        {
                                                            if (String.IsNullOrEmpty(sereServTein.VALUE)) continue;
                                                            Mps000222ADO exeSereServTein = new Mps000222ADO();
                                                            exeSereServTein.SERVICE_ID = serviceParent.ID;
                                                            exeSereServTein.SERVICE_REQ_ID = ReqAndParentSv.SERVICE_REQ_ID;
                                                            exeSereServTein.TEST_INDEX_CODE_1 = sereServTein.TEST_INDEX_CODE;
                                                            exeSereServTein.SERVICE_NAME_1 = sereServTein.TEST_INDEX_NAME;
                                                            exeSereServTein.TEST_INDEX_UNIT_NAME_1 = sereServTein.TEST_INDEX_UNIT_NAME;
                                                            exeSereServTein.VALUE_1 = sereServTein.VALUE;
                                                            exeSereServTein.VALUE_RANGE_1 = sereServTein.DESCRIPTION;
                                                            exeSereServTein.RESULT_CODE_1 = sereServTein.RESULT_CODE;
                                                            exeSereServTein.INTRUCTION_TIME_1 = sese.TDL_INTRUCTION_TIME;
                                                            exeSereServTein.SERVICE_CODE_1 = sese.TDL_SERVICE_CODE;
                                                            exeSereServTein.BACTERIUM_NAME = sereServTein.BACTERIUM_NAME;
                                                            exeSereServTein.ANTIBIOTIC_NAME = sereServTein.ANTIBIOTIC_RESISTANCE_NAME;
                                                            exeSereServTein.SRI_CODE = sereServTein.SRI_CODE;
                                                            V_HIS_TEST_INDEX_RANGE testIndexRange = new V_HIS_TEST_INDEX_RANGE();
                                                            testIndexRange = GetTestIndexRange(serviceReq.TDL_PATIENT_DOB, serviceReq.TDL_PATIENT_GENDER_ID, sereServTein.TEST_INDEX_ID, rdo.testIndexRange);
                                                            if (testIndexRange != null)
                                                            {
                                                                ProcessMaxMixValue(exeSereServTein, testIndexRange);
                                                            }

                                                            exeSeseTein.Add(exeSereServTein);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                if (exeSeseTein.Count > 0)
                                {
                                    List<Mps000222ADO> exeSereServ = new List<Mps000222ADO>();
                                    if (!isOnce)
                                    {
                                        int count = (exeSeseTein.Count % 2) == 1 ? ((exeSeseTein.Count / 2) + 1) : (exeSeseTein.Count / 2);
                                        for (int i = 0; i < count; i++)
                                        {
                                            var tein = new Mps000222ADO();
                                            tein.SERVICE_ID = exeSeseTein[i].SERVICE_ID;

                                            tein.RESULT_CODE_1 = exeSeseTein[i].RESULT_CODE_1;
                                            tein.SERVICE_NAME_1 = exeSeseTein[i].SERVICE_NAME_1;
                                            tein.TEST_INDEX_RANGE_1 = exeSeseTein[i].TEST_INDEX_RANGE_1;
                                            tein.TEST_INDEX_UNIT_NAME_1 = exeSeseTein[i].TEST_INDEX_UNIT_NAME_1;
                                            tein.VALUE_1 = exeSeseTein[i].VALUE_1;
                                            tein.VALUE_RANGE_1 = exeSeseTein[i].VALUE_RANGE_1;
                                            tein.INTRUCTION_TIME_1 = exeSeseTein[i].INTRUCTION_TIME_1;
                                            tein.SERVICE_CODE_1 = exeSeseTein[i].SERVICE_CODE_1;
                                            tein.TEST_INDEX_CODE_1 = exeSeseTein[i].TEST_INDEX_CODE_1;
                                            if (i + count < exeSeseTein.Count)
                                            {
                                                tein.RESULT_CODE_2 = exeSeseTein[i + count].RESULT_CODE_1;
                                                tein.SERVICE_NAME_2 = exeSeseTein[i + count].SERVICE_NAME_1;
                                                tein.TEST_INDEX_RANGE_2 = exeSeseTein[i + count].TEST_INDEX_RANGE_1;
                                                tein.TEST_INDEX_UNIT_NAME_2 = exeSeseTein[i + count].TEST_INDEX_UNIT_NAME_1;
                                                tein.VALUE_2 = exeSeseTein[i + count].VALUE_1;
                                                tein.VALUE_RANGE_2 = exeSeseTein[i + count].VALUE_RANGE_1;
                                                tein.INTRUCTION_TIME_2 = exeSeseTein[i + count].INTRUCTION_TIME_1;
                                                tein.SERVICE_CODE_2 = exeSeseTein[i + count].SERVICE_CODE_2;
                                                tein.TEST_INDEX_CODE_2 = exeSeseTein[i + count].TEST_INDEX_CODE_2;
                                            }
                                            tein.BACTERIUM_NAME = exeSeseTein[i].BACTERIUM_NAME;
                                            tein.ANTIBIOTIC_NAME = exeSeseTein[i].ANTIBIOTIC_RESISTANCE_NAME;
                                            tein.SRI_CODE = exeSeseTein[i].SRI_CODE;
                                            exeSereServ.Add(tein);
                                        }
                                    }
                                    else
                                    {
                                        exeSereServ = exeSeseTein;
                                    }

                                    this.ExesereServs.AddRange(exeSereServ);
                                    this.Service.Add(parent);
                                }
                            }
                        }
                    }
                    SetDataGroupServiceTest();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        MOS.EFMODEL.DataModels.V_HIS_TEST_INDEX_RANGE GetTestIndexRange(long dob, long? genderId, long? testIndexId, List<V_HIS_TEST_INDEX_RANGE> testIndexRanges)
        {
            MOS.EFMODEL.DataModels.V_HIS_TEST_INDEX_RANGE testIndexRange = new V_HIS_TEST_INDEX_RANGE();
            try
            {
                if (testIndexRanges != null && testIndexRanges.Count > 0)
                {
                    long? now = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                    int age = 0;

                    List<V_HIS_TEST_INDEX_RANGE> query = new List<V_HIS_TEST_INDEX_RANGE>();

                    foreach (var item in testIndexRanges)
                    {
                        if (item.TEST_INDEX_ID == testIndexId)
                        {
                            if (item.AGE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_AGE_TYPE.ID__YEAR)
                            {
                                age = Inventec.Common.DateTime.Calculation.DifferenceTime(dob, now ?? 0, Inventec.Common.DateTime.Calculation.UnitDifferenceTime.DAY)/365;
                            }
                            else if (item.AGE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_AGE_TYPE.ID__MONTH)
                            {
                                age = Inventec.Common.DateTime.Calculation.DifferenceMonth(dob, now ?? 0);
                            }
                            else if (item.AGE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_AGE_TYPE.ID__DAY)
                            {
                                age = Inventec.Common.DateTime.Calculation.DifferenceDate(dob, now ?? 0);
                            }
                            else if (item.AGE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_AGE_TYPE.ID__HOUR)
                            {
                                age = Inventec.Common.DateTime.Calculation.DifferenceTime(dob, now ?? 0, Inventec.Common.DateTime.Calculation.UnitDifferenceTime.HOUR);
                            }
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("item.AGE_TYPE_ID:", item.AGE_TYPE_ID));
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("dob:", dob));
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("age:", age));

                            if (((item.AGE_FROM.HasValue && item.AGE_FROM.Value <= age) || !item.AGE_FROM.HasValue)
                            && ((item.AGE_TO.HasValue && item.AGE_TO.Value >= age) || !item.AGE_TO.HasValue))
                            {
                                query.Add(item);
                            }
                        }
                    }
                    if (genderId == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__MALE)
                    {
                        query = query.Where(o => o.IS_MALE == 1).ToList();
                    }
                    else if (genderId == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__FEMALE)
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

        private void ProcessMaxMixValue(Mps000222ADO ti, V_HIS_TEST_INDEX_RANGE testIndexRange)
        {
            try
            {
                Decimal minValue = 0, maxValue = 0, value = 0;
                if (ti != null && testIndexRange != null)
                {
                    Decimal value1 = 0;
                    Decimal.TryParse((ti.VALUE_1 ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out value1);
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

                    if (!String.IsNullOrWhiteSpace(ti.VALUE_1))
                    {
                        Decimal.TryParse((ti.VALUE_1 ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out value);
                    }
                    //ti.VALUE_HL = ti.VALUE + "";

                    if (!String.IsNullOrEmpty(testIndexRange.NORMAL_VALUE))
                    {
                        ti.VALUE_RANGE = testIndexRange.NORMAL_VALUE;
                        if (!String.IsNullOrWhiteSpace(ti.VALUE_RANGE) && ti.VALUE_1 != null && ti.VALUE_1.ToString().ToUpper() == ti.VALUE_RANGE.ToUpper())
                        {
                            ti.HIGH_OR_LOW = "";
                        }
                        else
                        {
                            ti.HIGH_OR_LOW = " ";
                        }
                    }
                    else
                    {
                        ti.VALUE_RANGE = "";

                        if (testIndexRange.IS_ACCEPT_EQUAL_MIN == 1 && testIndexRange.IS_ACCEPT_EQUAL_MAX == null)
                        {
                            if (testIndexRange.MIN_VALUE != null)
                            {
                                ti.VALUE_RANGE += testIndexRange.MIN_VALUE + "<= ";
                            }

                            ti.VALUE_RANGE += "X";

                            if (testIndexRange.MAX_VALUE != null)
                            {
                                ti.VALUE_RANGE += " < " + testIndexRange.MAX_VALUE;
                            }

                            if (ti.VALUE_1 != null && ti.MIN_VALUE != null && ti.MIN_VALUE <= value1 && ti.MAX_VALUE != null && value1 < ti.MAX_VALUE)
                            {
                                ti.HIGH_OR_LOW = "";
                                //ti.VALUE_HL = ti.VALUE + "";
                            }
                            else if (ti.VALUE_1 != null && ti.MIN_VALUE != null && value1 < ti.MIN_VALUE)
                            {
                                ti.HIGH_OR_LOW = "L";
                                //ti.VALUE_HL = ti.VALUE + "L";
                            }
                            else if (ti.VALUE_1 != null && ti.MAX_VALUE != null && ti.MAX_VALUE <= value1)
                            {
                                ti.HIGH_OR_LOW = "H";
                                //ti.VALUE_HL = ti.VALUE + "H";
                            }
                            else
                            {
                                ti.HIGH_OR_LOW = "";
                                //ti.VALUE_HL = ti.VALUE + "";
                            }
                        }
                        else if (testIndexRange.IS_ACCEPT_EQUAL_MIN == 1 && testIndexRange.IS_ACCEPT_EQUAL_MAX == 1)
                        {
                            if (testIndexRange.MIN_VALUE != null)
                            {
                                ti.VALUE_RANGE += testIndexRange.MIN_VALUE + "<= ";
                            }

                            ti.VALUE_RANGE += "X";

                            if (testIndexRange.MAX_VALUE != null)
                            {
                                ti.VALUE_RANGE += " <= " + testIndexRange.MAX_VALUE;
                            }

                            if (ti.VALUE_1 != null && ti.MIN_VALUE != null && ti.MIN_VALUE <= value1 && ti.MAX_VALUE != null && value1 <= ti.MAX_VALUE)
                            {
                                ti.HIGH_OR_LOW = "";
                                //ti.VALUE_HL = ti.VALUE + "";
                            }
                            else if (ti.VALUE_1 != null && ti.MIN_VALUE != null && value1 < ti.MIN_VALUE)
                            {
                                ti.HIGH_OR_LOW = "L";
                                //ti.VALUE_HL = ti.VALUE + "L";
                            }
                            else if (ti.VALUE_1 != null && ti.MAX_VALUE != null && ti.MAX_VALUE < value1)
                            {
                                ti.HIGH_OR_LOW = "H";
                                //ti.VALUE_HL = ti.VALUE + "H";
                            }
                            else
                            {
                                ti.HIGH_OR_LOW = "";
                                //ti.VALUE_HL = ti.VALUE_RANGE + "";
                            }
                        }
                        else if (testIndexRange.IS_ACCEPT_EQUAL_MIN == null && testIndexRange.IS_ACCEPT_EQUAL_MAX == 1)
                        {
                            if (testIndexRange.MIN_VALUE != null)
                            {
                                ti.VALUE_RANGE += testIndexRange.MIN_VALUE + "< ";
                            }

                            ti.VALUE_RANGE += "X";

                            if (testIndexRange.MAX_VALUE != null)
                            {
                                ti.VALUE_RANGE += " <= " + testIndexRange.MAX_VALUE;
                            }

                            if (ti.VALUE_1 != null && ti.MIN_VALUE != null && ti.MIN_VALUE < value1 && ti.MAX_VALUE != null && value1 <= ti.MAX_VALUE)
                            {
                                ti.HIGH_OR_LOW = "";
                                //ti.VALUE_HL = ti.VALUE + "";
                            }
                            else if (ti.VALUE_1 != null && ti.MIN_VALUE != null && value1 < ti.MIN_VALUE)
                            {
                                ti.HIGH_OR_LOW = "L";
                                //ti.VALUE_HL = ti.VALUE + "L";
                            }
                            else if (ti.VALUE_1 != null && ti.MAX_VALUE != null && ti.MAX_VALUE < value1)
                            {
                                ti.HIGH_OR_LOW = "H";
                                //ti.VALUE_HL = ti.VALUE + "H";
                            }
                            else
                            {
                                ti.HIGH_OR_LOW = "";
                                //ti.VALUE_HL = ti.VALUE_RANGE + "";
                            }
                        }
                        else if (testIndexRange.IS_ACCEPT_EQUAL_MIN == null && testIndexRange.IS_ACCEPT_EQUAL_MAX == null)
                        {
                            if (testIndexRange.MIN_VALUE != null)
                            {
                                ti.VALUE_RANGE += testIndexRange.MIN_VALUE + "< ";
                            }

                            ti.VALUE_RANGE += "X";

                            if (testIndexRange.MAX_VALUE != null)
                            {
                                ti.VALUE_RANGE += " < " + testIndexRange.MAX_VALUE;
                            }

                            if (ti.VALUE_1 != null && ti.MIN_VALUE != null && ti.MIN_VALUE < value1 && ti.MAX_VALUE != null && value1 < ti.MAX_VALUE)
                            {
                                ti.HIGH_OR_LOW = "";
                                //ti.VALUE_HL = ti.VALUE + "";
                            }
                            else if (ti.VALUE_1 != null && ti.MIN_VALUE != null && value1 <= ti.MIN_VALUE)
                            {
                                ti.HIGH_OR_LOW = "L";
                                //ti.VALUE_HL = ti.VALUE + "L";
                            }
                            else if (ti.VALUE_1 != null && ti.MAX_VALUE != null && ti.MAX_VALUE <= value1)
                            {
                                ti.HIGH_OR_LOW = "H";
                                //ti.VALUE_HL = ti.VALUE + "H";
                            }
                            else
                            {
                                ti.HIGH_OR_LOW = "";
                                //ti.VALUE_HL = ti.VALUE_RANGE + "";
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

        private void SetData()
        {
            try
            {
                if (rdo.HisTreatment != null)
                {
                    SetSingleKey(new KeyValue(Mps000222ExtendSingleKey.ADDRESS, rdo.HisTreatment.TDL_PATIENT_ADDRESS));
                    SetSingleKey(new KeyValue(Mps000222ExtendSingleKey.VIR_ADDRESS, rdo.HisTreatment.TDL_PATIENT_ADDRESS));
                    SetSingleKey(new KeyValue(Mps000222ExtendSingleKey.PATIENT_CODE, rdo.HisTreatment.TDL_PATIENT_CODE));
                    SetSingleKey(new KeyValue(Mps000222ExtendSingleKey.DOB, rdo.HisTreatment.TDL_PATIENT_DOB));
                    SetSingleKey(new KeyValue(Mps000222ExtendSingleKey.GENDER_NAME, rdo.HisTreatment.TDL_PATIENT_GENDER_NAME));
                    SetSingleKey(new KeyValue(Mps000222ExtendSingleKey.MILITARY_RANK_NAME, rdo.HisTreatment.TDL_PATIENT_MILITARY_RANK_NAME));
                    SetSingleKey(new KeyValue(Mps000222ExtendSingleKey.VIR_PATIENT_NAME, rdo.HisTreatment.TDL_PATIENT_NAME));
                    SetSingleKey(new KeyValue(Mps000222ExtendSingleKey.NATIONAL_NAME, rdo.HisTreatment.TDL_PATIENT_NATIONAL_NAME));
                    SetSingleKey(new KeyValue(Mps000222ExtendSingleKey.WORK_PLACE, rdo.HisTreatment.TDL_PATIENT_WORK_PLACE));
                    SetSingleKey(new KeyValue(Mps000222ExtendSingleKey.WORK_PLACE_NAME, rdo.HisTreatment.TDL_PATIENT_WORK_PLACE_NAME));
                }

                if (rdo.VHisServiceReqExams != null && rdo.VHisServiceReqExams.Count > 0)
                {
                    var serviceReq = new V_HIS_SERVICE_REQ();

                    serviceReq.FULL_EXAM = string.Join(";", rdo.VHisServiceReqExams.Select(s => s.FULL_EXAM).Distinct().ToList());
                    serviceReq.PART_EXAM = string.Join(";", rdo.VHisServiceReqExams.Select(s => s.PART_EXAM).Distinct().ToList());
                    serviceReq.PART_EXAM_CIRCULATION = string.Join(";", rdo.VHisServiceReqExams.Select(s => s.PART_EXAM_CIRCULATION).Distinct().ToList());
                    serviceReq.PART_EXAM_DIGESTION = string.Join(";", rdo.VHisServiceReqExams.Select(s => s.PART_EXAM_DIGESTION).Distinct().ToList());
                    serviceReq.PART_EXAM_ENT = string.Join(";", rdo.VHisServiceReqExams.Select(s => s.PART_EXAM_ENT).Distinct().ToList());
                    serviceReq.PART_EXAM_EYE = string.Join(";", rdo.VHisServiceReqExams.Select(s => s.PART_EXAM_EYE).Distinct().ToList());
                    serviceReq.PART_EXAM_KIDNEY_UROLOGY = string.Join(";", rdo.VHisServiceReqExams.Select(s => s.PART_EXAM_KIDNEY_UROLOGY).Distinct().ToList());
                    serviceReq.PART_EXAM_MENTAL = string.Join(";", rdo.VHisServiceReqExams.Select(s => s.PART_EXAM_MENTAL).Distinct().ToList());
                    serviceReq.PART_EXAM_MOTION = string.Join(";", rdo.VHisServiceReqExams.Select(s => s.PART_EXAM_MOTION).Distinct().ToList());
                    serviceReq.PART_EXAM_MUSCLE_BONE = string.Join(";", rdo.VHisServiceReqExams.Select(s => s.PART_EXAM_MUSCLE_BONE).Distinct().ToList());
                    serviceReq.PART_EXAM_NEUROLOGICAL = string.Join(";", rdo.VHisServiceReqExams.Select(s => s.PART_EXAM_NEUROLOGICAL).Distinct().ToList());
                    serviceReq.PART_EXAM_NUTRITION = string.Join(";", rdo.VHisServiceReqExams.Select(s => s.PART_EXAM_NUTRITION).Distinct().ToList());
                    serviceReq.PART_EXAM_OBSTETRIC = string.Join(";", rdo.VHisServiceReqExams.Select(s => s.PART_EXAM_OBSTETRIC).Distinct().ToList());
                    serviceReq.PART_EXAM_OEND = string.Join(";", rdo.VHisServiceReqExams.Select(s => s.PART_EXAM_OEND).Distinct().ToList());
                    serviceReq.PART_EXAM_RESPIRATORY = string.Join(";", rdo.VHisServiceReqExams.Select(s => s.PART_EXAM_RESPIRATORY).Distinct().ToList());
                    serviceReq.PART_EXAM_STOMATOLOGY = string.Join(";", rdo.VHisServiceReqExams.Select(s => s.PART_EXAM_STOMATOLOGY).Distinct().ToList());
                    serviceReq.PATHOLOGICAL_HISTORY = string.Join(";", rdo.VHisServiceReqExams.Select(s => s.PATHOLOGICAL_HISTORY).Distinct().ToList());
                    serviceReq.PATHOLOGICAL_HISTORY_FAMILY = string.Join(";", rdo.VHisServiceReqExams.Select(s => s.PATHOLOGICAL_HISTORY_FAMILY).Distinct().ToList());
                    serviceReq.PATHOLOGICAL_PROCESS = string.Join(";", rdo.VHisServiceReqExams.Select(s => s.PATHOLOGICAL_PROCESS).Distinct().ToList());

                    SetSingleKey(new KeyValue(Mps000222ExtendSingleKey.EXECUTE_ROOM_NAME, rdo.VHisServiceReqExams.FirstOrDefault().EXECUTE_ROOM_NAME));
                    SetSingleKey(new KeyValue(Mps000222ExtendSingleKey.INTRUCTION_TIME, rdo.VHisServiceReqExams.FirstOrDefault().INTRUCTION_TIME));

                    SetSingleKey(new KeyValue(Mps000222ExtendSingleKey.FULL_EXAM_STR, serviceReq.FULL_EXAM));
                    SetSingleKey(new KeyValue(Mps000222ExtendSingleKey.PART_EXAM_STR, serviceReq.PART_EXAM));
                    SetSingleKey(new KeyValue(Mps000222ExtendSingleKey.PART_EXAM_CIRCULATION_STR, serviceReq.PART_EXAM_CIRCULATION));
                    SetSingleKey(new KeyValue(Mps000222ExtendSingleKey.PART_EXAM_DIGESTION_STR, serviceReq.PART_EXAM_DIGESTION));
                    SetSingleKey(new KeyValue(Mps000222ExtendSingleKey.PART_EXAM_ENT_STR, serviceReq.PART_EXAM_ENT));
                    SetSingleKey(new KeyValue(Mps000222ExtendSingleKey.PART_EXAM_EYE_STR, serviceReq.PART_EXAM_EYE));
                    SetSingleKey(new KeyValue(Mps000222ExtendSingleKey.PART_EXAM_KIDNEY_UROLOGY_STR, serviceReq.PART_EXAM_KIDNEY_UROLOGY));
                    SetSingleKey(new KeyValue(Mps000222ExtendSingleKey.PART_EXAM_MENTAL_STR, serviceReq.PART_EXAM_MENTAL));
                    SetSingleKey(new KeyValue(Mps000222ExtendSingleKey.PART_EXAM_MOTION_STR, serviceReq.PART_EXAM_MOTION));
                    SetSingleKey(new KeyValue(Mps000222ExtendSingleKey.PART_EXAM_MUSCLE_BONE_STR, serviceReq.PART_EXAM_MUSCLE_BONE));
                    SetSingleKey(new KeyValue(Mps000222ExtendSingleKey.PART_EXAM_NEUROLOGICAL_STR, serviceReq.PART_EXAM_NEUROLOGICAL));
                    SetSingleKey(new KeyValue(Mps000222ExtendSingleKey.PART_EXAM_NUTRITION_STR, serviceReq.PART_EXAM_NUTRITION));
                    SetSingleKey(new KeyValue(Mps000222ExtendSingleKey.PART_EXAM_OBSTETRIC_STR, serviceReq.PART_EXAM_OBSTETRIC));
                    SetSingleKey(new KeyValue(Mps000222ExtendSingleKey.PART_EXAM_OEND_STR, serviceReq.PART_EXAM_OEND));
                    SetSingleKey(new KeyValue(Mps000222ExtendSingleKey.PART_EXAM_RESPIRATORY_STR, serviceReq.PART_EXAM_RESPIRATORY));
                    SetSingleKey(new KeyValue(Mps000222ExtendSingleKey.PART_EXAM_STOMATOLOGY_STR, serviceReq.PART_EXAM_STOMATOLOGY));
                    SetSingleKey(new KeyValue(Mps000222ExtendSingleKey.PATHOLOGICAL_HISTORY_STR, serviceReq.PATHOLOGICAL_HISTORY));
                    SetSingleKey(new KeyValue(Mps000222ExtendSingleKey.PATHOLOGICAL_HISTORY_FAMILY_STR, serviceReq.PATHOLOGICAL_HISTORY_FAMILY));
                    SetSingleKey(new KeyValue(Mps000222ExtendSingleKey.PATHOLOGICAL_PROCESS_STR, serviceReq.PATHOLOGICAL_PROCESS));
                }

                if (rdo.Mps000222SDO != null &&
                    rdo.Mps000222SDO.HisServiceType != null && rdo.Mps000222SDO.HisServiceType.Count > 0 &&
                    rdo.Mps000222SDO.HisSereServs != null && rdo.Mps000222SDO.HisSereServs.Count > 0 &&
                    rdo.Mps000222SDO.HisSereServsExt != null && rdo.Mps000222SDO.HisSereServsExt.Count > 0)
                {
                    List<V_HIS_SERVICE> lstServiceDiim = new List<V_HIS_SERVICE>();
                    List<V_HIS_SERVICE> lstServiceFuex = new List<V_HIS_SERVICE>();
                    foreach (var item in rdo.Mps000222SDO.HisSereServs)
					{
                        var checkService = rdo.HisServices.FirstOrDefault(o=>o.ID == item.SERVICE_ID);
                        if (checkService != null)
						{
                            if (checkService.DIIM_TYPE_ID != null)
                                lstServiceDiim.Add(checkService);
                            if (checkService.FUEX_TYPE_ID != null)
                                lstServiceFuex.Add(checkService);
                        }                            
					}
                    var groupbyServiceType = rdo.Mps000222SDO.HisSereServs.GroupBy(o => o.TDL_SERVICE_TYPE_ID).ToList();
                    foreach (var group in groupbyServiceType)
                    {
                        List<long> seseId = null;
                        List<long> seseIdOrther = null;
                        if (group.FirstOrDefault().TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA && lstServiceDiim != null && lstServiceDiim.Count > 0)
                        {
                            seseId = group.Where(o => !lstServiceDiim.Select(d => d.ID).ToList().Exists(d => d == o.SERVICE_ID))
                                            .Select(o => o.ID).ToList();
                            seseIdOrther = group.Where(o => lstServiceDiim.Select(d => d.ID).ToList().Exists(d => d == o.SERVICE_ID))
                                            .Select(o => o.ID).ToList();
                        }
                        else if (group.FirstOrDefault().TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN && lstServiceFuex != null && lstServiceFuex.Count > 0)
                        {
                            seseId = group.Where(o => !lstServiceFuex.Select(d => d.ID).ToList().Exists(d => d == o.SERVICE_ID))
                                            .Select(o => o.ID).ToList();
                            seseIdOrther = group.Where(o => lstServiceFuex.Select(d => d.ID).ToList().Exists(d => d == o.SERVICE_ID))
                                            .Select(o => o.ID).ToList();
                        }
                        else
                        {
                            seseId = group.Select(o => o.ID).ToList();
                        }
						
							var cls = new Mps000222ADO_CLS();
                            cls.SERVICE_TYPE_ID = group.FirstOrDefault().TDL_SERVICE_TYPE_ID;
                            cls.SERVICE_TYPE_NAME = rdo.Mps000222SDO.HisServiceType.FirstOrDefault(o => o.ID == group.FirstOrDefault().TDL_SERVICE_TYPE_ID).SERVICE_TYPE_NAME;                      
                                this.ServiceCLS.Add(cls);
                            

                        if(seseIdOrther!=null && seseIdOrther.Count > 0)
						{
                            //var extsOrther = rdo.Mps000222SDO.HisSereServsExt.Where(o => seseIdOrther.Contains(o.SERE_SERV_ID)).ToList();
                            //if (extsOrther != null && extsOrther.Count > 0)
                            //{
                                if(group.FirstOrDefault().TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA)
								{
                                    var lstServiceDiimIds = lstServiceDiim.Select(o => o.DIIM_TYPE_ID).Distinct().ToList();
                                   
									foreach (var item in lstServiceDiimIds)
									{                                      
                                        var clsOrther = new Mps000222ADO_CLS();
                                        clsOrther.SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA;
                                        clsOrther.DIIM_TYPE_ID = item;
                                        clsOrther.SERVICE_TYPE_NAME = rdo.lstDiimType.FirstOrDefault(o => o.ID == item).DIIM_TYPE_NAME;
                                        this.ServiceCLS.Add(clsOrther);
                                    }
                                }
                                else if (group.FirstOrDefault().TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN)
                                {
                                    var lstServiceTypeIds = lstServiceFuex.Select(o => o.FUEX_TYPE_ID).Distinct().ToList();

                                    foreach (var item in lstServiceTypeIds)
                                    {                                       
                                        var clsOrther = new Mps000222ADO_CLS();
                                        clsOrther.SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN;
                                        clsOrther.FUEX_TYPE_ID = item;
                                        clsOrther.SERVICE_TYPE_NAME = rdo.lstFuexType.FirstOrDefault(o => o.ID == item).FUEX_TYPE_NAME;
                                        this.ServiceCLS.Add(clsOrther);
                                    }
                                }                              
                            //}
                        }                                                  
                    }

                    foreach (var sereServ in rdo.Mps000222SDO.HisSereServs)
                    {
                        var cls = new Mps000222ADO_CLS();
                        cls.SERVICE_TYPE_ID = sereServ.TDL_SERVICE_TYPE_ID;
                        var objType = rdo.HisServices.FirstOrDefault(o => o.ID == sereServ.SERVICE_ID);
                        if (objType != null)
                        {
                            cls.DIIM_TYPE_ID = objType.DIIM_TYPE_ID;
                            cls.FUEX_TYPE_ID = objType.FUEX_TYPE_ID;
                        }
                        cls.SERVICE_CODE = sereServ.TDL_SERVICE_CODE;
                        cls.SERVICE_NAME = sereServ.TDL_SERVICE_NAME;
                        cls.INTRUCTION_TIME = sereServ.TDL_INTRUCTION_TIME;
                        var ext = rdo.Mps000222SDO.HisSereServsExt.FirstOrDefault(o => o.SERE_SERV_ID == sereServ.ID);
                        if (ext != null)
                        {
                            cls.CONCLUDE = ext.CONCLUDE;
                            cls.BEGIN_TIME = ext.BEGIN_TIME;
                            cls.END_TIME = ext.END_TIME;
                        }

                        this.ServiceCLSFull.Add(cls);
                    }

                   
                    var DataDate = ServiceCLSFull.Select(o => new { o.SERVICE_TYPE_ID, o.DIIM_TYPE_ID, o.FUEX_TYPE_ID, o.INTRUCTION_TIME }).Distinct().ToList();
                    var lstTemp = new List<Mps000222ADO_CLS>();
					foreach (var item in ServiceCLS)
					{
                        var checkList = DataDate.Where(o => o.SERVICE_TYPE_ID == item.SERVICE_TYPE_ID && o.FUEX_TYPE_ID == item.FUEX_TYPE_ID && o.DIIM_TYPE_ID == item.DIIM_TYPE_ID).ToList();
						foreach (var child in checkList)
						{
                            Mps000222ADO_CLS ado = new Mps000222ADO_CLS();
                            ado.SERVICE_TYPE_ID = item.SERVICE_TYPE_ID;
                            ado.SERVICE_TYPE_NAME = item.SERVICE_TYPE_NAME;
                            ado.DIIM_TYPE_ID = item.DIIM_TYPE_ID;
                            ado.FUEX_TYPE_ID = item.FUEX_TYPE_ID;
                            ado.INTRUCTION_TIME = child.INTRUCTION_TIME;
                            lstTemp.Add(ado);
                        }
                    }
                    ServiceCLS = lstTemp;
					foreach (var item in ServiceCLS)
					{
						var checkConClude = this.ServiceCLSFull.Where(o => o.SERVICE_TYPE_ID == item.SERVICE_TYPE_ID & o.FUEX_TYPE_ID == item.FUEX_TYPE_ID && o.DIIM_TYPE_ID == item.DIIM_TYPE_ID && o.INTRUCTION_TIME == item.INTRUCTION_TIME).ToList();
						if (checkConClude != null && checkConClude.Count > 0)
						{
							item.CONCLUDE = String.Join("; ", checkConClude.Select(o => o.CONCLUDE).ToList());
						}
					}
				}

                AddObjectKeyIntoListkey<V_HIS_TREATMENT>(rdo.HisTreatment);
                if (rdo.HisDhst != null)
                {
                    AddObjectKeyIntoListkey<HIS_DHST>(rdo.HisDhst, false);
                }
                if (rdo.VHisPatientTypeAlter != null)
                {
                    SetSingleKey(new KeyValue(Mps000222ExtendSingleKey.HEIN_ADDRESS, rdo.VHisPatientTypeAlter.ADDRESS));
                    AddObjectKeyIntoListkey<V_HIS_PATIENT_TYPE_ALTER>(rdo.VHisPatientTypeAlter, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetBarcode()
        {
            try
            {
                Inventec.Common.BarcodeLib.Barcode barcodeTreatment = new Inventec.Common.BarcodeLib.Barcode(rdo.HisTreatment.TREATMENT_CODE);
                barcodeTreatment.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcodeTreatment.IncludeLabel = false;
                barcodeTreatment.Width = 120;
                barcodeTreatment.Height = 40;
                barcodeTreatment.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                barcodeTreatment.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcodeTreatment.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcodeTreatment.IncludeLabel = true;

                dicImage.Add(Mps000222ExtendSingleKey.TREATMENT_CODE_BAR, barcodeTreatment);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
