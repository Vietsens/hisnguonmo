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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HIS.Desktop.Plugins.Library.ElectronicBill.Base;
using HIS.Desktop.Plugins.Library.ElectronicBill.Template;
using HIS.Desktop.Plugins.Library.ElectronicBill.ProxyBehavior.BACHMAI.Model;
using HIS.Desktop.Plugins.Library.ElectronicBill.Data;
using Inventec.Common.Logging;
using System.Reflection;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using Inventec.Core;
using MOS.Filter;
using HIS.Desktop.ApiConsumer;

namespace HIS.Desktop.Plugins.Library.ElectronicBill.ProxyBehavior.BACHMAI
{
    class BACHMAIBehavior : IRun
    {
        private ElectronicBillDataInput ElectronicBillDataInput { get; set; }
        private string proxyUrl { get; set; }
        private string ServiceConfig;
        private string AccountConfig;

        private const string CreateTotalCode = "itong_hop";
        private const string CreateDetailCode = "ichi_tiet";
        private const string DeleteTotalCode = "dtong_hop";
        private const string DeleteDetailCode = "dchi_tiet";

        private const string CreateInvoice_TT78 = "api/create";
        private const string DeleteInvoice_TT78 = "api/remove";

        public BACHMAIBehavior(ElectronicBillDataInput _electronicBillDataInput, string serviceConfig, string accountConfig)
            : base()
        {
            this.ElectronicBillDataInput = _electronicBillDataInput;
            this.ServiceConfig = serviceConfig;
            this.AccountConfig = accountConfig;
        }

        ElectronicBillResult IRun.Run(ElectronicBillType.ENUM _electronicBillTypeEnum, TemplateEnum.TYPE templateType)
        {
            ElectronicBillResult result = new ElectronicBillResult();
            try
            {
                if (this.Check(_electronicBillTypeEnum, ref result))
                {

                    string[] configArr = ServiceConfig.Split('|');

                    string serviceUrl = configArr[1];
                    if (String.IsNullOrEmpty(serviceUrl))
                    {
                        Inventec.Common.Logging.LogSystem.Error("Khong tim thay dia chi Webservice URL");
                        ElectronicBillResultUtil.Set(ref result, false, "Không tìm thấy địa chỉ Webservice URL");
                        return result;
                    }

                    switch (_electronicBillTypeEnum)
                    {
                        case ElectronicBillType.ENUM.CREATE_INVOICE:
                            result = PrcessCreateInvoice(templateType);
                            break;
                        case ElectronicBillType.ENUM.GET_INVOICE_LINK:
                            ElectronicBillResultUtil.Set(ref result, false, "Chưa tích hợp tính năng này");
                            break;
                        case ElectronicBillType.ENUM.DELETE_INVOICE:
                        case ElectronicBillType.ENUM.CANCEL_INVOICE:
                            result = ProcessCancelInvoice();
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                ElectronicBillResultUtil.Set(ref result, false, ex.Message);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private ElectronicBillResult ProcessCancelInvoice()
        {
            ElectronicBillResult result = new ElectronicBillResult();

            string[] configArr = ServiceConfig.Split('|');

            string version = "";
            if (configArr.Count() > 2)
            {
                version = configArr[2];
            }

            if (version == "2")
            {
                E_BM_Data delData = new E_BM_Data();
                delData.ID_MASTER = ElectronicBillDataInput.TransactionCode;
                var dtong_hop = ApiConsumer.CreateRequest<object>(configArr[1], this.AccountConfig, DeleteInvoice_TT78, "", delData);

                result.Success = true;
                result.InvoiceSys = ProviderType.BACH_MAI;
            }
            else
            {
                List<object> delData = new List<object> { ElectronicBillDataInput.TransactionCode };

                var dchi_tiet = ApiConsumer.CreateRequest<List<object>>(configArr[1], this.AccountConfig, "", DeleteDetailCode, delData);

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dchi_tiet), dchi_tiet));

                var dtong_hop = ApiConsumer.CreateRequest<List<object>>(configArr[1], this.AccountConfig, "", DeleteTotalCode, delData);

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dtong_hop), dtong_hop));

                result.Success = true;
                result.InvoiceSys = ProviderType.BACH_MAI;
            }

