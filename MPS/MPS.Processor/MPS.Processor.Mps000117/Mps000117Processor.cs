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
using MPS.Processor.Mps000117.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000117
{
    class Mps000117Processor : AbstractProcessor
    {
        Mps000117PDO rdo;
        public Mps000117Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000117PDO)rdoBase;
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                SetBarcodeKey();
                SetSingleKeyPlus();
                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);
                List<HisAnticipateMetyADO> tempList = rdo.HisAnticipateMetyAdo;
                int numOrder = 0;
                tempList = tempList.OrderBy(bid => 
                        {
                            if (int.TryParse(bid.BID_NUM_ORDER, out numOrder))
                            {
                                return numOrder;
                            }
                            else
                            {
                                return int.MaxValue;
                            }
                        })
                        .ThenBy(bid => bid.MEDICINE_TYPE_CODE)
                        .ToList();
                objectTag.AddObjectData(store, "AnticipateMeties", rdo.HisAnticipateMetyAdo);
                objectTag.AddObjectData(store, "OrderByBid", tempList); 
                result = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
        public void SetBarcodeKey()
        {
            try
            {
                Inventec.Common.BarcodeLib.Barcode barcodeAnticipateCode = new Inventec.Common.BarcodeLib.Barcode(rdo.HisAnticipate.ANTICIPATE_CODE);
                barcodeAnticipateCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcodeAnticipateCode.IncludeLabel = false;
                barcodeAnticipateCode.Width = 120;
                barcodeAnticipateCode.Height = 40;
                barcodeAnticipateCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                barcodeAnticipateCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcodeAnticipateCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcodeAnticipateCode.IncludeLabel = true;

                dicImage.Add(Mps000117ExtendSingleKey.ANTICIPATE_CODE_BAR, barcodeAnticipateCode);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void SetSingleKeyPlus()
        {
            try
            {
                if (rdo.HisAnticipate != null)
                {
                    SetSingleKey(new KeyValue(Mps000117ExtendSingleKey.ANTICIPATE_CODE, rdo.HisAnticipate.ANTICIPATE_CODE));
                    SetSingleKey(new KeyValue(Mps000117ExtendSingleKey.USE_TIME, rdo.HisAnticipate.USE_TIME));
                    SetSingleKey(new KeyValue(Mps000117ExtendSingleKey.REQUEST_USERNAME, rdo.HisAnticipate.REQUEST_USERNAME));

                    if (rdo.HisAnticipateMetyAdo != null)
                    {
                        decimal SumPrice = rdo.HisAnticipateMetyAdo.Sum(o => o.TotalMoney);
                        string sumPriceStr = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(SumPrice));
                        string sumPriceText = Inventec.Common.String.Convert.CurrencyToVneseStringNoUpcase(sumPriceStr);
                        SetSingleKey(new KeyValue(Mps000117ExtendSingleKey.SUM_PRICE, SumPrice));
                        SetSingleKey(new KeyValue(Mps000117ExtendSingleKey.SUM_PRICE_TEXT, Inventec.Common.String.Convert.UppercaseFirst(sumPriceText)));
                    }
                }

                AddObjectKeyIntoListkey<V_HIS_ANTICIPATE>(rdo.HisAnticipate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
