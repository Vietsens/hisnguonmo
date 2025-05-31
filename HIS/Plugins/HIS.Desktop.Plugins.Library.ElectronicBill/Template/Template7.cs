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
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.ElectronicBill.Template
{
    class Template7 : IRunTemplate
    {
        private Base.ElectronicBillDataInput DataInput;

        public Template7(Base.ElectronicBillDataInput dataInput)
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
                    List<V_HIS_SERVICE> services = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => DataInput.SereServBill.Exists(e => e.TDL_SERVICE_ID == o.ID)).ToList();
                    if (DataInput.Treatment.TDL_PATIENT_TYPE_ID == HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT)
                    {
                        //thanh toán 2 sổ
                        //danh sách dịch vụ tại hóa đơn dv sẽ có số tiền bn cùng chi trả giống với danh sách hóa đơn thường
                        //chỉ lấy các dịch vụ có đối tượng bhyt và có tiền thanh toán lớn hơn hoặc bằng tiền bn cùng chi trả đưa vào xử lý theo bhyt
                        //danh sách dv có đối tượng than toán khác bhyt hoặc có tiền bn cùng chi trả nhỏ hơn tiền thanh toán trong chi tiết thì sẽ hiển thị chi tiết)
                        var sereServBhyt = DataInput.SereServBill.Where(o => o.TDL_PATIENT_TYPE_ID == HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT && o.PRICE - (o.TDL_TOTAL_PATIENT_PRICE_BHYT ?? 0) >= 0).ToList();
                        var sereServNotBhyt = DataInput.SereServBill.Where(o => o.TDL_PATIENT_TYPE_ID != HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT || (o.PRICE - (o.TDL_TOTAL_PATIENT_PRICE_BHYT ?? 0) < 0)).ToList();
                        if (sereServBhyt != null && sereServBhyt.Count > 0)
                        {
                            //Doi tuong benh nhan
                            if (DataInput.LastPatientTypeAlter == null)
                            {
                                Inventec.Common.Logging.LogSystem.Debug("Khong tim thay doi tuong benh nhan hien tai!");
                                return null;
                            }

                            ProductBase product = new ProductBase();
                            product.Amount = Inventec.Common.Number.Convert.NumberToNumberRoundMax4(sereServBhyt.Sum(o => o.TDL_TOTAL_PATIENT_PRICE_BHYT ?? 0));

                            decimal ratio = (new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(DataInput.LastPatientTypeAlter.HEIN_TREATMENT_TYPE_CODE, DataInput.LastPatientTypeAlter.HEIN_CARD_NUMBER, DataInput.Branch.HEIN_LEVEL_CODE, DataInput.LastPatientTypeAlter.RIGHT_ROUTE_CODE) ?? 0) * 100;
                            string prodName = "";

                            if (DataInput.LastPatientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU)
                            {
                                if (ratio != 100)
                                {
                                    prodName = String.Format("{0:0.####}% bệnh nhân đồng chi trả - Viện phí bệnh án ngoại trú", 100 - ratio);
                                }
                                else
                                {
                                    prodName = "Thu viện phí - Viện phí bệnh án ngoại trú";
                                }
                            }
                            else if (DataInput.LastPatientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU)
                            {
                                if (ratio != 100)
                                {
                                    prodName = String.Format("{0:0.####}% bệnh nhân đồng chi trả - Viện phí bệnh án nội trú", 100 - ratio);
                                }
                                else
                                {
                                    prodName = "Thu viện phí - Viện phí bệnh án nội trú";
                                }
                            }
                            else if (DataInput.LastPatientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM)
                            {
                                if (ratio != 100)
                                {
                                    prodName = String.Format("{0:0.####}% bệnh nhân đồng chi trả - Khám bệnh ngoại trú", 100 - ratio);
                                }
                                else
                                {
                                    prodName = "Thu viện phí - khám bệnh ngoại trú";
                                }
                            }
                            else
                            {
                                if (ratio != 100)
                                {
                                    prodName = String.Format("{0:0.####}% bệnh nhân đồng chi trả - Khám bệnh", 100 - ratio);
                                }
                                else
                                {
                                    prodName = "Thu viện phí - khám bệnh";
                                }
                            }

                            product.ProdUnit = "";
                            product.ProdName = prodName;
                            product.TaxRateID = Base.ProviderType.tax_KCT;
                            product.ProdCode = General.GetFirstWord(product.ProdName);
                            product.Type = sereServBhyt.Count() == sereServBhyt.Count(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC) ? 1 : 0;
                            result.Add(product);

                            var bncct = sereServBhyt.Where(o => o.PRICE - (o.TDL_TOTAL_PATIENT_PRICE_BHYT ?? 0) > 0).ToList();
                            if (bncct != null && bncct.Count > 0)
                            {
                                var groupPrice = bncct.GroupBy(o => new { o.TDL_SERVICE_ID, o.TDL_REAL_PRICE }).ToList();
                                foreach (var item in groupPrice)
                                {
                                    ProductBase product1 = new ProductBase();
                                    V_HIS_SERVICE service = services != null ? services.FirstOrDefault(o => o.ID == item.First().TDL_SERVICE_ID) : null;
                                    product1.ProdName = service != null ? (service.HEIN_SERVICE_BHYT_NAME ?? service.SERVICE_NAME) : item.First().TDL_SERVICE_NAME;
                                    product1.ProdQuantity = item.Sum(s => s.TDL_AMOUNT ?? 0);
                                    product1.Amount = Inventec.Common.Number.Convert.NumberToNumberRoundMax4(item.Sum(s => (s.PRICE) - (s.TDL_TOTAL_PATIENT_PRICE_BHYT ?? 0)));
                                    product1.ProdPrice = Inventec.Common.Number.Convert.NumberToNumberRoundMax4(product1.Amount / (product1.ProdQuantity ?? 1));
                                    product1.ProdUnit = service != null ? service.SERVICE_UNIT_NAME : "";
                                    product1.TaxRateID = (int)(service != null ? (service.TAX_RATE_TYPE ?? Base.ProviderType.tax_KCT) : Base.ProviderType.tax_KCT);
                                    product1.ProdCode = item.First().TDL_SERVICE_CODE;
                                    product1.Type = item.First().TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC ? 1 : 0;
                                    result.Add(product1);
                                }
                            }
                        }

                        if (sereServNotBhyt != null && sereServNotBhyt.Count > 0)
                        {
                            var groupPrice = sereServNotBhyt.GroupBy(o => new { o.TDL_SERVICE_ID, o.TDL_REAL_PRICE }).ToList();
                            foreach (var item in groupPrice)
                            {
                                V_HIS_SERVICE service = services != null ? services.FirstOrDefault(o => o.ID == item.First().TDL_SERVICE_ID) : null;
                                ProductBase product = new ProductBase();
                                product.ProdName = item.First().TDL_SERVICE_NAME;
                                //product.ProdPrice = Inventec.Common.Number.Convert.NumberToNumberRoundMax4(item.First().TDL_REAL_PRICE ?? 0);
                                product.ProdQuantity = item.Sum(s => s.TDL_AMOUNT ?? 0);
                                product.Amount = Inventec.Common.Number.Convert.NumberToNumberRoundMax4(item.Sum(s => s.PRICE));
                                product.ProdPrice = Inventec.Common.Number.Convert.NumberToNumberRoundMax4(product.Amount / (product.ProdQuantity ?? 1));
                                product.ProdUnit = service != null ? service.SERVICE_UNIT_NAME : "";
                                product.TaxRateID = (int)(service != null ? (service.TAX_RATE_TYPE ?? Base.ProviderType.tax_KCT) : Base.ProviderType.tax_KCT);
                                product.ProdCode = item.First().TDL_SERVICE_CODE;
                                product.Type = item.First().TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC ? 1 : 0;
                                result.Add(product);
                            }
                        }
                    }
                    else
                    {
                        var groupPrice = DataInput.SereServBill.GroupBy(o => new { o.TDL_SERVICE_ID, o.TDL_REAL_PRICE }).ToList();
                        foreach (var item in groupPrice)
                        {
                            V_HIS_SERVICE service = services != null ? services.FirstOrDefault(o => o.ID == item.First().TDL_SERVICE_ID) : null;
                            ProductBase product = new ProductBase();
                            product.ProdName = item.First().TDL_SERVICE_NAME;
                            //product.ProdPrice = Inventec.Common.Number.Convert.NumberToNumberRoundMax4(item.First().TDL_REAL_PRICE ?? 0);
                            product.ProdQuantity = item.Sum(s => s.TDL_AMOUNT ?? 0);
                            product.Amount = Inventec.Common.Number.Convert.NumberToNumberRoundMax4(item.Sum(s => s.PRICE));
                            product.ProdPrice = Inventec.Common.Number.Convert.NumberToNumberRoundMax4(product.Amount / (product.ProdQuantity ?? 1));
                            product.ProdUnit = service != null ? service.SERVICE_UNIT_NAME : "";
                            product.TaxRateID = (int)(service != null ? (service.TAX_RATE_TYPE ?? Base.ProviderType.tax_KCT) : Base.ProviderType.tax_KCT);
                            product.ProdCode = item.First().TDL_SERVICE_CODE;
                            product.Type = item.First().TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC ? 1 : 0;
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
