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
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000056.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace MPS.Processor.Mps000056
{
    class Mps000056Processor : AbstractProcessor
    {
        Mps000056PDO rdo;
        public Mps000056Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000056PDO)rdoBase;
        }

        internal void SetBarcodeKey()
        {
            try
            {
                //Inventec.Common.BarcodeLib.Barcode barcodePatientCode = new Inventec.Common.BarcodeLib.Barcode(rdo.Patient.PATIENT_CODE);
                //barcodePatientCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                //barcodePatientCode.IncludeLabel = false;
                //barcodePatientCode.Width = 120;
                //barcodePatientCode.Height = 40;
                //barcodePatientCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                //barcodePatientCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                //barcodePatientCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                //barcodePatientCode.IncludeLabel = true;

                //dicImage.Add(Mps000056ExtendSingleKey.BARCODE_PATIENT_CODE_STR, barcodePatientCode);

                //Inventec.Common.BarcodeLib.Barcode barcodeTreatment = new Inventec.Common.BarcodeLib.Barcode(rdo.currentHisTreatment.TREATMENT_CODE);
                //barcodeTreatment.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                //barcodeTreatment.IncludeLabel = false;
                //barcodeTreatment.Width = 120;
                //barcodeTreatment.Height = 40;
                //barcodeTreatment.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                //barcodeTreatment.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                //barcodeTreatment.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                //barcodeTreatment.IncludeLabel = true;

                //dicImage.Add(Mps000056ExtendSingleKey.BARCODE_TREATMENT_CODE_STR, barcodeTreatment);
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

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                SetSingleKey();
                singleTag.ProcessData(store, singleValueDictionary);
                //barCodeTag.ProcessData(store, dicImage);
                //objectTag.AddObjectData(store, "ServiceMediReact", rdo.expMestMediReact);

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }
        void SetSingleKey()
        {
            try
            {
                if (rdo.expMestMediReact != null)
                {
                    var react = rdo.expMestMediReact.FirstOrDefault();
                    AddObjectKeyIntoListkey<ExeMediReactADO>(react, false);
                }
                if (rdo.expMestMediReact != null)
                {
                    var react = rdo.expMestMediReact.FirstOrDefault();
                    AddObjectKeyIntoListkey<ExeMediReactADO>(react, false);
                }
                SetSingleKey(new KeyValue(Mps000056ExtendSingleKey.BED_ROOM_NAME, rdo.bedRoomName));
                AddObjectKeyIntoListkey<V_HIS_MEDI_REACT>(rdo.serviceReq, false);
                AddObjectKeyIntoListkey<TreatmentADO>(rdo.currentHisTreatment, false);
                AddObjectKeyIntoListkey<PatientADO>(rdo.Patient);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
