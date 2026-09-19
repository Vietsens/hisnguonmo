using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  HIS.Desktop.Plugins.HisImportKsk.Resources
{
    class ResourceMessage
    {
        /// <summary>
        /// Ten resource KHONG co khoang trang o dau - phai trung ten resource duoc nhung vao assembly.
        /// Message.Lang.resx (neutral) duoc nhung thang vao DLL chinh nen van doc duoc khi
        /// deploy DLL le, khong phu thuoc satellite assembly vi\.
        /// </summary>
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.HisImportKsk.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

        private static string Get(string key)
        {
            try
            {
                return Inventec.Common.Resource.Get.Value(key, languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return "";
        }

        /// <summary>Sinh mã bệnh nhân mới</summary>
        internal static string KetQuaBenhNhanMoi
        {
            get { return Get("KetQuaBenhNhanMoi"); }
        }

        /// <summary>Gắn vào mã bệnh nhân đã có</summary>
        internal static string KetQuaGanVaoBenhNhanDaCo
        {
            get { return Get("KetQuaGanVaoBenhNhanDaCo"); }
        }

        /// <summary>Trùng dòng trong file - không nhận</summary>
        internal static string KetQuaTrungDongTrongFile
        {
            get { return Get("KetQuaTrungDongTrongFile"); }
        }

        /// <summary>Thiếu số CCCD nên sẽ sinh mã bệnh nhân mới</summary>
        internal static string ThieuSoCccd
        {
            get { return Get("ThieuSoCccd"); }
        }

        /// <summary>Kết quả import</summary>
        internal static string TieuDeKetQuaImport
        {
            get { return Get("TieuDeKetQuaImport"); }
        }

        /// <summary>Bảng tổng hợp sau khi import - 6 tham số</summary>
        internal static string TongHopKetQuaImport
        {
            get { return Get("TongHopKetQuaImport"); }
        }

        /// <summary>Danh sách mã bệnh nhân đã có được gắn thêm đợt khám - 1 tham số</summary>
        internal static string DanhSachMaBenhNhanDaGan
        {
            get { return Get("DanhSachMaBenhNhanDaGan"); }
        }

        /// <summary>Có {0} dòng có cảnh báo, đề nghị rà soát</summary>
        internal static string CoDongCanhBaoCanRaSoat
        {
            get { return Get("CoDongCanhBaoCanRaSoat"); }
        }
    }
}
