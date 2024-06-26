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
using MPS.Processor.Mps000431.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000431
{
    public class Mps000431Processor : AbstractProcessor
    {
        Mps000431PDO rdo;

        public Mps000431Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000431PDO)rdoBase;
        }

        public override bool ProcessData()
        {
            bool result = true;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();

                SetBarcodeKey();

                SetSingleKey();

                //ghi đè PrintLogData và UniqueCodeData
                ProcessPrintLogData();
                //lấy số lần in
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);

                objectTag.AddObjectData(store, "ProductADO", rdo.lstProductADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void SetBarcodeKey()
        {
            try
            {
                if (rdo.HisTransaction != null)
                {
                    if (!String.IsNullOrEmpty(rdo.HisTransaction.TRANSACTION_CODE))
                    {
                        Inventec.Common.BarcodeLib.Barcode barcodeTransaction = new Inventec.Common.BarcodeLib.Barcode(rdo.HisTransaction.TRANSACTION_CODE);
                        barcodeTransaction.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                        barcodeTransaction.IncludeLabel = false;
                        barcodeTransaction.Width = 120;
                        barcodeTransaction.Height = 40;
                        barcodeTransaction.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                        barcodeTransaction.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                        barcodeTransaction.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                        barcodeTransaction.IncludeLabel = true;

                        dicImage.Add(Mps000431ExtendSingleKey.BARCODE_TRANSACTION_CODE, barcodeTransaction);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void SetSingleKey()
        {
            try
            {
                if (rdo.lstProductADO != null && rdo.lstProductADO.Count > 0)
                {
                    decimal Amount = 0;
                    foreach (var item in rdo.lstProductADO)
                    {
                        Amount += item.Amount;
                    }

                    string amountStr = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(Amount));
                    string amountText = Inventec.Common.String.Convert.CurrencyToVneseStringNoUpcase(amountStr);
                    SetSingleKey(new KeyValue(Mps000431ExtendSingleKey.AMOUNT, Amount));
                    SetSingleKey(new KeyValue(Mps000431ExtendSingleKey.AMOUNT_TEXT, amountText));
                    SetSingleKey(new KeyValue(Mps000431ExtendSingleKey.AMOUNT_TEXT_UPPER_FIRST, Inventec.Common.String.Convert.UppercaseFirst(amountText)));
                }

                AddObjectKeyIntoListkey<V_HIS_TRANSACTION>(rdo.HisTransaction, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        public override string ProcessPrintLogData()
        {
            string log = "";
            try
            {
                log = "Mã điều trị: " + rdo.HisTransaction.TREATMENT_CODE;
                log += " , Mã giao dịch: " + rdo.HisTransaction.TRANSACTION_CODE;
                log += "";
            }
            catch (Exception ex)
            {
                log = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return log;
        }

        public override string ProcessUniqueCodeData()
        {
            string result = "";
            try
            {
                string treatmentCode = "TREATMENT_CODE:" + rdo.HisTransaction.TREATMENT_CODE;
                string serviceReqCode = "TRANSACTION_CODE:" + rdo.HisTransaction.TRANSACTION_CODE;

                if (rdo != null && rdo.HisTransaction != null)
                    result = String.Format("{0}_{1}_{2}", this.printTypeCode, rdo.HisTransaction.TREATMENT_CODE, rdo.HisTransaction.TRANSACTION_CODE);
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
