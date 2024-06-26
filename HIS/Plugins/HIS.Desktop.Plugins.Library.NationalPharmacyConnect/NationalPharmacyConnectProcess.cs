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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Inventec.Common.Logging;
using HIS.Desktop.Plugins.Library.NationalPharmacyConnect.JsonADO;

namespace HIS.Desktop.Plugins.Library.NationalPharmacyConnect
{
    public class NationalPharmacyConnectProcess
    {
        private string address { get; set; }
        private string loginname { get; set; }
        private string password { get; set; }
        public NationalPharmacyConnectProcess(string _address, string _loginname, string _password)
        {
            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
                this.address = _address;
                this.loginname = _loginname;
                this.password = _password;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        DangNhap plv = null;

        //RebindingHandler _rebHandler = new RebindingHandler(GetIPInternet());

        private static List<IPAddress> GetIPInternet()
        {
            List<IPAddress> localIP = new List<IPAddress>();
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                localIP.Add(IPAddress.Parse(endPoint.Address.ToString()));
            }
            return localIP;
        }

        private void RegisToken()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(this.address);

                    TaiKhoan param = new TaiKhoan();
                    param.TenDangNhap = this.loginname;
                    param.MatKhau = this.password;

                    string data = Newtonsoft.Json.JsonConvert.SerializeObject(param);
                    var content = new StringContent(data, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = client.PostAsync("/api/tai_khoan/dang_nhap", content).Result;
                    string responseData = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string resultContent = responseData;
                        plv = (DangNhap)JsonConvert.DeserializeObject<DangNhap>(resultContent);
                    }
                    else
                    {
                        plv.Ma = ((int)response.StatusCode).ToString();
                        if (plv.TinNhan == null || plv.TinNhan.Equals(""))
                            plv.TinNhan = responseData;
                        plv.ThongTinDangNhap = new ThongTinDangNhap()
                        {
                            Token = string.Empty,
                            TokenType = string.Empty
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                plv.Ma = "001";
                if (ex.Message.Equals("Invalid URI: The format of the URI could not be determined.") || ex.Message.Equals("One or more errors occurred."))
                {
                    plv.TinNhan = "Địa chỉ API không chính xác.";
                }
                else
                {
                    plv.TinNhan = ex.ToString();
                }
                plv.ThongTinDangNhap = new ThongTinDangNhap()
                {
                    Token = string.Empty,
                    TokenType = string.Empty
                };
            }
        }

