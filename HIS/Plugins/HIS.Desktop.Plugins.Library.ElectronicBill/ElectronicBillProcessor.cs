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
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Plugins.Library.ElectronicBill.Base;
using HIS.Desktop.Plugins.Library.ElectronicBill.Config;
using HIS.Desktop.Plugins.Library.ElectronicBill.Data;
using HIS.Desktop.Plugins.Library.ElectronicBill.ProviderBehavior.BKAV;
using HIS.Desktop.Plugins.Library.ElectronicBill.ProviderBehavior.VIETTEL;
using HIS.Desktop.Plugins.Library.ElectronicBill.ProviderBehavior.VNPT;
using HIS.Desktop.Plugins.Library.ElectronicBill.ProviderBehavior.MOBIFONE;
using HIS.Desktop.Plugins.Library.ElectronicBill.ProxyBehavior.CTO;
using HIS.Desktop.Plugins.Library.ElectronicBill.Template;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Inventec.Core;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.Library.ElectronicBill.ProviderBehavior.CYBERBILL;

namespace HIS.Desktop.Plugins.Library.ElectronicBill
{
    public partial class ElectronicBillProcessor
    {
        ElectronicBillDataInput ElectronicBillDataInput { get; set; }
        TemplateEnum.TYPE TempType { get; set; }

