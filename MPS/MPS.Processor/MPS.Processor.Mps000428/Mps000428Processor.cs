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
using MPS.Processor.Mps000428.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000428
{
    public class Mps000428Processor : AbstractProcessor
    {
        Mps000428PDO rdo;
        public Mps000428Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000428PDO)rdoBase;
        }

        public void SetBarcodeKey()
        {
            try
            {
                Inventec.Common.BarcodeLib.Barcode barcodeBlock = new Inventec.Common.BarcodeLib.Barcode(rdo.HisServiceReq.BLOCK);
                barcodeBlock.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcodeBlock.IncludeLabel = false;
                barcodeBlock.Width = 120;
                barcodeBlock.Height = 40;
                barcodeBlock.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                barcodeBlock.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcodeBlock.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcodeBlock.IncludeLabel = true;

                dicImage.Add(Mps000428ExtendSingleKey.BLOCK_BAR, barcodeBlock);
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

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                this.SetSingleKey();
                this.SetBarcodeKey();
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

         public void SetSingleKey()
         {
             try
             {
                 if (rdo.HisServiceReq != null)
                 {
                     SetSingleKey(new KeyValue(Mps000428ExtendSingleKey.AGE, AgeUtil.CalculateFullAge(rdo.HisServiceReq.TDL_PATIENT_DOB)));
                 }

                 AddObjectKeyIntoListkey<V_HIS_SERVICE_REQ>(rdo.HisServiceReq, false);
             }
             catch (Exception ex)
             {
                 Inventec.Common.Logging.LogSystem.Error(ex);
             }
         }
    }
}
