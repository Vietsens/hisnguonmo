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
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace MPS.Core.Mps000169
{
    class Mps000169Processor : ProcessorBase, IProcessorPrint
    {
        Mps000169RDO rdo;
        Inventec.Common.FlexCellExport.Store store;
        string fileName;
        internal Dictionary<string, Inventec.Common.BarcodeLib.Barcode> dicImage = null;
        List<ExpMestMedicinePrintADO> ExpMestManuMedicineSumForPrints = new List<ExpMestMedicinePrintADO>();
        List<PrescriptionADO> listPresADO = new List<PrescriptionADO>();
        List<PrescriptionADO> listPatientADO = new List<PrescriptionADO>();
        List<ExpMestMedicinePrintADO> lstPrintADO = new List<ExpMestMedicinePrintADO>();

        internal Mps000169Processor(SAR_PRINT_TYPE config, string fileName, object data, MPS.Printer.PreviewType previewType, string printerName)
            : base(config, (RDOBase)data, previewType, printerName)
        {
            rdo = (Mps000169RDO)data;
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

                //Process list input data to list ADO for print
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

        internal void SetBarcodeKey()
        {
            try
            {
                Inventec.Common.BarcodeLib.Barcode expMestCodeBar = new Inventec.Common.BarcodeLib.Barcode(rdo.AggrExpMest.EXP_MEST_CODE);
                expMestCodeBar.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                expMestCodeBar.IncludeLabel = false;
                expMestCodeBar.Width = 120;
                expMestCodeBar.Height = 40;
                expMestCodeBar.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                expMestCodeBar.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                expMestCodeBar.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                expMestCodeBar.IncludeLabel = true;

                dicImage.Add(Mps000169ExtendSingleKey.EXP_MEST_CODE_BARCODE, expMestCodeBar);

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
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);
                objectTag.AddObjectData(store, "ExpMestAggregates", lstPrintADO);
                objectTag.AddObjectData(store, "Patient", listPatientADO);
                objectTag.AddRelationship(store, "ExpMestAggregates", "Patient", "MEDICINE_TYPE_ID", "MEDICINE_TYPE_ID");
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        void ProcessListADO()
        {
            try
            {
                ExpMestManuMedicineSumForPrints = new List<ExpMestMedicinePrintADO>();

                List<ExpMestMedicinePrintADO> lstPrintRequestADO = new List<ExpMestMedicinePrintADO>();
                if (rdo.AggrExpMest.EXP_MEST_STT_ID == rdo.HisExpMestSttId__Exported || rdo.AggrExpMest.EXP_MEST_STT_ID == rdo.HisExpMestSttId__Approved)
                {
                    lstPrintRequestADO = rdo.MedicineExpmestTypeADOs.Where(p => p.IN_EXECUTE == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_MEDICINE.IN_EXECUTE__TRUE).ToList();
                    foreach (var item in lstPrintRequestADO)
                    {
                        if (rdo.AggrExpMest.EXP_MEST_STT_ID == rdo.HisExpMestSttId__Exported)
                        {
                            item.AMOUNT_EXPORTED = item.AMOUNT;
                        }
                        item.AMOUNT_EXCUTE = item.AMOUNT;
                    }
                }
                else if (rdo.AggrExpMest.EXP_MEST_STT_ID == rdo.HisExpMestSttId__Draft || rdo.AggrExpMest.EXP_MEST_STT_ID == rdo.HisExpMestSttId__Request || rdo.AggrExpMest.EXP_MEST_STT_ID == rdo.HisExpMestSttId__Rejected)
                {
                    lstPrintRequestADO = rdo.MedicineExpmestTypeADOs.Where(p => p.IN_REQUEST == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_MEDICINE.IN_REQUEST__TRUE).ToList();
                }
                Dictionary<long, V_HIS_PRESCRIPTION> dicPrescription = new Dictionary<long, V_HIS_PRESCRIPTION>();
                if (rdo.listPrescription != null && rdo.listPrescription.Count > 0)
                {
                    foreach (var item in rdo.listPrescription)
                    {
                        dicPrescription[item.EXP_MEST_ID] = item;
                    }
                }

                var listAdoGroup = lstPrintRequestADO.GroupBy(p => p.MEDICINE_TYPE_ID).ToList();
                lstPrintADO = new List<ExpMestMedicinePrintADO>();
                foreach (var itemMediGr in listAdoGroup)
                {
                    ExpMestMedicinePrintADO ado = new ExpMestMedicinePrintADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<ExpMestMedicinePrintADO>(ado, itemMediGr.FirstOrDefault());

                    var checkInRequest = rdo.MedicineExpmestTypeADOs.Where(p => p.IN_REQUEST == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_MEDICINE.IN_REQUEST__TRUE && p.MEDICINE_TYPE_ID == itemMediGr.Key).Sum(o => o.AMOUNT);
                    if (checkInRequest > 0)
                    {
                        ado.AMOUNT_REQUEST = checkInRequest;
                    }
                    var checkInExecute = rdo.MedicineExpmestTypeADOs.Where
(p => p.IN_EXECUTE == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_MEDICINE.IN_EXECUTE__TRUE && p.MEDICINE_TYPE_ID == itemMediGr.Key).Sum(o => o.AMOUNT);
                    if (checkInExecute > 0)
                    {
                        if (rdo.AggrExpMest.EXP_MEST_STT_ID == rdo.HisExpMestSttId__Exported)
                        {
                            ado.AMOUNT_EXPORTED = checkInExecute;
                        }
                        ado.AMOUNT_EXCUTE = checkInExecute;
                    }


                    ado.AMOUNT = itemMediGr.Sum(p => p.AMOUNT);
                    //ado.AMOUNT_REQUEST = itemMediGr.Sum(p => p.AMOUNT_REQUEST);
                    //ado.AMOUNT_EXPORTED = itemMediGr.Sum(p => p.AMOUNT_EXPORTED);
                    ado.AMOUNT_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(ado.AMOUNT_REQUEST ?? 0)));
                    ado.AMOUNT_EXPORTED_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(ado.AMOUNT_EXPORTED ?? 0)));
                    ado.AMOUNT_EXCUTE_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(ado.AMOUNT_EXCUTE ?? 0)));

                    if (MPS.PrintConfig.VHisMedicineTypes != null && MPS.PrintConfig.VHisMedicineTypes.Count > 0)
                    {
                        var medicineData = MPS.PrintConfig.VHisMedicineTypes.FirstOrDefault(o => o.ID == ado.MEDICINE_TYPE_ID);
                        if (medicineData != null)
                        {
                            ado.DESCRIPTION = medicineData.DESCRIPTION;
                            ado.CONCENTRA = medicineData.CONCENTRA;
                            ado.PACKING_TYPE_NAME = medicineData.PACKING_TYPE_NAME;
                            ado.CONCENTRA_PACKING_TYPE_NAME = medicineData.PACKING_TYPE_NAME;
                            if (!String.IsNullOrEmpty(medicineData.PACKING_TYPE_NAME))
                            {
                                ado.CONCENTRA_PACKING_TYPE_NAME += ";" + medicineData.PACKING_TYPE_NAME;
                            }
                        }
                    }

                    lstPrintADO.Add(ado);
                    Dictionary<long, PrescriptionADO> dicData = new Dictionary<long, PrescriptionADO>();
                    Dictionary<long, object> dicCheckRequest = new Dictionary<long, object>();
                    foreach (var expMedicine in itemMediGr.ToList<ExpMestMedicinePrintADO>())
                    {

                        if (dicPrescription.ContainsKey(expMedicine.EXP_MEST_ID))
                        {
                            var pres = dicPrescription[expMedicine.EXP_MEST_ID];
                            PrescriptionADO adoTreat = null;
                            if (dicData.ContainsKey(pres.TREATMENT_ID))
                            {
                                adoTreat = dicData[pres.TREATMENT_ID];
                                adoTreat.AMOUNT += expMedicine.AMOUNT_REQUEST ?? 0;
                                if (rdo.AggrExpMest.EXP_MEST_STT_ID == rdo.HisExpMestSttId__Exported)
                                {
                                    //if (adoTreat.AMOUNT_EXPORTED == null)
                                    //    adoTreat.AMOUNT_EXPORTED = 0;
                                    adoTreat.AMOUNT_EXPORTED += expMedicine.AMOUNT_EXPORTED ?? 0;
                                }
                                //if (adoTreat.AMOUNT_EXCUTE == null)
                                //    adoTreat.AMOUNT_EXCUTE = 0;
                                adoTreat.AMOUNT_EXCUTE += expMedicine.AMOUNT_EXCUTE ?? 0;
                            }
                            else
                            {
                                adoTreat = new PrescriptionADO();
                                adoTreat.MEDICINE_TYPE_ID = itemMediGr.Key;
                                adoTreat.PATIENT_CODE = pres.PATIENT_CODE;
                                adoTreat.VIR_PATIENT_NAME = pres.VIR_PATIENT_NAME;
                                dicData[pres.TREATMENT_ID] = adoTreat;
                                adoTreat.AMOUNT = expMedicine.AMOUNT_REQUEST ?? 0;
                                if (rdo.AggrExpMest.EXP_MEST_STT_ID == rdo.HisExpMestSttId__Exported)
                                {
                                    adoTreat.AMOUNT_EXPORTED = expMedicine.AMOUNT_EXPORTED ?? 0;
                                }
                                adoTreat.AMOUNT_EXCUTE = expMedicine.AMOUNT_EXCUTE ?? 0;
                            }

                            if (!dicCheckRequest.ContainsKey(expMedicine.EXP_MEST_ID))
                            {
                                var amuontReq = rdo.MedicineExpmestTypeADOs.Where(p => p.IN_REQUEST == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_MEDICINE.IN_REQUEST__TRUE && p.EXP_MEST_ID == expMedicine.EXP_MEST_ID && p.MEDICINE_TYPE_ID == itemMediGr.Key).Sum(o => o.AMOUNT);
                                adoTreat.AMOUNT += amuontReq;
                                dicCheckRequest[expMedicine.EXP_MEST_ID] = true;
                            }
                        }
                        else
                        {
                            Inventec.Common.Logging.LogSystem.Error("Khong co thong tin don thuoc");
                        }
                    }
                    listPatientADO.AddRange(dicData.Select(s => s.Value).ToList());
                }
                foreach (var itemPatient in listPatientADO)
                {
                    itemPatient.AMOUNT_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(itemPatient.AMOUNT)));
                    itemPatient.AMOUNT_EXPORTED_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(itemPatient.AMOUNT_EXPORTED)));
                    itemPatient.AMOUNT_EXCUTE_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(itemPatient.AMOUNT_EXCUTE)));
                }
                if (lstPrintADO != null && lstPrintADO.Count > 0)
                {
                    lstPrintADO = lstPrintADO.OrderBy(p => p.MEDICINE_TYPE_NAME).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public class PrescriptionADO : V_HIS_PRESCRIPTION
        {
            public decimal AMOUNT_EXCUTE { get; set; }
            public string AMOUNT_EXCUTE_STRING { get; set; }
            public decimal AMOUNT_EXPORTED { get; set; }
            public string AMOUNT_EXPORTED_STRING { get; set; }
            public decimal AMOUNT { get; set; }
            public string AMOUNT_STRING { get; set; }
            public long MEDICINE_ID { get; set; }
            public long MEDICINE_TYPE_ID { get; set; }

            public PrescriptionADO() { }
            public PrescriptionADO(V_HIS_PRESCRIPTION data)
            {
                if (data != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<PrescriptionADO>(this, data);
                }
            }
        }
    }
}
