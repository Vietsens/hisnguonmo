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
    class Template6 : IRunTemplate
    {
        private Base.ElectronicBillDataInput DataInput;

        public Template6(Base.ElectronicBillDataInput dataInput)
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
                    var sereServExam = DataInput.SereServBill.Where(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH).ToList();
                    var sereServSubclinical = DataInput.SereServBill.Where(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN ||
                        o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA ||
                        o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN ||
                        o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS ||
                        o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA ||
                        o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__GPBL).ToList();
                    var sereServPttt = DataInput.SereServBill.Where(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT ||
                        o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT ||
                        o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PHCN).ToList();
                    var sereServBed = DataInput.SereServBill.Where(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G).ToList();
                    var sereServMediMate = DataInput.SereServBill.Where(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC ||
                        o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT ||
                        o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU).ToList();
                    var sereServOther = DataInput.SereServBill.Where(o => (sereServExam != null ? !sereServExam.Contains(o) : true) &&
                        (sereServSubclinical != null ? !sereServSubclinical.Contains(o) : true) &&
                        (sereServPttt != null ? !sereServPttt.Contains(o) : true) &&
                        (sereServBed != null ? !sereServBed.Contains(o) : true) &&
                        (sereServMediMate != null ? !sereServMediMate.Contains(o) : true)).ToList();

                    if (sereServExam != null && sereServExam.Count > 0)
                    {
                        ProductBase product = new ProductBase();
                        product.ProdName = "Khám bệnh";
                        product.ProdPrice = Inventec.Common.Number.Convert.NumberToNumberRoundMax4(sereServExam.Sum(s => s.PRICE));
                        product.ProdQuantity = 1;
                        product.Amount = Inventec.Common.Number.Convert.NumberToNumberRoundMax4(sereServExam.Sum(s => s.PRICE));
                        product.ProdUnit = " ";
                        product.TaxRateID = Base.ProviderType.tax_KCT;
                        product.ProdCode = "KB";
                        result.Add(product);
                    }

                    if (sereServSubclinical != null && sereServSubclinical.Count > 0)
                    {
                        ProductBase product = new ProductBase();
                        product.ProdName = "Cận lâm sàng";
                        product.ProdPrice = Inventec.Common.Number.Convert.NumberToNumberRoundMax4(sereServSubclinical.Sum(s => s.PRICE));
                        product.ProdQuantity = 1;
                        product.Amount = Inventec.Common.Number.Convert.NumberToNumberRoundMax4(sereServSubclinical.Sum(s => s.PRICE));
                        product.ProdUnit = " ";
                        product.TaxRateID = Base.ProviderType.tax_KCT;
                        product.ProdCode = "CLS";
                        result.Add(product);
                    }

                    if (sereServPttt != null && sereServPttt.Count > 0)
                    {
                        ProductBase product = new ProductBase();
                        product.ProdName = "Phẫu thuật, thủ thuật";
                        product.ProdPrice = Inventec.Common.Number.Convert.NumberToNumberRoundMax4(sereServPttt.Sum(s => s.PRICE));
                        product.ProdQuantity = 1;
                        product.Amount = Inventec.Common.Number.Convert.NumberToNumberRoundMax4(sereServPttt.Sum(s => s.PRICE));
                        product.ProdUnit = " ";
                        product.TaxRateID = Base.ProviderType.tax_KCT;
                        product.ProdCode = "PTTT";
                        result.Add(product);
                    }

                    if (sereServBed != null && sereServBed.Count > 0)
                    {
                        ProductBase product = new ProductBase();
                        product.ProdName = "Giường bệnh";
                        product.ProdPrice = Inventec.Common.Number.Convert.NumberToNumberRoundMax4(sereServBed.Sum(s => s.PRICE));
                        product.ProdQuantity = 1;
                        product.Amount = Inventec.Common.Number.Convert.NumberToNumberRoundMax4(sereServBed.Sum(s => s.PRICE));
                        product.ProdUnit = " ";
                        product.TaxRateID = Base.ProviderType.tax_KCT;
                        product.ProdCode = "GB";
                        result.Add(product);
                    }

                    if (sereServMediMate != null && sereServMediMate.Count > 0)
                    {
                        ProductBase product = new ProductBase();
                        product.ProdName = "Thuốc, VTYT";
                        product.ProdPrice = Inventec.Common.Number.Convert.NumberToNumberRoundMax4(sereServMediMate.Sum(s => s.PRICE));
                        product.ProdQuantity = 1;
                        product.Amount = Inventec.Common.Number.Convert.NumberToNumberRoundMax4(sereServMediMate.Sum(s => s.PRICE));
                        product.ProdUnit = " ";
                        product.TaxRateID = Base.ProviderType.tax_KCT;
                        product.ProdCode = "TH";
                        product.Type = 1;
                        result.Add(product);
                    }

                    if (sereServOther != null && sereServOther.Count > 0)
                    {
                        if ((DataInput.Transaction != null && DataInput.Transaction.SALE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SALE_TYPE.ID__SALE_OTHER)
                            || (DataInput.ListTransaction != null && DataInput.ListTransaction.Count > 0 && DataInput.ListTransaction.Exists(o => o.SALE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SALE_TYPE.ID__SALE_OTHER)))
                        {
                            var serviceTypesZero = sereServOther.Where(o => o.TDL_SERVICE_TYPE_ID == 0).ToList();
                            foreach (var item in serviceTypesZero)
                            {
                                ProductBase product = new ProductBase();
                                product.ProdName = item.TDL_SERVICE_NAME;
                                product.ProdPrice = Inventec.Common.Number.Convert.NumberToNumberRoundMax4(item.PRICE);
                                product.ProdQuantity = 1;
                                product.Amount = Inventec.Common.Number.Convert.NumberToNumberRoundMax4(item.PRICE);
                                product.ProdUnit = " ";
                                product.TaxRateID = Base.ProviderType.tax_KCT;
                                product.ProdCode = item.TDL_SERVICE_CODE;
                                product.Type = item.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC ? 1 : 0;
                                result.Add(product);
                            }

                            var ssOther = sereServOther.Where(o => o.TDL_SERVICE_TYPE_ID > 0).ToList();
                            if (ssOther != null && ssOther.Count > 0)
                            {
                                ProductBase product = new ProductBase();
                                product.ProdName = "Dịch vụ khác";
                                product.ProdPrice = Inventec.Common.Number.Convert.NumberToNumberRoundMax4(ssOther.Sum(s => s.PRICE));
                                product.ProdQuantity = 1;
                                product.Amount = Inventec.Common.Number.Convert.NumberToNumberRoundMax4(ssOther.Sum(s => s.PRICE));
                                product.ProdUnit = " ";
                                product.TaxRateID = Base.ProviderType.tax_KCT;
                                product.ProdCode = "DVKH";
                                product.Type = ssOther.Count() == ssOther.Count(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC) ? 1 : 0;
                                result.Add(product);
                            }
                        }
                        else
                        {
                            ProductBase product = new ProductBase();
                            product.ProdName = "Dịch vụ khác";
                            product.ProdPrice = Inventec.Common.Number.Convert.NumberToNumberRoundMax4(sereServOther.Sum(s => s.PRICE));
                            product.ProdQuantity = 1;
                            product.Amount = Inventec.Common.Number.Convert.NumberToNumberRoundMax4(sereServOther.Sum(s => s.PRICE));
                            product.ProdUnit = " ";
                            product.TaxRateID = Base.ProviderType.tax_KCT;
                            product.ProdCode = "DVKH";
                            product.Type = sereServOther.Count() == sereServOther.Count(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC) ? 1 : 0;
                            result.Add(product);
                        }
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
