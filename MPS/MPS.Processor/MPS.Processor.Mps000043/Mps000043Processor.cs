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
using MPS.Processor.Mps000043.PDO;
using MPS.ProcessorBase.Core;
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace MPS.Processor.Mps000043
{
    class Mps000043Processor : AbstractProcessor
    {
        Mps000043PDO rdo;
        public Mps000043Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000043PDO)rdoBase;
        }

        public void SetBarcodeKey()
        {
            try
            {
                Inventec.Common.BarcodeLib.Barcode barcodeTreatment = new Inventec.Common.BarcodeLib.Barcode(rdo.treatment.TREATMENT_CODE);
                barcodeTreatment.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcodeTreatment.IncludeLabel = false;
                barcodeTreatment.Width = 120;
                barcodeTreatment.Height = 40;
                barcodeTreatment.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                barcodeTreatment.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcodeTreatment.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcodeTreatment.IncludeLabel = true;

                dicImage.Add(Mps000043ExtendSingleKey.TREATMENT_CODE_BAR, barcodeTreatment);

                Inventec.Common.BarcodeLib.Barcode barcodePatient = new Inventec.Common.BarcodeLib.Barcode(rdo.patientADO.PATIENT_CODE);
                barcodePatient.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcodePatient.IncludeLabel = false;
                barcodePatient.Width = 120;
                barcodePatient.Height = 40;
                barcodePatient.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                barcodePatient.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcodePatient.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcodePatient.IncludeLabel = true;

                dicImage.Add(Mps000043ExtendSingleKey.PATIENT_CODE_BAR, barcodePatient);

                Inventec.Common.BarcodeLib.Barcode barcodeExpMest = new Inventec.Common.BarcodeLib.Barcode(rdo.HisPrescriptionSDO.ExpMest.EXP_MEST_CODE);
                barcodeExpMest.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcodeExpMest.IncludeLabel = false;
                barcodeExpMest.Width = 120;
                barcodeExpMest.Height = 40;
                barcodeExpMest.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                barcodeExpMest.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcodeExpMest.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcodeExpMest.IncludeLabel = true;

                dicImage.Add(Mps000043ExtendSingleKey.EXP_MEST_CODE_BAR, barcodeExpMest);
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
                barCodeTag.ProcessData(store, dicImage);
                objectTag.AddObjectData(store, "MedicineExpmest", rdo.lstMedicineExpmestTypeADO);
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
                SetSingleKey(new KeyValue(Mps000043ExtendSingleKey.EXP_MEST_CODE, rdo.resultCreateMedicine));
                if (rdo.mediStockExportName != null)
                    SetSingleKey(new KeyValue(Mps000043ExtendSingleKey.MEDI_STOCK_NAME, rdo.mediStockExportName));
                SetSingleKey(new KeyValue(Mps000043ExtendSingleKey.BED_ROOM_NAME, rdo.bebRoomName));
                if (rdo.HisPrescriptionSDO != null)
                {
                    SetSingleKey(new KeyValue(Mps000043ExtendSingleKey.REQ_USERNAME, this.rdo.HisPrescriptionSDO.ExpMest.REQ_USERNAME));
                    SetSingleKey(new KeyValue(Mps000043ExtendSingleKey.ICD_TEXT, this.rdo.HisPrescriptionSDO.IcdText));
                    SetSingleKey(new KeyValue(Mps000043ExtendSingleKey.ADVISE, this.rdo.HisPrescriptionSDO.Advise));
                    //SetSingleKey(new KeyValue(Mps000043ExtendSingleKey.USE_TIME_TO, Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.HisPrescriptionSDO.PrescriptionMedicines.Max(o => o.UseTimeTo)??0)));
                    SetSingleKey(new KeyValue(Mps000043ExtendSingleKey.USE_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.rdo.HisPrescriptionSDO.UseTime ?? 0)));
                }

                if (rdo.dataIcd != null)
                {
                    AddObjectKeyIntoListkey<HIS_ICD>(rdo.dataIcd);
                }

                if (rdo.dhst != null)
                {
                    AddObjectKeyIntoListkey<HIS_DHST>(rdo.dhst);
                }

                //if (hisServiceReqCombo != null)
                //{
                //    SetSingleKey(new KeyValue(Mps000043ExtendSingleKey.INSTRUCTION_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(hisServiceReqCombo.INTRUCTION_TIME)));

                //    AddObjectKeyIntoListkey<HisServiceReqCombo>(hisServiceReqCombo, false);
                //}
                if (rdo.HisPrescriptionSDO != null)
                {
                    AddObjectKeyIntoListkey<HisPrescriptionSDO>(rdo.HisPrescriptionSDO, false);
                }
                AddObjectKeyIntoListkey<PatyAlterBhytADO>(rdo.patyAlterBhytADO, false);
                AddObjectKeyIntoListkey<HIS_DEPARTMENT>(rdo.department, false);
                AddObjectKeyIntoListkey<PatientADO>(rdo.patientADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        class CustomerFuncRownumberData : TFlexCelUserFunction
        {
            public CustomerFuncRownumberData()
            {
            }
            public override object Evaluate(object[] parameters)
            {
                if (parameters == null || parameters.Length < 1)
                    throw new ArgumentException("Bad parameter count in call to Orders() user-defined function");

                long result = 0;
                try
                {
                    long rownumber = Convert.ToInt64(parameters[0]);
                    result = (rownumber + 1);
                }
                catch (Exception ex)
                {
                    LogSystem.Debug(ex);
                }

                return result;
            }
        }
    }
}
