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
using MPS.Processor.Mps000127.PDO;
using FlexCel.Report;
using MPS.ProcessorBase;

namespace MPS.Processor.Mps000127
{
    public partial class Mps000127Processor : AbstractProcessor
    {
        Mps000127PDO rdo;
        public Mps000127Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000127PDO)rdoBase;
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

                dicImage.Add(Mps000127ExtendSingleKey.TREATMENT_CODE_BAR, barcodeTreatment);
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
                objectTag.AddObjectData(store, "HeinServiceTypes", rdo.currentheinServiceTypes);
                objectTag.AddObjectData(store, "KTCFeeGroups", rdo.ktcFeeGroups);
                objectTag.AddObjectData(store, "ExecuteGroups", rdo.sereServExecuteGroups);
                objectTag.AddObjectData(store, "MaterialGroup", rdo.MaterialGroupADOs);
                objectTag.AddObjectData(store, "ListEkipUser", rdo.EkipUsers);

                objectTag.AddRelationship(store, "KTCFeeGroups", "ExecuteGroups", "IS_OUT_PARENT_FEE", "IS_OUT_PARENT_FEE");
                objectTag.AddRelationship(store, "ExecuteGroups", "HeinServiceTypes", "TDL_EXECUTE_GROUP_ID", "TDL_EXECUTE_GROUP_ID");
                objectTag.AddRelationship(store, "KTCFeeGroups", "HeinServiceTypes", "IS_OUT_PARENT_FEE", "IS_OUT_PARENT_FEE");
                objectTag.AddRelationship(store, "HeinServiceTypes", "Services", "TDL_HEIN_SERVICE_TYPE_ID", "TDL_HEIN_SERVICE_TYPE_ID");
                objectTag.AddRelationship(store, "ExecuteGroups", "Services", "TDL_EXECUTE_GROUP_ID", "TDL_EXECUTE_GROUP_ID");
                objectTag.AddRelationship(store, "KTCFeeGroups", "Services", "IS_OUT_PARENT_FEE", "IS_OUT_PARENT_FEE");