        public bool IsSuccessCode(string code)
        {
            try
            {
                if (code != null && code.Trim().Equals("200"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public DonThuocQuocGia DonThuoc(DonThuoc donThuoc, ThaoTac thaoTac)
        {
            DonThuocQuocGia donThuocQG = new DonThuocQuocGia();
            try
            {
                if (plv == null)
                {
                    RegisToken();
                }

                if (IsSuccessCode(plv.Ma))
                {
                    var key = plv.ThongTinDangNhap;
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(this.address);
                        client.DefaultRequestHeaders.Accept.Clear();
                        string dataToken = string.Format("bearer {0}", key.Token);
                        client.DefaultRequestHeaders.Add("Authorization", dataToken);

                        string data = Newtonsoft.Json.JsonConvert.SerializeObject(donThuoc);

                        var content = new StringContent(data, Encoding.UTF8, "application/json");

                        if (thaoTac == ThaoTac.TaoMoi)
                        {
                            HttpResponseMessage response = client.PostAsync("api/lien_thong/don_thuoc", content).Result;

                            string responseData = response.Content.ReadAsStringAsync().Result;
                            Inventec.Common.Logging.LogSystem.Info(responseData);

                            if (response.IsSuccessStatusCode)
                            {
                                donThuocQG = JsonConvert.DeserializeObject<DonThuocQuocGia>(responseData);
                                if (donThuocQG == null)
                                {
                                    donThuocQG = new DonThuocQuocGia();
                                    donThuocQG.Ma = string.Empty;
                                    donThuocQG.TinNhan = responseData;
                                }
                                if (donThuocQG.Ma != null && !IsSuccessCode(donThuocQG.Ma))
                                {
                                    if (donThuocQG.TinNhan == null || donThuocQG.TinNhan.Equals(""))
                                        donThuocQG.TinNhan = responseData;
                                }
                            }
                            else
                            {
                                Inventec.Common.Logging.LogSystem.Error("loi goi api: api/lien_thong/hoa_don");
                                donThuocQG.Ma = ((int)response.StatusCode).ToString();
                                donThuocQG.TinNhan = responseData;
                            }
                        }
                        else if (thaoTac == ThaoTac.CapNhat)
                        {
                            HttpResponseMessage response = client.PutAsync("api/lien_thong/don_thuoc", content).Result;
                            string responseData = response.Content.ReadAsStringAsync().Result;
                            Inventec.Common.Logging.LogSystem.Info(responseData);
                            if (response.IsSuccessStatusCode)
                            {
                                donThuocQG = JsonConvert.DeserializeObject<DonThuocQuocGia>(responseData);
                                if (donThuocQG == null)
                                {
                                    donThuocQG = new DonThuocQuocGia();
                                    donThuocQG.Ma = string.Empty;
                                    donThuocQG.TinNhan = responseData;
                                }
                                if (donThuocQG.Ma != null && !IsSuccessCode(donThuocQG.Ma))
                                {
                                    if (donThuocQG.TinNhan == null || donThuocQG.TinNhan.Equals(""))
                                        donThuocQG.TinNhan = responseData;
                                }
                            }
                            else
                            {
                                Inventec.Common.Logging.LogSystem.Error("loi goi api: api/lien_thong/don_thuoc");
                                donThuocQG.Ma = ((int)response.StatusCode).ToString();
                                donThuocQG.TinNhan = responseData;
                            }
                        }
                        else if (thaoTac == ThaoTac.Xoa)
                        {
                            HttpResponseMessage response = client.DeleteAsync("api/lien_thong/don_thuoc/" + donThuoc.ThongTinDonVi.MaCSKCB + "/" + donThuoc.MaDonThuocCSKCB).Result;
                            string responseData = response.Content.ReadAsStringAsync().Result;
                            Inventec.Common.Logging.LogSystem.Info(responseData);
                            if (response.IsSuccessStatusCode)
                            {
                                donThuocQG = JsonConvert.DeserializeObject<DonThuocQuocGia>(responseData);
                                if (donThuocQG == null)
                                {
                                    donThuocQG = new DonThuocQuocGia();
                                    donThuocQG.Ma = string.Empty;
                                    donThuocQG.TinNhan = responseData;
                                }
                                if (donThuocQG.Ma != null && !IsSuccessCode(donThuocQG.Ma))
                                {
                                    if (donThuocQG.TinNhan == null || donThuocQG.TinNhan.Equals(""))
                                        donThuocQG.TinNhan = responseData;
                                }
                            }
                            else
                            {
                                Inventec.Common.Logging.LogSystem.Error("loi goi api: api/lien_thong/don_thuoc");
                                donThuocQG.Ma = ((int)response.StatusCode).ToString();
                                donThuocQG.TinNhan = responseData;
                            }
                        }
                    }
                }
                else
                {
                    donThuocQG.Ma = plv.Ma;
                    donThuocQG.TinNhan = plv.TinNhan;
                    donThuocQG.MaDonThuocQuocGia = string.Empty;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);

                donThuocQG.Ma = "001";
                if (ex.Message.Equals("Invalid URI: The format of the URI could not be determined.") || ex.Message.Equals("One or more errors occurred."))
                {
                    donThuocQG.TinNhan = "Địa chỉ API không chính xác.";
                }
                else
                {
                    donThuocQG.TinNhan = ex.ToString();
                }
                donThuocQG.MaDonThuocQuocGia = string.Empty;
            }
            return donThuocQG;
        }

        public HoaDonQuocGia HoaDon(HoaDon hoadon, ThaoTac thaoTac)
        {
            HoaDonQuocGia hoaDonQG = new HoaDonQuocGia();
            try
            {
                if (plv == null)
                {
                    RegisToken();
                }
                if (IsSuccessCode(plv.Ma))
                {
                    var key = plv.ThongTinDangNhap;
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(this.address);
                        client.DefaultRequestHeaders.Accept.Clear();
                        string dataToken = string.Format("bearer {0}", key.Token);
                        client.DefaultRequestHeaders.Add("Authorization", dataToken);

                        string data = Newtonsoft.Json.JsonConvert.SerializeObject(hoadon);
                        var content = new StringContent(data, Encoding.UTF8, "application/json");

                        if (thaoTac == ThaoTac.TaoMoi)
                        {
                            HttpResponseMessage response = client.PostAsync("api/lien_thong/hoa_don", content).Result;
                            string responseData = response.Content.ReadAsStringAsync().Result;
                            Inventec.Common.Logging.LogSystem.Info(responseData);
                            if (response.IsSuccessStatusCode)
                            {
                                hoaDonQG = JsonConvert.DeserializeObject<HoaDonQuocGia>(responseData);
                                if (hoaDonQG == null)
                                {
                                    hoaDonQG = new HoaDonQuocGia();
                                    hoaDonQG.Ma = string.Empty;
                                    hoaDonQG.TinNhan = responseData;
                                }
                                if (hoaDonQG.Ma != null && !IsSuccessCode(hoaDonQG.Ma))
                                {
                                    if (hoaDonQG.TinNhan == null || hoaDonQG.TinNhan.Equals(""))
                                        hoaDonQG.TinNhan = responseData;
                                }
                            }
                            else
                            {
                                Inventec.Common.Logging.LogSystem.Error("loi goi api: api/lien_thong/hoa_don");
                                hoaDonQG.Ma = ((int)response.StatusCode).ToString();
                                hoaDonQG.TinNhan = responseData;
                            }
                        }
                        else if (thaoTac == ThaoTac.CapNhat)
                        {
                            HttpResponseMessage response = client.PutAsync("api/lien_thong/hoa_don", content).Result;
                            string responseData = response.Content.ReadAsStringAsync().Result;
                            Inventec.Common.Logging.LogSystem.Info(responseData);
                            if (response.IsSuccessStatusCode)
                            {
                                hoaDonQG = JsonConvert.DeserializeObject<HoaDonQuocGia>(responseData);
                                if (hoaDonQG == null)
                                {
                                    hoaDonQG = new HoaDonQuocGia();
                                    hoaDonQG.Ma = string.Empty;
                                    hoaDonQG.TinNhan = responseData;
                                }
                                if (hoaDonQG.Ma != null && !IsSuccessCode(hoaDonQG.Ma))
                                {
                                    if (hoaDonQG.TinNhan == null || hoaDonQG.TinNhan.Equals(""))
                                        hoaDonQG.TinNhan = responseData;
                                }
                            }
                            else
                            {
                                Inventec.Common.Logging.LogSystem.Error("loi goi api: api/lien_thong/hoa_don");
                                hoaDonQG.Ma = ((int)response.StatusCode).ToString();
                                hoaDonQG.TinNhan = responseData;
                            }
                        }
                        else if (thaoTac == ThaoTac.Xoa)
                        {
                            HttpResponseMessage response = client.DeleteAsync("api/lien_thong/hoa_don/" + hoadon.MaCoSo + "/" + hoadon.MaHoaDon).Result;
                            string responseData = response.Content.ReadAsStringAsync().Result;
                            Inventec.Common.Logging.LogSystem.Info(responseData);
                            if (response.IsSuccessStatusCode)
                            {
                                hoaDonQG = JsonConvert.DeserializeObject<HoaDonQuocGia>(responseData);
                                if (hoaDonQG == null)
                                {
                                    hoaDonQG = new HoaDonQuocGia();
                                    hoaDonQG.Ma = string.Empty;
                                    hoaDonQG.TinNhan = responseData;
                                }
                                if (hoaDonQG.Ma != null && !IsSuccessCode(hoaDonQG.Ma))
                                {
                                    if (hoaDonQG.TinNhan == null || hoaDonQG.TinNhan.Equals(""))
                                        hoaDonQG.TinNhan = responseData;
                                }
                            }
                            else
                            {
                                Inventec.Common.Logging.LogSystem.Error("loi goi api: api/lien_thong/hoa_don");
                                hoaDonQG.Ma = ((int)response.StatusCode).ToString();
                                hoaDonQG.TinNhan = responseData;
                            }
                        }
                    }
                }
                else
                {
                    hoaDonQG.Ma = plv.Ma;
                    hoaDonQG.TinNhan = plv.TinNhan;
                    hoaDonQG.MaHoaDonQuocGia = string.Empty;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                hoaDonQG.Ma = "001";
                if (ex.Message.Equals("Invalid URI: The format of the URI could not be determined.") || ex.Message.Equals("One or more errors occurred."))
                {
                    hoaDonQG.TinNhan = "Địa chỉ API không chính xác.";
                }
                else
                {
                    hoaDonQG.TinNhan = ex.ToString();
                }
                hoaDonQG.MaHoaDonQuocGia = string.Empty;
            }
            return hoaDonQG;
        }

        public PhieuNhapQuocGia PhieuNhap(PhieuNhap phieunhap, ThaoTac thaoTac)
        {
            PhieuNhapQuocGia phieuNhapQG = new PhieuNhapQuocGia();
            try
            {
                if (plv == null)
                {
                    RegisToken();
                }
                if (IsSuccessCode(plv.Ma))
                {
                    var key = plv.ThongTinDangNhap;
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(this.address);
                        client.DefaultRequestHeaders.Accept.Clear();
                        string dataToken = string.Format("bearer {0}", key.Token);
                        client.DefaultRequestHeaders.Add("Authorization", dataToken);

                        string data = Newtonsoft.Json.JsonConvert.SerializeObject(phieunhap);
                        var content = new StringContent(data, Encoding.UTF8, "application/json");

                        if (thaoTac == ThaoTac.TaoMoi)
                        {
                            HttpResponseMessage response = client.PostAsync("api/lien_thong/phieu_nhap", content).Result;
                            string responseData = response.Content.ReadAsStringAsync().Result;
                            Inventec.Common.Logging.LogSystem.Info(responseData);
                            if (response.IsSuccessStatusCode)
                            {
                                phieuNhapQG = JsonConvert.DeserializeObject<PhieuNhapQuocGia>(responseData);
                                if (phieuNhapQG == null)
                                {
                                    phieuNhapQG = new PhieuNhapQuocGia();
                                    phieuNhapQG.Ma = string.Empty;
                                    phieuNhapQG.TinNhan = responseData;
                                }
                                if (phieuNhapQG.Ma != null && !IsSuccessCode(phieuNhapQG.Ma))
                                {
                                    if (phieuNhapQG.TinNhan == null || phieuNhapQG.TinNhan.Equals(""))
                                        phieuNhapQG.TinNhan = responseData;
                                }
                            }
                            else
                            {
                                Inventec.Common.Logging.LogSystem.Error("loi goi api: api/lien_thong/phieu_nhap");
                                phieuNhapQG.Ma = ((int)response.StatusCode).ToString();
                                phieuNhapQG.TinNhan = responseData;
                            }
                        }
                        else if (thaoTac == ThaoTac.CapNhat)
                        {
                            HttpResponseMessage response = client.PutAsync("api/lien_thong/phieu_nhap", content).Result;
                            string responseData = response.Content.ReadAsStringAsync().Result;
                            Inventec.Common.Logging.LogSystem.Info(responseData);
                            if (response.IsSuccessStatusCode)
                            {
                                phieuNhapQG = JsonConvert.DeserializeObject<PhieuNhapQuocGia>(responseData);
                                if (phieuNhapQG == null)
                                {
                                    phieuNhapQG = new PhieuNhapQuocGia();
                                    phieuNhapQG.Ma = string.Empty;
                                    phieuNhapQG.TinNhan = responseData;
                                }
                                if (phieuNhapQG.Ma != null && !IsSuccessCode(phieuNhapQG.Ma))
                                {
                                    if (phieuNhapQG.TinNhan == null || phieuNhapQG.TinNhan.Equals(""))
                                        phieuNhapQG.TinNhan = responseData;
                                }
                            }
                            else
                            {
                                Inventec.Common.Logging.LogSystem.Error("loi goi api: api/lien_thong/phieu_nhap");
                                phieuNhapQG.Ma = ((int)response.StatusCode).ToString();
                                phieuNhapQG.TinNhan = responseData;
                            }
                        }
                        else if (thaoTac == ThaoTac.Xoa)
                        {
                            HttpResponseMessage response = client.DeleteAsync("api/lien_thong/phieu_nhap/" + phieunhap.MaCoSo + "/" + phieunhap.MaPhieu).Result;
                            string responseData = response.Content.ReadAsStringAsync().Result;
                            Inventec.Common.Logging.LogSystem.Info(responseData);
                            if (response.IsSuccessStatusCode)
                            {
                                phieuNhapQG = JsonConvert.DeserializeObject<PhieuNhapQuocGia>(responseData);
                                if (phieuNhapQG == null)
                                {
                                    phieuNhapQG = new PhieuNhapQuocGia();
                                    phieuNhapQG.Ma = string.Empty;
                                    phieuNhapQG.TinNhan = responseData;
                                }
                                if (phieuNhapQG.Ma != null && !IsSuccessCode(phieuNhapQG.Ma))
                                {
                                    if (phieuNhapQG.TinNhan == null || phieuNhapQG.TinNhan.Equals(""))
                                        phieuNhapQG.TinNhan = responseData;
                                }
                            }
                            else
                            {
                                Inventec.Common.Logging.LogSystem.Error("loi goi api: api/lien_thong/phieu_nhap");
                                phieuNhapQG.Ma = ((int)response.StatusCode).ToString();
                                phieuNhapQG.TinNhan = responseData;
                            }
                        }
                    }
                }
                else
                {
                    phieuNhapQG.Ma = plv.Ma;
                    phieuNhapQG.TinNhan = plv.TinNhan;
                    phieuNhapQG.MaPhieuNhapQuocGia = string.Empty;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                phieuNhapQG.Ma = "001";
                if (ex.Message.Equals("Invalid URI: The format of the URI could not be determined.") || ex.Message.Equals("One or more errors occurred."))
                {
                    phieuNhapQG.TinNhan = "Địa chỉ API không chính xác.";
                }
                else
                {
                    phieuNhapQG.TinNhan = ex.ToString();
                }
                phieuNhapQG.MaPhieuNhapQuocGia = string.Empty;
            }
            return phieuNhapQG;
        }

        public PhieuXuatQuocGia PhieuXuat(PhieuXuat phieuxuat, ThaoTac thaoTac)
        {
            PhieuXuatQuocGia phieuXuatQG = new PhieuXuatQuocGia();
            try
            {
                if (plv == null)
                {
                    RegisToken();
                }
                if (IsSuccessCode(plv.Ma))
                {
                    var key = plv.ThongTinDangNhap;
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(this.address);
                        client.DefaultRequestHeaders.Accept.Clear();
                        string dataToken = string.Format("bearer {0}", key.Token);
                        client.DefaultRequestHeaders.Add("Authorization", dataToken);
                        string data = Newtonsoft.Json.JsonConvert.SerializeObject(phieuxuat);
                        var content = new StringContent(data, Encoding.UTF8, "application/json");

                        if (thaoTac == ThaoTac.TaoMoi)
                        {
                            HttpResponseMessage response = client.PostAsync("api/lien_thong/phieu_xuat", content).Result;
                            string responseData = response.Content.ReadAsStringAsync().Result;
                            Inventec.Common.Logging.LogSystem.Info(responseData);
                            if (response.IsSuccessStatusCode)
                            {
                                phieuXuatQG = JsonConvert.DeserializeObject<PhieuXuatQuocGia>(responseData);
                                if (phieuXuatQG == null)
                                {
                                    phieuXuatQG = new PhieuXuatQuocGia();
                                    phieuXuatQG.Ma = string.Empty;
                                    phieuXuatQG.TinNhan = responseData;
                                }
                                if (phieuXuatQG.Ma != null && !IsSuccessCode(phieuXuatQG.Ma))
                                {
                                    if (phieuXuatQG.TinNhan == null || phieuXuatQG.TinNhan.Equals(""))
                                        phieuXuatQG.TinNhan = responseData;
                                }
                            }
                            else
                            {
                                Inventec.Common.Logging.LogSystem.Error("loi goi api: api/lien_thong/phieu_xuat");
                                phieuXuatQG.Ma = ((int)response.StatusCode).ToString();
                                phieuXuatQG.TinNhan = responseData;
                            }
                        }
                        else if (thaoTac == ThaoTac.CapNhat)
                        {
                            HttpResponseMessage response = client.PutAsync("api/phieu_xuat/phieu_nhap", content).Result;
                            string responseData = response.Content.ReadAsStringAsync().Result;
                            Inventec.Common.Logging.LogSystem.Info(responseData);
                            if (response.IsSuccessStatusCode)
                            {
                                phieuXuatQG = JsonConvert.DeserializeObject<PhieuXuatQuocGia>(responseData);
                                if (phieuXuatQG == null)
                                {
                                    phieuXuatQG = new PhieuXuatQuocGia();
                                    phieuXuatQG.Ma = string.Empty;
                                    phieuXuatQG.TinNhan = responseData;
                                }
                                if (phieuXuatQG.Ma != null && !IsSuccessCode(phieuXuatQG.Ma))
                                {
                                    if (phieuXuatQG.TinNhan == null || phieuXuatQG.TinNhan.Equals(""))
                                        phieuXuatQG.TinNhan = responseData;
                                }
                            }
                            else
                            {
                                Inventec.Common.Logging.LogSystem.Error("loi goi api: api/lien_thong/phieu_xuat");
                                phieuXuatQG.Ma = ((int)response.StatusCode).ToString();
                                phieuXuatQG.TinNhan = responseData;
                            }
                        }
                        else if (thaoTac == ThaoTac.Xoa)
                        {
                            HttpResponseMessage response = client.DeleteAsync("api/lien_thong/phieu_xuat/" + phieuxuat.MaCoSo + "/" + phieuxuat.MaPhieu).Result;
                            string responseData = response.Content.ReadAsStringAsync().Result;
                            Inventec.Common.Logging.LogSystem.Info(responseData);
                            if (response.IsSuccessStatusCode)
                            {
                                phieuXuatQG = JsonConvert.DeserializeObject<PhieuXuatQuocGia>(responseData);
                                if (phieuXuatQG == null)
                                {
                                    phieuXuatQG = new PhieuXuatQuocGia();
                                    phieuXuatQG.Ma = string.Empty;
                                    phieuXuatQG.TinNhan = responseData;
                                }
                                if (phieuXuatQG.Ma != null && !IsSuccessCode(phieuXuatQG.Ma))
                                {
                                    if (phieuXuatQG.TinNhan == null || phieuXuatQG.TinNhan.Equals(""))
                                        phieuXuatQG.TinNhan = responseData;
                                }
                            }
                            else
                            {
                                Inventec.Common.Logging.LogSystem.Error("loi goi api: api/lien_thong/phieu_xuat");
                                phieuXuatQG.Ma = ((int)response.StatusCode).ToString();
                                phieuXuatQG.TinNhan = responseData;
                            }
                        }
                    }
                }
                else
                {
                    phieuXuatQG.Ma = plv.Ma;
                    phieuXuatQG.TinNhan = plv.TinNhan;
                    phieuXuatQG.MaPhieuXuatQuocGia = string.Empty;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                phieuXuatQG.Ma = "001";
                if (ex.Message.Equals("Invalid URI: The format of the URI could not be determined.") || ex.Message.Equals("One or more errors occurred."))
                {
                    phieuXuatQG.TinNhan = "Địa chỉ API không chính xác.";
                }
                else
                {
                    phieuXuatQG.TinNhan = ex.ToString();
                }
                phieuXuatQG.MaPhieuXuatQuocGia = string.Empty;
            }
            return phieuXuatQG;
        }
    }
}
