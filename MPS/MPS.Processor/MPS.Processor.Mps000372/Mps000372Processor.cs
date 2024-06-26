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
using MPS.Processor.Mps000372.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MPS.Processor.Mps000372
{
    public class Mps000372Processor : AbstractProcessor
    {
        Mps000372PDO rdo;
        List<ExpMestAggregatePrintByPageADO> result;
        public Mps000372Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000372PDO)rdoBase;
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

                dicImage.Add(Mps000372ExtendSingleKey.EXP_MEST_CODE_BAR, barcodePatientCode);
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
                    for (int i = 1; i <= 44; i++)
                    {
                        System.Reflection.PropertyInfo piMedicineTypeName = typeof(ExpMestAggregatePrintByPageADO).GetProperty("MEDICINE_TYPE_NAME" + i);
                        singleValueDictionary.Add("MEDICINE_TYPE_NAME" + i, piMedicineTypeName.GetValue(item));
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
                    objectTag.AddObjectData(store, "Patients", item.ExpMestAggregatePrintADOs);
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

        string GetBedRoomByPatient(long treatmentId)
        {
            string result = String.Empty;
            try
            {
                var treatmentBedRooms = rdo.vHisTreatmentBedRooms.Where(o => o.TREATMENT_ID == treatmentId).ToList();
                if (treatmentBedRooms != null && treatmentBedRooms.Count > 0)
                {
                    result = treatmentBedRooms[0].BED_ROOM_NAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return result;
        }

        //bed_log có start_time lớn nhất và có service_req_id
        //có service_req_id là đã được chỉ đinh
        //còn chưa có thì mới gán
        //chưa tính tiền
        void GetBedByTreatment(long treatmentId, ref string BedCode, ref string BedName)
        {
            if (rdo._listBedLog == null || rdo._listBedLog.Count == 0)
                return;
            try
            {
                var bedLog = rdo._listBedLog
                    .Where(o => o.TREATMENT_ID == treatmentId && o.SERVICE_REQ_ID.HasValue)
                    .OrderByDescending(o => o.START_TIME).ToList();

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

        List<ExpMestAggregatePrintByPageADO> PrepareData(List<Mps000372ADO> medicineExpmestTypeADOs)
        {
            result = null;
            if (medicineExpmestTypeADOs != null && medicineExpmestTypeADOs.Count > 0)
            {
                result = new List<ExpMestAggregatePrintByPageADO>();
                List<long> distinctDates = medicineExpmestTypeADOs.OrderBy(o => o.MEDICINE_TYPE_NAME)
                    .Select(o => o.SERVICE_ID)
                    .Distinct().ToList();
                var trainGroups = medicineExpmestTypeADOs.GroupBy(o => new { o.Patient, o.TreatmentId });
                int index = 0;
                while (index < distinctDates.Count)
                {
                    ExpMestAggregatePrintByPageADO sdo = new ExpMestAggregatePrintByPageADO();

                    for (int i = 1; i <= 44; i++)
                    {
                        System.Reflection.PropertyInfo piMedicineTypeName = typeof(ExpMestAggregatePrintByPageADO).GetProperty("MEDICINE_TYPE_NAME" + i);
                        if (piMedicineTypeName != null)
                            piMedicineTypeName.SetValue(sdo, index < distinctDates.Count ? GetMedicineTypeName(distinctDates[index]) : "");

                        System.Reflection.PropertyInfo piMedicineTypeId = typeof(ExpMestAggregatePrintByPageADO).GetProperty("SERVICE_ID" + i);
                        if (piMedicineTypeId != null)
                            piMedicineTypeId.SetValue(sdo, index < distinctDates.Count ? distinctDates[index] : 0);

                        index++;
                    }

                    List<ExpMestAggregatePrintADO> trainPrints = new List<ExpMestAggregatePrintADO>();
                    foreach (var group in trainGroups)
                    {
                        ExpMestAggregatePrintADO trainPrint = new ExpMestAggregatePrintADO();
                        List<Mps000372ADO> trains = group.ToList();

                        if (group.Key.Patient != null)
                        {
                            trainPrint.PATIENT_ID = group.Key.Patient.ID;
                            trainPrint.PATIENT_CODE = group.Key.Patient.PATIENT_CODE;
                            trainPrint.VIR_PATIENT_NAME = group.Key.Patient.VIR_PATIENT_NAME;
                            trainPrint.AGE = AgeUtil.CalculateFullAge(group.Key.Patient.DOB);
                            trainPrint.TREATMENT_CODE = group.FirstOrDefault().TREATMENT_CODE;
                        }
                        trainPrint.BED_ROOM_NAMEs = GetBedRoomByPatient(group.Key.TreatmentId);
                        string bedCode = "", bedName="";
                        GetBedByTreatment(group.Key.TreatmentId, ref bedCode, ref bedName);
                        trainPrint.BED_CODE = bedCode;
                        trainPrint.BED_NAME = bedName;

                        for (int i = 1; i <= 44; i++)
                        {
                            System.Reflection.PropertyInfo piServiceId = typeof(ExpMestAggregatePrintByPageADO).GetProperty("SERVICE_ID" + i);
                            System.Reflection.PropertyInfo piAmount = typeof(ExpMestAggregatePrintADO).GetProperty("AMOUNT" + i);
                            if (piServiceId != null && piAmount != null)
                            {
                                decimal? amount = trains.Where(o => o.SERVICE_ID == (long)(piServiceId.GetValue(sdo))).Sum(o => o.AMOUNT);
                                amount = (amount == 0 ? null : amount);
                                piAmount.SetValue(trainPrint, amount);
                            }
                        }

                        trainPrints.Add(trainPrint);
                    }
                    sdo.ExpMestAggregatePrintADOs = trainPrints.OrderBy(o => o.BED_ROOM_NAMEs).ThenBy(o => o.VIR_PATIENT_NAME).ToList();
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
                    SetSingleKey(new KeyValue(Mps000372ExtendSingleKey.MIN_INTRUCTION_DATE_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToDateString(minTime)));
                    SetSingleKey(new KeyValue(Mps000372ExtendSingleKey.MIN_INTRUCTION_TIME_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(minTime)));
                    SetSingleKey(new KeyValue(Mps000372ExtendSingleKey.MIN_INTRUCTION_DATE_SEPARATE_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(minTime)));

                    SetSingleKey(new KeyValue(Mps000372ExtendSingleKey.MAX_INTRUCTION_TIME_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(maxTime)));
                    SetSingleKey(new KeyValue(Mps000372ExtendSingleKey.MAX_INTRUCTION_DATE_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToDateString(maxTime)));
                    SetSingleKey(new KeyValue(Mps000372ExtendSingleKey.MAX_INTRUCTION_DATE_SEPARATE_DISPLAY, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(maxTime)));
                }
                AddObjectKeyIntoListkey<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST>(rdo.AggrExpMest, false);
                AddObjectKeyIntoListkey<MOS.EFMODEL.DataModels.HIS_DEPARTMENT>(rdo.Department, false);

                SetSingleKey(new KeyValue(Mps000372ExtendSingleKey.CREATE_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(rdo.AggrExpMest.CREATE_TIME ?? 0)));
                SetSingleKey(new KeyValue(Mps000372ExtendSingleKey.CREATE_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.AggrExpMest.CREATE_TIME ?? 0)));
                SetSingleKey(new KeyValue(Mps000372ExtendSingleKey.CREATE_DATE_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(rdo.AggrExpMest.CREATE_TIME ?? 0)));
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