            return result;
        }

        private bool Check(ElectronicBillType.ENUM _electronicBillTypeEnum, ref ElectronicBillResult result)
        {
            bool valid = true;
            try
            {
                if (this.ElectronicBillDataInput == null)
                    throw new Exception("Không có dữ liệu phát hành");

                string[] configArr = ServiceConfig.Split('|');
                if (configArr.Length < 2)
                    throw new Exception("Sai định dạng cấu hình hệ thống.");

                if (String.IsNullOrWhiteSpace(AccountConfig))
                    throw new Exception("Sai cấu hình tài khoản.");

                if (_electronicBillTypeEnum == ElectronicBillType.ENUM.CREATE_INVOICE)
                {
                    if (this.ElectronicBillDataInput == null)
                        throw new Exception("Không có dữ liệu phát hành hóa đơn.");
                    if (this.ElectronicBillDataInput.Treatment == null)
                        throw new Exception("Không có thông tin hồ sơ điều trị.");
                    if (this.ElectronicBillDataInput.Transaction == null && (this.ElectronicBillDataInput.ListTransaction == null || this.ElectronicBillDataInput.ListTransaction.Count == 0))
                        throw new Exception("Không có dữ liệu hóa đơn");
                    if (this.ElectronicBillDataInput.Transaction == null && this.ElectronicBillDataInput.ListTransaction != null && this.ElectronicBillDataInput.ListTransaction.Count > 0)
                        throw new Exception("Chưa hỗ trợ xuất hóa đơn gộp");
                }
            }
            catch (Exception ex)
            {
                valid = false;
                ElectronicBillResultUtil.Set(ref result, false, ex.Message);
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private ElectronicBillResult PrcessCreateInvoice(TemplateEnum.TYPE templateType)
        {
            ElectronicBillResult result = new ElectronicBillResult();
            try
            {
                if (this.ElectronicBillDataInput != null)
                {
                    string[] configArr = ServiceConfig.Split('|');

                    string idMaster = "";
                    decimal discount = 0;
                    if (this.ElectronicBillDataInput.Transaction != null)
                    {
                        idMaster = this.ElectronicBillDataInput.Transaction.TRANSACTION_CODE;
                        discount = this.ElectronicBillDataInput.Transaction.EXEMPTION ?? 0;
                    }
                    else if (this.ElectronicBillDataInput.ListTransaction != null && this.ElectronicBillDataInput.ListTransaction.Count > 0)
                    {
                        List<string> trasactionCode = this.ElectronicBillDataInput.ListTransaction.Select(s => s.TRANSACTION_CODE).ToList();
                        trasactionCode = trasactionCode.OrderBy(o => o).ToList();

                        idMaster = trasactionCode.FirstOrDefault();
                        discount = this.ElectronicBillDataInput.ListTransaction.Sum(s => s.EXEMPTION ?? 0);
                    }

                    string version = "";
                    if (configArr.Count() > 2)
                    {
                        version = configArr[2];
                    }

                    if (version == "2")
                    {
                        result = ProccessCreateV2(this.ElectronicBillDataInput, templateType, configArr[1], idMaster, discount);
                    }
                    else
                    {
                        result = ProccessCreateV1(this.ElectronicBillDataInput, templateType, configArr[1], idMaster, discount);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        #region V2
        private ElectronicBillResult ProccessCreateV2(Base.ElectronicBillDataInput electronicBillDataInput, TemplateEnum.TYPE templateType, string ulr, string idMaster, decimal discount)
        {
            ElectronicBillResult result = new ElectronicBillResult();

            Tonghop_TT78 invoice = new Tonghop_TT78();
            if (this.ElectronicBillDataInput.Branch != null && !String.IsNullOrWhiteSpace(this.ElectronicBillDataInput.Branch.TAX_CODE))
            {
                invoice.ID_COMPANY = this.ElectronicBillDataInput.Branch.TAX_CODE.Replace("-", "");
            }

            invoice.ID_MASTER = idMaster;
            invoice.SODONHANG = idMaster;
            invoice.ID_MASTER_REF = idMaster;

            InvoiceInfo.InvoiceInfoADO ado = InvoiceInfo.InvoiceInfoProcessor.GetData(this.ElectronicBillDataInput);
            //ado luôn trả ra khác null

            invoice.DIACHI = ado.BuyerAddress ?? "";
            invoice.DIENTHOAINGUOIMUA = ado.BuyerPhone ?? "";
            invoice.TENKHACHHANG = ado.BuyerName;
            invoice.TENDONVI = ado.BuyerOrganization ?? "";
            invoice.MASOTHUE = ado.BuyerTaxCode ?? "";
            invoice.EMAILNGUOIMUA = ado.BuyerEmail ?? "";
            invoice.FAXNGUOIMUA = "";
            invoice.MAKHACHHANG = ado.BuyerCode;

            string paymentName = ado.PaymentMethod ?? "TM/CK";
            if (ElectronicBillDataInput.Transaction != null)
            {
                HIS_PAY_FORM payForm = BackendDataWorker.Get<HIS_PAY_FORM>().FirstOrDefault(o => o.ID == ElectronicBillDataInput.Transaction.PAY_FORM_ID);
                if (payForm != null)
                {
                    paymentName = payForm.ELECTRONIC_PAY_FORM_NAME ?? payForm.PAY_FORM_NAME;
                }
            }

            invoice.HINHTHUCTT = paymentName;

            string ngayGiaoDich = GetDate(ado.TransactionTime);

            invoice.NGAYDONHANG = ngayGiaoDich;

            invoice.NOIMOTAIKHOAN = "";
            invoice.SOTAIKHOAN = "";
            invoice.LOAITIENTE = "VND";
            invoice.TYGIA = 1;
            invoice.TONGTIENCKGG = (long)Math.Round(discount, 0, MidpointRounding.AwayFromZero);

            invoice.MADICHVU = "04";
            invoice.TINHTRANGHOADON = "-3";

            invoice.DIEMTHU = "";
            if (ElectronicBillDataInput.Transaction != null)
            {
                V_HIS_CASHIER_ROOM room = BackendDataWorker.Get<V_HIS_CASHIER_ROOM>().FirstOrDefault(o => o.ID == ElectronicBillDataInput.Transaction.CASHIER_ROOM_ID);
                if (room != null)
                {
                    invoice.DIEMTHU = room.CASHIER_ROOM_CODE ?? "";
                    invoice.MACN_CH = room.CASHIER_ROOM_CODE ?? "";
                    invoice.TENCN_CH = room.CASHIER_ROOM_NAME ?? "";
                }

                invoice.NVTHU = ElectronicBillDataInput.Transaction.CASHIER_USERNAME;
            }

            ProccessRoom(this.ElectronicBillDataInput, ref invoice);

            List<Chitiet_TT78> listEinvoiceLine = this.GetEinvoiceLineV2(templateType, idMaster, this.ElectronicBillDataInput);

            if (invoice != null && listEinvoiceLine != null && listEinvoiceLine.Count > 0)
            {
                decimal tongtien0 = 0;
                decimal tongtienvat5 = 0;
                decimal tongtienchuavat5 = 0;
                decimal tongtienvat10 = 0;
                decimal tongtienchuavat10 = 0;
                decimal tongtienkct = 0;
                decimal tongtienhang = 0;
                decimal tongtienthue = 0;
                decimal tongtientt = 0;
                decimal tongtienchuavatkhac = 0;
                decimal tongtienvatkhac = 0;
                foreach (var item in listEinvoiceLine)
                {
                    if (item.THUESUAT == "5")
                    {
                        tongtienvat5 += item.TIENTHUE;
                        tongtienchuavat5 += item.THANHTIEN;
                    }
                    else if (item.THUESUAT == "10")
                    {
                        tongtienvat10 += item.TIENTHUE;
                        tongtienchuavat10 += item.THANHTIEN;
                    }
                    else if (item.THUESUAT == "8")
                    {
                        tongtienvatkhac += item.TIENTHUE;
                        tongtienchuavatkhac += item.THANHTIEN;
                    }
                    else if (item.THUESUAT == "0")
                    {
                        tongtien0 += item.TONGTIEN;
                    }
                    else if (item.THUESUAT == "KCT")
                    {
                        tongtienkct += item.TONGTIEN;
                    }

                    tongtienhang += item.THANHTIEN;
                    tongtienthue += item.TIENTHUE;
                    tongtientt += item.TONGTIEN;
                }

                //gán lại bằng 0 với hóa đơn thường ko chịu thuế
                if (tongtienvat5 == 0 && tongtienvat10 == 0)
                {
                    tongtien0 = 0;
                }

                invoice.TONGTIEN0 = (long)Math.Round(tongtien0, 0, MidpointRounding.AwayFromZero);
                invoice.TONGTIENVAT5 = (long)Math.Round(tongtienvat5, 0, MidpointRounding.AwayFromZero);
                invoice.TONGTIENCHUAVAT5 = (long)Math.Round(tongtienchuavat5, 0, MidpointRounding.AwayFromZero);
                invoice.TONGTIENVAT10 = (long)Math.Round(tongtienvat10, 0, MidpointRounding.AwayFromZero);
                invoice.TONGTIENCHUAVAT10 = (long)Math.Round(tongtienchuavat10, 0, MidpointRounding.AwayFromZero);
                invoice.TONGTIENVATKHAC = (long)Math.Round(tongtienvatkhac, 0, MidpointRounding.AwayFromZero);
                invoice.TONGTIENCHUAVATKHAC = (long)Math.Round(tongtienchuavatkhac, 0, MidpointRounding.AwayFromZero);
                invoice.TONGTIENKCT = (long)Math.Round(tongtienkct, 0, MidpointRounding.AwayFromZero);
                invoice.TONGTIENHANG = (long)Math.Round(tongtienhang, 0, MidpointRounding.AwayFromZero);
                invoice.TONGTIENTHUE = (long)Math.Round(tongtienthue, 0, MidpointRounding.AwayFromZero);
                invoice.TONGTIENTT = (long)Math.Round(tongtientt, 0, MidpointRounding.AwayFromZero);

                invoice.SOTIENBANGCHU = Inventec.Common.String.Convert.CurrencyToVneseString(String.Format("{0:0.##}", invoice.TONGTIENTT)) + "đồng";
            }

            E_BM_Invoice_TT78 data = new E_BM_Invoice_TT78();
            data.tonghop = invoice;
            data.chitiet = listEinvoiceLine;

            try
            {
                var apiresult = ApiConsumer.CreateRequest<SendDataADO>(ulr, this.AccountConfig, CreateInvoice_TT78, "", data);
                if (apiresult != null && apiresult.data != null)
                {
                    result.Success = true;
                    result.InvoiceSys = ProviderType.BACH_MAI;
                    result.InvoiceCode = string.Format("{0}|{1}", ProviderType.BACH_MAI, this.ElectronicBillDataInput.Transaction.NUM_ORDER);
                    result.InvoiceNumOrder = "";
                    result.InvoiceTime = Inventec.Common.DateTime.Get.Now();
                }
                else
                {
                    ElectronicBillResultUtil.Set(ref result, false, "");
                    LogSystem.Error("Tao hoa don dien tu that bai. " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => invoice), invoice));
                }
            }
            catch (Exception ex)
            {
                ElectronicBillResultUtil.Set(ref result, false, ex.Message);
                LogSystem.Error("Tao hoa don dien tu that bai. " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => invoice), invoice));
            }

            return result;
        }

        private void ProccessRoom(ElectronicBillDataInput inputData, ref Tonghop_TT78 tongHop)
        {
            try
            {
                if (inputData.Treatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM)
                {
                    var endRoom = inputData.Treatment.END_ROOM_ID.HasValue ? BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == inputData.Treatment.END_ROOM_ID) : null;
                    if (endRoom != null)
                    {
                        tongHop.MAKHOA = endRoom.ROOM_CODE;
                        tongHop.TENKHOA = endRoom.ROOM_NAME;
                    }
                    else
                    {
                        HisSereServFilter ssFilter = new HisSereServFilter();
                        ssFilter.TREATMENT_ID = inputData.Treatment.ID;
                        ssFilter.TDL_SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH;
                        var sereServKham = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, ssFilter, null);
                        if (sereServKham != null && sereServKham.Count > 0)
                        {
                            sereServKham = sereServKham.OrderByDescending(o => o.TDL_IS_MAIN_EXAM ?? 0).ThenByDescending(o => o.TDL_INTRUCTION_TIME).ToList();
                            var exeRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == sereServKham.First().TDL_EXECUTE_ROOM_ID);
                            if (exeRoom != null)
                            {
                                tongHop.MAKHOA = exeRoom.ROOM_CODE;
                                tongHop.TENKHOA = exeRoom.ROOM_NAME;
                            }
                        }
                    }
                }
                else if (inputData.Treatment.LAST_DEPARTMENT_ID.HasValue)
                {
                    var department = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == inputData.Treatment.LAST_DEPARTMENT_ID.Value);
                    if (department != null)
                    {
                        tongHop.MAKHOA = department.DEPARTMENT_CODE;
                        tongHop.TENKHOA = department.DEPARTMENT_NAME;
                    }
                }
                else
                {
                    HisDepartmentTranLastFilter filter = new HisDepartmentTranLastFilter();
                    filter.TREATMENT_ID = inputData.Treatment.ID;
                    V_HIS_DEPARTMENT_TRAN tran = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<V_HIS_DEPARTMENT_TRAN>("api/HisDepartmentTran/GetLastByTreatmentId", ApiConsumers.MosConsumer, filter, null);
                    if (tran != null)
                    {
                        tongHop.MAKHOA = tran.DEPARTMENT_CODE;
                        tongHop.TENKHOA = tran.DEPARTMENT_NAME;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<Chitiet_TT78> GetEinvoiceLineV2(TemplateEnum.TYPE templateType, string idMaster, ElectronicBillDataInput electronicBillDataInput)
        {
            List<Chitiet_TT78> result = new List<Chitiet_TT78>();
            try
            {
                //hóa đơn thường không tạo chi tiết theo cấu hình template
                if (templateType != TemplateEnum.TYPE.TemplateNhaThuoc)
                {
                    TemplateFactory.ProcessDataSereServToSereServBill(templateType, ref electronicBillDataInput);
                    IRunTemplate iRunTemplate = TemplateFactory.MakeIRun(templateType, electronicBillDataInput);
                    var listProduct = iRunTemplate.Run();
                    if (listProduct == null)
                    {
                        throw new Exception("Không có thông tin chi tiết dịch vụ.");
                    }

                    List<ProductBase> listProductBase = (List<ProductBase>)listProduct;

                    int count = 1;
                    int countChiTieu1 = 1;
                    int countChiTieu2 = 1;
                    foreach (var item in listProductBase)
                    {
                        Chitiet_TT78 line = new Chitiet_TT78();
                        line.ID_MASTER = idMaster;
                        line.MAHANG = item.ProdCode ?? "";
                        line.TENHANG = item.ProdName ?? "";

                        line.DONVITINH = item.ProdUnit ?? "";
                        line.SOTHUTU = count + "";
                        line.SOLUONG = Math.Round(item.ProdQuantity ?? 1, 4, MidpointRounding.AwayFromZero);
                        line.DONGIA = Math.Round(item.ProdPrice ?? 0, 4, MidpointRounding.AwayFromZero);

                        line.THANHTIEN = Math.Round(item.Amount, 4, MidpointRounding.AwayFromZero);
                        line.TONGTIEN = Math.Round(item.Amount, 4, MidpointRounding.AwayFromZero);

                        line.THUESUAT = "KCT";
                        if (item.IsBHYT)
                        {
                            line.IDSTT = countChiTieu1;
                            line.CHITIEU1 = "TRA_BHYT";
                            line.MUCBHYTTRA = 1;
                            line.CHITIEU2 = "1." + countChiTieu1;
                            countChiTieu1++;
                        }
                        else
                        {
                            line.IDSTT = countChiTieu2;
                            line.CHITIEU1 = "TRA_100";
                            line.MUCBHYTTRA = 0;
                            line.CHITIEU2 = "2." + countChiTieu2;
                            countChiTieu2++;
                        }
                        result.Add(line);
                    }
                }
                else
                {
                    IRunTemplate iRunTemplate = TemplateFactory.MakeIRun(templateType, this.ElectronicBillDataInput);
                    var listProduct = iRunTemplate.Run();
                    if (listProduct == null)
                    {
                        throw new Exception("Loi phan tich listProductBase");
                    }

                    List<ProductBasePlus> listProductBase = (List<ProductBasePlus>)listProduct;
                    int count = 1;
                    foreach (var item in listProductBase)
                    {
                        Chitiet_TT78 line = new Chitiet_TT78();
                        line.ID_MASTER = idMaster;
                        line.MAHANG = item.ProdCode;
                        line.TENHANG = item.ProdName;
                        line.DONVITINH = item.ProdUnit;
                        line.IDSTT = count;
                        line.SOTHUTU = count + "";
                        if (item.ProdQuantity.HasValue)
                        {
                            line.SOLUONG = Math.Round(item.ProdQuantity.Value, 4, MidpointRounding.AwayFromZero);
                        }

                        if (item.ProdPrice.HasValue)
                        {
                            line.DONGIA = Math.Round(item.ProdPrice.Value, 2, MidpointRounding.AwayFromZero);
                        }

                        line.TONGTIEN = Math.Round(item.Amount, 2, MidpointRounding.AwayFromZero);

                        line.THUETTDB = "0";

                        line.TIENTHUE = Math.Round(item.TaxAmount ?? 0, 2, MidpointRounding.AwayFromZero);
                        line.THANHTIEN = Math.Round(item.AmountWithoutTax ?? 0, 2, MidpointRounding.AwayFromZero);

                        if (item.TaxPercentage == 1)
                        {
                            line.THUESUAT = "5";
                        }
                        else if (item.TaxPercentage == 2)
                        {
                            line.THUESUAT = "10";
                        }
                        else if (item.TaxPercentage == 3)
                        {
                            line.THUESUAT = "8";
                        }
                        else if (item.TaxPercentage == 0)
                        {
                            line.THUESUAT = "0";
                        }
                        if (item.IsBHYT)
                        {
                            line.CHITIEU1 = "TRA_BHYT";
                            line.MUCBHYTTRA = 1;
                        }
                        else
                        {
                            line.CHITIEU1 = "TRA_100";
                            line.MUCBHYTTRA = 0;
                        }
                        line.CHITIEU2 = "1." + count;
                        result.Add(line);
                        count++;
                    }
                }
            }
            catch (Exception ex)
            {
                result = new List<Chitiet_TT78>();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private Base.ElectronicBillDataInput MakeNewData(Base.ElectronicBillDataInput ElectronicBillDataInput, List<HIS_SERE_SERV_BILL> listDetail)
        {
            Base.ElectronicBillDataInput result = new ElectronicBillDataInput();
            result.SereServBill = listDetail;

            if (ElectronicBillDataInput != null)
            {
                result.Amount = listDetail.Sum(s => s.PRICE);
                result.Branch = ElectronicBillDataInput.Branch;
                result.InvoiceCode = ElectronicBillDataInput.InvoiceCode;
                result.IsTransactionList = ElectronicBillDataInput.IsTransactionList;
                result.ListTransaction = ElectronicBillDataInput.ListTransaction;
                result.PartnerInvoiceID = ElectronicBillDataInput.PartnerInvoiceID;
                result.SereServs = ElectronicBillDataInput.SereServs;
                result.SymbolCode = ElectronicBillDataInput.SymbolCode;
                result.TemplateCode = ElectronicBillDataInput.TemplateCode;
                result.Transaction = ElectronicBillDataInput.Transaction;
                result.Treatment = ElectronicBillDataInput.Treatment;
                result.LastPatientTypeAlter = ElectronicBillDataInput.LastPatientTypeAlter;
            }
            return result;
        }
        #endregion

        #region V1
        private ElectronicBillResult ProccessCreateV1(Base.ElectronicBillDataInput electronicBillDataInput, TemplateEnum.TYPE templateType, string url, string idMaster, decimal discount)
        {
            ElectronicBillResult result = new ElectronicBillResult();

            E_BM_Invoice invoice = new E_BM_Invoice();
            if (this.ElectronicBillDataInput.Branch != null && !String.IsNullOrWhiteSpace(this.ElectronicBillDataInput.Branch.TAX_CODE))
            {
                invoice.id_company = this.ElectronicBillDataInput.Branch.TAX_CODE.Replace("-", "");
            }

            invoice.id_master = idMaster;
            invoice.sodonhang = idMaster;

            InvoiceInfo.InvoiceInfoADO ado = InvoiceInfo.InvoiceInfoProcessor.GetData(this.ElectronicBillDataInput);
            //ado luôn trả ra khác null

            invoice.diachi = ado.BuyerAddress ?? "";
            invoice.dienthoainguoimua = ado.BuyerPhone ?? "";
            invoice.tenkhachhang = ado.BuyerName;
            invoice.tendonvi = ado.BuyerOrganization ?? "";
            invoice.masothue = ado.BuyerTaxCode ?? "";
            invoice.emailnguoimua = ado.BuyerEmail ?? "";
            invoice.faxnguoimua = "";
            invoice.maKhachHang = ado.BuyerCode;

            invoice.hinhthuctt = ado.PaymentMethod ?? "TM/CK";

            string ngayGiaoDich = GetDate(ado.TransactionTime);

            invoice.ngaydonhang = ngayGiaoDich;

            invoice.noimotaikhoan = "";
            invoice.sotaikhoan = "";
            invoice.loaitiente = "VND";
            invoice.tygia = 1;
            invoice.tongtienckgg = (long)Math.Round(discount, 0, MidpointRounding.AwayFromZero);
            invoice.macn_ch = "vietsens";
            invoice.tencn_ch = "Tập đoàn công nghệ VIETSEN";

            List<E_BM_InvoiceDetail> listEinvoiceLine = this.GetEinvoiceLine(templateType, idMaster, this.ElectronicBillDataInput);

            ProcessTongTien(invoice, listEinvoiceLine);

            //số tiền tạm ứng tại thời điểm thanh toán
            if (this.ElectronicBillDataInput.Transaction != null && this.ElectronicBillDataInput.Transaction.KC_AMOUNT.HasValue && this.ElectronicBillDataInput.Transaction.KC_AMOUNT.Value > 0)
            {
                invoice.tamung = (long)((this.ElectronicBillDataInput.Transaction.TREATMENT_DEPOSIT_AMOUNT ?? 0) - (this.ElectronicBillDataInput.Transaction.TREATMENT_REPAY_AMOUNT ?? 0) - (this.ElectronicBillDataInput.Transaction.TREATMENT_TRANSFER_AMOUNT ?? 0));
            }

            if (invoice.tongtientt - invoice.tamung > 0)
            {
                invoice.khachhangconphaitra = invoice.tongtientt - invoice.tamung;
            }
            else if (invoice.tongtientt - invoice.tamung < 0)
            {
                invoice.khachhangnhanlai = invoice.tamung - invoice.tongtientt;
            }

            List<object> dataInvoice = ProcessToObject(invoice);
            List<object> data = new List<object> { new List<object> { dataInvoice } };

            try
            {
                var apiresult = ApiConsumer.CreateRequest<List<object>>(url, this.AccountConfig, "", CreateTotalCode, data);
                if (apiresult != null && apiresult.Count > 0)
                {
                    if (apiresult.First() != null && apiresult.First().ToString() == "200")
                    {
                        List<object> detailInvoice = new List<object>();
                        foreach (var line in listEinvoiceLine)
                        {
                            List<object> dataLine = ProcessToObject(line);
                            detailInvoice.Add(dataLine);
                        }

                        List<object> dataDetail = new List<object> { detailInvoice };
                        var detailResult = ApiConsumer.CreateRequest<List<object>>(url, this.AccountConfig, "", CreateDetailCode, dataDetail);
                        if (detailResult != null && detailResult.Count > 0 && detailResult.First() != null && detailResult.First().ToString() == "200")
                        {
                            result.Success = true;
                            result.InvoiceSys = ProviderType.BACH_MAI;
                            result.InvoiceCode = string.Format("{0}|{1}", ProviderType.BACH_MAI, this.ElectronicBillDataInput.Transaction.NUM_ORDER);
                            result.InvoiceNumOrder = "";
                            result.InvoiceTime = Inventec.Common.DateTime.Get.Now();
                        }
                        else
                        {
                            ElectronicBillResultUtil.Set(ref result, false, string.Join(",", apiresult));
                            LogSystem.Error("Tao hoa don dien tu that bai. " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => invoice), invoice));
                            //gọi api xóa hóa đơn
                            List<object> delData = new List<object> { invoice.id_master };
                            var delResult = ApiConsumer.CreateRequest<List<object>>(url, this.AccountConfig, "", DeleteTotalCode, delData);
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => delResult), delResult));
                        }
                    }
                    else
                    {
                        ElectronicBillResultUtil.Set(ref result, false, string.Join(",", apiresult));
                        LogSystem.Error("Tao hoa don dien tu that bai. " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => invoice), invoice));
                    }
                }
                else
                {
                    ElectronicBillResultUtil.Set(ref result, false, "");
                    LogSystem.Error("Tao hoa don dien tu that bai. " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => invoice), invoice));
                }
            }
            catch (Exception ex)
            {
                ElectronicBillResultUtil.Set(ref result, false, ex.Message);
                LogSystem.Error("Tao hoa don dien tu that bai. " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => invoice), invoice));
            }

            return result;
        }

        private List<object> ProcessToObject<T>(T data)
        {
            List<object> result = new List<object>();
            try
            {
                Type type = typeof(T);
                PropertyInfo[] property = type.GetProperties();

                foreach (var item in property)
                {
                    result.Add(item.GetValue(data));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private List<E_BM_InvoiceDetail> GetEinvoiceLine(TemplateEnum.TYPE templateType, string idMaster, ElectronicBillDataInput electronicBillDataInput)
        {
            List<E_BM_InvoiceDetail> result = new List<E_BM_InvoiceDetail>();
            try
            {
                //hóa đơn thường không tạo chi tiết theo cấu hình template
                if (templateType != TemplateEnum.TYPE.TemplateNhaThuoc)
                {
                    TemplateFactory.ProcessDataSereServToSereServBill(templateType, ref electronicBillDataInput);

                    var listBhyt = electronicBillDataInput.SereServBill.Where(o => o.TDL_PATIENT_TYPE_ID == Config.HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT).ToList();
                    var listOther = electronicBillDataInput.SereServBill.Where(o => o.TDL_PATIENT_TYPE_ID != Config.HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT).ToList();
                    if (listBhyt != null && listBhyt.Count > 0)
                    {
                        var groupPrice = listBhyt.GroupBy(o => new { o.TDL_SERVICE_ID, o.TDL_REAL_PRICE }).ToList();
                        int count = 1;
                        foreach (var item in groupPrice)
                        {
                            E_BM_InvoiceDetail line = new E_BM_InvoiceDetail();
                            line.id_master = idMaster;
                            line.mahang = item.First().TDL_SERVICE_CODE;
                            line.tenhang = item.First().TDL_SERVICE_NAME;

                            HIS_SERVICE_UNIT serviceUnit = BackendDataWorker.Get<HIS_SERVICE_UNIT>().FirstOrDefault(o => item.First().TDL_SERVICE_UNIT_ID == o.ID);
                            line.donvitinh = serviceUnit != null ? serviceUnit.SERVICE_UNIT_NAME : "";
                            line.sothutu = count;
                            line.soluong = item.Sum(s => s.TDL_AMOUNT ?? 0);
                            line.dongia = Math.Round(item.First().TDL_REAL_PRICE ?? 0, 4, MidpointRounding.AwayFromZero);

                            line.thanhtien = Math.Round(item.Sum(s => s.PRICE), 4, MidpointRounding.AwayFromZero);
                            line.tongtien = Math.Round(item.Sum(s => s.PRICE), 4, MidpointRounding.AwayFromZero);

                            line.chitieu1 = "TRA_BHYT";
                            line.chitieu2 = "1." + count;
                            line.thuesuat = null;

                            line.mucbhtra = 0;
                            line.bhxhtra = (long)Math.Round(item.Sum(s => s.TDL_TOTAL_PATIENT_PRICE_BHYT ?? 0), 4, MidpointRounding.AwayFromZero);

                            result.Add(line);
                            count++;
                        }
                    }

                    if (listOther != null && listOther.Count > 0)
                    {
                        var groupPrice = listOther.GroupBy(o => new { o.TDL_SERVICE_ID, o.TDL_REAL_PRICE }).ToList();
                        int count = 1;
                        foreach (var item in groupPrice)
                        {
                            E_BM_InvoiceDetail line = new E_BM_InvoiceDetail();
                            line.id_master = idMaster;
                            line.mahang = item.First().TDL_SERVICE_CODE;
                            line.tenhang = item.First().TDL_SERVICE_NAME;

                            HIS_SERVICE_UNIT serviceUnit = BackendDataWorker.Get<HIS_SERVICE_UNIT>().FirstOrDefault(o => item.First().TDL_SERVICE_UNIT_ID == o.ID);
                            line.donvitinh = serviceUnit != null ? serviceUnit.SERVICE_UNIT_NAME : "";
                            line.sothutu = count;
                            line.soluong = Math.Round(item.Sum(s => s.TDL_AMOUNT ?? 0), 4, MidpointRounding.AwayFromZero);
                            line.dongia = Math.Round(item.First().TDL_REAL_PRICE ?? 0, 4, MidpointRounding.AwayFromZero);

                            line.thanhtien = Math.Round(item.Sum(s => s.PRICE), 4, MidpointRounding.AwayFromZero);
                            line.tongtien = Math.Round(item.Sum(s => s.PRICE), 4, MidpointRounding.AwayFromZero);

                            line.chitieu1 = "TRA_100";
                            if (listBhyt == null || listBhyt.Count <= 0)
                            {
                                line.chitieu2 = "1." + count;
                            }
                            else
                            {
                                line.chitieu2 = "2." + count;
                            }

                            line.thuesuat = null;

                            line.mucbhtra = 0;
                            line.bhxhtra = 0;

                            result.Add(line);
                            count++;
                        }
                    }
                }
                else
                {
                    IRunTemplate iRunTemplate = TemplateFactory.MakeIRun(templateType, this.ElectronicBillDataInput);
                    var listProduct = iRunTemplate.Run();
                    if (listProduct == null)
                    {
                        throw new Exception("Loi phan tich listProductBase");
                    }

                    List<ProductBasePlus> listProductBase = (List<ProductBasePlus>)listProduct;
                    int count = 1;
                    foreach (var item in listProductBase)
                    {
                        E_BM_InvoiceDetail line = new E_BM_InvoiceDetail();
                        line.id_master = idMaster;
                        line.mahang = item.ProdCode;
                        line.tenhang = item.ProdName;
                        line.donvitinh = item.ProdUnit;
                        line.sothutu = count;
                        if (item.ProdQuantity.HasValue)
                        {
                            line.soluong = Math.Round(item.ProdQuantity.Value, 4, MidpointRounding.AwayFromZero);
                        }

                        if (item.ProdPrice.HasValue)
                        {
                            line.dongia = Math.Round(item.ProdPrice.Value, 2, MidpointRounding.AwayFromZero);
                        }

                        line.tongtien = Math.Round(item.Amount, 2, MidpointRounding.AwayFromZero);

                        line.thuettdb = 0;

                        line.tienthue = Math.Round(item.TaxAmount ?? 0, 2, MidpointRounding.AwayFromZero);
                        line.thanhtien = Math.Round(item.AmountWithoutTax ?? 0, 2, MidpointRounding.AwayFromZero);

                        if (item.TaxPercentage == 1)
                        {
                            line.thuesuat = "5";
                        }
                        else if (item.TaxPercentage == 2)
                        {
                            line.thuesuat = "10";
                        }
                        else if (item.TaxPercentage == 3)
                        {
                            line.thuesuat = "8";
                        }
                        else if (item.TaxPercentage == 0)
                        {
                            line.thuesuat = "0";
                        }

                        line.chitieu1 = "TRA_100";
                        line.chitieu2 = "1." + count;

                        result.Add(line);
                        count++;
                    }
                }
            }
            catch (Exception ex)
            {
                result = new List<E_BM_InvoiceDetail>();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void ProcessTongTien(E_BM_Invoice invoice, List<E_BM_InvoiceDetail> listEinvoiceLine)
        {
            try
            {
                if (invoice != null && listEinvoiceLine != null && listEinvoiceLine.Count > 0)
                {
                    decimal tongtien0 = 0;
                    decimal tongtienvat5 = 0;
                    decimal tongtienchuavat5 = 0;
                    decimal tongtienvat10 = 0;
                    decimal tongtienchuavat10 = 0;
                    decimal tongtienkct = 0;
                    decimal tongtienhang = 0;
                    decimal tongtienthue = 0;
                    decimal tongtientt = 0;

                    foreach (var item in listEinvoiceLine)
                    {
                        switch (item.thuesuat)
                        {
                            case "0":
                                tongtien0 += item.tongtien;
                                break;
                            case "5":
                                tongtienvat5 += item.tongtien;
                                tongtienchuavat5 += item.thanhtien;
                                break;
                            case "10":
                                tongtienvat10 += item.tongtien;
                                tongtienchuavat10 += item.thanhtien;
                                break;
                            default:
                                tongtienkct += item.tongtien;
                                break;
                        }

                        tongtienhang += item.thanhtien;
                        tongtienthue += item.tienthue;
                        tongtientt += item.tongtien;
                    }

                    invoice.tongtien0 = (long)Math.Round(tongtien0, 0, MidpointRounding.AwayFromZero);
                    invoice.tongtienvat5 = (long)Math.Round(tongtienvat5, 0, MidpointRounding.AwayFromZero);
                    invoice.tongtienchuavat5 = (long)Math.Round(tongtienchuavat5, 0, MidpointRounding.AwayFromZero);
                    invoice.tongtienvat10 = (long)Math.Round(tongtienvat10, 0, MidpointRounding.AwayFromZero);
                    invoice.tongtienchuavat10 = (long)Math.Round(tongtienchuavat10, 0, MidpointRounding.AwayFromZero);
                    invoice.tongtienkct = (long)Math.Round(tongtienkct, 0, MidpointRounding.AwayFromZero);
                    invoice.tongtienhang = (long)Math.Round(tongtienhang, 0, MidpointRounding.AwayFromZero);
                    invoice.tongtienthue = (long)Math.Round(tongtienthue, 0, MidpointRounding.AwayFromZero);
                    invoice.tongtientt = (long)Math.Round(tongtientt, 0, MidpointRounding.AwayFromZero);

                    invoice.sotienbangchu = Inventec.Common.String.Convert.CurrencyToVneseString(String.Format("{0:0.##}", invoice.tongtientt)) + "đồng";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string GetDate(long time)
        {
            string result = "";
            try
            {
                string temp = time.ToString();
                if (temp != null && temp.Length >= 8)
                {
                    result = new StringBuilder().Append(temp.Substring(0, 4)).Append("-").Append(temp.Substring(4, 2)).Append("-").Append(temp.Substring(6, 2)).ToString();
                }
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
        #endregion
    }
}
