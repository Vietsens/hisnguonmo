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
using MPS.Processor.Mps000495.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000495
{
    public class Mps000495Processor : AbstractProcessor
    {
        Mps000495PDO rdo;

        public Mps000495Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000495PDO)rdoBase;
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
                barcodeSerialNumber.AlternateLabel = " ";
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
        List<ExpMestMaterialADO> materialSerial = new List<ExpMestMaterialADO>();
        private void SetQRCodeKey()
        {
            try
            {
                AddObjectKeyIntoListkey<V_HIS_SERVICE_REQ>(rdo.ServiceReqView, false);
                AddObjectKeyIntoListkey<V_HIS_SERE_SERV>(rdo.SereServView, false);
                if (rdo.ExpMestMaterialsView != null && rdo.ExpMestMaterialsView.Count > 0)
                {
                    AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MATERIAL, ExpMestMaterialADO>();
                    materialSerial = AutoMapper.Mapper.Map<List<V_HIS_EXP_MEST_MATERIAL>, List<ExpMestMaterialADO>>(rdo.ExpMestMaterialsView);
                    foreach (var item in materialSerial)
                    {
                        item.SERIAL_NUMBER_BAR = ProcessBarcode(item.SERIAL_NUMBER + "");
                    }
                }

                if (rdo.ServiceReqView != null && !String.IsNullOrEmpty(rdo.ServiceReqView.TREATMENT_CODE))
                {
                    Inventec.Common.BarcodeLib.Barcode barcodeTreatment = new Inventec.Common.BarcodeLib.Barcode(rdo.ServiceReqView.TREATMENT_CODE);
                    barcodeTreatment.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                    barcodeTreatment.IncludeLabel = false;
                    barcodeTreatment.Width = 120;
                    barcodeTreatment.Height = 40;
                    barcodeTreatment.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                    barcodeTreatment.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                    barcodeTreatment.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                    barcodeTreatment.AlternateLabel = " ";
                    barcodeTreatment.IncludeLabel = true;

                    dicImage.Add(Mps000495ExtendSinglekey.TREATMENT_CODE_BAR, barcodeTreatment);
                }

                if (materialSerial != null && materialSerial.Count > 0)
                {
                    SetSingleKey(new KeyValue(Mps000495ExtendSinglekey.MEDI_STOCK_NAME_STR, string.Join(", ", materialSerial.Select(o=>o.MEDI_STOCK_NAME))));
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

                if (materialSerial == null)
                    materialSerial = new List<ExpMestMaterialADO>();
                objectTag.AddObjectData(store, "EMaterial", materialSerial);
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
