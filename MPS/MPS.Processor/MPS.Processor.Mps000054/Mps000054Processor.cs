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
using MPS.Processor.Mps000054.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace MPS.Processor.Mps000054
{
    class Mps000054Processor : AbstractProcessor
    {
        Mps000054PDO rdo;
        public Mps000054Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000054PDO)rdoBase;
        }

        internal void SetBarcodeKey()
        {
            try
            {
                Inventec.Common.BarcodeLib.Barcode barcodeTreatmentCode = new Inventec.Common.BarcodeLib.Barcode(rdo.serviceReq.TREATMENT_CODE);
                barcodeTreatmentCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcodeTreatmentCode.IncludeLabel = false;
                barcodeTreatmentCode.Width = 120;
                barcodeTreatmentCode.Height = 40;
                barcodeTreatmentCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                barcodeTreatmentCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcodeTreatmentCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcodeTreatmentCode.IncludeLabel = true;

                dicImage.Add(Mps000054ExtendSingleKey.TREATMENT_CODE_BAR, barcodeTreatmentCode);

                //Inventec.Common.BarcodeLib.Barcode barcodeTreatment = new Inventec.Common.BarcodeLib.Barcode(rdo.currentHisTreatment.TREATMENT_CODE);
                //barcodeTreatment.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                //barcodeTreatment.IncludeLabel = false;
                //barcodeTreatment.Width = 120;
                //barcodeTreatment.Height = 40;
                //barcodeTreatment.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                //barcodeTreatment.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                //barcodeTreatment.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                //barcodeTreatment.IncludeLabel = true;

                //dicImage.Add(Mps000054ExtendSingleKey.BARCODE_TREATMENT_CODE_STR, barcodeTreatment);
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
                objectTag.AddObjectData(store, "ServiceMedicines", rdo.expMestMedicines);

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
                if (!string.IsNullOrEmpty(rdo.medi_stock_name))
                {
                    SetSingleKey(new KeyValue(Mps000054ExtendSingleKey.MEDI_STOCK_NAME, rdo.medi_stock_name));
                }
                if (!string.IsNullOrEmpty(rdo.serviceReqCode))
                {
                    SetSingleKey(new KeyValue(Mps000054ExtendSingleKey.SERVICE_REQ_TH, rdo.serviceReqCode));
                }
                if (rdo.expMestMedicines != null)
                {
                    var aa = rdo.expMestMedicines.FirstOrDefault();
                    SetSingleKey(new KeyValue(Mps000054ExtendSingleKey.EXP_MEST_CODE, aa.EXP_MEST_CODE));
                    SetSingleKey(new KeyValue(Mps000054ExtendSingleKey.USE_DATE_TO, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.expMestMedicines.Max(o => o.USE_TIME_TO ?? 0))));
                }
                else
                {
                    SetSingleKey(new KeyValue(Mps000054ExtendSingleKey.EXP_MEST_CODE, ""));
                }
                if (rdo.serviceReq != null)
                {
                    SetSingleKey(new KeyValue(Mps000054ExtendSingleKey.USE_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.serviceReq.USE_TIME ?? 0)));

                }
                if (rdo.dhsts != null)
                {
                    AddObjectKeyIntoListkey<HIS_DHST>(rdo.dhsts, false);
                }
                AddObjectKeyIntoListkey<V_HIS_PRESCRIPTION>(rdo.serviceReq, false);
                AddObjectKeyIntoListkey<TreatmentADO>(rdo.currentHisTreatment, false);
                AddObjectKeyIntoListkey<PatyAlterBhytADO>(rdo.PatyAlterBhyt);
                AddObjectKeyIntoListkey<PatientADO>(rdo.Patient);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
