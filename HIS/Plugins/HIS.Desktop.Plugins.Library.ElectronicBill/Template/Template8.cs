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
    class Template8 : IRunTemplate
    {
        private Base.ElectronicBillDataInput DataInput;

        public Template8(Base.ElectronicBillDataInput dataInput)
        {
            // TODO: Complete member initialization
            this.DataInput = dataInput;
        }

        public object Run()
        {
            List<ProductBase> result = new List<ProductBase>();
            if (DataInput.SereServBill != null && DataInput.SereServBill.Count > 0)
            {
                List<HIS_SERE_SERV_BILL> sereServsTotal = new List<HIS_SERE_SERV_BILL>();
                sereServsTotal.AddRange(DataInput.SereServBill);
                bool splitPriceBhyt = false;
                bool isServiceTransaction = false;

                //hóa đơn dịch vụ có BILL_TYPE_ID = 2
                //chi tiết của hóa đơn điện tử của hóa đơn dịch vụ sẽ không có dòng cùng chi trả bảo hiểm
                if (DataInput.Transaction != null && DataInput.Transaction.BILL_TYPE_ID == 2)
                {
                    isServiceTransaction = true;
                }

                string templateDetailStr = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HisConfigCFG.TemplateDetail);
                List<TemplateDetailADO> dataDetail = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TemplateDetailADO>>(templateDetailStr);
                if (dataDetail != null && dataDetail.Count > 0)
                {
                    dataDetail = dataDetail.OrderBy(o => o.NumOrder ?? 9999).ToList();
                    foreach (var detail in dataDetail)
                    {
                        if (!String.IsNullOrWhiteSpace(detail.HeinServiceTypeCodes))
                        {
                            List<string> heinServiceTypeCodes = detail.HeinServiceTypeCodes.Split('|').ToList();
                            detail.HeinServiceTypeIds = BackendDataWorker.Get<HIS_HEIN_SERVICE_TYPE>().Where(o => heinServiceTypeCodes.Contains(o.HEIN_SERVICE_TYPE_CODE)).Select(s => s.ID).ToList();
                        }

                        if (!String.IsNullOrWhiteSpace(detail.ServiceTypeCodes))
                        {
                            List<string> serviceTypeCodes = detail.ServiceTypeCodes.Split('|').ToList();
                            detail.ServiceTypeIds = BackendDataWorker.Get<HIS_SERVICE_TYPE>().Where(o => serviceTypeCodes.Contains(o.SERVICE_TYPE_CODE)).Select(s => s.ID).ToList();
                        }

                        if (!String.IsNullOrWhiteSpace(detail.ParentServiceCodes))
                        {
                            List<string> parentServiceCodes = detail.ParentServiceCodes.Split('|').ToList();
                            detail.ParentServiceIds = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => parentServiceCodes.Contains(o.SERVICE_CODE)).Select(s => s.ID).ToList();
                        }

                        if (!String.IsNullOrWhiteSpace(detail.TreatmentTypeCodes))
                        {
                            List<string> treatmentTypeCodes = detail.TreatmentTypeCodes.Split('|').ToList();
                            detail.TreatmentTypeIds = BackendDataWorker.Get<HIS_TREATMENT_TYPE>().Where(o => treatmentTypeCodes.Contains(o.TREATMENT_TYPE_CODE)).Select(s => s.ID).ToList();
                        }

                        if (!String.IsNullOrWhiteSpace(detail.PatientTypeCodes))
                        {
                            List<string> patientTypeCodes = detail.PatientTypeCodes.Split('|').ToList();
                            detail.PatientTypeIds = BackendDataWorker.Get<HIS_PATIENT_TYPE>().Where(o => patientTypeCodes.Contains(o.PATIENT_TYPE_CODE)).Select(s => s.ID).ToList();
                        }
                    }

                    Dictionary<string, string> dicXmlValues = General.ProcessDicValueString(this.DataInput);

                    foreach (var detail in dataDetail)
                    {
                        if (sereServsTotal.Count <= 0)
                            break;

                        //không có thông tin thì bỏ qua
                        if (String.IsNullOrWhiteSpace(detail.Display) &&
                            String.IsNullOrWhiteSpace(detail.ServiceTypeCodes) &&
                            String.IsNullOrWhiteSpace(detail.ServiceCodes) &&
                            String.IsNullOrWhiteSpace(detail.HeinServiceTypeCodes) &&
                            String.IsNullOrWhiteSpace(detail.TreatmentTypeCodes) &&
                            String.IsNullOrWhiteSpace(detail.ParentServiceCodes) &&
                            String.IsNullOrWhiteSpace(detail.PatientTypeCodes) &&
                            !detail.IsSplit.HasValue &&
                            !detail.IsBHYT.HasValue)
                            continue;

                        //bỏ qua chi tiết nhóm bảo hiểm nếu hóa đơn dịch vụ và có tách chi phí dịch vụ
                        if (detail.IsBHYT == 1 && detail.IsSplitBhytPrice == 1 && isServiceTransaction)
                        {
                            continue;
                        }

                        if (!String.IsNullOrWhiteSpace(detail.TreatmentTypeCodes) && detail.TreatmentTypeIds != null && detail.TreatmentTypeIds.Count > 0
                            && (DataInput.Treatment == null || !detail.TreatmentTypeIds.Contains(DataInput.Treatment.TDL_TREATMENT_TYPE_ID ?? 0)))
                        {
                            continue;
                        }

                        //thay thế các key trong tên hiển thị.
                        if (!String.IsNullOrWhiteSpace(detail.Display))
                        {
                            foreach (var item in dicXmlValues)
                            {
                                detail.Display = detail.Display.Replace(string.Format("<#{0};>", item.Key), item.Value);
                            }
                        }

                        //luôn gán tất cả để hiển thị theo gom nhóm hay chi tiết
                        List<HIS_SERE_SERV_BILL> sereServs = new List<HIS_SERE_SERV_BILL>();
                        sereServs.AddRange(sereServsTotal);

                        if (detail.IsBHYT == 1)
                        {
                            sereServs = sereServs.Where(o => o.TDL_PATIENT_TYPE_ID == HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT).ToList();
                            if (detail.IsSplitBhytPrice == 1 && detail.IsSplit != 1)
                            {
                                splitPriceBhyt = true;
                                sereServs = sereServs.Where(o => o.PRICE - (o.TDL_TOTAL_PATIENT_PRICE_BHYT ?? 0) >= 0).ToList();
                            }
                        }
                        else
                        {
                            if (!String.IsNullOrWhiteSpace(detail.PatientTypeCodes))
                            {
                                sereServs = detail.PatientTypeIds != null ? sereServs.Where(o => detail.PatientTypeIds.Contains(o.TDL_PATIENT_TYPE_ID ?? 0) || detail.PatientTypeIds.Contains(o.TDL_PRIMARY_PATIENT_TYPE_ID ?? 0)).ToList() : new List<HIS_SERE_SERV_BILL>();
                            }

                            if (!String.IsNullOrWhiteSpace(detail.HeinServiceTypeCodes))
                            {
                                sereServs = detail.HeinServiceTypeIds != null ? sereServs.Where(o => detail.HeinServiceTypeIds.Contains(o.TDL_HEIN_SERVICE_TYPE_ID ?? 0)).ToList() : new List<HIS_SERE_SERV_BILL>();
                            }

                            if (!String.IsNullOrWhiteSpace(detail.ServiceTypeCodes))
                            {
                                sereServs = detail.ServiceTypeIds != null ? sereServs.Where(o => detail.ServiceTypeIds.Contains(o.TDL_SERVICE_TYPE_ID ?? 0)).ToList() : new List<HIS_SERE_SERV_BILL>();
                            }

                            if (!String.IsNullOrWhiteSpace(detail.ParentServiceCodes))
                            {
                                List<long> ServiceIds = detail.ParentServiceIds != null ? BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => detail.ParentServiceIds.Contains(o.PARENT_ID ?? 0)).Select(s => s.ID).ToList() : new List<long>();
                                sereServs = sereServs.Where(o => ServiceIds.Contains(o.TDL_SERVICE_ID ?? 0)).ToList();
                            }

                            if (!String.IsNullOrWhiteSpace(detail.ServiceCodes))
                            {
                                List<string> serviceCodes = detail.ServiceCodes.Split('|').ToList();
                                sereServs = sereServs.Where(o => serviceCodes.Contains(o.TDL_SERVICE_CODE)).ToList();
                            }
                        }

                        if (sereServs != null && sereServs.Count > 0)
                        {
                            if (detail.IsSplit == 1)
                            {
                                var groupPrice = sereServs.GroupBy(o => new { o.TDL_SERVICE_ID, o.TDL_REAL_PRICE }).ToList();
                                List<V_HIS_SERVICE> services = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => sereServs.Exists(e => e.TDL_SERVICE_ID == o.ID)).ToList();

                                foreach (var item in groupPrice)
                                {
                                    decimal amount = 0;
                                    if (splitPriceBhyt)
                                    {
                                        amount += item.Where(o => o.TDL_PATIENT_TYPE_ID != HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT).Sum(s => s.PRICE);
                                        amount += item.Where(o => o.TDL_PATIENT_TYPE_ID == HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT).Sum(s => s.PRICE - (s.TDL_TOTAL_PATIENT_PRICE_BHYT ?? 0));
                                    }
                                    else
                                    {
                                        amount += item.Sum(s => s.PRICE);
                                    }
                                    if (amount == 0)
                                        continue;
                                    V_HIS_SERVICE service = services != null ? services.FirstOrDefault(o => o.ID == item.First().TDL_SERVICE_ID) : null;
                                    ProductBase product1 = new ProductBase();
                                    product1.ProdName = item.First().TDL_SERVICE_NAME;
                                    product1.Amount = Inventec.Common.Number.Convert.NumberToNumberRoundMax4(amount);
                                    product1.ProdUnit = service != null ? service.SERVICE_UNIT_NAME : "";
                                    product1.TaxRateID = (int)(service != null ? (service.TAX_RATE_TYPE ?? Base.ProviderType.tax_KCT) : Base.ProviderType.tax_KCT);
                                    product1.ProdCode = item.First().TDL_SERVICE_CODE;
                                    product1.Type = item.First().TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC ? 1 : 0;
                                    product1.ProdQuantity = item.Sum(o => o.TDL_AMOUNT);
                                    product1.ProdPrice = Math.Round(product1.Amount / (product1.ProdQuantity ?? 1), 0);
                                    if (!String.IsNullOrWhiteSpace(detail.Unit))
                                    {
                                        product1.ProdUnit = detail.Unit;
                                        product1.ProdQuantity = 1;
                                        product1.ProdPrice = product1.Amount;
                                    }
                                    product1.Stt = detail.Stt ?? 0;
                                    result.Add(product1);
                                }
                                Inventec.Common.Logging.LogSystem.Debug("_______RESULT:"+ Inventec.Common.Logging.LogUtil.TraceData("result",result));
                            }
                            else if (detail.IsBHYT == 1)
                            {
                                if (DataInput.LastPatientTypeAlter == null)
                                {
                                    Inventec.Common.Logging.LogSystem.Debug("Khong tim thay doi tuong benh nhan hien tai!");
                                    continue;
                                }

                                ProductBase product = new ProductBase();
                                if (splitPriceBhyt)//số tiền cùng chi trả bhyt
                                {
                                    product.Amount = Inventec.Common.Number.Convert.NumberToNumberRoundMax4(sereServs.Sum(o => o.TDL_TOTAL_PATIENT_PRICE_BHYT ?? 0));
                                }
                                else
                                {
                                    product.Amount = Inventec.Common.Number.Convert.NumberToNumberRoundMax4(sereServs.Sum(o => o.PRICE));
                                }
                                if (product.Amount == 0)
                                    continue;
                                decimal ratio = (new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(DataInput.LastPatientTypeAlter.HEIN_TREATMENT_TYPE_CODE, DataInput.LastPatientTypeAlter.HEIN_CARD_NUMBER, DataInput.LastPatientTypeAlter.LEVEL_CODE, DataInput.LastPatientTypeAlter.RIGHT_ROUTE_CODE) ?? 0) * 100;

                                string display = "";
                                //"Đồng chi trả BH {0}%|Công khám"
                                if (Math.Round(100 - ratio, 0, MidpointRounding.AwayFromZero) == 0)
                                {
                                    //Công khám
                                    display = detail.Display.Split('|').Last();
                                }
                                else
                                {
                                    //Đồng chi trả BH {0}%
                                    display = detail.Display.Split('|').First();
                                }

                                product.ProdName = String.Format(display, Math.Round(100 - ratio, 0, MidpointRounding.AwayFromZero));
                                product.ProdCode = General.GetFirstWord(product.ProdName);
                                product.ProdUnit = detail.Unit;
                                product.Stt = detail.Stt ?? 0;
                                product.IsBHYT = true;
                                result.Add(product);
                            }
                            else
                            {
                                decimal amount = 0;
                                if (splitPriceBhyt)
                                {
                                    amount += sereServs.Where(o => o.TDL_PATIENT_TYPE_ID != HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT).Sum(s => s.PRICE);
                                    amount += sereServs.Where(o => o.TDL_PATIENT_TYPE_ID == HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT).Sum(s => s.PRICE - (s.TDL_TOTAL_PATIENT_PRICE_BHYT ?? 0));
                                }
                                else
                                {
                                    amount += sereServs.Sum(s => s.PRICE);
                                }

                                ProductBase product = new ProductBase();
                                product.ProdName = detail.Display;
                                product.ProdUnit = detail.Unit;
                                product.Amount = Inventec.Common.Number.Convert.NumberToNumberRoundMax4(amount);
                                product.ProdCode = General.GetFirstWord(product.ProdName);
                                product.Type = sereServs.Count() == sereServs.Count(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC) ? 1 : 0;
                                product.Stt = detail.Stt ?? 0;
                                result.Add(product);
                            }

                            if (splitPriceBhyt && detail.IsBHYT == 1)
                            {
                                List<long> priceBhytIds = sereServs.Where(o => o.PRICE - (o.TDL_TOTAL_PATIENT_PRICE_BHYT ?? 0) == 0).Select(s => s.ID).ToList();
                                sereServsTotal = sereServsTotal.Where(o => !priceBhytIds.Contains(o.ID)).ToList();
                            }
                            else
                            {
                                sereServsTotal = sereServsTotal.Where(o => !sereServs.Select(s => s.ID).Contains(o.ID)).ToList();
                            }
                        }
                    }

                    if (sereServsTotal != null && sereServsTotal.Count > 0)
                    {
                        decimal amount = 0;
                        if (splitPriceBhyt)
                        {
                            amount = sereServsTotal.Where(o => o.TDL_PATIENT_TYPE_ID != HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT).Sum(s => s.PRICE);
                            amount += sereServsTotal.Where(o => o.TDL_PATIENT_TYPE_ID == HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT).Sum(s => s.PRICE - (s.TDL_TOTAL_PATIENT_PRICE_BHYT ?? 0));
                        }
                        else
                        {
                            amount = sereServsTotal.Sum(s => s.PRICE);
                        }
                        if (amount != 0)
                        {
                            ProductBase product = new ProductBase();
                            product.ProdName = "Dịch vụ khác";
                            product.ProdUnit = "Lần";
                            product.ProdCode = "DVK";
                            product.Amount = Inventec.Common.Number.Convert.NumberToNumberRoundMax4(amount);
                            product.Type = sereServsTotal.Count() == sereServsTotal.Count(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC) ? 1 : 0;
                            product.Stt = long.MaxValue;
                            result.Add(product);
                        }
                    }
                }
                else
                {
                    throw new Exception("Sai cấu hình chi tiết hóa đơn");
                }

                if (result.Count > 0)
                {
                    result = result.OrderBy(o => o.Stt).ToList();

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
            return result;
        }
    }
}
