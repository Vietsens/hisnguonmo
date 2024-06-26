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
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace MPS.Core.Mps000094
{
    class Mps000094Processor : ProcessorBase, IProcessorPrint
    {
        Mps000094RDO rdo;
        Inventec.Common.FlexCellExport.Store store;
        string fileName;
        internal Dictionary<string, Inventec.Common.BarcodeLib.Barcode> dicImage = null;

        internal Mps000094Processor(SAR_PRINT_TYPE config, string fileName, object data, MPS.Printer.PreviewType previewType, string printerName)
            : base(config, (RDOBase)data, previewType, printerName)
        {
            rdo = (Mps000094RDO)data;
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
                //if (rdo.currentMediRecord != null && rdo.currentMediRecord.MEDI_RECORD_CODE != null)
                //{
                //    Inventec.Common.BarcodeLib.Barcode barcodeMedirecord = new Inventec.Common.BarcodeLib.Barcode(rdo.currentMediRecord.MEDI_RECORD_CODE);

                //    barcodeMedirecord.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                //    barcodeMedirecord.IncludeLabel = false;
                //    barcodeMedirecord.Width = 120;
                //    barcodeMedirecord.Height = 40;
                //    barcodeMedirecord.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                //    barcodeMedirecord.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                //    barcodeMedirecord.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                //    barcodeMedirecord.IncludeLabel = true;

                //    dicImage.Add(Mps000094ExtendSingleKey.BARCODE_MEDI_RECORD_CODE, barcodeMedirecord);
                //}
                //if (rdo.currentMediRecord != null && rdo.currentMediRecord.PATIENT_CODE != null)
                //{
                //    Inventec.Common.BarcodeLib.Barcode barcodePatient = new Inventec.Common.BarcodeLib.Barcode(rdo.currentMediRecord.PATIENT_CODE);
                //    barcodePatient.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                //    barcodePatient.IncludeLabel = false;
                //    barcodePatient.Width = 120;
                //    barcodePatient.Height = 40;
                //    barcodePatient.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                //    barcodePatient.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                //    barcodePatient.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                //    barcodePatient.IncludeLabel = true;

                //    dicImage.Add(Mps000094ExtendSingleKey.BARCODE_PATIENT_CODE_STR, barcodePatient);
                //}
                if (rdo.currentTreatment != null && rdo.currentTreatment.STORE_CODE != null)
                {
                    Inventec.Common.BarcodeLib.Barcode barcodeTreamtment = new Inventec.Common.BarcodeLib.Barcode(rdo.currentTreatment.STORE_CODE);
                    barcodeTreamtment.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                    barcodeTreamtment.IncludeLabel = false;
                    barcodeTreamtment.Width = 150;
                    barcodeTreamtment.Height = 80;
                    barcodeTreamtment.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                    barcodeTreamtment.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                    barcodeTreamtment.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                    barcodeTreamtment.IncludeLabel = true;

                    dicImage.Add(Mps000094ExtendSingleKey.BARCODE_STORE_CODE, barcodeTreamtment);
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
        protected bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);

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
