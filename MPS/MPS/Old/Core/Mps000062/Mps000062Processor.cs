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
using MOS.EFMODEL.DataModels;
using MPS.ADO;
using MPS.ADO.TrackingPrint;
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace MPS.Core.Mps000062
{
    class Mps000062Processor : ProcessorBase, IProcessorPrint
    {
        Mps000062RDO rdo;
        internal List<Mps000062ADO> mps000062ADOs { get; set; }
        internal List<Mps000062ADOMedicines> lstMps000062ADOMedicines { get; set; }
        internal List<Mps000062ADOServiceCLS> lstMps000062ADOServiceCLS { get; set; }
        internal List<Mps000062RemedyCount> lstMps000062RemedyCount { get; set; }
        internal List<Mps000062ADOMaterials> lstMps000062ADOMaterials { get; set; }
        Inventec.Common.FlexCellExport.Store store;
        string fileName;
        internal Dictionary<string, Inventec.Common.BarcodeLib.Barcode> dicImage = new Dictionary<string, Inventec.Common.BarcodeLib.Barcode>();

        Dictionary<long, List<NumberDate>> dicCountNumber = new Dictionary<long, List<NumberDate>>();

        internal Mps000062Processor(SAR_PRINT_TYPE config, string fileName, object data, MPS.Printer.PreviewType previewType, string printerName)
            : base(config, (RDOBase)data, previewType, printerName)
        {
            rdo = (Mps000062RDO)data;
            this.fileName = fileName;
            store = new Inventec.Common.FlexCellExport.Store();
            dicImage = new Dictionary<string, Inventec.Common.BarcodeLib.Barcode>();
        }

        bool IProcessorPrint.Run()
        {
            bool result = false;
            bool valid = true;
            try
            {
                SetCommonSingleKey();
                rdo.SetSingleKey();
                SetSingleKey();

                //Cac ham dac thu khac cua rdo
                SetBarcodeKey();

                store.SetCommonFunctions();

                ProcessListADO();

                //Ham xu ly cac doi tuong su dung trong thu vien flexcelexport
                valid = valid && ProcessData();
                if (valid)
                {
                    using (MemoryStream streamResult = store.OutStream())
                    {
                        //Print preview
                        result = PrintPreview(streamResult, this.fileName);
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

        void BBandBG()
        {
            try
            {
                if (rdo.lstPrescription != null && rdo.lstPrescription.Count > 0)
                {
                    rdo.lstPrescription = rdo.lstPrescription.OrderBy(p => p.INTRUCTION_TIME).ToList();
                    var vHisPrescriptionGroup = rdo.lstPrescription.GroupBy(o => o.INTRUCTION_TIME.ToString().Substring(0, 8)).ToList();//group lại theo ngày chỉ định
                    int num = 1;
                    foreach (var itemGrPres in vHisPrescriptionGroup)
                    {
                        List<long> expMestIDs = new List<long>();
                        expMestIDs = itemGrPres.Select(p => p.EXP_MEST_ID).ToList();
                        if (rdo.lstExpMestMedicine != null && rdo.lstExpMestMedicine.Count > 0)
                        {
                            var listExpMest = rdo.lstExpMestMedicine.Where(p => (expMestIDs.Contains(p.EXP_MEST_ID)) && (p.IS_NEUROLOGICAL == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_TYPE.IS_NEUROLOGICAL__TRUE || p.IS_ADDICTIVE == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_TYPE.IS_ADDICTIVE__TRUE || p.IS_ANTIBIOTIC == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_TYPE.IS_ANTIBIOTIC__TRUE)).ToList();
                            if (listExpMest != null && listExpMest.Count > 0)
                            {
                                var expMestGroup = listExpMest.GroupBy(p => p.MEDICINE_TYPE_ID).ToList();
                                foreach (var expMedicine in expMestGroup)
                                {
                                    string key = itemGrPres.Key + "_" + expMedicine.FirstOrDefault().MEDICINE_TYPE_ID;
                                    if (!dicCountNumber.ContainsKey(expMedicine.FirstOrDefault().MEDICINE_TYPE_ID))
                                    {
                                        NumberDate ado = new NumberDate();
                                        ado.INTRUCTION_DATE = Inventec.Common.TypeConvert.Parse.ToInt64(itemGrPres.Key);
                                        ado.MEDICINE_TYPE_ID = expMedicine.FirstOrDefault().MEDICINE_TYPE_ID;
                                        ado.num = num;
                                        dicCountNumber[expMedicine.FirstOrDefault().MEDICINE_TYPE_ID] = new List<NumberDate>();
                                        dicCountNumber[expMedicine.FirstOrDefault().MEDICINE_TYPE_ID].Add(ado);
                                    }
                                    else
                                    {
                                        NumberDate ado = new NumberDate();
                                        ado.INTRUCTION_DATE = Inventec.Common.TypeConvert.Parse.ToInt64(itemGrPres.Key);
                                        ado.MEDICINE_TYPE_ID = expMedicine.FirstOrDefault().MEDICINE_TYPE_ID;
                                        ado.num = dicCountNumber[expMedicine.FirstOrDefault().MEDICINE_TYPE_ID].Count;
                                        dicCountNumber[expMedicine.FirstOrDefault().MEDICINE_TYPE_ID].Add(ado);
                                    }
                                }
                            }
                        }
                    }
                    num++;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessListADO()
        {
            try
            {
                mps000062ADOs = new List<Mps000062ADO>();
                lstMps000062ADOMedicines = new List<Mps000062ADOMedicines>();
                lstMps000062ADOServiceCLS = new List<Mps000062ADOServiceCLS>();
                lstMps000062RemedyCount = new List<Mps000062RemedyCount>();
                if (rdo.lstTracking != null && rdo.lstTracking.Count > 0)
                {
                    foreach (var itemTracking in rdo.lstTracking)
                    {
                        #region Thông tin chẩn đoán tờ điều trị
                        string icd_Name = "";
                        if (string.IsNullOrEmpty(itemTracking.ICD_MAIN_TEXT))
                        {
                            var icd = rdo.lstIcdId.Where(p => p.ID == itemTracking.ICD_ID).FirstOrDefault();
                            if (icd != null)
                            {
                                icd_Name = icd.ICD_NAME;
                            }
                        }
                        else
                        {
                            icd_Name = itemTracking.ICD_MAIN_TEXT;
                        }

                        #endregion

                        #region TrackingTime
                        MPS.ADO.Mps000062ADO _service = new Mps000062ADO();

                        Mapper.CreateMap<HIS_TRACKING, Mps000062ADO>();
                        _service = Mapper.Map<HIS_TRACKING, Mps000062ADO>(itemTracking);
                        _service.TRACKING_TIME_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(itemTracking.TRACKING_TIME);
                        _service.TRACKING_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(itemTracking.TRACKING_TIME);
                        _service.TRACKING_DATE_SEPARATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(itemTracking.TRACKING_TIME);
                        if (rdo.lstDHST != null)
                        {
                            var dhst = rdo.lstDHST.OrderByDescending(o => o.MODIFY_TIME).FirstOrDefault(o => o.TRACKING_ID == itemTracking.ID);

                            if (dhst != null)
                            {
                                _service.BELLY = dhst.BELLY;
                                _service.BLOOD_PRESSURE_MAX = dhst.BLOOD_PRESSURE_MAX;
                                _service.BLOOD_PRESSURE_MIN = dhst.BLOOD_PRESSURE_MIN;
                                _service.BREATH_RATE = dhst.BREATH_RATE;
                                _service.CHEST = dhst.CHEST;
                                _service.HEIGHT = dhst.HEIGHT;
                                _service.PULSE = dhst.PULSE;
                                _service.TEMPERATURE = dhst.TEMPERATURE;
                                _service.VIR_BMI = dhst.VIR_BMI;
                                _service.VIR_BODY_SURFACE_AREA = dhst.VIR_BODY_SURFACE_AREA;
                                _service.WEIGHT = dhst.WEIGHT;

                            }
                        }
                        #endregion

                        #region ServiceReqIds
                        List<long> ServiceReqIdsByTrackingId = new List<long>();
                        if (rdo.lstServiceReq != null && rdo.lstServiceReq.Count > 0)
                        {
                            ServiceReqIdsByTrackingId = rdo.lstServiceReq.Where(p => p.TRACKING_ID == itemTracking.ID).Select(x => x.ID).ToList();
                        }
                        #endregion

                        #region Dịch vụ CLS
                        List<V_HIS_SERE_SERV_7> sereServCLSByTracking = new List<V_HIS_SERE_SERV_7>();
                        if (ServiceReqIdsByTrackingId != null && ServiceReqIdsByTrackingId.Count > 0)
                        {
                            sereServCLSByTracking = rdo.lstSereServCLS.Where(p => ServiceReqIdsByTrackingId.Contains(p.SERVICE_REQ_ID)).ToList();
                            var CLSGroup = sereServCLSByTracking.GroupBy(p => p.SERVICE_TYPE_ID).Select(x => x.ToList()).ToList();
                            foreach (var itemGroups in CLSGroup)
                            {
                                MPS.ADO.TrackingPrint.Mps000062ADOServiceCLS group = new MPS.ADO.TrackingPrint.Mps000062ADOServiceCLS(itemGroups[0]);
                                group.SERVICE_NAME = "";
                                group.TRACKING_ID = itemTracking.ID;
                                foreach (var item in itemGroups)
                                {
                                    group.SERVICE_NAME += item.SERVICE_NAME + "; ";
                                }
                                lstMps000062ADOServiceCLS.Add(group);
                            }
                        }
                        #endregion

                        #region Thuốc

                        MPS.ADO.TrackingPrint.Mps000062RemedyCount remedyCountAddNull = new Mps000062RemedyCount();
                        remedyCountAddNull.TRACKING_ID = itemTracking.ID;
                        remedyCountAddNull.ID = 0;
                        remedyCountAddNull.REMEDY_COUNT = null;
                        if (rdo.lstPrescription != null && rdo.lstPrescription.Count > 0)
                        {
                            List<V_HIS_PRESCRIPTION> prescription = new List<V_HIS_PRESCRIPTION>();
                            if (ServiceReqIdsByTrackingId != null && ServiceReqIdsByTrackingId.Count > 0)
                            {
                                prescription = rdo.lstPrescription.Where(p => ServiceReqIdsByTrackingId.Contains(p.SERVICE_REQ_ID)).ToList().OrderBy(p => p.NUM_ORDER).ToList();

                                if (prescription != null && prescription.Count > 0)
                                {
                                    foreach (var itemPrescription in prescription)
                                    {
                                        //if (itemPrescription.REMEDY_COUNT > 0)
                                        //{
                                        MPS.ADO.TrackingPrint.Mps000062RemedyCount remedyCount = new MPS.ADO.TrackingPrint.Mps000062RemedyCount();
                                        remedyCount.TRACKING_ID = itemTracking.ID;
                                        remedyCount.ID = itemPrescription.EXP_MEST_ID;
                                        remedyCount.REMEDY_COUNT = itemPrescription.REMEDY_COUNT;
                                        lstMps000062RemedyCount.Add(remedyCount);
                                        //}
                                        List<V_HIS_EXP_MEST_MEDICINE> expMestMedicineByExpMestId = new List<V_HIS_EXP_MEST_MEDICINE>();
                                        expMestMedicineByExpMestId = rdo.lstExpMestMedicine.Where(p => p.EXP_MEST_ID == itemPrescription.EXP_MEST_ID).OrderBy(p => p.NUM_ORDER).ToList();
                                        foreach (var itemExpMestMedicine in expMestMedicineByExpMestId)
                                        {
                                            V_HIS_EXP_MEST_MEDICINE expMestMedicine = new V_HIS_EXP_MEST_MEDICINE();
                                            AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_METY, V_HIS_EXP_MEST_MEDICINE>();
                                            expMestMedicine = AutoMapper.Mapper.Map<V_HIS_EXP_MEST_MEDICINE>(itemExpMestMedicine);
                                            MPS.ADO.TrackingPrint.Mps000062ADOMedicines group = new MPS.ADO.TrackingPrint.Mps000062ADOMedicines(itemExpMestMedicine);
                                            group.TRACKING_ID = itemTracking.ID;
                                            group.REMEDY_COUNT = itemPrescription.REMEDY_COUNT;
                                            if (dicCountNumber != null && dicCountNumber.Count > 0 && dicCountNumber.ContainsKey(itemExpMestMedicine.MEDICINE_TYPE_ID))
                                            {
                                                var lstData = dicCountNumber[itemExpMestMedicine.MEDICINE_TYPE_ID];
                                                group.NUMBER_H_N = lstData.FirstOrDefault(p => p.INTRUCTION_DATE == Inventec.Common.TypeConvert.Parse.ToInt64(itemPrescription.INTRUCTION_TIME.ToString().Substring(0, 8))).num;
                                            }
                                            if (group.REMEDY_COUNT != null)
                                            {
                                                group.Amount_By_Remedy_Count = Inventec.Common.Number.Convert.NumberToNumberRoundAuto((group.AMOUNT / group.REMEDY_COUNT) ?? 0, 2);
                                            }
                                            lstMps000062ADOMedicines.Add(group);
                                        }
                                    }
                                }
                                else
                                {
                                    lstMps000062RemedyCount.Add(remedyCountAddNull);
                                }
                            }
                            else
                            {
                                lstMps000062RemedyCount.Add(remedyCountAddNull);
                            }
                        }
                        else
                        {
                            lstMps000062RemedyCount.Add(remedyCountAddNull);
                        }
                        #endregion
                        _service.ICD_NAME_TRACKING = icd_Name;
                        mps000062ADOs.Add(_service);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void SetBarcodeKey()
        {
            try
            {
                Inventec.Common.BarcodeLib.Barcode barcodePatientCode = new Inventec.Common.BarcodeLib.Barcode(rdo.currentTreatment.PATIENT_CODE);
                barcodePatientCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcodePatientCode.IncludeLabel = false;
                barcodePatientCode.Width = 120;
                barcodePatientCode.Height = 40;
                barcodePatientCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                barcodePatientCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcodePatientCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcodePatientCode.IncludeLabel = true;

                dicImage.Add(Mps000062ExtendSingleKey.BARCODE_PATIENT_CODE_STR, barcodePatientCode);

                Inventec.Common.BarcodeLib.Barcode barcodeTreatment = new Inventec.Common.BarcodeLib.Barcode(rdo.currentTreatment.TREATMENT_CODE);
                barcodeTreatment.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcodeTreatment.IncludeLabel = false;
                barcodeTreatment.Width = 120;
                barcodeTreatment.Height = 40;
                barcodeTreatment.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                barcodeTreatment.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcodeTreatment.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcodeTreatment.IncludeLabel = true;

                dicImage.Add(Mps000062ExtendSingleKey.BARCODE_TREATMENT_CODE_STR, barcodeTreatment);
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
        protected bool ProcessData()
        {
            bool result = false;
            try
            {
                if (rdo.keyVienTim == 1)
                {
                    BBandBG();
                }
                //ProcessListADO();
                ProcessListADOV222();

                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                SetBarcodeKey();
                SetSingleKey();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);
                //Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => mps000062ADOs), mps000062ADOs));
                //Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rdo.lstDHST), rdo.lstDHST));
                List<Mps000062ADOMedicines> listDataMedi = new List<Mps000062ADOMedicines>();
                if (lstMps000062ADOMedicines != null && lstMps000062ADOMedicines.Count > 0)
                {
                    var adoGroup = lstMps000062ADOMedicines.Where(p => p.REMEDY_COUNT == null && p.NUMBER_H_N == null).GroupBy(p => new { p.MEDICINE_TYPE_ID, p.TRACKING_ID, p.MEDICINE_USE_FORM_ID }).ToList();
                    var ado2 = lstMps000062ADOMedicines.Where(p => p.REMEDY_COUNT != null || p.NUMBER_H_N != null);
                    foreach (var item in adoGroup)
                    {
                        Mps000062ADOMedicines ado = new Mps000062ADOMedicines();
                        ado = AutoMapper.Mapper.Map<Mps000062ADOMedicines, Mps000062ADOMedicines>(item.FirstOrDefault());
                        ado.AMOUNT = item.Sum(p => p.AMOUNT);
                        listDataMedi.Add(ado);
                    }
                    if (ado2 != null && ado2.Count() > 0)
                    {
                        listDataMedi.AddRange(ado2);
                    }
                    listDataMedi = listDataMedi.OrderBy(p => p.TRACKING_ID).ThenBy(p => p.NUM_ORDER).ToList();
                }

                List<Mps000062ADOMaterials> listDataMate = new List<Mps000062ADOMaterials>();
                if (lstMps000062ADOMaterials != null && lstMps000062ADOMaterials.Count > 0)
                {
                    var adoGroup = lstMps000062ADOMaterials.GroupBy(p => new { p.MATERIAL_TYPE_ID, p.TRACKING_ID }).ToList();
                    foreach (var item in adoGroup)
                    {
                        Mps000062ADOMaterials ado = new Mps000062ADOMaterials();
                        ado = AutoMapper.Mapper.Map<Mps000062ADOMaterials, Mps000062ADOMaterials>(item.FirstOrDefault());
                        ado.AMOUNT = item.Sum(p => p.AMOUNT);
                        listDataMate.Add(ado);
                    }
                    listDataMate = listDataMate.OrderBy(p => p.TRACKING_ID).ThenBy(p => p.NUM_ORDER).ToList();
                }
                objectTag.AddObjectData(store, "TrackingADOs", mps000062ADOs);
                objectTag.AddObjectData(store, "RemedyCount", lstMps000062RemedyCount);
                objectTag.AddObjectData(store, "Medicines", listDataMedi);
                objectTag.AddObjectData(store, "Materials", lstMps000062ADOMaterials);
                objectTag.AddObjectData(store, "ServiceCLS", lstMps000062ADOServiceCLS);
                objectTag.AddObjectData(store, "CARE", rdo.lstHisCare);
                objectTag.AddObjectData(store, "CareDetail", rdo.lstHisCareDetail);
                objectTag.AddRelationship(store, "TrackingADOs", "RemedyCount", "ID", "TRACKING_ID");
                objectTag.AddRelationship(store, "RemedyCount", "Medicines", "ID", "EXP_MEST_ID");
                objectTag.AddRelationship(store, "TrackingADOs", "Materials", "ID", "TRACKING_ID");
                objectTag.AddRelationship(store, "TrackingADOs", "ServiceCLS", "ID", "TRACKING_ID");
                objectTag.AddRelationship(store, "TrackingADOs", "CARE", "ID", "TRACKING_ID");
                objectTag.AddRelationship(store, "TrackingADOs", "CareDetail", "ID", "TRACKING_ID");

                //objectTag.SetUserFunction(store, "FuncSameTitleCol1", new CustomerFuncMergeSameData(lstMps000062ADOMedicines, lstMps000062ADOServiceCLS, 1));
                //objectTag.SetUserFunction(store, "FuncSameTitleCol2", new CustomerFuncMergeSameData(lstMps000062ADOMedicines, lstMps000062ADOServiceCLS, 2));
                //objectTag.SetUserFunction(store, "FuncSameTitleColRemedy1", new CustomerFuncMergeSameDataRemedy(lstMps000062RemedyCount, 1));
                //objectTag.SetUserFunction(store, "FuncSameTitleColRemedy2", new CustomerFuncMergeSameDataRemedy(lstMps000062RemedyCount, 2));

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void ProcessListADOV222()
        {
            try
            {
                mps000062ADOs = new List<Mps000062ADO>();
                lstMps000062ADOMedicines = new List<Mps000062ADOMedicines>();
                lstMps000062ADOServiceCLS = new List<Mps000062ADOServiceCLS>();
                lstMps000062RemedyCount = new List<Mps000062RemedyCount>();
                lstMps000062ADOMaterials = new List<Mps000062ADOMaterials>();

                if (rdo.lstTracking != null && rdo.lstTracking.Count > 0)
                {
                    foreach (var itemTracking in rdo.lstTracking)
                    {
                        #region Thông tin chẩn đoán tờ điều trị
                        string icd_Name = "";
                        if (string.IsNullOrEmpty(itemTracking.ICD_MAIN_TEXT))
                        {
                            var icd = rdo.lstIcdId.Where(p => p.ID == itemTracking.ICD_ID).FirstOrDefault();
                            if (icd != null)
                            {
                                icd_Name = icd.ICD_NAME;
                            }
                        }
                        else
                        {
                            icd_Name = itemTracking.ICD_MAIN_TEXT;
                        }

                        #endregion

                        #region TrackingTime
                        MPS.ADO.Mps000062ADO _service = new Mps000062ADO();

                        Mapper.CreateMap<HIS_TRACKING, Mps000062ADO>();
                        _service = Mapper.Map<HIS_TRACKING, Mps000062ADO>(itemTracking);
                        _service.TRACKING_TIME_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(itemTracking.TRACKING_TIME);
                        _service.TRACKING_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(itemTracking.TRACKING_TIME);
                        _service.TRACKING_DATE_SEPARATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(itemTracking.TRACKING_TIME);
                        if (rdo.lstDHST != null)
                        {
                            var dhst = rdo.lstDHST.OrderByDescending(o => o.MODIFY_TIME).FirstOrDefault(o => o.TRACKING_ID == itemTracking.ID);

                            if (dhst != null)
                            {
                                _service.BELLY = dhst.BELLY;
                                _service.BLOOD_PRESSURE_MAX = dhst.BLOOD_PRESSURE_MAX;
                                _service.BLOOD_PRESSURE_MIN = dhst.BLOOD_PRESSURE_MIN;
                                _service.BREATH_RATE = dhst.BREATH_RATE;
                                _service.CHEST = dhst.CHEST;
                                _service.HEIGHT = dhst.HEIGHT;
                                _service.PULSE = dhst.PULSE;
                                _service.TEMPERATURE = dhst.TEMPERATURE;
                                _service.VIR_BMI = dhst.VIR_BMI;
                                _service.VIR_BODY_SURFACE_AREA = dhst.VIR_BODY_SURFACE_AREA;
                                _service.WEIGHT = dhst.WEIGHT;

                            }
                        }
                        #endregion

                        #region ServiceReqIds
                        List<long> ServiceReqIdsByTrackingId = new List<long>();
                        if (rdo.lstServiceReq != null && rdo.lstServiceReq.Count > 0)
                        {
                            ServiceReqIdsByTrackingId = rdo.lstServiceReq.Where(p => p.TRACKING_ID == itemTracking.ID).Select(x => x.ID).ToList();
                        }
                        #endregion

                        #region Dịch vụ CLS
                        List<V_HIS_SERE_SERV_7> sereServCLSByTracking = new List<V_HIS_SERE_SERV_7>();
                        if (ServiceReqIdsByTrackingId != null && ServiceReqIdsByTrackingId.Count > 0)
                        {
                            sereServCLSByTracking = rdo.lstSereServCLS.Where(p => ServiceReqIdsByTrackingId.Contains(p.SERVICE_REQ_ID)).ToList();
                            var CLSGroup = sereServCLSByTracking.GroupBy(p => p.SERVICE_TYPE_ID).Select(x => x.ToList()).ToList();
                            foreach (var itemGroups in CLSGroup)
                            {
                                MPS.ADO.TrackingPrint.Mps000062ADOServiceCLS group = new MPS.ADO.TrackingPrint.Mps000062ADOServiceCLS(itemGroups[0]);
                                group.SERVICE_NAME = "";
                                group.TRACKING_ID = itemTracking.ID;
                                foreach (var item in itemGroups)
                                {
                                    group.SERVICE_NAME += item.SERVICE_NAME + "; ";
                                }
                                lstMps000062ADOServiceCLS.Add(group);
                            }
                        }
                        #endregion

                        #region Thuốc
                        //Gan mac dinh thang
                        MPS.ADO.TrackingPrint.Mps000062RemedyCount remedyCountAddNull = new Mps000062RemedyCount();
                        remedyCountAddNull.TRACKING_ID = itemTracking.ID;
                        remedyCountAddNull.ID = 0;
                        remedyCountAddNull.REMEDY_COUNT = 0;

                        List<V_HIS_PRESCRIPTION> prescription = new List<V_HIS_PRESCRIPTION>();
                        if (ServiceReqIdsByTrackingId != null && ServiceReqIdsByTrackingId.Count > 0)
                        {
                            Inventec.Common.Logging.LogSystem.Info("Bat dau duyet====================");
                            foreach (var dic in rdo.dicPrescription)
                            {
                                if (!ServiceReqIdsByTrackingId.Contains(dic.Key))
                                    continue;
                                var itemPrescription = dic.Value;
                                MPS.ADO.TrackingPrint.Mps000062RemedyCount remedyCount = new MPS.ADO.TrackingPrint.Mps000062RemedyCount();
                                remedyCount.TRACKING_ID = itemTracking.ID;
                                remedyCount.ID = itemPrescription.EXP_MEST_ID;
                                remedyCount.REMEDY_COUNT = itemPrescription.REMEDY_COUNT;
                                lstMps000062RemedyCount.Add(remedyCount);
                                //}
                                List<V_HIS_EXP_MEST_MEDICINE> expMestMedicineByExpMestId = new List<V_HIS_EXP_MEST_MEDICINE>();
                                if (rdo.dicVHisExpMestMedicine.ContainsKey(itemPrescription.EXP_MEST_ID))
                                {
                                    expMestMedicineByExpMestId = rdo.dicVHisExpMestMedicine[itemPrescription.EXP_MEST_ID].OrderBy(p => p.NUM_ORDER).ToList();
                                }
                                Inventec.Common.Logging.LogSystem.Info("Bat dau duyet itemExpMestMedicine====================");
                                foreach (var itemExpMestMedicine in expMestMedicineByExpMestId)
                                {
                                    MPS.ADO.TrackingPrint.Mps000062ADOMedicines group = new MPS.ADO.TrackingPrint.Mps000062ADOMedicines(itemExpMestMedicine);
                                    group.TRACKING_ID = itemTracking.ID;

                                    if (dicCountNumber != null && dicCountNumber.Count > 0 && dicCountNumber.ContainsKey(itemExpMestMedicine.MEDICINE_TYPE_ID))
                                    {
                                        var lstData = dicCountNumber[itemExpMestMedicine.MEDICINE_TYPE_ID];
                                        group.NUMBER_H_N = lstData.FirstOrDefault(p => p.INTRUCTION_DATE == Inventec.Common.TypeConvert.Parse.ToInt64(itemPrescription.INTRUCTION_TIME.ToString().Substring(0, 8))).num;
                                    }
                                    if (group.REMEDY_COUNT != null)
                                    {
                                        group.REMEDY_COUNT = itemPrescription.REMEDY_COUNT;
                                        group.Amount_By_Remedy_Count = Inventec.Common.Number.Convert.NumberToNumberRoundAuto((group.AMOUNT / group.REMEDY_COUNT) ?? 0, 2);
                                    }
                                    lstMps000062ADOMedicines.Add(group);
                                }
                                Inventec.Common.Logging.LogSystem.Info("Ket thúc duyet itemExpMestMedicine====================");
                                //VT
                                if (rdo.dicVHisExpMestMaterial != null && rdo.dicVHisExpMestMaterial.Count > 0 && rdo.dicVHisExpMestMaterial.ContainsKey(itemPrescription.EXP_MEST_ID))
                                {
                                    List<V_HIS_EXP_MEST_MATERIAL> lstMaterial = new List<V_HIS_EXP_MEST_MATERIAL>();
                                    if (rdo.dicVHisExpMestMaterial.ContainsKey(itemPrescription.EXP_MEST_ID))
                                    {
                                        lstMaterial = rdo.dicVHisExpMestMaterial[itemPrescription.EXP_MEST_ID].OrderBy(p => p.NUM_ORDER).ToList();
                                    }
                                    foreach (var itemMate in lstMaterial)
                                    {
                                        Mps000062ADOMaterials ado = new Mps000062ADOMaterials(itemMate);
                                        ado.TRACKING_ID = itemTracking.ID;
                                        lstMps000062ADOMaterials.Add(ado);
                                    }
                                }
                            }

                            Inventec.Common.Logging.LogSystem.Info("Ket thúc duyet====================");
                        }
                        else
                        {
                            lstMps000062RemedyCount.Add(remedyCountAddNull);
                        }
                        #endregion
                        _service.ICD_NAME_TRACKING = icd_Name;
                        mps000062ADOs.Add(_service);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        class NumberDate
        {
            public long INTRUCTION_DATE { get; set; }
            public long MEDICINE_TYPE_ID { get; set; }
            public int num { get; set; }
        }

        public class Mps000062ADOMaterials : V_HIS_EXP_MEST_MATERIAL
        {
            public long TRACKING_ID { get; set; }

            public Mps000062ADOMaterials() { }
            public Mps000062ADOMaterials(V_HIS_EXP_MEST_MATERIAL data)
            {
                try
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<Mps000062ADOMaterials>(this, data);
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
        }
    }
}
