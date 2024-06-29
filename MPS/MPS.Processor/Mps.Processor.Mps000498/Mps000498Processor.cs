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
using LIS.EFMODEL.DataModels;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000498.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000498
{
    public class Mps000498Processor : AbstractProcessor
    {
        Mps000498PDO rdo;

        public Mps000498Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000498PDO)rdoBase;
        }

        private byte[] ProcessBarcode(string data)
        {
            byte[] result = null;
            try
            {
                Inventec.Common.BarcodeLib.Barcode barcodeTestServiceReq = new Inventec.Common.BarcodeLib.Barcode(data);
                barcodeTestServiceReq.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcodeTestServiceReq.IncludeLabel = false;
                barcodeTestServiceReq.Width = 220;
                barcodeTestServiceReq.Height = 40;
                barcodeTestServiceReq.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                barcodeTestServiceReq.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcodeTestServiceReq.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcodeTestServiceReq.IncludeLabel = true;
                result = Inventec.Common.FlexCellExport.Common.ConverterImageToArray(barcodeTestServiceReq.Encode(barcodeTestServiceReq.EncodedType, barcodeTestServiceReq.RawData));
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
                if (rdo.TransReq != null && rdo.ConfigList != null && rdo.ConfigList.Count > 0)
                {
                    var data = HIS.Desktop.Common.BankQrCode.QrCodeProcessor.CreateQrImage(rdo.TransReq, rdo.ConfigList);
                    SetSingleKey(new KeyValue("IMG_QR", data.FirstOrDefault().Value));
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
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));

                this.SetQRCodeKey();
                if (rdo.TransReq != null)
                {
                    AddObjectKeyIntoListkey<HIS_TRANS_REQ>(rdo.TransReq, false);
                }
                if (rdo.TreatmentView != null)
                {
                    AddObjectKeyIntoListkey<V_HIS_TREATMENT>(rdo.TreatmentView, false);
                }
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
