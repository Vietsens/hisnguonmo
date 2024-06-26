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
using MPS.Processor.Mps000494.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000494
{
    public class Mps000494Processor : AbstractProcessor
    {
        Mps000494PDO rdo;
        public Mps000494Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000494PDO)rdoBase;
        }
        private byte[] ProcessBarcode(string data)
        {
            byte[] result = null;
            try
            {
                Inventec.Common.BarcodeLib.Barcode barcodeSerialNumber = new Inventec.Common.BarcodeLib.Barcode(data);
                barcodeSerialNumber.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcodeSerialNumber.IncludeLabel = false;
                barcodeSerialNumber.Width = 1000;
                barcodeSerialNumber.Height = 50;
                barcodeSerialNumber.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                barcodeSerialNumber.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcodeSerialNumber.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcodeSerialNumber.IncludeLabel = true;
                result = Inventec.Common.FlexCellExport.Common.ConverterImageToArray(barcodeSerialNumber.Encode(barcodeSerialNumber.EncodedType, barcodeSerialNumber.RawData));
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
        private void SetQRCodeKey()
        {
            try
            {
                if(rdo.ListSerial !=null && rdo.ListSerial.Count > 0)
                {
                    foreach (var item in rdo.ListSerial)
                    {
                        item.SERIAL_NUMBER_BAR = ProcessBarcode(item.SERIAL_NUMBER + "");
                    }
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
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                this.SetQRCodeKey();
                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);
                if (rdo.ListSerial == null)
                    rdo.ListSerial = new List<SerialADO>();
                objectTag.AddObjectData(store, "ListSerial", rdo.ListSerial);
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
