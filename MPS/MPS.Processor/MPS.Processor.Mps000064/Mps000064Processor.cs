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
using MPS.Processor.Mps000064.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000064
{
    public class Mps000064Processor : AbstractProcessor
    {
        Mps000064PDO rdo;

        public Mps000064Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000064PDO)rdoBase;
        }

        internal void SetBarcodeKey()
        {
            try
            {
                Inventec.Common.BarcodeLib.Barcode barcodePatientCode = new Inventec.Common.BarcodeLib.Barcode(rdo.Patient.PATIENT_CODE);
                barcodePatientCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcodePatientCode.IncludeLabel = false;
                barcodePatientCode.Width = 120;
                barcodePatientCode.Height = 40;
                barcodePatientCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                barcodePatientCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcodePatientCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcodePatientCode.IncludeLabel = true;

                dicImage.Add(Mps000064ExtendSingleKey.BARCODE_PATIENT_CODE_STR, barcodePatientCode);

                //Inventec.Common.BarcodeLib.Barcode barcodeTreatment = new Inventec.Common.BarcodeLib.Barcode(rdo.departmentTran.TREATMENT_CODE);
                //barcodeTreatment.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                //barcodeTreatment.IncludeLabel = false;
                //barcodeTreatment.Width = 120;
                //barcodeTreatment.Height = 40;
                //barcodeTreatment.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                //barcodeTreatment.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                //barcodeTreatment.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                //barcodeTreatment.IncludeLabel = true;

                //dicImage.Add(Mps000064ExtendSingleKey.BARCODE_TREATMENT_CODE_STR, barcodeTreatment);
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


                SetBarcodeKey();
                SetSingleKey();


                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                //barCodeTag.ProcessData(store, dicImage);
                objectTag.AddObjectData(store, "RehaTrains", rdo.rehaTrainPrints);

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
                foreach (var item in rdo.lsRehaTrain)
                {
                    SetSingleKey(new KeyValue(Mps000064ExtendSingleKey.Day1, item.Day1));
                    SetSingleKey(new KeyValue(Mps000064ExtendSingleKey.Day2, item.Day2));
                    SetSingleKey(new KeyValue(Mps000064ExtendSingleKey.Day3, item.Day3));
                    SetSingleKey(new KeyValue(Mps000064ExtendSingleKey.Day4, item.Day4));
                    SetSingleKey(new KeyValue(Mps000064ExtendSingleKey.Day5, item.Day5));
                    SetSingleKey(new KeyValue(Mps000064ExtendSingleKey.Day6, item.Day6));
                    SetSingleKey(new KeyValue(Mps000064ExtendSingleKey.Day7, item.Day7));
                    SetSingleKey(new KeyValue(Mps000064ExtendSingleKey.Day8, item.Day8));
                    SetSingleKey(new KeyValue(Mps000064ExtendSingleKey.Day9, item.Day9));
                    SetSingleKey(new KeyValue(Mps000064ExtendSingleKey.Day10, item.Day10));
                    SetSingleKey(new KeyValue(Mps000064ExtendSingleKey.Day11, item.Day11));
                    SetSingleKey(new KeyValue(Mps000064ExtendSingleKey.Day12, item.Day12));
                    SetSingleKey(new KeyValue(Mps000064ExtendSingleKey.Day13, item.Day13));
                    SetSingleKey(new KeyValue(Mps000064ExtendSingleKey.Day14, item.Day14));
                    SetSingleKey(new KeyValue(Mps000064ExtendSingleKey.Day15, item.Day15));
                    SetSingleKey(new KeyValue(Mps000064ExtendSingleKey.Day16, item.Day16));
                    SetSingleKey(new KeyValue(Mps000064ExtendSingleKey.Day17, item.Day17));
                    SetSingleKey(new KeyValue(Mps000064ExtendSingleKey.Day18, item.Day18));
                    SetSingleKey(new KeyValue(Mps000064ExtendSingleKey.Day19, item.Day19));
                    SetSingleKey(new KeyValue(Mps000064ExtendSingleKey.Day20, item.Day20));
                    SetSingleKey(new KeyValue(Mps000064ExtendSingleKey.Day21, item.Day21));
                    SetSingleKey(new KeyValue(Mps000064ExtendSingleKey.Day22, item.Day22));
                    SetSingleKey(new KeyValue(Mps000064ExtendSingleKey.Day23, item.Day23));
                    SetSingleKey(new KeyValue(Mps000064ExtendSingleKey.Day24, item.Day24));
                }

                if (rdo.Patient != null)
                {
                    SetSingleKey(new KeyValue(Mps000064ExtendSingleKey.AGE, AgeUtil.CalculateFullAge(rdo.Patient.DOB)));
                }

                AddObjectKeyIntoListkey<HIS_DEPARTMENT>(rdo.department, false);
                AddObjectKeyIntoListkey<V_HIS_REHA_SERVICE_REQ>(rdo.rehaService, false);
                AddObjectKeyIntoListkey<V_HIS_REHA_SUM>(rdo.hisRehaSumRow, false);
                AddObjectKeyIntoListkey<V_HIS_PATIENT>(rdo.Patient);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
