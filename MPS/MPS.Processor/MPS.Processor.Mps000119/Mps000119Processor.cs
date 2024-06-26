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
using Inventec.Core;
using MPS.Processor.Mps000119.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000119
{
    public class Mps000119Processor : AbstractProcessor
    {
        Mps000119PDO rdo;
        public Mps000119Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000119PDO)rdoBase;
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

                if (store.ReadTemplate(System.IO.Path.GetFullPath(fileName)))
                {
                    ProcessSingleKey();
                    singleTag.ProcessData(store, singleValueDictionary);

                    objectTag.AddObjectData(store, "BidMetys", rdo.bidMetyAdos);

                    result = true;
                }
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
                if (rdo.hisBid != null)
                {
                    SetSingleKey(new KeyValue(Mps000119ExtendSingleKey.BID_NAME, rdo.hisBid.BID_NAME));
                    SetSingleKey(new KeyValue(Mps000119ExtendSingleKey.BID_NUMBER, rdo.hisBid.BID_NUMBER));

                    if (rdo.bidMetyAdos != null)
                    {
                        decimal SumPrice = rdo.bidMetyAdos.Sum(o => o.TotalMoney);
                        string sumPriceStr = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(SumPrice));
                        string sumPriceText = Inventec.Common.String.Convert.CurrencyToVneseStringNoUpcase(sumPriceStr);
                        SetSingleKey(new KeyValue(Mps000119ExtendSingleKey.SUM_PRICE, SumPrice));
                        SetSingleKey(new KeyValue(Mps000119ExtendSingleKey.SUM_PRICE_TEXT, Inventec.Common.String.Convert.UppercaseFirst(sumPriceText)));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        class CalculateMergerData : TFlexCelUserFunction
        {
            long typeId = 0;
            long mediMateTypeId = 0;

            public override object Evaluate(object[] parameters)
            {
                if (parameters == null || parameters.Length <= 0)
                    throw new ArgumentException("Bad parameter count in call to Orders() user-defined function");
                bool result = false;
                try
                {
                    long servicetypeId = Convert.ToInt64(parameters[0]);
                    long mediMateId = Convert.ToInt64(parameters[1]);

                    if (servicetypeId > 0 && mediMateId > 0)
                    {
                        if (this.typeId == servicetypeId && this.mediMateTypeId == mediMateId)
                        {
                            return true;
                        }
                        else
                        {
                            this.typeId = servicetypeId;
                            this.mediMateTypeId = mediMateId;
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                    result = false;
                }
                return result;
            }
        }
    }
}
