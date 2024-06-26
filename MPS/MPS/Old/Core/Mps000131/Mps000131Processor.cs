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
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using MPS.ADO;
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace MPS.Core.Mps000131
{
    class Mps000131Processor : ProcessorBase, IProcessorPrint
    {
        Mps000131RDO rdo;
        List<MPS.ADO.HisMediMateBloodInStock_Print> medicineInStockSdoPrints { get; set; }
        Inventec.Common.FlexCellExport.Store store;
        string fileName;
        internal Dictionary<string, Inventec.Common.BarcodeLib.Barcode> dicImage = null;

        internal Mps000131Processor(SAR_PRINT_TYPE config, string fileName, object data, MPS.Printer.PreviewType previewType, string printerName)
            : base(config, (RDOBase)data, previewType, printerName)
        {
            rdo = (Mps000131RDO)data;
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

                FunctionExcuteData(); //xử lý dữ liệu

                store.SetCommonFunctions();

                //Ham xu ly cac doi tuong su dung trong thu vien flexcelexport
                valid = valid && ProcessData();
                if (valid)
                {
                    using (MemoryStream streamResult = store.OutStream())
                    {
                        //Print preview
                        streamResult.Position = 0;
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
                //Inventec.Common.BarcodeLib.Barcode barcodePatientCode = new Inventec.Common.BarcodeLib.Barcode(rdo.ServiceReq.PATIENT_CODE);
                //barcodePatientCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                //barcodePatientCode.IncludeLabel = false;
                //barcodePatientCode.Width = 120;
                //barcodePatientCode.Height = 40;
                //barcodePatientCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                //barcodePatientCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                //barcodePatientCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                //barcodePatientCode.IncludeLabel = true;

                //dicImage.Add(Mps000131ExtendSingleKey.BARCODE_PATIENT_CODE_STR, barcodePatientCode);

                //Inventec.Common.BarcodeLib.Barcode barcodeTreatment = new Inventec.Common.BarcodeLib.Barcode(rdo.ServiceReq.TREATMENT_CODE);
                //barcodeTreatment.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                //barcodeTreatment.IncludeLabel = false;
                //barcodeTreatment.Width = 120;
                //barcodeTreatment.Height = 40;
                //barcodeTreatment.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                //barcodeTreatment.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                //barcodeTreatment.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                //barcodeTreatment.IncludeLabel = true;

                //dicImage.Add(Mps000131ExtendSingleKey.BARCODE_TREATMENT_CODE_STR, barcodeTreatment);

                //Inventec.Common.BarcodeLib.Barcode barcodeServiceReq = new Inventec.Common.BarcodeLib.Barcode(rdo.ServiceReq.SERVICE_REQ_CODE);
                //barcodeServiceReq.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                //barcodeServiceReq.IncludeLabel = false;
                //barcodeServiceReq.Width = 120;
                //barcodeServiceReq.Height = 40;
                //barcodeServiceReq.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                //barcodeServiceReq.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                //barcodeServiceReq.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                //barcodeServiceReq.IncludeLabel = true;

                //dicImage.Add(Mps000131ExtendSingleKey.BARCODE_SERVICE_REQ_CODE_STR, barcodeServiceReq);
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
                objectTag.AddObjectData(store, "ListSDO", medicineInStockSdoPrints);
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void FunctionExcuteData()
        {
            try
            {
                List<MOS.SDO.HisMedicineInStockSDO> medicineInStockSdos = new List<MOS.SDO.HisMedicineInStockSDO>();
                List<MOS.SDO.HisMaterialInStockSDO> materialInStockSdos = new List<MOS.SDO.HisMaterialInStockSDO>();
                List<MOS.SDO.HisMedicineInStockSDO> instockMedicines = new List<MOS.SDO.HisMedicineInStockSDO>();
                List<MOS.SDO.HisMaterialInStockSDO> instockMaterials = new List<MOS.SDO.HisMaterialInStockSDO>();
                List<MOS.SDO.HisMedicineInStockSDO> listRootMedicines = new List<MOS.SDO.HisMedicineInStockSDO>();
                List<MOS.SDO.HisMaterialInStockSDO> listRootMaterials = new List<MOS.SDO.HisMaterialInStockSDO>();
                medicineInStockSdoPrints = new List<HisMediMateBloodInStock_Print>();

                if (rdo.isCheckMedicine)
                {
                    List<MOS.SDO.HisMedicineInStockSDO> instockMedicineSdos = new List<MOS.SDO.HisMedicineInStockSDO>();
                    instockMedicineSdos = rdo.lstMedicineInStockSDO;
                    var selectedMedicines = instockMedicineSdos.Where(o => !String.IsNullOrEmpty(o.ParentNodeId)).ToList();
                    var selectedRootMedicines = instockMedicineSdos.Where(o => String.IsNullOrEmpty(o.ParentNodeId)).ToList();
                    Mapper.CreateMap<HisMedicineInStockSDO, HisMedicineInStockSDO>();
                    instockMedicines = Mapper.Map<List<HisMedicineInStockSDO>>(selectedMedicines);
                    listRootMedicines = Mapper.Map<List<HisMedicineInStockSDO>>(selectedRootMedicines);
                    int dem = 1;
                    foreach (var root in listRootMedicines)
                    {
                        root.MEDICINE_TYPE_NAME = dem + "." + root.MEDICINE_TYPE_NAME.ToUpper();
                        medicineInStockSdos.Add(root);
                        foreach (var medicineInStock in instockMedicines)
                        {
                            if (medicineInStock.ParentNodeId == root.NodeId)
                            {
                                medicineInStock.MEDICINE_TYPE_NAME = "     " + medicineInStock.MEDICINE_TYPE_NAME;
                                medicineInStockSdos.Add(medicineInStock);
                            }
                        }
                        dem++;
                    }

                    foreach (var item in medicineInStockSdos)
                    {
                        HisMediMateBloodInStock_Print medicine = new HisMediMateBloodInStock_Print();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HisMediMateBloodInStock_Print>(medicine, item);
                        medicine.EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(Convert.ToInt64
                        (item.EXPIRED_DATE ?? 0));
                        medicineInStockSdoPrints.Add(medicine);
                    }
                }
                else if (rdo.isCheckMaterial)
                {
                    List<MOS.SDO.HisMaterialInStockSDO> materialInstocks = new List<MOS.SDO.HisMaterialInStockSDO>();
                    materialInstocks = rdo.lstMaterialInStockSDO;
                    var selectedMaterials = materialInstocks.Where(o => !String.IsNullOrEmpty(o.ParentNodeId)).ToList();
                    var selectedRootMaterials = materialInstocks.Where(o => String.IsNullOrEmpty(o.ParentNodeId)).ToList();
                    Mapper.CreateMap<HisMaterialInStockSDO, HisMaterialInStockSDO>();
                    instockMaterials = Mapper.Map<List<HisMaterialInStockSDO>>(selectedMaterials);
                    listRootMaterials = Mapper.Map<List<HisMaterialInStockSDO>>(selectedRootMaterials);
                    instockMaterials = materialInstocks.Where(o => !String.IsNullOrEmpty(o.ParentNodeId)).ToList();
                    listRootMaterials = materialInstocks.Where(o => String.IsNullOrEmpty(o.ParentNodeId)).ToList();
                    int dem = 1;
                    foreach (var root in listRootMaterials)
                    {
                        MOS.SDO.HisMedicineInStockSDO rootMaterial = new MOS.SDO.HisMedicineInStockSDO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<MOS.SDO.HisMedicineInStockSDO>(rootMaterial, root);
                        rootMaterial.MEDICINE_TYPE_NAME = dem + "." + root.MATERIAL_TYPE_NAME.ToUpper();
                        rootMaterial.MEDICINE_TYPE_CODE = root.MATERIAL_TYPE_CODE;
                        medicineInStockSdos.Add(rootMaterial);
                        foreach (var materialInStock in instockMaterials)
                        {
                            if (materialInStock.ParentNodeId == root.NodeId)
                            {
                                MOS.SDO.HisMedicineInStockSDO rootMaterialChild = new MOS.SDO.HisMedicineInStockSDO();
                                Inventec.Common.Mapper.DataObjectMapper.Map<MOS.SDO.HisMedicineInStockSDO>(rootMaterialChild, materialInStock);
                                rootMaterialChild.MEDICINE_TYPE_NAME = "     " + materialInStock.MATERIAL_TYPE_NAME;
                                rootMaterialChild.MEDICINE_TYPE_CODE = materialInStock.MATERIAL_TYPE_CODE;
                                medicineInStockSdos.Add(rootMaterialChild);
                            }
                        }
                        dem++;
                    }
                    foreach (var item in medicineInStockSdos)
                    {
                        HisMediMateBloodInStock_Print medicine = new HisMediMateBloodInStock_Print();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HisMediMateBloodInStock_Print>(medicine, item);
                        medicine.EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(Convert.ToInt64
                        (item.EXPIRED_DATE ?? 0));
                        medicineInStockSdoPrints.Add(medicine);
                    }
                }
                else
                {
                    List<V_HIS_BLOOD> lstBlood = new List<V_HIS_BLOOD>();
                    lstBlood = rdo.lstBlood;
                    foreach (var item in lstBlood)
                    {
                        HisMediMateBloodInStock_Print blood = new HisMediMateBloodInStock_Print();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HisMediMateBloodInStock_Print>(blood, item);

                        blood.MEDICINE_TYPE_NAME = item.BLOOD_TYPE_NAME;
                        blood.MEDICINE_TYPE_CODE = item.BLOOD_TYPE_CODE;
                        blood.EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(item.EXPIRED_DATE ?? 0);
                        blood.TotalAmount = item.BLOOD_VOLUME_ID;
                        medicineInStockSdoPrints.Add(blood);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
