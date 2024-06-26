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
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPS.ProcessorBase.Core;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000193.PDO;
using FlexCel.Report;
using MPS.ProcessorBase;

namespace MPS.Processor.Mps000193
{
    public partial class Mps000193Processor : AbstractProcessor
    {
        Mps000193PDO rdo;
        public Mps000193Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000193PDO)rdoBase;
        }

        internal void SetBarcodeKey()
        {
            try
            {

                Inventec.Common.BarcodeLib.Barcode barcodeTreatment = new Inventec.Common.BarcodeLib.Barcode(rdo.treatment.TREATMENT_CODE);
                barcodeTreatment.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcodeTreatment.IncludeLabel = false;
                barcodeTreatment.Width = 120;
                barcodeTreatment.Height = 40;
                barcodeTreatment.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                barcodeTreatment.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcodeTreatment.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcodeTreatment.IncludeLabel = true;

                dicImage.Add(Mps000193ExtendSingleKey.TREATMENT_CODE_BAR, barcodeTreatment);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                DataInputProcess();
                ProcessGroupSereServ();
                ProcesTotalPrice();
                ProcessSingleKey();
                SetBarcodeKey();

                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);
                objectTag.AddObjectData(store, "SereServHighTechs", rdo.sereServKTCADOs);
                objectTag.AddObjectData(store, "Services", rdo.sereServADOs);
                objectTag.AddObjectData(store, "HeinServiceTypes", rdo.heinServiceTypes);
                objectTag.AddObjectData(store, "KTCFeeGroups", rdo.ktcFeeGroups);
                objectTag.AddObjectData(store, "ExecuteGroups", rdo.sereServExecuteGroups);

                objectTag.AddRelationship(store, "KTCFeeGroups", "ExecuteGroups", "IS_OUT_PARENT_FEE", "IS_OUT_PARENT_FEE");
                objectTag.AddRelationship(store, "ExecuteGroups", "HeinServiceTypes", "TDL_EXECUTE_GROUP_ID", "TDL_EXECUTE_GROUP_ID");
                objectTag.AddRelationship(store, "KTCFeeGroups", "HeinServiceTypes", "IS_OUT_PARENT_FEE", "IS_OUT_PARENT_FEE");
                objectTag.AddRelationship(store, "HeinServiceTypes", "Services", "TDL_HEIN_SERVICE_TYPE_ID", "TDL_HEIN_SERVICE_TYPE_ID");
                objectTag.AddRelationship(store, "ExecuteGroups", "Services", "TDL_EXECUTE_GROUP_ID", "TDL_EXECUTE_GROUP_ID");
                objectTag.AddRelationship(store, "KTCFeeGroups", "Services", "IS_OUT_PARENT_FEE", "IS_OUT_PARENT_FEE");

                objectTag.SetUserFunction(store, "ReplaceValue", new ReplaceValueFunction());

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        class ReplaceValueFunction : FlexCel.Report.TFlexCelUserFunction
        {
            public override object Evaluate(object[] parameters)
            {
                if (parameters == null || parameters.Length <= 0)
                    throw new ArgumentException("Bad parameter count in call to Orders() user-defined function");

                try
                {
                    string value = parameters[0] + "";
                    if (!String.IsNullOrEmpty(value))
                    {
                        value = value.Replace(';', '/');
                    }
                    return value;
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                    return parameters[0];
                }
            }
        }

        internal void DataInputProcess()
        {
            try
            {
                //Patient ADO
                rdo.patientADO = DataRawProcess.PatientRawToADO(rdo.patient);
                //BHYT ADO
                rdo.patyAlterBHYTADO = DataRawProcess.PatyAlterBHYTRawToADO(rdo.patyAlter);

                rdo.sereServADOs = new List<SereServADO>();
                rdo.sereServKTCADOs = new List<SereServADO>();

                //SereServKTCADO
                var sereServKTCADOTemps = new List<SereServADO>();
                sereServKTCADOTemps.AddRange(from r in rdo.sereServKTCs
                                             select new SereServADO(r, rdo.Services, rdo.HeinServiceTypes, rdo.Rooms));
                var sereServKTCGroups = sereServKTCADOTemps
                    .Where(o => o.IS_NO_EXECUTE != 1
                    && (o.VIR_PRICE_NO_EXPEND > 0 || o.PRICE_POLICY > 0)
                    && o.AMOUNT > 0)
                    .OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999)
                    .GroupBy(o => new
                    {
                        o.SERVICE_ID,
                        o.TOTAL_HEIN_PRICE_ONE_AMOUNT,
                        o.TDL_REQUEST_DEPARTMENT_ID,
                        o.IS_OUT_PARENT_FEE,
                        o.PATIENT_TYPE_ID,
                        o.PACKAGE_ID,
                        o.IS_EXPEND,
                        o.PARENT_ID
                    });

                foreach (var sereServKTCGroup in sereServKTCGroups)
                {
                    SereServADO sereServ = sereServKTCGroup.FirstOrDefault();
                    sereServ.AMOUNT = sereServKTCGroup.Sum(o => o.AMOUNT);
                    sereServ.VIR_TOTAL_PRICE_NO_EXPEND = sereServKTCGroup.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND);
                    sereServ.VIR_TOTAL_HEIN_PRICE = sereServKTCGroup.Sum(o => o.VIR_TOTAL_HEIN_PRICE);
                    rdo.sereServKTCADOs.Add(sereServ);
                }

                //SereServADO
                var sereServADOTemps = new List<SereServADO>();
                sereServADOTemps.AddRange(from r in rdo.sereServs
                                          select new SereServADO(r, rdo.Services
                , rdo.patyAlter, rdo.HeinServiceTypes, rdo.Rooms));

                //sereServkhong phai la dịch vu ky thuat cao
                var sereServGroups = sereServADOTemps
                    .Where(o =>
                        (rdo.sereServKTCs != null ? !rdo.sereServKTCs.Select(p => p.ID).Contains(o.ID) : true)
                        && o.AMOUNT > 0
                        && o.PRICE_BHYT > 0
                        && o.IS_NO_EXECUTE == null)
                    .OrderBy(o => o.IS_OUT_PARENT_FEE ?? 99999)
                    .ThenBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999)
                    .ThenBy(o => o.TDL_SERVICE_NAME)
                    .GroupBy(o => new
                    {
                        o.PRICE_BHYT,
                        o.SERVICE_ID,
                        o.VIR_PRICE_NO_EXPEND,
                        o.TDL_REQUEST_DEPARTMENT_ID,
                        o.IS_OUT_PARENT_FEE,
                        o.IS_EXPEND,
                        o.PARENT_ID
                    });

                foreach (var sereServGroup in sereServGroups)
                {
                    SereServADO sereServ = sereServGroup.FirstOrDefault();
                    if (sereServ.PARENT_ID == null && (sereServ.IS_OUT_PARENT_FEE != 1 || sereServ.IS_OUT_PARENT_FEE == null)
                        || (rdo.sereServKTCADOs != null && rdo.sereServKTCADOs.Where(o => o.ID == sereServ.PARENT_ID).Count() == 0))
                    {
                        sereServ.IS_OUT_PARENT_FEE = 1;
                    }

                    sereServ.AMOUNT = sereServGroup.Sum(o => o.AMOUNT);
                    sereServ.TOTAL_PRICE_BHYT = sereServGroup.Sum(o => o.TOTAL_PRICE_BHYT);
                    sereServ.VIR_TOTAL_HEIN_PRICE = sereServGroup.Sum(o => o.VIR_TOTAL_HEIN_PRICE);
                    sereServ.VIR_TOTAL_PATIENT_PRICE_BHYT = sereServGroup.Sum(o => o.VIR_TOTAL_PATIENT_PRICE_BHYT);
                    rdo.sereServADOs.Add(sereServ);
                }
                rdo.sereServADOs = rdo.sereServADOs.OrderBy(o => o.TDL_SERVICE_NAME).ToList();

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
                rdo.ktcFeeGroups = new List<SereServADO>();
                rdo.sereServExecuteGroups = new List<SereServADO>();
                rdo.heinServiceTypes = new List<SereServADO>();

                //Nhom chi phi trong, ngoai goi theo dich vu KTC
                var sereServKTCGroups = rdo.sereServADOs.GroupBy(o => o.IS_OUT_PARENT_FEE).ToList();
                foreach (var sereServKTCGroup in sereServKTCGroups)
                {
                    List<SereServADO> sereServADOTemps = sereServKTCGroup.ToList<SereServADO>();
                    SereServADO ktcFeeGroup = sereServKTCGroup.First();
                    if (ktcFeeGroup.IS_OUT_PARENT_FEE != 1 || ktcFeeGroup.IS_OUT_PARENT_FEE == null)
                    {
                        ktcFeeGroup.KTC_FEE_GROUP_NAME = "CHI PHÍ TRONG GÓI PHẪU THUẬT";
                    }
                    else
                    {
                        ktcFeeGroup.KTC_FEE_GROUP_NAME = "CHI PHÍ NGOÀI GÓI PHẪU THUẬT";
                    }

                    rdo.ktcFeeGroups.Add(ktcFeeGroup);

                    //Nhom Execute
                    var sereServExecuteGroupTemps = sereServADOTemps.GroupBy(o => o.TDL_EXECUTE_GROUP_ID).ToList();
                    foreach (var sereServExecuteGroupTemp in sereServExecuteGroupTemps)
                    {
                        SereServADO executeGroup = sereServExecuteGroupTemp.First();
                        if (executeGroup.TDL_EXECUTE_GROUP_ID == null)
                            executeGroup.EXECUTE_GROUP_NAME = "    ";
                        else
                            executeGroup.EXECUTE_GROUP_NAME = rdo.executeGroups.FirstOrDefault(o => o.ID == executeGroup.TDL_EXECUTE_GROUP_ID).EXECUTE_GROUP_NAME;
                        rdo.sereServExecuteGroups.Add(executeGroup);

                        //Nhom HeinServiceType
                        var heinServiceTypeGroups = sereServExecuteGroupTemp.OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999999999)
                            .GroupBy(o => o.TDL_HEIN_SERVICE_TYPE_ID).ToList();
                        foreach (var heinServiceTypeGroup in heinServiceTypeGroups)
                        {
                            SereServADO heinServiceType = heinServiceTypeGroup.First();
                            heinServiceType.HEIN_SERVICE_TYPE_NAME = heinServiceType.TDL_HEIN_SERVICE_TYPE_ID.HasValue ? heinServiceType.HEIN_SERVICE_TYPE_NAME : "Khác";

                            heinServiceType.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE = heinServiceTypeGroup
                                .Sum(o => o.VIR_TOTAL_HEIN_PRICE);
                            heinServiceType.TOTAL_PRICE_BHYT_HEIN_SERVICE_TYPE = heinServiceTypeGroup
                                             .Sum(o => o.TOTAL_PRICE_BHYT);
                            heinServiceType.TOTAL_PATIENT_PRICE_BHYT_HEIN_SERVICE_TYPE = heinServiceTypeGroup.Sum(o => o.VIR_TOTAL_PATIENT_PRICE_BHYT);

                            rdo.heinServiceTypes.Add(heinServiceType);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void ProcesTotalPrice()
        {
            try
            {
                #region TONG TIEN THEO GOI
                foreach (var ktcFee in rdo.ktcFeeGroups)
                {
                    List<SereServADO> sereServKTCTemps = rdo.sereServADOs.Where(o => o.IS_OUT_PARENT_FEE == ktcFee.IS_OUT_PARENT_FEE).ToList();

                    ktcFee.TOTAL_HEIN_PRICE_FEE_GROUP = sereServKTCTemps.Sum(o => o.VIR_TOTAL_HEIN_PRICE);
                    ktcFee.TOTAL_PRICE_BHYT_FEE_GROUP = sereServKTCTemps.Sum(o => o.TOTAL_PRICE_BHYT);
                    ktcFee.TOTAL_PATIENT_PRICE_BHYT_FEE_GROUP = sereServKTCTemps.Sum(o => o.VIR_TOTAL_PATIENT_PRICE_BHYT);


                    if (sereServKTCTemps != null && sereServKTCTemps.Count > 0)
                    {
                        foreach (var execute in rdo.sereServExecuteGroups)
                        {
                            List<SereServADO> sereServExecuteTemps = sereServKTCTemps.Where(o => o.TDL_EXECUTE_GROUP_ID == execute.TDL_EXECUTE_GROUP_ID).ToList();
                            if (sereServExecuteTemps != null && sereServExecuteTemps.Count > 0)
                            {
                                execute.TOTAL_HEIN_PRICE_EXECUTE_GROUP = sereServExecuteTemps.Sum(o => o.VIR_TOTAL_HEIN_PRICE);
                                execute.TOTAL_PRICE_BHYT_EXECUTE_GROUP = sereServKTCTemps.Sum(o => o.TOTAL_PRICE_BHYT);
                                execute.TOTAL_PATIENT_PRICE_BHYT_EXECUTE_GROUP = sereServKTCTemps.Sum(o => o.VIR_TOTAL_PATIENT_PRICE_BHYT);
                            }
                        }
                    }
                }
                #endregion

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ProcessSingleKey()
        {
            try
            {

                SetSingleKey(new KeyValue(Mps000193ExtendSingleKey.TREATMENT_CODE, rdo.treatment.TREATMENT_CODE));
                SetSingleKey(new KeyValue(Mps000193ExtendSingleKey.USERNAME_RETURN_RESULT, rdo.userNameReturnResult));
                SetSingleKey(new KeyValue(Mps000193ExtendSingleKey.STATUS_TREATMENT_OUT, rdo.statusTreatmentOut));
                if (rdo.patyAlterBHYTADO != null)
                {
                    if (rdo.patyAlterBHYTADO.IS_HEIN != null)
                        SetSingleKey(new KeyValue(Mps000193ExtendSingleKey.IS_HEIN, "X"));
                    else
                        SetSingleKey(new KeyValue(Mps000193ExtendSingleKey.IS_NOT_HEIN, "X"));
                    if (rdo.patyAlterBHYTADO.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE)
                    {
                        if (rdo.patyAlterBHYTADO.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.EMERGENCY)// la dung tuyen cap cuu
                        {
                            SetSingleKey(new KeyValue(Mps000193ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, "X"));
                            SetSingleKey(new KeyValue(Mps000193ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                            SetSingleKey(new KeyValue(Mps000193ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                        else if (rdo.patyAlterBHYTADO.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.PRESENT)// la dung tuyen: gioi thieu,
                        {
                            SetSingleKey(new KeyValue(Mps000193ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            SetSingleKey(new KeyValue(Mps000193ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                            SetSingleKey(new KeyValue(Mps000193ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                        else
                        {
                            SetSingleKey(new KeyValue(Mps000193ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            SetSingleKey(new KeyValue(Mps000193ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                            SetSingleKey(new KeyValue(Mps000193ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                    }
                    else if (rdo.patyAlterBHYTADO.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.FALSE)//trai tuyen
                    {
                        SetSingleKey(new KeyValue(Mps000193ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                        SetSingleKey(new KeyValue(Mps000193ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                        SetSingleKey(new KeyValue(Mps000193ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, "X"));
                    }

                    SetSingleKey(new KeyValue(Mps000193ExtendSingleKey.HEIN_CARD_ADDRESS, rdo.patyAlterBHYTADO.ADDRESS));
                }
                else
                    SetSingleKey(new KeyValue(Mps000193ExtendSingleKey.IS_NOT_HEIN, "X"));

                if (rdo.departmentTrans != null && rdo.departmentTrans.Count > 0)
                {
                    if (rdo.departmentTrans[0].DEPARTMENT_IN_TIME.HasValue)
                    {
                        SetSingleKey(new KeyValue(Mps000193ExtendSingleKey.OPEN_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.departmentTrans[0].DEPARTMENT_IN_TIME.Value)));
                    }
                    if (rdo.departmentTrans[rdo.departmentTrans.Count - 1] != null && rdo.departmentTrans.Count > 1)
                    {
                        SetSingleKey(new KeyValue(Mps000193ExtendSingleKey.DEPARTMENT_NAME_CLOSE_TREATMENT, rdo.departmentTrans[rdo.departmentTrans.Count - 1].DEPARTMENT_NAME));
                    }


                    //Thời gian vào khoa
                    if (rdo.departmentId > 0)
                    {
                        rdo.departmentTrans = rdo.departmentTrans.OrderBy(o => o.DEPARTMENT_IN_TIME).ToList();
                        V_HIS_DEPARTMENT_TRAN currentDepartmentTran = rdo.departmentTrans.FirstOrDefault(o => o.DEPARTMENT_ID == rdo.departmentId);
                        if (currentDepartmentTran != null)
                        {
                            if (currentDepartmentTran.DEPARTMENT_IN_TIME.HasValue)
                            {
                                SetSingleKey(new KeyValue(Mps000193ExtendSingleKey.TIME_DEPARTMENT_IN, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(currentDepartmentTran.DEPARTMENT_IN_TIME.Value)));
                            }

                            int indexCurrentDepartment = rdo.departmentTrans.IndexOf(currentDepartmentTran);
                            if (rdo.departmentTrans[indexCurrentDepartment + 1] != null)
                            {
                                if (rdo.departmentTrans[indexCurrentDepartment + 1].DEPARTMENT_IN_TIME.HasValue)
                                {
                                    SetSingleKey(new KeyValue(Mps000193ExtendSingleKey.TIME_DEPARTMENT_OUT, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.departmentTrans[indexCurrentDepartment + 1].DEPARTMENT_IN_TIME.Value)));
                                }
                            }
                        }
                    }
                }

                if (rdo.treatment != null)
                {
                    if (rdo.treatment.CLINICAL_IN_TIME.HasValue)
                    {
                        SetSingleKey(new KeyValue(Mps000193ExtendSingleKey.CLINICAL_IN_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.treatment.CLINICAL_IN_TIME.Value)));
                    }

                    if (rdo.treatment.OUT_TIME.HasValue)
                    {
                        SetSingleKey(new KeyValue(Mps000193ExtendSingleKey.CLOSE_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.treatment.OUT_TIME.Value)));
                    }
                }

                SetSingleKey(new KeyValue(Mps000193ExtendSingleKey.TOTAL_DAY, rdo.today));

                if (rdo.treatment != null)
                {
                    SetSingleKey(new KeyValue(Mps000193ExtendSingleKey.TRAN_PATI_MEDI_ORG_CODE, rdo.treatment.TRANSFER_IN_MEDI_ORG_CODE));
                    SetSingleKey(new KeyValue(Mps000193ExtendSingleKey.TRAN_PATI_MEDI_ORG_NAME, rdo.treatment.TRANSFER_IN_MEDI_ORG_NAME));
                }

                SetSingleKey(new KeyValue(Mps000193ExtendSingleKey.CURRENT_DATE_SEPARATE_FULL_STR, rdo.currentDateSeparateFullTime));

                if (!String.IsNullOrEmpty(rdo.departmentName))
                {
                    SetSingleKey(new KeyValue(Mps000193ExtendSingleKey.DEPARTMENT_NAME, rdo.departmentName));
                }

                decimal bhytthanhtoan_tong = 0;
                decimal nguonkhac_tong = 0;

                decimal thanhtien_tong_bhyt = 0;
                decimal bnthanhtoan_tong_bhyt = 0;

                if (rdo.sereServADOs != null && rdo.sereServADOs.Count > 0)
                {
                    bhytthanhtoan_tong = rdo.sereServADOs.Sum(o => o.VIR_TOTAL_HEIN_PRICE) ?? 0;
                    nguonkhac_tong = 0;

                    thanhtien_tong_bhyt = rdo.sereServADOs.Sum(o => o.TOTAL_PRICE_BHYT);
                    bnthanhtoan_tong_bhyt = rdo.sereServADOs.Sum(o => o.VIR_TOTAL_PATIENT_PRICE_BHYT ?? 0);
                }

                SetSingleKey(new KeyValue(Mps000193ExtendSingleKey.TOTAL_PRICE_BHYT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(thanhtien_tong_bhyt, 0)));
                SetSingleKey(new KeyValue(Mps000193ExtendSingleKey.TOTAL_PRICE_HEIN, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bhytthanhtoan_tong, 0)));
                SetSingleKey(new KeyValue(Mps000193ExtendSingleKey.TOTAL_PRICE_PATIENT_BHYT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bnthanhtoan_tong_bhyt, 0)));
                SetSingleKey(new KeyValue(Mps000193ExtendSingleKey.TOTAL_PRICE_OTHER, Inventec.Common.Number.Convert.NumberToStringRoundAuto(nguonkhac_tong, 0)));
                SetSingleKey(new KeyValue(Mps000193ExtendSingleKey.TOTAL_PRICE_BHYT_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(thanhtien_tong_bhyt).ToString())));
                SetSingleKey(new KeyValue(Mps000193ExtendSingleKey.TOTAL_PRICE_HEIN_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bhytthanhtoan_tong).ToString())));
                SetSingleKey(new KeyValue(Mps000193ExtendSingleKey.TOTAL_PRICE_PATIENT_BHYT_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bnthanhtoan_tong_bhyt).ToString())));
                SetSingleKey(new KeyValue(Mps000193ExtendSingleKey.TOTAL_PRICE_OTHER_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(nguonkhac_tong).ToString())));




                if (rdo.treatmentFees != null)
                {
                    SetSingleKey(new KeyValue(Mps000193ExtendSingleKey.TOTAL_DEPOSIT_AMOUNT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(rdo.treatmentFees[0].TOTAL_DEPOSIT_AMOUNT ?? 0, 0)));
                }
                else
                {
                    SetSingleKey(new KeyValue(Mps000193ExtendSingleKey.TOTAL_DEPOSIT_AMOUNT, "0"));
                }

                SetSingleKey(new KeyValue(Mps000193ExtendSingleKey.CREATOR_USERNAME, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName()));

                AddObjectKeyIntoListkey<PatientADO>(rdo.patientADO);
                AddObjectKeyIntoListkey<PatyAlterBhytADO>(rdo.patyAlterBHYTADO);
                AddObjectKeyIntoListkey<V_HIS_TREATMENT>(rdo.treatment, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
