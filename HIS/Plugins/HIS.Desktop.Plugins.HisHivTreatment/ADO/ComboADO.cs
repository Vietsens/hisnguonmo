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

namespace HIS.Desktop.Plugins.HisHivTreatment.ADO
{
    public class ComboADO
    {
        public long Value { get; set; }

        public string Name { get; set; }
        public ComboADO()
        {

        }
        public ComboADO(long value, string name)
        {
            this.Value = value;
            this.Name = name;
        }
        public List<ComboADO> listHivPatientType()
        {
            List<ComboADO> list = new List<ComboADO>();
            try
            {
                list.Add(new ComboADO(1, Resources.ResourceMessage.NguoiNhiemHIV));
                list.Add(new ComboADO(2, Resources.ResourceMessage.TrePhoiNhiemVoiHiv));
                list.Add(new ComboADO(3, Resources.ResourceMessage.DieuTriDuPhongTruocPhoiNhiem));
                list.Add(new ComboADO(4, Resources.ResourceMessage.DieuTriDuPhongSauPhoiNhiem));
                list.Add(new ComboADO(5, Resources.ResourceMessage.Khac));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return list;
        }
        public List<ComboADO> listHivPatientStatus()
        {
            List<ComboADO> list = new List<ComboADO>();
            try
            {
                list.Add(new ComboADO(1, Resources.ResourceMessage.TreDuoi18ThangSinhRaTuMeNhiemHiv));
                list.Add(new ComboADO(2, Resources.ResourceMessage.PhoiNhiem));
                list.Add(new ComboADO(3, Resources.ResourceMessage.DangDieuTriLao));
                list.Add(new ComboADO(4, Resources.ResourceMessage.CoBau));
                list.Add(new ComboADO(5, Resources.ResourceMessage.ChuyenDa));
                list.Add(new ComboADO(6, Resources.ResourceMessage.SauSinh));
                list.Add(new ComboADO(7, Resources.ResourceMessage.ViemGan));
                list.Add(new ComboADO(8, Resources.ResourceMessage.NghienChichMaTuy));
                list.Add(new ComboADO(9, Resources.ResourceMessage.Khac));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return list;
        }
        public List<ComboADO> listHivTreatmentReason()
        {
            List<ComboADO> list = new List<ComboADO>();
            try
            {
                list.Add(new ComboADO(1, Resources.ResourceMessage.BenhNhanHivMoiDangKyLanDau));
                list.Add(new ComboADO(2, Resources.ResourceMessage.BenhNhanHivChuaDieuTriArvChuyenToi));
                list.Add(new ComboADO(3, Resources.ResourceMessage.BenhNhanHivDaDieuTriArvChuyenToi));
                list.Add(new ComboADO(4, Resources.ResourceMessage.BenhNhanHivDaDieuTriArvNayDieuTriLai));
                list.Add(new ComboADO(5, Resources.ResourceMessage.BenhNhanHivChuaDieuTriArvDangKyLai));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return list;
        }
        public List<ComboADO> listRegimenLevel()
        {
            List<ComboADO> list = new List<ComboADO>();
            try
            {
                list.Add(new ComboADO(1, Resources.ResourceMessage.PhacDoBac + " 1"));
                list.Add(new ComboADO(2, Resources.ResourceMessage.PhacDoBac + " 2"));
                list.Add(new ComboADO(3, Resources.ResourceMessage.PhacDoBac + " 3"));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return list;
        }
        public List<ComboADO> listTuberculosisRegimen()
        {
            List<ComboADO> list = new List<ComboADO>();
            try
            {
                list.Add(new ComboADO(1, Resources.ResourceMessage.PhacDo + " 2RHZE/4RHE"));
                list.Add(new ComboADO(2, Resources.ResourceMessage.PhacDo + " 2RHZE/4RH"));
                list.Add(new ComboADO(3, Resources.ResourceMessage.PhacDo + " 2RHZE/10RHE"));
                list.Add(new ComboADO(4, Resources.ResourceMessage.PhacDo + " 2RHZE/10RH"));
                list.Add(new ComboADO(5, Resources.ResourceMessage.PhacDoKhacHIV));
                list.Add(new ComboADO(6, Resources.ResourceMessage.PhacDo + " INH"));
                list.Add(new ComboADO(7, Resources.ResourceMessage.PhacDo + " 3HP"));
                list.Add(new ComboADO(8, Resources.ResourceMessage.PhacDo + " 1HP"));
                list.Add(new ComboADO(9, Resources.ResourceMessage.PhacDo + " 3HR"));
                list.Add(new ComboADO(10, Resources.ResourceMessage.PhacDo + " 4R"));
                list.Add(new ComboADO(11, Resources.ResourceMessage.PhacDo + " 6L"));
                list.Add(new ComboADO(12, Resources.ResourceMessage.PhacDoKhacLao));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return list;
        }
        public List<ComboADO> listTuberculosisTreatmentType()
        {
            List<ComboADO> list = new List<ComboADO>();
            try
            {
                list.Add(new ComboADO(0, Resources.ResourceMessage.KhongDieuTriLao));
                list.Add(new ComboADO(1, Resources.ResourceMessage.DieuTriLaoTiemAn));
                list.Add(new ComboADO(2, Resources.ResourceMessage.DieuTriLao));
                list.Add(new ComboADO(3, Resources.ResourceMessage.DieuTriLaoKhangThuoc));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return list;
        }
        public List<ComboADO> listTestReason()
        {
            List<ComboADO> list = new List<ComboADO>();
            try
            {
                list.Add(new ComboADO(1, Resources.ResourceMessage.ThuongQuy));
                list.Add(new ComboADO(2, Resources.ResourceMessage.NghiNgoThatBaiDieuTri));
                list.Add(new ComboADO(3, Resources.ResourceMessage.Khac));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return list;
        }
        public List<ComboADO> listTestTime()
        {
            List<ComboADO> list = new List<ComboADO>();
            try
            {
                list.Add(new ComboADO(1, Resources.ResourceMessage.Lan + " 1"));
                list.Add(new ComboADO(2, Resources.ResourceMessage.Lan + " 2"));
                list.Add(new ComboADO(3, Resources.ResourceMessage.Lan3));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return list;
        }
        public List<ComboADO> listTestPcrResult()
        {
            List<ComboADO> list = new List<ComboADO>();
            try
            {
                list.Add(new ComboADO(0, Resources.ResourceMessage.AmTinh));
                list.Add(new ComboADO(1, Resources.ResourceMessage.DuongTinh));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return list;
        }
        public List<ComboADO> listHivTreatment()
        {
            List<ComboADO> list = new List<ComboADO>();
            try
            {
                list.Add(new ComboADO(1, Resources.ResourceMessage.DieuTriArv));
                list.Add(new ComboADO(2, Resources.ResourceMessage.DieuTriLao));
                list.Add(new ComboADO(3, Resources.ResourceMessage.DuPhongLao));
                list.Add(new ComboADO(4, "Cotrimoxazol"));
                list.Add(new ComboADO(5, "PLTMC"));
                list.Add(new ComboADO(6, Resources.ResourceMessage.DieuTriViemGanB));
                list.Add(new ComboADO(7, Resources.ResourceMessage.DieuTriViemGanC));
                list.Add(new ComboADO(8, Resources.ResourceMessage.Khac));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return list;
        }
        public List<ComboADO> listTestPcrRnaResult()
        {
            List<ComboADO> list = new List<ComboADO>();
            try
            {
                list.Add(new ComboADO(1, Resources.ResourceMessage.KhongPhatHien));
                list.Add(new ComboADO(2, Resources.ResourceMessage.Duoi50BanSao));
                list.Add(new ComboADO(3, Resources.ResourceMessage.Tu50DenDuoi200BanSao));
                list.Add(new ComboADO(4, Resources.ResourceMessage.Tu200DenDuoi1000BanSao));
                list.Add(new ComboADO(5, Resources.ResourceMessage.Tren1000BanSao));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return list;
        }

        public List<ComboADO> listRegimenHivReason()
        {
            List<ComboADO> list = new List<ComboADO>();
            try
            {
                list.Add(new ComboADO(1, Resources.ResourceMessage.ThieuThuoc));
                list.Add(new ComboADO(2, Resources.ResourceMessage.TacDungPhu));
                list.Add(new ComboADO(3, Resources.ResourceMessage.ThatBaiDieuTri));
                list.Add(new ComboADO(4, Resources.ResourceMessage.ToiUuHoaPhacDo));
                list.Add(new ComboADO(5, Resources.ResourceMessage.PhacDoMoiTheoHuongDanQuocGia));
                list.Add(new ComboADO(6, Resources.ResourceMessage.Khac));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return list;
        }

        public List<ComboADO> listGroupPatient()
        {
            List<ComboADO> list = new List<ComboADO>();
            try
            {
                list.Add(new ComboADO(0, Resources.ResourceMessage.NguoiSuDungMaTuy));
                list.Add(new ComboADO(1, Resources.ResourceMessage.NguoiBanDam));
                list.Add(new ComboADO(2, Resources.ResourceMessage.NguoiCoQuanHeTinhDucDongGioi));
                list.Add(new ComboADO(3, Resources.ResourceMessage.NguoiChuyenDoiGioiTinh));
                list.Add(new ComboADO(4, Resources.ResourceMessage.CungSongChungVoiNguoiNhiemHIV));
                list.Add(new ComboADO(5, Resources.ResourceMessage.NguoiCoQuanHeTinhDucVoiNguoiNhiemHIV));
                list.Add(new ComboADO(6, Resources.ResourceMessage.NguoiMacCacBenhLayTruyenQuaDuongTinhDuc));
                list.Add(new ComboADO(7, Resources.ResourceMessage.NguoiDiBienDong));
                list.Add(new ComboADO(8, Resources.ResourceMessage.NguoiMacBenhLao));
                list.Add(new ComboADO(9, Resources.ResourceMessage.NguoiCoTrieuChungLamSangNghiNgoNhiemHIV));
                list.Add(new ComboADO(10, Resources.ResourceMessage.PhamNhanNguoiBiTamGiam));
                list.Add(new ComboADO(11, Resources.ResourceMessage.CacDoiTuongKhac));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return list;
        }

        public List<ComboADO> listClinicalStage()
        {
            List<ComboADO> list = new List<ComboADO>();
            try
            {
                list.Add(new ComboADO(1, Resources.ResourceMessage.GiaiDoan1));
                list.Add(new ComboADO(2, Resources.ResourceMessage.GiaiDoan2));
                list.Add(new ComboADO(3, Resources.ResourceMessage.GiaiDoan3));
                list.Add(new ComboADO(4, Resources.ResourceMessage.GiaiDoan4));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return list;
        }

        public List<ComboADO> listTuberculosisTreatmentResult()
        {
            List<ComboADO> list = new List<ComboADO>();
            try
            {
                list.Add(new ComboADO(1, Resources.ResourceMessage.DangDieuTri));
                list.Add(new ComboADO(2, Resources.ResourceMessage.HoanThanh));
                list.Add(new ComboADO(3, Resources.ResourceMessage.ThatBai));
                list.Add(new ComboADO(4, Resources.ResourceMessage.TuVong));
                list.Add(new ComboADO(5, Resources.ResourceMessage.BoDieuTri));
                list.Add(new ComboADO(6, Resources.ResourceMessage.NgungDieuTri));
                list.Add(new ComboADO(7, Resources.ResourceMessage.KhongDanhGia));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return list;
        }

        public List<ComboADO> listTuberculosisScreening()
        {
            List<ComboADO> list = new List<ComboADO>();
            try
            {
                list.Add(new ComboADO(1, Resources.ResourceMessage.KhongSangLoc));
                list.Add(new ComboADO(2, Resources.ResourceMessage.SangLocTrieuChung));
                list.Add(new ComboADO(3, Resources.ResourceMessage.ChupXQuangPhoi));
                list.Add(new ComboADO(4, Resources.ResourceMessage.XetNghiemProteinPhanUngC));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return list;
        }
    }
}
