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
using MPS.Processor.Mps000444.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000444
{
    public class Mps000444Processor : AbstractProcessor
    {
        Mps000444PDO rdo;

        public Mps000444Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000444PDO)rdoBase;
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));

                SetBarcodeKey();
                SetSingleKey();
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
        public void SetBarcodeKey()
        {
            try
            {
                if (rdo.VaccineExam != null && !String.IsNullOrEmpty(rdo.VaccineExam.VACCINATION_EXAM_CODE))
                {
                    Inventec.Common.BarcodeLib.Barcode examCodeBar = new Inventec.Common.BarcodeLib.Barcode(rdo.VaccineExam.VACCINATION_EXAM_CODE);
                    examCodeBar.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                    examCodeBar.IncludeLabel = false;
                    examCodeBar.Width = 120;
                    examCodeBar.Height = 40;
                    examCodeBar.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                    examCodeBar.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                    examCodeBar.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                    examCodeBar.IncludeLabel = true;

                    dicImage.Add("EXAM_CODE_BAR", examCodeBar);
                }
                else if (rdo.VitaminA != null && !String.IsNullOrEmpty(rdo.VitaminA.VITAMIN_A_CODE))
                {
                    Inventec.Common.BarcodeLib.Barcode examCodeBar = new Inventec.Common.BarcodeLib.Barcode(rdo.VitaminA.VITAMIN_A_CODE);
                    examCodeBar.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                    examCodeBar.IncludeLabel = false;
                    examCodeBar.Width = 120;
                    examCodeBar.Height = 40;
                    examCodeBar.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                    examCodeBar.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                    examCodeBar.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                    examCodeBar.IncludeLabel = true;

                    dicImage.Add("EXAM_CODE_BAR", examCodeBar);
                }

                if (rdo.Patient != null && !String.IsNullOrWhiteSpace(rdo.Patient.PATIENT_CODE))
                {
                    Inventec.Common.BarcodeLib.Barcode patientCodeBar = new Inventec.Common.BarcodeLib.Barcode(rdo.Patient.PATIENT_CODE);
                    patientCodeBar.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                    patientCodeBar.IncludeLabel = false;
                    patientCodeBar.Width = 120;
                    patientCodeBar.Height = 40;
                    patientCodeBar.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                    patientCodeBar.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                    patientCodeBar.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                    patientCodeBar.IncludeLabel = true;

                    dicImage.Add("PATIENT_CODE_BAR", patientCodeBar);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetSingleKey()
        {
            try
            {
                //set NUM_ORDER trước vì trong V_HIS_PATIENT có NUM_ORDER
                //chỉ set  NUM_ORDER không set các thông tin khác để tránh ghi đè giá trị key khác
                if (rdo.VaccineExam != null)
                {
                    SetSingleKey(new KeyValue("NUM_ORDER", rdo.VaccineExam.NUM_ORDER));
                }
                else if (rdo.VitaminA != null)
                {
                    SetSingleKey(new KeyValue("NUM_ORDER", rdo.VitaminA.NUM_ORDER));
                }

                AddObjectKeyIntoListkey(rdo.Patient, false);
                if (rdo.VaccineExam != null)
                {
                    AddObjectKeyIntoListkey(rdo.VaccineExam, false);
                    SetSingleKey(new KeyValue("EXAM_CODE", rdo.VaccineExam.VACCINATION_EXAM_CODE));
                }
                else if (rdo.VitaminA != null)
                {
                    AddObjectKeyIntoListkey(rdo.VitaminA, false);
                    SetSingleKey(new KeyValue("EXAM_CODE", rdo.VitaminA.VITAMIN_A_CODE));
                }

                AddObjectKeyIntoListkey(rdo.Dhst, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
