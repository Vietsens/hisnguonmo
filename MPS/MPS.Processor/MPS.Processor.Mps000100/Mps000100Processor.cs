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
using AutoMapper;
using FlexCel.Report;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000100.ADO;
using MPS.Processor.Mps000100.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace MPS.Processor.Mps000100
{
    class Mps000100Processor : AbstractProcessor
    {
        Mps000100PDO rdo;
        List<Mps000100ADO> ImpMestManuMedicineSumForPrints = new List<Mps000100ADO>();
        List<Mps000100ADO> lstMedicineType = new List<Mps000100ADO>();
        List<Mps000100ADO> lstMedicineParent = new List<Mps000100ADO>();
        public Mps000100Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000100PDO)rdoBase;
        }

        public void SetBarcodeKey()
        {
            try
            {
                Inventec.Common.BarcodeLib.Barcode impMestCodeBar = new Inventec.Common.BarcodeLib.Barcode(rdo.AggrImpMest.IMP_MEST_CODE);
                impMestCodeBar.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                impMestCodeBar.IncludeLabel = false;
                impMestCodeBar.Width = 120;
                impMestCodeBar.Height = 40;
                impMestCodeBar.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                impMestCodeBar.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                impMestCodeBar.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                impMestCodeBar.IncludeLabel = true;

                dicImage.Add(Mps000100ExtendSingleKey.IMP_MEST_CODE_BARCODE, impMestCodeBar);

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
                ProcessListADO();
                GetMedicineGroup();
                GetMedicineParent();
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                SetSingleKey();
                SetBarcodeKey();

                //ghi đè PrintLogData và UniqueCodeData
                ProcessPrintLogData();
                //lấy số lần in
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));

                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);
                objectTag.AddObjectData(store, "ImpMestAggregates", ImpMestManuMedicineSumForPrints);
                objectTag.AddObjectData(store, "MedicineGroup", lstMedicineType);
                objectTag.AddObjectData(store, "MedicineParent", lstMedicineParent);
                objectTag.AddRelationship(store, "MedicineGroup", "ImpMestAggregates", "MEDICINE_GROUP_ID", "MEDICINE_GROUP_ID");
                objectTag.AddRelationship(store, "MedicineParent", "ImpMestAggregates", "MEDICINE_PARENT_ID", "MEDICINE_PARENT_ID");
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void GetMedicineGroup()
        {
            try
            {
                if (ImpMestManuMedicineSumForPrints != null && ImpMestManuMedicineSumForPrints.Count > 0)
                {
                    var group = ImpMestManuMedicineSumForPrints.GroupBy(o => new { o.MEDICINE_GROUP_ID});
                    foreach (var item in group)
                    {
                        var meGroup = rdo._MedicineTypes.FirstOrDefault(o => o.MEDICINE_GROUP_ID == item.ToList().First().MEDICINE_GROUP_ID);
                        if (meGroup != null)
                        {
                            Mps000100ADO ado = new Mps000100ADO();
                            ado.MEDICINE_GROUP_ID = meGroup.MEDICINE_GROUP_ID;
                            ado.MEDICINE_GROUP_CODE = meGroup.MEDICINE_GROUP_CODE;
                            ado.MEDICINE_GROUP_NAME = meGroup.MEDICINE_GROUP_NAME;
                            lstMedicineType.Add(ado);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetMedicineParent()
        {
            try
            {
                if (ImpMestManuMedicineSumForPrints != null && ImpMestManuMedicineSumForPrints.Count > 0)
                {
                    var group = ImpMestManuMedicineSumForPrints.GroupBy(o => o.MEDICINE_PARENT_ID);
                    foreach (var item in group)
                    {
                        lstMedicineParent.Add(item.ToList().First());
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ProcessListADO()
        {
            try
            {
                ImpMestManuMedicineSumForPrints = new List<Mps000100ADO>();
                if (rdo.IsMedicine && rdo._ImpMestMedicines != null && rdo._ImpMestMedicines.Count > 0)
                {
                    rdo._ImpMestMedicines = rdo._ImpMestMedicines.Where(p => Check(p)).ToList();

                    if (rdo.RoomIds != null && rdo.RoomIds.Count > 0)
                    {
                        rdo._ImpMestMedicines = rdo._ImpMestMedicines.Where(o => rdo.RoomIds.Contains(o.REQ_ROOM_ID ?? 0)).ToList();
                    }
                    var dataGroups = rdo._ImpMestMedicines.GroupBy(p => p.MEDICINE_TYPE_ID).Select(p => p.ToList()).ToList();
                    ImpMestManuMedicineSumForPrints.AddRange((from r in dataGroups select new Mps000100ADO(r, rdo._MedicineTypes, rdo.AggrImpMest.IMP_MEST_STT_ID, rdo.HisImpMestSttId__Imported, rdo.HisImpMestSttId__Approved)).ToList());
                }
                if (rdo.Ismaterial && rdo._ImpMestMaterials != null && rdo._ImpMestMaterials.Count > 0)
                {
                    rdo._ImpMestMaterials = rdo._ImpMestMaterials.Where(p => Check(p)).ToList();
                    if (rdo.RoomIds != null && rdo.RoomIds.Count > 0)
                    {
                        rdo._ImpMestMaterials = rdo._ImpMestMaterials.Where(o => rdo.RoomIds.Contains(o.REQ_ROOM_ID ?? 0)).ToList();
                    }
                    var dataGroups = rdo._ImpMestMaterials.GroupBy(p => p.MATERIAL_TYPE_ID).Select(p => p.ToList()).ToList();
                    ImpMestManuMedicineSumForPrints.AddRange((from r in dataGroups select new Mps000100ADO(r, rdo.AggrImpMest.IMP_MEST_STT_ID, rdo.HisImpMestSttId__Imported, rdo.HisImpMestSttId__Approved)).ToList());
                }

                if (ImpMestManuMedicineSumForPrints != null && ImpMestManuMedicineSumForPrints.Count > 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("rdo.OderOption____", rdo.OderOption));
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("ImpMestManuMedicineSumForPrints_____", ImpMestManuMedicineSumForPrints));
                    if (rdo.OderOption == 1)
                    {
                        ImpMestManuMedicineSumForPrints = ImpMestManuMedicineSumForPrints.OrderBy(o => o.Type).ThenBy(o => o.NUM_ORDER ?? 99999).ThenBy(p => p.MEDICINE_TYPE_NAME).ToList();
                    }
                    if (rdo.OderOption == 2)
                    {
                        ImpMestManuMedicineSumForPrints = ImpMestManuMedicineSumForPrints.OrderBy(o => o.MEDICINE_USE_FORM_NUM_ORDER).ThenBy(o => o.MEDICINE_TYPE_NAME).ToList();
                    }
                    if (rdo.OderOption == 3)
                    {
                        ImpMestManuMedicineSumForPrints = ImpMestManuMedicineSumForPrints.OrderByDescending(o => o.MEDICINE_USE_FORM_NUM_ORDER).ThenBy(p => p.MEDICINE_TYPE_NAME).ToList();
                    }
                    if (rdo.OderOption == 4)
                    {
                        ImpMestManuMedicineSumForPrints = ImpMestManuMedicineSumForPrints.OrderBy(o => o.SERVICE_UNIT_NAME).ThenBy(p => p.MEDICINE_TYPE_NAME).ToList();
                    }

                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        bool Check(V_HIS_IMP_MEST_MEDICINE _medi)
        {
            bool result = false;
            try
            {
                var data = rdo._MedicineTypes.FirstOrDefault(p => p.ID == _medi.MEDICINE_TYPE_ID);
                if (data != null)
                {
                    if (rdo.ServiceUnitIds != null
                        && rdo.ServiceUnitIds.Count > 0)
                    {
                        if (rdo.ServiceUnitIds.Contains(data.SERVICE_UNIT_ID))
                            result = true;
                    }
                    if (data.MEDICINE_USE_FORM_ID > 0)
                    {
                        if (rdo.UseFormIds != null
                    && rdo.UseFormIds.Count > 0 && rdo.UseFormIds.Contains(data.MEDICINE_USE_FORM_ID ?? 0))
                        {
                            result = result && true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        bool Check(V_HIS_IMP_MEST_MATERIAL _mate)
        {
            bool result = false;
            try
            {
                if (_mate != null)
                {
                    if (rdo.ServiceUnitIds != null && rdo.ServiceUnitIds.Count > 0)
                    {
                        if (rdo.ServiceUnitIds.Contains(_mate.SERVICE_UNIT_ID))
                            result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        void SetSingleKey()
        {
            try
            {
                if (rdo._MobaExpMests != null && rdo._MobaExpMests.Count > 0)
                {
                    var minTime = rdo._MobaExpMests.Min(p => p.TDL_INTRUCTION_TIME ?? 0);
                    var maxTime = rdo._MobaExpMests.Max(p => p.TDL_INTRUCTION_TIME ?? 0);
                    SetSingleKey(new KeyValue(Mps000100ExtendSingleKey.MIN_INTRUCTION_DATE_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToDateString(minTime)));
                    SetSingleKey(new KeyValue(Mps000100ExtendSingleKey.MIN_INTRUCTION_TIME_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(minTime)));
                    SetSingleKey(new KeyValue(Mps000100ExtendSingleKey.MAX_INTRUCTION_TIME_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(maxTime)));
                    SetSingleKey(new KeyValue(Mps000100ExtendSingleKey.MAX_INTRUCTION_DATE_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToDateString(maxTime)));
                }
                string name = "";
                switch (rdo.keyNameTitles)
                {
                    case IsTittle.TongHop:
                        name = "TỔNG HỢP";
                        break;
                    case IsTittle.ThuocThuong:
                        name = "THUỐC THƯỜNG";
                        break;
                    case IsTittle.VatTu:
                        name = "VẬT TƯ";
                        break;
                    case IsTittle.Corticoid:
                        name = "CORTICOID";
                        break;
                    case IsTittle.KhangSinh:
                        name = "KHÁNG SINH";
                        break;
                    case IsTittle.Lao:
                        name = "LAO";
                        break;
                    case IsTittle.DichTruyen:
                        name = "DỊCH TRUYỀN";
                        break;
                    case IsTittle.TienChat:
                        name = "TIỀN CHẤT";
                        break;
                    default:
                        break;
                }

                SetSingleKey(new KeyValue(Mps000100ExtendSingleKey.KEY_NAME_TITLES, name));
                AddObjectKeyIntoListkey<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST>(rdo.AggrImpMest, false);
                AddObjectKeyIntoListkey<MOS.EFMODEL.DataModels.HIS_DEPARTMENT>(rdo.Department, false);

                SetSingleKey(new KeyValue(Mps000100ExtendSingleKey.CREATE_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.AggrImpMest.CREATE_TIME ?? 0)));
                if (rdo.AggrImpMest.REQ_ROOM_ID.HasValue && rdo.HisRoom != null && rdo.HisRoom.Count > 0)
                {
                    var room = rdo.HisRoom.FirstOrDefault(o => o.ID == rdo.AggrImpMest.REQ_ROOM_ID);
                    if (room != null)
                    {
                        SetSingleKey(new KeyValue(Mps000100ExtendSingleKey.REQ_ROOM_NAME, room.ROOM_NAME));
                        SetSingleKey(new KeyValue(Mps000100ExtendSingleKey.REQ_ROOM_CODE, room.ROOM_CODE));
                    }
                }

                if (rdo.ListAggrAggrImpMest != null && rdo.ListAggrAggrImpMest.Count > 0)
                {
                    string impMestCodes = string.Join(",", rdo.ListAggrAggrImpMest.Select(o => o.IMP_MEST_CODE).Distinct().ToList());

                    SetSingleKey((new KeyValue(Mps000100ExtendSingleKey.IMP_MEST_CODEs, impMestCodes)));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public override string ProcessPrintLogData()
        {
            string log = "";
            try
            {
                if (rdo != null && rdo.AggrImpMest != null)
                {
                    log = LogDataImpMest("", rdo.AggrImpMest.IMP_MEST_CODE, rdo.AggrImpMest.REQ_DEPARTMENT_NAME + "_" + rdo.AggrImpMest.MEDI_STOCK_NAME);
                }
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
                int countMedicine = 0;
                int countMaterial = 0;
                if (rdo._ImpMestMedicines != null)
                {
                    countMedicine = rdo._ImpMestMedicines.Count;
                }

                if (rdo._ImpMestMaterials != null)
                {
                    countMaterial = rdo._ImpMestMaterials.Count;
                }

                if (rdo != null && rdo.AggrImpMest != null)
                    result = String.Format("{0}_{1}_{2}_{3}_{4}", printTypeCode, rdo.AggrImpMest.IMP_MEST_CODE, rdo.AggrImpMest.MEDI_STOCK_CODE, countMedicine, countMaterial);
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        //Mps000100PDO rdo;
        //List<Mps000100ADO> ImpMestManuMedicineSumForPrints = new List<Mps000100ADO>();
        //List<ImpMestAggregatePrintByPageADO> result;
        //public Mps000100Processor(CommonParam param, PrintData printData)
        //    : base(param, printData)
        //{
        //    rdo = (Mps000100PDO)rdoBase;
        //}

        //private void SetBarcodeKey()
        //{
        //    try
        //    {
        //        Inventec.Common.BarcodeLib.Barcode barcodePatientCode = new Inventec.Common.BarcodeLib.Barcode(rdo.AggrImpMest.IMP_MEST_CODE);
        //        barcodePatientCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
        //        barcodePatientCode.IncludeLabel = false;
        //        barcodePatientCode.Width = 120;
        //        barcodePatientCode.Height = 40;
        //        barcodePatientCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
        //        barcodePatientCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
        //        barcodePatientCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
        //        barcodePatientCode.IncludeLabel = true;

        //        dicImage.Add(Mps000100ExtendSingleKey.IMP_MEST_CODE_BAR, barcodePatientCode);

        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        //string GetMedicineTypeName(long serviceId)
        //{
        //    string result = "";
        //    try
        //    {
        //        result = rdo.MedicineImpmestTypeADOs.FirstOrDefault(o => o.SERVICE_ID == serviceId).MEDICINE_TYPE_NAME;
        //    }
        //    catch (Exception ex)
        //    {
        //        result = "";
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }

        //    return result;
        //}

        //string GetBedRoomByPatient(long treatmentId)
        //{
        //    string result = String.Empty;
        //    try
        //    {
        //        var treatmentBedRooms = rdo.vHisTreatmentBedRoom.Where(o => o.TREATMENT_ID == treatmentId).ToList();
        //        if (treatmentBedRooms != null && treatmentBedRooms.Count > 0)
        //        {
        //            result = treatmentBedRooms[0].BED_ROOM_NAME;
        //            //var bedRoomNames = treatmentBedRooms.Select(o => o.BED_ROOM_NAME).Distinct().ToArray();
        //            //result = string.Join(";", bedRoomNames);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }

        //    return result;
        //}

        //List<ImpMestAggregatePrintByPageADO> PrepareData(List<Mps000100ADO> medicineImpmestTypeADOs)
        //{
        //    result = null;
        //    if (medicineImpmestTypeADOs != null && medicineImpmestTypeADOs.Count > 0)
        //    {
        //        result = new List<ImpMestAggregatePrintByPageADO>();
        //        List<long> distinctDates = medicineImpmestTypeADOs.OrderBy(o => o.MEDICINE_NUM_ORDER).ThenBy(p => p.MEDICINE_TYPE_NAME)
        //            .Select(o => o.SERVICE_ID)
        //            .Distinct().ToList();
        //        var trainGroups = medicineImpmestTypeADOs.GroupBy(o => new { o.Patient, o.TreatmentId });
        //        int index = 0;
        //        while (index < distinctDates.Count)
        //        {
        //            ImpMestAggregatePrintByPageADO sdo = new ImpMestAggregatePrintByPageADO();

        //            for (int i = 1; i <= 44; i++)
        //            {
        //                System.Reflection.PropertyInfo piMedicineTypeName = typeof(ImpMestAggregatePrintByPageADO).GetProperty("MEDICINE_TYPE_NAME" + i);
        //                if (piMedicineTypeName != null)
        //                    piMedicineTypeName.SetValue(sdo, index < distinctDates.Count ? GetMedicineTypeName(distinctDates[index]) : "");

        //                System.Reflection.PropertyInfo piMedicineTypeId = typeof(ImpMestAggregatePrintByPageADO).GetProperty("SERVICE_ID" + i);
        //                if (piMedicineTypeId != null)
        //                    piMedicineTypeId.SetValue(sdo, index < distinctDates.Count ? distinctDates[index] : 0);

        //                index++;
        //            }

        //            List<ImpMestAggregatePrintADO> trainPrints = new List<ImpMestAggregatePrintADO>();
        //            foreach (var group in trainGroups)
        //            {
        //                ImpMestAggregatePrintADO trainPrint = new ImpMestAggregatePrintADO();
        //                List<Mps000100ADO> trains = group.ToList();

        //                if (group.Key.Patient != null)
        //                {
        //                    trainPrint.PATIENT_ID = group.Key.Patient.ID;
        //                    trainPrint.PATIENT_CODE = group.Key.Patient.PATIENT_CODE;
        //                    trainPrint.VIR_PATIENT_NAME = group.Key.Patient.VIR_PATIENT_NAME;
        //                    trainPrint.AGE = AgeUtil.CalculateFullAge(group.Key.Patient.DOB);
        //                }
        //                trainPrint.IS_BHYT = group.FirstOrDefault().IS_BHYT;
        //                trainPrint.TREATMENT_CODE = group.FirstOrDefault().TREATMENT_CODE;
        //                var assignRooms = rdo.vHisRoom.Where(o => o.ID == group.FirstOrDefault().REQ_ROOM_ID).ToList();
        //                //
        //                if (assignRooms != null && assignRooms.Count > 0)
        //                {
        //                    trainPrint.BED_ROOM_NAMEs = assignRooms.FirstOrDefault().ROOM_NAME;
        //                }
        //                else
        //                {
        //                    trainPrint.BED_ROOM_NAMEs = GetBedRoomByPatient(group.Key.TreatmentId);
        //                }

        //                for (int i = 1; i <= 44; i++)
        //                {
        //                    System.Reflection.PropertyInfo piServiceId = typeof(ImpMestAggregatePrintByPageADO).GetProperty("SERVICE_ID" + i);
        //                    System.Reflection.PropertyInfo piAmount = typeof(ImpMestAggregatePrintADO).GetProperty("AMOUNT" + i);
        //                    if (piServiceId != null && piAmount != null)
        //                    {
        //                        decimal? amount = trains.Where(o => o.SERVICE_ID == (long)(piServiceId.GetValue(sdo))).Sum(o => o.AMOUNT);
        //                        amount = (amount == 0 ? null : amount);
        //                        piAmount.SetValue(trainPrint, amount);
        //                    }
        //                }

        //                trainPrints.Add(trainPrint);
        //            }
        //            sdo.ImpMestAggregatePrintADOs = trainPrints.OrderBy(o => o.BED_ROOM_NAMEs).ThenBy(o => o.VIR_PATIENT_NAME).ToList();
        //            result.Add(sdo);
        //        }
        //    }
        //    return result;
        //}

        ///// <summary>
        ///// Ham xu ly du lieu da qua xu ly
        ///// Tao ra cac doi tuong du lieu xu dung trong thu vien xu ly file excel
        ///// </summary>
        ///// <returns></returns>
        //public override bool ProcessData()
        //{
        //    bool result = false;
        //    try
        //    {
        //        ImpMestManuMedicineSumForPrints = new List<Mps000100ADO>();
        //        var query = rdo.MedicineImpmestTypeADOs.AsQueryable();

        //        query = query
        //              .Where(o =>
        //              (
        //                  (rdo.IsMedicine
        //                      && o.IS_MEDICINE == true
        //                      && rdo.ServiceUnitIds.Contains(o.SERVICE_UNIT_ID)
        //                  )
        //                  || (rdo.Ismaterial
        //                      && o.IS_MEDICINE == false
        //                      && rdo.ServiceUnitIds.Contains(o.SERVICE_UNIT_ID)
        //                  )
        //              )
        //            );
        //        if (rdo.RoomIds != null && rdo.RoomIds.Count > 0)
        //        {
        //            query = query.Where(o => rdo.RoomIds.Contains(o.REQ_ROOM_ID ?? 0));
        //        }

        //        var impMestManuMedicineTemps = query.ToList();
        //        if (impMestManuMedicineTemps == null || impMestManuMedicineTemps.Count == 0)
        //        {
        //            throw new ArgumentNullException("expMestManuMedicineTemps after filter query is null");
        //        }

        //        List<ImpMestAggregatePrintByPageADO> impMestAggregatePrintByPageADOs = PrepareData(impMestManuMedicineTemps);
        //        if (impMestAggregatePrintByPageADOs == null || impMestAggregatePrintByPageADOs.Count == 0)
        //        {
        //            throw new ArgumentNullException("Get PrepareData - expMestAggregatePrintByPageADOs is null");
        //        }
        //        foreach (var item in impMestAggregatePrintByPageADOs)
        //        {
        //            for (int i = 1; i <= 44; i++)
        //            {
        //                System.Reflection.PropertyInfo piMedicineTypeName = typeof(ImpMestAggregatePrintByPageADO).GetProperty("MEDICINE_TYPE_NAME" + i);
        //                singleValueDictionary.Add("MEDICINE_TYPE_NAME" + i, piMedicineTypeName.GetValue(item));
        //            }

        //            Inventec.Common.FlexCellExport.Store store = new Inventec.Common.FlexCellExport.Store();

        //            Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
        //            Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
        //            Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
        //            SetBarcodeKey();
        //            SetSingleKey();
        //            store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
        //            singleTag.ProcessData(store, singleValueDictionary);
        //            barCodeTag.ProcessData(store, dicImage);
        //            objectTag.AddObjectData(store, "Patients", item.ImpMestAggregatePrintADOs);
        //            store.SetCommonFunctions();

        //            var streamResult = store.OutStream();
        //            if (streamResult != null && streamResult.Length > 0)
        //            {
        //                streamResult.Position = 0;

        //                //Print preview               
        //                result = PrintPreview(streamResult, this.fileName);
        //            }
        //            else
        //            {
        //                Inventec.Common.Logging.LogSystem.Warn("store.OutStream is null");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result = false;
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }

        //    return result;
        //}

        //private void SetSingleKey()
        //{
        //    try
        //    {
        //        //if (this.rdo.MedicineImpmestTypeADOs != null && this.rdo.MedicineImpmestTypeADOs.Count > 0)
        //        //{
        //        //long minUserTime = this.rdo.MedicineImpmestTypeADOs.Min(o => o.INTRUCTION_TIME);
        //        //long maxUserTime = this.rdo.MedicineImpmestTypeADOs.Max(o => o.INTRUCTION_TIME);
        //        //SetSingleKey(new KeyValue(Mps000100ExtendSingleKey.USER_TIME_MEDICINE, Inventec.Common.DateTime.Convert.TimeNumberToDateString(minUserTime)));
        //        //SetSingleKey(new KeyValue(Mps000100ExtendSingleKey.USER_TIME_TO_MEDICINE, Inventec.Common.DateTime.Convert.TimeNumberToDateString(maxUserTime)));
        //        //}

        //        AddObjectKeyIntoListkey<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST>(rdo.AggrImpMest, false);
        //        AddObjectKeyIntoListkey<MOS.EFMODEL.DataModels.HIS_DEPARTMENT>(rdo.Department, false);

        //        SetSingleKey(new KeyValue(Mps000100ExtendSingleKey.CREATE_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.AggrImpMest.CREATE_TIME ?? 0)));
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}
    }
}
