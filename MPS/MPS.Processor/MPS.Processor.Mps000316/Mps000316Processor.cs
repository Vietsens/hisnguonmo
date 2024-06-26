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
using MPS.Processor.Mps000316.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HIS.Desktop.LocalStorage.BackendData;

namespace MPS.Processor.Mps000316
{
    public class Mps000316Processor : AbstractProcessor
    {
        Mps000316PDO rdo;

        List<ADO.Mps000316ADO> ListSereServ = new List<ADO.Mps000316ADO>();
        List<ADO.Mps000316ADO> ListSereServParent = new List<ADO.Mps000316ADO>();

        List<ADO.Mps000316ADO> ListSereServTest = new List<ADO.Mps000316ADO>();

        List<ADO.Mps000316ADO> ListSereServMediMate = new List<ADO.Mps000316ADO>();

        List<V_HIS_SERVICE_REQ> ListServiceReqDonK = new List<V_HIS_SERVICE_REQ>();

        List<ADO.VHisRoomADO> ListVHisRoomADO = new List<ADO.VHisRoomADO>();

        List<ADO.MedicineTypeDetailADO> ListMedicineTypeDetailADO = new List<ADO.MedicineTypeDetailADO>();

        //List<IGrouping<string, ADO.MedicineTypeDetailADO>> GListMedicineTypeDetailADO = new List<IGrouping<string, ADO.MedicineTypeDetailADO>>();
        List<ADO.MedicineTypeDetailADO> GListMedicineTypeDetailADO = new List<ADO.MedicineTypeDetailADO>();

        List<ADO.MedicineTypeDetailADO> MListMedicineTypeDetailADO = new List<ADO.MedicineTypeDetailADO>();

        List<long> ServiceTypePress = new List<long>() { IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC, IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT, IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU };

        NumberStyles style = NumberStyles.Any;

