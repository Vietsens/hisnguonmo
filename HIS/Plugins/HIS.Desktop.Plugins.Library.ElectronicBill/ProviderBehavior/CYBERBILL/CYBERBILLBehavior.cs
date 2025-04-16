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
using DevExpress.XtraGrid.Views.Layout;
using HIS.Desktop.Plugins.Library.ElectronicBill.Base;
using HIS.Desktop.Plugins.Library.ElectronicBill.Data;
using HIS.Desktop.Plugins.Library.ElectronicBill.ProviderBehavior.CYBERBILL.Model;
using HIS.Desktop.Plugins.Library.ElectronicBill.ProviderBehavior.MOBIFONE.Model;
using HIS.Desktop.Plugins.Library.ElectronicBill.Template;
using Inventec.Common.EBillSoftDreams.Model;
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Library.ElectronicBill.ProviderBehavior.CYBERBILL
{
    public class CYBERBILLBehavior : IRun
    {
        private const string SUCCESS_CODE = "01";
        private LoginResultCyberbill login { get; set; }
        private TemplateEnum.TYPE TempType { get; set; }
        private string serviceConfig { get; set; }
        private string accountConfig { get; set; }
        private string serviceUrl { get; set; }
        ElectronicBillDataInput ElectronicBillDataInput { get; set; }
        private LoginDataCyberbill adoLogin { get; set; }
        private OutputElectronicBill OEBill { get; set; }
        private OutputConvertElectronicBill OCoEBill { get; set; }
        private OutputSignElectronicBill OSEBill { get; set; }
        private OutputCancelElectronicBill OCaEBill { get; set; }
        public CYBERBILLBehavior(Base.ElectronicBillDataInput electronicBillDataInput, string serviceConfig, string accountConfig)
        {
            this.ElectronicBillDataInput = electronicBillDataInput;
            this.serviceConfig = serviceConfig;
            this.accountConfig = accountConfig;
        }
        public ElectronicBillResult Run(ElectronicBillType.ENUM electronicBillType, TemplateEnum.TYPE _templateType)
        {
            ElectronicBillResult result = new ElectronicBillResult();
            try
            {
                if (this.Check(electronicBillType, ref result))
                {
                    this.TempType = _templateType;
                    string[] configArr = serviceConfig.Split('|');
                    serviceUrl = configArr[1];
                    if (String.IsNullOrEmpty(serviceUrl))
                    {
                        Inventec.Common.Logging.LogSystem.Error("Khong tim thay dia chi Webservice URL");
                        ElectronicBillResultUtil.Set(ref result, false, "Không tìm thấy địa chỉ Webservice URL");
                        return result;
                    }
                    string[] accountConfigArr = accountConfig.Split('|');
                    adoLogin = new LoginDataCyberbill();
                    adoLogin.username = accountConfigArr[0].Trim();
                    adoLogin.password = accountConfigArr[1].Trim();
                    adoLogin.doanhnghiep_mst = this.ElectronicBillDataInput.Branch.TAX_CODE;
                    login = ProcessLogin(adoLogin);

                    if (login == null || login.result == null || login.error != null)
                    {

                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => login), login));
                        ElectronicBillResultUtil.Set(ref result, false, login.error.details);
                        return result;
                    }
                    switch (electronicBillType)
                    {
                        case ElectronicBillType.ENUM.CREATE_INVOICE:
                            GuiHoaDonGoc(ref result);
                            KyHoaDon(ref result);
                            break;
                        case ElectronicBillType.ENUM.GET_INVOICE_LINK:
                            ChuyenDoiHoaDon(ref result);
                            break;
                        case ElectronicBillType.ENUM.DELETE_INVOICE:
                            break;
                        case ElectronicBillType.ENUM.CANCEL_INVOICE:
                            HuyHoaDon(ref result);
                            break;
                        case ElectronicBillType.ENUM.CONVERT_INVOICE:
                            CyberbillChuyenDoiHoaDon(ref result);
                            break;
                        case ElectronicBillType.ENUM.CREATE_INVOICE_DATA:
                            break;
                        case ElectronicBillType.ENUM.GET_INVOICE_INFO:
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        #region Login get token
        private LoginResultCyberbill ProcessLogin(LoginDataCyberbill obj)
        {
            LoginResultCyberbill result = null;
            try
            {
                string sendJsonData = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
                result = Base.ApiConsumerV2.CreateRequest<LoginResultCyberbill>(System.Net.WebRequestMethods.Http.Post, serviceUrl, Base.RequestUriStore.CyberbillLogin, null, null, sendJsonData);
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
        #endregion

        #region Gửi hóa đơn góc
        private InputElectronicBill IEBill()
        {
            var inv = InvoiceInfo.InvoiceInfoProcessor.GetData(ElectronicBillDataInput);
            InputElectronicBill hd = new InputElectronicBill();
            hd.doanhnghiep_mst = ElectronicBillDataInput.Branch.TAX_CODE;
            hd.loaihoadon_ma = ElectronicBillDataInput.TemplateCode;
            hd.mauso = ElectronicBillDataInput.TemplateCode;
            hd.ma_tracuu = "";
            hd.kyhieu = ElectronicBillDataInput.SymbolCode;
            hd.sophieu = "";
            hd.ma_hoadon = ElectronicBillDataInput.Transaction.TRANSACTION_CODE;
            hd.ngaylap = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            hd.dnmua_mst = inv.BuyerTaxCode;
            hd.dnmua_ten = inv.BuyerOrganization;
            hd.dnmua_tennguoimua = inv.BuyerName;
            hd.dnmua_diachi = inv.BuyerAddress;
            hd.dnmua_sdt = inv.BuyerPhone;
            hd.dnmua_email = inv.BuyerEmail;
            hd.thanhtoan_phuongthuc = 3;
            hd.thanhtoan_phuongthuc_ten = "Tiền mặt/Chuyển khoản";
            hd.thanhtoan_taikhoan = inv.BuyerAccountNumber;
            hd.thanhtoan_nganhang = "";
            hd.tiente_ma = "";
            hd.tygiangoaite = 0;
            hd.tongtien_chietkhau = 0;
            hd.ghichu = "";
            hd.tienthue = 0;
            hd.nguoilap = adoLogin.username;
            hd.khachhang_ma = ElectronicBillDataInput.Transaction.TDL_PATIENT_CODE;
            hd.matracuuhtkhac = ElectronicBillDataInput.Transaction.TRANSACTION_CODE;//hd.is_download_file = true;
            hd.dschitiet = DSCT();

            hd.dsthuesuat = new List<DanhSachThue>();
            List<DanhSachThue> dsthuesuat = new List<DanhSachThue>();
            if (hd.dschitiet != null && hd.dschitiet.Count > 0)
            {
                hd.tongtien_chuavat = hd.dschitiet.Sum(o => o.tongtien_chuathue ?? 0);
                hd.tongtien_covat = hd.dschitiet.Sum(o => o.tongtien_cothue ?? 0);
                var groupByMaThue = hd.dschitiet.GroupBy(o => o.mathue).ToList();
                foreach (var item in groupByMaThue)
                {
                    DanhSachThue st = new DanhSachThue();
                    st.mathue = item.First().mathue;
                    st.tongtien_chiuthue = item.Sum(o => o.tongtien_chuathue ?? 0);
                    st.tongtien_thue = item.Sum(o => o.tongtien_cothue ?? 0) - item.Sum(o => o.tongtien_chuathue ?? 0);
                    dsthuesuat.Add(st);
                }
            }
            hd.dsthuesuat = dsthuesuat;
            return hd;
        }

        private List<DanhSachChiTiet> DSCT()
        {
            List<DanhSachChiTiet> dschitiet = new List<DanhSachChiTiet>();
            try
            {
                IRunTemplate iRunTemplate = TemplateFactory.MakeIRun(this.TempType, ElectronicBillDataInput);
                var listProduct = iRunTemplate.Run();
                if (listProduct == null)
                {
                    throw new Exception("Loi phan tich listProductBase");
                }
                if (this.TempType == TemplateEnum.TYPE.TemplateNhaThuoc)
                {
                    var lstProductBasePlus = (List<ProductBasePlus>)listProduct;
                    int count = 1;
                    foreach (var item in lstProductBasePlus)
                    {
                        DanhSachChiTiet ddt = new DanhSachChiTiet();
                        ddt.stt = count++;
                        ddt.hanghoa_loai = 0;
                        ddt.khuyenmai = 0;
                        ddt.ma = item.ProdCode;
                        ddt.ten = item.ProdName;
                        ddt.donvitinh = item.ProdUnit;
                        if (item.ProdQuantity.HasValue)
                        {
                            ddt.soluong = Math.Round(item.ProdQuantity.Value, 0, MidpointRounding.AwayFromZero);
                        }
                        if (item.ProdPrice.HasValue)
                        {
                            ddt.dongia = Math.Round(item.ProdPrice.Value, 0, MidpointRounding.AwayFromZero);
                        }
                        ddt.phantram_chietkhau = 0;
                        ddt.tongtien_chietkhau = 0;
                        //ddt.phikhac_tyle = 0;
                        //ddt.phikhac_sotien = 0;
                        ddt.tongtien_chuathue = item.AmountWithoutTax;
                        if (!item.TaxPercentage.HasValue)
                        {
                            ddt.mathue = "-1";
                        }
                        else if (item.TaxPercentage == 1)
                        {
                            ddt.mathue = "5";
                        }
                        else if (item.TaxPercentage == 2)
                        {
                            ddt.mathue = "10";
                        }
                        else if (item.TaxPercentage == 3)
                        {
                            ddt.mathue = "8";
                        }
                        else if (item.TaxPercentage == 0)
                        {
                            ddt.mathue = "0";
                        }
                        ddt.tongtien_cothue = item.Amount;
                        ddt.tyletinhthue = 0;
                        dschitiet.Add(ddt);
                    }
                }
                else
                {
                    int count = 1;
                    var result = (List<ProductBase>)listProduct;
                    foreach (var item in result)
                    {
                        DanhSachChiTiet ddt = new DanhSachChiTiet();
                        ddt.stt = count++;
                        ddt.hanghoa_loai = 1;
                        ddt.khuyenmai = 0;
                        ddt.ma = item.ProdCode;
                        ddt.ten = item.ProdName;
                        ddt.donvitinh = item.ProdUnit;
                        ddt.soluong = item.ProdQuantity;
                        ddt.dongia = item.ProdPrice;
                        ddt.phantram_chietkhau = 0;
                        ddt.tongtien_chietkhau = 0;
                        //ddt.phikhac_tyle = 0;
                        //ddt.phikhac_sotien = 0;
                        ddt.tongtien_chuathue = item.Amount;
                        ddt.mathue = "-1";
                        ddt.tongtien_cothue = item.Amount;
                        ddt.tyletinhthue = 0;
                        dschitiet.Add(ddt);
                    }
                }
            }
            catch (Exception ex)
            {
                dschitiet = new List<DanhSachChiTiet>();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return dschitiet;
        }
        private void GuiHoaDonGoc(ref ElectronicBillResult result)
        {

            try
            {
                string sendJsonData = Newtonsoft.Json.JsonConvert.SerializeObject(IEBill());
                OEBill = Base.ApiConsumerV2.CreateRequest<OutputElectronicBill>(System.Net.WebRequestMethods.Http.Post, serviceUrl, Base.RequestUriStore.CyberbillGuiHoadonGoc, login.result.access_token, sendJsonData);
                result.InvoiceSys = ProviderType.CYBERBILL;
                if (OEBill != null && OEBill.result != null)
                {
                    if (OEBill.result.maketqua == SUCCESS_CODE)
                    {
                        result.Success = true;
                        result.InvoiceCode = OEBill.result.magiaodich;
                        result.InvoiceLookupCode = OEBill.result.magiaodich;
                        result.InvoiceLoginname = adoLogin.username;
                        result.InvoiceTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                    }
                    else
                    {
                        result.Success = false;
                        ElectronicBillResultUtil.Set(ref result, false, OEBill.result != null ? OEBill.result.motaketqua : "Gửi hóa đơn gốc thất bại");
                    }
                }
                else if (OEBill == null || (OEBill != null && OEBill.error != null))
                {
                    result.Success = false;
                    ElectronicBillResultUtil.Set(ref result, false, OEBill != null && OEBill.error != null ? OEBill.error.details : "Gửi hóa đơn gốc thất bại");
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        #endregion

        #region Ký hóa đơn
        private InputSignElectronicBill ISEBill()
        {
            InputSignElectronicBill input = new InputSignElectronicBill();
            input.doanhnghiep_mst = ElectronicBillDataInput.Branch.TAX_CODE;
            input.magiaodich = OEBill.result.magiaodich;
            input.ma_hoadon = ElectronicBillDataInput.Transaction.TRANSACTION_CODE;
            return input;

        }
        private void KyHoaDon(ref ElectronicBillResult result)
        {
            try
            {
                if (!result.Success)
                    return;
                string sendJsonData = Newtonsoft.Json.JsonConvert.SerializeObject(ISEBill());
                OSEBill = Base.ApiConsumerV2.CreateRequest<OutputSignElectronicBill>(System.Net.WebRequestMethods.Http.Post, serviceUrl, Base.RequestUriStore.CyberbillKyHoaDonHSM, login.result.access_token, sendJsonData);
                result.InvoiceSys = ProviderType.CYBERBILL;
                if (OSEBill != null && OSEBill.result != null)
                {
                    if (OEBill.result.maketqua == SUCCESS_CODE)
                    {
                        result.Success = true;
                        result.InvoiceNumOrder = OEBill.result.sohoadon;
                    }
                    else
                    {
                        result.Success = false;
                        ElectronicBillResultUtil.Set(ref result, false, OEBill.result != null ? OEBill.result.motaketqua : "Ký hóa đơn gốc thất bại");
                    }
                }
                else if (OSEBill == null || (OSEBill != null && OSEBill.error != null))
                {
                    result.Success = false;
                    ElectronicBillResultUtil.Set(ref result, false, OSEBill != null && OSEBill.error != null ? OSEBill.error.details : "Ký hóa đơn HSM thất bại");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        #endregion

        #region Chuyển đổi hóa đơn
        private InputConvertElectronicBill ICoEBill()
        {
            InputConvertElectronicBill input = new InputConvertElectronicBill();
            input.doanhnghiep_mst = ElectronicBillDataInput.Branch.TAX_CODE;
            input.magiaodich = ElectronicBillDataInput.InvoiceCode;
            input.ma_hoadon = ElectronicBillDataInput.Transaction.TRANSACTION_CODE;
            return input;

        }
        private void ChuyenDoiHoaDon(ref ElectronicBillResult result)
        {

            try
            {
                if (ElectronicBillDataInput == null || string.IsNullOrEmpty(ElectronicBillDataInput.InvoiceCode))
                    return;
                string sendJsonData = Newtonsoft.Json.JsonConvert.SerializeObject(ICoEBill());
                OCoEBill = Base.ApiConsumerV2.CreateRequest<OutputConvertElectronicBill>(System.Net.WebRequestMethods.Http.Post, serviceUrl, Base.RequestUriStore.CyberbillTaiHoaDon, login.result.access_token, sendJsonData);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData($"CyberbillChuyenDoiHoaDon", sendJsonData));
                result.InvoiceSys = ProviderType.CYBERBILL;
                if (OCoEBill != null && OCoEBill.result != null)
                {
                    if (OCoEBill.result.maketqua == SUCCESS_CODE)
                    {
                        result.Success = true;
                        var dic = Application.StartupPath + @"\temp";
                        if (!Directory.Exists(dic))
                            Directory.CreateDirectory(dic);
                        string fullName = dic + @"\" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf";
                        System.IO.File.WriteAllBytes(fullName, System.Convert.FromBase64String(OCoEBill.result.base64pdf));
                        result.InvoiceLink = fullName;
                    }
                    else
                    {
                        result.Success = false;
                        ElectronicBillResultUtil.Set(ref result, false, OEBill.result != null ? OEBill.result.motaketqua : "Chuyển đổi hóa đơn thất bại");
                    }
                }
                else if (OCoEBill == null || (OCoEBill != null && OCoEBill.error != null))
                {
                    result.Success = false;
                    ElectronicBillResultUtil.Set(ref result, false, OCoEBill != null && OCoEBill.error != null ? OCoEBill.error.details : "Chuyển đổi hóa đơn thất bại");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void CyberbillChuyenDoiHoaDon(ref ElectronicBillResult result)
        {

            try
            {
                if (ElectronicBillDataInput == null || string.IsNullOrEmpty(ElectronicBillDataInput.InvoiceCode))
                    return;   
                string sendJsonData = Newtonsoft.Json.JsonConvert.SerializeObject(ICoEBill());
                OCoEBill = Base.ApiConsumerV2.CreateRequest<OutputConvertElectronicBill>(System.Net.WebRequestMethods.Http.Post, serviceUrl, Base.RequestUriStore.CyberbillChuyenDoiHoaDon, login.result.access_token, sendJsonData);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData($"CyberbillChuyenDoiHoaDon", sendJsonData));

                result.InvoiceSys = ProviderType.CYBERBILL;
                if (OCoEBill != null && OCoEBill.result != null)
                {
                    if (OCoEBill.result.maketqua == SUCCESS_CODE)
                    {
                        result.Success = true;
                        var dic = Application.StartupPath + @"\temp";
                        if (!Directory.Exists(dic))
                            Directory.CreateDirectory(dic);
                        string fullName = dic + @"\" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf";
                        System.IO.File.WriteAllBytes(fullName, System.Convert.FromBase64String(OCoEBill.result.base64pdf));
                        result.InvoiceLink = fullName;
                    }
                    else
                    {
                        result.Success = false;
                        ElectronicBillResultUtil.Set(ref result, false, OEBill.result != null ? OEBill.result.motaketqua : "Chuyển đổi hóa đơn thất bại");
                    }
                }
                else if (OCoEBill == null || (OCoEBill != null && OCoEBill.error != null))
                {
                    result.Success = false;
                    ElectronicBillResultUtil.Set(ref result, false, OCoEBill != null && OCoEBill.error != null ? OCoEBill.error.details : "Chuyển đổi hóa đơn thất bại");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        #endregion

        #region Hủy hóa đơn
        private InputCancelElectronicBill ICaBill()
        {
            InputCancelElectronicBill ca = new InputCancelElectronicBill();
            ca.doanhnghiep_mst = ElectronicBillDataInput.Branch.TAX_CODE;
            ca.hoadon_loai = 7;
            ca.loaihoadon_ma = ElectronicBillDataInput.TemplateCode;
            ca.mauso = ElectronicBillDataInput.TemplateCode;
            ca.kyhieu = ElectronicBillDataInput.SymbolCode;
            ca.ma_hoadon = ElectronicBillDataInput.TransactionCode;
            ca.hoadon_goc = ElectronicBillDataInput.TransactionCode;
            ca.ngaylap = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            ca.hopdong_so = "";
            ca.hopdong_ngayky = "";
            ca.file_hopdong = "";
            ca.nguoilap = adoLogin.username;
            return ca;
        }
        private void HuyHoaDon(ref ElectronicBillResult result)
        {

            try
            {
                if (ElectronicBillDataInput == null || string.IsNullOrEmpty(ElectronicBillDataInput.InvoiceCode))
                    return;
                string sendJsonData = Newtonsoft.Json.JsonConvert.SerializeObject(ICaBill());
                OCaEBill = Base.ApiConsumerV2.CreateRequest<OutputCancelElectronicBill>(System.Net.WebRequestMethods.Http.Post, serviceUrl, Base.RequestUriStore.CyberbillHuyHoaDon, login.result.access_token, sendJsonData);
                result.InvoiceSys = ProviderType.CYBERBILL;
                if (OCaEBill != null && OCaEBill.result != null)
                {
                    if (OCaEBill.result.maketqua == SUCCESS_CODE)
                    {
                        result.Success = true;
                    }
                    else
                    {
                        result.Success = false;
                        ElectronicBillResultUtil.Set(ref result, false, OCaEBill.result != null ? OCaEBill.result.motaketqua : "Hủy hóa đơn thất bại");
                    }
                }
                else if (OCaEBill == null || (OCaEBill != null && OCaEBill.error != null))
                {
                    result.Success = false;
                    ElectronicBillResultUtil.Set(ref result, false, OCaEBill != null && OCaEBill.error != null ? OCaEBill.error.details : "Hủy hóa đơn thất bại");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        #endregion

        private bool Check(ElectronicBillType.ENUM _electronicBillTypeEnum, ref ElectronicBillResult electronicBillResult)
        {
            bool result = true;
            try
            {
                string[] configArr = serviceConfig.Split('|');
                if (configArr.Length < 2)
                    throw new Exception("Sai định dạng cấu hình hệ thống.");
                if (configArr[0] != ProviderType.CYBERBILL)
                    throw new Exception("Không đúng cấu hình nhà cung cấp MOBIFONE");

                string[] accountArr = accountConfig.Split('|');
                if (accountArr.Length != 2)
                    throw new Exception("Sai định dạng cấu hình tài khoản.");

                if (_electronicBillTypeEnum == ElectronicBillType.ENUM.CREATE_INVOICE)
                {
                    if (this.ElectronicBillDataInput == null)
                        throw new Exception("Không có dữ liệu phát hành hóa đơn.");
                    if (this.ElectronicBillDataInput.Treatment == null)
                        throw new Exception("Không có thông tin hồ sơ điều trị.");
                    if (this.ElectronicBillDataInput.Branch == null)
                        throw new Exception("Không có thông tin chi nhánh.");
                }
            }
            catch (Exception ex)
            {
                result = false;
                ElectronicBillResultUtil.Set(ref electronicBillResult, false, ex.Message);
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
    }
}
