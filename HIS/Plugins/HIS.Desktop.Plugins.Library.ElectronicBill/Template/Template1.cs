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
using Inventec.Common.Logging;
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
    public class Template1 : IRunTemplate
    {
        Dictionary<SERE_SERV_TYPE, List<HIS_SERE_SERV_BILL>> dicSereServType { get; set; }
        public enum SERE_SERV_TYPE
        {
            BHYT_NOT_SERVICE_CONFIG,
            NOT_BHYT_NOT_SERVICE_CONFIG,
            SERVICE_CONFIG,
            NOT_SERVICE_CONFIG
        }

        private long treatmentId { get; set; }
        private HIS_BRANCH branch { get; set; }
        private List<HIS_SERE_SERV_BILL> sereServs;
        private Base.ElectronicBillDataInput DataInput;

        public Template1(long _treatmentId, HIS_BRANCH _branch, List<HIS_SERE_SERV_BILL> list, Base.ElectronicBillDataInput dataInput)
        {
            this.treatmentId = _treatmentId;
            this.branch = _branch;
            this.sereServs = list;
            this.DataInput = dataInput;
        }

        object IRunTemplate.Run()
        {
            List<ProductBase> products = new List<ProductBase>();
            try
            {
                if (sereServs != null && sereServs.Count > 0)
                {
                    //Doi tuong benh nhan
                    if (DataInput.LastPatientTypeAlter == null)
                    {
                        LogSystem.Debug("Khong tim thay doi tuong benh nhan hien tai!");
                        return null;
                    }

                    List<V_HIS_SERVICE> services = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => sereServs.Exists(e => e.TDL_SERVICE_ID == o.ID)).ToList();

                    //Neu benh nhan doi tuong la BHYT
                    //- Nếu diện điều trị là điều trị ngoại trú và số % BHYT cùng chi trả khác 100%
                    //Số % bệnh nhân đồng chi trả - Viện phí bệnh án ngoại trú
                    if (DataInput.LastPatientTypeAlter.PATIENT_TYPE_ID == HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT)
                    {
                        dicSereServType = new Dictionary<SERE_SERV_TYPE, List<HIS_SERE_SERV_BILL>>();
                        dicSereServType[SERE_SERV_TYPE.BHYT_NOT_SERVICE_CONFIG] = new List<HIS_SERE_SERV_BILL>();
                        dicSereServType[SERE_SERV_TYPE.NOT_BHYT_NOT_SERVICE_CONFIG] = new List<HIS_SERE_SERV_BILL>();
                        dicSereServType[SERE_SERV_TYPE.SERVICE_CONFIG] = new List<HIS_SERE_SERV_BILL>();
                        dicSereServType[SERE_SERV_TYPE.NOT_SERVICE_CONFIG] = new List<HIS_SERE_SERV_BILL>();

                        dicSereServType[SERE_SERV_TYPE.SERVICE_CONFIG] = this.GetListSereServByConfig(sereServs);
                        dicSereServType[SERE_SERV_TYPE.NOT_SERVICE_CONFIG] = sereServs.Where(o => dicSereServType[SERE_SERV_TYPE.SERVICE_CONFIG] != null ? !dicSereServType[SERE_SERV_TYPE.SERVICE_CONFIG].Contains(o) : true).ToList();
                        dicSereServType[SERE_SERV_TYPE.BHYT_NOT_SERVICE_CONFIG] = dicSereServType[SERE_SERV_TYPE.NOT_SERVICE_CONFIG].Where(o => o.TDL_PATIENT_TYPE_ID == HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT).ToList();
                        dicSereServType[SERE_SERV_TYPE.NOT_BHYT_NOT_SERVICE_CONFIG] = dicSereServType[SERE_SERV_TYPE.NOT_SERVICE_CONFIG].Where(o => o.TDL_PATIENT_TYPE_ID != HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT).ToList();

                        //Check thêm xem có toàn vẹn dữ liệu không
                        if (dicSereServType[SERE_SERV_TYPE.NOT_BHYT_NOT_SERVICE_CONFIG].Count + dicSereServType[SERE_SERV_TYPE.BHYT_NOT_SERVICE_CONFIG].Count + dicSereServType[SERE_SERV_TYPE.SERVICE_CONFIG].Count != sereServs.Count)
                        {
                            throw new Exception("Loi : Kiem tra toan ven du lieu");
                        }

                        #region SERE_SERV_BHYT
                        if (dicSereServType[SERE_SERV_TYPE.BHYT_NOT_SERVICE_CONFIG] != null && dicSereServType[SERE_SERV_TYPE.BHYT_NOT_SERVICE_CONFIG].Count > 0)
                        {
                            ProductBase product = new ProductBase();
                            product.Amount = Inventec.Common.Number.Convert.NumberToNumberRoundMax4(dicSereServType[SERE_SERV_TYPE.BHYT_NOT_SERVICE_CONFIG].Sum(o => o.PRICE));

                            decimal ratio = (new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(DataInput.LastPatientTypeAlter.HEIN_TREATMENT_TYPE_CODE, DataInput.LastPatientTypeAlter.HEIN_CARD_NUMBER, branch.HEIN_LEVEL_CODE, DataInput.LastPatientTypeAlter.RIGHT_ROUTE_CODE) ?? 0) * 100;
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
                            product.ProdCode = General.GetFirstWord(product.ProdName);
                            product.Type = dicSereServType[SERE_SERV_TYPE.BHYT_NOT_SERVICE_CONFIG].Count() == dicSereServType[SERE_SERV_TYPE.BHYT_NOT_SERVICE_CONFIG].Count(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC) ? 1 : 0;
                            products.Add(product);
                        }
                        #endregion

                        #region SERE_SERV_NOT_BHYT
                        if (dicSereServType[SERE_SERV_TYPE.NOT_BHYT_NOT_SERVICE_CONFIG] != null && dicSereServType[SERE_SERV_TYPE.NOT_BHYT_NOT_SERVICE_CONFIG].Count > 0)
                        {
                            var sereServNotBHYTGroups = dicSereServType[SERE_SERV_TYPE.NOT_BHYT_NOT_SERVICE_CONFIG].GroupBy(o => o.TDL_SERVICE_TYPE_ID);
                            List<HIS_SERVICE_TYPE> serviceTypes = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_SERVICE_TYPE>();
                            foreach (var item in sereServNotBHYTGroups)
                            {
                                HIS_SERVICE_TYPE serviceType = serviceTypes.FirstOrDefault(o => o.ID == item.First().TDL_SERVICE_TYPE_ID);
                                ProductBase product = new ProductBase();
                                product.ProdName = serviceType != null ? serviceType.SERVICE_TYPE_NAME : "Khác";
                                product.ProdCode = General.GetFirstWord(product.ProdName);
                                product.Amount = Inventec.Common.Number.Convert.NumberToNumberRoundMax4(item.Sum(o => o.PRICE));
                                product.ProdUnit = "";
                                product.Type = item.Count() == item.Count(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC) ? 1 : 0;
                                products.Add(product);
                            }
                        }
                        #endregion

                        #region SERE_SERV_CONFIG
                        if (dicSereServType[SERE_SERV_TYPE.SERVICE_CONFIG] != null && dicSereServType[SERE_SERV_TYPE.SERVICE_CONFIG].Count > 0)
                        {
                            foreach (var item in dicSereServType[SERE_SERV_TYPE.SERVICE_CONFIG])
                            {
                                V_HIS_SERVICE service = services != null ? services.FirstOrDefault(o => o.ID == item.TDL_SERVICE_ID) : null;
                                ProductBase product = new ProductBase();
                                product.ProdName = item.TDL_SERVICE_NAME;
                                product.ProdCode = item.TDL_SERVICE_CODE;
                                //product.ProdPrice = Inventec.Common.Number.Convert.NumberToNumberRoundMax4(item.TDL_REAL_PRICE ?? 0);
                                product.ProdQuantity = item.TDL_AMOUNT ?? 0;
                                product.Amount = Inventec.Common.Number.Convert.NumberToNumberRoundMax4(item.PRICE);
                                product.ProdPrice = Inventec.Common.Number.Convert.NumberToNumberRoundMax4(product.Amount / (product.ProdQuantity ?? 1));
                                product.Type = item.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC ? 1 : 0;
                                product.ProdUnit = service != null ? service.SERVICE_UNIT_NAME : "";
                                product.TaxRateID = (int)(service != null ? (service.TAX_RATE_TYPE ?? Base.ProviderType.tax_KCT) : Base.ProviderType.tax_KCT);
                                products.Add(product);
                            }
                        }
                        #endregion
                    }
                    else// if (currentPatyAlter.PATIENT_TYPE_ID == HisPatientTypeCFG.PATIENT_TYPE_ID__IS_FEE)
                    {
                        if (DataInput.LastPatientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM)
                        {
                            var groupPrice = sereServs.GroupBy(o => new { o.TDL_SERVICE_ID, o.TDL_REAL_PRICE }).ToList();
                            foreach (var item in groupPrice)
                            {
                                V_HIS_SERVICE service = services != null ? services.FirstOrDefault(o => o.ID == item.First().TDL_SERVICE_ID) : null;
                                ProductBase product = new ProductBase();
                                product.ProdName = item.First().TDL_SERVICE_NAME;
                                product.ProdCode = item.First().TDL_SERVICE_CODE;
                                //product.ProdPrice = Inventec.Common.Number.Convert.NumberToNumberRoundMax4(item.First().TDL_REAL_PRICE ?? 0);
                                product.ProdQuantity = item.Sum(s => s.TDL_AMOUNT ?? 0);
                                product.Amount = Inventec.Common.Number.Convert.NumberToNumberRoundMax4(item.Sum(s => s.PRICE));
                                product.ProdUnit = service != null ? service.SERVICE_UNIT_NAME : "";
                                product.ProdPrice = Inventec.Common.Number.Convert.NumberToNumberRoundMax4(product.Amount / (product.ProdQuantity ?? 1));
                                product.TaxRateID = (int)(service != null ? (service.TAX_RATE_TYPE ?? Base.ProviderType.tax_KCT) : Base.ProviderType.tax_KCT);
                                product.Type = item.First().TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC ? 1 : 0;
                                products.Add(product);
                            }
                        }
                        else
                        {
                            ProductBase product = new ProductBase();
                            product.Amount = Inventec.Common.Number.Convert.NumberToNumberRoundMax4(sereServs.Sum(o => o.PRICE));
                            product.ProdName = "Thu viện phí nhân dân";
                            product.ProdCode = General.GetFirstWord(product.ProdName);
                            product.ProdUnit = "";
                            products.Add(product);
                        }
                    }
                }

                if (products.Count > 0)
                {
                    foreach (var product in products)
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return products;
        }

        private List<HIS_SERE_SERV_BILL> GetListSereServByConfig(List<HIS_SERE_SERV_BILL> sereServs)
        {
            List<HIS_SERE_SERV_BILL> sereServCfgs = new List<HIS_SERE_SERV_BILL>();
            try
            {
                string serviceIndepent = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HisConfigCFG.SERVICE_INDEPENDENT_DISPLAY);
                if (String.IsNullOrEmpty(serviceIndepent)) return new List<HIS_SERE_SERV_BILL>();
                string[] serviceIndepentStr = serviceIndepent.Split('|');
                if (serviceIndepentStr != null && serviceIndepentStr.Length > 0)
                {
                    sereServCfgs = sereServs.Where(o => serviceIndepentStr.Contains(o.TDL_SERVICE_CODE)).ToList();
                }
            }
            catch (Exception ex)
            {
                sereServCfgs = new List<HIS_SERE_SERV_BILL>();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return sereServCfgs;
        }
    }
}
