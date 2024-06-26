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
using MPS.Processor.Mps000077.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000077
{
    public class Mps000077Processor : AbstractProcessor
    {
        Mps000077PDO rdo;

        List<SampleServiceADO> ListSampleServicevADOs = new List<SampleServiceADO>();
        public Mps000077Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000077PDO)rdoBase;
        }

        public void SetBarcodeKey()
        {
            try
            {
                Inventec.Common.BarcodeLib.Barcode barcodePatientCode = new Inventec.Common.BarcodeLib.Barcode(rdo.CurrentBarCode.BARCODE);
                barcodePatientCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcodePatientCode.IncludeLabel = false;
                barcodePatientCode.Width = 120;
                barcodePatientCode.Height = 40;
                barcodePatientCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                barcodePatientCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcodePatientCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcodePatientCode.IncludeLabel = true;

                dicImage.Add(Mps000077ExtendSingleKey.BARCODE_CODE_BAR, barcodePatientCode);

                Inventec.Common.BarcodeLib.Barcode barcodeTreatment = new Inventec.Common.BarcodeLib.Barcode(rdo.HisServiceReq.TDL_TREATMENT_CODE);
                barcodeTreatment.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcodeTreatment.IncludeLabel = false;
                barcodeTreatment.Width = 120;
                barcodeTreatment.Height = 40;
                barcodeTreatment.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                barcodeTreatment.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcodeTreatment.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcodeTreatment.IncludeLabel = true;

                dicImage.Add(Mps000077ExtendSingleKey.BARCODE_TREATMENT_CODE_STR, barcodeTreatment);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
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
                if(rdo.LisSampleServices !=null && rdo.LisSampleServices.Count > 0)
                {
                    foreach (var item in rdo.LisSampleServices)
                    {
                        SampleServiceADO sampleService = new SampleServiceADO(item);
                        sampleService.LADID_BAR = ProcessBarcode(item.LABORATORY_CODE + "");
                        ListSampleServicevADOs.Add(sampleService);
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
                this.SetSingleKey();
                this.SetBarcodeKey();
                this.SetQRCodeKey();
                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);
                if (ListSampleServicevADOs == null)
                    ListSampleServicevADOs = new List<SampleServiceADO>();
                objectTag.AddObjectData(store, "SampleService", ListSampleServicevADOs);
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        public void SetSingleKey()
        {
            try
            {
                if (rdo.HisServiceReq != null)
                {
                    SetSingleKey(new KeyValue(Mps000077ExtendSingleKey.AGE, AgeUtil.CalculateFullAge(rdo.HisServiceReq.TDL_PATIENT_DOB)));
                }

                AddObjectKeyIntoListkey<V_LIS_SAMPLE>(rdo.CurrentBarCode, false);
                AddObjectKeyIntoListkey<V_HIS_SERVICE_REQ>(rdo.HisServiceReq, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
