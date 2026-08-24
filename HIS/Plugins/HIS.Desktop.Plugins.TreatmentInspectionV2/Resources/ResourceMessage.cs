using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.TreatmentInspectionV2.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.TreatmentInspectionV2.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

        internal static string Thongbao
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_TreatmentInspection__Thongbao", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string TranhCaoTai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_TreatmentInspection__TranhCaoTai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ChuaLuutuBenhAn
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_TreatmentInspection__ChuaLuutuBenhAn", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string DaDuyetGiamDinh
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_TreatmentInspection__DaDuyetGiamDinh", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string MauTrang
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_TreatmentInspection__MauTrang", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string Mauvang
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_TreatmentInspection__MauVang", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string MauXanh
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_TreatmentInspection__MauXanh", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string MauDo
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_TreatmentInspection__MauDo", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        #region Import of the inspection list

        /// <summary>Reads one message of the import feature.</summary>
        private static string GetImportMessage(string key)
        {
            try
            {
                return Inventec.Common.Resource.Get.Value(
                    "Plugins_TreatmentInspectionV2__" + key,
                    languageMessage,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return "";
        }

        /// <summary>Khong tim thay file mau {0} tren may tram...</summary>
        internal static string KhongTimThayFileMau { get { return GetImportMessage("KhongTimThayFileMau"); } }

        /// <summary>Ban co muon mo file ngay khong?</summary>
        internal static string BanCoMuonMoFileNgay { get { return GetImportMessage("BanCoMuonMoFileNgay"); } }

        /// <summary>Khong doc duoc file. Vui long kiem tra lai dinh dang file.</summary>
        internal static string KhongDocDuocFile { get { return GetImportMessage("KhongDocDuocFile"); } }

        /// <summary>File khong co du lieu de nhap khau.</summary>
        internal static string FileKhongCoDuLieu { get { return GetImportMessage("FileKhongCoDuLieu"); } }

        /// <summary>Khong quet duoc cot tag o dong 2 — huong dan xu ly.</summary>
        internal static string KhongQuetDuocCotTag { get { return GetImportMessage("KhongQuetDuocCotTag"); } }

        /// <summary>Doc duoc {0} dong nhung cot Ma dieu tri trong.</summary>
        internal static string DocDuocNhungThieuMaDieuTri { get { return GetImportMessage("DocDuocNhungThieuMaDieuTri"); } }

        /// <summary>File vuot qua {0} dong. Vui long chia nho file.</summary>
        internal static string VuotGioiHanSoDong { get { return GetImportMessage("VuotGioiHanSoDong"); } }

        /// <summary>Chua nhap ma dieu tri.</summary>
        internal static string ChuaNhapMaDieuTri { get { return GetImportMessage("ChuaNhapMaDieuTri"); } }

        /// <summary>Ma dieu tri khong dung dinh dang.</summary>
        internal static string MaDieuTriKhongDungDinhDang { get { return GetImportMessage("MaDieuTriKhongDungDinhDang"); } }

        /// <summary>Khong tim thay ho so dieu tri voi ma {0}.</summary>
        internal static string KhongTimThayHoSoDieuTri { get { return GetImportMessage("KhongTimThayHoSoDieuTri"); } }

        /// <summary>Trung voi dong so {0} trong file.</summary>
        internal static string TrungVoiDongTrongFile { get { return GetImportMessage("TrungVoiDongTrongFile"); } }

        /// <summary>Hop le</summary>
        internal static string TrangThaiHopLe { get { return GetImportMessage("TrangThaiHopLe"); } }

        /// <summary>Loi</summary>
        internal static string TrangThaiLoi { get { return GetImportMessage("TrangThaiLoi"); } }

        /// <summary>Da luu</summary>
        internal static string TrangThaiDaLuu { get { return GetImportMessage("TrangThaiDaLuu"); } }

        /// <summary>Canh bao</summary>
        internal static string TrangThaiCanhBao { get { return GetImportMessage("TrangThaiCanhBao"); } }

        /// <summary>Ho so nay da duoc tiep nhan giam dinh ngay {0}...</summary>
        internal static string HoSoDaTiepNhanTruocDo { get { return GetImportMessage("HoSoDaTiepNhanTruocDo"); } }

        /// <summary>Tong so dong: {0} | Hop le: {1} | Loi: {2} | File: {3}</summary>
        internal static string ThongKeNhapKhau { get { return GetImportMessage("ThongKeNhapKhau"); } }

        #endregion
    }
}
