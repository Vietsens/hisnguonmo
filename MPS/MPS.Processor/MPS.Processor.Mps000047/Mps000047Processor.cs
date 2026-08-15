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
using MPS.Processor.Mps000047.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using MOS.EFMODEL.DataModels;

namespace MPS.Processor.Mps000047
{
    public class Mps000047Processor : AbstractProcessor
    {
        Mps000047PDO rdo;
        List<ExpMestAggregatePrintByPageADO> result;
        public Mps000047Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000047PDO)rdoBase;
        }

        public void SetBarcodeKey()
        {
            try
            {
                Inventec.Common.BarcodeLib.Barcode barcodePatientCode = new Inventec.Common.BarcodeLib.Barcode(rdo.AggrExpMest.EXP_MEST_CODE);
                barcodePatientCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcodePatientCode.IncludeLabel = false;
                barcodePatientCode.Width = 120;
                barcodePatientCode.Height = 40;
                barcodePatientCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                barcodePatientCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcodePatientCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcodePatientCode.IncludeLabel = true;

                dicImage.Add(Mps000047ExtendSingleKey.EXP_MEST_CODE_BAR, barcodePatientCode);
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
                var expMestManuMedicineTemps = rdo.MedicineExpmestTypeADOs.ToList();
                if (expMestManuMedicineTemps == null || expMestManuMedicineTemps.Count == 0)
                {
                    throw new ArgumentNullException("expMestManuMedicineTemps after filter query is null");
                }

                List<ExpMestAggregatePrintByPageADO> expMestAggregatePrintByPageADOs = PrepareData(expMestManuMedicineTemps);
                if (expMestAggregatePrintByPageADOs == null || expMestAggregatePrintByPageADOs.Count == 0)
                {
                    throw new ArgumentNullException("Get PrepareData - expMestAggregatePrintByPageADOs is null");
                }

                foreach (var item in expMestAggregatePrintByPageADOs)
                {
                    for (int i = 1; i <= rdo.keyColumnSize; i++)
                    {
                        System.Reflection.PropertyInfo piMedicineTypeName = typeof(ExpMestAggregatePrintByPageADO).GetProperty("MEDICINE_TYPE_NAME" + i);
                        singleValueDictionary.Add("MEDICINE_TYPE_NAME" + i, piMedicineTypeName.GetValue(item));

                        System.Reflection.PropertyInfo piConCenTra = typeof(ExpMestAggregatePrintByPageADO).GetProperty("CONCENTRA" + i);
                        singleValueDictionary.Add("CONCENTRA" + i, piConCenTra.GetValue(item));

                        System.Reflection.PropertyInfo piMedicineUF = typeof(ExpMestAggregatePrintByPageADO).GetProperty("MEDICINE_USE_FORM_NAME" + i);
                        singleValueDictionary.Add("MEDICINE_USE_FORM_NAME" + i, piMedicineUF.GetValue(item));

                        //qtcode
                        System.Reflection.PropertyInfo piMedicineUnit = typeof(ExpMestAggregatePrintByPageADO).GetProperty("MEDICINE_UNIT_NAME" + i);
                        singleValueDictionary.Add("MEDICINE_UNIT_NAME" + i, piMedicineUnit.GetValue(item));
                    }


                    Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                    Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();

                    Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                    SetBarcodeKey();
                    SetSingleKey();

                    //ghi đè PrintLogData và UniqueCodeData 
                    ProcessPrintLogData();
                    //lấy số lần in
                    SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));

                    store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                    singleTag.ProcessData(store, singleValueDictionary);
                    barCodeTag.ProcessData(store, dicImage);

                    //Ca 2 danh sach deu da duoc sap xep trong PrepareData:
                    //phong chi dinh (giong luoi chon phong grdRoom) -> buong -> giuong -> ten benh nhan. 
                    //Khong sap xep lai o day de khong pha vo thu tu do.
                    objectTag.AddObjectData(store, "Patients", item.ExpMestAggregatePrintADOs);

                    objectTag.AddObjectData(store, "PatientRooms", item.ExpMestAggregateReqRoomPrintADOs);
                    result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        string GetMedicineTypeName(long serviceId)
        {
            string result = "";
            try
            {
                result = rdo.MedicineExpmestTypeADOs.FirstOrDefault(o => o.SERVICE_ID == serviceId).MEDICINE_TYPE_NAME;
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }
        string GetMediUseForm(long serviceId)
        {
            string result = "";
            try
            {
                result = rdo.MedicineExpmestTypeADOs.FirstOrDefault(o => o.SERVICE_ID == serviceId).MEDICINE_USE_FORM_NAME;
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }
        string GetConCenTra(long serviceId)
        {
            string result = "";
            try
            {
                result = rdo.MedicineExpmestTypeADOs.FirstOrDefault(o => o.SERVICE_ID == serviceId).CONCENTRA;
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        //qtcode
        string GetServiceUnitName(long serviceId)
        {
            string result = "";
            try
            {
                result = rdo.MedicineExpmestTypeADOs.FirstOrDefault(o => o.SERVICE_ID == serviceId).SERVICE_UNIT_NAME ?? "";
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
        //        Các bước lấy ra dữ liệu buồng như sau:
        //- B1: Lấy ra dữ liệu buồng của hồ sơ điều trị đó và tương ứng với khoa (V_HIS_TREATMENT_BED_ROOM có DEPARTMENT_ID tương ứng với khoa, TREATMENT_ID tương ứng với hồ sơ điều trị)
        //- B2: Kiểm tra, trong dữ liệu buồng có được ở B1, nếu tồn tại V_HIS_TREATMENT_BED_ROOM có REMOVE_TIME null thì lấy V_HIS_TREATMENT_BED_ROOM có ADD_TIME lớn nhất mà REMOVE_TIME null
        //- B3: Nếu không tồn tại V_HIS_TREATMENT_BED_ROOM nào có REMOVE_TIME null thì lấy V_HIS_TREATMENT_BED_ROOM có REMOVE_TIME lớn nhất.
        string GetBedRoomByPatient(long treatmentId)
        {
            string result = String.Empty;
            try
            {
                var treatmentBedRooms = rdo.vHisTreatmentBedRooms.Where(o => o.TREATMENT_ID == treatmentId && o.DEPARTMENT_ID == rdo.Department.ID).ToList();

                List<V_HIS_TREATMENT_BED_ROOM> lsttreatmentBedRoom = new List<V_HIS_TREATMENT_BED_ROOM>();

                if (treatmentBedRooms != null && treatmentBedRooms.Count > 0)
                {
                    //result = treatmentBedRooms[0].BED_ROOM_NAME;

                    var checkFinishTime = treatmentBedRooms.Where(o => o.REMOVE_TIME == null).ToList();

                    if (checkFinishTime != null && checkFinishTime.Count > 0)
                    {
                        lsttreatmentBedRoom = checkFinishTime.OrderByDescending(o => o.ADD_TIME).ToList();
                    }
                    else
                    {
                        lsttreatmentBedRoom = treatmentBedRooms.OrderByDescending(o => o.REMOVE_TIME).ToList();
                    }


                    if (lsttreatmentBedRoom != null && lsttreatmentBedRoom.Count > 0)
                    {
                        result = lsttreatmentBedRoom[0].BED_ROOM_NAME;
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return result;
        }

//        Các bước lấy ra dữ liệu giường như sau:
//- B1: Lấy ra dữ liệu lịch sử giường của hồ sơ điều trị đó và tương ứng với khoa (V_HIS_BED_LOG có DEPARTMENT_ID tương ứng với khoa, TREATMENT_ID tương ứng với hồ sơ điều trị)
//- B2: Kiểm tra, trong dữ liệu "Lịch sử giường" có được ở B1, nếu tồn tại V_HIS_BED_LOG có FINISH_TIME null thì lấy V_HIS_BED_LOG có START_TIME lớn nhất mà FINISH_TIME null
//- B3: Nếu không tồn tại V_HIS_BED_LOG nào có FINISH_TIME null thì lấy V_HIS_BED_LOG có FINISH_TIME lớn nhất.
        void GetBedByTreatment(long treatmentId, ref string BedCode, ref string BedName)
        {
            if (rdo._listBedLog == null || rdo._listBedLog.Count == 0)
                return;
            try
            {
                //var bedLog = rdo._listBedLog
                //    .Where(o => o.TREATMENT_ID == treatmentId)
                //    .OrderByDescending(o => o.START_TIME).ToList();

                var bedLodDepartment = rdo._listBedLog.Where(o => o.TREATMENT_ID == treatmentId && o.DEPARTMENT_ID == rdo.Department.ID).ToList();

                List<V_HIS_BED_LOG> bedLog = new List<V_HIS_BED_LOG>();

                var checkFinishTime = (bedLodDepartment != null && bedLodDepartment.Count > 0) ? bedLodDepartment.Where(o => o.FINISH_TIME == null).ToList() : null;

                if (checkFinishTime != null && checkFinishTime.Count > 0)
                {
                    bedLog = checkFinishTime.OrderByDescending(o => o.START_TIME).ToList();
                }
                else 
                {
                    bedLog = bedLodDepartment.OrderByDescending(o => o.FINISH_TIME).ToList();
                }


                if (bedLog != null && bedLog.Count > 0)
                {
                    BedCode = bedLog[0].BED_CODE;
                    BedName = bedLog[0].BED_NAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        /// <summary>
        /// Lay thu tu cac phong chi dinh dung nhu danh sach hien thi tren luoi chon phong (grdRoom)
        /// cua chuc nang HIS.Desktop.Plugins.AggrExpMestPrintFilter: theo thu tu V_HIS_ROOM trong cache,
        /// chi giu lai cac phong chi dinh co trong du lieu in.
        /// </summary>
        Dictionary<long, int> GetReqRoomOrders(List<Mps000047ADO> medicineExpmestTypeADOs)
        {
            Dictionary<long, int> result = new Dictionary<long, int>();
            try
            {
                List<long> reqRoomIds = medicineExpmestTypeADOs.Select(o => o.REQ_ROOM_ID).Distinct().ToList();
                var rooms = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>()
                    .Where(o => reqRoomIds.Contains(o.ID)).ToList();
                foreach (var room in rooms)
                {
                    if (!result.ContainsKey(room.ID))
                    {
                        result.Add(room.ID, result.Count);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return result;
        }

        /// <summary>
        /// Thu tu phong cua 1 benh nhan. Benh nhan co thuoc/vat tu cua nhieu phong chi dinh
        /// thi lay phong dung dau trong luoi chon phong. Phong khong xac dinh duoc thi xep cuoi. 
        /// </summary>
        int GetReqRoomOrder(Dictionary<long, int> reqRoomOrders, List<Mps000047ADO> trains)
        {
            int result = int.MaxValue;
            try
            {
                foreach (var train in trains)
                {
                    int roomOrder = reqRoomOrders.ContainsKey(train.REQ_ROOM_ID) ? reqRoomOrders[train.REQ_ROOM_ID] : int.MaxValue;
                    if (roomOrder < result)
                    {
                        result = roomOrder;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return result;
        }

        /// <summary>
        /// Khoa sap xep tu nhien: cac cum chu so duoc dem 0 cho bang do dai
        /// de "Buong 2" dung truoc "Buong 10" (sap xep chuoi thuong thi "10" dung truoc "2").
        /// Gia tri rong luon xep cuoi.
        /// </summary>
        static string GetNaturalOrderKey(string value)
        {
            string result = "";
            try
            {
                if (String.IsNullOrWhiteSpace(value))
                    return "￿";

                StringBuilder builder = new StringBuilder();
                int index = 0;
                while (index < value.Length)
                {
                    if (Char.IsDigit(value[index]))
                    {
                        int start = index;
                        while (index < value.Length && Char.IsDigit(value[index]))
                        {
                            index++;
                        }
                        builder.Append(value.Substring(start, index - start).TrimStart('0').PadLeft(12, '0'));
                    }
                    else
                    {
                        builder.Append(Char.ToUpperInvariant(value[index]));
                        index++;
                    }
                }
                result = builder.ToString();
            }
            catch (Exception ex)
            {
                result = value ?? "";
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return result;
        }

        /// <summary>
        /// Sap xep danh sach benh nhan hien thi tren bieu in:
        /// phong chi dinh (dung thu tu luoi chon phong grdRoom) -> buong -> giuong -> ten -> ma benh nhan.
        /// </summary>
        List<ExpMestAggregatePrintADO> SortPatientPrints(List<ExpMestAggregatePrintADO> trainPrints, List<int> roomOrders)
        {
            List<ExpMestAggregatePrintADO> result = trainPrints;
            try
            {
                result = trainPrints
                    .Select((o, i) => new { PRINT_ADO = o, ROOM_ORDER = (i < roomOrders.Count ? roomOrders[i] : int.MaxValue) })
                    .OrderBy(o => o.ROOM_ORDER)
                    .ThenBy(o => GetNaturalOrderKey(o.PRINT_ADO.BED_ROOM_NAMEs), StringComparer.Ordinal)
                    .ThenBy(o => GetNaturalOrderKey(o.PRINT_ADO.BED_CODE), StringComparer.Ordinal)
                    .ThenBy(o => GetNaturalOrderKey(o.PRINT_ADO.BED_NAME), StringComparer.Ordinal)
                    .ThenBy(o => o.PRINT_ADO.VIR_PATIENT_NAME)
                    .ThenBy(o => o.PRINT_ADO.PATIENT_CODE)
                    .Select(o => o.PRINT_ADO)
                    .ToList();
            }
            catch (Exception ex)
            {
                result = trainPrints;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        List<ExpMestAggregatePrintByPageADO> PrepareData(List<Mps000047ADO> medicineExpmestTypeADOs)
        {
            result = null;
            if (medicineExpmestTypeADOs != null && medicineExpmestTypeADOs.Count > 0)
            {
                result = new List<ExpMestAggregatePrintByPageADO>();
                Dictionary<long, int> reqRoomOrders = GetReqRoomOrders(medicineExpmestTypeADOs);
                List<long> distinctDates = medicineExpmestTypeADOs.OrderBy(o => o.MEDICINE_TYPE_NAME)
                    .Select(o => o.SERVICE_ID)
                    .Distinct().ToList();
                int index = 0;
                while (index < distinctDates.Count)
                {
                    ExpMestAggregatePrintByPageADO sdo = new ExpMestAggregatePrintByPageADO();

                    for (int i = 1; i <= rdo.keyColumnSize; i++)
                    {
                        System.Reflection.PropertyInfo piMedicineTypeName = typeof(ExpMestAggregatePrintByPageADO).GetProperty("MEDICINE_TYPE_NAME" + i);
                        if (piMedicineTypeName != null)
                            piMedicineTypeName.SetValue(sdo, index < distinctDates.Count ? GetMedicineTypeName(distinctDates[index]) : "");

                        System.Reflection.PropertyInfo piMedicineTypeId = typeof(ExpMestAggregatePrintByPageADO).GetProperty("SERVICE_ID" + i);
                        if (piMedicineTypeId != null)
                            piMedicineTypeId.SetValue(sdo, index < distinctDates.Count ? distinctDates[index] : 0);

                        System.Reflection.PropertyInfo piConCenTra = typeof(ExpMestAggregatePrintByPageADO).GetProperty("CONCENTRA" + i);
                        if (piConCenTra != null)
                            piConCenTra.SetValue(sdo, index < distinctDates.Count ? GetConCenTra(distinctDates[index]) : "");

                        System.Reflection.PropertyInfo piMedicineUF = typeof(ExpMestAggregatePrintByPageADO).GetProperty("MEDICINE_USE_FORM_NAME" + i);
                        if (piMedicineUF != null)
                            piMedicineUF.SetValue(sdo, index < distinctDates.Count ? GetMediUseForm(distinctDates[index]) : "");
                        //qtcode
                        System.Reflection.PropertyInfo piMedicineUnit = typeof(ExpMestAggregatePrintByPageADO).GetProperty("MEDICINE_UNIT_NAME" + i);
                        if (piMedicineUnit != null)
                            piMedicineUnit.SetValue(sdo, index < distinctDates.Count ? GetServiceUnitName(distinctDates[index]) : "");

                        index++;
                    }

                    var trainGroups = medicineExpmestTypeADOs.GroupBy(o => new { o.Patient, o.TreatmentId });
                    List<ExpMestAggregatePrintADO> trainPrints = new List<ExpMestAggregatePrintADO>();
                    List<int> trainPrintRoomOrders = new List<int>();
                    foreach (var group in trainGroups)
                    {
                        ExpMestAggregatePrintADO trainPrint = new ExpMestAggregatePrintADO();
                        List<Mps000047ADO> trains = group.ToList();

                        if (group.Key.Patient != null)
                        {
                            trainPrint.PATIENT_ID = group.Key.Patient.ID;
                            trainPrint.PATIENT_CODE = group.Key.Patient.PATIENT_CODE;
                            trainPrint.VIR_PATIENT_NAME = group.Key.Patient.VIR_PATIENT_NAME;
                            trainPrint.DOB = group.Key.Patient.DOB;
                            trainPrint.GENDER_NAME = group.Key.Patient.GENDER_NAME;
                            trainPrint.AGE = AgeUtil.CalculateFullAge(group.Key.Patient.DOB);
                            trainPrint.TREATMENT_CODE = group.FirstOrDefault().TREATMENT_CODE;
                        }
                        trainPrint.BED_ROOM_NAMEs = GetBedRoomByPatient(group.Key.TreatmentId);
                        string bedCode = "", bedName="";
                        GetBedByTreatment(group.Key.TreatmentId, ref bedCode, ref bedName);
                        trainPrint.BED_CODE = bedCode;
                        trainPrint.BED_NAME = bedName;

                        for (int i = 1; i <= rdo.keyColumnSize; i++)
                        {
                            System.Reflection.PropertyInfo piServiceId = typeof(ExpMestAggregatePrintByPageADO).GetProperty("SERVICE_ID" + i);
                            System.Reflection.PropertyInfo piAmount = typeof(ExpMestAggregatePrintADO).GetProperty("AMOUNT" + i);
                            System.Reflection.PropertyInfo piThAmount = typeof(ExpMestAggregatePrintADO).GetProperty("TH_AMOUNT" + i);
                            if (piServiceId != null && piAmount != null)
                            {
                                decimal? amount = trains.Where(o => o.SERVICE_ID == (long)(piServiceId.GetValue(sdo))).Sum(o => o.AMOUNT);
                                amount = (amount == 0 ? null : amount);
                                piAmount.SetValue(trainPrint, amount);
                            }

                            // qtcode
                            if (piServiceId != null && piThAmount != null)
                            {
                                decimal? thAmount = trains.Where(o => o.SERVICE_ID == (long)(piServiceId.GetValue(sdo))).Sum(o => o.TH_AMOUNT);
                                thAmount = (thAmount == 0 ? null : thAmount);
                                piThAmount.SetValue(trainPrint, thAmount);
                            }
                        }

                        trainPrints.Add(trainPrint);
                        trainPrintRoomOrders.Add(GetReqRoomOrder(reqRoomOrders, trains));
                    }
                    //Sap xep theo dung thu tu phong chi dinh hien thi tren luoi chon phong (grdRoom - AggrExpMestPrintFilter),
                    //cung phong thi theo buong -> giuong -> ten benh nhan
                    sdo.ExpMestAggregatePrintADOs = SortPatientPrints(trainPrints, trainPrintRoomOrders);

                    var trainGroupsRoom = medicineExpmestTypeADOs.GroupBy(o => new { o.Patient, o.TreatmentId,o.REQ_ROOM_ID });
                    List<ExpMestAggregatePrintADO> trainPrintsRoom = new List<ExpMestAggregatePrintADO>();
                    List<int> trainPrintRoomOrdersByRoom = new List<int>();
                    foreach (var group in trainGroupsRoom)
                    {
                        ExpMestAggregatePrintADO trainPrint = new ExpMestAggregatePrintADO();
                        List<Mps000047ADO> trains = group.ToList();

                        if (group.Key.Patient != null)
                        {
                            trainPrint.PATIENT_ID = group.Key.Patient.ID;
                            trainPrint.PATIENT_CODE = group.Key.Patient.PATIENT_CODE;
                            trainPrint.VIR_PATIENT_NAME = group.Key.Patient.VIR_PATIENT_NAME;
                            trainPrint.DOB = group.Key.Patient.DOB;
                            trainPrint.GENDER_NAME = group.Key.Patient.GENDER_NAME;
                            trainPrint.AGE = AgeUtil.CalculateFullAge(group.Key.Patient.DOB);
                            trainPrint.TREATMENT_CODE = group.FirstOrDefault().TREATMENT_CODE;
                        }
                        trainPrint.BED_ROOM_NAMEs = GetBedRoomByPatient(group.Key.TreatmentId);
                        trainPrint.REQ_ROOM_NAME = group.FirstOrDefault().REQ_ROOM_NAME;
                        string bedCode = "", bedName = "";
                        GetBedByTreatment(group.Key.TreatmentId, ref bedCode, ref bedName);
                        trainPrint.BED_CODE = bedCode;
                        trainPrint.BED_NAME = bedName;

                        for (int i = 1; i <= rdo.keyColumnSize; i++)
                        {
                            System.Reflection.PropertyInfo piServiceId = typeof(ExpMestAggregatePrintByPageADO).GetProperty("SERVICE_ID" + i);
                            System.Reflection.PropertyInfo piAmount = typeof(ExpMestAggregatePrintADO).GetProperty("AMOUNT" + i);
                            System.Reflection.PropertyInfo piThAmount = typeof(ExpMestAggregatePrintADO).GetProperty("TH_AMOUNT" + i);
                            if (piServiceId != null && piAmount != null)
                            {
                                decimal? amount = trains.Where(o => o.SERVICE_ID == (long)(piServiceId.GetValue(sdo))).Sum(o => o.AMOUNT);
                                amount = (amount == 0 ? null : amount);
                                piAmount.SetValue(trainPrint, amount);
                            }
                            if (piServiceId != null && piThAmount != null)
                            {
                                decimal? thAmount = trains.Where(o => o.SERVICE_ID == (long)(piServiceId.GetValue(sdo))).Sum(o => o.TH_AMOUNT);
                                thAmount = (thAmount == 0 ? null : thAmount);
                                piThAmount.SetValue(trainPrint, thAmount);
                            }
                        }

                        trainPrintsRoom.Add(trainPrint);
                        trainPrintRoomOrdersByRoom.Add(reqRoomOrders.ContainsKey(group.Key.REQ_ROOM_ID) ? reqRoomOrders[group.Key.REQ_ROOM_ID] : int.MaxValue);
                    }
                    //Bieu in tach theo phong: cot dau tien la phong chi dinh nen phai gom theo dung thu tu phong tren luoi chon phong
                    sdo.ExpMestAggregateReqRoomPrintADOs = SortPatientPrints(trainPrintsRoom, trainPrintRoomOrdersByRoom);
                    result.Add(sdo);
                }
            }
            return result;
        }

        private void SetSingleKey()
        {
            try
            {
                if (rdo._ExpMests_Print != null && rdo._ExpMests_Print.Count > 0)
                {
                    var minTime = rdo._ExpMests_Print.Min(p => p.TDL_INTRUCTION_TIME ?? 0);
                    var maxTime = rdo._ExpMests_Print.Max(p => p.TDL_INTRUCTION_TIME ?? 0);
                    SetSingleKey(new KeyValue(Mps000047ExtendSingleKey.MIN_USE_TIME, (rdo._ExpMests_Print.Min(p => p.TDL_USE_TIME ?? 0))));
                    SetSingleKey(new KeyValue(Mps000047ExtendSingleKey.MAX_USE_TIME, (rdo._ExpMests_Print.Max(p => p.TDL_USE_TIME ?? 0))));
                    SetSingleKey(new KeyValue(Mps000047ExtendSingleKey.MIN_INTRUCTION_DATE_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToDateString(minTime)));
                    SetSingleKey(new KeyValue(Mps000047ExtendSingleKey.MIN_INTRUCTION_TIME_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(minTime)));
                    SetSingleKey(new KeyValue(Mps000047ExtendSingleKey.MIN_INTRUCTION_DATE_SEPARATE_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(minTime)));

                    SetSingleKey(new KeyValue(Mps000047ExtendSingleKey.MAX_INTRUCTION_TIME_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(maxTime)));
                    SetSingleKey(new KeyValue(Mps000047ExtendSingleKey.MAX_INTRUCTION_DATE_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToDateString(maxTime)));
                    SetSingleKey(new KeyValue(Mps000047ExtendSingleKey.MAX_INTRUCTION_DATE_SEPARATE_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(maxTime)));
                }
                AddObjectKeyIntoListkey<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST>(rdo.AggrExpMest, false);
                AddObjectKeyIntoListkey<MOS.EFMODEL.DataModels.HIS_DEPARTMENT>(rdo.Department, false);

                SetSingleKey(new KeyValue(Mps000047ExtendSingleKey.CREATE_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(rdo.AggrExpMest.CREATE_TIME ?? 0)));
                SetSingleKey(new KeyValue(Mps000047ExtendSingleKey.CREATE_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.AggrExpMest.CREATE_TIME ?? 0)));
                SetSingleKey(new KeyValue(Mps000047ExtendSingleKey.CREATE_DATE_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(rdo.AggrExpMest.CREATE_TIME ?? 0)));

                SetSingleKey(new KeyValue(Mps000047ExtendSingleKey.TimeFilterOption, rdo.TimeFilterOption ));
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
                if (rdo != null && rdo._ExpMests_Print != null)
                {
                    List<string> treatmentCodes = rdo._ExpMests_Print.Select(s => s.TDL_TREATMENT_CODE).Distinct().ToList();
                    List<string> expMestCodes = rdo._ExpMests_Print.Select(s => s.EXP_MEST_CODE).Distinct().ToList();
                    log = LogDataExpMest(string.Join(";", treatmentCodes), string.Join(";", expMestCodes), "");
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
                if (rdo != null && rdo.AggrExpMest != null && rdo._ExpMests_Print != null)
                    result = String.Format("{0}_{1}_{2}_{3}_{4}_{5}_{6}", printTypeCode, rdo.AggrExpMest.EXP_MEST_CODE, rdo.AggrExpMest.MEDI_STOCK_CODE, rdo._ExpMests_Print.Count(), "Phiếu lĩnh", rdo.MedicineExpmestTypeADOs.FirstOrDefault().MEDICINE_TYPE_CODE, rdo.MedicineExpmestTypeADOs.Count());
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
