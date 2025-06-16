using System;

class ResourceMessage
{
    static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.HisPatientBankAccount.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());
    internal static string DuLieuPhongDangChonThuocNhieuChiNhanh
    {
        get
        {
            try
            {
                return Inventec.Common.Resource.Get.Value("Plugin_ChooseRoom__DuLieuPhongDangChonThuocNhieuChiNhanh", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return "";
        }
    }
    internal static string InvoiceBook_GiaTriLonHonKhong
    {
        get
        {
            try
            {
                return Inventec.Common.Resource.Get.Value("InvoiceBook_GiaTriLonHonKhong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return "";
        }
    }
}