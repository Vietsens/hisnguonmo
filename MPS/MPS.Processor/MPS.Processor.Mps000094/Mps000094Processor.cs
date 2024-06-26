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

namespace MPS.Processor.Mps000094
{
    class Mps000094Processor : AbstractProcessor
    {
        Mps000094PDO rdo;
        public Mps000094Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000094PDO)rdoBase;
        }

        private void SetBarcodeKey()
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
                    barcodeTreamtment.Width = 200;
                    barcodeTreamtment.Height = 100;
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
        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                SetBarcodeKey();
                SetSingleKey();
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
        private void SetSingleKey()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}


