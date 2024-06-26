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
using MOS.EFMODEL.DataModels;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPS.Processor.Mps000098.PDO;
using MPS.ProcessorBase;

namespace MPS.Processor.Mps000098
{
    class Mps000098Processor : AbstractProcessor
    {
        Mps000098PDO rdo;
        public Mps000098Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000098PDO)rdoBase;
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                ProcessGroupSereServ();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                ProcessSingleKey();
                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);

                objectTag.AddObjectData(store, "rdo.ServiceGroups", rdo.ServiceGroups);
                objectTag.AddObjectData(store, "Departments", rdo.DepartmentGroups);
                objectTag.AddObjectData(store, "Services", rdo.sereServInKTCs);

                objectTag.AddRelationship(store, "rdo.ServiceGroups", "Departments", "HEIN_SERVICE_TYPE_ID", "HEIN_SERVICE_TYPE_ID");
                objectTag.AddRelationship(store, "Departments", "Services", "REQUEST_DEPARTMENT_ID", "REQUEST_DEPARTMENT_ID");
                objectTag.AddRelationship(store, "rdo.ServiceGroups", "Services", "HEIN_SERVICE_TYPE_ID", "HEIN_SERVICE_TYPE_ID");

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        void ProcessSingleKey()
        {
            try
            {
                if (rdo.departmentTrans != null && rdo.departmentTrans.Count > 0)
                {
                    SetSingleKey(new KeyValue(Mps000098ExtendSingleKey.OPEN_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.departmentTrans[0].LOG_TIME)));
                    if (rdo.departmentTrans[rdo.departmentTrans.Count - 1] != null && rdo.departmentTrans.Count > 1)
                    {
                        SetSingleKey(new KeyValue(Mps000098ExtendSingleKey.CLOSE_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.departmentTrans[rdo.departmentTrans.Count - 1].LOG_TIME)));
                        SetSingleKey(new KeyValue(Mps000098ExtendSingleKey.TOTAL_DAY, rdo.totalDay));
                    }
                }

                if (rdo.departmentName != null)
                {
                    SetSingleKey(new KeyValue(Mps000098ExtendSingleKey.DEPARTMENT_NAME, rdo.departmentName));
                }


                if (rdo.patyAlterBhytADO != null)
                {
                    if (rdo.patyAlterBhytADO.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE)
                    {
                        if (rdo.patyAlterBhytADO.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.EMERGENCY)// la dung tuyen cap cuu
                        {
                            SetSingleKey(new KeyValue(Mps000098ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, "X"));
                            SetSingleKey(new KeyValue(Mps000098ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                            SetSingleKey(new KeyValue(Mps000098ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                        else if (rdo.patyAlterBhytADO.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.PRESENT)// la dung tuyen: gioi thieu,
                        {
                            SetSingleKey(new KeyValue(Mps000098ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            SetSingleKey(new KeyValue(Mps000098ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                            SetSingleKey(new KeyValue(Mps000098ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                        else
                        {
                            SetSingleKey(new KeyValue(Mps000098ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            SetSingleKey(new KeyValue(Mps000098ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                            SetSingleKey(new KeyValue(Mps000098ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                    }
                    else if (rdo.patyAlterBhytADO.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.FALSE)//trai tuyen
                    {
                        SetSingleKey(new KeyValue(Mps000098ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                        SetSingleKey(new KeyValue(Mps000098ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                        SetSingleKey(new KeyValue(Mps000098ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, "X"));
                    }

                    if (rdo.patyAlterBhytADO.IS_HEIN != null)
                        SetSingleKey(new KeyValue(Mps000098ExtendSingleKey.IS_HEIN, "X"));
                    else
                        SetSingleKey(new KeyValue(Mps000098ExtendSingleKey.IS_NOT_HEIN, "X"));
                }


                if (rdo.hisTranPati != null)
                {
                    SetSingleKey(new KeyValue(Mps000098ExtendSingleKey.TRAN_PATI_MEDI_ORG_CODE, rdo.hisTranPati.MEDI_ORG_CODE));
                    SetSingleKey(new KeyValue(Mps000098ExtendSingleKey.TRAN_PATI_MEDI_ORG_NAME, rdo.hisTranPati.MEDI_ORG_NAME));
                }

                decimal thanhtien_tong = 0;
                decimal bhytthanhtoan_tong = 0;
                decimal nguonkhac_tong = 0;
                decimal bnthanhtoan_tong = 0;

                if (rdo.sereServInKTCs != null && rdo.sereServInKTCs.Count > 0)
                {
                    thanhtien_tong = rdo.sereServInKTCs.Sum(o => o.VIR_TOTAL_PRICE_NO_ADD_PRICE) ?? 0;
                    bhytthanhtoan_tong = rdo.sereServInKTCs.Sum(o => o.VIR_TOTAL_HEIN_PRICE) ?? 0;
                    bnthanhtoan_tong = rdo.sereServInKTCs.Sum(o => o.VIR_TOTAL_PATIENT_PRICE) ?? 0;
                    nguonkhac_tong = 0;
                }

                SetSingleKey(new KeyValue(Mps000098ExtendSingleKey.TOTAL_PRICE, Inventec.Common.Number.Convert.NumberToStringRoundAuto(thanhtien_tong, 0)));
                SetSingleKey(new KeyValue(Mps000098ExtendSingleKey.TOTAL_PRICE_HEIN, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bhytthanhtoan_tong, 0)));
                SetSingleKey(new KeyValue(Mps000098ExtendSingleKey.TOTAL_PRICE_PATIENT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bnthanhtoan_tong, 0)));
                SetSingleKey(new KeyValue(Mps000098ExtendSingleKey.TOTAL_PRICE_OTHER, Inventec.Common.Number.Convert.NumberToStringRoundAuto(nguonkhac_tong, 0)));
                SetSingleKey(new KeyValue(Mps000098ExtendSingleKey.TOTAL_PRICE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(thanhtien_tong).ToString())));
                SetSingleKey(new KeyValue(Mps000098ExtendSingleKey.TOTAL_PRICE_HEIN_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bhytthanhtoan_tong).ToString())));
                SetSingleKey(new KeyValue(Mps000098ExtendSingleKey.TOTAL_PRICE_PATIENT_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bnthanhtoan_tong).ToString())));
                SetSingleKey(new KeyValue(Mps000098ExtendSingleKey.TOTAL_PRICE_OTHER_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(nguonkhac_tong).ToString())));

                if (rdo.treatmentFees != null)
                {
                    SetSingleKey(new KeyValue(Mps000098ExtendSingleKey.TOTAL_DEPOSIT_AMOUNT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(rdo.treatmentFees[0].TOTAL_DEPOSIT_AMOUNT ?? 0, 0)));
                }
                else
                {
                    SetSingleKey(new KeyValue(Mps000098ExtendSingleKey.TOTAL_DEPOSIT_AMOUNT, "0"));
                }

                //Tên dịch vụ kỹ thuật cao
                if (rdo.sereServKTC != null)
                {
                    SetSingleKey(new KeyValue(Mps000098ExtendSingleKey.SERVICE_HIGHTECH, rdo.sereServKTC.SERVICE_NAME));
                    SetSingleKey(new KeyValue(Mps000098ExtendSingleKey.PRICE_POLICY, Inventec.Common.Number.Convert.NumberToStringRoundAuto(rdo.sereServKTC.PRICE_POLICY, 0)));
                    SetSingleKey(new KeyValue(Mps000098ExtendSingleKey.PRICE_BHYT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(rdo.sereServKTC.PRICE_BHYT, 0)));
                }

                AddObjectKeyIntoListkey<PatyAlterBhytADO>(rdo.patyAlterBhytADO, false);
                AddObjectKeyIntoListkey<V_HIS_TREATMENT>(rdo.currentHisTreatment, false);
                AddObjectKeyIntoListkey<PatientADO>(rdo.patientADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void ProcessGroupSereServ()
        {
            try
            {
                rdo.sereServInKTCs = rdo.sereServInKTCs.Where(o => o.IS_NO_EXECUTE == null).ToList();

                rdo.ServiceGroups = new List<SereServGroupPlusADO>();
                rdo.DepartmentGroups = new List<SereServGroupPlusADO>();
                var sServceReportGroups = rdo.sereServInKTCs.GroupBy(o => o.HEIN_SERVICE_TYPE_ID).ToList();
                foreach (var sServceReportGroup in sServceReportGroups)
                {
                    List<SereServGroupPlusADO> subSServiceReportGroup = sServceReportGroup.ToList<SereServGroupPlusADO>();
                    SereServGroupPlusADO itemSServiceReportGroup = subSServiceReportGroup.First();
                    if (itemSServiceReportGroup.HEIN_SERVICE_TYPE_ID != null)
                        itemSServiceReportGroup.HEIN_SERVICE_TYPE_NAME = rdo.ServiceReports.Where(o => o.ID == subSServiceReportGroup.First().HEIN_SERVICE_TYPE_ID).SingleOrDefault().HEIN_SERVICE_TYPE_NAME;
                    else
                        itemSServiceReportGroup.HEIN_SERVICE_TYPE_NAME = "Khác";

                    itemSServiceReportGroup.TOTAL_PRICE_SERVICE_GROUP = subSServiceReportGroup.Sum(o => o.VIR_TOTAL_PRICE_NO_ADD_PRICE) ?? 0;
                    itemSServiceReportGroup.TOTAL_HEIN_PRICE_SERVICE_GROUP = subSServiceReportGroup.Sum(o => o.VIR_TOTAL_HEIN_PRICE) ?? 0;
                    itemSServiceReportGroup.TOTAL_PATIENT_PRICE_SERVICE_GROUP = subSServiceReportGroup.Sum(o => o.VIR_TOTAL_PATIENT_PRICE) ?? 0;
                    rdo.ServiceGroups.Add(itemSServiceReportGroup);


                    //Nhom Department
                    var sDepartmentGroups = subSServiceReportGroup.OrderBy(o => o.REQUEST_DEPARTMENT_ID).GroupBy(o => o.REQUEST_DEPARTMENT_ID).ToList();
                    foreach (var sDepartmentGroup in sDepartmentGroups)
                    {
                        List<SereServGroupPlusADO> subSDepartmentGroups = sDepartmentGroup.ToList<SereServGroupPlusADO>();
                        SereServGroupPlusADO itemSDepartmentGroup = subSDepartmentGroups.First();

                        itemSDepartmentGroup.TOTAL_PRICE_DEPARTMENT_GROUP = sDepartmentGroup.Where(o => o.REQUEST_DEPARTMENT_ID == itemSDepartmentGroup.REQUEST_DEPARTMENT_ID).Sum(o => o.VIR_TOTAL_PRICE) ?? 0;
                        itemSDepartmentGroup.TOTAL_PATIENT_PRICE_DEPARTMENT_GROUP = sDepartmentGroup.Where(o => o.REQUEST_DEPARTMENT_ID == itemSDepartmentGroup.REQUEST_DEPARTMENT_ID).Sum(o => o.VIR_TOTAL_PATIENT_PRICE) ?? 0;
                        itemSDepartmentGroup.TOTAL_HEIN_PRICE_DEPARTMENT_GROUP = sDepartmentGroup.Where(o => o.REQUEST_DEPARTMENT_ID == itemSDepartmentGroup.REQUEST_DEPARTMENT_ID).Sum(o => o.VIR_TOTAL_HEIN_PRICE) ?? 0;
                        rdo.DepartmentGroups.Add(itemSDepartmentGroup);

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