        public ElectronicBillProcessor(ElectronicBillDataInput _electronicBillDataInput, TemplateEnum.TYPE _templateType)
        {
            try
            {
                this.ElectronicBillDataInput = _electronicBillDataInput;
                this.TempType = _templateType;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public ElectronicBillProcessor(ElectronicBillDataInput _electronicBillDataInput)
        {
            try
            {
                this.ElectronicBillDataInput = _electronicBillDataInput;
                this.TempType = TemplateEnum.TYPE.Template1;

                long temCFG = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<long>(Config.HisConfigCFG.His_Desktop_plugins_ElectriconicBill_Template);
                if (temCFG != 1)
                {
                    try
                    {
                        this.TempType = (TemplateEnum.TYPE)temCFG;
                    }
                    catch (Exception)
                    {
                        this.TempType = TemplateEnum.TYPE.Template1;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public ElectronicBillResult Run(ElectronicBillType.ENUM type)
        {
            ElectronicBillResult result = new ElectronicBillResult();

            try
            {
                //xóa dữ liệu
                Base.General.DicDataBuyerInfo = null;

                //Lấy dữ liệu Cấu hình
                string serviceConfig = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HisConfigCFG.ELECTRONIC_BILL__CONFIG);
                string accountConfig = ConfigApplicationWorker.Get<string>(SdaConfigKey.ACCOUNT_CONFIG_KEY);
                if (ElectronicBillDataInput.EinvoiceTypeId.HasValue)
                {
                    switch (ElectronicBillDataInput.EinvoiceTypeId.Value)
                    {
                        case IMSys.DbConfig.HIS_RS.HIS_EINVOICE_TYPE.ID__BKAV:
                            var bkav = BackendDataWorker.Get<HIS_EINVOICE_TYPE>().FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_EINVOICE_TYPE.ID__BKAV);
                            serviceConfig = string.Format("{0}|{1}", ProviderType.BKAV, bkav != null ? bkav.VALUE : "");
                            accountConfig = ConfigApplicationWorker.Get<string>(SdaConfigKey.ACCOUNT_CONFIG_KEY__BKAV);
                            break;
                        case IMSys.DbConfig.HIS_RS.HIS_EINVOICE_TYPE.ID__VIETEL:
                            var vietel = BackendDataWorker.Get<HIS_EINVOICE_TYPE>().FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_EINVOICE_TYPE.ID__VIETEL);
                            serviceConfig = string.Format("{0}|{1}", ProviderType.VIETTEL, vietel != null ? vietel.VALUE : "");
                            accountConfig = ConfigApplicationWorker.Get<string>(SdaConfigKey.ACCOUNT_CONFIG_KEY__VIETEL);
                            break;
                        case IMSys.DbConfig.HIS_RS.HIS_EINVOICE_TYPE.ID__VNPT:
                            var vnpt = BackendDataWorker.Get<HIS_EINVOICE_TYPE>().FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_EINVOICE_TYPE.ID__VNPT);
                            List<string> configTotal = new List<string>();
                            configTotal.Add(ProviderType.VNPT);

                            GetConfigVnpt(vnpt != null ? vnpt.VALUE : "", ElectronicBillDataInput, ref configTotal);

                            serviceConfig = string.Join("|", configTotal);
                            accountConfig = ConfigApplicationWorker.Get<string>(SdaConfigKey.ACCOUNT_CONFIG_KEY__VNPT);
                            break;
                        case IMSys.DbConfig.HIS_RS.HIS_EINVOICE_TYPE.ID__CTO:
                            var cto = BackendDataWorker.Get<HIS_EINVOICE_TYPE>().FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_EINVOICE_TYPE.ID__CTO);
                            serviceConfig = string.Format("{0}|{1}", ProviderType.CTO, cto != null ? cto.VALUE : "");
                            accountConfig = ProviderType.CTO;//gán dữ liệu để bỏ qua check;
                            break;
                        case IMSys.DbConfig.HIS_RS.HIS_EINVOICE_TYPE.ID__MOBIFONE:
                            var mobifone = BackendDataWorker.Get<HIS_EINVOICE_TYPE>().FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_EINVOICE_TYPE.ID__MOBIFONE);
                            serviceConfig = string.Format("{0}|{1}", ProviderType.MOBIFONE, mobifone != null ? mobifone.VALUE : "");
                            accountConfig = ConfigApplicationWorker.Get<string>(SdaConfigKey.ACCOUNT_CONFIG_KEY__MOBIFONE);
                            break;
                        case IMSys.DbConfig.HIS_RS.HIS_EINVOICE_TYPE.ID__CYBERBILL:
                            var cyberbill = BackendDataWorker.Get<HIS_EINVOICE_TYPE>().FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_EINVOICE_TYPE.ID__CYBERBILL);
                            serviceConfig = string.Format("{0}|{1}", ProviderType.CYBERBILL, cyberbill != null ? cyberbill.VALUE : "");
                            accountConfig = ConfigApplicationWorker.Get<string>(SdaConfigKey.ACCOUNT_CONFIG_KEY__CYBERBILL);
                            break;
                        case IMSys.DbConfig.HIS_RS.HIS_EINVOICE_TYPE.ID__SOFTDREAM:
                            var sodr = BackendDataWorker.Get<HIS_EINVOICE_TYPE>().FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_EINVOICE_TYPE.ID__SOFTDREAM);
                            serviceConfig = string.Format("{0}|{1}", ProviderType.SoftDream, sodr != null ? sodr.VALUE : "");
                            accountConfig = ConfigApplicationWorker.Get<string>(SdaConfigKey.ACCOUNT_CONFIG_KEY__SOFTDREAM);
                            break;
                        default:
                            break;
                    }
                }

                HisConfigCFG.LoadConfig();
                GetCurrentPatientTypeAlter();

