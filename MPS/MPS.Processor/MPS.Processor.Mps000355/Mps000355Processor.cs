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
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPS.Processor.Mps000355.PDO;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System.Drawing;

namespace MPS.Processor.Mps000355
{
    public class Mps000355Processor: AbstractProcessor
    {
         List<HisBillGoodsADO> serviceAdos = new List<HisBillGoodsADO>();
        Mps000355PDO rdo;
        public Mps000355Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000355PDO)rdoBase;
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
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                ProcessSingleKey();
                ProcessListData();
                SetBarcodeKey();

                //ghi đè PrintLogData và UniqueCodeData
                ProcessPrintLogData();
                //lấy số lần in
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));
                if (rdo.HisBillGoods == null) rdo.HisBillGoods = new List<HIS_BILL_GOODS>();
                singleTag.ProcessData(store, singleValueDictionary);
                objectTag.AddObjectData(store, "Services1", serviceAdos);
                objectTag.AddObjectData(store, "Services2", serviceAdos);
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

        void ProcessSingleKey()
        {
            try
            {
                if (rdo.HisTransaction != null)
                {
                    AddObjectKeyIntoListkey<V_HIS_TRANSACTION>(rdo.HisTransaction, false);
                    string amountStr = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(rdo.HisTransaction.AMOUNT));
                    SetSingleKey(new KeyValue(Mps000355ExtendSingleKey.AMOUNT_TEXT_UPPER_FIRST, Inventec.Common.String.Convert.CurrencyToVneseString(amountStr)));
                    decimal amountAfterExem = rdo.HisTransaction.AMOUNT - (rdo.HisTransaction.EXEMPTION ?? 0);
                    SetSingleKey(new KeyValue(Mps000355ExtendSingleKey.AMOUNT_AFTER_EXEMPTION, Inventec.Common.Number.Convert.NumberToNumberRoundMax4(amountAfterExem)));
                    string amountAfterExemStr = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(amountAfterExem));
                    string amountAfterExemText = Inventec.Common.String.Convert.CurrencyToVneseStringNoUpcase(amountAfterExemStr);
                    SetSingleKey(new KeyValue(Mps000355ExtendSingleKey.AMOUNT_AFTER_EXEMPTION_TEXT, amountAfterExemText));
                    SetSingleKey(new KeyValue(Mps000355ExtendSingleKey.AMOUNT_AFTER_EXEMPTION_TEXT_UPPER_FIRST, Inventec.Common.String.Convert.UppercaseFirst(amountAfterExemText)));

                    string amountAwayZeroStr = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(Math.Round(rdo.HisTransaction.AMOUNT, 0, MidpointRounding.AwayFromZero)));
                    SetSingleKey(new KeyValue(Mps000355ExtendSingleKey.AMOUNT_AWAY_ZERO_TEXT_UPPER_FIRST, Inventec.Common.String.Convert.CurrencyToVneseString(amountAwayZeroStr)));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void ProcessListData()
        {
            try
            {
                if (rdo.HisBillGoods != null && rdo.HisBillGoods.Count > 0)
                {
                    foreach (var item in rdo.HisBillGoods)
                    {
                        HisBillGoodsADO ado = new HisBillGoodsADO(item);
                        this.serviceAdos.Add(ado);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SetBarcodeKey()
        {
            try
            {
                if (!String.IsNullOrEmpty(rdo.HisTransaction.TRANSACTION_CODE))
                {
                    Inventec.Common.BarcodeLib.Barcode barcodeTransactionCode = new Inventec.Common.BarcodeLib.Barcode(rdo.HisTransaction.TRANSACTION_CODE);
                    barcodeTransactionCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                    barcodeTransactionCode.IncludeLabel = false;
                    barcodeTransactionCode.Width = 120;
                    barcodeTransactionCode.Height = 40;
                    barcodeTransactionCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                    barcodeTransactionCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                    barcodeTransactionCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                    barcodeTransactionCode.IncludeLabel = true;

                    dicImage.Add(Mps000355ExtendSingleKey.TRANSACTION_CODE_BAR, barcodeTransactionCode);
                }
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
                log = LogDataTransaction(rdo.HisTransaction.TREATMENT_CODE, rdo.HisTransaction.TRANSACTION_CODE, "");
                log += "SoTien: " + rdo.HisTransaction.AMOUNT;
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
                if (rdo != null && rdo.HisTransaction != null)
                    result = String.Format("{0}_{1}_{2}", rdo.HisTransaction.TREATMENT_CODE, rdo.HisTransaction.TRANSACTION_CODE, rdo.HisTransaction.ACCOUNT_BOOK_CODE);
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
