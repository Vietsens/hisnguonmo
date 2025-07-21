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
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000353.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000353
{
    public class Mps000353Processor : AbstractProcessor
    {
        Mps000353PDO rdo;
        private List<ExpMestMedicineSDO> expMestMedicinesTYPE;
        private List<ExpMestMedicineSDO> expMestMedicines_Sort;
        public Mps000353Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000353PDO)rdoBase;
        }

        //Lọc thuốc theo thứ tự
        private void MedicinesSort()
        {
            try
            {
                expMestMedicines_Sort = new List<ExpMestMedicineSDO>();
                if (rdo.expMestMedicines != null && rdo.expMestMedicines.Count > 0)
                {
                    Inventec.Common.Logging.LogSystem.Warn("MPS000353____________" + rdo.KeyUseForm);
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
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                ProcessBarcodeKey();
                ProcessSingleKey();
                ProcessListData();

                //ghi đè PrintLogData và UniqueCodeData
                ProcessPrintLogData();
                //lấy số lần in
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));

                this.SetSignatureKeyImageByCFG();
                MedicinesSort();
                if (store.ReadTemplate(System.IO.Path.GetFullPath(fileName)))
                {
                    singleTag.ProcessData(store, singleValueDictionary);
                    barCodeTag.ProcessData(store, dicImage);
                    objectTag.AddObjectData(store, "type", expMestMedicinesTYPE);
                    objectTag.AddObjectData(store, "MedicineExpmest", expMestMedicines_Sort);
                    objectTag.AddRelationship(store, "type", "MedicineExpmest", "PATIENT_TYPE_NAME", "PATIENT_TYPE_NAME");

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

        void ProcessSingleKey()
        {
            try
            {
                if (rdo.Mps000353ADO != null)
                {
                    AddObjectKeyIntoListkey<Mps000353ADO>(rdo.Mps000353ADO, false);
                }
                if (rdo.hisDhst != null)
                {
                    AddObjectKeyIntoListkey<HIS_DHST>(rdo.hisDhst, false);
                }

                if (rdo.HisPrescription != null)
                {
                    if (rdo.expMestMedicines != null && rdo.expMestMedicines.Count > 0)
                    {
                        decimal tong = 0;
                        foreach (var item in rdo.expMestMedicines)
                        {
                            tong += item.AMOUNT * ((item.PRICE ?? 0) * (1 + (item.VAT_RATIO ?? 0)));
                        }
                        SetSingleKey(new KeyValue(Mps000353ExtendSingleKey.TOTAL_PRICE_PRESCRIPTION, tong));

                        //nếu không có use_time_to trong V_HIS_PRESCRIPTION thì lấy trong ds thuốc 
                        if (rdo.HisPrescription != null && !rdo.HisPrescription.USE_TIME_TO.HasValue)
                        {
                            var maxUseTimeTo = rdo.expMestMedicines.Where(o => o.USE_TIME_TO.HasValue).ToList();
                            if (maxUseTimeTo != null && maxUseTimeTo.Count > 0)
                            {
                                var useTimeTo = maxUseTimeTo.Max(m => m.USE_TIME_TO).Value;
                                SetSingleKey(new KeyValue(Mps000353ExtendSingleKey.USE_TIME_TO_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(useTimeTo)));
                            }
                        }
                        else
                            SetSingleKey(new KeyValue(Mps000353ExtendSingleKey.USE_TIME_TO_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.HisPrescription.USE_TIME_TO ?? 0)));

                        if (rdo.HisPrescription != null && rdo.HisPrescription.USE_TIME.HasValue)
                        {
                            SetSingleKey(new KeyValue(Mps000353ExtendSingleKey.USER_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.HisPrescription.USE_TIME ?? 0)));
                        }
                        else
                        {
                            SetSingleKey(new KeyValue(Mps000353ExtendSingleKey.USER_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.HisPrescription.INTRUCTION_TIME)));
                        }
                    }

                    SetSingleKey(new KeyValue(Mps000353ExtendSingleKey.INTRUCTION_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.HisPrescription.INTRUCTION_TIME)));
                    SetSingleKey(new KeyValue(Mps000353ExtendSingleKey.INTRUCTION_TIME_FULL_SRT, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(rdo.HisPrescription.INTRUCTION_TIME)));

                    SetSingleKey((new KeyValue(Mps000353ExtendSingleKey.AGE, Inventec.Common.DateTime.Calculation.AgeCaption(rdo.HisPrescription.TDL_PATIENT_DOB))));
                    SetSingleKey((new KeyValue(Mps000353ExtendSingleKey.DOB_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.HisPrescription.TDL_PATIENT_DOB))));
                    SetSingleKey((new KeyValue(Mps000353ExtendSingleKey.D_O_B, rdo.HisPrescription.TDL_PATIENT_DOB.ToString().Substring(0, 4))));

                    SetSingleKey(new KeyValue(Mps000353ExtendSingleKey.NATIONAL_NAME, rdo.HisPrescription.TDL_PATIENT_NATIONAL_NAME));
                    SetSingleKey(new KeyValue(Mps000353ExtendSingleKey.WORK_PLACE, rdo.HisPrescription.TDL_PATIENT_WORK_PLACE_NAME));
                    SetSingleKey(new KeyValue(Mps000353ExtendSingleKey.ADDRESS, rdo.HisPrescription.TDL_PATIENT_ADDRESS));
                    SetSingleKey(new KeyValue(Mps000353ExtendSingleKey.CAREER_NAME, rdo.HisPrescription.TDL_PATIENT_CAREER_NAME));
                    SetSingleKey(new KeyValue(Mps000353ExtendSingleKey.PATIENT_CODE, rdo.HisPrescription.TDL_PATIENT_CODE));
                    SetSingleKey(new KeyValue(Mps000353ExtendSingleKey.DISTRICT_CODE, rdo.HisPrescription.TDL_PATIENT_DISTRICT_CODE));
                    SetSingleKey(new KeyValue(Mps000353ExtendSingleKey.GENDER_NAME, rdo.HisPrescription.TDL_PATIENT_GENDER_NAME));
                    SetSingleKey(new KeyValue(Mps000353ExtendSingleKey.MILITARY_RANK_NAME, rdo.HisPrescription.TDL_PATIENT_MILITARY_RANK_NAME));
                    SetSingleKey(new KeyValue(Mps000353ExtendSingleKey.DOB, rdo.HisPrescription.TDL_PATIENT_DOB));
                    SetSingleKey(new KeyValue(Mps000353ExtendSingleKey.VIR_ADDRESS, rdo.HisPrescription.TDL_PATIENT_ADDRESS));
                    SetSingleKey(new KeyValue(Mps000353ExtendSingleKey.VIR_PATIENT_NAME, rdo.HisPrescription.TDL_PATIENT_NAME));


                    SetSingleKey(new KeyValue(Mps000353ExtendSingleKey.REQ_ICD_CODE, rdo.HisPrescription.ICD_CODE));
                    SetSingleKey(new KeyValue(Mps000353ExtendSingleKey.REQ_ICD_NAME, rdo.HisPrescription.ICD_NAME));
                    SetSingleKey(new KeyValue(Mps000353ExtendSingleKey.REQ_ICD_SUB_CODE, rdo.HisPrescription.ICD_SUB_CODE));
                    SetSingleKey(new KeyValue(Mps000353ExtendSingleKey.REQ_ICD_TEXT, rdo.HisPrescription.ICD_TEXT));
                }

                if (rdo.vHisPatientTypeAlter != null)
                {
                    if (!String.IsNullOrEmpty(rdo.vHisPatientTypeAlter.HEIN_CARD_NUMBER))
                    {
                        SetSingleKey((new KeyValue(Mps000353ExtendSingleKey.HEIN_CARD_NUMBER_SEPARATE, HeinCardHelper.SetHeinCardNumberDisplayByNumber(rdo.vHisPatientTypeAlter.HEIN_CARD_NUMBER))));
                        SetSingleKey((new KeyValue(Mps000353ExtendSingleKey.IS_HEIN, "X")));
                        SetSingleKey((new KeyValue(Mps000353ExtendSingleKey.IS_VIENPHI, "")));
                        SetSingleKey((new KeyValue(Mps000353ExtendSingleKey.HEIN_CARD_NUMBER_1, rdo.vHisPatientTypeAlter.HEIN_CARD_NUMBER.Substring(0, 2))));
                        SetSingleKey((new KeyValue(Mps000353ExtendSingleKey.HEIN_CARD_NUMBER_2, rdo.vHisPatientTypeAlter.HEIN_CARD_NUMBER.Substring(2, 1))));
                        SetSingleKey((new KeyValue(Mps000353ExtendSingleKey.HEIN_CARD_NUMBER_3, rdo.vHisPatientTypeAlter.HEIN_CARD_NUMBER.Substring(3, 2))));
                        SetSingleKey((new KeyValue(Mps000353ExtendSingleKey.HEIN_CARD_NUMBER_4, rdo.vHisPatientTypeAlter.HEIN_CARD_NUMBER.Substring(5, 2))));
                        SetSingleKey((new KeyValue(Mps000353ExtendSingleKey.HEIN_CARD_NUMBER_5, rdo.vHisPatientTypeAlter.HEIN_CARD_NUMBER.Substring(7, 3))));
                        SetSingleKey((new KeyValue(Mps000353ExtendSingleKey.HEIN_CARD_NUMBER_6, rdo.vHisPatientTypeAlter.HEIN_CARD_NUMBER.Substring(10, 5))));
                        SetSingleKey(new KeyValue(Mps000353ExtendSingleKey.STR_HEIN_CARD_FROM_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.vHisPatientTypeAlter.HEIN_CARD_FROM_TIME ?? 0)));
                        SetSingleKey(new KeyValue(Mps000353ExtendSingleKey.STR_HEIN_CARD_TO_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.vHisPatientTypeAlter.HEIN_CARD_TO_TIME ?? 0)));
                    }
                    else
                    {
                        SetSingleKey((new KeyValue(Mps000353ExtendSingleKey.IS_HEIN, "")));
                        SetSingleKey((new KeyValue(Mps000353ExtendSingleKey.IS_VIENPHI, "X")));
                    }
                }

                if (rdo.hisServiceReq_Exam != null && rdo.HisPrescription != null)
                {
                    PropertyInfo[] pis = typeof(HIS_SERVICE_REQ).GetProperties();
                    if (pis != null && pis.Length > 0)
                    {
                        foreach (var pi in pis)
                        {
                            if (pi.GetGetMethod().IsVirtual) continue;
                            try
                            {
                                if (pi.GetValue(rdo.HisPrescription) == null)
                                {
                                    pi.SetValue(rdo.HisPrescription, pi.GetValue(rdo.hisServiceReq_Exam));
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                    //AddObjectKeyIntoListkey<HIS_SERVICE_REQ>(rdo.hisServiceReq_Exam, false);
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

                AddObjectKeyIntoListkey<HIS_SERVICE_REQ>(rdo.HisPrescription, false);
                AddObjectKeyIntoListkey<V_HIS_PATIENT_TYPE_ALTER>(rdo.vHisPatientTypeAlter);
                if (rdo.hisTreatment != null)
                {
                    AddObjectKeyIntoListkey<HIS_TREATMENT>(rdo.hisTreatment, false);
                }

                if (rdo.hisServiceReq_Exam != null)
                {
                    SetSingleKey((new KeyValue(Mps000353ExtendSingleKey.PART_EXAM_EYE_STR, ProcessDataEye(rdo.hisServiceReq_Exam.PART_EXAM_EYE))));
                    SetSingleKey((new KeyValue(Mps000353ExtendSingleKey.PART_EXAM_EYE_TENSION_LEFT_STR, ProcessDataEye(rdo.hisServiceReq_Exam.PART_EXAM_EYE_TENSION_LEFT))));
                    SetSingleKey((new KeyValue(Mps000353ExtendSingleKey.PART_EXAM_EYE_TENSION_RIGHT_STR, ProcessDataEye(rdo.hisServiceReq_Exam.PART_EXAM_EYE_TENSION_RIGHT))));
                    SetSingleKey((new KeyValue(Mps000353ExtendSingleKey.PART_EXAM_EYESIGHT_LEFT_STR, ProcessDataEye(rdo.hisServiceReq_Exam.PART_EXAM_EYESIGHT_LEFT))));
                    SetSingleKey((new KeyValue(Mps000353ExtendSingleKey.PART_EXAM_EYESIGHT_RIGHT_STR, ProcessDataEye(rdo.hisServiceReq_Exam.PART_EXAM_EYESIGHT_RIGHT))));
                    SetSingleKey((new KeyValue(Mps000353ExtendSingleKey.PART_EXAM_EYESIGHT_GLASS_LEFT_STR, ProcessDataEye(rdo.hisServiceReq_Exam.PART_EXAM_EYESIGHT_GLASS_LEFT))));
                    SetSingleKey((new KeyValue(Mps000353ExtendSingleKey.PART_EXAM_EYESIGHT_GLASS_RIGHT_STR, ProcessDataEye(rdo.hisServiceReq_Exam.PART_EXAM_EYESIGHT_GLASS_RIGHT))));
                }
                if (rdo.HisExpMest != null)
                {
                    AddObjectKeyIntoListkey(rdo.HisExpMest, false);

                    string title = "C";
                    var expMestMedicine = rdo.expMestMedicines != null && rdo.expMestMedicines.Count() > 0 ? rdo.expMestMedicines.FirstOrDefault() : null;
                    if (expMestMedicine != null && expMestMedicine.IS_NEUROLOGICAL == 1) title = "H";
                    else if (expMestMedicine != null && expMestMedicine.IS_ADDICTIVE == 1) title = "G";
                    string serviceReqCode = rdo.HisPrescription != null ? (rdo.HisPrescription.SERVICE_REQ_CODE) : null;
                    string electronicExpMestCode = string.Format("{0}{1}-{2}", MPS.ProcessorBase.PrintConfig.MediOrgCode, HIS.ERXConnect.ERXCode.Encode(Convert.ToInt64(serviceReqCode)), title);
                    SetSingleKey(new KeyValue(Mps000353ExtendSingleKey.ELECTRONIC_EXP_MEST_CODE, electronicExpMestCode));
                }

                string ttServiceNames = "";
                string clsServiceNames = "";
                if (rdo.ListSereServCls != null && rdo.ListSereServCls.Count > 0)
                {
                    var listTt = rdo.ListSereServCls.Where(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT && o.IS_NO_EXECUTE != 1).ToList();
                    var listOther = rdo.ListSereServCls.Where(o => o.TDL_SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT && o.IS_NO_EXECUTE != 1).ToList();

                    if (listTt != null && listTt.Count > 0)
                    {
                        ttServiceNames = string.Join(",", listTt.Select(s => s.TDL_SERVICE_NAME).Distinct().ToList());
                    }

                    if (listOther != null && listOther.Count > 0)
                    {
                        clsServiceNames = string.Join(",", listOther.Select(s => s.TDL_SERVICE_NAME).Distinct().ToList());
                    }
                }

                SetSingleKey((new KeyValue(Mps000353ExtendSingleKey.TT_SERVICE_NAME, ttServiceNames)));
                SetSingleKey((new KeyValue(Mps000353ExtendSingleKey.CLS_SERVICE_NAME, clsServiceNames)));

                if (rdo.HisExpMest != null)
                {
                    AddObjectKeyIntoListkey(rdo.HisExpMest, false);
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

        void ProcessBarcodeKey()
        {
            try
            {
                if (rdo.HisPrescription != null)
                {
                    if (!String.IsNullOrEmpty(rdo.HisPrescription.TDL_TREATMENT_CODE))
                    {
                        Inventec.Common.BarcodeLib.Barcode barcodeTreatmentCode = new Inventec.Common.BarcodeLib.Barcode(rdo.HisPrescription.TDL_TREATMENT_CODE);
                        barcodeTreatmentCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                        barcodeTreatmentCode.IncludeLabel = false;
                        barcodeTreatmentCode.Width = 120;
                        barcodeTreatmentCode.Height = 40;
                        barcodeTreatmentCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                        barcodeTreatmentCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                        barcodeTreatmentCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                        barcodeTreatmentCode.IncludeLabel = true;
                        dicImage.Add(Mps000353ExtendSingleKey.TREATMENT_CODE_BARCODE, barcodeTreatmentCode);
                    }

                    if (!String.IsNullOrEmpty(rdo.HisPrescription.TDL_PATIENT_CODE))
                    {
                        Inventec.Common.BarcodeLib.Barcode barcodePatient = new Inventec.Common.BarcodeLib.Barcode(rdo.HisPrescription.TDL_PATIENT_CODE);
                        barcodePatient.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                        barcodePatient.IncludeLabel = false;
                        barcodePatient.Width = 120;
                        barcodePatient.Height = 40;
                        barcodePatient.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                        barcodePatient.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                        barcodePatient.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                        barcodePatient.IncludeLabel = true;
                        dicImage.Add(Mps000353ExtendSingleKey.PATIENT_CODE_BARCODE, barcodePatient);
                    }

                    if (!String.IsNullOrEmpty(rdo.HisPrescription.SERVICE_REQ_CODE))
                    {
                        Inventec.Common.BarcodeLib.Barcode barcodeServiceReq = new Inventec.Common.BarcodeLib.Barcode(rdo.HisPrescription.SERVICE_REQ_CODE);
                        barcodeServiceReq.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                        barcodeServiceReq.IncludeLabel = false;
                        barcodeServiceReq.Width = 120;
                        barcodeServiceReq.Height = 40;
                        barcodeServiceReq.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                        barcodeServiceReq.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                        barcodeServiceReq.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                        barcodeServiceReq.IncludeLabel = true;
                        dicImage.Add(Mps000353ExtendSingleKey.SERVICE_REQ_CODE_BAR, barcodeServiceReq);
                    }
                }

                if (rdo.Mps000353ADO != null && !String.IsNullOrEmpty(rdo.Mps000353ADO.EXP_MEST_CODE))
                {
                    Inventec.Common.BarcodeLib.Barcode expMestCodeBarCode = new Inventec.Common.BarcodeLib.Barcode(rdo.Mps000353ADO.EXP_MEST_CODE);
                    expMestCodeBarCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                    expMestCodeBarCode.IncludeLabel = false;
                    expMestCodeBarCode.Width = 120;
                    expMestCodeBarCode.Height = 40;
                    expMestCodeBarCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                    expMestCodeBarCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                    expMestCodeBarCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                    expMestCodeBarCode.IncludeLabel = true;
                    dicImage.Add(Mps000353ExtendSingleKey.EXP_MEST_CODE_BARCODE, expMestCodeBarCode);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
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
                        if (item.First().PATIENT_TYPE_ID > 0)
                        {
                            ExpMestMedicineSDO ado = new ExpMestMedicineSDO();
                            ado.PATIENT_TYPE_ID = item.First().PATIENT_TYPE_ID;
                            ado.PATIENT_TYPE_NAME = item.First().PATIENT_TYPE_NAME;
                            expMestMedicinesTYPE.Add(ado);
                        }
                    }

                    expMestMedicinesTYPE = expMestMedicinesTYPE.OrderBy(o => o.PATIENT_TYPE_ID).ToList();
                    if (rdo.expMestMedicines.Exists(o => o.PATIENT_TYPE_ID == 0))
                    {
                        ExpMestMedicineSDO ado = new ExpMestMedicineSDO();
                        ado.PATIENT_TYPE_ID = rdo.expMestMedicines.First(o => o.PATIENT_TYPE_ID == 0).PATIENT_TYPE_ID;
                        ado.PATIENT_TYPE_NAME = rdo.expMestMedicines.First(o => o.PATIENT_TYPE_ID == 0).PATIENT_TYPE_NAME;
                        expMestMedicinesTYPE.Add(ado);
                    }
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
                log = this.LogDataExpMest(rdo.HisPrescription.TDL_TREATMENT_CODE, rdo.HisPrescription.SERVICE_REQ_CODE, "");
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
                if (rdo != null && rdo.HisPrescription != null)
                {
                    string treatmentCode = "TREATMENT_CODE:" + rdo.HisPrescription.TDL_TREATMENT_CODE;
                    string serviceReqCode = "SERVICE_REQ_CODE:" + rdo.HisPrescription.SERVICE_REQ_CODE;
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
