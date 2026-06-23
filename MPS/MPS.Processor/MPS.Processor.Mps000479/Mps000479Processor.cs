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
using Inventec.Desktop.Common.LanguageManager;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000479.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000479
{
    public class Mps000479Processor : AbstractProcessor
    {
        private const string TREATMENT_CODE_BARCODE = "TREATMENT_CODE_BARCODE";

        Mps000479PDO rdo;
        public Mps000479Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000479PDO)rdoBase;
        }

        //Sinh barcode mã điều trị để in lên phiếu số thứ tự phát thuốc
        private void SetTreatmentCodeBarcode()
        {
            try
            {
                string treatmentCode = "";
                if (rdo._VExpMest != null && !String.IsNullOrEmpty(rdo._VExpMest.TDL_TREATMENT_CODE))
                {
                    treatmentCode = rdo._VExpMest.TDL_TREATMENT_CODE;
                }
                else if (rdo._ExpMest != null && !String.IsNullOrEmpty(rdo._ExpMest.TDL_TREATMENT_CODE))
                {
                    treatmentCode = rdo._ExpMest.TDL_TREATMENT_CODE;
                }

                if (!String.IsNullOrEmpty(treatmentCode))
                {
                    Inventec.Common.BarcodeLib.Barcode barcodeTreatmentCode = new Inventec.Common.BarcodeLib.Barcode(treatmentCode);
                    barcodeTreatmentCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                    barcodeTreatmentCode.IncludeLabel = false;
                    barcodeTreatmentCode.Width = 120;
                    barcodeTreatmentCode.Height = 40;
                    barcodeTreatmentCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                    barcodeTreatmentCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                    barcodeTreatmentCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                    barcodeTreatmentCode.IncludeLabel = true;

                    dicImage.Add(TREATMENT_CODE_BARCODE, barcodeTreatmentCode);
                }
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
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));

				Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rdo._ExpMest), rdo._ExpMest));
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rdo._VExpMest), rdo._VExpMest));
                if(rdo._ExpMest != null)
                    AddObjectKeyIntoListkey<HIS_EXP_MEST>(rdo._ExpMest, false);
                if(rdo._VExpMest != null)
                    AddObjectKeyIntoListkey<V_HIS_EXP_MEST>(rdo._VExpMest, false);

                SetTreatmentCodeBarcode();

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
