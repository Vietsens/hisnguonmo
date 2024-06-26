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
using HIS.Desktop.Common.BankQrCode;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.LibraryHein.Bhyt;
using MPS.Processor.Mps000044.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MPS.Processor.Mps000044
{
    class Mps000044Processor : AbstractProcessor
    {
        private Mps000044PDO rdo;
        private List<ExpMestMedicineSDO> expMestMedicinesTYPE;
        private List<ExpMestMedicineSDO> expMestMedicines_Sort;

        public Mps000044Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000044PDO)rdoBase;
            //if (rdoBase != null && rdoBase is Mps000044PDO)
            //{
            //    rdo = (Mps000044PDO)rdoBase;
            //}
            //else
            //{
            //    rdo = DeserializeObjectJson<Mps000044PDO>(this.printDataBase.data);
            //}
        }

        private void SetBarcodeKey()
        {
            try
            {
                if (rdo.vHisPrescription5 != null)
                {
                    if (!String.IsNullOrEmpty(rdo.vHisPrescription5.TDL_TREATMENT_CODE))
                    {
                        Inventec.Common.BarcodeLib.Barcode barcodeTreatmentCode = new Inventec.Common.BarcodeLib.Barcode(rdo.vHisPrescription5.TDL_TREATMENT_CODE);
                        barcodeTreatmentCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                        barcodeTreatmentCode.IncludeLabel = false;
                        barcodeTreatmentCode.Width = 120;
                        barcodeTreatmentCode.Height = 40;
                        barcodeTreatmentCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                        barcodeTreatmentCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                        barcodeTreatmentCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                        barcodeTreatmentCode.IncludeLabel = true;

                        dicImage.Add(Mps000044ExtendSingleKey.TREATMENT_CODE_BARCODE, barcodeTreatmentCode);
                    }

                    if (!String.IsNullOrEmpty(rdo.vHisPrescription5.SERVICE_REQ_CODE))
                    {
                        Inventec.Common.BarcodeLib.Barcode barcodeTreatmentCode = new Inventec.Common.BarcodeLib.Barcode(rdo.vHisPrescription5.SERVICE_REQ_CODE);
                        barcodeTreatmentCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                        barcodeTreatmentCode.IncludeLabel = false;
                        barcodeTreatmentCode.Width = 120;
                        barcodeTreatmentCode.Height = 40;
                        barcodeTreatmentCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                        barcodeTreatmentCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                        barcodeTreatmentCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                        barcodeTreatmentCode.IncludeLabel = true;

                        dicImage.Add(Mps000044ExtendSingleKey.SERVICE_REQ_CODE_BARCODE, barcodeTreatmentCode);
                    }

                    if (!String.IsNullOrEmpty(rdo.Mps000044ADO.EXP_MEST_CODE))
                    {
                        Inventec.Common.BarcodeLib.Barcode expMestCodeBarCode = new Inventec.Common.BarcodeLib.Barcode(rdo.Mps000044ADO.EXP_MEST_CODE);
                        expMestCodeBarCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                        expMestCodeBarCode.IncludeLabel = false;
                        expMestCodeBarCode.Width = 120;
                        expMestCodeBarCode.Height = 40;
                        expMestCodeBarCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                        expMestCodeBarCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                        expMestCodeBarCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                        expMestCodeBarCode.IncludeLabel = true;

                        dicImage.Add(Mps000044ExtendSingleKey.EXP_MEST_CODE_BARCODE, expMestCodeBarCode);
                    }

                    if (!String.IsNullOrEmpty(rdo.vHisPrescription5.TDL_PATIENT_CODE))
                    {
                        Inventec.Common.BarcodeLib.Barcode barcodePatient = new Inventec.Common.BarcodeLib.Barcode(rdo.vHisPrescription5.TDL_PATIENT_CODE);
                        barcodePatient.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                        barcodePatient.IncludeLabel = false;
                        barcodePatient.Width = 120;
                        barcodePatient.Height = 40;
                        barcodePatient.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                        barcodePatient.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                        barcodePatient.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                        barcodePatient.IncludeLabel = true;

                        dicImage.Add(Mps000044ExtendSingleKey.PATIENT_CODE_BARCODE, barcodePatient);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //Lọc thuốc theo thứ tự
        private void MedicinesSort()
        {
            try
            {
                expMestMedicines_Sort = new List<ExpMestMedicineSDO>();
                if (rdo.expMestMedicines != null && rdo.expMestMedicines.Count > 0)
                {
                    Inventec.Common.Logging.LogSystem.Warn("MPS000044____________" + rdo.KeyUseForm);
                    if (rdo.KeyUseForm == 1)
                    {

                        expMestMedicines_Sort = rdo.expMestMedicines.OrderByDescending(p => p.MEDICINE_USE_FORM_NUM_ORDER).ThenBy(o => o.NUM_ORDER).ToList();
                    }
                    else if (rdo.KeyUseForm == 2)
                    {
                        expMestMedicines_Sort = rdo.expMestMedicines.OrderBy(p => p.MEDICINE_USE_FORM_NUM_ORDER).ThenBy(o => o.NUM_ORDER).ToList();
                    }
                    else
                    {
                        expMestMedicines_Sort = rdo.expMestMedicines.ToList();
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
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                SetSingleKey();
                ProcessListData();
                SetQrCode();
                //ghi đè PrintLogData và UniqueCodeData
                ProcessPrintLogData();
                //lấy số lần in
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));

                SetTreatmentQrCodeBase();
                this.SetSignatureKeyImageByCFG();
                MedicinesSort();

                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => expMestMedicines_Sort), expMestMedicines_Sort));
                if (expMestMedicinesTYPE == null)
                    expMestMedicinesTYPE = new List<ExpMestMedicineSDO>();
                objectTag.AddObjectData(store, "type", expMestMedicinesTYPE);
                objectTag.AddObjectData(store, "ServiceMedicines", expMestMedicines_Sort);
                objectTag.AddRelationship(store, "type", "ServiceMedicines", "PATIENT_TYPE_NAME", "PATIENT_TYPE_NAME");

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        public override string ProcessPrintLogData()
        {
            string log = "";
            try
            {
                log = this.LogDataExpMests(rdo.vHisPrescription5.TDL_TREATMENT_CODE, rdo.vHisPrescription5.SERVICE_REQ_CODE, "");
            }
            catch (Exception ex)
            {
                log = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return log;
        }

        private string LogDataExpMests(string treatmentCode, string expMestCodes, string message)
        {
            string result = "";
            try
            {
                result += message + ". ";

                if (!String.IsNullOrWhiteSpace(treatmentCode))
                {
                    result += string.Format("TREATMENT_CODE: {0}. ", treatmentCode);
                }

                result += string.Format("EXP_MEST_CODE: {0}. ", expMestCodes);
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        public override string ProcessUniqueCodeData()
        {
            string result = "";
            try
            {
                if (rdo != null && rdo.vHisPrescription5 != null)
                {
                    string treatmentCode = "TREATMENT_CODE:" + rdo.vHisPrescription5.TDL_TREATMENT_CODE;
                    string serviceReqCode = "SERVICE_REQ_CODE:" + rdo.vHisPrescription5.SERVICE_REQ_CODE;
                    string serviceCode = "";
                    if (rdo.expMestMedicines != null && rdo.expMestMedicines.Count > 0)
                    {
                        var serviceFirst = rdo.expMestMedicines.OrderBy(o => o.MEDICINE_TYPE_CODE).First();
                        serviceCode = "SERVICE_CODE:" + serviceFirst.MEDICINE_TYPE_CODE;
                    }

                    result = String.Format("{0} {1} {2} {3}", printTypeCode, treatmentCode, serviceReqCode, serviceCode);
                }
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void ProcessListData()
        {
            try
            {
                if (rdo.expMestMedicines != null && rdo.expMestMedicines.Count > 0)
                {
                    var groupType = rdo.expMestMedicines.GroupBy(o => o.PATIENT_TYPE_ID).ToList();
                    expMestMedicinesTYPE = new List<ExpMestMedicineSDO>();
                    foreach (var item in groupType)
                    {
                        ExpMestMedicineSDO ado = new ExpMestMedicineSDO();
                        ado.PATIENT_TYPE_ID = item.First().PATIENT_TYPE_ID;
                        ado.PATIENT_TYPE_NAME = item.First().PATIENT_TYPE_NAME;
                        expMestMedicinesTYPE.Add(ado);
                    }

                    expMestMedicinesTYPE = expMestMedicinesTYPE.OrderBy(o => o.PATIENT_TYPE_ID).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void SetSingleKey()
        {
            try
            {

                double? UseDayMax = null;
                if (rdo.vHisPrescription5 != null)
                {
                    if (rdo.expMestMedicines != null && rdo.expMestMedicines.Count > 0)
                    {
                        decimal tong = 0, PriceExpend = 0;

                        int dem = 0;
                        foreach (var item in rdo.expMestMedicines)
                        {
                            tong += item.AMOUNT * ((item.PRICE ?? 0) * (1 + (item.VAT_RATIO ?? 0)));
                            if (item.IS_EXPEND != 1)
                            {
                                PriceExpend += item.AMOUNT * ((item.PRICE ?? 0) * (1 + (item.VAT_RATIO ?? 0)));
                            }
                            else
                            {
                                PriceExpend += 0;
                            }
                            if (item.EXP_MEST_ID > 0)
                            {
                                DateTime IntructionTime = Inventec.Common.TypeConvert.Parse.ToDateTime(Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.vHisPrescription5.INTRUCTION_TIME));
                                DateTime UseTimeTo = Inventec.Common.TypeConvert.Parse.ToDateTime(Inventec.Common.DateTime.Convert.TimeNumberToTimeString(item.USE_TIME_TO ?? 0));
                                Inventec.Common.Logging.LogSystem.Debug("UseTimeTo: " + UseTimeTo + " IntructionTime: " + IntructionTime);

                                TimeSpan diff = UseTimeTo - IntructionTime;
                                var UseDay = diff.TotalDays + 1;

                                double chekuse = UseDayMax ?? 0;

                                if (UseDay > chekuse)
                                {
                                    UseDayMax = UseDay;
                                }
                            }

                            if (item.Type == 2)
                            {
                                dem++;
                            }
                        }

                        if (dem == rdo.expMestMedicines.Count)
                        {
                            UseDayMax = null;
                        }
                        SetSingleKey(new KeyValue(Mps000044ExtendSingleKey.PRICE_PRESCRIPTION_EXPEND, PriceExpend));
                        SetSingleKey(new KeyValue(Mps000044ExtendSingleKey.TOTAL_PRICE_PRESCRIPTION, tong));
                    }

                    long useTimeTo = 0;
                    var maxUseTimeTo = rdo.expMestMedicines.Where(o => o.USE_TIME_TO.HasValue).ToList();
                    if (maxUseTimeTo != null && maxUseTimeTo.Count > 0)
                    {
                        useTimeTo = maxUseTimeTo.Max(m => m.USE_TIME_TO).Value;
                    }

                    if (!rdo.vHisPrescription5.USE_TIME_TO.HasValue && useTimeTo > 0)
                    {
                        SetSingleKey(new KeyValue(Mps000044ExtendSingleKey.USE_TIME_TO_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(useTimeTo)));
                    }
                    else
                        SetSingleKey(new KeyValue(Mps000044ExtendSingleKey.USE_TIME_TO_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.vHisPrescription5.USE_TIME_TO ?? 0)));

                    if (useTimeTo > 0)
                    {
                        SetSingleKey(new KeyValue(Mps000044ExtendSingleKey.DETAIL_MAX_USE_TIME_TO_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(useTimeTo)));
                    }

                    if (rdo.vHisPrescription5.USE_TIME.HasValue)
                    {
                        SetSingleKey(new KeyValue(Mps000044ExtendSingleKey.USER_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.vHisPrescription5.USE_TIME ?? 0)));
                    }
                    else
                    {
                        SetSingleKey(new KeyValue(Mps000044ExtendSingleKey.USER_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.vHisPrescription5.INTRUCTION_TIME)));
                    }

                    SetSingleKey(new KeyValue(Mps000044ExtendSingleKey.INTRUCTION_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.vHisPrescription5.INTRUCTION_TIME)));
                    SetSingleKey(new KeyValue(Mps000044ExtendSingleKey.INTRUCTION_TIME_FULL_SRT, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(rdo.vHisPrescription5.INTRUCTION_TIME)));

                    SetSingleKey((new KeyValue(Mps000044ExtendSingleKey.AGE, AgeUtil.CalculateFullAge(rdo.vHisPrescription5.TDL_PATIENT_DOB))));
                    SetSingleKey((new KeyValue(Mps000044ExtendSingleKey.DOB_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.vHisPrescription5.TDL_PATIENT_DOB))));
                    SetSingleKey((new KeyValue(Mps000044ExtendSingleKey.D_O_B, rdo.vHisPrescription5.TDL_PATIENT_DOB.ToString().Length >= 4 ? rdo.vHisPrescription5.TDL_PATIENT_DOB.ToString().Substring(0, 4) : "0")));

                    SetSingleKey(new KeyValue(Mps000044ExtendSingleKey.NATIONAL_NAME, rdo.vHisPrescription5.TDL_PATIENT_NATIONAL_NAME));
                    SetSingleKey(new KeyValue(Mps000044ExtendSingleKey.WORK_PLACE, rdo.vHisPrescription5.TDL_PATIENT_WORK_PLACE_NAME));
                    SetSingleKey(new KeyValue(Mps000044ExtendSingleKey.ADDRESS, rdo.vHisPrescription5.TDL_PATIENT_ADDRESS));
                    SetSingleKey(new KeyValue(Mps000044ExtendSingleKey.CAREER_NAME, rdo.vHisPrescription5.TDL_PATIENT_CAREER_NAME));
                    SetSingleKey(new KeyValue(Mps000044ExtendSingleKey.PATIENT_CODE, rdo.vHisPrescription5.TDL_PATIENT_CODE));
                    SetSingleKey(new KeyValue(Mps000044ExtendSingleKey.DISTRICT_CODE, rdo.vHisPrescription5.TDL_PATIENT_DISTRICT_CODE));
                    SetSingleKey(new KeyValue(Mps000044ExtendSingleKey.GENDER_NAME, rdo.vHisPrescription5.TDL_PATIENT_GENDER_NAME));
                    SetSingleKey(new KeyValue(Mps000044ExtendSingleKey.MILITARY_RANK_NAME, rdo.vHisPrescription5.TDL_PATIENT_MILITARY_RANK_NAME));
                    SetSingleKey(new KeyValue(Mps000044ExtendSingleKey.DOB, rdo.vHisPrescription5.TDL_PATIENT_DOB));
                    SetSingleKey(new KeyValue(Mps000044ExtendSingleKey.VIR_ADDRESS, rdo.vHisPrescription5.TDL_PATIENT_ADDRESS));
                    SetSingleKey(new KeyValue(Mps000044ExtendSingleKey.VIR_PATIENT_NAME, rdo.vHisPrescription5.TDL_PATIENT_NAME));
                }

                SetSingleKey(new KeyValue(Mps000044ExtendSingleKey.USE_DAY, UseDayMax));

                if (rdo.vHisPatientTypeAlter != null)
                {
                    if (!String.IsNullOrEmpty(rdo.vHisPatientTypeAlter.HEIN_CARD_NUMBER))
                    {
                        SetSingleKey((new KeyValue(Mps000044ExtendSingleKey.HEIN_CARD_NUMBER_SEPARATE, HeinCardHelper.SetHeinCardNumberDisplayByNumber(rdo.vHisPatientTypeAlter.HEIN_CARD_NUMBER))));
                        SetSingleKey((new KeyValue(Mps000044ExtendSingleKey.IS_HEIN, "X")));
                        SetSingleKey((new KeyValue(Mps000044ExtendSingleKey.IS_VIENPHI, "")));
                        SetSingleKey((new KeyValue(Mps000044ExtendSingleKey.HEIN_CARD_NUMBER_1, rdo.vHisPatientTypeAlter.HEIN_CARD_NUMBER.Substring(0, 2))));
                        SetSingleKey((new KeyValue(Mps000044ExtendSingleKey.HEIN_CARD_NUMBER_2, rdo.vHisPatientTypeAlter.HEIN_CARD_NUMBER.Substring(2, 1))));
                        SetSingleKey((new KeyValue(Mps000044ExtendSingleKey.HEIN_CARD_NUMBER_3, rdo.vHisPatientTypeAlter.HEIN_CARD_NUMBER.Substring(3, 2))));
                        SetSingleKey((new KeyValue(Mps000044ExtendSingleKey.HEIN_CARD_NUMBER_4, rdo.vHisPatientTypeAlter.HEIN_CARD_NUMBER.Substring(5, 2))));
                        SetSingleKey((new KeyValue(Mps000044ExtendSingleKey.HEIN_CARD_NUMBER_5, rdo.vHisPatientTypeAlter.HEIN_CARD_NUMBER.Substring(7, 3))));
                        SetSingleKey((new KeyValue(Mps000044ExtendSingleKey.HEIN_CARD_NUMBER_6, rdo.vHisPatientTypeAlter.HEIN_CARD_NUMBER.Substring(10, 5))));
                        SetSingleKey(new KeyValue(Mps000044ExtendSingleKey.STR_HEIN_CARD_FROM_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.vHisPatientTypeAlter.HEIN_CARD_FROM_TIME ?? 0)));
                        SetSingleKey(new KeyValue(Mps000044ExtendSingleKey.STR_HEIN_CARD_TO_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.vHisPatientTypeAlter.HEIN_CARD_TO_TIME ?? 0)));
                        SetSingleKey((new KeyValue(Mps000044ExtendSingleKey.HEIN_ADDRESS, rdo.vHisPatientTypeAlter.ADDRESS)));
                        SetSingleKey((new KeyValue(Mps000044ExtendSingleKey.HEIN_MEDI_ORG_CODE, rdo.vHisPatientTypeAlter.HEIN_MEDI_ORG_CODE)));
                        SetSingleKey((new KeyValue(Mps000044ExtendSingleKey.HEIN_MEDI_ORG_NAME, rdo.vHisPatientTypeAlter.HEIN_MEDI_ORG_NAME)));
                    }
                    else
                    {
                        SetSingleKey((new KeyValue(Mps000044ExtendSingleKey.IS_HEIN, "")));
                        SetSingleKey((new KeyValue(Mps000044ExtendSingleKey.IS_VIENPHI, "X")));
                    }

                    var HeinRatio = new BhytHeinProcessor().GetDefaultHeinRatio(rdo.vHisPatientTypeAlter.HEIN_TREATMENT_TYPE_CODE, rdo.vHisPatientTypeAlter.HEIN_CARD_NUMBER, rdo.vHisPatientTypeAlter.LEVEL_CODE, rdo.vHisPatientTypeAlter.RIGHT_ROUTE_CODE);

                    SetSingleKey((new KeyValue(Mps000044ExtendSingleKey.HEIN_RATIO, HeinRatio)));
                }

                if (rdo.Mps000044ADO != null)
                {
                    if (!String.IsNullOrWhiteSpace(rdo.Mps000044ADO.KEY_NAME_TITLES))
                    {
                        string keyName = rdo.Mps000044ADO.KEY_NAME_TITLES.Trim('"');
                        SetSingleKey(new KeyValue(Mps000044ExtendSingleKey.KEY_NAME_TITLE, keyName));
                    }

                    AddObjectKeyIntoListkey<Mps000044ADO>(rdo.Mps000044ADO, false);
                }

                if (rdo.dhsts != null)
                {
                    AddObjectKeyIntoListkey<HIS_DHST>(rdo.dhsts, false);
                }

                if (rdo.hisTreatment != null)
                {
                    SetSingleKey((new KeyValue("APPOINTMENT_CODE", rdo.hisTreatment.APPOINTMENT_CODE)));
                    SetSingleKey((new KeyValue("APPOINTMENT_DATE", rdo.hisTreatment.APPOINTMENT_DATE)));
                    SetSingleKey((new KeyValue("APPOINTMENT_DESC", rdo.hisTreatment.APPOINTMENT_DESC)));
                    SetSingleKey((new KeyValue("APPOINTMENT_SURGERY", rdo.hisTreatment.APPOINTMENT_SURGERY)));
                    SetSingleKey((new KeyValue("APPOINTMENT_TIME", rdo.hisTreatment.APPOINTMENT_TIME)));
                    SetSingleKey((new KeyValue("APPOINTMENT_EXAM_ROOM_IDS", rdo.hisTreatment.APPOINTMENT_EXAM_ROOM_IDS)));
                }
                if (rdo.hisServiceReq_CurentExam != null && rdo.vHisPrescription5 != null)
                {
                    PropertyInfo[] pis = typeof(HIS_SERVICE_REQ).GetProperties();
                    if (pis != null && pis.Length > 0)
                    {
                        foreach (var pi in pis)
                        {
                            if (pi.GetGetMethod().IsVirtual) continue;
                            try
                            {
                                if (pi.GetValue(rdo.vHisPrescription5) == null)
                                {
                                    pi.SetValue(rdo.vHisPrescription5, pi.GetValue(rdo.hisServiceReq_CurentExam));
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                }
                AddObjectKeyIntoListkey<HIS_SERVICE_REQ>(rdo.vHisPrescription5, false);
                AddObjectKeyIntoListkey<V_HIS_PATIENT_TYPE_ALTER>(rdo.vHisPatientTypeAlter, false);
                AddObjectKeyIntoListkey<HIS_TREATMENT>(rdo.hisTreatment, false);
                if (rdo.HisExpMest != null)
                {
                    AddObjectKeyIntoListkey(rdo.HisExpMest, false);

                    string title = "C";
                    var expMestMedicine = rdo.expMestMedicines != null && rdo.expMestMedicines.Count() > 0 ? rdo.expMestMedicines.FirstOrDefault() : null;
                    if (expMestMedicine != null && expMestMedicine.IS_NEUROLOGICAL == 1) title = "H";
                    else if (expMestMedicine != null && expMestMedicine.IS_ADDICTIVE == 1) title = "N";
                    string serviceReqCode = rdo.vHisPrescription5 != null ? (rdo.vHisPrescription5.SERVICE_REQ_CODE) : null;
                    string electronicExpMestCode = string.Format("{0}{1}-{2}", MPS.ProcessorBase.PrintConfig.MediOrgCode, HIS.ERXConnect.ERXCode.Encode(Convert.ToInt64(serviceReqCode)), title);
                    SetSingleKey(new KeyValue(Mps000044ExtendSingleKey.ELECTRONIC_EXP_MEST_CODE, electronicExpMestCode));
                }

                if (rdo.hisServiceReq_CurentExam != null)
                {
                    SetSingleKey(new KeyValue(Mps000044ExtendSingleKey.ICD_CODE_EXAM, rdo.hisServiceReq_CurentExam.ICD_CODE));
                    SetSingleKey(new KeyValue(Mps000044ExtendSingleKey.ICD_NAME_EXAM, rdo.hisServiceReq_CurentExam.ICD_NAME));
                    SetSingleKey(new KeyValue(Mps000044ExtendSingleKey.ICD_SUB_CODE_EXAM, rdo.hisServiceReq_CurentExam.ICD_SUB_CODE));
                    SetSingleKey(new KeyValue(Mps000044ExtendSingleKey.ICD_TEXT_EXAM, rdo.hisServiceReq_CurentExam.ICD_TEXT));
                    SetSingleKey((new KeyValue(Mps000044ExtendSingleKey.PART_EXAM_EYE_STR, ProcessDataEye(rdo.hisServiceReq_CurentExam.PART_EXAM_EYE))));
                    SetSingleKey((new KeyValue(Mps000044ExtendSingleKey.PART_EXAM_EYE_TENSION_LEFT_STR, ProcessDataEye(rdo.hisServiceReq_CurentExam.PART_EXAM_EYE_TENSION_LEFT))));
                    SetSingleKey((new KeyValue(Mps000044ExtendSingleKey.PART_EXAM_EYE_TENSION_RIGHT_STR, ProcessDataEye(rdo.hisServiceReq_CurentExam.PART_EXAM_EYE_TENSION_RIGHT))));
                    SetSingleKey((new KeyValue(Mps000044ExtendSingleKey.PART_EXAM_EYESIGHT_LEFT_STR, ProcessDataEye(rdo.hisServiceReq_CurentExam.PART_EXAM_EYESIGHT_LEFT))));
                    SetSingleKey((new KeyValue(Mps000044ExtendSingleKey.PART_EXAM_EYESIGHT_RIGHT_STR, ProcessDataEye(rdo.hisServiceReq_CurentExam.PART_EXAM_EYESIGHT_RIGHT))));
                    SetSingleKey((new KeyValue(Mps000044ExtendSingleKey.PART_EXAM_EYESIGHT_GLASS_LEFT_STR, ProcessDataEye(rdo.hisServiceReq_CurentExam.PART_EXAM_EYESIGHT_GLASS_LEFT))));
                    SetSingleKey((new KeyValue(Mps000044ExtendSingleKey.PART_EXAM_EYESIGHT_GLASS_RIGHT_STR, ProcessDataEye(rdo.hisServiceReq_CurentExam.PART_EXAM_EYESIGHT_GLASS_RIGHT))));
                    SetSingleKey((new KeyValue(Mps000044ExtendSingleKey.PART_EXAM_HOLE_GLASS_LEFT_STR, ProcessDataEye(rdo.hisServiceReq_CurentExam.PART_EXAM_HOLE_GLASS_LEFT))));
                    SetSingleKey((new KeyValue(Mps000044ExtendSingleKey.PART_EXAM_HOLE_GLASS_RIGHT_STR, ProcessDataEye(rdo.hisServiceReq_CurentExam.PART_EXAM_HOLE_GLASS_RIGHT))));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private string ProcessDataEye(string p)
        {
            string result = p;
            try
            {
                if (!String.IsNullOrEmpty(p))
                {
                    bool addText = true;
                    foreach (var item in p)
                    {
                        if (Char.IsLetter(item))
                        {
                            addText = false;
                            break;
                        }
                    }

                    if (addText)
                    {
                        result += "/10";
                    }
                }
            }
            catch (Exception ex)
            {
                result = p;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
        private void SetQrCode()
        {
            try
            {
                if (rdo.TransReq != null && rdo.ListHisConfigPaymentQrCode != null && rdo.ListHisConfigPaymentQrCode.Count > 0)
                {
                    var data = QrCodeProcessor.CreateQrImage(rdo.TransReq, rdo.ListHisConfigPaymentQrCode);
                    if (data != null && data.Count > 0)
                    {
                        foreach (var item in data)
                        {
                            SetSingleKey(new KeyValue(item.Key, item.Value));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public string Encode(long id)
        {
            string result = "";
            try
            {
                long tmp;
                long dn = 0;
                string rs = "";
                for (long l = id; l > 0; l = l / 36)
                {
                    tmp = l % 36;
                    if (tmp < 10)
                        tmp = tmp + 48;
                    else
                        tmp = tmp + 55;
                    dn = dn * 100 + tmp;
                }
                for (long m = dn; m > 0; m = m / 100)
                {
                    long s = m % 100;
                    rs += (char)s;
                }
                if (rs.Length < 7)
                {
                    int c = rs.Length;
                    while (7 - c > 0)
                    {
                        c++;
                        result += "0";
                    }
                }
                result += rs;
            }
            catch (Exception ex)
            {
                result = "0000000";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }

    public class HeinCardHelper
    {
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
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = heinCardNumber;
            }
            return result;
        }

        public static string TrimHeinCardNumber(string chucodau)
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
    }

}
