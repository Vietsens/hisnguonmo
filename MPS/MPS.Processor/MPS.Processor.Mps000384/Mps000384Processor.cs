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
using FlexCel.Report;
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using MPS.ProcessorBase.Core;
using Inventec.Core;
using MPS.ProcessorBase;
using System;
using System.Linq;
using MPS.Processor.Mps000384.PDO;
using System.Reflection;
using System.Data;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using FRD.ProcessorBase.EmrCommonData;

namespace MPS.Processor.Mps000384
{
    class Mps000384Processor : AbstractProcessor
    {
        Mps000384PDO rdo;
        List<PhauThuatThuThuat> ListPhauThuatThuThuat;
        public Mps000384Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000384PDO)rdoBase;
        }

        public void SetBarcodeKey()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Ham xu ly du lieu da qua xu ly
        /// Tao ra cac doi tuong du lieu xu dung trong thu vien xu ly file excel
        /// </summary>
        /// <returns></returns>
        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                SetBarcodeKey();
                SetSingleKey();
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                if (dicImage != null && dicImage.Count > 0)
                    barCodeTag.ProcessData(store, dicImage);
                if (ListPhauThuatThuThuat == null)
                    ListPhauThuatThuThuat = new List<PhauThuatThuThuat>();
                objectTag.AddObjectData<PhauThuatThuThuat>(store, "ListPhauThuatThuThuat", ListPhauThuatThuThuat);

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        void SetSingleKey()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Error("Start SetSingleKey");
                int tempInt = 0;
                long longTemp = 0;
                bool boolTemp = false;
                SetSingleKeyADO _SetSingleKeyADO = new SetSingleKeyADO();
                if (rdo._SarFormDatas != null && rdo._SarFormDatas.Count > 0)
                {
                    foreach (var item in rdo._SarFormDatas)
                    {
                        string name = item.KEY;
                        string value = item.VALUE;
                        try
                        {
                            if (item.KEY.Equals("ListPhauThuatThuThuat"))
                            {
                                if (!String.IsNullOrEmpty(item.VALUE))
                                    ListPhauThuatThuThuat = JsonConvert.DeserializeObject<List<PhauThuatThuThuat>>(item.VALUE);
                                else
                                    ListPhauThuatThuThuat = new List<PhauThuatThuThuat>();
                            }
                            else if (item.KEY.Equals("ChuyenVien"))
                            {
                                long.TryParse(item.VALUE, out longTemp);
                                _SetSingleKeyADO.ChuyenVien = (longTemp == IMSys.DbConfig.HIS_RS.HIS_TRAN_PATI_FORM.ID__DUOI_LEN_KHONG_LIEN_KE || longTemp == IMSys.DbConfig.HIS_RS.HIS_TRAN_PATI_FORM.ID__DUOI_LEN_LIEN_KE) ? 1 : longTemp == IMSys.DbConfig.HIS_RS.HIS_TRAN_PATI_FORM.ID__TREN_XUONG ? 2 : 3;
                            }
                            else if (item.KEY.Equals("DoiTuong"))
                            {
                                long.TryParse(item.VALUE, out longTemp);
                                _SetSingleKeyADO.DoiTuong = (longTemp == HisConfigCFG.PatientTypeId__BHYT) ? 1 : longTemp == HisConfigCFG.PatientTypeId__VP ? 2 : 3;
                                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => longTemp), longTemp) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _SetSingleKeyADO.DoiTuong), _SetSingleKeyADO.DoiTuong) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _SetSingleKeyADO.DoiTuong), _SetSingleKeyADO.DoiTuong) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisConfigCFG.PatientTypeId__BHYT), HisConfigCFG.PatientTypeId__BHYT) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisConfigCFG.PatientTypeId__BHYT), HisConfigCFG.PatientTypeId__BHYT));
                            }
                            else if (item.KEY.Equals("TrucTiepVao"))
                            {
                                _SetSingleKeyADO.TrucTiepVao = item.VALUE.Equals("Cấp cứu") ? 1 : (item.VALUE.Equals("Khoa khám bệnh") ? 2 : 3); // 0: khoa dieu tri
                            }
                            else if (item.KEY.Equals("NoiGioiThieu"))
                            {
                                _SetSingleKeyADO.NoiGioiThieu = item.VALUE.Equals("Cơ quan y tế") ? 1 : (item.VALUE.Equals("Tự đến") ? 2 : 3); // 0 : khoa điều trị
                            }
                            else
                            {
                                foreach (PropertyInfo propertyInfo in _SetSingleKeyADO.GetType().GetProperties())
                                {
                                    if (propertyInfo.Name.Equals(item.KEY))
                                    {
                                        if (propertyInfo.PropertyType.Equals(typeof(short)) || propertyInfo.PropertyType.Equals(typeof(short?)))
                                        {
                                            propertyInfo.SetValue(_SetSingleKeyADO, short.Parse(value));
                                        }
                                        else if (propertyInfo.PropertyType.Equals(typeof(decimal)) || propertyInfo.PropertyType.Equals(typeof(decimal?)))
                                        {
                                            propertyInfo.SetValue(_SetSingleKeyADO, decimal.Parse(value));
                                        }
                                        else if (propertyInfo.PropertyType.Equals(typeof(long)) || propertyInfo.PropertyType.Equals(typeof(long?)))
                                        {
                                            propertyInfo.SetValue(_SetSingleKeyADO, long.Parse(value));
                                        }
                                        else if (propertyInfo.PropertyType.Equals(typeof(int)) || propertyInfo.PropertyType.Equals(typeof(int?)))
                                        {
                                            propertyInfo.SetValue(_SetSingleKeyADO, int.Parse(value));
                                        }
                                        else if (propertyInfo.PropertyType.Equals(typeof(double)) || propertyInfo.PropertyType.Equals(typeof(double?)))
                                        {
                                            propertyInfo.SetValue(_SetSingleKeyADO, double.Parse(value));
                                        }
                                        else if (propertyInfo.PropertyType.Equals(typeof(float)) || propertyInfo.PropertyType.Equals(typeof(float?)))
                                        {
                                            propertyInfo.SetValue(_SetSingleKeyADO, float.Parse(value));
                                        }
                                        else if (propertyInfo.PropertyType.Equals(typeof(bool)) || propertyInfo.PropertyType.Equals(typeof(bool?)))
                                        {
                                            if (value.ToLower() == "true" || value.ToLower() == "1")
                                            {
                                                propertyInfo.SetValue(_SetSingleKeyADO, true);
                                            }
                                            else if (value.ToLower() == "false" || value.ToLower() == "0")
                                            {
                                                propertyInfo.SetValue(_SetSingleKeyADO, false);
                                            }
                                        }
                                        else if (propertyInfo.PropertyType.Equals(typeof(System.DateTime)) || propertyInfo.PropertyType.Equals(typeof(System.DateTime?)))
                                        {
                                            propertyInfo.SetValue(_SetSingleKeyADO, System.DateTime.Parse(value));
                                        }
                                        else
                                        {
                                            propertyInfo.SetValue(_SetSingleKeyADO, value);
                                        }
                                    }
                                }

                            }
                        }
                        catch (Exception ex2)
                        { Inventec.Common.Logging.LogSystem.Warn("name:" + name + "|value:" + value + "", ex2); }
                    }
                }

                AddObjectKeyIntoListkey<SetSingleKeyADO>(_SetSingleKeyADO);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _SetSingleKeyADO), _SetSingleKeyADO));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //phuongdt edit bỏ hết các key tách 1,2,3 đi, chỉ để key giá trị gốc, trong excel đã hỗ trợ hàm cắt chuỗi, hiển thị giá trị gốc hay giá trị cắt thì sau chỉ cần sửa template, không phải sửa code
        public class SetSingleKeyADO
        {
            public string TenBenhAn { get; set; }
            public string SoYTe { get; set; }
            public string BenhVien { get; set; }
            public string MaYTe { get; set; }
            public string MaBenhNhan { get; set; }
            public string TenBenhNhan { get; set; }
            public long NgaySinh { get; set; }
            public string GioiTinh { get; set; }
            public string NgheNghiep { get; set; }
            public string MaNgheNghiep { get; set; }
            public string DanToc { get; set; }
            public string MaDanToc { get; set; }
            public string NgoaiKieu { get; set; }
            public string MaNgoaiKieu { get; set; }
            public string SoNha { get; set; }
            public string ThonPho { get; set; }
            public string XaPhuong { get; set; }
            public string MaXaPhuong { get; set; }
            public string HuyenQuan { get; set; }
            public string MaHuyenQuan { get; set; }
            public string TinhThanhPho { get; set; }
            public string MaTinhThanhPho { get; set; }
            public string DiaChi { get; set; }
            public string NoiLamViec { get; set; }
            public long DoiTuong { get; set; }
            public long? NgayHetHanBHYT { get; set; }
            public string SoTheBHYT { get; set; }
            public long? NgayDangKyBHYT { get; set; }
            public string TenNoiDangKyBHYT { get; set; }
            public string MaNoiDangKyBHYT { get; set; }
            public long? NgayDuocHuong5Nam { get; set; }
            public string HoTenDiaChiNguoiNha { get; set; }
            public string SoDienThoaiNguoiNha { get; set; }
            public string CapBac { get; set; }
            public string DonVi { get; set; }

            public long? NgayVaoVien { get; set; } //
            public int TrucTiepVao { get; set; }//
            public int NoiGioiThieu { get; set; } //
            public int VaoVienDoBenhNayLanThu { get; set; }//
            public string TenKhoaVao { get; set; }//
            public long? NgayVaoKhoa { get; set; }//
            public int? SoNgayDieuTriTaiKhoa { get; set; }//
            public string ChuyenKhoa1 { get; set; }//
            public long? NgayChuyenKhoa1 { get; set; }//
            public int? SoNgayDieuTriKhoa1 { get; set; }//
            public string ChuyenKhoa2 { get; set; }//
            public long? NgayChuyenKhoa2 { get; set; }//
            public int? SoNgayDieuTriKhoa2 { get; set; }//
            public string ChuyenKhoa3 { get; set; }//
            public long? NgayChuyenKhoa3 { get; set; }//
            public int? SoNgayDieuTriKhoa3 { get; set; }//
            public long? ChuyenVien { get; set; }//
            public string TenVienChuyenBenhNhanDen { get; set; }//
            public long? NgayRaVien { get; set; }//
            public string TinhTrangRaVien { get; set; }
            public long? IdTinhTrangRaVien { get; set; }//
            public string TongSoNgayDieuTri1 { get; set; }//
            public string ChanDoan_NoiChuyenDen { get; set; }//
            public string MaICD_NoiChuyenDen { get; set; }//
            public string ChanDoan_KKB_CapCuu { get; set; }//
            public string MaICD_KKB_CapCuu { get; set; }//
            public string ChanDoan_KhiVaoKhoaDieuTri { get; set; }//
            public string MaICD_KhiVaoKhoaDieuTri { get; set; }//
            public string BenhChinh_RaVien { get; set; }//
            public string MaICD_BenhChinh_RaVien { get; set; }//
            public string BenhKemTheo_RaVien { get; set; }//
            public string MaICD_BenhKemTheo_RaVien { get; set; }//

            public bool TaiBien { get; set; }//
            public bool BienChung { get; set; }//
            public bool DoPhauThuat { get; set; }
            public bool DoGayMe { get; set; }
            public bool DoNhiemKhuan { get; set; }

            public int Khac { get; set; }
            public string KetQuaDieuTri { get; set; }//
            public long? IdKetQuaDieuTri { get; set; }//
            public string GiaiPhauBenh { get; set; }//
            public long? NgayTuVong { get; set; }//
            public string LyDoTuVong { get; set; }//
            public string MaLyDoTuVong { get; set; }//
            public string ThoiGianTuVong { get; set; }//
            public string MaThoiGianTuVong { get; set; }//
            public string NguyenNhanChinhTuVong { get; set; }//
            public string MaICD_NguyenNhanChinhTuVong { get; set; }
            public bool KhamNghiemTuThi { get; set; }//
            public string ChanDoanGiaiPhauTuThi { get; set; }//
            public string MaICD_ChanDoanGiaiPhauTuThi { get; set; }
            public string TongSoNgayDieuTriSauPT { get; set; }//
            public string TongSoLanPhauThuat { get; set; }//
            public string NguyenNhan_BenhChinh_RaVien { get; set; }//
            public string MaICD_NguyenNhan_BenhChinh_RV { get; set; }
            public string ChanDoanTruocPhauThuat { get; set; }//
            public string ChanDoanSauPhauThuat { get; set; }//
            public string MaICD_ChanDoanSauPhauThuat { get; set; }
            public string MaICD_ChanDoanTruocPhauThuat { get; set; }
            public string GiamDocBenhVien { get; set; }
            public string TruongKhoa { get; set; }
            public string KhoaRaVien { get; set; }
            public string SoLuuTru { get; set; }
            public string SoNgoaiTru { get; set; }
            public string Buong { get; set; }
            public string Giuong { get; set; }
            public string MaKhoa { get; set; }
            public string Khoa { get; set; }

            public string LyDoVaoVien { get; set; }
            public int VaoNgayThu { get; set; }
            public string QuaTrinhBenhLy { get; set; }
            public string TienSuBenhBanThan { get; set; }
            public string TienSuBenhGiaDinh { get; set; }
            public string ToanThan { get; set; }
            public string TuanHoan { get; set; }
            public string HoHap { get; set; }
            public string TieuHoa { get; set; }
            public string ThanTietNieuSinhDuc { get; set; }
            public string ThanKinh { get; set; }
            public string CoXuongKhop { get; set; }
            public string TaiMuiHong { get; set; }
            public string RangHamMat { get; set; }
            public string Mat { get; set; }
            public string Khac_CacCoQuan { get; set; }
            public string CacXetNghiemCanLamSangCanLam { get; set; }
            public string TomTatBenhAn { get; set; }
            public string BenhChinh { get; set; }
            public string BenhKemTheo { get; set; }
            public string PhanBiet { get; set; }
            public string TienLuong { get; set; }
            public string HuongDieuTri { get; set; }
            public long NgayKhamBenh { get; set; }
            public string BacSyLamBenhAn { get; set; }
            public string QuaTrinhBenhLyVaDienBien { get; set; }
            public string TomTatKetQuaXetNghiem { get; set; }
            public string PhuongPhapDieuTri { get; set; }
            public string TinhTrangNguoiBenhRaVien { get; set; }
            public string HuongDieuTriVaCacCheDoTiepTheo { get; set; }
            public string NguoiNhanHoSo { get; set; }
            public string NguoiGiaoHoSo { get; set; }
            public long? NgayTongKet { get; set; }
            public string BacSyDieuTri { get; set; }
            public string BacSyKhamBenh { get; set; }
            public string BenhNgoaiKhoa { set; get; }
            public string Mach { get; set; }
            public string NhietDo { get; set; }
            public string HuyetAp { get; set; }
            public string NhipTho { get; set; }
            public string CanNang { get; set; }
            public string ChieuCao { get; set; }
            public string BMI { get; set; }
            public string DiUng { get; set; }
            public string DiUng_Text { get; set; }
            public string MaTuy { get; set; }
            public string MaTuy_Text { get; set; }
            public string RuouBia { get; set; }
            public string RuouBia_Text { get; set; }
            public string ThuocLa { get; set; }
            public string ThuocLa_Text { get; set; }
            public string ThuocLao { get; set; }
            public string ThuocLao_Text { get; set; }
            public string Khac_DacDiemLienQuanBenh { get; set; }
            public string Khac_DacDiemLienQuanBenh_Text { get; set; }

            public string LeDao_MatPhai { set; get; }
            public string LeDao_MatTrai { set; get; }
            public string MiMat_MatPhai { set; get; }
            public string MiMat_MatTrai { set; get; }
            public string KetMac_MatPhai { set; get; }
            public string KetMac_MatTrai { set; get; }
            public string THMH_MatPhai { set; get; }
            public string THMH_MatTrai { set; get; }
            public string GiacMac_MatPhai { set; get; }
            public string GiacMac_MatTrai { set; get; }
            public string CungMac_MatPhai { set; get; }
            public string CungMac_MatTrai { set; get; }
            public string TienPhong_MatPhai { set; get; }
            public string TienPhong_MatTrai { set; get; }
            public string MongMat_MatPhai { set; get; }
            public string MongMat_MatTrai { set; get; }
            public string DongTuPX_MatPhai { set; get; }
            public string DongTuPX_MatTrai { set; get; }
            public string TTT_MatPhai { set; get; }
            public string TTT_MatTrai { set; get; }
            public string TTD_MatPhai { set; get; }
            public string TTD_MatTrai { set; get; }
            public string SoiBongDT_MatPhai { set; get; }
            public string SoiBongDT_MatTrai { set; get; }
            public string TinhHinhNC_MatPhai { set; get; }
            public string TinhHinhNC_MatTrai { set; get; }
            public string HocMat_MatPhai { set; get; }
            public string HocMat_MatTrai { set; get; }
            public string DayMat_MatPhai { set; get; }
            public string DayMat_MatTrai { set; get; }

            public string ThiLucKhongKinhMP { set; get; }
            public string ThiLucKhongKinhMT { set; get; }
            public string NhanApRaVienMP { set; get; }
            public string NhanApRaVienMT { set; get; }
            public string RaVienCoKinhMP { set; get; }
            public string RaVienCoKinhMT { set; get; }
            public string ThiTruongRaVienMP { set; get; }
            public string ThiTruongRaVienMT { set; get; }

            public string MaChanDoanPhu_KhiVaoKhoaDieuTri { get; set; }//
            public string ChanDoanPhu_KhiVaoKhoaDieuTri { get; set; }//

            public string QuaTrinhBenhLyVaDienBienLSVaXN { get; set; }

            public bool PhauThuat { set; get; }
            public bool ThuThuat { set; get; }

            public string XQuang { get; set; }
            public string CTScanner { get; set; }
            public string SieuAm { get; set; }
            public string XetNghiem { get; set; }
            public string Khac_Text { get; set; }
            public string ToanBoHoSo { get; set; }
        }

        public class PhoneADO
        {
            public int Action { get; set; }
            public string PHONE_NUMBER { get; set; }
            public string RELATIVE_NAME { get; set; }
        }

        public class ServiceReqADO
        {
            public long? START_TIME { get; set; }
            public long? FINISH_TIME { get; set; }
            public string REQUEST_DEPARTMENT_NAME { get; set; }
            public string EXECUTE_LOGINNAME { get; set; }
            public string RESULT { get; set; }
        }


    }
}
