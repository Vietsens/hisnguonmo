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
    /// Tạo 1 chi tiết hóa đơn tổng hợp
    /// </summary>
    class Template11 : IRunTemplate
    {
        private Base.ElectronicBillDataInput DataInput;

        public Template11(Base.ElectronicBillDataInput dataInput)
        {
            // TODO: Complete member initialization
            this.DataInput = dataInput;
        }

        public object Run()
        {
            List<ProductBase> result = new List<ProductBase>();

            //lấy chi tiết gom theo mẫu 8
            IRunTemplate iRunTemplate = TemplateFactory.MakeIRun(TemplateEnum.TYPE.Template8, DataInput);
            var listProduct = iRunTemplate.Run();
            if (listProduct == null)
            {
                throw new Exception("Không có thông tin chi tiết dịch vụ.");
            }

            if (listProduct.GetType() == typeof(List<ProductBase>))
            {
                decimal amount = 0;
                List<string> content = new List<string>();
                List<ProductBase> listProductBase = (List<ProductBase>)listProduct;
                foreach (var item in listProductBase)
                {
                    content.Add(string.Format("{0}({1})", item.ProdName, Inventec.Common.Number.Convert.NumberToStringRoundMax4(item.Amount)));
                    amount += item.Amount;
                }

                if (content.Count > 0)
                {
                    ProductBase product = new ProductBase();

                    product.ProdName = string.Join("; ", content);
                    product.ProdCode = "TTVP";
                    product.ProdPrice = amount;
                    product.ProdQuantity = 1;
                    product.Amount = amount;
                    product.TaxRateID = Base.ProviderType.tax_KCT;

                    result.Add(product);
                }
            }

            //cấu hình làm tròn đã xử lý ở mẫu 8 nên chỉ thêm xử lý ẩn hiện thông tin đơn giá, số lượng, đvt
            if (result.Count > 0)
            {
                result = result.OrderBy(o => o.Stt).ToList();

                foreach (var product in result)
                {
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

            return result;
        }
    }
}