        public Mps000316Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000316PDO)rdoBase;
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

                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);

                objectTag.AddObjectData(store, "RoomHK", ListVHisRoomADO);

                objectTag.AddObjectData(store, "ServiceDonK", ListServiceReqDonK);
                objectTag.AddObjectData(store, "ServiceCLS", ListSereServParent);
                objectTag.AddObjectData(store, "ServiceCLSFull", ListSereServ);
                objectTag.AddObjectData(store, "ServiceCLSFullDetail", ListSereServ);
                objectTag.AddObjectData(store, "ServicePres", ListSereServMediMate);
                objectTag.AddObjectData(store, "ServiceTest", ListSereServTest);
                objectTag.AddRelationship(store, "ServiceCLS", "ServiceCLSFull", "TDL_SERVICE_TYPE_ID", "TDL_SERVICE_TYPE_ID");

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("GListMedicineTypeDetailADO:_______ ", GListMedicineTypeDetailADO));
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("ListMedicineTypeDetailADO:______ ", ListMedicineTypeDetailADO));
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("MListMedicineTypeDetailADO:______ ", MListMedicineTypeDetailADO));

                objectTag.AddObjectData(store, "MListMedicineTypeDetail", MListMedicineTypeDetailADO);
                objectTag.AddObjectData(store, "GMedicineTypeDetail", GListMedicineTypeDetailADO);
                objectTag.AddObjectData(store, "MedicineTypeDetail", ListMedicineTypeDetailADO);
                objectTag.AddRelationship(store, "GMedicineTypeDetail", "MedicineTypeDetail", "REQUEST_ROOM_NAME", "REQUEST_ROOM_NAME");
                //objectTag.AddRelationship(store, "GMedicineTypeDetail", "MedicineTypeDetail", "REQUEST_LOGINNAME", "REQUEST_LOGINNAME");
                objectTag.AddRelationship(store, "MListMedicineTypeDetail", "MedicineTypeDetail", "MEDI_STOCK_ID", "MEDI_STOCK_ID");
                objectTag.AddRelationship(store, "MListMedicineTypeDetail", "GMedicineTypeDetail", "REQUEST_ROOM_NAME", "REQUEST_ROOM_NAME");
                //objectTag.AddRelationship(store, "MListMedicineTypeDetail", "GMedicineTypeDetail", "REQUEST_LOGINNAME", "REQUEST_LOGINNAME");
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
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

        //chế biến từ danh sách his_sere_serv
        private void SetService()
        {
            try
            {
                if (rdo.HisSereServs != null && rdo.HisSereServs.Count > 0)
                {
                    var groupSereServType = rdo.HisSereServs.GroupBy(g => g.TDL_SERVICE_TYPE_ID).ToList();
                    foreach (var ssServiceType in groupSereServType)
                    {
                        if (ssServiceType.First().TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN)
                        {
                            ProcessListSereServXN(ssServiceType.ToList());
                        }
                        else if (ServiceTypePress.Contains(ssServiceType.First().TDL_SERVICE_TYPE_ID))
                        {
                            ProcessListPress(ssServiceType.ToList());
                        }
                        else
                        {
                            ProcessListTotal(ssServiceType.ToList());
                        }
                        Inventec.Common.Logging.LogSystem.Debug("ssServiceType.ToList() count" + ssServiceType.ToList().Count);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessListSereServXN(List<HIS_SERE_SERV> listSereServ)
        {
            try
            {
                if (listSereServ != null && listSereServ.Count > 0)
                {
                    var dicSereServTeins = new Dictionary<long, List<V_HIS_SERE_SERV_TEIN>>();

                    if (rdo.VHisSereServTeins != null && rdo.VHisSereServTeins.Count > 0)
                    {
                        //sắp xếp lại danh sách thông số
                        var orderTeins = rdo.VHisSereServTeins.OrderByDescending(o => o.NUM_ORDER).ToList();
                        foreach (var item in orderTeins)
                        {
                            if (!dicSereServTeins.ContainsKey(item.SERE_SERV_ID))
                                dicSereServTeins[item.SERE_SERV_ID] = new List<V_HIS_SERE_SERV_TEIN>();

                            dicSereServTeins[item.SERE_SERV_ID].Add(item);
                        }
                    }

                    List<ADO.Mps000316ADO> ListSereServAdo = new List<ADO.Mps000316ADO>();
                    foreach (var item in listSereServ)
                    {
                        ListSereServAdo.Add(new ADO.Mps000316ADO(item, rdo.VHisServices));
                    }

                    //điền dịch vụ theo thứ tự
                    ListSereServAdo = ListSereServAdo.OrderByDescending(p => p.SERVICE_NUM_ODER).ThenBy(p => p.TDL_SERVICE_NAME).ToList();
                    //int dem = 1;
                    foreach (var item in ListSereServAdo)
                    {
                        if (dicSereServTeins.ContainsKey(item.ID))
                        {
                            var testSS = dicSereServTeins[item.ID];
                            if (testSS != null && testSS.Count == 1 && testSS[0].IS_NOT_SHOW_SERVICE == 1)
                            {
                                //item.TDL_SERVICE_NAME = dem + ". " + item.TDL_SERVICE_NAME;
                                item.TEST_INDEX_RANGE = "";
                                item.TEST_INDEX_UNIT_NAME = testSS.First().TEST_INDEX_UNIT_NAME;
                                item.VALUE_STR = testSS.First().VALUE;
                                item.RESULT_CODE = testSS.First().RESULT_CODE;
                                item.IMPORTANT = testSS.First().IS_IMPORTANT == 1 ? "X" : "";

                                V_HIS_SERVICE_REQ serviceReq = rdo.VHisServiceReqTests != null ? rdo.VHisServiceReqTests.FirstOrDefault(o => o.ID == item.SERVICE_REQ_ID) : null;
                                if (serviceReq != null)
                                {
                                    item.FINISH_TIME_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(serviceReq.FINISH_TIME ?? 0);
                                    item.START_TIME_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(serviceReq.START_TIME ?? 0);

                                }

                                V_HIS_TEST_INDEX_RANGE testIndexRange = GetTestIndexRange(rdo.VHisTreatment.TDL_PATIENT_DOB, rdo.VHisTreatment.TDL_PATIENT_GENDER_ID, testSS.First().TEST_INDEX_ID, rdo.VHistTestIndexRanges);
                                if (testIndexRange != null)
                                {
                                    ProcessMaxMixValue(item, testIndexRange);
                                }

                                ListSereServTest.Add(item);
                            }
                            else if (testSS != null && (testSS.Count > 1 || (testSS.Count == 1 && testSS[0].IS_NOT_SHOW_SERVICE != 1)))
                            {
                                //item.TDL_SERVICE_NAME = dem + ". " + item.TDL_SERVICE_NAME;
                                item.TEST_INDEX_RANGE = "";
                                item.TEST_INDEX_UNIT_NAME = "";
                                item.VALUE_STR = "";

                                V_HIS_SERVICE_REQ serviceReq = rdo.VHisServiceReqTests != null ? rdo.VHisServiceReqTests.FirstOrDefault(o => o.ID == item.SERVICE_REQ_ID) : null;
                                if (serviceReq != null)
                                {
                                    item.FINISH_TIME_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(serviceReq.FINISH_TIME ?? 0);
                                    item.START_TIME_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(serviceReq.START_TIME ?? 0);
                                }

                                ListSereServTest.Add(item);

                                foreach (var sereServTein in testSS)
                                {
                                    if (sereServTein.SERE_SERV_ID == item.ID)
                                    {
                                        ADO.Mps000316ADO exeSereServTein = new ADO.Mps000316ADO();

                                        exeSereServTein.TDL_SERVICE_NAME = "    " + sereServTein.TEST_INDEX_NAME;
                                        exeSereServTein.TEST_INDEX_UNIT_NAME = sereServTein.TEST_INDEX_UNIT_NAME;
                                        exeSereServTein.VALUE_STR = sereServTein.VALUE;
                                        exeSereServTein.RESULT_CODE = sereServTein.RESULT_CODE;
                                        exeSereServTein.IMPORTANT = sereServTein.IS_IMPORTANT == 1 ? "X" : "";

                                        V_HIS_TEST_INDEX_RANGE testIndexRange = GetTestIndexRange(rdo.VHisTreatment.TDL_PATIENT_DOB, rdo.VHisTreatment.TDL_PATIENT_GENDER_ID, sereServTein.TEST_INDEX_ID, rdo.VHistTestIndexRanges);
                                        if (testIndexRange != null)
                                        {
                                            ProcessMaxMixValue(exeSereServTein, testIndexRange);
                                        }

                                        ListSereServTest.Add(exeSereServTein);
                                    }
                                }
                            }
                            //dem++;
                        }
                    }
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
                    long age = Inventec.Common.DateTime.Calculation.Age(dob);

                    var query = testIndexRanges.Where(o => o.TEST_INDEX_ID == testIndexId
                            && ((o.AGE_FROM.HasValue && o.AGE_FROM.Value <= age) || !o.AGE_FROM.HasValue)
                            && ((o.AGE_TO.HasValue && o.AGE_TO.Value >= age) || !o.AGE_TO.HasValue));
                    if (genderId == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__MALE)
                    {
                        query = query.Where(o => o.IS_MALE == 1);
                    }
                    else if (genderId == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__FEMALE)
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

        private void ProcessMaxMixValue(ADO.Mps000316ADO ti, V_HIS_TEST_INDEX_RANGE testIndexRange)
        {
            try
            {
                Decimal minValue = 0, maxValue = 0, value = 0;
                if (ti != null && testIndexRange != null)
                {
                    Decimal value1 = 0;
                    Decimal.TryParse((ti.VALUE_STR ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out value1);
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

                    if (!String.IsNullOrWhiteSpace(ti.VALUE_STR))
                    {
                        Decimal.TryParse((ti.VALUE_STR ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out value);
                    }

                    if (!String.IsNullOrEmpty(testIndexRange.NORMAL_VALUE))
                    {
                        ti.VALUE_RANGE = testIndexRange.NORMAL_VALUE;
                        if (!String.IsNullOrWhiteSpace(ti.VALUE_RANGE) && ti.VALUE_STR != null && ti.VALUE_STR.ToString().ToUpper() == ti.VALUE_RANGE.ToUpper())
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

                            if (ti.VALUE_STR != null && ti.MIN_VALUE != null && ti.MIN_VALUE <= value1 && ti.MAX_VALUE != null && value1 < ti.MAX_VALUE)
                            {
                                ti.HIGH_OR_LOW = "";
                            }
                            else if (ti.VALUE_STR != null && ti.MIN_VALUE != null && value1 < ti.MIN_VALUE)
                            {
                                ti.HIGH_OR_LOW = "L";
                            }
                            else if (ti.VALUE_STR != null && ti.MAX_VALUE != null && ti.MAX_VALUE <= value1)
                            {
                                ti.HIGH_OR_LOW = "H";
                            }
                            else
                            {
                                ti.HIGH_OR_LOW = "";
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

                            if (ti.VALUE_STR != null && ti.MIN_VALUE != null && ti.MIN_VALUE <= value1 && ti.MAX_VALUE != null && value1 <= ti.MAX_VALUE)
                            {
                                ti.HIGH_OR_LOW = "";
                            }
                            else if (ti.VALUE_STR != null && ti.MIN_VALUE != null && value1 < ti.MIN_VALUE)
                            {
                                ti.HIGH_OR_LOW = "L";
                            }
                            else if (ti.VALUE_STR != null && ti.MAX_VALUE != null && ti.MAX_VALUE < value1)
                            {
                                ti.HIGH_OR_LOW = "H";
                            }
                            else
                            {
                                ti.HIGH_OR_LOW = "";
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

                            if (ti.VALUE_STR != null && ti.MIN_VALUE != null && ti.MIN_VALUE < value1 && ti.MAX_VALUE != null && value1 <= ti.MAX_VALUE)
                            {
                                ti.HIGH_OR_LOW = "";
                            }
                            else if (ti.VALUE_STR != null && ti.MIN_VALUE != null && value1 < ti.MIN_VALUE)
                            {
                                ti.HIGH_OR_LOW = "L";
                            }
                            else if (ti.VALUE_STR != null && ti.MAX_VALUE != null && ti.MAX_VALUE < value1)
                            {
                                ti.HIGH_OR_LOW = "H";
                            }
                            else
                            {
                                ti.HIGH_OR_LOW = "";
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

                            if (ti.VALUE_STR != null && ti.MIN_VALUE != null && ti.MIN_VALUE < value1 && ti.MAX_VALUE != null && value1 < ti.MAX_VALUE)
                            {
                                ti.HIGH_OR_LOW = "";
                            }
                            else if (ti.VALUE_STR != null && ti.MIN_VALUE != null && value1 <= ti.MIN_VALUE)
                            {
                                ti.HIGH_OR_LOW = "L";
                            }
                            else if (ti.VALUE_STR != null && ti.MAX_VALUE != null && ti.MAX_VALUE <= value1)
                            {
                                ti.HIGH_OR_LOW = "H";
                            }
                            else
                            {
                                ti.HIGH_OR_LOW = "";
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

        private void ProcessListTotal(List<HIS_SERE_SERV> ssServiceType)
        {
            try
            {
                if (ssServiceType != null && ssServiceType.Count > 0)
                {
                    ADO.Mps000316ADO parent = new ADO.Mps000316ADO(ssServiceType.First(), rdo.VHisServices);
                    ListSereServParent.Add(parent);

                    foreach (var item in ssServiceType)
                    {
                        ADO.Mps000316ADO ado = new ADO.Mps000316ADO(item, rdo.VHisServices);
                        if (rdo.HisSereServsExts != null && rdo.HisSereServsExts.Count > 0)
                        {
                            var ext = rdo.HisSereServsExts.FirstOrDefault(o => o.SERE_SERV_ID == item.ID);
                            if (ext != null)
                            {
                                ado.CONCLUDE = ext.CONCLUDE;
                                ado.DESCRIPTION = ext.DESCRIPTION;
                                ado.NOTE = ext.NOTE;
                                ado.NUMBER_OF_FILM = ext.NUMBER_OF_FILM;
                                ado.BEGIN_TIME = ext.BEGIN_TIME;
                                ado.END_TIME = ext.END_TIME;
                                ado.BEGIN_TIME_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(ext.BEGIN_TIME ?? 0);
                                ado.END_TIME_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(ext.END_TIME ?? 0);
                            }

                            ListSereServ.Add(ado);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessListPress(List<HIS_SERE_SERV> ssServiceType)
        {
            try
            {
                if (ssServiceType != null && ssServiceType.Count > 0)
                {
                    foreach (var item in ssServiceType)
                    {
                        ListSereServMediMate.Add(new ADO.Mps000316ADO(item, rdo.VHisServices, rdo.ListMedicine, rdo.ListMaterial));
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
                if (rdo.VHisPatient != null) AddObjectKeyIntoListkey<V_HIS_PATIENT>(rdo.VHisPatient, false);

                if (rdo.VHisTreatment != null) AddObjectKeyIntoListkey<V_HIS_TREATMENT>(rdo.VHisTreatment, false);

                if (rdo.VHisServiceReqExam != null)
                {
                    AddObjectKeyIntoListkey<V_HIS_SERVICE_REQ>(rdo.VHisServiceReqExam, false);
                    SetSingleKey(new KeyValue(Mps000316ExtendSingleKey.PROVISIONAL_DIAGNOSIS, rdo.VHisServiceReqExam.PROVISIONAL_DIAGNOSIS));
                    SetSingleKey(new KeyValue(Mps000316ExtendSingleKey.ADVISE_STR, rdo.VHisServiceReqExam.ADVISE));
                }
            
                if (rdo.HisDhst != null) AddObjectKeyIntoListkey<HIS_DHST>(rdo.HisDhst, false);

                if (rdo.VHisPatientTypeAlter != null)
                {
                    SetSingleKey(new KeyValue(Mps000316ExtendSingleKey.HEIN_ADDRESS, rdo.VHisPatientTypeAlter.ADDRESS));
                    AddObjectKeyIntoListkey<V_HIS_PATIENT_TYPE_ALTER>(rdo.VHisPatientTypeAlter, false);
                }

                if (rdo.VHisServiceReqDonK != null && rdo.VHisServiceReqDonK.Count > 0)
                {
                    var GrServiceReqDonK = rdo.VHisServiceReqDonK.GroupBy(o => o.EXECUTE_ROOM_ID);

                    foreach (var itemG in GrServiceReqDonK)
                    {
                        V_HIS_SERVICE_REQ Servicereq = new V_HIS_SERVICE_REQ();
                        Servicereq.EXECUTE_ROOM_ID = itemG.FirstOrDefault().EXECUTE_ROOM_ID;
                        Servicereq.EXECUTE_ROOM_CODE = itemG.FirstOrDefault().EXECUTE_ROOM_CODE;
                        Servicereq.EXECUTE_ROOM_NAME = itemG.FirstOrDefault().EXECUTE_ROOM_NAME;

                        var lstAdvise = itemG.Where(o => !String.IsNullOrEmpty(o.ADVISE)).Select(p => p.ADVISE).ToList();
                        if (lstAdvise != null && lstAdvise.Count > 0)
                        {
                            Servicereq.ADVISE = String.Join(",", lstAdvise);
                        }
                        ListServiceReqDonK.Add(Servicereq);
                    }
                }

                if (rdo.VHisRooms != null && rdo.VHisRooms.Count > 0)
                {
                    foreach (var item in rdo.VHisRooms)
                    {
                        ADO.VHisRoomADO ado = new ADO.VHisRoomADO(item);

                        if (rdo.VHisTreatment != null)
                        {
                            ado.APPOINTMENT_TIME = rdo.VHisTreatment.APPOINTMENT_TIME;

                            ado.APPOINTMENT_TIME_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.VHisTreatment.APPOINTMENT_TIME ?? 0);
                        }

                        ListVHisRoomADO.Add(ado);
                    }
                }

                if (rdo.VHisServiceReqHk != null && rdo.VHisServiceReqHk.Count > 0)
                {
                    foreach (var item in rdo.VHisServiceReqHk)
                    {
                        ADO.VHisRoomADO ado = new ADO.VHisRoomADO();

                        ado.APPOINTMENT_TIME = item.APPOINTMENT_TIME;

                        ado.APPOINTMENT_TIME_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(item.APPOINTMENT_TIME ?? 0);
                        ado.ID = item.EXECUTE_ROOM_ID;
                        ado.ROOM_CODE = item.EXECUTE_ROOM_CODE;
                        ado.ROOM_NAME = item.EXECUTE_ROOM_NAME;

                        ListVHisRoomADO.Add(ado);
                    }
                }
                if (rdo.prescriptionADO != null)
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("rdo.prescriptionADO:_______ ", rdo.prescriptionADO));
                    ; var rooms = BackendDataWorker.Get<V_HIS_ROOM>();
                    if (rdo.prescriptionADO.VHisExpMestMedicine != null && rdo.prescriptionADO.VHisExpMestMedicine.Count > 0)
                    {
                        foreach (var item in rdo.prescriptionADO.VHisExpMestMedicine)
                        {
                            ADO.MedicineTypeDetailADO medicineDetailAdo = new ADO.MedicineTypeDetailADO(rdo.prescriptionADO.HisServiceReq, rooms, item, null, null, null);
                            ListMedicineTypeDetailADO.Add(medicineDetailAdo);
                        }
                    }

                    if (rdo.prescriptionADO.VHisExpMestMaterial != null && rdo.prescriptionADO.VHisExpMestMaterial.Count > 0)
                    {
                        foreach (var item in rdo.prescriptionADO.VHisExpMestMaterial)
                        {
                            ADO.MedicineTypeDetailADO medicineDetailAdo = new ADO.MedicineTypeDetailADO(rdo.prescriptionADO.HisServiceReq, rooms, null, item, null, null);
                            ListMedicineTypeDetailADO.Add(medicineDetailAdo);
                        }
                    }

                    if (rdo.prescriptionADO.VHisServiceReqMety != null && rdo.prescriptionADO.VHisServiceReqMety.Count > 0)
                    {
                        foreach (var item in rdo.prescriptionADO.VHisServiceReqMety)
                        {
                            ADO.MedicineTypeDetailADO medicineDetailAdo = new ADO.MedicineTypeDetailADO(rdo.prescriptionADO.HisServiceReq, rooms, null, null, item, null);
                            ListMedicineTypeDetailADO.Add(medicineDetailAdo);
                        }
                    }

                    if (rdo.prescriptionADO.VHisServiceReqMaty != null && rdo.prescriptionADO.VHisServiceReqMaty.Count > 0)
                    {
                        foreach (var item in rdo.prescriptionADO.VHisServiceReqMaty)
                        {
                            ADO.MedicineTypeDetailADO medicineDetailAdo = new ADO.MedicineTypeDetailADO(rdo.prescriptionADO.HisServiceReq, rooms, null, null, null, item);
                            ListMedicineTypeDetailADO.Add(medicineDetailAdo);
                        }
                    }
                    ListMedicineTypeDetailADO = ListMedicineTypeDetailADO.OrderBy(o => o.INTRUCTION_TIME).ThenBy(p => p.NUM_ORDER).ToList();
                    //#GListMedicineTypeDetailADO = ListMedicineTypeDetailADO.GroupBy(o => o.EXPMEST_REQUEST_ROOM_NAME).ToList();
                    GListMedicineTypeDetailADO = ListMedicineTypeDetailADO.GroupBy(o => new { o.REQUEST_ROOM_NAME }).Select(o => o.First()).OrderByDescending(k => k.REQUEST_ROOM_NAME).ToList();

                    MListMedicineTypeDetailADO = ListMedicineTypeDetailADO.GroupBy(o => new { o.REQUEST_ROOM_NAME, o.MEDI_STOCK_ID }).Select(o => o.First()).OrderByDescending(k => k.MEDI_STOCK_ID).ToList();
                    //int count = 0;
                    long currentMediStockId = -1;
                    for (int i = 0; i < MListMedicineTypeDetailADO.Count; i++)
                    {
                        if (currentMediStockId == MListMedicineTypeDetailADO[i].MEDI_STOCK_ID)
                            continue;
                        if (i < MListMedicineTypeDetailADO.Count - 1)
                        {
                            if (((MListMedicineTypeDetailADO[i].MEDI_STOCK_ID != MListMedicineTypeDetailADO[i + 1].MEDI_STOCK_ID || MListMedicineTypeDetailADO[i].MEDI_STOCK_ID == MListMedicineTypeDetailADO[i + 1].MEDI_STOCK_ID) && MListMedicineTypeDetailADO[i].MEDI_STOCK_ID != 0)
                                || MListMedicineTypeDetailADO[i].MEDI_STOCK_ID == 0)
                            {
                                MListMedicineTypeDetailADO[i].CHECK_MEDI_STOCK = 1;
                                currentMediStockId = MListMedicineTypeDetailADO[i].MEDI_STOCK_ID;
                            }
                        }
                        else if (i == MListMedicineTypeDetailADO.Count - 1)
                        {
                            if (MListMedicineTypeDetailADO.Count > 1)
                            {
                                if (MListMedicineTypeDetailADO[i].MEDI_STOCK_ID != MListMedicineTypeDetailADO[i - 1].MEDI_STOCK_ID)
                                {
                                    MListMedicineTypeDetailADO[i].CHECK_MEDI_STOCK = 1;
                                    currentMediStockId = MListMedicineTypeDetailADO[i].MEDI_STOCK_ID;
                                }
                            }
                            else
                            {
                                MListMedicineTypeDetailADO[i].CHECK_MEDI_STOCK = 1;
                                currentMediStockId = MListMedicineTypeDetailADO[i].MEDI_STOCK_ID;
                            }
                        }
                    }



                    //foreach (var item in MListMedicineTypeDetailADO)
                    //{
                    //    if (item.MEDI_STOCK_ID == 0)
                    //    {
                    //        if (count > 0)
                    //        {
                    //            item.CHECK_MEDI_STOCK = 1;
                    //        }                      
                    //        count++;
                    //    }
                    //}
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
                if (rdo.VHisTreatment != null)
                {
                    Inventec.Common.BarcodeLib.Barcode barcodeTreatment = new Inventec.Common.BarcodeLib.Barcode(rdo.VHisTreatment.TREATMENT_CODE);
                    barcodeTreatment.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                    barcodeTreatment.IncludeLabel = false;
                    barcodeTreatment.Width = 120;
                    barcodeTreatment.Height = 40;
                    barcodeTreatment.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                    barcodeTreatment.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                    barcodeTreatment.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                    barcodeTreatment.IncludeLabel = true;

                    dicImage.Add(Mps000316ExtendSingleKey.TREATMENT_CODE_BAR, barcodeTreatment);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
