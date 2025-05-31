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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.Library.ElectronicBill.Config;
using HIS.Desktop.Plugins.Library.ElectronicBill.Data;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.ElectronicBill.Template
{
    class Template4 : IRunTemplate
    {
        private Base.ElectronicBillDataInput DataInput;

        public Template4(Base.ElectronicBillDataInput dataInput)
        {
            // TODO: Complete member initialization
            this.DataInput = dataInput;
        }

        public object Run()
        {
            List<ProductBase> result = new List<ProductBase>();
            try
            {
                if (DataInput.SereServBill != null && DataInput.SereServBill.Count > 0)
                {
                    var groupPrice = DataInput.SereServBill.GroupBy(o => new { o.TDL_SERVICE_ID, o.TDL_REAL_PRICE }).ToList();

                    List<V_HIS_SERVICE> services = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => DataInput.SereServBill.Exists(e => e.TDL_SERVICE_ID == o.ID)).ToList();
                    foreach (var item in groupPrice)
                    {
                        V_HIS_SERVICE service = services != null ? services.FirstOrDefault(o => o.ID == item.First().TDL_SERVICE_ID) : null;
                        ProductBase product = new ProductBase();
                        product.ProdName = item.First().TDL_SERVICE_NAME;
                        product.ProdCode = item.First().TDL_SERVICE_CODE;
                        //product.ProdPrice = Inventec.Common.Number.Convert.NumberToNumberRoundMax4(item.First().TDL_REAL_PRICE ?? 0);
                        product.ProdQuantity = item.Sum(s => s.TDL_AMOUNT ?? 0);
                        product.Amount = Inventec.Common.Number.Convert.NumberToNumberRoundMax4(item.Sum(s => s.PRICE));
                        product.ProdPrice = Inventec.Common.Number.Convert.NumberToNumberRoundMax4(product.Amount / (product.ProdQuantity ?? 1));
                        product.ProdUnit = service != null ? service.SERVICE_UNIT_NAME : "";
                        product.TaxRateID = (int)(service != null ? (service.TAX_RATE_TYPE ?? Base.ProviderType.tax_KCT) : Base.ProviderType.tax_KCT);
                        product.Type = item.First().TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC ? 1 : 0;
                        if ((product.ProdPrice ?? 0) * (product.ProdQuantity ?? 0) != product.Amount)
                        {
                            product.ProdPrice = Math.Round(product.Amount / (product.ProdQuantity ?? 1), 2);
                        }

                        result.Add(product);
                    }
                }

                if (result.Count > 0)
                {
                    foreach (var product in result)
                    {
                        if (HisConfigCFG.RoundTransactionAmountOption == "1")
                        {
                            product.Amount = Math.Round(product.Amount, 0, MidpointRounding.AwayFromZero);
                            product.ProdPrice = Math.Round(product.ProdPrice ?? 0, 0, MidpointRounding.AwayFromZero);
                        }
                        else if (HisConfigCFG.RoundTransactionAmountOption == "2")
                        {
                            product.Amount = Math.Round(product.Amount, 0, MidpointRounding.AwayFromZero);
                        }

                        if (HisConfigCFG.IsHidePrice)
                        {
                            product.ProdPrice = null;
                        }

                        if (HisConfigCFG.IsHideQuantity)
                        {
                            product.ProdQuantity = null;
                        }

                        if (HisConfigCFG.IsHideUnitName)
                        {
                            product.ProdUnit = "";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
