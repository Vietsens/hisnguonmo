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
using MPS.Processor.Mps000496.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000496
{
    public class Mps000496Processor : AbstractProcessor
    {
        Mps000496PDO rdo;

        List<SampleServiceADO> ListSampleServicevADOs = new List<SampleServiceADO>();
        public Mps000496Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000496PDO)rdoBase;
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
                if (rdo.CurrentBarCode != null && rdo.CurrentBarCode.Count > 0)
                {
                    foreach (var item in rdo.CurrentBarCode)
                    {
                        SampleServiceADO sampleService = new SampleServiceADO(item);
                        sampleService.BARCODE_BAR = ProcessBarcode(item.BARCODE + "");
                        sampleService.AGE = AgeUtil.CalculateFullAge(sampleService.DOB ?? 0);
                        ListSampleServicevADOs.Add(sampleService);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        //<#xxx.REQUEST_DEPARTMENT_NAME;> : Tên khoa yêu cầu
        //<#xxx.TDL_PATIENT_NAME;> : tên bệnh nhân
        //<#xxx.TREATMENT_CODE;> : mã điều trị
        //<#xxx.SERVICE_REQ_CODE;> : mã y lệnh
        //<#xxx.PATIENT_CODE;> : mã bệnh nhân
        //<#xxx.BARCODE_BAR;>: Ảnh barcode
        //<#xxx.AGE;>: Tuổi


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
                if (ListSampleServicevADOs == null)
                    ListSampleServicevADOs = new List<SampleServiceADO>();

                List<SampleServiceADO> listData0 = new List<SampleServiceADO>();
                List<SampleServiceADO> listData1 = new List<SampleServiceADO>();
                List<SampleServiceADO> listData2 = new List<SampleServiceADO>();
                List<SampleServiceADO> listData3 = new List<SampleServiceADO>();

                for (int i = 0; i < ListSampleServicevADOs.Count; i++)
                {
                    int d = i % 4;
                    switch (d)
                    {
                        case 0:
                            listData0.Add(ListSampleServicevADOs[i]);
                            break;
                        case 1:
                            listData1.Add(ListSampleServicevADOs[i]);
                            break;
                        case 2:
                            listData2.Add(ListSampleServicevADOs[i]);
                            break;
                        case 3:
                            listData3.Add(ListSampleServicevADOs[i]);
                            break;
                        default:
                            break;
                    }
                }

                //luôn tạo danh sách để không lỗi mẫu
                objectTag.AddObjectData(store, "listData0", listData0);
                objectTag.AddObjectData(store, "listData1", listData1);
                objectTag.AddObjectData(store, "listData2", listData2);
                objectTag.AddObjectData(store, "listData3", listData3);

                objectTag.AddObjectData(store, "Sample", ListSampleServicevADOs);
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
