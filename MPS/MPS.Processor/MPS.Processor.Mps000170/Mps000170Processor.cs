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
using MPS.Processor.Mps000170.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000170
{
    class Mps000170Processor : AbstractProcessor
    {
        Mps000170PDO rdo;
        List<Mps000170ADO> _ListAdo = null;

        public Mps000170Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000170PDO)rdoBase;
            _ListAdo = new List<Mps000170ADO>();
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                SetSingleKey();
                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                objectTag.AddObjectData(store, "ListMediMate", _ListAdo);
                objectTag.AddObjectData(store, "ListImpMestUser", rdo._ListIpmMestUser);
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void SetSingleKey()
        {
            try
            {
                decimal sumPrice = 0;
                if (rdo._ImpMestMedicines != null && rdo._ImpMestMedicines.Count > 0)
                {
                    // sắp xếp theo thứ tự tăng dần của id
                    rdo._ImpMestMedicines = rdo._ImpMestMedicines.OrderBy(p => p.ID).ToList();
                    foreach (var item in rdo._ImpMestMedicines)
                    {
                        _ListAdo.Add(new Mps000170ADO(item));
                        if (!item.PRICE.HasValue)
                            continue;
                        sumPrice += item.AMOUNT * item.PRICE.Value * (1 + (item.IMP_VAT_RATIO));
                    }
                }
                if (rdo._ImpMestMaterials != null && rdo._ImpMestMaterials.Count > 0)
                {
                    // sắp xếp theo thứ tự tăng dần của id
                    rdo._ImpMestMaterials = rdo._ImpMestMaterials.OrderBy(p => p.ID).ToList();
                    foreach (var item in rdo._ImpMestMaterials)
                    {
                        _ListAdo.Add(new Mps000170ADO(item));
                        if (!item.PRICE.HasValue)
                            continue;
                        sumPrice += item.AMOUNT * item.PRICE.Value * (1 + (item.IMP_VAT_RATIO));
                    }
                }

                SetSingleKey(new KeyValue(Mps000170ExtendSingleKey.SUM_PRICE, Inventec.Common.Number.Convert.NumberToString(sumPrice)));
                string sumPriceString = String.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(sumPrice));
                SetSingleKey(new KeyValue(Mps000170ExtendSingleKey.SUM_PRICE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(sumPriceString)));

                if (rdo._ImpMest != null)
                {
                    SetSingleKey(new KeyValue(Mps000170ExtendSingleKey.IMP_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo._ImpMest.IMP_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000170ExtendSingleKey.CREATE_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo._ImpMest.CREATE_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000170ExtendSingleKey.CREATE_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo._ImpMest.CREATE_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000170ExtendSingleKey.CREATE_DATE_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(rdo._ImpMest.CREATE_TIME ?? 0)));
                    AddObjectKeyIntoListkey<V_HIS_IMP_MEST>(rdo._ImpMest, false);
                }
                if (rdo._ImpMest != null && rdo._ImpMest.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DK)
                {
                    SetSingleKey(new KeyValue(Mps000170ExtendSingleKey.Titles, "ĐẦU KỲ"));
                }
                else if (rdo._ImpMest != null && rdo._ImpMest.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__KK)
                {
                    SetSingleKey(new KeyValue(Mps000170ExtendSingleKey.Titles, "KIỂM KÊ"));
                }
                else if (rdo._ImpMest != null && rdo._ImpMest.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__KHAC)
                {
                    SetSingleKey(new KeyValue(Mps000170ExtendSingleKey.Titles, "KHÁC"));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
