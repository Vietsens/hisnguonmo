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
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using MPS.ProcessorBase.Core;
using MPS.Processor.Mps000037.PDO;
using Inventec.Core;
using MPS.ProcessorBase;
using MOS.EFMODEL.DataModels;
using System.Text;
using System.Linq;
using Inventec.Common.QRCoder;

namespace MPS.Processor.Mps000037
{
    class Mps000037Processor : AbstractProcessor
    {
        Mps000037PDO rdo;
        const string config = "_Bordereau";
        bool isBordereau = false;
        List<SereServGroupPlusADO> ListAdo = new List<SereServGroupPlusADO>();

        public Mps000037Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            isBordereau = printData.fileName.Contains(config);
            rdo = (Mps000037PDO)rdoBase;
        }

        public void SetBarcodeKey()
        {
            try
            {
                if (rdo != null)
                {
                    if (rdo.lstServiceReq != null)
                    {
                        if (!String.IsNullOrWhiteSpace(rdo.lstServiceReq.TDL_PATIENT_CODE))
                        {
                            Inventec.Common.BarcodeLib.Barcode barcodePatientCode = new Inventec.Common.BarcodeLib.Barcode(rdo.lstServiceReq.TDL_PATIENT_CODE);
                            barcodePatientCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                            barcodePatientCode.IncludeLabel = false;
                            barcodePatientCode.Width = 120;
                            barcodePatientCode.Height = 40;
                            barcodePatientCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                            barcodePatientCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                            barcodePatientCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                            barcodePatientCode.IncludeLabel = true;

                            dicImage.Add(Mps000037ExtendSingleKey.PATIENT_CODE_BAR, barcodePatientCode);
                        }

                        //if (!String.IsNullOrWhiteSpace(rdo.lstServiceReq.SERVICE_REQ_CODE))
                        //{
                        //    Inventec.Common.BarcodeLib.Barcode barcodeServiceReq = new Inventec.Common.BarcodeLib.Barcode(rdo.lstServiceReq.SERVICE_REQ_CODE);
                        //    barcodeServiceReq.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                        //    barcodeServiceReq.IncludeLabel = false;
                        //    barcodeServiceReq.Width = 120;
                        //    barcodeServiceReq.Height = 40;
                        //    barcodeServiceReq.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                        //    barcodeServiceReq.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                        //    barcodeServiceReq.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                        //    barcodeServiceReq.IncludeLabel = true;

                        //    dicImage.Add(Mps000037ExtendSingleKey.BARCODE_SERVICE_REQ_CODE, barcodeServiceReq);
                        //}
                    }
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
                SetBarcodeKey();
                SetSingleKey();
                ProcessListData();
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);

                var ExecuteRoomGroups = new List<SereServGroupPlusADO>();

                // minhnq
                if (rdo.ListServiceReqPrint != null && rdo.ListServiceReqPrint.Count() > 0 && ListAdo != null && ListAdo.Count() > 0)
                {
                    foreach (var item in ListAdo)
                    {
                        var serviceReq = rdo.ListServiceReqPrint.Where(o => o.ID == item.SERVICE_REQ_ID).First();
                        item.NUM_ORDER = serviceReq.NUM_ORDER;
                    }
                }

                if (isBordereau)
                {
                    var sExecuteRoomGroups = (ListAdo != null && ListAdo.Count > 0) ? ListAdo.GroupBy(o => o.HEIN_SERVICE_TYPE_NAME).ToList() : null;
                    foreach (var sExecuteRoomGroup in sExecuteRoomGroups)
                    {
                        SereServGroupPlusADO itemSExecuteGroup = sExecuteRoomGroup.First();
                        ExecuteRoomGroups.Add(itemSExecuteGroup);
                    }

                    ExecuteRoomGroups = (ExecuteRoomGroups != null && ExecuteRoomGroups.Count > 0) ? ExecuteRoomGroups.OrderBy(o => o.EXECUTE_ROOM_NAME).ThenBy(p => p.EXECUTE_DEPARTMENT_NAME).ToList() : ExecuteRoomGroups;
                    var grType = ListAdo.GroupBy(o => o.TDL_SERVICE_TYPE_ID).ToList();

                    List<HIS_SERVICE_REQ_TYPE> srTypeList = new List<HIS_SERVICE_REQ_TYPE>();
                    List<V_HIS_SERVICE_REQ> srList = new List<V_HIS_SERVICE_REQ>();
                    foreach (var item in grType)
                    {
                        //Tạo key ServiceReqType và ServiceReq
                        if (rdo.ListServiceReqType != null && rdo.ListServiceReqType.Count > 0)
                            srTypeList.Add(rdo.ListServiceReqType.FirstOrDefault(o => o.ID == item.Key));

                        var grServiceReq = item.GroupBy(o => o.SERVICE_REQ_ID).ToList();
                        if (rdo.ListServiceReqPrint != null && rdo.ListServiceReqPrint.Count > 0 && rdo.ListServiceReqPrint.Exists(o => grServiceReq.Exists(p => p.Key == o.ID)))
                        {
                            srList.AddRange(rdo.ListServiceReqPrint.Where(o => grServiceReq.Exists(p => p.Key == o.ID)));
                        }
                    }
                    objectTag.AddObjectData(store, "ServiceReqType", srTypeList.Count > 0 ? srTypeList.OrderBy(o => o.SERVICE_REQ_TYPE_NAME).ToList() : new List<HIS_SERVICE_REQ_TYPE>());
                    objectTag.AddObjectData(store, "ServiceReq", srList.Count > 0 ? srList.OrderBy(o => o.SERVICE_REQ_TYPE_NAME).ThenBy(o => o.INTRUCTION_TIME).ToList() : new List<V_HIS_SERVICE_REQ>());
                    objectTag.AddObjectData(store, "Services", ListAdo);
                    objectTag.AddObjectData(store, "ExecuteRoomGroups", ExecuteRoomGroups);
                    objectTag.AddRelationship(store, "ExecuteRoomGroups", "Services", "HEIN_SERVICE_TYPE_NAME", "HEIN_SERVICE_TYPE_NAME");
                }
                else
                {
                    var sExecuteRoomGroups = ListAdo.GroupBy(o => o.TDL_EXECUTE_ROOM_ID).ToList();
                    foreach (var sExecuteRoomGroup in sExecuteRoomGroups)
                    {
                        SereServGroupPlusADO itemSExecuteGroup = sExecuteRoomGroup.First();
                        ExecuteRoomGroups.Add(itemSExecuteGroup);
                    }

                    var serviceTypeGroup = new List<SereServGroupPlusADO>();
                    var servcieParent = new List<V_HIS_SERVICE>();
                    var grType = ListAdo.GroupBy(o => o.TDL_SERVICE_TYPE_ID).ToList();
                    List<HIS_SERVICE_REQ_TYPE> srTypeList = new List<HIS_SERVICE_REQ_TYPE>();
                    List<V_HIS_SERVICE_REQ> srList = new List<V_HIS_SERVICE_REQ>();
                    foreach (var item in grType)
                    {
                        //Tạo key ServiceReqType và ServiceReq
                        if (rdo.ListServiceReqType != null && rdo.ListServiceReqType.Count > 0)
                            srTypeList.Add(rdo.ListServiceReqType.FirstOrDefault(o => o.ID == item.Key));

                        var grServiceReq = item.GroupBy(o => o.SERVICE_REQ_ID).ToList();
                        if (rdo.ListServiceReqPrint != null && rdo.ListServiceReqPrint.Count > 0 && rdo.ListServiceReqPrint.Exists(o => grServiceReq.Exists(p => p.Key == o.ID)))
                        {
                            srList.AddRange(rdo.ListServiceReqPrint.Where(o => grServiceReq.Exists(p => p.Key == o.ID)));
                        }
                        //
                        serviceTypeGroup.Add(item.First());

                        var grParent = item.GroupBy(o => o.SERVICE_PARENT_ID).ToList();
                        foreach (var gr in grParent)
                        {
                            if (rdo.ListService != null && rdo.ListService.Count > 0)
                            {
                                var parent = rdo.ListService.FirstOrDefault(o => o.ID == gr.First().SERVICE_PARENT_ID);
                                if (parent != null)
                                {
                                    servcieParent.Add(parent);
                                }
                                else
                                {
                                    servcieParent.Add(new V_HIS_SERVICE() { SERVICE_TYPE_ID = gr.First().TDL_SERVICE_TYPE_ID, SERVICE_NAME = gr.First().SERVICE_TYPE_NAME });
                                }
                            }
                            else
                            {
                                servcieParent.Add(new V_HIS_SERVICE() { SERVICE_TYPE_ID = gr.First().TDL_SERVICE_TYPE_ID, SERVICE_NAME = gr.First().SERVICE_TYPE_NAME });
                            }
                        }
                    }

                    ListAdo = ListAdo.OrderBy(o => o.ID).ThenBy(p => p.SERVICE_NAME).ToList();
                    objectTag.AddObjectData(store, "ServiceReqType", srTypeList.Count > 0 ? srTypeList.OrderBy(o => o.SERVICE_REQ_TYPE_NAME).ToList() : new List<HIS_SERVICE_REQ_TYPE>());
                    objectTag.AddObjectData(store, "ServiceReq", srList.Count > 0 ? srList.OrderBy(o => o.SERVICE_REQ_TYPE_NAME).ThenBy(o => o.INTRUCTION_TIME).ToList() : new List<V_HIS_SERVICE_REQ>());
                    var Rooms = ListAdo.Select(o => new {o.SERVICE_REQ_ID, o.TDL_EXECUTE_ROOM_ID, o.TDL_SERVICE_TYPE_ID,o.SERVICE_PARENT_ID,o.REQUEST_ROOM_NAME }).ToList();
                    objectTag.AddObjectData(store, "Services", ListAdo);
                    objectTag.AddRelationship(store, "ServiceReqType", "ServiceReq", "ID", "SERVICE_REQ_TYPE_ID");
                    objectTag.AddRelationship(store, "ServiceReq", "Services", "ID", "SERVICE_REQ_ID");
                    objectTag.AddObjectData(store, "ExecuteRoomGroups", ExecuteRoomGroups);
                    objectTag.AddRelationship(store, "ExecuteRoomGroups", "Services", "TDL_EXECUTE_ROOM_ID", "TDL_EXECUTE_ROOM_ID");

                    objectTag.AddObjectData(store, "ServicesType", serviceTypeGroup);
                    objectTag.AddObjectData(store, "ServicesParent", servcieParent);
                    objectTag.AddObjectData(store, "ServicesRoom", Rooms);
                    objectTag.AddRelationship(store, "ServicesType", "ServicesParent", "TDL_SERVICE_TYPE_ID", "SERVICE_TYPE_ID");
                    objectTag.AddRelationship(store, "ServicesType", "Services", "TDL_SERVICE_TYPE_ID", "TDL_SERVICE_TYPE_ID");
                    objectTag.AddRelationship(store, "ServicesType", "ServicesRoom", "TDL_SERVICE_TYPE_ID", "TDL_SERVICE_TYPE_ID");
                    objectTag.AddRelationship(store, "ServicesParent", "Services", "ID", "SERVICE_PARENT_ID");
                    objectTag.AddRelationship(store, "ServicesParent", "ServicesRoom", "ID", "SERVICE_PARENT_ID");
                    objectTag.AddRelationship(store, "ServicesRoom", "Services", "REQUEST_ROOM_NAME", "REQUEST_ROOM_NAME");

                    objectTag.SetUserFunction(store, "FuncSameTitleCol", new CustomerFuncMergeSameData());
                }

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void SetSingleKey()
        {
            try
            {
                decimal thanhtien_tong = 0, bhytthanhtoan_tong = 0, nguonkhac_tong = 0, bnthanhtoan_tong = 0;
                for (int i = 0; i < rdo.SereServs_All.Count; i++)
                {
                    thanhtien_tong += rdo.SereServs_All[i].VIR_TOTAL_PRICE ?? 0;
                    bhytthanhtoan_tong += rdo.SereServs_All[i].VIR_TOTAL_HEIN_PRICE ?? 0;
                    bnthanhtoan_tong += rdo.SereServs_All[i].VIR_TOTAL_PATIENT_PRICE ?? 0;
                }

                SetSingleKey(new KeyValue(Mps000037ExtendSingleKey.TOTAL_PRICE, Inventec.Common.Number.Convert.NumberToString(thanhtien_tong, 0)));
                SetSingleKey(new KeyValue(Mps000037ExtendSingleKey.TOTAL_PRICE_HEIN, Inventec.Common.Number.Convert.NumberToString(bhytthanhtoan_tong, 0)));
                SetSingleKey(new KeyValue(Mps000037ExtendSingleKey.TOTAL_PRICE_PATIENT, Inventec.Common.Number.Convert.NumberToString(bnthanhtoan_tong, 0)));
                SetSingleKey(new KeyValue(Mps000037ExtendSingleKey.TOTAL_PRICE_OTHER, Inventec.Common.Number.Convert.NumberToString(nguonkhac_tong, 0)));
                SetSingleKey(new KeyValue(Mps000037ExtendSingleKey.TOTAL_PRICE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(thanhtien_tong).ToString())));

                if (rdo.Mps000037ADO != null)
                {
                    SetSingleKey(new KeyValue(Mps000037ExtendSingleKey.BED_ROOM_NAME, rdo.Mps000037ADO.bebRoomName));
                    SetSingleKey(new KeyValue(Mps000037ExtendSingleKey.DEPARTMENT_NAME, rdo.Mps000037ADO.departmentName));
                    SetSingleKey(new KeyValue(Mps000037ExtendSingleKey.RATIO, rdo.Mps000037ADO.ratio));
                    SetSingleKey(new KeyValue(Mps000037ExtendSingleKey.RATIO_STR, (rdo.Mps000037ADO.ratio * 100) + "%"));
                    AddObjectKeyIntoListkey(rdo.Mps000037ADO, false);
                }

                //if (rdo.hisServiceReqCombo != null)
                //{
                //    SetSingleKey(new KeyValue(Mps000037ExtendSingleKey.INSTRUCTION_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.hisServiceReqCombo.INTRUCTION_TIME)));

                //    AddObjectKeyIntoListkey<HisServiceReqCombo>(rdo.hisServiceReqCombo, false);
                //}

                if (rdo.lstServiceReq != null)
                {
                    SetSingleKey(new KeyValue(Mps000037ExtendSingleKey.INTRUCTION_TIME_FULL_STR,
                        GlobalQuery.GetCurrentTimeSeparateBeginTime(
                        Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(
                        rdo.lstServiceReq.INTRUCTION_TIME) ?? DateTime.MinValue)));

                    SetSingleKey(new KeyValue(Mps000037ExtendSingleKey.INTRUCTION_DATE_FULL_STR,
                        Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(
                        rdo.lstServiceReq.INTRUCTION_TIME)));

                    SetSingleKey(new KeyValue(Mps000037ExtendSingleKey.INSTRUCTION_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.lstServiceReq.INTRUCTION_TIME)));
                    SetSingleKey(new KeyValue(Mps000037ExtendSingleKey.AGE, Inventec.Common.DateTime.Calculation.AgeCaption(rdo.lstServiceReq.TDL_PATIENT_DOB)));
                    SetSingleKey(new KeyValue(Mps000037ExtendSingleKey.STR_YEAR, rdo.lstServiceReq.TDL_PATIENT_DOB.ToString().Substring(0, 4)));
                    SetSingleKey(new KeyValue(Mps000037ExtendSingleKey.STR_DOB, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.lstServiceReq.TDL_PATIENT_DOB)));

                    SetSingleKey(new KeyValue(Mps000037ExtendSingleKey.FINISH_TIME_STR,
                        Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.lstServiceReq.FINISH_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000037ExtendSingleKey.START_TIME_STR,
                        Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.lstServiceReq.START_TIME ?? 0)));

                    SetSingleKey(new KeyValue(Mps000037ExtendSingleKey.FINISH_TIME_FULL_STR,
                        GlobalQuery.GetCurrentTimeSeparateBeginTime(
                        Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(
                        rdo.lstServiceReq.FINISH_TIME ?? 0) ?? DateTime.MinValue)));

                    SetSingleKey(new KeyValue(Mps000037ExtendSingleKey.FINISH_DATE_FULL_STR,
                        Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(rdo.lstServiceReq.FINISH_TIME ?? 0)));

                    SetSingleKey(new KeyValue(Mps000037ExtendSingleKey.INTRUCTION_TIME_FULL_STR,
                        GlobalQuery.GetCurrentTimeSeparateBeginTime(
                        Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(
                        rdo.lstServiceReq.INTRUCTION_TIME) ?? DateTime.MinValue)));

                    SetSingleKey(new KeyValue(Mps000037ExtendSingleKey.INTRUCTION_DATE_FULL_STR,
                        Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(
                        rdo.lstServiceReq.INTRUCTION_TIME)));

                    SetSingleKey(new KeyValue(Mps000037ExtendSingleKey.REQ_ICD_CODE, rdo.lstServiceReq.ICD_CODE));
                    //SetSingleKey(new KeyValue(Mps000037ExtendSingleKey.REQ_ICD_MAIN_TEXT, rdo.lstServiceReq.ICD_MAIN_TEXT));
                    SetSingleKey(new KeyValue(Mps000037ExtendSingleKey.REQ_ICD_NAME, rdo.lstServiceReq.ICD_NAME));
                    SetSingleKey(new KeyValue(Mps000037ExtendSingleKey.REQ_ICD_SUB_CODE, rdo.lstServiceReq.ICD_SUB_CODE));
                    SetSingleKey(new KeyValue(Mps000037ExtendSingleKey.REQ_ICD_TEXT, rdo.lstServiceReq.ICD_TEXT));

                    SetSingleKey(new KeyValue(Mps000037ExtendSingleKey.DEPARTMENT_NAME, rdo.lstServiceReq.EXECUTE_DEPARTMENT_NAME));

                    if (rdo.currentHisTreatment == null) rdo.currentHisTreatment = new HIS_TREATMENT();

                    string Address = !String.IsNullOrEmpty(rdo.lstServiceReq.TDL_PATIENT_ADDRESS) ?
                        rdo.lstServiceReq.TDL_PATIENT_ADDRESS :
                        rdo.currentHisTreatment.TDL_PATIENT_ADDRESS;
                    SetSingleKey(new KeyValue(Mps000037ExtendSingleKey.ADDRESS, Address));
                    SetSingleKey(new KeyValue(Mps000037ExtendSingleKey.VIR_ADDRESS, Address));

                    string career = !String.IsNullOrEmpty(rdo.lstServiceReq.TDL_PATIENT_CAREER_NAME) ?
                        rdo.lstServiceReq.TDL_PATIENT_CAREER_NAME :
                        rdo.currentHisTreatment.TDL_PATIENT_CAREER_NAME;
                    SetSingleKey(new KeyValue(Mps000037ExtendSingleKey.CAREER_NAME, career));

                    string code = !String.IsNullOrEmpty(rdo.lstServiceReq.TDL_PATIENT_CODE) ?
                        rdo.lstServiceReq.TDL_PATIENT_CODE :
                        rdo.currentHisTreatment.TDL_PATIENT_CODE;
                    SetSingleKey(new KeyValue(Mps000037ExtendSingleKey.PATIENT_CODE, code));

                    long dob = rdo.lstServiceReq.TDL_PATIENT_DOB > 0 ?
                        rdo.lstServiceReq.TDL_PATIENT_DOB :
                        rdo.currentHisTreatment.TDL_PATIENT_DOB;
                    SetSingleKey(new KeyValue(Mps000037ExtendSingleKey.DOB, dob > 0 ? dob : 00000000000000));

                    string gender = !String.IsNullOrEmpty(rdo.lstServiceReq.TDL_PATIENT_GENDER_NAME) ?
                        rdo.lstServiceReq.TDL_PATIENT_GENDER_NAME :
                        rdo.currentHisTreatment.TDL_PATIENT_GENDER_NAME;
                    SetSingleKey(new KeyValue(Mps000037ExtendSingleKey.GENDER_NAME, gender));

                    string rank = !String.IsNullOrEmpty(rdo.lstServiceReq.TDL_PATIENT_MILITARY_RANK_NAME) ?
                        rdo.lstServiceReq.TDL_PATIENT_MILITARY_RANK_NAME :
                        rdo.currentHisTreatment.TDL_PATIENT_MILITARY_RANK_NAME;
                    SetSingleKey(new KeyValue(Mps000037ExtendSingleKey.MILITARY_RANK_NAME, rank));

                    string name = !String.IsNullOrEmpty(rdo.lstServiceReq.TDL_PATIENT_NAME) ?
                        rdo.lstServiceReq.TDL_PATIENT_NAME :
                        rdo.currentHisTreatment.TDL_PATIENT_NAME;
                    SetSingleKey(new KeyValue(Mps000037ExtendSingleKey.VIR_PATIENT_NAME, name));

                    string qg = !String.IsNullOrEmpty(rdo.lstServiceReq.TDL_PATIENT_NATIONAL_NAME) ?
                        rdo.lstServiceReq.TDL_PATIENT_NATIONAL_NAME :
                        rdo.currentHisTreatment.TDL_PATIENT_NATIONAL_NAME;
                    SetSingleKey(new KeyValue(Mps000037ExtendSingleKey.NATIONAL_NAME, qg));

                    string work = !String.IsNullOrEmpty(rdo.lstServiceReq.TDL_PATIENT_WORK_PLACE) ?
                        rdo.lstServiceReq.TDL_PATIENT_WORK_PLACE :
                        rdo.currentHisTreatment.TDL_PATIENT_WORK_PLACE;
                    SetSingleKey(new KeyValue(Mps000037ExtendSingleKey.WORK_PLACE, work));

                    string work_name = !String.IsNullOrEmpty(rdo.lstServiceReq.TDL_PATIENT_WORK_PLACE_NAME) ?
                        rdo.lstServiceReq.TDL_PATIENT_WORK_PLACE_NAME :
                        rdo.currentHisTreatment.TDL_PATIENT_WORK_PLACE_NAME;
                    SetSingleKey(new KeyValue(Mps000037ExtendSingleKey.WORK_PLACE_NAME, work_name));

                    if (rdo.currentHisTreatment.CLINICAL_IN_TIME.HasValue)
                    {
                        SetSingleKey(new KeyValue(Mps000037ExtendSingleKey.OPEN_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.currentHisTreatment.CLINICAL_IN_TIME ?? 0)));
                    }
                    else
                    {
                        SetSingleKey(new KeyValue(Mps000037ExtendSingleKey.OPEN_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.currentHisTreatment.IN_TIME)));
                    }

                    List<string> infos = new List<string>();
                    infos.Add(rdo.lstServiceReq.TDL_PATIENT_CODE);
                    infos.Add(rdo.lstServiceReq.TDL_PATIENT_NAME);
                    infos.Add(rdo.lstServiceReq.TDL_PATIENT_GENDER_NAME);
                    infos.Add(rdo.lstServiceReq.TDL_PATIENT_DOB.ToString().Substring(0, 4));
                    infos.Add(rdo.lstServiceReq.SERVICE_REQ_CODE);

                    string totalInfo = string.Join("\t", infos);
                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(totalInfo, QRCodeGenerator.ECCLevel.Q);
                    BitmapByteQRCode qrCode = new BitmapByteQRCode(qrCodeData);
                    byte[] qrCodeImage = qrCode.GetGraphic(20);
                    SetSingleKey(Mps000037ExtendSingleKey.QRCODE_PATIENT, qrCodeImage);

                    SetSingleKey(new KeyValue("IS_EMERGENCY_REQ", rdo.lstServiceReq.IS_EMERGENCY));
                    SetSingleKey(new KeyValue(Mps000037ExtendSingleKey.REQ_PROVISIONAL_DIAGNOSIS, rdo.lstServiceReq.PROVISIONAL_DIAGNOSIS));
                }

                if (rdo.V_HIS_PATIENT_TYPE_ALTER != null)
                {
                    if (rdo.V_HIS_PATIENT_TYPE_ALTER.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE)
                    {
                        if (rdo.V_HIS_PATIENT_TYPE_ALTER.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.EMERGENCY)// la dung tuyen cap cuu
                        {
                            SetSingleKey(new KeyValue(Mps000037ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, "X"));
                            SetSingleKey(new KeyValue(Mps000037ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                            SetSingleKey(new KeyValue(Mps000037ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                        else if (rdo.V_HIS_PATIENT_TYPE_ALTER.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.PRESENT)// la dung tuyen: gioi thieu,
                        {
                            SetSingleKey(new KeyValue(Mps000037ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            SetSingleKey(new KeyValue(Mps000037ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                            SetSingleKey(new KeyValue(Mps000037ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                        else
                        {
                            SetSingleKey(new KeyValue(Mps000037ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            SetSingleKey(new KeyValue(Mps000037ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                            SetSingleKey(new KeyValue(Mps000037ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                    }
                    else if (rdo.V_HIS_PATIENT_TYPE_ALTER.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.FALSE)//trai tuyen
                    {
                        SetSingleKey(new KeyValue(Mps000037ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                        SetSingleKey(new KeyValue(Mps000037ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                        SetSingleKey(new KeyValue(Mps000037ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, "X"));
                    }

                    if (!String.IsNullOrEmpty(rdo.V_HIS_PATIENT_TYPE_ALTER.HEIN_CARD_NUMBER))
                    {
                        SetSingleKey((new KeyValue(Mps000037ExtendSingleKey.HEIN_CARD_NUMBER_SEPARATE, SetHeinCardNumberDisplayByNumber(rdo.V_HIS_PATIENT_TYPE_ALTER.HEIN_CARD_NUMBER))));
                        SetSingleKey((new KeyValue(Mps000037ExtendSingleKey.IS_HEIN, "X")));
                        SetSingleKey((new KeyValue(Mps000037ExtendSingleKey.IS_VIENPHI, "")));
                        SetSingleKey((new KeyValue(Mps000037ExtendSingleKey.HEIN_CARD_NUMBER_1, rdo.V_HIS_PATIENT_TYPE_ALTER.HEIN_CARD_NUMBER.Substring(0, 2))));
                        SetSingleKey((new KeyValue(Mps000037ExtendSingleKey.HEIN_CARD_NUMBER_2, rdo.V_HIS_PATIENT_TYPE_ALTER.HEIN_CARD_NUMBER.Substring(2, 1))));
                        SetSingleKey((new KeyValue(Mps000037ExtendSingleKey.HEIN_CARD_NUMBER_3, rdo.V_HIS_PATIENT_TYPE_ALTER.HEIN_CARD_NUMBER.Substring(3, 2))));
                        SetSingleKey((new KeyValue(Mps000037ExtendSingleKey.HEIN_CARD_NUMBER_4, rdo.V_HIS_PATIENT_TYPE_ALTER.HEIN_CARD_NUMBER.Substring(5, 2))));
                        SetSingleKey((new KeyValue(Mps000037ExtendSingleKey.HEIN_CARD_NUMBER_5, rdo.V_HIS_PATIENT_TYPE_ALTER.HEIN_CARD_NUMBER.Substring(7, 3))));
                        SetSingleKey((new KeyValue(Mps000037ExtendSingleKey.HEIN_CARD_NUMBER_6, rdo.V_HIS_PATIENT_TYPE_ALTER.HEIN_CARD_NUMBER.Substring(10, 5))));
                        SetSingleKey((new KeyValue(Mps000037ExtendSingleKey.STR_HEIN_CARD_FROM_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString((rdo.V_HIS_PATIENT_TYPE_ALTER.HEIN_CARD_FROM_TIME ?? 0)))));
                        SetSingleKey((new KeyValue(Mps000037ExtendSingleKey.STR_HEIN_CARD_TO_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString((rdo.V_HIS_PATIENT_TYPE_ALTER.HEIN_CARD_TO_TIME ?? 0)))));
                        SetSingleKey(new KeyValue(Mps000037ExtendSingleKey.HEIN_ADDRESS, rdo.V_HIS_PATIENT_TYPE_ALTER.ADDRESS));
                    }
                    else
                    {
                        SetSingleKey(new KeyValue(Mps000037ExtendSingleKey.IS_NOT_HEIN, "X"));
                        SetSingleKey((new KeyValue(Mps000037ExtendSingleKey.IS_VIENPHI, "X")));
                    }
                }

                if (rdo.ListServiceReqPrint != null && rdo.ListServiceReqPrint.Count > 0)
                {
                    var reqOrder = rdo.ListServiceReqPrint.OrderBy(o => o.ESTIMATE_TIME_FROM ?? 99999999999999).ThenBy(o => o.INTRUCTION_TIME).ThenBy(o => o.ID).Select(s => s.EXECUTE_ROOM_NAME).ToList();
                    //Lúc in ra, cần xử lý để ko in ra dữ liệu có 2 phòng trùng nhau và xếp liên tiếp nhau:
                    List<string> order = new List<string>();
                    order.Add(reqOrder[0]);
                    if (reqOrder.Count > 1)
                    {
                        for (int i = 1; i < reqOrder.Count; i++)
                        {
                            if (reqOrder[i] == reqOrder[i - 1])
                            {
                                continue;
                            }

                            order.Add(reqOrder[i]);
                        }
                    }

                    string estimateTimeOrder = string.Join(" --> ", order);
                    SetSingleKey(new KeyValue(Mps000037ExtendSingleKey.ESTIMATE_TIME_ORDER, estimateTimeOrder));
                    SetSingleKey(new KeyValue(Mps000037ExtendSingleKey.MIN_ASSIGN_TURN_CODE, rdo.ListServiceReqPrint.Min(o => o.ASSIGN_TURN_CODE)));
                }

                AddObjectKeyIntoListkey<V_HIS_PATIENT_TYPE_ALTER>(rdo.V_HIS_PATIENT_TYPE_ALTER, false);
                AddObjectKeyIntoListkey<V_HIS_SERVICE_REQ>(rdo.lstServiceReq);
                AddObjectKeyIntoListkey<HIS_TREATMENT>(rdo.currentHisTreatment, true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public static string SetHeinCardNumberDisplayByNumber(string heinCardNumber)
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
                LogSystem.Error(ex);
                result = heinCardNumber;
            }
            return result;
        }

        private void ProcessListData()
        {
            try
            {
                Dictionary<long, HIS_SERE_SERV_EXT> dicSereServExtData = new Dictionary<long, HIS_SERE_SERV_EXT>();
                if (rdo.SereServExt != null && rdo.SereServExt.Count > 0)
                {
                    dicSereServExtData = rdo.SereServExt.ToDictionary(o => o.SERE_SERV_ID, o => o);
                }

                foreach (var item in rdo.SereServs_All)
                {
                    SereServGroupPlusADO sereServ_service37 = new SereServGroupPlusADO();

                    Inventec.Common.Mapper.DataObjectMapper.Map<SereServGroupPlusADO>(sereServ_service37, item);

                    sereServ_service37.SERVICE_CODE = item.TDL_SERVICE_CODE;
                    sereServ_service37.SERVICE_NAME = item.TDL_SERVICE_NAME;
                    sereServ_service37.SERVICE_REQ_CODE = item.TDL_SERVICE_REQ_CODE;
                    sereServ_service37.VIR_TOTAL_PRICE_OTHER = "0";
                    var req = rdo.ListServiceReqPrint.FirstOrDefault(o => o.ID == item.SERVICE_REQ_ID);
                    sereServ_service37.EXECUTE_ROOM_NAME = req != null ? req.EXECUTE_ROOM_NAME : item.EXECUTE_ROOM_NAME;

                    if (dicSereServExtData != null && dicSereServExtData.ContainsKey(item.ID))
                    {
                        sereServ_service37.CONCLUDE = dicSereServExtData[item.ID].CONCLUDE;
                        sereServ_service37.DESCRIPTION = dicSereServExtData[item.ID].DESCRIPTION;
                        sereServ_service37.BEGIN_TIME = dicSereServExtData[item.ID].BEGIN_TIME;
                        sereServ_service37.END_TIME = dicSereServExtData[item.ID].END_TIME;
                        sereServ_service37.INSTRUCTION_NOTE = dicSereServExtData[item.ID].INSTRUCTION_NOTE;
                        sereServ_service37.NOTE = dicSereServExtData[item.ID].NOTE;
                    }

                    if (rdo.ListService != null && rdo.ListService.Count > 0)
                    {
                        var service = rdo.ListService.FirstOrDefault(o => o.ID == item.SERVICE_ID);
                        sereServ_service37.SERVICE_PARENT_ID = service != null && service.PARENT_ID.HasValue ? service.PARENT_ID.Value : 0;
                    }

                    ListAdo.Add(sereServ_service37);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        class CustomerFuncMergeSameData : TFlexCelUserFunction
        {
            string Name;
            public override object Evaluate(object[] parameters)
            {
                if (parameters == null || parameters.Length < 1)
                    throw new ArgumentException("Bad parameter count in call to Orders() user-defined function");
                bool result = false;
                try
                {
                    string _Name = parameters[0].ToString();
                    if (_Name != null)
                    {
                        if (Name == _Name)
                        {
                            return true;
                        }
                        else
                        {
                            Name = _Name;
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    return false;
                    Inventec.Common.Logging.LogSystem.Debug(ex);
                }

                return result;
            }
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
                    LogSystem.Debug(ex);
                }

                return result;
            }
        }
    }
}
