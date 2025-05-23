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
using HIS.Desktop.Plugins.Library.ElectronicBill.Config;
using HIS.Desktop.Plugins.Library.ElectronicBill.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.ElectronicBill.Template
{
    /// <summary>
    /// Mẫu này giống mẫu Template2 nhưng khác tên.
    /// </summary>
    public class Template3 : IRunTemplate
    {
        private Base.ElectronicBillDataInput DataInput;

        public Template3(Base.ElectronicBillDataInput dataInput)
        {
            // TODO: Complete member initialization
            this.DataInput = dataInput;
        }

        public object Run()
        {
            List<Data.ProductBase> result = new List<ProductBase>();
            try
            {
                decimal amount = 0;
                if (DataInput != null)
                {
                    if (DataInput.Amount.HasValue)
                    {
                        amount = DataInput.Amount.Value;
                    }
                    else if (DataInput.SereServBill != null)
                    {
                        amount = DataInput.SereServBill.Sum(s => s.PRICE);
                    }
                }

                //làm tròn
                amount = Math.Round(amount, 0);

                ProductBase product = new ProductBase();
                product.ProdName = "THANH TOÁN VIỆN PHÍ";
                product.ProdCode = "TTVP";
                product.ProdPrice = amount;
                product.ProdQuantity = 1;
                product.Amount = amount;
                product.ProdUnit = "lần";
                product.TaxRateID = Base.ProviderType.tax_KCT;
                result.Add(product);

                if (result.Count > 0)
                {
                    foreach (var pd in result)
                    {
                        if (HisConfigCFG.RoundTransactionAmountOption == "1")
                        {
                            pd.Amount = Math.Round(pd.Amount, 0, MidpointRounding.AwayFromZero);
                            pd.ProdPrice = Math.Round(pd.ProdPrice ?? 0, 0, MidpointRounding.AwayFromZero);
                        }
                        else if (HisConfigCFG.RoundTransactionAmountOption == "2")
                        {
                            pd.Amount = Math.Round(pd.Amount, 0, MidpointRounding.AwayFromZero);
                        }

                        if (HisConfigCFG.IsHidePrice)
                        {
                            pd.ProdPrice = null;
                        }

                        if (HisConfigCFG.IsHideQuantity)
                        {
                            pd.ProdQuantity = null;
                        }

                        if (HisConfigCFG.IsHideUnitName)
                        {
                            pd.ProdUnit = "";
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
