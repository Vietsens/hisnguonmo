using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisImportBid.Resources
{
    class ResourceLanguageManager
    {
        internal static ResourceManager LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisImportBid.Resources.Lang", typeof(HIS.Desktop.Plugins.HisImportBid.FormImportBid).Assembly);

        internal static string HeThongTBKQXLYCCuaFrontendThatBai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HeThongTBKQXLYCCuaFrontendThatBai", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThongBao
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThongBao", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string CanhBaoThuoc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("CanhBaoThuoc", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string CanhBaoVatTu
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("CanhBaoVatTu", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string CanhBaoMau
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("CanhBaoMau", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhongCoNhaCungCap
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongCoNhaCungCap", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string SoLuongKhongDuocAm
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("SoLuongKhongDuocAm", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhongCoSttThau
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongCoSttThau", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string SttThauQuaDai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("SttThauQuaDai", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string BiTrung
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BiTrung", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string NhomThauQuaDai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("NhomThauQuaDai", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string GoiThauQuaDai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("GoiThauQuaDai", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string SaiDinhDangNam
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("SaiDinhDangNam", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThieuTruongDuLieuBatBuoc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThieuTruongDuLieuBatBuoc", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhongCoThongTinNam
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongCoThongTinNam", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhongCoThongTinGoiThau
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongCoThongTinGoiThau", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhongCoThongTinTenThau
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongCoThongTinTenThau", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhongCoThongTinQDThau
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongCoThongTinQDThau", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string KhongCoThongTinMaQDThau
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongCoThongTinMaQDThau", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string GoiThauKhongDuDoDai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("GoiThauKhongDuDoDai", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string NamKhongDuocLonHonNamHienTai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("NamKhongDuocLonHonNamHienTai", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhongCoThongTinLoaiThau
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongCoThongTinLoaiThau", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhongCoSoLuong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongCoSoLuong", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhongCoGiaNhap
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongCoGiaNhap", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string GiaKhongDuocAm
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("GiaKhongDuocAm", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhongCoVAT
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongCoVAT", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string VATKhongDuocAm
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("VATKhongDuocAm", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhongDuocNhapMaVatTuTuongDuong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongDuocNhapMaVatTuTuongDuong", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhongDuocNhapMaVatTuKhiNhapMaVatTuTuongDuong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongDuocNhapMaVatTuKhiNhapMaVatTuTuongDuong", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string MaVatTuTuongDuongQuaDai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("MaVatTuTuongDuongQuaDai", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string MaDuThauQuaDai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("MaDuThauQuaDai", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhongDuocNhapMaDuThau
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongDuocNhapMaDuThau", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string MaTrungThauQuaDai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("MaTrungThauQuaDai", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhongDuocNhapMaTrungThau
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongDuocNhapMaTrungThau", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string TenTrungThauQuaDai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("TenTrungThauQuaDai", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhongDuocNhapTenTrungThau
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongDuocNhapTenTrungThau", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string QuyCachDongGoiQuaDai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("QuyCachDongGoiQuaDai", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhongDuocNhapQuyCachDongGoi
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongDuocNhapQuyCachDongGoi", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string TenBHYTQuaDai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("TenBHYTQuaDai", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhongDuocNhapTenBHYT
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongDuocNhapTenBHYT", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string NongDoHamLuongQuaDai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("NongDoHamLuongQuaDai", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string SoDangKyQuaDai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("SoDangKyQuaDai", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhongDuocNhapSoDangKy
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongDuocNhapSoDangKy", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string HangsanxuatQuaDai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HangsanxuatQuaDai", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string NuocsanxuatQuaDai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("NuocsanxuatQuaDai", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string MaQDThauQuaDai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("MaQDThauQuaDai", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThangTuoiThoQuaDai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThangTuoiThoQuaDai", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string NgayTuoiThoQuaDai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("NgayTuoiThoQuaDai", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string GioTuoiThoQuaDai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("GioTuoiThoQuaDai", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string TuoiThoKhongHopLe
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("TuoiThoKhongHopLe", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThangTuoiThoSaiDinhDang
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThangTuoiThoSaiDinhDang", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string NgayTuoiThoSaiDinhDang
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("NgayTuoiThoSaiDinhDang", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string GioTuoiThoSaiDinhDang
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("GioTuoiThoSaiDinhDang", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
    }
}
