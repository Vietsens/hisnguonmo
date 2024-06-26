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
using HIS.Desktop.LocalStorage.HisConfig;
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.ElectronicBill.Config
{
    internal class HisConfigCFG
    {
        internal const string ELECTRONIC_BILL__CONFIG = "HIS.DESKTOP.LIBRARY.ELECTRONIC_BILL.CONFIG";
        internal const string SERVICE_INDEPENDENT_DISPLAY = "HIS.DESKTOP.LIBRARY.ELECTRONIC_BILL.SERVICE_INDEPENDENT_DISPLAY";

        internal const string His_Desktop_plugins_ElectriconicBill_Viettel_TaxBreakdown = "HIS.Desktop.Plugins.Library.ElectronicBill.Viettel.TaxBreakdown";
        internal const string His_Desktop_plugins_ElectriconicBill_Viettel_Metadata = "HIS.Desktop.Plugins.Library.ElectronicBill.Viettel.Metadata";

        //private const string His_Desktop_plugins_ElectriconicBill_IsViewTreatmentCode = "HIS.Desktop.Plugins.Library.ElectronicBill.IsViewTreatmentCode";
        private const string His_Desktop_plugins_ElectriconicBill_NameOption = "HIS.Desktop.Plugins.Library.ElectronicBill.NameOption";
        private const string AutoPrintTypeCFG = "HIS.Desktop.Plugins.TransactionBill.ElectronicBill.AutoPrintType";//hóa đơn thường, hóa đơn dịch vụ
        private const string BuyerOrganizationOptionCFG = "HIS.Desktop.Plugins.TransactionBill.ElectronicBill.AutoFill.BuyerOrganizationOption";
        private const string BuyerNameOptionCFG = "HIS.Desktop.Plugins.Library.ElectronicBill.BuyerNameOption";
        private const string BuyerCodeOptionCFG = "HIS.Desktop.Plugins.Library.ElectronicBill.BuyerCodeOption";

        private const string ElectronicBillXmlInvoicePlusCFG = "HIS.Desktop.Plugins.Library.ElectronicBill.XmlInvoicePlus";

        internal const string His_Desktop_plugins_ElectriconicBill_Template = "HIS.Desktop.Plugins.Library.ElectronicBill.Template";
        internal const string TemplateDetail = "HIS.Desktop.Plugins.Library.ElectronicBill.Template.Detail";
        internal const string TemplateSplitVat = "HIS.Desktop.Plugins.Library.ElectronicBill.Template.SplitServicesWithVat";
        private const string splitDetai = "HIS.Desktop.Plugins.Library.ElectronicBill.TempalteSymbol.SplitDetail";

        private const string DetailInfoOptionCFG = "HIS.Desktop.Plugins.Library.ElectronicBill.DetailInfoOption";
        private const string ConvertVatRatioCFG = "HIS.Desktop.Plugins.Library.ElectronicBill.ConvertVatRatio";
        private const string RoundTransactionAmountCFG = "HIS.Desktop.Plugins.Library.ElectronicBill.RoundTransactionAmount";
        private const string VatOptionCFG = "HIS.Desktop.Plugins.TransactionList.ElectronicBill.VatOption";

        //internal static bool IsViewTreatmentCodeCFG;
        internal static bool IsHideUnitName;
        internal static bool IsHideQuantity;
        internal static bool IsHidePrice;
        internal static bool IsSwapNameOption;
        internal static bool IsPrintNormal;
        internal static bool IsSplitServicesWithVat;
        internal static List<string> listTempalteSymbol;
        internal static string ElectronicBillXmlInvoicePlus;
        internal static string BuyerNameOption;
        internal static string BuyerCodeOption;
        internal static string BuyerOrganizationOption;
        internal static string RoundTransactionAmountOption;
        internal static string VatOption;
        internal static Dictionary<decimal, decimal> dicVatConvert = new Dictionary<decimal, decimal>();
        #region Enum TaxBreakdownOption
        internal enum TaxBreakdownOption
        {
            Null = 0,
            CoThongTinThue = 1,
            KhongHienThiThongTinThue = 2
        }
        #endregion
        /// <summary>
        /// 1 - Bổ sung thông tin thuế theo thông tin chi tiết thanh toán.
        /// 2 - Không hiển thị thông tin thuế("TaxBreakdown":[],  "totalTaxAmount": null, "taxPercentage": null).
        /// Khác - Thông tin thuế để giá trị rỗng ("TaxBreakdown":[]).
        /// </summary>
        internal static TaxBreakdownOption Viettel_TaxBreakdownOption;

        internal static void LoadConfig()
        {
            try
            {
                LogSystem.Debug("LoadConfig => 1");
                //IsViewTreatmentCodeCFG = GetValue(His_Desktop_plugins_ElectriconicBill_IsViewTreatmentCode) == "1";
                IsSwapNameOption = GetValue(His_Desktop_plugins_ElectriconicBill_NameOption) == "1";
                IsPrintNormal = GetValue(AutoPrintTypeCFG) == "1";
                IsSplitServicesWithVat = GetValue(TemplateSplitVat) == "1";
                ElectronicBillXmlInvoicePlus = GetValue(ElectronicBillXmlInvoicePlusCFG);
                BuyerNameOption = GetValue(BuyerNameOptionCFG);
                BuyerCodeOption = GetValue(BuyerCodeOptionCFG);
                BuyerOrganizationOption = GetValue(BuyerOrganizationOptionCFG);
                VatOption = GetValue(VatOptionCFG);
                string taxBreakdownCFG = GetValue(Config.HisConfigCFG.His_Desktop_plugins_ElectriconicBill_Viettel_TaxBreakdown);
                switch (taxBreakdownCFG)
                {
                    case "1":
                        Viettel_TaxBreakdownOption = TaxBreakdownOption.CoThongTinThue;
                        break;
                    case "2":
                        Viettel_TaxBreakdownOption = TaxBreakdownOption.KhongHienThiThongTinThue;
                        break;
                    default:
                        Viettel_TaxBreakdownOption = TaxBreakdownOption.Null;
                        break;
                }
                LogSystem.Debug("LoadConfig => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal static void LoadConfigForDetail()
        {
            try
            {
                listTempalteSymbol = new List<string>();
                string dataTempalteSymbol = GetValue(splitDetai);
                if (!String.IsNullOrWhiteSpace(dataTempalteSymbol))
                {
                    listTempalteSymbol = dataTempalteSymbol.Split('|').ToList();
                }

                RoundTransactionAmountOption = GetValue(RoundTransactionAmountCFG);

                IsHideUnitName = false;
                IsHideQuantity = false;
                IsHidePrice = false;
                string detailInfo = GetValue(DetailInfoOptionCFG);
                if (!String.IsNullOrWhiteSpace(detailInfo))
                {
                    var splitDetail = detailInfo.Split('|');
                    for (int i = 0; i < splitDetail.Length; i++)
                    {
                        if (splitDetail[i] == "1")
                        {
                            if (i == 0)
                            {
                                IsHideUnitName = true;
                            }
                            else if (i == 1)
                            {
                                IsHideQuantity = true;
                            }
                            else if (i == 2)
                            {
                                IsHidePrice = true;
                            }
                        }
                    }
                }

                dicVatConvert = new Dictionary<decimal, decimal>();
                string convertVatInfo = GetValue(ConvertVatRatioCFG);
                if (!String.IsNullOrWhiteSpace(convertVatInfo))
                {
                    var splitVatInfo = convertVatInfo.Split('|');
                    foreach (var item in splitVatInfo)
                    {
                        string[] splitK = new string[1] { "->" };
                        var vats = item.Split(splitK, StringSplitOptions.None);
                        if (vats.Length >= 2)
                        {
                            decimal oldVat = Inventec.Common.TypeConvert.Parse.ToDecimal(vats[0]);
                            decimal newVat = Inventec.Common.TypeConvert.Parse.ToDecimal(vats[1]);

                            //vat hợp lệ mới thêm vào diction
                            bool isAdd = true;
                            isAdd = isAdd && (oldVat > 0 || (oldVat == 0 && vats[0] == "0"));
                            isAdd = isAdd && (newVat > 0 || (newVat == 0 && vats[1] == "0"));
                            if (isAdd)
                            {
                                dicVatConvert[oldVat] = newVat;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private static string GetValue(string key)
        {
            try
            {
                return HisConfigs.Get<string>(key);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return "";
        }

        internal static List<V_HIS_NONE_MEDI_SERVICE> V_HIS_NONE_MEDI_SERVICEs
        {
            get
            {
                List<V_HIS_NONE_MEDI_SERVICE> result = new List<V_HIS_NONE_MEDI_SERVICE>();
                try
                {
                    result = BackendDataWorker.Get<V_HIS_NONE_MEDI_SERVICE>();
                }
                catch (Exception ex)
                {
                    result = new List<V_HIS_NONE_MEDI_SERVICE>();
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }

                if (result == null)
                {
                    result = new List<V_HIS_NONE_MEDI_SERVICE>();
                }

                return result;
            }
        }

        internal static List<V_HIS_MATERIAL_TYPE> V_HIS_MATERIAL_TYPEs
        {
            get
            {
                List<V_HIS_MATERIAL_TYPE> result = new List<V_HIS_MATERIAL_TYPE>();
                try
                {
                    result = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>();
                }
                catch (Exception ex)
                {
                    result = new List<V_HIS_MATERIAL_TYPE>();
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }

                if (result == null)
                {
                    result = new List<V_HIS_MATERIAL_TYPE>();
                }

                return result;
            }
        }

        internal static List<V_HIS_MEDICINE_TYPE> V_HIS_MEDICINE_TYPEs
        {
            get
            {
                List<V_HIS_MEDICINE_TYPE> result = new List<V_HIS_MEDICINE_TYPE>();
                try
                {
                    result = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>();
                }
                catch (Exception ex)
                {
                    result = new List<V_HIS_MEDICINE_TYPE>();
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }

                if (result == null)
                {
                    result = new List<V_HIS_MEDICINE_TYPE>();
                }

                return result;
            }
        }
    }
}