                if (this.Check(ref result, serviceConfig, accountConfig, type))
                {
                    string provider = GetProvider(serviceConfig);
                    IRun iRun = null;
                    switch (provider)
                    {
                        case ProviderType.VNPT:
                            iRun = new VNPTBehavior(this.ElectronicBillDataInput, serviceConfig, accountConfig);
                            break;
                        case ProviderType.VIETSENS:
                            //iRun = new VietSensBehavior(Invoices, sereServ5s, config);
                            break;
                        case ProviderType.BKAV:
                            iRun = new BKAVBehavior(this.ElectronicBillDataInput, serviceConfig, accountConfig);
                            break;
                        case ProviderType.VIETTEL:
                            iRun = new VIETTELBehavior(this.ElectronicBillDataInput, serviceConfig, accountConfig);
                            break;
                        case ProviderType.CongThuong:
                            iRun = new ProviderBehavior.MOIT.VOITBehavior(this.ElectronicBillDataInput, serviceConfig, accountConfig);
                            break;
                        case ProviderType.SoftDream:
                            iRun = new ProviderBehavior.SODR.SODRBehavior(this.ElectronicBillDataInput, serviceConfig, accountConfig, ElectronicBillDataInput.EinvoiceTypeId.HasValue);
                            break;
                        case ProviderType.MISA:
                            iRun = new ProviderBehavior.MISA.MISABehavior(this.ElectronicBillDataInput, serviceConfig, accountConfig);
                            break;
                        case ProviderType.safecert:
                            iRun = new ProviderBehavior.SAFECERT.SAFECERTBehavior(this.ElectronicBillDataInput, serviceConfig, accountConfig);
                            break;
                        case ProviderType.CTO:
                            iRun = new ProxyBehavior.CTO.CTOBehavior(this.ElectronicBillDataInput, serviceConfig);
                            break;
                        case ProviderType.BACH_MAI:
                            iRun = new ProxyBehavior.BACHMAI.BACHMAIBehavior(this.ElectronicBillDataInput, serviceConfig, accountConfig);
                            break;
                        case ProviderType.MOBIFONE:
                            iRun = new MIBIFONEBehavior(this.ElectronicBillDataInput, serviceConfig, accountConfig);
                            break;
                        case ProviderType.CYBERBILL:
                            iRun = new CYBERBILLBehavior(this.ElectronicBillDataInput, serviceConfig, accountConfig);
                            break;
                        default:
                            break;
                    }

                    result = iRun != null ? iRun.Run(type, TempType) : null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void GetCurrentPatientTypeAlter()
        {
            try
            {
                if (this.ElectronicBillDataInput.LastPatientTypeAlter == null)
                {
                    if (this.ElectronicBillDataInput != null && this.ElectronicBillDataInput.Treatment != null && this.ElectronicBillDataInput.Treatment.ID > 0)
                    {
                        CommonParam param = new CommonParam();
                        this.ElectronicBillDataInput.LastPatientTypeAlter = new BackendAdapter(param).Get<V_HIS_PATIENT_TYPE_ALTER>("/api/HisPatientTypeAlter/GetViewLastByTreatmentId", ApiConsumers.MosConsumer, this.ElectronicBillDataInput.Treatment.ID, param);
                    }
                    else //không có treatmentid là thanh toán khác.
                    {
                        this.ElectronicBillDataInput.LastPatientTypeAlter = new V_HIS_PATIENT_TYPE_ALTER();
                        //khám để hiển thị toàn bộ
                        this.ElectronicBillDataInput.LastPatientTypeAlter.TREATMENT_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM;
                        this.ElectronicBillDataInput.LastPatientTypeAlter.PATIENT_TYPE_ID = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetConfigVnpt(string config, ElectronicBillDataInput dataInput, ref List<string> configTotal)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(config))
                {
                    string[] cfg = config.Split('|');
                    for (int i = 0; i < cfg.Length; i++)
                    {
                        configTotal.Add(cfg[i].Trim());
                        if (i == 2)
                        {
                            configTotal.Add(dataInput.TemplateCode);
                            configTotal.Add(dataInput.SymbolCode);
                        }
                    }

                    if (configTotal.Count < 7)
                    {
                        for (int i = 0; i < 7 - configTotal.Count; i++)
                        {
                            configTotal.Add("0");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public static bool GetInvoiceInfo(ElectronicBillDataInput _electronicBillDataInput, ref string invoiceSys, ref string invoiceCode, ref string errorMess)
        {
            bool result = false;
            try
            {
                string serviceConfig = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HisConfigCFG.ELECTRONIC_BILL__CONFIG);

                if (_electronicBillDataInput.EinvoiceTypeId.HasValue)
                {
                    switch (_electronicBillDataInput.EinvoiceTypeId.Value)
                    {
                        case IMSys.DbConfig.HIS_RS.HIS_EINVOICE_TYPE.ID__BKAV:
                            var bkav = BackendDataWorker.Get<HIS_EINVOICE_TYPE>().FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_EINVOICE_TYPE.ID__BKAV);
                            serviceConfig = string.Format("{0}|{1}", ProviderType.BKAV, bkav != null ? bkav.VALUE : "");
                            break;
                        case IMSys.DbConfig.HIS_RS.HIS_EINVOICE_TYPE.ID__VIETEL:
                            var vietel = BackendDataWorker.Get<HIS_EINVOICE_TYPE>().FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_EINVOICE_TYPE.ID__VIETEL);
                            serviceConfig = string.Format("{0}|{1}", ProviderType.VIETTEL, vietel != null ? vietel.VALUE : "");
                            break;
                        case IMSys.DbConfig.HIS_RS.HIS_EINVOICE_TYPE.ID__VNPT:
                            var vnpt = BackendDataWorker.Get<HIS_EINVOICE_TYPE>().FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_EINVOICE_TYPE.ID__VNPT);
                            serviceConfig = string.Format("{0}|{1}", ProviderType.VNPT, vnpt != null ? vnpt.VALUE : "");
                            break;
                        default:
                            break;
                    }
                }

                string provider = GetProvider(serviceConfig);
                switch (provider)
                {
                    case ProviderType.VNPT:
                        invoiceSys = ProviderType.VNPT;
                        invoiceCode = _electronicBillDataInput.InvoiceCode;
                        result = true;
                        break;
                    case ProviderType.BKAV:
                        invoiceSys = ProviderType.BKAV;
                        invoiceCode = _electronicBillDataInput.InvoiceCode;
                        result = true;
                        break;
                    case ProviderType.VIETTEL:
                        if (!String.IsNullOrWhiteSpace(_electronicBillDataInput.TemplateCode) && !String.IsNullOrWhiteSpace(_electronicBillDataInput.SymbolCode))
                        {
                            invoiceSys = ProviderType.VIETTEL;
                            invoiceCode = string.Format("{0}|{1}", _electronicBillDataInput.InvoiceCode, GetNumOrder(_electronicBillDataInput.ENumOrder));
                            result = true;
                        }
                        else
                        {
                            if (errorMess == null) errorMess = "";

                            if (String.IsNullOrWhiteSpace(_electronicBillDataInput.TemplateCode) || String.IsNullOrWhiteSpace(_electronicBillDataInput.SymbolCode))
                            {
                                errorMess += "Giao dịch hiện tại thiếu thông tin ";

                                if (String.IsNullOrWhiteSpace(_electronicBillDataInput.TemplateCode))
                                {
                                    errorMess += " mẫu số,";
                                }

                                if (String.IsNullOrWhiteSpace(_electronicBillDataInput.SymbolCode))
                                {
                                    errorMess += " ký hiệu.";
                                }
                            }
                        }
                        break;
                    case ProviderType.CongThuong:
                        invoiceSys = ProviderType.CongThuong;
                        invoiceCode = _electronicBillDataInput.InvoiceCode;
                        result = true;
                        break;
                    case ProviderType.SoftDream:
                        invoiceSys = ProviderType.SoftDream;
                        invoiceCode = _electronicBillDataInput.InvoiceCode;
                        result = true;
                        break;
                    case ProviderType.MISA:
                        invoiceSys = ProviderType.MISA;
                        invoiceCode = _electronicBillDataInput.InvoiceCode;
                        result = true;
                        break;
                    case ProviderType.safecert:
                        invoiceSys = ProviderType.safecert;
                        invoiceCode = _electronicBillDataInput.InvoiceCode;
                        result = true;
                        break;
                    case ProviderType.CTO:
                        invoiceSys = ProviderType.CTO;
                        invoiceCode = _electronicBillDataInput.InvoiceCode;
                        result = true;
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private bool Check(ref ElectronicBillResult electronicBillResult, string serviceConfig, string accountConfig, ElectronicBillType.ENUM type)
        {
            bool result = true;
            try
            {
                if (String.IsNullOrEmpty(serviceConfig))
                {
                    Inventec.Common.Logging.LogSystem.Error("Khong tim thay thong tin cau hinh serviceConfig");
                    result = false;
                    ElectronicBillResultUtil.Set(ref electronicBillResult, false, "Không tìm thấy thông tin cấu hình serviceConfig");
                }
                else if (String.IsNullOrEmpty(accountConfig) && !serviceConfig.Contains(ProviderType.safecert))
                {
                    Inventec.Common.Logging.LogSystem.Error("Khong tim thay thong tin cau hinh tai khoan");
                    result = false;
                    ElectronicBillResultUtil.Set(ref electronicBillResult, false, "Không tìm thấy thông tin cấu hình tài khoản");
                }
                //else if (this.TempType == null)
                //{
                //    Inventec.Common.Logging.LogSystem.Error("Khong tim thay thong tin Template");
                //    result = false;
                //    ElectronicBillResultUtil.Set(ref electronicBillResult, false, "Không tìm xác định được Template");
                //}
                else if (type == ElectronicBillType.ENUM.CREATE_INVOICE && ElectronicBillDataInput.Transaction == null && (ElectronicBillDataInput.ListTransaction == null || ElectronicBillDataInput.ListTransaction.Count == 0))
                {
                    Inventec.Common.Logging.LogSystem.Error("Khong tim thay thong tin giao dich");
                    result = false;
                    ElectronicBillResultUtil.Set(ref electronicBillResult, false, "Không xác định được giao dịch");
                }
                else if (ElectronicBillDataInput.ListTransaction != null && ElectronicBillDataInput.ListTransaction.Count > 0)
                {
                    var groupTreatment = ElectronicBillDataInput.ListTransaction.GroupBy(o => o.TREATMENT_ID).ToList();
                    if (groupTreatment.Count > 1)
                    {
                        Inventec.Common.Logging.LogSystem.Error("Giao dich thuoc nhieu ho so dieu tri: " + string.Join(", ", groupTreatment.Select(s => s.Key)));
                        result = false;
                        ElectronicBillResultUtil.Set(ref electronicBillResult, false, "Các giao dịch không cùng hồ sơ điều trị");
                    }

                    var replaceTransaction = ElectronicBillDataInput.ListTransaction.Where(o => o.ORIGINAL_TRANSACTION_ID.HasValue).ToList();
                    if (replaceTransaction != null && replaceTransaction.Count > 0)
                    {
                        Inventec.Common.Logging.LogSystem.Error("Giao dich la thay the: " + string.Join(", ", replaceTransaction.Select(s => s.TRANSACTION_CODE)));
                        result = false;
                        ElectronicBillResultUtil.Set(ref electronicBillResult, false, string.Format("Giao dịch {0} là giao dịch thay thế không cho phép gộp", string.Join(", ", replaceTransaction.Select(s => s.TRANSACTION_CODE))));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private static string GetProvider(string config)
        {
            string result = null;
            try
            {
                if (String.IsNullOrEmpty(config))
                    throw new Exception("Khong tim thay thong tin cau hinh. Vui long kiem tra key cau hinh : HIS.DESKTOP.LIBRARY.ELECTRONIC_BILL.CONFIG");

                string[] configArr = config.Split('|');
                if (!ProviderType.TYPE.Contains(configArr[0].Trim()))
                    throw new Exception("Khong tim thay nha cung cap dich vu tu cau hinh. Vui long kiem tra key cau hinh : HIS.DESKTOP.LIBRARY.ELECTRONIC_BILL.CONFIG");

                result = configArr[0].Trim();
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private static string GetNumOrder(string p)
        {
            string result = p;
            try
            {
                if (!String.IsNullOrWhiteSpace(p))
                {
                    int startIndex = 0;
                    int length = p.Length;
                    if (p.Length > 7)
                    {
                        startIndex = p.Length - 7;
                        length = 7;
                    }

                    result = p.Substring(startIndex, length);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
