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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.Library.ElectronicBill.Base;
using HIS.Desktop.Plugins.Library.ElectronicBill.Config;
using HIS.Desktop.Plugins.Library.ElectronicBill.Data;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.ElectronicBill.Template
{
    class Template9 : IRunTemplate
    {
        private Base.ElectronicBillDataInput DataInput;

        public Template9(Base.ElectronicBillDataInput dataInput)
        {
            // TODO: Complete member initialization
            this.DataInput = dataInput;
        }

        public object Run()
        {
            List<ProductBase> result = new List<ProductBase>();
            if (DataInput.SereServBill != null && DataInput.SereServBill.Count > 0)
            {
                var groupType = DataInput.SereServBill.GroupBy(o => o.TDL_PATIENT_TYPE_ID ?? 0).ToList();
                decimal totalPhuThu = 0;
                decimal totalBhyt = 0;

                List<ProductBase> typeProduct = new List<ProductBase>();

                foreach (var detail in groupType)
                {
                    if (detail.Key == HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT)
                    {
                        totalBhyt = detail.Sum(o => o.TDL_TOTAL_PATIENT_PRICE_BHYT ?? 0);
                        totalPhuThu += detail.Sum(o => o.PRICE - (o.TDL_TOTAL_PATIENT_PRICE_BHYT ?? 0));
                    }
                    else
                    {
                        string name = "Tiền đối tượng khác";
                        var patientType = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == detail.Key);
                        if (patientType != null)
                        {
                            name = "Tiền " + patientType.PATIENT_TYPE_NAME;
                        }

                        decimal amount = detail.Sum(o => o.PRICE);
                        decimal checkAmout = detail.Sum(o => (o.TDL_AMOUNT ?? 0) * (o.TDL_ORIGINAL_PRICE ?? 0) * (1 + (o.TDL_VAT_RATIO ?? 0)) - (o.TDL_DISCOUNT ?? 0));

                        if (checkAmout > 0 && amount > checkAmout)
                        {
                            totalPhuThu += amount - checkAmout;
                            amount = checkAmout;
                        }

                        ProductBase product = new ProductBase();
                        product.ProdName = name;
                        product.Amount = Inventec.Common.Number.Convert.NumberToNumberRoundMax4(amount);
                        product.ProdUnit = " ";
                        product.ProdCode = General.GetFirstWord(product.ProdName);
                        typeProduct.Add(product);
                    }
                }

                if (totalBhyt > 0)
                {
                    if (DataInput.LastPatientTypeAlter == null)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("Khong tim thay doi tuong benh nhan hien tai!");
                        throw new Exception("Không tìm thấy đối tượng bệnh nhân hiện tại");
                    }

                    decimal ratio = (new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(DataInput.LastPatientTypeAlter.HEIN_TREATMENT_TYPE_CODE, DataInput.LastPatientTypeAlter.HEIN_CARD_NUMBER, DataInput.LastPatientTypeAlter.LEVEL_CODE, DataInput.LastPatientTypeAlter.RIGHT_ROUTE_CODE) ?? 0) * 100;

                    ProductBase product = new ProductBase();
                    product.ProdName = String.Format("Đồng chi trả bảo hiểm {0}%", Math.Round(100 - ratio, 0, MidpointRounding.AwayFromZero)); ;
                    product.Amount = Inventec.Common.Number.Convert.NumberToNumberRoundMax4(totalBhyt);
                    product.ProdUnit = " ";
                    product.ProdCode = General.GetFirstWord(product.ProdName);
                    result.Add(product);
                }

                if (typeProduct != null && typeProduct.Count > 0)
                {
                    typeProduct = typeProduct.OrderBy(o => o.ProdName).ToList();
                    foreach (var item in typeProduct)
                    {
                        result.Add(item);
                    }
                }

                if (totalPhuThu > 0)
                {
                    ProductBase product = new ProductBase();
                    product.ProdName = "Tiền phụ thu";
                    product.Amount = Inventec.Common.Number.Convert.NumberToNumberRoundMax4(totalPhuThu);
                    product.ProdUnit = " ";
                    product.ProdCode = General.GetFirstWord(product.ProdName);
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

            return result;
        }
    }
}