                objectTag.AddRelationship(store, "KTCFeeGroups", "MaterialGroup", "IS_OUT_PARENT_FEE", "IS_OUT_PARENT_FEE");
                objectTag.AddRelationship(store, "ExecuteGroups", "MaterialGroup", "TDL_EXECUTE_GROUP_ID", "TDL_EXECUTE_GROUP_ID");
                objectTag.AddRelationship(store, "HeinServiceTypes", "MaterialGroup", "TDL_HEIN_SERVICE_TYPE_ID", "TDL_HEIN_SERVICE_TYPE_ID");
                objectTag.AddRelationship(store, "MaterialGroup", "Services", "MATERIAL_GROUP", "MATERIAL_GROUP");

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
                                             select new SereServADO(r, rdo.Services, rdo.HeinServiceTypes, rdo.Rooms, rdo.SereServExts, rdo.MaterialTypes));
                var sereServKTCGroups = sereServKTCADOTemps
                    .Where(o => o.IS_NO_EXECUTE != 1
                    && (o.VIR_PRICE_NO_EXPEND > 0 || o.PRICE_POLICY > 0)
                    && o.AMOUNT > 0)
                    .OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999).ThenBy(n => n.STENT_ORDER ?? 99999).ToList();
                //.GroupBy(o => new
                //{
                //    o.SERVICE_ID,
                //    o.TOTAL_HEIN_PRICE_ONE_AMOUNT,
                //    o.TDL_REQUEST_DEPARTMENT_ID,
                //    o.IS_OUT_PARENT_FEE,
                //    o.PATIENT_TYPE_ID,
                //    o.PACKAGE_ID,
                //    o.IS_EXPEND,
                //    o.PARENT_ID
                //});

                //foreach (var sereServKTCGroup in sereServKTCGroups)
                //{
                //    SereServADO sereServ = sereServKTCGroup.FirstOrDefault();
                //    sereServ.AMOUNT = sereServKTCGroup.Sum(o => o.AMOUNT);
                //    sereServ.VIR_TOTAL_PRICE_NO_EXPEND = sereServKTCGroup.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND);
                //    sereServ.VIR_TOTAL_HEIN_PRICE = sereServKTCGroup.Sum(o => o.VIR_TOTAL_HEIN_PRICE);
                //    rdo.sereServKTCADOs.Add(sereServ);
                //}

                rdo.sereServKTCADOs = sereServKTCGroups;

                //SereServADO
                var sereServADOTemps = new List<SereServADO>();
                sereServADOTemps.AddRange(from r in rdo.sereServs
                                          select new SereServADO(r, rdo.Services, rdo.patyAlterBHYTADO, rdo.HeinServiceTypes, rdo.Rooms, rdo.SereServExts, rdo.MaterialTypes));

                //#region dịch vụ khám, pttt, ptc gom theo khoa thực hiện
                //foreach (var item in sereServADOTemps)
                //{
                //    if ((item.SERVICE_TYPE_ID == ServiceTypeCfg.SERVICE_TYPE_ID__EXAM
                //        || item.SERVICE_TYPE_ID == ServiceTypeCfg.SERVICE_TYPE_ID__MISU
                //        || item.SERVICE_TYPE_ID == ServiceTypeCfg.SERVICE_TYPE_ID__SURG)
                //        && item.PARENT_ID != null)
                //    {
                //        item.REQUEST_DEPARTMENT_ID = item.EXECUTE_DEPARTMENT_ID;
                //    }
                //}
                //#endregion

                //sereServkhong phai la dịch vu ky thuat cao
                var sereServGroups = sereServADOTemps
                    .Where(o =>
                        (rdo.sereServKTCs != null ? !rdo.sereServKTCs.Select(p => p.ID).Contains(o.ID) : true)
                        && o.AMOUNT > 0
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

                    if (rdo.SingleKeyValue.Type == 1)
                    {
                        sereServ.VIR_PRICE = sereServGroup.First().VIR_PRICE_NO_EXPEND;
                        sereServ.VIR_TOTAL_PRICE = sereServGroup.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND);
                    }
                    if (rdo.SingleKeyValue.Type == 2)
                    {
                        sereServ.VIR_TOTAL_PRICE = sereServGroup.Sum(o => o.VIR_TOTAL_PRICE);
                    }

                    sereServ.AMOUNT = sereServGroup.Sum(o => o.AMOUNT);
                    sereServ.VIR_TOTAL_PATIENT_PRICE = sereServGroup.Sum(o => o.VIR_TOTAL_PATIENT_PRICE);
                    sereServ.VIR_TOTAL_PATIENT_PRICE_BHYT = sereServGroup.Sum(o => o.VIR_TOTAL_PATIENT_PRICE_BHYT);
                    sereServ.VIR_TOTAL_HEIN_PRICE = sereServGroup.Sum(o => o.VIR_TOTAL_HEIN_PRICE);
                    sereServ.VIR_TOTAL_PATIENT_PRICE_NO_DC = sereServGroup.Sum(o => o.VIR_TOTAL_PATIENT_PRICE_NO_DC);
                    sereServ.VIR_TOTAL_PATIENT_PRICE_TEMP = sereServGroup.Sum(o => o.VIR_TOTAL_PATIENT_PRICE_TEMP);
                    sereServ.VIR_TOTAL_PRICE_NO_ADD_PRICE = sereServGroup.Sum(o => o.VIR_TOTAL_PRICE_NO_ADD_PRICE);

                    rdo.sereServADOs.Add(sereServ);
                }

                rdo.sereServADOs = rdo.sereServADOs.OrderBy(o => o.TDL_SERVICE_NAME).ThenBy(p => p.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999999).ThenBy(n => n.STENT_ORDER ?? 99999999).ToList();
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
                rdo.currentheinServiceTypes = new List<SereServADO>();
                rdo.MaterialGroupADOs = new List<SereServADO>();

                //Nhom chi phi trong, ngoai goi theo dich vu KTC
                var sereServKTCGroups = rdo.sereServADOs.GroupBy(o => o.IS_OUT_PARENT_FEE).ToList();
                foreach (var sereServKTCGroup in sereServKTCGroups)
                {
                    List<SereServADO> sereServADOTemps = sereServKTCGroup.ToList<SereServADO>();
                    SereServADO ktcFeeGroup = new SereServADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<SereServADO>(ktcFeeGroup, sereServKTCGroup.First());
                    if (ktcFeeGroup.IS_OUT_PARENT_FEE != 1 || ktcFeeGroup.IS_OUT_PARENT_FEE == null)
                    {
                        ktcFeeGroup.KTC_FEE_GROUP_NAME = "CHI PHÍ TRONG GÓI PHẪU THUẬT";
                    }
                    else
                    {
                        ktcFeeGroup.KTC_FEE_GROUP_NAME = "CHI PHÍ NGOÀI GÓI PHẪU THUẬT";
                    }

                    ktcFeeGroup.VIR_TOTAL_PRICE = sereServKTCGroup.Sum(o => o.VIR_TOTAL_PRICE);
                    ktcFeeGroup.VIR_TOTAL_HEIN_PRICE = sereServKTCGroup.Sum(o => o.VIR_TOTAL_HEIN_PRICE);
                    ktcFeeGroup.VIR_TOTAL_PATIENT_PRICE = sereServKTCGroup.Sum(o => o.VIR_TOTAL_PATIENT_PRICE);
                    ktcFeeGroup.VIR_TOTAL_PATIENT_PRICE_BHYT = sereServKTCGroup.Sum(o => o.VIR_TOTAL_PATIENT_PRICE_BHYT);
                    ktcFeeGroup.OTHER_SOURCE_PRICE = sereServKTCGroup.Sum(o => o.OTHER_SOURCE_PRICE);

                    rdo.ktcFeeGroups.Add(ktcFeeGroup);

                    //Nhom Execute
                    var sereServExecuteGroupTemps = sereServADOTemps.GroupBy(o => o.TDL_EXECUTE_GROUP_ID).ToList();
                    foreach (var sereServExecuteGroupTemp in sereServExecuteGroupTemps)
                    {
                        SereServADO executeGroup = new SereServADO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<SereServADO>(executeGroup, sereServExecuteGroupTemp.First());
                        if (executeGroup.TDL_EXECUTE_GROUP_ID == null)
                            executeGroup.EXECUTE_GROUP_NAME = "    ";
                        else
                            executeGroup.EXECUTE_GROUP_NAME = rdo.executeGroups.FirstOrDefault(o => o.ID == executeGroup.TDL_EXECUTE_GROUP_ID).EXECUTE_GROUP_NAME;

                        executeGroup.VIR_TOTAL_PRICE = sereServExecuteGroupTemp.Sum(o => o.VIR_TOTAL_PRICE);
                        executeGroup.VIR_TOTAL_HEIN_PRICE = sereServExecuteGroupTemp.Sum(o => o.VIR_TOTAL_HEIN_PRICE);
                        executeGroup.VIR_TOTAL_PATIENT_PRICE = sereServExecuteGroupTemp.Sum(o => o.VIR_TOTAL_PATIENT_PRICE);
                        executeGroup.VIR_TOTAL_PATIENT_PRICE_BHYT = sereServExecuteGroupTemp.Sum(o => o.VIR_TOTAL_PATIENT_PRICE_BHYT);
                        executeGroup.OTHER_SOURCE_PRICE = sereServExecuteGroupTemp.Sum(o => o.OTHER_SOURCE_PRICE);

                        rdo.sereServExecuteGroups.Add(executeGroup);

                        //Nhom HeinServiceType
                        var heinServiceTypeGroups = sereServExecuteGroupTemp.OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999999999)
                            .GroupBy(o => o.TDL_HEIN_SERVICE_TYPE_ID).ToList();
                        foreach (var heinServiceTypeGroup in heinServiceTypeGroups)
                        {
                                SereServADO heinServiceType = new SereServADO();
                                Inventec.Common.Mapper.DataObjectMapper.Map<SereServADO>(heinServiceType, heinServiceTypeGroup.First());
                            heinServiceType.HEIN_SERVICE_TYPE_NAME = heinServiceType.TDL_HEIN_SERVICE_TYPE_ID.HasValue ? heinServiceType.HEIN_SERVICE_TYPE_NAME : "Khác";

                            heinServiceType.TOTAL_PRICE_HEIN_SERVICE_TYPE = heinServiceTypeGroup
                                              .Sum(o => o.VIR_TOTAL_PRICE);
                            heinServiceType.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE = heinServiceTypeGroup
                                .Sum(o => o.VIR_TOTAL_HEIN_PRICE);
                            heinServiceType.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE = heinServiceTypeGroup.Sum(o => o.VIR_TOTAL_PATIENT_PRICE);

                            heinServiceType.VIR_TOTAL_PRICE = heinServiceTypeGroup.Sum(o => o.VIR_TOTAL_PRICE);
                            heinServiceType.VIR_TOTAL_HEIN_PRICE = heinServiceTypeGroup.Sum(o => o.VIR_TOTAL_HEIN_PRICE);
                            heinServiceType.VIR_TOTAL_PATIENT_PRICE = heinServiceTypeGroup.Sum(o => o.VIR_TOTAL_PATIENT_PRICE);
                            heinServiceType.VIR_TOTAL_PATIENT_PRICE_BHYT = heinServiceTypeGroup.Sum(o => o.VIR_TOTAL_PATIENT_PRICE_BHYT);
                            heinServiceType.OTHER_SOURCE_PRICE = heinServiceTypeGroup.Sum(o => o.OTHER_SOURCE_PRICE);

                            rdo.currentheinServiceTypes.Add(heinServiceType);

                            var SSMGroups = heinServiceTypeGroup.OrderBy(o => o.MATERIAL_GROUP !=0 ? o.MATERIAL_GROUP : 99999999999).GroupBy(o => o.MATERIAL_GROUP).ToList();
                            foreach (var SSMGroup in SSMGroups)
                            {
                                SereServADO ado = new SereServADO();
                                Inventec.Common.Mapper.DataObjectMapper.Map<SereServADO>(ado, SSMGroup.First());
                                ado.VIR_TOTAL_PRICE = SSMGroup.Sum(o => o.VIR_TOTAL_PRICE);
                                ado.VIR_TOTAL_HEIN_PRICE = SSMGroup.Sum(o => o.VIR_TOTAL_HEIN_PRICE);
                                ado.VIR_TOTAL_PATIENT_PRICE = SSMGroup.Sum(o => o.VIR_TOTAL_PATIENT_PRICE);
                                ado.VIR_TOTAL_PATIENT_PRICE_BHYT = SSMGroup.Sum(o => o.VIR_TOTAL_PATIENT_PRICE_BHYT);
                                ado.OTHER_SOURCE_PRICE = SSMGroup.Sum(o => o.OTHER_SOURCE_PRICE);
                                rdo.MaterialGroupADOs.Add(ado);
                            }
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

                    ktcFee.TOTAL_PRICE_KTC_FEE_GROUP = sereServKTCTemps.Sum(o => o.VIR_TOTAL_PRICE);
                    ktcFee.TOTAL_HEIN_PRICE_FEE_GROUP = sereServKTCTemps.Sum(o => o.VIR_TOTAL_HEIN_PRICE);
                    ktcFee.TOTAL_PATIENT_PRICE_FEE_GROUP = sereServKTCTemps.Sum(o => o.VIR_TOTAL_PATIENT_PRICE);

                    if (sereServKTCTemps != null && sereServKTCTemps.Count > 0)
                    {
                        foreach (var execute in rdo.sereServExecuteGroups)
                        {
                            List<SereServADO> sereServExecuteTemps = sereServKTCTemps.Where(o => o.TDL_EXECUTE_GROUP_ID == execute.TDL_EXECUTE_GROUP_ID).ToList();
                            if (sereServExecuteTemps != null && sereServExecuteTemps.Count > 0)
                            {
                                execute.TOTAL_PRICE_EXECUTE_GROUP = sereServExecuteTemps.Sum(o => o.VIR_TOTAL_PRICE);
                                execute.TOTAL_HEIN_PRICE_EXECUTE_GROUP = sereServExecuteTemps.Sum(o => o.VIR_TOTAL_HEIN_PRICE);
                                execute.TOTAL_PATIENT_PRICE_EXECUTE_GROUP = sereServExecuteTemps.Sum(o => o.VIR_TOTAL_PATIENT_PRICE);
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

                SetSingleKey(new KeyValue(Mps000127ExtendSingleKey.TREATMENT_CODE, rdo.treatment.TREATMENT_CODE));
                SetSingleKey(new KeyValue(Mps000127ExtendSingleKey.USERNAME_RETURN_RESULT, rdo.SingleKeyValue.UserNameReturnResult));
                SetSingleKey(new KeyValue(Mps000127ExtendSingleKey.STATUS_TREATMENT_OUT, rdo.SingleKeyValue.StatusTreatmentOut));
                if (rdo.patyAlterBHYTADO != null)
                {
                    if (rdo.patyAlterBHYTADO.IS_HEIN != null)
                        SetSingleKey(new KeyValue(Mps000127ExtendSingleKey.IS_HEIN, "X"));
                    else
                        SetSingleKey(new KeyValue(Mps000127ExtendSingleKey.IS_NOT_HEIN, "X"));
                    if (rdo.patyAlterBHYTADO.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE)
                    {
                        if (rdo.patyAlterBHYTADO.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.EMERGENCY)// la dung tuyen cap cuu
                        {
                            SetSingleKey(new KeyValue(Mps000127ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, "X"));
                            SetSingleKey(new KeyValue(Mps000127ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                            SetSingleKey(new KeyValue(Mps000127ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                        else if (rdo.patyAlterBHYTADO.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.PRESENT)// la dung tuyen: gioi thieu,
                        {
                            SetSingleKey(new KeyValue(Mps000127ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            SetSingleKey(new KeyValue(Mps000127ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                            SetSingleKey(new KeyValue(Mps000127ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                        else
                        {
                            SetSingleKey(new KeyValue(Mps000127ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            SetSingleKey(new KeyValue(Mps000127ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                            SetSingleKey(new KeyValue(Mps000127ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                    }
                    else if (rdo.patyAlterBHYTADO.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.FALSE)//trai tuyen
                    {
                        SetSingleKey(new KeyValue(Mps000127ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                        SetSingleKey(new KeyValue(Mps000127ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                        SetSingleKey(new KeyValue(Mps000127ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, "X"));
                    }

                    SetSingleKey(new KeyValue(Mps000127ExtendSingleKey.HEIN_CARD_ADDRESS, rdo.patyAlterBHYTADO.ADDRESS));
                }
                else
                    SetSingleKey(new KeyValue(Mps000127ExtendSingleKey.IS_NOT_HEIN, "X"));

                if (rdo.departmentTrans != null && rdo.departmentTrans.Count > 0)
                {
                    SetSingleKey(new KeyValue(Mps000127ExtendSingleKey.OPEN_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.departmentTrans[0].DEPARTMENT_IN_TIME ?? 0)));
                    if (rdo.departmentTrans[rdo.departmentTrans.Count - 1] != null && rdo.departmentTrans.Count > 1)
                    {
                        SetSingleKey(new KeyValue(Mps000127ExtendSingleKey.DEPARTMENT_NAME_CLOSE_TREATMENT, rdo.departmentTrans[rdo.departmentTrans.Count - 1].DEPARTMENT_NAME));
                    }


                    //Thời gian vào khoa
                    if (rdo.SingleKeyValue.DepartmentId > 0)
                    {
                        rdo.departmentTrans = rdo.departmentTrans.OrderBy(o => o.DEPARTMENT_IN_TIME).ToList();
                        V_HIS_DEPARTMENT_TRAN currentDepartmentTran = rdo.departmentTrans.FirstOrDefault(o => o.DEPARTMENT_ID == rdo.SingleKeyValue.DepartmentId);
                        if (currentDepartmentTran != null)
                        {
                            if (currentDepartmentTran.DEPARTMENT_IN_TIME.HasValue)
                            {
                                SetSingleKey(new KeyValue(Mps000127ExtendSingleKey.TIME_DEPARTMENT_IN, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(currentDepartmentTran.DEPARTMENT_IN_TIME.Value)));
                            }

                            int indexCurrentDepartment = rdo.departmentTrans.IndexOf(currentDepartmentTran);
                            if (rdo.departmentTrans.Count > (indexCurrentDepartment + 1))
                            {
                                if (rdo.departmentTrans[indexCurrentDepartment + 1].DEPARTMENT_IN_TIME.HasValue)
                                {
                                    SetSingleKey(new KeyValue(Mps000127ExtendSingleKey.TIME_DEPARTMENT_OUT, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.departmentTrans[indexCurrentDepartment + 1].DEPARTMENT_IN_TIME.Value)));
                                }
                            }
                        }
                    }
                }

                if (rdo.treatment != null)
                {
                    if (rdo.treatment.CLINICAL_IN_TIME.HasValue)
                    {
                        SetSingleKey(new KeyValue(Mps000127ExtendSingleKey.CLINICAL_IN_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.treatment.CLINICAL_IN_TIME.Value)));
                    }

                    if (rdo.treatment.OUT_TIME.HasValue)
                    {
                        SetSingleKey(new KeyValue(Mps000127ExtendSingleKey.CLOSE_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.treatment.OUT_TIME.Value)));
                    }
                }

                SetSingleKey(new KeyValue(Mps000127ExtendSingleKey.TOTAL_DAY, rdo.SingleKeyValue.Today));

                if (rdo.treatment != null)
                {
                    SetSingleKey(new KeyValue(Mps000127ExtendSingleKey.TRAN_PATI_MEDI_ORG_CODE, rdo.treatment.TRANSFER_IN_MEDI_ORG_CODE));
                    SetSingleKey(new KeyValue(Mps000127ExtendSingleKey.TRAN_PATI_MEDI_ORG_NAME, rdo.treatment.TRANSFER_IN_MEDI_ORG_NAME));
                }

                if (!String.IsNullOrEmpty(rdo.SingleKeyValue.DepartmentName))
                {
                    SetSingleKey(new KeyValue(Mps000127ExtendSingleKey.DEPARTMENT_NAME, rdo.SingleKeyValue.DepartmentName));
                }

                decimal thanhtien_tong = 0;
                decimal bhytthanhtoan_tong = 0;
                decimal nguonkhac_tong = 0;
                decimal bnthanhtoan_tong = 0;

                if (rdo.sereServADOs != null && rdo.sereServADOs.Count > 0)
                {
                    thanhtien_tong = rdo.sereServADOs.Sum(o => o.VIR_TOTAL_PRICE) ?? -999;
                    bhytthanhtoan_tong = rdo.sereServADOs.Sum(o => o.VIR_TOTAL_HEIN_PRICE) ?? 0;
                    bnthanhtoan_tong = rdo.sereServADOs.Sum(o => o.VIR_TOTAL_PATIENT_PRICE) ?? 0;
                    nguonkhac_tong = 0;
                }

                SetSingleKey(new KeyValue(Mps000127ExtendSingleKey.TOTAL_PRICE, Inventec.Common.Number.Convert.NumberToStringRoundAuto(thanhtien_tong, 0)));
                SetSingleKey(new KeyValue(Mps000127ExtendSingleKey.TOTAL_PRICE_HEIN, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bhytthanhtoan_tong, 0)));
                SetSingleKey(new KeyValue(Mps000127ExtendSingleKey.TOTAL_PRICE_PATIENT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bnthanhtoan_tong, 0)));
                SetSingleKey(new KeyValue(Mps000127ExtendSingleKey.TOTAL_PRICE_OTHER, Inventec.Common.Number.Convert.NumberToStringRoundAuto(nguonkhac_tong, 0)));
                SetSingleKey(new KeyValue(Mps000127ExtendSingleKey.TOTAL_PRICE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(thanhtien_tong).ToString())));
                SetSingleKey(new KeyValue(Mps000127ExtendSingleKey.TOTAL_PRICE_HEIN_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bhytthanhtoan_tong).ToString())));
                SetSingleKey(new KeyValue(Mps000127ExtendSingleKey.TOTAL_PRICE_PATIENT_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bnthanhtoan_tong).ToString())));
                SetSingleKey(new KeyValue(Mps000127ExtendSingleKey.TOTAL_PRICE_OTHER_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(nguonkhac_tong).ToString())));

                if (rdo.treatmentFees != null)
                {
                    SetSingleKey(new KeyValue(Mps000127ExtendSingleKey.TOTAL_DEPOSIT_AMOUNT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(rdo.treatmentFees[0].TOTAL_DEPOSIT_AMOUNT ?? 0, 0)));
                }
                else
                {
                    SetSingleKey(new KeyValue(Mps000127ExtendSingleKey.TOTAL_DEPOSIT_AMOUNT, "0"));
                }

                SetSingleKey(new KeyValue(Mps000127ExtendSingleKey.CREATOR_USERNAME, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName()));

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
