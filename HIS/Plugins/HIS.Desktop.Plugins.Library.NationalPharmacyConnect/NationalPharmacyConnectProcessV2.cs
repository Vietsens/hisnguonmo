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
    public class NationalPharmacyConnectProcessV2
    {
        private string address { get; set; }
        private string loginname { get; set; }
        private string password { get; set; }
        public NationalPharmacyConnectProcessV2(string _address, string _loginname, string _password)
        {
            try
            {
                this.address = _address;
                this.loginname = _loginname;
                this.password = _password;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        DangNhap dangNhap = null;

        RebindingHandler _rebHandler = new RebindingHandler(GetIPInternet());

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

        public async Task DangNhap()
        {
            dangNhap = new DangNhap();
            try
            {
                TaiKhoan taiKhoan = new TaiKhoan()
                {
                    TenDangNhap = this.loginname,
                    MatKhau = this.password
                };

                using (HttpClient httpClient = new HttpClient())
                {
                    //httpClient.Timeout = new TimeSpan(300);
                    httpClient.BaseAddress = new Uri(this.address);
                    string data = JsonConvert.SerializeObject(taiKhoan);
                    var content = new StringContent(data, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await httpClient.PostAsync("api/tai_khoan/dang_nhap", content);
                    if (response.IsSuccessStatusCode)
                    {
                        dangNhap = JsonConvert.DeserializeObject<DangNhap>(response.Content.ReadAsStringAsync().Result);
                    }
                    else
                    {
                        //dangNhap = JsonConvert.DeserializeObject<DangNhap>(response.Content.ReadAsStringAsync().Result);
                        dangNhap.Ma = ((int)response.StatusCode).ToString();
                        //if (dangNhap.TinNhan == null || dangNhap.TinNhan.Equals(""))
                        //dangNhap.TinNhan = response.Content.ReadAsStringAsync().Result;
                        dangNhap.ThongTinDangNhap = new ThongTinDangNhap()
                        {
                            Token = string.Empty,
                            TokenType = string.Empty
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                dangNhap.Ma = "001";
                if (ex.Message.Equals("Invalid URI: The format of the URI could not be determined.") || ex.Message.Equals("One or more errors occurred."))
                {
                    dangNhap.TinNhan = "Địa chỉ API không chính xác.";
                }
                else
                {
                    dangNhap.TinNhan = ex.ToString();
                }
                dangNhap.ThongTinDangNhap = new ThongTinDangNhap()
                {
                    Token = string.Empty,
                    TokenType = string.Empty
                };
            }
        }

        public DonThuocQuocGia DonThuoc(DonThuoc donThuoc, ThaoTac thaoTac)
        {
            DonThuocQuocGia donThuocQG = new DonThuocQuocGia();
            try
            {
                if (dangNhap == null)
                    DangNhap();
                //DangNhap dangNhap = DangNhap(tenDangNhap, matKhau);
                if (IsSuccessCode(dangNhap.Ma))
                {
                    using (HttpClient httpClient = new HttpClient(_rebHandler))
                    {
                        httpClient.BaseAddress = new Uri(this.address);
                        httpClient.DefaultRequestHeaders.Accept.Clear();
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(dangNhap.ThongTinDangNhap.TokenType, dangNhap.ThongTinDangNhap.Token);
                        HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(donThuoc), System.Text.Encoding.UTF8, "application/json");
                        // thêm mới
                        if (thaoTac == ThaoTac.TaoMoi)
                        {
                            HttpResponseMessage response = httpClient.PostAsync("api/lien_thong/don_thuoc", httpContent).Result;
                            if (response.IsSuccessStatusCode)
                            {
                                donThuocQG = JsonConvert.DeserializeObject<DonThuocQuocGia>(response.Content.ReadAsStringAsync().Result);
                                if (donThuocQG == null)
                                {
                                    donThuocQG = new DonThuocQuocGia();
                                    donThuocQG.Ma = string.Empty;
                                    donThuocQG.TinNhan = response.Content.ReadAsStringAsync().Result;
                                }
                                if (donThuocQG.Ma != null && !IsSuccessCode(donThuocQG.Ma))
                                {
                                    if (donThuocQG.TinNhan == null || donThuocQG.TinNhan.Equals(""))
                                        donThuocQG.TinNhan = response.Content.ReadAsStringAsync().Result;
                                }
                            }
                            else
                            {
                                donThuocQG.Ma = ((int)response.StatusCode).ToString();
                                donThuocQG.TinNhan = response.Content.ReadAsStringAsync().Result;
                            }
                        }
                        else if (thaoTac == ThaoTac.CapNhat)
                        {
                            HttpResponseMessage response = httpClient.PutAsync("api/lien_thong/don_thuoc", httpContent).Result;
                            if (response.IsSuccessStatusCode)
                            {
                                donThuocQG = JsonConvert.DeserializeObject<DonThuocQuocGia>(response.Content.ReadAsStringAsync().Result);
                                if (donThuocQG == null)
                                {
                                    donThuocQG = new DonThuocQuocGia();
                                    donThuocQG.Ma = string.Empty;
                                    donThuocQG.TinNhan = response.Content.ReadAsStringAsync().Result;
                                }
                                if (donThuocQG.Ma != null && !IsSuccessCode(donThuocQG.Ma))
                                {
                                    if (donThuocQG.TinNhan == null || donThuocQG.TinNhan.Equals(""))
                                        donThuocQG.TinNhan = response.Content.ReadAsStringAsync().Result;
                                }
                            }
                            else
                            {
                                donThuocQG.Ma = ((int)response.StatusCode).ToString();
                                donThuocQG.TinNhan = response.Content.ReadAsStringAsync().Result;
                            }
                        }
                        else if (thaoTac == ThaoTac.Xoa)
                        {
                            HttpResponseMessage response = httpClient.DeleteAsync("api/lien_thong/don_thuoc/" + donThuoc.ThongTinDonVi.MaCSKCB + "/" + donThuoc.MaDonThuocCSKCB).Result;
                            if (response.IsSuccessStatusCode)
                            {
                                donThuocQG = JsonConvert.DeserializeObject<DonThuocQuocGia>(response.Content.ReadAsStringAsync().Result);
                                if (donThuocQG == null)
                                {
                                    donThuocQG = new DonThuocQuocGia();
                                    donThuocQG.Ma = string.Empty;
                                    donThuocQG.TinNhan = response.Content.ReadAsStringAsync().Result;
                                }
                                if (donThuocQG.Ma != null && !IsSuccessCode(donThuocQG.Ma))
                                {
                                    if (donThuocQG.TinNhan == null || donThuocQG.TinNhan.Equals(""))
                                        donThuocQG.TinNhan = response.Content.ReadAsStringAsync().Result;
                                }
                            }
                            else
                            {
                                donThuocQG.Ma = ((int)response.StatusCode).ToString();
                                donThuocQG.TinNhan = response.Content.ReadAsStringAsync().Result;
                            }
                        }
                    }
                }
                else
                {
                    donThuocQG.Ma = dangNhap.Ma;
                    donThuocQG.TinNhan = dangNhap.TinNhan;
                    donThuocQG.MaDonThuocQuocGia = string.Empty;
                }
            }
            catch (Exception ex)
            {
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

        public HoaDonQuocGia HoaDon(HoaDon hoaDon, ThaoTac thaoTac)
        {
            HoaDonQuocGia hoaDonQG = new HoaDonQuocGia();
            try
            {
                if (dangNhap == null)
                    DangNhap();
                //DangNhap dangNhap = DangNhap(tenDangNhap, matKhau);
                if (IsSuccessCode(dangNhap.Ma))
                {
                    using (HttpClient httpClient = new HttpClient(_rebHandler))
                    {
                        httpClient.BaseAddress = new Uri(this.address);
                        httpClient.DefaultRequestHeaders.Accept.Clear();
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(dangNhap.ThongTinDangNhap.TokenType, dangNhap.ThongTinDangNhap.Token);
                        HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(hoaDon), System.Text.Encoding.UTF8, "application/json");
                        if (thaoTac == ThaoTac.TaoMoi)
                        {
                            HttpResponseMessage response = httpClient.PostAsync("api/lien_thong/hoa_don", httpContent).Result;
                            if (response.IsSuccessStatusCode)
                            {
                                hoaDonQG = JsonConvert.DeserializeObject<HoaDonQuocGia>(response.Content.ReadAsStringAsync().Result);
                                if (hoaDonQG == null)
                                {
                                    hoaDonQG = new HoaDonQuocGia();
                                    hoaDonQG.Ma = string.Empty;
                                    hoaDonQG.TinNhan = response.Content.ReadAsStringAsync().Result;
                                }
                                if (hoaDonQG.Ma != null && !IsSuccessCode(hoaDonQG.Ma))
                                {
                                    if (hoaDonQG.TinNhan == null || hoaDonQG.TinNhan.Equals(""))
                                        hoaDonQG.TinNhan = response.Content.ReadAsStringAsync().Result;
                                }
                            }
                            else
                            {
                                hoaDonQG.Ma = ((int)response.StatusCode).ToString();
                                hoaDonQG.TinNhan = response.Content.ReadAsStringAsync().Result;
                            }
                        }
                        else if (thaoTac == ThaoTac.CapNhat)
                        {
                            HttpResponseMessage response = httpClient.PutAsync("api/lien_thong/hoa_don", httpContent).Result;
                            if (response.IsSuccessStatusCode)
                            {
                                hoaDonQG = JsonConvert.DeserializeObject<HoaDonQuocGia>(response.Content.ReadAsStringAsync().Result);
                                if (hoaDonQG == null)
                                {
                                    hoaDonQG = new HoaDonQuocGia();
                                    hoaDonQG.Ma = string.Empty;
                                    hoaDonQG.TinNhan = response.Content.ReadAsStringAsync().Result;
                                }
                                if (hoaDonQG.Ma != null && !IsSuccessCode(hoaDonQG.Ma))
                                {
                                    if (hoaDonQG.TinNhan == null || hoaDonQG.TinNhan.Equals(""))
                                        hoaDonQG.TinNhan = response.Content.ReadAsStringAsync().Result;
                                }
                            }
                            else
                            {
                                hoaDonQG.Ma = ((int)response.StatusCode).ToString();
                                hoaDonQG.TinNhan = response.Content.ReadAsStringAsync().Result;
                            }
                        }
                        else if (thaoTac == ThaoTac.Xoa)
                        {
                            HttpResponseMessage response = httpClient.DeleteAsync("api/lien_thong/hoa_don/" + hoaDon.MaCoSo + "/" + hoaDon.MaHoaDon).Result;
                            if (response.IsSuccessStatusCode)
                            {
                                hoaDonQG = JsonConvert.DeserializeObject<HoaDonQuocGia>(response.Content.ReadAsStringAsync().Result);
                                if (hoaDonQG == null)
                                {
                                    hoaDonQG = new HoaDonQuocGia();
                                    hoaDonQG.Ma = string.Empty;
                                    hoaDonQG.TinNhan = response.Content.ReadAsStringAsync().Result;
                                }
                                if (hoaDonQG.Ma != null && !IsSuccessCode(hoaDonQG.Ma))
                                {
                                    if (hoaDonQG.TinNhan == null || hoaDonQG.TinNhan.Equals(""))
                                        hoaDonQG.TinNhan = response.Content.ReadAsStringAsync().Result;
                                }
                            }
                            else
                            {
                                hoaDonQG.Ma = ((int)response.StatusCode).ToString();
                                hoaDonQG.TinNhan = response.Content.ReadAsStringAsync().Result;
                            }
                        }
                    }
                }
                else
                {
                    hoaDonQG.Ma = dangNhap.Ma;
                    hoaDonQG.TinNhan = dangNhap.TinNhan;
                    hoaDonQG.MaHoaDonQuocGia = string.Empty;
                }
            }
            catch (Exception ex)
            {
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

        public PhieuNhapQuocGia PhieuNhap(PhieuNhap phieuNhap, ThaoTac thaoTac)
        {
            PhieuNhapQuocGia phieuNhapQG = new PhieuNhapQuocGia();
            try
            {
                if (dangNhap == null)
                    DangNhap();
                //DangNhap dangNhap = DangNhap(tenDangNhap, matKhau);
                if (IsSuccessCode(dangNhap.Ma))
                {
                    using (HttpClient httpClient = new HttpClient(_rebHandler))
                    {
                        httpClient.BaseAddress = new Uri(this.address);
                        httpClient.DefaultRequestHeaders.Accept.Clear();
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(dangNhap.ThongTinDangNhap.TokenType, dangNhap.ThongTinDangNhap.Token);
                        HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(phieuNhap), System.Text.Encoding.UTF8, "application/json");
                        if (thaoTac == ThaoTac.TaoMoi)
                        {
                            HttpResponseMessage response = httpClient.PostAsync("api/lien_thong/phieu_nhap", httpContent).Result;
                            if (response.IsSuccessStatusCode)
                            {
                                phieuNhapQG = JsonConvert.DeserializeObject<PhieuNhapQuocGia>(response.Content.ReadAsStringAsync().Result);
                                if (phieuNhapQG == null)
                                {
                                    phieuNhapQG = new PhieuNhapQuocGia();
                                    phieuNhapQG.Ma = string.Empty;
                                    phieuNhapQG.TinNhan = response.Content.ReadAsStringAsync().Result;
                                }
                                if (phieuNhapQG.Ma != null && !IsSuccessCode(phieuNhapQG.Ma))
                                {
                                    if (phieuNhapQG.TinNhan == null || phieuNhapQG.TinNhan.Equals(""))
                                        phieuNhapQG.TinNhan = response.Content.ReadAsStringAsync().Result;
                                }
                            }
                            else
                            {
                                phieuNhapQG.Ma = ((int)response.StatusCode).ToString();
                                phieuNhapQG.TinNhan = response.Content.ReadAsStringAsync().Result;
                            }
                        }
                        else if (thaoTac == ThaoTac.CapNhat)
                        {
                            HttpResponseMessage response = httpClient.PutAsync("api/lien_thong/phieu_nhap", httpContent).Result;
                            if (response.IsSuccessStatusCode)
                            {
                                phieuNhapQG = JsonConvert.DeserializeObject<PhieuNhapQuocGia>(response.Content.ReadAsStringAsync().Result);
                                if (phieuNhapQG == null)
                                {
                                    phieuNhapQG = new PhieuNhapQuocGia();
                                    phieuNhapQG.Ma = string.Empty;
                                    phieuNhapQG.TinNhan = response.Content.ReadAsStringAsync().Result;
                                }
                                if (phieuNhapQG.Ma != null && !IsSuccessCode(phieuNhapQG.Ma))
                                {
                                    if (phieuNhapQG.TinNhan == null || phieuNhapQG.TinNhan.Equals(""))
                                        phieuNhapQG.TinNhan = response.Content.ReadAsStringAsync().Result;
                                }
                            }
                            else
                            {
                                phieuNhapQG.Ma = ((int)response.StatusCode).ToString();
                                phieuNhapQG.TinNhan = response.Content.ReadAsStringAsync().Result;
                            }
                        }
                        else if (thaoTac == ThaoTac.Xoa)
                        {
                            HttpResponseMessage response = httpClient.DeleteAsync("api/lien_thong/phieu_nhap/" + phieuNhap.MaCoSo + "/" + phieuNhap.MaPhieu).Result;
                            if (response.IsSuccessStatusCode)
                            {
                                phieuNhapQG = JsonConvert.DeserializeObject<PhieuNhapQuocGia>(response.Content.ReadAsStringAsync().Result);
                                if (phieuNhapQG == null)
                                {
                                    phieuNhapQG = new PhieuNhapQuocGia();
                                    phieuNhapQG.Ma = string.Empty;
                                    phieuNhapQG.TinNhan = response.Content.ReadAsStringAsync().Result;
                                }
                                if (phieuNhapQG.Ma != null && !IsSuccessCode(phieuNhapQG.Ma))
                                {
                                    if (phieuNhapQG.TinNhan == null || phieuNhapQG.TinNhan.Equals(""))
                                        phieuNhapQG.TinNhan = response.Content.ReadAsStringAsync().Result;
                                }
                            }
                            else
                            {
                                phieuNhapQG.Ma = ((int)response.StatusCode).ToString();
                                phieuNhapQG.TinNhan = response.Content.ReadAsStringAsync().Result;
                            }
                        }
                    }
                }
                else
                {
                    phieuNhapQG.Ma = dangNhap.Ma;
                    phieuNhapQG.TinNhan = dangNhap.TinNhan;
                    phieuNhapQG.MaPhieuNhapQuocGia = string.Empty;
                }
            }
            catch (Exception ex)
            {
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

        public async Task<PhieuXuatQuocGia> PhieuXuat(PhieuXuat phieuXuat, ThaoTac thaoTac)
        {
            PhieuXuatQuocGia phieuXuatQG = new PhieuXuatQuocGia();
            try
            {
                if (dangNhap == null)
                    await DangNhap();
                //DangNhap dangNhap = DangNhap(tenDangNhap, matKhau);
                if (IsSuccessCode(dangNhap.Ma))
                {
                    using (HttpClient httpClient = new HttpClient(_rebHandler))
                    {
                        httpClient.BaseAddress = new Uri(this.address);
                        httpClient.DefaultRequestHeaders.Accept.Clear();
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(dangNhap.ThongTinDangNhap.TokenType, dangNhap.ThongTinDangNhap.Token);
                        HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(phieuXuat), System.Text.Encoding.UTF8, "application/json");

                        //httpClient.DefaultRequestHeaders.Accept.Clear();
                        //string dataToken = string.Format("bearer {0}", dangNhap.ThongTinDangNhap.Token);
                        //httpClient.DefaultRequestHeaders.Add("Authorization", dataToken);
                        //string data = Newtonsoft.Json.JsonConvert.SerializeObject(phieuXuat);
                        //var httpContent = new StringContent(data, Encoding.UTF8, "application/json");
                        if (thaoTac == ThaoTac.TaoMoi)
                        {
                            HttpResponseMessage response = httpClient.PostAsync("api/lien_thong/phieu_xuat", httpContent).Result;
                            if (response.IsSuccessStatusCode)
                            {
                                phieuXuatQG = JsonConvert.DeserializeObject<PhieuXuatQuocGia>(response.Content.ReadAsStringAsync().Result);
                                if (phieuXuatQG == null)
                                {
                                    phieuXuatQG = new PhieuXuatQuocGia();
                                    phieuXuatQG.Ma = string.Empty;
                                    phieuXuatQG.TinNhan = response.Content.ReadAsStringAsync().Result;
                                }
                                if (phieuXuatQG.Ma != null && !IsSuccessCode(phieuXuatQG.Ma))
                                {
                                    if (phieuXuatQG.TinNhan == null || phieuXuatQG.TinNhan.Equals(""))
                                        phieuXuatQG.TinNhan = response.Content.ReadAsStringAsync().Result;
                                }
                            }
                            else
                            {
                                phieuXuatQG.Ma = ((int)response.StatusCode).ToString();
                                phieuXuatQG.TinNhan = response.Content.ReadAsStringAsync().Result;
                            }
                        }
                        else if (thaoTac == ThaoTac.CapNhat)
                        {
                            HttpResponseMessage response = httpClient.PutAsync("api/lien_thong/phieu_xuat", httpContent).Result;
                            if (response.IsSuccessStatusCode)
                            {
                                phieuXuatQG = JsonConvert.DeserializeObject<PhieuXuatQuocGia>(response.Content.ReadAsStringAsync().Result);
                                if (phieuXuatQG == null)
                                {
                                    phieuXuatQG = new PhieuXuatQuocGia();
                                    phieuXuatQG.Ma = string.Empty;
                                    phieuXuatQG.TinNhan = response.Content.ReadAsStringAsync().Result;
                                }
                                if (phieuXuatQG.Ma != null && !IsSuccessCode(phieuXuatQG.Ma))
                                {
                                    if (phieuXuatQG.TinNhan == null || phieuXuatQG.TinNhan.Equals(""))
                                        phieuXuatQG.TinNhan = response.Content.ReadAsStringAsync().Result;
                                }
                            }
                            else
                            {
                                phieuXuatQG.Ma = ((int)response.StatusCode).ToString();
                                phieuXuatQG.TinNhan = response.Content.ReadAsStringAsync().Result;
                            }
                        }
                        else if (thaoTac == ThaoTac.Xoa)
                        {
                            HttpResponseMessage response = httpClient.DeleteAsync("api/lien_thong/phieu_xuat/" + phieuXuat.MaCoSo + "/" + phieuXuat.MaPhieu).Result;
                            if (response.IsSuccessStatusCode)
                            {
                                phieuXuatQG = JsonConvert.DeserializeObject<PhieuXuatQuocGia>(response.Content.ReadAsStringAsync().Result);
                                if (phieuXuatQG == null)
                                {
                                    phieuXuatQG = new PhieuXuatQuocGia();
                                    phieuXuatQG.Ma = string.Empty;
                                    phieuXuatQG.TinNhan = response.Content.ReadAsStringAsync().Result;
                                }
                                if (phieuXuatQG.Ma != null && !IsSuccessCode(phieuXuatQG.Ma))
                                {
                                    if (phieuXuatQG.TinNhan == null || phieuXuatQG.TinNhan.Equals(""))
                                        phieuXuatQG.TinNhan = response.Content.ReadAsStringAsync().Result;
                                }
                            }
                            else
                            {
                                phieuXuatQG.Ma = ((int)response.StatusCode).ToString();
                                phieuXuatQG.TinNhan = response.Content.ReadAsStringAsync().Result;
                            }
                        }
                    }
                }
                else
                {
                    phieuXuatQG.Ma = dangNhap.Ma;
                    phieuXuatQG.TinNhan = dangNhap.TinNhan;
                    phieuXuatQG.MaPhieuXuatQuocGia = string.Empty;
                }
            }
            catch (Exception ex)
            {
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
