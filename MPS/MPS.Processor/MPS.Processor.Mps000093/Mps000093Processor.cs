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
using MPS.ProcessorBase.Core;
using SAR.EFMODEL.DataModels;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.IO;
using MPS.Processor.Mps000093.PDO;
using System.Linq;

namespace MPS.Processor.Mps000093
{
    class Mps000093Processor : AbstractProcessor
    {
        Mps000093PDO rdo;
        List<Mps000093ADO> ImpMestManuMedicineSumForPrints = new List<Mps000093ADO>();
        List<ImpMestAggregatePrintByPageADO> result;
        public Mps000093Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000093PDO)rdoBase;
        }

        private void SetBarcodeKey()
        {
            try
            {
                Inventec.Common.BarcodeLib.Barcode barcodePatientCode = new Inventec.Common.BarcodeLib.Barcode(rdo.AggrImpMest.IMP_MEST_CODE);
                barcodePatientCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcodePatientCode.IncludeLabel = false;
                barcodePatientCode.Width = 120;
                barcodePatientCode.Height = 40;
                barcodePatientCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                barcodePatientCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcodePatientCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcodePatientCode.IncludeLabel = true;

                dicImage.Add(Mps000093ExtendSingleKey.IMP_MEST_CODE_BAR, barcodePatientCode);

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
                result = rdo.MedicineImpmestTypeADOs.FirstOrDefault(o => o.SERVICE_ID == serviceId).MEDICINE_TYPE_NAME;
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
                var treatmentBedRooms = rdo.vHisTreatmentBedRoom.Where(o => o.TREATMENT_ID == treatmentId).ToList();
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

        List<ImpMestAggregatePrintByPageADO> PrepareData(List<Mps000093ADO> medicineImpmestTypeADOs)
        {
            result = null;
            if (medicineImpmestTypeADOs != null && medicineImpmestTypeADOs.Count > 0)
            {
                result = new List<ImpMestAggregatePrintByPageADO>();
                List<long> distinctDates = medicineImpmestTypeADOs.OrderBy(o => o.MEDICINE_TYPE_NAME)
                    .Select(o => o.SERVICE_ID)
                    .Distinct().ToList();
                var trainGroups = medicineImpmestTypeADOs.GroupBy(o => new { o.Patient, o.TreatmentId });
                int index = 0;
                while (index < distinctDates.Count)
                {
                    ImpMestAggregatePrintByPageADO sdo = new ImpMestAggregatePrintByPageADO();

                    for (int i = 1; i <= 44; i++)
                    {
                        System.Reflection.PropertyInfo piMedicineTypeName = typeof(ImpMestAggregatePrintByPageADO).GetProperty("MEDICINE_TYPE_NAME" + i);
                        if (piMedicineTypeName != null)
                            piMedicineTypeName.SetValue(sdo, index < distinctDates.Count ? GetMedicineTypeName(distinctDates[index]) : "");

                        System.Reflection.PropertyInfo piMedicineTypeId = typeof(ImpMestAggregatePrintByPageADO).GetProperty("SERVICE_ID" + i);
                        if (piMedicineTypeId != null)
                            piMedicineTypeId.SetValue(sdo, index < distinctDates.Count ? distinctDates[index] : 0);

                        index++;
                    }

                    List<ImpMestAggregatePrintADO> trainPrints = new List<ImpMestAggregatePrintADO>();
                    foreach (var group in trainGroups)
                    {
                        ImpMestAggregatePrintADO trainPrint = new ImpMestAggregatePrintADO();
                        List<Mps000093ADO> trains = group.ToList();

                        if (group.Key.Patient != null)
                        {
                            trainPrint.PATIENT_ID = group.Key.Patient.ID;
                            trainPrint.PATIENT_CODE = group.Key.Patient.PATIENT_CODE;
                            trainPrint.VIR_PATIENT_NAME = group.Key.Patient.VIR_PATIENT_NAME;
                            trainPrint.AGE = AgeUtil.CalculateFullAge(group.Key.Patient.DOB);
                        }
                        trainPrint.IS_BHYT = group.FirstOrDefault().IS_BHYT;
                        trainPrint.TREATMENT_CODE = group.FirstOrDefault().TREATMENT_CODE;
                        var assignRooms = rdo.vHisRoom.Where(o => o.ID == group.FirstOrDefault().REQ_ROOM_ID).ToList();
                        //
                        if (assignRooms != null && assignRooms.Count > 0)
                        {
                            trainPrint.BED_ROOM_NAMEs = assignRooms.FirstOrDefault().ROOM_NAME;
                        }
                        else
                        {
                            trainPrint.BED_ROOM_NAMEs = GetBedRoomByPatient(group.Key.TreatmentId);
                        }

                        for (int i = 1; i <= 44; i++)
                        {
                            System.Reflection.PropertyInfo piServiceId = typeof(ImpMestAggregatePrintByPageADO).GetProperty("SERVICE_ID" + i);
                            System.Reflection.PropertyInfo piAmount = typeof(ImpMestAggregatePrintADO).GetProperty("AMOUNT" + i);
                            if (piServiceId != null && piAmount != null)
                            {
                                decimal? amount = trains.Where(o => o.SERVICE_ID == (long)(piServiceId.GetValue(sdo))).Sum(o => o.AMOUNT);
                                amount = (amount == 0 ? null : amount);
                                piAmount.SetValue(trainPrint, amount);
                            }
                        }

                        trainPrints.Add(trainPrint);
                    }
                    sdo.ImpMestAggregatePrintADOs = trainPrints.OrderBy(o => o.BED_ROOM_NAMEs).ThenBy(o => o.VIR_PATIENT_NAME).ToList();
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
        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                ImpMestManuMedicineSumForPrints = new List<Mps000093ADO>();
                var query = rdo.MedicineImpmestTypeADOs.AsQueryable();

                query = query
                      .Where(o =>
                      (
                          (rdo.IsMedicine
                              && o.IS_MEDICINE == true
                              && rdo.ServiceUnitIds.Contains(o.SERVICE_UNIT_ID)
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

                var impMestManuMedicineTemps = query.ToList();
                if (impMestManuMedicineTemps == null || impMestManuMedicineTemps.Count == 0)
                {
                    throw new ArgumentNullException("expMestManuMedicineTemps after filter query is null");
                }

                List<ImpMestAggregatePrintByPageADO> impMestAggregatePrintByPageADOs = PrepareData(impMestManuMedicineTemps);
                if (impMestAggregatePrintByPageADOs == null || impMestAggregatePrintByPageADOs.Count == 0)
                {
                    throw new ArgumentNullException("Get PrepareData - expMestAggregatePrintByPageADOs is null");
                }
                foreach (var item in impMestAggregatePrintByPageADOs)
                {
                    for (int i = 1; i <= 44; i++)
                    {
                        System.Reflection.PropertyInfo piMedicineTypeName = typeof(ImpMestAggregatePrintByPageADO).GetProperty("MEDICINE_TYPE_NAME" + i);
                        singleValueDictionary.Add("MEDICINE_TYPE_NAME" + i, piMedicineTypeName.GetValue(item));
                    }

                    Inventec.Common.FlexCellExport.Store store = new Inventec.Common.FlexCellExport.Store();

                    Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                    Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                    Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                    SetBarcodeKey();
                    SetSingleKey();
                    store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                    singleTag.ProcessData(store, singleValueDictionary);
                    barCodeTag.ProcessData(store, dicImage);
                    objectTag.AddObjectData(store, "Patients", item.ImpMestAggregatePrintADOs);
                    store.SetCommonFunctions();

                    var streamResult = store.OutStream();
                    if (streamResult != null && streamResult.Length > 0)
                    {
                        streamResult.Position = 0;

                        //Print preview               
                        result = PrintPreview(streamResult, this.fileName, store.DictionaryTemplateKey);
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
        private void SetSingleKey()
        {
            try
            {
                //if (this.rdo.MedicineImpmestTypeADOs != null && this.rdo.MedicineImpmestTypeADOs.Count > 0)
                //{
                //long minUserTime = this.rdo.MedicineImpmestTypeADOs.Min(o => o.INTRUCTION_TIME);
                //long maxUserTime = this.rdo.MedicineImpmestTypeADOs.Max(o => o.INTRUCTION_TIME);
                //SetSingleKey(new KeyValue(Mps000093ExtendSingleKey.USER_TIME_MEDICINE, Inventec.Common.DateTime.Convert.TimeNumberToDateString(minUserTime)));
                //SetSingleKey(new KeyValue(Mps000093ExtendSingleKey.USER_TIME_TO_MEDICINE, Inventec.Common.DateTime.Convert.TimeNumberToDateString(maxUserTime)));
                //}

                AddObjectKeyIntoListkey<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST>(rdo.AggrImpMest, false);
                AddObjectKeyIntoListkey<MOS.EFMODEL.DataModels.HIS_DEPARTMENT>(rdo.Department, false);

                SetSingleKey(new KeyValue(Mps000093ExtendSingleKey.CREATE_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.AggrImpMest.CREATE_TIME ?? 0)));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

