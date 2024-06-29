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
    class TemplateNhaThuoc : IRunTemplate
    {
        private Base.ElectronicBillDataInput DataInput;

        public TemplateNhaThuoc(Base.ElectronicBillDataInput dataInput)
        {
            this.DataInput = dataInput;
        }

        public object Run()
        {
            List<ProductBasePlus> result = new List<ProductBasePlus>();
            try
            {
                if (DataInput.SereServs != null && DataInput.SereServs.Count > 0)
                {
                    List<V_HIS_MEDICINE_TYPE> hisMedicineType = HisConfigCFG.V_HIS_MEDICINE_TYPEs.Where(o => DataInput.SereServs.Exists(e => e.MEDICINE_ID == o.ID)).ToList();
                    List<V_HIS_MATERIAL_TYPE> hisMaterialType = HisConfigCFG.V_HIS_MATERIAL_TYPEs.Where(o => DataInput.SereServs.Exists(e => e.MATERIAL_ID == o.ID)).ToList();
                    List<V_HIS_NONE_MEDI_SERVICE> hisNoneType = HisConfigCFG.V_HIS_NONE_MEDI_SERVICEs.Where(o => DataInput.SereServs.Exists(e => e.OTHER_PAY_SOURCE_ID == o.ID)).ToList();

                    foreach (var item in DataInput.SereServs)
                    {
                        V_HIS_MEDICINE_TYPE mety = hisMedicineType != null ? hisMedicineType.FirstOrDefault(o => o.ID == item.MEDICINE_ID) : null;
                        V_HIS_MATERIAL_TYPE maty = hisMaterialType != null ? hisMaterialType.FirstOrDefault(o => o.ID == item.MATERIAL_ID) : null;
                        V_HIS_NONE_MEDI_SERVICE noty = hisNoneType != null ? hisNoneType.FirstOrDefault(o => o.ID == item.OTHER_PAY_SOURCE_ID) : null;

                        string serviceUnit = item.SERVICE_UNIT_NAME;
                        string serviceCode = item.TDL_SERVICE_CODE;

                        if (String.IsNullOrWhiteSpace(serviceUnit))
                        {
                            if (mety != null)
                            {
                                serviceUnit = mety.SERVICE_UNIT_NAME;
                            }
                            else if (maty != null)
                            {
                                serviceUnit = maty.SERVICE_UNIT_NAME;
                            }
                            else if (noty != null)
                            {
                                serviceUnit = noty.SERVICE_UNIT_NAME;
                            }
                        }

                        if (String.IsNullOrWhiteSpace(serviceCode))
                        {
                            if (mety != null)
                            {
                                serviceCode = mety.MEDICINE_TYPE_CODE;
                            }
                            else if (maty != null)
                            {
                                serviceCode = maty.MATERIAL_TYPE_CODE;
                            }
                            else if (noty != null)
                            {
                                serviceCode = noty.NONE_MEDI_SERVICE_CODE;
                            }
                        }

                        if (HisConfigCFG.VatOption == "2")
                        {
                            item.PRICE = item.PRICE * (1 + item.VAT_RATIO);
                            item.VAT_RATIO = 0;
                        }

                        ProductBasePlus product = new ProductBasePlus();
                        product.ProdName = item.TDL_SERVICE_NAME;
                        product.ProdUnit = serviceUnit;
                        product.ProdCode = serviceCode;
                        product.ProdQuantity = item.AMOUNT;

                        product.TaxConvert = item.VAT_RATIO * 100;
                        product.ProdPrice = Math.Round(item.PRICE, 4);
                        product.TaxAmount = Math.Round(item.PRICE * item.AMOUNT * item.VAT_RATIO, 4);

                        if (HisConfigCFG.dicVatConvert != null && HisConfigCFG.dicVatConvert.Count > 0)
                        {
                            decimal vat = Math.Round(item.VAT_RATIO * 100, 0);
                            if (HisConfigCFG.dicVatConvert.ContainsKey(vat))
                            {
                                product.TaxConvert = HisConfigCFG.dicVatConvert[vat];
                                product.ProdPrice = Math.Round(item.PRICE * (1 + item.VAT_RATIO) / (1 + product.TaxConvert / 100), 4);
                                product.TaxAmount = Math.Round(item.PRICE * (1 + item.VAT_RATIO) / (1 + product.TaxConvert / 100) * item.AMOUNT * (product.TaxConvert / 100), 0);
                            }
                        }

                        product.AmountWithoutTax = Math.Round((product.ProdPrice ?? 0) * (product.ProdQuantity ?? 0), 4) - (item.DISCOUNT ?? 0);
                        product.Amount = (product.AmountWithoutTax ?? 0) + (product.TaxAmount ?? 0);

                        if (HisConfigCFG.VatOption == "3")
                        {
                            product.AmountWithoutTax = product.Amount;
                            product.ProdPrice = Math.Round((product.AmountWithoutTax ?? 0) / (product.ProdQuantity ?? 0), 4);
                            product.TaxAmount = 0;
                            product.TaxPercentage = null;
                        }
                        else
                        {
                            if (product.TaxConvert == 0)
                            {
                                product.TaxPercentage = 0;
                            }
                            else if (product.TaxConvert == 5)
                            {
                                product.TaxPercentage = 1;
                            }
                            else if (product.TaxConvert == 10)
                            {
                                product.TaxPercentage = 2;
                            }
                            else if (product.TaxConvert == 8)
                            {
                                product.TaxPercentage = 3;
                            }
                            else if (product.TaxConvert > 0)
                            {
                                product.TaxPercentage = 99999;
                            }
                        }
                        if (HisConfigCFG.RoundTransactionAmountOption == "1" || HisConfigCFG.RoundTransactionAmountOption == "2")
                        {
                            if (HisConfigCFG.RoundTransactionAmountOption == "1")
                                product.ProdPrice = Math.Round(product.ProdPrice ?? 0, 0, MidpointRounding.AwayFromZero);
                            product.Amount = Math.Round(product.Amount, 0, MidpointRounding.AwayFromZero);
                            product.TaxAmount = Math.Round(product.TaxAmount ?? 0, 0, MidpointRounding.AwayFromZero);
                            product.AmountWithoutTax = Math.Round(product.AmountWithoutTax ?? 0, 0, MidpointRounding.AwayFromZero);
                        }
                        result.Add(product);
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
