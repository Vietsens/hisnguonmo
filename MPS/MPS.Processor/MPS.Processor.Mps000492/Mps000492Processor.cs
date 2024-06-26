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
using MPS.Processor.Mps000492.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000492
{
    class Mps000492Processor : AbstractProcessor
    {
        Mps000492PDO rdo;
        public Mps000492Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000492PDO)rdoBase;
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));

                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));
                SetBarcodeKey();

                barCodeTag.ProcessData(store, dicImage);
                if (rdo.RationSchedule != null)
                {
                    SetSingleKey(new KeyValue(Mps000492ExtendSingleKey.YEAR_STR, rdo.TreatmentBedRoom.TDL_PATIENT_DOB.ToString().Substring(0, 4)));
                    SetSingleKey(new KeyValue(Mps000492ExtendSingleKey.TDL_PATIENT_TYPE_NAME, rdo.TreatmentBedRoom.PATIENT_TYPE_NAME));
                    SetSingleKey(new KeyValue(Mps000492ExtendSingleKey.IS_FOR_HOMIE_STR, rdo.RationSchedule.IS_FOR_HOMIE == (short?)1 ? "1" : "0"));
                    SetSingleKey(new KeyValue(Mps000492ExtendSingleKey.IS_HALF_IN_FIRST_DAY_STR, rdo.RationSchedule.IS_HALF_IN_FIRST_DAY == (short?)1 ? "1" : "0"));
                }
                AddObjectKeyIntoListkey<L_HIS_TREATMENT_BED_ROOM>(rdo.TreatmentBedRoom, true);
                AddObjectKeyIntoListkey<V_HIS_RATION_SCHEDULE>(rdo.RationSchedule, true);

                singleTag.ProcessData(store, singleValueDictionary);

                result = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
        public void SetBarcodeKey()
        {
            try
            {
                if (rdo != null)
                {
                    if (!string.IsNullOrEmpty(rdo.TreatmentBedRoom.TREATMENT_CODE))
                    {
                        Inventec.Common.BarcodeLib.Barcode barcode = new Inventec.Common.BarcodeLib.Barcode(rdo.TreatmentBedRoom.TREATMENT_CODE);
                        barcode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                        barcode.IncludeLabel = false;
                        barcode.Width = 120;
                        barcode.Height = 40;
                        barcode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                        barcode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                        barcode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                        barcode.IncludeLabel = true;

                        dicImage.Add(Mps000492ExtendSingleKey.TREATMENT_CODE_BAR, barcode);
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
