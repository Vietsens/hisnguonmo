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
using FlexCel.Report;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000375.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000375
{
    class Mps000375Processor : AbstractProcessor
    {
        Mps000375PDO rdo;

        public Mps000375Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000375PDO)rdoBase;
        }

        public void SetBarcodeKey()
        {
            try
            {
                if (rdo.currentHisTreatment != null && !String.IsNullOrWhiteSpace(rdo.currentHisTreatment.TREATMENT_CODE))
                {
                    Inventec.Common.BarcodeLib.Barcode barcodeTreatmentCode = new Inventec.Common.BarcodeLib.Barcode(rdo.currentHisTreatment.TREATMENT_CODE);
                    barcodeTreatmentCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                    barcodeTreatmentCode.IncludeLabel = false;
                    barcodeTreatmentCode.Width = 120;
                    barcodeTreatmentCode.Height = 40;
                    barcodeTreatmentCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                    barcodeTreatmentCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                    barcodeTreatmentCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                    barcodeTreatmentCode.IncludeLabel = true;

                    dicImage.Add(Mps000375ExtendSingleKey.TREATMENT_CODE_BAR, barcodeTreatmentCode);
                }

                if (rdo.currentHisTreatment != null && !String.IsNullOrWhiteSpace(rdo.currentHisTreatment.TDL_PATIENT_CODE))
                {
                    Inventec.Common.BarcodeLib.Barcode barcodeTreatmentCode = new Inventec.Common.BarcodeLib.Barcode(rdo.currentHisTreatment.TDL_PATIENT_CODE);
                    barcodeTreatmentCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                    barcodeTreatmentCode.IncludeLabel = false;
                    barcodeTreatmentCode.Width = 120;
                    barcodeTreatmentCode.Height = 40;
                    barcodeTreatmentCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                    barcodeTreatmentCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                    barcodeTreatmentCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                    barcodeTreatmentCode.IncludeLabel = true;

                    dicImage.Add(Mps000375ExtendSingleKey.PATIENT_CODE_BAR, barcodeTreatmentCode);
                }

                if (rdo.deposit != null && !String.IsNullOrWhiteSpace(rdo.deposit.TRANSACTION_CODE))
                {
                    Inventec.Common.BarcodeLib.Barcode barcodeTreatmentCode = new Inventec.Common.BarcodeLib.Barcode(rdo.deposit.TRANSACTION_CODE);
                    barcodeTreatmentCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                    barcodeTreatmentCode.IncludeLabel = false;
                    barcodeTreatmentCode.Width = 120;
                    barcodeTreatmentCode.Height = 40;
                    barcodeTreatmentCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                    barcodeTreatmentCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                    barcodeTreatmentCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                    barcodeTreatmentCode.IncludeLabel = true;

                    dicImage.Add(Mps000375ExtendSingleKey.TRANSACTION_CODE_BAR, barcodeTreatmentCode);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Ham xu ly du lieu da qua xu ly
        /// Tao ra cac doi tuong du lieu xu dung trong thu vien xu ly file excel
        /// </summary>
        /// <returns></returns>
        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();


                if (rdo.ServiceGroups == null)
                {
                    rdo.ServiceGroups = new List<SereServGroupPlusADO>();
                }
                if (rdo.sereServNotHiTechs == null)
                {
                    rdo.sereServNotHiTechs = new List<SereServGroupPlusADO>();
                }
                if (rdo.DepartmentGroups == null)
                {
                    rdo.DepartmentGroups = new List<SereServGroupPlusADO>();
                }
                if (rdo.HightTechDepartmentGroups == null)
                {
                    rdo.HightTechDepartmentGroups = new List<SereServGroupPlusADO>();
                }
                if (rdo.ServiceVTTTDepartmentGroup == null)
                {
                    rdo.ServiceVTTTDepartmentGroup = new List<SereServGroupPlusADO>();
                }
                if (rdo.sereServHitechs == null)
                {
                    rdo.sereServHitechs = new List<SereServGroupPlusADO>();
                }
                if (rdo.sereServVTTTs == null)
                {
                    rdo.sereServVTTTs = new List<SereServGroupPlusADO>();
                }
                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                UpdateHeinServicePrice();
                OrderServiceNameInSereServs();
                ProcessGroupSereServ();
                ProcessDereDetail();
                ProcessTotalPriceGroup();
                ProcessSingleKey();
                SetBarcodeKey();

                //ghi đè PrintLogData và UniqueCodeData
                ProcessPrintLogData();
                //lấy số lần in
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));

                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);

                Inventec.Common.Logging.LogSystem.Warn("------- KAKA du lieu trong mps102 rdo.sereServNotHiTechs " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rdo.sereServNotHiTechs), rdo.sereServNotHiTechs));
                Inventec.Common.Logging.LogSystem.Warn("------- KAKA du lieu trong mps102 rdo.ServiceGroups " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rdo.ServiceGroups), rdo.ServiceGroups));
                Inventec.Common.Logging.LogSystem.Warn("------- KAKA du lieu trong mps102 rdo.DepartmentGroups " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rdo.DepartmentGroups), rdo.DepartmentGroups));
                Inventec.Common.Logging.LogSystem.Warn("------- KAKA du lieu trong mps102 rdo.HightTechDepartmentGroups " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rdo.HightTechDepartmentGroups), rdo.HightTechDepartmentGroups));
                Inventec.Common.Logging.LogSystem.Warn("------- KAKA du lieu trong mps102 rdo.ServiceVTTTDepartmentGroup " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rdo.ServiceVTTTDepartmentGroup), rdo.ServiceVTTTDepartmentGroup));
                Inventec.Common.Logging.LogSystem.Warn("------- KAKA du lieu trong mps102 rdo.sereServHitechs " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rdo.sereServHitechs), rdo.sereServHitechs));
                Inventec.Common.Logging.LogSystem.Warn("------- KAKA du lieu trong mps102 rdo.sereServVTTTs " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rdo.sereServVTTTs), rdo.sereServVTTTs));

                objectTag.AddObjectData(store, "Services", rdo.sereServNotHiTechs);
                objectTag.AddObjectData(store, "ServiceGroups", rdo.ServiceGroups);
                objectTag.AddObjectData(store, "Departments", rdo.DepartmentGroups);
                objectTag.AddObjectData(store, "HightTechDepartments", rdo.HightTechDepartmentGroups);
                objectTag.AddObjectData(store, "ServiceVTTTDepartments", rdo.ServiceVTTTDepartmentGroup);
                objectTag.AddObjectData(store, "ServiceHightTechs", rdo.sereServHitechs);
                objectTag.AddObjectData(store, "ServiceVTTTs", rdo.sereServVTTTs);

                objectTag.AddRelationship(store, "ServiceGroups", "Departments", "TDL_HEIN_SERVICE_TYPE_ID", "TDL_HEIN_SERVICE_TYPE_ID");
                objectTag.AddRelationship(store, "Departments", "Services", "TDL_REQUEST_DEPARTMENT_ID", "TDL_REQUEST_DEPARTMENT_ID");
                objectTag.AddRelationship(store, "ServiceGroups", "Services", "TDL_HEIN_SERVICE_TYPE_ID", "TDL_HEIN_SERVICE_TYPE_ID");
                objectTag.AddRelationship(store, "ServiceGroups", "HightTechDepartments", "TDL_HEIN_SERVICE_TYPE_ID", "TDL_HEIN_SERVICE_TYPE_ID");
                objectTag.AddRelationship(store, "ServiceGroups", "ServiceHightTechs", "TDL_HEIN_SERVICE_TYPE_ID", "TDL_HEIN_SERVICE_TYPE_ID");

                objectTag.AddRelationship(store, "HightTechDepartments", "ServiceHightTechs", "TDL_EXECUTE_DEPARTMENT_ID", "TDL_EXECUTE_DEPARTMENT_ID");

                objectTag.AddRelationship(store, "ServiceGroups", "ServiceVTTTDepartments", "TDL_HEIN_SERVICE_TYPE_ID", "DEPARTMENT__GROUP_SERVICE_REPORT");

                objectTag.AddRelationship(store, "ServiceVTTTDepartments", "ServiceVTTTs", "TDL_REQUEST_DEPARTMENT_ID", "TDL_REQUEST_DEPARTMENT_ID");

                objectTag.AddRelationship(store, "ServiceGroups", "ServiceVTTTs", "TDL_HEIN_SERVICE_TYPE_ID", "SERE_SERV__GROUP_SERVICE_REPORT");

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        class CustomerFuncRownumberData : TFlexCelUserFunction
        {
            public CustomerFuncRownumberData()
            {
            }

            public override object Evaluate(object[] parameters)
            {
                if (parameters == null || parameters.Length < 1)
                    throw new ArgumentException("Bad parameter count in call to Orders() user-defined function");

                long result = 0;
                try
                {
                    long rownumber = Convert.ToInt64(parameters[0]);
                    result = (rownumber + 1);
                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }

                return result;
            }
        }

        private void ProcessDereDetail()
        {
            try
            {
                rdo.thuChenhLech = 0;
                rdo.thuDongChiTra = 0;
                foreach (var item in this.rdo.dereDetails)
                {
                    //rdo.thuChenhLech += item.AMOUNT;
                    //rdo.thuDongChiTra += item.TDL_VIR_TOTAL_HEIN_PRICE;
                    decimal chenhLech = (item.TDL_VIR_PRICE - (item.TDL_HEIN_LIMIT_PRICE ?? item.TDL_VIR_PRICE)) * item.TDL_AMOUNT;
                    rdo.thuChenhLech += chenhLech;
                    rdo.thuDongChiTra += item.AMOUNT - chenhLech;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //update lại thành tiền bảo hiểm để tránh trường hợp chuyển đổi đối tượng bị lệch so với lúc tạo
        void UpdateHeinServicePrice()
        {
            try
            {

                if (rdo.sereServNotHiTechs != null && rdo.sereServNotHiTechs.Count > 0 && rdo.dereDetails != null && rdo.dereDetails.Count > 0)
                {
                    rdo.sereServNotHiTechs.ForEach(o => o.VIR_TOTAL_HEIN_PRICE = rdo.dereDetails.FirstOrDefault(m => m.SERE_SERV_ID == o.ID).TDL_VIR_TOTAL_HEIN_PRICE);
                    rdo.sereServNotHiTechs.ForEach(o => o.VIR_HEIN_PRICE = rdo.dereDetails.FirstOrDefault(m => m.SERE_SERV_ID == o.ID).TDL_VIR_HEIN_PRICE);
                    //rdo.sereServNotHiTechs.ForEach(o => o.VIR_TOTAL_PATIENT_PRICE = (o.VIR_TOTAL_PRICE - rdo.dereDetails.FirstOrDefault(m => m.SERE_SERV_ID == o.ID).TDL_VIR_HEIN_PRICE));
                    rdo.sereServNotHiTechs.ForEach(o => o.VIR_TOTAL_PATIENT_PRICE = rdo.dereDetails.FirstOrDefault(m => m.SERE_SERV_ID == o.ID).AMOUNT);
                    rdo.sereServNotHiTechs.ForEach(o => o.VIR_TOTAL_PRICE = rdo.dereDetails.FirstOrDefault(m => m.SERE_SERV_ID == o.ID).TDL_VIR_TOTAL_PRICE);
                    rdo.sereServNotHiTechs.ForEach(o => o.VIR_PRICE = rdo.dereDetails.FirstOrDefault(m => m.SERE_SERV_ID == o.ID).TDL_VIR_PRICE);
                }

                if (rdo.sereServHitechs != null && rdo.sereServHitechs.Count > 0 && rdo.dereDetails != null && rdo.dereDetails.Count > 0)
                {
                    rdo.sereServHitechs.ForEach(o => o.VIR_TOTAL_HEIN_PRICE = rdo.dereDetails.FirstOrDefault(m => m.SERE_SERV_ID == o.ID).TDL_VIR_TOTAL_HEIN_PRICE);
                    rdo.sereServHitechs.ForEach(o => o.VIR_HEIN_PRICE = rdo.dereDetails.FirstOrDefault(m => m.SERE_SERV_ID == o.ID).TDL_VIR_HEIN_PRICE);
                    //rdo.sereServHitechs.ForEach(o => o.VIR_TOTAL_PATIENT_PRICE = (o.VIR_TOTAL_PRICE - rdo.dereDetails.FirstOrDefault(m => m.SERE_SERV_ID == o.ID).TDL_VIR_HEIN_PRICE));
                    rdo.sereServHitechs.ForEach(o => o.VIR_TOTAL_PATIENT_PRICE = rdo.dereDetails.FirstOrDefault(m => m.SERE_SERV_ID == o.ID).AMOUNT);
                    rdo.sereServHitechs.ForEach(o => o.VIR_TOTAL_PRICE = rdo.dereDetails.FirstOrDefault(m => m.SERE_SERV_ID == o.ID).TDL_VIR_TOTAL_PRICE);
                    rdo.sereServHitechs.ForEach(o => o.VIR_PRICE = rdo.dereDetails.FirstOrDefault(m => m.SERE_SERV_ID == o.ID).TDL_VIR_PRICE);
                }
                if (rdo.sereServVTTTs != null && rdo.sereServVTTTs.Count > 0 && rdo.dereDetails != null && rdo.dereDetails.Count > 0)
                {
                    rdo.sereServVTTTs.ForEach(o => o.VIR_TOTAL_HEIN_PRICE = rdo.dereDetails.FirstOrDefault(m => m.SERE_SERV_ID == o.ID).TDL_VIR_TOTAL_HEIN_PRICE);
                    rdo.sereServVTTTs.ForEach(o => o.VIR_HEIN_PRICE = rdo.dereDetails.FirstOrDefault(m => m.SERE_SERV_ID == o.ID).TDL_VIR_HEIN_PRICE);
                    //rdo.sereServVTTTs.ForEach(o => o.VIR_TOTAL_PATIENT_PRICE = (o.VIR_TOTAL_PRICE - rdo.dereDetails.FirstOrDefault(m => m.SERE_SERV_ID == o.ID).TDL_VIR_HEIN_PRICE));
                    rdo.sereServVTTTs.ForEach(o => o.VIR_TOTAL_PATIENT_PRICE = rdo.dereDetails.FirstOrDefault(m => m.SERE_SERV_ID == o.ID).AMOUNT);
                    rdo.sereServVTTTs.ForEach(o => o.VIR_TOTAL_PRICE = rdo.dereDetails.FirstOrDefault(m => m.SERE_SERV_ID == o.ID).TDL_VIR_TOTAL_PRICE);
                    rdo.sereServVTTTs.ForEach(o => o.VIR_PRICE = rdo.dereDetails.FirstOrDefault(m => m.SERE_SERV_ID == o.ID).TDL_VIR_PRICE);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void OrderServiceNameInSereServs()
        {
            try
            {
                if (rdo.sereServNotHiTechs != null && rdo.sereServNotHiTechs.Count > 0)
                {
                    rdo.sereServNotHiTechs = rdo.sereServNotHiTechs.OrderBy(o => o.TDL_SERVICE_NAME).ThenBy(m => m.TDL_SERVICE_CODE).ToList();
                }

                if (rdo.sereServHitechs != null && rdo.sereServHitechs.Count > 0)
                {
                    rdo.sereServHitechs = rdo.sereServHitechs.OrderBy(o => o.TDL_SERVICE_NAME).ToList();
                }
                if (rdo.sereServVTTTs != null && rdo.sereServVTTTs.Count > 0)
                {
                    rdo.sereServVTTTs = rdo.sereServVTTTs.OrderBy(o => o.TDL_SERVICE_NAME).ToList();
                }
                // TODO
                if (rdo.HightTechDepartmentGroups != null && rdo.HightTechDepartmentGroups.Count > 0)
                {
                    rdo.HightTechDepartmentGroups = rdo.HightTechDepartmentGroups.OrderBy(o => o.EXECUTE_DEPARTMENT_NAME).ToList();
                }
                if (rdo.DepartmentGroups != null && rdo.DepartmentGroups.Count > 0)
                {
                    rdo.DepartmentGroups = rdo.DepartmentGroups.OrderBy(o => o.REQUEST_DEPARTMENT_NAME).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void ProcessSingleKey()
        {
            try
            {
                if (rdo.departmentTrans != null && rdo.departmentTrans.Count > 0)
                {
                    //SetSingleKey(new KeyValue(Mps000102ExtendSingleKey.OPEN_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.departmentTrans[0].)));//TODO
                    SetSingleKey(new KeyValue(Mps000375ExtendSingleKey.OPEN_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.departmentTrans[0].DEPARTMENT_IN_TIME ?? 0)));
                    if (rdo.departmentTrans[rdo.departmentTrans.Count - 1] != null && rdo.departmentTrans.Count > 1)
                    {
                        //SetSingleKey(new KeyValue(Mps000102ExtendSingleKey.CLOSE_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.departmentTrans[rdo.departmentTrans.Count - 1].LOG_TIME))); //TODO
                        //SetSingleKey(new KeyValue(Mps000102ExtendSingleKey.CLOSE_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.departmentTrans[rdo.departmentTrans.Count - 1].))); //TODO

                    }
                }
                //Số ngày điều trị
                SetSingleKey(new KeyValue(Mps000375ExtendSingleKey.TOTAL_DAY, rdo.totalDay));
                if (this.rdo.hisServiceReq != null)
                {
                    SetSingleKey(new KeyValue(Mps000375ExtendSingleKey.FIRST_EXAM_ROOM_NAME, this.rdo.hisServiceReq.EXECUTE_ROOM_NAME));
                }

                SetSingleKey(new KeyValue(Mps000375ExtendSingleKey.RATIO, this.rdo.ratio));

                if (rdo.departmentName != null)
                {
                    SetSingleKey(new KeyValue(Mps000375ExtendSingleKey.DEPARTMENT_NAME, rdo.departmentName));
                }

                if (rdo.patyAlterBhytADO != null)
                {
                    if (rdo.patyAlterBhytADO.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE)
                    {
                        if (rdo.patyAlterBhytADO.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.EMERGENCY)// la dung tuyen cap cuu
                        {
                            SetSingleKey(new KeyValue(Mps000375ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, "X"));
                            SetSingleKey(new KeyValue(Mps000375ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                            SetSingleKey(new KeyValue(Mps000375ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                        else if (rdo.patyAlterBhytADO.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.PRESENT)// la dung tuyen: gioi thieu,
                        {
                            SetSingleKey(new KeyValue(Mps000375ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            SetSingleKey(new KeyValue(Mps000375ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                            SetSingleKey(new KeyValue(Mps000375ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                        else
                        {
                            SetSingleKey(new KeyValue(Mps000375ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            SetSingleKey(new KeyValue(Mps000375ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                            SetSingleKey(new KeyValue(Mps000375ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                    }
                    else if (rdo.patyAlterBhytADO.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.FALSE)//trai tuyen
                    {
                        SetSingleKey(new KeyValue(Mps000375ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                        SetSingleKey(new KeyValue(Mps000375ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                        SetSingleKey(new KeyValue(Mps000375ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, "X"));
                    }

                    if (!String.IsNullOrEmpty(rdo.patyAlterBhytADO.HEIN_CARD_NUMBER))
                    {
                        SetSingleKey((new KeyValue(Mps000375ExtendSingleKey.HEIN_CARD_NUMBER_SEPARATE, SetHeinCardNumberDisplayByNumber(rdo.patyAlterBhytADO.HEIN_CARD_NUMBER))));
                        SetSingleKey((new KeyValue(Mps000375ExtendSingleKey.IS_HEIN, "X")));
                        SetSingleKey((new KeyValue(Mps000375ExtendSingleKey.IS_VIENPHI, "")));
                        SetSingleKey((new KeyValue(Mps000375ExtendSingleKey.HEIN_CARD_NUMBER_1, rdo.patyAlterBhytADO.HEIN_CARD_NUMBER.Substring(0, 2))));
                        SetSingleKey((new KeyValue(Mps000375ExtendSingleKey.HEIN_CARD_NUMBER_2, rdo.patyAlterBhytADO.HEIN_CARD_NUMBER.Substring(2, 1))));
                        SetSingleKey((new KeyValue(Mps000375ExtendSingleKey.HEIN_CARD_NUMBER_3, rdo.patyAlterBhytADO.HEIN_CARD_NUMBER.Substring(3, 2))));
                        SetSingleKey((new KeyValue(Mps000375ExtendSingleKey.HEIN_CARD_NUMBER_4, rdo.patyAlterBhytADO.HEIN_CARD_NUMBER.Substring(5, 2))));
                        SetSingleKey((new KeyValue(Mps000375ExtendSingleKey.HEIN_CARD_NUMBER_5, rdo.patyAlterBhytADO.HEIN_CARD_NUMBER.Substring(7, 3))));
                        SetSingleKey((new KeyValue(Mps000375ExtendSingleKey.HEIN_CARD_NUMBER_6, rdo.patyAlterBhytADO.HEIN_CARD_NUMBER.Substring(10, 5))));
                        SetSingleKey(new KeyValue(Mps000375ExtendSingleKey.STR_HEIN_CARD_FROM_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.patyAlterBhytADO.HEIN_CARD_FROM_TIME ?? 0)));
                        SetSingleKey(new KeyValue(Mps000375ExtendSingleKey.STR_HEIN_CARD_TO_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.patyAlterBhytADO.HEIN_CARD_TO_TIME ?? 0)));
                        SetSingleKey((new KeyValue(Mps000375ExtendSingleKey.HEIN_MEDI_ORG_CODE, rdo.patyAlterBhytADO.HEIN_MEDI_ORG_CODE)));
                        SetSingleKey((new KeyValue(Mps000375ExtendSingleKey.HEIN_MEDI_ORG_NAME, rdo.patyAlterBhytADO.HEIN_MEDI_ORG_NAME)));
                    }
                    else
                    {
                        SetSingleKey((new KeyValue(Mps000375ExtendSingleKey.IS_HEIN, "")));
                        SetSingleKey((new KeyValue(Mps000375ExtendSingleKey.IS_VIENPHI, "X")));
                    }
                }

                decimal thanhtien_tong = 0;
                decimal bhytthanhtoan_tong = 0;
                decimal nguonkhac_tong = 0;
                decimal bnthanhtoan_tong = 0;

                decimal thanhtien_tong_vttt = 0;
                decimal bhytthanhtoan_tong_vttt = 0;
                decimal bnthanhtoan_tong_vttt = 0;

                decimal thanhtien_tong_dvc = 0;
                decimal bhytthanhtoan_tong_dvc = 0;
                decimal bnthanhtoan_tong_dvc = 0;



                if (rdo.sereServNotHiTechs != null && rdo.sereServNotHiTechs.Count > 0)
                {
                    if (rdo.sereServVTTTs != null && rdo.sereServVTTTs.Count > 0)
                    {
                        thanhtien_tong_vttt = rdo.sereServVTTTs.Sum(o => (o.VIR_TOTAL_PRICE)) ?? 0;
                        bhytthanhtoan_tong_vttt = rdo.sereServVTTTs.Sum(o => (o.VIR_TOTAL_HEIN_PRICE)) ?? 0;
                        bnthanhtoan_tong_vttt = rdo.sereServVTTTs.Sum(o => (o.VIR_TOTAL_PATIENT_PRICE)) ?? 0;
                    }
                    if (rdo.sereServHitechs != null && rdo.sereServHitechs.Count > 0)
                    {
                        thanhtien_tong_dvc = rdo.sereServHitechs.Sum(o => (o.VIR_TOTAL_PRICE)) ?? 0;
                        bhytthanhtoan_tong_dvc = rdo.sereServHitechs.Sum(o => (o.VIR_TOTAL_HEIN_PRICE)) ?? 0;
                        bnthanhtoan_tong_dvc = rdo.sereServHitechs.Sum(o => (o.VIR_TOTAL_PATIENT_PRICE)) ?? 0;
                    }

                    thanhtien_tong = (thanhtien_tong_dvc + thanhtien_tong_vttt + rdo.sereServNotHiTechs.Sum(o => (o.VIR_TOTAL_PRICE))) ?? 0;
                    bhytthanhtoan_tong = (bhytthanhtoan_tong_dvc + bhytthanhtoan_tong_vttt + rdo.sereServNotHiTechs.Sum(o => (o.VIR_TOTAL_HEIN_PRICE))) ?? 0;
                    bnthanhtoan_tong = (bnthanhtoan_tong_dvc + bnthanhtoan_tong_vttt + rdo.sereServNotHiTechs.Sum(o => (o.VIR_TOTAL_PATIENT_PRICE))) ?? 0;
                    nguonkhac_tong = 0;
                }

                SetSingleKey(new KeyValue(Mps000375ExtendSingleKey.VIR_TOTAL_PRICE_HEIN_LIMIT, rdo.thuChenhLech));
                SetSingleKey(new KeyValue(Mps000375ExtendSingleKey.VIR_DEPOSIT_AMOUNT, rdo.thuDongChiTra));

                SetSingleKey(new KeyValue(Mps000375ExtendSingleKey.TOTAL_PRICE, thanhtien_tong));
                SetSingleKey(new KeyValue(Mps000375ExtendSingleKey.TOTAL_PRICE_HEIN, bhytthanhtoan_tong));
                SetSingleKey(new KeyValue(Mps000375ExtendSingleKey.TOTAL_PRICE_PATIENT, bnthanhtoan_tong));
                SetSingleKey(new KeyValue(Mps000375ExtendSingleKey.TOTAL_PRICE_OTHER, Inventec.Common.Number.Convert.NumberToStringRoundAuto(nguonkhac_tong, 0)));
                SetSingleKey(new KeyValue(Mps000375ExtendSingleKey.TOTAL_PRICE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(thanhtien_tong).ToString())));
                SetSingleKey(new KeyValue(Mps000375ExtendSingleKey.TOTAL_PRICE_HEIN_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bhytthanhtoan_tong).ToString())));
                SetSingleKey(new KeyValue(Mps000375ExtendSingleKey.TOTAL_PRICE_PATIENT_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bnthanhtoan_tong).ToString())));
                SetSingleKey(new KeyValue(Mps000375ExtendSingleKey.TOTAL_PRICE_OTHER_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(nguonkhac_tong).ToString())));

                SetSingleKey(new KeyValue(Mps000375ExtendSingleKey.VIR_TOTAL_PRICE_HEIN_LIMIT_TO_VN, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(rdo.thuChenhLech).ToString())));
                SetSingleKey(new KeyValue(Mps000375ExtendSingleKey.VIR_DEPOSIT_AMOUNT_TO_VN, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(rdo.thuDongChiTra).ToString())));
                SetSingleKey(new KeyValue(Mps000375ExtendSingleKey.NUM_ORDER_TRANSACTION, this.rdo.deposit.NUM_ORDER));

                AddObjectKeyIntoListkey<V_HIS_TRANSACTION>(this.rdo.deposit, false);
                AddObjectKeyIntoListkey<V_HIS_PATIENT_TYPE_ALTER>(rdo.patyAlterBhytADO, false);
                AddObjectKeyIntoListkey<V_HIS_TREATMENT_FEE>(rdo.currentHisTreatment, false);
                AddObjectKeyIntoListkey<PatientADO>(rdo.patientADO);
                AddObjectKeyIntoListkey<V_HIS_SERVICE_REQ>(rdo.hisServiceReq, false);
                SetSingleKey(new KeyValue(Mps000375ExtendSingleKey.HEIN_CARD_NUMBER, rdo.currentHisTreatment.TDL_HEIN_CARD_NUMBER));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessTotalPriceGroup()
        {
            try
            {
                decimal totalPriceHightTech = 0;
                decimal totalPriceNotHightTech = 0;
                decimal totalPriceVTTT = 0;

                foreach (var ServiceGroup in rdo.ServiceGroups)
                {
                    totalPriceHightTech = rdo.sereServHitechs.Where(o => o.TDL_HEIN_SERVICE_TYPE_ID == ServiceGroup.TDL_HEIN_SERVICE_TYPE_ID).Sum(o => o.VIR_TOTAL_PRICE) ?? 0;
                    totalPriceVTTT = rdo.sereServVTTTs.Where(o => o.SERE_SERV__GROUP_SERVICE_REPORT == ServiceGroup.TDL_HEIN_SERVICE_TYPE_ID).Sum(o => o.VIR_TOTAL_PRICE) ?? 0;
                    totalPriceNotHightTech = rdo.sereServNotHiTechs.Where(o => o.TDL_HEIN_SERVICE_TYPE_ID == ServiceGroup.TDL_HEIN_SERVICE_TYPE_ID).Sum(o => o.VIR_TOTAL_PRICE) ?? 0;
                    ServiceGroup.TOTAL_PRICE_SERVICE_GROUP = totalPriceHightTech + totalPriceVTTT + totalPriceNotHightTech;

                    totalPriceHightTech = rdo.sereServHitechs.Where(o => o.TDL_HEIN_SERVICE_TYPE_ID == ServiceGroup.TDL_HEIN_SERVICE_TYPE_ID).Sum(o => o.VIR_TOTAL_HEIN_PRICE) ?? 0;
                    totalPriceVTTT = rdo.sereServVTTTs.Where(o => o.SERE_SERV__GROUP_SERVICE_REPORT == ServiceGroup.TDL_HEIN_SERVICE_TYPE_ID).Sum(o => o.VIR_TOTAL_HEIN_PRICE) ?? 0;
                    totalPriceNotHightTech = rdo.sereServNotHiTechs.Where(o => o.TDL_HEIN_SERVICE_TYPE_ID == ServiceGroup.TDL_HEIN_SERVICE_TYPE_ID).Sum(o => o.VIR_TOTAL_HEIN_PRICE) ?? 0;

                    ServiceGroup.TOTAL_HEIN_PRICE_SERVICE_GROUP = totalPriceHightTech + totalPriceVTTT + totalPriceNotHightTech;

                    totalPriceHightTech = rdo.sereServHitechs.Where(o => o.TDL_HEIN_SERVICE_TYPE_ID == ServiceGroup.TDL_HEIN_SERVICE_TYPE_ID).Sum(o => o.VIR_TOTAL_PATIENT_PRICE) ?? 0;
                    totalPriceVTTT = rdo.sereServVTTTs.Where(o => o.SERE_SERV__GROUP_SERVICE_REPORT == ServiceGroup.TDL_HEIN_SERVICE_TYPE_ID).Sum(o => o.VIR_TOTAL_PATIENT_PRICE) ?? 0;
                    totalPriceNotHightTech = rdo.sereServNotHiTechs.Where(o => o.TDL_HEIN_SERVICE_TYPE_ID == ServiceGroup.TDL_HEIN_SERVICE_TYPE_ID).Sum(o => o.VIR_TOTAL_PATIENT_PRICE) ?? 0;

                    ServiceGroup.TOTAL_PATIENT_PRICE_SERVICE_GROUP = totalPriceHightTech + totalPriceVTTT + totalPriceNotHightTech;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessGroupSereServ()
        {
            try
            {
                rdo.ServiceGroups = new List<SereServGroupPlusADO>();
                rdo.DepartmentGroups = new List<SereServGroupPlusADO>();
                rdo.HightTechDepartmentGroups = new List<SereServGroupPlusADO>();
                rdo.ServiceVTTTDepartmentGroup = new List<SereServGroupPlusADO>();

                //Group Service Not Hitech
                var sServceReportGroups = rdo.sereServNotHiTechs.GroupBy(o => o.TDL_HEIN_SERVICE_TYPE_ID).ToList();
                foreach (var sServceReportGroup in sServceReportGroups)
                {
                    List<SereServGroupPlusADO> subSServiceReportGroup = sServceReportGroup.ToList<SereServGroupPlusADO>();

                    SereServGroupPlusADO itemSServiceReportGroup = subSServiceReportGroup.First();
                    if (itemSServiceReportGroup.TDL_HEIN_SERVICE_TYPE_ID != null)
                        itemSServiceReportGroup.HEIN_SERVICE_TYPE_NAME = rdo.ServiceReports.Where(o => o.ID == subSServiceReportGroup.First().TDL_HEIN_SERVICE_TYPE_ID).SingleOrDefault().HEIN_SERVICE_TYPE_NAME;
                    else
                        itemSServiceReportGroup.HEIN_SERVICE_TYPE_NAME = "Khác";

                    rdo.ServiceGroups.Add(itemSServiceReportGroup);

                    //Nhom Department
                    var sDepartmentGroups = subSServiceReportGroup.OrderBy(o => o.TDL_REQUEST_DEPARTMENT_ID).GroupBy(o => o.TDL_REQUEST_DEPARTMENT_ID).ToList();
                    foreach (var sDepartmentGroup in sDepartmentGroups)
                    {
                        List<SereServGroupPlusADO> subSDepartmentGroups = sDepartmentGroup.ToList<SereServGroupPlusADO>();
                        SereServGroupPlusADO itemSDepartmentGroup = subSDepartmentGroups.First();

                        itemSDepartmentGroup.TOTAL_PRICE_DEPARTMENT_GROUP = sDepartmentGroup.Where(o => o.TDL_REQUEST_DEPARTMENT_ID == itemSDepartmentGroup.TDL_REQUEST_DEPARTMENT_ID).Sum(o => o.VIR_TOTAL_PRICE) ?? 0;
                        itemSDepartmentGroup.TOTAL_PATIENT_PRICE_DEPARTMENT_GROUP = sDepartmentGroup.Where(o => o.TDL_REQUEST_DEPARTMENT_ID == itemSDepartmentGroup.TDL_REQUEST_DEPARTMENT_ID).Sum(o => o.VIR_TOTAL_PATIENT_PRICE) ?? 0;
                        itemSDepartmentGroup.TOTAL_HEIN_PRICE_DEPARTMENT_GROUP = sDepartmentGroup.Where(o => o.TDL_REQUEST_DEPARTMENT_ID == itemSDepartmentGroup.TDL_REQUEST_DEPARTMENT_ID).Sum(o => o.VIR_TOTAL_HEIN_PRICE) ?? 0;

                        rdo.DepartmentGroups.Add(itemSDepartmentGroup);
                    }
                }

                //Group ServiceVTTT Department
                var ServiceVTTTDepartmentGroups = rdo.sereServVTTTs.OrderBy(o => o.TDL_REQUEST_DEPARTMENT_ID).GroupBy(o => o.TDL_REQUEST_DEPARTMENT_ID).ToList();
                foreach (var ServiceVTTTDepartment in ServiceVTTTDepartmentGroups)
                {
                    List<SereServGroupPlusADO> subServiceVTTTDepartmentGroups = ServiceVTTTDepartment.ToList<SereServGroupPlusADO>();
                    SereServGroupPlusADO itemServiceVTTTDepartmentGroup = subServiceVTTTDepartmentGroups.First();
                    itemServiceVTTTDepartmentGroup.DEPARTMENT__GROUP_SERVICE_REPORT = rdo.sereServHitechs.First().TDL_HEIN_SERVICE_TYPE_ID ?? 0;

                    itemServiceVTTTDepartmentGroup.TOTAL_PRICE_DEPARTMENT_GROUP = rdo.sereServVTTTs.Where(o => o.TDL_REQUEST_DEPARTMENT_ID == itemServiceVTTTDepartmentGroup.TDL_REQUEST_DEPARTMENT_ID).Sum(o => o.VIR_TOTAL_PRICE) ?? 0;
                    itemServiceVTTTDepartmentGroup.TOTAL_PATIENT_PRICE_DEPARTMENT_GROUP = rdo.sereServVTTTs.Where(o => o.TDL_REQUEST_DEPARTMENT_ID == itemServiceVTTTDepartmentGroup.TDL_REQUEST_DEPARTMENT_ID).Sum(o => o.VIR_TOTAL_PATIENT_PRICE) ?? 0;
                    itemServiceVTTTDepartmentGroup.TOTAL_HEIN_PRICE_DEPARTMENT_GROUP = rdo.sereServVTTTs.Where(o => o.TDL_REQUEST_DEPARTMENT_ID == itemServiceVTTTDepartmentGroup.TDL_REQUEST_DEPARTMENT_ID).Sum(o => o.VIR_TOTAL_HEIN_PRICE) ?? 0;

                    foreach (var sereServVTTT in rdo.sereServVTTTs)
                    {
                        sereServVTTT.SERE_SERV__GROUP_SERVICE_REPORT = rdo.sereServHitechs.First().TDL_HEIN_SERVICE_TYPE_ID ?? 0;
                    }

                    rdo.ServiceVTTTDepartmentGroup.Add(itemServiceVTTTDepartmentGroup);
                }

                //Group HightTech Department
                var sHightTechDepartmentGroups = rdo.sereServHitechs.OrderBy(o => o.TDL_EXECUTE_DEPARTMENT_ID).GroupBy(o => o.TDL_EXECUTE_DEPARTMENT_ID).ToList();
                foreach (var sHightTechDepartmentGroup in sHightTechDepartmentGroups)
                {
                    List<SereServGroupPlusADO> subHightTechDepartmentGroups = sHightTechDepartmentGroup.ToList<SereServGroupPlusADO>();
                    SereServGroupPlusADO itemHightTechDepartmentGroup = subHightTechDepartmentGroups.First();
                    rdo.HightTechDepartmentGroups.Add(itemHightTechDepartmentGroup);

                    itemHightTechDepartmentGroup.TOTAL_PRICE_DEPARTMENT_GROUP = sHightTechDepartmentGroup.Where(o => o.TDL_EXECUTE_DEPARTMENT_ID == itemHightTechDepartmentGroup.TDL_EXECUTE_DEPARTMENT_ID).Sum(o => o.VIR_TOTAL_PRICE) ?? 0;
                    itemHightTechDepartmentGroup.TOTAL_PATIENT_PRICE_DEPARTMENT_GROUP = sHightTechDepartmentGroup.Where(o => o.TDL_EXECUTE_DEPARTMENT_ID == itemHightTechDepartmentGroup.TDL_EXECUTE_DEPARTMENT_ID).Sum(o => o.VIR_TOTAL_PATIENT_PRICE) ?? 0;
                    itemHightTechDepartmentGroup.TOTAL_HEIN_PRICE_DEPARTMENT_GROUP = sHightTechDepartmentGroup.Where(o => o.TDL_EXECUTE_DEPARTMENT_ID == itemHightTechDepartmentGroup.TDL_EXECUTE_DEPARTMENT_ID).Sum(o => o.VIR_TOTAL_HEIN_PRICE) ?? 0;


                    var checkExitServiceGroup = rdo.ServiceGroups.Where(o => o.TDL_EXECUTE_DEPARTMENT_ID == itemHightTechDepartmentGroup.TDL_EXECUTE_DEPARTMENT_ID).ToList();
                    if (checkExitServiceGroup.Count == 0)
                    {
                        itemHightTechDepartmentGroup.HEIN_SERVICE_TYPE_NAME = rdo.ServiceReports.Where(o => o.ID == subHightTechDepartmentGroups.First().TDL_HEIN_SERVICE_TYPE_ID).SingleOrDefault().HEIN_SERVICE_TYPE_NAME;
                        rdo.ServiceGroups.Add(itemHightTechDepartmentGroup);
                    }
                }

                ////Sắp xếp lại nhóm Service Group
                //rdo.ServiceGroups = rdo.ServiceGroups.OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 9999).ToList();

            }

            //}
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        string SetHeinCardNumberDisplayByNumber(string heinCardNumber)
        {
            string result = "";
            try
            {
                if (!String.IsNullOrWhiteSpace(heinCardNumber) && heinCardNumber.Length == 15)
                {
                    string separateSymbol = "-";
                    result = new StringBuilder().Append(heinCardNumber.Substring(0, 2)).Append(separateSymbol).Append(heinCardNumber.Substring(2, 1)).Append(separateSymbol).Append(heinCardNumber.Substring(3, 2)).Append(separateSymbol).Append(heinCardNumber.Substring(5, 2)).Append(separateSymbol).Append(heinCardNumber.Substring(7, 3)).Append(separateSymbol).Append(heinCardNumber.Substring(10, 5)).ToString();
                }
                else
                {
                    result = heinCardNumber;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = heinCardNumber;
            }
            return result;
        }

        string TrimHeinCardNumber(string chucodau)
        {
            string result = "";
            try
            {
                result = System.Text.RegularExpressions.Regex.Replace(chucodau, @"[-,_ ]|[_]{2}|[_]{3}|[_]{4}|[_]{5}", "").ToUpper();
            }
            catch (Exception ex)
            {

            }

            return result;
        }

        public override string ProcessPrintLogData()
        {
            string log = "";
            try
            {
                log = LogDataTransaction(rdo.currentHisTreatment.TREATMENT_CODE, rdo.deposit.TRANSACTION_CODE, "");
                log += "thuChenhLech: " + rdo.thuChenhLech + " thuDongChiTra: " + rdo.thuDongChiTra;
            }
            catch (Exception ex)
            {
                log = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return log;
        }

        public override string ProcessUniqueCodeData()
        {
            string result = "";
            try
            {
                if (rdo != null && rdo.deposit != null)
                    result = String.Format("{0}_{1}_{2}_{3}", this.printTypeCode, rdo.deposit.TREATMENT_CODE, rdo.deposit.TRANSACTION_CODE, rdo.deposit.ACCOUNT_BOOK_CODE);
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
