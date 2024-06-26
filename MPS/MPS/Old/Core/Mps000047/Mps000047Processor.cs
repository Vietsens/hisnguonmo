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
using MPS.ADO;
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace MPS.Core.Mps000047
{
    class Mps000047Processor : ProcessorBase, IProcessorPrint
    {
        Mps000047RDO rdo;
        Inventec.Common.FlexCellExport.Store store;
        string fileName;
        internal Dictionary<string, Inventec.Common.BarcodeLib.Barcode> dicImage = null;
        List<ExpMestMedicinePrintADO> ExpMestManuMedicineSumForPrints = new List<ExpMestMedicinePrintADO>();
        List<ExpMestAggregatePrintByPageADO> result;
        internal Mps000047Processor(SAR_PRINT_TYPE config, string fileName, object data, MPS.Printer.PreviewType previewType, string printerName)
            : base(config, (RDOBase)data, previewType, printerName)
        {
            rdo = (Mps000047RDO)data;
            this.fileName = fileName;
            store = new Inventec.Common.FlexCellExport.Store();
            dicImage = new Dictionary<string, Inventec.Common.BarcodeLib.Barcode>();
        }

        bool IProcessorPrint.Run()
        {
            bool result = false;
            try
            {
                SetCommonSingleKey();
                rdo.SetSingleKey();
                SetSingleKey();

                //Cac ham dac thu khac cua rdo
                SetBarcodeKey();

                store.SetCommonFunctions();

                //Ham xu ly cac doi tuong su dung trong thu vien flexcelexport
                ProcessData();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        void SetBarcodeKey()
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
                var treatmentBedRooms = MPS.PrintConfig.VHisTreatmentBedRooms.Where(o => o.TREATMENT_ID == treatmentId).ToList();
                if (treatmentBedRooms != null && treatmentBedRooms.Count > 0)
                {
                    result = treatmentBedRooms[0].BED_ROOM_NAME;
                    //var bedRoomNames = treatmentBedRooms.Select(o => o.BED_ROOM_NAME).Distinct().ToArray();
                    //result = string.Join(";", bedRoomNames);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return result;
        }

        List<ExpMestAggregatePrintByPageADO> PrepareData(List<ExpMestMedicinePrintADO> medicineExpmestTypeADOs)
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
                        List<ExpMestMedicinePrintADO> trains = group.ToList();

                        if (group.Key.Patient != null)
                        {
                            trainPrint.PATIENT_ID = group.Key.Patient.ID;
                            trainPrint.PATIENT_CODE = group.Key.Patient.PATIENT_CODE;
                            trainPrint.VIR_PATIENT_NAME = group.Key.Patient.VIR_PATIENT_NAME;
                            trainPrint.FIRST_NAME = group.Key.Patient.FIRST_NAME;
                            trainPrint.AGE = AgeUtil.CalculateFullAge(group.Key.Patient.DOB);
                            trainPrint.TREATMENT_CODE = group.FirstOrDefault().TREATMENT_CODE;
                        }
                        trainPrint.BED_ROOM_NAMEs = GetBedRoomByPatient(group.Key.TreatmentId);

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
                    sdo.ExpMestAggregatePrintADOs = trainPrints.OrderBy(o => o.BED_ROOM_NAMEs).ThenBy(o => o.FIRST_NAME).ToList();
                    result.Add(sdo);
                }
            }
            return result;
        }

        /// <summary>
        /// Ham xu ly du lieu da qua xu ly
        /// Tao ra cac doi tuong du lieu xu dung trong thu vien xu ly file excel
        /// </summary>
        /// <returns></returns>
        protected bool ProcessData()
        {
            bool result = false;
            try
            {
                ExpMestManuMedicineSumForPrints = new List<ExpMestMedicinePrintADO>();
                List<ExpMestMedicinePrintADO> lstPrintADO = new List<ExpMestMedicinePrintADO>();
                if (rdo.AggrExpMest.EXP_MEST_STT_ID == rdo.HisExpMestSttId__Exported || rdo.AggrExpMest.EXP_MEST_STT_ID == rdo.HisExpMestSttId__Approved)
                {
                    lstPrintADO = rdo.MedicineExpmestTypeADOs.Where(p => p.IN_EXECUTE == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_MEDICINE.IN_EXECUTE__TRUE).ToList();
                }
                else if (rdo.AggrExpMest.EXP_MEST_STT_ID == rdo.HisExpMestSttId__Draft || rdo.AggrExpMest.EXP_MEST_STT_ID == rdo.HisExpMestSttId__Request || rdo.AggrExpMest.EXP_MEST_STT_ID == rdo.HisExpMestSttId__Rejected)
                {
                    lstPrintADO = rdo.MedicineExpmestTypeADOs.Where(p => p.IN_REQUEST == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_MEDICINE.IN_REQUEST__TRUE).ToList();
                }
                var query = lstPrintADO.AsQueryable();

                query = query
                      .Where(o =>
                      (
                          (rdo.IsMedicine
                              && o.IS_MEDICINE == true
                              && rdo.ServiceUnitIds.Contains(o.SERVICE_UNIT_ID)
                              && (o.MEDICINE_USE_FORM_ID == null || (o.MEDICINE_USE_FORM_ID != null && rdo.UseFormIds.Contains(o.MEDICINE_USE_FORM_ID ?? 0)))
                          )
                          || (rdo.Ismaterial
                              && o.IS_MEDICINE == false
                              && rdo.ServiceUnitIds.Contains(o.SERVICE_UNIT_ID)
                          )
                      )
                    );
                if (rdo.RoomIds != null && rdo.RoomIds.Count > 0)
                {
                    query = query.Where(o => rdo.RoomIds.Contains(o.REQ_ROOM_ID ?? 0));
                }

                var expMestManuMedicineTemps = query.ToList();
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
                    Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                    Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();

                    Inventec.Common.FlexCellExport.Store store = new Inventec.Common.FlexCellExport.Store();
                    store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                    singleTag.ProcessData(store, singleValueDictionary);
                    barCodeTag.ProcessData(store, dicImage);
                    objectTag.AddObjectData(store, "Patients", item.ExpMestAggregatePrintADOs);
                    store.SetCommonFunctions();

                    var streamResult = store.OutStream();
                    if (streamResult != null && streamResult.Length > 0)
                    {
                        streamResult.Position = 0;

                        //Print preview               
                        result = PrintPreview(streamResult, this.fileName);
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Warn("store.OutStream is null");
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return result;
        }
    }
}
