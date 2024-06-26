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
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace MPS.Core.Mps000050
{
    class Mps000050Processor : ProcessorBase, IProcessorPrint
    {
        Mps000050RDO rdo;
        Inventec.Common.FlexCellExport.Store store;
        string fileName;
        internal Dictionary<string, Inventec.Common.BarcodeLib.Barcode> dicImage = null;

        internal Mps000050Processor(SAR_PRINT_TYPE config, string fileName, object data, MPS.Printer.PreviewType previewType, string printerName)
            : base(config, (RDOBase)data, previewType, printerName)
        {
            rdo = (Mps000050RDO)data;
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
                
                Inventec.Common.BarcodeLib.Barcode barcodeTreatment = new Inventec.Common.BarcodeLib.Barcode(rdo.vHisPrescription.TREATMENT_CODE);
                barcodeTreatment.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcodeTreatment.IncludeLabel = false;
                barcodeTreatment.Width = 120;
                barcodeTreatment.Height = 40;
                barcodeTreatment.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                barcodeTreatment.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcodeTreatment.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcodeTreatment.IncludeLabel = true;
                dicImage.Add(Mps000050ExtendSingleKey.BARCODE_TREATMENT_CODE_STR, barcodeTreatment);

                Inventec.Common.BarcodeLib.Barcode barcodePatient = new Inventec.Common.BarcodeLib.Barcode(rdo.Patient.PATIENT_CODE);
                barcodePatient.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcodePatient.IncludeLabel = false;
                barcodePatient.Width = 120;
                barcodePatient.Height = 40;
                barcodePatient.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                barcodePatient.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcodePatient.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcodePatient.IncludeLabel = true;
                dicImage.Add(Mps000050ExtendSingleKey.PATIENT_CODE_BAR, barcodePatient);

                Inventec.Common.BarcodeLib.Barcode barcodeExpMest = new Inventec.Common.BarcodeLib.Barcode(rdo.vHisPrescription.EXP_MEST_CODE);
                barcodeExpMest.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcodeExpMest.IncludeLabel = false;
                barcodeExpMest.Width = 120;
                barcodeExpMest.Height = 40;
                barcodeExpMest.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                barcodeExpMest.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcodeExpMest.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcodeExpMest.IncludeLabel = true;

                dicImage.Add(Mps000050ExtendSingleKey.EXP_MEST_CODE_BAR, barcodeExpMest);
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
                if (rdo.expMestMedicines != null && rdo.expMestMedicines.Count > 0)
                {
                    objectTag.AddObjectData(store, "ServiceMedicines", rdo.expMestMedicines);
                }
                
                if (rdo.medicineExpmestTypeADOs != null && rdo.medicineExpmestTypeADOs.Count > 0)
                {
                    objectTag.AddObjectData(store, "ServiceMedicines", rdo.medicineExpmestTypeADOs);
                }

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }
    }
}
