using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: Inventec.Desktop.Core.Plugin]

// Neutral language = Vietnamese -> resources/Lang.resx + Message.Lang.resx được nhúng thẳng vào DLL chính
// như neutral fallback. Khi CurrentUICulture không khớp satellite (vi/en), ResourceManager sẽ tìm thấy
// nội dung trong DLL chính -> KHÔNG bị popup rỗng.
[assembly: NeutralResourcesLanguage("vi")]

[assembly: AssemblyTitle("HIS.Desktop.Plugins.HisPatientPackage")]
[assembly: AssemblyDescription("Quản lý gói dịch vụ bệnh nhân (đăng ký/sửa - màn 6.1, danh sách - màn 6.2)")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("HIS.Desktop.Plugins.HisPatientPackage")]
[assembly: AssemblyCopyright("Copyright ©  2026")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: ComVisible(false)]
[assembly: Guid("7f3a9c2e-1b4d-4e6a-9c8f-2d5e7a1b3c4d")]

[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
