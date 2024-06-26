using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.BedRoomWithIn.Resources
{
    public class ResourceLanguageManager
    {
        public static ResourceManager LanguageResource { get; set; }

        public static string TaiKhoanKhongCoQuyenThucHienChucNang
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("TaiKhoanKhongCoQuyenThucHienChucNang", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
                return "";
            }
        }

        public static string KhongTimThayIcdTuongUng
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_ExamServiceReqExecute__KhongTimThayIcdTuongUng", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
                return "";
            }
        }

        public static string ThoiGianNhapKhoaKhongDuocNhoHonTgVaoVien
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_ExamServiceReqExecute__ThoiGianNhapKhoaKhongDuocNhoHonTgVaoVien", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
                return "";
            }
        }

        public static string ThongBao
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThongBao", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
                return "";
            }
        }

        public static string GiuongVuotQuaSoLuong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("GiuongVuotQuaSoLuong", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
                return "";
            }
        }

        public static string GiuongDaCoBenhNhanNam
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("GiuongDaCoBenhNhanNam", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
                return "";
            }
        }

        public static string GiuongDaVuotQuaSucChua
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("GiuongDaVuotQuaSucChua", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
                return "";
            }
        }

        public static string DichVuCoDTPTBatBuocNhungKhongCoChinhSachGia
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DichVuCoDTPTBatBuocNhungKhongCoChinhSachGia", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
                return "";
            }
        }

        public static string DichVuCoDTTTBatBuocNhungKhongCoChinhSachGia
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DichVuCoDTTTBatBuocNhungKhongCoChinhSachGia", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
                return "";
            }
        }

        public static string BatBuocChonDichVuGiuongKhiChonGiuong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BatBuocChonDichVuGiuongKhiChonGiuong", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
                return "";
            }
        }

        public static string BatBuocChonDoiTuongThanhToanKhiChonGiuong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BatBuocChonDoiTuongThanhToanKhiChonGiuong", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
                return "";
            }
        }

        public static string BatBuocChonDoiTuongThanhToanKhiChonDTPT
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BatBuocChonDoiTuongThanhToanKhiChonDTPT", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
                return "";
            }
        }

        public static string GiuongDaCoBenhNhanCoNamGhepKhong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("GiuongDaCoBenhNhanCoNamGhepKhong", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
                return "";
            }
        }

        public static string BanChuChonBuongTiepTuc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BanChuChonBuongTiepTuc", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
                return "";
            }
        }

        public static string BanPhaiChonBuongKhiTiepNhan
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BanPhaiChonBuongKhiTiepNhan", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
                return "";
            }
        }

        public static string ThoiGianTiepNhanPhaiLonHonThoiGianKhoaTruoc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThoiGianTiepNhanPhaiLonHonThoiGianKhoaTruoc", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
                return "";
            }
        }

        public static string BenhNhanCoDienDieuTriKhac
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BenhNhanCoDienDieuTriKhac", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
                return "";
            }
        }
    }
}
