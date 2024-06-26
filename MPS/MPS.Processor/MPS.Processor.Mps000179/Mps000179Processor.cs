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
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000179.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000179
{
    public class Mps000179Processor : AbstractProcessor
    {
        Mps000179PDO rdo;

        public Mps000179Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000179PDO)rdoBase;
        }

        internal void SetBarcodeKey()
        {
            try
            {
                Inventec.Common.BarcodeLib.Barcode barcodePatientCode = new Inventec.Common.BarcodeLib.Barcode(rdo.currentPatient.PATIENT_CODE);
                barcodePatientCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcodePatientCode.IncludeLabel = false;
                barcodePatientCode.Width = 120;
                barcodePatientCode.Height = 40;
                barcodePatientCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                barcodePatientCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcodePatientCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcodePatientCode.IncludeLabel = true;

                dicImage.Add(Mps000179ExtendSingleKey.BARCODE_PATIENT_CODE_STR, barcodePatientCode);

                Inventec.Common.BarcodeLib.Barcode barcodeTreatmentCode = new Inventec.Common.BarcodeLib.Barcode(rdo.currentTreatment.TREATMENT_CODE);
                barcodeTreatmentCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcodeTreatmentCode.IncludeLabel = false;
                barcodeTreatmentCode.Width = 120;
                barcodeTreatmentCode.Height = 40;
                barcodeTreatmentCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                barcodeTreatmentCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcodeTreatmentCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcodeTreatmentCode.IncludeLabel = true;

                dicImage.Add(Mps000179ExtendSingleKey.BARCODE_TREATMENT_CODE_STR, barcodeTreatmentCode);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();


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
                if (rdo.currentPatient != null)
                {
                    SetSingleKey(new KeyValue(Mps000179ExtendSingleKey.AGE, AgeUtil.CalculateFullAge(rdo.currentPatient.DOB)));
                    SetSingleKey(new KeyValue(Mps000179ExtendSingleKey.DOB_STR, rdo.currentPatient.DOB.ToString().Substring(0, 4)));

                }
                if (rdo.currentPatyAlterBhyt != null)
                {
                    SetSingleKey(new KeyValue(Mps000179ExtendSingleKey.HEIN_CARD_FROM_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.currentPatyAlterBhyt.HEIN_CARD_FROM_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000179ExtendSingleKey.HEIN_CARD_TO_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.currentPatyAlterBhyt.HEIN_CARD_TO_TIME ?? 0)));
                }

                if (rdo.currentTreatment != null)
                {
                    SetSingleKey(new KeyValue(Mps000179ExtendSingleKey.CLINICAL_IN_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.currentTreatment.CLINICAL_IN_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000179ExtendSingleKey.ICD_CODE, rdo.currentTreatment.ICD_CODE.ToString() ?? ""));
                    SetSingleKey(new KeyValue(Mps000179ExtendSingleKey.ICD_NAME, rdo.currentTreatment.ICD_NAME != null ? rdo.currentTreatment.ICD_NAME.ToString() : ""));
                    SetSingleKey(new KeyValue(Mps000179ExtendSingleKey.ICD_SUB_CODE, rdo.currentTreatment.ICD_SUB_CODE != null ? rdo.currentTreatment.ICD_SUB_CODE.ToString() : ""));
                    //SetSingleKey(new KeyValue(Mps000179ExtendSingleKey.ICD_SUB_CODE, rdo.currentTreatment.ICD_SUB_CODE.ToString() ?? ""));
                    SetSingleKey(new KeyValue(Mps000179ExtendSingleKey.ICD_TEXT, rdo.currentTreatment.ICD_TEXT != null ? rdo.currentTreatment.ICD_TEXT.ToString() : ""));
                }
                AddObjectKeyIntoListkey<V_HIS_DEPARTMENT_TRAN>(rdo.currentDepartmentTran, false);
                AddObjectKeyIntoListkey<V_HIS_PATIENT_TYPE_ALTER>(rdo.currentPatyAlterBhyt, false);
                AddObjectKeyIntoListkey<V_HIS_TREATMENT>(rdo.currentTreatment, false);
                AddObjectKeyIntoListkey<V_HIS_PATIENT>(rdo.currentPatient, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
